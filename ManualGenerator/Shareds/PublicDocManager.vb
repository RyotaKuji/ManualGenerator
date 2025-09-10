Imports System.IO
Imports System.Text
Imports Newtonsoft.Json

Public Class PublicDocManager : Implements IDisposable

	Private ReadOnly repo As New PublicDoc_R

	Public Async Function PublishAsync(entity As Document_E) As Task(Of String)
		Try
			repo.CreateOrUpdate(entity)

			' JSON を一時ファイルに保存
			Dim json As String = JsonConvert.SerializeObject(entity)
			Dim tempJsonPath As String = Path.GetTempFileName()
			File.WriteAllText(tempJsonPath, json, Encoding.UTF8)

			' php.exe を起動
			Dim psi As New ProcessStartInfo() With {
				.FileName = My.Resources.PHPExePath,
				.Arguments = $"""{My.Resources.PHPScriptPath}"" ""{tempJsonPath}""",
				.RedirectStandardOutput = True,
				.RedirectStandardError = True,
				.UseShellExecute = False,
				.CreateNoWindow = True
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

			' 一時ファイル削除
			File.Delete(tempJsonPath)

			Dim documentPath = GetDocumentPath(entity.Id)
			File.WriteAllText(documentPath, htmlContent, Encoding.UTF8)

			Return documentPath

		Catch ex As Exception
			Return ""
		End Try
	End Function

	Public Shared Function GetDocumentPath(id As String) As String
		Return Path.Combine(My.Resources.PublicDocDir, $"{id}.html")
	End Function

	Public Sub Dispose() Implements IDisposable.Dispose
		repo.Dispose()
	End Sub

End Class
