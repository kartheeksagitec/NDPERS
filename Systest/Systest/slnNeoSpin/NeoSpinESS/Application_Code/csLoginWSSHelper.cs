using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sagitec.Common;
using Sagitec.WebClient;
using NeoSpin.BusinessObjects;
using System.Collections;
using System.Data;
using Sagitec.Interface;
using System.Configuration;
//using gov.nd.appstest.secure;
using Sagitec.BusinessObjects;
using Sagitec.WebControls;
using NeoSpin.CustomDataObjects;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using Sagitec.MVVMClient;
using NDLogin.gov.nd.intranetappstest.secure;

/// <summary>
/// Summary description for csLoginWSSHelper
/// </summary>
public static class csLoginWSSHelper
{
    // PIR 11920
    public static T Clone<T>(this T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }

    public static utlUserInfo SetSessionVariables(string astrNDPERSLoginId, string astrLastName, string astrFirstName,
        string astrUserType, string astrUserID, int astrUserSerialId, string astrColorScheme, string astrEmailID, MVVMSession aobjSessionData, bool ablnIsPrimaryAuthAgent = false)
    {
        utlUserInfo lobjUserInfo = new utlUserInfo();
        lobjUserInfo.istrUserId = astrUserID;
        lobjUserInfo.iintUserSerialId = astrUserSerialId;
        lobjUserInfo.istrUserType = astrUserType;
        lobjUserInfo.istrLastName = astrLastName;
        lobjUserInfo.istrFirstName = astrFirstName;
        lobjUserInfo.istrEmailId = astrEmailID;
        lobjUserInfo.istrColorScheme = astrColorScheme;

        aobjSessionData["UserInfoObject"] = lobjUserInfo;
        aobjSessionData["UserID"] = lobjUserInfo.istrUserId;
        aobjSessionData["UserSerialID"] = lobjUserInfo.iintUserSerialId;
        aobjSessionData["UserName"] = lobjUserInfo.istrFirstName + " " + lobjUserInfo.istrLastName; //ESS Redesign PIR- 13789 changed user name format.
        aobjSessionData["ColorScheme"] = lobjUserInfo.istrColorScheme;
        aobjSessionData["UserType"] = lobjUserInfo.istrUserType;
        aobjSessionData["NDPERSLoginID"] = astrNDPERSLoginId;
        aobjSessionData["IsPAG"] = ablnIsPrimaryAuthAgent;
        return lobjUserInfo;
    }

    public static void SetUserSecurityForContact(int aintOrgID, int aintContactID, IBusinessTier asrvBusinessTier, MVVMSession aobjSessionData)
    {
        Hashtable lhstParam = new Hashtable();
        lhstParam = new Hashtable();
        lhstParam.Add("aintOrgID", aintOrgID);
        lhstParam.Add("aintContactID", aintContactID);
        DataTable ldtSecurity = (DataTable)asrvBusinessTier.ExecuteMethod("GetSecurityForContact", lhstParam, false, new Dictionary<string, object>());
        aobjSessionData["UserSecurity"] = ldtSecurity;
    }
    //For MSS Layout Change

    public static string GetLaunchURLforEmployerPortal(int aintOrgID, int aintContactID, IBusinessTier asrvBusinessTier, MVVMSession aobjSessionData)
    {
        Hashtable lhstOrgID = new Hashtable(); // PIR 9773
        lhstOrgID.Add("aintOrgId", Convert.ToInt32(aintOrgID));
        Dictionary<string, object> ldctParams = new Dictionary<string, object>();
        aobjSessionData["MSSAccessValue"] = Convert.ToString(asrvBusinessTier.ExecuteMethod("GetESSAccessValue", lhstOrgID, false, ldctParams));

        ArrayList larrSelectedRows = new ArrayList();
        Hashtable lhstSelectedRows = new Hashtable();
        //set Empty for Trace log functionality issue as null throws Exception on LoadComplete.
        string lstrProfileEmail = string.Empty;
        bool lblnIsExternalUser = false;
        if (aobjSessionData["NDPERSEmailID"] != null)
        {
            lstrProfileEmail = aobjSessionData["NDPERSEmailID"].ToString();
            lblnIsExternalUser = true;
        }
        lhstSelectedRows.Add("aint_org_id", aintOrgID);
        lhstSelectedRows.Add("aint_contact_id", aintContactID);
        lhstSelectedRows.Add("astr_profile_email_id", lstrProfileEmail);
        lhstSelectedRows.Add("abln_is_external_user", lblnIsExternalUser);
        larrSelectedRows.Add(lhstSelectedRows);
        return "wfmESSActiveMemberHomeMaintenance";
    }

    public static string SetSessionAndLaunchEmployerPortalHome(int aintOrgID, int aintContactID, IBusinessTier asrvBusinessTier, MVVMSession aobjSessionData)
    {
        Hashtable lhstParam = new Hashtable();
        lhstParam.Add("aintContactID", aintContactID);

        Dictionary<string, object> ldctParams = new Dictionary<string, object>();
        busContact lobjContact = (busContact)asrvBusinessTier.ExecuteMethod("LoadContact", lhstParam, false, ldctParams);

        lhstParam = new Hashtable();
        lhstParam.Add("aintOrgID", aintOrgID);
        busOrganization lobjOrganization = (busOrganization)asrvBusinessTier.ExecuteMethod("LoadOrganization", lhstParam, false, ldctParams);
        //Central Payroll and Universities should only have the ability to lookup remittances
        if (lobjOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState &&
            lobjOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueHigherEd)
        {
            aobjSessionData["EmpType"] = "University";
        }
        else
        {
            aobjSessionData["EmpType"] = null;
        }
        if (lobjOrganization.icdoOrganization.central_payroll_flag == busConstant.Flag_Yes)
        {
            aobjSessionData["CentralPayroll"] = "Yes";
        }
        else
        {
            aobjSessionData["CentralPayroll"] = null;
        }
        bool lblnIsPrimaryAuthAgent = false;
        lhstParam = new Hashtable();
        lhstParam.Add("aintOrgID", aintOrgID);
        lhstParam.Add("aintContactID", aintContactID);
        lblnIsPrimaryAuthAgent = (bool)asrvBusinessTier.ExecuteMethod("IsPrimaryAuthAgent", lhstParam, false, ldctParams);

        int lintUserSerialID = 0;
        if (aobjSessionData["UserSerialID"] != null)
            lintUserSerialID = Convert.ToInt32(aobjSessionData["UserSerialID"]);


        ldctParams[utlConstants.istrConstUserSerialID] = lintUserSerialID;

        string lstrUserID = string.Empty;
        //Setting the Audit Trail
        if (aobjSessionData["UserType"] != null)
        {
            if (aobjSessionData["UserType"].ToString() == busConstant.UserTypeInternal)
            {
                lstrUserID = (string)aobjSessionData["UserID"] ?? string.Empty;

                ldctParams[utlConstants.istrConstUserID] = lstrUserID;
                ldctParams[utlConstants.istrConstFormName] = "wfmLoginEI.aspx";
                asrvBusinessTier.StoreProcessLog(lstrUserID + " - Internal User successfully logged in to WSS Employer Portal", ldctParams);
            }
            else
            {
                lstrUserID = (string)lobjOrganization.icdoOrganization.org_code + "_" + aintContactID ?? string.Empty;
                ldctParams[utlConstants.istrConstUserID] = lstrUserID;
                ldctParams[utlConstants.istrConstFormName] = "wfmLoginEE.aspx";
                asrvBusinessTier.StoreProcessLog(lstrUserID + " - External User successfully logged in to WSS Employer Portal", ldctParams);
            }
        }

        csLoginWSSHelper.SetSessionVariables(lobjContact.icdoContact.ndpers_login_id,
                                                            lobjContact.icdoContact.last_name,
                                                            lobjContact.icdoContact.first_name,
                                                            (string)aobjSessionData["UserType"] ?? string.Empty,
                                                            lstrUserID,
                                                            lintUserSerialID,
                                                            (string)aobjSessionData["ColorScheme"] ?? string.Empty,
                                                            lobjContact.icdoContact.email_address, aobjSessionData, lblnIsPrimaryAuthAgent);
        //Setting the User Security for Contact
        csLoginWSSHelper.SetUserSecurityForContact(aintOrgID, aintContactID, asrvBusinessTier, aobjSessionData);

        //Launching the Portal
        return csLoginWSSHelper.GetLaunchURLforEmployerPortal(aintOrgID, aintContactID, asrvBusinessTier, aobjSessionData);
    }

    public static void AddUserToSecureWayGroup(IBusinessTier asrvBusinessTier, MVVMSession aobjSessionData)
    {
        //Adding User into SecureWay Group
        Hashtable lhstParams = new Hashtable();
        lhstParams.Add("astrKey", null);
        lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayGroup"]);
        string lstrSecureWayGroup = (string)asrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, new Dictionary<string, object>());

        lhstParams = new Hashtable();
        lhstParams.Add("astrKey", null);
        lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayUser"]);
        string lstrSecureWayUser = (string)asrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, new Dictionary<string, object>());

        lhstParams = new Hashtable();
        lhstParams.Add("astrKey", null);
        lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayPassword"]);
        string lstrSecureWayPassword = (string)asrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, new Dictionary<string, object>());

        LdapService ldapService = new LdapService();
        ldapService.Url = ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["ServerEnvironment"].ToString() + "NDLdapWS"];
        ldapService.addUserToGroup(aobjSessionData["NDPERSLoginID"].ToString(), lstrSecureWayGroup, lstrSecureWayUser, lstrSecureWayPassword);
    }

    public static string GetSecureWayGroupName(IBusinessTier asrvBusinessTier)
    {
        Hashtable lhstParams = new Hashtable();
        lhstParams.Add("astrKey", null);
        lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayGroup"]);
        return (string)asrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, new Dictionary<string, object>());
    }
}
