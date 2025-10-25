Imports System.Windows.Markup

Public Class RichTextBoxHelper

	Public Shared ReadOnly BindableDocumentProperty As DependencyProperty =
		DependencyProperty.RegisterAttached(
			"BindableDocument",
			GetType(String),
			GetType(RichTextBoxHelper),
			New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AddressOf OnBindableDocumentChanged))

	Public Shared Sub SetBindableDocument(element As DependencyObject, value As String)
		element.SetValue(BindableDocumentProperty, value)
	End Sub

	Public Shared Function GetBindableDocument(element As DependencyObject) As String
		Return CStr(element.GetValue(BindableDocumentProperty))
	End Function

	''' <summary>
	''' XML文字列変更時に FlowDocument を構築して RichTextBox に設定
	''' </summary>
	Private Shared Sub OnBindableDocumentChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
		Dim rtb = TryCast(d, RichTextBox)
		If rtb Is Nothing Then Return
		Dim xml As String = TryCast(e.NewValue, String)
		SetFlowDocument(xml, rtb.Document)
	End Sub

	Private Shared Function SetFlowDocument(xml As String, doc As FlowDocument) As FlowDocument
		' 空の場合は最低1つの Paragraph を入れた空ドキュメントを返す
		If String.IsNullOrWhiteSpace(xml) Then
			Dim emptyDoc As New FlowDocument()
			emptyDoc.Blocks.Add(New Paragraph())
			Return emptyDoc
		End If

		Try
			' 文字列から FlowDocument を読み込む
			Dim parsed = TryCast(XamlReader.Parse(xml), FlowDocument)
			If parsed Is Nothing Then
				Return New FlowDocument(New Paragraph(New Run("[FlowDocument パース失敗]")))
			End If

			doc.Blocks.Clear()
			For Each block As Block In parsed.Blocks
				Dim clone As Block = CType(XamlReader.Parse(XamlWriter.Save(block)), Block)
				doc.Blocks.Add(clone)
			Next
			Return doc

		Catch ex As Exception
			' エラー時にはメッセージ付きの FlowDocument を返す
			Dim errorDoc As New FlowDocument()
			errorDoc.Blocks.Add(New Paragraph(New Run($"[変換エラー: {ex.Message}]")))
			Return errorDoc
		End Try
	End Function
End Class