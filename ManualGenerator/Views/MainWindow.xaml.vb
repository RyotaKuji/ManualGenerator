Class MainWindow

	Private ReadOnly Property VM As MainWindow_VM
		Get
			Return TryCast(DataContext, MainWindow_VM)
		End Get
	End Property

	Public Sub New()

		InitializeComponent()
		DataContext = New MainWindow_VM()

		NavigateToListPage()
	End Sub

	' ListPage へ移動
	Public Sub NavigateToListPage()
		VM.NavigateToListPage()
	End Sub

	' EditorPage へ移動
	Public Sub NavigateToEditorPage(Optional id As String = Nothing)
		VM.NavigateToEditorPage(id)
	End Sub

End Class
