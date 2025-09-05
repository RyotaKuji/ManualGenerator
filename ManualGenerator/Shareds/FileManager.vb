Imports System.IO
Imports System.Text
Public Class FileManager

	Private Const DataFileBasePath As String = Configuration.DataFileBasePath
	Private Const DataFileExtension As String = "json"

	Public Shared Function GetFilePathById(id As String) As String
		Return Path.Combine(DataFileBasePath, $"{id}.{DataFileExtension}")
	End Function

	Public Shared Function GetId(path As String) As String
		Return IO.Path.GetFileNameWithoutExtension(path)
	End Function

	Public Shared Function ExistsFile(path As String)
		Return File.Exists(path)
	End Function

	Public Shared Function WriteContent(content As String, filePath As String) As Boolean
		Try
			Using writer As New StreamWriter(filePath, False, Encoding.UTF8)
				writer.WriteLine(content)
			End Using
			Return True
		Catch
			Return False
		End Try
	End Function

	Public Shared Function ReadContent(filePath As String) As String

		If Not File.Exists(filePath) Then
			Return ""
		End If

		Dim content As String
		Try
			Using reader As New StreamReader(filePath, Encoding.UTF8)
				content = reader.ReadToEnd()
			End Using
		Catch
			content = ""
		End Try

		Return content

	End Function

End Class
