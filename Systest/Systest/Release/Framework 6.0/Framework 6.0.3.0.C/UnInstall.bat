c:
cd C:\PERSLinkSource\PERSLink\QA\Framework

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

iisreset
pause


