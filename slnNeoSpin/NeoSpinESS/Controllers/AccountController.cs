using Neo.Model;
using NeoSpin.BusinessObjects;
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web;
using NeoSpin.Interface;
using Sagitec.Interface;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using Sagitec.ExceptionPub;
using Sagitec.WebClient;
namespace Neo.Controllers
{


    public class AccountController : AccountControllerBase
    {
        // utlUserInfo iobjUserInfo = new utlUserInfo();

        //string CookieName = "PERSLinkESSSession";

        #region Properties
        /// <summary>
        /// Gets or sets integer ORG ID
        /// </summary>
        /// <value>
        /// Organization ID (Null able)
        /// </value>
        public int? _iintOrgId { get; set; }

        public DataTable idtbEmployer { get; set; }
        #endregion

        #region Action Results

        /// <summary>
        /// Method to Get Dashboard Message Count for home page.
        /// </summary>
        /// <param name="ContactId"></param>
        /// <param name="OrgId"></param>
        /// <returns></returns>
        public void GetMessageCount(int ContactId, int OrgId)
        {
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                Hashtable lhstParams = new Hashtable();
                lhstParams.Add("aintContactID", ContactId);
                lhstParams.Add("aintOrgID", OrgId);
                iobjSessionData["MessageCount"] = (int)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetESSUnreadMessagesCount", lhstParams, false, null);
            }
            catch(Exception ex)
            {

            }
            finally{
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }
        protected override string BeforeLoginRedirect(string astrReturnUrl)
        {
            if (idictParams.ContainsKey("CreateCookies") && Convert.ToString(idictParams["CreateCookies"]) == "Yes"/*NeoConstant.Common.Flag.Yes*/)
            {
                HttpCookie lobjHttpCookie = new HttpCookie(Convert.ToString(idictParams["CookieName"]));
                lobjHttpCookie.Values.Add("IsMachineRegistered", "Yes"/*NeoConstant.Common.Flag.Yes*/);
                lobjHttpCookie.Values.Add("MachineNonce", Convert.ToString(idictParams["MachineNonce"]));
                lobjHttpCookie.Values.Add("SequenceNonce", Convert.ToString(idictParams["SequenceNonce"]));
                //Here we cant use busNeoBaseGlobalFunctions.GetSystemDate() both projects will be tight coupled i.e NeoSpinESS and NeoSpinBuisnessObject
                lobjHttpCookie.Expires = DateTime.Now.AddYears(60);
                Response.Cookies.Add(lobjHttpCookie);
            }

            // GetFooterDetails();
            //FW Upgrade :: wfmDefault.aspx.cs file code conversion (btn_OpenPDF method), Load Base directory in session
            DataTable ldtbSystemManagement = null;
            ldtbSystemManagement = isrvServers.isrvDbCache.GetDBSystemManagement();
            string lstrBaseDirectory = Convert.ToString(ldtbSystemManagement.Rows[0]["base_directory"]);
            iobjSessionData["base_directory"] = lstrBaseDirectory;
            iobjSessionData["Region_value"] = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);   //PIR 24081 MSS template with Test word
            return base.BeforeLoginRedirect(astrReturnUrl);
        }

        /// <summary>
        /// Overriding it for landing of proper login portal i.e. internal or external
        /// </summary>
        /// <param name="astrUrl">URL</param>
        /// <param name="astrController">Controller</param>        
        protected override void BeforeLogoutRedirect(out string astrUrl, out string astrController)
        {
            if (Convert.ToBoolean(iobjSessionData["IsExternalLogin"]))
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
                {
                    astrUrl = "wfmLoginEE";
                    astrController = "Account";
                }
                else
                {
                    astrUrl = "~/wfmLoginEE.aspx";
                    astrController = "";
                }
            }
            else if (Convert.ToBoolean(iobjSessionData["IsFromImageClick"]) == true)
            {
                astrUrl = "Logout";
                astrController = "Account";
            }
            else
            {
                astrUrl = iobjSessionData["LogoutAction"] != null ? Convert.ToString(iobjSessionData["LogoutAction"]) : "wfmLoginEI";
                astrController = "Account";
            }            
        }

        #endregion Action Results

        #region Public Methods

        /// <summary>        
        // Validate Organization Code or Contact Id - If Organization Code or Contact ID is invalid ,show Error       
        /// </summary>
        /// <param name="iintContactId">iintContactId</param>
        /// <returns>boolean</returns>
        protected bool ValidateSelectionCriteriaContact(int iintContactId)
        {
            //sudhirk- to validate organization contact is present or not
            _iintOrgId = Convert.ToInt32(isrvServers.isrvBusinessTier.DBExecuteScalar("entOrganizationContact.GetOrgIdByOrgContactId", new object[] { iintContactId }, idictParams));
            if (_iintOrgId.HasValue == false || _iintOrgId == 0)
            {
                return false;
            }
            return true;
        }

        #endregion
        #region [ESS External Portal Login]

        /// <summary>
        /// Set Parameters for external user
        /// </summary>
        /// <param name="autlUserInfo">autlUserInfo</param>
        protected override void SetParams(utlUserInfo autlUserInfo)
        {
            if (iblnExternalUser)
            {
                idictParams[utlConstants.istrIsMVVM] = true;
                idictParams[utlConstants.istrSessionID] = Session.SessionID;
                idictParams[utlConstants.istrConstUserID] = imdlLogin?.UserName?.Trim() ?? autlUserInfo.istrUserId;
                idictParams[utlConstants.istrConstUserSerialID] = autlUserInfo.iintUserSerialId;
                idictParams[utlConstants.istrConstFormName] = istrLoginFormName;
                iobjTraceInfo.istrActionTarget = istrLoginFormName;
                idictParams[utlConstants.istrRequestApplicationName] = ConfigurationManager.AppSettings["ApplicationName"];
                idictParams[utlConstants.istrRequestIPAddress] = GetIP();
                idictParams[utlConstants.istrRequestMACAddress] = GetMAC();  // Replace with actual MAC Address
                idictParams[utlConstants.istrRequestMachineName] = Environment.MachineName;
                idictParams[utlConstants.istrWindowName] = imdlLogin?.LoginWindowName ?? autlUserInfo.istrLoginWindowName;
                idictParams[utlConstants.istrLogInfo] = iobjSessionData[utlConstants.istrLogInfo];

                iobjSessionData["UserID"] = autlUserInfo.istrUserId;
                iobjSessionData["UserSerialID"] = autlUserInfo.iintUserSerialId;
                iobjSessionData["UserSerialID"] = autlUserInfo.iintUserSerialId;
                iobjSessionData["UserFirstName"] = autlUserInfo.istrFirstName;
                iobjSessionData["UserLastName"] = autlUserInfo.istrLastName;
                iobjSessionData["UserEmailId"] = autlUserInfo.istrEmailId;
                iobjSessionData["InitialPage"] = string.IsNullOrEmpty(autlUserInfo.istrInitialPage) ? "wfmESSHomePageMaintenance" : autlUserInfo.istrInitialPage;
                iobjSessionData["Landing_Page"] = string.IsNullOrEmpty(autlUserInfo.istrInitialPage) ? "wfmESSHomePageMaintenance" : autlUserInfo.istrInitialPage;


                iobjSessionData[utlConstants.istrUserType] = autlUserInfo.istrUserType;
                iobjSessionData["UserName"] = (string.IsNullOrEmpty(autlUserInfo.istrLastName) ? String.Empty : autlUserInfo.istrLastName + ", ") + autlUserInfo.istrFirstName;
                iobjSessionData["UserName"] = autlUserInfo.FullName;//iutlESSUserInfo.istrEmailId;
                iobjSessionData["UserFullName"] = !String.IsNullOrEmpty(autlUserInfo.istrMiddleInitial)
                                            ? autlUserInfo.istrFirstName + " " + autlUserInfo.istrMiddleInitial + " " + autlUserInfo.istrLastName
                                            : autlUserInfo.istrFirstName + " " + autlUserInfo.istrLastName;
                iobjSessionData["aintOrgId"] = autlUserInfo.iintOrgId;
                iobjSessionData["Org_Id"] = autlUserInfo.iintOrgId;
                idictParams["Org_Contact_Id"] = Convert.ToInt32(autlUserInfo.iobjTemp1);
                idictParams["ORG_CONTACT_ID"] = Convert.ToInt32(autlUserInfo.iobjTemp1);
            }
            else
            {
                base.SetParams(autlUserInfo);
            }
        }
        #endregion [ESS External Portal Login]

        #region ESS Internal MVVM

        [HttpGet]
        public ActionResult wfmLoginEE(string astrReturnUrl)
        {
            try
            {
                string lstrESSOIDCClientID = ConfigurationManager.AppSettings["ESSOIDCClientID"];
                if (lstrESSOIDCClientID.IsNullOrEmpty())
                {
                    throw new Exception($"ESSOIDCClientID config is missing from web.config of ESS");
                }
                string lstrESSOIDCIssuerUrl = ConfigurationManager.AppSettings["ESSOIDCIssuerUrl"];
                if (lstrESSOIDCIssuerUrl.IsNullOrEmpty())
                {
                    throw new Exception($"ESSOIDCIssuerUrl config is missing from web.config of ESS");
                }
                string lstrESSOIDCScope = ConfigurationManager.AppSettings["ESSOIDCScope"];
                if (lstrESSOIDCScope.IsNullOrEmpty())
                {
                    throw new Exception($"ESSOIDCScope config is missing from web.config of ESS");
                }
                string lstrESSOIDCCallbackUri = ConfigurationManager.AppSettings["ESSOIDCCallbackUri"];
                if (lstrESSOIDCCallbackUri.IsNullOrEmpty())
                {
                    throw new Exception($"ESSOIDCCallbackUri config is missing from web.config of ESS");
                }
                var lvarConfigUrls = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var lvarResponse = lvarConfigUrls.GetAsync(lstrESSOIDCIssuerUrl + "/.well-known/openid-configuration").Result;
                string lstrAuthEndpoint = null;
                if (lvarResponse.IsSuccessStatusCode)
                {
                    string responseString = lvarResponse.Content.ReadAsStringAsync().Result;
                    JObject responseJson = JObject.Parse(responseString);
                    lstrAuthEndpoint = responseJson["authorization_endpoint"].ToString();
                }
                istrCookieName = "PERSlinkESSExternalCookie";
                base.Login(astrReturnUrl);
                Guid lstrState = Guid.NewGuid();
                iobjSessionData["StateToken"] = lstrState.ToString();
                string lstrRedirectUrl = $"{lstrAuthEndpoint}?client_id={lstrESSOIDCClientID}&response_type=code&state={lstrState}&scope={lstrESSOIDCScope}&redirect_uri={lstrESSOIDCCallbackUri}";
                return Redirect(lstrRedirectUrl);
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                throw;
            }
        }

        [HttpGet]
        public ActionResult wfmLoginEI(string astrReturnUrl)
        {
            base.Login(astrReturnUrl);
            imdlLogin.LoginWindowName = iobjSessionData?[utlConstants.istrWindowName]?.ToString();
            return View(imdlLogin);
        }


        /// <summary>
        /// Method to login
        /// </summary>
        /// <param name="model">Login Model</param>
        /// <param name="astrReturnUrl">Url</param>
        /// <returns>Navigate Login page</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult wfmLoginEI(LoginModel model, string astrReturnUrl)
        {
            string lstrReturnUrl = string.Empty;
            try
            {
                if ((model.UserName == null || model.UserName == "") && (model.Password == null || model.Password == "") && (model.ReferenceID.IsNull() || model.ReferenceID == 0))
                {
                    model.Message = "UserName required. <br> Password required. <br> Contact ID required.";
                    return View(model);
                }
                if ((model.UserName == null || model.UserName == "") && (model.Password == null || model.Password == ""))
                {
                    model.Message = "UserName required. <br> Password required.";
                    return View(model);
                }
                if ((model.Password == null || model.Password == "") && (model.ReferenceID.IsNull() || model.ReferenceID == 0))
                {
                    model.Message = "Password required. <br> Contact ID required.";
                    return View(model);
                }
                if ((model.UserName == null || model.UserName == "") && (model.ReferenceID.IsNull() || model.ReferenceID == 0))
                {
                    model.Message = "UserName required.<br> Contact ID required.";
                    return View(model);
                }
                if ((model.UserName == null || model.UserName == ""))
                {
                    model.Message = "UserName required.";
                    return View(model);
                }
                if ((model.Password == null || model.Password == ""))
                {
                    model.Message = "Password required.";
                    return View(model);
                }
                if ((model.ReferenceID.IsNull() || model.ReferenceID == 0))
                {
                    model.Message = "Invalid Contact ID.";
                    return View(model);
                }

                if (model.UserName != null && model.UserName != "")
                {
                    utlUserInfo lobjUserInfo = isrvServers.isrvDbCache.ValidateUser(model.UserName, model.Password, istrApplicationName);


                    if (!lobjUserInfo.iblnAuthenticated)
                    {
                        model.Message = lobjUserInfo.istrMessage;
                        return View(model);
                    }

                    Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                    ldctParams[utlConstants.istrConstUserID] = model.UserName;
                    ldctParams[utlConstants.istrConstUserSerialID] = lobjUserInfo.iintUserSerialId;
                    ldctParams[utlConstants.istrConstFormName] = "wfmLoginEI";

                    DataTable ldtbUserSecurity = isrvServers.isrvBusinessTier.GetUserSecurity(model.UserName, ldctParams);
                    //BR - 001- 38 WSS Role Internal User only Get access to Portal
                    var lenuResult = ldtbUserSecurity.AsEnumerable().Where(i => i.Field<int>("resource_id") == 2002 && i.Field<int>("security_level") > 0).AsDataTable();
                    if (lenuResult.Rows.Count == 0)
                    {
                        model.Message = "You are not authorized to access Internal User WSS Access.";
                        return View(model);
                    }
                    lstrReturnUrl = ValidateContact(model);
                    if (lstrReturnUrl.IsNotNullOrEmpty() && model.Message.IsEmpty() && !lstrReturnUrl.Equals("wfmLoginEI"))
                    {
                        //Populate default values
                        lobjUserInfo.istrUserId = model.UserName;
                        istrTimeOutCodeValue = "TIWS";
                        istrCookieName = "PerslinkESSCookies";
                        base.Login(model, astrReturnUrl);
                        iobjSessionData["ContactID"] = model.ReferenceID;
                        iobjSessionData["UserID"] = lobjUserInfo.istrUserId;
                        idictParams["UserID"] = lobjUserInfo.istrUserId;
                        iobjSessionData["UserSerialID"] = lobjUserInfo.iintUserSerialId;
                        //iobjSessionData["ColorScheme"] = Page.Theme; //"ESSControlsTheme"; //"Green";//For MSS Layout change
                        iobjSessionData["UserType"] = busConstant.UserTypeInternal;
                        //Setting the User Security Here to lanuch the Lookup in Impersonate Screen.
                        iobjSessionData["UserSecurity"] = ldtbUserSecurity;
                        iobjSessionData["UserName"] = lobjUserInfo.istrLastName + ", " + lobjUserInfo.istrFirstName;
                        iobjSessionData["IsFromWhichPortal"] = "ESS";
                        idictParams["IsFromWhichPortal"] = "ESS";
                    }

                    //Hashtable lhstParams = new Hashtable();
                    //lhstParams.Add("astrKey", null);
                    //lhstParams.Add("astrValue", model.Password);
                    //string lstrEncryptedPassword = (string)isrvServers.isrvBusinessTier.ExecuteMethod("SagitecEncrypt", lhstParams, false, ldctParams);
                    //iobjSessionData["AccessDenied"] = lstrEncryptedPassword;
                }

                //ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), this);
                iobjSessionData["UserMenu"] = null;

                iobjSessionData["RedirectOnLogoff"] = "wfmLoginEI";
                iobjSessionData["LogoutAction"] = "wfmLoginEI";
                //string lstrNewID = AssignNewSessionID();  

                if (model.ReferenceID.ToString() != null && model.ReferenceID.ToString() != "" && model.ReferenceID != 0)
                {
                    if (lstrReturnUrl.IsNotNullOrEmpty() && model.Message.IsEmpty())
                    {
                        return RedirectToAction(lstrReturnUrl);
                    }
                    else
                    {
                        return View(model);
                    }
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception e)
            {
                model.Message = "Server is Down. Kindly try after sometime.";
                return View(model);
            }

        }

        
        private string ValidateContact(LoginModel model)
        {
            DataTable ldtEmployer = new DataTable();
            Dictionary<string, object> idctParams = new Dictionary<string, object>();

            if (iobjSessionData["UserSerialID"] != null)
                idctParams[utlConstants.istrConstUserSerialID] = (int)iobjSessionData["UserSerialID"];
            if (iobjSessionData["UserId"] != null)
                idctParams[utlConstants.istrConstUserID] = Convert.ToString(iobjSessionData["UserId"]);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");

            INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
            //INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = (INeoSpinBusinessTier)Activator.GetObject(typeof(INeoSpinBusinessTier), lstrUrl);
            try
            {
                //TextBox txbContact = (TextBox)lgnBase.FindControl("txbContactID");
                int aintContactID = Convert.ToInt32(model.ReferenceID);
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("aintContactID", aintContactID);
                busContact lobjContact = (busContact)isrvNeoSpinMSSBusinessTier.ExecuteMethod("LoadContact", lhstParam, false, idctParams);
                if (lobjContact.icdoContact.contact_id > 0)
                {
                    if (lobjContact.icdoContact.ndpers_login_id != null)
                    {
                        lhstParam = new Hashtable();
                        lhstParam.Add("aintContactID", lobjContact.icdoContact.contact_id);
                        ldtEmployer = (DataTable)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetEmployersForContact", lhstParam, false, idctParams);
                        if (ldtEmployer.Rows.Count == 0)
                        {
                            model.Message = "Selected Contact Id is not mapped to any Organization. Please select another contact to proceed.";
                            return "wfmLoginEI";
                        }
                        else if (ldtEmployer.Rows.Count == 1)
                        {
                            if (IsContactWellnessCoordAndOther(Convert.ToInt32(ldtEmployer.Rows[0]["org_id"]), lobjContact.icdoContact.contact_id, isrvNeoSpinMSSBusinessTier))
                            {
                                iobjSessionData["ContactID"] = lobjContact.icdoContact.contact_id;

                                //GetMessageCount(lobjContact.icdoContact.contact_id, Convert.ToInt32(ldtEmployer.Rows[0]["org_id"]));

                                return "wfmEmployerSelectionInternal";
                            }
                            else
                            {
                                model.Message = "You are not authorized to access WSS Screens.";
                                return "wfmLoginEI";
                            }
                        }
                        else
                        {
                            iobjSessionData["ContactID"] = lobjContact.icdoContact.contact_id;
                            return "wfmEmployerSelectionInternal";
                        }
                    }
                    else
                    {
                        model.Message = "Selected Contact is not associated with PERSLink Group. Please contact ITD for more details.";
                        return "wfmLoginEI";
                    }
                }
                else
                {
                    model.Message = "Invalid Contact Id";
                    return "wfmLoginEI";
                }
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }
        public bool IsContactWellnessCoordAndOther(int aintOrgID, int aintContactID, IBusinessTier asrvBusinessTier)
        {
            Hashtable lhstParam = new Hashtable();
            lhstParam = new Hashtable();
            lhstParam.Add("aintOrgID", aintOrgID);
            lhstParam.Add("aintContactID", aintContactID);
            DataTable ldtSecurity = (DataTable)asrvBusinessTier.ExecuteMethod("GetOrgContactRoles", lhstParam, false, new Dictionary<string, object>());
            var x = ldtSecurity.AsEnumerable().Where(row => (row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleWellnessCoordinator || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleOther)
                && (row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleAgent) || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleAuthorizedAgent
                || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleFinance || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRolePrimaryAuthorizedAgent);
            if (x.Count() > 0)
                return true;
            return false;
        }

        [HttpGet]
        public ActionResult wfmEmployerSelectionInternal(string astrReturnUrl)
        {
            AssignNewSessionID(); //F/W Upgrade PIR 11557 - Security Values Not Getting Refreshed  if new session not assigned.
            ViewBag.ReturnUrl = astrReturnUrl;
            wfmEmployerSelection EmployerSelectionModel = new wfmEmployerSelection();
            SetAntiForgeryToken(EmployerSelectionModel);
            string RedirectURL = RedirectToHomeOrEmployerSelection(EmployerSelectionModel);
            GetMessageCount(Convert.ToInt32(iobjSessionData["ContactID"]), Convert.ToInt32(iobjSessionData["OrgID"]));
            if (RedirectURL.IsNotNullOrEmpty() && EmployerSelectionModel.Message.IsEmpty() && RedirectURL != "wfmEmployerSelectionInternal")
            {
                string url1 = "/" + Request.Url.AbsolutePath.Split('/')[1];
                //if (!url1.EndsWith("/"))
                //{
                //    url1 = url1 + "/";
                //}
                iobjSessionData["Landing_Page"] = RedirectURL;
                idictParams["Landing_Page"] = RedirectURL;
                return Redirect(url1);
            }
            else
            {
                return View(EmployerSelectionModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult wfmEmployerSelectionInternal(wfmEmployerSelection aEmployerSelectionModel, string astrReturnUrl)
        {
            INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = null;
            try
            {
                SetAntiForgeryToken(aEmployerSelectionModel);
                iobjSessionData["OrgID"] = aEmployerSelectionModel.ORG_ID;
                iobjSessionData["OrgName"] = aEmployerSelectionModel.ORG_NAME;
                idictParams["OrgID"] = (int)iobjSessionData["OrgID"];
                idtbEmployer = (DataTable)iobjSessionData["dtbEmployer"];
                var results = (from m in idtbEmployer.AsEnumerable()
                               where m.Field<Int32>("org_id") == Convert.ToInt32(aEmployerSelectionModel.ORG_ID)
                               select m).FirstOrDefault();
                if (results.IsNotNull())
                {
                    iobjSessionData["OrgCode"] = results.ItemArray[1];
                    iobjSessionData["OrgName"] = results.ItemArray[2];
                }
                string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");

                isrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);

                string lstrURL = csLoginWSSHelper.SetSessionAndLaunchEmployerPortalHome(Convert.ToInt32(iobjSessionData["OrgID"]), Convert.ToInt32(iobjSessionData["ContactID"]), isrvNeoSpinMSSBusinessTier, iobjSessionData);
                idictParams["CentralPayroll"] = iobjSessionData["CentralPayroll"];

                if (IsContactWellnessCoordAndOther(Convert.ToInt32(iobjSessionData["OrgID"]), Convert.ToInt32(iobjSessionData["ContactID"]), isrvNeoSpinMSSBusinessTier))
                {
                    //Added for 9 Apr Framework changes - START
                    if (iobjSessionData["UserType"].ToString() == busConstant.UserTypeInternal)
                    {
                        idictParams[utlConstants.istrConstUserID] = iobjSessionData["UserID"];
                        idictParams[utlConstants.istrRequestApplicationName] = ConfigurationManager.AppSettings["ApplicationName"];
                    }
                    else
                    {
                        idictParams[utlConstants.istrConstUserID] = iobjSessionData["NDPERSLoginID"];
                        idictParams[utlConstants.istrRequestApplicationName] = ConfigurationManager.AppSettings["ApplicationName"];
                    }
                    idictParams[utlConstants.istrRequestMACAddress] = "00:00:00:00";  // Replace with actual MAC Address
                    idictParams[utlConstants.istrRequestIPAddress] = GetIP();
                    idictParams[utlConstants.istrRequestMachineName] = Request.UserHostName;
                    idictParams[utlConstants.istrConstUserSerialID] = Convert.ToInt32(iobjSessionData["UserSerialID"]);
                    idictParams[utlConstants.istrWindowName] = iobjSessionData[utlConstants.istrWindowName];
                    idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";
                    idictParams["IsPAG"] = iobjSessionData["IsPAG"];
                    //Tracing Code Start
                    iobjSessionData[utlConstants.istrLogInfo] = iobjTraceInfo.iobjLogInfo;
                    iobjSessionData[utlConstants.istrLogQuery] = false;
                    //Tracing Code End
                    iobjSessionData["Landing_Page"] = lstrURL;
                    idictParams["Landing_Page"] = lstrURL;

                    GetMessageCount(Convert.ToInt32(iobjSessionData["ContactID"]), Convert.ToInt32(iobjSessionData["OrgID"]));
                    //return Redirect("/" + Request.Url.AbsolutePath.Split('/')[1]);
                    string url1 = "/" + Request.Url.AbsolutePath.Split('/')[1];
                    //if (!url1.EndsWith("/"))
                    //{
                    //    url1 = url1 + "/";
                    //}
                    return Redirect(url1);
                }
                else
                {
                    aEmployerSelectionModel.Message = "You are not authorized to access WSS Screens for selected Employer. Please select different Employer.";
                    //F/W Upgrade PIR 11888 - Select list items are posted back as null since they were generated dynamically on get
                    aEmployerSelectionModel.EmployerList = (iobjSessionData["ORG_ID"].IsNotNull() && iobjSessionData["ORG_ID"] is SelectList) 
                                                            ? (SelectList)iobjSessionData["ORG_ID"] : new SelectList(new List<SelectListItem>());
                    return View(aEmployerSelectionModel);
                }
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }

        }

        public string RedirectToHomeOrEmployerSelection(wfmEmployerSelection EmployerSelectionModel)
        {
            INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = null;
            try
            {
                Hashtable lhstParam = new Hashtable();
                int lintContactID = (int)iobjSessionData["ContactID"];
                idictParams["ContactID"] = (int)iobjSessionData["ContactID"];
                idictParams["OrgContactID"] = (int)idictParams["ContactID"];
                lhstParam.Add("aintContactID", lintContactID);
                //Framework.SessionRemove("dtbEmployer");
                string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");

                isrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                idtbEmployer = (DataTable)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetEmployersForContact", lhstParam, false, new Dictionary<string, object>());

                if (idtbEmployer.Rows.Count == 0)
                {
                    iobjSessionData["dtbEmployerCount"] = "";
                    iobjSessionData["dtbEmployer"] = idtbEmployer;
                    List<SelectListItem> EMPLOYER_LIST = new List<SelectListItem>();
                    if (idtbEmployer != null && idtbEmployer.Rows.Count > 0)
                    {
                        foreach (DataRow dr in idtbEmployer.Rows)
                        {
                            EMPLOYER_LIST.Add(new SelectListItem { Text = Convert.ToString(dr["ORG_NAME"]), Value = Convert.ToString(dr["ORG_ID"]) });
                        }
                    }
                    EmployerSelectionModel.EmployerList = new List<SelectListItem>();
                    EmployerSelectionModel.EmployerList = EMPLOYER_LIST;
                    iobjSessionData["ORG_ID"] = new SelectList(EMPLOYER_LIST, "Value", "Text", "");
                    return EmployerSelectionModel.Message = "You are not authorized to view any organization details. Please contact ITD for further assistance.";
                }
                else if (idtbEmployer.Rows.Count == 1)
                {
                    iobjSessionData["dtbEmployerCount"] = "";
                    iobjSessionData["OrgID"] = idtbEmployer.Rows[0]["org_id"];
                    iobjSessionData["OrgCode"] = idtbEmployer.Rows[0]["org_code"];
                    iobjSessionData["OrgName"] = idtbEmployer.Rows[0]["org_name"];
                    string lstrURL = csLoginWSSHelper.SetSessionAndLaunchEmployerPortalHome(Convert.ToInt32(iobjSessionData["OrgID"]), Convert.ToInt32(iobjSessionData["ContactID"]), isrvNeoSpinMSSBusinessTier, iobjSessionData);
                    idictParams["CentralPayroll"] = iobjSessionData["CentralPayroll"];
                    idictParams["OrgID"] = iobjSessionData["OrgID"];
                    idictParams["IsPAG"] = iobjSessionData["IsPAG"];
                    //Added for 9 Apr Framework changes - START
                    if (iobjSessionData["UserType"].ToString() == busConstant.UserTypeInternal)
                    {
                        idictParams[utlConstants.istrConstUserID] = iobjSessionData["UserID"];
                        idictParams[utlConstants.istrRequestApplicationName] = ConfigurationManager.AppSettings["ApplicationName"];
                    }
                    else
                    {
                        idictParams[utlConstants.istrConstUserID] = iobjSessionData["NDPERSLoginID"];
                        idictParams[utlConstants.istrRequestApplicationName] = ConfigurationManager.AppSettings["ApplicationName"];
                    }
                    idictParams[utlConstants.istrRequestMACAddress] = "00:00:00:00";  // Replace with actual MAC Address
                    idictParams[utlConstants.istrRequestIPAddress] = GetIP();
                    idictParams[utlConstants.istrRequestMachineName] = Request.UserHostName;
                    idictParams[utlConstants.istrConstUserSerialID] = iobjSessionData["UserSerialID"];

                    idictParams[utlConstants.istrWindowName] = iobjSessionData[utlConstants.istrWindowName];
                    idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";

                    iobjSessionData[utlConstants.istrLogInfo] = iobjTraceInfo.iobjLogInfo;
                    iobjSessionData[utlConstants.istrLogQuery] = false;

                    return lstrURL;
                }
                else
                {

                    iobjSessionData["dtbEmployer"] = idtbEmployer;
                    List<SelectListItem> EMPLOYER_LIST = new List<SelectListItem>();
                    if (idtbEmployer != null && idtbEmployer.Rows.Count > 0)
                    {
                        iobjSessionData["dtbEmployerCount"] = "Multiple";
                        foreach (DataRow dr in idtbEmployer.Rows)
                        {
                            EMPLOYER_LIST.Add(new SelectListItem { Text = Convert.ToString(dr["ORG_NAME"]), Value = Convert.ToString(dr["ORG_ID"]) });
                        }
                    }
                    EmployerSelectionModel.EmployerList = new List<SelectListItem>();
                    EmployerSelectionModel.EmployerList = EMPLOYER_LIST;
                    iobjSessionData["ORG_ID"] = new SelectList(EMPLOYER_LIST, "Value", "Text", "");
                    //idictParams["OrgID"] = iobjSessionData["OrgID"];
                    //ddlEmployers.DataTextField = "org_name";
                    //ddlEmployers.DataValueField = "org_id";
                    //ddlEmployers.sfwDataField = "OrgCode";
                    //ddlEmployers.DataBind();  
                    return "wfmEmployerSelectionInternal";
                }
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }

        [HttpGet]
        public ActionResult wfmImpersonateContact(string astrReturnUrl)
        {
            ViewBag.ReturnUrl = astrReturnUrl;
            wfmEmployerSelection EmployerSelectionModel = new wfmEmployerSelection();
            SetAntiForgeryToken(EmployerSelectionModel);
            return View(EmployerSelectionModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult wfmImpersonateContact(wfmEmployerSelection aEmployerSelectionModel, string astrReturnUrl)
        {
            INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = null;
            try
            {
                SetAntiForgeryToken(aEmployerSelectionModel);
                if (iobjSessionData["UserSerialID"] != null)
                    idictParams[utlConstants.istrConstUserSerialID] = (int)iobjSessionData["UserSerialID"];
                if (iobjSessionData["UserId"] != null)
                    idictParams[utlConstants.istrConstUserID] = Convert.ToString(iobjSessionData["UserId"]);
                DataTable ldtEmployer = new DataTable();
                int aintContactID = Convert.ToInt32(aEmployerSelectionModel.ContactID);
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("aintContactID", aintContactID);
                string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");

                isrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                busContact lobjContact = (busContact)isrvNeoSpinMSSBusinessTier.ExecuteMethod("LoadContact", lhstParam, false, idictParams);

                if (lobjContact.icdoContact.contact_id > 0)
                {
                    if (lobjContact.icdoContact.ndpers_login_id != null)
                    {
                        lhstParam = new Hashtable();
                        lhstParam.Add("aintContactID", lobjContact.icdoContact.contact_id);
                        ldtEmployer = (DataTable)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetEmployersForContact", lhstParam, false, idictParams);

                        if (ldtEmployer != null && ldtEmployer.Rows.Count > 0)
                        {
                            GetMessageCount(lobjContact.icdoContact.contact_id, Convert.ToInt32(ldtEmployer.Rows[0]["org_id"]));
                        }
                        if (ldtEmployer.Rows.Count == 0)
                        {
                            aEmployerSelectionModel.Message = "Selected Contact Id is not mapped to any Organization. Please select another contact to proceed.";
                            //return Redirect("wfmImpersonateContact");
                            return View(aEmployerSelectionModel);
                        }
                        else if (ldtEmployer.Rows.Count == 1)
                        {
                            if (IsContactWellnessCoordAndOther(Convert.ToInt32(ldtEmployer.Rows[0]["org_id"]), lobjContact.icdoContact.contact_id, isrvNeoSpinMSSBusinessTier))
                            {
                                iobjSessionData["ContactID"] = lobjContact.icdoContact.contact_id;
                                // Session["PersonID"] = lobjContact.icdoContact.contact_id;
                                return Redirect("wfmEmployerSelectionInternal");
                            }
                            else
                            {
                                aEmployerSelectionModel.Message = "You are not authorized to access WSS Screens.";
                                //return Redirect("wfmImpersonateContact");
                                return View(aEmployerSelectionModel);
                            }
                        }
                        else
                        {
                            iobjSessionData["ContactID"] = lobjContact.icdoContact.contact_id;
                            return Redirect("wfmEmployerSelectionInternal");
                        }
                    }
                    else
                    {
                        aEmployerSelectionModel.Message = "Selected Contact is not associated with PERSLink Group. Please contact ITD for more details.";
                        //return Redirect("wfmImpersonateContact");
                        return View(aEmployerSelectionModel);
                    }
                }
                else
                {
                    aEmployerSelectionModel.Message = "Invalid Contact Id";
                    //return Redirect("wfmImpersonateContact");
                    return View(aEmployerSelectionModel);
                }
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }
        public ActionResult PostPublicAuthLogout()
        {
            return base.Logout(null);
        }
        [HttpGet]
        public override ActionResult Logout(string astrReturnUrl)
        {
            if (Convert.ToBoolean(iobjSessionData["IsExternalLogin"]) && Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
            {
                return Redirect(Convert.ToString(ConfigurationManager.AppSettings["ESSOIDCLogoutEndpoint"]));
            }
            //return base.Logout(astrReturnUrl);
            wfmEmployerSelection model = new wfmEmployerSelection();
            int lintKeyValue = 0;
            if (idictParams.ContainsKey(utlConstants.istrLogInfo))
            {
                utlLogInfo lobjLogInfo = (utlLogInfo)idictParams[utlConstants.istrLogInfo];
                if (lobjLogInfo != null)
                {
                    lintKeyValue = lobjLogInfo.iintLogInstanceID;
                }
            }

            if (lintKeyValue > 0)
            {
                isrvServers.isrvBusinessTier.LogUserLogoff(lintKeyValue, idictParams);
            }
            // Redirect to the Home Page (that should be intercepted and redirected to the Login Page first)
            string lstrUrl;
            string lstrController;
            BeforeLogoutRedirect(out lstrUrl, out lstrController);

            FormsAuthentication.SignOut();

            Session.RemoveAll();
            Session.Abandon();

            iobjSessionData.Clear(isrvServers.isrvBusinessTier);//7783 discuss if this can be merged with LogUserLogOff

            // Clear authentication cookie
            HttpCookie rFormsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            rFormsCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(rFormsCookie);

            if (lstrUrl.EndsWith(".aspx") && String.IsNullOrEmpty(lstrController))
            {
                return Redirect(lstrUrl);
            }
            else if ((!string.IsNullOrEmpty(lstrController)) && lstrController.Equals("Account") && (!string.IsNullOrEmpty(lstrUrl) && lstrUrl.Equals("Logout")))
            {
                return View(model);
            }
            return RedirectToAction(lstrUrl);
        }
        //[HttpPost]
        //public ActionResult Logout(LoginModel model, string astrReturnUrl)
        //{
        //    string lstrUrl;
        //    string lstrController;
        //    BeforeLogoutRedirect(out lstrUrl, out lstrController);
        //    return RedirectToAction(lstrUrl, lstrController);
        //}
        #endregion      
        public ActionResult OIDCRedirect(string astrReturnUrl)
        {
            try
            {
                string lstrState = Convert.ToString(iobjSessionData["StateToken"]);
                //if (Request != null && Request.Params != null && Request.Params["state"] != null && Request.Params["state"].ToString().Equals(lstrState))
                //{
                    if (Request.Params["error"] != null && Request.Params["error"].ToString().Equals("user_canceled_request"))
                    {
                        // For IntelliTrust if user clicks on back button.
                        return RedirectToAction("wfmLoginEE", "Account");
                    }
                    string lstrESSOIDCClientID = ConfigurationManager.AppSettings["ESSOIDCClientID"];
                    if (lstrESSOIDCClientID.IsNullOrEmpty())
                    {
                        // ESSOIDCClientID config is missing from web.config of ESS.
                        throw new Exception("ESS - OIDCRedirect - ESSOIDCClientID config is missing from web.config.");
                    }
                    string lstrESSOIDCClientSecret = ConfigurationManager.AppSettings["ESSOIDCClientSecret"];
                    if (lstrESSOIDCClientSecret.IsNullOrEmpty())
                    {
                        // ESSOIDCClientSecret config is missing from web.config of ESS.
                        throw new Exception("ESS - OIDCRedirect - ESSOIDCClientSecret config is missing from web.config.");
                    }
                    string lstrESSOIDCIssuerUrl = ConfigurationManager.AppSettings["ESSOIDCIssuerUrl"];
                    if (lstrESSOIDCIssuerUrl.IsNullOrEmpty())
                    {
                        // ESSOIDCIssuerUrl config is missing from web.config of ESS.
                        throw new Exception("ESS - OIDCRedirect - ESSOIDCIssuerUrl config is missing from web.config.");
                    }

                    string lstrESSOIDCCallbackUri = ConfigurationManager.AppSettings["ESSOIDCCallbackUri"];
                    if (lstrESSOIDCCallbackUri.IsNullOrEmpty())
                    {
                        // ESSOIDCCallbackUri config is missing from web.config of ESS.
                        throw new Exception("ESS - OIDCRedirect - ESSOIDCCallbackUri config is missing from web.config.");
                    }
                    var lvarConfigUrls = new HttpClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var lvarResponse = lvarConfigUrls.GetAsync(lstrESSOIDCIssuerUrl + "/.well-known/openid-configuration").Result;
                    string lstrTokenEndpoint = null, lstrJwksUri = null;
                    if (lvarResponse.IsSuccessStatusCode)
                    {
                        string responseString = lvarResponse.Content.ReadAsStringAsync().Result;
                        JObject responseJson = JObject.Parse(responseString);
                        lstrTokenEndpoint = responseJson["token_endpoint"].ToString();
                        lstrJwksUri = responseJson["jwks_uri"].ToString();
                    }
                    ArrayList larrResult = null;
                    INeoSpinBusinessTier lsrvNeoSpinESSBusinessTier = null;
                    string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
                    try
                    {
                        lsrvNeoSpinESSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                        Hashtable lhstParams = new Hashtable();
                        lhstParams.Add("astrAuthCode", Request.QueryString["code"]);
                        lhstParams.Add("astrTokenEndpoint", lstrTokenEndpoint);
                        lhstParams.Add("astrClientId", lstrESSOIDCClientID);
                        lhstParams.Add("astrClientSecret", lstrESSOIDCClientSecret);
                        lhstParams.Add("astrRedirectUrl", lstrESSOIDCCallbackUri);
                        lhstParams.Add("astrJwksUri", lstrJwksUri);
                        lhstParams.Add("astrIssuer", lstrESSOIDCIssuerUrl);
                        lhstParams.Add("ablnIsMember", false);
                        larrResult = (ArrayList)lsrvNeoSpinESSBusinessTier.ExecuteMethod("OIDCMSSUserValidate", lhstParams, true, idictParams);
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.Publish(ex);
                        throw;
                    }
                    finally
                    {
                        HelperFunction.CloseChannel(lsrvNeoSpinESSBusinessTier);
                    }
                    Dictionary<string, object> ldictUserParams = null;
                    iblnExternalUser = true;
                    busContact lbusContact = null;
                    if (larrResult.Count > 0)
                    {
                        if (larrResult[0] is utlError)
                        {
                            throw new Exception((larrResult[0] as utlError).istrErrorMessage);
                        }
                        else
                        {
                            ldictUserParams = larrResult[0] as Dictionary<string, object>;
                            if (ldictUserParams.IsNotNull() && ldictUserParams.ContainsKey("busContact") && ldictUserParams["busContact"] is busContact)
                            {
                                lbusContact = (busContact)ldictUserParams["busContact"];
                            }
							idictParams[utlConstants.istrRequestIPAddress] = GetIP();
                            idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";
                            iobjSessionData["NDPERSLoginID"] = ldictUserParams["ndpers_login_id"];
                            idictParams[utlConstants.istrConstUserID] = ldictUserParams["ndpers_login_id"];
                            idictParams[utlConstants.istrRequestApplicationName] = ConfigurationManager.AppSettings["ApplicationName"];
                            iobjSessionData["NDPERSEmailID"] = ldictUserParams["email"];
                            iobjSessionData["NDPERSEmailIDESS"] = ldictUserParams["email"];
                            idictParams["NDPERSEmailIDESS"] = ldictUserParams["email"];
                            iobjSessionData["NDPERSFirstName"] = ldictUserParams["given_name"];
                            iobjSessionData["NDPERSLastName"] = ldictUserParams["family_name"];
                            istrCookieName = "PERSlinkESSExternalCookie";
                            istrTimeOutCodeValue = "TIMS";
                            ModelState.Clear();
                            if (imdlLogin.IsNull())
                                imdlLogin = new LoginModel();
                            imdlLogin.UserName = Convert.ToString(ldictUserParams["ndpers_login_id"]);
                            base.Login(imdlLogin, astrReturnUrl);
                            iobjSessionData["IsFromWhichPortal"] = "ESS";
                            idictParams["IsFromWhichPortal"] = "ESS";
                            idictParams["NDPERSEmailID"] = ldictUserParams["email"];
                            iobjSessionData["UserID"] = Convert.ToString(ldictUserParams["ndpers_login_id"]);
                            iobjSessionData["UserSerialID"] = 0;
                            iobjSessionData["UserType"] = busConstant.UserTypeEmployer;
                            iobjSessionData["ColorScheme"] = string.Empty;//For ESS Layout change
                            iobjSessionData["IsExternalUser"] = true;
                            if (lbusContact.icdoContact.contact_id > 0)
                            {
                                DataTable idtbEmployer = new DataTable();
                                try
                                {
                                    lsrvNeoSpinESSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                                    Hashtable lhstParamForContact = new Hashtable();
                                    lhstParamForContact.Add("aintContactID", lbusContact.icdoContact.contact_id);
                                    idtbEmployer = (DataTable)lsrvNeoSpinESSBusinessTier.ExecuteMethod("GetEmployersForContact", lhstParamForContact, false, new Dictionary<string, object>());
                                }
                                catch (Exception ex)
                                {
                                    ExceptionManager.Publish(ex);
                                    throw;
                                }
                                finally
                                {
                                    HelperFunction.CloseChannel(lsrvNeoSpinESSBusinessTier);
                                }
                                iobjSessionData["IsExternalUser"] = true;
                                idictParams["UserSecurity"] = iobjSessionData["UserSecurity"];
                                isrvServers.isrvBusinessTier.StoreProcessLog("Contact ID " + lbusContact.icdoContact.contact_id.ToString() + " - External User successfully logged in to ESS Portal", idictParams);
                                //iobjSessionData["Landing_Page"] = lstrURL;
                                //iobjSessionData["InitialPage"] = lstrURL;
                                iobjSessionData["UserLoggedOn"] = "true";
                                iobjSessionData["IsExternalLogin"] = true;
                                idictParams["IsExternalLogin"] = true;
                                idictParams["UserLoggedOn"] = "true";
                                idictParams["IsfromESSPortal"] = "true";
                                iobjSessionData["UserLoggedOn"] = "true";
                                Framework.SessionForWindow["UserLoggedOn"] = "true";
                                idictParams["PersonCertify"] = iobjSessionData["PersonCertify"];
                                Framework.istrWindowName = iobjSessionData[utlConstants.istrWindowName].ToString();
                                iobjSessionData["WindowName"] = Framework.istrWindowName;
                                iobjSessionData["dictParams"] = idictParams;
                                iobjSessionData.idictParams = idictParams;
                                DataTable ldtbSystemManagement = isrvServers.isrvDbCache.GetDBSystemManagement();
                                string lstrBaseDirectory = Convert.ToString(ldtbSystemManagement.Rows[0]["base_directory"]);
                                iobjSessionData["base_directory"] = lstrBaseDirectory;
                                iobjSessionData["Region_value"] = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);
                                iobjSessionData["PopUpMessageForCertify"] = isrvServers.isrvDbCache.GetMessageText(busConstant.PopUpMessageForCertify);
                                //string url1 = string.Empty;

                                if (idtbEmployer.Rows.Count >= 1)
                                {
                                    iobjSessionData["ContactID"] = lbusContact.icdoContact.contact_id;
                                    iobjSessionData["UserID"] = string.Empty;
                                    iobjSessionData["ColorScheme"] = string.Empty;
                                    iobjSessionData["UserLoggedOn"] = "true";
                                    return Redirect("~/wfmEmployerSelection.aspx");

                                }
                                else
                                {
                                    return Redirect("~/wfmRequestOnlineAccessForContact.aspx");
                                }
                            }
                            else
                            {
                                return Redirect("~/wfmRequestOnlineAccessForContact.aspx");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("OIDCUserValidate : Returned Null");
                    }
                //}
                //else
                //{
                //    throw new Exception("ESS - OIDCRedirect - Config Section is missing from Web.Config.");
                //}
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                throw;
            }
        }
    }
}
