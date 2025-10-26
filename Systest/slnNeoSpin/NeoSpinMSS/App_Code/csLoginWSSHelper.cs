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

    public static void SetUserSecurityForMember(int aintPersonID, IBusinessTier asrvBusinessTier, MVVMSession aobjSessionData)
    {
        Hashtable lhstParam = new Hashtable();
        lhstParam.Add("aintPersonID", aintPersonID);
        DataTable ldtSecurity = (DataTable)asrvBusinessTier.ExecuteMethod("GetSecurityForMember", lhstParam, false, new Dictionary<string,object>());
        aobjSessionData["UserSecurity"] = ldtSecurity;
    }

    //For MSS Layout Change
    public static string GetLaunchURLforMemberPortal(int aintPersonID, IBusinessTier asrvBusinessTier,MVVMSession aobjSessionData, string astrProfileEmailID = null, bool ablnIsExternalUser = false)
    {
        aobjSessionData["EnrolledInCOBRA"] = false;  //PIR-13473
        ArrayList larrSelectedRows = new ArrayList();
        Hashtable lhstSelectedRows = new Hashtable();
        lhstSelectedRows.Add("person_id", aintPersonID);

        //When we enable full audit log from appsetting, it is throwing error on home page of mss because email id is null. Thats why we need to set it to empty.
        if (astrProfileEmailID.IsNullOrEmpty())
            astrProfileEmailID = string.Empty;

        lhstSelectedRows.Add("profile_email_id", astrProfileEmailID);
        lhstSelectedRows.Add("is_external_user", ablnIsExternalUser);
        larrSelectedRows.Add(lhstSelectedRows);
        Dictionary<string, object> ldctParams = new Dictionary<string, object>();
        Hashtable lhstParam = new Hashtable();
        lhstParam.Add("aintPersonId", Convert.ToInt32(aintPersonID));
        bool iblnIsPersonNotCertified = false;
        Hashtable lhstPersonID = new Hashtable();
        lhstPersonID.Add("aintPersonId", Convert.ToInt32(aintPersonID));
        aobjSessionData["MSSAccessValue"] = Convert.ToString(asrvBusinessTier.ExecuteMethod("GetMSSAccessValue", lhstPersonID, false, ldctParams));
        aobjSessionData["EnrolledInCOBRA"] = (bool)asrvBusinessTier.ExecuteMethod("IsEnrolledInCOBRA", lhstParam, false, ldctParams);
        #region PIR-18492:
        // user has to direct to person profile, PersonCertify is used to control other panels accessibilty.
        //when a member first signs into MSS (first linking the LDAP ID to MSS) the system does display the Personal Profile and member is not able to navigate in MSS,         
        aobjSessionData["IsExternalUser"] = ablnIsExternalUser;
        aobjSessionData["PersonCertify"] = true;
        if (!(bool)asrvBusinessTier.ExecuteMethod("IsPersonVertify", lhstParam, false, ldctParams) && ablnIsExternalUser)
        {
            aobjSessionData["PersonCertify"] = false;
            iblnIsPersonNotCertified = true;
            //return "wfmMSSProfileMaintenance";
        }
        #endregion PIR-18492:
        if ((bool)asrvBusinessTier.ExecuteMethod("IsPersonRetiredOrWithdrawnPlan", lhstParam, false,ldctParams) || 
            (bool)asrvBusinessTier.ExecuteMethod("IsRetiree", lhstParam, false, ldctParams) ||
            (bool)asrvBusinessTier.ExecuteMethod("IsInsurancePlanRetirees", lhstParam, false, ldctParams)||
             Convert.ToBoolean(aobjSessionData["EnrolledInCOBRA"]))    // PROD PIR 8861   //PIR- 13473-If member has Enrolled to COBRA then show retiree Home page 
        {            
            aobjSessionData["IsRetiree"] = true;
            aobjSessionData["IsMemberBothRetireeAndActive"] = false;
            if ((bool)asrvBusinessTier.ExecuteMethod("IsActiveMember", lhstParam, false, ldctParams))
            {
                aobjSessionData["IsMemberBothRetireeAndActive"] = true;
                return "wfmMSSSwitchMemberAccount";
            }
            aobjSessionData["Member"] = "RetireeMember";
            if (iblnIsPersonNotCertified)
                return "wfmMSSProfileMaintenance";
            return "wfmMSSRetireeHomeMaintenance";
        }
        aobjSessionData["IsMemberBothRetireeAndActive"] = false;
        if (Convert.ToString(aobjSessionData["MSSAccessValue"]) == busConstant.OrganizationLimitedAccess ||
            Convert.ToString(aobjSessionData["MSSAccessValue"]) == string.Empty)
        {            
            return "wfmMSSHomeLimited";
        }        
        aobjSessionData["Member"] = "ActiveMember";
        aobjSessionData["IsRetiree"] = false;
        if (iblnIsPersonNotCertified)
            return "wfmMSSProfileMaintenance";
        return "wfmMSSActiveMemberHomeMaintenance";
    }

    public static void AddUserToSecureWayGroup(IBusinessTier asrvBusinessTier,MVVMSession aobjSessionData)
    {
        //Adding User into SecureWay Group
        Hashtable lhstParams = new Hashtable();
        lhstParams.Add("astrKey", null);
        lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayGroup"]);
        string lstrSecureWayGroup = (string)asrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, new Dictionary<string,object>());

        lhstParams = new Hashtable();
        lhstParams.Add("astrKey", null);
        lhstParams.Add("astrValue", ConfigurationManager.AppSettings["SecureWayUser"]);
        string lstrSecureWayUser = (string)asrvBusinessTier.ExecuteMethod("SagitecDecrypt", lhstParams, false, new Dictionary<string,object>());

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
