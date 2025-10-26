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
    public class busServicePurchaseAmortizationScheduleWeb : busExtendBase
    {
        public busServicePurchaseAmortizationScheduleWeb()
        {
            if (icdoServicePurchaseHeader.IsNull())
                icdoServicePurchaseHeader = new cdoServicePurchaseHeader();
        }

        public int iintBenefitCalculatorId { get; set; }
        public string istrUnusedSickLeavePurchase { get; set; }
        public string istrConsolidatedPurchase { get; set; }

        public cdoServicePurchaseHeader icdoServicePurchaseHeader { get; set; }
        public busServicePurchaseHeader ibusSickLeavePurchaseHeader { get; set; }
        public busServicePurchaseHeader ibusConsolidatedPurchaseHeader { get; set; }
        public busServicePurchaseHeader ibusServicePurchaseHeader { get; set; }

        public void LoadSickLeavePurchaseHeader(int aintServicePurchaseID)
        {
            if (ibusSickLeavePurchaseHeader.IsNull())
                ibusSickLeavePurchaseHeader = new busServicePurchaseHeader { icdoServicePurchaseHeader = new cdoServicePurchaseHeader() };
            ibusSickLeavePurchaseHeader.FindServicePurchaseHeader(aintServicePurchaseID);
        }

        public void LoadConsolidatedPurchaseHeader(int aintServicePurchaseID)
        {
            if (ibusConsolidatedPurchaseHeader.IsNull())
                ibusConsolidatedPurchaseHeader = new busServicePurchaseHeader { icdoServicePurchaseHeader = new cdoServicePurchaseHeader() };
            ibusConsolidatedPurchaseHeader.FindServicePurchaseHeader(aintServicePurchaseID);
        }

        public void LoadPurchaseHeader(int aintServicePurchaseID)
        {
            if (ibusServicePurchaseHeader.IsNull())
                ibusServicePurchaseHeader = new busServicePurchaseHeader { icdoServicePurchaseHeader = new cdoServicePurchaseHeader() };
            ibusServicePurchaseHeader.FindServicePurchaseHeader(aintServicePurchaseID);
        }

        public void GenerateWhatifSchedule()
        {
            if (ibusServicePurchaseHeader.IsNotNull())
            {
                if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment)
                {
                    ibusServicePurchaseHeader.iclbServicePurchaseFutureSchedule =
                        busServicePurchaseAmortizationSchedule.CalculateWhatIfAmortizationSchedule(ibusServicePurchaseHeader, iobjPassInfo);
                }
            }
        }

        public busWssBenefitCalculator ibusBenefitCalculator { get; set; }
        public void LoadBenefitCalculator()
        {
            if (ibusBenefitCalculator.IsNull())
                ibusBenefitCalculator = new busWssBenefitCalculator();

            ibusBenefitCalculator.FindWssBenefitCalculator(iintBenefitCalculatorId);
        }

        //FW6 Upgrade:: PIR-11765 Service Purchase Amortization Schedule Visiblity For label text for lblLesseramount
        public bool IsServicePurchaseLbl()
        {

            if (ibusConsolidatedPurchaseHeader.IsNotNull())
            { 
                if (ibusConsolidatedPurchaseHeader.iclbServicePurchaseAmortizationSchedule.IsNotNull())
                {
                    if (ibusConsolidatedPurchaseHeader.iclbServicePurchaseAmortizationSchedule.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
