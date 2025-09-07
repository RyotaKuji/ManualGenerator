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
	End Sub

	' ListPage へ移動
	Public Sub NavigateToListPage()
		VM.EditorPage_VM = Nothing
		Frame.Navigate(New ListPage())
	End Sub

	' EditorPage へ移動
	Public Sub NavigateToEditorPage(id As String)
		Dim pageVM As New EditorPage_VM(id)
		VM.EditorPage_VM = pageVM

		Frame.Navigate(New EditorPage(pageVM))
	End Sub

	Private Sub Back(sender As Object, e As RoutedEventArgs)
		NavigateToListPage()
	End Sub

End Class
