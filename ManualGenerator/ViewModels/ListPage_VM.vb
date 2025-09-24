Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ListPage_VM : Inherits ObservableObject

	Public Property Items As New ObservableCollection(Of Document_E)

	Public Property SelectedItem As Document_E

	Public Property SelectedCommand As RelayCommand
	Public Property CreateNewCommand As RelayCommand
	Public Property SwitchPubStatusCommand As RelayCommand(Of Definitions.PubStatus)

	Private repo As Document_R

	Private pubStatus_Items As New Dictionary(Of Definitions.PubStatus, List(Of Document_E))

	Public Sub New()
		SelectedCommand = New RelayCommand(AddressOf Selected)
		CreateNewCommand = New RelayCommand(AddressOf CreateNew)
		SwitchPubStatusCommand = New RelayCommand(Of Definitions.PubStatus)(AddressOf SwitchPubStatus)

		Task.Run(
			Async Function()

				Await Initialize()

				' Enumの全値でTaskを作成
				Dim tasks = New List(Of Task)()
				For Each status As Definitions.PubStatus In [Enum].GetValues(GetType(Definitions.PubStatus))
					tasks.Add(
						Task.Run(
							Async Function()
								Dim items = Await repo.ReadAllAsync(status)
								SyncLock pubStatus_Items
									pubStatus_Items(status) = items
								End SyncLock
							End Function
						)
					)
				Next

				Await Task.WhenAll(tasks)

				' UI スレッドでプロパティ/コレクション更新
				Await Application.Current.Dispatcher.InvokeAsync(
					Sub()
						SetItems(Definitions.PubStatus.Draft)
					End Sub,
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
		Dim mainWindow As MainWindow = Application.Current.MainWindow
		mainWindow.NavigateToEditorPage(id)
	End Sub

	Private Sub CreateNew()
		Dim mainWindow As MainWindow = Application.Current.MainWindow
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
