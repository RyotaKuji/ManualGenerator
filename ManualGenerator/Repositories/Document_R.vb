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
			Throw New AppException(ex)
		End Try
	End Function

	' READ by Id
	Public Async Function ReadAsync(id As String) As Task(Of Document_E)
		Try
			Dim doc = Await db.FindAsync(Of Document_E)(id)

			If doc IsNot Nothing Then
				doc.Sections = Await db.Table(Of Section_E)().
					Where(Function(sect) sect.DocumentId = id).
					OrderBy(Function(section) section.OrderIndex).
					ToListAsync()
			End If

			Return doc
		Catch ex As Exception
			Throw New AppException(ex)
		End Try
	End Function

	' READ by Title
	Public Async Function ReadAllAsync(pubStatus As Definitions.PubStatus) As Task(Of List(Of Document_E))
		Try
			Dim statusValue As Integer = pubStatus

			Dim docs As List(Of Document_E) =
			Await db.Table(Of Document_E)().
					Where(Function(d) d.PubStatusValue = statusValue).
					ToListAsync()

			' 各 Document に対応する Section を読み込む（逐次）
			For Each doc In docs
				doc.Sections = Await db.Table(Of Section_E)().
					Where(Function(s) s.DocumentId = doc.Id).
					OrderBy(Function(s) s.OrderIndex).
					ToListAsync()
			Next

			Return docs

		Catch ex As Exception
			Throw New AppException(ex)
		End Try
	End Function

	' CREATE or UPDATE (セクションは差分更新)
	Public Async Function CreateOrUpdateAsync(doc As Document_E, pubStatus As Definitions.PubStatus) As Task

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

					If pubStatus = Definitions.PubStatus.Published Then

						Dim deletedId = Document_E.GetIdWithPubStatus(doc.Id, Definitions.PubStatus.Draft)

						Dim deleteSections = conn.Table(Of Section_E)().
							Where(Function(s) s.DocumentId = deletedId).
							ToList()
						For Each sec In deleteSections
							conn.Delete(sec)
						Next

						conn.Delete(Of Document_E)(deletedId)

					End If
				Catch ex As Exception
					Throw New AppException(ex)
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
			Throw New AppException(ex)
		End Try
	End Function
End Class
