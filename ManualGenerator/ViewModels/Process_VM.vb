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
	<JsonIgnore>
	Public Property SelectionChangedCommand As RelayCommand(Of String)

	Private _contentTypeParameter As String
	<JsonIgnore>
	Public Property ContentType As String
		Get
			Return _contentTypeParameter
		End Get
		Set(value As String)
			SetProperty(_contentTypeParameter, value)
		End Set
	End Property

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

	Public Sub New()
		SelectionChangedCommand = New RelayCommand(Of String)(AddressOf SelectionChanged)
		SelectionChanged("Process")
	End Sub

	Private Sub SelectionChanged(contentType As String)
		Me.ContentType = If(contentType = "Heading", "見出し", "手順")
	End Sub

End Class
