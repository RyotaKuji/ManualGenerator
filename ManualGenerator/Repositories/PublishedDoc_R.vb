Imports SQLite

Public Class PublishedDoc_R : Implements IDisposable

	Private ReadOnly db As SQLiteConnection

	'Public Sub New()
	'	db = New SQLiteConnection(Configuration.DbPath)
	'	db.CreateTable(Of PublishedDoc_E)()
	'End Sub

	'' READ by Id
	'Public Function Read(id As String) As PublishedDoc_E
	'	Return db.Find(Of PublishedDoc_E)(id)
	'End Function

	'' READ by Title
	'Public Function ReadAllByTitle(keyword As String) As IEnumerable(Of PublishedDoc_E)
	'	Dim docs As List(Of PublishedDoc_E)

	'	If String.IsNullOrWhiteSpace(keyword) Then
	'		docs = db.Table(Of PublishedDoc_E).ToList()
	'	Else
	'		docs = db.Table(Of PublishedDoc_E).
	'			Where(Function(d) d.Title.Contains(keyword)).
	'			ToList()
	'	End If

	'	Return docs
	'End Function

	'' READ by Text
	'Public Function ReadAllByText(keyword As String) As IEnumerable(Of PublishedDoc_E)
	'	Dim docs As List(Of PublishedDoc_E)
	'	If String.IsNullOrWhiteSpace(keyword) Then
	'		docs = db.Table(Of PublishedDoc_E).ToList()
	'	Else
	'		docs = db.Table(Of PublishedDoc_E).
	'			Where(Function(d) d.Text.Contains(keyword)).
	'			ToList()
	'	End If
	'	Return docs
	'End Function

	'' CREATE or UPDATE
	'Public Sub CreateOrUpdate(doc As PublishedDoc_E)
	'	db.RunInTransaction(
	'	Sub()
	'		' --- Document 保存 ---
	'		Dim existingDoc = db.Find(Of PublishedDoc_E)(doc.Id)

	'		If existingDoc Is Nothing Then
	'			db.Insert(doc)
	'		Else
	'			db.Update(doc)
	'		End If
	'	End Sub)
	'End Sub

	'' DELETE
	'Public Sub Delete(id As String)
	'	db.Delete(Of PublishedDoc_E)(id)
	'End Sub

	Public Sub Dispose() Implements IDisposable.Dispose
		db.Dispose()
	End Sub

End Class
