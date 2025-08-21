Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.Input

Public Class EditorArea_VM
	Public ReadOnly Property SubmitCommand As RelayCommand

	Public ReadOnly Property Processes As New ObservableCollection(Of ProcessGroup_VM)

	Public Sub New()
		SubmitCommand = New RelayCommand(AddressOf Submit)
	End Sub

	' 送信
	Private Sub Submit()
		Processes.Add(New ProcessGroup_VM())
	End Sub

End Class
