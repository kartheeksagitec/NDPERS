C:
cd "%~dp0"  
gacutil.exe -u SagitecCommon
gacutil.exe -u SagitecInterface
gacutil.exe -u SagitecExceptionPub.Interfaces
gacutil.exe -u SagitecExceptionPub
gacutil.exe -u SagitecBusinessObjects
gacutil.exe -u SagitecBusinessTier
gacutil.exe -u SagitecDBCache
gacutil.exe -u SagitecDBUtility
gacutil.exe -u SagitecMetaDataCache
gacutil.exe -u SagitecWebControls
gacutil.exe -u SagitecWebClient
gacutil.exe -u SagitecRules
gacutil.exe -u SagitecMVVMClient
gacutil.exe -u SagitecBPM
gacutil.exe -u SagitecCorBuilder
gacutil.exe -u HtmlAgilityPack
gacutil.exe -u HtmlSanitizationLibrary
gacutil.exe -u AntiXSSLibrary
gacutil.exe -u OpenXmlPowerTools.dll

gacutil.exe -i SagitecCommon.dll
gacutil.exe -i SagitecInterface.dll
gacutil.exe -i SagitecExceptionPub.Interfaces.dll
gacutil.exe -i SagitecExceptionPub.dll
gacutil.exe -i SagitecBusinessObjects.dll
gacutil.exe -i SagitecBusinessTier.dll
gacutil.exe -i SagitecDBCache.dll
gacutil.exe -i SagitecDBUtility.dll
gacutil.exe -i SagitecMetaDataCache.dll
gacutil.exe -i SagitecWebControls.dll
gacutil.exe -i SagitecWebClient.dll
gacutil.exe -i SagitecRules.dll
gacutil.exe -i SagitecMVVMClient.dll
gacutil.exe -i SagitecBPM.dll
gacutil.exe -i SagitecCorBuilder.dll
gacutil.exe -i HtmlAgilityPack.dll
gacutil.exe -i HtmlSanitizationLibrary.dll
gacutil.exe -i AntiXSSLibrary.dll
gacutil.exe -i OpenXmlPowerTools.dll

pause
iisreset
pause
