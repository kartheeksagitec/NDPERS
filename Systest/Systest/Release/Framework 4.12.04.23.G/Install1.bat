cd C:\PersLink\PERSLink\QA\Release\FrameworkNew\NewRealease
"gacutil.exe" -u SagitecCommon,Version=4.0.0.0
"gacutil.exe" -u SagitecInterface,Version=4.0.0.0
"gacutil.exe" -u SagitecExceptionPub.Interfaces,Version=4.0.0.0
"gacutil.exe" -u SagitecExceptionPub,Version=4.0.0.0
"gacutil.exe" -u SagitecBusinessObjects,Version=4.0.0.0
"gacutil.exe" -u SagitecBusinessTier,Version=4.0.0.0
"gacutil.exe" -u SagitecCorBuilder,Version=4.0.0.0
"gacutil.exe" -u SagitecCorBuilder2003,Version=4.0.0.0
"gacutil.exe" -u SagitecCorBuilder2007,Version=4.0.0.0
"gacutil.exe" -u SagitecDBCache,Version=4.0.0.0
"gacutil.exe" -u SagitecDBUtility,Version=4.0.0.0
"gacutil.exe" -u SagitecMetaDataCache,Version=4.0.0.0
"gacutil.exe" -u SagitecWebControls,Version=4.0.0.0
"gacutil.exe" -u SagitecWebClient,Version=4.0.0.0
"gacutil.exe" -u SagitecCorBuilderOpenXML,Version=4.0.0.0

pause
"gacutil.exe" -i SagitecCommon.dll
"gacutil.exe" -i SagitecInterface.dll
"gacutil.exe" -i SagitecExceptionPub.Interfaces.dll
"gacutil.exe" -i SagitecExceptionPub.dll
"gacutil.exe" -i SagitecBusinessObjects.dll
"gacutil.exe" -i SagitecBusinessTier.dll
"gacutil.exe" -i SagitecCorBuilder2003.dll
"gacutil.exe" -i SagitecDBCache.dll
"gacutil.exe" -i SagitecDBUtility.dll
"gacutil.exe" -i SagitecMetaDataCache.dll
"gacutil.exe" -i SagitecWebControls.dll
"gacutil.exe" -i SagitecWebClient.dll
"gacutil.exe" -i SagitecCorBuilder2007.dll
"gacutil.exe" -i SagitecCorBuilderOpenXML.dll

pause
iisreset

