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

	Protected Overrides Sub OnPreviewMouseWheel(e As MouseWheelEventArgs)
		MyBase.OnPreviewMouseWheel(e)

		' 親のScrollViewerを探してスクロールさせる
		Dim scrollViewer = FindParent(Of ScrollViewer)(Me)
		If scrollViewer IsNot Nothing Then
			If e.Delta <> 0 Then
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta)
				e.Handled = True
			End If
		End If
	End Sub

	' ヘルパーメソッド
	Private Shared Function FindParent(Of T As DependencyObject)(child As DependencyObject) As T
		Dim parentObject As DependencyObject = VisualTreeHelper.GetParent(child)
		If parentObject Is Nothing Then Return Nothing
		If TypeOf parentObject Is T Then Return CType(parentObject, T)
		Return FindParent(Of T)(parentObject)
	End Function
End Class
