#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
using Sagitec.Bpm;
using System.Linq;
using System.Collections.Generic;

#endregion

namespace NeoBase.BPM
{
    ///// <summary>
    ///// Class NeoSpin.BusinessObjects.busBpmActivityInstance:
    ///// Inherited from busBpmActivityInstanceGen, the class is used to customize the business object busBpmActivityInstanceGen.
    ///// </summary>
    ///// 
    //[Serializable]
    //public class busNeobaseBpmEscalationInstance : busBpmEscalationInstance, ICommunicationRecipientProvider
    //{
    //    /// <summary>
    //    /// This function is used to get the Correspondence Recipient.
    //    /// </summary>
    //    /// <param name="astrTemplateCode"></param>
    //    /// <param name="ablnImplementedForTemplate"></param>
    //    /// <returns></returns>
    //    public List<clsCommunicationRecipient> GetCorRecipient(string astrTemplateCode, out bool ablnImplementedForTemplate)
    //    {
    //        List<clsCommunicationRecipient> llstEscalationCommunicationList = new List<clsCommunicationRecipient>();

    //        List<int> llstUserSerialIds = GetRecipientUserSerialIds();
    //        foreach (int userSerialId in llstUserSerialIds)
    //        {
    //            //busUser lbusUser = new busUser();
    //            //lbusUser.FindByPrimaryKey(userSerialId);
    //            llstEscalationCommunicationList.Add(ClassMapper.GetObject<CorrespondenceBase>().LoadInternalUserRecipient(userSerialId));
    //        }
    //        ablnImplementedForTemplate = true;
    //        return llstEscalationCommunicationList;
    //    }
    //}
    
}
