Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class MainWindow_VM : Inherits ObservableObject

	Private _editorPage_VM As EditorPage_VM
	Public Property EditorPage_VM As EditorPage_VM
		Get
			Return _editorPage_VM
		End Get
		Set(value As EditorPage_VM)
			SetProperty(_editorPage_VM, value)
			OnPropertyChanged(NameOf(IsEditorMode))
		End Set
	End Property
	Private _listPage_VM As ListPage_VM
	Public Property ListPage_VM As ListPage_VM
		Get
			Return _listPage_VM
		End Get
		Set(value As ListPage_VM)
			SetProperty(_listPage_VM, value)
			OnPropertyChanged(NameOf(IsEditorMode))
		End Set
	End Property

	Public ReadOnly Property IsEditorMode As Boolean
		Get
			Return EditorPage_VM IsNot Nothing
		End Get
	End Property

	Public ReadOnly Property SaveDraftCommand As RelayCommand
	Public ReadOnly Property PublishDocCommand As RelayCommand

	Public Sub New()
		SaveDraftCommand = New RelayCommand(AddressOf SaveDraft)
		PublishDocCommand = New RelayCommand(AddressOf PublishDoc)
	End Sub

	Private Sub SaveDraft()
		EditorPage_VM?.SaveDraftCommand.Execute(Nothing)
	End Sub

	Private Sub PublishDoc()
		EditorPage_VM?.PublishDocCommand.Execute(Nothing)
	End Sub
End Class
