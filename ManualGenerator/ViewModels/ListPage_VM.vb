Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports ManualGenerator.Definitions

Public Class ListPage_VM : Inherits ObservableObject

	Public Property Items As New ObservableCollection(Of Document_E)

	Public Property SelectedItem As Document_E

	Private _selectedPubStatus As PubStatus = PubStatus.Draft
	Public Property SelectedPubStatus As PubStatus
		Get
			Return _selectedPubStatus
		End Get
		Set(value As PubStatus)
			If SetProperty(_selectedPubStatus, value) Then
				SetDisplayedItems(value)
			End If
		End Set
	End Property

	Private _keyword As String
	Public Property Keyword As String
		Get
			Return _keyword
		End Get
		Set(value As String)
			SetProperty(_keyword, value)
		End Set
	End Property

	Public Property SelectedCommand As New RelayCommand(AddressOf Selected)
	Public Property SearchCommand As New AsyncRelayCommand(AddressOf Search)
	Public Property CreateNewCommand As New RelayCommand(AddressOf CreateNew)
	Public Property SwitchPubStatusCommand As New RelayCommand(Of PubStatus)(AddressOf SwitchPubStatus)

	Private repo As Document_R

	Private pubStatus_Items As New Dictionary(Of PubStatus, List(Of Document_E))

	Public Sub New()
		Task.Run(
			Async Function()

				Await Initialize()

				Dim items As List(Of Document_E) = Await repo.ReadAllAsync(New Document_Query())
				SetResults(items)

				Await SetDisplayedItemsAsync(PubStatus.Draft)
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

	Private Async Function Search() As Task

		Dim query As New Document_Query() With {
			.Keyword = Keyword
		}
		Dim items As List(Of Document_E) = Await repo.ReadAllAsync(query)
		SetResults(items)

		Await SetDisplayedItemsAsync(SelectedPubStatus)
	End Function

	Private Sub SwitchPubStatus(newStatus As PubStatus)
		SetDisplayedItems(newStatus)
	End Sub

	Private Sub SetResults(items As List(Of Document_E))
		pubStatus_Items =
			items.GroupBy(Function(i) i.PubStatus).
			ToDictionary(Function(g) g.Key, Function(g) g.ToList())
	End Sub

	''' <summary>
	''' ' UI スレッドでコレクションを更新する
	''' </summary>
	''' <param name="pubStatus">表示する PubStatus</param>
	Private Async Function SetDisplayedItemsAsync(pubStatus As PubStatus) As Task
		Await Application.Current.Dispatcher.InvokeAsync(
			Sub() SetDisplayedItems(pubStatus),
			DispatcherPriority.DataBind)
	End Function

	Private Sub SetDisplayedItems(pubStatus As PubStatus)
		Items.Clear()

		If pubStatus_Items.ContainsKey(pubStatus) = False Then
			Return
		End If

		Dim displayItems As List(Of Document_E) = pubStatus_Items(pubStatus)
		For Each doc In displayItems
			Items.Add(doc)
		Next
	End Sub
End Class
