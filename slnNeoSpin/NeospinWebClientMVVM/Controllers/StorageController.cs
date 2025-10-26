#region [Using directives]
using Newtonsoft.Json;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
#endregion [Using directives]

namespace Neo.Controllers
{
    /// <summary>
    /// Class Neo.StorageController
    /// </summary>
    public class StorageController : StorageControllerBase
    {
        #region [Overriden Methods]
        /// <summary>
        /// Overriding  method to save 'save for later' flag when user clicks on save button on correspondence editor Tool.
        /// </summary>
        /// <param name="aParams">Object Parameters</param>
        /// <returns>Http Responce message</returns>
        //[HttpPost]
        //public override HttpResponseMessage UpdateCorrespondenceStatus(object aParams)
        //{
        //    Dictionary<string, object> ldictParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(aParams));

        //    if (isrvServers == null)
        //    {
        //        isrvServers = new srvServers();
        //    }

        //    isrvServers.ConnectToBT("wfmCorrespondenceClientMVVM");
        //    int lintCorTrackingId = 0;

        //    if (ldictParams.ContainsKey("FileName"))
        //    {
        //        lintCorTrackingId = Convert.ToInt32(ldictParams["TrackingID"]);
        //    }

        //    Hashtable lhstParams = new Hashtable();
        //    lhstParams.Add("aintCorTrackingID", lintCorTrackingId);
        //    lhstParams.Add("adictParams", ldictParams.ToDictionary(k => k.Key, k => Convert.ToString(k.Value)));
        //    try
        //    {
        //        int lintMessageId = (int)isrvServers.isrvBusinessTier.ExecuteMethod("SaveCommunicationMethod", lhstParams, false, ldictParams);
        //    }
        //    catch (Exception exception)
        //    {
        //        string lstrErrorMesssage = exception.Message;
        //    }
        //    return base.UpdateCorrespondenceStatus(aParams);
        //}
        #endregion
    }
}