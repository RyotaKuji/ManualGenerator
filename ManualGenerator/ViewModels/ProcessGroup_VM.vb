Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ProcessGroup_VM
	Inherits ObservableObject

	Public Property BeginInputCommand As RelayCommand

	Public Property Heading As String
	Public Property Description As String
	Public Property ImagePath As String = "/Resources/Images/picture.png"

	' ロストフォーカス直後はプロパティの値が変わらないため、引数で受け取って手動で更新
	Public Sub ChangedHeading(text As String)
		Heading = text
		BeginInputCommand?.Execute(Me)
	End Sub

End Class
