Public Class EditorClosingManager

	Public Property HasChange As Boolean = False

	Private Shared ReadOnly instance As New EditorClosingManager()

	Public Shared Function GetInstance() As EditorClosingManager
		Return instance
	End Function
End Class
