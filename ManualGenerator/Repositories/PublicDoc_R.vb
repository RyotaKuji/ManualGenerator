Imports System.Text
Imports SQLite

Public Class PublicDoc_R : Implements IDisposable

	Private ReadOnly db As SQLiteConnection

	Public Sub New()
		db = New SQLiteConnection(Configuration.DbPath)
		db.CreateTable(Of PublicDoc_E)()
	End Sub

	' READ by Id
	Public Function Read(id As String) As PublicDoc_E
		Return db.Find(Of PublicDoc_E)(id)
	End Function

	' READ by Title
	Public Function ReadAllByTitle(keyword As String) As IEnumerable(Of PublicDoc_E)
		Dim docs As List(Of PublicDoc_E)

		If String.IsNullOrWhiteSpace(keyword) Then
			docs = db.Table(Of PublicDoc_E).ToList()
		Else
			docs = db.Table(Of PublicDoc_E).
				Where(Function(d) d.Title.Contains(keyword)).
				ToList()
		End If

		Return docs
	End Function

	' READ by Text
	Public Function ReadAllByText(keyword As String) As IEnumerable(Of PublicDoc_E)
		Dim docs As List(Of PublicDoc_E)
		If String.IsNullOrWhiteSpace(keyword) Then
			docs = db.Table(Of PublicDoc_E).ToList()
		Else
			docs = db.Table(Of PublicDoc_E).
				Where(Function(d) d.Text.Contains(keyword)).
				ToList()
		End If
		Return docs
	End Function

	' CREATE or UPDATE
	Public Sub CreateOrUpdate(doc As Document_E)
		Dim allText As New StringBuilder($"{doc.Title} ")
		For Each sec As Section_E In doc.Sections
			Dim heading As String = sec.Heading
			Dim text As String = FlowDocumentToHtmlConverter.Convert(sec.DescriptionXaml)
			allText.Append($"{heading} {text} ")
		Next

		Dim entity As New PublicDoc_E()
		With entity
			.Id = doc.Id
			.Title = doc.Title
			.Text = allText.ToString()
		End With

		db.RunInTransaction(
		Sub()
			' --- Document 保存 ---
			Dim existingDoc = db.Find(Of PublicDoc_E)(doc.Id)

			If existingDoc Is Nothing Then
				db.Insert(entity)
			Else
				db.Update(entity)
			End If
		End Sub)
	End Sub

	' DELETE
	Public Sub Delete(id As String)
		db.Delete(Of PublicDoc_E)(id)
	End Sub

	Public Sub Dispose() Implements IDisposable.Dispose
		db.Dispose()
	End Sub

End Class
