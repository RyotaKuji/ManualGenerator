Public Class UserSettings

	Public ReadOnly Property UserName As String
		Get
			Return Environment.UserName
		End Get
	End Property

	Public Property DisplayUserName As String
		Get
			If Not String.IsNullOrWhiteSpace(My.MySettings.Default.DisplayUserName) Then
				Return My.MySettings.Default.DisplayUserName
			Else
				Return UserName
			End If
		End Get
		Set(value As String)
			If Not String.IsNullOrWhiteSpace(value) Then
				My.MySettings.Default.DisplayUserName = value
				My.MySettings.Default.Save()
			End If
		End Set
	End Property

	Public Property Department As String
		Get
			Return My.MySettings.Default.Department
		End Get
		Set(value As String)
			My.MySettings.Default.Department = value
			My.MySettings.Default.Save()
		End Set
	End Property

	Public ReadOnly Property IsSetDisplayUserName As Boolean
		Get
			Return Not String.IsNullOrWhiteSpace(My.MySettings.Default.DisplayUserName)
		End Get
	End Property

	Private Shared ReadOnly instance As New UserSettings()

	Public Shared Function GetInstance() As UserSettings
		Return instance
	End Function

End Class
