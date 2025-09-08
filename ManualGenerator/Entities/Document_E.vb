Imports SQLite

<Table("Documents")>
Public Class Document_E
	<PrimaryKey>
	Public Property Id As String = Guid.NewGuid().ToString()

	<Indexed>
	Public Property Title As String

	<Ignore>
	Public Property Sections As IEnumerable(Of Section_E) = {}
End Class