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
	public class cdoPersonAccountDependent : doPersonAccountDependent
	{
		public cdoPersonAccountDependent() : base()
		{
		}

        private int _plan_id;
        public int plan_id
        {
            get { return _plan_id; }
            set { _plan_id = value; }
        }

        private string _istrPlanName;
        public string istrPlanName
        {
            get { return _istrPlanName; }
            set { _istrPlanName = value; }
        }

        private bool _lblnValueEntered;
        public bool lblnValueEntered
        {
            get { return _lblnValueEntered; }
            set { _lblnValueEntered = value; }
        }

        //prod pir 7613
        public bool iblnNeedToCreateAutomaticRHIC { get; set; }
        
        public DateTime start_date_no_null
        {
            get 
            {
                if (start_date != DateTime.MinValue)
                    return start_date;
                return DateTime.MaxValue; 
            }
        }

        public DateTime end_date_no_null
        {
            get
            {
                if (end_date != DateTime.MinValue)
                    return end_date;
                return DateTime.MaxValue;
            }
        }

        public decimal Total_Premium_Amount { get; set;}

        //PIR - 16731
		public override int Update()
        {
            DateTime ldtOldEndDate = DateTime.MaxValue;
            if (this.ihstOldValues != null && this.ihstOldValues.Count > 0)
            {
              ldtOldEndDate = Convert.ToDateTime(this.ihstOldValues["end_date"]);
            }
            base.Update();
            if (this.plan_id == busConstant.PlanIdGroupHealth)
            {
                if ((ldtOldEndDate == DateTime.MinValue) && (this.end_date != DateTime.MinValue))
                {
                    busPersonAccountDependent lobjbusPersonAccountDependent = new busPersonAccountDependent { icdoPersonAccountDependent = this };
                    lobjbusPersonAccountDependent.InitiateWorkFlowMaintainRHICUncombined();
                }
            }
            return 1;
        }
    } 
} 
