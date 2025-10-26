#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoOrgPlan : doOrgPlan
	{
		public cdoOrgPlan() : base()
		{
		}

        private DateTime _end_date_no_null;
        public DateTime end_date_no_null
        {
            get 
            {
                if (participation_end_date == DateTime.MinValue)
                    _end_date_no_null = DateTime.MaxValue;
                else
                    _end_date_no_null = participation_end_date;
                return _end_date_no_null; 
            }
        }

        public string istrWellnessFlagFormatted
        {
            get
            {
                if (wellness_flag == busConstant.Flag_Yes)
                    return "Yes";
                else
                    return "No";
            }
        }

        //public string istrPreTaxPurchase
        //{
        //    get
        //    {
        //        if (pre_tax_purchase == busConstant.Flag_Yes)
        //            return "Yes";
        //        else
        //            return "No";
        //    }
        //}
    } 
} 
