Imports CommunityToolkit.Mvvm.ComponentModel

Public Class MainWindow_VM : Inherits ObservableObject

	Private _currentHeader As Object
	Public Property CurrentHeader As Object
		Get
			Return _currentHeader
		End Get
		Set(value As Object)
			SetProperty(_currentHeader, value)
		End Set
	End Property

	Private _currentPage As Object
	Public Property CurrentPage As Object
		Get
			Return _currentPage
		End Get
		Set(value As Object)
			SetProperty(_currentPage, value)
		End Set
	End Property

	' ListPage へ移動
	Public Sub NavigateToListPage()
		Dim vm = New ListPage_VM()
		CurrentHeader = New Header_ListPage() With {
			.DataContext = vm
		}
		CurrentPage = New ListPage With {
			.DataContext = vm
		}
	End Sub

	' EditorPage へ移動
	Public Sub NavigateToEditorPage(Optional id As String = Nothing)
		Dim vm = New EditorPage_VM(id)
		CurrentHeader = New Header_EditorPage() With {
			.DataContext = vm
		}
		CurrentPage = New EditorPage() With {
			.DataContext = vm
		}
	End Sub
End Class
