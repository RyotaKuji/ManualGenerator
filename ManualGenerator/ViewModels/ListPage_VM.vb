Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ListPage_VM : Inherits ObservableObject

	Public Property Items As New ObservableCollection(Of Document_E)

	Private _isPublic As Boolean
	Public Property IsPublic As Boolean
		Get
			Return _isPublic
		End Get
		Set(value As Boolean)
			If SetProperty(_isPublic, value) Then
				SetItems(value)
			End If
		End Set
	End Property

	Public Property SelectedItem As Document_E

	Public Property SelectedCommand As RelayCommand
	Public Property CreateNewCommand As RelayCommand
	Public Property SwitchIsPublicCommand As RelayCommand(Of Definitions.PubStatus)

	Private repo As Document_R

	Private draftItems As List(Of Document_E)
	Private publicItems As List(Of Document_E)

	Public Sub New()
		SelectedCommand = New RelayCommand(AddressOf Selected)
		CreateNewCommand = New RelayCommand(AddressOf CreateNew)
		SwitchIsPublicCommand = New RelayCommand(Of Definitions.PubStatus)(Sub(b) IsPublic = b = Definitions.PubStatus.Published)

		Task.Run(
			Async Function()

				Await Initialize()

				' 結果を取得
				draftItems = Await repo.ReadAllByTitleAsync("", False)
				publicItems = Await repo.ReadAllByTitleAsync("", True)

				' UI スレッドでプロパティ/コレクション更新
				Await Application.Current.Dispatcher.InvokeAsync(
					Sub()
						IsPublic = False
						SetItems(IsPublic)
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

	Private Sub SetItems(isPublic As Boolean)

		Dim displayItems As List(Of Document_E) = If(isPublic, publicItems, draftItems)

		Items.Clear()
		For Each doc In displayItems
			Items.Add(doc)
		Next
	End Sub
End Class
