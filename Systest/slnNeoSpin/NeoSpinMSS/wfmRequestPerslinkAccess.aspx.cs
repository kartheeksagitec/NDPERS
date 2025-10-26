using NeoSpin.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using Sagitec.Interface;
using Sagitec.MVVMClient;
using Sagitec.WebClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NeoSpinMSS
{
    public partial class wfmRequestPerslinkAccess : wfmMainDB
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
                //pir 8168 start
                ListItem lstItem;
                lstItem = new ListItem();
                lstItem.Value = "";
                lstItem.Text = "";
                ddlMonth.Items.Add(lstItem);
                ddlDay.Items.Add(lstItem);
                ddlYear.Items.Add(lstItem);
                //end
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

        Dictionary<string, object> idctParams;
        private void InitializeUserParams()
        {
            idctParams = new Dictionary<string, object>();

            if (iobjSessionData["UserSerialID"] != null)
                idctParams[utlConstants.istrConstUserSerialID] = (int)iobjSessionData["UserSerialID"];
            if (iobjSessionData["UserId"] != null)
                idctParams[utlConstants.istrConstUserID] = Convert.ToString(iobjSessionData["UserId"]);
        }


        protected void btnRequestAccess_Click(object sender, EventArgs e)
        {
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeospinWss");
            IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
            try
            {
                InitializeUserParams();
                Hashtable lhstParam = new Hashtable();
                lhstParam.Add("astrLastName", txbLastName.Text.ToString());
                lhstParam.Add("astrSSN", txbLast4DigitsSSN.Text.ToString());
                lhstParam.Add("astrDOB", ddlMonth.SelectedValue.ToString() + "/" + ddlDay.SelectedValue.ToString() + "/" + ddlYear.SelectedValue.ToString());
                busPerson lobjPerson = (busPerson)lsrvBusinessTier.ExecuteMethod("GrantPersLinkAccess", lhstParam, false, idctParams);
                String Email = "";
                
                try
                {
                    if (lobjPerson != null && lobjPerson.icdoPerson != null && lobjPerson.icdoPerson.person_id > 0)
                    {
                        Email = lobjPerson.icdoPerson.email_address;
                        if (lobjPerson.icdoPerson.communication_preference_value == busConstant.PersonCommPrefRegMail) // PIR 10400
                        {
                            InitializeUserParams();
                            lhstParam = new Hashtable();
                            //  lhstParam.Add(lobjPerson);
                            lhstParam.Add("lobjPerson", lobjPerson);
                            lsrvBusinessTier.ExecuteMethod("GenerateCorrespondenceForSendingPersLinkID", lhstParam, false, idctParams);
                            // PIR 9051 - Change Message Description
                            string lstrScript = "alert('Your PERSLink Member Id will be sent to your home mailing address that is on file with NDPERS if not requested within last 24 hours.'); window.close();";//pir 8631
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open Message Box", lstrScript, true);
                        }
                        else if (lobjPerson.icdoPerson.communication_preference_value == busConstant.PersonCommPrefMail && Email.IsNotNullOrEmpty())
                        {
                            lblError.Text = string.Empty;
                            string lstrBody = "Dear " + lobjPerson.icdoPerson.last_name + " " + lobjPerson.icdoPerson.first_name + ":\n\nYour PERSLink Member ID was requested through the NDPERS On-line Member ID application.  Your Member ID is : " + lobjPerson.icdoPerson.person_id.ToString() + ".\n\nIf you did not request your Member ID, please call NDPERS at 701-328-3900 or 800-803-7377.\n\nThank you,\n\nNorth Dakota Public Employees Retirement System";
                            //HelperFunction.SendMail(AppSettingsHelper.Instance.Configuration["WSSRetrieveMailFrom"].ToString(), Email, "Request for NDPERS MEMBER ID", msg);
                            string lstrSubject = "PERSLink Member ID";
                            string lstrMailFrom = ConfigurationManager.AppSettings["WSSRetrieveMailFrom"];
                            string lstrSmptpServer = ConfigurationManager.AppSettings["SmtpServer"];
                            SmtpClient lsmc = new SmtpClient(lstrSmptpServer);
                            lsmc.Send(lstrMailFrom, Email, lstrSubject, lstrBody);
                            lsmc.Dispose();
                            // PIR 9051 - Change Message Description
                            string lstrScript = "alert('Your PERSLink Member Id has been sent to your e-mail address that is on file with NDPERS.'); window.close();";//pir 8631
                            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Open Message Box", lstrScript, true);
                        }
                    }
                    else
                    {
                        lblError.Text = "Sorry! No matching record exists with the given credentials";
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text = "Sorry mail sending failed. Please contact NDPERS Office";
                    ExceptionManager.Publish(ex);
                }
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
        }
            
    }
}