Imports System.IO
Imports System.Text
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
		Dim html As String = FlowDocumentToHtmlConverter.FlowDocumentToHtmlConverter.Convert(xaml)

		If VM IsNot Nothing Then
			VM.DescriptionXaml = xaml
			VM.DescriptionHtml = html
		End If
	End Sub

	Private Sub Command_Hyperlink(sender As Object, e As RoutedEventArgs)
		If DescriptionEditor Is Nothing Then Return

		Dim textSelection As TextSelection = DescriptionEditor.Selection

		Dim pointerStart As TextPointer = textSelection.Start
		Dim pointerEnd As TextPointer = textSelection.End

		Dim selectedElements As IEnumerable(Of TextElement) = GetElementsFromSelection(textSelection)

		' 選択範囲に HyperLink が含まれている場合は、（先頭の）HyperLink を編集
		Dim firstHyperlink As Hyperlink = selectedElements.OfType(Of Hyperlink)().FirstOrDefault()
		If firstHyperlink IsNot Nothing Then
			pointerStart = firstHyperlink.ElementStart
			pointerEnd = firstHyperlink.ElementEnd
			textSelection.Select(pointerStart, pointerEnd)
			EditHyperlink(firstHyperlink)
		Else
			' 複数段落にまたがる選択は、開始位置から段落末尾までに変更
			If pointerStart.Paragraph IsNot pointerEnd.Paragraph Then
				pointerEnd = pointerStart.Paragraph.ElementEnd
				textSelection.Select(pointerStart, pointerEnd)
			End If

			InsertHyperlink(textSelection)

		End If

	End Sub

	Private Sub InsertHyperlink(selection As TextSelection)

		Dim dialog As New HyperlinkDialog(selection.Text) With {
			.Owner = Window.GetWindow(Me),
			.WindowStartupLocation = WindowStartupLocation.CenterOwner
		}

		Dim dialogResult As Boolean? = dialog.ShowDialog()

		If dialogResult = False Then
			Return
		End If

		Dim uri As String = dialog.Uri
		Dim displayText As String = dialog.DisplayText

		' 選択範囲の要素を取得
		Dim selectedElements As IEnumerable(Of TextElement) = GetElementsFromSelection(selection)

		' 変更対象の Paragraph
		Dim targetParagraph As Paragraph = selection.Start.Paragraph

		' Paragraph が存在しない場合は追加
		If targetParagraph Is Nothing Then
			Dim newParagraph As New Paragraph()
			DescriptionEditor.Document.Blocks.Add(newParagraph)
			selection.Select(newParagraph.ContentStart, newParagraph.ContentStart)

			targetParagraph = newParagraph
		End If

		' Start

		' 選択開始位置の Run
		Dim borderRun_start As Run = TryCast(selection.Start.Parent, Run)
		' 選択開始位置の Run
		Dim borderRun_end As Run = TryCast(selection.End.Parent, Run)

		' 選択開始位置以前のテキスト
		Dim previousText As String = selection.Start.GetTextInRun(LogicalDirection.Backward)
		' 選択開始位置以前のテキスト
		Dim nextText As String = selection.End.GetTextInRun(LogicalDirection.Forward)

		If borderRun_start Is borderRun_end And borderRun_start IsNot Nothing Then
			' 選択範囲が1つのRun内にある場合

			borderRun_start.Text = previousText

			' end は新規作成
			Dim newRun_end As New Run(nextText)
			CopyProperties(newRun_end, borderRun_start)
			targetParagraph.Inlines.InsertAfter(borderRun_start, newRun_end)
			borderRun_end = newRun_end
		Else
			' 選択範囲が複数のRunにまたがる場合

			If borderRun_start IsNot Nothing Then
				borderRun_start.Text = previousText
			End If

			If borderRun_end IsNot Nothing Then
				borderRun_end.Text = nextText
			End If
		End If

		Dim hyperlink As Hyperlink = CreateHyperlink(uri, displayText)
		' 前後のプロパティをコピーして挿入
		If borderRun_start IsNot Nothing Then
			CopyProperties(hyperlink, borderRun_start)
			targetParagraph.Inlines.InsertAfter(borderRun_start, hyperlink)
		ElseIf borderRun_end IsNot Nothing Then
			CopyProperties(hyperlink, borderRun_end)
			targetParagraph.Inlines.InsertBefore(borderRun_end, hyperlink)
		Else
			targetParagraph.Inlines.Add(hyperlink)
		End If

		For Each elem As TextElement In selectedElements
			Dim run As Run = TryCast(elem, Run)
			If run IsNot Nothing Then
				If (run IsNot borderRun_start And
					run IsNot borderRun_end) Or
					run.Text Is String.Empty Then
					targetParagraph.Inlines.Remove(run)
				End If
			End If
		Next
	End Sub

	Private Function CreateHyperlink(uri As String, text As String) As Hyperlink

		Dim hyperlink As New Hyperlink(New Run(text)) With {
			.NavigateUri = New Uri(uri)
		}
		AddHandler hyperlink.RequestNavigate, AddressOf OpenLink
		Return hyperlink

	End Function

	Private Sub EditHyperlink(hyperLink As Hyperlink)

		Dim text As String = GetInlineText(hyperLink.Inlines)

		Dim dialog As New HyperlinkDialog(hyperLink.NavigateUri.AbsoluteUri, text) With {
			.Owner = Window.GetWindow(Me),
			.WindowStartupLocation = WindowStartupLocation.CenterOwner
		}

		Dim dialogResult As Boolean = dialog.ShowDialog()

		If dialogResult = False Then
			Return
		End If

		hyperLink.NavigateUri = New Uri(dialog.Uri)
		hyperLink.Inlines.Clear()
		hyperLink.Inlines.Add(New Run(dialog.DisplayText))
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

	Private Shared Function GetElementsFromSelection(selection As TextSelection) As IEnumerable(Of TextElement)
		Dim pointerStart As TextPointer = selection.Start
		Dim pointerEnd As TextPointer = selection.End

		Dim elements As New HashSet(Of TextElement)()

		' ポインタを進めながら走査
		Dim navigator As TextPointer = pointerStart

		While navigator IsNot Nothing AndAlso navigator.CompareTo(pointerEnd) <= 0

			If TypeOf navigator.Parent IsNot TextElement Then
				' 親が TextElement でない場合はスキップ
				navigator = navigator.GetNextContextPosition(LogicalDirection.Forward)
				Continue While
			End If

			Dim elem As TextElement = navigator.Parent
			While TypeOf elem.Parent IsNot Paragraph AndAlso
				  TypeOf elem.Parent IsNot FlowDocument
				elem = elem.Parent
			End While
			If TypeOf elem Is Inline Or
				TypeOf elem Is Hyperlink Then
				elements.Add(elem)
			End If

			' 次の位置へ
			navigator = navigator.GetNextContextPosition(LogicalDirection.Forward)
		End While

		Return elements
	End Function

	Private Function GetInlineText(textElement As InlineCollection) As String
		Dim sb As New StringBuilder()
		AppendInlineText(textElement, sb)
		Return sb.ToString()
	End Function

	Private Sub AppendInlineText(inlines As InlineCollection, sb As StringBuilder)
		For Each inline As Inline In inlines
			If TypeOf inline Is Run Then
				sb.Append(DirectCast(inline, Run).Text)
			ElseIf TypeOf inline Is Span Then
				AppendInlineText(DirectCast(inline, Span).Inlines, sb)
			End If
		Next
	End Sub

	Private Sub OpenLink(sender As Object, e As RequestNavigateEventArgs)
		Dim hyperlink As Hyperlink = TryCast(e.OriginalSource, Hyperlink)
		If hyperlink IsNot Nothing AndAlso hyperlink.NavigateUri IsNot Nothing Then
			Process.Start(New ProcessStartInfo(hyperlink.NavigateUri.AbsoluteUri) With {.UseShellExecute = True})
			e.Handled = True
		End If
	End Sub
End Class
