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
    public partial class busEmployerPayrollDetailLookup
    {
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;

            if ((ahstParam["employer_payroll_header_id"] != null) && (ahstParam["employer_payroll_header_id"].ToString() != ""))
            {
                DataTable ldtbList = busNeoSpinBase.Select<cdoEmployerPayrollHeader>(new string[1] { "employer_payroll_header_id" }
                                            , new object[1] { Convert.ToInt32(ahstParam["employer_payroll_header_id"]) }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    if (ldtbList.Rows[0]["header_type_value"].ToString() == busConstant.PayrollHeaderBenefitTypeInsr)
                    {
                        lobjError = AddError(4691, String.Empty);
                        larrErrors.Add(lobjError);
                    }
                    if ((ldtbList.Rows[0]["status_value"].ToString() == busConstant.PayrollHeaderStatusIgnored)
                        || (ldtbList.Rows[0]["status_value"].ToString() == busConstant.PayrollHeaderStatusPosted)
                        || (ldtbList.Rows[0]["status_value"].ToString() == busConstant.PayrollHeaderStatusReadyToPost))
                    {
                        lobjError = AddError(4692, String.Empty);
                        larrErrors.Add(lobjError);
                    }
                }
                else
                {
                    lobjError = AddError(4693, String.Empty);
                    larrErrors.Add(lobjError);
                }
            }
            else
            {
                lobjError = AddError(4694, String.Empty);
                larrErrors.Add(lobjError);
            }
            return larrErrors;
        }
        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            busEmployerPayrollDetail lbusEmployerPayrollDetail = (busEmployerPayrollDetail)aobjBus;
            lbusEmployerPayrollDetail.ibusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
            lbusEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(adtrRow);                        
            lbusEmployerPayrollDetail.ibusEmployerPayrollHeader.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            lbusEmployerPayrollDetail.ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.LoadData(adtrRow);

        }

        //Code Optimization changes

        //protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        //{
            //busEmployerPayrollDetail lbusEmployerPayrollDetail = (busEmployerPayrollDetail)aobjBus;

            //lbusEmployerPayrollDetail.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            //lbusEmployerPayrollDetail.ibusPlan.icdoPlan.LoadData(adtrRow);
            //lbusEmployerPayrollDetail.ibusPerson = new busPerson() { icdoPerson = new cdoPerson() };
            //lbusEmployerPayrollDetail.ibusPerson.icdoPerson.LoadData(adtrRow);
            //if (lbusEmployerPayrollDetail.ibusEmployerPayrollHeader == null)
            //{
            //    lbusEmployerPayrollDetail.LoadPayrollHeader();
            //    lbusEmployerPayrollDetail.ibusEmployerPayrollHeader.LoadOrganization();
            //}
            //if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date != DateTime.MinValue)
            //{
            //    lbusEmployerPayrollDetail.pay_period = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            //}
            //else
            //{
            //    lbusEmployerPayrollDetail.pay_period = String.Empty;
            //}
            //if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus != DateTime.MinValue)
            //{
            //    lbusEmployerPayrollDetail.pay_end_month = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
            //}
            //else
            //{
            //    lbusEmployerPayrollDetail.pay_end_month = String.Empty;
            //}
        //}

        // ESS Backlog PIR - 13416
        public void LoadCommentsOfPayrollDetails()
        {
            const string lstrSpaceSeperator = " ";
            StringBuilder sb = new StringBuilder();
            foreach (busEmployerPayrollDetail iobjbusEmployerPayrollDetail in iclbPayrollDetail)
            {
                iobjbusEmployerPayrollDetail.LoadEmployerPayrollDetailComments();
                if (iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Count > 0)
                {
                    sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.FirstOrDefault().icdoComments.comments.ToString());
                    sb.Append(lstrSpaceSeperator);
                    sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.FirstOrDefault().icdoComments.created_by.ToString());
                    sb.Append(lstrSpaceSeperator);
                    sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.FirstOrDefault().icdoComments.created_date.ToString());
                    if (iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Count > 1)
                    {
                        sb.Append(";");
                        sb.Append(lstrSpaceSeperator);
                        sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Last().icdoComments.comments.ToString());
                        sb.Append(lstrSpaceSeperator);
                        sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Last().icdoComments.created_by.ToString());
                        sb.Append(lstrSpaceSeperator);
                        sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Last().icdoComments.created_date.ToString());
                    }
                    iobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.istrlookupComment += sb.ToString();
                    sb.Clear();
                }
            }
        }
    }
}
