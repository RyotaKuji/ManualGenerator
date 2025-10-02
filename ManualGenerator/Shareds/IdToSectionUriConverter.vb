Public Class IdToSectionUriConverter

	Public Shared Function Convert(id As String) As Uri
		If String.IsNullOrWhiteSpace(id) Then
			Return Nothing
		End If
		id = Section_E.GetBaseId(id)
		Return New Uri($"#{id}", UriKind.Relative)
	End Function

	Public Shared Function ConvertBack(uri As Uri) As String
		If uri Is Nothing Then
			Return Nothing
		End If
		Dim uriStr As String = uri.OriginalString
		Dim id As String = uriStr.TrimStart("#"c)
		Dim baseId As String = Section_E.GetBaseId(id)
		Return baseId
	End Function

End Class
