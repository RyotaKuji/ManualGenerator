Imports CommunityToolkit.Mvvm.ComponentModel

Public Class UserSettingsDialog_VM : Inherits ObservableObject

	Private ReadOnly Property settings As UserSettings = UserSettings.GetInstance()

	Public Property DisplayUserName As String
		Get
			Return settings.DisplayUserName
		End Get
		Set(value As String)
			If Not String.IsNullOrWhiteSpace(value) Then
				SetProperty(settings.DisplayUserName, value)
			End If
		End Set
	End Property

	Public ReadOnly Property IsSetDisplayUserName As Boolean
		Get
			Return settings.IsSetDisplayUserName
		End Get
	End Property

	Public Property Department As String
		Get
			Return settings.Department
		End Get
		Set(value As String)
			SetProperty(settings.Department, value)
		End Set
	End Property
End Class
