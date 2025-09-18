Imports SQLite

<Table("Sections")>
Public Class Section_E
	<PrimaryKey>
	Public Property Id As String = Guid.NewGuid().ToString()

	<Indexed>
	Public Property DocumentId As String

	Public Property Heading As String
	Public Property DescriptionXaml As String
	Public Property DescriptionHtml As String
	Public Property DescriptionText As String
	Public Property ImagePath As String
	Public Property IsHeadline As Boolean
	Public Property OrderIndex As Integer
End Class