Class EditorPage
	Private ReadOnly Property VM As EditorPage_VM
		Get
			Return TryCast(DataContext, EditorPage_VM)
		End Get
	End Property

	Public Sub New(vm As EditorPage_VM)
		InitializeComponent()
		DataContext = vm
	End Sub
End Class
