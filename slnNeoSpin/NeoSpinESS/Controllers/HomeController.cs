using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Web.Script.Serialization;
using Sagitec.BusinessObjects;
using NeoSpinConstants;
using System.Web.Security;
using Sagitec.Interface;
using System.Configuration;
using NeoSpin.Interface;
using NeoSpin.BusinessObjects;

namespace Neo.Controllers
{
    public class HomeController : AccountControllerBase
    {
        /// <summary>
        /// Initializing Landing Page Parameters
        /// </summary>
        /// <returns>ActionResult</returns>
        [Authorize]
        public override ActionResult Index()
        {
            UiHelperFunction helperFunctions = new UiHelperFunction();
            helperFunctions.InitializeScreenSessionsAndGetLaunchURL();
            LoginModel model = new LoginModel();
            if (iobjSessionData != null && iobjSessionData["IsExternalLogin"] != null && Convert.ToBoolean(iobjSessionData["IsExternalLogin"]) == true)
            {               
                utlUserInfo lutlUserInfo = null;
                Hashtable lhstParams = new Hashtable();
               // int lintOrgContactID = 0;

                //isrvServers.ConnectToBT("wfmLoginESS");
                lutlUserInfo = (utlUserInfo)iobjSessionData["UserInfoObject"];
                iobjSessionData["UserInfoObject"] = lutlUserInfo;
                idictParams[utlConstants.istrRequestIPAddress] = GetIP();
                idictParams[utlConstants.istrConstPageMode] = utlPageMode.Update;
                if (!this.idictParams.ContainsKey("UserID"))
                {
                    this.idictParams.Add("UserID", Convert.ToString(iobjSessionData["UserId"]));
                }
                idictParams[utlConstants.istrClientDetails] = HelperFunction.GetClientDetailsByRequestObject(Request);
                if (!idictParams.ContainsKey(utlConstants.istrLogInfo) && System.Web.HttpContext.Current.Items.Contains("TraceInfo"))
                {
                    idictParams[utlConstants.istrLogInfo] = ((utlTraceInfo)System.Web.HttpContext.Current.Items["TraceInfo"]).iobjLogInfo;
                }
                LogInstance(iobjSessionData.istrSessionId);
                iobjSessionData.idictParams = idictParams;
                iobjSessionData["InitialPage"] = iobjSessionData["Landing_Page"];
            }
            
            model.iobjSessionData = iobjSessionData;
            SetAntiForgeryToken(model);
            
            return View(model);
        }

        [HttpPost]
        public JsonResult SubmitFile([System.Web.Http.FromBody]IEnumerable<HttpPostedFileBase> files, [System.Web.Http.FromBody] object data)
        {
            var lvarjss = new JavaScriptSerializer();
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)this.iobjSessionData["dictParams"];
            ViewModelObj lvmViewModel = new ViewModelObj();
            utlResponseData lobjResponseData = null;
            utlResponseMessage lobjResponseMessage = new utlResponseMessage();
            string lstrData = string.Empty;
            string lstrFormName = string.Empty;
            string lstrFileUploadPath = string.Empty;
            bool lblnFileUploadPath = false;

            try
            {
                lstrData = ((string[])data)[0];
            }
            catch (Exception E)
            {
                throw E;
            }

            lobjResponseData = (utlResponseData)lvarjss.Deserialize<utlResponseData>(lstrData);
            if (lobjResponseData != null)
            {
                lstrFormName = lobjResponseData.istrFormName;
                if (files != null && files.Count() > 0)
                {
                    foreach (HttpPostedFileBase fileAttachment in files)
                    {
                        string lstrFileInfoName = Path.GetFileName(fileAttachment.FileName);
                        int lintfileAttachmentLength = fileAttachment.ContentLength;
                        byte[] larrBytes = new byte[lintfileAttachmentLength];
                        fileAttachment.InputStream.Read(larrBytes, 0, lintfileAttachmentLength);

                        // Defaulting File Upload Limit to 10 MB if no setting found in web.config o.w. reads from web.config.
                        string lstrFileUploadLimit = System.Configuration.ConfigurationManager.AppSettings[UIConstants.FILE_UPLOAD_LIMIT];
                        int lintFileUploadLimit = 0;

                        if (!int.TryParse(lstrFileUploadLimit, out lintFileUploadLimit))
                        {
                            lintFileUploadLimit = 10485760;
                        }
                        else
                        {
                            //Logger.getInstance().LogInfo(LoggerConstants.LOG_INFO_EXPECTED_ELSE_CONDITION_PCLASS_PMETHOD_PCONDITION, "HomeController", "SubmitFile", "File Upload Limit found in web.config.");
                        }

                        this.idictParams[utlConstants.istrConstFormName] = lstrFileInfoName;
                        this.isrvServers.ConnectToBT(lstrFormName);
                        Hashtable lhstParams = new Hashtable();

                        switch (lstrFormName)
                        {
                            case "wfmUploadFileMaintenance":
                                try
                                {
                                    string lstrOrgCode = idictParams["OrgID"].ToString();
                                    string lstrFileType = lobjResponseData.HeaderData["MaintenanceData"]["ddlFileType"].ToString();

                                    if (lstrFileType != String.Empty)
                                    {
                                        //Hashtable lhstParams = new Hashtable();
                                        lhstParams.Add("astrUserId", idictParams["UserID"] + "");
                                        lhstParams.Add("aintFileType", Convert.ToInt32(lobjResponseData.HeaderData["MaintenanceData"]["ddlFileType"]));
                                        lhstParams.Add("astrFileName", lstrFileInfoName);
                                        lhstParams.Add("aintReferenceId", Convert.ToInt32(idictParams["OrgID"]));
                                        lhstParams.Add("aBuffer", larrBytes);
                                        lhstParams.Add("astrMailFrom", "NeoSpin.UploadFile@sagitec.com");
                                        lhstParams.Add("astrEmailId", "jeeva.subburaj@sagitec.com");
                                        lhstParams.Add("astrSubject", lstrFileInfoName + " has been successfully received ");
                                        lhstParams.Add("astrMessage", lstrFileInfoName + " will soon be uploaded into NeoSpin system and all the transaction edits will be run. " + "Once the upload and edits are completed you will be informed via email");
                                        //isrvBusinessTier.ExecuteMethod("WriteToFile", lhstParams, false, idictParams);
                                        lhstParams.Add("ablnAlwaysCreateFileHeader", true);
                                        //idictParams.Add(utlConstants.istrConstFormName, lstrFormName);
                                        ArrayList larrErrors = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("ValidateAndWriteToFile", lhstParams, false, idictParams);
                                        if (larrErrors.Count > 0 && !(larrErrors[0] is busFileHdr))
                                        {
                                            lvmViewModel.ValidationSummary = new ArrayList();
                                            foreach (utlError lutlError in larrErrors.OfType<utlError>().ToList())
                                            {
                                                lvmViewModel.ValidationSummary.Add(lutlError);
                                            }
                                            //lobjResponseMessage = UiHelperFunction.GetMessage(NeoConstant.File.FileUpload.Msg_File_Uploaderror);
                                        }
                                        idictParams["SubmittedDate"] = DateTime.Now;

                                        switch (lobjResponseData.HeaderData["MaintenanceData"]["ddlFileType"].ToString())
                                        {

                                            case "10":
                                                idictParams["SelectedBenefitVUPR"] = "RETR";
                                                break;

                                            case "11":
                                                idictParams["SelectedBenefitVUPR"] = "DEFF";
                                                break;

                                            case "12":
                                                idictParams["SelectedBenefitVUPR"] = "INSR";
                                                break;

                                            case "28":
                                                idictParams["SelectedBenefitVUPR"] = "PRCH";
                                                break;
                                        }

                                        //Framework.Redirect("~/wfmDefault.aspx?FormID=wfmCreateReportCompletionMaintenance");

                                    }

                                }
                                catch (Exception e)
                                {
                                    lobjResponseMessage = UiHelperFunction.GetMessage(99, new object[1] { e });
                                    lvmViewModel.DomainModel = new utlResponseData();
                                    lvmViewModel.ValidationSummary = new ArrayList();
                                }

                                break;                           

                            case "wfmESSUploadDocumentsMaintenance":
                                try
                                {
                                    string lstrFileName = fileAttachment.FileName;
                                    lhstParams.Add("astrFileName", lstrFileName);
                                    lhstParams.Add("uploadedFileContent", larrBytes);
                                    ArrayList larrErrors = (ArrayList)this.isrvServers.isrvBusinessTier.ExecuteMethod("UploadEssDocs", lhstParams, false, ldictParams);
                                    if (larrErrors.Count > 0 && (larrErrors[0] is utlError))
                                    {
                                        lvmViewModel.ValidationSummary = new ArrayList();
                                        foreach (utlError lutlError in larrErrors.OfType<utlError>().ToList())
                                        {
                                            lvmViewModel.ValidationSummary.Add(lutlError);
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    lobjResponseMessage = UiHelperFunction.GetMessage(99, new object[1] { e });
                                    lvmViewModel.DomainModel = new utlResponseData();
                                    lvmViewModel.ValidationSummary = new ArrayList();
                                }
                                break;
                        }
                    }
                }

            }

            lvmViewModel.DomainModel = new utlResponseData();
            lvmViewModel.ResponseMessage = lobjResponseMessage;
            lvmViewModel.ExtraInfoFields["FormId"] = lstrFormName;
            return this.Json(lvmViewModel, JsonRequestBehavior.AllowGet);
        }

        private IDBCache _isrvDBCache;
        public ActionResult GetReport()
        {
            byte[] lbytFileBytes = null;
            //_isrvDBCache = WCFClient<IDBCache>.CreateChannel(srvServers.istrConfigDBCacheURL);

            INeoSpinBusinessTier lsrvNeoSpinMSSBusinessTier = null;
            try
            {
                string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
                lsrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                PopulateBaseDirectory(lsrvNeoSpinMSSBusinessTier);
                string lstrBaseDirectory = Convert.ToString(iobjSessionData["base_directory"]);
                string ReportURL = lstrBaseDirectory + "ESSForms\\Contact_Us.pdf";
                Hashtable lhstParams = new Hashtable();
                lhstParams.Add("astrFilePath", ReportURL);
                if (lsrvNeoSpinMSSBusinessTier.ExecuteMethod("DownloadPIRAttachment", lhstParams, false, idictParams) is byte[])
                {
                    lbytFileBytes = (byte[])lsrvNeoSpinMSSBusinessTier.ExecuteMethod("DownloadPIRAttachment", lhstParams, false, idictParams);
                }
                return File(lbytFileBytes, "application/pdf");
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvNeoSpinMSSBusinessTier);
            }
        }
        
        public ActionResult OpenESSForms(string astrFileName, int aintESSFlag = 0)
        {
            string lstrReportURL;
            byte[] lbytFileBytes = null;
            try
            {
                _isrvDBCache = WCFClient<IDBCache>.CreateChannel(srvServers.istrConfigDBCacheURL);
                INeoSpinBusinessTier lsrvNeoSpinMSSBusinessTier = null;
                try
                {
                    string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
                    lsrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                    PopulateBaseDirectory(lsrvNeoSpinMSSBusinessTier);
                    string lstrBaseDirectory = Convert.ToString(iobjSessionData["base_directory"]);
                    if (astrFileName == "SFN-53176.pdf")
                        lstrReportURL = lstrBaseDirectory + "PDFCorrespondence\\Templates\\" + astrFileName;
                    else if (aintESSFlag == 1)
                        lstrReportURL = _isrvDBCache.GetPathInfo("ESSHELP") + astrFileName;
                    else
                        lstrReportURL = lstrBaseDirectory + "ESSForms\\" + astrFileName;
                    Hashtable lhstParams = new Hashtable();
                    lhstParams.Add("astrFilePath", lstrReportURL);
                    if (lsrvNeoSpinMSSBusinessTier.ExecuteMethod("DownloadPIRAttachment", lhstParams, false, idictParams) is byte[])
                    {
                        lbytFileBytes = (byte[])lsrvNeoSpinMSSBusinessTier.ExecuteMethod("DownloadPIRAttachment", lhstParams, false, idictParams);
                    }
                    return File(lbytFileBytes, "application/pdf");
                }
                finally
                {
                    HelperFunction.CloseChannel(lsrvNeoSpinMSSBusinessTier);
                }

            }
            catch (Exception e)
            {
                //throw new Exception($"Unable to connect to DatabaseCache server at address {istrConfigDBCacheURL}, following error occurred {e.Message}");
            }
            finally
            {
                HelperFunction.CloseChannel(_isrvDBCache);
            }

            return File(lbytFileBytes, "application/pdf");
        }
        
        [HttpGet]
        public ActionResult OpenPDFRender(string astrFileName)
        {
            LoginModel lobjLoginModel = new LoginModel();
            SetAntiForgeryToken(lobjLoginModel);
            if (iobjSessionData["dictParams"] != null)
            {
                Dictionary<string, object> dicParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
                lobjLoginModel.LoginWindowName = dicParams["WindowName"].ToString();
            }

			
            byte[] lbytFileBytes = null;
            INeoSpinBusinessTier lsrvNeoSpinMSSBusinessTier = null;
            try
            {
                string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
                lsrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                PopulateBaseDirectory(lsrvNeoSpinMSSBusinessTier);
                string lstrBasePath = Convert.ToString(iobjSessionData["base_directory"]);
                Hashtable lhstParams = new Hashtable();
                lhstParams.Add("astrFilePath", lstrBasePath + astrFileName);
                if (lsrvNeoSpinMSSBusinessTier.ExecuteMethod("DownloadPIRAttachment", lhstParams, false, idictParams) is byte[])
                {
                    lbytFileBytes = (byte[])lsrvNeoSpinMSSBusinessTier.ExecuteMethod("DownloadPIRAttachment", lhstParams, false, idictParams);
                }
                return File(lbytFileBytes, "application/pdf");
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvNeoSpinMSSBusinessTier);
            }

        }
        public void PopulateBaseDirectory(INeoSpinBusinessTier asrvNeoSpinMSSBusinessTier)
        {
            //Append OR condition for different session object
            if (string.IsNullOrEmpty(Convert.ToString(iobjSessionData["base_directory"])))
            {
                Hashtable lhstParams = new Hashtable();
                if (asrvNeoSpinMSSBusinessTier.ExecuteMethod("GetBaseDirectory", lhstParams, false, idictParams) is string)
                {
                    iobjSessionData["base_directory"] = (string)asrvNeoSpinMSSBusinessTier.ExecuteMethod("GetBaseDirectory", lhstParams, false, idictParams);
                }
            }
        }
    }
}
