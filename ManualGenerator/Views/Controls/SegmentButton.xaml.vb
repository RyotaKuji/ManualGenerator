Public Class SegmentedButton

	Public Property Item1Text As String
	Public Property Item2Text As String
	Public Property Item1Annotation As String
	Public Property Item2Annotation As String
	Public Property DefaultSelectedIndex As Integer

	Public Shared ReadOnly _Item1 As DependencyProperty =
		DependencyProperty.Register(
			"Item1",
			GetType(Object),
			GetType(SegmentedButton),
			New PropertyMetadata(Nothing))

	Public Property Item1 As Object
		Get
			Return GetValue(_Item1)
		End Get
		Set(value As Object)
			SetValue(_Item1, value)
		End Set
	End Property

	Public Shared ReadOnly _Item2 As DependencyProperty =
		DependencyProperty.Register(
			"Item2",
			GetType(Object),
			GetType(SegmentedButton),
			New PropertyMetadata(Nothing))

	Public Property Item2 As Object
		Get
			Return GetValue(_Item2)
		End Get
		Set(value As Object)
			SetValue(_Item2, value)
		End Set
	End Property

	Public Shared ReadOnly _selectionChanged As DependencyProperty =
		DependencyProperty.Register(
			"SelectionChanged",
			GetType(ICommand),
			GetType(SegmentedButton),
			New PropertyMetadata(Nothing)
		)

	Public Property SelectionChanged As ICommand
		Get
			Return CType(GetValue(_selectionChanged), ICommand)
		End Get
		Set(value As ICommand)
			SetValue(_selectionChanged, value)
		End Set
	End Property

	Private Sub OnLoaded()
		Select Case DefaultSelectedIndex
			Case 1
				Segment1.IsEnabled = False
				Segment2.IsEnabled = True
			Case 2
				Segment1.IsEnabled = True
				Segment2.IsEnabled = False
			Case Else
				Segment1.IsEnabled = False
				Segment2.IsEnabled = True
		End Select
	End Sub

	Private Sub Clicked(sender As Object, e As MouseButtonEventArgs)
		Segment1.IsEnabled = sender IsNot Segment1
		Segment2.IsEnabled = sender IsNot Segment2

		Dim selectedItem = If(sender Is Segment1, Item1, Item2)

		If SelectionChanged IsNot Nothing AndAlso
			SelectionChanged.CanExecute(selectedItem) Then
			SelectionChanged.Execute(selectedItem)
		End If
	End Sub

End Class
