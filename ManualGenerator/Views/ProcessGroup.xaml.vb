Public Class ProcessGroup

	Private ReadOnly Property VM As ProcessGroup_VM
		Get
			Return TryCast(DataContext, ProcessGroup_VM)
		End Get
	End Property

	Private Sub Heading_LostFocus(sender As Object, e As RoutedEventArgs)
		VM?.ChangedHeading(Heading.Text)
	End Sub

End Class
