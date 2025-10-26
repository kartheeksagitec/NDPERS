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
using Sagitec.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busServicePurchaseDetailConsolidated : busServicePurchaseDetailConsolidatedGen
    {
        private busServicePurchaseDetail _ibusServicePurchaseDetail;
        public busServicePurchaseDetail ibusServicePurchaseDetail
        {
            get
            {
                return _ibusServicePurchaseDetail;
            }
            set
            {
                _ibusServicePurchaseDetail = value;
            }
        }

        public bool iblnHeaderValidating = false;
        public void LoadServicePurchaseDetail()
        {
            // Load the object only when the service purchase detail object is null
            if (ibusServicePurchaseDetail == null)
            {
                DataTable ldtbList = Select<cdoServicePurchaseDetail>(
                    new string[1] { "service_purchase_detail_id" },
                    new object[1] { icdoServicePurchaseDetailConsolidated.service_purchase_detail_id }, null, null);

                if (ldtbList.Rows.Count > 0)
                {
                    ibusServicePurchaseDetail = new busServicePurchaseDetail();
                    ibusServicePurchaseDetail.icdoServicePurchaseDetail = new cdoServicePurchaseDetail();
                    ibusServicePurchaseDetail.icdoServicePurchaseDetail.LoadData(ldtbList.Rows[0]);
                }
            }
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
                _ibusOrganization = new busOrganization();

            _ibusOrganization.FindOrganization(icdoServicePurchaseDetailConsolidated.org_id);
        }

        public override void AfterPersistChanges()
        {
            // Now go ahead and calculate the value of Total Time to Purchase to be saved in the SGT_Service_Purchase_Detail table.
            // We do this by loading all the consolidated detail records for the current Service_Purchase_Detail_Id, this
            // is done to avoid doing complicated calculations when there is a change in date from/date to and time to purchase..
            // whenever there is some change in the consolidated, we need to go ahead and recalculate all the values in Header
            // and detail as well.

            ibusServicePurchaseDetail.RecomputeCalculatedFields();
            ibusServicePurchaseDetail.icdoServicePurchaseDetail.Update();

            //Assigning this Detail Object to Header Primary Detail Object So that Calculation takes the latest values
            ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail = ibusServicePurchaseDetail;

            //Load the Amoritinzation Schedule Before Updating the Total Contract Amount
            if (ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.number_of_payments != 0)
            {
                //PIR 913 : Every time when we add / modify the amount, we need to repopulate the expected payment amount
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount = 0;
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadAmortizationSchedule();
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.iblnIsPaymentElectionChanged = true;
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.UpdatePaymentElection();
            }

            //Update the Total Contract Amount            
            ibusServicePurchaseDetail.ibusServicePurchaseHeader.CalculateTotalContractAmount();
            ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.Update();

            base.AfterPersistChanges();
        }

        // This method is only used for calculating the Total Time to Purchase for Validation Rules only
        public bool GetTotalTimeToPurchaseForValidation()
        {
            bool lblnResult = false;
            if (ibusServicePurchaseDetail.icdoServicePurchaseDetail.judges_conversion_flag == "Y")
            {
                DataTable ldtbList = Select<cdoServicePurchaseDetailConsolidated>(
                 new string[1] { "service_purchase_detail_id" },
                 new object[1] { icdoServicePurchaseDetailConsolidated.service_purchase_detail_id }, null, null);

                if (ldtbList.Rows.Count > 0)
                {
                    Collection<busServicePurchaseDetailConsolidated> iclbServicePurchaseDetailConsolidated = new Collection<busServicePurchaseDetailConsolidated>();
                    iclbServicePurchaseDetailConsolidated = GetCollection<busServicePurchaseDetailConsolidated>(ldtbList,
                                                                                        "icdoServicePurchaseDetailConsolidated");
                    int lintTotalTimeToPurchase = 0;
                    foreach (busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated in iclbServicePurchaseDetailConsolidated)
                    {
                        lintTotalTimeToPurchase = lintTotalTimeToPurchase +
                                                  lobjServicePurchaseDetailConsolidated.
                                                      icdoServicePurchaseDetailConsolidated.calculated_time_to_purchase;
                    }
                    if (lintTotalTimeToPurchase / 12 > 20)
                    {
                        lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //Need to load Organization object always, to check whether user entered org code id is valid or not
            ibusOrganization = new busOrganization();
            if (icdoServicePurchaseDetailConsolidated.istrOrgCodeId != null)
            {
                ibusOrganization.FindOrganizationByOrgCode(icdoServicePurchaseDetailConsolidated.istrOrgCodeId);
                icdoServicePurchaseDetailConsolidated.org_id = ibusOrganization.icdoOrganization.org_id;
            }
            else
                icdoServicePurchaseDetailConsolidated.org_id = 0;
            CalculateTimeToPurchase();
            base.BeforeValidate(aenmPageMode);
        }

        public void CalculateTimeToPurchase()
        {
            int lintDiffInMonths;
            int lintDiffInYears;

            DateTime ldtDateFrom = DateTime.MinValue;
            DateTime ldtDateTo = DateTime.MinValue;
            int lintTotalTimeToPurchase = 0;

            if ((icdoServicePurchaseDetailConsolidated.service_purchase_start_date > DateTime.MinValue) &&
                (icdoServicePurchaseDetailConsolidated.service_purchase_end_date > DateTime.MinValue))
            {
                ldtDateFrom = new DateTime(icdoServicePurchaseDetailConsolidated.service_purchase_start_date.Year,
                                     icdoServicePurchaseDetailConsolidated.service_purchase_start_date.Month, 1);

                ldtDateTo = new DateTime(icdoServicePurchaseDetailConsolidated.service_purchase_end_date.Year,
                                 icdoServicePurchaseDetailConsolidated.service_purchase_end_date.Month, 1);

                lintTotalTimeToPurchase = HelperFunction.GetMonthSpan(ldtDateFrom, ldtDateTo, out lintDiffInYears, out lintDiffInMonths) + 1;
            }

            //PIR 416 : Exclude the Months which are already posted contribution
            int lintContribAlreadyPostedMonths = 0;
            if (icdoServicePurchaseDetailConsolidated.service_purchase_start_date != DateTime.MinValue)
            {
                if (ldtDateTo > DateTime.MinValue)
                {
                    while (ldtDateTo >= ldtDateFrom)
                    {
                        if (ibusServicePurchaseDetail.GetPSCForMonthYear(ldtDateTo.Month, ldtDateTo.Year) > 0)
                        {
                            lintContribAlreadyPostedMonths++;
                        }
                        ldtDateTo = ldtDateTo.AddMonths(-1);
                    }
                }
            }

            lintTotalTimeToPurchase = lintTotalTimeToPurchase - lintContribAlreadyPostedMonths;

            // Only for "Additional Service Credit" Credit type we have to add the "Time to Purchase"
            if (icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Additional_Service_Credit)
            {
                lintTotalTimeToPurchase = lintTotalTimeToPurchase + icdoServicePurchaseDetailConsolidated.time_to_purchase;
            }
            icdoServicePurchaseDetailConsolidated.calculated_time_to_purchase = lintTotalTimeToPurchase;
            icdoServicePurchaseDetailConsolidated.time_to_purchase_contribution_months = lintContribAlreadyPostedMonths;
        }

        // Code to make sure the validation happens in a top down fashion from Header -> Detail -> Consolidated Detail
        public override bool ValidateSoftErrors()
        {
            if (iblnHeaderValidating)
            {
                if (ibusSoftErrors == null)
                {
                    LoadErrors();
                }
                iblnClearSoftErrors = false;
                ibusSoftErrors.iblnClearError = false;
                return base.ValidateSoftErrors();
            }
            else
            {
                bool lblnResult = ibusServicePurchaseDetail.ValidateSoftErrors();
                if (ibusSoftErrors == null)
                {
                    LoadErrors();
                }
                return lblnResult;
            }
        }

        // get member's actual age
        public int GetMemberAge()
        {
            return ibusServicePurchaseDetail.iintMemberAge(icdoServicePurchaseDetailConsolidated.service_purchase_start_date);
        }

        /// <summary>
        /// check if the sum of all additional service credit is not exceeding 60 for the Given Person. (Excluded the VOID Entries)
        /// </summary>
        /// <returns></returns>
        public bool IsTotalASCExceeds60ForPayorEmployer()
        {
            decimal ldecCalculatedTimeToPurchase = 0.00M;
            DataTable ldtbDetailRecordsExcpetCurrent =
                Select("cdoServicePurchaseDetailConsolidated.GetTotalASCForPersonByPayorEmployer",
                       new object[2]
                           {
                               ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.person_id,
                               icdoServicePurchaseDetailConsolidated.service_purchase_consolidated_detail_id
                           });
            if (ldtbDetailRecordsExcpetCurrent.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbDetailRecordsExcpetCurrent.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["calculated_time_to_purchase"].ToString()))
                    {
                        ldecCalculatedTimeToPurchase += Convert.ToDecimal(dr["calculated_time_to_purchase"]);
                    }
                }
            }

            ldecCalculatedTimeToPurchase = ldecCalculatedTimeToPurchase + icdoServicePurchaseDetailConsolidated.time_to_purchase;
            if (ldecCalculatedTimeToPurchase > 60)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// check if the sum of all additional service credit is not exceeding 60 for the Given Person. (Excluded the VOID Entries)
        /// </summary>
        /// <returns></returns>
        public bool IsTotalASCExceeds60ForPayorEmployee()
        {
            decimal ldecCalculatedTimeToPurchase = 0.00M;
            DataTable ldtbDetailRecordsExcpetCurrent =
                Select("cdoServicePurchaseDetailConsolidated.GetTotalASCForPersonByPayorEmployee",
                       new object[2]
                           {
                               ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.person_id,
                               icdoServicePurchaseDetailConsolidated.service_purchase_consolidated_detail_id
                           });
            if (ldtbDetailRecordsExcpetCurrent.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbDetailRecordsExcpetCurrent.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["calculated_time_to_purchase"].ToString()))
                    {
                        ldecCalculatedTimeToPurchase += Convert.ToDecimal(dr["calculated_time_to_purchase"]);
                    }
                }
            }

            ldecCalculatedTimeToPurchase = ldecCalculatedTimeToPurchase + icdoServicePurchaseDetailConsolidated.time_to_purchase;

            if (ldecCalculatedTimeToPurchase > 60)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// PIR 923 - Detailed Mail Sent from Maik to Jeeva Dated on 
        /// For ‘Judges Conversion’ only allow ‘Previous PERS Service’; cannot include other consolidated purchase types.
        /// </summary>
        /// <returns></returns>
        public bool IsInvalidServiceCreditTypeSelectedForJudgesConversion()
        {
            bool lblnResult = false;
            if (ibusServicePurchaseDetail.icdoServicePurchaseDetail.judges_conversion_flag == busConstant.Flag_Yes)
            {
                if (icdoServicePurchaseDetailConsolidated.service_credit_type_value != busConstant.Service_Purchase_Type_Previous_Pers_Employment)
                {
                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// UAT PIR 717 : Judges Conversion Purchase, if the Purchase is more than 240 Months
        /// </summary>
        /// <returns></returns>
        public bool IsPurchseMorethan240MonthsForJudgesConversion()
        {
            bool lblnResult = false;
            if (ibusServicePurchaseDetail.icdoServicePurchaseDetail.judges_conversion_flag == busConstant.Flag_Yes)
            {
                if (icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Previous_Pers_Employment)
                {
                    if (icdoServicePurchaseDetailConsolidated.calculated_time_to_purchase > 240)
                    {
                        lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// Look for Retirement Contributions.  If a gap in time exists check for purchases which are paid in full.  
        /// The time purchased must have a date range which covers the missed contributions.  
        /// Also account for adjustments which may reverse a previously posted contribution
        /// </summary>
        /// <returns></returns>
        public bool IsContributionExistsForPurchasePeriodForJudgesConversion()
        {
            if (ibusServicePurchaseDetail.icdoServicePurchaseDetail.judges_conversion_flag == busConstant.Flag_Yes)
            {
                if (icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Previous_Pers_Employment)
                {
                    if ((icdoServicePurchaseDetailConsolidated.service_purchase_start_date != DateTime.MinValue) &&
                        (icdoServicePurchaseDetailConsolidated.service_purchase_end_date != DateTime.MinValue))
                    {
                        //Check If Main Account Available During that Period
                        if (ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson == null)
                            ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPerson();

                        if (ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson.iclbRetirementAccount == null)
                            ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson.LoadRetirementAccount();

                        foreach (busPersonAccount lobjRetirementAccount in ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson.iclbRetirementAccount)
                        {
                            //Only Main Account Allowed for Judges Conversion
                            if (lobjRetirementAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain ||
                                lobjRetirementAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020
                                )//PIR 20232
                            {
                                //Load the Contribution By Month
                                DataTable ldtbList =
                                    Select("cdoPersonAccountRetirementContribution.LoadTotalContributionByMonth",
                                           new object[1] { lobjRetirementAccount.icdoPersonAccount.person_account_id });

                                if (ldtbList.Rows.Count > 0)
                                {
                                    DateTime ldtStartDate =
                                        new DateTime(
                                            icdoServicePurchaseDetailConsolidated.service_purchase_start_date.Year,
                                            icdoServicePurchaseDetailConsolidated.service_purchase_start_date.Month, 1);

                                    DateTime ldtEndDate =
                                        new DateTime(
                                            icdoServicePurchaseDetailConsolidated.service_purchase_end_date.Year,
                                            icdoServicePurchaseDetailConsolidated.service_purchase_end_date.Month, 1);

                                    while (ldtEndDate >= ldtStartDate)
                                    {
                                        bool lblnFound = false;
                                        foreach (DataRow dr in ldtbList.Rows)
                                        {
                                            if ((dr["Month"].ToString() == ldtEndDate.Month.ToString()) &&
                                                (dr["Year"].ToString() == ldtEndDate.Year.ToString()))
                                            {
                                                decimal ldecTotalContrAmt = Convert.ToDecimal(dr["TotalContributionAmount"].ToString());
                                                if (ldecTotalContrAmt > 0)
                                                {
                                                    lblnFound = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (!lblnFound) return false;

                                        ldtEndDate = ldtEndDate.AddMonths(-1);
                                    }
                                    return true;
                                }
                            }
                        }
                    }

                }
            }
            return false;
        }

        #region Properties for Correspondence

        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get { return _ibusOrganization; }
            set { _ibusOrganization = value; }
        }

        #endregion

        #region Method for Correspondence

        public override busBase GetCorPerson()
        {
            if (ibusServicePurchaseDetail == null)
                LoadServicePurchaseDetail();
            if (ibusServicePurchaseDetail.ibusServicePurchaseHeader == null)
                ibusServicePurchaseDetail.LoadServicePurchaseHeader();
            if (ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson == null)
                ibusServicePurchaseDetail.ibusServicePurchaseHeader.LoadPerson();
            return ibusServicePurchaseDetail.ibusServicePurchaseHeader.ibusPerson;
        }

        #endregion

        // PIR 9262
        public override busBase GetCorOrganization()
        {
            LoadOrganization();
            return this.ibusOrganization;
        }



         public string istrPaymentMinusExpirationDateDate
        {
            get
            {
                return ibusServicePurchaseDetail.ibusServicePurchaseHeader.icdoServicePurchaseHeader.expiration_date.ToString(busConstant.DateFormatLongDate);
            }
        }
    }
}
