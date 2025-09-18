Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ListPage_VM : Inherits ObservableObject

	Public Property Documents As New ObservableCollection(Of Document_E)
	Public Property AllDocs As New ObservableCollection(Of Document_E)

	Public Property SelectedItem As Document_E

	Public Property SelectedCommand As RelayCommand

	Private repo As Document_R

	Public Sub New()
		SelectedCommand = New RelayCommand(AddressOf Selected)
		Task.Run(
			Async Function()

				Await Initialize()

				' 結果を取得
				Dim draftItems As List(Of Document_E) = Await GetDocumentItems()

				' UI スレッドでプロパティ/コレクション更新
				Await Application.Current.Dispatcher.InvokeAsync(
					Sub()
						Documents.Clear()
						For Each doc In draftItems
							Documents.Add(doc)
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

	Private Async Function GetDocumentItems() As Task(Of List(Of Document_E))
		Return Await repo.ReadAllByTitleAsync("")
	End Function

	Private Async Function SetItemsAsync() As Task
		Documents.Clear()
		Dim draftDocsResult = Await repo.ReadAllByTitleAsync("")
		For Each doc In draftDocsResult
			Documents.Add(doc)
		Next
	End Function
End Class
