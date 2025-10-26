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
	public class busPaymentScheduleStepGen : busExtendBase
    {
		public busPaymentScheduleStepGen()
		{

		}

		public cdoPaymentScheduleStep icdoPaymentScheduleStep { get; set; }




		public virtual bool FindPaymentScheduleStep(int Aintpaymentschedulestepid)
		{
			bool lblnResult = false;
			if (icdoPaymentScheduleStep == null)
			{
				icdoPaymentScheduleStep = new cdoPaymentScheduleStep();
			}
			if (icdoPaymentScheduleStep.SelectRow(new object[1] { Aintpaymentschedulestepid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        public busPaymentStepRef ibusPaymentStep { get; set; }
        public void LoadPaymentStep()
        {
            if (ibusPaymentStep == null)
                ibusPaymentStep = new busPaymentStepRef();
            ibusPaymentStep.FindPaymentStepRef(icdoPaymentScheduleStep.payment_step_id);
        }
	}
}
    