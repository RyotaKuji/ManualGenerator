Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Newtonsoft.Json

Public Class Process_VM
	Inherits ObservableObject

	<JsonIgnore>
	Public Property ConfirmCommand As RelayCommand
	<JsonIgnore>
	Public Property RemoveCommand As RelayCommand

	Public Property Heading As String
	Public Property DescriptionXaml As String
	Public Property DescriptionHtml As String
	Public Property ImagePath As String

End Class
