Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Newtonsoft.Json

Public Class EditorPage_VM
	Inherits ObservableObject

	Public ReadOnly Property SubmitCommand As RelayCommand
	Public ReadOnly Property AddCommand As RelayCommand(Of Process_VM)

	Public ReadOnly Property Id As String

	Public ReadOnly Property Processes As New ObservableCollection(Of Process_VM)

	Public Sub New()
		SubmitCommand = New RelayCommand(AddressOf Submit)
		AddCommand = New RelayCommand(Of Process_VM)(AddressOf AddItem)
	End Sub

	Public Sub New(id As String)
		Me.New()
		Dim loadingResult = Load(id)
		If loadingResult Then
			Me.Id = id
		Else
			Me.Id = Guid.NewGuid().ToString()
		End If

		If Processes.Count = 0 Then
			SetDefaultItems()
		End If
	End Sub

	Private Function Load(id As String) As Boolean
		Dim filePath = FileManager.GetFilePathById(id)

		If Not FileManager.ExistsFile(filePath) Then
			Return False
		End If

		Try
			Dim json As String = FileManager.ReadContent(filePath)
			Dim deserialized = JsonConvert.DeserializeObject(Of IEnumerable(Of Process_VM))(json)
			Processes.Clear()
			For Each item In deserialized
				Dim formedItem As Process_VM = GetProcessObject(item)
				Processes.Add(item)
			Next

			MarkLastItem()

			Return True
		Catch ex As Exception
			Dim s As String = ex.Message
			Return False
		End Try
	End Function

	Private Sub Save()
		Dim filePath = FileManager.GetFilePathById(Id)
		Dim json As String = JsonConvert.SerializeObject(Processes)
		FileManager.WriteContent(json, filePath)
	End Sub

	' 送信
	Private Sub Submit()
		Save()
	End Sub

	Private Sub SetDefaultItems()
		Dim heading As New Process_VM With {
			.IsHeading = True
		}
		Dim process As New Process_VM With {
			.IsHeading = False
		}
		AddItem(heading)
		AddItem(process)
	End Sub

	Public Sub AddItem(Optional item As Process_VM = Nothing)
		Dim index As Integer = Processes.IndexOf(item)
		Dim newProcess = GetProcessObject()

		If index >= 0 And index < Processes.Count Then
			Processes.Insert(index, newProcess)
		Else
			Processes.Add(newProcess)
		End If
		MarkLastItem()
	End Sub

	Private Sub RemoveItem(item As Process_VM)
		If Processes.Count > 1 Then
			Processes.Remove(item)
		End If
		MarkLastItem()
	End Sub

	Private Sub MarkLastItem()
		For Each item In Processes
			item.IsLastItem = False
		Next
		Processes.Last().IsLastItem = True
	End Sub

	Private Function GetProcessObject(Optional item As Process_VM = Nothing) As Process_VM
		If item Is Nothing Then
			item = New Process_VM
		End If

		item.RemoveCommand = New RelayCommand(
			Sub()
				RemoveItem(item)
			End Sub)

		Return item
	End Function

End Class
