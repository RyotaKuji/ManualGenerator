Imports System.Threading
Imports System.Windows.Forms
Imports System.Windows.Media.Animation

Public Class Summary_Horizontal

	Private ReadOnly Property ShouledKeepVisible As Boolean
		Get
			If IsMouseOver Then
				Return True
			End If

			For Each item In listBox.Items
				Dim listBoxItem As ListBoxItem = TryCast(listBox.ItemContainerGenerator.ContainerFromItem(item), ListBoxItem)
				If listBoxItem IsNot Nothing AndAlso listBoxItem.IsFocused Then
					Return True
				End If
			Next

			Return False
		End Get
	End Property

	Private cts As CancellationTokenSource

	Public Sub New()
		InitializeComponent()
		AddHandler LostFocus, Sub() Hide()
		AddHandler MouseLeave, Sub() HideWithDelay()
	End Sub

	Public Sub Display()
		If Opacity > 0 Then
			Return
		End If

		Dim fadeIn As New DoubleAnimation() With {
			.From = 0,
			.To = 1,
			.Duration = New Duration(TimeSpan.FromSeconds(0.4)),
			.EasingFunction = New CubicEase() With {.EasingMode = EasingMode.EaseInOut},
			.FillBehavior = FillBehavior.Stop
		}
		AddHandler fadeIn.Completed, Sub()
										 Opacity = 1
									 End Sub

		BeginAnimation(UIElement.OpacityProperty, fadeIn)

		HideWithDelay()
	End Sub

	Private Async Sub HideWithDelay()
		If cts?.Token.IsCancellationRequested = False Then
			cts?.Token.ThrowIfCancellationRequested()
			cts.Cancel()
			cts.Dispose()
			cts = Nothing
		End If

		Dim newCts As New CancellationTokenSource()
		cts = newCts

		Dim task As Task = Task.Delay(3000, cts.Token)

		If task.IsCanceled = False Then
			Try
				Await task
				Hide()
			Catch e As TaskCanceledException
			End Try
		End If
	End Sub

	Private Sub Hide()
		If Opacity < 1 Then
			Return
		End If

		If ShouledKeepVisible() Then
			Return
		End If

		Dim anim As New DoubleAnimation() With {
			.From = 1,
			.To = 0,
			.Duration = New Duration(TimeSpan.FromSeconds(0.4)),
			.EasingFunction = New CubicEase() With {.EasingMode = EasingMode.EaseInOut},
			.FillBehavior = FillBehavior.HoldEnd
		}
		BeginAnimation(UIElement.OpacityProperty, anim)
	End Sub

	''' <summary>
	''' Selected ステータスを無効化
	''' </summary>
	Private Sub ListBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
		listBox.SelectedItem = Nothing
	End Sub
End Class
