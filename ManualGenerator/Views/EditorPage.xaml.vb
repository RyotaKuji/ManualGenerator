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

	Private Sub ListBox_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
		e.Handled = True
		Dim eventArg = New MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
		eventArg.RoutedEvent = UIElement.MouseWheelEvent
		eventArg.Source = sender
		ScrollViewer.RaiseEvent(eventArg)
	End Sub
End Class
