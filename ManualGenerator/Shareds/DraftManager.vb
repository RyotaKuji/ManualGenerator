Imports System.IO
Imports Newtonsoft.Json

Public Class DraftManager

	Public Shared Function Load(Optional id As String = Nothing) As Document_E
		If String.IsNullOrWhiteSpace(id) Then
			Return Nothing
		End If
		Dim path As String = GetFilePathById(id)
		If Not FileManager.ExistsFile(path) Then
			Return Nothing
		End If
		Dim json As String = FileManager.ReadContent(path)
		Try
			Dim entity As Document_E = JsonConvert.DeserializeObject(Of Document_E)(json)
			Return entity
		Catch ex As Exception
			Return Nothing
		End Try
	End Function

	Public Shared Function Save(entity As Document_E) As Boolean
		entity.Id = ConvertToDraftId(entity.Id)
		Dim json As String = JsonConvert.SerializeObject(entity, Formatting.Indented)
		Dim path As String = GetFilePathById(entity.Id)
		Return FileManager.WriteContent(json, path)
	End Function

	Private Shared Function GetFilePathById(id As String) As String
		id = ConvertToDraftId(id)
		Return Path.Combine(Configuration.DataFileBasePath, $"{id}.json")
	End Function

	Private Shared Function ConvertToDraftId(id As String) As String
		If IsDraftId(id) Then
			Return id
		Else
			Return $"{id}{Configuration.DraftSuffix}"
		End If
	End Function

	Private Shared Function IsDraftId(id As String) As Boolean
		Return id.Contains(Configuration.DraftSuffix)
	End Function
End Class
