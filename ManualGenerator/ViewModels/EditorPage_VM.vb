Imports System.Collections.ObjectModel
Imports System.Windows.Threading
Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class EditorPage_VM
	Inherits ObservableObject

	Public ReadOnly Property Id As String
		Get
			Return entity.Id
		End Get
	End Property

	Private _title As String
	Public Property Title As String
		Get
			Return _title
		End Get
		Set(value As String)
			If SetProperty(_title, value) Then
				entity.Title = value
				closingManager.HasChange = True
				If Not String.IsNullOrWhiteSpace(value) Then
					HasTitleError = False
				End If
			End If
		End Set
	End Property

	Private _hasTitleError As Boolean
	Public Property HasTitleError As Boolean
		Get
			Return _hasTitleError
		End Get
		Set(value As Boolean)
			SetProperty(_hasTitleError, value)
		End Set
	End Property

	Public ReadOnly Property Sections As New ObservableCollection(Of Section_VM)

	Public ReadOnly Property SaveDraftCommand As New AsyncRelayCommand(AddressOf SaveDraftAsync)
	Public ReadOnly Property PublishDocCommand As New AsyncRelayCommand(AddressOf PublishDocAsync)
	Public ReadOnly Property BackCommand As AsyncRelayCommand = New AsyncRelayCommand(AddressOf Back)

	Public ReadOnly Property AddSectionCommand As New RelayCommand(Of Section_VM)(AddressOf AddSection)
	Public Event OnRequestedTitleInput()
	Public Event OnSaved()

	Private entity As Document_E

	Private repo As Document_R
	Private ReadOnly closingManager As EditorClosingManager = EditorClosingManager.GetInstance()
	Private ReadOnly sectionsManager As CurrentSectionsManager = CurrentSectionsManager.GetInstance()

	Public Sub New(Optional id As String = Nothing)

		sectionsManager.VMs = Sections

		Task.Run(
			Async Function()

				Await Initialize()

				Dim entity = Await GetEntityAsync(id)
				Me.entity = entity

				' UI スレッドでプロパティ/コレクション更新
				Await Application.Current.Dispatcher.InvokeAsync(
					Sub()

						Title = entity.Title
						SetSections()

						' closingManager を初期化
						closingManager.HasChange = False
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

	''' <summary>
	''' 指定した Id のファイルを読み込み
	''' </summary>
	''' <param name="id">ドキュメント ID</param>
	''' <returns>Entity（失敗した場合は Nothing）</returns>
	Private Async Function GetEntityAsync(id As String) As Task(Of Document_E)

		Dim entity As Document_E = Await repo.ReadAsync(id)

		If entity Is Nothing Then
			entity = New Document_E()
		End If

		Return entity

	End Function

	Private Sub SetSections()
		' 要素を追加
		Sections.Clear()
		For Each item As Section_E In entity.Sections
			Dim sectionVM = New Section_VM(item)
			SetCommands(sectionVM)
			Sections.Add(sectionVM)
		Next

		' 要素が空の場合はデフォルトの要素を追加
		If Sections.Count = 0 Then
			SetDefaultItems()
		End If

		MarkSectionInfo()
	End Sub

	''' <summary>
	''' 保存
	''' </summary>
	Private Async Function SaveAsync(pubStatus As Definitions.PubStatus) As Task

		If String.IsNullOrWhiteSpace(Title) Then
			RequestTitleInput()
			StyledMessageBox.Show("タイトルを入力してください", "エラー", MessageBoxButton.OK)
			Return
		End If

		If pubStatus = Definitions.PubStatus.Published Then
			For Each section In Sections
				If String.IsNullOrWhiteSpace(section.Heading) Then
					section.RequestHeadingInput()
					StyledMessageBox.Show("見出しを入力してください", "エラー", MessageBoxButton.OK)
					Return
				End If
			Next
		End If

		' Entities の内容を Entity に反映
		Dim sectionEntities = Sections.Select(Function(x) x.Entity)
		For i As Integer = 0 To sectionEntities.Count() - 1
			sectionEntities(i).OrderIndex = i
		Next

		entity.Sections = sectionEntities

		Dim userInfo As UserInfo = UserInfo.GetInstance()
		entity.Author = userInfo.Name
		entity.Department = userInfo.Description

		Await repo.CreateOrUpdateAsync(entity, pubStatus)

		RaiseEvent OnSaved()

		closingManager.HasChange = False
	End Function

	''' <summary>
	''' 下書き保存
	''' </summary>
	Private Async Function SaveDraftAsync() As Task
		Await SaveAsync(Definitions.PubStatus.Draft)
	End Function

	''' <summary>
	''' 公開
	''' </summary>
	Private Async Function PublishDocAsync() As Task
		Await SaveAsync(Definitions.PubStatus.Published)
	End Function

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

		MarkSectionInfo()
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

		MarkSectionInfo()

		closingManager.HasChange = True
	End Sub

	''' <summary>
	''' 指定した要素を削除
	''' </summary>
	''' <param name="item"></param>
	Private Sub RemoveItem(item As Section_VM)

		If Sections.Count <= 1 Then
			Return
		End If

		Dim message As String = If(String.IsNullOrWhiteSpace(item.Heading), "セクション", item.Heading & vbCrLf) & "を削除しますか？"
		Dim result As MessageBoxResult = StyledMessageBox.Show(
			message, "削除", MessageBoxButton.YesNo, MessageBoxResult.No)

		If result <> MessageBoxResult.Yes Then
			Return
		End If

		Sections.Remove(item)
		XmlManager.GetInstance().RemovedSection(item.Id)

		MarkSectionInfo()

		closingManager.HasChange = True
	End Sub

	Private Sub MarkSectionInfo()
		MarkLastSection()
		MarkRemovable()
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
	''' 要素が1つだけの場合に IsRemovable を設定
	''' RemoveButton が非表示になる
	''' </summary>
	Private Sub MarkRemovable()
		For Each item In Sections
			item.IsRemovable = True
		Next
		If Sections.Count = 1 Then
			Sections.First().IsRemovable = False
		End If
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

	''' <summary>
	''' タイトル入力を要求
	''' </summary>
	Private Sub RequestTitleInput()
		HasTitleError = True
		RaiseEvent OnRequestedTitleInput()
	End Sub

	Private Async Function Back() As Task
		Dim result = Await CheckClosing()

		If result Then
			Dim mainWindow = CType(Application.Current.MainWindow, MainWindow)
			mainWindow.NavigateToListPage()
		End If
	End Function

	Public Async Function CheckClosing() As Task(Of Boolean)
		If closingManager.HasChange Then
			Dim result As MessageBoxResult = StyledMessageBox.Show(
			"変更を保存しますか？",
			"保存確認",
			MessageBoxButton.YesNoCancel,
			MessageBoxResult.Yes,
			{"保存", "保存しない", "キャンセル"})

			Select Case result
				Case MessageBoxResult.Yes
					' 保存に成功したら、closingManager.HasChange = False になる
					Await SaveDraftAsync()
				Case MessageBoxResult.No
					closingManager.HasChange = False
			End Select
		End If

		Return closingManager.HasChange = False
	End Function
End Class
