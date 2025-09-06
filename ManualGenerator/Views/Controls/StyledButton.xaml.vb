Public Class StyledButton
	Public Property CornerRadius As Double
	Public Property FilterColor As String
	Public Property BorderThickness_BoldRate As Double

	Public ReadOnly Property BorderThickness_Bold As Thickness
		Get
			Return New Thickness(BorderThickness.Left * BorderThickness_BoldRate,
								 BorderThickness.Top * BorderThickness_BoldRate,
								 BorderThickness.Right * BorderThickness_BoldRate,
								 BorderThickness.Bottom * BorderThickness_BoldRate)
		End Get
	End Property

	Public ReadOnly Property Padding_BoldDiff As Thickness
		Get
			Return New Thickness(BorderThickness_Bold.Left - BorderThickness.Left,
								 BorderThickness_Bold.Top - BorderThickness.Top,
								 BorderThickness_Bold.Right - BorderThickness.Right,
								 BorderThickness_Bold.Bottom - BorderThickness.Bottom)
		End Get
	End Property

	Public Sub New()
		InitializeComponent()
		CornerRadius = 2
		BorderThickness_BoldRate = 1.5
		FilterColor = "LightGray"
		BorderThickness = New Thickness(1)
	End Sub
End Class
