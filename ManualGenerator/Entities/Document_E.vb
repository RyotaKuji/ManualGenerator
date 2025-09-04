Public Class Document_E
	Public Id As String = Guid.NewGuid().ToString()
	Public Title As String
	Public Sections As IEnumerable(Of Section_E)
End Class
