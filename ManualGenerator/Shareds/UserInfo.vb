Imports System.DirectoryServices.AccountManagement

Public Class UserInfo

	Public ReadOnly Property Name As String
	Public Property Description As String

	Private Shared ReadOnly instance As New UserInfo()

	Public Shared Function GetInstance() As UserInfo
		Return instance
	End Function

	Private Sub New()
		Try
			Using user As UserPrincipal = UserPrincipal.Current
				Name = user.Name
				Description = user.Description
			End Using
		Catch
			Name = Environment.UserName
			Description = ""
		End Try
	End Sub

End Class
