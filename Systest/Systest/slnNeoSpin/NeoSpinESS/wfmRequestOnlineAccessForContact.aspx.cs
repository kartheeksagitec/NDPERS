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
    public partial class wfmRequestOnlineAccessForContact : wfmClientBasePage
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
        public DataTable idtbQuestions { get; set; }

        private Dictionary<string, object> idctParams;
        private void InitializeUserParams()
        {
            idctParams = new Dictionary<string, object>();

            if (iobjSessionData["UserSerialID"] != null)
                idctParams[utlConstants.istrConstUserSerialID] = (int)iobjSessionData["UserSerialID"];
            if (iobjSessionData["UserId"] != null)
                idctParams[utlConstants.istrConstUserID] = Convert.ToString(iobjSessionData["UserId"]);
        }

        protected override void Page_Load(object sender, EventArgs e)
        {

            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                if (!IsPostBack)
                {
                    InitializeUserParams();
                    Hashtable lhstParam = new Hashtable();
                    idtbQuestions = (DataTable)lsrvBusinessTier.ExecuteMethod("GetESSQuestionsForOnlineAccess", lhstParam, false, idctParams);

                    ddlQuestions.DataSource = idtbQuestions;
                    ddlQuestions.DataTextField = "description";
                    ddlQuestions.DataValueField = "code_value";
                    ddlQuestions.DataBind();
                }
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

        protected void btnRequestAccess_Click(object sender, EventArgs e)
        {
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                InitializeUserParams();
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("astrOrgCodeID", txbOrgCode.Text.ToString());
                lhstParam.Add("aintContactID", Convert.ToInt32(txbContactID.Text));
                lhstParam.Add("astrQuestionCode", ddlQuestions.SelectedValue.ToString());
                lhstParam.Add("astrAnswer", txbAnswer.Text.ToString());
                lhstParam.Add("astrNDPERSLoginID", iobjSessionData["NDPERSLoginID"].ToString());
                lhstParam.Add("astrFirstName", iobjSessionData["NDPERSFirstName"].ToString());
                lhstParam.Add("astrLastName", iobjSessionData["NDPERSLastName"].ToString());
                int lintErrorID = (int)lsrvBusinessTier.ExecuteMethod("GrantOnlineAccessForEmployer", lhstParam, false, idctParams);

                // PIR 10266 -- Throw specific errors.
                switch (lintErrorID)
                {
                    case 0:
                        busOrganization lobjOrganization = new busOrganization { icdoOrganization = new NeoSpin.CustomDataObjects.cdoOrganization() };
                        Hashtable lhstParam1 = new Hashtable();
                        lhstParam1.Add("astrOrgCodeID", txbOrgCode.Text.ToString());
                        lobjOrganization = (busOrganization)lsrvBusinessTier.ExecuteMethod("LoadOrganizationByOrgCode", lhstParam1, false, idctParams);
                        //Adding User to SecureWay Group
                        if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
                            csLoginWSSHelper.AddUserToSecureWayGroup(lsrvBusinessTier, iobjSessionData);
                        iobjSessionData["OrgID"] = lobjOrganization.icdoOrganization.org_id;
                        iobjSessionData["ContactID"] = Convert.ToInt32(txbContactID.Text);
                        iobjSessionData["OrgName"] = lobjOrganization.icdoOrganization.org_name;
                        iobjSessionData["OrgCode"] = lobjOrganization.icdoOrganization.org_code;
                        iobjSessionData["UserType"] = busConstant.UserTypeEmployer;
                        ////Check Contact Id is WellnessCoord or Other role.
                        //if (IsContactWellnessCoordAndOther(Convert.ToInt32(Session["OrgID"]), Convert.ToInt32(Session["ContactID"])))
                        //{
                        string lstrURL = csLoginWSSHelper.SetSessionAndLaunchEmployerPortalHome((int)iobjSessionData["OrgID"], (int)iobjSessionData["ContactID"], lsrvBusinessTier, iobjSessionData);
                        iobjSessionData["Landing_Page"] = lstrURL;
                        iobjSessionData.idictParams["Landing_Page"] = lstrURL;
                        iobjSessionData.idictParams["InitialPage"] = lstrURL;
                        iobjSessionData["UserLoggedOn"] = "true";
                        iobjSessionData["IsExternalLogin"] = true;
                        iobjSessionData.idictParams["IsExternalLogin"] = true;
                        iobjSessionData.idictParams["UserLoggedOn"] = "true";
                        iobjSessionData.idictParams["IsfromESSPortal"] = "true";
                        iobjSessionData["UserLoggedOn"] = "true";
                        Framework.SessionForWindow["UserLoggedOn"] = "true";
                        iobjSessionData["IsFromWhichPortal"] = "ESS";
                        iobjSessionData.idictParams["IsFromWhichPortal"] = "ESS";
                        iobjSessionData.idictParams["IsPAG"] = iobjSessionData["IsPAG"];
                        Framework.istrWindowName = iobjSessionData[utlConstants.istrWindowName].ToString();
                        iobjSessionData["WindowName"] = Framework.istrWindowName;
                        //F/W PIR 21660 Ess login issue - Inactive Employer Login (External Login)
                        iobjSessionData.idictParams[utlConstants.istrConstUserID] = iobjSessionData["NDPERSLoginID"];
                        iobjSessionData.idictParams[utlConstants.istrRequestApplicationName] = istrApplicationName;
                        iobjSessionData.idictParams[utlConstants.istrWindowName] = Framework.istrWindowName;
                        iobjSessionData.idictParams[utlConstants.istrRequestInvalidLoginFlag] = "N";
                        string url1 = UrlHelper.GenerateUrl("Default", "Index", "Home", null, RouteTable.Routes, HttpContext.Current.Request.RequestContext, false);
                        if (System.Configuration.ConfigurationManager.AppSettings["IsRootPath"] != null && Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsRootPath"]))
                        {

                        }
                        else
                        {
                            url1 = url1.Substring(0, url1.Length - 1);
                        }
                        Framework.Redirect(url1);
                        break;
                    case 1:
                        lblError.Text = "Sorry! First/Last Name does not match name on file at NDPERS. Please contact your Payroll Officer.";
                        break;
                    case 2:
                        lblError.Text = "Sorry! Answers for the validation question is wrong! Please contact PERSLink Administrator!";
                        break;
                    case 3:
                        lblError.Text = "Sorry! Invalid Contact ID for the Organization. Please contact your Payroll Officer.";
                        break;
                    case 4:
                        lblError.Text = "Sorry! Invalid Organization ID. Please contact your Payroll Officer.";
                        break;
                    case 5:
                        lblError.Text = "You are not authorized to access WSS Screens.";
                        break;
                    case 6:
                        lblError.Text = "Sorry! User ID already exists. Please contact your Payroll Officer.";
                        break;
                    default:
                        lblError.Text = "Sorry! Authorization Failed! Please contact PERSLink Administrator!";
                        break;
                }
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
            
        }


        // Check contact Id is WellnessCoord or Other role.
        public bool IsContactWellnessCoordAndOther(int aintOrgID, int aintContactID)
        {
            Hashtable lhstParam = new Hashtable();
            lhstParam = new Hashtable();
            lhstParam.Add("aintOrgID", aintOrgID);
            lhstParam.Add("aintContactID", aintContactID);

            DataTable ldtUserSecurity = (DataTable)isrvBusinessTier.ExecuteMethod("GetOrgContactRoles", lhstParam, false, new Dictionary<string, object>());
            var x = ldtUserSecurity.AsEnumerable().Where(row => (row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleWellnessCoordinator || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleOther)
                && (row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleAgent) || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleAuthorizedAgent
                || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRoleFinance || row.Field<string>("CONTACT_ROLE_VALUE") == busConstant.OrgContactRolePrimaryAuthorizedAgent);
            if (x.Count() > 0)
                return true;
            return false;
        }
    }
}