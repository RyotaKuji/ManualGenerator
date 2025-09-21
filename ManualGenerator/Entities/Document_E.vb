Imports SQLite

<Table("Publics")>
Public Class Document_E
	<PrimaryKey, NotNull>
	Public Property Id As String = Guid.NewGuid().ToString()

	<NotNull>
	Public Property Title As String

	<Ignore>
	Public Property Sections As IEnumerable(Of Section_E) = {}

	Public Property IsPublic As Boolean
		Get
			Return IsPublicId(Id)
		End Get
		Set(value As Boolean)
		End Set
	End Property

	Public Function GetPublicEntity() As Document_E
		Dim publicDoc As New Document_E With {
			.Id = GetPublicId(),
			.Title = Title,
			.Sections = Sections.Select(Function(s) s.GetPublicEntity()).ToList()
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