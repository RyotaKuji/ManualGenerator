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
				closingManager.HasChange = True

				If Not String.IsNullOrWhiteSpace(value) Then
					' 自身を参照しているリンクのテキストを更新
					xmlManager.ChangeSectionHeading(Id, _heading, value)
					' エラー状態を解除
					HasHeadingError = False
				End If
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
				closingManager.HasChange = True
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
				closingManager.HasChange = True
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
				closingManager.HasChange = True
			End If
		End Set
	End Property

	Private _isLastItem As Boolean
	Public Property IsLastItem As Boolean
		Get
			Return _isLastItem
		End Get
		Set(value As Boolean)
			SetProperty(_isLastItem, value)
		End Set
	End Property

	Private _isRemovable As Boolean
	Public Property IsRemovable As Boolean
		Get
			Return _isRemovable
		End Get
		Set(value As Boolean)
			SetProperty(_isRemovable, value)
		End Set
	End Property

	Private _hasHeadingError As Boolean
	Public Property HasHeadingError As Boolean
		Get
			Return _hasHeadingError
		End Get
		Set(value As Boolean)
			SetProperty(_hasHeadingError, value)
		End Set
	End Property

	Public Property RemoveCommand As RelayCommand
	Public Event ScrollToSelfEvent()
	Public Event RequestHeadingInputEvent()

	Public ReadOnly Property SwitchPrintingScreenModeCommand As New RelayCommand(AddressOf SwitchPrintingScreenMode)
	Public ReadOnly Property RequestScrollCommand As New RelayCommand(Of Uri)(AddressOf RequestScroll)

	Private ReadOnly closingManager As EditorClosingManager = EditorClosingManager.GetInstance()
	Private ReadOnly xmlManager As XmlManager = XmlManager.GetInstance()
	Private ReadOnly printScreen As PrintScreen = PrintScreen.GetInstance()

	Public Sub New()
		Entity = New Section_E()
		xmlManager.Sections.Add(Me)
	End Sub

	Public Sub New(entity As Section_E)
		Me.Entity = entity
		xmlManager.Sections.Add(Me)
	End Sub

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
		printScreen.Start()

		AddHandler printScreen.ChangedImage,
			AddressOf ChangedImage

		AddHandler printScreen.Stopped,
			AddressOf StoppedPrintScreen
	End Sub

	Private Sub StopPrintingScreen()
		printScreen.Stop()
	End Sub

	Private Sub ChangedImage(path As String)
		ImagePath = path
	End Sub

	Private Sub StoppedPrintScreen()
		RemoveHandler printScreen.ChangedImage, AddressOf ChangedImage
		RemoveHandler printScreen.Stopped, AddressOf StoppedPrintScreen
		InPrintScreenMode = False

		Task.Run(Async Function()
					 Await SaveImageAsync()
				 End Function)
	End Sub

	Private Async Function SaveImageAsync() As Task

		Dim sourcePath As String = ImagePath
		Dim fileName As String = Path.GetFileName(sourcePath)
		Dim destPath As String = Path.Combine(My.Resources.ImageDir, fileName)
		Try
			Using sourceStream As New FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync:=True),
			  destStream As New FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync:=True)

				Await sourceStream.CopyToAsync(destStream)
			End Using
		Catch ex As Exception
			Return
		End Try
		ImagePath = destPath
	End Function

	Private Sub RequestScroll(uri As Uri)
		Dim manager As CurrentSectionsManager = CurrentSectionsManager.GetInstance()
		manager.RequestScroll(uri)
	End Sub

	Public Sub ScrollToSelf()
		RaiseEvent ScrollToSelfEvent()
	End Sub

	Public Sub RequestHeadingInput()
		HasHeadingError = True
		RaiseEvent RequestHeadingInputEvent()
	End Sub

End Class
