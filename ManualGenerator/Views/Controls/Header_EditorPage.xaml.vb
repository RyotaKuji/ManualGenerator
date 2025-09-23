Public Class Header_EditorPage

	Private Sub Back(sender As Object, e As RoutedEventArgs)
		Dim mainWindow = CType(Application.Current.MainWindow, MainWindow)
		mainWindow.NavigateToListPage()
	End Sub
End Class
