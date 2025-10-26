c:
cd C:\PERSLinkSource\PERSLink\Dev\Release\OpenXmlFramework

"gacutil.exe" -u SagitecCorBuilderOpenXML,Version=4.0.0.0
"gacutil.exe" -u OpenXmlPowerTools,Version=1.0.0.0

pause

"gacutil.exe" -i SagitecCorBuilderOpenXML.dll
"gacutil.exe" -i OpenXmlPowerTools.dll

pause
iisreset

