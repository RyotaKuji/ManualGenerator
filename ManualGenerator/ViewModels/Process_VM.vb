Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Newtonsoft.Json

Public Class Process_VM
	Inherits ObservableObject

	Public Property Heading As String
	Public Property DescriptionXaml As String
	Public Property DescriptionHtml As String
	Public Property ImagePath As String

	<JsonIgnore>
	Public Property RemoveCommand As RelayCommand

	Private _isLastItem As Boolean
	<JsonIgnore>
	Public Property IsLastItem As Boolean
		Get
			Return _isLastItem
		End Get
		Set(value As Boolean)
			SetProperty(_isLastItem, value)
		End Set
	End Property

End Class
