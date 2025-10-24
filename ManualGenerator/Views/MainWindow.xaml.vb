Class MainWindow

	Private ReadOnly Property VM As MainWindow_VM
		Get
			Return TryCast(DataContext, MainWindow_VM)
		End Get
	End Property

	Public Sub New()

		InitializeComponent()
		DataContext = New MainWindow_VM()

		NavigateToListPage()

		AddHandler Closing, AddressOf Window_Closing
	End Sub

	' ListPage へ移動
	Public Sub NavigateToListPage()
		VM.NavigateToListPage()
	End Sub

	' EditorPage へ移動
	Public Sub NavigateToEditorPage(Optional id As String = Nothing)
		VM.NavigateToEditorPage(id)
	End Sub

	Private Async Sub Window_Closing(sender As Object, e As ComponentModel.CancelEventArgs)

		Dim result = Await VM.CheckClosing()
		If result Then
			RemoveHandler Closing, AddressOf Window_Closing
		Else
			e.Cancel = True
		End If
	End Sub

	Private Sub ClickedClose(sender As Object, e As RoutedEventArgs)
		Close()
	End Sub

	Private Sub ClickedMaximize(sender As Object, e As RoutedEventArgs)
		Select Case WindowState
			Case WindowState.Maximized
				WindowState = WindowState.Normal
			Case WindowState.Normal
				WindowState = WindowState.Maximized
		End Select
	End Sub

	Private Sub ClickedMinimize(sender As Object, e As RoutedEventArgs)
		WindowState = WindowState.Minimized
	End Sub
End Class
