Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports ManualGenerator.Definitions

Public Class ListPage_VM : Inherits ObservableObject

	Public ReadOnly Property Items As New ObservableCollection(Of Document_E)

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

	Private _selectedSearchMode As SearchMode = SearchMode.All
	Public Property SelectedSearchMode As SearchMode
		Get
			Return _selectedSearchMode
		End Get
		Set(value As SearchMode)
			SetProperty(_selectedSearchMode, value)
		End Set
	End Property

	Public Property SelectedCommand As New RelayCommand(AddressOf Selected)
	Public Property SearchCommand As New AsyncRelayCommand(AddressOf Search)
	Public Property CreateNewCommand As New RelayCommand(AddressOf CreateNew)
	Public Property SwitchPubStatusCommand As New RelayCommand(Of PubStatus)(AddressOf SwitchPubStatus)

	Public ReadOnly Property SearchMode_Jp As New Dictionary(Of SearchMode, String) From {
		{SearchMode.All, "キーワード"},
		{SearchMode.Title, "タイトル"},
		{SearchMode.DocumentId, "ドキュメントID"}
	}

	Public Enum SearchMode
		All
		Title
		DocumentId
	End Enum

	Private repo As Document_R

	Private pubStatus_Items As New Dictionary(Of PubStatus, IEnumerable(Of Document_E))

	Private Shared instance As ListPage_VM

	Public Shared Function GetInstance() As ListPage_VM
		If instance Is Nothing Then
			instance = New ListPage_VM()
		End If
		Return instance
	End Function

	Private Sub New()
		Task.Run(
			Async Function()

				Await Initialize()

				Dim query = New DocumentQuery() With {.AuthorId = UserInfo.GetInstance().Id}
				Dim queries = New Dictionary(Of PubStatus, DocumentQuery) From {
					{
						PubStatus.Published, query
					},
					{
						PubStatus.Draft, query
					}
				}
				pubStatus_Items = Await repo.ReadAllAsync(queries)

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
		If SelectedItem Is Nothing Then
			Return
		End If

		Dim id As String = SelectedItem.Id
		Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)
		mainWindow.NavigateToEditorPage(id)
	End Sub

	Private Sub CreateNew()
		Dim mainWindow As MainWindow = CType(Application.Current.MainWindow, MainWindow)
		mainWindow.NavigateToEditorPage()
	End Sub

	Private Async Function Search() As Task

		Dim query As New DocumentQuery()
		With query
			Select Case SelectedSearchMode
				Case SearchMode.All
					.Keyword = Keyword
					.AuthorId = UserInfo.GetInstance().Id
				Case SearchMode.Title
					.Title = Keyword
					.AuthorId = UserInfo.GetInstance().Id
				Case SearchMode.DocumentId
					.DocumentId = Keyword
			End Select
		End With

		Dim queries = New Dictionary(Of PubStatus, DocumentQuery) From
		{
			{PubStatus.Published, query},
			{PubStatus.Draft, query}
		}

		pubStatus_Items = Await repo.ReadAllAsync(queries)

		Await SetDisplayedItemsAsync(SelectedPubStatus)
	End Function

	Private Sub SwitchPubStatus(newStatus As PubStatus)
		SetDisplayedItems(newStatus)
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

		Dim displayItems As IEnumerable(Of Document_E) = pubStatus_Items(pubStatus)
		For Each doc In displayItems
			Items.Add(doc)
		Next
	End Sub
End Class
