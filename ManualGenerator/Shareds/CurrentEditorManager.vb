Public Class CurrentEditorManager

	Public Property Sections As IEnumerable(Of Section_E)

	Public Event ScrollToEvent(uri As Uri)

	Private Shared instance As New CurrentEditorManager()

	Public Shared Function GetInstance() As CurrentEditorManager
		Return instance
	End Function

	Public Sub ScrollTo(uri As Uri)
		RaiseEvent ScrollToEvent(uri)
	End Sub
End Class
