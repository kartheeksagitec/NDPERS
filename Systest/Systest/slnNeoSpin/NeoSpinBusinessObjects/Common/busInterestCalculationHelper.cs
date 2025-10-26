using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Reflection;
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;

namespace NeoSpin.BusinessObjects
{
    public static class busInterestCalculationHelper
    {
        /// <summary>
        /// Calculate Interest on total balance based on Actuarial Interest Rate.
        /// </summary>
        /// <param name="ldecBalance"></param>
        /// <returns></returns>
        public static decimal CalculateInterest(decimal adecBalance, int aintPlanID)
        {
            return CalculateInterest(adecBalance, aintPlanID, DateTime.Today);
        }
        //UAT PIR 2100 - Plan ID added in Interest Percentage. 
        public static decimal CalculateInterest(decimal adecBalance, int aintPlanID, DateTime adtEffectiveDate)
        {
            decimal ldecActuarialIntRate = 0;
            Collection<cdoCodeValue> lclbActurialInterest = new Collection<cdoCodeValue>();
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            //Filter by the plan id
            DataTable ldtbActurialInterest = lobjPassInfo.isrvDBCache.GetCodeValues(2324, null, null, aintPlanID.ToString());
            var lenuList = ldtbActurialInterest.AsEnumerable().Where(i => Convert.ToDateTime(i.Field<string>("data1")) <= adtEffectiveDate)
                                                              .OrderByDescending(i => Convert.ToDateTime(i.Field<string>("data1")));
            DataTable ldtbResult = lenuList.AsDataTable();
            if (ldtbResult.Rows.Count > 0)
            {
                ldecActuarialIntRate = Convert.ToDecimal(ldtbResult.Rows[0].Field<string>("data2"));
                ldecActuarialIntRate = (ldecActuarialIntRate - 0.5M) / 100;
            }
            return (adecBalance * (ldecActuarialIntRate)) / 12;
        }

        //TODO:- Take percentage from sys contant
        /// <summary>
        /// Calculate Interest on total balance based on Flat Interest Rate.
        /// </summary>
        /// <param name="ldecBalance"></param>
        /// <returns></returns>
        public static decimal CalculateInterestForFlatPercentage(decimal adecBalance)
        {
            return CalculateInterestForFlatPercentage(adecBalance, DateTime.Today);
        }

        public static decimal CalculateInterestForFlatPercentage(decimal adecBalance, DateTime adtEffectiveDate)
        {
            decimal ldecFlatIntRate = 0.00M;
            Collection<cdoCodeValue> lclbFlatInterest = new Collection<cdoCodeValue>();
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            DataTable ldtbFlatInterest = lobjPassInfo.isrvDBCache.GetCodeValues(2323);
            lclbFlatInterest = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbFlatInterest);
            lclbFlatInterest = busGlobalFunctions.Sort<cdoCodeValue>("code_value_order desc", lclbFlatInterest);
            foreach (cdoCodeValue lcdoInterest in lclbFlatInterest)
            {
                DateTime ldtEffftectiveDate = Convert.ToDateTime(lcdoInterest.data1);
                if (ldtEffftectiveDate <= adtEffectiveDate)
                {
                    ldecFlatIntRate = Convert.ToDecimal(lcdoInterest.data2);
                    ldecFlatIntRate = (ldecFlatIntRate) / 100;
                    break;
                }
            }
            return (adecBalance * (ldecFlatIntRate)) / 12;
        }

        public static DateTime GetInterestBatchLastRunDate()
        {
            DateTime ldtInterestBatchLastRunDate = DateTime.MinValue;

            busBatchSchedule lobjInterestPostingBatchSchedule = new busBatchSchedule();
            lobjInterestPostingBatchSchedule.FindBatchSchedule(20);
            if ((lobjInterestPostingBatchSchedule.icdoBatchSchedule.frequency_in_days == 0)
                && (lobjInterestPostingBatchSchedule.icdoBatchSchedule.frequency_in_months == 0))
            {
                ldtInterestBatchLastRunDate = lobjInterestPostingBatchSchedule.icdoBatchSchedule.next_run_date;
            }
            else if (lobjInterestPostingBatchSchedule.icdoBatchSchedule.frequency_in_days > 0)
            {
                ldtInterestBatchLastRunDate = lobjInterestPostingBatchSchedule.icdoBatchSchedule.next_run_date.AddDays(-lobjInterestPostingBatchSchedule.icdoBatchSchedule.frequency_in_days);
            }
            else if (lobjInterestPostingBatchSchedule.icdoBatchSchedule.frequency_in_months > 0)
            {
                ldtInterestBatchLastRunDate = lobjInterestPostingBatchSchedule.icdoBatchSchedule.next_run_date.AddMonths(-lobjInterestPostingBatchSchedule.icdoBatchSchedule.frequency_in_months);
            }
            else if ((lobjInterestPostingBatchSchedule.icdoBatchSchedule.frequency_in_days < 0) || (lobjInterestPostingBatchSchedule.icdoBatchSchedule.frequency_in_months < 0))
            {
                //throw an error
            }
            return ldtInterestBatchLastRunDate;
        }

        /// <summary>
        /// UCS - 079 : Method to calculate Amortization Interest
        /// </summary>
        /// <param name="adtCalculationDate">Calculation Date</param>
        /// <param name="adtLastMonthDate">Last Recovery Date</param>
        /// <param name="adecAmount">Balance recovery amount</param>
        /// <returns>Amortization Interest</returns>
        public static decimal CalculateAmortizationInterest(DateTime adtCalculationDate, DateTime adtLastMonthDate, decimal adecAmount)
        {
            decimal ldecAmortizationInterest = 0.0M;
            decimal ldecInterest = 0.0M;
            Collection<cdoCodeValue> lclbCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtCodeValue = busBase.Select<cdoCodeValue>(new string[1] { "code_id" }, new object[1] { 2906 }, null, null);
            busBase lobjBase = new busBase();
            lclbCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtCodeValue);
            cdoCodeValue lcdoInterest = lclbCodeValue.OrderByDescending(o => Convert.ToDateTime(o.data2))
                                        .Where(o => Convert.ToDateTime(o.data2) <= DateTime.Today)
                                        .FirstOrDefault();
            if (lcdoInterest != null)
                ldecInterest = Convert.ToDecimal(lcdoInterest.data1);

            int lintMonths = adtLastMonthDate == DateTime.MinValue ? 0 : busGlobalFunctions.DateDiffByMonth(adtLastMonthDate, adtCalculationDate) - 1;
            if (lintMonths > 0)
            {
                ldecAmortizationInterest = adecAmount * lintMonths * ((ldecInterest / Convert.ToDecimal(100)) / Convert.ToDecimal(12));
            }
            return ldecAmortizationInterest;
        }
    }
}
