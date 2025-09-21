Imports SQLite

<Table("Sections")>
Public Class Section_E
	<PrimaryKey>
	Public Property Id As String = Guid.NewGuid().ToString()

	<Indexed>
	Public Property DocumentId As String

	Public Property Heading As String
	Public Property DescriptionXaml As String
	Public Property DescriptionHtml As String
	Public Property DescriptionText As String
	Public Property ImagePath As String
	Public Property IsHeadline As Boolean
	Public Property OrderIndex As Integer

	Public Property IsPublic As Boolean
		Get
			Return IsPublicId(Id)
		End Get
		Set(value As Boolean)
		End Set
	End Property

	Public Function GetPublicEntity() As Section_E
		Dim publicDoc As New Section_E With {
			.Id = GetPublicId(),
			.DocumentId = Document_E.GetPublicId(DocumentId),
			.Heading = Heading,
			.DescriptionXaml = DescriptionXaml,
			.DescriptionHtml = DescriptionHtml,
			.DescriptionText = DescriptionText,
			.ImagePath = ImagePath,
			.IsHeadline = IsHeadline,
			.OrderIndex = OrderIndex
		}
		Return publicDoc
	End Function

	Public Function GetPublicId() As String
		Return GetPublicId(Id)
	End Function

	Public Function GetDraftId() As String
		Return GetDraftId(Id)
	End Function

	Public Shared Function GetPublicId(id As String) As String
		If IsPublicId(id) Then
			Return id
		Else
			Return id & My.Resources.PublicSuffix
		End If
	End Function

	Public Shared Function GetDraftId(id As String) As String
		If IsPublicId(id) Then
			Return id.TrimEnd(My.Resources.PublicSuffix)
		Else
			Return id
		End If
	End Function

	Public Shared Function IsPublicId(id As String) As Boolean
		Return id.EndsWith(My.Resources.PublicSuffix)
	End Function
End Class