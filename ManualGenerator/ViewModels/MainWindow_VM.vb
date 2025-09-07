Imports CommunityToolkit.Mvvm.ComponentModel

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

	Public ReadOnly Property IsEditorMode As Boolean
		Get
			Return EditorPage_VM IsNot Nothing
		End Get
	End Property
End Class
