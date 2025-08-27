Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ListPage_VM
	Inherits ObservableObject

	Public Property ItemNames As New List(Of String)

	Public Property SelectedItem As String

	Public Property SelectedCommand As RelayCommand

	Public Sub New()
		SelectedCommand = New RelayCommand(AddressOf Selected)
		ItemNames = IO.Directory.GetFiles(FileManager.DataFileBasePath, "*", IO.SearchOption.AllDirectories).ToList()
	End Sub

	Private Sub Selected()
		Dim id As String = FileManager.GetId(SelectedItem)
		Dim mainWindow As MainWindow = Application.Current.MainWindow
		mainWindow.NavigateToEditorPage(id)
	End Sub
End Class
