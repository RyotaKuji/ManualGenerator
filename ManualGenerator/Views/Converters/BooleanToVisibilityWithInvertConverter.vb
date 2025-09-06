Imports System.Globalization

Public Class BooleanToVisibilityWithInvertConverter : Implements IValueConverter

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		Dim flag As Boolean = False
		If TypeOf value Is Boolean Then
			flag = CBool(value)
		End If
		flag = Not flag
		Return If(flag, Visibility.Visible, Visibility.Collapsed)
	End Function

	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		If TypeOf value Is Visibility Then
			Dim visibility As Visibility = CType(value, Visibility)
			Return visibility <> Visibility.Visible
		End If
		Return False
	End Function
End Class
