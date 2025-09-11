Public Class UserSettings

	Public Shared ReadOnly Property UserName As String
		Get
			Return Environment.UserName
		End Get
	End Property

	Public Shared Property DisplayUserName As String
		Get
			Return My.MySettings.Default.DisplayUserName
		End Get
		Set(value As String)
			If Not String.IsNullOrWhiteSpace(value) Then
				My.MySettings.Default.DisplayUserName = value
			End If
		End Set
	End Property

End Class
