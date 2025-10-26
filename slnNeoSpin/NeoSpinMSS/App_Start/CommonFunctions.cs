using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using Sagitec.Common;
using Sagitec.Interface;
using System.Configuration;
using Sagitec.BusinessObjects;
using System.Data;
using Sagitec.WebClient;
using NeoSpin.BusinessObjects;

/// <summary>
/// Summary description for CommonFunctions
/// </summary>
public class CommonFunctions
{
	public CommonFunctions()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public static Dictionary<string, object> GetParams(string astrUserID, int aintUserSerialID, string astrFormName)
    {
        Dictionary<string, object> ldictParams = new Dictionary<string, object>();
        ldictParams[utlConstants.istrConstUserID] = astrUserID;
        ldictParams[utlConstants.istrConstUserSerialID] = aintUserSerialID;
        ldictParams[utlConstants.istrConstFormName] = astrFormName;
        return ldictParams;
    }

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

    public static void CreateAndAddCookie(HttpResponse aobjResponse, string astrCookieKey, 
        string astrCookieValue, DateTime adtCookieexpiryDateTime)
    {
        //Store the username in the cookie for future use
        HttpCookie lHttpCookie = new HttpCookie(astrCookieKey);
        lHttpCookie.Expires = adtCookieexpiryDateTime;
        lHttpCookie.Value = astrCookieValue;
        //Check if a cookie with the same name exists
        if (aobjResponse.Cookies.Get(astrCookieKey) != null)
        {
            aobjResponse.Cookies.Remove(astrCookieKey);
        }
        aobjResponse.Cookies.Add(lHttpCookie);
    }

    public static string GetCookieValue(HttpRequest aobjRequest, string astrCookieKey)
    {
        string lstrCookieValue = string.Empty;
        if (aobjRequest.Cookies.Get(astrCookieKey) != null)
        {
            lstrCookieValue = aobjRequest.Cookies[astrCookieKey].Value;
        }
        return lstrCookieValue;
    }

    public static HttpCookie GetCookie(HttpResponse aobjResponse, string astrCookieKey)
    {
        return aobjResponse.Cookies.Get(astrCookieKey);
    }

  

    public static void RedirectPage(HttpResponse lobjResponse, string astrURL, bool ablnClearSession = false)
    {
        if (ablnClearSession)
        {
            Framework.ClearSessionWindow();
        }

        Framework.Redirect(astrURL);
    }
   
    public static ArrayList GetImages(string astrImagePath)
    {
        string[] larrImages = System.IO.Directory.GetFiles(astrImagePath, "*.jpg");
        Random lobjRandom = new Random();
        long llngGeneratedValue;
        int lintRandomValue;
        ArrayList laryImageList = new ArrayList();
        if (larrImages.Length >= 20)
        {
            while (laryImageList.Count < 20)
            {
                llngGeneratedValue = lobjRandom.Next(1000) + busGlobalFunctions.GetSystemDate().Ticks;
                lintRandomValue = (int)(llngGeneratedValue % larrImages.Length);
                if (!laryImageList.Contains(larrImages[lintRandomValue]))
                {
                    laryImageList.Add(larrImages[lintRandomValue]);
                }
            }
        }
        else
        {
            if (larrImages.Length > 0)
            {
                laryImageList.AddRange(larrImages);    //load all images.
                while (true)
                {
                    if (laryImageList.Count < 20)  //if loaded images are less than 20 then repeat the first image.
                    {
                        laryImageList.Add(larrImages[0]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        return laryImageList;
    }

    /// <summary>
    /// Redirects the page with given parameters in new window and new url.
    /// </summary>
    /// <param name="url"></param>
    public static void Redirect(string url, string astrFeatures)
    {
        HttpContext context = HttpContext.Current;
        Page page = (Page)context.Handler;
        url = page.ResolveClientUrl(url);
        string lstrScript = "fwkOpenPopupWindow('" + url + "','" + astrFeatures + "');";
        ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", lstrScript, true);
    }  

    /// <summary>
    /// Method returns the active object of srvMSS.
    /// </summary>
    /// <returns></returns>
    public static IBusinessTier GetMSSTier()
    {
        string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvMSS");
        IBusinessTier lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
        return lsrvBusinessTier;
    }

    /// <summary>
    /// Get the redirect URL based on user type.
    /// </summary>
    /// <param name="ldtResult"></param>
    /// <param name="aintActiveMemberAccountId"></param>
    /// <returns></returns>
    public static string GetMSSRedirectURL(DataTable ldtResult, int? aintActiveMemberAccountId)
    {   
        string lstrURL = "wfmMSSActiveMemberHomeMaintenance";

        //Member has only active member account
        //then redirect member to active member home screen            
        lstrURL = "wfmMSSActiveMemberHomeMaintenance";
        Framework.SessionForWindow["ActiveMemberAccountID"] = aintActiveMemberAccountId;
        Framework.SessionForWindow["IsActiveMemberAccountSelected"] = true;
        Framework.SessionForWindow["MEMBER_ACCOUNT_ID"] = aintActiveMemberAccountId;
        return lstrURL;
    }

    /// <summary>
    /// Returns formatted AGENCY ID
    /// </summary>
    /// <param name="astrNeoSpinID"></param>
    /// <returns></returns>
    public static string GetFormattedNeoSpinID(string astrNeoSpinID)
    {
        if (!string.IsNullOrEmpty(astrNeoSpinID) && astrNeoSpinID.Length >= 9)
        {
            return string.Format("{0}-{1}-{2}", astrNeoSpinID.Substring(0, 3), astrNeoSpinID.Substring(3, 3), astrNeoSpinID.Substring(6, 3));
        }
        else
        {
            return astrNeoSpinID;
        }
    }

}



public struct UserInfo
{
    public string istrUserID;
    public string istrVNAVCustomerID;
    public string istrFirstName;
    public string istrLastName;
    public bool iblnIsAuthenticated;
    public UserStatus ienmUserStatus;
}

public enum UserStatus
{
    //These status are coming from sgsContact
    NOACCESS = 0,
    REGISTRATION_PENDING,
    ACTIVE,
    LOCKED,
    DISABLED,

    //Following status are only for MyVRS
    SUCCESSFUL,
    USER_DOES_NOT_EXISTS,
    INVALID_USER,
    MY_VRS_TOKEN_EXPIRED,
    INVALID_MY_VRS_TOKEN,
    NO_STATUS,

}
