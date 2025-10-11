Imports System.Windows.Media.Animation

Public Class Header_EditorPage

	Public Sub New()
		InitializeComponent()
		AddHandler Loaded, AddressOf Page_Loaded
	End Sub

	Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
		Dim VM = TryCast(DataContext, EditorPage_VM)
		AddHandler VM.OnSaved, AddressOf OnSaved
	End Sub

	Private Sub OnSaved()
		Dim storyboard As Storyboard = TryCast(savedSign.FindResource("savedAnimation"), Storyboard)
		storyboard?.Begin()
	End Sub
End Class
