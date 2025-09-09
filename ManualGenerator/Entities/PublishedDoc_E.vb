Imports SQLite

<Table("PublishedDocs")>
Public Class PublishedDoc_E
	<PrimaryKey>
	Public Property Id As String

	<Indexed>
	Public Property Title As String

	<Indexed>
	Public Property Text As String

	Public Sub New(doc As Document_E)
		Id = doc.Id
		Title = doc.Title
	End Sub
End Class
