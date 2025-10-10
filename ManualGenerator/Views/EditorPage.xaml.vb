Imports System.Windows.Controls.Primitives
Imports CommunityToolkit.Mvvm.Input

Class EditorPage
	Private ReadOnly Property VM As EditorPage_VM
		Get
			Return TryCast(DataContext, EditorPage_VM)
		End Get
	End Property

	Public ReadOnly Property SaveCommand As ICommand

	Public Sub New()
		' Command の初期化
		' InitializeComponent の前に登録が必要
		SaveCommand = New RelayCommand(AddressOf Save)

		InitializeComponent()

		AddHandler Loaded, AddressOf Page_Loaded
	End Sub

	Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
		AddHandler VM.RequestTitleInputEvent, AddressOf RequestTitleInput
	End Sub

	''' <summary>
	''' 編集中の RichTextBox の内容を確定する
	''' ショートカットキー使用時に編集中の内容が反映されない問題を回避するため
	''' </summary>
	Private Sub Save()
		ConfirmEditingTextBox()
		VM.SaveDraftCommand.Execute(Nothing)
	End Sub

	Private Sub ConfirmEditingTextBox()
		' フォーカス要素が TextBox/RichTextBox なら、編集中の内容を確定
		Dim rtb As TextBoxBase = TryCast(Keyboard.FocusedElement, TextBoxBase)
		If rtb IsNot Nothing Then
			' フォーカスを移動して編集中の内容を確定
			Dim request = New TraversalRequest(FocusNavigationDirection.Next)
			rtb.MoveFocus(request)
			' 元の要素にフォーカスを戻す
			Keyboard.Focus(rtb)
		End If
	End Sub

	''' <summary>
	''' ListBox のマウスホイールイベントを ScrollViewer に転送する
	''' </summary>
	Private Sub ListBox_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
		e.Handled = True
		Dim eventArg = New MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
		eventArg.RoutedEvent = UIElement.MouseWheelEvent
		eventArg.Source = sender
		ScrollViewer.RaiseEvent(eventArg)
	End Sub

	Private Sub RequestTitleInput()
		ScrollViewer.ScrollToTop()
		Title.Focus()
	End Sub
End Class
