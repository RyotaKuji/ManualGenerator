Public Class CurrentSectionsManager

	Public Property VMs As IEnumerable(Of Section_VM)

	Private Shared ReadOnly instance As New CurrentSectionsManager()

	Public Shared Function GetInstance() As CurrentSectionsManager
		Return instance
	End Function

	Public Sub RequestScroll(uri As Uri)
		Dim baseId As String = IdToSectionUriConverter.ConvertBack(uri)
		Dim targetSection As Section_VM = VMs?.FirstOrDefault(Function(s) Section_E.GetBaseId(s.Entity.Id) = baseId)
		targetSection?.ScrollToSelf()
	End Sub
End Class
