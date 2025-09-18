Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class Section_VM : Inherits ObservableObject

	Private __entity As Section_E
	Public Property Entity As Section_E
		Get
			Return __entity
		End Get
		Set(value As Section_E)
			__entity = value
			Heading = value.Heading
			DescriptionXaml = value.DescriptionXaml
			ImagePath = value.ImagePath
			IsHeadline = value.IsHeadline
		End Set
	End Property

	Private _heading As String
	Public Property Heading As String
		Get
			Return _heading
		End Get
		Set(value As String)
			If SetProperty(_heading, value) Then
				Entity.Heading = value
			End If
		End Set
	End Property

	Private _descriptionXaml As String
	Public Property DescriptionXaml As String
		Get
			Return _descriptionXaml
		End Get
		Set(value As String)
			If SetProperty(_descriptionXaml, value) Then
				Entity.DescriptionXaml = value
			End If
		End Set
	End Property

	Private _descriptionText As String
	Public Property DescriptionText As String
		Get
			Return _descriptionText
		End Get
		Set(value As String)
			If SetProperty(_descriptionText, value) Then
				Entity.DescriptionText = value
			End If
		End Set
	End Property

	Private _imagePath As String
	Public Property ImagePath As String
		Get
			Return _imagePath
		End Get
		Set(value As String)
			If SetProperty(_imagePath, value) Then
				Entity.ImagePath = value
				OnPropertyChanged(NameOf(HasImage))
			End If
		End Set
	End Property

	Public ReadOnly Property HasImage As Boolean
		Get
			Return Not String.IsNullOrEmpty(ImagePath)
		End Get
	End Property

	Private _isHeadline As Boolean
	Public Property IsHeadline As Boolean
		Get
			Return _isHeadline
		End Get
		Set(value As Boolean)
			If SetProperty(_isHeadline, value) Then
				Entity.IsHeadline = value
			End If
		End Set
	End Property

	Public Property RemoveCommand As RelayCommand

	Public ReadOnly Property SelectImageCommand As RelayCommand

	Public Sub New()
		Entity = New Section_E()
		SelectImageCommand = New RelayCommand(AddressOf SelectImage)
	End Sub

	Public Sub New(entity As Section_E)
		Me.Entity = entity
		SelectImageCommand = New RelayCommand(AddressOf SelectImage)
	End Sub

	Private _isLastItem As Boolean
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
