using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Sagitec.Common;
using Sagitec.Interface;
using System.Web.Script.Serialization;
using Sagitec.MVVMClient;
using Newtonsoft.Json;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Diagnostics;
using NeoSpinConstants;
using System.Xml.Linq;
using System.Xml;

namespace Neo.Controllers
{
    // [Authorize]
    public partial class NeoController : ApiControllerBase
    {
        
        /// <summary>
        /// PIR 16321 : Added Menu and SubMenu according to Membership account and Benefit Recipient.(DS1302-ACT0001-UI0001-BR0012)
        /// </summary>
        /// <param name="astrFormID">Form ID</param>
        /// <param name="axmlSitemapNode">Sitemap Node</param>
        /// <returns>Bool</returns>
        [HttpPost]
        public override bool AddToMenuAfterCustomRestrictions(string astrFormID, XElement axmlSitemapNode)
        {
            Dictionary<string, object> ldictParams = iobjSessionData["dictParams"] as Dictionary<string, object>;
            isrvServers.ConnectToBT(istrSenderForm);
            if (Convert.ToString(iobjSessionData["Landing_Page"]) == "wfmMSSHomeLimited")
            {
                return false;
            }
            if (HttpContext.Current.Session["Member"].IsNotNull())
            {
                if (axmlSitemapNode.Attribute("title").Value == "Retirement Distribution/Deferral" ||
                    axmlSitemapNode.Attribute("title").Value == "Upload Documents")
                {
                    DataTable ldtbAppWizard = isrvServers.isrvDbCache.GetCacheData("SGS_CODE_VALUE", "CODE_ID=52 and CODE_VALUE = 'APWI'");
                    if (ldtbAppWizard.IsNotNull() && ldtbAppWizard.Rows.Count > 0)
                    {
                        string lstrIsVisibleAppWizard = ldtbAppWizard.Rows[0]["DATA1"] != DBNull.Value ? 
                                                        Convert.ToString(ldtbAppWizard.Rows[0]["DATA1"]) : string.Empty;
                        if (!string.IsNullOrEmpty(lstrIsVisibleAppWizard) && lstrIsVisibleAppWizard.ToLower() != "y")
                            return false;
                    }
                }
                if (axmlSitemapNode.Attribute("title").Value == "Annual Enrollment")
                {
                    DataTable ldtbANNE = isrvServers.isrvDbCache.GetCacheData("SGS_CODE_VALUE", "CODE_ID=52 and CODE_VALUE = 'ANNE'");
                    if (ldtbANNE.IsNotNull() && ldtbANNE.Rows.Count > 0)
                    {
                        DateTime lstrANNEBeginDate = Convert.ToDateTime(ldtbANNE.Rows[0]["DATA1"]);
                        DateTime lstrANNEEndDate = Convert.ToDateTime(ldtbANNE.Rows[0]["DATA2"]);
                        if (lstrANNEBeginDate != DateTime.MinValue && lstrANNEEndDate != DateTime.MinValue && !(DateTime.Today>=lstrANNEBeginDate && DateTime.Today<=lstrANNEEndDate))
                            return false;
                    }
                }
                string lstrVisibility = axmlSitemapNode.Attribute("sfwVisibility").Value;
                string lstrMemberType = HttpContext.Current.Session["Member"].ToString();
                if (lstrVisibility != lstrMemberType)
                    return false;
            }
            return true;
        }

        [HttpPost]
        public bool IsEmailAddressNotWaived(Dictionary<string, string> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            int aintPayeeAccountId = 0;
            if (!Parameters.IsNullOrEmpty())
                aintPayeeAccountId = Convert.ToInt32(Parameters["aintPayeeAccountId"]);
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("Aintpayeeaccountid", Convert.ToInt32(aintPayeeAccountId));
            bool lobjResult = (bool)isrvServers.isrvBusinessTier.ExecuteMethod("IsEmailAddressNotWaived", lhstParam, false, ldictParams);
            return lobjResult;
        }

        //F.W Upgrade  6.0 
        [HttpPost]
        public bool IsGeneratedOTPExpired(Dictionary<string, string> Parameters) 
        {
            isrvServers.ConnectToBT(istrSenderForm);
            int aintPersonID = 0;
            if (!Parameters.IsNullOrEmpty())
                aintPersonID = Convert.ToInt32(Parameters["AintPersonID"]);
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("AintPersonID", Convert.ToInt32(aintPersonID));
            bool lobjResult = (bool)isrvServers.isrvBusinessTier.ExecuteMethod("IsGeneratedOTPExpired", lhstParam, false, ldictParams);
            return lobjResult;
        }

        [HttpPost]
        public int GetMSSUnreadMessagesCountNeo(Dictionary<string, int> Parameters)
        {
            //isrvServers.ConnectToBT(istrSenderForm);
            int aintPersonID = 0;
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            aintPersonID = Convert.ToInt32(ldictParams["PersonID"]);
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintPersonID", Convert.ToInt32(aintPersonID));
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            IBusinessTier lsrvBusinessTier = null;
            int lobjResult = 0;
            try
            {
                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                lobjResult = (int)lsrvBusinessTier.ExecuteMethod("GetMSSUnreadMessagesCount", lhstParam, false, ldictParams);
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }            
           iobjSessionData["MessageCount"] = lobjResult;
            return lobjResult;

        }
		
		//FW Upgrade - Sub menu related code conversion
        [HttpPost]
        public void SetPayeeAccountId(Dictionary<string, int> Parameters)
        {
            int aintPayeeAccountId = 0;
            aintPayeeAccountId = Convert.ToInt32(Parameters["aintPayeeAccountId"]);
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            ldictParams["PayeeAccountIdMenu"] = aintPayeeAccountId;
            iobjSessionData["dictParams"] = ldictParams;
        }

        [HttpPost]
        public string checkSinglePlan()
        {
            isrvServers.ConnectToBT(istrSenderForm);
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            IBusinessTier lsrvBusinessTier = null;
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParam = new Hashtable();
            string lobjResult = string.Empty;
            try
            {
                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                lobjResult = (string)lsrvBusinessTier.ExecuteMethod("checkSinglePlan", lhstParam, false, ldictParams);
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
            return lobjResult;
        }

        [HttpPost]
        public object GetBenOptionsByEffectiveDate1(Dictionary<string, string> Parameters)
        {
            string lstrBenType = string.Empty;
            string lstrRetDate = string.Empty;
            int lintBenProvisionId=0;
            string lstrMaritalStatusValue = string.Empty;
            isrvServers.ConnectToBT(istrSenderForm);
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            IBusinessTier lsrvBusinessTier = null;
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            lstrBenType =Parameters["astrBenType"];
            lstrRetDate = Parameters["astrRetDate"];
            lintBenProvisionId = Convert.ToInt32(Parameters["aintBenProvisionId"]);
            lstrMaritalStatusValue = Parameters["astrMaritalStatusValue"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("astrBenType", lstrBenType);
            lhstParam.Add("astrRetDate", lstrRetDate);
            lhstParam.Add("aintBenProvisionId",lintBenProvisionId);
            lhstParam.Add("astrMaritalStatusValue", lstrMaritalStatusValue);
            lhstParam.Add("ablnReturnCollection", false);
            object lobjResult = null;
            try
            {
                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                lobjResult = lsrvBusinessTier.ExecuteMethod("GetBenOptionsByEffectiveDate", lhstParam, false, ldictParams);
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
            return lobjResult;
        }


        [HttpPost]
        public ArrayList btnVertifyOTP_Click(Dictionary<string, string> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            IBusinessTier lsrvBusinessTier = null;
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            string astrActivationCode = Parameters["AstrActivationCode"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("AstrActivationCode", astrActivationCode);
            ArrayList lobjResult = null;
            try
            {
                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                lobjResult = (ArrayList)lsrvBusinessTier.ExecuteMethod("btnVertifyOTP_Click", lhstParam, false, ldictParams);
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
            return lobjResult;
        }
 
        [HttpPost]
        public bool IsFromImageClick()
        {
            try
            {
                iobjSessionData["IsFromImageClick"] = true;
                iobjSessionData["RedirectOnLogoff"] = "";
                return true;
            }
            finally
            {
            }
        }
        [HttpPost]
        public object GetBenefitAccountTypeByPlan(Dictionary<string, string> Parameters)
        {
            int lintPlanId = 0;
            isrvServers.ConnectToBT(istrSenderForm);
            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
            IBusinessTier lsrvBusinessTier = null;
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            lintPlanId = Convert.ToInt32(Parameters["aintPlanId"]);
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintPlanId", lintPlanId);
            object lobjResult = null;
            try
            {
                lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                lobjResult = lsrvBusinessTier.ExecuteMethod("LoadBenefitAccountTypeByPlan", lhstParam, false, ldictParams);
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
            return lobjResult;
        }
        [HttpPost]
        public bool IsBankInfoAlreadyExists(Dictionary<string, string> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            string lstrRoutingNumber = string.Empty;
            string lstrAccountNumber = string.Empty;
            string lstrAccountType = string.Empty;
            string lstrPersonAccountIds = string.Empty;
            if (!Parameters.IsNullOrEmpty())
            {
                lstrRoutingNumber = Parameters["astrRoutingNumber"];
                lstrAccountNumber = Parameters["astrAccountNumber"];
                lstrAccountType = Parameters["astrAccountType"];
                lstrPersonAccountIds = Parameters["astrPersonAccountIds"];
            }
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("astrRoutingNumber", lstrRoutingNumber);
            lhstParam.Add("astrAccountNumber", lstrAccountNumber);
            lhstParam.Add("astrAccountType", lstrAccountType);
            lhstParam.Add("astrPersonAccountIds", lstrPersonAccountIds);
            bool lobjResult = (bool)isrvServers.isrvBusinessTier.ExecuteMethod("IsBankInfoAlreadyExists", lhstParam, false, ldictParams);
            return lobjResult;
        }
    }
}