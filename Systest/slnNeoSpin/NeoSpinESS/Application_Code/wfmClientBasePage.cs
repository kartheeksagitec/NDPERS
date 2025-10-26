using System;
using System.Web;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using Sagitec.Interface;
using Sagitec.Common;
using Sagitec.WebClient;
using NeoSpin.Interface;
using System.Collections.Generic;
using Sagitec.MVVMClient;
using System.Web.SessionState;
using System.Reflection;

/// <summary>
/// Summary description for ClientBasePage.
/// </summary>
public class wfmClientBasePage : wfmMainDB
{
    #region private variables declaration section
    /**
		* Declare all private variables in this section
		* */
    //the application configuration reader

    private const string FINGER_PRINT_BROWSER = "fp_browser";
    private const string FINGER_PRINT_SCREEN = "fp_screen";
    private const string FINGER_PRINT_SOFTWARE = "fp_software";
    private const string FINGER_PRINT_TIMEZONE = "fp_timezone";
    private const string FINGER_PRINT_LANGUAGE = "fp_language";

    protected utlUserInfo iutlESSUserInfo;
    private Label lblExternalMessage;
    private Label lblInternalMessage;
    private static Hashtable _ihstAppSettings = new Hashtable();
    protected MVVMSession iobjSessionData;

    #endregion

    #region public variables declaration section

    public const string EXTERNAL_LAGERS_UESR_NAME_COOKIE = "CalSTRSLAGERSCookie";
    //public const string EXTERNAL_UESR_NAME_COOKIE = "EWPExternalUserName";
    public const string EXTERNAL_UESR_NAME_COOKIE = "ESSExternalUserName";
    public const string EXTERNAL_USER_TYPE_COOKIE = "NeoSpinUserType";

    //org name
    public readonly string ORG_NAME;

    //usergroup name
    public readonly string USERGROUP_NAME;

    //The passmark WSDL URL for both administration and authentication services
    public enum enmAuthServicesRequest { Enroll, Set, Get, SignIn }

    //Allow IP Address Emulation
    public bool ALLOW_IP_EMULATION = false;
    public string istrCalSTRSProblemMessage = "";

    #endregion

    #region cookie and session settings
    private int _iintCookieExpirationTime = 0;
    private int _iintSessionExpirationTime = 0;
    #endregion

    #region Required Methods

    /// <summary>
    /// wfmClientBasePage Constructor
    /// </summary>
    public wfmClientBasePage()
    {
        //Web configuration appsettings reader
        try
        {
            _iintCookieExpirationTime = (ConfigurationManager.AppSettings["cookieExpirationTime"] != null) ? Convert.ToInt32(ConfigurationManager.AppSettings["cookieExpirationTime"]) : 28800;
            _iintSessionExpirationTime = (ConfigurationManager.AppSettings["sessionExpirationTime"] != null) ? Convert.ToInt32(ConfigurationManager.AppSettings["sessionExpirationTime"]) : 20;
        }
        catch (Exception exception)
        {
            throw exception;
        }
    }

    /// <summary>
    /// Load Method
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (Session != null)
        {
            object LogOffClick = iobjSessionData["LogoffClick"];// "LogoffClick"];
            if (LogOffClick != null)
            {
                //if ( iobjSessionData["RedirectOnLogoff"] != null)
                if (iobjSessionData["RedirectOnLogoff"] != null)
                    //this.RedirectTo( iobjSessionData["RedirectOnLogoff"].ToString());
                    this.RedirectTo(Convert.ToString(iobjSessionData["RedirectOnLogoff"]));
            }
        }

    }

    /// <summary>
    /// OnInit method
    /// </summary>
    /// <param name="e"></param>
    override protected void OnInit(EventArgs e)
    {
        iobjSessionData = new MVVMSession(Session.SessionID);
        //Clear iobjSessionData when user comes on wfmLoginE.aspx page as it is causing issue when user comes on wfmLoginE.page without logout.
        //if (this is LoginPages_wfmLoginE)
        //    //iobjSessionData.Clear();
        //    if (Master != null)
        //    {
        //        Master.Attributes.Add("DoServerProcessing", "N");
        //    }
        if (iobjSessionData != null)
        {
            if (Session.IsNewSession)
            {
                string lstrCookieHeader = Request.Headers["Cookie"];
                if ((null != lstrCookieHeader) && (lstrCookieHeader.IndexOf("ASP.NET_SessionId") >= 0))
                {
                    Response.Redirect("SessionExpirePage.htm");
                }
            }
        }
        base.OnInit(e);
    }

    /// <summary>
    /// Page_Load method
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void Page_Load(object sender, EventArgs e)
    {
        Page.ClientScript.RegisterStartupScript(this.GetType(), "JsFunc", "window.history.forward(); window.history.forward(4);", true);
        //Get info from session
        iutlESSUserInfo = (utlUserInfo)iobjSessionData["EwpUserInfoObject"];
        if (iutlESSUserInfo == null)
        {
            iutlESSUserInfo = new utlUserInfo();
            iutlESSUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.NotYetSignedIn;
        }
        try
        {
            if (iutlESSUserInfo.istrPassmarkMessage == null ||
                iutlESSUserInfo.FullName == null ||
                iutlESSUserInfo.istrPassmarkId == null ||
                iutlESSUserInfo.istrAgencyPlan == null)
            {
                iutlESSUserInfo = new utlUserInfo();
                iutlESSUserInfo.ienuEWPUserStatus = sfwEWPUserStatus.NotYetSignedIn;
            }

        }
        catch (Exception exp)
        {
            WriteEWPTrace("PROBLEM {wfmClientBasePage}: Page_Load()-." + exp.Message);
        }
        lblExternalMessage = (Label)GetControl(Page, "lblExternalMessage");
        lblInternalMessage = (Label)GetControl(Page, "lblInternalMessage");
        //Clear the message
        if (lblExternalMessage != null) lblExternalMessage.Text = "";
    }

    /// <summary>
    /// Reset Cookies.
    /// </summary>
    protected void ResetCookies()
    {
        Response.Cookies.Remove(EXTERNAL_UESR_NAME_COOKIE);
        Response.Cookies.Remove(EXTERNAL_USER_TYPE_COOKIE);
    }

    /// <summary>
    /// Set user security
    /// </summary>
    protected void SetUserSecurity()
    {
        iobjSessionData["EwpUserInfoObject"] = iutlESSUserInfo;
        IBusinessTier lsrvBusinessTier = GetNeoSpinESSBusinessTier();
        try
        {
            // Getting security details
            Hashtable lhstParameter = new Hashtable();
            //lhstParameter.Add("aintEWPID", Convert.ToInt32(iutlESSUserInfo.iintEWPId));
            //lhstParameter.Add("aintOrgID", Convert.ToInt32(iutlESSUserInfo.iintOrgId));
            lhstParameter.Add("aintOrgContactID", iutlESSUserInfo.iintReferenceID);
            iobjSessionData["UserSecurity"] = (DataTable)lsrvBusinessTier.ExecuteMethod("GetESSSecurityForExternalUser", lhstParameter, false, this.idictParams);
        }
        finally
        {
            HelperFunction.CloseChannel(lsrvBusinessTier);
        }

        // DataTable ldtbSecurity = (DataTable)isrvServers.isrvBusinessTier.ExecuteMethod("GetESSSecurityForExternalUser", lhstParameter, false, idictParams);

        iobjSessionData["UserID"] = iutlESSUserInfo.istrEmailId;
        iobjSessionData["UserSerialID"] = iutlESSUserInfo.iintUserSerialId;
        iobjSessionData["UserName"] = Convert.ToString(new StringBuilder(iutlESSUserInfo.istrLastName).Append(", ").Append(iutlESSUserInfo.istrFirstName));
        iobjSessionData["UserInfo"] = isrvDBCache.GetUserInfo(iutlESSUserInfo.istrUserId);

        //Session Handled by logging the last page accessed time
        SetSessionTimeoutInfo();

        iobjSessionData["ColorScheme"] = iutlESSUserInfo.istrColorScheme;
        iobjSessionData["RedirectOnLogoff"] = "wfmLoginE.aspx";
    }

    /// <summary>
    /// Set the user session time out
    /// </summary>
    protected void SetSessionTimeoutInfo()
    {
        PutInSession(DateTime.Now, "LastTimePageAccessed");
        PutInSession(_iintSessionExpirationTime, "SessionExpirationTime");
    }

    /// <summary>
    /// Set the userinfo object in session
    /// </summary>
    protected void SetUserInfoObject()
    {
        if (iutlESSUserInfo == null)
            return;
        iobjSessionData["EwpUserInfoObject"] = iutlESSUserInfo;
        SetMessages();
    }

    /// <summary>
    /// Set the message
    /// </summary>
    protected void SetMessages()
    {
        if (lblExternalMessage != null)
        {
            lblExternalMessage.Text = iutlESSUserInfo.istrMessage;
        }
        //On some condition enable and display the internal message too
        if (lblInternalMessage != null)
        {
            lblInternalMessage.Text = iutlESSUserInfo.istrPassmarkMessage;
            lblInternalMessage.Visible = false;
        }
    }

    /// <summary>
    /// Writing trace information
    /// </summary>
    /// <param name="astrMessage"></param>
    protected void WriteEWPTrace(string astrMessage)
    {
        //if (iutlESSUserInfo != null)
        //{
        //    if (iutlESSUserInfo.istrTraceFlag == null) return;
        //    if (iutlESSUserInfo.istrTraceFlag != NeoSpinConstants.NeoConstant.EWP_MWP.FLAG_YES) return;
        //}
        //else
        //    return;

        //iutlESSUserInfo.istrTraceInfo = istrFormName + " - " + astrMessage;
        ////Also store the message in the user table for informational purposes
        //INeoSpinBusinessTier isrvNeoSpinBusinessTier = GetNeoSpinESSBusinessTier();
        //Hashtable lhstParameter = new Hashtable();
        //lhstParameter.Add("aobjUserInfo", iutlESSUserInfo);
        ////isrvNeoSpinBusinessTier.ExecuteMethod("WriteEWPTrace", lhstParameter, false, this.idictParams);
    }

    /// <summary>
    /// Get business tier
    /// </summary>
    /// <returns></returns>
    //protected IMetaDataCache GetMetaDataCache()
    //{
    //    string lstrUrl = ConfigurationManager.AppSettings["MetaDataCacheUrl"];
    //    //isrvMetaDataCache = (INeoSpinMetaDataCache)Activator.GetObject(typeof(INeoSpinMetaDataCache), lstrUrl);
    //    return isrvMetaDataCache;
    //}

    /// <summary>
    /// Get business tier
    /// </summary>
    /// <returns></returns>
    protected IDBCache GetDBCache()
    {
        return isrvDBCache;
    }

    /// <summary>
    /// Get MSS business tier
    /// </summary>
    /// <returns></returns>
    protected IBusinessTier GetNeoSpinESSBusinessTier()
    {
        string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvESS");
        return WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
    }

    /// <summary>
    /// All redirects in this sample application are centralized 
    /// The redirection can be changed to server.transfer if need be at one go.
    /// Also, response.redirect throws an uncatchable and very annoying exception, 
    /// the exception can be handled here instead of everywhere
    /// </summary>
    /// <param name="astrFilePath"></param>
    public void RedirectTo(string astrFilePath)
    {
        WriteEWPTrace("RedirectTo requested. To:" + astrFilePath);
        //We prefer response.redirect over server.transfer
        //Do not end the Httpresponse, because it throws an uncatchable & annoying ThreadAbortException
        SetUserInfoObject();
        try
        {
            Response.Redirect(astrFilePath);
        }
        catch (ThreadAbortException)
        {
            //reset the thread abort to supress the exception.
            Thread.ResetAbort();
        }
    }

    /// <summary>
    /// Saves the response data in Session
    /// </summary>
    /// <param name="response"></param>
    /// <param name="astrKey"></param>
    public void PutInSession(object anySerializableObject, string astrKey)
    {
        try
        {
            //Store the data object in session
            iobjSessionData[astrKey] = anySerializableObject;
        }
        catch (Exception ex)
        {
            iutlESSUserInfo.istrMessage = istrCalSTRSProblemMessage;
            iutlESSUserInfo.istrMessageInternal = "PutInSession key=" + astrKey + ex.Message;
            //this.RedirectTo(NeoSpinConstants.NeoConstant.EWP_MWP.WFMPAGENAMES.LOGINPAGES_WFMLOGINERRORPAGE);
        }
    }

    /// <summary>
    /// Gets the saved response data object from Session
    /// </summary>
    /// <param name="response"></param>
    /// <param name="astrKey"></param>
    public object GetFromSession(string astrKey)
    {
        try
        {
            //returned the stored object from session
            return iobjSessionData[astrKey];
        }
        catch (Exception ex)
        {
            iutlESSUserInfo.istrMessage = istrCalSTRSProblemMessage;
            iutlESSUserInfo.istrMessageInternal = "GetFromSession key=" + astrKey + ex.Message;
            throw ex;
        }
    }

    /// <summary>
    /// Gets the saved response data object from Session
    /// </summary>
    /// <param name="response"></param>
    /// <param name="astrKey"></param>
    public void RemoveFromSession(string astrKey)
    {
        try
        {
            //returned the stored object from session
            Session.Remove(astrKey);
        }
        catch (Exception ex)
        {
            iutlESSUserInfo.istrMessage = istrCalSTRSProblemMessage;
            iutlESSUserInfo.istrMessageInternal = "RemoveFromSession key=" + astrKey + ex.Message;
            //this.RedirectTo(NeoSpinConstants.NeoConstant.EWP_MWP.WFMPAGENAMES.LOGINPAGES_WFMLOGINERRORPAGE);
            throw ex;
        }
    }

    /// <summary>
    /// This function will return the cookie expiration time, by hours
    /// </summary>
    /// <returns>Date time</returns>
    protected DateTime GetCookieExpirationTime()
    {
        //return DateTime.Now.AddHours(1);
        return DateTime.Now.AddMinutes(_iintCookieExpirationTime);
    }

    /// <summary>
    /// Get cookies value
    /// </summary>
    /// <param name="aobjRequest"></param>
    /// <param name="astrCookieKey"></param>
    /// <returns></returns>
    public static string GetCookieValue(HttpRequest aobjRequest, string astrCookieKey)
    {
        string lstrCookieValue = string.Empty;
        if (aobjRequest.Cookies.Get(astrCookieKey) != null)
        {
            lstrCookieValue = aobjRequest.Cookies[astrCookieKey].Value;
        }
        return lstrCookieValue;
    }

    /// <summary>
    /// Create and add in cookies.
    /// </summary>
    /// <param name="aobjResponse"></param>
    /// <param name="astrCookieKey"></param>
    /// <param name="astrCookieValue"></param>
    /// <param name="adtCookieexpiryDateTime"></param>
    public static void CreateAndAddCookie(HttpResponse aobjResponse, string astrCookieKey, string astrCookieValue, DateTime adtCookieexpiryDateTime)
    {
        //Store the username in the cookie for future use
        HttpCookie lcokVNAVCookie = new HttpCookie(astrCookieKey);
        lcokVNAVCookie.Expires = adtCookieexpiryDateTime; // Convert.ToDateTime("1/1/2050");
        lcokVNAVCookie.Value = astrCookieValue;

        //Check if a cookie with the same name exists
        if (aobjResponse.Cookies.Get(astrCookieKey) != null)
        {
            aobjResponse.Cookies.Remove(astrCookieKey);
        }
        aobjResponse.Cookies.Add(lcokVNAVCookie);
    }

    #endregion

    #region Enrollment

    public const string SESSION_KEY_ENROLLMENT_USERID = "ENROLLMENT_USERID";
    public const string SESSION_KEY_ENROLLMENT_PASSWORD = "ENROLLMENT_PASSWORD";
    public const string METHOD_COMPLETE_ENROLLMENT = "CompleteMemberEnrollment";

    /// <summary>
    /// Remove OnClick Functionality On HomeLink
    /// </summary>
    //protected void RemoveOnClickFunctionalityOnHomeLink()
    //{
    //    HyperLink hlnkHome = (HyperLink)GetControl(Page, "hlnkHome");
    //    if (hlnkHome != null)
    //    {
    //        hlnkHome.Attributes.Remove("OnClick");
    //    }
    //}
    #endregion

    #region Common Methods

    /// <summary>
    /// Get Machine Label
    /// </summary>
    /// <param name="aobjHttpRequest"></param>
    /// <returns></returns>
    public static string GetMachineLabel(HttpRequest aobjHttpRequest)
    {
        string lstrReturnValue = string.Empty;
        lstrReturnValue = aobjHttpRequest.ServerVariables["REMOTE_HOST"];
        if (string.IsNullOrWhiteSpace(lstrReturnValue))
        {
            lstrReturnValue = aobjHttpRequest.ServerVariables["REMOTE_ADDR"];
        }
        return lstrReturnValue;
    }

    /// <summary>
    /// Validate member portal user id
    /// </summary>
    /// <param name="astrMWPUserId">Member user id</param>
    /// <param name="aintMwpId">MWP id</param>
    //public ExecutionResult CreatePassword(string astrEntrustUserID, string astrPassword)
    //{
    //    IBusinessTier lsrvBusinessTier = GetNeoSpinESSBusinessTier();
    //    try
    //    {
    //        Hashtable lhstParameter = new Hashtable();
    //        lhstParameter.Add("astrEntrustUserID", astrEntrustUserID);
    //        lhstParameter.Add("astrPassword", astrPassword);
    //        return (ExecutionResult)lsrvBusinessTier.ExecuteMethod("CreateOrSetPassword", lhstParameter, false, this.idictParams);
    //    }
    //    finally
    //    {
    //        HelperFunction.CloseChannel(lsrvBusinessTier);
    //    }

    //}

    /// <summary>
    /// Validate Password
    /// </summary>
    /// <param name="astrEntrustUserID"></param>
    /// <param name="astrPassword"></param>
    /// <returns></returns>
    //public ExecutionResult ValidatePassword(string astrEntrustUserID, string astrPassword)
    //{
    //    IBusinessTier lsrvBusinessTier = GetNeoSpinESSBusinessTier();
    //    try
    //    {
    //        Hashtable lhstParameter = new Hashtable();
    //        lhstParameter.Add("astrEntrustUserID", astrEntrustUserID);
    //        lhstParameter.Add("astrPassword", astrPassword);

    //        return (ExecutionResult)lsrvBusinessTier.ExecuteMethod("ValidatePassword", lhstParameter, false, new Dictionary<string, object>());
    //    }
    //    finally
    //    {
    //        HelperFunction.CloseChannel(lsrvBusinessTier);
    //    }

    //}

    /// <summary>
    /// Validate OTP
    /// </summary>
    /// <param name="astrEntrustUserID"></param>
    /// <param name="astrOTP"></param>
    /// <returns></returns>
    //public ExecutionResult ValidateOTP(string astrEntrustUserID, string astrOTP)
    //{
    //    IBusinessTier lsrvBusinessTier = GetNeoSpinESSBusinessTier();
    //    try
    //    {
    //        Hashtable lhstParameter = new Hashtable();
    //        lhstParameter.Add("astrEntrustUserID", astrEntrustUserID);
    //        lhstParameter.Add("astrOTP", astrOTP);
    //        return (ExecutionResult)lsrvBusinessTier.ExecuteMethod("ValidateOTP", lhstParameter, false, new Dictionary<string, object>());
    //    }
    //    finally
    //    {
    //        HelperFunction.CloseChannel(lsrvBusinessTier);
    //    }
    //}

    /// <summary>
    /// Update Status
    /// </summary>
    /// <param name="astrUserID">Passing User ID</param>
    /// <param name="astrWebAccessStatus"></param>
    //public void UpdateStatus(string astrUserID, string astrWebAccessStatus)
    //{
    //    IBusinessTier lsrvBusinessTier = GetNeoSpinESSBusinessTier();
    //    try
    //    {
    //        Hashtable lhstParameter = new Hashtable();
    //        lhstParameter.Add("astrEntrustUserID", astrUserID);
    //        lhstParameter.Add("astrWebAccessStatus", astrWebAccessStatus);
    //        lhstParameter.Add("astrWebAccessStatusReason", "");
    //        lsrvBusinessTier.ExecuteMethod("UpdateStatus", lhstParameter, false, new Dictionary<string, object>());
    //    }
    //    finally
    //    {
    //        HelperFunction.CloseChannel(lsrvBusinessTier);
    //    }
    //    //isrvNeoSpinBusinessTier.ExecuteMethod(NeoSpinConstants.NeoConstant.EWP_MWP.ESS_METHODS.UPDATE_STATUS, lhstParameter, false, new Dictionary<string, object>());

    //}

    /// <summary>
    /// Redirect to Home Page
    /// </summary>
    /// <param name="ablnIsRequestFromSelectAgency"></param>
    /// <param name="ablnFromExternalLoginPage"></param>
    //public void RedirectToHomePage(bool ablnIsRequestFromSelectAgency, bool ablnFromExternalLoginPage = true)
    //{

    //    iutlESSUserInfo = (utlESSUserInfo)iobjSessionData["EwpUserInfoObject"];

    //    SetUserSecurity();

    //    // In case of LoginI, this is already taken care in wfmLoginI.aspx.cs
    //    if (ablnFromExternalLoginPage)
    //    {
    //        if (!ablnIsRequestFromSelectAgency)
    //            UpdateStatus(iutlESSUserInfo.istrEmailId, "SLOG");

    //        iobjSessionData["RedirectOnLogoff"] = Convert.ToString("wfmLoginE.aspx");
    //    }
    //    else
    //    {
    //        iobjSessionData["RedirectOnLogoff"] = "wfmLoginI.aspx";
    //    }

    //    GetMetaDataCache();

    //    //Uncomments once Roles and security functionality is completed.
    //    //ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("~/Web.sitemap"), isrvMetaDataCache);

    //    ArrayList larrMenu = new ArrayList();
    //    larrMenu.Add("Organization");

    //    iobjSessionData["UserMenu"] = larrMenu;

    //    if (!ablnIsRequestFromSelectAgency)
    //    {
    //        bool lblnContactHasMultipleOrgs = CheckForMutlipleorg();

    //        string lstrNavUrl = string.Empty;

    //        if (lblnContactHasMultipleOrgs)
    //            Response.Redirect("wfmSelectAgency.aspx");
    //        else
    //            RedirectToEWPHomePage();
    //    }
    //    else
    //    {
    //        RedirectToEWPHomePage();
    //    }
    //}

    /// <summary>
    /// Redirect to EWP Home Page
    /// </summary>
    //private void RedirectToEWPHomePage()
    //{
    //    IBusinessTier lsrvBusinessTier = GetNeoSpinESSBusinessTier();
    //    try
    //    {
    //        Hashtable lhstParameter = new Hashtable();
    //        // Getting User Name details
    //        lhstParameter.Add("aintEssUserId", Convert.ToInt32(iutlESSUserInfo.iintEWPId));
    //        string lstrUserName = Convert.ToString(lsrvBusinessTier.ExecuteMethod("GetESSUserNamebyEssUserId", lhstParameter, false, this.idictParams));

    //        iobjSessionData["UserLoggedOn"] = true;
    //        iobjSessionData["Org_Id"] = iutlESSUserInfo.iintOrgId;
    //        iobjSessionData["IsExternalUserLogin"] = true;
    //        iobjSessionData["Org_Contact_Id"] = iutlESSUserInfo.iintOrgContactID;
    //        iobjSessionData["UserName"] = lstrUserName;//iutlESSUserInfo.istrEmailId;
    //        iobjSessionData["Landing_Page"] = "wfmESSHomePageMaintenance";
    //        iobjSessionData["aintOrgId"] = iutlESSUserInfo.iintOrgId;

    //        iobjSessionData["UserID"] = iutlESSUserInfo.istrUserId;
    //        iobjSessionData["UserSerialID"] = iutlESSUserInfo.iintUserSerialId;

    //        Dictionary<string, object> ldictParams = new Dictionary<string, object>();
    //        if ((Dictionary<string, object>)iobjSessionData["dictParams"] != null)
    //        {
    //            ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
    //        }

    //        Sagitec.MVVMClient.srvServers isrvServers = new Sagitec.MVVMClient.srvServers();
    //        isrvServers.ConnectToBT("wfmLoginESS");

    //        idictParams = CommonFunctions.InitializeSessionParameters(iutlESSUserInfo.iintOrgContactID, ldictParams, iobjSessionData);

    //        Framework.SessionRemove(utlConstants.istrSelectedActivityInstanceID);
    //        FormsAuthentication.SetAuthCookie(iutlESSUserInfo.istrEmailId, false);

    //        lhstParameter.Add("aintOrgContactID", iutlESSUserInfo.iintOrgContactID);

    //        iobjSessionData["UserSecurity"] = isrvServers.isrvBusinessTier.GetUserSecurity(iutlESSUserInfo.istrEmailId, CommonFunctions.GetParams(iutlESSUserInfo.istrUserId, iutlESSUserInfo.iintUserSerialId, ""));

    //        CommonFunctions.InitializeScreenSessionsAndGetLaunchURL(iobjSessionData);

    //        Sagitec.MVVMClient.LoginModel lmdlLogin = new Sagitec.MVVMClient.LoginModel();
    //        lmdlLogin.LoginWindowName = Convert.ToString(iobjSessionData[utlConstants.istrWindowName]);

    //        Sagitec.MVVMClient.AccountControllerBase.SetAntiForgeryToken(lmdlLogin);

    //        ldictParams[utlConstants.istrConstUserSerialID] = int.Parse(Convert.ToString(iobjSessionData["UserSerialID"]));
    //        ldictParams[utlConstants.istrTransactionID] = Convert.ToString(iobjSessionData[utlConstants.istrTransactionID]);
    //        ldictParams[utlConstants.istrSenderControlID] = string.Empty;
    //        ldictParams[utlConstants.istrConstSenderForm] = string.Empty;
    //        ldictParams[utlConstants.istrConstSenderKey] = 0;
    //        ldictParams[utlConstants.istrConstPostBackControl] = string.Empty;
    //        ldictParams[utlConstants.istrWindowName] = iobjSessionData[utlConstants.istrWindowName];
    //        ldictParams[utlConstants.istrRequestApplicationName] = Convert.ToString(ConfigurationManager.AppSettings["ApplicationName"]);

    //        ldictParams[utlConstants.istrConstPageMode] = utlPageMode.Update;

    //        ldictParams["Org_Contact_Id"] = iutlESSUserInfo.iintOrgContactID;
    //        ldictParams["aintOrgId"] = iutlESSUserInfo.iintOrgId;

    //        ldictParams["ESS_User_ID"] = iutlESSUserInfo.istrEmailId;

    //        //iutlESSUserInfo.iblnLogSelectQuery = true;
    //        //iutlESSUserInfo.iblnLogUpdateQuery = true;
    //        //iutlESSUserInfo.iblnLogInsertQuery = true;
    //        //iutlESSUserInfo.iblnLogDeleteQuery = true;

    //        //ldictParams[utlConstants.istrActivityLogLevel] = iutlESSUserInfo.iintActivityLogLevel;
    //        //ldictParams[utlConstants.istrActivityLogSelectQuery] = iutlESSUserInfo.iblnLogSelectQuery;
    //        //ldictParams[utlConstants.istrActivityLogInsertQuery] = iutlESSUserInfo.iblnLogInsertQuery;
    //        //ldictParams[utlConstants.istrActivityLogUpdateQuery] = iutlESSUserInfo.iblnLogUpdateQuery;
    //        //ldictParams[utlConstants.istrActivityLogDeleteQuery] = iutlESSUserInfo.iblnLogDeleteQuery;

    //        //iobjSessionData[utlConstants.istrActivityLogLevel] = iutlESSUserInfo.iintActivityLogLevel;
    //        //iobjSessionData[utlConstants.istrActivityLogSelectQuery] = iutlESSUserInfo.iblnLogSelectQuery;
    //        //iobjSessionData[utlConstants.istrActivityLogInsertQuery] = iutlESSUserInfo.iblnLogInsertQuery;
    //        //iobjSessionData[utlConstants.istrActivityLogUpdateQuery] = iutlESSUserInfo.iblnLogUpdateQuery;
    //        //iobjSessionData[utlConstants.istrActivityLogDeleteQuery] = iutlESSUserInfo.iblnLogDeleteQuery;

    //        // Yamini: ESS SEcurity implementation as per security matrix   
    //        DataTable ldtSecurity = (DataTable)lsrvBusinessTier.ExecuteMethod("GetESSSecurityForExternalUser", lhstParameter, false, this.idictParams);
    //        iobjSessionData["UserSecurity"] = ldtSecurity;
    //        iobjSessionData["dictParams"] = ldictParams;
    //        iobjSessionData["UserInfoObject"] = iutlESSUserInfo;

    //        Guid lguidTransactionID = Guid.NewGuid();
    //        ldictParams[utlConstants.istrTransactionID] = lguidTransactionID;
    //        Hashtable lhstSecurityTable = new Hashtable();
    //        lhstSecurityTable.Add("adtbUserSecurity", ldtSecurity);

    //        isrvServers.ConnectToBT("wfmLogin");
    //        isrvServers.isrvBusinessTier.ExecuteMethod("MVVMSetUserSecurityTable", lhstSecurityTable, false, ldictParams);

    //        string lstrUrl = "/" + Request.Url.AbsolutePath.Split('/')[1];
    //        Response.Redirect(lstrUrl);
    //    }
    //    finally
    //    {
    //        HelperFunction.CloseChannel(lsrvBusinessTier);
    //    }
    //}

    /// <summary>
    /// Check for mutliple organizations
    /// </summary>
    /// <returns></returns>
    protected bool CheckForMutlipleorg()
    {
        IBusinessTier lsrvBuisnessTier = GetNeoSpinESSBusinessTier();
        try
        {
            Hashtable lhstParameter = new Hashtable();
            lhstParameter.Add("astrEmailID", iutlESSUserInfo.istrEmailId);
            bool lblnContactHasMultipleOrgs = (bool)lsrvBuisnessTier.ExecuteMethod("IsMultipleOrgForSameUser", lhstParameter, false, this.idictParams);
            return lblnContactHasMultipleOrgs;
        }
        finally
        {
            HelperFunction.CloseChannel(lsrvBuisnessTier);
        }
    }

    /// <summary>
    /// Validate EWP User
    /// </summary>
    /// <param name="astrEWPUserId"></param>
    protected void ValidateEWPUser(string astrEWPUserId)
    {
        // INeoSpinBusinessTier isrvNeoSpinBusinessTier = GetNeoSpinESSBusinessTier();
        string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvESS");
        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);


        try
        {
            Hashtable lhstParameter = new Hashtable();
            lhstParameter.Add("astrEWPUserId", astrEWPUserId);
            iutlESSUserInfo = (utlUserInfo)lsrvBusinessTier.ExecuteMethod("ValidateEWPUser", lhstParameter, false, this.idictParams);
        }
        finally
        {
            HelperFunction.CloseChannel(lsrvBusinessTier);
        }
        return;
    }

    /// <summary>
    /// Validate ESS Internal User
    /// </summary>
    /// <param name="astrEmailID"></param>
    /// <param name="astrPassword"></param>
    /// <param name="astrAgencyID"></param>
    public void ValidateESSInternalUser(string astrEmailID, string astrPassword, string astrAgencyID)
    {
        string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinESS");
        INeoSpinBusinessTier isrvNeoSpinBusinessTier = (INeoSpinBusinessTier)Activator.GetObject(typeof(INeoSpinBusinessTier), lstrUrl);
        Hashtable lhstParameter = new Hashtable();
        lhstParameter.Add("astrUserId", astrEmailID);
        lhstParameter.Add("astrPassword", astrPassword);
        lhstParameter.Add("astrAgencyId", astrAgencyID);
        iutlESSUserInfo = (utlUserInfo)isrvNeoSpinBusinessTier.ExecuteMethod("ValidateESSInternalUser", lhstParameter, false, new System.Collections.Generic.Dictionary<string, object>());
        return;
    }

    /// <summary>
    /// Register Machine
    /// </summary>
    /// <param name="astrEntrustUserID"></param>
    /// <param name="ablnRegisterMachine"></param>
    /// <returns></returns>
    //public ExecutionResult RegisterMachine(string astrEntrustUserID, bool ablnRegisterMachine)
    //{
    //    IBusinessTier lsrvBusinessTier = GetNeoSpinESSBusinessTier();
    //    try
    //    {
    //        Hashtable lhstParameter = new Hashtable();
    //        lhstParameter = new Hashtable();
    //        lhstParameter.Add("astrEntrustUserID", astrEntrustUserID);
    //        lhstParameter.Add("astrBrowserName", Request.Browser.Browser);      //Get the client browser name.
    //        lhstParameter.Add("astrOS", Request.Browser.Platform);              //Get the client OS
    //        lhstParameter.Add("astrMachineLabel", GetMachineLabel(Request)); //Machine Name
    //        lhstParameter.Add("astrMachineNonce", GetCookieValue(Request, "ESS_ENTRUST_MACHINE_NONCE_" + astrEntrustUserID)); //Machine Nonce
    //        lhstParameter.Add("astrSequenceNonce", GetCookieValue(Request, "ESS_ENTRUST_SEQUENCE_NONCE_" + astrEntrustUserID));   //get sequence nonce
    //        ExecutionResult lobjExecutionResult = (ExecutionResult)lsrvBusinessTier.ExecuteMethod("ValidateMachineSecret", lhstParameter, false, new Dictionary<string, object>());
    //        GenericChallenge lobjGenericChallenge = lobjExecutionResult.iobjReturnValue as GenericChallenge;
    //        if (lobjGenericChallenge != null)
    //        {
    //            if (lobjGenericChallenge.riskScoringResult.MachineAuthenticationPassed == true)
    //            {
    //                CreateAndAddCookie(Response, "ESS_ENTRUST_MACHINE_NONCE_" + astrEntrustUserID, lobjGenericChallenge.machineSecret.machineNonce, DateTime.MaxValue);
    //                CreateAndAddCookie(Response, "ESS_ENTRUST_SEQUENCE_NONCE_" + astrEntrustUserID, lobjGenericChallenge.machineSecret.sequenceNonce, DateTime.MaxValue);
    //            }
    //            else
    //            {
    //                //iobjSessionData["ENTRUST_GENERIC_CHALLENGE"] = lobjGenericChallenge;
    //                iobjSessionGenericChallenge = lobjGenericChallenge;

    //                lhstParameter = new Hashtable();
    //                lhstParameter.Add("astrEnrustUserID", astrEntrustUserID);
    //                lobjExecutionResult = (ExecutionResult)lsrvBusinessTier.ExecuteMethod("GetQASecret", lhstParameter, false, new Dictionary<string, object>());

    //                IdentityGuardAdminServiceV10API.NameValue[] larrQuestions = null;
    //                Dictionary<string, string> larrQuestionsInOrderByEntrust = new Dictionary<string, string>();
    //                if (lobjExecutionResult.iobjReturnValue != null)
    //                {
    //                    larrQuestions = lobjExecutionResult.iobjReturnValue as IdentityGuardAdminServiceV10API.NameValue[];
    //                    foreach (string lstrQuestion in lobjGenericChallenge.QAChallenge)
    //                    {
    //                        var objQ = larrQuestions.Where(nv => nv.Name == lstrQuestion).FirstOrDefault();
    //                        if (objQ != null)
    //                        {
    //                            larrQuestionsInOrderByEntrust.Add(lstrQuestion, objQ.Value);
    //                        }
    //                    }
    //                    iobjSessionData["USER_ALL_QUESTIONS"] = larrQuestionsInOrderByEntrust;
    //                }
    //            }
    //        }
    //        lhstParameter = new Hashtable();
    //        lhstParameter.Add("astrEntrustUserID", astrEntrustUserID);
    //        lhstParameter.Add("astrBrowserName", Request.Browser.Browser);      //Get the client browser name.
    //        lhstParameter.Add("astrOS", Request.Browser.Platform);              //Get the client OS
    //        lhstParameter.Add("astrMachineLabel", GetMachineLabel(Request));    //Get machine label
    //        lhstParameter.Add("astrMachineNonce", "");              //Get the machine nonce
    //                                                                //Dictionary<string, string> ldecUserResponses =  iobjSessionData["USER_ALL_QUESTIONS"] as Dictionary<string, string>;
    //        Dictionary<string, string> ldecUserResponses = iobjSessionData["USER_ALL_QUESTIONS"] as Dictionary<string, string>;
    //        ArrayList larrUserResponse = GetUserResponses(ldecUserResponses);
    //        lhstParameter.Add("aaryQAResponse", larrUserResponse);
    //        lhstParameter.Add("ablnRegisterMachineSecret", ablnRegisterMachine);

    //        return (ExecutionResult)lsrvBusinessTier.ExecuteMethod("RegisterMachine", lhstParameter, false, new Dictionary<string, object>());
    //    }
    //    finally
    //    {
    //        HelperFunction.CloseChannel(lsrvBusinessTier);
    //    }
    //}

    /// <summary>
    /// Get user responses
    /// </summary>
    /// <param name="adecUserResponse"></param>
    /// <returns></returns>
    private ArrayList GetUserResponses(Dictionary<string, string> adecUserResponse)
    {
        ArrayList larrReturnValue = new ArrayList();

        if (adecUserResponse != null && adecUserResponse.Count > 0)
        {
            foreach (string lstrKey in adecUserResponse.Keys)
            {
                larrReturnValue.Add(adecUserResponse[lstrKey]);
            }
        }
        return larrReturnValue;
    }

    //protected override object SaveViewState()
    //{
    //    if (iobjSessionData != null) iobjSessionData.Update(true);
    //    return base.SaveViewState();
    //}

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

    #endregion
    //public GenericChallenge iobjSessionGenericChallenge
    //{
    //    get
    //    {
    //        return HelperFunction.DeSerializeToObject(iobjSessionData["ENTRUST_GENERIC_CHALLENGE"] as byte[]) as GenericChallenge;
    //    }
    //    set
    //    {
    //        iobjSessionData["ENTRUST_GENERIC_CHALLENGE"] = HelperFunction.SerializeObject(value);
    //    }
    //}
}
