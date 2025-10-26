using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;

namespace Neo.Controllers
{
    public class UiHelperFunction
    {
        public string InitializeScreenSessionsAndGetLaunchURL()
        {
            Dictionary<string, object> ldictParams = (Dictionary<string, object>)HttpContext.Current.Session["dictParams"];
  
            ldictParams["PersonID"] = HttpContext.Current.Session["PersonID"];
            
            return null;
        }

       
        /// <summary>
        /// Get Message
        /// </summary>
        /// <precondition>Message id should be greater than 0</precondition>
        /// <postcondition>Returns display message for Message id.</postcondition>
        /// <sideeffect></sideeffect>
        /// <param name="aintMessageID">Message ID</param>
        /// <param name="aarrParam">Param</param>
        /// <returns>utlResponseMessage</returns>
        // Modified Method For Framework Version 6.0.0.36
        public static utlResponseMessage GetMessage(int aintMessageID, object[] aarrParam = null)
        {
            IDBCache lsrvDBCache = GetDBCacheURL();
            try
            {
                utlMessageInfo lobjutlMessageInfo = lsrvDBCache.GetMessageInfo(aintMessageID);
                if (lobjutlMessageInfo == null)
                {
                    return null;
                }

                utlResponseMessage lobjResponseMessage = new utlResponseMessage();
                lobjResponseMessage.istrMessageID = "Msg ID : " + Convert.ToString(aintMessageID);
                if (aarrParam == null)
                {
                    lobjResponseMessage.istrMessage = " [ " + lobjutlMessageInfo.display_message + " ]";
                }
                else
                {
                    lobjResponseMessage.istrMessage = " [ " + string.Format(lobjutlMessageInfo.display_message + " ]", aarrParam);
                }

                return lobjResponseMessage;
            }
            finally
            {
                HelperFunction.CloseChannel(lsrvDBCache);
            }
        }

        /// <summary>
        /// THis method will create DB Cache Url
        /// </summary>
        /// <precondition>.</precondition>
        /// <postcondition>Creates DB Cache Url</postcondition>
        /// <sideeffect></sideeffect>
        /// <param name="astrSrvName">Srv Name</param>
        /// <returns>IDBCache</returns>
        public static IDBCache GetDBCacheURL(string astrSrvName = "srvDBCache")
        {
            return WCFClient<IDBCache>.CreateChannel(string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], astrSrvName));
        }
        
    }
}