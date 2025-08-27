Imports System.IO
Imports System.Windows.Markup

Public Class Section

	Private ReadOnly Property VM As Section_VM
		Get
			Return TryCast(DataContext, Section_VM)
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

	Private Sub ConfirmedDescription(sender As Object, e As RoutedEventArgs)
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

		' 出力
		Dim html As String = XamlToHtmlConverter.XamlToHtmlConverter.ConvertXamlToHtml(xaml)

		If VM IsNot Nothing Then
			VM.DescriptionXaml = xaml
			VM.DescriptionHtml = html
		End If
	End Sub
End Class
