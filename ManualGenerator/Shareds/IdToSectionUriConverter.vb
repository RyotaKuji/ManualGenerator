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
		Dim baseId As String = Section_E.GetBaseId(uri.Fragment.TrimStart("#"c))
		Return baseId
	End Function

End Class
