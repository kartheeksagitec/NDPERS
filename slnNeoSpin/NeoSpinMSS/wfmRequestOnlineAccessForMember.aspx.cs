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

namespace NeoSpinMSS
{
    public partial class wfmRequestOnlineAccessForMember : wfmMainDB
    {
        protected MVVMSession iobjSessionData;
        public DataTable idtbQuestions { get; set; }

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
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                btnRequestAccess.Attributes.Add("OnClick", "this.value='Please Wait...';this.disabled = true;" + this.GetPostBackEventReference(btnRequestAccess)); //pir 8631                
                ListItem lstItem;
                for (int i = 1; i <= 12; i++)
                {
                    lstItem = new ListItem();
                    lstItem.Value = i.ToString();
                    lstItem.Text = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(i);
                    ddlMonth.Items.Add(lstItem);
                }
                for (int i = 1; i <= 31; i++)
                {
                    lstItem = new ListItem();
                    lstItem.Value = i.ToString();
                    lstItem.Text = i.ToString();
                    ddlDay.Items.Add(lstItem);
                }
                for (int i = DateTime.Today.Year - 120; i < DateTime.Today.Year + 10; i++)
                {
                    lstItem = new ListItem();
                    lstItem.Value = i.ToString();
                    lstItem.Text = i.ToString();
                    ddlYear.Items.Add(lstItem);
                }
            }
        }
        protected override void OnInit(EventArgs e)
        {
            istrFormName = "wfmLoginWSS";
            base.OnInit(e);
        }
        protected virtual void SetSessionValidationCookie(string astrUserId)
        {
            HttpCookie lstrMVVMSessionAuthenticationCookie = new HttpCookie(istrApplicationName + utlConstants.istrMVVMSessionValidationCookieKey);
            lstrMVVMSessionAuthenticationCookie.HttpOnly = true;
            lstrMVVMSessionAuthenticationCookie.Value = HelperFunction.SagitecEncrypt(null, astrUserId);
            Response.Cookies.Add(lstrMVVMSessionAuthenticationCookie);
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
        protected void btnRequestAccess_Click(object sender, EventArgs e)
        {
            /*if (ddlQuestion1.SelectedValue == ddlQuestion2.SelectedValue)
            {
                lblError.Text = "Both questions can not be same!";
                return;
            }*/
            //MSS Pir 7114
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                Hashtable lhstParam = new Hashtable();
                Int32 lint32PersonId;
                if (txbPERSLinkID.Text.IsNumeric() && int.TryParse(txbPERSLinkID.Text, out lint32PersonId))//Test if the personid is of type int32
                {
                    lhstParam.Add("aintPersonID", Convert.ToInt32(txbPERSLinkID.Text));
                    lhstParam.Add("astrQuestion1Code", busConstant.Question_SSN);
                    lhstParam.Add("astrAnswer1", txbLast4DigitsSSN.Text.ToString());
                    lhstParam.Add("astrQuestion2Code", busConstant.Question_DateOfBirth);
                    lhstParam.Add("astrAnswer2", ddlMonth.SelectedValue.ToString() + "/" + ddlDay.SelectedValue.ToString() + "/" + ddlYear.SelectedValue.ToString());
                    lhstParam.Add("astrNDPERSLoginID", iobjSessionData["NDPERSLoginID"].ToString());
                    lhstParam.Add("astrFirstName", iobjSessionData["NDPERSFirstName"].ToString());
                    lhstParam.Add("astrLastName", iobjSessionData["NDPERSLastName"].ToString());

                    Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                    ldctParams[utlConstants.istrConstUserID] = txbPERSLinkID.Text;
                    int lintErrorID = (int)lsrvBusinessTier.ExecuteMethod("GrantOnlineAccessForMember", lhstParam, false, ldctParams);

                    // PIR 10266 -- Throw specific errors.
                    switch (lintErrorID)
                    {
                        case 0:
                            busPerson lobjPerson = new busPerson { icdoPerson = new NeoSpin.CustomDataObjects.cdoPerson() };
                            Hashtable lhstParam1 = new Hashtable();
                            lhstParam1.Add("aintPersonID", Convert.ToInt32(txbPERSLinkID.Text));
                            lobjPerson = (busPerson)lsrvBusinessTier.ExecuteMethod("LoadPerson", lhstParam1, false, ldctParams);
                            iobjSessionData["PersonID"] = lobjPerson.icdoPerson.person_id;
                            iobjSessionData["MSSDisplayName"] = lobjPerson.icdoPerson.istrMSSDisplayName;
                            iobjSessionData["ColorScheme"] = "ControlsTheme";//For MSS Layout change
                            int lintUserSerialID = 0;
                            if (iobjSessionData["UserSerialID"] != null)
                                lintUserSerialID = (int)iobjSessionData["UserSerialID"];
                            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
                                csLoginWSSHelper.AddUserToSecureWayGroup(isrvBusinessTier, iobjSessionData);
                            csLoginWSSHelper.SetUserSecurityForMember(lobjPerson.icdoPerson.person_id, isrvBusinessTier, iobjSessionData);
                            iobjSessionData.idictParams["UserSecurity"] = iobjSessionData["UserSecurity"];
                            string lstrURL = csLoginWSSHelper.GetLaunchURLforMemberPortal(lobjPerson.icdoPerson.person_id, isrvBusinessTier, iobjSessionData, (string)iobjSessionData["NDPERSEmailID"], true);
                            iobjSessionData["Landing_Page"] = lstrURL;
                            iobjSessionData["InitialPage"] = lstrURL;
                            iobjSessionData["UserLoggedOn"] = "true";
                            iobjSessionData["IsExternalLogin"] = true;
                            iobjSessionData.idictParams["IsExternalLogin"] = true;
                            iobjSessionData.idictParams["UserLoggedOn"] = "true";
                            iobjSessionData.idictParams["IsfromMSSPortal"] = "true";
                            iobjSessionData.idictParams["IsFromWhichPortal"] = "MSS";

                            DataTable ldtbSystemManagement = null;
                            ldtbSystemManagement = isrvDBCache.GetDBSystemManagement();
                            string lstrBaseDirectory = Convert.ToString(ldtbSystemManagement.Rows[0]["base_directory"]);
                            iobjSessionData["base_directory"] = lstrBaseDirectory;
                            iobjSessionData["Region_value"] = Convert.ToString(ldtbSystemManagement.Rows[0]["Region_value"]);
                            iobjSessionData["PopUpMessageForCertify"] = GetMessageText(lsrvBusinessTier, ldctParams, busConstant.PopUpMessageForCertify);
                            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
                                iobjSessionData.idictParams[utlConstants.istrConstUserID] = idictParams[utlConstants.istrConstUserID];
                            iobjSessionData["UserLoggedOn"] = "true";
                            Framework.SessionForWindow["UserLoggedOn"] = "true";
                            iobjSessionData.idictParams["PersonCertify"] = iobjSessionData["PersonCertify"];
                            iobjSessionData["EMAILWAIVERFLAG"] = lobjPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || lobjPerson.icdoPerson.email_waiver_flag == "N" ? "N" : "Y";
                            iobjSessionData.idictParams["EMAILWAIVERFLAG"] = lobjPerson.icdoPerson.email_waiver_flag.IsNullOrEmpty() || lobjPerson.icdoPerson.email_waiver_flag == "N" ? "N" : "Y";
                            Framework.istrWindowName = iobjSessionData[utlConstants.istrWindowName].ToString();
                            iobjSessionData["WindowName"] = Framework.istrWindowName;
                            if (!Convert.ToBoolean(ConfigurationManager.AppSettings["IsPublicAuthLoginEnabled"]))
                                SetSessionValidationCookie(idictParams[utlConstants.istrConstUserID].ToString());


                            string url1 = string.Empty;
                            if (iobjSessionData["IsMemberBothRetireeAndActive"].Equals(true))
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
                                }
                            }
                            Framework.Redirect(url1);
                            break;
                        case 1:
                            lblError.Text = "Sorry! Last Name does not match name on file at NDPERS. Please contact your Payroll Officer.";
                            break;
                        case 2:
                            lblError.Text = "Sorry! Entered Date of Birth does not match on file at NDPERS! Please contact your Payroll Officer.";
                            break;
                        case 3:
                            lblError.Text = "Sorry! Entered SSN Info does not match on file at NDPERS! Please contact your Payroll Officer.";
                            break;
                        case 4:
                            lblError.Text = "Sorry! Member deceased as per NDPERS file. Please contact your Payroll Officer.";
                            break;
                        case 5:
                            lblError.Text = "Sorry! Invalid Member ID. Please contact your Payroll Officer.";
                            break;
                        default:
                            lblError.Text = "Sorry! Authorization Failed! Please contact PERSLink Administrator!";
                            break;
                    }
                }
                else
                {
                    lblError.Text = "Sorry! Member ID must be numeric!";
                }
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
        }
            
    }
}