//using Neo.Model;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using NeoSpin.BusinessObjects;
using System.Web.Script.Serialization;
using System.Text;
using NeoSpin.Common;
using NeoSpin.DataObjects;
using NeoSpinConstants;
using NeoSpin.Interface;
using Sagitec.BusinessObjects;

namespace Neo.Controllers
{
    /// <summary>
    /// Class HomeController
    /// </summary>
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
            if(!idictParams.ContainsKey(utlConstants.istrRequestApplicationName))
                idictParams[utlConstants.istrRequestApplicationName] = ConfigurationManager.AppSettings[utlConstants.istrRequestApplicationName];
            if(!idictParams.ContainsKey(utlConstants.istrRequestInvalidLoginFlag))
                idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";
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
                    this.idictParams.Add("UserID", iobjSessionData["UserId"].ToString());
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

        /// <summary>
        /// Submit File for upload
        /// </summary>
        /// <precondition>File should be selected</precondition>
        /// <postcondition>Uploads file to ECM</postcondition>
        /// <sideeffect></sideeffect>
        /// <param name="files">files</param>
        /// <param name="data">data</param>
        /// <returns>JsonResult</returns>
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

                        switch (lstrFormName){                            

                            case "wfmWssDocsUploadMaintenance":
                                try
                                {
                                    string lstrDocumentName = lobjResponseData.HeaderData["MaintenanceData"]["ddlWssDocumentTypes"].ToString();
                                    if (lstrDocumentName != String.Empty)
                                    {
                                        lhstParams.Add("astrDocumentId", lstrDocumentName);
                                        string lstrFileName = fileAttachment.FileName;
                                        lhstParams.Add("astrFileName", lstrFileName);
                                        lhstParams.Add("uploadedFileContent", larrBytes);
                                        ArrayList larrErrors = (ArrayList)this.isrvServers.isrvBusinessTier.ExecuteMethod("UploadWssDocs", lhstParams, false, ldictParams);
                                        if (larrErrors.Count > 0 && (larrErrors[0] is utlError))
                                        {
                                            lvmViewModel.ValidationSummary = new ArrayList();
                                            foreach (utlError lutlError in larrErrors.OfType<utlError>().ToList())
                                            {
                                                lvmViewModel.ValidationSummary.Add(lutlError);
                                            }
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

		//FW Upgrade :: wfmDefault.aspx.cs code conversion(btn_OpenPDF method), OpenPDFRender() method for render PDF file in new tab
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
