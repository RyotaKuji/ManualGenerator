Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ListPage_VM : Inherits ObservableObject

	Public Property Publics As New ObservableCollection(Of Document_E)
	Public Property Drafts As New ObservableCollection(Of Document_E)

	Public Property SelectedItem As Document_E

	Public Property SelectedCommand As RelayCommand

	Private repo As Document_R

	Public Sub New()
		SelectedCommand = New RelayCommand(AddressOf Selected)
		Task.Run(
			Async Function()

				Await Initialize()

				' 結果を取得
				Dim draftItems As List(Of Document_E) = Await GetDocuments(False)
				Dim PublicItems As List(Of Document_E) = Await GetDocuments(True)

				' UI スレッドでプロパティ/コレクション更新
				Await Application.Current.Dispatcher.InvokeAsync(
					Sub()
						Publics.Clear()
						For Each doc In draftItems
							Publics.Add(doc)
						Next
						Drafts.Clear()
						For Each doc In PublicItems
							Drafts.Add(doc)
						Next
					End Sub,
					DispatcherPriority.DataBind)
			End Function
		)
	End Sub

	Private Async Function Initialize() As Task
		If repo Is Nothing Then
			repo = Await Document_R.CreateAsync()
		End If
	End Function

	Private Sub Selected()
		Dim id As String = SelectedItem.Id
		Dim mainWindow As MainWindow = Application.Current.MainWindow
		mainWindow.NavigateToEditorPage(id)
	End Sub

	Private Async Function GetDocuments(isPublic As Boolean) As Task(Of List(Of Document_E))
		Return Await repo.ReadAllByTitleAsync("", isPublic)
	End Function

	Private Async Function SetItemsAsync() As Task
		Drafts.Clear()
		Dim draftDocsResult = Await repo.ReadAllByTitleAsync("", False)
		For Each doc In draftDocsResult
			Drafts.Add(doc)
		Next
		Publics.Clear()
		Dim publicDocsResult = Await repo.ReadAllByTitleAsync("", True)
		For Each doc In publicDocsResult
			Publics.Add(doc)
		Next
	End Function
End Class
