Imports System.IO
Imports System.Text
Imports System.Windows.Markup

Public Class ProcessGroup

	Private ReadOnly Property VM As ProcessGroup_VM
		Get
			Return TryCast(DataContext, ProcessGroup_VM)
		End Get
	End Property

	Private Sub SetDescription(sender As Object, e As RoutedEventArgs)
		Dim xaml As String = VM?.DescriptionXaml
		If String.IsNullOrEmpty(xaml) Then
			Return
		End If

		Dim doc = XamlReader.Parse(xaml)
		FlowDocument.Blocks.Clear()
		For Each a In doc.Blocks
			Dim clone = XamlReader.Parse(XamlWriter.Save(a))
			FlowDocument.Blocks.Add(clone)
		Next
	End Sub

	Private Sub Description_TextChanged(sender As Object, e As TextChangedEventArgs)
		If FlowDocument Is Nothing Then
			Return
		End If
		' FlowDocument を XAML 文字列に変換
		Dim xaml As String
		Using ms As New MemoryStream()
			XamlWriter.Save(FlowDocument, ms)
			ms.Position = 0
			Using sr As New StreamReader(ms)
				xaml = sr.ReadToEnd()
			End Using
		End Using

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

		If VM IsNot Nothing Then
			VM.DescriptionXaml = xaml
			VM.DescriptionHtml = html
		End If
	End Sub
End Class
