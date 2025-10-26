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
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NeoSpinESS
{
    public partial class wfmEmployerSelection : wfmClientBasePage
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
        public DataTable idtbEmployer { get; set; }
        protected override void Page_Load(object sender, EventArgs e)
        {
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                if (!IsPostBack)
                {
                    Hashtable lhstParam = new Hashtable();
                    int lintContactID = (int)iobjSessionData["ContactID"];
                    lhstParam.Add("aintContactID", lintContactID);
                    //Framework.SessionRemove("dtbEmployer");
                    iobjSessionData.Remove("dtbEmployer");
                    iobjSessionData["IsFromWhichPortal"] = "ESS";
                    idictParams["IsFromWhichPortal"] = "ESS";
                    idtbEmployer = (DataTable)lsrvBusinessTier.ExecuteMethod("GetEmployersForContact", lhstParam, false, new Dictionary<string, object>());
                    if (idtbEmployer.Rows.Count == 0)
                    {
                        iobjSessionData["dtbEmployerCount"] = "";
                        lblError.Text = "You are not authorized to view any organization details. Please contact ITD for further assistance.";
                    }
                    else if (idtbEmployer.Rows.Count == 1)
                    {
                        iobjSessionData["dtbEmployerCount"] = "";
                        iobjSessionData["OrgID"] = idtbEmployer.Rows[0]["org_id"];
                        iobjSessionData["OrgCode"] = idtbEmployer.Rows[0]["org_code"];
                        iobjSessionData["OrgName"] = idtbEmployer.Rows[0]["org_name"];
                        string lstrURL = csLoginWSSHelper.SetSessionAndLaunchEmployerPortalHome(Convert.ToInt32(iobjSessionData["OrgID"]), Convert.ToInt32(iobjSessionData["ContactID"]), lsrvBusinessTier, iobjSessionData);

                        //Added for 9 Apr Framework changes - START
                        if (iobjSessionData["UserType"].ToString() == busConstant.UserTypeInternal)
                        {
                            idictParams[utlConstants.istrConstUserID] = iobjSessionData["UserID"];
                            idictParams[utlConstants.istrRequestApplicationName] = istrApplicationName;
                        }
                        else
                        {
                            idictParams[utlConstants.istrConstUserID] = iobjSessionData["NDPERSLoginID"];
                            idictParams[utlConstants.istrRequestApplicationName] = istrApplicationName;
                        }
                        idictParams[utlConstants.istrRequestMACAddress] = "00:00:00:00";  // Replace with actual MAC Address
                        idictParams[utlConstants.istrRequestIPAddress] = GetIPAddress();
                        idictParams[utlConstants.istrRequestMachineName] = Request.UserHostName;
                        idictParams[utlConstants.istrConstUserSerialID] = iobjSessionData["UserSerialID"];

                        idictParams[utlConstants.istrWindowName] = Framework.istrWindowName;
                        idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";

                        iobjSessionData[utlConstants.istrLogInfo] = iobjTraceInfo.iobjLogInfo;
                        iobjSessionData[utlConstants.istrLogQuery] = false;
                        //Tracing Code Start
                        //int lintKeyValue = isrvBusinessTier.LogUserLogin(Context.Session.SessionID, DateTime.Now, idictParams);
                        //Framework.SessionForWindow["UserActivityLogId"] = lintKeyValue;
                        //Framework.SessionForWindow[utlConstants.istrActivityLogLevel] = 3;//lobjUserInfo.iintActivityLogLevel;
                        //Framework.SessionForWindow[utlConstants.istrActivityLogSelectQuery] = true;//lobjUserInfo.iblnLogSelectQuery;
                        //Framework.SessionForWindow[utlConstants.istrActivityLogInsertQuery] = true;//lobjUserInfo.iblnLogInsertQuery;
                        //Framework.SessionForWindow[utlConstants.istrActivityLogUpdateQuery] = true;//lobjUserInfo.iblnLogUpdateQuery;
                        //Framework.SessionForWindow[utlConstants.istrActivityLogDeleteQuery] = true;//lobjUserInfo.iblnLogDeleteQuery;
                        //Tracing Code End

                        //ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), this);
                        //iobjSessionData["UserMenu"] = larrMenu;

                        iobjSessionData["Landing_Page"] = lstrURL;
                        idictParams["Landing_Page"] = lstrURL;
                        idictParams["InitialPage"] = lstrURL;
                        iobjSessionData["UserLoggedOn"] = "true";
                        iobjSessionData["IsExternalLogin"] = true;
                        idictParams["IsExternalLogin"] = true;
                        idictParams["UserLoggedOn"] = "true";
                        idictParams["IsfromESSPortal"] = "true";
                        iobjSessionData["UserLoggedOn"] = "true";
                        Framework.SessionForWindow["UserLoggedOn"] = "true";
                        idictParams["IsPAG"] = iobjSessionData["IsPAG"];
                        idictParams["NDPERSEmailIDESS"] = iobjSessionData["NDPERSEmailIDESS"];
                        //iobjSessionData["LogoutAction"] = "~/wfmLoginEE.aspx";
                        Framework.istrWindowName = iobjSessionData[utlConstants.istrWindowName].ToString();
                        iobjSessionData["WindowName"] = Framework.istrWindowName;
                        iobjSessionData["dictParams"] = idictParams;
                        iobjSessionData.idictParams = idictParams;
                        string url1 = UrlHelper.GenerateUrl("Default", "Index", "Home", null, RouteTable.Routes, HttpContext.Current.Request.RequestContext, false);
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
                        Framework.Redirect(url1);
                    }
                    else
                    {
                                             
                        iobjSessionData["dtbEmployerCount"] = "Multiple";
                        iobjSessionData["dtbEmployer"] = idtbEmployer;                        
                        ddlEmployers.DataSource = idtbEmployer;
                        ddlEmployers.DataTextField = "org_name";
                        ddlEmployers.DataValueField = "org_id";
                        ddlEmployers.sfwDataField = "OrgCode";                        
                        ddlEmployers.DataBind();
                        ddlEmployers.Items.Insert(0, new ListItem("---Select---", ""));
                        ddlEmployers.Items[0].Selected = true;
                    }
                }
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
            
        }
        public bool IsContactWellnessCoordAndOther(int aintOrgID, int aintContactID)
        {
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                Hashtable lhstParam = new Hashtable();
                lhstParam = new Hashtable();
                lhstParam.Add("aintOrgID", aintOrgID);
                lhstParam.Add("aintContactID", aintContactID);

                DataTable ldtUserSecurity = (DataTable)lsrvBusinessTier.ExecuteMethod("GetOrgContactRoles", lhstParam, false, new Dictionary<string, object>());
                var x = ldtUserSecurity.AsEnumerable().Where(row => (row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleWellnessCoordinator || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleOther)
                    && (row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleAgent) || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleAuthorizedAgent
                    || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleFinance || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRolePrimaryAuthorizedAgent);
                if (x.Count() > 0)
                    return true;
                return false;
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }            
        }

        protected void btnSelect_Click(object sender, EventArgs e)
        {
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                if (ddlEmployers.SelectedValue != "")
                {
                    iobjSessionData["OrgID"] = Convert.ToInt32(ddlEmployers.SelectedValue);
                    iobjSessionData["OrgName"] = ddlEmployers.SelectedItem;
                }
                else
                {
                    lblError.Text = "You are not authorized to access WSS Screens for selected Employer. Please select different Employer. <br> The ORG_ID field is required.";
                    return;
                }
                
                idtbEmployer = (DataTable)iobjSessionData["dtbEmployer"];
                var results = (from m in idtbEmployer.AsEnumerable()
                               where m.Field<Int32>("org_id") == Convert.ToInt32(ddlEmployers.SelectedValue)
                               select m).FirstOrDefault();
                iobjSessionData["OrgCode"] = results.ItemArray[1];

                string lstrURL = csLoginWSSHelper.SetSessionAndLaunchEmployerPortalHome(Convert.ToInt32(iobjSessionData["OrgID"]), Convert.ToInt32(iobjSessionData["ContactID"]), lsrvBusinessTier, iobjSessionData);

                //ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), this);
                //iobjSessionData["UserMenu"] = larrMenu;
                if (IsContactWellnessCoordAndOther(Convert.ToInt32(iobjSessionData["OrgID"]), Convert.ToInt32(iobjSessionData["ContactID"])))
                {
                    //Added for 9 Apr Framework changes - START
                    if (iobjSessionData["UserType"].ToString() == busConstant.UserTypeInternal)
                    {
                        idictParams[utlConstants.istrConstUserID] = iobjSessionData["UserID"];
                        idictParams[utlConstants.istrRequestApplicationName] = istrApplicationName;
                    }
                    else
                    {
                        idictParams[utlConstants.istrConstUserID] = iobjSessionData["NDPERSLoginID"];
                        idictParams[utlConstants.istrRequestApplicationName] = istrApplicationName;
                    }
                    idictParams[utlConstants.istrRequestMACAddress] = "00:00:00:00";  // Replace with actual MAC Address
                    idictParams[utlConstants.istrRequestIPAddress] = GetIPAddress();
                    idictParams[utlConstants.istrRequestMachineName] = Request.UserHostName;
                    idictParams[utlConstants.istrConstUserSerialID] = Convert.ToInt32(iobjSessionData["UserSerialID"]);
                    idictParams[utlConstants.istrWindowName] = Framework.istrWindowName;
                    idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";
                    //Tracing Code Start
                    iobjSessionData[utlConstants.istrLogInfo] = iobjTraceInfo.iobjLogInfo;
                    iobjSessionData[utlConstants.istrLogQuery] = false;

                    //int lintKeyValue = isrvBusinessTier.LogUserLogin(Context.Session.SessionID, DateTime.Now, idictParams);
                    //Framework.SessionForWindow["UserActivityLogId"] = lintKeyValue;
                    //Framework.SessionForWindow[utlConstants.istrActivityLogLevel] = 3;//lobjUserInfo.iintActivityLogLevel;
                    //Framework.SessionForWindow[utlConstants.istrActivityLogSelectQuery] = true;//lobjUserInfo.iblnLogSelectQuery;
                    //Framework.SessionForWindow[utlConstants.istrActivityLogInsertQuery] = true;//lobjUserInfo.iblnLogInsertQuery;
                    //Framework.SessionForWindow[utlConstants.istrActivityLogUpdateQuery] = true;//lobjUserInfo.iblnLogUpdateQuery;
                    //Framework.SessionForWindow[utlConstants.istrActivityLogDeleteQuery] = true;//lobjUserInfo.iblnLogDeleteQuery;
                    //Tracing Code End
                    iobjSessionData["Landing_Page"] = lstrURL;
                    idictParams["Landing_Page"] = lstrURL;
                    idictParams["InitialPage"] = lstrURL;
                    iobjSessionData["UserLoggedOn"] = "true";
                    iobjSessionData["IsExternalLogin"] = true;
                    idictParams["IsExternalLogin"] = true;
                    idictParams["UserLoggedOn"] = "true";
                    idictParams["IsfromESSPortal"] = "true";
                    iobjSessionData["UserLoggedOn"] = "true";
                    iobjSessionData["IsFromWhichPortal"] = "ESS";
                    idictParams["IsFromWhichPortal"] = "ESS";
                    //iobjSessionData["LogoutAction"] = "~/wfmLoginEE.aspx";
                    Framework.SessionForWindow["UserLoggedOn"] = "true";
                    idictParams["IsPAG"] = iobjSessionData["IsPAG"];
                    idictParams["NDPERSEmailIDESS"] = iobjSessionData["NDPERSEmailIDESS"];
                    Framework.istrWindowName = iobjSessionData[utlConstants.istrWindowName].ToString();
                    iobjSessionData["WindowName"] = Framework.istrWindowName;
                    iobjSessionData["dictParams"] = idictParams;
                    iobjSessionData.idictParams = idictParams;
                    string url1 = UrlHelper.GenerateUrl("Default", "Index", "Home", null, RouteTable.Routes, HttpContext.Current.Request.RequestContext, false);
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
                    Framework.Redirect(url1);
                }
                else
                    lblError.Text = "You are not authorized to access WSS Screens for selected Employer. Please select different Employer.";
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }            
        }

        protected override void OnInit(EventArgs e)
        {
            istrFormName = "wfmLoginWSS";
            base.OnInit(e);
        }
    }
}