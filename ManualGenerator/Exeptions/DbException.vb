Public Class DbException : Inherits Exception
	Public Sub New(ex As Exception)
		MyBase.New(ex.Message, ex)
		Application.Current.Dispatcher.Invoke(Sub() StyledMessageBox.Show($"以下の点を確認してください。{Environment.NewLine}{Environment.NewLine}・ネットワークに接続しているか。{Environment.NewLine}・ドメインユーザーとしてログインしているか。", "[error] DB接続失敗"))
	End Sub
End Class
