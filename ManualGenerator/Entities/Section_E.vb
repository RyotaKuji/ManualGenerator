Imports SQLite

<Table("Sections")>
Public Class Section_E
	<PrimaryKey, NotNull>
	Public Property Id As String
		Get
			Return GetIdWithPubStatus(BaseId, PubStatus)
		End Get
		Set(value As String)
			BaseId = GetBaseId(value)
		End Set
	End Property

	<Ignore>
	Private Property BaseId As String = Guid.NewGuid().ToString()

	<Indexed, NotNull>
	Public Property DocumentId As String

	Public Property Heading As String
	Public Property DescriptionXaml As String
	Public Property DescriptionHtml As String
	Public Property DescriptionText As String
	Public Property ImagePath As String
	Public Property IsHeadline As Boolean
	Public Property OrderIndex As Integer

	' SQLite.NET では Enum を直接保存できないため、整数値として保存する
	<NotNull>
	Public Property PubStatusValue As Integer = 0

	<Ignore>
	Public Property PubStatus As Definitions.PubStatus
		Get
			Return CType(PubStatusValue, Definitions.PubStatus)
		End Get
		Set(value As Definitions.PubStatus)
			PubStatusValue = value
		End Set
	End Property

	Public Function Clone(pubStatus As Definitions.PubStatus) As Section_E
		If Me.PubStatus = pubStatus Then
			Return Me
		End If

		Dim newEntity As New Section_E With {
			.BaseId = BaseId,
			.PubStatus = pubStatus,
			.DocumentId = Document_E.GetIdWithPubStatus(DocumentId, pubStatus),
			.Heading = Heading,
			.DescriptionXaml = DescriptionXaml,
			.DescriptionHtml = DescriptionHtml,
			.DescriptionText = DescriptionText,
			.ImagePath = ImagePath,
			.IsHeadline = IsHeadline,
			.OrderIndex = OrderIndex
		}

		Return newEntity
	End Function

	Public Shared Function GetIdWithPubStatus(id As String, pubStatus As Definitions.PubStatus) As String
		Dim baseId As String = GetBaseId(id)
		Dim suffix As String = My.Resources.SuffixDelimiter & pubStatus.ToString()
		Return baseId & suffix
	End Function

	Private Shared Function GetBaseId(id As String) As String
		Dim delimiterIndex As Integer = id.IndexOf(My.Resources.SuffixDelimiter)
		If delimiterIndex = -1 Then
			Return id
		End If

		Return id.Substring(0, delimiterIndex)
	End Function

End Class