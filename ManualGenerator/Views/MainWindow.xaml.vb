Class MainWindow

	Public Sub New()
		InitializeComponent()
		Frame.Navigate(New Uri("Views/EditorPage.xaml", UriKind.Relative))
	End Sub

End Class
