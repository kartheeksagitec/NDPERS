using Neo;
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;


namespace Neo
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : BaseGlobal
    {        protected override void AfterApplication_Start()
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
            List<string> lstr = new List<string> { "CreateCookieForRegistration","IsEmailAddressNotWaived", "GetMSSUnreadMessagesCountNeo","OpenPDFFile", "SetPayeeAccountId", "checkSinglePlan", "GetBenOptionsByEffectiveDate1", "IsGeneratedOTPExpired", "btnVertifyOTP_Click",
                                                    "IsFromImageClick", "ValidateNew","GetBenefitAccountTypeByPlan","IsBankInfoAlreadyExists" };
            lstr.ForEach(lstrName =>
            {
                aBaseDelegatingHandler.AddMethodNameToSenderFormNotRequiredList(lstrName);
                aBaseDelegatingHandler.AddMethodNameToSenderIDNotRequiredList(lstrName);
            });
            // aBaseDelegatingHandler.AddMapForWebMethodsToAPIMethods("btnCustomMethod_Click", "CustomWebApiMethod");
        }
    }
}