Imports System.IO
Imports Newtonsoft.Json

Public Class PublicManager

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
		entity.Id = ConvertToPublicId(entity.Id)
		Dim json As String = JsonConvert.SerializeObject(entity, Formatting.Indented)
		Dim path As String = GetFilePathById(entity.Id)
		Return FileManager.WriteContent(json, path)
	End Function

	Private Shared Function GetFilePathById(id As String) As String
		id = ConvertToPublicId(id)
		Return Path.Combine(Configuration.DataFileBasePath, $"{id}.json")
	End Function

	Private Shared Function ConvertToPublicId(id As String) As String
		If IsPublicId(id) Then
			Return id
		Else
			Return id.Replace(Configuration.DraftSuffix, "")
		End If
	End Function

	Private Shared Function IsPublicId(id As String) As Boolean
		Return id.Contains(Configuration.DraftSuffix) = False
	End Function

End Class
