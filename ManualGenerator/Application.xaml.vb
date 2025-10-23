Imports System.IO
Imports System.Reflection
Imports Microsoft.VisualBasic.ApplicationServices

Class Application

	Public Sub New()
		InitializeComponent()
		Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NDAwMDczNkAzMzMwMmUzMDJlMzAzYjMzMzAzYkRWZ2lsY2d3dnlEakludXlNYmNpc3J1V0d2MEZLSzZhMlJXMTB1MEVNU2M9")
		' 時間がかかるため、予め取得しておく
		Task.Run(Sub() UserInfo.GetInstance())
	End Sub

	Protected Overrides Sub OnExit(e As ExitEventArgs)
		Try
			Dim latestAppInfoJson As String
			Using reader As New StreamReader(My.Resources.LatestAppInfo)
				latestAppInfoJson = reader.ReadToEnd()
			End Using

			If String.IsNullOrWhiteSpace(latestAppInfoJson) Then
				Throw New FileNotFoundException()
			End If
			Dim root As Newtonsoft.Json.Linq.JObject = DirectCast(Newtonsoft.Json.JsonConvert.DeserializeObject(latestAppInfoJson), Newtonsoft.Json.Linq.JObject)
			Dim latestVersion As New Version(root("version").ToString())
			If latestVersion.CompareTo(Assembly.GetExecutingAssembly().GetName().Version) > 0 Then
				Dim psi As New ProcessStartInfo("upgrade.bat") With
				{
					.Arguments = My.Resources.LatestAppDir,
					.CreateNoWindow = True,
					.UseShellExecute = False
				}
				Process.Start(psi)
			End If
		Catch ex As Exception
		Finally
			MyBase.OnExit(e)
		End Try
	End Sub
End Class
