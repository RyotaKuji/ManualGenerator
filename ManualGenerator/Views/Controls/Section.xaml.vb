Imports System.Text
Imports System.Windows.Markup

Public Class Section

	Private ReadOnly Property VM As Section_VM
		Get
			Return TryCast(DataContext, Section_VM)
		End Get
	End Property

	Public Sub New()
		InitializeComponent()
		AddHandler Loaded, AddressOf Section_Loaded
		AddHandler DescriptionEditor.Loaded, AddressOf DescriptionEditor_Loaded
	End Sub

	Private Sub Section_Loaded(sender As Object, e As RoutedEventArgs)
		AddHandler VM.ScrollToSelfEvent, AddressOf ScrollToSelf
	End Sub

	Private Sub DescriptionEditor_Loaded(sender As Object, e As RoutedEventArgs)
		With DescriptionEditor.Document
			.FontSize = 16
			.LineHeight = 1
			.FontFamily = New FontFamily("Noto Sans JP")
		End With
		AddHandler DescriptionEditor.LostFocus, AddressOf ConfirmDescription
		AddEventToHyperlinks()
	End Sub

	Private Sub ConfirmDescription(sender As Object, e As RoutedEventArgs)
		Dim range As New TextRange(DescriptionEditor.Document.ContentStart, DescriptionEditor.Document.ContentEnd)
		RichTextBoxHelper.SetBindableDocument(DescriptionEditor, XamlWriter.Save(DescriptionEditor.Document))
		AddEventToHyperlinks()
	End Sub

	Private Sub AddEventToHyperlinks()
		If DescriptionEditor Is Nothing Then Return
		Dim doc As FlowDocument = DescriptionEditor.Document
		Dim hyperlinks As List(Of Hyperlink) = GetAllHyperlinks(doc)
		For Each link As Hyperlink In hyperlinks
			RemoveHandler link.RequestNavigate, AddressOf OpenLink
			AddHandler link.RequestNavigate, AddressOf OpenLink
		Next
	End Sub

	Private Function GetAllHyperlinks(doc As FlowDocument) As List(Of Hyperlink)
		Dim list As New List(Of Hyperlink)
		FindHyperLinkFromLogicalTree(doc, list)
		Return list
	End Function

	Private Sub FindHyperLinkFromLogicalTree(obj As Object, list As List(Of Hyperlink))
		If TypeOf obj IsNot DependencyObject Then
			Return
		End If

		For Each child As Object In LogicalTreeHelper.GetChildren(CType(obj, DependencyObject))
			If TypeOf child Is Hyperlink Then
				list.Add(DirectCast(child, Hyperlink))
			End If
			FindHyperLinkFromLogicalTree(child, list)
		Next
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

		' 選択開始位置の Run
		Dim borderRun_start As Run = TryCast(selection.Start.Parent, Run)
		' 選択終了位置の Run
		Dim borderRun_end As Run = TryCast(selection.End.Parent, Run)

		' 選択開始位置以前のテキスト
		Dim previousText As String = selection.Start.GetTextInRun(LogicalDirection.Backward)
		' 選択終了位置以降のテキスト
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
			CopyProperties(hyperlink, DescriptionEditor.Document)
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

		Dim a As New Uri(uri, UriKind.RelativeOrAbsolute)

		Dim hyperlink As New Hyperlink(New Run(text)) With {
			.NavigateUri = New Uri(uri, UriKind.RelativeOrAbsolute)
		}
		AddHandler hyperlink.RequestNavigate, AddressOf OpenLink
		Return hyperlink

	End Function

	Private Sub EditHyperlink(hyperLink As Hyperlink)

		Dim text As String = GetInlineText(hyperLink.Inlines)

		Dim dialog As New HyperlinkDialog(hyperLink.NavigateUri.OriginalString, text) With {
			.Owner = Window.GetWindow(Me),
			.WindowStartupLocation = WindowStartupLocation.CenterOwner
		}

		Dim dialogResult As Boolean? = dialog.ShowDialog()

		If dialogResult Is Nothing OrElse
			dialogResult = False Then
			Return
		End If

		Dim a As Uri = New Uri(dialog.Uri, UriKind.RelativeOrAbsolute)
		hyperLink.NavigateUri = New Uri(dialog.Uri, UriKind.RelativeOrAbsolute)
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

	Private Shared Sub CopyProperties(destination As Inline, source As FlowDocument)
		With destination
			.FontFamily = source.FontFamily
			.FontSize = source.FontSize
			.FontStretch = source.FontStretch
			.FontStyle = source.FontStyle
			.FontWeight = source.FontWeight
			.Background = source.Background
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

			Dim elem As TextElement = CType(navigator.Parent, TextElement)
			While TypeOf elem.Parent IsNot Paragraph AndAlso
				  TypeOf elem.Parent IsNot FlowDocument
				elem = CType(elem.Parent, TextElement)
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

			Dim uri As Uri = hyperlink.NavigateUri

			If uri.IsAbsoluteUri Then
				Process.Start(New ProcessStartInfo(hyperlink.NavigateUri.AbsoluteUri) With {.UseShellExecute = True})
			Else
				VM.RequestScrollCommand.Execute(uri)
			End If
			e.Handled = True
		End If
	End Sub

	Private Sub ScrollToSelf()
		BringIntoView()
	End Sub

End Class
