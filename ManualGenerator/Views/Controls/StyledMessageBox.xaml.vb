Public Class StyledMessageBox

	Private Sub New(messageBoxText As String, caption As String, button As MessageBoxButton, defaultResult As MessageBoxResult)

		InitializeComponent()

		Message.Text = messageBoxText
		Me.Title.Content = caption
		Owner = Application.Current.MainWindow
		WindowStartupLocation = WindowStartupLocation.CenterOwner

		InvalidateMeasure()

		Select Case button
			Case MessageBoxButton.OK
				Button_OK.Visibility = Visibility.Visible
			Case MessageBoxButton.OKCancel
				Button_OK.Visibility = Visibility.Visible
				Button_Cancel.Visibility = Visibility.Visible
			Case MessageBoxButton.YesNo
				Button_Yes.Visibility = Visibility.Visible
				Button_No.Visibility = Visibility.Visible
			Case MessageBoxButton.YesNoCancel
				Button_Yes.Visibility = Visibility.Visible
				Button_No.Visibility = Visibility.Visible
				Button_Cancel.Visibility = Visibility.Visible
			Case Else
				Button_OK.Visibility = Visibility.Visible
		End Select

		Select Case defaultResult
			Case MessageBoxResult.OK
				Button_OK.IsDefault = True
				Button_OK.Focus()
			Case MessageBoxResult.Cancel
				Button_Cancel.IsDefault = True
				Button_Cancel.Focus()
			Case MessageBoxResult.Yes
				Button_Yes.IsDefault = True
				Button_Yes.Focus()
			Case MessageBoxResult.No
				Button_No.IsDefault = True
				Button_No.Focus()
		End Select

	End Sub

	Public Overloads Shared Function Show(messageBoxText As String, caption As String, button As MessageBoxButton, defaultResult As MessageBoxResult) As MessageBoxResult
		Dim msgBox As New StyledMessageBox(messageBoxText, caption, button, defaultResult)

		Application.Current.MainWindow.Opacity = 0.7

		Dim result As Boolean? = msgBox.ShowDialog()

		If result.HasValue Then
			Select Case button
				Case MessageBoxButton.OK
					Return MessageBoxResult.OK
				Case MessageBoxButton.OKCancel
					If result.Value Then
						Return MessageBoxResult.OK
					Else
						Return MessageBoxResult.Cancel
					End If
				Case MessageBoxButton.YesNo
					If result.Value Then
						Return MessageBoxResult.Yes
					Else
						Return MessageBoxResult.No
					End If
				Case MessageBoxButton.YesNoCancel
					If result.Value Then
						Return MessageBoxResult.Yes
					Else
						Return MessageBoxResult.Cancel
					End If
				Case Else
					Return MessageBoxResult.None
			End Select
		Else
			Return MessageBoxResult.None
		End If

		Application.Current.MainWindow.Opacity = 1.0
	End Function

	Public Overloads Shared Function Show(messageBoxText As String, caption As String, button As MessageBoxButton, icon As MessageBoxImage, defaultResult As MessageBoxResult) As MessageBoxResult
		Return Show(messageBoxText, caption, button, defaultResult)
	End Function

	Public Overloads Shared Function Show(messageBoxText As String, caption As String, button As MessageBoxButton, icon As MessageBoxImage) As MessageBoxResult
		Return Show(messageBoxText, caption, button, MessageBoxResult.OK)
	End Function

	Public Overloads Shared Function Show(messageBoxText As String, caption As String, button As MessageBoxButton) As MessageBoxResult
		Return Show(messageBoxText, caption, button, MessageBoxResult.OK)
	End Function

	Public Overloads Shared Function Show(messageBoxText As String, caption As String) As MessageBoxResult
		Return Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxResult.OK)
	End Function

	Public Overloads Shared Function Show(messageBoxText As String) As MessageBoxResult
		Return Show(messageBoxText, String.Empty, MessageBoxButton.OK, MessageBoxResult.OK)
	End Function

	Protected Overrides Sub OnClosed(e As EventArgs)
		MyBase.OnClosed(e)

		Application.Current.MainWindow.Opacity = 1.0
	End Sub

	Private Sub OK_Clicked(sender As Object, e As RoutedEventArgs)
		DialogResult = True
	End Sub

	Private Sub Cancel_Clicked(sender As Object, e As RoutedEventArgs)
		DialogResult = False
	End Sub

	Private Sub Yes_Clicked(sender As Object, e As RoutedEventArgs)
		DialogResult = True
	End Sub

	Private Sub No_Clicked(sender As Object, e As RoutedEventArgs)
		DialogResult = False
	End Sub
End Class
