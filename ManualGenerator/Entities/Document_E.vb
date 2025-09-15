Imports SQLite

<Table("Documents")>
Public Class Document_E
	<PrimaryKey, NotNull>
	Public Property Id As String = Guid.NewGuid().ToString()

	<NotNull>
	Public Property IsPublished As Boolean = False

	<NotNull>
	Public Property Title As String

	<Ignore>
	Public Property Sections As IEnumerable(Of Section_E) = {}
End Class