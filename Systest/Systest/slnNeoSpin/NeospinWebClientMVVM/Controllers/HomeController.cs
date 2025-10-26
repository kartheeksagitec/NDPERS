using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace Neo.Controllers
{
    public class HomeController : AccountControllerBase
    {
        utlUserInfo iobjUserInfo = new utlUserInfo();
        
        #region[FS004 and FS005 START- File UPLOAD related Functions]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="files"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SubmitFile([System.Web.Http.FromBody]IEnumerable<HttpPostedFileBase> files, [System.Web.Http.FromBody] object data)
        {
            var jss = new JavaScriptSerializer();
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            ViewModelObj vmViewModel = new ViewModelObj();
            utlResponseData lobjResponseData = null;
            utlResponseMessage lobjResponseMessage = new utlResponseMessage();
            string strData = string.Empty;
            string lstrFormName = string.Empty;
            string lstrFileUploadPath = string.Empty;

            strData = ((string[])data)[0];

            lobjResponseData = (utlResponseData)jss.Deserialize<utlResponseData>(strData);
            if (lobjResponseData != null)
            {
                lstrFormName = lobjResponseData.istrFormName;
                if (files != null && files.Count() > 0)
                {
                    foreach (HttpPostedFileBase fileAttachment in files)
                    {
                        #region Set Initial Common Operation

                        string lstrFileInfoName = Path.GetFileName(fileAttachment.FileName);
                        int fileAttachmentLength = fileAttachment.ContentLength;
                        byte[] larrBytes = new byte[fileAttachmentLength];
                        fileAttachment.InputStream.Read(larrBytes, 0, fileAttachmentLength);

                        //Defaulting File Upload Limit to 10 MB if no setting found in web.config o.w. reads from web.config.
                        string lstrFileUploadLimit = System.Configuration.ConfigurationManager.AppSettings[UIConstants.FILE_UPLOAD_LIMIT];
                        int lintFileUploadLimit = 0;
                        if (!int.TryParse(lstrFileUploadLimit, out lintFileUploadLimit))
                        {
                            lintFileUploadLimit = 10485760;
                        }

                        //idictParams[utlConstants.istrConstFormName] = lstrFileInfoName;
                        isrvServers.ConnectToBT(lstrFormName);
                        Hashtable lhstParams = new Hashtable();

                        #endregion Set Initial Common Operation

                        switch (lstrFormName)
                        {
                            case UIConstants.UploadPIRAttachment:
                                try
                                {
                                    lstrFileInfoName = lstrFileInfoName.Replace("'", "");
                                    string lstrPIRID = lobjResponseData.HeaderData["MaintenanceData"]["PIRId"].ToString();
                                    lhstParams.Add("astrPIRID", lstrPIRID);
                                    lhstParams.Add("astrFileName", lstrFileInfoName);
                                    lhstParams.Add("afleByteArray", larrBytes);
                                    ArrayList larrErrors = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("UploadPIRAttachment", lhstParams, false, idictParams);
                                    if(larrErrors.Count > 0)
                                    {
                                        vmViewModel.ValidationSummary.Add((utlError)larrErrors[0]);
                                    }
                                }
                                catch (Exception _exc)
                                {
                                    throw _exc;
                                }
                                break;

                            case UIConstants.UploadFileMaintenance:
                                try
                                {
                                    if (fileAttachment != null)
                                    {
                                        string lstrFileType = Convert.ToString(lobjResponseData.HeaderData["MaintenanceData"]["ddlFileType"]);
                                        // Write data into a file
                                        if (lstrFileType != String.Empty)
                                            {
                                                
                                                lhstParams.Add("astrUserId", idictParams["UserID"]+"");
                                                lhstParams.Add("astrAgencyID", String.Empty);
                                                lhstParams.Add("aintFileType", Convert.ToInt32(lstrFileType));
                                                lhstParams.Add("astrFileName", lstrFileInfoName);
                                                lhstParams.Add("aBuffer", larrBytes);
                                                lhstParams.Add("astrMailFrom", "NeoSpin.UploadFile@sagitec.com");
                                                lhstParams.Add("astrEmailId", "jeeva.subburaj@sagitec.com");
                                                lhstParams.Add("astrSubject", lstrFileInfoName + " has been successfully received ");
                                                lhstParams.Add("astrMessage", lstrFileInfoName + " will soon be uploaded into NeoSpin system and all the transaction edits will be run. " +
                                                    "Once the upload and edits are completed you will be informed via email");

                                                ArrayList larrErrors = (ArrayList)isrvServers.isrvBusinessTier.ExecuteMethod("ValidateAndWriteToFile", lhstParams, false, idictParams);                                                
                                            }
                                            else
                                            {
                                                    utlError lutlError = new utlError
                                                    {
                                                        istrErrorMessage = "Please select the File Type."
                                                    };
                                                    vmViewModel.ValidationSummary.Add(lutlError);
                                                    //lblInfo.Text = "Please select the File Type.";
                                            }
                                        
                                    }
                                }
                                catch (Exception _exc)
                                {
                                    throw _exc;
                                }
                                break;                                                               
                        }
                    }
                }                
            }
            vmViewModel.DomainModel = new utlResponseData();
            vmViewModel.ResponseMessage = lobjResponseMessage;
            vmViewModel.ExtraInfoFields["FormId"] = lstrFormName;
            return Json(vmViewModel, JsonRequestBehavior.AllowGet);
        }
        

        #endregion[FS004 and FS005 END File UPLOAD related Functions]

        [HttpPost]
        public ActionResult OpenPDFRender()
        {
            var jss = new JavaScriptSerializer();
            Dictionary<string, string> ldctNavigationParams = jss.Deserialize<Dictionary<string, string>>(HttpContext.Request.Params["aobjDownload"].ToString());
            LoginModel lobjLoginModel = new LoginModel();
            byte[] lbytFileBytes = null;
            SetAntiForgeryToken(lobjLoginModel);
            if (iobjSessionData["dictParams"] != null)
            {
                Dictionary<string, object> dicParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
                lobjLoginModel.LoginWindowName = dicParams["WindowName"].ToString();
            }
            var istrSenderForm = HttpContext.Request.Params["SenderForm"];
            isrvServers.ConnectToBT(istrSenderForm);
            Hashtable lhstParam = new Hashtable();
            try
            {
                if (istrSenderForm == "wfmPersonOverviewMaintenance")
                {
                    string lstrMASSelectionID = ldctNavigationParams.First().Value;
                    string lstrReportName = ldctNavigationParams.Last().Value;
                    lhstParam.Add("aintSelectionID", Convert.ToInt32(lstrMASSelectionID));
                    lhstParam.Add("astrReportName", lstrReportName);
                    lbytFileBytes = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("CreateMemberAnnualStatement", lhstParam, false, idictParams);
                }
                if (istrSenderForm == "wfmViewRequestLifeEnrollmentMaintenanceLOB" ||
                    istrSenderForm == "wfmViewRequestPensionPlanDCRetirementEnrollmentMaintenanceLOB" ||
                    istrSenderForm == "wfmViewRequestPensionPlanMainRetirementOptionalEnrollmentMaintenanceLOB" ||
                    istrSenderForm == "wfmViewRequestPensionPlanDCOptionalEnrollmentMaintenanceLOB")
                {
                    string CorrPDFPath = isrvServers.isrvDbCache.GetPathInfo("CorrPdf");
                    string lstrReportName = ldctNavigationParams.First().Value;
                    string lstrFilePath = CorrPDFPath + "\\" + lstrReportName;
                    lhstParam.Add("astrFilePath", lstrFilePath);
                    lbytFileBytes = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("DownloadPIRAttachment", lhstParam, false, idictParams);
                }
                return File(lbytFileBytes, "application/pdf");
            }
            finally
            {
                HelperFunction.CloseChannel(isrvServers);
            }

        }

        [HttpGet]
        public ActionResult MapRender()
        {
            LoginModel objLoginModel = new LoginModel();

            SetAntiForgeryToken(objLoginModel);

            if (iobjSessionData["dictParams"] != null)
            {
                Dictionary<string, object> dicParams = (Dictionary<string, object>)iobjSessionData["dictParams"];

                objLoginModel.LoginWindowName = dicParams["WindowName"].ToString();

            }
            return View(objLoginModel);
        }

        [HttpGet]
        public ActionResult BPMNMap()
        {
            LoginModel objLoginModel = new LoginModel();

            SetAntiForgeryToken(objLoginModel);

            if (iobjSessionData["dictParams"] != null)
            {
                Dictionary<string, object> dicParams = (Dictionary<string, object>)iobjSessionData["dictParams"];

                objLoginModel.LoginWindowName = dicParams["WindowName"].ToString();

            }
            return View(objLoginModel);
        }


        [HttpGet]
        public ActionResult BPMNReadOnlyMap()
        {
            LoginModel objLoginModel = new LoginModel();

            SetAntiForgeryToken(objLoginModel);

            if (Session["dictParams"] != null)
            {
                Dictionary<string, object> dicParams = (Dictionary<string, object>)Session["dictParams"];

                objLoginModel.LoginWindowName = Convert.ToString(dicParams["WindowName"]);

            }
            return View(objLoginModel);
        }
    }
}
