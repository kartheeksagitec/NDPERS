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

            ldictParams["ContactID"] = HttpContext.Current.Session["ContactID"];
            ldictParams["OrgID"] = HttpContext.Current.Session["OrgID"];

            return null;
        }
        //Modified Method For Framework Version 6.0.0.36
        public static utlResponseMessage GetMessage(int aintMessageID, object[] aarrParam = null)
        {
            IDBCache lsrvDBCache = GetDBCacheURL();
            try
            {
                utlMessageInfo lobjutlMessageInfo = lsrvDBCache.GetMessageInfo(aintMessageID);
                if (lobjutlMessageInfo == null)
                    return null;
                utlResponseMessage lobjResponseMessage = new utlResponseMessage();
                lobjResponseMessage.istrMessageID = "Msg ID : " + aintMessageID.ToString();
                if (aarrParam == null)
                {
                    lobjResponseMessage.istrMessage = " [ " + lobjutlMessageInfo.display_message + " ]";
                }
                else
                {
                    lobjResponseMessage.istrMessage = " [ " + String.Format(lobjutlMessageInfo.display_message + " ]", aarrParam);
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
        /// <param name="astrSrvName"></param>
        /// <returns></returns>
        public static IDBCache GetDBCacheURL(string astrSrvName = "srvDBCache")
        {
            return WCFClient<IDBCache>.CreateChannel(string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], astrSrvName));
        }


        //public static Collection<utlError> GetErrorMessage(int aintMessageID)
        //{
        //    IDBCache lsrvDBCache = GetDBCacheURL();
        //    try
        //    {
        //        utlMessageInfo lobjutlMessageInfo = lsrvDBCache.GetMessageInfo(aintMessageID);
        //        if (lobjutlMessageInfo == null)
        //            return null;

        //        return new Collection<utlError>() { new utlError { istrErrorMessage = lobjutlMessageInfo.display_message, istrValidationRule = "File Upload", istrErrorID = "ERRO" } };
        //    }
        //    finally
        //    {
        //        HelperFunction.CloseChannel(lsrvDBCache);
        //    }
        //}
    }
}