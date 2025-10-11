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

		' フォーカスを移動して編集中の内容を確定
		Dim request = New TraversalRequest(FocusNavigationDirection.Next)
		Dim focusedElement = Keyboard.FocusedElement
		Dim frameworkElement As FrameworkElement = TryCast(focusedElement, FrameworkElement)
		If frameworkElement IsNot Nothing Then
			frameworkElement.MoveFocus(request)
			Keyboard.Focus(frameworkElement)
		End If

		Dim result = Await VM.CheckClosing()
		If result Then
			RemoveHandler Closing, AddressOf Window_Closing
		Else
			e.Cancel = True
		End If
	End Sub

End Class
