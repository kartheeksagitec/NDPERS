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
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NeoSpinMSS
{
    public partial class wfmLoginME : wfmMainDB
    {
        protected MVVMSession iobjSessionData;
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
                string lstrapplicationpath = Request.ApplicationPath;
                string lstrProjectAndRegion = ConfigurationManager.AppSettings["ProjectAndRegion"];

                utlUserInfo lobjUserInfo = (utlUserInfo)iobjSessionData["UserInfoObject"];

                string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
                IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                try
                {
                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("projectregion", lstrProjectAndRegion);
                    lhstParam.Add("apppath", lstrapplicationpath);

                    DataTable ldtResult = lsrvBusinessTier.ExecuteQuery("cdoCodeValue.GetRedirectValue", lhstParam, idictParams);

                    if (ldtResult.Rows.Count > 0)
                    {
                        iobjSessionData["NavUrl"] = ldtResult.Rows[0]["Comments"].ToString() + "wfmLoginME.aspx";
                        iobjSessionData["OrgUrl"] = ldtResult.Rows[0]["Comments"].ToString() + "wfmLoginME.aspx";
                        Framework.Redirect("wfmRedirectPage.aspx");
                    }
                    base.OnInit(e);
                }
                finally
                {
                    HelperFunction.CloseChannel(lsrvBusinessTier);
                }
                
            }
            catch (Exception ex)
            {
                Framework.Redirect("wfmMaintenance.aspx");
            }
        }

        string lstrCookieName = "WSSCookie";
        protected void Page_Load(object sender, EventArgs e)
        {
            NDLoginInstance.btnLoginClick += NDLogin_btnLoginClick;
            lblError.Visible = false;
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

                HttpCookie lcokKitsCookie = Request.Cookies.Get(lstrCookieName);

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
                iobjSessionData["RedirectOnLogoff"] = "wfmLoginME.aspx";
                lmdlLogin.LoginWindowName = hfldLoginWindowName.Value;
                //  Framework.istrWindowName = hfldLoginWindowName.Value;
                Sagitec.MVVMClient.AccountControllerBase.SetAntiForgeryToken(lmdlLogin);
                if(Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
                {
                    Framework.Redirect("~/Account/wfmLoginME");
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
            string lstrProfileUserId = string.Empty;
            Control lclcTextbox = NDLoginInstance.FindControl("txtLoginID");
            if (lclcTextbox is TextBox)
            {
                lstrProfileUserId = ((TextBox)lclcTextbox).Text;
            }
            NDLoginInstance.Authenticate(NDLogin.NDLogin.AuthTypes.NDLdapWS);                    
            //bool lboolIsLoginSuccess = false;
            //bool lboolIsUserLocked = false;
            string lstrIPAddress = string.Empty;

            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("astrNDLoginID", lstrProfileUserId);
                Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                ldctParams[utlConstants.istrConstUserID] = lstrProfileUserId;
                ldctParams[utlConstants.istrConstFormName] = "wfmLoginME.aspx";
                busPerson lobjPerson = new busPerson() { icdoPerson = new NeoSpin.CustomDataObjects.cdoPerson() };
                ldctParams[utlConstants.istrIsMVVM] = true;
                lobjPerson = (busPerson)lsrvBusinessTier.ExecuteMethod("LoadPersonByNDLoginID", lhstParam, false, ldctParams);

                //Hashtable lhstParamToSetUserLoginHistory = new Hashtable();
                //lhstParamToSetUserLoginHistory.Add("lobjPerson", lobjPerson);
                //lhstParamToSetUserLoginHistory.Add("astrProfileUserID", lstrProfileUserId);
                //PIR 18492 -Call IP Address function if java script function doesnot return location details.
                //As discussed with vasu,due to security concern below free third party api code is commented.
                //if (string.IsNullOrEmpty(hfldUserCountry.Value) && string.IsNullOrEmpty(hfldUserRegionName.Value) && string.IsNullOrEmpty(hfldUserCity.Value))
                //{
                //    //lstrIPAddress = GetIPAddress();
                //}
                //lhstParamToSetUserLoginHistory.Add("astrUserCountry", hfldUserCountry.Value);
                //lhstParamToSetUserLoginHistory.Add("astrUserRegionName", hfldUserRegionName.Value);
                //lhstParamToSetUserLoginHistory.Add("astrUserCity", hfldUserCity.Value);
                //lhstParamToSetUserLoginHistory.Add("astrIPAddress", lstrIPAddress);

                //Dictionary<string, object> ldctParamsToSetUserLoginHistory = new Dictionary<string, object>();

                if (NDLoginInstance.IsAuthenticated && lobjPerson.icdoPerson.is_user_locked != "Y")
                {
                    //wfmMainDB.SetAuthenticatedWindow();
                    UpdateLogInstance(NDLoginInstance.ProfileUserID, 0, busConstant.UserTypeMember, "ControlsTheme", wfmMainDB.istrApplicationName, NDLoginInstance.IsAuthenticated);

                    iobjSessionData["NDPERSLoginID"] = NDLoginInstance.ProfileUserID;
                    iobjSessionData["NDPERSEmailID"] = NDLoginInstance.ProfileEmail;
                    iobjSessionData["NDPERSFirstName"] = NDLoginInstance.ProfileFName;
                    iobjSessionData["NDPERSLastName"] = NDLoginInstance.ProfileLName;
                    iobjSessionData["RedirecOnLogoff"] = "wfmLoginME.aspx";
                    iobjSessionData["IsFromWhichPortal"] = "MSS";
                    idictParams["IsFromWhichPortal"] = "MSS";
                    idictParams["NDPERSEmailID"] = NDLoginInstance.ProfileEmail;
                    iobjSessionData["NDPERSEmailID"] = NDLoginInstance.ProfileEmail;
                    // Set the session timeout based on TIMS constants in cache - PIR 7723
                    string lstrTimeout = isrvDBCache.GetConstantValue("TIMS");
                    if (lstrTimeout != "")
                    {
                        int lintTimeout = Convert.ToInt32(lstrTimeout);
                        Session.Timeout = lintTimeout;
                    }
                    FormsAuthentication.RedirectFromLoginPage(NDLoginInstance.ProfileUserID, true);
                    if (lobjPerson.icdoPerson.person_id > 0)
                    {
                        iobjSessionData["PersonID"] = lobjPerson.icdoPerson.person_id;
                        idictParams["PersonID"] = lobjPerson.icdoPerson.person_id;
                        iobjSessionData["UserID"] = lobjPerson.icdoPerson.person_id.ToString();
                        iobjSessionData["UserSerialID"] = 0;
                        iobjSessionData["UserType"] = busConstant.UserTypeMember;
                        iobjSessionData["ColorScheme"] = "ControlsTheme";//For MSS Layout change
                        iobjSessionData["MSSDisplayName"] = lobjPerson.icdoPerson.istrMSSDisplayName;//For MSS Layout change
                        int lintUserSerialID = 0;
                        if (iobjSessionData["UserSerialID"] != null)
                            lintUserSerialID = Convert.ToInt32(iobjSessionData["UserSerialID"]);

                        string lstrUserID = lobjPerson.icdoPerson.person_id.ToString() ?? string.Empty;
                        if (iobjSessionData["UserID"] != null)
                            lstrUserID = Convert.ToString(iobjSessionData["UserID"]);

                        //csLoginWSSHelper.SetSessionVariables(lobjPerson.icdoPerson.ndpers_login_id,
                        //                                    lobjPerson.icdoPerson.last_name,
                        //                                    lobjPerson.icdoPerson.first_name,
                        //                                    (string)iobjSessionData["UserType"] ?? string.Empty,
                        //                                    (string)iobjSessionData["UserID"] ?? string.Empty,
                        //                                    lintUserSerialID,
                        //                                    (string)iobjSessionData["ColorScheme"] ?? string.Empty,
                        //                                    lobjPerson.icdoPerson.email_address, iobjSessionData);

                        //Setting the User Security
                        csLoginWSSHelper.SetUserSecurityForMember(lobjPerson.icdoPerson.person_id, lsrvBusinessTier, iobjSessionData);
                        idictParams["UserSecurity"] = iobjSessionData["UserSecurity"];
                        //ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), this);
                        //iobjSessionData["UserMenu"] = larrMenu;

                        iobjSessionData["EMAILWAIVERFLAG"] = lobjPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || lobjPerson.icdoPerson.email_waiver_flag == "N" ? "N" : "Y";
                        idictParams["EMAILWAIVERFLAG"] = lobjPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || lobjPerson.icdoPerson.email_waiver_flag == "N" ? "N" : "Y";

                        iobjSessionData["IsExternalUser"] = true; //PIR-18492 : For External user
                                                                  //Launching the Portal
                        string lstrURL = csLoginWSSHelper.GetLaunchURLforMemberPortal(lobjPerson.icdoPerson.person_id, lsrvBusinessTier, iobjSessionData, NDLoginInstance.ProfileEmail, true);
                        //Setting the Audit Trail
                        ldctParams[utlConstants.istrConstUserID] = "PERSLink ID " + lobjPerson.icdoPerson.person_id.ToString();

                        lsrvBusinessTier.StoreProcessLog("PERSLink ID " + lobjPerson.icdoPerson.person_id.ToString() + " - External User successfully logged in to WSS Member Portal", ldctParams);
                        iobjSessionData["Landing_Page"] = lstrURL;
                        iobjSessionData["InitialPage"] = lstrURL;
                        iobjSessionData["UserLoggedOn"] = "true";
                        iobjSessionData["IsExternalLogin"] = true;
                        idictParams["IsExternalLogin"] = true;
                        idictParams["UserLoggedOn"] = "true";
                        idictParams["IsfromMSSPortal"] = "true";
                        iobjSessionData["UserLoggedOn"] = "true";
                        Framework.SessionForWindow["UserLoggedOn"] = "true";
                        idictParams["PersonCertify"] = iobjSessionData["PersonCertify"];
                        Framework.istrWindowName = iobjSessionData[utlConstants.istrWindowName].ToString();
                        iobjSessionData["WindowName"] = Framework.istrWindowName;
                        iobjSessionData["dictParams"] = idictParams;
                        iobjSessionData.idictParams = idictParams;
                        //lboolIsLoginSuccess = true;
                        DataTable ldtbSystemManagement = null;
                        ldtbSystemManagement = isrvDBCache.GetDBSystemManagement();
                        string lstrBaseDirectory = Convert.ToString(ldtbSystemManagement.Rows[0]["base_directory"]);
                        iobjSessionData["base_directory"] = lstrBaseDirectory;
                        iobjSessionData["Region_value"] = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);
                        iobjSessionData["PopUpMessageForCertify"] = GetMessageText(lsrvBusinessTier, ldctParams, busConstant.PopUpMessageForCertify);
                        //lhstParamToSetUserLoginHistory.Add("lboolIsLoginSuccess", lboolIsLoginSuccess);
                        //lboolIsUserLocked = (bool)lsrvBusinessTier.ExecuteMethod("SetUserLoginHistory", lhstParamToSetUserLoginHistory, false, ldctParamsToSetUserLoginHistory);
                        SetSessionValidationCookie(idictParams[utlConstants.istrConstUserID].ToString());
                        string url1 = string.Empty;
                        if (lstrURL.Equals("wfmMSSSwitchMemberAccount"))
                        {
                            url1 = UrlHelper.GenerateUrl("Default", "wfmMSSSwitchMemberAccount", "Account", null, RouteTable.Routes, HttpContext.Current.Request.RequestContext, false);
                        }
                        else
                        {
                            url1 = UrlHelper.GenerateUrl("Default", "Index", "Home", null, RouteTable.Routes, HttpContext.Current.Request.RequestContext, false);
                            if (System.Configuration.ConfigurationManager.AppSettings["IsRootPath"] != null && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsRootPath"]))
                            {

                            }
                            else
                            {
                                url1 = url1.Substring(0, url1.Length - 1);
                                //if (!url1.EndsWith("/"))
                                //{
                                //    url1 = url1 + "/";
                                //}

                            }
                        }
                        Framework.Redirect(url1);
                    }
                    else
                    {
                        Framework.Redirect("wfmRequestOnlineAccessForMember.aspx");
                    }
                }
                else
                {
                    //lhstParamToSetUserLoginHistory.Add("lboolIsLoginSuccess", lboolIsLoginSuccess);
                    //if (lobjPerson.icdoPerson.person_id > 0)
                    //{
                    //    lboolIsUserLocked = (bool)lsrvBusinessTier.ExecuteMethod("SetUserLoginHistory", lhstParamToSetUserLoginHistory, false, ldctParamsToSetUserLoginHistory);
                    //}

                    Hashtable lhstParam1 = new Hashtable();
                    lhstParam1.Add("aintMsgId", busConstant.LoginErrorMessage);
                    lblError.Visible = true;
                    object lobjMsgText = lsrvBusinessTier.ExecuteMethod("LoadMessage", lhstParam1, false, ldctParams);
                    if (lobjMsgText is string)
                        lblError.Text = Convert.ToString(lobjMsgText);
                    //NDLoginInstance.ErrorMessage = "Invalid Login Id and/Or Password.";
                    //if (lboolIsUserLocked)
                    //{
                    //    lblError.Text = "Please contact NDPERS for assistance by calling (701) 328-3900.";
                    //    //NDLoginInstance.ErrorMessage = "Please contact NDPERS for assistance by calling (701) 328-3900.";
                    //}
                }
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
        }
        protected string GetMessageText(IBusinessTier lsrvBusinessTier, Dictionary<string, object> ldctParams, int aintMessageID)
        {
            Hashtable lhstParam1 = new Hashtable();
            lhstParam1.Add("aintMsgId", aintMessageID);
            object lobjMsgText = lsrvBusinessTier.ExecuteMethod("LoadMessage", lhstParam1, false, ldctParams);
            if (lobjMsgText is string)
                return Convert.ToString(lobjMsgText);
            else
                return string.Empty;
        }
        protected string AssignNewSessionID()
        {
            var Context = System.Web.HttpContext.Current;
            System.Web.SessionState.SessionIDManager manager = new System.Web.SessionState.SessionIDManager();
            string oldId = manager.GetSessionID(Context);
            //MVVMFramework.ClearSessionStore(oldId);
            string newId = manager.CreateSessionID(Context);
            bool isAdd = false, isRedir = false;
            manager.SaveSessionID(Context, newId, out isRedir, out isAdd);
            HttpApplication ctx = (HttpApplication)Context.ApplicationInstance;
            HttpModuleCollection mods = ctx.Modules;
            System.Web.SessionState.SessionStateModule ssm = (SessionStateModule)mods.Get("Session");
            System.Reflection.FieldInfo[] fields = ssm.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            SessionStateStoreProviderBase store = null;
            System.Reflection.FieldInfo rqIdField = null, rqLockIdField = null, rqStateNotFoundField = null;
            foreach (System.Reflection.FieldInfo field in fields)
            {
                if (field.Name.Equals("_store")) store = (SessionStateStoreProviderBase)field.GetValue(ssm);
                if (field.Name.Equals("_rqId")) rqIdField = field;
                if (field.Name.Equals("_rqLockId")) rqLockIdField = field;
                if (field.Name.Equals("_rqSessionStateNotFound")) rqStateNotFoundField = field;
            }
            object lockId = rqLockIdField.GetValue(ssm);
            if ((lockId != null) && (oldId != null)) store.ReleaseItemExclusive(Context, oldId, lockId);
            rqStateNotFoundField.SetValue(ssm, true);
            rqIdField.SetValue(ssm, newId);
            idictParams[utlConstants.istrSessionID] = newId;
            iobjSessionData.istrSessionId = newId;
            return newId;
        }
    }
}