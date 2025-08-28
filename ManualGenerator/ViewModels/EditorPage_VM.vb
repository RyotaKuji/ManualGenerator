Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input
Imports Newtonsoft.Json

Public Class EditorPage_VM
	Inherits ObservableObject

	Public ReadOnly Property SubmitCommand As RelayCommand
	Public ReadOnly Property AddCommand As RelayCommand(Of Section_VM)

	Public ReadOnly Property Id As String

	Public ReadOnly Property Sections As New ObservableCollection(Of Section_VM)

	Public Sub New(Optional id As String = Nothing)
		' Command の初期化
		SubmitCommand = New RelayCommand(AddressOf Submit)
		AddCommand = New RelayCommand(Of Section_VM)(AddressOf AddItem)

		' ドキュメントのロード
		Dim isLoadSucceeded = Load(id)
		If isLoadSucceeded Then
			Me.Id = id
		Else
			' ロードに失敗した場合は新しいドキュメントとして扱う
			Me.Id = Guid.NewGuid().ToString()
		End If

		' 要素が空の場合はデフォルトの要素を追加
		If Sections.Count = 0 Then
			SetDefaultItems()
		End If

	End Sub

	''' <summary>
	''' 指定した Id のファイルを読み込み
	''' </summary>
	''' <param name="id">ドキュメント ID</param>
	''' <returns>読み込みに成功したか</returns>
	Private Function Load(id As String) As Boolean
		' ファイルパスの取得
		Dim filePath = FileManager.GetFilePathById(id)

		' ファイルが存在しない場合は失敗
		If Not FileManager.ExistsFile(filePath) Then
			Return False
		End If

		Try
			' JSON を読み込み、デシリアライズ
			Dim json As String = FileManager.ReadContent(filePath)
			Dim deserialized = JsonConvert.DeserializeObject(Of IEnumerable(Of Section_VM))(json)
			' デシアライズできなかったら失敗
			If deserialized Is Nothing Then
				Return False
			End If

			' 要素を追加
			Sections.Clear()
			For Each item In deserialized
				Dim formedItem As Section_VM = SetCommands(item)
				Sections.Add(item)
			Next

			MarkLastItem()
			Return True

		Catch ex As Exception
			' 例外が発生した場合は失敗
			Return False
		End Try
	End Function

	''' <summary>
	''' 送信
	''' </summary>
	Private Sub Submit()
		Save()
		Dim result As String = HTMLService.GetFullHTML(FileManager.GetFilePathById(Id))
	End Sub

	''' <summary>
	''' 保存
	''' </summary>
	Private Sub Save()
		Dim filePath = FileManager.GetFilePathById(Id)
		Dim json As String = JsonConvert.SerializeObject(Sections)
		FileManager.WriteContent(json, filePath)
	End Sub

	''' <summary>
	''' デフォルトの要素を追加
	''' 見出し要素と通常要素を1つずつ
	''' </summary>
	Private Sub SetDefaultItems()
		Dim headingItem As New Section_VM With {
			.IsHeading = True
		}
		Dim processItem As New Section_VM With {
			.IsHeading = False
		}
		Sections.Add(headingItem)
		Sections.Add(processItem)

		MarkLastItem()
	End Sub

	''' <summary>
	''' 指定した要素の前に新しい要素を追加
	''' item が Nothing か 無効なインデックスの場合は末尾に追加
	''' </summary>
	''' <param name="item">追加したい位置の直後にある要素</param>
	Public Sub AddItem(Optional item As Section_VM = Nothing)
		' item が Nothing の場合は新しい Section_VM を作成
		Dim index As Integer = Sections.IndexOf(item)

		Dim addedItem As New Section_VM
		If index >= 0 And index < Sections.Count Then
			' 有効なインデックスの場合
			Sections.Insert(index, addedItem)
		Else
			' 無効なインデックスの場合
			Sections.Add(addedItem)
		End If

		MarkLastItem()
	End Sub

	''' <summary>
	''' 指定した要素を削除
	''' </summary>
	''' <param name="item"></param>
	Private Sub RemoveItem(item As Section_VM)
		If Sections.Count > 1 Then
			Sections.Remove(item)
		End If
		MarkLastItem()
	End Sub

	''' <summary>
	''' 末尾の要素に IsLastItem を設定
	''' AddCommandBar が表示される
	''' </summary>
	Private Sub MarkLastItem()
		For Each item In Sections
			item.IsLastItem = False
		Next
		Sections.Last().IsLastItem = True
	End Sub

	''' <summary>
	''' Section_VM に Command を設定
	''' </summary>
	''' <param name="item"></param>
	''' <returns>Command を設定した Section_VM</returns>
	Private Function SetCommands(item As Section_VM) As Section_VM

		item.RemoveCommand = New RelayCommand(
			Sub()
				RemoveItem(item)
			End Sub)

		Return item
	End Function

End Class
