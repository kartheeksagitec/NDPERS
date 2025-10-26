CD C:
CD C:\PERSLinkSource\PERSLink\FW6.0DEV\Framework

gacutil.exe -u SagitecCommon,Version=6.0.0.0
gacutil.exe -u SagitecInterface,Version=6.0.0.0
gacutil.exe -u SagitecExceptionPub.Interfaces,Version=6.0.0.0
gacutil.exe -u SagitecExceptionPub,Version=6.0.0.0
gacutil.exe -u SagitecBusinessObjects,Version=6.0.0.0
gacutil.exe -u SagitecBusinessTier,Version=6.0.0.0
gacutil.exe -u SagitecDBCache,Version=6.0.0.0
gacutil.exe -u SagitecDBUtility,Version=6.0.0.0
gacutil.exe -u SagitecMetaDataCache,Version=6.0.0.0
gacutil.exe -u SagitecWebControls,Version=6.0.0.0
gacutil.exe -u SagitecWebClient,Version=6.0.0.0
gacutil.exe -u SagitecRules,Version=6.0.0.0
gacutil.exe -u SagitecMVVMClient,Version=6.0.0.0
gacutil.exe -u SagitecBPM,Version=6.0.0.0
gacutil.exe -u SagitecCorBuilder,Version=6.0.0.0
gacutil.exe -u SagitecQuestionnaire,Version=6.0.0.0

gacutil.exe -u SagitecCommon,Version=5.0.0.0
gacutil.exe -u SagitecInterface,Version=5.0.0.0
gacutil.exe -u SagitecExceptionPub.Interfaces,Version=5.0.0.0
gacutil.exe -u SagitecExceptionPub,Version=5.0.0.0
gacutil.exe -u SagitecBusinessObjects,Version=5.0.0.0
gacutil.exe -u SagitecBusinessTier,Version=5.0.0.0
gacutil.exe -u SagitecDBCache,Version=5.0.0.0
gacutil.exe -u SagitecDBUtility,Version=5.0.0.0
gacutil.exe -u SagitecMetaDataCache,Version=5.0.0.0
gacutil.exe -u SagitecWebControls,Version=5.0.0.0
gacutil.exe -u SagitecWebClient,Version=5.0.0.0
gacutil.exe -u SagitecRules,Version=5.0.0.0
gacutil.exe -u SagitecMVVMClient,Version=5.0.0.0
gacutil.exe -u SagitecBPM,Version=5.0.0.0
gacutil.exe -u SagitecCorBuilder,Version=5.0.0.0
gacutil.exe -u SagitecQuestionnaire,Version=5.0.0.0

pause
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
gacutil.exe -i SagitecCorBuilder.dll
gacutil.exe -i SagitecMVVMClient.dll
gacutil.exe -i SagitecBPM.dll
gacutil.exe -i SagitecQuestionnaire.dll
gacutil.exe -i OpenXmlPowerTools.dll

iisreset
pause
