Public Class HyperlinkDialog

	Private ReadOnly Property VM As HyperlinkDialog_VM
		Get
			Return TryCast(DataContext, HyperlinkDialog_VM)
		End Get
	End Property

	Public ReadOnly Property DisplayText As String
		Get
			Return VM.DisplayText
		End Get
	End Property

	Public ReadOnly Property Uri As String
		Get
			Return VM.Uri
		End Get
	End Property

	' 多重クローズ防止
	Private IsClosed As Boolean = False

	Public Sub New(defaultText As String)
		InitializeComponent()
		DataContext = New HyperlinkDialog_VM(defaultText)

		Application.Current.MainWindow.Opacity = 0.7
	End Sub

	Public Sub New(defaultUri As String, defaultText As String)
		InitializeComponent()
		DataContext = New HyperlinkDialog_VM(defaultUri, defaultText)

		Application.Current.MainWindow.Opacity = 0.7
	End Sub

	Protected Overrides Sub OnClosed(e As EventArgs)
		MyBase.OnClosed(e)

		Application.Current.MainWindow.Opacity = 1.0
	End Sub

	' 画面表示後にサイズを調整
	Private Sub Window_ContentRendered(sender As Object, e As EventArgs) Handles Me.ContentRendered
		InvalidateMeasure()
	End Sub

	Private Sub OK_Clicked(sender As Object, e As RoutedEventArgs)

		If VM.Validate() = False Then
			Return
		End If

		TrySetResult(True)
	End Sub

	Private Sub Cancel_Clicked(sender As Object, e As RoutedEventArgs)
		TrySetResult(False)
	End Sub

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
