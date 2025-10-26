using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;
using Sagitec.WebClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NeoSpinMSS
{
    public partial class wfmRedirectPage : wfmMainDB
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
            utlUserInfo lobjUserInfo = (utlUserInfo)iobjSessionData["UserInfoObject"];            
            if (iobjSessionData["NavUrl"] != null)
                HyperLink1.NavigateUrl = iobjSessionData["NavURL"].ToString();
            if (iobjSessionData["OrgUrl"] != null)
                Label7.Text = iobjSessionData["OrgUrl"].ToString();
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}