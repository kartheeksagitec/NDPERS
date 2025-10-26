#region [Using directives]
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Web.Mvc;
#endregion [Using directives]

namespace Neo.Controllers
{
    /// <summary>
    /// Controller Neo.Controllers.AccountController
    /// </summary>
    public class AccountController : AccountControllerBase
    {
        //utlUserInfo iobjUserInfo = new utlUserInfo();
        //string lstrCookieName = "KitsCookie";
        /// <summary>
        /// Overridden BeforeLoginRedirect to bind footer with the application details
        /// </summary>
        /// <param name="astrReturnUrl"></param>
        /// <returns></returns>
        protected override string BeforeLoginRedirect(string astrReturnUrl)
        {
            GetFooterDetails();
            //iobjSessionData["Landing_Page"] = iobjSessionData["InitialPage"];
            return base.BeforeLoginRedirect(astrReturnUrl);
        }
        protected override void BeforeLogoutRedirect(out string astrUrl, out string astrController)
        {
            astrController = "Account";
            astrUrl = "wfmLogin";
        }        
        
        /// <summary>
        /// Method to get Parameters related to show on the footer of master page
        /// </summary>
        /// <returns></returns>
        private void GetFooterDetails()
        {
            Dictionary<string, object> ldicMasterParam = null;
            try
            {
                ldicMasterParam = new Dictionary<string, object>();
                string lstrProductVersion = isrvServers.isrvMetaDataCache.GetProductVersion();
                string lstrReleaseDate = "Framework : " + lstrProductVersion + ", Solution : " + Convert.ToString(ServiceHelper.idteReleaseDate);
                string lstrMachineName = HttpContext.Server.MachineName;
                ldicMasterParam.Add(UIConstants.RELEASE_DATE, lstrReleaseDate);
                ldicMasterParam.Add(UIConstants.REQUEST_MACHINE_NAME, lstrMachineName);

                IPAddress[] larrAddresslist = Dns.GetHostAddresses(Convert.ToString(GetIP()));
                if (larrAddresslist != null && larrAddresslist.Length > 0)
                {
                    ldicMasterParam.Add(UIConstants.REQUEST_IP_ADDRESS, Convert.ToString(larrAddresslist[0]));
                }

                ldicMasterParam.Add(UIConstants.REQUEST_APP_SERVER, ServiceHelper.iobjTraceInfo.istrAppServer);
                ldicMasterParam.Add(UIConstants.REGION, istrRegionValue);
                ldicMasterParam.Add(UIConstants.PRODUCT_REGION, istrProductRegion);
                if (idtApplicationDate != DateTime.MinValue)
                {
                    ldicMasterParam.Add(UIConstants.BATCH_DATE, idtApplicationDate.ToString());
                }
                else
                {
                    ldicMasterParam.Add(UIConstants.BATCH_DATE, DateTime.Now.ToString());
                }
                iobjSessionData[UIConstants.DICT_MASTER_PARAMS] = ldicMasterParam;
            }
            finally
            {
                ldicMasterParam = null;
            }
        }

        

        /// <summary>
        /// Login
        /// </summary>
        /// <precondition>
        /// </precondition>
        /// <postcondition>
        /// </postcondition>
        /// <param name="astrReturnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult wfmLogin(string astrReturnUrl)
        {
            base.Login(astrReturnUrl);
            //imdlLogin.LoginWindowName = iobjSessionData?[utlConstants.istrWindowName]?.ToString();
            return View(imdlLogin);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult wfmLogin(LoginModel model, string astrReturnUrl)
        {
            if (string.IsNullOrEmpty(model.UserName))
            {
                model.Message += "Username Required.";
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                model.Message += "<br> Password Required.";
            }
            if (!string.IsNullOrEmpty(model.Message)) return View(model);
            SetAntiForgeryToken(model);
            Hashtable lhstParams = new Hashtable();
            lhstParams.Add("astrKey", null);
            lhstParams.Add("astrValue", model.Password);
            string lstrEncryptedPassword = (string)this.isrvServers.isrvBusinessTier.ExecuteMethod("SagitecEncrypt", lhstParams, false, idictParams);
            iobjSessionData["AccessDenied"] = lstrEncryptedPassword;
            return base.Login(model, astrReturnUrl);
            
        }        
    }
}
