#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using System.Globalization;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoServicePurchaseDetailUserra : doServicePurchaseDetailUserra
    {
        public cdoServicePurchaseDetailUserra()
            : base()
        {
        }

        private decimal _idecTotalPurchaseCost = 0;
        public decimal TotalPurchaseCost
        {
            get
            {
                //PIR 9314 - Employee pickup added in calculation
                _idecTotalPurchaseCost = (employee_contribution + employee_pickup + employer_contribution + rhic_contribution);
                return _idecTotalPurchaseCost;
            }
        }

        private string _idtMissingMonthFormatted;

        public string idtMissingMonthFormatted
        {
            set
            {
                _idtMissingMonthFormatted =value;
            }
            get
            {
                if ((missed_salary_month != DateTime.MinValue) && (_idtMissingMonthFormatted == null))
                { _idtMissingMonthFormatted = missed_salary_month.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US")); }                            
                return _idtMissingMonthFormatted;  
            }           
        }	
    }
}