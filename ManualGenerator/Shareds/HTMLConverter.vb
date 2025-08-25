Imports System.Text

Public Class HTMLConverter
	Public Shared Function ConvertToHTML(xaml As String) As String

		Dim doc As XDocument = XDocument.Parse(xaml)
		Dim ns As XNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation"

		Dim sb As New StringBuilder()
		sb.AppendLine("<html>")
		sb.AppendLine("<body>")

		' Paragraph を処理
		For Each para In doc.Root.Elements(ns + "Paragraph")
			sb.Append("<p>")

			' Run を処理
			For Each run In para.Elements(ns + "Run")
				Dim text As String = run.Value
				Dim hasBold As Boolean = (run.Attribute("FontWeight") IsNot Nothing AndAlso run.Attribute("FontWeight").Value = "Bold")
				Dim hasItalic As Boolean = (run.Attribute("FontStyle") IsNot Nothing AndAlso run.Attribute("FontStyle").Value = "Italic")

				If hasBold Then sb.Append("<b>")
				If hasItalic Then sb.Append("<i>")

				sb.Append(Net.WebUtility.HtmlEncode(text))

				If hasItalic Then sb.Append("</i>")
				If hasBold Then sb.Append("</b>")
			Next

			sb.AppendLine("</p>")
		Next

		sb.AppendLine("</body>")
		sb.AppendLine("</html>")

		' 出力
		Dim html As String = sb.ToString()

		Return html
	End Function
End Class
