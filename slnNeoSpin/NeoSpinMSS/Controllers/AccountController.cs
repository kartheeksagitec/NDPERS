//using Neo.Model;
//using NeoBase.Common;
using NeoSpin.BusinessObjects;
using Sagitec.Common;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using NeoSpin.Interface;
using NeoSpinMSS.Model;
using System.Web.Security;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using Sagitec.ExceptionPub;
using Sagitec.WebClient;
using NeoSpin.Common;

namespace Neo.Controllers
{
    public class AccountController : AccountControllerBase
    {
        
        public int? _iintUserId { get; set; }

        #region Action Results

        /// <summary>
        /// Overridden BeforeLoginRedirect to bind footer with the application details
        /// </summary>
        /// <param name="astrReturnUrl">astrReturnUrl</param>
        /// <returns>string</returns>
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
            iobjSessionData["Region_value"] = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);     //PIR 24081 MSS template with Test word
            return base.BeforeLoginRedirect(astrReturnUrl);
        }

        #endregion Action Results



        #region

        [HttpGet]
        public ActionResult wfmLoginME(string astrReturnUrl)
        {
            try
            {
                string lstrMSSOIDCClientID = ConfigurationManager.AppSettings["MSSOIDCClientID"];
                if (lstrMSSOIDCClientID.IsNullOrEmpty())
                {
                    throw new Exception($"MSSOIDCClientID config is missing from web.config of MSS");
                }
                string lstrMSSOIDCIssuerUrl = ConfigurationManager.AppSettings["MSSOIDCIssuerUrl"];
                if (lstrMSSOIDCIssuerUrl.IsNullOrEmpty())
                {
                    throw new Exception($"MSSOIDCIssuerUrl config is missing from web.config of MSS");
                }
                string lstrMSSOIDCScope = ConfigurationManager.AppSettings["MSSOIDCScope"];
                if (lstrMSSOIDCScope.IsNullOrEmpty())
                {
                    throw new Exception($"MSSOIDCScope config is missing from web.config of MSS");
                }
                string lstrMSSOIDCCallbackUri = ConfigurationManager.AppSettings["MSSOIDCCallbackUri"];
                if (lstrMSSOIDCCallbackUri.IsNullOrEmpty())
                {
                    throw new Exception($"MSSOIDCCallbackUri config is missing from web.config of MSS");
                }
                var lvarConfigUrls = new HttpClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var lvarResponse = lvarConfigUrls.GetAsync(lstrMSSOIDCIssuerUrl + "/.well-known/openid-configuration").Result;
                string lstrAuthEndpoint = null;
                if (lvarResponse.IsSuccessStatusCode)
                {
                    string responseString = lvarResponse.Content.ReadAsStringAsync().Result;
                    JObject responseJson = JObject.Parse(responseString);
                    lstrAuthEndpoint = responseJson["authorization_endpoint"].ToString();
                }
                istrCookieName = "PERSlinkMSSExternalCookie";
                base.Login(astrReturnUrl);
                Guid lstrState = Guid.NewGuid();
                iobjSessionData["StateToken"] = lstrState.ToString();
                string lstrRedirectUrl = $"{lstrAuthEndpoint}?client_id={lstrMSSOIDCClientID}&response_type=code&state={lstrState}&scope={lstrMSSOIDCScope}&redirect_uri={lstrMSSOIDCCallbackUri}";
                return Redirect(lstrRedirectUrl);
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                throw;
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
        public ActionResult wfmLoginMI(string astrReturnUrl)
        {
            
            base.Login(astrReturnUrl);
            imdlLogin.LoginWindowName = iobjSessionData?[utlConstants.istrWindowName]?.ToString();
            return View(imdlLogin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult wfmLoginMI(LoginModel model, string astrReturnUrl)
        {
           
            if ((model.UserName == null || model.UserName == "") && (model.Password == null || model.Password == "") && (model.ReferenceID.IsNull() || model.ReferenceID == 0))
            {
                model.Message = "NDPERS User ID required. <br> Password required. <br> Member ID required.";
                return View(model);
            }
            if ((model.UserName == null || model.UserName == "") && (model.Password == null || model.Password == ""))
            {
                model.Message = "NDPERS User ID required. <br> Password required.";
                return View(model);
            }
            if ((model.Password == null || model.Password == "") && (model.ReferenceID.IsNull() || model.ReferenceID == 0))
            {
                model.Message = "Password required. <br> Member ID required.";
                return View(model);
            }
            if ((model.UserName == null || model.UserName == "") && (model.ReferenceID.IsNull() || model.ReferenceID == 0))
            {
                model.Message = "NDPERS User ID required.<br> Member ID required.";
                return View(model);
            }
            if ((model.UserName == null || model.UserName == ""))
            {
                model.Message = "NDPERS User ID required.";
                return View(model);
            }
            if ((model.Password == null || model.Password == ""))
            {
                model.Message = "Password required.";
                return View(model);
            }
            if ((model.ReferenceID.IsNull() || model.ReferenceID == 0))
            {
                model.Message = "Invalid Member ID.";
                return View(model);
            }
            utlUserInfo lobjUserInfo = isrvServers.isrvDbCache.ValidateUser(model.UserName, model.Password, istrApplicationName);

            if (!lobjUserInfo.iblnAuthenticated)
            {

                model.Message = lobjUserInfo.istrMessage;
                return View(model);
            }
            
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams[utlConstants.istrConstUserID] = model.UserName;
            ldctParams[utlConstants.istrConstUserSerialID] = lobjUserInfo.iintUserSerialId;
            ldctParams[utlConstants.istrConstFormName] = "wfmLoginMI";
            
            DataTable ldtbUserSecurity = null;
            try
            {
                ldtbUserSecurity = isrvServers.isrvBusinessTier.GetUserSecurity(model.UserName, ldctParams);
            }
            catch (Exception)
            {
                model.Message = "Unable to connect to internal Servers.";
                return View(model);
            }
            //BR - 001- 38 WSS Role Internal User only Get access to Portal
            var lenuResult = ldtbUserSecurity.AsEnumerable().Where(i => i.Field<int>("resource_id") == 2002 && i.Field<int>("security_level") > 0).AsDataTable();
            if (lenuResult.Rows.Count == 0)
            {
                model.Message = "You are not authorized to access Internal User WSS Access.";
                return View(model);
            }           
            
            
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);

            try
            {
                Hashtable lhstParam = new Hashtable();
                Int32 lint32PersonId;
                if (!int.TryParse(model.ReferenceID.ToString(), out lint32PersonId))//Test if the personid is of type int32
                {
                    model.Message = "Invalid Person ID";
                    return View(model);
                }
                lhstParam.Add("aintPersonID", model.ReferenceID);
                ldctParams["PersonID"] = model.ReferenceID;
                ldctParams["UserID"] = lobjUserInfo.istrUserId;
                ldctParams[utlConstants.istrIsMVVM] = true;
                ldctParams["IsFromWhichPortal"] = "MSS";
                ldctParams[utlConstants.istrSessionID] = idictParams[utlConstants.istrSessionID];

                busPerson lobjPerson = (busPerson)isrvNeoSpinMSSBusinessTier.ExecuteMethod("LoadPerson", lhstParam, false, ldctParams);
                if (lobjPerson.icdoPerson.person_id > 0)
                {

                    iobjSessionData["PersonID"] = lobjPerson.icdoPerson.person_id;
                    idictParams["PersonID"] = lobjPerson.icdoPerson.person_id;                     
                }

                else
                {
                    model.Message = "Invalid Person ID";
                    return View(model);
                }
                istrCookieName = "PerslinkMSSCookies";
                istrTimeOutCodeValue = "TIMS";
                base.Login(model, astrReturnUrl);
                iobjSessionData["IsFromWhichPortal"] = "MSS";
                idictParams["IsFromWhichPortal"] = "MSS";
                iobjSessionData["PersonID"] = lobjPerson.icdoPerson.person_id;
                lobjUserInfo.istrUserId = model.UserName;

                iobjSessionData[utlConstants.istrConstUserID] = lobjUserInfo.istrUserId;
                iobjSessionData[utlConstants.istrConstUserSerialID] = lobjUserInfo.iintUserSerialId;
                iobjSessionData["ColorScheme"] = "ControlsTheme"; // MSS should always bind to ControlsTheme
                iobjSessionData["UserType"] = busConstant.UserTypeInternal;
                //Setting the User Security Here to lanuch the Lookup in Impersonate Screen.        
                iobjSessionData["UserSecurity"] = ldtbUserSecurity;
                iobjSessionData["UserName"] = lobjUserInfo.istrLastName + ", " + lobjUserInfo.istrFirstName;

                iobjSessionData["MSSDisplayName"] = lobjPerson.icdoPerson.istrMSSDisplayName;
                iobjSessionData["EMAILWAIVERFLAG"] = lobjPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || lobjPerson.icdoPerson.email_waiver_flag == "N" ? "N" : "Y";
                idictParams["EMAILWAIVERFLAG"] = lobjPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || lobjPerson.icdoPerson.email_waiver_flag == "N" ? "N" : "Y";
                iobjSessionData["MessageCount"] = (int)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetMSSUnreadMessagesCount", lhstParam, false, ldctParams);

                int lintUserSerialID = 0;
                if (iobjSessionData["UserSerialID"] != null)
                    lintUserSerialID = (int)iobjSessionData["UserSerialID"];

                string lstrUserID = lobjPerson.icdoPerson.person_id.ToString() ?? string.Empty;
                if (iobjSessionData["UserId"] != null)
                    lstrUserID = Convert.ToString(iobjSessionData["UserId"]);

                //Setting the User Security
                csLoginWSSHelper.SetUserSecurityForMember(lobjPerson.icdoPerson.person_id, isrvNeoSpinMSSBusinessTier, iobjSessionData);


                //ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), this);
                iobjSessionData["UserMenu"] = null;

                //Launching the Portal

                
                iobjSessionData["RedirectOnLogoff"] = "wfmLoginMI";
                iobjSessionData["IsExternalUser"] = false; //PIR-18492: For internal User flag is False
                string lstrURL = csLoginWSSHelper.GetLaunchURLforMemberPortal(lobjPerson.icdoPerson.person_id, isrvNeoSpinMSSBusinessTier, iobjSessionData);
                iobjSessionData.idictParams = idictParams;

                isrvServers.isrvBusinessTier.StoreProcessLog(iobjSessionData["UserID"].ToString() + " -Internal User successfully logged in to WSS Member Portal", ldctParams);
                idictParams["PersonCertify"] = iobjSessionData["PersonCertify"];
                if (lstrURL.IsNotNullOrEmpty() && lstrURL != "wfmMSSSwitchMemberAccount" && model.Message.IsEmpty())
                {
                    iobjSessionData["Landing_Page"] = lstrURL;
                    iobjSessionData["InitialPage"] = lstrURL;
                    idictParams["Landing_Page"] = lstrURL;
                    return Redirect("/" + Request.Url.AbsolutePath.Split('/')[1]);
                }
                else if (lstrURL == "wfmMSSSwitchMemberAccount")
                {
                    return RedirectToAction("wfmMSSSwitchMemberAccount");
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                return View(model);
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }

        [HttpGet]
        public ActionResult wfmMSSSwitchMember(string astrReturnUrl)
        {
            ViewBag.ReturnUrl = astrReturnUrl;
            wfmMSSSwitchMember model = new wfmMSSSwitchMember();
            SetAntiForgeryToken(model);
            model.LoginWindowName = iobjSessionData?[utlConstants.istrWindowName]?.ToString();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult wfmMSSSwitchMember(wfmMSSSwitchMember model, string astrReturnUrl)
        {
            SetAntiForgeryToken(model);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                if (model.MemberID.ToString() == "0")
                {
                    model.Message = "Invalid Contact ID";
                    return View(model);
                }
                else
                {
                    Int32 lint32PersonId;
                    if (!int.TryParse(model.MemberID.ToString(), out lint32PersonId))//Test if the personid is of type int32
                    {
                        model.Message = "Invalid Person ID";
                        return View(model);
                    }

                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("aintPersonID", model.MemberID);
                    Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                    ldctParams[utlConstants.istrConstFormName] = "wfmMSSSwitchMember";
                    ldctParams[utlConstants.istrConstUserID] = Convert.ToString(model.MemberID);
                    iobjSessionData["MessageCount"] = (int)isrvNeoSpinMSSBusinessTier.ExecuteMethod("GetMSSUnreadMessagesCount", lhstParam, false, ldctParams);

                    busPerson lobjPerson = (busPerson)isrvNeoSpinMSSBusinessTier.ExecuteMethod("LoadPerson", lhstParam, false, ldctParams);
                    if (lobjPerson.icdoPerson.person_id > 0)
                    {
                        iobjSessionData["PersonID"] = lobjPerson.icdoPerson.person_id;
                        idictParams["PersonID"] = lobjPerson.icdoPerson.person_id;
                        iobjSessionData["MSSDisplayName"] = lobjPerson.icdoPerson.istrMSSDisplayName;

                        int lintUserSerialID = 0;
                        if (iobjSessionData["UserSerialID"] != null)
                            lintUserSerialID = (int)iobjSessionData["UserSerialID"];

                        string lstrUserID = lobjPerson.icdoPerson.person_id.ToString() ?? string.Empty;
                        if (iobjSessionData["UserId"] != null)
                            lstrUserID = Convert.ToString(iobjSessionData["UserId"]);

                        //Setting the User Security
                        csLoginWSSHelper.SetUserSecurityForMember(lobjPerson.icdoPerson.person_id, isrvNeoSpinMSSBusinessTier, iobjSessionData);

                        iobjSessionData["UserMenu"] = null;
                        bool lblnExternalUser = iobjSessionData["IsExternalUser"].IsNotNull() ? Convert.ToBoolean(iobjSessionData["IsExternalUser"]) : false;//PIR-18492
                                                                                                                                                             //Launching the Portal
                        string lstrURL = csLoginWSSHelper.GetLaunchURLforMemberPortal(lobjPerson.icdoPerson.person_id, isrvNeoSpinMSSBusinessTier, iobjSessionData, lblnExternalUser.ToString());
                        //Setting the Audit Trail
                        ldctParams[utlConstants.istrConstUserID] = iobjSessionData["UserID"].ToString();
                        ldctParams[utlConstants.istrConstFormName] = "wfmLoginMI";

                        isrvNeoSpinMSSBusinessTier.StoreProcessLog(iobjSessionData["UserID"].ToString() + " -Internal User successfully logged in to WSS Member Portal", ldctParams);
                        //Framework.Redirect(lstrURL);
                        if (lstrURL.IsNotNullOrEmpty() && lstrURL != "wfmMSSSwitchMemberAccount" && model.Message.IsEmpty())
                        {
                            iobjSessionData["Landing_Page"] = lstrURL;
                            iobjSessionData["InitialPage"] = lstrURL;
                            idictParams["Landing_Page"] = lstrURL;
                            return Redirect("/" + Request.Url.AbsolutePath.Split('/')[1]);
                        }
                        else if (lstrURL == "wfmMSSSwitchMemberAccount")
                        {
                            return RedirectToAction("wfmMSSSwitchMemberAccount");
                        }
                        else
                        {
                            return View(model);
                        }

                    }
                    else
                    {
                        model.Message = "Invalid Person ID";
                        return View(model);
                    }

                }
            }
            catch (Exception ex)
            {
                return View(model);
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }

        public ActionResult wfmMSSSwitchMemberAccount(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            wfmMSSSwitchMemberAccount model = new wfmMSSSwitchMemberAccount();
            SetAntiForgeryToken(model);
            model.LoginWindowName = iobjSessionData?[utlConstants.istrWindowName]?.ToString();
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult wfmMSSSwitchMemberAccount(wfmMSSSwitchMemberAccount model, string astrReturnUrl)
        {
            SetAntiForgeryToken(model);
            string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            INeoSpinBusinessTier isrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintPersonId", Convert.ToInt32(iobjSessionData["PersonID"]));
            iobjSessionData["PersonCertify"] = true;
            idictParams["PersonCertify"] = true;
            if (!(bool)isrvNeoSpinMSSBusinessTier.ExecuteMethod("IsPersonVertify", lhstParam, false, ldctParams) && Convert.ToBoolean(iobjSessionData["IsExternalUser"]))
            {
                iobjSessionData["PersonCertify"] = false;
                idictParams["PersonCertify"] = false;
                //return "wfmMSSProfileMaintenance";
            }
            try
            {
                string url1 = "/" + Request.Url.AbsolutePath.Split('/')[1];
                //if (!url1.EndsWith("/"))
                //{
                //    url1 = url1 + "/";
                //}
                if ((iobjSessionData["IsExternalUser"].IsNotNull() && Convert.ToBoolean(iobjSessionData["IsExternalUser"])) &&
                !(iobjSessionData["PersonCertify"].IsNotNull() && Convert.ToBoolean(iobjSessionData["PersonCertify"])))
                {
                    if (model.IsActiveAccountSelected == "ACTV" && Convert.ToBoolean(iobjSessionData["IsRetiree"]))
                    {
                        iobjSessionData["IsRetiree"] = false;
                        iobjSessionData["Member"] = "ActiveMember";
                    }
                    else if (model.IsActiveAccountSelected == "RETR")
                    {
                        iobjSessionData["IsRetiree"] = true;
                        iobjSessionData["Member"] = "RetireeMember";
                    }
                    iobjSessionData["Landing_Page"] = "wfmMSSProfileMaintenance";
                    iobjSessionData["InitialPage"] = "wfmMSSProfileMaintenance";
                    idictParams["Landing_Page"] = "wfmMSSProfileMaintenance";
                    return Redirect(url1);
                }
                else if (model.IsActiveAccountSelected == "ACTV")
                {
                    if (Convert.ToBoolean(iobjSessionData["IsRetiree"]))
                    {
                        iobjSessionData["IsRetiree"] = false;                       
                    }
                    iobjSessionData["Member"] = "ActiveMember";
                    if (Convert.ToString(iobjSessionData["MSSAccessValue"]) == busConstant.OrganizationLimitedAccess)
                    {
                        iobjSessionData["Landing_Page"] = "wfmMSSHomeLimited";
                        iobjSessionData["InitialPage"] = "wfmMSSHomeLimited";
                        idictParams["Landing_Page"] = "wfmMSSHomeLimited";
                        return Redirect(url1);
                    }
                    else
                    {
                        iobjSessionData["Landing_Page"] = "wfmMSSActiveMemberHomeMaintenance";
                        iobjSessionData["InitialPage"] = "wfmMSSActiveMemberHomeMaintenance";
                        idictParams["Landing_Page"] = "wfmMSSActiveMemberHomeMaintenance";
                        return Redirect(url1);
                    }
                }
                else if (model.IsActiveAccountSelected == "RETR")
                {
                    iobjSessionData["IsRetiree"] = true;
                    iobjSessionData["Member"] = "RetireeMember";
                    iobjSessionData["Landing_Page"] = "wfmMSSRetireeHomeMaintenance";
                    iobjSessionData["InitialPage"] = "wfmMSSRetireeHomeMaintenance";
                    idictParams["Landing_Page"] = "wfmMSSRetireeHomeMaintenance";
                    return Redirect(url1);
                }
                else
                {
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                return View(model);
            }
            finally
            {
                HelperFunction.CloseChannel(isrvNeoSpinMSSBusinessTier);
            }
        }

        #endregion

        #region [Public Methods]

        /// <summary>
        /// Overriding Log out method to redirect in toaspx page or controller page
        /// </summary>
        /// <param name="model"></param>
        /// <param name="astrReturnUrl"></param>
        /// <returns></returns>
        protected override void BeforeLogoutRedirect(out string astrUrl, out string astrController)
        {
            if (Convert.ToBoolean(iobjSessionData["IsExternalLogin"]))
            {
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
                {
                    astrUrl = "wfmLoginME";
                    astrController = "Account";
                }
                else
                {
                    astrUrl = "~/wfmLoginME.aspx";
                    astrController = "";
                }
            }
            else if (iobjSessionData.Keys.Contains("RedirectOnLogoff") && Convert.ToString(iobjSessionData["RedirectOnLogoff"]) == "wfmLoginMI")
            {
                astrUrl = "wfmLoginMI";
                astrController = "Account";
            }
            else if(Convert.ToBoolean(iobjSessionData["IsFromImageClick"]) == true)
            {
                astrUrl = "Logout";
                astrController = "Account";
            }
            else
            {
                astrUrl = "wfmLoginMI";
                astrController = "Account";
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
                return Redirect(Convert.ToString(ConfigurationManager.AppSettings["MSSOIDCLogoutEndpoint"]));
            }
            //return base.Logout(astrReturnUrl);
            wfmMSSSwitchMember model = new wfmMSSSwitchMember();
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
            else if((!string.IsNullOrEmpty(lstrController)) && lstrController.Equals("Account") && (!string.IsNullOrEmpty(lstrUrl) && lstrUrl.Equals("Logout")))
            {
                return View(model);
            }
            return RedirectToAction(lstrUrl);
        }
       
        #endregion [Public Methods]

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
                        return RedirectToAction("wfmLoginME", "Account");
                    }
                    string lstrMSSOIDCClientID = ConfigurationManager.AppSettings["MSSOIDCClientID"];
                    if (lstrMSSOIDCClientID.IsNullOrEmpty())
                    {
                        // MSSOIDCClientID config is missing from web.config of MSS.
                        throw new Exception("MSS - OIDCRedirect - MSSOIDCClientID config is missing from web.config.");
                    }
                    string lstrMSSOIDCClientSecret = ConfigurationManager.AppSettings["MSSOIDCClientSecret"];
                    if (lstrMSSOIDCClientSecret.IsNullOrEmpty())
                    {
                        // MSSOIDCClientSecret config is missing from web.config of MSS.
                        throw new Exception("MSS - OIDCRedirect - MSSOIDCClientSecret config is missing from web.config.");
                    }
                    string lstrMSSOIDCIssuerUrl = ConfigurationManager.AppSettings["MSSOIDCIssuerUrl"];
                    if (lstrMSSOIDCIssuerUrl.IsNullOrEmpty())
                    {
                        // MSSOIDCIssuerUrl config is missing from web.config of MSS.
                        throw new Exception("MSS - OIDCRedirect - MSSOIDCIssuerUrl config is missing from web.config.");
                    }

                    string lstrMSSOIDCCallbackUri = ConfigurationManager.AppSettings["MSSOIDCCallbackUri"];
                    if (lstrMSSOIDCCallbackUri.IsNullOrEmpty())
                    {
                        // MSSOIDCCallbackUri config is missing from web.config of MSS.
                        throw new Exception("MSS - OIDCRedirect - MSSOIDCCallbackUri config is missing from web.config.");
                    }
                    var lvarConfigUrls = new HttpClient();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    var lvarResponse = lvarConfigUrls.GetAsync(lstrMSSOIDCIssuerUrl + "/.well-known/openid-configuration").Result;
                    string lstrTokenEndpoint = null, lstrJwksUri = null;
                    if (lvarResponse.IsSuccessStatusCode)
                    {
                        string responseString = lvarResponse.Content.ReadAsStringAsync().Result;
                        JObject responseJson = JObject.Parse(responseString);
                        lstrTokenEndpoint = responseJson["token_endpoint"].ToString();
                        lstrJwksUri = responseJson["jwks_uri"].ToString();
                    }
                    ArrayList larrResult = null;
                    INeoSpinBusinessTier lsrvNeoSpinMSSBusinessTier = null;
                    string lstrUrl = String.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
                    try
                    {
                        lsrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                        Hashtable lhstParams = new Hashtable();
                        lhstParams.Add("astrAuthCode", Request.QueryString["code"]);
                        lhstParams.Add("astrTokenEndpoint", lstrTokenEndpoint);
                        lhstParams.Add("astrClientId", lstrMSSOIDCClientID);
                        lhstParams.Add("astrClientSecret", lstrMSSOIDCClientSecret);
                        lhstParams.Add("astrRedirectUrl", lstrMSSOIDCCallbackUri);
                        lhstParams.Add("astrJwksUri", lstrJwksUri);
                        lhstParams.Add("astrIssuer", lstrMSSOIDCIssuerUrl);
                        lhstParams.Add("ablnIsMember", true);
                        larrResult = (ArrayList)lsrvNeoSpinMSSBusinessTier.ExecuteMethod("OIDCMSSUserValidate", lhstParams, true, idictParams);
                    }
                    catch (Exception ex)
                    {
                        ExceptionManager.Publish(ex);
                        throw;
                    }
                    finally
                    {
                        HelperFunction.CloseChannel(lsrvNeoSpinMSSBusinessTier);
                    }
                    Dictionary<string, object> ldictUserParams = null;
                    iblnExternalUser = true;
                    utlUserInfo lutlUserInfo = null;
                    busPerson lbusPerson = null;
                    if (larrResult.Count > 0)
                    {
                        if (larrResult[0] is utlError)
                        {
                            throw new Exception((larrResult[0] as utlError).istrErrorMessage);
                        }
                        else
                        {
                            ldictUserParams = larrResult[0] as Dictionary<string, object>;
                            if (ldictUserParams.IsNotNull() && ldictUserParams.ContainsKey("busPerson") && ldictUserParams["busPerson"] is busPerson)
                            {
                                lbusPerson = (busPerson)ldictUserParams["busPerson"];
                            }
                            idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";
                            iobjSessionData["NDPERSLoginID"] = ldictUserParams["ndpers_login_id"];
                            iobjSessionData["NDPERSFirstName"] = ldictUserParams["given_name"];
                            iobjSessionData["NDPERSLastName"] = ldictUserParams["family_name"];
                            iobjSessionData["id_token"] = ldictUserParams["id_token"];
                            iobjSessionData["NDPERSLoginID"] = ldictUserParams["ndpers_login_id"];
                            idictParams[utlConstants.istrConstUserID] = ldictUserParams["ndpers_login_id"];
                            idictParams[utlConstants.istrRequestApplicationName] = ConfigurationManager.AppSettings["ApplicationName"];
                            idictParams[utlConstants.istrRequestIPAddress] = GetIP();
                            iobjSessionData["NDPERSEmailID"] = ldictUserParams["email"];
                            iobjSessionData["NDPERSFirstName"] = ldictUserParams["given_name"];
                            iobjSessionData["NDPERSLastName"] = ldictUserParams["family_name"];
                            istrCookieName = "PERSlinkMSSExternalCookie";
                            istrTimeOutCodeValue = "TIMS";
                            ModelState.Clear();
                            if (imdlLogin.IsNull())
                                imdlLogin = new LoginModel();
                            imdlLogin.UserName = Convert.ToString(ldictUserParams["ndpers_login_id"]);
                            base.Login(imdlLogin, astrReturnUrl);
                            iobjSessionData["IsFromWhichPortal"] = "MSS";
                            idictParams["IsFromWhichPortal"] = "MSS";
                            idictParams["NDPERSEmailID"] = ldictUserParams["email"];
                            DataTable ldtbSystemManagement = isrvServers.isrvDbCache.GetDBSystemManagement();
                            string lstrBaseDirectory = Convert.ToString(ldtbSystemManagement.Rows[0]["base_directory"]);
                            iobjSessionData["base_directory"] = lstrBaseDirectory;
                            iobjSessionData["Region_value"] = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);
                            iobjSessionData["PopUpMessageForCertify"] = isrvServers.isrvDbCache.GetMessageText(busConstant.PopUpMessageForCertify);
                            if (lbusPerson.icdoPerson.person_id > 0)
                            {
                                iobjSessionData["PersonID"] = lbusPerson.icdoPerson.person_id;
                                idictParams["PersonID"] = lbusPerson.icdoPerson.person_id;
                                iobjSessionData["UserID"] = Convert.ToString(ldictUserParams["ndpers_login_id"]);
                                iobjSessionData["UserSerialID"] = 0;
                                iobjSessionData["UserType"] = busConstant.UserTypeMember;
                                iobjSessionData["ColorScheme"] = "ControlsTheme";//For MSS Layout change
                                iobjSessionData["MSSDisplayName"] = lbusPerson.icdoPerson.istrMSSDisplayName;//For MSS Layout change
                                //int lintUserSerialID = 0;
                                //if (iobjSessionData["UserSerialID"] != null)
                                //    lintUserSerialID = Convert.ToInt32(iobjSessionData["UserSerialID"]);
                                iobjSessionData["EMAILWAIVERFLAG"] = lbusPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || lbusPerson.icdoPerson.email_waiver_flag == "N" ? "N" : "Y";
                                idictParams["EMAILWAIVERFLAG"] = lbusPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || lbusPerson.icdoPerson.email_waiver_flag == "N" ? "N" : "Y";
                                iobjSessionData["IsExternalUser"] = true;
                                string lstrURL = string.Empty;
                                try
                                {
                                    lsrvNeoSpinMSSBusinessTier = WCFClient<INeoSpinBusinessTier>.CreateChannel(lstrUrl);
                                    csLoginWSSHelper.SetUserSecurityForMember(lbusPerson.icdoPerson.person_id, lsrvNeoSpinMSSBusinessTier, iobjSessionData);
                                    lstrURL = csLoginWSSHelper.GetLaunchURLforMemberPortal(lbusPerson.icdoPerson.person_id, lsrvNeoSpinMSSBusinessTier, iobjSessionData, Convert.ToString(iobjSessionData["NDPERSEmailID"]), true);
                                }
                                catch (Exception ex)
                                {
                                    ExceptionManager.Publish(ex);
                                    throw;
                                }
                                finally
                                {
                                    HelperFunction.CloseChannel(lsrvNeoSpinMSSBusinessTier);
                                }
                                idictParams["UserSecurity"] = iobjSessionData["UserSecurity"];
                                isrvServers.isrvBusinessTier.StoreProcessLog("PERSLink ID " + lbusPerson.icdoPerson.person_id.ToString() + " - External User successfully logged in to WSS Member Portal", idictParams);
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
                                string url1 = string.Empty;
                                if (lstrURL.Equals("wfmMSSSwitchMemberAccount"))
                                {
                                    return RedirectToAction("wfmMSSSwitchMemberAccount");
                                }
                                else
                                {
                                    return Redirect("/" + Request.Url.AbsolutePath.Split('/')[1]);
                                }
                            }
                            else
                            {
                                return Redirect("~/wfmRequestOnlineAccessForMember.aspx");
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
                //    throw new Exception("MSS - OIDCRedirect - Config Section is missing from Web.Config.");
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
