set SOURCE=src
set SERVICE=A4CS

del /Q bin\*.*

csc /r:System.XML.Linq.dll /r:System.ServiceModel.dll /r:System.ServiceModel.Web.dll /lib:lib /r:Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll /r:Microsoft.Data.Edm.dll /r:Microsoft.Data.OData.dll /r:Microsoft.Data.Services.Client.dll /r:Microsoft.Data.Services.dll /r:System.Spatial.dll /target:library /out:bin\%SERVICE%.dll %SOURCE%.svc.cs
pause

copy .\lib\*.dll .\bin
pause
