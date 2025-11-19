Imports System.DirectoryServices.AccountManagement

Public Class UserInfo
	Public ReadOnly Property Id As String
	Public ReadOnly Property Name As String
	Public ReadOnly Property Department As String
	Private Shared ReadOnly instance As New UserInfo()

	Public Shared Function GetInstance() As UserInfo
		Return instance
	End Function

	Private Sub New()
		Try
			Using user As UserPrincipal = UserPrincipal.Current
				Id = user.Guid.ToString() '+ "AA"
				Name = user.Name '+ "AA"
				Department = user.Description '+ "AA"
			End Using
		Catch ex As Exception
			Throw New UserInfoException(ex)
		End Try
	End Sub

End Class
