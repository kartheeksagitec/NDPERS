#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.Common;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.CustomDataObjects;
using System.Linq;
using NeoSpin.DataObjects;
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitRefundApplication : busBenefitApplication
    {
        private const string CONTRIBUTING = "CONT";
        private const string OPENEMPLOYMENT = "OPEN";
        private const string DAY31RULEFORPAYMENTDATE = "31DP";
        private const string DAY31RULEFORRECIEVEDDATE = "31DR";

        //Visible Calculate button if there is No application exist for refund
        public bool IsRefundCalculated()
        {
            if (iclbBenefitRefundCalculation == null)
                LoadRefundBenefitCalculation();
            if (iclbBenefitRefundCalculation.Count > 0)
            {
                return true;
            }
            return false;
        }

        public bool IsMemberEligibleForRefund()
        {
            string lstrMemberStatus = IsMemberEligibleAsOnPaymentDateAndRecievedDate();
            if ((lstrMemberStatus == CONTRIBUTING) || (lstrMemberStatus == OPENEMPLOYMENT) || (lstrMemberStatus == DAY31RULEFORPAYMENTDATE) ||
                (lstrMemberStatus == DAY31RULEFORRECIEVEDDATE))
            {
                return true;
            }
            return false;
        }


        //UCS -058 - BR-09 Check whether DRO Benefit Application exists ,if so throw an error
        public string ValidateBenefitDROApplicationExist()
        {
            if (IsBenefitDROApplicationExist())
            {
                foreach (busBenefitDroApplication lobjBenefitDROAppilcation in iclbBenefitDROApplication)
                {
                    if (lobjBenefitDROAppilcation.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
                    {
                        return busConstant.DROApplicationStatusQualified;
                    }
                    else if ((lobjBenefitDROAppilcation.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusRecieved) ||
                        (lobjBenefitDROAppilcation.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved))
                    {
                        return busConstant.DROApplicationStatusApproved;
                    }
                }
            }
            return string.Empty;
        }
        private Collection<busBenefitRefundCalculation> _iclbBenefitRefundCalculation;
        public Collection<busBenefitRefundCalculation> iclbBenefitRefundCalculation
        {
            get { return _iclbBenefitRefundCalculation; }
            set { _iclbBenefitRefundCalculation = value; }
        }
        public void LoadRefundBenefitCalculation()
        {
            if (_iclbBenefitRefundCalculation == null)
                _iclbBenefitRefundCalculation = new Collection<busBenefitRefundCalculation>();
            DataTable ldtbBenefitRefundCalculation = Select("cdoBenefitApplication.LoadRefundBenefitCalculation",
                                            new object[1] { icdoBenefitApplication.benefit_application_id });
            foreach (DataRow drRefundBenefitCalculation in ldtbBenefitRefundCalculation.Rows)
            {
                busBenefitRefundCalculation lobjBenefitRefundCalculation = new busBenefitRefundCalculation();
                lobjBenefitRefundCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
                lobjBenefitRefundCalculation.icdoBenefitRefundCalculation = new cdoBenefitRefundCalculation();
                lobjBenefitRefundCalculation.icdoBenefitRefundCalculation.LoadData(drRefundBenefitCalculation);
                lobjBenefitRefundCalculation.icdoBenefitCalculation.LoadData(drRefundBenefitCalculation);
                _iclbBenefitRefundCalculation.Add(lobjBenefitRefundCalculation);
            }
        }
        public void LoadPayeeAccountID()
        {
            if (_iclbBenefitRefundCalculation == null)
                LoadRefundBenefitCalculation();
            foreach (busBenefitRefundCalculation lobjBenefitRefundCalculation in iclbBenefitRefundCalculation)
            {
                lobjBenefitRefundCalculation.LoadPayeeAccountID();
            }
        }
        public void LoadPerson()
        {
            if (ibusPerson == null)
            {
                ibusPerson = new busPerson();
            }
            ibusPerson.FindPerson(icdoBenefitApplication.member_person_id);
        }

        public override ArrayList btnCancelClicked()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            // PIR 17082
            larrList = CheckCancelClickIsValid();
            if (larrList.Count == 0)
            {
                icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusCancelled;
                icdoBenefitApplication.action_status_effective_date = DateTime.Now;
                icdoBenefitApplication.Update();

                //busWorkflowHelper.UpdateWorkflowActivityByEvent(this, enmNextAction.Cancel, busConstant.ActivityStatusCancelled, iobjPassInfo);
                larrList.Add(this);
            }
            return larrList;
        }
        public ArrayList btnSuspendClicked()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusDeferred;
            icdoBenefitApplication.action_status_effective_date = DateTime.Now;
            icdoBenefitApplication.Update();

            busBpmActivityInstance lobjActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
            if (lobjActivityInstance != null && lobjActivityInstance.icdoBpmActivityInstance != null && lobjActivityInstance.icdoBpmActivityInstance.activity_instance_id > 0)
            {
                //PIR 1212. As per discussion with satya suspend the workflow for 6 months, and the batch will resume it in the 5 th or 6 th month
                busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusInProcess,ibusBaseActivityInstance,utlPassInfo.iobjPassInfo);

                lobjActivityInstance.icdoBpmActivityInstance.suspension_end_date = DateTime.Now.AddMonths(6);
                busWorkflowHelper.UpdateWorkflowActivityByStatus(busConstant.ActivityStatusSuspended, ibusBaseActivityInstance, iobjPassInfo);
            }
            larrList.Add(this);
            return larrList;
        }
        public override ArrayList btnPendingVerificationClicked()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                //1498
                //if the previous status is Verfied
                //change the status of calculation and Payee account as Pending approval and review
                if (icdoBenefitApplication.ihstOldValues.Count > 0)
                {
                    if (icdoBenefitApplication.ihstOldValues["action_status_value"].ToString() == busConstant.ApplicationActionStatusVerified)
                        ChangeStatusOfPayeeCalculationIfAppStatusIsPendingVerified();
                }
                icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;
                icdoBenefitApplication.action_status_effective_date = DateTime.Today;
                icdoBenefitApplication.Update();
            }
            return larrList;
        }
        public override ArrayList btnVerfiyClicked()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
            if (icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (ibusPerson == null)
                    LoadPerson();
                //BR - 08 Change Payee status to review if date of death of death is recorded on person aacount
                if (ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                {
                    lobjError = AddError(1913, "");
                    larrList.Add(lobjError);
                }
                //PIR 16284 & 17801 //PIR 19010 & 18993
                if (DoesPersonHaveOpenContriEmpDtlByPlan())
                {
                    lobjError = AddError(7505, "");
                    larrList.Add(lobjError);
                }
                if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionRegularRefund && icdoBenefitApplication.payment_date == DateTime.MinValue)
                {
                    lobjError = new utlError();
                    lobjError = AddError(7524, string.Empty);//Payment date is required
                    larrList.Add(lobjError);
                }
                if (IsBenefitOptionAutoOrRegularRefund() && icdoBenefitApplication.termination_date == DateTime.MinValue)
                {
                    lobjError = new utlError();
                    lobjError = AddError(1905, string.Empty);// Termination Date is required.
                    larrList.Add(lobjError);
                }
                ////PIR 25920 DC 2025 Part 2 DBDC transfer new validation for special election just check the december contribution instead all 
                //if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
                //{
                //    if(icdoBenefitApplication.suppress_warnings_flag != busConstant.Flag_Yes)
                //    { 
                //        decimal ldecTotalSalaryForPayPeriodDate = 0;
                //        DataTable ldtbEligibleWages = Select("entPersonAccountRetirementContribution.GetAcctIDAndSalaryForRetirementContByPayPeriod",
                //                                            new object[2] { new DateTime(2024,12,01), icdoBenefitApplication.member_person_id });
                //        foreach (DataRow dr in ldtbEligibleWages.Rows)
                //        {
                //            ldecTotalSalaryForPayPeriodDate += Convert.ToDecimal(dr["salary_amount"].ToString());
                //        }
                //        if (ldecTotalSalaryForPayPeriodDate == 0)
                //        {
                //            lobjError = AddError(10523, "");
                //            larrList.Add(lobjError);
                //        }
                //    }
                //}
                     
                if (ibusPerson.iclbBenefitApplication == null)
                {
                    ibusPerson.LoadBenefitApplication();
                }

                //BR -21 The system must not allow user to change the ‘Application Status’ to ‘Verified’
                //if there exists another ‘Regular Refund’ or ‘Auto Refund’ application in ‘Valid’ status and ‘Payment Date’ is same

                if (ibusPerson.iclbBenefitApplication.Count > 0)
                {
                    foreach (busBenefitApplication lobjRefundApplication in ibusPerson.iclbBenefitApplication)
                    {
                        if ((lobjRefundApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                            && (!lobjRefundApplication.IsApplicationCancelledOrDenied())
                            && (lobjRefundApplication.icdoBenefitApplication.payment_date == icdoBenefitApplication.payment_date)
                            && (lobjRefundApplication.icdoBenefitApplication.status_value == busConstant.StatusValid)
                            && (lobjRefundApplication.icdoBenefitApplication.plan_id == icdoBenefitApplication.plan_id) //prod pir 4110
                            && (lobjRefundApplication.icdoBenefitApplication.benefit_application_id != icdoBenefitApplication.benefit_application_id)
                            && (IsBenefitOptionAutoOrRegularRefund()))
                        {
                            lobjError = AddError(7511, "");
                            larrList.Add(lobjError);
                            break;
                        }
                    }
                }
                //Jon PIR
                if (ValidateBenefitDROApplicationExist() == busConstant.DROApplicationStatusApproved)
                {
                    lobjError = AddError(7539, "");
                    larrList.Add(lobjError);
                }
				//PIR-16812
                if(icdoBenefitApplication.termination_date != DateTime.MinValue)
                {
                  if(AreAnyOutstandingDetailsExist())
                  {
                      lobjError = AddError(0, "Member has outstanding Payroll Details.");
                      larrList.Add(lobjError);
                  }

                }
                if (larrList.Count == 0)
                {
                    //If the benefit option is DB to DC Transfer ,then set payment date as system date + 7 days by the time of application verification
					//PIR 25920 DC 2025 Part 2 
                    if ((icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer || 
                            icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
                         && (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)
                         && (icdoBenefitApplication.payment_date == DateTime.MinValue))
                    {
                        icdoBenefitApplication.payment_date = DateTime.Today.AddDays(7);
                    }

                    //set the ‘Payment Date’ to be next ‘Benefit Payment Date’ by the time of application verification

                    else if ((icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer)
                         && (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB))
                    {
                        icdoBenefitApplication.payment_date = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
                    }
                    else if ((icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE ||
                        icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers) &&
                        ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB &&
                        icdoBenefitApplication.payment_date == DateTime.MinValue)
                    {
                        icdoBenefitApplication.payment_date = DateTime.Today;
                    }
                    icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusVerified;
                    icdoBenefitApplication.action_status_effective_date = DateTime.Today;
                    icdoBenefitApplication.Update();
                    larrList.Add(this);
                }
            }
            this.EvaluateInitialLoadRules();
            return larrList;
        }
        // DC Plan Member can select only Benefit option Regular refund   
        public bool IsDcMemberSelectedTransfer()
        {
            bool lbnIsDcMemberSelectedTransfer = false;
            if (icdoBenefitApplication.plan_id == busConstant.PlanIdDC ||
                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                icdoBenefitApplication.plan_id == busConstant.PlanIdDC2025) //PIR 25920
            {
                if ((icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer) ||
                    (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer) ||
                    (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE) ||
                    (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers))
                {
                    lbnIsDcMemberSelectedTransfer = true;
                }
            }
            return lbnIsDcMemberSelectedTransfer;
        }
        private bool IsBenefitOptionAutoOrRegularRefund()
        {
            if ((icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionRegularRefund) ||
                            (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionAutoRefund))
            {
                return true;
            }
            return false;
        }

        //BR - 10,11,12,31
        public string IsMemberEligibleAsOnPaymentDateAndRecievedDate()
        {
            bool lbnlOpenEmploymentExist = false;
            bool lbnlNonContributing = false;
            DateTime ldteMaxEndDate = new DateTime();
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();
            if (ibusPerson.icolPersonEmployment.Count > 0)
            {
                foreach (busPersonEmployment lobjPersonEmployment in ibusPerson.icolPersonEmployment)
                {

                    if (lobjPersonEmployment.icolPersonEmploymentDetail == null)
                        lobjPersonEmployment.LoadPersonEmploymentDetail();
                    foreach (busPersonEmploymentDetail lobjPersonEmploymentDetail in lobjPersonEmployment.icolPersonEmploymentDetail)
                    {
                        if (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                        {
                            lbnlOpenEmploymentExist = true;
                            if (IsBenefitOptionAutoOrRegularRefund())
                            {
                                if (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing)
                                {
                                    return OPENEMPLOYMENT;
                                }
                                else if ((lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing) ||
                                    (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA) ||
                                    (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM) ||
                                    (lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA))
                                {
                                    return CONTRIBUTING;
                                }
                            }
                        }
                        else
                        {
                            if (ldteMaxEndDate < lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date)
                            {
                                ldteMaxEndDate = lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date;
                            }
                        }
                    }
                }
            }
            if (!lbnlOpenEmploymentExist)
            {
                TimeSpan ltsPaymentDate = icdoBenefitApplication.payment_date.Subtract(ldteMaxEndDate);
                if (ltsPaymentDate.Days < 31)
                {
                    return DAY31RULEFORPAYMENTDATE;
                }
            }
            TimeSpan ltsRecievedDate = icdoBenefitApplication.received_date.Subtract(ldteMaxEndDate);
            if (ltsRecievedDate.Days < 31)
            {
                return DAY31RULEFORRECIEVEDDATE;
            }
            return string.Empty;
        }
        /* BR - 98 
           Allow the TFFR transfer either if the Job Class is ‘CTE Certified Teacher’ or ‘DPI Certified Teacher’ for ‘Benefit Option’ of ‘DB to TFFR Transfer CTE/DPI’ 
           or any Job Class for ‘Benefit Option’ of ‘DB to TFFR Transfer for Dual Members’. Else throw an error
        */
        public bool IsMemberAllowedForTFFRTransfer()
        {
            bool lblIsJobClassCareerAndTechEdCertifiedTeacherExist = false;
            if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.iclbEmploymentDetail == null)
                    ibusPersonAccount.LoadAllPersonEmploymentDetails(false);
                foreach (busPersonEmploymentDetail lobjemploymentdetail in ibusPersonAccount.iclbEmploymentDetail)
                {
                    if ((lobjemploymentdetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassCareerAndTechEdCertifiedTeacher) ||
                        (lobjemploymentdetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassDeptofPublicInstructionCertifiedTeacher))
                    {
                        lblIsJobClassCareerAndTechEdCertifiedTeacherExist = true;
                        break;
                    }
                }
                if (!lblIsJobClassCareerAndTechEdCertifiedTeacherExist)
                {
                    return true;
                }
            }
            return false;
        }
        //PIR 2148
        public void SetTerminationDate()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment();
            if (ibusPerson.icolPersonEmployment.Count > 0)
            {
                if (ibusPerson.icolPersonEmployment[0].icdoPersonEmployment.end_date != DateTime.MinValue)
                    icdoBenefitApplication.termination_date = ibusPerson.icolPersonEmployment[0].icdoPersonEmployment.end_date;
            }
        }
        //BR -check whether Application not received within 90 days of employment,ifnot throw an error
        public bool IsTFFRApplicationRecievedWithin90days()
        {
            if ((icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE) ||
                (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers))
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.iclbEmploymentDetail == null)
                    ibusPersonAccount.LoadAllPersonEmploymentDetails(false);
                foreach (busPersonEmploymentDetail lobjemploymentdetail in ibusPersonAccount.iclbEmploymentDetail)
                {
                    if (lobjemploymentdetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                    {
                        if ((lobjemploymentdetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassCareerAndTechEdCertifiedTeacher) ||
                            (lobjemploymentdetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassDeptofPublicInstructionCertifiedTeacher))
                        {
                            TimeSpan ltsDays = icdoBenefitApplication.received_date.Subtract(lobjemploymentdetail.icdoPersonEmploymentDetail.start_date);
                            if (ltsDays.Days > 90)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        //If a person is applying for DB To TIAA-CREF Transfer, den person account should be suspended status
        //Else throw an error
        public bool IsPersonAccountSuspended()
        {
            if (icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetimentSuspended)
                {
                    return true;
                }
            }
            return false;
        }

        //if hardship approved flag or thirty days waiver_flag is not checked ,then payment date should be after 30 days  from application recieved date
        //else throw an error
        public bool IsPaymentDateValid()
        {
            if ((icdoBenefitApplication.hardship_approved_flag == busConstant.Flag_No) &&
                (icdoBenefitApplication.thirty_days_waiver_flag == busConstant.Flag_No))
            {
                TimeSpan ltsDays = icdoBenefitApplication.payment_date.Subtract(icdoBenefitApplication.received_date);
                if (ltsDays.Days < 31)
                {
                    return true;
                }
            }
            return false;
        }
    // as per satya's mail on 3/26/2010
        public string IsPaymentDateValidBasedonHardshipWithrwalOr30DaysWaiver()
        {
            string lstrValidationString = string.Empty;
            DateTime ldtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            DateTime ldtPaymentDate = DateTime.MinValue;
            if (IsBenefitOptionAutoOrRegularRefund() && icdoBenefitApplication.termination_date != DateTime.MinValue)
            {
                ldtPaymentDate = icdoBenefitApplication.termination_date.AddDays(31);

                DateTime ldtFOMFollwingTermDatePlusOneMonth = new DateTime(ldtPaymentDate.AddMonths(1).Year, ldtPaymentDate.AddMonths(1).Month, 1);
                if (icdoBenefitApplication.hardship_approved_flag == busConstant.Flag_No && icdoBenefitApplication.thirty_days_waiver_flag == busConstant.Flag_No)
                {
                    ldtPaymentDate = ldtFOMFollwingTermDatePlusOneMonth > ldtNextBenefitPaymentDate ? ldtFOMFollwingTermDatePlusOneMonth : ldtNextBenefitPaymentDate;
                    DateTime ldtFOMFollwingAppDatePlus30 = DateTime.MinValue;
                    DateTime ldtppDatePlus30 = DateTime.MinValue;
                    if (icdoBenefitApplication.received_date != DateTime.MinValue)
                    {
                        ldtppDatePlus30 = icdoBenefitApplication.received_date.AddDays(30);
                        ldtFOMFollwingAppDatePlus30 = new DateTime(ldtppDatePlus30.AddMonths(1).Year, ldtppDatePlus30.AddMonths(1).Month, 1);
                    }
                    ldtPaymentDate = ldtPaymentDate > ldtFOMFollwingAppDatePlus30 ? ldtPaymentDate : ldtFOMFollwingAppDatePlus30;
                    if (icdoBenefitApplication.termination_date != DateTime.MinValue &&
                       icdoBenefitApplication.payment_date != DateTime.MinValue &&
                       icdoBenefitApplication.payment_date < ldtPaymentDate) //uat pirs 2171 & 2183 : as per satya, need to check less than condition
                    {
                        lstrValidationString = "AFLD";
                    }
                }
                else if (icdoBenefitApplication.hardship_approved_flag == busConstant.Flag_No && icdoBenefitApplication.thirty_days_waiver_flag == busConstant.Flag_Yes)
                {
                    ldtPaymentDate = ldtFOMFollwingTermDatePlusOneMonth > ldtNextBenefitPaymentDate ? ldtFOMFollwingTermDatePlusOneMonth : ldtNextBenefitPaymentDate;
                    if (icdoBenefitApplication.termination_date != DateTime.MinValue &&
                       icdoBenefitApplication.payment_date != DateTime.MinValue &&
                       icdoBenefitApplication.payment_date < ldtPaymentDate) //uat pirs 2171 & 2183 : as per satya, need to check less than condition
                    {
                        lstrValidationString = "TFLE";
                    }
                }
                else if ((icdoBenefitApplication.hardship_approved_flag == busConstant.Flag_Yes && icdoBenefitApplication.thirty_days_waiver_flag == busConstant.Flag_Yes) ||
                   (icdoBenefitApplication.hardship_approved_flag == busConstant.Flag_Yes && icdoBenefitApplication.thirty_days_waiver_flag == busConstant.Flag_No) )
                {
                    if (icdoBenefitApplication.termination_date != DateTime.MinValue &&
                       icdoBenefitApplication.payment_date != DateTime.MinValue &&
                       icdoBenefitApplication.payment_date < ldtPaymentDate)
                    {
                        lstrValidationString = "TFLD";
                    }
                }
            }
            return lstrValidationString;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (IsBenefitDROApplicationExist())
            {
                icdoBenefitApplication.qdro_on_file_flag = busConstant.Flag_Yes;
            }
            base.BeforeValidate(aenmPageMode);
        }
        public override void BeforePersistChanges()
        {            
            DateTime ldtEmpEndDate = DateTime.MinValue;

            //BR - 19 - If any change in application change the status to pending approval
            icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusPending;


            //BR - 37 The system must populate ‘Benefit Option’ into corresponding ‘Benefit Sub Type’ 
            icdoBenefitApplication.benefit_sub_type_value = icdoBenefitApplication.benefit_option_value;
            icdoBenefitApplication.retirement_org_id = GetOrgIdAsLatestEmploymentOrgId(ibusPersonAccount.icdoPersonAccount.person_account_id,
                icdoBenefitApplication.benefit_account_type_value, ref ldtEmpEndDate);
            LoadRTWPersonAccount();
            base.BeforePersistChanges();
        }

        //BR 82- if Benefit Option DB To TIAA CREF Transfer and total tvsc less than 36 months ,then throw an error
        public bool IsTotalTVSCLessThan36Months()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.icdoPersonAccount.Total_VSC == 0.0M)
                ibusPersonAccount.LoadTotalVSC();
            if (ibusPersonAccount.icdoPersonAccount.Total_VSC < 36)
            {
                return true;
            }
            return false;
        }

        public override void AfterPersistChanges()
        {
            if (ibusPerson == null)
                LoadPerson();
            //BR 07 Change Payee status to review if date of death of death is recorded on person aacount
            if (ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
            {
                ChangePayeeAccountStatusToReview();
            }
        }
        //BR - 057-39,40,41,45,46,36,35
        public void CreateAdjustmentRefund(busEmployerPayrollDetail abusEmployerPayrollDetail,DateTime adtContributionDate, 
            bool ablnEmployerReportBatchIndicator, decimal adecAdditionalInterestAmount = 0, decimal adecERPreTaxInterestAmount = 0, decimal adecERVestedAmount = 0)
        {
            bool lblnCreateAdjustmentRefundCalculation = false;

            //Backlog PIR 9924 - commented out below condition of payee account status = processed because we are creating status of payee account = review before this function called. 
            //if ((icdoBenefitApplication.status_value == busConstant.ApplicationStatusProcessed) && (IsRefundPayeeAccountProccessed()))
            //{
                if (icdoBenefitApplication.termination_date.GetLastDayofMonth() >= adtContributionDate)
                {
                    lblnCreateAdjustmentRefundCalculation = true;
                }
            //}
            //ADJ calculation was getting created even if no final calculation exists, it should only be created when final calculation exists 
            //in pending approval status
            if (iclbBenefitRefundCalculation == null)
                LoadRefundBenefitCalculation();
            if ((iclbBenefitRefundCalculation.Count > 0) && (iclbBenefitRefundCalculation.Any(i => i.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal &&
                i.icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusPendingApproval)))
            {
                lblnCreateAdjustmentRefundCalculation = true;
            }
            else
            {
                lblnCreateAdjustmentRefundCalculation = false;
            }

            if (lblnCreateAdjustmentRefundCalculation)
            {
                bool lblnIsRefundAdjustmentExists = false;
                if (iclbBenefitRefundCalculation == null)
                    LoadRefundBenefitCalculation();

                foreach (busBenefitRefundCalculation lobjRefundCalcluation in iclbBenefitRefundCalculation)
                {
                    if (lobjRefundCalcluation.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                    {
                        if (!lobjRefundCalcluation.IsRefundCalculationProccessedforAdjustment())
                        {
                            lblnIsRefundAdjustmentExists = true;
                            lobjRefundCalcluation.UpdateAdjustmentForRefundCalculation(abusEmployerPayrollDetail, ablnEmployerReportBatchIndicator,
                            adecAdditionalInterestAmount: adecAdditionalInterestAmount, adecERPreTaxInterestAmount: adecERPreTaxInterestAmount, adecERVestedAmount: adecERVestedAmount);
                            continue;
                        }
                    }
                    // PROD PIR ID 5181
                    //PROD PIR 5609
                    if (lobjRefundCalcluation.icdoBenefitCalculation.action_status_value == busConstant.BenefitActionStatusApproved &&
                        lobjRefundCalcluation.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal &&
                        lobjRefundCalcluation.icdoBenefitCalculation.status_value != busConstant.CalculationStatusProcessed)
                    {
                        UpdateRefundCalculationStatus(lobjRefundCalcluation);
                        lblnIsRefundAdjustmentExists = true;
                    }
                }

                if (!lblnIsRefundAdjustmentExists)
                {
                    CreateAdjustmentCalculation(abusEmployerPayrollDetail, ablnEmployerReportBatchIndicator, adecAdditionalInterestAmount:adecAdditionalInterestAmount,
                    adecERPretaxInterst: adecERPreTaxInterestAmount, adecVestedERAmount: adecERVestedAmount);
                }
            }
        }

        private void CreateAdjustmentCalculation(busEmployerPayrollDetail abusEmployerPayrollDetail, bool ablnEmployerReportBatchIndicator, decimal adecAdditionalInterestAmount = 0,decimal adecERPretaxInterst = 0,
            decimal adecVestedERAmount = 0)
        {
            busBenefitRefundCalculation lbusBenefitRefundCalculation = new busBenefitRefundCalculation();
            lbusBenefitRefundCalculation.icdoBenefitCalculation = new cdoBenefitCalculation();
            lbusBenefitRefundCalculation.icdoBenefitRefundCalculation = new cdoBenefitRefundCalculation();
            //PIR 11946
            if (abusEmployerPayrollDetail.IsNotNull() && abusEmployerPayrollDetail.ibusPersonAccount.IsNotNull())
                lbusBenefitRefundCalculation.ibusPersonAccount = abusEmployerPayrollDetail.ibusPersonAccount;

            lbusBenefitRefundCalculation.icdoBenefitCalculation.benefit_application_id = icdoBenefitApplication.benefit_application_id;
            lbusBenefitRefundCalculation.icdoBenefitCalculation.person_id = icdoBenefitApplication.member_person_id;
            lbusBenefitRefundCalculation.icdoBenefitCalculation.plan_id = icdoBenefitApplication.plan_id;
            lbusBenefitRefundCalculation.ibusRefundBenefitApplication = this;
            lbusBenefitRefundCalculation.LoadDefaultValues(false);
            lbusBenefitRefundCalculation.LoadBenefitCalculationPayeeForNewMode();
            if (ablnEmployerReportBatchIndicator)
            {
                lbusBenefitRefundCalculation.SetAdjustmentRefundForAdditionalContributions(abusEmployerPayrollDetail);
                //prod pir 6153
                lbusBenefitRefundCalculation.icdoBenefitRefundCalculation.ee_interest_amount += adecAdditionalInterestAmount;
            }
            else
            {
                lbusBenefitRefundCalculation.icdoBenefitRefundCalculation.ee_interest_amount += adecAdditionalInterestAmount;
                lbusBenefitRefundCalculation.icdoBenefitRefundCalculation.er_interest_amount += adecERPretaxInterst;
                lbusBenefitRefundCalculation.icdoBenefitRefundCalculation.vested_er_contribution_amount += adecVestedERAmount;
            }        
            lbusBenefitRefundCalculation.icdoBenefitRefundCalculation.benefit_calculation_id
                                         = lbusBenefitRefundCalculation.icdoBenefitCalculation.benefit_calculation_id;
            lbusBenefitRefundCalculation.icdoBenefitRefundCalculation.benefit_option_value_to_compare = lbusBenefitRefundCalculation.icdoBenefitCalculation.benefit_option_value;
            lbusBenefitRefundCalculation.icdoBenefitRefundCalculation.person_id = lbusBenefitRefundCalculation.icdoBenefitCalculation.person_id;  
            lbusBenefitRefundCalculation.iblnAdditionalContributionsReportedFlag = true;
            lbusBenefitRefundCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
            lbusBenefitRefundCalculation.icdoBenefitRefundCalculation.ienuObjectState = ObjectState.Insert;            
            lbusBenefitRefundCalculation.BeforePersistChanges();           
            lbusBenefitRefundCalculation.PersistChanges();
            lbusBenefitRefundCalculation.ValidateSoftErrors();
            lbusBenefitRefundCalculation.UpdateValidateStatus();
            lbusBenefitRefundCalculation.AfterPersistChanges();
        }

        private bool IsRefundPayeeAccountProccessed()
        {
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus == null)
                    lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundProcessed)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateRefundCalculationStatus(busBenefitRefundCalculation lobjRefundCalcluation)
        {
            lobjRefundCalcluation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
            lobjRefundCalcluation.icdoBenefitCalculation.Update();           
            lobjRefundCalcluation.ibusRefundBenefitApplication = this;
            lobjRefundCalcluation.iblnAdditionalContributionsReportedFlag = true;
            lobjRefundCalcluation.ValidateSoftErrors();
            lobjRefundCalcluation.UpdateValidateStatus();
        }

        private bool IsRefundCalculationProccessedForFinal(busBenefitRefundCalculation aobjRefundBenefitCalcluation)
        {
            if ((aobjRefundBenefitCalcluation.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal)
                && (aobjRefundBenefitCalcluation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                && (aobjRefundBenefitCalcluation.icdoBenefitCalculation.status_value == busConstant.CalculationStatusProcessed))
            {
                return true;
            }
            return false;
        }

        #region Properties for Correspondence
        //Corrs. UCS - 057
        //Property to return month part of payment date
        public string istrDistributionMonth
        {
            get
            {
                return icdoBenefitApplication.payment_date.ToString("MMMM");
            }
        }

        //Corrs. UCS-57 - TIAA-CREF TRANSFER
        //Property to return TVSC
        public decimal idecTotalTVSC
        {
            get
            {
                if (icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                {
                    // added received date as parameter done by Deepa
                    // in order to get only the contribution those falls below this date
                    return Math.Round(ibusPerson.GetTotalVSCForPerson(true, icdoBenefitApplication.received_date), MidpointRounding.AwayFromZero);
                }
                else
                {
                    // added received date as parameter done by Deepa
                    // in order to get only the contribution those falls below this date
                    return Math.Round(ibusPerson.GetTotalVSCForPerson(false, icdoBenefitApplication.received_date), MidpointRounding.AwayFromZero);
                }
            }
        }

        #endregion

        #region Methods for Correpondence
        public override busBase GetCorPerson()
        {
            return ibusPerson;
        }
        #endregion


        // PIR 25920 Apply visibility on new benefit option 
        public Collection<cdoCodeValue> LoadBenefitOptionsWithSpecialElection()
        {
            Collection<cdoCodeValue> lclcBenefitOption = new Collection<cdoCodeValue>();
            Collection<busCodeValue> lclbBenefitOption = busGlobalFunctions.LoadCodeValueByData1And2(2216, null, busConstant.Refund, "=","=");
            if (lclbBenefitOption.Count > 0)
            {
                foreach (busCodeValue lobjCodeValue in lclbBenefitOption)
                {
                    if (lobjCodeValue.icdoCodeValue.code_value == busConstant.BenefitOptionDBToDCTransferSpecialElection)
                    {
                        if (ibusPersonAccount == null)
                            LoadPersonAccount();
                        ibusPersonAccount.LoadHb1040Communication();
                        if (ibusPersonAccount.ibusHb1040Communication.IsNotNull() && ibusPersonAccount.ibusHb1040Communication.icdoHb1040Communication.IsNotNull()
                            && ibusPersonAccount.ibusHb1040Communication.icdoHb1040Communication.person_account_id > 0
                            && ibusPersonAccount.ibusHb1040Communication.icdoHb1040Communication.person_account_id == ibusPersonAccount.icdoPersonAccount.person_account_id)
                        {
                            lclcBenefitOption.Add(lobjCodeValue.icdoCodeValue);
                        }
                    }
                    else
                        lclcBenefitOption.Add(lobjCodeValue.icdoCodeValue);
                }
            }
            lclcBenefitOption = busGlobalFunctions.Sort<cdoCodeValue>("code_value_order", lclcBenefitOption);
            return lclcBenefitOption;
        }

        //PIR 25920 DC 2025 Part 2 DBDC transfer new validation for special election just check the december contribution instead all 
        public bool IsDecemberContributionNotReceivedforMember()
        {
            decimal ldecTotalSalaryForPayPeriodDate = 0;
            DataTable ldtbEligibleWages = Select("entPersonAccountRetirementContribution.GetAcctIDAndSalaryForRetirementContByPayPeriod",
                                                new object[2] { new DateTime(2024, 12, 01), icdoBenefitApplication.member_person_id });
            foreach (DataRow dr in ldtbEligibleWages.Rows)
            {
                ldecTotalSalaryForPayPeriodDate += Convert.ToDecimal(dr["salary_amount"].ToString());
            }
            if (ldecTotalSalaryForPayPeriodDate == 0)
            {
                return true;
            }
            return false;
        }

    }
}
