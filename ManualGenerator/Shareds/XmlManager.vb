Public Class XmlManager

	Public IdToXml As New Dictionary(Of String, String)

	Public Sub ChangeSectionHeading(id As String, oldText As String, newText As String)

	End Sub

	Private Shared instance As New XmlManager()

	Public Shared Function GetInstance() As XmlManager
		Return instance
	End Function

End Class
