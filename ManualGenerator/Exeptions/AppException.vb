Public Class AppException : Inherits Exception
	Public Sub New(ex As Exception)
		MyBase.New(ex.Message, ex)
		Dim message = $"{ex.Message}{vbCr}{vbCr}{ex.StackTrace}"
		Application.Current.Dispatcher.Invoke(Sub() MessageBox.Show(message, "エラー", MessageBoxButton.OK, MessageBoxImage.Error))
	End Sub
End Class
