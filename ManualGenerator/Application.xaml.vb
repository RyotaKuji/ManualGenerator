Class Application

	' Startup、Exit、DispatcherUnhandledException などのアプリケーション レベルのイベントは、
	' このファイルで処理できます。
	Public Sub New()
		' アプリケーションの初期化をここで行います。
		InitializeComponent()

		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDAwMDczNkAzMzMwMmUzMDJlMzAzYjMzMzAzYkRWZ2lsY2d3dnlEakludXlNYmNpc3J1V0d2MEZLSzZhMlJXMTB1MEVNU2M9")
	End Sub
End Class
