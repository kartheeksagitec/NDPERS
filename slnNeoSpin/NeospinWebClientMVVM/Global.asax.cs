using Sagitec.MVVMClient;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Optimization;

namespace Neo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode , 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : BaseGlobal
    {
        protected override void AfterApplication_Start()
        {
            base.AfterApplication_Start();
            AuthConfig.RegisterAuth();
            //ServiceHelper.Initialize(utlServiceType.Remoting);
            var larrAppJsBundles = ConfigurationManager.AppSettings["JsFilesForBundle"].Split(',').Select(s => "~/Scripts/App/" + s.Trim()).ToArray();
            MVVMBundleConfig.RegisterJSBundles(larrAppJsBundles);
            var larrAppCssBundles = ConfigurationManager.AppSettings["CssFilesForBundle"].Split(',').Select(s => "~/Styles/" + s.Trim()).ToArray();
            MVVMBundleConfig.RegisterCssBundles(BundleTable.Bundles, "~/bundles/AppCSS", larrAppCssBundles);
            IncludeCustomWebmethodsToAPIMethodsMap();
        }
        //F/W Upgrade  : LOB_Anti Forgery Issue.
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            if (exception is HttpAntiForgeryException || exception.Message.Contains("The provided anti-forgery token was meant for user"))
            {
                Response.Clear();
                Server.ClearError();
                Response.Redirect("~/Account/wfmLogin");
            }
        }

        /// <summary>
        /// List of methods for which to ignore security check
        /// </summary>
        /// <param name="aBaseDelegatingHandler"></param>
        protected override void SetIgnoreListParamsForSecurityCheck(BaseDelegatingHandler aBaseDelegatingHandler)
        {
            base.SetIgnoreListParamsForSecurityCheck(aBaseDelegatingHandler);

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("SetLogOut");                                // example for #1                  
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("getsystemregion");                            // example for #2         
            //aBaseDelegatingHandler.AddMapForWebMethodsToAPIMethods("btnCustomMethod_Click", "CustomWebApiMethod");       // example for #3            

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("getecmdocumenturls");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("getecmdocumenturls");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("viewecmdocument");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("viewecmdocument");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("annotateecmdocument");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("annotateecmdocument");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetNavParamsData");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetNavParamsData");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetCommTemplateInfo");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetCommTemplateInfo");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("FinishNonEditableCorrespondence");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("FinishNonEditableCorrespondence");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetGeneratedCommunicationInfo");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetGeneratedCommunicationInfo");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetDecryptedFilePath");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetDecryptedFilePath");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("InsertReportRequest");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("InsertReportRequest");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetReportDetail");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetReportDetail");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetGeneratedCorrInfo");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetGeneratedCorrInfo");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("EditCorrOnLocalTool");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("EditCorrOnLocalTool");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetCorrEnclosureInfo");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetCorrEnclosureInfo");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("SetHelpFile");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("SetHelpFile");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("DeletePIRAttachment");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("DeletePIRAttachment");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("RemoveWSSAccess");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("RemoveWSSAccess");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("btnIgnoreSelectedRows_Click");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("btnIgnoreSelectedRows_Click");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("PublishToWSS");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("PublishToWSS");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("CorrespondenceImage");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("CorrespondenceImage");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("InsertReportRequest");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("InsertReportRequest");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("ReturnFileNetURL");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("ReturnFileNetURL");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetFileNameImageURLs");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetFileNameImageURLs");
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("InactiveSelectedOrgContact");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("InactiveSelectedOrgContact");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("View1099RReport_Click");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("View1099RReport_Click");
            //PIR 25920 Droplist Refresh from DatePicker
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetADECAmountValuesByEffectiveDate");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetADECAmountValuesByEffectiveDate");
        }

        /// <summary>
        /// Place holder to load custom API methods for the mapping
        /// </summary>
        private void IncludeCustomWebmethodsToAPIMethodsMap()
        {
            BaseDelegatingHandler.idictWebMethodsToAPIMethods["btnRefreshServers_Click"].Add("RefreshAllAppServers");
        }
    }
}