Public Class UpdateException : Inherits Exception
	Public Sub New(ex As Exception)
		MyBase.New(ex.Message, ex)
		Application.Current.Dispatcher.Invoke(Sub() StyledMessageBox.Show(ex.Message, "[error] データ更新失敗"))
	End Sub
End Class
