Imports ManualGenerator.Definitions
Imports Microsoft.VisualBasic.Devices
Imports SQLite

Public Class Document_R
	Private ReadOnly db As SQLiteAsyncConnection

	Private Sub New(dbPath As String)
		db = New SQLiteAsyncConnection(dbPath)
	End Sub

	' 非同期ファクトリ（テーブル作成もここでAwait）
	Public Shared Async Function CreateAsync() As Task(Of Document_R)
		Try
			Dim inst = New Document_R(My.Resources.DBPath)

			Await inst.db.CreateTableAsync(Of Document_E)()
			Await inst.db.CreateTableAsync(Of Section_E)()

			Return inst

		Catch ex As Exception
			Throw New DbException(ex)
		End Try
	End Function

	' READ by Id
	Public Async Function ReadAsync(id As String) As Task(Of Document_E)
		Try
			Dim doc = Await db.FindAsync(Of Document_E)(id)

			If doc IsNot Nothing Then
				doc.Sections = Await db.Table(Of Section_E)().
					Where(Function(section) section.DocumentId = id).
					OrderBy(Function(section) section.OrderIndex).
					ToListAsync()
			End If

			Return doc
		Catch ex As Exception
			Throw New DbException(ex)
		End Try
	End Function

	' READ by Query
	Public Async Function ReadAllAsync(queries As Dictionary(Of PubStatus, DocumentQuery)) As Task(Of Dictionary(Of PubStatus, IEnumerable(Of Document_E)))
		Try
			Dim result As New Dictionary(Of PubStatus, IEnumerable(Of Document_E))()

			For Each pubStatus_query As KeyValuePair(Of PubStatus, DocumentQuery) In queries
				Dim pubStatus As PubStatus = pubStatus_query.Key
				Dim query As DocumentQuery = pubStatus_query.Value

				Dim pubStatusValue As Integer = pubStatus

				Dim docs As New List(Of Document_E)

				If String.IsNullOrWhiteSpace(query.Keyword) = False Then

					Dim keyword As String = query.Keyword

					' ドキュメント情報で検索
					Dim docsByDocInfo As List(Of Document_E) =
						Await db.Table(Of Document_E).
								Where(Function(doc) doc.PubStatusValue = pubStatusValue).
								Where(Function(doc) doc.Id = keyword OrElse
													doc.BaseId = keyword OrElse
													doc.Title.Contains(keyword) OrElse
													doc.AuthorName.Contains(keyword)).ToListAsync()

					' セクション本文で検索
					Dim sections As List(Of Section_E) = Await db.Table(Of Section_E).
						Where(Function(section) section.Heading.Contains(keyword) OrElse
												section.DescriptionText.Contains(keyword)).ToListAsync()
					Dim idsBySection As HashSet(Of String) = sections.Select(Function(section) section.DocumentId).ToHashSet()
					Dim docsBySection As List(Of Document_E) = Await ReadAllByIds(pubStatus, idsBySection)

					docs.AddRange(docsByDocInfo)
					docs.AddRange(docsBySection)

					' ID について一意にする
					docs = docs.GroupBy(Function(x) x.Id).Select(Function(g) g.First()).ToList()
				Else
					' ドキュメント情報で検索

					Dim exeQuery As AsyncTableQuery(Of Document_E) = db.Table(Of Document_E)

					exeQuery = exeQuery.Where(Function(doc) doc.PubStatusValue = pubStatusValue)

					If String.IsNullOrWhiteSpace(query.Title) = False Then
						exeQuery = exeQuery.Where(Function(doc) doc.Title.Contains(query.Title))
					End If
					If String.IsNullOrWhiteSpace(query.AuthorId) = False Then
						exeQuery = exeQuery.Where(Function(doc) doc.AuthorId = query.AuthorId)
					End If
					If String.IsNullOrWhiteSpace(query.AuthorName) = False Then
						exeQuery = exeQuery.Where(Function(doc) doc.AuthorName.Contains(query.AuthorName))
					End If
					If String.IsNullOrWhiteSpace(query.DocumentId) = False Then
						exeQuery = exeQuery.Where(Function(doc) doc.BaseId = query.DocumentId OrElse doc.Id = query.DocumentId)
					End If

					docs = Await exeQuery.ToListAsync()

				End If

				result(pubStatus) = docs
			Next

			Return result
		Catch ex As Exception
			Throw New DbException(ex)
		End Try
	End Function

	' Read docs by Ids
	Private Async Function ReadAllByIds(pubStatus As PubStatus, ids As HashSet(Of String)) As Task(Of List(Of Document_E))
		Try
			If ids Is Nothing OrElse ids.Count = 0 Then
				Return New List(Of Document_E)
			End If
			Dim placeholder_ids = String.Join(",", Enumerable.Range(0, ids.Count).Select(Function(i) "?"))
			Dim query = $"SELECT * FROM Documents WHERE PubStatusValue = {CInt(pubStatus)} AND Id IN ({placeholder_ids})"
			Dim args = ids.ToArray()

			Dim docs As List(Of Document_E) = Await db.QueryAsync(Of Document_E)(query, args)

			Return docs

		Catch ex As Exception
			Throw New DbException(ex)
		End Try
	End Function

	' CREATE or UPDATE (セクションは差分更新)
	Public Async Function CreateOrUpdateAsync(doc As Document_E, pubStatus As PubStatus) As Task

		For i As Integer = 0 To If(doc.Sections?.Count(), 0) - 1
			Dim sec = doc.Sections(i)
			sec.DocumentId = doc.Id
			sec.OrderIndex = i
		Next

		doc = doc.Clone(pubStatus)

		' 1トランザクションで実行（同期APIはコールバック内のSQLiteConnectionで使用可）
		Await db.RunInTransactionAsync(
			Sub(conn As SQLiteConnection)
				Try
					' --- Document 保存 ---
					Dim existingDoc = conn.Find(Of Document_E)(doc.Id)
					If existingDoc Is Nothing Then

						conn.Insert(doc)
						For Each sec In doc.Sections
							conn.Insert(sec)
						Next
					Else

						conn.Update(doc)

						' Entities 差分更新
						If doc.Sections IsNot Nothing Then
							Dim existingSections = conn.Table(Of Section_E)().
								Where(Function(s) s.DocumentId = doc.Id).
								ToList()

							Dim existingDict = existingSections.ToDictionary(Function(sec) sec.Id, StringComparer.OrdinalIgnoreCase)

							For Each sec In doc.Sections
								If existingDict.ContainsKey(sec.Id) Then
									' 更新
									conn.Update(sec)
									existingDict.Remove(sec.Id)
								Else
									' 新規追加
									conn.Insert(sec)
								End If
							Next

							' 残っているものは削除対象
							For Each toDelete In existingDict.Values
								conn.Delete(toDelete)
							Next
						End If
					End If

					If pubStatus = PubStatus.Published Then

						Dim deletedId = Document_E.GetIdWithPubStatus(doc.Id, PubStatus.Draft)

						Dim deleteSections = conn.Table(Of Section_E)().
							Where(Function(s) s.DocumentId = deletedId).
							ToList()
						For Each sec In deleteSections
							conn.Delete(sec)
						Next

						conn.Delete(Of Document_E)(deletedId)

					End If
				Catch ex As Exception
					Throw New DbException(ex)
				End Try
			End Sub
		)
	End Function

	' DELETE
	Public Async Function DeleteAsync(id As String) As Task
		Try
			Dim sections = Await db.Table(Of Section_E)().
			Where(Function(sec) sec.DocumentId = id).
			ToListAsync()

			For Each sec In sections
				Await db.DeleteAsync(sec)
			Next

			Await db.DeleteAsync(Of Document_E)(id)

		Catch ex As Exception
			Throw New DbException(ex)
		End Try
	End Function
End Class
