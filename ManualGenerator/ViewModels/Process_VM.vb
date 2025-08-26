Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Newtonsoft.Json

Public Class Process_VM
	Inherits ObservableObject

	Public Property Heading As String
	Public Property DescriptionXaml As String
	Public Property DescriptionHtml As String
	Public Property ImagePath As String
	Private _isHeadingVar As Boolean
	Public Property IsHeading As Boolean
		Get
			Return _isHeadingVar
		End Get
		Set(value As Boolean)
			SetProperty(_isHeadingVar, value)
		End Set
	End Property

	<JsonIgnore>
	Public Const HeadingFlag = "Heading"
	<JsonIgnore>
	Public Const ProcessFlag = "Process"

	<JsonIgnore>
	Public Property RemoveCommand As RelayCommand
	<JsonIgnore>
	Public Property SelectionChangedCommand As RelayCommand(Of String)

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
		SelectionChanged(ProcessFlag)
	End Sub

	Private Sub SelectionChanged(contentType As String)
		IsHeading = contentType = HeadingFlag
	End Sub

End Class
