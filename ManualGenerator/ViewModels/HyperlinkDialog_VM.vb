Imports System.Collections.ObjectModel
Imports CommunityToolkit.Mvvm.ComponentModel

Public Class HyperlinkDialog_VM : Inherits ObservableObject

	Private _displayText As String
	Public Property DisplayText As String
		Get
			Return _displayText
		End Get
		Set(value As String)
			SetProperty(_displayText, value)
		End Set
	End Property

	Public ReadOnly Property Uri As String
		Get
			Select Case Mode
				Case UriMode.ExternalUri
					Return ExternalUri
				Case UriMode.SectionUri
					Return SectionUri
				Case Else
					Throw New NotImplementedException("未実装のモード")
			End Select
		End Get
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

	' 外部リンク
	Public Property _externalUri As String
	Public Property ExternalUri As String
		Get
			Return _externalUri
		End Get
		Set(value As String)
			SetProperty(_externalUri, value)
		End Set
	End Property

	' セクションリンク
	Public SectionUri As String

	Private _selectedSection As Section_VM
	Public Property SelectedSection As Section_VM
		Get
			Return _selectedSection
		End Get
		Set(value As Section_VM)
			If SetProperty(_selectedSection, value) Then
				SectionUri = ConvertSectionToUri(value)
				DisplayText = value.Heading
			End If
		End Set
	End Property

	Public ReadOnly Property Sections As New ObservableCollection(Of Section_VM)
	Private ReadOnly sectionsManager As CurrentSectionsManager = CurrentSectionsManager.GetInstance()

	Private _mode As UriMode
	Public Property Mode As UriMode
		Get
			Return _mode
		End Get
		Set(value As UriMode)
			SetProperty(_mode, value)
		End Set
	End Property

	Public Enum UriMode
		ExternalUri
		SectionUri
	End Enum

	Public Sub New(defaultText As String)

		If sectionsManager.VMs IsNot Nothing Then
			For Each Section In sectionsManager.VMs
				Sections.Add(Section)
			Next
		End If

		If IsValidExternalUri(defaultText) Then
			ExternalUri = defaultText
			Mode = UriMode.ExternalUri
		ElseIf IsValidSectionUri(defaultText, Sections) Then
			SectionUri = defaultText
			SelectedSection = Sections.First(Function(s) ConvertSectionToUri(s) = SectionUri)
			Mode = UriMode.SectionUri
		End If

		DisplayText = defaultText
	End Sub

	Public Sub New(defaultUri As String, defaultText As String)

		If sectionsManager.VMs IsNot Nothing Then
			For Each Section In sectionsManager.VMs
				Sections.Add(Section)
			Next
		End If

		If IsValidExternalUri(defaultUri) Then
			ExternalUri = defaultUri
			Mode = UriMode.ExternalUri
		ElseIf IsValidSectionUri(defaultUri, Sections) Then
			SectionUri = defaultUri
			SelectedSection = Sections.First(Function(s) ConvertSectionToUri(s) = SectionUri)
			Mode = UriMode.SectionUri
		End If

		DisplayText = defaultText
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
		ElseIf IsValidExternalUri(Uri) = False And
				IsValidSectionUri(Uri, Sections) = False Then
			ErrorMessage_Uri = "リンクの形式が正しくありません。"
			isValid = False
		End If

		Return isValid

	End Function

	Private Shared Function ConvertSectionToUri(section As Section_VM) As String
		Return IdToSectionUriConverter.Convert(section.Id).OriginalString
	End Function

	Private Shared Function IsValidExternalUri(uri As String) As Boolean
		Return System.Uri.IsWellFormedUriString(uri, UriKind.Absolute)
	End Function

	Private Shared Function IsValidSectionUri(uri As String, sections As IEnumerable(Of Section_VM)) As Boolean
		Return sections.Any(Function(s) ConvertSectionToUri(s) = uri)
	End Function
End Class
