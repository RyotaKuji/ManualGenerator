Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ListPage_VM : Inherits ObservableObject

	Public Property Documents As New ObservableCollection(Of Document_E)
	Public Property WebDocs As New ObservableCollection(Of WebDoc_E)
	Public Property AllDocs As New ObservableCollection(Of Document_E)

	Public Property SelectedItem As Document_E

	Public Property SelectedCommand As RelayCommand

	Private documentRepo As Document_R
	Private webDocRepo As WebDoc_R

	Public Sub New()
		SelectedCommand = New RelayCommand(AddressOf Selected)
		Task.Run(
			Async Function()
				documentRepo = Await Document_R.CreateAsync()
				webDocRepo = Await WebDoc_R.CreateAsync()

				Dim webDocTask As Task(Of List(Of WebDoc_E)) = GetWebDocItems()
				Dim documentTask As Task(Of List(Of Document_E)) = GetDocumentItems()

				' 両方終わるまで待つ
				Await Task.WhenAll(webDocTask, documentTask)

				' 結果を取得
				Dim webDocItems As List(Of WebDoc_E) = webDocTask.Result
				Dim draftItems As List(Of Document_E) = documentTask.Result

				' UI スレッドでプロパティ/コレクション更新
				Await Application.Current.Dispatcher.InvokeAsync(
					Sub()
						WebDocs.Clear()
						For Each webDoc In webDocItems
							WebDocs.Add(webDoc)
						Next
						Documents.Clear()
						For Each doc In draftItems
							Documents.Add(doc)
						Next
					End Sub,
					DispatcherPriority.DataBind)
			End Function
		)
	End Sub

	Private Sub Selected()
		Dim id As String = SelectedItem.Id
		Dim mainWindow As MainWindow = Application.Current.MainWindow
		mainWindow.NavigateToEditorPage(id)
	End Sub

	Private Async Function GetWebDocItems() As Task(Of List(Of WebDoc_E))
		Return Await webDocRepo.ReadAllByTitleAsync("")
	End Function

	Private Async Function GetDocumentItems() As Task(Of List(Of Document_E))
		Return Await documentRepo.ReadAllByTitleAsync("")
	End Function

	Private Async Function SetItemsAsync() As Task
		If documentRepo Is Nothing Then
			documentRepo = Await Document_R.CreateAsync()
		End If

		Documents.Clear()
		Dim draftDocsResult = Await documentRepo.ReadAllByTitleAsync("")
		For Each doc In draftDocsResult
			Documents.Add(doc)
		Next

		WebDocs.Clear()
		Dim webDocsResult = Await webDocRepo.ReadAllByTitleAsync("")
		For Each webDoc In webDocsResult
			WebDocs.Add(webDoc)
		Next

	End Function
End Class
