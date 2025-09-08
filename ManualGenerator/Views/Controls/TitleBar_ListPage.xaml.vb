Public Class TitleBar_ListPage
	Private Sub CreateNew(sender As Object, e As RoutedEventArgs)
		Dim mainWindow = CType(Application.Current.MainWindow, MainWindow)
		mainWindow.NavigateToEditorPage()
	End Sub
End Class
