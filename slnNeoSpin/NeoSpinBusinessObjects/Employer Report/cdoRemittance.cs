#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoRemittance : doRemittance
	{
		public cdoRemittance() : base()
		{
		}

        public decimal idecRefundAmount
        {
            get
            {
                return (overridden_refund_amount == 0.0M ? computed_refund_amount : overridden_refund_amount);
            }
        }

        //UCS - 079
        //Property to contain gross reduction amount
        //To display in datagrid and capture the amount
        public decimal idecGrossReductionAmount { get; set; }
        //UCS - 079
        //Property to contain AppliedDate
        //To display in datagrid and capture the Date
        public DateTime idtAppliedDate { get; set; }

        //ucs - 038
        //property to contain available remittance amount
        //to post into ibs detail table as part of balance forward
        public decimal idecAvailableRemittanceAmount { get; set; }

        public string allocation_status { get; set; }

        public string allocation_status_description
        {
            get
            {
                return iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1310, allocation_status);
            }
        }

		public int remittance_history_header_id { get; set; }
    } 
} 
