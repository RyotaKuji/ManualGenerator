Imports SQLite

Public Class DocumentRepository
	Private ReadOnly _db As SQLiteConnection

	Public Sub New()
		_db = New SQLiteConnection(Configuration.DbPath)
		_db.CreateTable(Of Document_E)()
		_db.CreateTable(Of Section_E)()
	End Sub

	' READ by Id
	Public Function Read(id As String) As Document_E
		Dim doc = _db.Find(Of Document_E)(id)

		If doc IsNot Nothing Then
			doc.Sections = _db.Table(Of Section_E)().
				Where(Function(sect) sect.DocumentId = id).
				OrderBy(Function(section) section.OrderIndex)
		End If

		Return doc
	End Function

	' CREATE or UPDATE (セクションは差分更新)
	Public Sub CreateOrUpdate(doc As Document_E)
		_db.RunInTransaction(
		Sub()
			' --- Document 保存 ---
			Dim existingDoc = _db.Find(Of Document_E)(doc.Id)

			If existingDoc Is Nothing Then

				_db.Insert(doc)
				For i As Integer = 0 To doc.Sections.Count() - 1
					Dim sec = doc.Sections(i)
					sec.DocumentId = doc.Id
					sec.OrderIndex = i
					_db.Insert(sec)
				Next

			Else

				_db.Update(doc)

				' --- Sections 差分更新 ---
				If doc.Sections IsNot Nothing Then
					Dim existingSections = _db.Table(Of Section_E)().
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
							_db.Update(sec)
							existingDict.Remove(sec.Id)
						Else
							' 新規追加
							_db.Insert(sec)
						End If

						' 残っているものは削除対象
						For Each toDelete In existingDict.Values
							_db.Delete(toDelete)
						Next
					Next
				End If
			End If
		End Sub)
	End Sub

	' DELETE
	Public Sub Delete(id As String)
		Dim sections = _db.Table(Of Section_E)().
			Where(Function(sec) sec.DocumentId = id).
			ToList()

		For Each sec In sections
			_db.Delete(sec)
		Next

		_db.Delete(Of Document_E)(id)
	End Sub

	' READ by Title (部分一致 / 全件)
	Public Function ReadAllByTitle(keyword As String) As IEnumerable(Of Document_E)
		Dim docs As List(Of Document_E)

		If String.IsNullOrWhiteSpace(keyword) Then
			docs = _db.Table(Of Document_E).ToList()
		Else
			docs = _db.Table(Of Document_E).
				Where(Function(d) d.Title.Contains(keyword)).
				ToList()
		End If

		' 各 Document に対応する Section を読み込む
		For Each doc In docs
			doc.Sections = _db.Table(Of Section_E)().
				Where(Function(s) s.DocumentId = doc.Id).
				OrderBy(Function(s) s.OrderIndex).
				ToList()
		Next

		Return docs
	End Function
End Class
