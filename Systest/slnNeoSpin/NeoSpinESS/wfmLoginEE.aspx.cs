using NeoSpin.BusinessObjects;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;
using Sagitec.WebClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NeoSpinESS
{
    public partial class wfmLoginEE : wfmClientBasePage
    {
        protected override void OnPreInit(EventArgs e)
        {
            iobjSessionData = new MVVMSession(Session.SessionID);
            ihflLoginWindowName = (HiddenField)GetControl(this, "hfldLoginWindowName");
            if (ihflLoginWindowName == null && Master != null)
            {
                ihflLoginWindowName = (HiddenField)Master.FindControl("hfldLoginWindowName");
            }
            if (ihflLoginWindowName != null)
            {
                ihflLoginWindowName.Value = iobjSessionData["WindowName"] != null ? iobjSessionData["WindowName"].ToString() : Guid.NewGuid().ToString();
            }
            Framework.istrWindowName = null;
            base.OnPreInit(e);
            Framework.istrWindowName = iobjSessionData["WindowName"] != null ? iobjSessionData["WindowName"].ToString() : Guid.NewGuid().ToString();
        }
        protected override void OnInit(EventArgs e)
        {
            istrFormName = "wfmLoginWSS";
            Session["IsNewSession"] = "false";
            try
            {
                base.OnInit(e);
            }
            catch (Exception ex)
            {
                Framework.Redirect("wfmMaintenance.aspx");
            }
        }

        //string lstrCookieName = "PERSLinkWSSSession";
        protected override void Page_Load(object sender, EventArgs e)
        {            
            NDLoginInstance.btnLoginClick += NDLogin_btnLoginClick;
            
            if (!this.IsPostBack)
            {
                LoginModel lmdlLogin = new LoginModel();
                Session.Clear();
                FormsAuthentication.SignOut();
                idictParams?.Clear();
                iobjTraceInfo.iobjLogInfo.iintLogInstanceID = 0;
                iobjSessionData.idictParams.Clear();
                iobjSessionData.idictParams = null;
                hfldLoginWindowName.Value = "";
                Framework.istrWindowName = null;
                DeleteSessionValidationCookie();
                // Clear authentication cookie
                HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                rFormsCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(rFormsCookie);

                HttpCookie lcokKitsCookie = Request.Cookies.Get(EXTERNAL_UESR_NAME_COOKIE);

                //Session["WindowName"] = hfldLoginWindowName.Value;
                if (string.IsNullOrEmpty(hfldLoginWindowName.Value))
                {
                    hfldLoginWindowName.Value = Guid.NewGuid().ToString();
                }
                SetNoCache();
                AssignNewSessionID();
                iobjSessionData[utlConstants.istrWindowName] = hfldLoginWindowName.Value;
                iobjSessionData.idictParams[utlConstants.istrWindowName] = hfldLoginWindowName.Value;
                idictParams[utlConstants.istrWindowName] = hfldLoginWindowName.Value;
                // Framework.istrWindowName = hfldLoginWindowName.Value;
                iobjSessionData["RedirectOnLogoff"] = "wfmLoginEE.aspx";
                lmdlLogin.LoginWindowName = hfldLoginWindowName.Value;
                //  Framework.istrWindowName = hfldLoginWindowName.Value;
                Sagitec.MVVMClient.AccountControllerBase.SetAntiForgeryToken(lmdlLogin);
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
                {
                    Framework.Redirect("~/Account/wfmLoginEE");
                }
            }
        }

        protected virtual void DeleteSessionValidationCookie()
        {
            HttpCookie lstrMVVMSessionAuthenticationCookie = Request.Cookies.Get(istrApplicationName + utlConstants.istrMVVMSessionValidationCookieKey);
            if (lstrMVVMSessionAuthenticationCookie != null)
            {
                lstrMVVMSessionAuthenticationCookie.Expires = DateTime.Now.AddYears(-1);
                Response.Cookies.Add(lstrMVVMSessionAuthenticationCookie);
            }
        }

        /// <summary>
        /// Method to set session validation cookie - This helps to identify if session is in proc and app pool / iis recycles
        /// </summary>
        protected virtual void SetSessionValidationCookie(string astrUserId)
        {
            HttpCookie lstrMVVMSessionAuthenticationCookie = new HttpCookie(istrApplicationName + utlConstants.istrMVVMSessionValidationCookieKey);
            lstrMVVMSessionAuthenticationCookie.HttpOnly = true;
            lstrMVVMSessionAuthenticationCookie.Value = HelperFunction.SagitecEncrypt(null, astrUserId);
            Response.Cookies.Add(lstrMVVMSessionAuthenticationCookie);
        }
        protected void NDLogin_btnLoginClick(object sender, EventArgs args)
        {
            NDLoginInstance.Authenticate(NDLogin.NDLogin.AuthTypes.NDLdapWS);            
            if (NDLoginInstance.IsAuthenticated)
            {
                //wfmMainDB.SetAuthenticatedWindow();
                UpdateLogInstance(NDLoginInstance.ProfileUserID, 0, busConstant.UserTypeEmployer, string.Empty, wfmMainDB.istrApplicationName, NDLoginInstance.IsAuthenticated);


                //This Session needs to be set here to pass the value to Request Online Page  
                string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
                IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                try
                {
                    iobjSessionData["NDPERSLoginID"] = NDLoginInstance.ProfileUserID;
                    iobjSessionData["NDPERSEmailID"] = NDLoginInstance.ProfileEmail;
                    iobjSessionData["NDPERSFirstName"] = NDLoginInstance.ProfileFName;
                    iobjSessionData["NDPERSLastName"] = NDLoginInstance.ProfileLName;
                    iobjSessionData["RedirecOnLogoff"] = "wfmLoginEE.aspx";
                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("astrNDLoginID", NDLoginInstance.ProfileUserID);
                    iobjSessionData["IsExternalLogin"] = true;
                    iobjSessionData["IsFromWhichPortal"] = "ESS";
                    idictParams["IsFromWhichPortal"] = "ESS";
                    iobjSessionData["NDPERSEmailIDESS"] = NDLoginInstance.ProfileEmail;
                    idictParams["NDPERSEmailIDESS"] = NDLoginInstance.ProfileEmail;
                    Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                    ldctParams[utlConstants.istrConstUserID] = NDLoginInstance.ProfileUserID;
                    ldctParams[utlConstants.istrConstFormName] = "wfmLoginEE.aspx";
                    DataTable ldtbSystemManagement = null;
                    ldtbSystemManagement = isrvDBCache.GetDBSystemManagement();
                    string lstrBaseDirectory = Convert.ToString(ldtbSystemManagement.Rows[0]["base_directory"]);
                    iobjSessionData["base_directory"] = lstrBaseDirectory;
                    iobjSessionData["Region_value"] = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);
                    busContact lobjContact = (busContact)lsrvBusinessTier.ExecuteMethod("LoadContactByNDLoginID", lhstParam, false, ldctParams);

                    // Set the session timeout based on TIWS constants in cache
                    string lstrTimeout = isrvDBCache.GetConstantValue("TIWS");
                    if (lstrTimeout != "")
                    {
                        int lintTimeout = Convert.ToInt32(lstrTimeout);
                        Session.Timeout = lintTimeout;
                    }
                    FormsAuthentication.RedirectFromLoginPage(NDLoginInstance.ProfileUserID, true);
                    SetSessionValidationCookie(idictParams[utlConstants.istrConstUserID].ToString());
                    if (lobjContact.icdoContact.contact_id > 0)
                    {
                        Hashtable lhstParamForContact = new Hashtable();
                        DataTable idtbEmployer = new DataTable();
                        lhstParamForContact.Add("aintContactID", lobjContact.icdoContact.contact_id);
                        idtbEmployer = (DataTable)lsrvBusinessTier.ExecuteMethod("GetEmployersForContact", lhstParamForContact, false, new Dictionary<string, object>());
                        //InitializeCookies();
                        if (idtbEmployer.Rows.Count >= 1)
                        {
                            iobjSessionData["ContactID"] = lobjContact.icdoContact.contact_id;
                            iobjSessionData["UserID"] = string.Empty;
                            iobjSessionData["UserSerialID"] = 0;
                            iobjSessionData["UserType"] = busConstant.UserTypeEmployer;
                            iobjSessionData["ColorScheme"] = string.Empty;
                            iobjSessionData["UserLoggedOn"] = "true";
                            Framework.Redirect("wfmEmployerSelection.aspx");
                        }
                        else
                        {
                            Framework.Redirect("wfmRequestOnlineAccessForContact.aspx");
                        }
                    }
                    else
                    {
                        Framework.Redirect("wfmRequestOnlineAccessForContact.aspx");
                    }
                }
                finally
                {
                    HelperFunction.CloseChannel(lsrvBusinessTier);
                }
                
            }
            else
            {
                NDLoginInstance.CustomErrorMsg = "Authentication failed!";
            }
        }
        //private void InitializeCookies()
        //{
        //    //External user name
        //    HttpCookie lcokMWPCookie = new HttpCookie(EXTERNAL_UESR_NAME_COOKIE);
        //    lcokMWPCookie.Expires = Convert.ToDateTime("1/1/2050");
        //    lcokMWPCookie.Value = lstrCookieName;
        //    lcokMWPCookie.HttpOnly = true;
        //    //Check if a cookie with the same name exists
        //    if (Response.Cookies.Get(EXTERNAL_UESR_NAME_COOKIE) != null)
        //    {
        //        Response.Cookies.Remove(EXTERNAL_UESR_NAME_COOKIE);
        //    }
        //    Response.Cookies.Add(lcokMWPCookie);

        //    //Store the usertype in the cookie for future use
        //    lcokMWPCookie = new HttpCookie(EXTERNAL_USER_TYPE_COOKIE);
        //    lcokMWPCookie.Expires = Convert.ToDateTime("1/1/2050");
        //    lcokMWPCookie.Value = busConstant.UserTypeEmployer;
        //    lcokMWPCookie.HttpOnly = true;
        //    //Check if a cookie with the same name exists
        //    if (Response.Cookies.Get(EXTERNAL_USER_TYPE_COOKIE) != null)
        //    {
        //        Response.Cookies.Remove(EXTERNAL_USER_TYPE_COOKIE);
        //    }
        //    Response.Cookies.Add(lcokMWPCookie);

        //    //Store the login page in the cookie for future use   
        //    HttpCookie lcokLoginPageCookie = new HttpCookie(UIConstants.LoginPageCookie);
        //    lcokLoginPageCookie.Expires = Convert.ToDateTime("1/1/2050");
        //    lcokLoginPageCookie.Value = "wfmLoginE.aspx";
        //    lcokLoginPageCookie.HttpOnly = true;

        //    //Check if a cookie with the same name exists
        //    if (Response.Cookies.Get(UIConstants.LoginPageCookie) != null)
        //    {
        //        Response.Cookies.Remove(UIConstants.LoginPageCookie);
        //    }
        //    Response.Cookies.Add(lcokLoginPageCookie);
        //}
    }
}