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
    public class cdoPayeeAccountPaymentItemType : doPayeeAccountPaymentItemType
    {
        public cdoPayeeAccountPaymentItemType()
            : base()
        {
        }

        private string _vendor_org_code;
        public string vendor_org_code
        {
            get { return _vendor_org_code; }
            set { _vendor_org_code = value; }
        }
        public DateTime end_date_no_null
        {
            get
            {
                if (end_date == DateTime.MinValue)
                    return DateTime.MaxValue;
                return end_date;
            }
        }
        public decimal amount_multiplied_by_item_direction { get; set; }

       
    }
}