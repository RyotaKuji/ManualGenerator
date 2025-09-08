Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ListPage_VM : Inherits ObservableObject

	Public Property DraftDocs As New ObservableCollection(Of Document_E)
	Public Property PublicDocs As New ObservableCollection(Of Document_E)

	Public Property SelectedItem As Document_E

	Public Property SelectedCommand As RelayCommand

	Private repo As New DocumentRepository()

	Public Sub New()
		SelectedCommand = New RelayCommand(AddressOf Selected)
		SetItems()
	End Sub

	Private Sub Selected()
		Dim id As String = SelectedItem.Id
		Dim mainWindow As MainWindow = Application.Current.MainWindow
		mainWindow.NavigateToEditorPage(id)
	End Sub

	Private Sub SetItems()
		DraftDocs.Clear()
		Dim draftDocsResult = repo.ReadAllByTitle("")
		For Each doc In draftDocsResult
			DraftDocs.Add(doc)
		Next
	End Sub
End Class
