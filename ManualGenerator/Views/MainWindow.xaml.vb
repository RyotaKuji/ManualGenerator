Class MainWindow

	Public Sub New()
		InitializeComponent()
		NavigateToListPage()
	End Sub

	Public Sub NavigateToListPage()
		Frame.Navigate(New Uri("Views/ListPage.xaml", UriKind.Relative))
	End Sub

	Public Sub NavigateToEditorPage(id As String)
		Dim vm As New EditorPage_VM(id)
		Frame.Navigate(New EditorPage(vm))
	End Sub

End Class
