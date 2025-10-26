using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

using Sagitec.Common;

using Sagitec.MVVMClient;
using System.Web.Helpers;
using System.Configuration;

namespace Neo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : BaseGlobal
    {
        protected override void AfterApplication_Start()
        {
            base.AfterApplication_Start();
            
            //AuthConfig.RegisterAuth();
            //ServiceHelper.Initialize(utlServiceType.Remoting);
            var larrAppJsBundles = ConfigurationManager.AppSettings["JsFilesForBundle"].Split(',').Select(s => "~/Scripts/App/" + s.Trim()).ToArray();
            MVVMBundleConfig.RegisterJSBundles(larrAppJsBundles);
            var larrAppCssBundles = ConfigurationManager.AppSettings["CssFilesForBundle"].Split(',').Select(s => "~/Styles/" + s.Trim()).ToArray();
            MVVMBundleConfig.RegisterCssBundles(BundleTable.Bundles, "~/bundles/AppSideCSS", larrAppCssBundles);

        }
        protected void Application_OnError()
        {
            Exception ex = HttpContext.Current.Server.GetLastError();

            if (ex.GetType().FullName == "System.Net.Sockets.SocketException")
            {
                if (((System.Net.Sockets.SocketException)ex).ErrorCode == 10061)
                {
                    Response.Redirect("~/GenericError.html");
                }
            }
        }
        //FMk 6.0.0.35.28?
        protected override void SetIgnoreListParamsForSecurityCheck(BaseDelegatingHandler aBaseDelegatingHandler)
        {
            base.SetIgnoreListParamsForSecurityCheck(aBaseDelegatingHandler);
            //List<string> lstr = new List<string> { "ExportDataExcel", "DownLoadAttachment", "GetUserName", "GetECMDocumentUrls", "GetProcessECMDocument", "GetTaxWithholding" };
            //lstr.ForEach(lstrName =>
            //{
            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("OpenESSForms");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("OpenESSForms");

            //aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetEmploymentChangeRequestID_Controller");
            //aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetEmploymentChangeRequestID_Controller");            

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("ViewEssReport_Click");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("ViewEssReport_Click");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("CreateRemittanceReport");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("CreateRemittanceReport");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("OpenPDFRender");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("OpenPDFRender");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("CreateRemittanceReportPath");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("CreateRemittanceReportPath");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("GetESSUnreadMessagesCountNeo");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("GetESSUnreadMessagesCountNeo");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("IsFromImageClick");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("IsFromImageClick");

            aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList("ValidateNew");
            aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList("ValidateNew");
            //});
            // aBaseDelegatingHandler.AddMapForWebMethodsToAPIMethods("btnCustomMethod_Click", "CustomWebApiMethod");
        }
    }
}