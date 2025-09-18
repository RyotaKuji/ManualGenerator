Imports System.Text
Imports SQLite

<Table("WebDocs")>
Public Class WebDoc_E

	<PrimaryKey, NotNull>
	Public Property Id As String
	Public Property Title As String
	Public Property Text As String

	Public Sub New(document As Document_E)
		Id = document.Id
		Title = document.Title
		Dim sb As New StringBuilder()
		sb.Append(document.Title + " ")
		For Each section As Section_E In document.Sections
			sb.Append(section.Heading + " ")
			sb.Append(section.DescriptionText + " ")
		Next
		Text = sb.ToString()
	End Sub
End Class
