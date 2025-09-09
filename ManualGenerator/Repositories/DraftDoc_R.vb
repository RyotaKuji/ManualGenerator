Imports SQLite

Public Class DraftDoc_R : Implements IDisposable
	Private ReadOnly db As SQLiteConnection

	Public Sub New()
		db = New SQLiteConnection(Configuration.DbPath)
		db.CreateTable(Of Document_E)()
		db.CreateTable(Of Section_E)()
	End Sub

	' READ by Id
	Public Function Read(id As String) As Document_E
		Dim doc = db.Find(Of Document_E)(id)

		If doc IsNot Nothing Then
			doc.Sections = db.Table(Of Section_E)().
				Where(Function(sect) sect.DocumentId = id).
				OrderBy(Function(section) section.OrderIndex)
		End If

		Return doc
	End Function

	' READ by Title
	Public Function ReadAllByTitle(keyword As String) As IEnumerable(Of Document_E)
		Dim docs As List(Of Document_E)

		If String.IsNullOrWhiteSpace(keyword) Then
			docs = db.Table(Of Document_E).ToList()
		Else
			docs = db.Table(Of Document_E).
				Where(Function(d) d.Title.Contains(keyword)).
				ToList()
		End If

		' 各 Document に対応する Section を読み込む
		For Each doc In docs
			doc.Sections = db.Table(Of Section_E)().
				Where(Function(s) s.DocumentId = doc.Id).
				OrderBy(Function(s) s.OrderIndex).
				ToList()
		Next

		Return docs
	End Function

	' CREATE or UPDATE (セクションは差分更新)
	Public Sub CreateOrUpdate(doc As Document_E)
		db.RunInTransaction(
		Sub()
			' --- Document 保存 ---
			Dim existingDoc = db.Find(Of Document_E)(doc.Id)

			If existingDoc Is Nothing Then

				db.Insert(doc)
				For i As Integer = 0 To doc.Sections.Count() - 1
					Dim sec = doc.Sections(i)
					sec.DocumentId = doc.Id
					sec.OrderIndex = i
					db.Insert(sec)
				Next

			Else

				db.Update(doc)

				' --- Sections 差分更新 ---
				If doc.Sections IsNot Nothing Then
					Dim existingSections = db.Table(Of Section_E)().
						Where(Function(s) s.DocumentId = doc.Id).
						ToList()

					' 既存 Section を Id で引当て
					Dim existingDict = existingSections.ToDictionary(Function(sec) sec.Id, StringComparer.OrdinalIgnoreCase)

					For i As Integer = 0 To doc.Sections.Count() - 1
						Dim sec = doc.Sections(i)
						sec.DocumentId = doc.Id
						sec.OrderIndex = i

						If existingDict.ContainsKey(sec.Id) Then
							' 更新
							db.Update(sec)
							existingDict.Remove(sec.Id)
						Else
							' 新規追加
							db.Insert(sec)
						End If

						' 残っているものは削除対象
						For Each toDelete In existingDict.Values
							db.Delete(toDelete)
						Next
					Next
				End If
			End If
		End Sub)
	End Sub

	' DELETE
	Public Sub Delete(id As String)
		Dim sections = db.Table(Of Section_E)().
			Where(Function(sec) sec.DocumentId = id).
			ToList()

		For Each sec In sections
			db.Delete(sec)
		Next

		db.Delete(Of Document_E)(id)
	End Sub

	Public Sub Dispose() Implements IDisposable.Dispose
		db.Dispose()
	End Sub
End Class
