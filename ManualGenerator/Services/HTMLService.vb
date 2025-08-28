Imports System.IO

Public Class HTMLService

	' 実行するPHPファイルのパス
	Private Const phpFilePath As String = "C:\Users\raytrek\Downloads\hello.php"
	' PHP実行ファイルのパス（環境に合わせて修正）
	Private Const phpExePath As String = "php"

	Public Shared Function GetFullHTML(jsonFilePath As String) As String

		Dim psi As New ProcessStartInfo With {
			.FileName = phpExePath,
			.Arguments = $"""{phpFilePath}"" ""{jsonFilePath}""",
			.RedirectStandardOutput = True,
			.UseShellExecute = False,
			.CreateNoWindow = True
		}

		Using process As Process = Process.Start(psi)
			Using reader As StreamReader = process.StandardOutput
				process.WaitForExit()
				Dim result As String = reader.ReadToEnd()
				Return result
			End Using
		End Using
	End Function
End Class
