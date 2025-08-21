Imports CommunityToolkit.Mvvm.ComponentModel

Public Class ProcessGroup_VM
	Inherits ObservableObject

	Public Property Subtitle As String
	Public Property Description As String
	Public Property ImagePath As String = "/Resources/Images/picture.png"

	Public Sub New()
	End Sub

End Class
