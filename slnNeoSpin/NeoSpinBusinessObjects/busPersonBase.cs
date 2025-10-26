using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Reflection;
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonBase : busExtendBase
    {
        /// <summary>
        /// Determines the Age from given Start date to End Date.
        /// Referred to Total months as int and Total Month and Years as Decimal value.
        /// </summary>
        public static void CalculateAge(DateTime adteFromDate, DateTime adteToDate, ref int aintMonths, ref decimal adecMonthAndYear, int aintDecimallength,
            ref int aintMemberAgeYear, ref int aintMemberAgeMonths) 
        {
            int lintYears, lintMonths;
            adteFromDate = new DateTime(adteFromDate.Year, adteFromDate.Month, 01);
            adteToDate = new DateTime(adteToDate.Year, adteToDate.Month, 01);
            aintMonths = HelperFunction.GetMonthSpan(adteFromDate, adteToDate, out lintYears, out lintMonths);
            decimal ldecMonths = Convert.ToDecimal(lintMonths);
            adecMonthAndYear = Convert.ToDecimal(lintYears) + (ldecMonths / 12);
            adecMonthAndYear = Math.Round(adecMonthAndYear, aintDecimallength, MidpointRounding.AwayFromZero);

            aintMemberAgeMonths = lintMonths;
            aintMemberAgeYear = lintYears;
        }

        /// <summary>
        /// Returns the value after slicing to the given no. of decimals without any rounding.
        /// </summary>
        public static decimal Slice(decimal adecValue, int aintNoOfDecimals)
        {
            string lstrResult = string.Empty;
            decimal ldecResult = 0.0M;
            string[] lstrarr = Convert.ToString(adecValue).Split(new char[1] { '.' });
            if (lstrarr.Length > 1)
            {
                if (lstrarr[1].Length > aintNoOfDecimals)
                {
                    lstrarr[1] = lstrarr[1].Substring(0, aintNoOfDecimals);
                }
                lstrResult = lstrarr[0] + "." + lstrarr[1];
                ldecResult = Convert.ToDecimal(lstrResult);
                return ldecResult;
            }
            return adecValue;
        }

        /// <summary>
        ///  Returns the value after slicing to 2 units of decimals without any rounding
        /// </summary>
        public static decimal Slice(decimal adecValue)
        {
            return Slice(adecValue, 2);
        }

        public static decimal CalculateStandardRHICAmount(decimal adecPSCinYears, decimal adecRateinEffect, int aintPlanID)
        {
            decimal ldecRHICAmount = adecPSCinYears * adecRateinEffect;
            //if (aintPlanID == busConstant.PlanIdJobService)
            //    return Slice(ldecRHICAmount, 2);
            //else
            return Math.Round(ldecRHICAmount, 2, MidpointRounding.AwayFromZero);
        }

        #region Getting Salary Records

        /// PIR 20229 - Loads the months Salary Records for FAS2020 logic
        /// </summary>
        /// <param name="adteStartDate"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="aintPlanID"></param>
        /// <param name="ablnIsMemberRTW"></param>
        /// <param name="aintPersonAccountID"></param>
        /// <returns></returns>
        public Collection<busPersonAccountRetirementContribution> GetSalaryRecordsFAS2020(DateTime adteStartDate, int aintPersonID, int aintPlanID, int aintRTWPersonAccountID,
                                                                                                int aintPersonAccountID = 0, bool ablnIsMemberRTW = false)
        {
            Collection<busPersonAccountRetirementContribution> lclbRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            // Load all the Service Contribution by given Person Account ID and Retirement Date 
            DataTable ldtbResult;
            int lintPayPeriodMonth = 0, lintPayPeriodYear = 0;

            if (ablnIsMemberRTW)
            {
                ldtbResult = busBase.Select("cdoBenefitCalculationFasMonths.LoadFASSalaryRecords2020RTW",
                                                       new object[4] { aintPersonID, aintPlanID, adteStartDate, aintPersonAccountID });
            }
            else
            {
                ldtbResult = busBase.Select("cdoBenefitCalculationFasMonths.LoadFASSalaryRecords2020",
                                                        new object[5] { aintPersonID, aintPlanID, aintRTWPersonAccountID, adteStartDate, aintPersonAccountID });
            }

            foreach (DataRow drContribution in ldtbResult.Rows)
            {
                busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
                lobjRetirementContribution.ibusPARetirement = new busPersonAccountRetirement();
                lobjRetirementContribution.ibusPARetirement.icdoPersonAccount = new cdoPersonAccount();
                lobjRetirementContribution.ibusPARetirement.ibusPlan = new busPlan();
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan = new cdoPlan();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.LoadData(drContribution);
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan.LoadData(drContribution);

                if (lintPayPeriodMonth == lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month &&
                    lintPayPeriodYear == lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year)
                {
                    lclbRetirementContribution[lclbRetirementContribution.Count - 1].icdoPersonAccountRetirementContribution.salary_amount += lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount;
                    lclbRetirementContribution[lclbRetirementContribution.Count - 1].ibusPARetirement.ibusPlan.icdoPlan.plan_name = string.Empty;
                }
                else
                {
                    lclbRetirementContribution.Add(lobjRetirementContribution);
                }

                lintPayPeriodMonth = lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month;
                lintPayPeriodYear = lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year;

                if (lclbRetirementContribution.Count == 180)
                    break;
            }

            return lclbRetirementContribution;
        }


        /// <summary>
        /// PIR 20229 - Get top 3 non overlapping date salary record blocks for FAS 2020 logic
        /// </summary>
        /// <param name="adteStartDate"></param>
        /// <param name="aintPersonID"></param>
        /// <param name="aintPlanID"></param>
        /// <param name="ablnIsMemberRTW"></param>
        /// <param name="aintPersonAccountID"></param>
        /// <returns></returns>
        public Collection<busPersonAccountRetirementContribution> GetSalaryRecordBlocksFAS2020(Collection<busPersonAccountRetirementContribution> lclbRetirementContribution)
        {
            Collection<busPersonAccountRetirementContribution> lclbRetirementContributionUpdated = new Collection<busPersonAccountRetirementContribution>();
            lclbRetirementContribution = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>("icdoPersonAccountRetirementContribution.pay_period_year desc,icdoPersonAccountRetirementContribution.pay_period_month desc", lclbRetirementContribution);
            //If salary records are less than or equal to 36 months then average salary with non zero salary records count.
            //Calculates average salary in CalculateFAS2020 function.
            if (lclbRetirementContribution.Count <= 36)
            {
                return lclbRetirementContribution;
            }
            //Declare Datatable to store start and end date of each blocks with salary and block no.
            DataTable ldtbBlockDetails = new DataTable();
            DataColumn ldc1 = new DataColumn("StartDate", Type.GetType("System.DateTime"));
            DataColumn ldc2 = new DataColumn("EndDate", Type.GetType("System.DateTime"));
            DataColumn ldc3 = new DataColumn("AvgSalary", Type.GetType("System.Decimal"));
            DataColumn ldc4 = new DataColumn("BlockNo", Type.GetType("System.Int32"));
            ldtbBlockDetails.Columns.Add(ldc1);
            ldtbBlockDetails.Columns.Add(ldc2);
            ldtbBlockDetails.Columns.Add(ldc3);
            ldtbBlockDetails.Columns.Add(ldc4);

            int lintRetirementContributionCnt = lclbRetirementContribution.Count > 180 ? 180 : lclbRetirementContribution.Count;
            int lintSalaryIterationCount = 12;
            int lintRecordCnt = 0, lintBlockNo = 1;
            decimal ldecAvgSalary = 0.0M;

            
            //If salary records are greater than 36 then do following block logic to get top 3 non overlapping date salary record blocks
            //Repeat following 12 record block logic on lclbRetirementContribution till lintSalaryIterationCount
            for (int i = 0; i < lintSalaryIterationCount; i++)
            {
                int lintBlockCnt = 0, lintZeroCnt = 0, lintSalaryBlockCount = 0;
                lintSalaryBlockCount = (lintRetirementContributionCnt - i) / 12;
                DateTime adtEndDate = new DateTime();
                for (int j = i; j < lintRetirementContributionCnt; j++)
                {
                    ldecAvgSalary = ldecAvgSalary + Convert.ToDecimal(lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.salary_amount);
                    lintZeroCnt = Convert.ToDecimal(lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.salary_amount) == 0 ? lintZeroCnt + 1 : lintZeroCnt;
                    //Add block records to list
                    busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
                    lobjRetirementContribution.ibusPARetirement = lclbRetirementContribution[j].ibusPARetirement;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.person_account_id;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month = lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.pay_period_month;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year = lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.pay_period_year;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount = lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.salary_amount;
                    lobjRetirementContribution.ibnlIsProjectedSalaryRecord = lclbRetirementContribution[j].ibnlIsProjectedSalaryRecord;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.BlockNo = lintBlockNo;

                    if (lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.effective_date == null ||
                        lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.effective_date == DateTime.MinValue)
                    {
                        DateTime ldteEffective_date = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                                       lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month, 01);
                        lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = ldteEffective_date;
                    }
                    else
                    {
                        lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.effective_date;
                    }

                    lclbRetirementContributionUpdated.Add(lobjRetirementContribution);

                    if (lintRecordCnt == 0)
                    {
                        adtEndDate = new DateTime(Convert.ToInt32(lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.pay_period_year),
                                                    Convert.ToInt32(lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.pay_period_month), 1).Date.GetLastDayofMonth();
                    }
                    if (lintRecordCnt == 11)
                    {
                        DateTime adtStartDate = new DateTime(Convert.ToInt32(lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.pay_period_year),
                                                             Convert.ToInt32(lclbRetirementContribution[j].icdoPersonAccountRetirementContribution.pay_period_month), 1).Date;
                        //fill data table
                        DataRow dr = ldtbBlockDetails.NewRow();
                        dr["StartDate"] = adtStartDate;
                        dr["EndDate"] = adtEndDate.GetLastDayofMonth();
                        dr["AvgSalary"] = lintZeroCnt == 12 ? 0 : ldecAvgSalary / (12 - lintZeroCnt);  //Average salary with non zero salary record cnt
                        dr["BlockNo"] = lintBlockNo;
                        ldtbBlockDetails.Rows.Add(dr);
                        adtStartDate = new DateTime();
                        ldecAvgSalary = 0.0M;
                        lintZeroCnt = 0;
                        lintRecordCnt = 0;
                        lintBlockCnt++;
                        lintBlockNo++;
                        //If BlockCnt is equal to SalaryBlockCount then terminate current iteration then start next iteration.
                        if (lintBlockCnt == lintSalaryBlockCount)
                            break;
                    }
                    else
                    {
                        lintRecordCnt++;
                    }
                }
            }

            //select top 3 highest Salary blocks and it should be non overlapping date blocks
            ldtbBlockDetails.DefaultView.Sort = "AvgSalary DESC,EndDate DESC"; //PIR 21264 - FAS calculation  
            ldtbBlockDetails = ldtbBlockDetails.DefaultView.ToTable();
            DataTable ldtbFinalSalaryRecords = ldtbBlockDetails.Clone();

            bool lblnIsValidDate = false;
            decimal ldecSalary = 0.00M;
            foreach (DataRow item in ldtbBlockDetails.Rows)
            {
                DateTime ldtStartDate = Convert.ToDateTime(item["StartDate"]).Date;
                DateTime ldtEndDate = Convert.ToDateTime(item["EndDate"]).Date;

                //Once ldtbFinalSalaryRecords have item then check date overlapping.  
                for (int i = 0; i < ldtbFinalSalaryRecords.Rows.Count; i++)
                {
                    DateTime ldtStartDateFinal = Convert.ToDateTime(ldtbFinalSalaryRecords.Rows[i]["StartDate"]).Date;
                    DateTime ldtEndDateFinal = Convert.ToDateTime(ldtbFinalSalaryRecords.Rows[i]["EndDate"]).Date;
                    //check date overlapping of blocks
                    if (busGlobalFunctions.CheckDateOverlapping(ldtStartDate, ldtStartDateFinal, ldtEndDateFinal) ||
                        busGlobalFunctions.CheckDateOverlapping(ldtEndDate, ldtStartDateFinal, ldtEndDateFinal))
                    {
                        lblnIsValidDate = false;
                        break;
                    }
                    else
                    {
                        lblnIsValidDate = true;
                    }
                }

                //Always add first highest salary block or Add next highest salary block if IsValiddate is true.
                if (ldtbFinalSalaryRecords.Rows.Count == 0 || lblnIsValidDate)
                {
                    ldtbFinalSalaryRecords.ImportRow(item);
                    ldecSalary += Convert.ToDecimal(item["AvgSalary"]);
                }
                //once we get top three salary blocks then come out of loop.
                if (ldtbFinalSalaryRecords.Rows.Count == 3)
                {
                    ldecSalary = Math.Round(ldecSalary / ldtbFinalSalaryRecords.Rows.Count, 2);
                    break;
                }
            }

            //Select top 3 blocks salary records from salary collection.
            List<int> lstBlockNos = new List<int>();
            lstBlockNos = ldtbFinalSalaryRecords.AsEnumerable().Select(x => x.Field<Int32>("BlockNo")).ToList();
            lclbRetirementContribution = lclbRetirementContributionUpdated.Where(t => lstBlockNos.Contains(t.icdoPersonAccountRetirementContribution.BlockNo)).ToList<busPersonAccountRetirementContribution>().ToCollection<busPersonAccountRetirementContribution>();
            return lclbRetirementContribution;
        }

        /// <summary>
        /// <summary>
        /// Loads the months Salary Records
        /// </summary>
        /// <param name="aintPersonAccountID">Person Account ID</param>
        /// <param name="adteRetirementDate">Retirement Date</param>
        /// <returns>Salary Records</returns>
        public static Collection<busPersonAccountRetirementContribution> GetSalaryRecords(DateTime adteStartDate, int aintPersonID, int aintPlanID,
                                                                            int aintFASMonths, int aintFASPeriodRange, int aintRTWPersonAccountID, 
                                                                            bool ablnIsMemberRTW = false, int aintPersonAccountID = 0)
        {
            Collection<busPersonAccountRetirementContribution> lclbRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            //PIR 18053
            DataTable ldtbResult;
            if (ablnIsMemberRTW)
                ldtbResult = busBase.Select("cdoBenefitCalculationFasMonths.LoadFASSalaryRecordsRTW",
                                           new object[6] { aintFASMonths, aintPersonID, aintPlanID, aintPersonAccountID, aintFASPeriodRange, adteStartDate });
            else
                ldtbResult = busBase.Select("cdoBenefitCalculationFasMonths.LoadFASSalaryRecords",
                                           new object[6] { aintFASMonths, aintPersonID, aintPlanID, aintRTWPersonAccountID, aintFASPeriodRange, adteStartDate });

            foreach (DataRow drContribution in ldtbResult.Rows)
            {
                busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
                lobjRetirementContribution.ibusPARetirement = new busPersonAccountRetirement();
                lobjRetirementContribution.ibusPARetirement.icdoPersonAccount = new cdoPersonAccount();
                lobjRetirementContribution.ibusPARetirement.ibusPlan = new busPlan();
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan = new cdoPlan();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.LoadData(drContribution);
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan.LoadData(drContribution);
                lclbRetirementContribution.Add(lobjRetirementContribution);
            }
            return lclbRetirementContribution;
        }

        /// <summary>
        /// Loads the highest consecutive Job Service Salary Records for the given Person Account and Retirement Date
        /// </summary>
        /// <param name="aintPersonAccountID">Person Account ID</param>
        /// <param name="adteRetirementDate">Retirement Date</param>
        /// <returns>Job Service Salary Records</returns>
        public static Collection<busPersonAccountRetirementContribution> GetJobServiceSalaryRecords(DateTime adteStartDate, int aintPersonID, int aintPlanID,
                                            string strCalculationType, int aintSalaryMonthIncrease, decimal adecPercentageSalaryIncrease,
                                            DateTime adteTerminationDate, int aintFASConsecutiveMonths, int aintRTWPersonAccountID, utlPassInfo aobjPassInfo,
                                            bool ablnIsMemberRTW = false, int aintPersonAccountID = 0)
        {
            // 1. Load all the Job Service Contribution by given Person Account ID and Retirement Date
            Collection<busPersonAccountRetirementContribution> lclbJobServiceSalaryRecords = new Collection<busPersonAccountRetirementContribution>();

            //PIR 18053 - Load Salary of the latest employment for RTW
            DataTable ldtbResult;

            if (ablnIsMemberRTW)
            {
                ldtbResult = busBase.Select("cdoBenefitCalculationFasMonths.LoadJobServiceSalaryRecordsRTW",
                                                        new object[4] { aintPersonID, aintPlanID, adteStartDate, aintPersonAccountID });
            }
            else
            {
                ldtbResult = busBase.Select("cdoBenefitCalculationFasMonths.LoadJobServiceSalaryRecords",
                                                        new object[4] { aintPersonID, aintPlanID, adteStartDate, aintRTWPersonAccountID });
            }
            foreach (DataRow ldtrContribution in ldtbResult.Rows)
            {
                busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
                lobjRetirementContribution.ibusPARetirement = new busPersonAccountRetirement();
                lobjRetirementContribution.ibusPARetirement.icdoPersonAccount = new cdoPersonAccount();
                lobjRetirementContribution.ibusPARetirement.ibusPlan = new busPlan();
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan = new cdoPlan();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.LoadData(ldtrContribution);
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan.LoadData(ldtrContribution);
                lclbJobServiceSalaryRecords.Add(lobjRetirementContribution);
            }

            if (strCalculationType == busConstant.CalculationTypeEstimate || strCalculationType == busConstant.CalculationTypeEstimateSubsequent)  //PIR 19594
            {
                Collection<busPersonAccountRetirementContribution> lclbProjectedSalaryRecords = new Collection<busPersonAccountRetirementContribution>();
                lclbProjectedSalaryRecords = GetProjectedSalaryRecords(aintPersonID, aintPlanID, adteTerminationDate, aintSalaryMonthIncrease,
                                                                            adecPercentageSalaryIncrease, aobjPassInfo);
                foreach (busPersonAccountRetirementContribution lobjProjSalaryRecord in lclbProjectedSalaryRecords)
                {
                    lclbJobServiceSalaryRecords.Add(lobjProjSalaryRecord);
                }
            }
            // 2. Load the 36 Consecutive Salary amount as Total Salary amount
            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lclbJobServiceSalaryRecords)
            {
                lobjRetirementContribution.iclbJobServiceContributions = new Collection<busPersonAccountRetirementContribution>();
                DateTime ldteStartDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month, 01);
                // 2.a Loop the contributions to check whether the consecutive next 36 month records exists
                for (int i = 0; i <= (aintFASConsecutiveMonths - 1); i++)
                {
                    bool lblnIsContributionExits = false;
                    foreach (busPersonAccountRetirementContribution lobjRetrContr in lclbJobServiceSalaryRecords)
                    {
                        if ((lobjRetrContr.icdoPersonAccountRetirementContribution.pay_period_month == ldteStartDate.Month) &&
                            (lobjRetrContr.icdoPersonAccountRetirementContribution.pay_period_year == ldteStartDate.Year))
                        {
                            // 2.b Contribution exists for the give Month and Year
                            lobjRetirementContribution.iclbJobServiceContributions.Add(lobjRetrContr);
                            lobjRetirementContribution.idecTotalSalaryAmount += lobjRetrContr.icdoPersonAccountRetirementContribution.salary_amount;
                            lblnIsContributionExits = true;
                            break;
                        }
                    }
                    if (!lblnIsContributionExits)
                        break;
                    ldteStartDate = ldteStartDate.AddMonths(1);
                }
            }
            // 3. Sort the Contribution collection by Total Salary amount and returns the top for highest salary amount
            Collection<busPersonAccountRetirementContribution> lclbFinalSalaryRecords = new Collection<busPersonAccountRetirementContribution>();
            lclbJobServiceSalaryRecords = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>("idecTotalSalaryAmount desc", lclbJobServiceSalaryRecords);
            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lclbJobServiceSalaryRecords)
            {
                if (lobjRetirementContribution.iclbJobServiceContributions.Count == aintFASConsecutiveMonths)
                {
                    lclbFinalSalaryRecords = lobjRetirementContribution.iclbJobServiceContributions;
                    break;
                }
            }
            return lclbFinalSalaryRecords;
        }

        /// <summary>
        /// Loads the projected Salary Records for the given date
        /// </summary>
        /// <param name="aintApplicationID">Int</param>
        /// <param name="astrPersonAccountID">string</param>
        /// <param name="adteRetirementDate">DateTime</param>
        /// <param name="aintPersonAccountID">Person Account ID</param>
        /// <param name="aintMonthOfIncrease">Int</param>
        /// <param name="adecPercetageOfIncrease">decimal</param>
        /// <returns> Collection<busPersonAccountRetirementContribution> </returns>
        public static Collection<busPersonAccountRetirementContribution> GetProjectedSalaryRecords(int aintPersonID, int aintPlanID, DateTime adteTerminationDate,
                                                                          int aintSalaryMonthIncrease, decimal adecPercentageSalaryIncrease, utlPassInfo aobjPassInfo
                                                                           , busPerson abusPerson = null, DataTable adtbLastSalaryWithoutPersonAccount = null,
                                                                          bool ablnIsFromFAS2020 = false)
        {
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
            if (abusPerson == null)
            {
                lobjPerson.FindPerson(aintPersonID);
            }
            else
            {
                lobjPerson = abusPerson;
            }
            /* UAT PIR : 935 FAS projection for seasonal data */
            if (!lobjPerson.iblnIsEmploymentSeasonalLoaded)
                lobjPerson.LoadEmploymentSeasonal();

            DateTime ldtNextIncrementDate = new DateTime();
            Collection<busPersonAccountRetirementContribution> lclbRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            lobjRetirementContribution.ibusPARetirement = new busPersonAccountRetirement();
            lobjRetirementContribution.ibusPARetirement.icdoPersonAccount = new cdoPersonAccount();
            lobjRetirementContribution.ibusPARetirement.ibusPlan = new busPlan();
            lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan = new cdoPlan();
            // For PIR ID 1920, to load RTW refunded FAS the extra parameter is added to the query.
            if (adtbLastSalaryWithoutPersonAccount == null)
                adtbLastSalaryWithoutPersonAccount = busBase.Select("cdoBenefitCalculationFasMonths.LoadLastSalaryRecord", new object[3] { aintPersonID, aintPlanID, 0 });
            if (adtbLastSalaryWithoutPersonAccount.Rows.Count > 0)
            {
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.LoadData(adtbLastSalaryWithoutPersonAccount.Rows[0]);
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan.LoadData(adtbLastSalaryWithoutPersonAccount.Rows[0]);
                DateTime ldtLastContributionDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month, 1);
                if (ldtLastContributionDate != DateTime.MinValue)
                {
                    decimal ldecLastSalaryAmount = lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount;
                    int lintmonthoficrease = aintSalaryMonthIncrease;
                    if (lintmonthoficrease == 0)
                    {
                        lintmonthoficrease = 1;
                    }
                    if (aintSalaryMonthIncrease > ldtLastContributionDate.Month)
                    {
                        ldtNextIncrementDate = new DateTime(ldtLastContributionDate.Year, lintmonthoficrease, 1);
                    }
                    else
                    {
                        ldtNextIncrementDate = new DateTime(ldtLastContributionDate.Year + 1, lintmonthoficrease, 1);
                    }
                    decimal ldecNewSalary = lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount;
                    ldtLastContributionDate = ldtLastContributionDate.AddMonths(1);
                    while (ldtLastContributionDate < adteTerminationDate)
                    {
                        if (ldtLastContributionDate.Month == ldtNextIncrementDate.Month)
                        {
                            ldecNewSalary = ldecNewSalary + ((ldecNewSalary * adecPercentageSalaryIncrease) / 100);
                        }
                        busPersonAccountRetirementContribution lobjProjectedSalary = new busPersonAccountRetirementContribution();
                        lobjProjectedSalary.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
                        lobjProjectedSalary.ibusPARetirement = new busPersonAccountRetirement();
                        lobjProjectedSalary.ibusPARetirement.icdoPersonAccount = new cdoPersonAccount();
                        lobjProjectedSalary.ibusPARetirement.ibusPlan = new busPlan();
                        lobjProjectedSalary.ibusPARetirement.ibusPlan.icdoPlan = new cdoPlan();
                        lobjProjectedSalary.icdoPersonAccountRetirementContribution.person_account_id =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id;
                        lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_month = ldtLastContributionDate.Month;
                        lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_year = ldtLastContributionDate.Year;
                        lobjProjectedSalary.icdoPersonAccountRetirementContribution.effective_date = new DateTime(ldtLastContributionDate.Year, ldtLastContributionDate.Month, 1);
                        lobjProjectedSalary.icdoPersonAccountRetirementContribution.salary_amount = ldecNewSalary;
                        lobjProjectedSalary.ibnlIsProjectedSalaryRecord = true;
                        lobjProjectedSalary.ibusPARetirement.ibusPlan = lobjRetirementContribution.ibusPARetirement.ibusPlan;
                        /* UAT PIR : 935 FAS projection for seasonal data */
                        bool bAllowProjection = true;
                        if (lobjPerson.iblnIsEmploymentSeasonal)
                        {
                            bAllowProjection = IsFiscalMonthForSeasonalEligible(ldtLastContributionDate.Month, lobjPerson.iintSeasonalMonths);
                        }

                        if (bAllowProjection)
                        {
                            lclbRetirementContribution.Add(lobjProjectedSalary);
                        }

                        //Add record with salary amount zero for FAS2020 Projection.
                        if (!bAllowProjection && ablnIsFromFAS2020)
                        {
                            lobjProjectedSalary.icdoPersonAccountRetirementContribution.salary_amount = 0.0M;
                            lclbRetirementContribution.Add(lobjProjectedSalary);
                        }
                        /***************************************/
                        ldtLastContributionDate = ldtLastContributionDate.AddMonths(1);
                    }
                }
            }
            return lclbRetirementContribution;
        }

        #endregion
        /* UAT PIR : 935 FAS projection for seasonal data */
        public static bool IsFiscalMonthForSeasonalEligible(int aintactualMonth, int aintSeasonalLimit)
        {
            int lintFiscalMonth = 0;
            if (aintactualMonth >= 7)
            {
                lintFiscalMonth = aintactualMonth - 6;
            }
            else
            {
                lintFiscalMonth = aintactualMonth + 6;
            }

            if (lintFiscalMonth <= aintSeasonalLimit)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Return the Formatted FAS by Plan
        /// </summary>
        /// <param name="adecTotalSalaryAmount">FAS Amount</param>
        /// <param name="aintFASDivider">FAS Divider</param>
        /// <param name="aintPlanID">Plan ID</param>
        /// <returns>Formatted FAS</returns>
        public static decimal FormatFAS(decimal adecTotalSalaryAmount, int aintFASDivider, int aintPlanID)
        {
            decimal ldecTotalSalaryAmount = 0.0M;
            if (aintFASDivider != 0)
                ldecTotalSalaryAmount = adecTotalSalaryAmount / aintFASDivider;
            if (aintPlanID == busConstant.PlanIdJobService)
            {
                ldecTotalSalaryAmount = Slice(ldecTotalSalaryAmount, 2);
            }
            else
            {
                ldecTotalSalaryAmount = Math.Round(ldecTotalSalaryAmount, 2, MidpointRounding.AwayFromZero);
            }
            return ldecTotalSalaryAmount;
        }

        #region Benefit Multiplier

        public static Collection<cdoBenefitProvisionMultiplier> LoadBenefitProvisionMultiplier(DateTime adteCalculationDate, int aintBenefitProvisionID,
                                                                string astrBenefitAccountType, string astrIsConversionRecord)
        {
            Collection<cdoBenefitProvisionMultiplier> lclbBenefitProvisionMultiplier = new Collection<cdoBenefitProvisionMultiplier>();
            if (adteCalculationDate != DateTime.MinValue)
            {
                DataTable ldtbResult = busBase.Select("cdoBenefitProvisionMultiplier.GetBenefitProvisionMultiplier",
                                            new object[4] { adteCalculationDate, aintBenefitProvisionID, astrBenefitAccountType, astrIsConversionRecord });
                foreach (DataRow drTier in ldtbResult.Rows)
                {
                    cdoBenefitProvisionMultiplier lcdoBenefitProvisionMultiplier = new cdoBenefitProvisionMultiplier();
                    lcdoBenefitProvisionMultiplier.LoadData(drTier);
                    lclbBenefitProvisionMultiplier.Add(lcdoBenefitProvisionMultiplier);
                }
            }
            return lclbBenefitProvisionMultiplier;
        }

        public static Collection<cdoBenefitProvisionMultiplier> LoadBenefitProvisionMultiplier(DateTime adteCalculationDate, int aintBenefitProvisionID,
                                                                        string astrBenefitAccountType)
        {
            return (LoadBenefitProvisionMultiplier(adteCalculationDate, aintBenefitProvisionID, astrBenefitAccountType, "N"));
        }

        #endregion

        # region Vesting Rules Normal and Early rules
        //Check of vesting and normal and Early rules based on
        //1. plan 
        //2. member age
        //3. benefit type
        //4. employment service
        //5. TVSC                        

        //Load all the rows based on the plan provision id
        public static Collection<cdoBenefitProvisionEligibility> LoadEligibilityForPlan(int aintPlanID, int aintBenefitProvisionId, string astrBenefitType,
                                                                             string astrEligibilityTypeValue, utlPassInfo aobjPassInfo, DateTime? adtmPlanStartDate, busDBCacheData aobjDBCacheData = null)
        {
            if (aobjDBCacheData == null)
            {
                aobjDBCacheData = new busDBCacheData();
                aobjDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(aobjPassInfo);
            }
            DataTable lobjProvisionEligibility = aobjDBCacheData.idtbCachedBenefitProvisionEligibility.AsEnumerable().Where(dtrow => dtrow.Field<int>("benefit_provision_id") == aintBenefitProvisionId
                                                         && dtrow.Field<string>("benefit_account_type_value") == astrBenefitType
                                                         && dtrow.Field<string>("eligibility_type_value") == astrEligibilityTypeValue
                                                         && dtrow.Field<DateTime>("effective_date") <= (adtmPlanStartDate == DateTime.MinValue ? DateTime.Now : adtmPlanStartDate)).AsDataTable();

            Collection<cdoBenefitProvisionEligibility> aclbBenefitProvisionEligibility = new Collection<cdoBenefitProvisionEligibility>();
            aclbBenefitProvisionEligibility = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(lobjProvisionEligibility);

            return aclbBenefitProvisionEligibility.OrderByDescending(i => i.benefit_provision_eligibility_id).ToList().ToCollection();
        }

        //checks for normal eligibility           
        //this method checks for eligibility based on the plan and rule of 85 or 80 
        //which ever is applicable as per plan defined in UCS
        public static bool CheckIsPersonEligibleforNormal(int aintPlanId, int aintBenefitProvisionID, string astrBenefitType, decimal adecMemberAge, int aintMemberAge,
            decimal adecTVSC, DateTime adtDateToCompare, busPersonAccount abusPersonAccount, utlPassInfo aobjPassInfo, bool ablnIsEstimate, DateTime adtTerminationDate, decimal adecExtraServiceCredit)
        {
            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionNormalEligibility = LoadEligibilityForPlan(aintPlanId, aintBenefitProvisionID, astrBenefitType, busConstant.BenefitProvisionEligibilityNormal, aobjPassInfo, abusPersonAccount?.icdoPersonAccount?.start_date);

            if (abusPersonAccount.ibusPerson.IsNull())
                abusPersonAccount.LoadPerson();
            //PIR 14646 - Finding out to which benefit tier of main the member belong to
            busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
            lbusPersonAccountRetirement.FindPersonAccountRetirement(abusPersonAccount.icdoPersonAccount.person_account_id);
            if (astrBenefitType == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (aintPlanId == busConstant.PlanIdMain)
                {
                    if (string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) || lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit1997Tier)
                        lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == busConstant.MainBenefit1997Tier).ToList().ToCollection();
                    else
                        lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == busConstant.MainBenefit2016Tier).ToList().ToCollection();
                }
                else if (aintPlanId == busConstant.PlanIdBCILawEnf)//PIR 26282
                {
                    if (string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) || lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.BCIBenefit2011Tier)
                        lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == busConstant.BCIBenefit2011Tier).ToList().ToCollection();
                    else
                        lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == busConstant.BCIBenefit2023Tier).ToList().ToCollection();
                }
                //PIR 26544
                else if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
                    lclbBenefitProvisionNormalEligibility = lclbBenefitProvisionNormalEligibility.Where(i => i.benefit_tier_value == lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value).ToList().ToCollection();
            }
            if (lclbBenefitProvisionNormalEligibility.IsNotNull() && lclbBenefitProvisionNormalEligibility.Count > 0)
            {
                int lintIterations = 0;
                foreach (cdoBenefitProvisionEligibility lcdoBenefitProvisionEligibility in lclbBenefitProvisionNormalEligibility.OrderByDescending(i=>i.effective_date))
                {
                    if (lintIterations >= 1 && string.IsNullOrEmpty(lcdoBenefitProvisionEligibility.grouping_logic))
                        continue;
                    if (lcdoBenefitProvisionEligibility.age_plus_service > 0)
                    {
                        DateTime ldteNRDateByrRule = DateTime.MinValue;
                        if (IsMemberSatisfyRuleOf85Or80(aintMemberAge, adecTVSC, lcdoBenefitProvisionEligibility.age_plus_service, abusPersonAccount, adtTerminationDate,
                                aobjPassInfo, ablnIsEstimate, adecExtraServiceCredit, out ldteNRDateByrRule))
                        {
                            if (lcdoBenefitProvisionEligibility.minimum_age > 0)
                            {
                                if (adecMemberAge >= lcdoBenefitProvisionEligibility.minimum_age)
                                    return true;
								//PIR-16343 Added to calculate Person's age on rule met date.
                                if (abusPersonAccount.ibusPerson == null)
                                    abusPersonAccount.LoadPerson();
                                int aintMonths = 0;
                                decimal adecMonthAndYear = 0;
                                int aintMemberAgeYear = 0;
                                int aintMemberAgeMonths = 0;
                                //PIR 24069 Trying to generate a b.e. for Tashe Jensen 260504, getting error msg even included the service purchse-- so adding 1 month into person date of birth
                                CalculateAge(abusPersonAccount.ibusPerson.icdoPerson.date_of_birth.AddMonths(1), ldteNRDateByrRule, ref aintMonths, ref adecMonthAndYear, 4, ref aintMemberAgeYear, ref aintMemberAgeMonths);
                                if (aintMemberAgeYear >= lcdoBenefitProvisionEligibility.minimum_age)
                                    return true;
                            }
                            else
                            {
                                return true;
                            }
                        }
                    }
                    if (lcdoBenefitProvisionEligibility.age > 0 || lcdoBenefitProvisionEligibility.service >= 0)
                    {
                        bool lblnAge = false, lblnService = false;
                        if (adecMemberAge >= lcdoBenefitProvisionEligibility.age)
                            lblnAge = true;
                        if (lcdoBenefitProvisionEligibility.service > 0 && adecTVSC >= lcdoBenefitProvisionEligibility.service)
                            lblnService = true;
                        if (lcdoBenefitProvisionEligibility.service == 0) lblnService = true;
                        if (lblnAge && lblnService)
                            return true;
                    }
                    //PIR 26697
                    if (adecMemberAge >= lcdoBenefitProvisionEligibility.age)
                        return true;
                    lintIterations++;
                }
            }
            return false;

            //if ((aintPlanId == busConstant.PlanIdMain) || (aintPlanId == busConstant.PlanIdJudges))
            //{
            //    //Prod PIR: 4370. Extra VSC entered should be accounted inorder to find NRD Date.
            //    if (IsMemberSatisfyRuleOf85Or80(aintMemberAge, adecTVSC, lclbBenefitProvisionNormalEligibility[0].age_plus_service, abusPersonAccount, adtTerminationDate,
            //        aobjPassInfo, ablnIsEstimate, adecExtraServiceCredit))
            //    {
            //        if (aintPlanId == busConstant.PlanIdJudges)
            //            return true;
            //        else if (aintPlanId == busConstant.PlanIdMain && astrBenefitType == busConstant.ApplicationBenefitTypeRetirement)
            //        {
            //            if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit2016Tier)
            //            {
            //                if (adecMemberAge >= lclbBenefitProvisionNormalEligibility[0].minimum_age)
            //                    return true;
            //            }
            //            else
            //            {
            //                return true;
            //            }
            //        }
            //        else if (aintPlanId == busConstant.PlanIdMain && astrBenefitType == busConstant.ApplicationBenefitTypePreRetirementDeath)
            //        {
            //            return true;
            //        }
            //    }

            //    if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age)))
            //        return true;
            //}
            //if ((aintPlanId == busConstant.PlanIdLE) || (aintPlanId == busConstant.PlanIdLEWithoutPS)
            //    || (aintPlanId == busConstant.PlanIdBCILawEnf)) //pir 7943
            //{
            //    if (IsEmploymentConsecutive(lclbBenefitProvisionNormalEligibility[0].service, lclbBenefitProvisionNormalEligibility[0].immediately_before_service_flag
            //        , adtDateToCompare, abusPersonAccount, ablnIsEstimate))
            //    {
            //        if (IsMemberSatisfyRuleOf85Or80(aintMemberAge, adecTVSC, lclbBenefitProvisionNormalEligibility[0].age_plus_service, abusPersonAccount, adtTerminationDate,
            //        aobjPassInfo, ablnIsEstimate, adecExtraServiceCredit))
            //        {
            //            return true;
            //        }
            //        if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age)))
            //            return true;
            //    }
            //}
            //if (aintPlanId == busConstant.PlanIdHP)
            //{
            //    if (IsMemberSatisfyRuleOf85Or80(aintMemberAge, adecTVSC, lclbBenefitProvisionNormalEligibility[0].age_plus_service, abusPersonAccount, adtTerminationDate,
            //        aobjPassInfo, ablnIsEstimate, adecExtraServiceCredit))
            //    {
            //        return true;
            //    }
            //    if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age)))
            //        return true;
            //}
            //if (aintPlanId == busConstant.PlanIdNG)
            //{
            //    if ((adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age)))
            //        && (IsEmploymentConsecutive(lclbBenefitProvisionNormalEligibility[0].service, lclbBenefitProvisionNormalEligibility[0].immediately_before_service_flag,
            //                adtDateToCompare, abusPersonAccount, ablnIsEstimate)))
            //        return true;
            //}
            //if (aintPlanId == busConstant.PlanIdJobService)
            //{
            //    if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age)))
            //        return true;
            //    else if ((adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[1].age)))
            //                       && (((adecTVSC) >= (lclbBenefitProvisionNormalEligibility[1].service))))
            //        return true;
            //    else if ((adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[2].age)))
            //        && (((adecTVSC) >= (lclbBenefitProvisionNormalEligibility[2].service))))
            //        return true;
            //    else if ((adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[3].age))
            //              && (((adecTVSC) >= (lclbBenefitProvisionNormalEligibility[3].service)))))
            //        return true;
            //}
            //return false;
        }

        private static bool IsMemberSatisfyRuleOf85Or80(int aintMemberAge, decimal adecTVSC, int aintRuleNo, busPersonAccount abusPersonAccount, DateTime adtTerminationDate,
            utlPassInfo aobjPassInfo, bool ablnIsEstimate, decimal adecExtraServiceCredit, out DateTime adteNormalRetireDateByRule)
        {
            //UAT PIR: 1163. Checking for Rule of 85 or 80 Paranthesis Correction
            int lintMembersAgeInMonthsAsOnRule = 0;
            decimal ldecMemberAgeBasedOnRule = 0.0M;
            int lintMemberAgeYearPart = 0;
            int lintMemberAgeMonthPart = 0;

            if (abusPersonAccount.ibusPerson.IsNull())
                abusPersonAccount.LoadPerson();

            DateTime ldtNormalRetirementDate2 = GetNormalRetirementDateByRuleOf80Or85(abusPersonAccount.ibusPerson.icdoPerson.date_of_birth, Convert.ToDecimal(aintRuleNo) * 12,
                     adtTerminationDate, abusPersonAccount.icdoPersonAccount.person_account_id, ablnIsEstimate,
                     adecExtraServiceCredit, aobjPassInfo);
            adteNormalRetireDateByRule = ldtNormalRetirementDate2;
            CalculateAge(abusPersonAccount.ibusPerson.icdoPerson.date_of_birth, ldtNormalRetirementDate2,
                        ref lintMembersAgeInMonthsAsOnRule, ref ldecMemberAgeBasedOnRule, 2, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);
            
            //adecMemberAgefor85or80 = ((Convert.ToDecimal(aintRuleNo) * 12) - (adecTVSC));

            if (aintMemberAge >= (lintMembersAgeInMonthsAsOnRule - 1))
                return true;
            return false;
        }

        /// <summary>
        ///Check for vesting Eligibility 
        ///eligibility changes as per the plan selected and based 
        ///on the member s age based on the retirement entered on the screen
        ///returns bool value after comparing values
        ///and code values entered in the code value table
        /// </summary>
        /// <param name="aintPlanId"></param>
        /// <param name="astrPlanCode"></param>
        /// <param name="acdoVestingEligibilityDatas"></param>
        /// <param name="adecTVSC"></param>
        /// <param name="adecMemberAge"></param>
        /// <returns></returns>
        public static bool CheckIsPersonVested(int aintPlanId, string astrPlanCode, int aintBenefitProvisionID, string astrBenefitType,
                                                decimal adecTVSC, decimal adecMemberAge, DateTime adtDateToCompare, bool ablnIsEstimate,
                                                DateTime adtTerminationDate, busPersonAccount abusPersonAccount, utlPassInfo aobjPassInfo,
                                                busDBCacheData aobjDBCacheData = null, bool ablnIsFromBatch = false)
        {
            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionVestedEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionVestedEligibility = LoadEligibilityForPlan(aintPlanId, aintBenefitProvisionID, astrBenefitType, busConstant.BenefitProvisionEligibilityVested,
                aobjPassInfo, abusPersonAccount?.icdoPersonAccount?.start_date, aobjDBCacheData: aobjDBCacheData);

            //PIR 26666
            if (abusPersonAccount.IsNotNull() && abusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                lclbBenefitProvisionVestedEligibility = LoadEligibilityForPlanByBenefitTier(aintPlanId, astrBenefitType, abusPersonAccount, lclbBenefitProvisionVestedEligibility);
            }
            if (lclbBenefitProvisionVestedEligibility.Count > 0)
            {
                //PIR 26282 - Post Production Issue - Taking Normal Age only when age_while_employed_flag is Y to check vesting 
                int lintBenefitProvisionNormalEligibilityAge = 0;
                if (lclbBenefitProvisionVestedEligibility[0].age_while_employed_flag == busConstant.Flag_Yes)
                {
                    Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = LoadEligibilityForPlan(aintPlanId, aintBenefitProvisionID, astrBenefitType, busConstant.BenefitProvisionEligibilityNormal,
                        aobjPassInfo, abusPersonAccount?.icdoPersonAccount?.start_date, aobjDBCacheData: aobjDBCacheData);
                    //PIR 26666
                    if (abusPersonAccount.IsNotNull() && abusPersonAccount.icdoPersonAccount.person_account_id > 0)
                    {
                        lclbBenefitProvisionNormalEligibility = LoadEligibilityForPlanByBenefitTier(aintPlanId, astrBenefitType, abusPersonAccount, lclbBenefitProvisionNormalEligibility);
                    }
                    if (lclbBenefitProvisionNormalEligibility.Count > 0)
                        lintBenefitProvisionNormalEligibilityAge = lclbBenefitProvisionNormalEligibility[0].age;
                }
				//PIR 26697 - Commented below line after discussed with Maik
                if ( //(adtTerminationDate.AddMonths(1).Month == adtDateToCompare.Month && adtTerminationDate.AddMonths(1).Year == adtDateToCompare.Year) &&
                    lclbBenefitProvisionVestedEligibility[0].age_while_employed_flag == busConstant.Flag_Yes &&
                    adecMemberAge >= lintBenefitProvisionNormalEligibilityAge)
                {
                    return true;
                }
                else
                {
                    if ((adecMemberAge >= lclbBenefitProvisionVestedEligibility[0].age &&
                        adecTVSC >= lclbBenefitProvisionVestedEligibility[0].service) || (ablnIsFromBatch && adecTVSC >= lclbBenefitProvisionVestedEligibility[0].service)) //Service based vesting Logic //PIR 26854
                    {
                        //Check CONSECUTIVE_SERVICE_FLAG
                        if (string.IsNullOrEmpty(lclbBenefitProvisionVestedEligibility[0].consecutive_service_flag) ||
                                lclbBenefitProvisionVestedEligibility[0].consecutive_service_flag == busConstant.Flag_No)
                        {
                            return true;
                        }
                        else
                        {
                            if (IsTVSCConsecutive(lclbBenefitProvisionVestedEligibility[0].service, adtTerminationDate, adtDateToCompare, abusPersonAccount, ablnIsEstimate))
                            {
                                if (string.IsNullOrEmpty(lclbBenefitProvisionVestedEligibility[0].immediately_before_service_flag) ||
                                    lclbBenefitProvisionVestedEligibility[0].immediately_before_service_flag == busConstant.Flag_No)
                                {
                                    //Service is consecutive and immediately before service flag is null or no scenario, 
                                    return true;
                                }
                                else
                                {
                                    //SERVICE is immediately prior to Ret Date
                                    //what to check to determine if the service is immediately prior to retiremet date ??
                                    if (IsEmploymentConsecutive(lclbBenefitProvisionVestedEligibility[0].service, lclbBenefitProvisionVestedEligibility[0].immediately_before_service_flag,
                                        adtDateToCompare, abusPersonAccount, ablnIsEstimate))
                                        return true; //eligible based on service.
                                }
                            }
                        }
                    }
                    else // Age Plus Service based vesting logic 
                    {
                        if (lclbBenefitProvisionVestedEligibility[0].age_plus_service > 0
                            && Math.Round((adecMemberAge + (adecTVSC/12)), 4, MidpointRounding.AwayFromZero) >= lclbBenefitProvisionVestedEligibility[0].age_plus_service)
                        {
                            return true;
                        }
                    }
                }
                //else
                //{
                //    //member age is < AGE – Not eligible based on AGE – Not eligible since Service is already checked
                //    return false;
                //}
                //PIR 14646 Commented as to make vesting logic code independent and to make vesting logic completely based on sgt_benefit_provision_eligibility table
                //if (aintPlanId == busConstant.PlanIdDC) return true;
                //if ((aintPlanId == busConstant.PlanIdMain)
                //    || (aintPlanId == busConstant.PlanIdJudges)
                //    || (aintPlanId == busConstant.PlanIdHP
                //    || aintPlanId == busConstant.PlanIdLE)
                //    || (aintPlanId == busConstant.PlanIdLEWithoutPS)
                //    || (aintPlanId == busConstant.PlanIdNG)
                //    || (aintPlanId == busConstant.PlanIdBCILawEnf))
                //{
                //    if (((adecTVSC) >= (lclbBenefitProvisionVestedEligibility[0].service))
                //        || (adecMemberAge >= (lclbBenefitProvisionVestedEligibility[0].age)))
                //        return true;
                //}

                //// AS per discussion with maik, PIR 14403 changes.
                ////if ((aintPlanId == busConstant.PlanIdLE)
                ////    || (aintPlanId == busConstant.PlanIdLEWithoutPS)
                ////    || (aintPlanId == busConstant.PlanIdNG)
                ////    || (aintPlanId == busConstant.PlanIdBCILawEnf)) //pir 7943
                ////{
                ////    if (IsTVSCConsecutive(lclbBenefitProvisionVestedEligibility[0].service, adtTerminationDate, adtDateToCompare, abusPersonAccount, ablnIsEstimate)) // PIR 8829- remove age eligibility check
                ////        return true;
                ////}

                //if (aintPlanId == busConstant.PlanIdJobService)
                //{
                //    if ((adecTVSC) >= (lclbBenefitProvisionVestedEligibility[0].service))
                //        return true;
                //}
            }
            return false;
        }

        private static Collection<cdoBenefitProvisionEligibility> LoadEligibilityForPlanByBenefitTier(int aintPlanId, string astrBenefitType, busPersonAccount abusPersonAccount, Collection<cdoBenefitProvisionEligibility> aclbBenefitProvisionEligibility)
        {
            if (abusPersonAccount.IsNotNull() && abusPersonAccount.icdoPersonAccount.person_account_id > 0 && aclbBenefitProvisionEligibility.Count > 0)
            {
                busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
                lbusPersonAccountRetirement.FindPersonAccountRetirement(abusPersonAccount.icdoPersonAccount.person_account_id);
                if (astrBenefitType == busConstant.ApplicationBenefitTypeRetirement)
                {
                    if (aintPlanId == busConstant.PlanIdMain)
                    {
                        if (string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) || lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit1997Tier)
                            aclbBenefitProvisionEligibility = aclbBenefitProvisionEligibility.Where(i => i.benefit_tier_value == busConstant.MainBenefit1997Tier).ToList().ToCollection();
                        else
                            aclbBenefitProvisionEligibility = aclbBenefitProvisionEligibility.Where(i => i.benefit_tier_value == lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value).ToList().ToCollection();
                    }
                    else if (aintPlanId == busConstant.PlanIdBCILawEnf)
                    {
                        if (string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) || lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.BCIBenefit2011Tier)
                            aclbBenefitProvisionEligibility = aclbBenefitProvisionEligibility.Where(i => i.benefit_tier_value == busConstant.BCIBenefit2011Tier).ToList().ToCollection();
                        else
                            aclbBenefitProvisionEligibility = aclbBenefitProvisionEligibility.Where(i => i.benefit_tier_value == lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value).ToList().ToCollection();
                    }
                    //PIR 26544
                    else if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
                        aclbBenefitProvisionEligibility = aclbBenefitProvisionEligibility.Where(i => i.benefit_tier_value == lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value).ToList().ToCollection();
                }
            }
            return aclbBenefitProvisionEligibility;
        }

        /// <summary>
        ///Check for vesting Eligibility for service purchase estimate
        ///eligibility changes as per the plan selected and based 
        ///on service based only
        ///returns bool value after comparing values
        ///and code values entered in the code value table
        /// </summary>
        /// <param name="aintPlanId"></param>
        /// <param name="astrPlanCode"></param>
        /// <param name="acdoVestingEligibilityDatas"></param>
        /// <param name="adecTVSC"></param>        
        /// <returns></returns>
        public static bool CheckIsPersonVestedForEstimateServicePurchase(int aintPlanId, int aintBenefitProvisionID, string astrBenefitType,
                                                decimal adecTVSC, busPersonAccount abusPersonAccount, utlPassInfo aobjPassInfo,
                                                busDBCacheData aobjDBCacheData = null)
        {
            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionVestedEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionVestedEligibility = LoadEligibilityForPlan(aintPlanId, aintBenefitProvisionID, astrBenefitType, busConstant.BenefitProvisionEligibilityVested,
                aobjPassInfo, abusPersonAccount?.icdoPersonAccount?.start_date, aobjDBCacheData: aobjDBCacheData);

            //PIR 26666
            if (abusPersonAccount.IsNotNull() && abusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                lclbBenefitProvisionVestedEligibility = LoadEligibilityForPlanByBenefitTier(aintPlanId, astrBenefitType, abusPersonAccount, lclbBenefitProvisionVestedEligibility);
            }

            if (lclbBenefitProvisionVestedEligibility.Count > 0)
            {
                if (adecTVSC >= lclbBenefitProvisionVestedEligibility[0].service) //Service based vesting Logic
                {
                    return true;
                }
            }
            return false;
        }

        //BR-051-30
        //checks for early retirement eligibility
        //eligibility changes as per the plan selected 
        //returns bool value based on the member s age based on the retirement entered on the screen
        //and code values entered in the code value table
        public static bool CheckISPersonEligibleForEarly(int aintPlanId, int aintProvisionID, string astrBenefitType, decimal adecMemberAge,
                                                        decimal adecTVSC, ref string astrEarlyReductionWaivedFlag, DateTime adtDateToCompare,
            busPersonAccount abusPersonAccount, utlPassInfo aobjPassInfo, bool ablnIsEstimate)
        {
            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionEarlyEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionEarlyEligibility = LoadEligibilityForPlan(aintPlanId, aintProvisionID, astrBenefitType, busConstant.BenefitProvisionEligibilityEarly, aobjPassInfo, abusPersonAccount?.icdoPersonAccount?.start_date);
            if (abusPersonAccount.IsNotNull() && abusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                lclbBenefitProvisionEarlyEligibility = LoadEligibilityForPlanByBenefitTier(aintPlanId, astrBenefitType, abusPersonAccount, lclbBenefitProvisionEarlyEligibility);
            }
            if (lclbBenefitProvisionEarlyEligibility.IsNotNull() && lclbBenefitProvisionEarlyEligibility.Count > 0)
            {
                int lintIterations = 0;
                foreach (cdoBenefitProvisionEligibility lcdoBenefitProvisionEligibility in lclbBenefitProvisionEarlyEligibility.OrderByDescending(i => i.effective_date))
                {
                    if (lintIterations >= 1 && string.IsNullOrEmpty(lcdoBenefitProvisionEligibility.grouping_logic))
                        continue;
                    if (lcdoBenefitProvisionEligibility.age > 0 && string.IsNullOrEmpty(lcdoBenefitProvisionEligibility.grouping_logic))
                    {
                        if (adecMemberAge >= lclbBenefitProvisionEarlyEligibility[0].age)
                            return true;
                    }
                    if ((lcdoBenefitProvisionEligibility.age > 0 || lcdoBenefitProvisionEligibility.service >= 0) && (!string.IsNullOrEmpty(lcdoBenefitProvisionEligibility.grouping_logic)))
                    {
                        bool lblnAge = false, lblnService = false;
                        if (adecMemberAge >= lcdoBenefitProvisionEligibility.age)
                            lblnAge = true;
                        if (lcdoBenefitProvisionEligibility.service > 0 && adecTVSC >= lcdoBenefitProvisionEligibility.service)
                            lblnService = true;
                        if (lcdoBenefitProvisionEligibility.service == 0) lblnService = true;
                        if (lblnAge && lblnService)
                            return true;
                    }
                    lintIterations++;
                }
            }
            return false;
            //PIR 14646 - Commented to make early eligibility logic more dependent on benefit provision eligibility table
                //if (aintPlanId == busConstant.PlanIdMain)
                //{
                //    if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[0].age)))
                //        return true;
                //}
                //if (aintPlanId == busConstant.PlanIdHP)
                //{
                //    if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[0].age)))
                //        return true;
                //}
                //if (aintPlanId == busConstant.PlanIdJudges)
                //{
                //    if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[0].age)))
                //        return true;
                //}
                //if ((aintPlanId == busConstant.PlanIdLE)
                //    || (aintPlanId == busConstant.PlanIdLEWithoutPS)
                //    || (aintPlanId == busConstant.PlanIdNG)
                //    || (aintPlanId == busConstant.PlanIdBCILawEnf)) //pir 7943
                //{
                //    if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[0].age)))
                //    {
                //        astrEarlyReductionWaivedFlag = busConstant.Flag_No;
                //        //Special case when member attains 55 or over and still not having 36 months of consecutive service
                //        //Set EARLY_REDUCTION_WAIVED_FLAG as Yes
                //        if ((adecMemberAge >= busConstant.MinAgeForEarlyReductionSpecialCase)
                //            && (!(IsEmploymentConsecutive(Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[0].service),
                //                                                    lclbBenefitProvisionEarlyEligibility[0].immediately_before_service_flag,
                //                                                    adtDateToCompare, abusPersonAccount, ablnIsEstimate))))
                //        {
                //            astrEarlyReductionWaivedFlag = busConstant.Flag_Yes;
                //        }
                //        return true;
                //    }
                //}
                //if (aintPlanId == busConstant.PlanIdJobService)
                //{
                //    if (adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[0].age)))
                //        return true;
                //    else if ((adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[1].age)))
                //                 && (((adecTVSC) >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[1].service)))))
                //        return true;
                //    else if ((adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[2].age)))
                //            && (((adecTVSC) >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[2].service)))))
                //        return true;
                //    else if ((adecMemberAge >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[3].age))
                //            && (((adecTVSC) >= (Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[3].service))))))
                //        return true;
                //}
            //}
            //return false;
        }

        //BR-051-30
        // Check if employment s consecutive
        //no of consecutive months are passed as parameter 
        //Load all the retirement contribution for the person account 
        //subtract the number of months from the retirement date and name it ldtFromRetirementDate
        //run while loop till retirement date (ldtToRetirementDate)
        // and check if there is contribution for the last months( No of months based on the parameter passed)
        //adtDate may be retirement date or date of death
        private static bool IsEmploymentConsecutive(int aintNoOfConsecutiveMonths, string astrIsImmediateRetirement, DateTime adtDate, busPersonAccount abusPersonAccount,bool ablnIsEstimate)
        {
            bool lblnContributionExists = false;
            string lstrorderbyClause = string.Empty;

            lstrorderbyClause = "icdoPersonAccountRetirementContribution.pay_period_year asc,icdoPersonAccountRetirementContribution.pay_period_month asc";
            if (astrIsImmediateRetirement == busConstant.Flag_Yes)
                lstrorderbyClause = "icdoPersonAccountRetirementContribution.pay_period_year desc,icdoPersonAccountRetirementContribution.pay_period_month desc";

            if (abusPersonAccount.iclbRetirementContributionAllAsOfDate == null)
            {
                abusPersonAccount.LoadRetirementContributionByDate(string.Empty, adtDate);
            }

            Collection<busPersonAccountRetirementContribution> lclbRetirementContributionAllAsOfDate = new Collection<busPersonAccountRetirementContribution>();
            if ((abusPersonAccount.iclbRetirementContributionAllAsOfDate != null)
                && (abusPersonAccount.iclbRetirementContributionAllAsOfDate.Count > 0))
            {
                foreach (busPersonAccountRetirementContribution lobjRetirementContribution in abusPersonAccount.iclbRetirementContributionAllAsOfDate)
                {
                    if ((lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month != 0) &&
                        (lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year != 0) &&
                        (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value != busConstant.SubSystemInterestCredit)) // PROD PIR ID 4071
                    {
                        lclbRetirementContributionAllAsOfDate.Add(lobjRetirementContribution);
                    }
                }
            }            
			//Prod PIR:4008. Checking the contributions after projecting in case of Estimate.
            if (ablnIsEstimate)
            {
                lclbRetirementContributionAllAsOfDate = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>("icdoPersonAccountRetirementContribution.pay_period_year desc,icdoPersonAccountRetirementContribution.pay_period_month desc", lclbRetirementContributionAllAsOfDate);
                DateTime adtStartDate = DateTime.MinValue;

                if (lclbRetirementContributionAllAsOfDate.Count > 0)
                {            
                    adtStartDate=  new DateTime(lclbRetirementContributionAllAsOfDate[0].icdoPersonAccountRetirementContribution.pay_period_year,
                                    lclbRetirementContributionAllAsOfDate[0].icdoPersonAccountRetirementContribution.pay_period_month, 01);
                    adtStartDate = adtStartDate.AddMonths(1);
                    while (adtStartDate < adtDate)
                    {
                        busPersonAccountRetirementContribution lobjTempContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                        lobjTempContribution.icdoPersonAccountRetirementContribution.pay_period_month = adtStartDate.Month;
                        lobjTempContribution.icdoPersonAccountRetirementContribution.pay_period_year = adtStartDate.Year;
                        lobjTempContribution.icdoPersonAccountRetirementContribution.effective_date = adtStartDate;
                        lobjTempContribution.icdoPersonAccountRetirementContribution.salary_amount = 1000M; //PIR 14646 - Vesting logic changes
                        adtStartDate = adtStartDate.AddMonths(1);
                        lclbRetirementContributionAllAsOfDate.Add(lobjTempContribution);
                    }
                }
            }

            lclbRetirementContributionAllAsOfDate = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>(lstrorderbyClause, lclbRetirementContributionAllAsOfDate);
            if ((lclbRetirementContributionAllAsOfDate != null) &&
                (lclbRetirementContributionAllAsOfDate.Count > 0) && (lclbRetirementContributionAllAsOfDate.Count >= aintNoOfConsecutiveMonths))
            {
                if (astrIsImmediateRetirement == busConstant.Flag_Yes)
                {
                    // PROD PIR ID 4071
                    DateTime ldteStartDate = adtDate.AddMonths(-aintNoOfConsecutiveMonths);
                    DateTime ldteEndDate = adtDate.AddMonths(-1);
                    int lintCounter = 0;
                    while (ldteStartDate <= ldteEndDate)
                    {
                        if (!lclbRetirementContributionAllAsOfDate.Where(lobj => lobj.icdoPersonAccountRetirementContribution.pay_period_month == ldteStartDate.Month &&
                                                lobj.icdoPersonAccountRetirementContribution.pay_period_year == ldteStartDate.Year &&
                                                lobj.icdoPersonAccountRetirementContribution.salary_amount > 0.0M).Any()) //PIR 14646
                        {
                            lblnContributionExists = false; break; // No Consecutive employment
                        }
                        lintCounter++;
                        ldteStartDate = ldteStartDate.AddMonths(1);
                    }
                    if (lintCounter == aintNoOfConsecutiveMonths)
                        lblnContributionExists = true;
                }
                else
                {
                    int lintcounter = 1;
                    foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lclbRetirementContributionAllAsOfDate)
                    {
                        DateTime ldtCompareDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month,
                            01);
                        ldtCompareDate = ldtCompareDate.AddMonths(aintNoOfConsecutiveMonths);
                        int lintCompareDateYearpart = ldtCompareDate.Year;
                        int lintCompareDateMonthpart = ldtCompareDate.Month;
                        int lintContributionCounter = lintcounter + aintNoOfConsecutiveMonths - 1;
                        lintcounter = lintcounter + 1;
                        if (lclbRetirementContributionAllAsOfDate.Count >= lintContributionCounter + 1)
                        {
                            if ((lintCompareDateYearpart == lclbRetirementContributionAllAsOfDate[lintContributionCounter].icdoPersonAccountRetirementContribution.pay_period_year) &&
                            (lintCompareDateMonthpart == lclbRetirementContributionAllAsOfDate[lintContributionCounter].icdoPersonAccountRetirementContribution.pay_period_month))
                            {
                                lblnContributionExists = true;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                
            }
            return lblnContributionExists;
        }

        // this method which check if the person got continuous consecutive 36 months of TVSC
        private static bool IsTVSCConsecutive(int aintNoOfConsecutiveMonths, DateTime adtTerminationDate, DateTime adtRetirementDate,
                                                busPersonAccount abusPersonAccount, bool ablnIsEstimate)
        {
            int lintProjectedMonths = 0;
            if (abusPersonAccount.ibusPerson == null)
                abusPersonAccount.LoadPerson();

            if (abusPersonAccount.ibusPerson.icolPersonAccountByBenefitType == null)
                abusPersonAccount.ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement);

            foreach (busPersonAccount lobjPersonAccount in abusPersonAccount.ibusPerson.icolPersonAccountByBenefitType)
            {
                if (lobjPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
                {
                    if (lobjPersonAccount.iclbRetirementContributionAllAsOfDate == null)
                    {
                        lobjPersonAccount.LoadRetirementContributionByDate
                            ("icdoPersonAccountRetirementContribution.pay_period_year asc,icdoPersonAccountRetirementContribution.pay_period_month asc", adtRetirementDate);
                    }
                    if (lobjPersonAccount.iclbRetirementContributionAllAsOfDate.Count > 0)
                    {
                        //This has been resorted because iclbRetirementContributionAllAsOfDate is already loaded when the person is checked for normal eligibility
						//PIR 8887
                        String lstrorderbyClause = "icdoPersonAccountRetirementContribution.pay_period_year asc,icdoPersonAccountRetirementContribution.pay_period_month asc";
                        lobjPersonAccount.iclbRetirementContributionAllAsOfDate = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>(lstrorderbyClause, lobjPersonAccount.iclbRetirementContributionAllAsOfDate);
                        // Get the Last Contribution Record
                        DateTime ldteFirstContributionDate = DateTime.MinValue;
                        //PIR 14646 Changes - As part of vesting logic change, salary amount > 0 condition added as discussed on call with Maik
                        busPersonAccountRetirementContribution lobjFirstContribution = lobjPersonAccount.iclbRetirementContributionAllAsOfDate.Where(lobj =>
                                                    (lobj.icdoPersonAccountRetirementContribution.pay_period_month != 0) &&
                                                    (lobj.icdoPersonAccountRetirementContribution.pay_period_year != 0) &&
                                                    (lobj.icdoPersonAccountRetirementContribution.salary_amount > 0.0M)).FirstOrDefault();
                        if (lobjFirstContribution != null)
                            ldteFirstContributionDate = new DateTime(
                                    lobjFirstContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                    lobjFirstContribution.icdoPersonAccountRetirementContribution.pay_period_month, 01);

                        // Get the First Contribution Record
                        DateTime ldteLastContributionDate = DateTime.MinValue;
                        busPersonAccountRetirementContribution lobjLastContribution = lobjPersonAccount.iclbRetirementContributionAllAsOfDate.Where(lobj =>
                                                    (lobj.icdoPersonAccountRetirementContribution.pay_period_month != 0) &&
                                                    (lobj.icdoPersonAccountRetirementContribution.pay_period_year != 0) &&
                                                    (lobj.icdoPersonAccountRetirementContribution.salary_amount > 0.0M)).LastOrDefault();
                        if (lobjLastContribution != null)
                            ldteLastContributionDate = new DateTime(
                                        lobjLastContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                        lobjLastContribution.icdoPersonAccountRetirementContribution.pay_period_month, 01);

                        DateTime ldteDateToStartCheck = ldteLastContributionDate;
                        DateTime ldteLastContributionwrtTerminationDate = ldteLastContributionDate;
                        /// UAT PIR ID 1095 - In case of Estimates, the consecutive 
                        if (ablnIsEstimate)
                        {
                            /// This is a exceptional scenario, when the there is a lot of months difference between
                            /// Retirement date & Termination date and the contribution record exits till between that.
                            /// And we have to project data till Termination date.
                            if (ldteLastContributionDate > adtTerminationDate)
                            {
                                busPersonAccount lobjPA = lobjPersonAccount;
                                lobjPA.LoadRetirementContributionByDate
                                                    ("icdoPersonAccountRetirementContribution.pay_period_year asc,icdoPersonAccountRetirementContribution.pay_period_month asc",
                                                    adtTerminationDate.AddMonths(1));
                                busPersonAccountRetirementContribution lobjLast = lobjPersonAccount.iclbRetirementContributionAllAsOfDate.Where(lobj =>
                                                                                    (lobj.icdoPersonAccountRetirementContribution.pay_period_month != 0) &&
                                                                                    (lobj.icdoPersonAccountRetirementContribution.pay_period_year != 0) &&
                                                                                    (lobj.icdoPersonAccountRetirementContribution.salary_amount > 0.0M)).LastOrDefault();
                                ldteLastContributionwrtTerminationDate = new DateTime(
                                        lobjLastContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                        lobjLastContribution.icdoPersonAccountRetirementContribution.pay_period_month, 01);
                            }
                            else
                                ldteDateToStartCheck = adtTerminationDate;
                        }
                        lintProjectedMonths = 1; // The Last contribution Record makes this count.

                        if ((ldteDateToStartCheck != DateTime.MinValue) && (ldteFirstContributionDate != DateTime.MinValue))
                        {
                            while (ldteDateToStartCheck > ldteFirstContributionDate)
                            {
                                //UAT PIR - 1229
                                ldteDateToStartCheck = ldteDateToStartCheck.AddMonths(-1);
                                busPersonAccountRetirementContribution lobjContribution = lobjPersonAccount.iclbRetirementContributionAllAsOfDate.Where(lobj =>
                                        (lobj.icdoPersonAccountRetirementContribution.pay_period_year != 0) &&
                                        (lobj.icdoPersonAccountRetirementContribution.pay_period_month != 0) &&
                                        (lobj.icdoPersonAccountRetirementContribution.pay_period_month == ldteDateToStartCheck.Month) &&
                                        (lobj.icdoPersonAccountRetirementContribution.pay_period_year == ldteDateToStartCheck.Year) &&
                                        (lobj.icdoPersonAccountRetirementContribution.salary_amount > 0.0M)).FirstOrDefault();
                                if ((lobjContribution.IsNotNull()) && (lobjContribution.icdoPersonAccountRetirementContribution.person_account_id != 0))
                                    lintProjectedMonths += 1;   // Contribution exists
                                else
                                {
                                    // No contribution exists
                                    if ((ablnIsEstimate) &&
                                    (busGlobalFunctions.CheckDateOverlapping(ldteDateToStartCheck, ldteLastContributionwrtTerminationDate, adtTerminationDate)))
                                        lintProjectedMonths += 1;
                                    else
                                        lintProjectedMonths = 0;
                                }

                                if (lintProjectedMonths >= aintNoOfConsecutiveMonths)
                                    return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        # endregion


        //Load Benefit Options based on Plan and benefit type 
        //get collection of code values
        //loop thru collection and get select only those values that are of same benefit type
        public static Collection<cdoCodeValue> LoadBenefitOptionsBasedOnPlans(int aintPlanId, string astrBenefitAccountType)
        {
            Collection<cdoCodeValue> lclcBenefitOption = new Collection<cdoCodeValue>();
            Collection<busCodeValue> lclbBenefitOption = busGlobalFunctions.LoadCodeValueByData1(1903, aintPlanId.ToString());
            if (lclbBenefitOption.Count > 0)
            {
                foreach (busCodeValue lobjCodeValue in lclbBenefitOption)
                {
                    if (lobjCodeValue.icdoCodeValue.data2 == astrBenefitAccountType)
                    {
                        busCodeValue lobjCodeValueNew = new busCodeValue();
                        lobjCodeValueNew.icdoCodeValue = new cdoCodeValue();
                        lobjCodeValueNew.icdoCodeValue = busGlobalFunctions.GetCodeValueDetails(2216, lobjCodeValue.icdoCodeValue.data3);

                        lclcBenefitOption.Add(lobjCodeValueNew.icdoCodeValue);
                    }
                }
            }
            lclcBenefitOption = busGlobalFunctions.Sort<cdoCodeValue>("code_value_order", lclcBenefitOption);
            return lclcBenefitOption;
        }

        // Returns true if Benefit Account already exists for the given Person ID 
        public int IsBenefitAccountExists(int aintPersonAccountID, string astrBenefitAccountType)
        {
            int lintBenefitAccountID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoBenefitCalculation.GetBenefitAccountID",
                                        new object[2] { aintPersonAccountID, astrBenefitAccountType },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            return lintBenefitAccountID;
        }
        public static busBenefitAccount GetBenefitAccount(int aintPersonAccountID, string astrBenefitAccountType)
        {
            busBenefitAccount lobjbenefitaccount = new busBenefitAccount();
            lobjbenefitaccount.icdoBenefitAccount = new cdoBenefitAccount();
            DataTable ldtbResult = busBase.Select("cdoBenefitCalculation.GetBenefitAccount",
                                          new object[2] { aintPersonAccountID, astrBenefitAccountType });
            if (ldtbResult.Rows.Count > 0)
            {
                lobjbenefitaccount.icdoBenefitAccount.LoadData(ldtbResult.Rows[0]);
            }
            return lobjbenefitaccount;
        }
        public int ManageBenefitAccount(decimal adecStartingTaxableAmount, decimal adecStartingNonTaxableAmount, string astrStatusValue,
                                  string astrRHICBenefitOptionValue, decimal adecRHICBenefitAmount, decimal adecPSC, decimal adecTVSC,
                                    int aintRetirementOrgID, int aintBenefitAccountId, DateTime adtSSLIChangeDate, decimal adecEstmSSBenefitAmount,
                                    string astrRuleIndicatorValue, decimal adecOptionFactor, decimal adecRHICOptionFactor)
        {
            return ManageBenefitAccount(adecStartingTaxableAmount, adecStartingNonTaxableAmount, astrStatusValue,
                                           astrRHICBenefitOptionValue, adecRHICBenefitAmount, adecPSC, adecTVSC,
                                             aintRetirementOrgID, aintBenefitAccountId, adtSSLIChangeDate, adecEstmSSBenefitAmount,
                                             astrRuleIndicatorValue, adecOptionFactor, adecRHICOptionFactor, 0);
        }

        public int ManageBenefitAccount(decimal adecStartingTaxableAmount, decimal adecStartingNonTaxableAmount, string astrStatusValue,
                                          string astrRHICBenefitOptionValue, decimal adecRHICBenefitAmount, decimal adecPSC, decimal adecTVSC,
                                            int aintRetirementOrgID, int aintBenefitAccountId, DateTime adtSSLIChangeDate, decimal adecEstmSSBenefitAmount,
                                            string astrRuleIndicatorValue, decimal adecOptionFactor, decimal adecRHICOptionFactor, decimal adecSpouseRHICAmount)
        {
            if (astrRHICBenefitOptionValue != null)
            {
                busBenefitAccount lobjBenefitAccount = new busBenefitAccount();
                if (aintBenefitAccountId > 0)
                {
                    lobjBenefitAccount.FindBenefitAccount(aintBenefitAccountId);
                }
                else
                {
                    lobjBenefitAccount.icdoBenefitAccount = new cdoBenefitAccount();
                }
                lobjBenefitAccount.icdoBenefitAccount.starting_taxable_amount = adecStartingTaxableAmount;
                lobjBenefitAccount.icdoBenefitAccount.starting_nontaxable_amount = adecStartingNonTaxableAmount;
                lobjBenefitAccount.icdoBenefitAccount.status_value = astrStatusValue;
                lobjBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value = astrRHICBenefitOptionValue;
                lobjBenefitAccount.icdoBenefitAccount.rhic_benefit_amount = adecRHICBenefitAmount;
                lobjBenefitAccount.icdoBenefitAccount.pension_service_credit = adecPSC;
                lobjBenefitAccount.icdoBenefitAccount.total_vested_service_credit = adecTVSC;
                lobjBenefitAccount.icdoBenefitAccount.retirement_org_id = aintRetirementOrgID;
                lobjBenefitAccount.icdoBenefitAccount.ssli_change_date = adtSSLIChangeDate;
                lobjBenefitAccount.icdoBenefitAccount.estimated_ss_benefit_amount = adecEstmSSBenefitAmount;
                lobjBenefitAccount.icdoBenefitAccount.rule_indicator_value = astrRuleIndicatorValue;
                lobjBenefitAccount.icdoBenefitAccount.option_factor = adecOptionFactor;
                lobjBenefitAccount.icdoBenefitAccount.rhic_option_factor = adecRHICOptionFactor;
                lobjBenefitAccount.icdoBenefitAccount.spouse_rhic_amount = adecSpouseRHICAmount;
                if (aintBenefitAccountId > 0)
                {
                    if (lobjBenefitAccount.icdoBenefitAccount.Update() == 1)
                        return lobjBenefitAccount.icdoBenefitAccount.benefit_account_id;
                }
                else
                {
                    if (lobjBenefitAccount.icdoBenefitAccount.Insert() == 1)
                        return lobjBenefitAccount.icdoBenefitAccount.benefit_account_id;
                }
            }
            return 0;
        }
        public static DateTime GetNormalRetirementDateBasedOnNormalEligibility(int aintPlanID, string astrPlanCode, int aintProvisionID,
                                        string astrBenefitAccountType, DateTime adtPersonDateOfBirth, decimal adecTVSC,
                                        int aintConversionByRuleorAge, utlPassInfo aobjPassInfo, DateTime adtTerminationDate,
                                        int aintPersonAccountId, bool isCalTypeEstimate, decimal adecServiceEntered, DateTime? adteRetirementDate = null,
                                        busDBCacheData aobjDBCacheData = null, busPersonAccount abusPersonAccount = null)
        {
            DateTime ldteNRDbyAge = new DateTime();
            DateTime ldteNRDbyRule = new DateTime();
            if (adteRetirementDate.IsNull())
                adteRetirementDate = DateTime.MinValue; //PIR 14646

            return GetNormalRetirementDateBasedOnNormalEligibility(aintPlanID, astrPlanCode, aintProvisionID,
                                         astrBenefitAccountType, adtPersonDateOfBirth, adecTVSC,
                                         aintConversionByRuleorAge, aobjPassInfo, adtTerminationDate,
                                         aintPersonAccountId, isCalTypeEstimate, adecServiceEntered,
                                         ref ldteNRDbyAge, ref ldteNRDbyRule, false, adteRetirementDate, aobjDBCacheData, abusPersonAccount);
        }
        /// <summary>
        /// Get Normal retirement date based on the member eligibility
        /// </summary>
        /// <param name="aintPlanID"></param>
        /// <param name="astrPlanCode"></param>
        /// <param name="aintProvisionID"></param>
        /// <param name="astrBenefitAccountType"></param>
        /// <param name="adtPersonDateOfBirth"></param>
        /// <param name="adecTVSC"></param>
        /// <returns></returns>
        public static DateTime GetNormalRetirementDateBasedOnNormalEligibility(int aintPlanID, string astrPlanCode, int aintProvisionID,
                                        string astrBenefitAccountType, DateTime adtPersonDateOfBirth, decimal adecTVSC,
                                        int aintConversionByRuleorAge, utlPassInfo aobjPassInfo, DateTime adtTerminationDate,
                                        int aintPersonAccountId, bool isCalTypeEstimate, decimal adecServiceEntered,
                                        ref DateTime adteNRDAge, ref DateTime adteNRDRule,
                                        bool IsVSCProjectiontobeExcluded, DateTime? adteRetirementDate,
                                        busDBCacheData aobjDBCacheData = null, busPersonAccount abusPersonAccount = null)
        {
            DateTime ldtNormalRetirementDate = DateTime.MinValue;
            DateTime ldtNormalRetirementDate2 = DateTime.MinValue;
            decimal ldecTVSCMonthsToBeSubtracted = 0;
            int lintNoOfYearsToBeAdded1 = 0;
            if (abusPersonAccount.IsNull())
            {
                abusPersonAccount = new busPersonAccount();
                abusPersonAccount.FindPersonAccount(aintPersonAccountId);
            }

            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionNormalEligibility = LoadEligibilityForPlan(aintPlanID, aintProvisionID, astrBenefitAccountType, busConstant.BenefitProvisionEligibilityNormal, aobjPassInfo, abusPersonAccount?.icdoPersonAccount?.start_date, aobjDBCacheData);
            //PIR 14646
            if (abusPersonAccount.IsNotNull() && aintPersonAccountId > 0)
            {
                lclbBenefitProvisionNormalEligibility = LoadEligibilityForPlanByBenefitTier(aintPlanID, astrBenefitAccountType, abusPersonAccount, lclbBenefitProvisionNormalEligibility);
            }
            if (lclbBenefitProvisionNormalEligibility.Count > 0)
            {
                if (aintPlanID != busConstant.PlanIdJobService)
                {
                    lintNoOfYearsToBeAdded1 = (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age));
                }
                else
                {
                    if (adecTVSC >= lclbBenefitProvisionNormalEligibility[1].service)
                    {
                        lintNoOfYearsToBeAdded1 = lclbBenefitProvisionNormalEligibility[1].age;
                    }
                    else if (adecTVSC >= lclbBenefitProvisionNormalEligibility[3].service)
                    {
                        lintNoOfYearsToBeAdded1 = lclbBenefitProvisionNormalEligibility[3].age;
                    }
                    else if (adecTVSC >= lclbBenefitProvisionNormalEligibility[2].service)
                    {
                        lintNoOfYearsToBeAdded1 = lclbBenefitProvisionNormalEligibility[2].age;
                    }
                    else
                    {
                        lintNoOfYearsToBeAdded1 = lclbBenefitProvisionNormalEligibility[0].age;
                    }
                }

                DateTime ldtDateOnWhichMemberAttainsNormalEligibility1 = adtPersonDateOfBirth.AddYears(lintNoOfYearsToBeAdded1);
                DateTime ldtNormalRetirementDate1 = ldtDateOnWhichMemberAttainsNormalEligibility1.AddMonths(1);
                ldtNormalRetirementDate1 = new DateTime(ldtNormalRetirementDate1.Year, ldtNormalRetirementDate1.Month, 1);
                DateTime ldtProjectedRetirementDate = DateTime.MinValue;
                //PIR 26282
                if (lclbBenefitProvisionNormalEligibility[0].age_while_employed_flag != busConstant.Flag_Yes 
                    && aintPlanID != busConstant.PlanIdJobService)
                {
                    if(abusPersonAccount.icdoPersonAccount.person_account_id > 0)
                    {
                        if (abusPersonAccount.idtProjectedRetirementDateByService == DateTime.MinValue)
                            abusPersonAccount.CalculateProjectedRetirementDateByService(lclbBenefitProvisionNormalEligibility[0].service, isCalTypeEstimate, adecServiceEntered);
                        ldtProjectedRetirementDate = abusPersonAccount.idtProjectedRetirementDateByService;
                    }
                    if (lclbBenefitProvisionNormalEligibility[0].service <= 0)
                    {
                        DateTime ldteFromDate = adtPersonDateOfBirth.AddMonths(1);
                        int lintTotalMonths = 0;
                        int lintMemberAgeMonthPart = 0;
                        int lintMemberAgeYearPart = 0;
                        decimal adecMonthAndYear = 0M;

                        CalculateAge(ldteFromDate, ldtNormalRetirementDate1, ref lintTotalMonths, ref adecMonthAndYear,
                            4, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);

                        if (CheckIsPersonVested(aintPlanID, astrPlanCode, aintProvisionID, astrBenefitAccountType, adecTVSC, lintMemberAgeYearPart,
                                adtTerminationDate, isCalTypeEstimate, adtTerminationDate, abusPersonAccount, aobjPassInfo))
                        {
                            if (abusPersonAccount.IsNotNull())
                            {
                                DateTime ldtmLastContributionDate = abusPersonAccount.LoadLastSalaryWithoutPersonAccount();
                                ldtProjectedRetirementDate = ldtmLastContributionDate.AddMonths(1);
                                ldtProjectedRetirementDate = new DateTime(ldtProjectedRetirementDate.Year, ldtProjectedRetirementDate.Month, 1);
                            }
                        }
                    }
                }
                if (ldtProjectedRetirementDate != DateTime.MinValue && ldtProjectedRetirementDate > ldtNormalRetirementDate1)
                    ldtNormalRetirementDate1 = ldtProjectedRetirementDate;

                if ((aintPlanID == busConstant.PlanIdMain)
                    || (aintPlanID == busConstant.PlanIdHP)
                    || (aintPlanID == busConstant.PlanIdJudges)
                    || (aintPlanID == busConstant.PlanIdLE)
                    || (aintPlanID == busConstant.PlanIdLEWithoutPS)
                    || (aintPlanID == busConstant.PlanIdDC) //UAT PIR:1164....DC plan Also added for Rule of 85
                   || (aintPlanID == busConstant.PlanIdBCILawEnf) 
                    || (aintPlanID == busConstant.PlanIdStatePublicSafety) //PIR 25729
                    || (aintPlanID == busConstant.PlanIdNG) // PIR 25729
                    || (aintPlanID == busConstant.PlanIdMain2020) //PIR 20232
                    || (aintPlanID == busConstant.PlanIdDC2020) //PIR 20232
                    || (aintPlanID == busConstant.PlanIdDC2025)) //PIR 25920 //pir 7943
                {
                    //if (adecTVSC != 0.00M)
                    //{
                    //    ldecTVSCMonthsToBeSubtracted = Convert.ToDecimal((adecTVSC - 1) / 12);
                    //}
                    //int lintNoOfMonthsToBeAdded2 = Convert.ToInt32((((Convert.ToDecimal(lclbBenefitProvisionNormalEligibility[0].age_plus_service)) - ldecTVSCMonthsToBeSubtracted)) * 12);
                    //DateTime ldtDateOnWhichMemberAttainsNormalEligibility2 = adtPersonDateOfBirth.AddMonths(lintNoOfMonthsToBeAdded2);
                    //ldtNormalRetirementDate2 = ldtDateOnWhichMemberAttainsNormalEligibility2.AddMonths(1);
                    //ldtNormalRetirementDate2 = new DateTime(ldtNormalRetirementDate2.Year, ldtNormalRetirementDate2.Month, 1);

                    /*******************/

                    decimal ldecTargetTVSC = lclbBenefitProvisionNormalEligibility[0].age_plus_service * 12;
                    if (abusPersonAccount == null)
                    {
                        ldtNormalRetirementDate2 = GetNormalRetirementDateByRuleOf80Or85(adtPersonDateOfBirth, ldecTargetTVSC,
                                                             adtTerminationDate, aintPersonAccountId, isCalTypeEstimate,
                                                             adecServiceEntered, aobjPassInfo);
                    }
                    else
                    {
                        ldtNormalRetirementDate2 = GetNormalRetirementDateByRuleOf80Or85(adtPersonDateOfBirth, ldecTargetTVSC,
                                                             adtTerminationDate, abusPersonAccount, isCalTypeEstimate,
                                                             adecServiceEntered, aobjPassInfo, IsVSCProjectiontobeExcluded);
                    }
                }

                ldtNormalRetirementDate = ldtNormalRetirementDate1;
                adteNRDAge = ldtNormalRetirementDate1;
                adteNRDRule = ldtNormalRetirementDate2;
                //PIR 14646
                if (lclbBenefitProvisionNormalEligibility.Count > 0 
                    && aintConversionByRuleorAge != busConstant.ConvertedToNormalByRule
                    && astrBenefitAccountType == busConstant.ApplicationBenefitTypeRetirement)
                {
                    int lintMembersAgeInMonthsAsOnRule = 0;
                    decimal ldecMemberAgeBasedOnRule = 0.0M;
                    int lintMemberAgeYearPart = 0;
                    int lintMemberAgeMonthPart = 0;

                    //PIR-16343 calculating Person's age as on rule met date and increasing NRD by rule if it do not meets minimum age requirements.
                    //PIR 24069 Trying to generate a b.e. for Tashe Jensen 260504, getting error msg even included the service purchse-- so adding 1 month into person date of birth
                    CalculateAge(adtPersonDateOfBirth.AddMonths(1), ldtNormalRetirementDate2,
                                ref lintMembersAgeInMonthsAsOnRule, ref ldecMemberAgeBasedOnRule, 2, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);
                    if (lclbBenefitProvisionNormalEligibility[0].minimum_age >0 && lintMemberAgeYearPart < lclbBenefitProvisionNormalEligibility[0].minimum_age)
                    {
                        DateTime ldtDateOnWhichMemberAttainsMinAge = adtPersonDateOfBirth.AddYears(lclbBenefitProvisionNormalEligibility[0].minimum_age);
                        ldtNormalRetirementDate2 = ldtDateOnWhichMemberAttainsMinAge.AddMonths(1);
                        ldtNormalRetirementDate2 = new DateTime(ldtNormalRetirementDate2.Year, ldtNormalRetirementDate2.Month, 1);
                        adteNRDRule = ldtNormalRetirementDate2;
                    }
                }
                if ((ldtNormalRetirementDate1 > ldtNormalRetirementDate2) && (ldtNormalRetirementDate2 != DateTime.MinValue))
                    ldtNormalRetirementDate = ldtNormalRetirementDate2;

                // UCS-080 - BR-080-03 - Convert Disability to Normal Payee Account
                if (aintConversionByRuleorAge == busConstant.ConvertedToNormalByRule)
                    ldtNormalRetirementDate = ldtNormalRetirementDate2;
                if (aintConversionByRuleorAge == busConstant.ConvertedToNormalByAge)
                    ldtNormalRetirementDate = ldtNormalRetirementDate1;
            }
            return ldtNormalRetirementDate;
        }

        public static DateTime GetNormalRetirementDateByRuleOf80Or85(DateTime adtDateOfBirth, decimal adecTargetVSC,
                                      DateTime adtTerminationDate, int aintPersonAccountId, bool IsCalTypeEstimate,
                                      decimal adecVSCEntered, utlPassInfo aobjPassInfo)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            lobjPersonAccount.FindPersonAccount(aintPersonAccountId);
            return GetNormalRetirementDateByRuleOf80Or85(adtDateOfBirth, adecTargetVSC,
                                      adtTerminationDate, lobjPersonAccount, IsCalTypeEstimate,
                                      adecVSCEntered, aobjPassInfo, false);

        }

        public static DateTime GetNormalRetirementDateByRuleOf80Or85(DateTime adtDateOfBirth, decimal adecTargetVSC,
                              DateTime adtTerminationDate, busPersonAccount abusPersonAccount, bool IsCalTypeEstimate,
                              decimal adecVSCEntered, utlPassInfo aobjPassInfo)
        {
            return GetNormalRetirementDateByRuleOf80Or85(adtDateOfBirth, adecTargetVSC,
                                       adtTerminationDate, abusPersonAccount, IsCalTypeEstimate,
                                       adecVSCEntered, aobjPassInfo, false);
        }

        public static DateTime GetNormalRetirementDateByRuleOf80Or85(DateTime adtDateOfBirth, decimal adecTargetVSC,
                                      DateTime adtTerminationDate, busPersonAccount abusPersonAccount, bool IsCalTypeEstimate,
                                      decimal adecVSCEntered, utlPassInfo aobjPassInfo, bool IsVSCProjectiontobeExcluded)
        {
            DateTime ldtNormalRetirementDate = DateTime.MinValue;
            DateTime ldtMinimumStartDate = DateTime.MinValue;
            DateTime ldtMaximumStartDate = DateTime.MinValue;
            DateTime ldtCalculationStartDate = DateTime.MinValue;

            int lintYears = 0;
            int lintMonths = 0;
            decimal ldecMonthsDiffBetDOBAndCalStartDate = 0;
            decimal ldecExactVSC = 0;

            var lenumPAContributionFilteredList = new List<busPersonAccountRetirementContribution>();

            if (abusPersonAccount.ibusPerson.IsNull())
                abusPersonAccount.LoadPerson();

            if (abusPersonAccount.ibusPerson.icolPersonAccount.IsNull())
                abusPersonAccount.ibusPerson.LoadPersonAccount();

            //step 1 a : for plans other than job service
            if (abusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
            {
                var lenumPersonAccount = abusPersonAccount.ibusPerson.icolPersonAccount
                                                       .Where(lobjPA => lobjPA.icdoPersonAccount.plan_id != busConstant.PlanIdJobService &&
                                                           lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn)
                                                        .OrderByDescending(lobjPA => lobjPA.icdoPersonAccount.start_date);
                if (lenumPersonAccount.Count() > 0)
                    ldtMinimumStartDate = lenumPersonAccount.Last().icdoPersonAccount.start_date;
            }
            else
            {
                //step 1 b : for plan job service             
                var lenumPersonAccount = abusPersonAccount.ibusPerson.icolPersonAccount
                                                       .Where(lobjPA => lobjPA.icdoPersonAccount.plan_id == busConstant.PlanIdJobService)
                                                        .OrderByDescending(lobjPA => lobjPA.icdoPersonAccount.start_date);
                if (lenumPersonAccount.Count() > 0)
                    ldtMinimumStartDate = lenumPersonAccount.Last().icdoPersonAccount.start_date;
            }

            //Step 1 b Now from the Contributions Find the Least Effective_date.
            //UAT PIR: 1705. To Find the least of the Contribution Date and Compare it with the least of plan Start date. Then Set the least of the them.
            //step 1b - Get maximum start date and least start date.                    
            DateTime ldtContributionleastDate = DateTime.MinValue;

            //for plans other than job service
            if (abusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
            {
                if (abusPersonAccount.ibusPerson.iclbRetirementContributionExcludeJobService.IsNull())
                    abusPersonAccount.ibusPerson.LoadRetContritbutionExcludeJobService();

                lenumPAContributionFilteredList = abusPersonAccount.ibusPerson.iclbRetirementContributionExcludeJobService
                    .Where(lobjPA => lobjPA.icdoPersonAccountRetirementContribution.vested_service_credit != 0 &&
                        lobjPA.icdoPersonAccountRetirementContribution.subsystem_value != busConstant.SubSystemValueBenefitPayment).ToList(); //PIR 14890 - Do not consider contributions of PMNT when determining the NRD
            }
            else
            {
                if (abusPersonAccount.ibusPerson.iclbJobServiceRetirementContribution.IsNull())
                    abusPersonAccount.ibusPerson.LoadJobServiceRetContritbution();

                lenumPAContributionFilteredList = abusPersonAccount.ibusPerson.iclbJobServiceRetirementContribution
               .Where(lobjPA => lobjPA.icdoPersonAccountRetirementContribution.vested_service_credit != 0 &&
                        lobjPA.icdoPersonAccountRetirementContribution.subsystem_value != busConstant.SubSystemValueBenefitPayment).ToList(); //PIR 14890 - Do not consider contributions of PMNT when determining the NRD
            }

            var lenumContributionByEffectiveDate = lenumPAContributionFilteredList.OrderByDescending(lobjPA =>
                                                                        lobjPA.icdoPersonAccountRetirementContribution.effective_date);
            if (lenumContributionByEffectiveDate.Count() > 0)
            {
                ldtMaximumStartDate = lenumContributionByEffectiveDate.First().icdoPersonAccountRetirementContribution.effective_date;
                ldtContributionleastDate = lenumContributionByEffectiveDate.Last().icdoPersonAccountRetirementContribution.effective_date;
            }

            if ((ldtContributionleastDate != DateTime.MinValue)
                && (ldtContributionleastDate < ldtMinimumStartDate))
            {
                ldtMinimumStartDate = ldtContributionleastDate;
            }

            if (ldtMinimumStartDate != DateTime.MinValue)
            {
                ldtCalculationStartDate = ldtMinimumStartDate.AddMonths(-1);

                DateTime ldtFromDate = new DateTime(adtDateOfBirth.Year, adtDateOfBirth.Month, 01);
                DateTime ldtToDate = new DateTime(ldtCalculationStartDate.Year, ldtCalculationStartDate.Month, 01);

                //step 2 - calculate the difference in months between date of birth and calculation start date
                int lintDiffInMonths = HelperFunction.GetMonthSpan(ldtFromDate, ldtToDate, out lintYears, out lintMonths);
                ldecMonthsDiffBetDOBAndCalStartDate = Convert.ToDecimal(lintDiffInMonths);

                //step 3 - get exact TVSC
                ldecExactVSC = adecTargetVSC - ldecMonthsDiffBetDOBAndCalStartDate;

                //get exact VSC for calculation type estimate TODO
                //Code Commented Since if it is Estimate then adecVSCEntered will have Value otherwise zero.Also in Final Retirement Calculation the TFFR -TIAA Service needs to be included.
                //if (IsCalTypeEstimate)
                //{
                ldecExactVSC = ldecExactVSC - adecVSCEntered;
                //}

                //step 5 - get normal retirement date               
                //DataTable ldtList = busBase.Select("cdoBenefitApplication.GetNRDDetails", new object[4]
                //                                                                { ldtMinimumStartDate, ldtMaximumStartDate, 
                //                                                                abusPersonAccount.icdoPersonAccount.person_id,
                //                                                                abusPersonAccount.icdoPersonAccount.plan_id });

                DataTable ldtList = LoadNDRDetails(ldtMinimumStartDate, ldtMaximumStartDate, abusPersonAccount);

                if (ldtList.Rows.Count > 0)
                {
                    var lenumGetFilteredListByCumTotal = ldtList.AsEnumerable().Where(dr => dr.Field<decimal>("CumTotal") >= ldecExactVSC);

                    if (lenumGetFilteredListByCumTotal.AsDataTable().Rows.Count > 0)
                    {
                        var lenumListOrderedByCumTotal = lenumGetFilteredListByCumTotal.AsEnumerable().First();
                        if (lenumListOrderedByCumTotal.ItemArray.Count() > 0)
                        {
                            int lintNRDMonth = 0;
                            int lintNRDYear = 0;
                            //PIR: 1965
                            //NRD is the date arrived after adding 1 month. 
                            lintNRDMonth = lenumListOrderedByCumTotal.Field<int>("Month");
                            lintNRDYear = lenumListOrderedByCumTotal.Field<int>("year");

                            ldtNormalRetirementDate = new DateTime(lintNRDYear, lintNRDMonth, 1);
                            ldtNormalRetirementDate = ldtNormalRetirementDate.AddMonths(1);
                        }
                    }
                    else
                    {
                        var ldtblistOrderedByCumTotal = ldtList.AsEnumerable().OrderByDescending(dr => dr.Field<decimal>("CumTotal")).First();

                        DateTime ldtBeginDate = DateTime.MinValue;
                        DateTime ldtSeasonalStartDate = DateTime.MinValue;
                        DateTime ldtSeasonalEndDate = DateTime.MinValue;

                        bool lblnIsNRDReached = false;
                        int lintLastMonth = 0;
                        int lintLastYear = 0;
                        decimal ldecLastCumTotal = 0.00M;

                        lintLastMonth = ldtblistOrderedByCumTotal.Field<int>("Month");
                        lintLastYear = ldtblistOrderedByCumTotal.Field<int>("year");
                        ldecLastCumTotal = Math.Round(ldtblistOrderedByCumTotal.Field<decimal>("CumTotal"), 4, MidpointRounding.AwayFromZero);

                        ldtBeginDate = new DateTime(lintLastYear, lintLastMonth, 1);

                        //seasonal
                        if (!abusPersonAccount.ibusPerson.iblnIsEmploymentSeasonalLoaded)
                            abusPersonAccount.ibusPerson.LoadEmploymentSeasonal();
                        ldtSeasonalStartDate = new DateTime(ldtBeginDate.Year, 7, 1);
                        ldtSeasonalEndDate = ldtSeasonalStartDate.AddMonths(abusPersonAccount.ibusPerson.iintSeasonalMonths - 1);

                        // The Begin date should be the next of the month of the last contribution
                        ldtBeginDate = ldtBeginDate.AddMonths(1);

                        if (ldtBeginDate != DateTime.MinValue)
                        {

                            Boolean IsPersonNonContributing = CheckIfEmploymentISContributingStatus(abusPersonAccount);
                            while (!lblnIsNRDReached)
                            {
                                decimal ldecAgeVSC = 1;
                                decimal lintNumberVSCContributionsToincrease = 1;

                                if(!IsVSCProjectiontobeExcluded)
                                {
                                    //check 
                                    if ((ldtBeginDate > adtTerminationDate) && (adtTerminationDate != DateTime.MinValue))
                                    {
                                        lintNumberVSCContributionsToincrease = 0;
                                    }
                                    if ((adtTerminationDate != DateTime.MinValue) && (ldtBeginDate <= adtTerminationDate) && !IsCalTypeEstimate)
                                    {
                                        lintNumberVSCContributionsToincrease = 0; //PIR 14890 - Remove logic to project service credit for Application and Final Calculation                                   
                                    }
                                    if (!busGlobalFunctions.CheckDateOverlapping(ldtBeginDate, abusPersonAccount.icdoPersonAccount.start_date,
                                                                                                         abusPersonAccount.icdoPersonAccount.end_date))
                                    {
                                        lintNumberVSCContributionsToincrease = 0;
                                    }
                                    if (abusPersonAccount.ibusPerson.iblnIsEmploymentSeasonal)
                                    {
                                        if (!busGlobalFunctions.CheckDateOverlapping(ldtBeginDate, ldtSeasonalStartDate, ldtSeasonalEndDate))
                                        {
                                            lintNumberVSCContributionsToincrease = 0;
                                        }
                                    }
                                    if (!IsPersonNonContributing && IsCalTypeEstimate)
                                    {
                                        lintNumberVSCContributionsToincrease = 0;
                                    }
                                }

                                //get new cumulative total
                                ldecLastCumTotal = ldecLastCumTotal + ldecAgeVSC + lintNumberVSCContributionsToincrease;

                                if (ldecLastCumTotal >= ldecExactVSC)
                                {
                                    lblnIsNRDReached = true;
                                    break;
                                }
                                ldtBeginDate = ldtBeginDate.AddMonths(1);

                                //if seasonal employee
                                if ((ldtBeginDate.Month == 7) && (ldtBeginDate.Day == 1))
                                {
                                    if (abusPersonAccount.ibusPerson.iblnIsEmploymentSeasonal)
                                    {
                                        ldtSeasonalStartDate = new DateTime(ldtBeginDate.Year, 07, 01);
                                        ldtSeasonalEndDate = ldtSeasonalStartDate.AddMonths(abusPersonAccount.ibusPerson.iintSeasonalMonths - 1);
                                    }
                                }
                            }
                            ldtNormalRetirementDate = ldtBeginDate.AddMonths(1);
                        }
                    }
                }
            }
            return ldtNormalRetirementDate;
        }

        private static DataTable LoadNDRDetails(DateTime adtFromDate, DateTime adtToDate, busPersonAccount aobjPersonAccount)
        {
            var lenumPAContributionFilteredList = new List<busPersonAccountRetirementContribution>();

            if (aobjPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
            {
                if (aobjPersonAccount.ibusPerson.iclbRetirementContributionExcludeJobService.IsNull())
                    aobjPersonAccount.ibusPerson.LoadRetContritbutionExcludeJobService();

                lenumPAContributionFilteredList = aobjPersonAccount.ibusPerson.iclbRetirementContributionExcludeJobService
                    .Where(lobjPA => lobjPA.icdoPersonAccountRetirementContribution.vested_service_credit != 0 &&
                        lobjPA.icdoPersonAccountRetirementContribution.subsystem_value != busConstant.SubSystemValueBenefitPayment).ToList(); //PIR 14890 - Do not consider contributions of PMNT when determining the NRD
            }
            else
            {
                if (aobjPersonAccount.ibusPerson.iclbJobServiceRetirementContribution.IsNull())
                    aobjPersonAccount.ibusPerson.LoadJobServiceRetContritbution();

                lenumPAContributionFilteredList = aobjPersonAccount.ibusPerson.iclbJobServiceRetirementContribution
               .Where(lobjPA => lobjPA.icdoPersonAccountRetirementContribution.vested_service_credit != 0 &&
                        lobjPA.icdoPersonAccountRetirementContribution.subsystem_value != busConstant.SubSystemValueBenefitPayment).ToList(); //PIR 14890 - Do not consider contributions of PMNT when determining the NRD
            }

            DataTable lResultDatatable = new DataTable();
            decimal ldeCummTotal = 0.00M;

            //declare columns
            DataColumn ldc1 = new DataColumn("Month", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("Year", Type.GetType("System.Int32"));
            DataColumn ldc3 = new DataColumn("AgePSC", Type.GetType("System.Decimal"));
            DataColumn ldc4 = new DataColumn("ActualPSC", Type.GetType("System.Decimal"));
            DataColumn ldc5 = new DataColumn("CumTotal", Type.GetType("System.Decimal"));

            lResultDatatable.Columns.Add(ldc1);
            lResultDatatable.Columns.Add(ldc2);
            lResultDatatable.Columns.Add(ldc3);
            lResultDatatable.Columns.Add(ldc4);
            lResultDatatable.Columns.Add(ldc5);

            while (adtFromDate <= adtToDate)
            {
                //fill data table
                DataRow dr = lResultDatatable.NewRow();
                dr["Month"] = adtFromDate.Month;
                dr["Year"] = adtFromDate.Year;
                dr["AgePSC"] = 1;

                //get vsc for from date
                var ldecSumOfTVSC = lenumPAContributionFilteredList
                                        .Where(lobjPACont => (lobjPACont.icdoPersonAccountRetirementContribution.pay_period_month == adtFromDate.Month
                                        && lobjPACont.icdoPersonAccountRetirementContribution.pay_period_year == adtFromDate.Year)
                                        || (lobjPACont.icdoPersonAccountRetirementContribution.effective_date.Month == adtFromDate.Month
                                        && lobjPACont.icdoPersonAccountRetirementContribution.effective_date.Year == adtFromDate.Year))
                                        .Sum(lobjPAC => lobjPAC.icdoPersonAccountRetirementContribution.vested_service_credit);
                dr["ActualPSC"] = ldecSumOfTVSC;

                //sum cum total
                ldeCummTotal = ldeCummTotal + ldecSumOfTVSC + Convert.ToDecimal(dr["AgePSC"]);

                adtFromDate = adtFromDate.AddMonths(1);
                dr["CumTotal"] = ldeCummTotal;

                lResultDatatable.Rows.Add(dr);
            }

            return lResultDatatable;
        }

        public static DateTime GetNormalRetirementDateBasedOnNormalEligibilityForDRO(int aintPlanID, string astrPlanCode, int aintProvisionID, string astrBenefitAccountType, DateTime adtPersonDateOfBirth, decimal adecTVSC, utlPassInfo aobjPassInfo, busPersonAccount abusPersonAccount)
        {
            DateTime ldtNormalRetirementDate = DateTime.MinValue;
            DateTime ldtNormalRetirementDate2 = DateTime.MinValue;
            int lintTVSCMonthsToBeSubtracted = 0;
            int lintNoOfYearsToBeAdded1 = 0;

            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionNormalEligibility = LoadEligibilityForPlan(aintPlanID, aintProvisionID, astrBenefitAccountType, busConstant.BenefitProvisionEligibilityNormal, aobjPassInfo, abusPersonAccount?.icdoPersonAccount?.start_date); ;

            if (lclbBenefitProvisionNormalEligibility.Count > 0)
            {
                if (aintPlanID != busConstant.PlanIdJobService)
                {
                    lintNoOfYearsToBeAdded1 = (Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age));
                }
                else
                {
                    if (adecTVSC >= lclbBenefitProvisionNormalEligibility[1].service)
                    {
                        lintNoOfYearsToBeAdded1 = lclbBenefitProvisionNormalEligibility[1].age;
                    }
                    else if (adecTVSC >= lclbBenefitProvisionNormalEligibility[3].service)
                    {
                        lintNoOfYearsToBeAdded1 = lclbBenefitProvisionNormalEligibility[3].age;
                    }
                    else if (adecTVSC >= lclbBenefitProvisionNormalEligibility[2].service)
                    {
                        lintNoOfYearsToBeAdded1 = lclbBenefitProvisionNormalEligibility[2].age;
                    }
                    else
                    {
                        lintNoOfYearsToBeAdded1 = lclbBenefitProvisionNormalEligibility[0].age;
                    }
                }
            }
            DateTime ldtDateOnWhichMemberAttainsNormalEligibility1 = adtPersonDateOfBirth.AddYears(lintNoOfYearsToBeAdded1);
            return ldtDateOnWhichMemberAttainsNormalEligibility1;
        }

        //get Early retirement date based on the early retirement eligibility
        public static DateTime GetEarlyRetirementDateBasedOnEarlyRetirement(int aintPlanID, int aintBenefitProvisionID, string astrBenefitAccountType,
                                                                                                                     DateTime adtMemberDateOfBirth, decimal adecTVSC, utlPassInfo aobjPassInfo, busPersonAccount abusPersonAccount)
        {
            int lintNoOfYearsToBeAdded1 = 0;
            DateTime ldtEarlyRetirementDate = DateTime.MinValue;

            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionEarlyEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionEarlyEligibility = LoadEligibilityForPlan(aintPlanID, aintBenefitProvisionID, astrBenefitAccountType, busConstant.BenefitProvisionEligibilityEarly, aobjPassInfo, abusPersonAccount?.icdoPersonAccount?.start_date);

            if (aintPlanID != busConstant.PlanIdJobService)
            {
                lintNoOfYearsToBeAdded1 = Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[0].age);
            }
            else
            {
                if (adecTVSC >= lclbBenefitProvisionEarlyEligibility[1].age_plus_service)
                {
                    lintNoOfYearsToBeAdded1 = lclbBenefitProvisionEarlyEligibility[1].age;
                }
                else if (adecTVSC >= lclbBenefitProvisionEarlyEligibility[2].age_plus_service)
                {
                    lintNoOfYearsToBeAdded1 = lclbBenefitProvisionEarlyEligibility[2].age;
                }
                else if (adecTVSC >= lclbBenefitProvisionEarlyEligibility[3].age_plus_service)
                {
                    lintNoOfYearsToBeAdded1 = lclbBenefitProvisionEarlyEligibility[3].age;
                }
                else
                {
                    lintNoOfYearsToBeAdded1 = lclbBenefitProvisionEarlyEligibility[0].age;
                }
            }
            ldtEarlyRetirementDate = adtMemberDateOfBirth.AddYears(lintNoOfYearsToBeAdded1);

            return ldtEarlyRetirementDate;
        }

        //UAT PIR: 1687 DNRO when there is open Employment.
        /*Single plan accounts:HP,Job Service,Main,NG,LE,Judges,DC --Fetch latest employment header end date
        Multiple plan accounts:
        If member has plan accounts in Enrolled or Suspended status for any combination of Main, LE, NG or Judges and still has open employment with Main, LE, NG or Judges, the member cannot retire in Main, LE, NG or Judges.  I.e.  Main and NG, termed in Main, employed in NG – cannot retire in Main
        If member has plan accounts in Enrolled or Suspended status for any combination of HP or Job Service and still has open employment with HP or Job Service, the member can retire in HP or Job Service.  I.e.  Job Service and HP, termed in HP, employed in Job Service – can retire in HP
        If member has plan accounts in Enrolled or Suspended status for Main, LE, NG or Judges and HP or Job Service 
        •	and still has open employment with HP or Job Service, the member cannot retire in Main, LE, NG or Judges.  I.e.  Main and HP, termed in Main, employed in HP – cannot retire in Main
        •	and still has open employment with Main, LE, NG or Judges, the member can retire in HP or Job Service.  I.e.  Main and HP, termed in HP, employed in Main – can retire in HP
        Special condition:
        Job Service – If member has a plan account for Job Service and also has a plan account for Main but never changed employers (still employed with Job Service org) then they are not eligible to retire in Job Service until employment with Job Service org is terminated.  
        HP – If member has a plan account for HP and also has a plan account for Main but never changed employers (still employed with Highway Patrol org) then they are not eligible to retire in HP until employment with Highway Patrol org is terminated.  
         */
        public int GetLastPersonEmploymentEndDate(busPersonAccount abusPersonAccount, ref DateTime adtEmploymentEnddate)
        {
            bool iblnIsMemberDual = IsMemberDualAcrossAllPlans(abusPersonAccount);
            DateTime ldteEmploymentEndDate = DateTime.MinValue;
            int lintEmploymentOrgid = 0;

            // All Employment ir-respective of Plan
            if (abusPersonAccount.ibusPerson == null)
                abusPersonAccount.LoadPerson();
            if (abusPersonAccount.ibusPerson.icolPersonEmployment.IsNull())
                abusPersonAccount.ibusPerson.LoadPersonEmployment();
            busPersonEmployment lobjpersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
            //PIR 25917 - Order by changed to select employment with end date null first.
            lobjpersonEmployment = abusPersonAccount.ibusPerson.icolPersonEmployment.Where
                                    (i => i.icdoPersonEmployment.start_date != i.icdoPersonEmployment.end_date).OrderByDescending
                                    (i => i.icdoPersonEmployment.end_date == DateTime.MinValue).ThenByDescending
                                    (i => i.icdoPersonEmployment.start_date).ThenByDescending
                                    (i => i.icdoPersonEmployment.end_date).FirstOrDefault();
            if (lobjpersonEmployment.IsNotNull())
            {
                ldteEmploymentEndDate = lobjpersonEmployment.icdoPersonEmployment.end_date;
            }

            // All Employment respective to Plan
            if (abusPersonAccount.iclbEmploymentDetail == null)
                abusPersonAccount.LoadAllPersonEmploymentDetails(false);
            busPersonEmploymentDetail lobjbusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            
            //PIR 20335 - Issue 2 - Ignoring employment detail with same start date and end date.
            lobjbusPersonEmploymentDetail = abusPersonAccount.iclbEmploymentDetail.Where
                                            (i => i.icdoPersonEmploymentDetail.start_date != i.icdoPersonEmploymentDetail.end_date).OrderByDescending
                                            (i => i.icdoPersonEmploymentDetail.end_date == DateTime.MinValue).ThenByDescending //PIR 25917
                                            (i => i.icdoPersonEmploymentDetail.start_date).ThenByDescending
                                            (i => i.icdoPersonEmploymentDetail.end_date).FirstOrDefault();

            if (lobjbusPersonEmploymentDetail.IsNotNull())
            {
                // PROD PIR ID 5234: Termination should be the recent associated employment's end date
                if (lobjbusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    lobjbusPersonEmploymentDetail.LoadPersonEmployment();
                adtEmploymentEnddate = lobjbusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date;
                lintEmploymentOrgid = lobjpersonEmployment.icdoPersonEmployment.org_id;
            }

            if (ldteEmploymentEndDate == DateTime.MinValue && iblnIsMemberDual)
            {
                // Has Dual plan and Open Employment
                if ((abusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdHP) &&
                    (abusPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService))
                {
                    // UAT PIR ID 1687 : Multiple Plan Accounts, Scenario 1
                    adtEmploymentEnddate = ldteEmploymentEndDate; // This stops from verifying the Application.                
                }
            }
            return lintEmploymentOrgid;
        }

        public bool IsMemberDualAcrossAllPlans(busPersonAccount abusPersonAccount)
        {
            //Get all the person accounts for the person. If the person has an person account other than the current plan for which calcualtion is done, he is dual.
            //Withdrawn,Cancelled plans not to be used.

            if (abusPersonAccount.ibusPerson == null)
                abusPersonAccount.LoadPerson();

            if (abusPersonAccount.ibusPerson.icolPersonAccount.IsNull())
                abusPersonAccount.ibusPerson.LoadPersonAccount();

            var lintPACount = abusPersonAccount.ibusPerson.icolPersonAccount.Where(obj =>
                                        (obj.icdoPersonAccount.plan_id != abusPersonAccount.icdoPersonAccount.plan_id) &&
                                        (obj.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn) &&
                                        (obj.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirmentCancelled) &&
                                        (obj.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusTransferDC) &&
                                            //(obj.icdoPersonAccount.plan_id != busConstant.PlanIdJobService) &&
                                        (obj.ibusPlan.IsRetirementPlan())).Count();

            if (lintPACount > 0)
            {
                return true;
            }
            return false;
        }

        public int GetOrgIdAsLatestEmploymentOrgId(busPersonAccount abusPersonAccount, string astrBenefitAccountType, ref DateTime adtTerminationDate)
        {
            //UAT 1687 DNRO Option when there is open employment
            if ((astrBenefitAccountType == busConstant.ApplicationBenefitTypeRetirement) ||
                (astrBenefitAccountType == busConstant.ApplicationBenefitTypeDisability) ||
                (astrBenefitAccountType == busConstant.ApplicationBenefitTypePreRetirementDeath))
            {
                return GetLastPersonEmploymentEndDate(abusPersonAccount, ref adtTerminationDate);
            }


            if (abusPersonAccount.iclbEmploymentDetail == null)
                abusPersonAccount.LoadAllPersonEmploymentDetails(false);
            if (astrBenefitAccountType == busConstant.ApplicationBenefitTypeRefund)
            {
                abusPersonAccount.iclbEmploymentDetail = busGlobalFunctions.Sort<busPersonEmploymentDetail>
                                ("icdoPersonEmploymentDetail.start_date desc", abusPersonAccount.iclbEmploymentDetail);
            }
            else
            {
                abusPersonAccount.iclbEmploymentDetail = busGlobalFunctions.Sort<busPersonEmploymentDetail>
                                ("icdoPersonEmploymentDetail.start_date desc,icdoPersonEmploymentDetail.end_date desc",
                                abusPersonAccount.iclbEmploymentDetail);
            }
            foreach (busPersonEmploymentDetail lobjEmploymentDetail in abusPersonAccount.iclbEmploymentDetail)
            {
                if (lobjEmploymentDetail.ibusPersonEmployment == null)
                    lobjEmploymentDetail.LoadPersonEmployment();

                if (astrBenefitAccountType == busConstant.ApplicationBenefitTypeRefund)
                {
                    return lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                }
                else if (astrBenefitAccountType == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {

                    adtTerminationDate = lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date;
                    return lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                }
                else
                {
                    if (lobjEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing)
                    {
                        adtTerminationDate = lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date;
                        return lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                    }
                }
            }
            return 0;
        }
        public int GetOrgIdAsLatestEmploymentOrgId(int aintPersonAccountID, string astrBenefitAccountType, ref DateTime adtTerminationDate)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount();
            if (lobjPersonAccount.FindPersonAccount(aintPersonAccountID))
            {
                return GetOrgIdAsLatestEmploymentOrgId(lobjPersonAccount, astrBenefitAccountType, ref adtTerminationDate);
            }
            return 0;
        }

        public void CalculateQDROAmount(bool bsetQDROAmount, int aintPesonID, int aintPlanID, ref decimal adecQDROeeposttaxamount, ref decimal adecQDROOeepretaxamount,
            ref decimal adecQDROeeerpickupamount, ref decimal idecQDROervestedamount, ref decimal adecQDROquadrointerestamount, ref decimal adecQDROcapitalgain,
            ref decimal adecTotalQDROAmount, ref decimal adecTaxableQDROAmount, ref decimal adecNonTaxableQDROAmount, ref decimal adecQDROAdditionalInterest) //PROD PIR 4800
        {
            decimal ldecTotalQDROAmount = 0.0M;
            decimal ldecTotalNonTaxableQDROAmount = 0.0M;
            decimal ldecTotalTaxableQDROAmount = 0.0M;

            bool bAllowQDROAmount = false;

            busBenefitApplication lobjbenefitApplication = new busBenefitApplication();
            lobjbenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            lobjbenefitApplication.icdoBenefitApplication.member_person_id = aintPesonID;
            lobjbenefitApplication.icdoBenefitApplication.plan_id = aintPlanID;
            lobjbenefitApplication.LoadDROApplication();


            foreach (busBenefitDroApplication lobjbenefitdroApplication in lobjbenefitApplication.iclbBenefitDROApplication)
            {
                if (lobjbenefitdroApplication.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified)
                {
                    ldecTotalQDROAmount = ldecTotalQDROAmount + lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_amount;

                    lobjbenefitdroApplication.LoadDROCalculation();

                    bAllowQDROAmount = true;
                    if (lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.dro_calculation_id > 0)
                    {
                        if (lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusProcessed)
                        {
                            bAllowQDROAmount = false;
                        }
                        else if (lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusApproved)
                        {
                            adecQDROeeposttaxamount = lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_post_tax_amount;
                            ldecTotalNonTaxableQDROAmount = ldecTotalNonTaxableQDROAmount + adecQDROeeposttaxamount;

                            //Calculating Taxable Components of QDRO

                            adecQDROOeepretaxamount = lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_pre_tax_amount;
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROOeepretaxamount;

                            adecQDROeeerpickupamount = lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.ee_er_pickup_amount;
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROeeerpickupamount;

                            idecQDROervestedamount = lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.er_vested_amount;
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecQDROervestedamount;

                            adecQDROquadrointerestamount = lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.interest_amount;
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROquadrointerestamount;
                            //PROD PIR 4800
                            adecQDROAdditionalInterest = lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest_amount;
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROAdditionalInterest;

                            adecQDROcapitalgain = lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.capital_gain;
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROcapitalgain;
                            bAllowQDROAmount = false;
                        }
                    }
                    if (bAllowQDROAmount)
                    {
                        //Calculating Non Taxable Components of QDRO
                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_post_tax_amount > 0.0M)
                        {
                            adecQDROeeposttaxamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_post_tax_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalNonTaxableQDROAmount = ldecTotalNonTaxableQDROAmount + adecQDROeeposttaxamount;

                        }
                        else
                        {
                            adecQDROeeposttaxamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_ee_post_tax_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalNonTaxableQDROAmount = ldecTotalNonTaxableQDROAmount + adecQDROeeposttaxamount;
                        }

                        //Calculating Taxable Components of QDRO

                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_pre_tax_amount > 0.0M)
                        {
                            adecQDROOeepretaxamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_pre_tax_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROOeepretaxamount;

                        }
                        else
                        {
                            adecQDROOeepretaxamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_ee_pre_tax_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROOeepretaxamount;

                        }
                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_er_pickup_amount > 0.0M)
                        {
                            adecQDROeeerpickupamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_er_pickup_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROeeerpickupamount;

                        }
                        else
                        {
                            adecQDROeeerpickupamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_ee_er_pickup_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROeeerpickupamount;

                        }
                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_er_vested_amount > 0.0M)
                        {
                            idecQDROervestedamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_er_vested_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecQDROervestedamount;

                        }
                        else
                        {
                            idecQDROervestedamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_er_vested_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecQDROervestedamount;

                        }
                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_interest_amount > 0.0M)
                        {
                            adecQDROquadrointerestamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_interest_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROquadrointerestamount;

                        }
                        else
                        {
                            adecQDROquadrointerestamount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_interest_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROquadrointerestamount;

                        }

                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_capital_gain > 0.0M)
                        {
                            adecQDROcapitalgain = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_capital_gain * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROcapitalgain;

                        }
                        else
                        {
                            adecQDROcapitalgain = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_capital_gain * lobjbenefitdroApplication.icdoBenefitDroApplication.member_withdrawal_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + adecQDROcapitalgain;

                        }
                    }
                }
            }
            if (bsetQDROAmount)
            {
                adecTotalQDROAmount = ldecTotalQDROAmount;
            }
            adecTaxableQDROAmount = ldecTotalTaxableQDROAmount;
            adecNonTaxableQDROAmount = ldecTotalNonTaxableQDROAmount;
        }

        // UCS-093 - Calculate Required Minimum Distribution Amount
        public decimal CalculateRMDAmount(Decimal adecAge, string astrBenefitPaymentTypeValue, decimal adecMemberAccountBalance, DateTime adteCalculationDate)
        {
            if (adecMemberAccountBalance != 0M)
            {
                if (adecAge >= busConstant.MaxRetirementAgeAtNormalRetDate)
                {
                    int lintTotalYears = Convert.ToInt32(Slice(adecAge, 0));
                    string lstrFactorType = string.Empty;
                    decimal ldecFactor = 0M;
                    // Determine Factor Type
                    if (astrBenefitPaymentTypeValue == busConstant.ApplicationBenefitTypeRefund)
                        lstrFactorType = busConstant.LifeExpectancyTypeUniform;
                    else
                        lstrFactorType = busConstant.LifeExpectancyTypeSingle;

                    // Determine Factor
                    DataTable ldtbResult = iobjPassInfo.isrvDBCache.GetCacheData("sgt_life_expectancy_factor", null);
                    var lfactor = from obj in ldtbResult.AsEnumerable()
                                  where (Convert.ToInt32(obj["MEMBER_AGE"]) == lintTotalYears &&
                                  Convert.ToString(obj["FACTOR_TYPE_VALUE"]) == lstrFactorType &&
                                  Convert.ToDateTime(obj["EFFECTIVE_DATE"]) <= (adteCalculationDate == DateTime.MinValue ? DateTime.Today : adteCalculationDate))
                                  orderby Convert.ToDateTime(obj["EFFECTIVE_DATE"]) descending
                                  select obj;
                    if (lfactor.Count() > 0)
                        ldecFactor = Convert.ToDecimal(((DataRow)lfactor.First())["FACTOR"]);

                    // Calculate RMD Amount
                    if (ldecFactor != 0M)
                        return Math.Round((adecMemberAccountBalance / ldecFactor), 2, MidpointRounding.AwayFromZero);
                }
            }
            return 0M;
        }

        public static void FetchDeathBenefitOption(int aintBenefitProvisionId, string astrBenefitAccountType, bool ablnIspersonMarried,
                                                    string astrBenefitOptionValue, string astrPostRetirementDeathReasonValue, ref string astrDestinationBenefitOption,
                                                    ref string astrRelationshipValue, ref string astrSpouseAtDeath, ref string astrMonthlyBenefitIndicator,
                                                    bool ablnIsTermDatePastDateofDeath, utlPassInfo aobjPassInfo)
        {
            FetchDeathBenefitOption(aintBenefitProvisionId, astrBenefitAccountType, ablnIspersonMarried, astrBenefitOptionValue,
                                    astrPostRetirementDeathReasonValue, false, ref astrDestinationBenefitOption, ref astrRelationshipValue,
                                    ref astrSpouseAtDeath, ref astrMonthlyBenefitIndicator, ablnIsTermDatePastDateofDeath, aobjPassInfo);
        }

        /// <summary>
        /// to get details related to post retirement death benefit option
        /// </summary>
        /// <param name="aintBenefitProvisionId">provision id of plan id</param>
        /// <param name="astrBenefitAccountType">source benefit account type value</param>
        /// <param name="ablnIspersonMarried">is person married flag</param>
        /// <param name="astrBenefitOptionValue">source benefit option</param>
        /// <param name="astrPostRetirementDeathReasonValue">death reason value</param>
        /// <param name="ablnIsBenefitOptionDestination">Is destination benefit option</param>
        /// <param name="astrDestinationBenefitOption">return destination benefit option</param>
        /// <param name="astrRelationshipValue">return relationship value</param>
        /// <param name="astrSpouseAtDeath">return bool Is spouse at death</param>
        /// <param name="astrMonthlyBenefitIndicator">return is monthly benefit indicator</param>
        /// <returns></returns>
        public static void FetchDeathBenefitOption(int aintBenefitProvisionId, string astrBenefitAccountType, bool ablnIspersonMarried,
            string astrBenefitOptionValue, string astrPostRetirementDeathReasonValue, bool ablnIsBenefitOptionDestination,
            ref string astrDestinationBenefitOption, ref string astrRelationshipValue, ref string astrSpouseAtDeath,
            ref string astrMonthlyBenefitIndicator, bool ablnIsTermDatePastDateofDeath, utlPassInfo aobjPassInfo)
        {
            cdoPostRetirementDeathBenefitOptionRef lcdoPSTDBenefitOptionRef = new cdoPostRetirementDeathBenefitOptionRef();
            lcdoPSTDBenefitOptionRef = FetchDeathBenefitOption(aintBenefitProvisionId, astrBenefitAccountType, ablnIspersonMarried,
                                            astrBenefitOptionValue, astrPostRetirementDeathReasonValue, ablnIsBenefitOptionDestination, ablnIsTermDatePastDateofDeath, aobjPassInfo);

            astrDestinationBenefitOption = lcdoPSTDBenefitOptionRef.destination_benefit_option_value;
            astrRelationshipValue = lcdoPSTDBenefitOptionRef.account_relation_value;
            astrSpouseAtDeath = lcdoPSTDBenefitOptionRef.is_spouse_at_death;
            astrMonthlyBenefitIndicator = lcdoPSTDBenefitOptionRef.is_monthly_benefit_flag;
        }

        public static cdoPostRetirementDeathBenefitOptionRef FetchDeathBenefitOption(int aintBenefitProvisionId, string astrBenefitAccountType, bool ablnIspersonMarried,
                string astrBenefitOptionValue, string astrPostRetirementDeathReasonValue, bool ablnIsBenefitOptionDestination,
                bool ablnIsTermDatePastDateofDeath, utlPassInfo aobjPassInfo)
        {
            var ldtbDeathBenefitOption = new DataTable();
            string lstrAccountRelationShipfilter = string.Empty;
            string lstrIsTermCertainDatePastDateofDeath = "N";

            string lstrBenefitOptionColumnName = "SOURCE_BENEFIT_OPTION_VALUE";
            if (ablnIsBenefitOptionDestination)
                lstrBenefitOptionColumnName = "DESTINATION_BENEFIT_OPTION_VALUE";

            busDBCacheData ibusDBCacheData = new busDBCacheData();
            cdoPostRetirementDeathBenefitOptionRef lcdoPSTDBenefitOptionRef = new cdoPostRetirementDeathBenefitOptionRef();
            ibusDBCacheData.idtbCachedPostRetirementDeathBenefitOptions = busGlobalFunctions.LoadPostRetirementDeathBenefitOption(aobjPassInfo);

            if (((aintBenefitProvisionId == busConstant.PlanIdHP) || (aintBenefitProvisionId == busConstant.PlanIdJudges)) &&
                (astrBenefitOptionValue == busConstant.BenefitOptionNormalRetBenefit))
            {
                if (ablnIspersonMarried)
                    lstrAccountRelationShipfilter = busConstant.AccountRelationshipJointAnnuitant;
                else
                    lstrAccountRelationShipfilter = busConstant.AccountRelationshipBeneficiary;
            }


            if ((ablnIsTermDatePastDateofDeath) && (astrPostRetirementDeathReasonValue != busConstant.PostRetirementFirstBeneficiaryDeath))
            {
                lstrIsTermCertainDatePastDateofDeath = "Y";
            }

            if (lstrAccountRelationShipfilter.IsNotEmpty())
            {
                ldtbDeathBenefitOption = ibusDBCacheData.idtbCachedPostRetirementDeathBenefitOptions.AsEnumerable()
                                             .Where(lrow => lrow.Field<int>("BENEFIT_PROVISION_ID") == aintBenefitProvisionId
                                            && lrow.Field<string>("SOURCE_BENEFIT_ACCOUNT_TYPE_VALUE") == astrBenefitAccountType
                                            && lrow.Field<string>(lstrBenefitOptionColumnName) == astrBenefitOptionValue
                                            && lrow.Field<string>("ACCOUNT_RELATION_VALUE") == lstrAccountRelationShipfilter
                                            && lrow.Field<string>("IS_TERMDATE_PAST_DEATH_DATE_FLAG") == lstrIsTermCertainDatePastDateofDeath
                                            && lrow.Field<string>("POST_RETIREMENT_DEATH_REASON_TYPE_VALUE") == astrPostRetirementDeathReasonValue).AsDataTable();
            }
            else
            {
                ldtbDeathBenefitOption = ibusDBCacheData.idtbCachedPostRetirementDeathBenefitOptions.AsEnumerable()
                                             .Where(lrow => lrow.Field<int>("BENEFIT_PROVISION_ID") == aintBenefitProvisionId
                                            && lrow.Field<string>("SOURCE_BENEFIT_ACCOUNT_TYPE_VALUE") == astrBenefitAccountType
                                            && lrow.Field<string>(lstrBenefitOptionColumnName) == astrBenefitOptionValue
                                            && lrow.Field<string>("IS_TERMDATE_PAST_DEATH_DATE_FLAG") == lstrIsTermCertainDatePastDateofDeath
                                            && lrow.Field<string>("POST_RETIREMENT_DEATH_REASON_TYPE_VALUE") == astrPostRetirementDeathReasonValue).AsDataTable();
            }

            //To Set the ref variables.
            if (ldtbDeathBenefitOption.Rows.Count > 0)
            {
                lcdoPSTDBenefitOptionRef.LoadData(ldtbDeathBenefitOption.Rows[0]);
            }
            return lcdoPSTDBenefitOptionRef;
        }

        public int GetTermCertainYears(string astrBenefitOptionValue)
        {
            int lintTermCertainYears = 0;
            if ((astrBenefitOptionValue == "L05C") ||
                (astrBenefitOptionValue == "L10C") ||
                (astrBenefitOptionValue == "L20C") ||
                (astrBenefitOptionValue == "LA10") ||
                (astrBenefitOptionValue == "LA15") ||
                (astrBenefitOptionValue == "LA20") ||
                (astrBenefitOptionValue == "LB05") ||
                (astrBenefitOptionValue == "LB10") ||
                (astrBenefitOptionValue == "LB15") ||
                (astrBenefitOptionValue == "LB20") ||
                (astrBenefitOptionValue == "T10C") ||
                (astrBenefitOptionValue == "T15C") ||
                (astrBenefitOptionValue == "T20C") ||
                (astrBenefitOptionValue == "5YTL"))
            {
                lintTermCertainYears = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(2216, astrBenefitOptionValue, iobjPassInfo));
            }
            return lintTermCertainYears;
        }

        public static decimal GetVestedERSchedulePercentage(DateTime adtEffectiveDate, string astrRetirementType, decimal adecTVSC, ref int aintReferenceID)
        {
            int lintTVSC = 0;
            decimal ldecVestedERPercentage = 0;

            lintTVSC = (int)adecTVSC;
            DataTable ldtbResult = Select("cdoPersonAccountRetirementContribution.GetERVestedSchedulePercentage", new object[3]{
                                    adtEffectiveDate,lintTVSC,astrRetirementType});
            foreach (DataRow dr in ldtbResult.Rows)
            {
                decimal ldecFromTVSC = 0.0M;
                decimal ldecToTVSC = 0.0M;

                if (dr["VSC_FROM"] != DBNull.Value)
                {
                    ldecFromTVSC = Convert.ToDecimal(dr["VSC_FROM"]);
                }
                if (dr["VSC_TO"] != DBNull.Value)
                {
                    ldecToTVSC = Convert.ToDecimal(dr["VSC_TO"]);
                }

                if ((adecTVSC >= ldecFromTVSC) && (adecTVSC <= ldecToTVSC))
                {
                    if (dr["ER_VESTED_PERCENTAGE"] != DBNull.Value)
                        ldecVestedERPercentage = Convert.ToDecimal(dr["ER_VESTED_PERCENTAGE"]);

                    if (dr["VESTED_ER_PERCENTAGE_REF_ID"] != DBNull.Value)
                        aintReferenceID = Convert.ToInt32(dr["VESTED_ER_PERCENTAGE_REF_ID"]);
                }
            }
            return ldecVestedERPercentage;
        }

        //UAT PIR: 855. Need an additional Field for Displaying the SSLI Effective Date.        
        public static DateTime GetSSLIEffectivedate(DateTime adtDateofBirth, decimal adecSSLIAge)
        {
            DateTime ldtSSLIEffectivedate = new DateTime();
            int lintExtraMonthsToAdd = 1;

            if ((adtDateofBirth.Day != 1) && (adtDateofBirth.Day != 2))
            {
                lintExtraMonthsToAdd = 2;
            }

            DateTime ldtTempDate = adtDateofBirth.AddMonths(
                                Convert.ToInt32(adecSSLIAge * 12) + lintExtraMonthsToAdd);
            ldtSSLIEffectivedate = new DateTime(ldtTempDate.Year, ldtTempDate.Month, 1);
            return ldtSSLIEffectivedate;
        }





        /// <summary> 
        /// //PIR : 14177 - Check if at least one ACTIVE Employment Detail with CONTRIBUTING STATUS exists.  
        //If there is only a Non Contributing, LOA, or LOAM we do not have to project. 
        //If there are two employment records; one Contributing and one Non Contributing we still need to project
        /// </summary>
        /// <returns></returns>
        public static bool CheckIfEmploymentISContributingStatus(busPersonAccount abusPersonAccount)
        {

            if (abusPersonAccount.iclbAllPersonEmploymentDetails == null)
                abusPersonAccount.LoadAllPersonEmploymentDtls();

            foreach (busPersonEmploymentDetail lobjEmploymentDetail in abusPersonAccount.iclbAllPersonEmploymentDetails)
            {
                if (lobjEmploymentDetail.icdoPersonEmploymentDetail.status_value != busConstant.EmploymentStatusNonContributing //PIR 21633
                    && (lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue ||
                        lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date >= DateTime.Now)) //PIR 19830 - Future date condition added
                    return true;
            }
            return false;
        }


    }
}
