Class ListPage
	Private ReadOnly Property VM As ListPage_VM
		Get
			Return TryCast(DataContext, ListPage_VM)
		End Get
	End Property

	Public Sub New()
		InitializeComponent()
		DataContext = New ListPage_VM()
	End Sub
End Class
