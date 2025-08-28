Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Newtonsoft.Json

Public Class Section_VM
	Inherits ObservableObject

	Public Property Heading As String

	Public Property DescriptionXaml As String

	Public Property DescriptionHtml As String

	Private _imagePath As String
	Public Property ImagePath As String
		Get
			Return _imagePath
		End Get
		Set(value As String)
			SetProperty(_imagePath, value)
			OnPropertyChanged(NameOf(HasImage))
		End Set
	End Property

	<JsonIgnore>
	Public ReadOnly Property HasImage As Boolean
		Get
			Return Not String.IsNullOrEmpty(ImagePath)
		End Get
	End Property

	Private _isHeading As Boolean
	Public Property IsHeading As Boolean
		Get
			Return _isHeading
		End Get
		Set(value As Boolean)
			SetProperty(_isHeading, value)
		End Set
	End Property

	<JsonIgnore>
	Public Property RemoveCommand As RelayCommand

	<JsonIgnore>
	Public ReadOnly Property SelectImageCommand As RelayCommand

	Public Sub New()
		SelectImageCommand = New RelayCommand(AddressOf SelectImage)
	End Sub

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

	Private Sub SelectImage()
		Dim dialog = New Microsoft.Win32.OpenFileDialog With {
			.Filter = "画像ファイル|*.png;*.jpeg;*.jpg"
		}

		Dim selected = dialog.ShowDialog()

		If selected Then

			Dim filename = dialog.FileName
			ImagePath = filename

		End If
	End Sub

End Class
