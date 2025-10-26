#region [Using directives]
//using gov.nd.appstest.secure;
using NDLogin.ActiveDirectory;
using NeoSpin.BusinessObjects;
using NeoSpin.Common;
using Sagitec.Bpm;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
#endregion [Using directives]

namespace Neo.Controllers
{
    /// <summary>
    /// Class Neo.NeoController
    /// </summary>
    public partial class NeoController : ApiControllerBase
    {
        #region[FS006 START- Communication related Functions]

        /// <summary>
        /// Overidden method to configure customized buttons on correspondence editor.
        /// </summary>
        /// <param name="adictCorrInfo">adictCorrInfo</param>   
        /// <param name="alstCorrToolCustomControls">alstCorrToolCustomControls</param> 
        public override void ModifyCustomButtonConfigurationForCorrTool(Dictionary<string, object> adictCorrInfo, out List<utlCorrToolCustomControl> alstCorrToolCustomControls)
        {
            alstCorrToolCustomControls = new List<utlCorrToolCustomControl>();

            Dictionary<string, object> ldictParams = SetParams("wfmLogin");
            isrvServers.ConnectToBT(string.Empty);
            int lintCorTrackingId = Convert.ToInt32(adictCorrInfo["TrackingID"]);
            

            base.ModifyCustomButtonConfigurationForCorrTool(adictCorrInfo, out alstCorrToolCustomControls);
            alstCorrToolCustomControls = new List<utlCorrToolCustomControl>();
            utlCorrToolCustomButton autlCorrToolCustomButton1 = new utlCorrToolCustomButton();
            autlCorrToolCustomButton1.istrText = "Image";
            autlCorrToolCustomButton1.istrControlID = "ImageButton";
            autlCorrToolCustomButton1.istrSuccessMessage = "Correspondence is successfully imaged.";
            autlCorrToolCustomButton1.iblnVisible = true;
            autlCorrToolCustomButton1.iblnCloseAfterExecution = false;
            autlCorrToolCustomButton1.iblnNeedToSaveData = true;
            alstCorrToolCustomControls.Add(autlCorrToolCustomButton1);
        }

        

        #endregion[FS002 END- Administer System related Functions]
        
        [HttpPost]
        public object SetHelpFile()
        {
            string lstrCurrentForm = istrSenderForm;
            string lstrApplicationName = HttpContext.Current.Request.ApplicationPath.ToString().Substring(1);
            string lstrOnlineHelpFolderPath = ConfigurationManager.AppSettings["OnlineHelpFolderPath"] ?? String.Empty;
            string lstrOnlineHelpPrivateFolderPath = ConfigurationManager.AppSettings["OnlineHelpPrivateFolderPath"] ?? String.Empty;
            string lstrURL = lstrApplicationName+"/"+lstrOnlineHelpFolderPath;// +this.istrFormName + ".htm'"; //new help file
            string lstrPrivateFiles = ConfigurationManager.AppSettings["PrivateHelpFiles"] ?? String.Empty;
            if (lstrPrivateFiles.IndexOf(istrSenderForm) >= 0)
                lstrURL = lstrApplicationName + "/" + lstrOnlineHelpPrivateFolderPath;

            return lstrURL;
        }

        [HttpPost]
        public bool DeletePIRAttachment(Dictionary<string, string> adictParameters)
        {
            bool lstrPath;
            isrvServers.ConnectToBT(istrSenderForm);
            string astrFileName = "";
            int aintPirId = 0;
            if (!adictParameters.IsNullOrEmpty())
            {
                astrFileName = adictParameters["astrFileName"];
                aintPirId = Convert.ToInt32(adictParameters["aintPIRID"]);
            }                
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("astrFilePath", astrFileName);
            lstrPath = (bool)isrvServers.isrvBusinessTier.ExecuteMethod("DeletePIRAttachment", lhstParam, false, ldictParams);
            return lstrPath;
        }

        [HttpPost]
        public bool RemoveWSSAccess(Dictionary<string, string> adictParameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            string astrNDPERSLoginID = "";
            if (!adictParameters.IsNullOrEmpty())
            {
                astrNDPERSLoginID = adictParameters["NdpersLoginId"];
            }
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("astrKey", null);
            lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayGroup"]);
            string lstrSecureWayGroup = (string)isrvServers.isrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, ldictParams);

            lhstParams = new Hashtable();
            lhstParams.Add("astrKey", null);
            lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayUser"]);
            string lstrSecureWayUser = (string)isrvServers.isrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, ldictParams);

            lhstParams = new Hashtable();
            lhstParams.Add("astrKey", null);
            lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayPassword"]);
            string lstrSecureWayPassword = (string)isrvServers.isrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, ldictParams);

            LdapService ldapService = new LdapService();
            ldapService.Url = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["ServerEnvironment"].ToString() + "NDLdapWS"];
            ldapService.removeUserFromGroup(astrNDPERSLoginID, lstrSecureWayGroup, lstrSecureWayUser, lstrSecureWayPassword);
            return true;
        }
        [HttpPost]
        public string btnIgnoreSelectedRows_Click(Dictionary<string, string> adictParameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            string lstrReturnMessage = "";
            List<int> lintEmployerPayrollDetailIds = new List<int>();
            string lstrPayrollDeatilsIDs = "";
            string lstrComment = "";
            bool lblnIsSetIgnorStatus = false;
            if (!adictParameters.IsNullOrEmpty())
            {
                lstrPayrollDeatilsIDs = adictParameters["PayrollDeatilsIDs"];
                lstrComment = adictParameters["Comment"];
                lblnIsSetIgnorStatus = Convert.ToBoolean(adictParameters["ablnIsSetIgnorStatus"]);
                lintEmployerPayrollDetailIds = lstrPayrollDeatilsIDs.Split(',').Select(sValue => Convert.ToInt32(sValue)).ToList();
            }

            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("aEmpPayDetailIds", lintEmployerPayrollDetailIds);
            lhstParams.Add("astrComment", lstrComment);
            lhstParams.Add("ablnIsSetIgnorStatus", lblnIsSetIgnorStatus);
            List<int> llstPayrollDetailIds = (List<int>)isrvServers.isrvBusinessTier.ExecuteMethod("IgnoreSelectedPayrollDetail", lhstParams, false, ldictParams);
            if (llstPayrollDetailIds.Count == 0)
            {
                if(lblnIsSetIgnorStatus)
                    lstrReturnMessage = "Selected payroll details have been Ignored.."; 
                else
                    lstrReturnMessage = "Selected payroll details comments have been updated..";
            }
            else
            {
                if (lblnIsSetIgnorStatus)
                {
                    string lstrNotIgnoredDetails = llstPayrollDetailIds.Select(i => i.ToString()).Aggregate((a, b) => a + ", " + b);
                    lstrReturnMessage = "Payroll detail Ids " + lstrNotIgnoredDetails + " have not been ignored as they had already been posted.";
                }
            }
            return lstrReturnMessage;
        }
        [HttpPost]
        public string PublishToWSS(Dictionary<string, string> adictParameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            string lstReturnMessage = string.Empty;
            ArrayList lArrayList = new ArrayList();
            if (!Convert.ToString(adictParameters["astrFileName"]).IsNullOrEmpty())
            {
                string lintCorTrackingcharId = string.Empty;
                if (adictParameters.Count > 0)
                {
                    if (adictParameters.ContainsKey("astrFileName"))
                    {
                        int lintCorTrackingId = DecryptFilePath(Convert.ToString(adictParameters["astrFileName"]));
                        lintCorTrackingcharId = Convert.ToString(lintCorTrackingId).PadLeft(10, '0');
                    }
                }
                Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
                string templatename = Convert.ToString(adictParameters["CorrTemplate"]);
                string astrGeneratedDocumentName = templatename + "-" + lintCorTrackingcharId + ".docx";
                if (lintCorTrackingcharId.IsNotNullOrEmpty() && templatename.IsNotNullOrEmpty())
                {
                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("astrFileName", astrGeneratedDocumentName);
                    lhstParam.Add("astrUserID", Convert.ToString(ldictParams["UserID"]));
                    lhstParam.Add("aintUserSerialID", Convert.ToInt32(ldictParams["UserSerialID"]));                    
                    lArrayList = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("PublishToWSSAndSendMail", lhstParam, false, ldictParams);
                    if (lArrayList.Count == 0)
                    {
                        //to make image in filenet
                        ArrayList larrResult = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("InitializeFileNetUploadService", lhstParam, false, ldictParams);
                        if (larrResult.Count == 0)
                        {
                            lstReturnMessage = "Correspondence is successfully published!";
                        }
                        else
                        {
                            utlError lobjError = (utlError)larrResult[0];
                            if (!String.IsNullOrEmpty(lobjError.istrErrorID))
                            {
                                lstReturnMessage = isrvServers.isrvDbCache.GetMessageText(Convert.ToInt32(lobjError.istrErrorID));
                            }
                            else
                            {
                                lstReturnMessage =lobjError.istrErrorMessage;
                            }                            
                        }
                        return lstReturnMessage;
                    }
                    else
                    {
                        utlError lobjError = (utlError)lArrayList[0];
                        if (!String.IsNullOrEmpty(lobjError.istrErrorID))
                        {
                            lstReturnMessage = isrvServers.isrvDbCache.GetMessageText(Convert.ToInt32(lobjError.istrErrorID));
                        }
                        else
                        {
                            lstReturnMessage = lobjError.istrErrorMessage;
                        }
                    }
                    return lstReturnMessage;
                }
            }
            else
            {
                lstReturnMessage = "Correspondence must be generated before publishing!";
            }
            return lstReturnMessage;

        }

        private int DecryptFilePath(string astrFilePath)
        {
            int lintTrackingID = 0;

            if (astrFilePath.IsNotNullOrEmpty())
            {
                string lstrEncryptionKey = ControlsHelper2.GetEncryptionKey(HttpContext.Current.Session.SessionID);
                string lstrEncryptionIV = ControlsHelper2.GetEncryptionIV(HttpContext.Current.Session.SessionID);
                string lstrFilePath = HelperFunction.SagitecDecryptFIPS(astrFilePath, lstrEncryptionKey, lstrEncryptionIV);
                if (lstrFilePath != null)
                {
                    int.TryParse(lstrFilePath.Substring(lstrFilePath.LastIndexOf("-") + 1).Substring(0, lstrFilePath.LastIndexOf(".") - lstrFilePath.LastIndexOf("-") - 1), out lintTrackingID);
                }
            }
            return lintTrackingID;
        }

        [HttpPost]
        public string CorrespondenceImage(Dictionary<string, string> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            string lstReturnMessage = string.Empty;
            ArrayList lArrayList = new ArrayList();
            if (!Convert.ToString(Parameters["astrFileName"]).IsNullOrEmpty())
            {
                string lintCorTrackingcharId = string.Empty;
                if (Parameters.Count > 0)
                {
                    if (Parameters.ContainsKey("astrFileName"))
                    {
                        int lintCorTrackingId = DecryptFilePath(Convert.ToString(Parameters["astrFileName"]));
                        lintCorTrackingcharId = Convert.ToString(lintCorTrackingId).PadLeft(10, '0');
                    }
                }
                Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
                string templatename = Convert.ToString(Parameters["CorrTemplate"]);
                string astrGeneratedDocumentName = templatename + "-" + lintCorTrackingcharId + ".docx";
                if (lintCorTrackingcharId.IsNotNullOrEmpty() && templatename.IsNotNullOrEmpty())
                {
                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("astrFileName", astrGeneratedDocumentName);
                    lhstParam.Add("astrUserID", Convert.ToString(ldictParams["UserID"]));
                    lhstParam.Add("aintUserSerialID", Convert.ToInt32(ldictParams["UserSerialID"]));
                    lArrayList = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("InitializeFileNetUploadService", lhstParam, false, ldictParams);
                    if (lArrayList.Count == 0)
                    {
                        lstReturnMessage = "Correspondence is successfully imaged!";
                        return lstReturnMessage;
                    }
                    else
                    {
                        utlError lobjError = (utlError)lArrayList[0];
                        if (!String.IsNullOrEmpty(lobjError.istrErrorID))
                        {
                            lstReturnMessage = isrvServers.isrvDbCache.GetMessageText(Convert.ToInt32(lobjError.istrErrorID));
                        }
                        else
                        {
                            lstReturnMessage = lobjError.istrErrorMessage;
                        }
                    }
                    return lstReturnMessage;
                }
            }
            else
            {
                lstReturnMessage = "Correspondence must be generated before Imaging!";
            }
            return lstReturnMessage;
        }

        [HttpPost]
        public HttpResponseMessage InsertReportRequest()
        {
            HttpResponseMessage result;
            bool lblnOpenInNewTab = false;
            int lintErrLevel = -1;
            if (isrvServers == null)
            {
                isrvServers = new srvServers();
            }
            var jss = new JavaScriptSerializer();
            ReportModel reportModel = jss.Deserialize<ReportModel>(HttpContext.Current.Request["adictParams"].ToString());
            isrvServers.ConnectToBT("wfmReportClientMVVM");

            Hashtable lhstReportParams = new Hashtable();
            int startIndex = HttpContext.Current.Server.MapPath("").ToUpper().IndexOf("\\API");
            if (startIndex > 0)
            {
                string reportpath = HttpContext.Current.Server.MapPath("").Substring(0, startIndex);
                lhstReportParams.Add("astrReportPath", reportpath + "\\Reports\\");
            }
            

            lhstReportParams.Add("aclsReportModel", reportModel);
            Dictionary<string, object> ldictParams = SetParams("wfmReportClientMVVM");
            ArrayList lhstResult = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("GenerateReportDataSet", lhstReportParams, false, ldictParams);

            if (lhstResult.Count > 0 && lhstResult[0] is utlError)
            {
                result = new HttpResponseMessage(HttpStatusCode.OK);
                string lstrError = "try{ if(window && window.parent){ window.parent.nsEvents.downloadError('" + ((utlError)lhstResult[0]).istrErrorMessage + "');} }catch(e){}";
                if (lblnOpenInNewTab)
                    lstrError = "try{ if(window && window.opener && window.opener.parent){ window.opener.parent.nsEvents.downloadError('" + ((utlError)lhstResult[0]).istrErrorMessage + "', window);} }catch(e){ alert('Error in download.'); window.close(); }";

                var lstrErrorHtml = "<html><body><script type='text/javascript'>" + lstrError + "</script></body></html>";
                result.Content = new StringContent(lstrErrorHtml, Encoding.UTF8, "text/html");
                return result;
            }

            result = new HttpResponseMessage(HttpStatusCode.OK);

            lintErrLevel = 0;
            string lstrFileName = (String)lhstResult[0];
            //if (string.IsNullOrEmpty(lstrFileName))
            //56002 - {0} - {1} # Failed - Invalid File: {2}.
            //throw new FileHandlingException(56002, lstrFileName);

            Byte[] lbytArrContent = (Byte[])lhstResult[1];
            lintErrLevel = 2;
            string lstrContentType = null;
            if (lhstResult.Count >= 3)
                lstrContentType = (String)lhstResult[2];
            if (string.IsNullOrEmpty(lstrContentType))
                lstrContentType = MimeMapping.GetMimeMapping(lstrFileName);

            lintErrLevel = 3;
            //Fix for download file issue on iPad
            var dataStream = new System.IO.MemoryStream(lbytArrContent);
            result.Content = new StreamContent(dataStream);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = lstrFileName;
            string lstrFileType = System.IO.Path.GetExtension(lstrFileName);

            if (!string.IsNullOrEmpty(lstrFileType))
            {
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd" + lstrFileType);
            }
            else
            {
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            }
            return result;
        }

        [HttpPost]
        public string ReturnFileNetURL(Dictionary<string, string> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            string lstReturnURL = string.Empty;
            string istrHostName = ConfigurationManager.AppSettings["FILENET_URL"];
            string istrUserName = ConfigurationManager.AppSettings["FileNetUserName"];
            string istrPassword = ConfigurationManager.AppSettings["FileNetPassword"];
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParams = new Hashtable();
            if ((String.IsNullOrEmpty(istrUserName)) || (String.IsNullOrEmpty(istrPassword)))
            {
                istrUserName = Convert.ToString(ldictParams["UserID"]);
                lhstParams.Add("astrKey", null);
                lhstParams.Add("astrValue", Convert.ToString(iobjSessionData["AccessDenied"]));
                istrPassword = (string)(string)isrvServers.isrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, ldictParams);
            }
            else
            {
                //Decrypt the Encrypted Web.Config Password    
                lhstParams.Add("astrKey", null);
                lhstParams.Add("astrValue", istrPassword);
                istrPassword = (string)(string)isrvServers.isrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, ldictParams);
            }

            try
            {
                string lstrObjectStore = Convert.ToString(Parameters["astrObjectStore"]);
                string lstrVersionSeriesId = Convert.ToString(Parameters["astrVersionSeriesID"]);
                string lstrDocumentID = Convert.ToString(Parameters["astrDocumentID"]);
                string lstrDocName = Convert.ToString(Parameters["astrDocumentTitle"]);


                lstReturnURL = istrHostName + lstrVersionSeriesId;
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
            }
            return lstReturnURL;
        }


        [HttpPost]
        public List<string> GetFileNameImageURLs(Dictionary<string, string> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParams = new Hashtable();
            Collection<busSolBpmProcessInstanceAttachments> lclbProcInstanceImageData = null;
            if (Parameters.ContainsKey("aintProcessInstanceID"))
            {
                lhstParams.Add("aintProcessInstanceID", Parameters["aintProcessInstanceID"]);
                lclbProcInstanceImageData = (Collection<busSolBpmProcessInstanceAttachments>)isrvServers.isrvBusinessTier.ExecuteMethod("LoadImageDataByProcessInstance", lhstParams, false, ldictParams);
            }
            else if (Parameters.ContainsKey("aintProcessInstanceAttachmentId"))
            {
                lhstParams.Add("aintProcessInstanceAttachmentId", Parameters["aintProcessInstanceAttachmentId"]);
                lclbProcInstanceImageData = (Collection<busSolBpmProcessInstanceAttachments>)isrvServers.isrvBusinessTier.ExecuteMethod("LoadImageDataByAttachmentId", lhstParams, false, ldictParams);
            }



            List<string> lstrFileNetUrls = new List<string>();
            if (lclbProcInstanceImageData != null)
            {
                string istrHostName = ConfigurationManager.AppSettings["FILENET_URL"];
                foreach (busSolBpmProcessInstanceAttachments lbusProcessInstanceImageData in lclbProcInstanceImageData)
                {
                    lstrFileNetUrls.Add(istrHostName + lbusProcessInstanceImageData.version_series_id);
                }
            }
            return lstrFileNetUrls;
        }

        //PIR 25920 Droplist Refresh from DatePicker
        [HttpPost]
        public object GetADECAmountValuesByEffectiveDate(Dictionary<string, string> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParams = new Hashtable();
            object lobjResult = null;
            if (Parameters.ContainsKey("astrHistoryChangeDate"))
            {
                lhstParams.Add("astrHistoryChangeDate", Parameters["astrHistoryChangeDate"]);
                lhstParams.Add("aintPersonEmploymentDtlId", Parameters["aintPersonEmploymentDtlId"]);
                lhstParams.Add("aintPersonAccountId", Parameters["aintPersonAccountId"]);
                try
                {
                    lobjResult = isrvServers.isrvBusinessTier.ExecuteMethod("GetADECAmountValuesByEffectiveDate", lhstParams, false, ldictParams);
                }
                finally
                {                    
                }                
            }
            return lobjResult;
        }

        private ArrayList GetActivityLaunchScreen(Dictionary<String, String> AllParams)
        {
            string astrFormID = AllParams["FormID"].ToString();
            isrvServers.ConnectToBT(astrFormID);
            Dictionary<string, object> ldictParams = SetParams(astrFormID);
            ldictParams["ButtonID"] = AllParams["ButtonID"].ToString();
            ldictParams["sfwCheckOut"] = AllParams["sfwCheckOut"] != null;
            ldictParams["sfwResume"] = AllParams["sfwResume"] != null;
            ldictParams["PrimaryKey"] = AllParams["PrimaryKey"].ToString();
            ldictParams["GridID"] = AllParams["GridID"].ToString();
            if (AllParams.ContainsKey("SelectedIndex") && AllParams["SelectedIndex"] != null)
            {
                ldictParams["SelectedIndex"] = int.Parse(AllParams["SelectedIndex"].ToString());
            }
            else
            {
                ldictParams["SelectedIndex"] = 0;
            }
            if (AllParams.ContainsKey("ActivityInstanceID") && AllParams["ActivityInstanceID"] != null && AllParams["ActivityInstanceID"] !="")
            {
                ldictParams["ActivityInstanceID"] = int.Parse(AllParams["ActivityInstanceID"].ToString());
            }
            else
            {
                ldictParams["ActivityInstanceID"] = 0;
            }
            //7783 - discuss and merge multiple BT calls
            return isrvServers.isrvBusinessTier.MVVMLaunchActivityScreen(ldictParams);
        }
        /// <summary>
        /// WorkflowExecuteMethod method is used to execute the workflow method for BPM
        /// </summary>
        /// <param name="AllParams">AllParams is keyValue pair of parameters to be required</param>
        /// <returns>Returns HttpResponseMessage object containing ViewModelObj object</returns>
        public override HttpResponseMessage WorkflowExecuteMethod(Dictionary<string, string> AllParams)
        {
            try
            {
                bool lblnBpmActivityInstance = false;
                bool lblnRefreshCenterLeft = false;
                int lintActivityInstanceId = 0;
                string lstrCenterLeftForm = "wfmBPMWorkflowCenterLeftMaintenance";
                ArrayList larrResponse = new ArrayList();
                string astrFormID = AllParams["FormID"].ToString();
                Dictionary<string, object> ldictParams = SetParams(astrFormID);

                if (AllParams.ContainsKey("IsRefreshCenterLeft") && Convert.ToBoolean(AllParams["IsRefreshCenterLeft"]))
                {
                    lblnRefreshCenterLeft = true;
                    if (AllParams.ContainsKey("CenterLeftForm"))
                        lstrCenterLeftForm = AllParams["CenterLeftForm"].ToString();
                }

                ArrayList larrLaunchInfo = GetActivityLaunchScreen(AllParams);

                string lstrActiveForm = "";
                HttpResponseMessage lhrmMessage = MVVMHelperFunctions.CheckResponseForError(Request, vm, larrLaunchInfo);
                if (lhrmMessage != null)
                    return lhrmMessage;

                if (vm.HasErrors) //if error
                {
                    larrResponse.Add(vm);
                }
                else if (larrLaunchInfo.Count == 2) // if success
                {
                    vm = new ViewModelObj();
                    vm.DomainModel = MVVMHelperFunctions.DeserializeObject<utlResponseData>(larrLaunchInfo[0] as string);
                    vm.ExtraInfoFields["FormId"] = astrFormID;
                    vm.ExtraInfoFields["KeyField"] = vm.DomainModel.KeysData.ContainsKey("PrimaryKey") ? vm.DomainModel.KeysData["PrimaryKey"] : "0";
                    larrResponse.Add(vm); //First object - response from launch info

                    utlWorkflowActivityInfo lobjWorkflowActivityInfo = (utlWorkflowActivityInfo)larrLaunchInfo[1];
                    System.Reflection.PropertyInfo lprfActivityInstance = lobjWorkflowActivityInfo.ibusBaseActivityInstance.GetType().GetProperty("icdoBpmActivityInstance");
                    lblnBpmActivityInstance = (null != lprfActivityInstance);
                    lintActivityInstanceId = (int)HelperFunction.GetObjectValue(lobjWorkflowActivityInfo.ibusBaseActivityInstance, lblnBpmActivityInstance ? "icdoBpmActivityInstance.activity_instance_id" : "icdoActivityInstance.activity_instance_id", ReturnType.Object);
                    lstrActiveForm = lobjWorkflowActivityInfo.istrURL;
                    if (!lstrActiveForm.Contains("Lookup")) //Maintenance Page
                    {
                        if (lblnBpmActivityInstance)
                        {
                            Dictionary<string, object> ldictActivityInstanceDetails = new Dictionary<string, object>();
                            ldictActivityInstanceDetails["ActivityInstanceType"] = "BPM";
                            ldictActivityInstanceDetails["ActivityInstanceId"] = lintActivityInstanceId;
                            if (AllParams.ContainsKey("sfwViewActivityScreen"))
                            {
                                ldictActivityInstanceDetails["sfwViewActivityScreen"] = AllParams["sfwViewActivityScreen"] != null;
                            }
                            ldictActivityInstanceDetails[utlConstants.istrConstPageMode] = lobjWorkflowActivityInfo.ienmPageMode.ToString();
                            ldictActivityInstanceDetails["navParams"] = HelperFunction.EncryptNavigationParams(lobjWorkflowActivityInfo.ihstLaunchParameters, lstrActiveForm, iobjSessionData.istrSessionId);
                            ldictActivityInstanceDetails["FromBPM"] = true;
                            ldictActivityInstanceDetails["FormId"] = lstrActiveForm;

                            vm = new ViewModelObj();
                            vm.DomainModel = new utlResponseData();
                            vm.DomainModel.OtherData["LaunchDetails"] = ldictActivityInstanceDetails;
                            vm.ExtraInfoFields["FormId"] = lstrActiveForm;
                            larrResponse.Add(vm);
                        }
                        else
                        {
                            isrvServers.ConnectToBT(lstrActiveForm);
                            ldictParams = SetParams(lstrActiveForm);
                            if (lblnBpmActivityInstance)
                                ldictParams["FromBPM"] = true;
                            if (ldictParams.ContainsKey("PrimaryKey"))
                            {
                                ldictParams["PrimaryKey"] = "0";
                            }
                            ldictParams[utlConstants.istrConstPageMode] = lobjWorkflowActivityInfo.ienmPageMode;

                            ArrayList larrLaunchScreenObject = null;
                            try
                            {
                                ldictParams["sfwViewActivityScreen"] = AllParams["sfwViewActivityScreen"] != null;
                                ldictParams["SendReturnParams"] = true;
                                ldictParams["navParams"] = HelperFunction.EncryptNavigationParams(lobjWorkflowActivityInfo.ihstLaunchParameters, lstrActiveForm, iobjSessionData.istrSessionId);
                                larrLaunchScreenObject = isrvServers.isrvBusinessTier.MVVMGetInitialData(lobjWorkflowActivityInfo.ihstLaunchParameters, 0, ldictParams);
                                ldictParams.Remove("SendReturnParams");
                            }
                            finally
                            {
                                if (ldictParams.ContainsKey("sfwViewActivityScreen"))
                                {
                                    ldictParams.Remove("sfwViewActivityScreen");
                                }
                            }
                            lhrmMessage = MVVMHelperFunctions.CheckResponseForError(Request, vm, larrLaunchScreenObject);

                            if (lhrmMessage != null)
                                return lhrmMessage;

                            ldictParams.Remove(utlConstants.istrActivityInstance);

                            if (vm.HasErrors) //error
                            {
                                larrResponse.Add(vm); //Center left error
                            }
                            else //no error
                            {
                                utlResponseData lobjutlResponseData = MVVMHelperFunctions.DeserializeObject<utlResponseData>(larrLaunchScreenObject[0] as string);
                                if (lobjutlResponseData != null)
                                {
                                    ldictParams["PrimaryKey"] = lobjutlResponseData.KeysData[utlConstants.istrPrimaryKey];
                                }

                                vm = new ViewModelObj();
                                vm.DomainModel = lobjutlResponseData;
                                vm.ExtraInfoFields["FormId"] = lstrActiveForm;
                                vm.ExtraInfoFields["KeyField"] = vm.DomainModel.KeysData.ContainsKey("PrimaryKey") ? vm.DomainModel.KeysData["PrimaryKey"] : "0";
                                if (lblnBpmActivityInstance)
                                {
                                    var lstrEncryptedValue = HelperFunction.EncryptString("FromBPM", ldictParams[utlConstants.istrSessionID].ToString(), lstrActiveForm);
                                    vm.ExtraInfoFields["Allow"] = HttpUtility.UrlEncode(lstrEncryptedValue);
                                }
                                if ((lobjWorkflowActivityInfo.ienmPageMode == utlPageMode.New && vm.DomainModel.KeysData[utlConstants.istrPrimaryKey] == "0") || vm.DomainModel.KeysData.ContainsKey("RetainNewMode"))
                                {
                                    vm.ExtraInfoFields["IsNewForm"] = "true";
                                }
                                vm.DomainModel.OtherData["NavigationParams"] = lobjWorkflowActivityInfo.ihstLaunchParameters;
                                larrResponse.Add(vm); //Launch Screen object
                            }
                        }
                    }
                    else //Lookup Form
                    {
                        vm = new ViewModelObj();
                        vm.DomainModel = MVVMHelperFunctions.DeserializeObject<utlResponseData>(larrLaunchInfo[0] as string);
                        vm.ExtraInfoFields["FormId"] = astrFormID;
                        vm.ExtraInfoFields["KeyField"] = vm.DomainModel.KeysData.ContainsKey("PrimaryKey") ? vm.DomainModel.KeysData["PrimaryKey"] : "0";

                        larrResponse.Add(vm); //Launch Info

                        vm = new ViewModelObj();
                        ArrayList larrSearchCriteria = new ArrayList();

                        Hashtable lhstActInfo = new Hashtable();
                        busBpmActivityInstance lbusBaseActivityInstance = lobjWorkflowActivityInfo.ibusBaseActivityInstance as busBpmActivityInstance;
                        if (lbusBaseActivityInstance != null)
                        {
                            lhstActInfo.Add("ProcessName", lbusBaseActivityInstance?.ibusBpmProcessInstance?.ibusBpmProcess?.icdoBpmProcess?.description);
                            lhstActInfo.Add("ActivityName", lbusBaseActivityInstance?.ibusBpmActivity?.icdoBpmActivity?.name);
                            lhstActInfo.Add("ProcessInstanceId", lbusBaseActivityInstance?.ibusBpmProcessInstance?.icdoBpmProcessInstance?.process_instance_id);
                            lhstActInfo.Add("ActivityDetailsNavParams", HelperFunction.EncryptString($"aintactivityinstanceid=#{lbusBaseActivityInstance.icdoBpmActivityInstance.activity_instance_id}", utlConstants.istrMenuNavParamKey, utlConstants.istrFormNameNavParamKey));
                            larrSearchCriteria.Add(lhstActInfo);
                        }
                        if (lobjWorkflowActivityInfo.ihstLaunchParameters != null && lobjWorkflowActivityInfo.ihstLaunchParameters.Count > 0)
                        {
                            larrSearchCriteria.Add(lobjWorkflowActivityInfo.ihstLaunchParameters);
                        }
                        vm.DomainModel.OtherData["SearchCriteria"] = larrSearchCriteria;
                        vm.ExtraInfoFields["FormId"] = lstrActiveForm;
                        if (lblnBpmActivityInstance)
                        {
                            vm.ExtraInfoFields["ActivityInstanceType"] = "BPM";
                        }
                        else
                        {
                            vm.ExtraInfoFields["ActivityInstanceType"] = "WorkFlow";
                        }
                        vm.ExtraInfoFields["ActivityInstanceId"] = lintActivityInstanceId.ToString();
                        larrResponse.Add(vm); //Lookup Sceen
                    }

                    //Check if launched from My Basket screen then refresh center left object
                    if (((ViewModelObj)larrResponse[0]).ValidationSummary.Count == 0)
                    {
                        //-- send center left object
                        lstrActiveForm = lstrCenterLeftForm;
                        if (!string.IsNullOrEmpty(lstrActiveForm))
                        {
                            if (lblnRefreshCenterLeft && ((ViewModelObj)larrResponse[0]).ExtraInfoFields["FormId"] != lstrActiveForm)
                            {
                                XmlObject lobjFormXml = utlTemplateCacheClient.Get(lstrActiveForm);
                                if (lobjFormXml != null)
                                {
                                    int lintCenterLeftSecurityLevel = 0;
                                    int lintResource = lobjFormXml.GetAttributeValue<int>("sfwResource");
                                    lblnRefreshCenterLeft = MVVMHelperFunctions.HasAccess(lintResource, MVVMHelperFunctions.GetUserSecurityDetails(ldictParams), false, out lintCenterLeftSecurityLevel);
                                }
                            }
                            if (lblnRefreshCenterLeft)
                            {
                                Hashtable lhstCenterLeftLaunchParameters = new Hashtable();
                                lhstCenterLeftLaunchParameters["aintActivityInstanceID"] = lintActivityInstanceId;
                                string lstrNavParams = HelperFunction.EncryptNavigationParams(lhstCenterLeftLaunchParameters, lstrActiveForm, iobjSessionData.istrSessionId);

                                if (((ViewModelObj)larrResponse[0]).ExtraInfoFields["FormId"] != lstrActiveForm)
                                {
                                    vm = new ViewModelObj();
                                    vm.ExtraInfoFields["FormId"] = lstrActiveForm;
                                    vm.ExtraInfoFields["CenterLeftActivityInstanceId"] = lintActivityInstanceId.ToString();
                                    vm.ExtraInfoFields["CenterLeftNavParams"] = lstrNavParams;
                                    larrResponse.Add(vm);
                                }
                                else
                                {
                                    ((ViewModelObj)larrResponse[0]).ExtraInfoFields["CenterLeftActivityInstanceId"] = lintActivityInstanceId.ToString();
                                    ((ViewModelObj)larrResponse[0]).ExtraInfoFields["CenterLeftNavParams"] = lstrNavParams;
                                }
                            }
                        }
                        //-- center left object ends
                    }
                }
                var lResult = Request.CreateResponse(HttpStatusCode.OK, larrResponse);

                return lResult;
            }
            catch (Exception e)
            {
                return HandleGlobalError(e, AllParams);
            }
        }

        [HttpPost]
        public bool InactiveSelectedOrgContact(Dictionary<string, string> adictParameters)
        {
            bool lstrResult;
            isrvServers.ConnectToBT(istrSenderForm);
            string aintOrgIDs = "";
            int aintContactId = 0;
            if (!adictParameters.IsNullOrEmpty())
            {
                aintOrgIDs = adictParameters["aintOrgIDs"];
                aintContactId = Convert.ToInt32(adictParameters["aintContactId"]);
            }
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintOrgIDs", aintOrgIDs);
            lhstParam.Add("aintContactId", aintContactId);
            lstrResult = (bool)isrvServers.isrvBusinessTier.ExecuteMethod("InactiveSelectedOrgContact", lhstParam, false, ldictParams);
            return lstrResult;
        }
    }
}