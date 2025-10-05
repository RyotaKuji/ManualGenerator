Imports System.Text.RegularExpressions

Public Class XmlManager

	Public Sections As New HashSet(Of Section_VM)

	Public Sub ChangeSectionHeading(id As String, oldText As String, newText As String)
		If String.IsNullOrEmpty(id) Or
			String.IsNullOrEmpty(oldText) Or
			String.IsNullOrEmpty(newText) Then
			Return
		End If
		id = Section_E.GetBaseId(id)
		For Each section As Section_VM In Sections
			Dim xml As String = section.DescriptionXml
			Dim pattern As String = $"(<Hyperlink[^>]*NavigateUri=""#{Regex.Escape(id)}""[^>]*>){Regex.Escape(oldText)}(</Hyperlink>)"

			Dim replaced As String = Regex.Replace(xml, pattern, $"$1{newText}$2", RegexOptions.IgnoreCase Or RegexOptions.Singleline)

			If xml <> replaced Then
				section.DescriptionXml = replaced
			End If
		Next
	End Sub

	Public Sub RemovedSection(id As String)
		If String.IsNullOrEmpty(id) Then
			Return
		End If
		id = Section_E.GetBaseId(id)

		For Each section As Section_VM In Sections
			Dim xml As String = section.DescriptionXml
			If xml Is Nothing Then
				Continue For
			End If

			' Hyperlink を見つけて、Foreground="Red" を付加する
			Dim pattern As String = $"(<Hyperlink[^>]*NavigateUri=""#{Regex.Escape(id)}""[^>]*)(>)"
			Dim replaced As String = Regex.Replace(xml, pattern, "$1 Foreground=""Red""$2", RegexOptions.IgnoreCase Or RegexOptions.Singleline)

			If xml <> replaced Then
				section.DescriptionXml = replaced
			End If
		Next
	End Sub

	Private Shared instance As New XmlManager()

	Public Shared Function GetInstance() As XmlManager
		Return instance
	End Function

End Class
