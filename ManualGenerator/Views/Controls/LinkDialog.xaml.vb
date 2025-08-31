Public Class LinkDialog

	Public Property Link As String
	Public Property DisplayText As String

	' 多重クローズ防止
	Private IsClosed As Boolean = False

	Public Sub New(defaultText As String)

		InitializeComponent()

		Application.Current.MainWindow.Opacity = 0.7

		If IsValidLink(defaultText) Then
			TextBox_Link.Text = defaultText
		Else
			TextBox_DisplayText.Text = defaultText
		End If

	End Sub

	Protected Overrides Sub OnClosed(e As EventArgs)
		MyBase.OnClosed(e)

		Application.Current.MainWindow.Opacity = 1.0
	End Sub

	Private Shared Function IsValidLink(link As String) As Boolean
		Return Uri.IsWellFormedUriString(link, UriKind.Absolute)
	End Function

	' 画面表示後にサイズを調整
	Private Sub Window_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
		InvalidateMeasure()
	End Sub

	Private Sub OK_Clicked(sender As Object, e As RoutedEventArgs)

		If Validate() = False Then
			Return
		End If

		Link = TextBox_Link.Text
		DisplayText = TextBox_DisplayText.Text

		TrySetResult(True)
	End Sub

	Private Sub Cancel_Clicked(sender As Object, e As RoutedEventArgs)
		TrySetResult(False)
	End Sub

	Private Function Validate() As Boolean
		Return True
		Dim isValid As Boolean = True

		If String.IsNullOrWhiteSpace(TextBox_DisplayText.Text) Then
			ErrorMessage_DisplayText.Text = "入力されていません。"
			isValid = False
		End If
		If String.IsNullOrWhiteSpace(TextBox_Link.Text) Then
			ErrorMessage_Link.Text = "入力されていません。"
			isValid = False
		ElseIf IsValidLink(TextBox_Link.Text) = False Then
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
