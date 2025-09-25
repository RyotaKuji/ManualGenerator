Imports System.IO
Imports System.Windows.Threading

Public Class PrintScreenManager

	Private Shared clipboardTimer As DispatcherTimer
	Private Shared lastClipboardImage As BitmapSource

	Private Shared result As IPrintScreenResult

	Public Shared Sub StartPrintScreen(result As IPrintScreenResult)
		If clipboardTimer IsNot Nothing Then
			clipboardTimer.Stop()
		End If

		PrintScreenManager.result = result

		lastClipboardImage = Clipboard.GetImage()
		clipboardTimer = New DispatcherTimer()
		clipboardTimer.Interval = TimeSpan.FromMilliseconds(500)
		AddHandler clipboardTimer.Tick, AddressOf CheckClipboard
		clipboardTimer.Start()

		' Snipping Tool を起動
		Try
			Process.Start("explorer.exe", "ms-screenclip:")
		Catch ex As Exception
			MessageBox.Show("Snipping Toolの起動に失敗しました: " & ex.Message)
		End Try
	End Sub

	Private Shared Sub CheckClipboard(sender As Object, e As EventArgs)
		Dim img = Clipboard.GetImage()
		' 取得した画像が開始時と異なる場合のみ処理
		If img IsNot Nothing AndAlso (lastClipboardImage Is Nothing OrElse Not IsSameImage(img, lastClipboardImage)) Then
			lastClipboardImage = img
			clipboardTimer.Stop()

			Dim tempPath As String = "C:\WorkSpace\Prog\ManualGenerator\snip_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".png"
			Using fileStream As New FileStream(tempPath, FileMode.Create)
				Dim encoder As New PngBitmapEncoder()
				encoder.Frames.Add(BitmapFrame.Create(img))
				encoder.Save(fileStream)
			End Using

			result.ImagePath = tempPath
		End If

	End Sub

	Private Shared Function IsSameImage(img1 As BitmapSource, img2 As BitmapSource) As Boolean
		If img1 Is Nothing OrElse img2 Is Nothing Then Return False
		If img1.PixelWidth <> img2.PixelWidth OrElse img1.PixelHeight <> img2.PixelHeight Then Return False
		If img1.Format <> img2.Format Then Return False

		Dim stride As Integer = img1.PixelWidth * (img1.Format.BitsPerPixel \ 8)
		Dim pixels1(stride * img1.PixelHeight - 1) As Byte
		Dim pixels2(stride * img2.PixelHeight - 1) As Byte

		img1.CopyPixels(pixels1, stride, 0)
		img2.CopyPixels(pixels2, stride, 0)

		Return pixels1.SequenceEqual(pixels2)
	End Function
End Class
