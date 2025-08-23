Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class EditorArea_VM
	Inherits ObservableObject

	Public ReadOnly Property SubmitCommand As RelayCommand

	Public ReadOnly Property Processes As New ObservableCollection(Of ProcessGroup_VM)

	Public Sub New()
		SubmitCommand = New RelayCommand(AddressOf Submit)
	End Sub

	' 送信
	Private Sub Submit()
		AddItem()
	End Sub

	Private Sub AdjustItemsLength(item As ProcessGroup_VM)

		Dim hasHeadingText As Boolean = String.IsNullOrWhiteSpace(item.Heading) = False

		If item Is Processes.Last() And hasHeadingText Then
			' 末尾要素の見出しが入力された場合、要素を追加
			AddItem()
		End If
	End Sub

	Private Sub AddItem(Optional index As Integer = -1)
		Dim newProcess As New ProcessGroup_VM()
		newProcess.BeginInputCommand = New RelayCommand(
			Sub()
				AdjustItemsLength(newProcess)
			End Sub)

		If index >= 0 And index < Processes.Count Then
			Processes.Insert(index, newProcess)
		Else
			Processes.Add(newProcess)
		End If
	End Sub

	Private Sub RemoveItem(item As ProcessGroup_VM)
		Processes.Remove(item)
	End Sub

	Private Function IsRemovableItem(item As ProcessGroup_VM) As Boolean
		Dim isHeadingEmpty As Boolean = String.IsNullOrWhiteSpace(item.Heading)
		Return isHeadingEmpty
	End Function

End Class
