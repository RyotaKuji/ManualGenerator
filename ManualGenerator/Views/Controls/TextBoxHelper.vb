Imports System.Globalization

Public NotInheritable Class TextBoxHelper

	Private Sub New()
	End Sub

	Public Shared Function GetPlaceholder(obj As DependencyObject) As String
		Return CType(obj.GetValue(PlaceholderProperty), String)
	End Function

	Public Shared Sub SetPlaceholder(obj As DependencyObject, value As String)
		obj.SetValue(PlaceholderProperty, value)
	End Sub

	Public Shared ReadOnly PlaceholderProperty As DependencyProperty =
		DependencyProperty.RegisterAttached(
			"Placeholder",
			GetType(String),
			GetType(TextBoxHelper),
			New FrameworkPropertyMetadata(Nothing, AddressOf OnPlaceholderChanged))

	Private Shared Sub OnPlaceholderChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
		Dim textBoxControl = TryCast(d, TextBox)
		If textBoxControl IsNot Nothing Then
			If Not textBoxControl.IsLoaded Then
				RemoveHandler textBoxControl.Loaded, AddressOf TextBoxControl_Loaded
				AddHandler textBoxControl.Loaded, AddressOf TextBoxControl_Loaded
			End If

			RemoveHandler textBoxControl.TextChanged, AddressOf TextBoxControl_TextChanged
			AddHandler textBoxControl.TextChanged, AddressOf TextBoxControl_TextChanged

			Dim adorner As PlaceholderAdorner = Nothing
			If GetOrCreateAdorner(textBoxControl, adorner) Then
				adorner.InvalidateVisual()
			End If
		End If
	End Sub

	Private Shared Sub TextBoxControl_Loaded(sender As Object, e As RoutedEventArgs)
		Dim textBoxControl = TryCast(sender, TextBox)
		If textBoxControl IsNot Nothing Then
			RemoveHandler textBoxControl.Loaded, AddressOf TextBoxControl_Loaded
			Dim dummy As PlaceholderAdorner = Nothing
			GetOrCreateAdorner(textBoxControl, dummy)
		End If
	End Sub

	Private Shared Sub TextBoxControl_TextChanged(sender As Object, e As TextChangedEventArgs)
		Dim textBoxControl = TryCast(sender, TextBox)
		Dim adorner As PlaceholderAdorner = Nothing
		If textBoxControl IsNot Nothing AndAlso GetOrCreateAdorner(textBoxControl, adorner) Then
			If textBoxControl.Text.Length > 0 Then
				adorner.Visibility = Visibility.Hidden
			Else
				adorner.Visibility = Visibility.Visible
				adorner.InvalidateVisual()
			End If
		End If
	End Sub

	Private Shared Function GetOrCreateAdorner(textBoxControl As TextBox, ByRef adorner As PlaceholderAdorner) As Boolean
		Dim layer As AdornerLayer = AdornerLayer.GetAdornerLayer(textBoxControl)

		If layer Is Nothing Then
			adorner = Nothing
			Return False
		End If

		adorner = layer.GetAdorners(textBoxControl)?.OfType(Of PlaceholderAdorner)().FirstOrDefault()

		If adorner Is Nothing Then
			adorner = New PlaceholderAdorner(textBoxControl)
			layer.Add(adorner)
		End If

		Return True
	End Function

	Public Class PlaceholderAdorner
		Inherits Adorner

		Public Sub New(textBox As TextBox)
			MyBase.New(textBox)
			Me.IsHitTestVisible = False
		End Sub

		Protected Overrides Sub OnRender(drawingContext As DrawingContext)
			Dim textBoxControl = CType(AdornedElement, TextBox)

			If Not String.IsNullOrEmpty(textBoxControl.Text) Then
				Return
			End If

			Dim placeholderValue As String = TextBoxHelper.GetPlaceholder(textBoxControl)
			If String.IsNullOrEmpty(placeholderValue) Then
				Return
			End If

			Dim baseBrush = SystemColors.InactiveCaptionBrush
			Dim colorBrush As New SolidColorBrush(baseBrush.Color) With {
				.Opacity = 0.5
			}

			Dim text As New FormattedText(
				placeholderValue,
				CultureInfo.CurrentCulture,
				textBoxControl.FlowDirection,
				New Typeface(textBoxControl.FontFamily,
							 textBoxControl.FontStyle,
							 textBoxControl.FontWeight,
							 textBoxControl.FontStretch),
				textBoxControl.FontSize,
				colorBrush,
				VisualTreeHelper.GetDpi(textBoxControl).PixelsPerDip)

			text.TextAlignment = textBoxControl.TextAlignment

			text.MaxTextWidth = Math.Max(textBoxControl.ActualWidth - textBoxControl.Padding.Left - textBoxControl.Padding.Right, 10)
			text.MaxTextHeight = Math.Max(textBoxControl.ActualHeight, 10)

			Dim renderingOffset As New Point(textBoxControl.Padding.Left, textBoxControl.Padding.Top)

			Dim part = TryCast(textBoxControl.Template.FindName("PART_ContentHost", textBoxControl), FrameworkElement)
			If part IsNot Nothing Then
				Dim partPosition As Point = part.TransformToAncestor(textBoxControl).Transform(New Point(0, 0))
				renderingOffset.X += partPosition.X
				renderingOffset.Y += partPosition.Y

				text.MaxTextWidth = Math.Max(part.ActualWidth - renderingOffset.X, 10)
				text.MaxTextHeight = Math.Max(part.ActualHeight, 10)
			End If

			drawingContext.DrawText(text, renderingOffset)
		End Sub
	End Class
End Class
