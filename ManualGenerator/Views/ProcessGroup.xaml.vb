Imports System.IO
Imports System.Text
Imports System.Windows.Markup

Public Class ProcessGroup

	Private ReadOnly Property VM As ProcessGroup_VM
		Get
			Return TryCast(DataContext, ProcessGroup_VM)
		End Get
	End Property

	Private Sub Heading_LostFocus(sender As Object, e As RoutedEventArgs)
		VM?.ChangedHeading(Heading.Text)

		' FlowDocument を XAML 文字列に変換
		Dim xamlText As String
		Using ms As New MemoryStream()
			XamlWriter.Save(FlowDocument, ms)
			ms.Position = 0
			Using sr As New StreamReader(ms)
				xamlText = sr.ReadToEnd()
			End Using
		End Using

		Dim doc As XDocument = XDocument.Parse(xamlText)
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

				sb.Append(System.Net.WebUtility.HtmlEncode(text))

				If hasItalic Then sb.Append("</i>")
				If hasBold Then sb.Append("</b>")
			Next

			sb.AppendLine("</p>")
		Next

		sb.AppendLine("</body>")
		sb.AppendLine("</html>")

		' 出力
		Dim html As String = sb.ToString()
	End Sub
End Class
