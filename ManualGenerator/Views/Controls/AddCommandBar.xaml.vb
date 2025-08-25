Public Class AddCommandBar

	Public Shared ReadOnly CommandProperty As DependencyProperty =
		DependencyProperty.Register(
			"Command",
			GetType(ICommand),
			GetType(AddCommandBar),
			New PropertyMetadata(Nothing)
		)


	Public Property Command As ICommand
		Get
			Return CType(GetValue(CommandProperty), ICommand)
		End Get
		Set(value As ICommand)
			SetValue(CommandProperty, value)
		End Set
	End Property


	' CommandParameter の依存関係プロパティ
	Public Shared ReadOnly CommandParameterProperty As DependencyProperty =
		DependencyProperty.Register(
			"CommandParameter",
			GetType(Object),
			GetType(AddCommandBar),
			New PropertyMetadata(Nothing)
		)

	Public Property CommandParameter As Object
		Get
			Return GetValue(CommandParameterProperty)
		End Get
		Set(value As Object)
			SetValue(CommandParameterProperty, value)
		End Set
	End Property

	Private Sub OnClick(sender As Object, e As MouseButtonEventArgs)
		If Command IsNot Nothing AndAlso
			Command.CanExecute(CommandParameter) Then
			Command.Execute(CommandParameter)
		End If
	End Sub
End Class
