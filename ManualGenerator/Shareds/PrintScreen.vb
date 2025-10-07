Imports System.IO
Imports System.Windows.Threading

Public Class PrintScreen

	Private clipboardTimer As DispatcherTimer
	Private lastClipboardImage As BitmapSource

	Public Event ChangedImage(path As String)
	Public Event Stopped()

	Private Shared ReadOnly instance As New PrintScreen()

	Public Shared Function GetInstance() As PrintScreen
		Return instance
	End Function

	Public Sub Start()
		Me.Stop()

		clipboardTimer = New DispatcherTimer With {
			.Interval = TimeSpan.FromMilliseconds(500)
		}
		AddHandler clipboardTimer.Tick, AddressOf CheckClipboard
		clipboardTimer.Start()

		lastClipboardImage = Clipboard.GetImage()

		' Snipping Tool を起動
		Try
			Process.Start("explorer.exe", "ms-screenclip:")
		Catch ex As Exception
			MessageBox.Show("Snipping Toolの起動に失敗しました: " & ex.Message)
		End Try
	End Sub

	Public Sub [Stop]()
		If clipboardTimer IsNot Nothing Then
			clipboardTimer.Stop()
			RemoveHandler clipboardTimer.Tick, AddressOf CheckClipboard
			clipboardTimer = Nothing
		End If

		If StoppedEvent IsNot Nothing Then
			RaiseEvent Stopped()
		End If
	End Sub

	Private Sub CheckClipboard(sender As Object, e As EventArgs)
		Dim img = Clipboard.GetImage()
		' 取得した画像が開始時と異なる場合のみ処理
		If img IsNot Nothing AndAlso (lastClipboardImage Is Nothing OrElse Not IsSameImage(img, lastClipboardImage)) Then
			lastClipboardImage = img
			Dim path As String = IO.Path.Combine(IO.Path.GetTempPath(), $"{Guid.NewGuid()}.png")
			Using fileStream As New FileStream(path, FileMode.Create)
				Dim encoder As New PngBitmapEncoder()
				encoder.Frames.Add(BitmapFrame.Create(img))
				encoder.Save(fileStream)
			End Using

			RaiseEvent ChangedImage(path)
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
