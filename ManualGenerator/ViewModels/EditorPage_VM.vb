Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class EditorPage_VM : Inherits ObservableObject

	Private ReadOnly Entity As Document_E

	Public ReadOnly Property Id As String
		Get
			Return Entity.Id
		End Get
	End Property

	Private _title As String
	Public Property Title As String
		Get
			Return _title
		End Get
		Set(value As String)
			If SetProperty(_title, value) Then
				Entity.Title = value
			End If
		End Set
	End Property

	Public ReadOnly Property Sections As New ObservableCollection(Of Section_VM)

	Public ReadOnly Property SaveDraftCommand As RelayCommand
	Public ReadOnly Property PublishDocCommand As RelayCommand
	Public ReadOnly Property DeleteDocCommand As RelayCommand

	Public ReadOnly Property AddSectionCommand As RelayCommand(Of Section_VM)

	Public Sub New(Optional id As String = Nothing)
		' Command の初期化
		SaveDraftCommand = New RelayCommand(AddressOf SaveDraft)
		AddSectionCommand = New RelayCommand(Of Section_VM)(AddressOf AddSection)

		' ドキュメントのロード
		Dim entity = Load(id)
		If entity Is Nothing Then
			Me.Entity = New Document_E()
		Else
			Me.Entity = entity
		End If

		Title = entity.Title

		' 要素が空の場合はデフォルトの要素を追加
		If Sections.Count = 0 Then
			SetDefaultItems()
		End If

	End Sub

	''' <summary>
	''' 指定した Id のファイルを読み込み
	''' </summary>
	''' <param name="id">ドキュメント ID</param>
	''' <returns>Entity（失敗した場合は Nothing）</returns>
	Private Function Load(id As String) As Document_E

		Dim entity As Document_E = DraftManager.Load(id)
		If entity Is Nothing Then
			Return Nothing
		End If

		' 要素を追加
		Sections.Clear()
		For Each item As Section_E In entity.Sections
			Dim sectionVM = New Section_VM(item)
			SetCommands(sectionVM)
			Sections.Add(sectionVM)
		Next

		MarkLastSection()
		Return entity

	End Function

	''' <summary>
	''' 保存
	''' </summary>
	Private Sub SaveDraft()

		' Sections の内容を Entity に反映
		Entity.Sections = Sections.Select(Function(x) x.Entity)
		DraftManager.Save(Entity)
	End Sub

	''' <summary>
	''' デフォルトの要素を追加
	''' 見出し要素と通常要素を1つずつ
	''' </summary>
	Private Sub SetDefaultItems()
		Dim headingItem As New Section_VM With {
			.IsHeadline = True
		}
		Dim processItem As New Section_VM With {
			.IsHeadline = False
		}

		SetCommands(headingItem)
		SetCommands(processItem)

		Sections.Add(headingItem)
		Sections.Add(processItem)

		MarkLastSection()
	End Sub

	''' <summary>
	''' 指定した要素の前に新しい要素を追加
	''' item が Nothing か 無効なインデックスの場合は末尾に追加
	''' </summary>
	''' <param name="item">追加したい位置の直後にある要素</param>
	Public Sub AddSection(Optional item As Section_VM = Nothing)
		' item が Nothing の場合は新しい Section_VM を作成
		Dim index As Integer = Sections.IndexOf(item)

		Dim addedItem As New Section_VM()
		SetCommands(addedItem)

		If index >= 0 And index < Sections.Count Then
			' 有効なインデックスの場合
			Sections.Insert(index, addedItem)
		Else
			' 無効なインデックスの場合
			Sections.Add(addedItem)
		End If

		MarkLastSection()
	End Sub

	''' <summary>
	''' 指定した要素を削除
	''' </summary>
	''' <param name="item"></param>
	Private Sub RemoveItem(item As Section_VM)
		If Sections.Count > 1 Then
			Sections.Remove(item)
		End If
		MarkLastSection()
	End Sub

	''' <summary>
	''' 末尾の要素に IsLastItem を設定
	''' AddCommandBar が表示される
	''' </summary>
	Private Sub MarkLastSection()
		For Each item In Sections
			item.IsLastItem = False
		Next
		Sections.Last().IsLastItem = True
	End Sub

	''' <summary>
	''' Section_VM に Command を設定
	''' </summary>
	''' <param name="item"></param>
	Private Sub SetCommands(item As Section_VM)

		item.RemoveCommand = New RelayCommand(
			Sub()
				RemoveItem(item)
			End Sub)

	End Sub

End Class
