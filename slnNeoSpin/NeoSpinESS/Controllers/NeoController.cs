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
using System.Xml.Linq;

namespace Neo.Controllers
{
    // [Authorize]
    public partial class NeoController : ApiControllerBase
    {
        //FMUpgrade: Changes for method btnOpen_Click(wfmESSEmployeeLookup) in Default.aspx.cs
        //[HttpPost]
        //public object GetEmploymentChangeRequestID_Controller(Dictionary<string, int> Parameters)
        //{
        //    try
        //    {
        //        isrvServers.ConnectToBT(istrSenderForm);

        //        int aintPlanID = 0;
        //        int aintemploymentId = 0;
        //        int aintemploymentdetailid = 0;
        //        if (!Parameters.IsNullOrEmpty())
        //            aintPlanID = Convert.ToInt32(Parameters["aintplanid"]);
        //            aintemploymentId = Convert.ToInt32(Parameters["aintemploymentId"]);
        //            aintemploymentdetailid = Convert.ToInt32(Parameters["aintemploymentdetailid"]);

        //        Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
        //        Hashtable lhstParam = new Hashtable();
        //        lhstParam.Add("aintplanid", Convert.ToInt32(aintPlanID));
        //        lhstParam.Add("aintemploymentId", Convert.ToInt32(aintemploymentId));
        //        lhstParam.Add("aintemploymentdetailid", Convert.ToInt32(aintemploymentdetailid));

        //        var lobjResult = isrvServers.isrvBusinessTier.ExecuteMethod("GetEmploymentChangeRequestID", lhstParam, false, ldictParams);
        //        return lobjResult.ToString();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        // FMUpgrade : Added for Default page conversion of btn_OpenPDFFromNavigationParam method
        public byte[] CreateRemittanceReport(Dictionary<string, string> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);            
            int aintEmployerPayrollHeaderId = 0;            
            if (!Parameters.IsNullOrEmpty())
                aintEmployerPayrollHeaderId = Convert.ToInt32(Parameters["aintEmployerPayrollHeaderId"]);
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintEmployerPayrollHeaderId", Convert.ToInt32(aintEmployerPayrollHeaderId));                       
            byte[] lobjResult = (byte[])isrvServers.isrvBusinessTier.ExecuteMethod("CreateRemittanceReport", lhstParam, false, ldictParams);
            return lobjResult;
        }

        // FMUpgrade : (PIR 11480) Open PDF in new tab
        public string CreateRemittanceReportPath(Dictionary<string, string> Parameters)
        {
            string lstrPath = string.Empty;
            isrvServers.ConnectToBT(istrSenderForm);
            int aintEmployerPayrollHeaderId = 0;
            if (!Parameters.IsNullOrEmpty())
                aintEmployerPayrollHeaderId = Convert.ToInt32(Parameters["aintEmployerPayrollHeaderId"]);
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            Hashtable lhstParam = new Hashtable();
            lhstParam.Add("aintEmployerPayrollHeaderId", Convert.ToInt32(aintEmployerPayrollHeaderId));
            lstrPath = (string)isrvServers.isrvBusinessTier.ExecuteMethod("CreateRemittanceReportPath", lhstParam, false, ldictParams);
            return lstrPath;
        }
        /// <summary>
        /// Menu Visibility Through Javascript Causing random issues, so visibility handled on the server side. 
        /// F/W Upgrade PIR 11803, 11804 - Menu Gets Collapsed on random navigation, empty list item getting added on random navigation issues fix - MSS
        /// </summary>
        /// <param name="astrFormID"></param>
        /// <param name="axmlSitemapNode"></param>
        /// <returns></returns>
        [HttpPost]
        public override bool AddToMenuAfterCustomRestrictions(string astrFormID, XElement axmlSitemapNode)
        {
            string lstrVisibility = axmlSitemapNode.Attribute("sfwVisibility")?.Value;
            if(!string.IsNullOrEmpty(lstrVisibility))
            {
                string lstrUrl = axmlSitemapNode.Attribute("url")?.Value;
                string lstrIsCentralPayroll = Convert.ToString(HttpContext.Current.Session["CentralPayroll"]);
                if (!string.IsNullOrEmpty(lstrUrl))
                {
                    if (lstrUrl.IndexOf("wfmESSRemittanceLookup") >= 0 && (string.IsNullOrEmpty(lstrIsCentralPayroll) || lstrIsCentralPayroll.ToLower() != "yes"))
                        return false;
                    else if (lstrUrl.IndexOf("wfmESSDepositLookup") >= 0 && !string.IsNullOrEmpty(lstrIsCentralPayroll) && lstrIsCentralPayroll.ToLower() == "yes")
                        return false;
                }
            }
            return true;
        }

        //F/W upgade PIR 12004
        [HttpPost]
        public int GetESSUnreadMessagesCountNeo(Dictionary<string, int> Parameters)
        {
            isrvServers.ConnectToBT(istrSenderForm);
            int aintContactID = 0;
            int aintOrgID = 0;
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)iobjSessionData["dictParams"];
            IBusinessTier lsrvBusinessTier = null;
            try
            {
                if (ldictParams.ContainsKey("ContactID") && ldictParams.ContainsKey("OrgID"))
                {
                    aintContactID = Convert.ToInt32(ldictParams["ContactID"]);
                    aintOrgID = Convert.ToInt32(iobjSessionData["OrgID"]);
                    Hashtable lhstParam = new Hashtable();
                    lhstParam.Add("aintContactID", aintContactID);
                    lhstParam.Add("aintOrgID", aintOrgID);
                    string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvNeoSpinWSS");
                    lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                    int lobjResult = (int)lsrvBusinessTier.ExecuteMethod("GetESSUnreadMessagesCount", lhstParam, false, ldictParams);
                    iobjSessionData["MessageCount"] = lobjResult;
                    return lobjResult;
                }
                return 0;
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvBusinessTier);
            }
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
    }
}