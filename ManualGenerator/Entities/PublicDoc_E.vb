Imports SQLite

<Table("PublicDocs")>
Public Class PublicDoc_E
	<PrimaryKey>
	Public Property Id As String

	<Indexed>
	Public Property Title As String

	<Indexed>
	Public Property Text As String
End Class
