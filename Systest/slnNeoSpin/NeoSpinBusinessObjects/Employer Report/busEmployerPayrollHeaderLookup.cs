#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Globalization;
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busEmployerPayrollHeaderLookup
    {
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;

            if (ahstParam["astr_benefit_type"] == null || (ahstParam["astr_benefit_type"]!=null && ahstParam["astr_benefit_type"].ToString()=="All"))
            {
                lobjError = AddError(4737, String.Empty);
                larrErrors.Add(lobjError);
            }
            return larrErrors;
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busEmployerPayrollHeader lbusEmployerPayrollHeader = (busEmployerPayrollHeader)aobjBus;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(adtrRow);
            lbusEmployerPayrollHeader.ibusOrganization = new busOrganization();
            lbusEmployerPayrollHeader.ibusOrganization.icdoOrganization = new cdoOrganization();
            lbusEmployerPayrollHeader.ibusOrganization.icdoOrganization.LoadData(adtrRow); 
            if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
            {
                lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            }
            else
            {
                lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = String.Empty;
            }
            
            lbusEmployerPayrollHeader.idecTotalPremiumReportedForIns = Convert.ToDecimal(adtrRow["InsuranceAmount"]) - Convert.ToDecimal(adtrRow["NegInsuranceAdjAmount"]);
            //PIR 24399
            if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) 
            {
                lbusEmployerPayrollHeader.LoadRetirementContributionByPlan();
                lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecRemainingBalance = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecOutstandingRetirementBalance + lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecOutstandingRHICBalance;
                    //+lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecOutstandingADECBalance;    //PIR 25920 DC 2025 changes
            }
            if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                lbusEmployerPayrollHeader.LoadESSDeferredCompContributionByPlan();
                lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecRemainingBalance = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecOutstandingDeferredCompBalance;
            }
            if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                lbusEmployerPayrollHeader.LoadESSInsurancePremiumByPlan();
                lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecRemainingBalance = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecInsuranceContributionByPlan;
            }
            if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                lbusEmployerPayrollHeader.LoadPurchaseByPlan();
                lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecRemainingBalance = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.idecOutstandingAmount;
            }
        }
       
	    // ESS Backlog PIR - 13416
        public void LoadCommentsOfPayrollHeaders()
        {
            const string lstrSpaceSeperator = " ";
            StringBuilder sb = new StringBuilder();
            foreach (busEmployerPayrollHeader iobjbusEmployerPayrollHeader in icolEmployerPayrollHeader)
            {
                iobjbusEmployerPayrollHeader.LoadEmployerPayrollHeaderComments();
                if (iobjbusEmployerPayrollHeader.iclbPayrollHeaderCommentsHistory.Count > 0)
                {
                    sb.Append(iobjbusEmployerPayrollHeader.iclbPayrollHeaderCommentsHistory.FirstOrDefault().icdoComments.comments.ToString());
                    sb.Append(lstrSpaceSeperator);
                    sb.Append(iobjbusEmployerPayrollHeader.iclbPayrollHeaderCommentsHistory.FirstOrDefault().icdoComments.created_by.ToString());
                    sb.Append(lstrSpaceSeperator);
                    sb.Append(iobjbusEmployerPayrollHeader.iclbPayrollHeaderCommentsHistory.FirstOrDefault().icdoComments.created_date.ToString());
                    if (iobjbusEmployerPayrollHeader.iclbPayrollHeaderCommentsHistory.Count > 1)
                    {
                        sb.Append(";");
                        sb.Append(lstrSpaceSeperator);
                        sb.Append(iobjbusEmployerPayrollHeader.iclbPayrollHeaderCommentsHistory.Last().icdoComments.comments.ToString());
                        sb.Append(lstrSpaceSeperator);
                        sb.Append(iobjbusEmployerPayrollHeader.iclbPayrollHeaderCommentsHistory.Last().icdoComments.created_by.ToString());
                        sb.Append(lstrSpaceSeperator);
                        sb.Append(iobjbusEmployerPayrollHeader.iclbPayrollHeaderCommentsHistory.Last().icdoComments.created_date.ToString());
                    }
                    iobjbusEmployerPayrollHeader.icdoEmployerPayrollHeader.istrlookupComment += sb.ToString();
                    sb.Clear();
                }
            }
        }
    }
}

