Imports System.IO
Imports System.Text

Public Class WebDocManager

	Public Async Function PublishAsync(id As String) As Task(Of String)
		Try
			' php.exe を起動
			Dim psi As New ProcessStartInfo() With {
				.FileName = My.Resources.PHPExePath,
				.Arguments = $"""{My.Resources.PHPScriptPath}"" ""{id}""",
				.RedirectStandardOutput = True,
				.RedirectStandardError = True,
				.UseShellExecute = False,
				.CreateNoWindow = True,
				.StandardOutputEncoding = Encoding.UTF8,
				.StandardErrorEncoding = Encoding.UTF8
			}

			Dim htmlContent As String = ""
			Using proc As Process = Process.Start(psi)
				htmlContent = Await proc.StandardOutput.ReadToEndAsync()
				Dim err = Await proc.StandardError.ReadToEndAsync()
				proc.WaitForExit()
				If Not String.IsNullOrEmpty(err) Or
					String.IsNullOrEmpty(htmlContent) Then
					Return ""
				End If
			End Using

			Dim documentPath = GetDocumentPath(id)
			File.WriteAllText(documentPath, htmlContent, Encoding.UTF8)

			Return documentPath

		Catch ex As Exception
			Return ""
		End Try
	End Function

	Public Shared Function GetDocumentPath(id As String) As String
		Return Path.Combine(My.Resources.PublicDocDir, $"{id}.html")
	End Function

End Class
