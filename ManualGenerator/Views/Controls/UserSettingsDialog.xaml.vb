Public Class UserSettingsDialog

	Private ReadOnly Property VM As UserSettingsDialog_VM
		Get
			Return TryCast(DataContext, UserSettingsDialog_VM)
		End Get
	End Property

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

	' 画面表示後にサイズを調整
	Private Sub Window_ContentRendered(sender As Object, e As EventArgs)
		InvalidateMeasure()
	End Sub

	Private Sub Window_Deactivated(sender As Object, e As EventArgs)
		If VM.IsSetDisplayUserName Then
			TrySetResult(True)
		End If
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
