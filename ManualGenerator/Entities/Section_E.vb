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
	Public Property DescriptionXml As String
	Public Property ImagePath As String
	Public Property IsHeadline As Boolean
	Public Property OrderIndex As Integer

	Public Property DescriptionHtml As String
		Get
			Return XmlToHtmlConverter.Convert(DescriptionXml)
		End Get
		' SQLite.Net で有効にするため、ReadOnly にしない
		Set(value As String)
		End Set
	End Property

	Public Property DescriptionText As String
		Get
			Dim xDoc As XDocument = XDocument.Parse(DescriptionXml)
			Dim text = String.Concat(xDoc.DescendantNodes().OfType(Of XText)().Select(Function(t) t.Value))
			Return text
		End Get
		' SQLite.Net で有効にするため、ReadOnly にしない
		Set(value As String)
		End Set
	End Property

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

		Dim newEntity As New Section_E With {
			.BaseId = BaseId,
			.PubStatus = pubStatus,
			.DocumentId = Document_E.GetIdWithPubStatus(DocumentId, pubStatus),
			.Heading = Heading,
			.DescriptionXml = DescriptionXml,
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

	Public Shared Function GetBaseId(id As String) As String
		Dim delimiterIndex As Integer = id.IndexOf(My.Resources.SuffixDelimiter)
		If delimiterIndex = -1 Then
			Return id
		End If

		Return id.Substring(0, delimiterIndex)
	End Function

End Class