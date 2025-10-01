Imports System.Globalization

Public Class EnumToVisibilityConverter : Implements IValueConverter

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		Dim isMatch As Boolean = value IsNot Nothing AndAlso value.Equals([Enum].Parse(value.GetType(), parameter.ToString()))

		Return If(isMatch, Visibility.Visible, Visibility.Collapsed)
	End Function

	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		If TypeOf value Is Visibility AndAlso CInt(value) = CInt(Visibility.Visible) Then
			If parameter IsNot Nothing Then
				Return [Enum].Parse(targetType, parameter.ToString())
			End If
		End If
		Return Binding.DoNothing
	End Function
End Class