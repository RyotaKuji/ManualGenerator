Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.ComponentModel

Public Class HyperlinkDialog_VM : Inherits ObservableObject

	Private _uri As String
	Public Property Uri As String
		Get
			Return _uri
		End Get
		Set(value As String)
			SetProperty(_uri, value)
		End Set
	End Property

	Private _displayText As String
	Public Property DisplayText As String
		Get
			Return _displayText
		End Get
		Set(value As String)
			SetProperty(_displayText, value)
		End Set
	End Property

	Private _ErrorMessage_DisplayText As String
	Public Property ErrorMessage_DisplayText As String
		Get
			Return _ErrorMessage_DisplayText
		End Get
		Set(value As String)
			SetProperty(_ErrorMessage_DisplayText, value)
		End Set
	End Property

	Private _ErrorMessage_Uri As String
	Public Property ErrorMessage_Uri As String
		Get
			Return _ErrorMessage_Uri
		End Get
		Set(value As String)
			SetProperty(_ErrorMessage_Uri, value)
		End Set
	End Property

	Public ReadOnly Property Sections As New ObservableCollection(Of Section_E)

	Public Sub New(defaultText As String)

		If IsValidUri(defaultText) Then
			Uri = defaultText
		End If

		DisplayText = defaultText

		If EditorPage_VM.Entity IsNot Nothing Then
			For Each section In EditorPage_VM.Entity.Sections
				Sections.Add(section)
			Next
		End If
	End Sub

	Public Sub New(defaultUri As String, defaultText As String)

		If IsValidUri(defaultUri) Then
			Uri = defaultUri
		End If

		DisplayText = defaultText

		If EditorPage_VM.Entity IsNot Nothing Then
			For Each section In EditorPage_VM.Entity.Sections
				Sections.Add(section)
			Next
		End If
	End Sub

	Public Function Validate() As Boolean

		Dim isValid As Boolean = True

		If String.IsNullOrWhiteSpace(DisplayText) Then
			ErrorMessage_DisplayText = "入力されていません。"
			isValid = False
		End If
		If String.IsNullOrWhiteSpace(Uri) Then
			ErrorMessage_Uri = "入力されていません。"
			isValid = False
		ElseIf IsValidUri(Uri) = False Then
			ErrorMessage_Uri = "URLの形式が正しくありません。"
			isValid = False
		End If

		Return isValid

	End Function

	Private Shared Function IsValidUri(link As String) As Boolean
		Return System.Uri.IsWellFormedUriString(link, UriKind.Absolute)
	End Function
End Class
