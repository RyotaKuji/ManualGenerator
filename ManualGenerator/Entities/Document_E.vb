Imports SQLite

<Table("Documents")>
Public Class Document_E

	<PrimaryKey, NotNull>
	Public Property Id As String
		Get
			Return GetIdWithPubStatus(BaseId, PubStatus)
		End Get
		Set(value As String)
			BaseId = GetBaseId(value)
		End Set
	End Property

	Public Property BaseId As String = Guid.NewGuid().ToString()

	<NotNull>
	Public Property Title As String

	Public Property Author As String

	Public Property Department As String

	<Ignore>
	Public Property Sections As IEnumerable(Of Section_E) = {}

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

	Public Function Clone(pubStatus As Definitions.PubStatus) As Document_E

		Dim newEntity As New Document_E With {
			.BaseId = BaseId,
			.PubStatus = pubStatus,
			.Title = Title,
			.Author = Author,
			.Department = Department,
			.Sections = Sections.Select(Function(s) s.Clone(pubStatus)).ToList()
		}
		Return newEntity
	End Function

	Public Shared Function GetIdWithPubStatus(id As String, pubStatus As Definitions.PubStatus) As String
		Dim baseId As String = GetBaseId(id)
		Dim suffix As String = GetSuffix(pubStatus)
		Return baseId & suffix
	End Function

	Public Shared Function GetBaseId(id As String) As String
		Dim delimiterIndex As Integer = id.IndexOf(My.Resources.SuffixDelimiter)
		If delimiterIndex = -1 Then
			Return id
		End If

		Return id.Substring(0, delimiterIndex)
	End Function

	Private Shared Function GetSuffix(pubStatus As Definitions.PubStatus) As String
		Return My.Resources.SuffixDelimiter & pubStatus.ToString()
	End Function

End Class