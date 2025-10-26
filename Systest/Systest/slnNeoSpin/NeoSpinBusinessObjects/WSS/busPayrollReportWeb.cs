using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
using System.Collections;
using Sagitec.Common;
using System;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using System.Linq;

namespace NeoSpin.BusinessObjects
{

    [Serializable]
    public class busPayrollReportWeb : busExtendBase
    {
        public busPayrollReportWeb()
        {
            iblnNoMainCDO = true;
        }
        public string astrReportSelection { get; set; }
        public string astrBenefitType { get; set; }
        public string astrBenefitTypeVUPR { get; set; }
        public string astrBenefitTypeSCPR { get; set; }
        public string astrReportTypeValue { get; set; }
        public string astrReportTypeValueBonus { get; set; }
        public int aintOrgID { get; set; }
        public int aintContactID { get; set; }
        public DateTime adtPayPeriodDate { get; set; }
        public DateTime adtPayPeriodStartDate { get; set; }
        public DateTime  adtPayPeriodEndDate { get; set; }
       
        public DateTime adtPayCheckDate { get; set; }		
        public bool isNewClick { get; set; }
		//Added extra property
        public string astrIsDetailPopulatedRet { get; set; }

        public string astrIsDetailPopulated { get; set; }

        public string astrVideoPath { get; set; }

        public ArrayList iarrErrorList { get; set; }

        public Collection<cdoCodeValue> LoadEmployeeOptions()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(7001);
         
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            if (!IsEnrollin457ApplicableESS())
            {
                lclcCodeValue.Remove(lclcCodeValue.Where(OBJ=>OBJ.code_value=="OTRP").FirstOrDefault());
            }
            return lclcCodeValue;
        }
        public bool IsEnrollin457ApplicableESS()
        {
            busOrganization ibusOrganization = new busOrganization();
            if (ibusOrganization.FindOrganization(aintOrgID))
            {
                ibusOrganization.LoadOrgPlan();
                if (ibusOrganization.iclbOrgPlan.Where(o =>
               o.icdoOrgPlan.plan_id == busConstant.PlanIdOther457 &&
               busGlobalFunctions.CheckDateOverlapping(DateTime.Today, o.icdoOrgPlan.participation_start_date, o.icdoOrgPlan.participation_end_date)).Count() > 0)
                {
                    return true;
                }
            }
           
            return false;
        }
        public Collection<cdoPlan> GetPlanByHeaderTypeRetirement()
        {
            DataTable ldtbPlan = new DataTable();
            String lstrBenefitTypeValue;
            lstrBenefitTypeValue = busEmployerReportHelper.GetBenefitTypeForEmployerHeaderType(busConstant.PayrollHeaderBenefitTypeRtmt);
            ldtbPlan = Select<cdoPlan>(
                    new string[1] { "benefit_type_value" }, new object[1] { lstrBenefitTypeValue }, null, null);
            Collection<cdoPlan> lcdoPlan = new Collection<cdoPlan>();
            lcdoPlan.Add(new cdoPlan { plan_id = 0 });
            foreach (cdoPlan tcdoPlan in Sagitec.DataObjects.doBase.LoadData<cdoPlan>(ldtbPlan))
            {
                lcdoPlan.Add(tcdoPlan);
            }
            return lcdoPlan;
        }
        //PIR 13773 - Do not display ‘Create Payroll Report’ for employers where SGT_ORGANIZATION.CENTRAL_PAYROLL_FLAG = ‘Y’ on payroll report landing page
        public Collection<cdoCodeValue> LoadPayrollReportLandingOptions()
        {
            busOrganization lbusOrganization = new busOrganization();
            Collection<cdoCodeValue> lclcPayrollReportLandingOptions = new Collection<cdoCodeValue>();
            if (lbusOrganization.FindOrganization(aintOrgID))
            {
                 Collection<cdoCodeValue> lclcPayrollReportLandingCodeValues = GetCodeValue(7000);
                 foreach (cdoCodeValue lcdoCodeValue in lclcPayrollReportLandingCodeValues)
                 {
                     if (lcdoCodeValue.code_value == busConstant.ESSCreatePayrollReport &&
                         !string.IsNullOrEmpty(lbusOrganization.icdoOrganization.central_payroll_flag) &&
                         lbusOrganization.icdoOrganization.central_payroll_flag.ToUpper() == busConstant.Flag_Yes.ToUpper())
                         continue;
                     lclcPayrollReportLandingOptions.Add(lcdoCodeValue);
                 }
            }
            return lclcPayrollReportLandingOptions;
        }
        public ArrayList BusinessMethodPayrollReport()
        {
            ArrayList larrList = new ArrayList();
            iarrErrorList = new ArrayList();
            utlError lobjError = new utlError();
            bool isReportingMonthValid = true;
            if (astrReportSelection.IsNotNullOrEmpty())
            {
                if(astrReportSelection == "VUPR")
                {
                    if (astrBenefitTypeVUPR.IsNotNullOrEmpty())
                    {
                        larrList.Add(this);
                        return larrList;
                    }
                    else
                    {
                        lobjError = AddError(0,"Please Select Benefit Type.");
                        lobjError.istrFocusControl = "ddlDropDownVUPR";
                        iarrErrorList.Add(lobjError);
                        return iarrErrorList;
                    }
                }
                else if (astrReportSelection == "INPR")
                {                    
                    larrList.Add(this);
                    return larrList;                                       
                }
                else if(astrReportSelection == "CRPR")
                {
                    if (astrBenefitType.IsNotNullOrEmpty())
                    {
                        switch (astrBenefitType)
                        {
                            case "INSR":
                            case "RETR":
                                if(adtPayPeriodDate.IsNotNull() && adtPayPeriodDate.ToString() != "" && astrReportTypeValueBonus.IsNotNullOrEmpty() && IsReportingMonthValid(adtPayPeriodDate, isReportingMonthValid))
                                {
                                    if (astrReportTypeValueBonus == "REG")
                                    {
                                        string lstrGoliveDate = iobjPassInfo.isrvDBCache.GetConstantValue("PGLD");
                                        if (lstrGoliveDate != "")
                                        {
                                            DateTime ldtGoliveDate = Convert.ToDateTime(lstrGoliveDate);
                                            DateTime ldtReportingMonth;
                                            //DateTime ldtReportingMonth = Convert.ToDateTime(ltxtPayPeriod.Text);
                                            if (DateTime.TryParse(adtPayPeriodDate.ToString(), out ldtReportingMonth))
                                            {
                                                if (ldtGoliveDate > ldtReportingMonth)
                                                {
                                                    lobjError = AddError(0, "Regular Report creation with Reporting Month prior to Go Live date is not permitted.");
                                                    iarrErrorList.Add(lobjError);
                                                    return iarrErrorList;
                                                }
                                                else
                                                {
                                                    if (IsPayPeriodValid(true))
                                                    {
                                                        if (CheckHeaderDateContinuity(true))
                                                        {
                                                            isNewClick = true;
                                                            larrList.Add(this);
                                                            return larrList;
                                                            //ilblMessage.Visible = false;
                                                            //sfwButton btnNew = (sfwButton)GetControl(this, "btnNew");
                                                            //btnNew_Click(btnNew, e);
                                                        }
                                                        else
                                                        {
                                                            lobjError = AddError(0, "Pay Period is not starting from the end of last pay period date.");
                                                            lobjError.istrFocusControl = "txtPayPeriod";
                                                            iarrErrorList.Add(lobjError);
                                                            return iarrErrorList;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        lobjError = AddError(0, "Report for " + adtPayPeriodDate.ToString("MM/yyyy") + " already exists.  Please change Reporting Month");
                                                        lobjError.istrFocusControl = "txtPayPeriod";
                                                        iarrErrorList.Add(lobjError);
                                                        return iarrErrorList;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                lobjError = AddError(0, "Please Enter a Valid reporting month.");
                                                iarrErrorList.Add(lobjError);
                                                return iarrErrorList;
                                            }
                                        }
                                    }
                                    else if (astrReportTypeValueBonus == "ADJS")
                                    {
                                        if (CheckReportingMonthForAdjAndBonusRetro(adtPayPeriodDate.ToString(), true))
                                        {
                                            if (string.IsNullOrEmpty(astrIsDetailPopulatedRet))
                                            {
                                                lobjError = AddError(0, "Is This Adjustment For All Employees? Please Select Yes Or No.");
                                                lobjError.istrFocusControl = "ddlForAllDropDownListRet";
                                                iarrErrorList.Add(lobjError);
                                                return iarrErrorList;
                                                //ilblMessage.Visible = true;
                                                //ilblMessage.Text = "Is This Adjustment For All Employees? Please Select Yes Or No.";
                                                //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                            }

                                            else if (astrIsDetailPopulatedRet == "YES")
                                            {

                                                //ilblMessage.Visible = false;
                                                //sfwButton btnNew = (sfwButton)GetControl(this, "btnNew");
                                                //btnNew_Click(btnNew, e);
                                                isNewClick = true;
                                                larrList.Add(this);
                                                return larrList;
                                            }
                                            //for performance optimization, using separate form for no case
                                            else if (astrIsDetailPopulatedRet == "NO")
                                            {

                                                //ilblMessage.Visible = false;
                                                //sfwButton btnNoNew = (sfwButton)GetControl(this, "btnNoNew");
                                                //btnNew_Click(btnNoNew, e);
                                                isNewClick = false;
                                                larrList.Add(this);
                                                return larrList;
                                            }
                                        }
                                    }
                                    else if (astrReportTypeValueBonus == "BONS")
                                    {

                                        if (CheckReportingMonthForAdjAndBonusRetro(adtPayPeriodDate.ToString(), true))
                                        {
                                            if (string.IsNullOrEmpty(astrIsDetailPopulatedRet))
                                            {
                                                lobjError = AddError(0, "Is This Adjustment For All Employees? Please Select Yes Or No.");
                                                lobjError.istrFocusControl = "ddlForAllDropDownListRet";
                                                iarrErrorList.Add(lobjError);
                                                return iarrErrorList;
                                                //ilblMessage.Visible = true;
                                                //ilblMessage.Text = "Is This Adjustment For All Employees? Please Select Yes Or No.";
                                                //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                            }
                                            else if (astrIsDetailPopulatedRet == "YES")
                                            {
                                                astrReportTypeValue = "BONS";
                                                isNewClick = true;
                                                larrList.Add(this);
                                                return larrList;
                                                //if (lddlReportType.IsNotNull()) lddlReportType.SelectedValue = "BONS";
                                                //ilblMessage.Visible = false;
                                                //sfwButton btnNew = (sfwButton)GetControl(this, "btnNew");
                                                //btnNew_Click(btnNew, e);
                                            }
                                            //for performance optimization, using separate form for no case
                                            else if (astrIsDetailPopulatedRet == "NO")
                                            {
                                                astrReportTypeValue = "BONS";
                                                isNewClick = false;
                                                larrList.Add(this);
                                                return larrList;
                                                //if (lddlReportType.IsNotNull()) lddlReportType.SelectedValue = "BONS";
                                                //ilblMessage.Visible = false;
                                                //sfwButton btnNoNew = (sfwButton)GetControl(this, "btnNoNew");
                                                //btnNew_Click(btnNoNew, e);
                                            }
                                        }
                                    }                                                                                                            
                                }
                                else
                                {
                                    //ilblMessage.Visible = true;

                                    if (!string.IsNullOrEmpty(astrReportTypeValueBonus))
                                    {
                                        if ((string.IsNullOrEmpty(adtPayPeriodDate.ToString()) || adtPayPeriodDate.Equals(new DateTime())) && astrReportTypeValueBonus.IsNotNullOrEmpty())
                                        {
                                            lobjError = AddError(0, "Please Enter PayPeriod.");
                                            lobjError.istrFocusControl = "txtPayPeriod";
                                            iarrErrorList.Add(lobjError);
                                            //ilblMessage.Text = "Please Enter PayPeriod.";
                                        }

                                        if (adtPayPeriodDate != DateTime.MinValue && isReportingMonthValid)
                                        {
                                            lobjError = AddError(0, "Reporting Month is Invalid.");
                                            iarrErrorList.Add(lobjError);
                                            //ilblMessage.Text = "Reporting Month is Invalid.";
                                        }
                                    }
                                    else
                                    {
                                        lobjError = AddError(0, "Please Select Report Type.");
                                        lobjError.istrFocusControl = "ddlRetirementReportTypes";
                                        iarrErrorList.Add(lobjError);
                                        //ilblMessage.Text = "Please Select Report Type.";
                                    }
                                    return iarrErrorList;
                                    //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                }

                                break;
                            case "PRCH":
                                astrReportTypeValue = "REG";
                                isNewClick = true;
                                larrList.Add(this);
                                return larrList;
                                //ilblMessage.Visible = false;
                                //sfwButton btnNew1 = (sfwButton)GetControl(this, "btnNew");
                                //btnNew_Click(btnNew1, e);
                                //}
                                //else
                                //{
                                //    ilblMessage.Visible = true;
                                //    ilblMessage.Text = "Please Select Report Type.";
                                //    ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                //}
                                break;
                            case "DEFF":
                                //string aa = adtPayCheckDate.ToString();
                                //if(aa=="1/1/0001")
                                //{
                                //    aa = null;
                                //}

                               // if (adtPayCheckDate.ToString().IsNotNullOrEmpty() && (adtPayPeriodEndDate.ToString().IsNotNullOrEmpty() && adtPayPeriodStartDate.ToString().IsNotNullOrEmpty())
                              //      && astrReportTypeValue.IsNotNullOrEmpty())

                                    if (adtPayCheckDate != DateTime.MinValue && adtPayPeriodEndDate != DateTime.MinValue && adtPayPeriodStartDate  != DateTime.MinValue
                                    && astrReportTypeValue.IsNotNullOrEmpty())
                                {

                                    adtPayPeriodDate = new DateTime();

                                    String strErrorMessage = "";

                                    //PIR 14154  landing page validations for deferred comp. 
                                    if (ValidatePayPeriodDates(adtPayPeriodStartDate, adtPayPeriodEndDate, out strErrorMessage))
                                    {
                                        if (IsStartDateIsGreaterThanEndDate())
                                        {
                                            //PIR 13892 - The Pay Check Date should be within 30 days of the Pay Period End Date.  So it could be 30 days prior or after the Pay Period End Date - Maik Mail dated March 11, 2015
                                            if (Math.Abs(busGlobalFunctions.DateDiffInDays(Convert.ToDateTime(adtPayPeriodEndDate.ToString()), Convert.ToDateTime(adtPayCheckDate.ToString()))) <= 30)
                                            {

                                                if (astrReportTypeValue == "REG")
                                                {
                                                    //PIR 13944 - Pay Period date is compared to GoLiveDate
                                                    string lstrGoliveDate = iobjPassInfo.isrvDBCache.GetConstantValue("PGLD");
                                                    if (lstrGoliveDate != "")
                                                    {
                                                        DateTime ldtGoliveDate = Convert.ToDateTime(lstrGoliveDate);
                                                        DateTime ldtRPayPeriodStartDate;
                                                        //DateTime ldtReportingMonth = Convert.ToDateTime(ltxtPayPeriod.Text);
                                                        if (DateTime.TryParse(adtPayPeriodStartDate.ToString(), out ldtRPayPeriodStartDate))
                                                        {
                                                            if (ldtGoliveDate > ldtRPayPeriodStartDate)
                                                            {
                                                                lobjError = AddError(0, "Regular Report creation with Pay Period Date prior to Go Live date is not permitted.");
                                                                iarrErrorList.Add(lobjError);
                                                                return iarrErrorList;
                                                                //ilblMessage.Visible = true;
                                                                //ilblMessage.Text = "Regular Report creation with Pay Period Date prior to Go Live date is not permitted.";
                                                                //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                                            }
                                                            else
                                                            {
                                                                if (IsPayPeriodValid(false))
                                                                {
                                                                    if (CheckHeaderDateContinuity(false))
                                                                    {
                                                                        isNewClick = true;
                                                                        larrList.Add(this);
                                                                        return larrList;
                                                                        //sfwButton btnNew = (sfwButton)GetControl(this, "btnNew");
                                                                        //btnNew_Click(btnNew, e);
                                                                    }
                                                                    else
                                                                    {
                                                                        lobjError = AddError(0, "Pay Period is not starting from the end of last pay period date.");
                                                                        iarrErrorList.Add(lobjError);
                                                                        return iarrErrorList;
                                                                        //ilblMessage.Visible = true;
                                                                        //ilblMessage.Text = "Pay Period is not starting from the end of last pay period date.";
                                                                        //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    lobjError = AddError(0, "Report for " + adtPayPeriodStartDate.ToString() + " and " + adtPayPeriodEndDate.ToString() + " already exists.  Please change dates");
                                                                    lobjError.istrFocusControl = "Empty";
                                                                    lobjError.istrFocusControls = "txtAdtPayPeriodStartDate;txtAdtPayPeriodEndDate";
                                                                    iarrErrorList.Add(lobjError);
                                                                    return iarrErrorList;
                                                                    //ilblMessage.Visible = true;
                                                                    //ilblMessage.Text = "Report for " + ltxtAdtPayPeriodStartDate.Text + " and " + ltxtAdtPayPeriodEndDate.Text + " already exists.  Please change dates";
                                                                    //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                                                }

                                                            }
                                                        }
                                                        else
                                                        {
                                                            lobjError = AddError(0, "Please Enter a Valid Pay Period Date.");
                                                            iarrErrorList.Add(lobjError);
                                                            return iarrErrorList;
                                                            //ilblMessage.Visible = true;
                                                            //ilblMessage.Text = "Please Enter a Valid Pay Period Date.";
                                                            //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                                        }
                                                    }
                                                }
                                                else if (astrReportTypeValue == "ADJS")
                                                {
                                                    //PIR 13944  - Pay PEriod Date
                                                    if (CheckReportingMonthForAdjAndBonusRetro(adtPayPeriodStartDate.ToString(), false))
                                                    {
                                                        if (string.IsNullOrEmpty(astrIsDetailPopulated))
                                                        {
                                                            lobjError = AddError(0, "Is This Adjustment For All Employees? Please Select Yes Or No.");
                                                            lobjError.istrFocusControl = "ddlForAllDropDownList";
                                                            iarrErrorList.Add(lobjError);
                                                            return iarrErrorList;
                                                            //ilblMessage.Visible = true;
                                                            //ilblMessage.Text = "Is This Adjustment For All Employees? Please Select Yes Or No.";
                                                            //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                                        }
                                                        else if (astrIsDetailPopulated == "YES")
                                                        {
                                                            isNewClick = true;
                                                            larrList.Add(this);
                                                            return larrList;
                                                            //ilblMessage.Visible = false;
                                                            //sfwButton btnNew = (sfwButton)GetControl(this, "btnNew");
                                                            //btnNew_Click(btnNew, e);
                                                        }
                                                        //for performance optimization, using separate form for no case
                                                        else if (astrIsDetailPopulated == "NO")
                                                        {
                                                            isNewClick = false;
                                                            larrList.Add(this);
                                                            return larrList;
                                                            //ilblMessage.Visible = false;
                                                            //sfwButton btnNoNew = (sfwButton)GetControl(this, "btnNoNew");
                                                            //btnNew_Click(btnNoNew, e);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                lobjError = AddError(0, "The Pay Check Date should be within 30 days of the Pay Period End Date.");


                                                iarrErrorList.Add(lobjError);

                                                lobjError.istrFocusControl = "txtAdtPayCheckDate";
                                                return iarrErrorList;
                                                //ilblMessage.Visible = true;
                                                //ilblMessage.Text = "The Pay Check Date should be within 30 days of the Pay Period End Date.";
                                                //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                            }
                                        }
                                        else
                                        {
                                            lobjError = AddError(0, "Start date cannot be greater than end date.");
                                            iarrErrorList.Add(lobjError);
                                            return iarrErrorList;
                                            //ilblMessage.Visible = true;
                                            //ilblMessage.Text = "Start date cannot be greater than end date.";
                                            //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                        }
                                    }
                                    else
                                    {
                                        lobjError = AddError(0, strErrorMessage);
                                        lobjError.istrFocusControl = "Empty";
                                        lobjError.istrFocusControls = "txtAdtPayPeriodStartDate;txtAdtPayPeriodEndDate";
                                        iarrErrorList.Add(lobjError);
                                        return iarrErrorList;
                                        //ilblMessage.Visible = true;
                                        //ilblMessage.Text = strErrorMessage;
                                        //ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";

                                    }
                                }

                                else
                                {
                                    //ilblMessage.Visible = true;
                                                                         
                                    //ilblMessage.Text = "Please Enter Pay Period End Date";
                                    if (adtPayPeriodStartDate == DateTime.MinValue)
                                    {
                                        lobjError = AddError(0, "Please Enter Pay Period Start Date");
                                        lobjError.istrFocusControl = "txtAdtPayPeriodStartDate";
                                        iarrErrorList.Add(lobjError);
                                    }
                                    if (adtPayPeriodEndDate == DateTime.MinValue)
                                    {
                                        lobjError = AddError(0, "Please Enter Pay Period End Date");
                                        lobjError.istrFocusControl = "txtAdtPayPeriodEndDate";
                                        iarrErrorList.Add(lobjError);
                                    } 
                                    //ilblMessage.Text = "Please Enter Pay Period Start Date";
                                    if (adtPayCheckDate == DateTime.MinValue)
                                    {
                                        lobjError = AddError(0, "Please Enter Pay Check Date");
                                        lobjError.istrFocusControl = "txtAdtPayCheckDate";
                                        iarrErrorList.Add(lobjError);
                                    }
                                        //ilblMessage.Text = "Please Enter Pay Check Date";
                                    if (astrReportTypeValue.IsNullOrEmpty())
                                    {
                                        lobjError = AddError(0, "Please Select Report Type.");
                                        lobjError.istrFocusControl = "ddlReportType";
                                        iarrErrorList.Add(lobjError);
                                    }
                                    return iarrErrorList;
                                    //ilblMessage.Text = "Please Select Report Type.";
                                    // ilblMessage.ForeColor = System.Drawing.Color.Red; ilblMessageId.Text = "MSSMsg";
                                }
                                break;
                        }
                    }
                    else
                    {
                        lobjError = AddError(0, "Please Select Benefit Type.");
                        lobjError.istrFocusControl = "ddlBenefitType";
                        iarrErrorList.Add(lobjError);
                        return iarrErrorList;
                    }
                }
                else if (astrReportSelection == "SCPR")
                {
                    if (astrBenefitTypeSCPR.IsNotNullOrEmpty())
                    {
                        larrList.Add(this);
                        return larrList;
                    }
                    else
                    {
                        lobjError = AddError(0, "Please Select Benefit Type.");
                        lobjError.istrFocusControl = "ddlBenefitTypeSearchDetail";
                        iarrErrorList.Add(lobjError);
                        return iarrErrorList;
                    }
                }
            }
            else if (string.IsNullOrEmpty(astrReportTypeValue))
            {
                lobjError = AddError(0, "Please Select Any Option.");
                lobjError.istrFocusControl = "ddlPayrollReport";
                iarrErrorList.Add(lobjError);
                return iarrErrorList;

            }

            return larrList;
        }
        private Boolean IsReportingMonthValid(DateTime strReportingMonth,bool isReportingMonthValid)
        {

            //string[] strdates = strReportingMonth.Split("/");

            int month = 0;


            int year = 0;

            // For some cases the Reporting month field is read with the remaining underscores ("_")
            //so addidng below check ex "01/111_"

            //if (strdates[0].IsNotNullOrEmpty() && !strdates[0].Contains("_"))
            //{
            //    month = Convert.ToInt32(strdates[0]);
            //}

            //if (strdates[1].IsNotNullOrEmpty() && !strdates[1].Contains("_"))
            //{
            //    year = Convert.ToInt32(strdates[1]);
            //}
            if (strReportingMonth.IsNotNull())
            {
                month = strReportingMonth.Month;

                year = strReportingMonth.Year;
            }       


            if ((month >= 1 && month <= 12) && (year >= 1901 && year <= 2100))
            {
                isReportingMonthValid = true;
                return true;
            }

            isReportingMonthValid = false;
            return false;
        }

        public bool IsPayPeriodValid(bool IsRetr)
        {
            if (IsRetr)
            {
                astrReportTypeValue = astrReportTypeValueBonus;
            }
            else
            {
                astrReportTypeValue = astrReportTypeValue;
            }
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader { org_id = aintOrgID, header_type_value = astrBenefitType, report_type_value = astrReportTypeValue, pay_period = adtPayPeriodDate.ToString(), pay_period_start_date = adtPayPeriodStartDate, pay_period_end_date = adtPayPeriodEndDate } };
            if (astrBenefitType != busConstant.PlanBenefitTypeDeferredComp)
                return lbusEmployerPayrollHeader.IsPayPeriodValid();
            else
                return lbusEmployerPayrollHeader.IsPayPeriodValidForDefComp();
        }

        public bool CheckHeaderDateContinuity(bool IsRetr)
        {
            if (IsRetr)
            {
                astrReportTypeValue = astrReportTypeValueBonus;
            }
            else
            {
                astrReportTypeValue = astrReportTypeValue;
            }
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader { org_id = aintOrgID, header_type_value = astrBenefitType, report_type_value = astrReportTypeValue, pay_period = adtPayPeriodDate.ToString(), payroll_paid_date = adtPayPeriodDate, pay_period_start_date = adtPayPeriodStartDate, pay_period_end_date = adtPayPeriodEndDate } };
            return lbusEmployerPayrollHeader.CheckHeaderDateContinuity();
        }
        private Boolean CheckReportingMonthForAdjAndBonusRetro(String strReportingMonth, Boolean blnIsRetirement)
        {

            DateTime ldtReportingMonth;
            utlError lobjError = new utlError();
            //DateTime ldtReportingMonth = Convert.ToDateTime(ltxtPayPeriod.Text);
            if (DateTime.TryParse(strReportingMonth, out ldtReportingMonth))
            {

                if (ldtReportingMonth < new DateTime(1977, 01, 01))
                {
                    //ilblMessage.Visible = true;
                    if (blnIsRetirement)
                    {
                        lobjError = AddError(0, "Reporting month should be greater than or equal to 01/1977");
                        iarrErrorList.Add(lobjError);
                        //return iarrErrorList;
                        //ilblMessage.Text = "Reporting month should be greater than or equal to 01/1977";
                    }
                    else
                    {
                        lobjError = AddError(0, "Pay Period Date should be greater than or equal to 01/1977");
                        iarrErrorList.Add(lobjError);
                        //ilblMessage.Text = "Pay Period Date should be greater than or equal to 01/1977";
                    }

                    return false;
                }
            }
            return true;

        }
        private Boolean ValidatePayPeriodDates(DateTime strPayPeriodStartDate, DateTime strPayPeriodEndDate, out String strErrorMessage)
        {

            strErrorMessage = "";

            if (!ValidateDates(strPayPeriodStartDate))
            {
                strErrorMessage = "Please Enter Valid Pay Period Start Date";
                return false;
            }

            if (!ValidateDates(strPayPeriodEndDate))
            {
                strErrorMessage = "Please Enter Valid Pay Period End Date";
                return false;
            }
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader { org_id = aintOrgID, header_type_value = astrBenefitType, report_type_value = astrReportSelection, pay_period = adtPayPeriodDate.ToString(), payroll_paid_date = adtPayPeriodDate, pay_period_start_date = adtPayPeriodStartDate, pay_period_end_date = adtPayPeriodEndDate } };
            string lstrFrequency = lbusEmployerPayrollHeader.GetDeferredCompFrequencyPeriodByOrg(aintOrgID);
            //PIR 19701 Day_Of_Month from Org Plan Maintenance to be used for verify validation 4690
            int lintDayofMonth = 1;
            if (lstrFrequency == busConstant.DeffCompFrequencyMonthly || lstrFrequency == busConstant.DeffCompFrequencySemiMonthly)
                lintDayofMonth = lbusEmployerPayrollHeader.GetDayOfMonthForMonthlyFrequency(Convert.ToDateTime(strPayPeriodStartDate), Convert.ToDateTime(strPayPeriodEndDate));
            DateTime ldtEndDateLastPaid = busEmployerReportHelper.GetEndDateByReportFrequency(Convert.ToDateTime(strPayPeriodStartDate), lstrFrequency, lintDayofMonth);


            if (ldtEndDateLastPaid != Convert.ToDateTime(strPayPeriodEndDate))
            {
                strErrorMessage = "Start Date and End Should match as per frequency reported by the employer in their Deferred Compensation Plan Participation.";
                return false;
            }

            return true;
        }
        private Boolean ValidateDates(DateTime strPayPeriodDate)
        {

            //String[] strdates = strPayPeriodDate.Split("/");
            Int32 year = 0, month = 0, day = 0;

            Boolean isleap = false;

            // For some cases the Reporting month field is read with the remaining underscores ("_")
            //so addidng below check ex "01/111_"

            //if (strdates[0].IsNotNullOrEmpty() && !strdates[0].Contains("_"))
            //{
            //    month = Convert.ToInt32(strdates[0]);
            //}

            //if (strdates[1].IsNotNullOrEmpty() && !strdates[1].Contains("_"))
            //{
            //    day = Convert.ToInt32(strdates[1]);

            //}
            //if (strdates[2].IsNotNullOrEmpty() && !strdates[2].Contains("_"))
            //{
            //    year = Convert.ToInt32(strdates[2]);
            //}
            if (strPayPeriodDate.IsNotNull())
            {
                month = strPayPeriodDate.Month;

                day = strPayPeriodDate.Day;

                year = strPayPeriodDate.Year;
            }            

            isleap = DateTime.IsLeapYear(year);

            if ((month < 1 || month > 12) || (day < 1 || day > 31) || (year < 1901 || year > 2100))
            {
                return false;
            }

            if ((month == 4 || month == 6 || month == 9 || month == 11) && day == 31)
            {
                return false;
            }

            if (month == 2)
            {

                if (day > 29 || (day == 29 && !isleap))
                {
                    return false;
                }
            }

            return true;
        }
        public bool IsStartDateIsGreaterThanEndDate()
        {
            DateTime PayPeriodBeginDate = !string.IsNullOrEmpty(adtPayPeriodStartDate.ToString()) ? Convert.ToDateTime(adtPayPeriodStartDate.ToString()) : DateTime.MinValue;
            DateTime PayPeriodEndDate = !string.IsNullOrEmpty(adtPayPeriodEndDate.ToString()) ? Convert.ToDateTime(adtPayPeriodEndDate.ToString()) : DateTime.MinValue;
            return PayPeriodBeginDate < PayPeriodEndDate;
        }
    }
}
