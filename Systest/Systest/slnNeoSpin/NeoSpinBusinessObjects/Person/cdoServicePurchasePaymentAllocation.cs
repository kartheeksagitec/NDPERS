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
    public class cdoServicePurchasePaymentAllocation : doServicePurchasePaymentAllocation
    {
        public cdoServicePurchasePaymentAllocation()
            : base()
        {
        }

        private decimal _remittance_amount;

        public decimal remittance_amount
        {
            get { return _remittance_amount; }
            set { _remittance_amount = value; }
        }

        // This derived property is used to display the prorated vsc in custom formated
        public string prorated_vsc_formatted
        {
            get
            {
                if (prorated_vsc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(prorated_vsc / 12).ToString(),
                                    Math.Round((prorated_vsc % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(prorated_vsc / 12).ToString(),
                                     Math.Round((prorated_vsc % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        // This derived property is used to display the prorated psc in custom formated
        public string prorated_psc_formatted
        {
            get
            {
                if (prorated_psc < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(prorated_psc / 12).ToString(),
                                    Math.Round((prorated_psc % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(prorated_psc / 12).ToString(),
                                     Math.Round((prorated_psc % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }
    }
}
