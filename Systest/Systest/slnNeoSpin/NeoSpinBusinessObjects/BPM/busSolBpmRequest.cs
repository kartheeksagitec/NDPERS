#region Using directives

using NeoBase.BPM;
using NeoBase.Common;
using NeoBase.Common.DataObjects;
using NeoSpin.Common;
using NeoSpin.DataObjects;
using Sagitec.Bpm;
using Sagitec.Common;
using Sagitec.DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busBpmRequest:
    /// Inherited from busBpmRequestGen, the class is used to customize the business object busBpmRequestGen.
    /// </summary>
    [Serializable]

    public class busSolBpmRequest : busNeobaseBpmRequest //busBpmRequest
    {
        /// Developer - Rahul Mane
        /// Date - 09-24-2021
        /// Iteration - Main-Iteration10 
        /// Comment - This method call from busNeobaseBpmRequest 
        /// <summary>
        /// Calls  the IncomingDocumentRecevied method to notify communication module for the incoming document.

        public override void IncomingDocumentReceived(busBpmRequest abusBpmRequest)
        {
            //if (abusBpmRequest.icdoBpmRequest.tracking_id > 0 && abusBpmRequest.icdoBpmRequest.reason_value.IsNotNullOrEmpty())
            //{
            //    new busCommunication().IncomingDocumentReceived(abusBpmRequest.icdoBpmRequest.tracking_id, abusBpmRequest.icdoBpmRequest.reason_value);
            //}
        }
		
        public string istrOrgCode { get; set; }
    }
}
