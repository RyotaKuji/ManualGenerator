Imports SQLite

<Table("Documents")>
Public Class Document_E

	<PrimaryKey, NotNull>
	Public Property Id As String
		Get
			Return BaseId & If(IsPublic, My.Resources.PublicSuffix, My.Resources.DraftSuffix)
		End Get
		Set(value As String)
			BaseId = GetBaseId(value)
		End Set
	End Property

	<Ignore>
	Private Property BaseId As String = Guid.NewGuid().ToString()

	<NotNull>
	Public Property Title As String

	<Ignore>
	Public Property Sections As IEnumerable(Of Section_E) = {}

	Public Property IsPublic As Boolean

	Public Function Clone(isPublic As Boolean)
		If Me.IsPublic = isPublic Then
			Return Me
		End If

		Dim newEntity As New Document_E With {
			.BaseId = BaseId,
			.IsPublic = isPublic,
			.Title = Title,
			.Sections = Sections.Select(Function(s) s.Clone(isPublic)).ToList()
		}
		Return newEntity
	End Function

	Public Shared Function GetIdWithPubStatus(id As String, isPublic As Boolean) As String
		Dim baseId As String = id.Substring(0, id.IndexOf(My.Resources.SuffixDelimiter))
		Dim suffix As String = If(isPublic, My.Resources.PublicSuffix, My.Resources.DraftSuffix)
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