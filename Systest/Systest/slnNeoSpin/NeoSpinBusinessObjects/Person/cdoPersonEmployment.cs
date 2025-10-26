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
	public class cdoPersonEmployment : doPersonEmployment
	{
		public cdoPersonEmployment() : base()
		{
		}
        private string _istrOrgCodeID;
        public string istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }
        private string _istrOrgName;
        public string istrOrgName
        {
            get { return _istrOrgName; }
            set { _istrOrgName = value; }
        }
        private DateTime _end_date_no_null;
        public DateTime end_date_no_null
        {
            get 
            {
                if (end_date == DateTime.MinValue)
                    _end_date_no_null = DateTime.MaxValue;
                else
                    _end_date_no_null = end_date;
                return _end_date_no_null; 
            }
        }
        public DateTime end_date_no_null_today
        {
            get
            {
                return (end_date == DateTime.MinValue) ? DateTime.Today : end_date;
            }
        }

        //Date of last paycheck 'PIR-8298'
        //public DateTime date_of_last_paycheck { get; set; }


        // PIR 8852
        //public DateTime new_date_of_last_paycheck { get; set; } 

        public string end_long_date
        {
            get
            { 
                return end_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }

        // To Determine the screen while checking for the possible duplicate person records
        // 1 - indicates the Person Employment Maintenance
        // 2 - indicates the Person Employment Maintenance
        public int iintScreenIdentifier
        {
            get
            {
                return 2;
            }
        }

        public string start_date_long
        {
            get
            {
                return start_date.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }

        public DateTime new_start_date { get; set; }
        
        public string istrStartDateFSA
        {
            get
            {
                if (start_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return start_date.ToString(busConstant.DateFormatYearMonthDay);
            }
        }

        public string istrEndDateFSA
        {
            get
            {
                // PIR 10792 Always return blank Employment termination date 
                if (end_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return end_date.ToString(busConstant.DateFormatYearMonthDay);
            }
        }

        //PIR - 14214
        public override int Update()
        {
            DateTime ldtOldEndDate = DateTime.MaxValue;
            if (this.ihstOldValues != null && this.ihstOldValues.Count > 0)
            {
                ldtOldEndDate = Convert.ToDateTime(this.ihstOldValues["end_date"]);
            }

            base.Update();
            if (ldtOldEndDate == DateTime.MinValue && this.end_date != DateTime.MinValue)
            {
                busPersonEmployment lobjbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = this };
                lobjbusPersonEmployment.InitiateWorkFlowForServicePurchasePaymentInstallmentsTermination();
            }
            return 1;
        }
        public bool iblnEmploymentEndDateFromDeath { get; set; }

    }
} 
