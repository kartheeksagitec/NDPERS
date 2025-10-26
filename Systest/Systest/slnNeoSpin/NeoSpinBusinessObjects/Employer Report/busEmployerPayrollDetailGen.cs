#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public partial class busEmployerPayrollDetail : busExtendBase
	{
		public busEmployerPayrollDetail()
		{

		}
        public bool iblnIsFromESS { get; set; }
		private cdoEmployerPayrollDetail _icdoEmployerPayrollDetail;
		public cdoEmployerPayrollDetail icdoEmployerPayrollDetail
		{
			get
			{
				return _icdoEmployerPayrollDetail;
			}

			set
			{
				_icdoEmployerPayrollDetail = value;
			}
		}

        private Collection<busEmployerPayrollDetail> _iclbEmpPayrollDetailMedicare;
        public Collection<busEmployerPayrollDetail> iclbEmpPayrollDetailMedicare
        {
            get { return _iclbEmpPayrollDetailMedicare; }
            set { _iclbEmpPayrollDetailMedicare = value; }
        }

		public bool FindEmployerPayrollDetail(int Aintemployerpayrolldetailid)
		{
			bool lblnResult = false;
			if (_icdoEmployerPayrollDetail == null)
			{
				_icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();
			}
			if (_icdoEmployerPayrollDetail.SelectRow(new object[1] { Aintemployerpayrolldetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        //PIR 16601
        public bool FindEmployerPayrollDetailMedicare(int AintPersonID, DateTime adtPayPeriodDate)
        {
            bool lblnResult = false;

            DataTable ldtbList = SelectWithOperator<cdoEmployerPayrollDetail>(
                new string[5] { "person_id", "pay_period_date", "record_type_value", "plan_id", "status_value" },
                new string[5] { "=", "=", "=", "=", "<>" },
                new object[5] { AintPersonID,  adtPayPeriodDate, busConstant.PayrollDetailRecordTypeRegular, busConstant.PlanIdMedicarePartD, 
                busConstant.PayrollDetailStatusIgnored}, null);

            if (icdoEmployerPayrollDetail == null)
                icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();

            iclbEmpPayrollDetailMedicare = GetCollection<busEmployerPayrollDetail>(ldtbList, "icdoEmployerPayrollDetail");

            if (ldtbList.Rows.Count > 0)
            {
                icdoEmployerPayrollDetail.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
	}
}
