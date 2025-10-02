Imports System.Text

Public Class FlowDocumentToHtmlConverter

	Private Shared ReadOnly ns As XNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"

	Public Shared Function Convert(flowDocXml As String) As String
		Dim doc As XDocument = XDocument.Parse(flowDocXml)
		Dim sb As New StringBuilder()

		For Each node In doc.Root.Elements()
			Dim s = ConvertNode(node)
			sb.Append(ConvertNode(node))
		Next

		Return sb.ToString()
	End Function

	Private Shared Function ConvertNode(node As XElement) As String
		Select Case node.Name.LocalName
			Case "Paragraph"
				Return $"<p{MakeStyle(node)}>{ConvertChildren(node)}</p>"

			Case "Run"
				Dim text As String = node.Value
				Return $"<span{MakeStyle(node)}>{Net.WebUtility.HtmlEncode(text)}</span>"

			Case "Hyperlink"
				Dim href As String = node.Attribute("NavigateUri")?.Value
				Return $"<a href=""{href}""{MakeStyle(node)}>{ConvertChildren(node)}</a>"

			Case Else
				Return ConvertChildren(node)
		End Select
	End Function

	Private Shared Function ConvertChildren(node As XElement) As String
		Dim sb As New StringBuilder()
		For Each child In node.Nodes()
			If TypeOf child Is XElement Then
				sb.Append(ConvertNode(DirectCast(child, XElement)))
			ElseIf TypeOf child Is XText Then
				sb.Append(Net.WebUtility.HtmlEncode(child.ToString()))
			End If
		Next
		Return sb.ToString()
	End Function

	Private Shared Function MakeStyle(node As XElement) As String
		Dim styles As New List(Of String)

		If node.Attribute("FontFamily") IsNot Nothing Then
			styles.Add($"font-family:{node.Attribute("FontFamily").Value}")
		End If
		If node.Attribute("FontSize") IsNot Nothing Then
			styles.Add($"font-size:{node.Attribute("FontSize").Value}px")
		End If
		If node.Attribute("FontWeight") IsNot Nothing Then
			Dim fw = node.Attribute("FontWeight").Value.ToLower()
			styles.Add($"font-weight:{fw}")
		End If
		If node.Attribute("FontStyle") IsNot Nothing Then
			styles.Add($"font-style:{node.Attribute("FontStyle").Value.ToLower()}")
		End If
		If node.Attribute("Foreground") IsNot Nothing Then
			styles.Add($"color:{ConvertColor(node.Attribute("Foreground").Value)}")
		End If
		If node.Attribute("Background") IsNot Nothing AndAlso node.Attribute("Background").Value <> "{x:Null}" Then
			styles.Add($"background-color:{ConvertColor(node.Attribute("Background").Value)}")
		End If
		If node.Attribute("LineHeight") IsNot Nothing Then
			styles.Add($"line-height:{node.Attribute("LineHeight").Value}")
		End If
		If node.Attribute("PagePadding") IsNot Nothing Then
			styles.Add($"padding:{node.Attribute("PagePadding").Value}")
		End If

		' ---- TextDecorations の処理 ----
		Dim decoList As New List(Of String)
		Dim textDecorationsElement As XElement = Nothing
		Select Case node.Name.LocalName
			Case "Run"
				textDecorationsElement = node.Element(ns + "Run.TextDecorations")
			Case "Hyperlink"
				textDecorationsElement = node.Element(ns + "Hyperlink.TextDecorations")
		End Select
		If textDecorationsElement IsNot Nothing Then
			For Each deco In textDecorationsElement.Elements(ns + "TextDecoration")
				Dim loc = deco.Attribute("Location")?.Value
				If Not String.IsNullOrEmpty(loc) Then
					Select Case loc.ToLower()
						Case "underline"
							decoList.Add("underline")
						Case "strikethrough"
							decoList.Add("line-through")
						Case "overline"
							decoList.Add("overline")
					End Select
				End If
			Next
		End If
		If decoList.Count > 0 Then
			styles.Add("text-decoration:" & String.Join(" ", decoList))
		End If
		' ------------------------------

		If styles.Count > 0 Then
			Return $" style=""{String.Join(";", styles)}"""
		End If
		Return ""
	End Function

	Private Shared Function ConvertColor(value As String) As String
		If value.StartsWith("#") AndAlso value.Length = 9 Then
			Return "#" & value.Substring(3) ' #AARRGGBB → #RRGGBB
		End If
		Return value
	End Function

End Class
