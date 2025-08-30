Public Class LinkDialog
	Inherits Window

	Public Property Link As String
	Public Property DisplayText As String

	' 多重クローズ防止
	Private IsClosed As Boolean = False

	Public Sub New()
		InitializeComponent()
		Application.Current.MainWindow.Opacity = 0.7
	End Sub

	Protected Overrides Sub OnClosed(e As EventArgs)
		MyBase.OnClosed(e)
		Application.Current.MainWindow.Opacity = 1.0
	End Sub

	Private Sub Window_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
		InvalidateMeasure()
	End Sub

	Private Sub OK_Clicked(sender As Object, e As RoutedEventArgs)

		If Validate() = False Then
			Return
		End If

		TrySetResult(True)
	End Sub

	Private Sub Cancel_Clicked(sender As Object, e As RoutedEventArgs)
		TrySetResult(False)
	End Sub

	Private Function Validate() As Boolean

		Dim isValid As Boolean = True

		If String.IsNullOrWhiteSpace(TextBox_DisplayText.Text) Then
			ErrorMessage_DisplayText.Text = "入力されていません。"
			isValid = False
		End If
		If String.IsNullOrWhiteSpace(TextBox_Link.Text) Then
			ErrorMessage_Link.Text = "入力されていません。"
			isValid = False
		ElseIf Uri.IsWellFormedUriString(TextBox_Link.Text, UriKind.Absolute) = False Then
			ErrorMessage_Link.Text = "URLの形式が正しくありません。"
			isValid = False
		End If

		Return isValid

	End Function

	Private Sub Window_Deactivated(sender As Object, e As EventArgs)
		TrySetResult(False)
	End Sub

	' 多重クローズ防止
	' クローズ後に DialogResult を設定すると例外が発生
	Private Sub TrySetResult(dialogResult As Boolean)
		If IsClosed = False Then
			IsClosed = True
			Me.DialogResult = dialogResult
		End If
	End Sub
End Class
