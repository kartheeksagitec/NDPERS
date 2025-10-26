#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoServicePurchaseAmortizationSchedule : doServicePurchaseAmortizationSchedule
    {
        public cdoServicePurchaseAmortizationSchedule()
            : base()
        {
        }

        private int _iintRemittanceID;
        public int iintRemittanceID
        {
            get { return _iintRemittanceID; }
            set { _iintRemittanceID = value; }
        }

        private int _iintEmployerPayrollDetailID;
        public int iintEmployerPayrollDetailID
        {
            get { return _iintEmployerPayrollDetailID; }
            set { _iintEmployerPayrollDetailID = value; }
        }

        private string _istrPaymentClass;

        public string istrPaymentClass
        {
            get { return _istrPaymentClass; }
            set { _istrPaymentClass = value; }
        }

        private decimal _idecPayOffAmount;

        public decimal idecPayOffAmount
        {
            get { return _idecPayOffAmount; }
            set { _idecPayOffAmount = value; }
        }

        private decimal _idecPayOffAmountActualValue;

        public decimal idecPayOffAmountActualValue
        {
            get { return _idecPayOffAmountActualValue; }
            set { _idecPayOffAmountActualValue = value; }
        }

        private decimal _idecBeginningPrincipalBalance;

        public decimal idecBeginningPrincipalBalance
        {
            get { return _idecBeginningPrincipalBalance; }
            set { _idecBeginningPrincipalBalance = value; }
        }

        private DateTime _idtActualDueDate;

        public DateTime idtActualDueDate
        {
            get { return _idtActualDueDate; }
            set { _idtActualDueDate = value; }
        }

        public string idecPayOffAmountFormatted
        {
            get
            {
                if (_idecPayOffAmount == 0)
                    return String.Empty;
                return _idecPayOffAmount.ToString(".00");
            }
        }

        //this property will be used to navigate to concerned maintenance.
        private string _istrReferenceConst;
        public string istrReferenceConst
        {
            get { return _istrReferenceConst; }
            set { _istrReferenceConst = value; }
        }

        //this property will be used to navigate to concerned maintenance.
        private int _iintReferenceID;
        public int iintReferenceID
        {
            get { return _iintReferenceID; }
            set { _iintReferenceID = value; }
        }

        private int _iintSerPurPaymentAllocationID;
        public int iintSerPurPaymentAllocationID
        {
            get { return _iintSerPurPaymentAllocationID; }
            set { _iintSerPurPaymentAllocationID = value; }
        }

        // This derived property is used to display the prorated vsc in custom formated
        public string prorated_vsc_formatted
        {
            get
            {
                if (prorated_vsc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(prorated_vsc / 12).ToString(),
                                     Math.Round((prorated_vsc % 12), 6, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(prorated_vsc / 12).ToString(),
                                     Math.Round((prorated_vsc % 12), 6, MidpointRounding.AwayFromZero).ToString());
            }
        }

        // This derived property is used to display the prorated psc in custom formated
        public string prorated_psc_formatted
        {
            get
            {
                if (prorated_psc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(prorated_psc / 12).ToString(),
                                    Math.Round((prorated_psc % 12), 6, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(prorated_psc / 12).ToString(),
                                     Math.Round((prorated_psc % 12), 6, MidpointRounding.AwayFromZero).ToString());
            }
        }
        private string _istrPaymentNumber;
        public string istrPaymentNumber
        {
            get { return _istrPaymentNumber; }
            set { _istrPaymentNumber = value; }
        }
    }
}
