Imports SQLite

Public Class WebDoc_R
	Private ReadOnly db As SQLiteAsyncConnection

	Private Sub New(dbPath As String)
		db = New SQLiteAsyncConnection(dbPath)
	End Sub

	' 非同期ファクトリ（テーブル作成もここでAwait）
	Public Shared Async Function CreateAsync() As Task(Of WebDoc_R)
		Dim inst = New WebDoc_R(My.Resources.DBPath)
		Try
			Await inst.db.CreateTableAsync(Of WebDoc_E)()
		Catch ex As Exception
			Dim exStr As String = ex.Message
		End Try

		Return inst
	End Function

	' READ by Title
	Public Async Function ReadAllByTitleAsync(keyword As String) As Task(Of List(Of WebDoc_E))
		Dim docs As List(Of WebDoc_E)

		If String.IsNullOrWhiteSpace(keyword) Then
			docs = Await db.Table(Of WebDoc_E)().
						ToListAsync()
		Else
			docs = Await db.Table(Of WebDoc_E)().
						Where(Function(d) d.Title.Contains(keyword)).
						ToListAsync()
		End If

		Return docs
	End Function

	' CREATE or UPDATE (セクションは差分更新)
	Public Async Function CreateOrUpdateAsync(doc As Document_E) As Task

		' 1トランザクションで実行（同期APIはコールバック内のSQLiteConnectionで使用可）
		Await db.RunInTransactionAsync(
			Sub(conn As SQLiteConnection)

				Dim webDoc = New WebDoc_E(doc)

				' --- Document 保存 ---
				Dim existingDoc = conn.Find(Of WebDoc_E)(webDoc.Id)
				If existingDoc Is Nothing Then
					conn.Insert(webDoc)
				Else
					conn.Update(webDoc)
				End If
			End Sub
		)
	End Function

End Class
