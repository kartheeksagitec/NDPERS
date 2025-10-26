using System;
using System.Web;
using Sagitec.WebClient;

using System.Collections;
using Sagitec.ExceptionPub;
using Sagitec.Common;
using Sagitec.Interface;

public partial class LoginPages_wfmTermsAndConditions : wfmClientBasePage
{
    #region [Properties]
    /// <summary>
    /// Holds User Info from Session.
    /// </summary>
    utlUserInfo iutlNeoSpinUserInfo;

    #endregion

    #region [Overriden Methods]
    /// <summary>
    /// Initialize control values & set common properties of base class wfmMainDB before page is going to Load.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnInit(EventArgs e)
    {
        try
        {
            //Pushawart: Restricted response header from passing unwanted information to browser.
            Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
            Response.Cache.SetNoStore();
            Response.AppendHeader("Cache-Control", "no-cache, no-store, private, must-revalidate"); // HTTP 1.1.
            Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            Response.AppendHeader("Expires", "0"); // Proxies.

            ilblMessage = this.lblMessage;
            ilblMessageId = this.lblMessageID;
            ivlsErrors = this.vlsErrors;
            base.OnInit(e);
        }
        catch (Exception ex)
        {
            iblnErrorOccured = true;
            ExceptionManager.Publish(new Exception("The detailed error is: " + ex.Message));
            return;
        }
    }
    #endregion

    #region [Protected Methods]

    /// <summary>
    /// Call when button Continue is clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnContinue_Click(object sender, EventArgs e)
    {
        //Implemented for BR-802-14.
        if (chkAgree.Checked == false)
        {
             DisplayError(utlMessageType.Solution,4020, null);
            return;
        }
        iutlNeoSpinUserInfo = (utlUserInfo)Framework.SessionForWindow["UserInfoObject"];
        bool lblnResult = UpdateUserAcceptanceDate(iutlNeoSpinUserInfo.istrUserId);

        if (lblnResult)
        {
            Framework.SessionAdd("IAgree", "Y");
            Framework.SessionForWindow["RequestFromTermsAndConditionsPage"] = true;
            Framework.Redirect("wfmChangePassword.aspx");
        }
        else
        {
            iutlNeoSpinUserInfo.istrMessage = "Unexpected error occured during process, please contact AGENCY for more information.";
             DisplayError(iutlNeoSpinUserInfo.istrMessage, null);
        }
    }

    /// <summary>
    /// Call when Cancel button clicked And redirect User to Login page.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override void btnCancel_Click(object sender, EventArgs e)
    {
        base.btnCancel_Click(sender, e);

        //Implemented for BR-802-13.
        Framework.SessionAdd("IAgree", "N");
        Framework.Redirect("~/wfmLoginE.aspx");
    }

    #endregion

    #region [Private Methods]
    /// <summary>
    /// Call Business Tier method.
    /// Update User Acceptance Date to DateTime.Now in Org Contact Table when user accepet Terms and Conditions during registration process.
    /// </summary>
    /// <param name="astrEntrustUserID"></param>
    /// <returns></returns>
    private bool UpdateUserAcceptanceDate(string astrEntrustUserID)
    {
        IBusinessTier lsrvBusinessTier = GetNeoSpinESSBusinessTier();
        try
        {
            Hashtable lhstParameter = new Hashtable();
            lhstParameter.Add("astrEntrustUserID", astrEntrustUserID);
            return (bool)lsrvBusinessTier.ExecuteMethod("UpdateUserAcceptanceDate", lhstParameter, false, this.idictParams);
        }
        finally
        {
            HelperFunction.CloseChannel(lsrvBusinessTier);
        }

    }

    #endregion
}
