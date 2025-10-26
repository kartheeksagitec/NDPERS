#region Using directives

using System;
using NeoBase.Common;
using Sagitec.Bpm;
using System.Collections.Generic;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busSolBpmProcessEscalationInstance:
    /// Inherited from busBpmProcessEscalationInstance, the class is used to customize the business object busBpmProcessEscalationInstance.
    /// </summary>
    //[Serializable]
    //public class busNeobaseBpmProcessEscalationInstance : busBpmProcessEscalationInstance, ICommunicationRecipientProvider
    //{
    //    /// <summary>
    //    /// This function is used to get the Escalation Communication list.
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
    //            llstEscalationCommunicationList.Add(new CorrespondenceBase().LoadInternalUserRecipient(userSerialId));
    //        }
    //        ablnImplementedForTemplate = true;
    //        return llstEscalationCommunicationList;
    //    }


    //}
}
