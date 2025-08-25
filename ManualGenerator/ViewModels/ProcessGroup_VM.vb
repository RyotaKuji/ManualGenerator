Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Newtonsoft.Json

Public Class ProcessGroup_VM
	Inherits ObservableObject

	<JsonIgnore>
	Public Property RemoveCommand As RelayCommand

	Public Property Heading As String
	Public Property DescriptionXaml As String
	Public Property DescriptionHtml As String
	Public Property ImagePath As String = "/Resources/Images/picture.png"

End Class
