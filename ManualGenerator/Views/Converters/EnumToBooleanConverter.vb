Imports System.Globalization

Public Class EnumToBooleanConverter
	Implements IValueConverter

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		Return value IsNot Nothing AndAlso value.Equals([Enum].Parse(value.GetType(), parameter.ToString()))
	End Function

	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		If CBool(value) Then
			Return [Enum].Parse(targetType, parameter.ToString())
		End If
		Return Binding.DoNothing
	End Function
End Class