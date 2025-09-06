Public Class Document_E
	Public Property Id As String = Guid.NewGuid().ToString()
	Public Property Title As String
	Public Property Sections As IEnumerable(Of Section_E)
End Class
