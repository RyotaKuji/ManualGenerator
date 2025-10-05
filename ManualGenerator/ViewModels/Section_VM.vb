Imports System.IO
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class Section_VM
	Inherits ObservableObject

	Private __entity As Section_E
	Public Property Entity As Section_E
		Get
			Return __entity
		End Get
		Set(value As Section_E)
			__entity = value
			Heading = value.Heading
			DescriptionXml = value.DescriptionXml
			ImagePath = value.ImagePath
			IsHeadline = value.IsHeadline

			XmlManager.GetInstance().IdToXml(Entity.Id) = DescriptionXml
		End Set
	End Property

	Public ReadOnly Property Id As String
		Get
			Return Entity.Id
		End Get
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

	Private _descriptionXml As String
	Public Property DescriptionXml As String
		Get
			Return _descriptionXml
		End Get
		Set(value As String)
			If SetProperty(_descriptionXml, value) Then
				Entity.DescriptionXml = value
			End If
		End Set
	End Property

	Private _descriptionHtml As String
	Public Property DescriptionHtml As String
		Get
			Return _descriptionHtml
		End Get
		Set(value As String)
			If SetProperty(_descriptionHtml, value) Then
				Entity.DescriptionHtml = value
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
				OnPropertyChanged(NameOf(IsVisibleEmptyIcon))
			End If
		End Set
	End Property

	Public ReadOnly Property HasImage As Boolean
		Get
			Return File.Exists(ImagePath)
		End Get
	End Property

	Private _inPrintScreenMode As Boolean
	Public Property InPrintScreenMode As Boolean
		Get
			Return _inPrintScreenMode
		End Get
		Set(value As Boolean)
			If SetProperty(_inPrintScreenMode, value) Then
				OnPropertyChanged(NameOf(IsVisibleEmptyIcon))
			End If
		End Set
	End Property

	Public ReadOnly Property IsVisibleEmptyIcon As Boolean
		Get
			Return (Not HasImage) AndAlso (Not InPrintScreenMode)
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
	Public Event ScrollToSelfEvent()

	Public ReadOnly Property SwitchPrintingScreenModeCommand As RelayCommand
	Public ReadOnly Property RequestScrollCommand As RelayCommand(Of Uri)

	Public Sub New()
		Entity = New Section_E()
		SwitchPrintingScreenModeCommand = New RelayCommand(AddressOf SwitchPrintingScreenMode)
		RequestScrollCommand = New RelayCommand(Of Uri)(AddressOf RequestScroll)
	End Sub

	Public Sub New(entity As Section_E)
		Me.Entity = entity
		SwitchPrintingScreenModeCommand = New RelayCommand(AddressOf SwitchPrintingScreenMode)
		RequestScrollCommand = New RelayCommand(Of Uri)(AddressOf RequestScroll)
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

	Private Sub SwitchPrintingScreenMode()
		If InPrintScreenMode Then
			StopPrintingScreen()
			InPrintScreenMode = False
		Else
			StartPrintScreen()
			InPrintScreenMode = True
		End If
	End Sub

	Private Sub StartPrintScreen()
		PrintScreenManager.StartPrintScreen()

		AddHandler PrintScreenManager.ChangedImage,
			AddressOf ChangedImage

		AddHandler PrintScreenManager.Stopped,
			AddressOf StoppedPrintScreen
	End Sub

	Private Sub StopPrintingScreen()
		PrintScreenManager.StopPrintScreen()
	End Sub

	Private Sub ChangedImage(path As String)
		ImagePath = path
	End Sub

	Private Sub StoppedPrintScreen()
		RemoveHandler PrintScreenManager.ChangedImage, AddressOf ChangedImage
		RemoveHandler PrintScreenManager.Stopped, AddressOf StoppedPrintScreen
		InPrintScreenMode = False
	End Sub

	Private Sub RequestScroll(uri As Uri)
		Dim manager As CurrentSectionsManager = CurrentSectionsManager.GetInstance()
		manager.RequestScroll(uri)
	End Sub

	Public Sub ScrollToSelf()
		RaiseEvent ScrollToSelfEvent()
	End Sub

End Class
