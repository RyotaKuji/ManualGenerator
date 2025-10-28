Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports ManualGenerator.Definitions

Public Class ListPage_VM : Inherits ObservableObject

	Public Property Items As New ObservableCollection(Of Document_E)

	Public Property SelectedItem As Document_E

	Private _selectedPubStatus As Definitions.PubStatus = Definitions.PubStatus.Draft
	Public Property SelectedPubStatus As Definitions.PubStatus
		Get
			Return _selectedPubStatus
		End Get
		Set(value As Definitions.PubStatus)
			If SetProperty(_selectedPubStatus, value) Then
				SetItems(value)
			End If
		End Set
	End Property

	Public Property SelectedCommand As New RelayCommand(AddressOf Selected)
	Public Property CreateNewCommand As New RelayCommand(AddressOf CreateNew)
	Public Property SwitchPubStatusCommand As New RelayCommand(Of Definitions.PubStatus)(AddressOf SwitchPubStatus)

	Private repo As Document_R

	Private ReadOnly pubStatus_Items As New Dictionary(Of Definitions.PubStatus, List(Of Document_E))

	Public Sub New()
		Task.Run(
			Async Function()

				Await Initialize()

				' Enumの全値でTaskを作成
				Dim tasks = New List(Of Task)()

				tasks.Add(
					Task.Run(
						Async Function()
							Dim items = Await repo.ReadAllAsync(PubStatus.Published)
							SyncLock pubStatus_Items
								pubStatus_Items(PubStatus.Published) = items
							End SyncLock
						End Function
					)
				)

				If UserInfo.GetInstance()?.Name IsNot Nothing Then
					tasks.Add(
						Task.Run(
							Async Function()
								Dim items = Await repo.ReadAllAsync(PubStatus.Draft, UserInfo.GetInstance().Name)
								SyncLock pubStatus_Items
									pubStatus_Items(PubStatus.Draft) = items
								End SyncLock
							End Function
						)
					)
				End If

				Await Task.WhenAll(tasks)

				' UI スレッドでプロパティ/コレクション更新
				Await Application.Current.Dispatcher.InvokeAsync(
					Sub() SetItems(Definitions.PubStatus.Draft),
					DispatcherPriority.DataBind)
			End Function
		)
	End Sub

	Private Async Function Initialize() As Task
		If repo Is Nothing Then
			repo = Await Document_R.CreateAsync()
		End If
	End Function

	Private Sub Selected()
		Dim id As String = SelectedItem.Id
		Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)
		mainWindow.NavigateToEditorPage(id)
	End Sub

	Private Sub CreateNew()
		Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)
		mainWindow.NavigateToEditorPage()
	End Sub

	Private Sub SwitchPubStatus(newStatus As Definitions.PubStatus)
		SetItems(newStatus)
	End Sub

	Private Sub SetItems(pubStatus As Definitions.PubStatus)

		Dim displayItems As List(Of Document_E) = pubStatus_Items(pubStatus)

		Items.Clear()
		For Each doc In displayItems
			Items.Add(doc)
		Next
	End Sub
End Class
