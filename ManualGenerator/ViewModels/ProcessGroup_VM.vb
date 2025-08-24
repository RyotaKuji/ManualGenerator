Imports CommunityToolkit.Mvvm.ComponentModel
Imports CommunityToolkit.Mvvm.Input

Public Class ProcessGroup_VM
	Inherits ObservableObject

	Public Property BeginInputCommand As RelayCommand
	Public Property RemoveCommand As RelayCommand

	Private _heading As String
	Public Property Heading As String
		Get
			Return _heading
		End Get
		Set(value As String)
			SetProperty(_heading, value)
			OnPropertyChanged(NameOf(IsRemovable))
		End Set
	End Property

	Private _description As String
	Public Property Description As String
		Get
			Return _description
		End Get
		Set(value As String)
			SetProperty(_description, value)
			OnPropertyChanged(NameOf(IsRemovable))
		End Set
	End Property

	Public ReadOnly Property IsRemovable As Boolean
		Get
			Return String.IsNullOrWhiteSpace(Heading) AndAlso
				String.IsNullOrWhiteSpace(Description)
		End Get
	End Property

	Public Property ImagePath As String = "/Resources/Images/picture.png"

	' ロストフォーカス直後はプロパティの値が変わらないため、引数で受け取って手動で更新
	Public Sub ChangedHeading(text As String)
		Heading = text
		BeginInputCommand?.Execute(Me)
	End Sub

End Class
