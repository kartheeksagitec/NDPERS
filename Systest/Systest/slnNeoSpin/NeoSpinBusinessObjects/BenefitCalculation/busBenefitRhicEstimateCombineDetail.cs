#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitRhicEstimateCombineDetail : busBenefitRhicEstimateCombineDetailGen
    {
        //We need this property to show Person Full Name in Estimated Grid for both Person Account and Payee Account Records.
        //Payee account does not have person account object
        public busPerson ibusPerson { get; set; }
        public busPlan ibusPlan { get; set; }

    }
}
