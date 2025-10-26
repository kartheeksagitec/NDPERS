using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busConsolidatedPurchaseWeb : busExtendBase
    {
        public busConsolidatedPurchaseWeb()
        {
            icdoWssConsolidatedPurchaseWeb = new cdoWssBenefitCalculator();
        }

        public DateTime idtFromDate { get; set; }
        public DateTime idtToDate { get; set; }
        public string istrConsolidatePurchaseType { get; set; }
        public string istrConsolidatePurchaseTypeValue { get; set; }
        //consolidate additional service credits
        public int iintAdditionalServiceCredits { get; set; }
        public cdoWssBenefitCalculator icdoWssConsolidatedPurchaseWeb { get; set; }

        public bool IsValidEntry()
        {
            if (istrConsolidatePurchaseTypeValue == busConstant.Service_Purchase_Type_Additional_Service_Credit)
            {
                if (iintAdditionalServiceCredits > 0)
                {
                    return true;
                }
            }
            else
            {
                if ((idtFromDate != DateTime.MinValue) && (idtToDate != DateTime.MinValue))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
