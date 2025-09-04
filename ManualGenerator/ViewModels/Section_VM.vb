Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class Section_VM
	Inherits ObservableObject

	Public Property Entity As Section_E

	Public Property Heading As String
		Get
			Return Entity.Heading
		End Get
		Set(value As String)
			SetProperty(Entity.Heading, value)
		End Set
	End Property

	Public Property DescriptionXaml As String
		Get
			Return Entity.DescriptionXaml
		End Get
		Set(value As String)
			SetProperty(Entity.DescriptionXaml, value)
		End Set
	End Property

	Public Property ImagePath As String
		Get
			Return Entity.ImagePath
		End Get
		Set(value As String)
			SetProperty(Entity.ImagePath, value)
			OnPropertyChanged(NameOf(HasImage))
		End Set
	End Property

	Public ReadOnly Property HasImage As Boolean
		Get
			Return Not String.IsNullOrEmpty(ImagePath)
		End Get
	End Property

	Public Property IsHeadline As Boolean
		Get
			Return Entity.IsHeadline
		End Get
		Set(value As Boolean)
			SetProperty(Entity.IsHeadline, value)
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
