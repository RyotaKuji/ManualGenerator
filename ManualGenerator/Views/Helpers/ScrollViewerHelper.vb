Public Class ScrollViewerHelper

	Public Shared Sub ScrollToItem(
		sv As ScrollViewer,
		element As FrameworkElement,
		Optional topPadding As Double = 0.0,
		Optional alsoLeft As Boolean = False)

		If sv Is Nothing OrElse element Is Nothing Then Exit Sub

		' 要素の現在位置（ビューポート座標）を取得
		' TransformToAncestor(scrollViewer) は現在のスクロール量を反映した座標を返す
		Dim t As GeneralTransform = element.TransformToAncestor(sv)
		Dim rect As Rect = t.TransformBounds(New Rect(New Point(0, 0), element.RenderSize))

		' 新しい絶対オフセット = 現在のオフセット + 要素の Top（ビューポート座標） - 任意の余白
		Dim targetVertical As Double = sv.VerticalOffset + rect.Top - topPadding

		' 横方向も合わせたい場合（要素の左端を左端に）
		Dim targetHorizontal As Double = sv.HorizontalOffset
		If alsoLeft Then
			targetHorizontal = sv.HorizontalOffset + rect.Left
		End If

		' 範囲クランプ
		Dim maxV As Double = Math.Max(0, sv.ExtentHeight - sv.ViewportHeight)
		Dim maxH As Double = Math.Max(0, sv.ExtentWidth - sv.ViewportWidth)
		targetVertical = Math.Max(0, Math.Min(targetVertical, maxV))
		targetHorizontal = Math.Max(0, Math.Min(targetHorizontal, maxH))

		' 即時スクロール
		sv.ScrollToVerticalOffset(targetVertical)
		If alsoLeft Then sv.ScrollToHorizontalOffset(targetHorizontal)
	End Sub

End Class
