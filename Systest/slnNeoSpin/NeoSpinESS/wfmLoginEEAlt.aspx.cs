using NeoSpin.BusinessObjects;
using Sagitec.MVVMClient;
using Sagitec.WebClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NeoSpinESS
{
    public partial class wfmLoginEEAlt : wfmClientBasePage
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
            base.OnInit(e);
        }

        string lstrCookieName = "WSSCookie";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs args)
        {
            string lstrNDPERSLoginID = "rkathait";
            DataTable ldtEmployer = new DataTable();
            int aintContactID = Convert.ToInt32(txbContactID.Text.Trim());
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintContactID", aintContactID);
            busContact lobjContact = (busContact)isrvBusinessTier.ExecuteMethod("LoadContact", lhstParam, false, new Dictionary<string, object>());
            if (lobjContact.icdoContact.contact_id > 0)
            {
                //wfmMainDB.SetAuthenticatedWindow(hfldLoginWindowName.Value);
                //This Session needs to be set here to pass the value to Request Online Page
                iobjSessionData["NDPERSLoginID"] = lstrNDPERSLoginID;
                iobjSessionData["NDPERSEmailID"] = lobjContact.icdoContact.email_address;
                iobjSessionData["NDPERSFirstName"] = lobjContact.icdoContact.first_name;
                iobjSessionData["NDPERSLastName"] = lobjContact.icdoContact.last_name;
                iobjSessionData["RedirecOnLogoff"] = "wfmLoginEEAlt.aspx";

                iobjSessionData["ContactID"] = lobjContact.icdoContact.contact_id;
                iobjSessionData["UserID"] = string.Empty;
                iobjSessionData["UserSerialID"] = 0;
                iobjSessionData["UserType"] = busConstant.UserTypeEmployer;
                iobjSessionData["ColorScheme"] = string.Empty;

                lhstParam = new Hashtable();
                lhstParam.Add("aintContactID", lobjContact.icdoContact.contact_id);

                ldtEmployer = (DataTable)isrvBusinessTier.ExecuteMethod("GetEmployersForContact", lhstParam, false, new Dictionary<string, object>());
                bool lblnValidOrg = false;
                foreach (DataRow ldrRow in ldtEmployer.Rows)
                {
                    if (ldrRow["org_code"].ToString() == txbOrgCode.Text.Trim())
                    {
                        if (ldrRow["Emp_Type"].ToString().ToLower() == txbEmpType.Text.ToLower())
                        {
                            lblnValidOrg = true;
                            iobjSessionData["OrgID"] = ldrRow["org_id"];
                            break;
                        }
                    }
                }
                if (!lblnValidOrg)
                {
                    lblError.Text = "Selected Contact Id is not mapped to any Organization. Please select another contact to proceed.";
                }
                else
                {
                    string lstrURL = csLoginWSSHelper.SetSessionAndLaunchEmployerPortalHome(Convert.ToInt32(iobjSessionData["OrgID"]), Convert.ToInt32(iobjSessionData["ContactID"]), isrvBusinessTier,iobjSessionData);

                    ArrayList larrMenu = wfmMainDB.LoadMenu(Server.MapPath("Web.sitemap"), this);
                    iobjSessionData["UserMenu"] = larrMenu;

                    Framework.Redirect(lstrURL);
                }
            }
            else
            {
                lblError.Text = "Invalid Contact Id";
            }
        }
    }
}