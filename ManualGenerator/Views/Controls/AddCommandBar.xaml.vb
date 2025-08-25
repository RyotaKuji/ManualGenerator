Public Class AddCommandBar

	Public Shared ReadOnly _command As DependencyProperty =
		DependencyProperty.Register(
			"Command",
			GetType(ICommand),
			GetType(AddCommandBar),
			New PropertyMetadata(Nothing)
		)

	Public Property Command As ICommand
		Get
			Return CType(GetValue(_command), ICommand)
		End Get
		Set(value As ICommand)
			SetValue(_command, value)
		End Set
	End Property

	' CommandParameter の依存関係プロパティ
	Public Shared ReadOnly _commandParameter As DependencyProperty =
		DependencyProperty.Register(
			"CommandParameter",
			GetType(Object),
			GetType(AddCommandBar),
			New PropertyMetadata(Nothing)
		)

	Public Property CommandParameter As Object
		Get
			Return GetValue(_commandParameter)
		End Get
		Set(value As Object)
			SetValue(_commandParameter, value)
		End Set
	End Property

	Private Sub OnClick(sender As Object, e As MouseButtonEventArgs)
		If Command IsNot Nothing AndAlso
			Command.CanExecute(CommandParameter) Then
			Command.Execute(CommandParameter)
		End If
	End Sub
End Class
