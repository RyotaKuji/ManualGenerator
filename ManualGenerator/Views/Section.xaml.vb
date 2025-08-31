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

		If String.IsNullOrEmpty(xaml) Then Return

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

	Private Sub InsertLink(sender As Object, e As RoutedEventArgs)
		If DescriptionEditor Is Nothing Then Return

		Dim textSelection As TextSelection = DescriptionEditor.Selection

		Dim selectionStart As TextPointer = textSelection.Start
		Dim selectionEnd As TextPointer = textSelection.End

		' 複数段落にまたがる選択は、開始位置から段落末尾までに変更
		If selectionStart.Paragraph IsNot selectionEnd.Paragraph Then
			selectionEnd = selectionStart.Paragraph.ElementEnd
			textSelection.Select(selectionStart, selectionEnd)
		End If

		Dim dialog As New LinkDialog(textSelection.Text) With {
			.Owner = Window.GetWindow(Me),
			.WindowStartupLocation = WindowStartupLocation.CenterOwner
		}

		Dim dialogResult As Boolean? = dialog.ShowDialog()

		If dialogResult <> True Then
			Return
		End If

		Dim link As String = dialog.Link
		Dim displayText As String = dialog.DisplayText

		Dim selectedInlines As IEnumerable(Of Inline) = GetInlinesFromSelection(DescriptionEditor)

		' Start
		Dim runAtStart As Run = TryCast(selectionStart.Parent, Run)
		Dim previousText As String = selectionStart.GetTextInRun(LogicalDirection.Backward)
		Dim newRunAtStart As New Run(previousText)
		If runAtStart IsNot Nothing Then
			CopyProperties(newRunAtStart, runAtStart)
		End If

		Dim paragraphAtStart As Paragraph
		If runAtStart IsNot Nothing Then
			paragraphAtStart = TryCast(runAtStart.Parent, Paragraph)
		Else
			runAtStart = New Run()
			paragraphAtStart = New Paragraph(runAtStart)
			DescriptionEditor.Document.Blocks.Add(paragraphAtStart)
		End If

		paragraphAtStart.Inlines.InsertBefore(runAtStart, newRunAtStart)

		' End
		Dim runAtEnd As Run = TryCast(selectionEnd.Parent, Run)
		If runAtEnd IsNot Nothing Then
			Dim nextText As String = selectionEnd.GetTextInRun(LogicalDirection.Forward)
			Dim newRunAtEnd As New Run(nextText)
			CopyProperties(newRunAtEnd, runAtEnd)

			Dim paragraphAtEnd As Paragraph = TryCast(runAtEnd.Parent, Paragraph)
			paragraphAtEnd?.Inlines.InsertAfter(runAtEnd, newRunAtEnd)

		End If

		' Hyperlink
		Dim hyperlink As New Hyperlink(New Run(displayText)) With {
			.NavigateUri = New Uri(link)
		}
		AddHandler hyperlink.RequestNavigate, AddressOf OpenLink
		CopyProperties(hyperlink, runAtStart)

		paragraphAtStart.Inlines.InsertAfter(newRunAtStart, hyperlink)

		' 削除
		For Each inline As Inline In selectedInlines
			Dim parentParagraph As Paragraph = TryCast(inline.Parent, Paragraph)
			parentParagraph?.Inlines.Remove(inline)
		Next

		DescriptionEditor.CaretPosition = hyperlink.ElementEnd
	End Sub

	Private Shared Sub CopyProperties(destination As Inline, source As Inline)
		With destination
			.FontFamily = source.FontFamily
			.FontSize = source.FontSize
			.FontStretch = source.FontStretch
			.FontStyle = source.FontStyle
			.FontWeight = source.FontWeight
			.Background = source.Background
			If destination.GetType() Is source.GetType() Then
				.Foreground = source.Foreground
				.TextDecorations = source.TextDecorations
			End If
		End With
	End Sub

	Private Shared Function GetInlinesFromSelection(rtb As RichTextBox) As IEnumerable(Of Inline)
		Dim selection As TextSelection = rtb.Selection
		Dim pointerStart As TextPointer = selection.Start
		Dim pointerEnd As TextPointer = selection.End

		Dim inlines As New HashSet(Of Inline)()

		' ポインタを進めながら走査
		Dim navigator As TextPointer = pointerStart

		While navigator IsNot Nothing AndAlso navigator.CompareTo(pointerEnd) <= 0
			' 今の位置の Inline を取得
			Dim inline As Inline = TryCast(navigator.Parent, Inline)
			If inline IsNot Nothing Then
				inlines.Add(inline)
			End If

			' 次の位置へ
			navigator = navigator.GetNextContextPosition(LogicalDirection.Forward)
		End While

		Return inlines
	End Function

	Private Sub OpenLink(sender As Object, e As RequestNavigateEventArgs)
		Dim hyperlink As Hyperlink = TryCast(e.OriginalSource, Hyperlink)
		If hyperlink IsNot Nothing AndAlso hyperlink.NavigateUri IsNot Nothing Then
			Process.Start(New ProcessStartInfo(hyperlink.NavigateUri.AbsoluteUri) With {.UseShellExecute = True})
			e.Handled = True
		End If
	End Sub
End Class
