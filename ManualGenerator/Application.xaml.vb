Class Application

	Public Sub New()
		InitializeComponent()
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDAwMDczNkAzMzMwMmUzMDJlMzAzYjMzMzAzYkRWZ2lsY2d3dnlEakludXlNYmNpc3J1V0d2MEZLSzZhMlJXMTB1MEVNU2M9")
		' 時間がかかるため、予め取得しておく
		Task.Run(Sub() UserInfo.GetInstance())
	End Sub
End Class
