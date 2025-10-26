using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busCheckFileOut : busFileBaseOut
    {
        public busCheckFileOut()
        { }
        //Property to store data which need to be written to file
        public Collection<busCheckFileData> iclbCheckFileData { get; set; }
        //Propety to store check file details
        public DataTable idtCheckFile { get; set; }
        //property to store check stub rhic details
        public DataTable idtGHDVPersonAccount { get; set; }
        //property to store Payment details
        public DataTable idtPayments { get; set; }
        //property to store RHIC amount details
        public DataTable idtRHICDetails { get; set; }

        public DateTime idtPaymentDate { get; set; }

        public bool iblnFromMonthlyOrAdhocBatch { get; set; }

        public override void InitializeFile()
        {
            istrFileName = "CheckFile" + "_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }

        /// <summary>
        /// Method to load the collection which need to be written to Check file
        /// </summary>
        /// <param name="adtCheckFile">Datatable</param>
        public void LoadCheckFile(DataTable adtCheckFile)
        {
            iclbCheckFileData = new Collection<busCheckFileData>();
            idtCheckFile = (DataTable)iarrParameters[0];
            int lintPaymentScheduleID = Convert.ToInt32(iarrParameters[1]);
            DateTime ldtPaymentDate = Convert.ToDateTime(iarrParameters[2]);
            iblnFromMonthlyOrAdhocBatch = Convert.ToBoolean(iarrParameters[3]);
            if (iblnFromMonthlyOrAdhocBatch)
            {
                idtPayments = busBase.Select("cdoPaymentHistoryDetail.LoadPaymentDeductionRecords", new object[1] { lintPaymentScheduleID });
            }
            else
            {
                idtPayments = busBase.Select("cdoPaymentHistoryDetail.LoadPaymentDeductionsForVendorPayment", new object[1] { lintPaymentScheduleID });
            }
            idtGHDVPersonAccount = busBase.Select("cdoPaymentHistoryDetail.LoadGHDVAccounts", new object[2] { ldtPaymentDate, lintPaymentScheduleID });
            idtRHICDetails = busBase.Select("cdoBenefitRhicCombine.LoadRHICDetails", new object[2] { ldtPaymentDate, lintPaymentScheduleID });
            idtPaymentDate = ldtPaymentDate;

            //Loading DB Cache (optimization)
            LoadDBCacheData();

            //Loading Complete Activte Provider Org Plan List (Optimization Purpose)
            LoadActiveProviders(ldtPaymentDate);

            //Loading the Life Option Data by Org (Optimization)
            LoadLifeOptionData(ldtPaymentDate);

            //Loading the GHDV History (Optimization)
            LoadGHDVHistory(ldtPaymentDate);

            //Loading the Life History (Optimization)
            LoadLifeHistory(ldtPaymentDate);

            foreach (DataRow dr in idtCheckFile.Rows)
            {
                busCheckFileData lobjCheckFileData = new busCheckFileData();
                lobjCheckFileData.InitializeObjects();

                lobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.LoadData(dr);
                lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.LoadData(dr);
                //prod pir 4359
                if (!string.IsNullOrEmpty(lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description) &&
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_value != busConstant.US_Code_ID)
                {
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description =
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description.ToUpper();
                }
                else
                    lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.addr_country_description = string.Empty;
                lobjCheckFileData.istrPersonIDOrgCodeID = dr["istrPersonIdOrOrgCode"] != DBNull.Value ?
                    Convert.ToString(dr["istrPersonIdOrOrgCode"]) : string.Empty;
                lobjCheckFileData.istrPersonName = dr["PERSONNAME"] != DBNull.Value ?
                    Convert.ToString(dr["PERSONNAME"]).ToUpper() : string.Empty;
                lobjCheckFileData.istrPlanName = dr["PLAN_NAME"] != DBNull.Value ?
                    Convert.ToString(dr["PLAN_NAME"]) : string.Empty;
                lobjCheckFileData.istrBenefitTypeDesc = dr["BENEFIT_TYPE"] != DBNull.Value ?
                    Convert.ToString(dr["BENEFIT_TYPE"]) : string.Empty;
                lobjCheckFileData.istrBenefitOptionDesc = dr["BENEFIT_OPTION"] != DBNull.Value ?
                    Convert.ToString(dr["BENEFIT_OPTION"]) : string.Empty;
               
                if (iblnFromMonthlyOrAdhocBatch)
                    LoadRHICDetails(lobjCheckFileData);

                LoadPaymentDetails(lobjCheckFileData);

                if (lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount > 0)
                {
                    lobjCheckFileData.istrAmountInWords =
                        busGlobalFunctions.AmountToWords(lobjCheckFileData.ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount.ToString()).ToUpper();
                }
                iclbCheckFileData.Add(lobjCheckFileData);
            }
        }

        /// <summary>
        /// method to load payment details (Both payments and deductions)
        /// </summary>
        /// <param name="aobjCheckFileData">Check file business object</param>
        private void LoadPaymentDetails(busCheckFileData aobjCheckFileData)
        {
            DataTable ldtPayments = new DataTable();
            DataTable ldtDeductions = new DataTable();
            if (iblnFromMonthlyOrAdhocBatch)
            {
                ldtPayments = (from ldrPayment in idtPayments.AsEnumerable()
                               where (ldrPayment.Field<int?>("payee_account_id").IsNull() || ldrPayment.Field<int>("payee_account_id") ==
                                                   aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id ) &&
                                      ldrPayment.Field<int>("item_type_direction") == 1 &&
                                      (
                                        aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id > 0 &&
                                          ( (ldrPayment.Field<int?>("person_id").IsNotNull() && ldrPayment.Field<int>("person_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id) ||
                                             (ldrPayment.Field<int?>("org_id").IsNotNull() && ldrPayment.Field<int>("org_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id))
                                       )
                               orderby ldrPayment.Field<int>("payment_item_type_id") //pir 8129
                               select ldrPayment).AsDataTable();
                ldtDeductions = (from ldrPayment in idtPayments.AsEnumerable()
                                 where (ldrPayment.Field<int?>("payee_account_id").IsNull() || ldrPayment.Field<int>("payee_account_id") ==
                                                     aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id) &&
                                        ldrPayment.Field<int>("item_type_direction") == -1 &&
                                      (
                                        aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id > 0 &&
                                          ((ldrPayment.Field<int?>("person_id").IsNotNull() && ldrPayment.Field<int>("person_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id) ||
                                             (ldrPayment.Field<int?>("org_id").IsNotNull() && ldrPayment.Field<int>("org_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id))
                                       )
                                 orderby ldrPayment.Field<int>("payment_item_type_id") //pir 8129
                                 select ldrPayment).AsDataTable();
            }
            else
            {
                ldtPayments = (from ldrPayment in idtPayments.AsEnumerable()
                               where ldrPayment.Field<int>("org_id") ==
                                                   aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id && //IN QUERY WE TAKE ORG ID AND PUT ALIAS AS PERSON ID
                                      ldrPayment.Field<int>("item_type_direction") == 1
                               select ldrPayment).AsDataTable();
                ldtDeductions = (from ldrPayment in idtPayments.AsEnumerable()
                                 where ldrPayment.Field<int>("org_id") ==
                                                     aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id && //IN QUERY WE TAKE ORG ID AND PUT ALIAS AS PERSON ID
                                        ldrPayment.Field<int>("item_type_direction") == -1
                                 select ldrPayment).AsDataTable();
            }
            int lintChkCompNumber = 1;
            foreach (DataRow dr in ldtPayments.Rows)
            {
                switch (lintChkCompNumber)
                {
                    case 1:
                        aobjCheckFileData.idecPayAmount1 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent1 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD1 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc1 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount1 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent1 != 0.0M || aobjCheckFileData.idecPayYTD1 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 2:
                        aobjCheckFileData.idecPayAmount2 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent2 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD2 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc2 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount2 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent2 != 0.0M || aobjCheckFileData.idecPayYTD2 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 3:
                        aobjCheckFileData.idecPayAmount3 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent3 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD3 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc3 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount3 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent3 != 0.0M || aobjCheckFileData.idecPayYTD3 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 4:
                        aobjCheckFileData.idecPayAmount4 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent4 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD4 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc4 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount4 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent4 != 0.0M || aobjCheckFileData.idecPayYTD4 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 5:
                        aobjCheckFileData.idecPayAmount5 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent5 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD5 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc5 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount5 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent5 != 0.0M || aobjCheckFileData.idecPayYTD5 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 6:
                        aobjCheckFileData.idecPayAmount6 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent6 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD6 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc6 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount6 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent6 != 0.0M || aobjCheckFileData.idecPayYTD6 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 7:
                        aobjCheckFileData.idecPayAmount7 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent7 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD7 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc7 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount7 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent7 != 0.0M || aobjCheckFileData.idecPayYTD7 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 8:
                        aobjCheckFileData.idecPayAmount8 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent8 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD8 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc8 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount8 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent8 != 0.0M || aobjCheckFileData.idecPayYTD8 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 9:
                        aobjCheckFileData.idecPayAmount9 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent9 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD9 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc9 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount9 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent9 != 0.0M || aobjCheckFileData.idecPayYTD9 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 10:
                        aobjCheckFileData.idecPayAmount10 = dr["ftm_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayCurrent10 = dr["ltd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_payment"]) : 0.0M;
                        aobjCheckFileData.idecPayYTD10 = dr["ytd_payment"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_payment"]) : 0.0M;
                        aobjCheckFileData.istrPayCheckCompDesc10 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecPayAmount10 != 0.0M ||
                        aobjCheckFileData.idecPayCurrent10 != 0.0M || aobjCheckFileData.idecPayYTD10 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                }
                lintChkCompNumber++;
            }
            aobjCheckFileData.idecPayTotals = (from ldrPayment in ldtPayments.AsEnumerable()
                                               select ldrPayment.Field<decimal>("ftm_payment")).Sum();

            lintChkCompNumber = 1;
            foreach (DataRow dr in ldtDeductions.Rows)
            {
                switch (lintChkCompNumber)
                {
                    case 1:
                        aobjCheckFileData.idecDedAmount1 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent1 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD1 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc1 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount1 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent1 != 0.0M || aobjCheckFileData.idecDedYTD1 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 2:
                        aobjCheckFileData.idecDedAmount2 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent2 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD2 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc2 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount2 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent2 != 0.0M || aobjCheckFileData.idecDedYTD2 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 3:
                        aobjCheckFileData.idecDedAmount3 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent3 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD3 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc3 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount3 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent3 != 0.0M || aobjCheckFileData.idecDedYTD3 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 4:
                        aobjCheckFileData.idecDedAmount4 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent4 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD4 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc4 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount4 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent4 != 0.0M || aobjCheckFileData.idecDedYTD4 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 5:
                        aobjCheckFileData.idecDedAmount5 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent5 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD5 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc5 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount5 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent5 != 0.0M || aobjCheckFileData.idecDedYTD5 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 6:
                        aobjCheckFileData.idecDedAmount6 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent6 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD6 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc6 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount6 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent6 != 0.0M || aobjCheckFileData.idecDedYTD6 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 7:
                        aobjCheckFileData.idecDedAmount7 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent7 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD7 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc7 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount7 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent7 != 0.0M || aobjCheckFileData.idecDedYTD7 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 8:
                        aobjCheckFileData.idecDedAmount8 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent8 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD8 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc8 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount8 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent8 != 0.0M || aobjCheckFileData.idecDedYTD8 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 9:
                        aobjCheckFileData.idecDedAmount9 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent9 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD9 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc9 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount9 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent9 != 0.0M || aobjCheckFileData.idecDedYTD9 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 10:
                        aobjCheckFileData.idecDedAmount10 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent10 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD10 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc10 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount10 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent10 != 0.0M || aobjCheckFileData.idecDedYTD10 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 11:
                        aobjCheckFileData.idecDedAmount11 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent11 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD11 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc11 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount11 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent11 != 0.0M || aobjCheckFileData.idecDedYTD11 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 12:
                        aobjCheckFileData.idecDedAmount12 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent12 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD12 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc12 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount12 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent12 != 0.0M || aobjCheckFileData.idecDedYTD12 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 13:
                        aobjCheckFileData.idecDedAmount13 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent13 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD13 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc13 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount13 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent13 != 0.0M || aobjCheckFileData.idecDedYTD13 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 14:
                        aobjCheckFileData.idecDedAmount14 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent14 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD14 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc14 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount14 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent14 != 0.0M || aobjCheckFileData.idecDedYTD14 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 15:
                        aobjCheckFileData.idecDedAmount15 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent15 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD15 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc15 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount15 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent15 != 0.0M || aobjCheckFileData.idecDedYTD15 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 16:
                        aobjCheckFileData.idecDedAmount16 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent16 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD16 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc16 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount16 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent16 != 0.0M || aobjCheckFileData.idecDedYTD16 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 17:
                        aobjCheckFileData.idecDedAmount17 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent17 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD17 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc17 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount17 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent17 != 0.0M || aobjCheckFileData.idecDedYTD17 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 18:
                        aobjCheckFileData.idecDedAmount18 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent18 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD18 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc18 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount18 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent18 != 0.0M || aobjCheckFileData.idecDedYTD18 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 19:
                        aobjCheckFileData.idecDedAmount19 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent19 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD19 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc19 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount19 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent19 != 0.0M || aobjCheckFileData.idecDedYTD19 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                    case 20:
                        aobjCheckFileData.idecDedAmount20 = dr["ftm_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ftm_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedCurrent20 = dr["ltd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ltd_deduction"]) : 0.0M;
                        aobjCheckFileData.idecDedYTD20 = dr["ytd_deduction"] != DBNull.Value ?
                            Convert.ToDecimal(dr["ytd_deduction"]) : 0.0M;
                        aobjCheckFileData.istrDedCheckCompDesc20 = dr["item_type_description"] != DBNull.Value && (aobjCheckFileData.idecDedAmount20 != 0.0M ||
                        aobjCheckFileData.idecDedCurrent20 != 0.0M || aobjCheckFileData.idecDedYTD20 != 0.0M) ?
                            Convert.ToString(dr["item_type_description"]) : string.Empty;
                        break;
                }
                lintChkCompNumber++;
            }
            aobjCheckFileData.idecDedTotals = (from ldrDeduction in ldtDeductions.AsEnumerable()
                                               select ldrDeduction.Field<decimal>("ftm_deduction")).Sum();

            aobjCheckFileData.idecNetAmount = aobjCheckFileData.idecPayTotals - aobjCheckFileData.idecDedTotals;
        }

        /// <summary>
        /// Method to load RHIC details
        /// </summary>
        /// <param name="aobjCheckFileData">check file business object</param>
        private void LoadRHICDetails(busCheckFileData aobjCheckFileData)
        {
            DataRow ldrGHDV = idtGHDVPersonAccount.AsEnumerable()
                                .Where(o => o.Field<int>("payee_account_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id)
                                .FirstOrDefault();
            aobjCheckFileData.idecTotalRHICAmount = idtRHICDetails.AsEnumerable()
                        .Where(o => o.Field<int>("payee_account_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id)
                        .Select(o => o.Field<decimal>("combined_rhic_amount")).Sum();
            aobjCheckFileData.idecRHICApplied = idtRHICDetails.AsEnumerable()
                       .Where(o => o.Field<int>("payee_account_id") == aobjCheckFileData.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id)
                       .Select(o => o.Field<decimal>("total_other_rhic_amount")).Sum();

            if (ldrGHDV != null && ldrGHDV["plan_id"] != DBNull.Value)
            {
                aobjCheckFileData.idecNetPremium = LoadHealthMedicarePremium(ldrGHDV);

                if (Convert.ToInt32(ldrGHDV["plan_id"]) == busConstant.PlanIdGroupHealth)
                {
                    aobjCheckFileData.idecGroupHealthPremium = LoadHealthMedicarePremium(ldrGHDV);
                    aobjCheckFileData.idecNetPremium = aobjCheckFileData.idecGroupHealthPremium - aobjCheckFileData.idecRHICApplied;
                }
                else if (Convert.ToInt32(ldrGHDV["plan_id"]) == busConstant.PlanIdMedicarePartD)
                {
                    aobjCheckFileData.idecMedicarePartDPremium = LoadHealthMedicarePremium(ldrGHDV);
                    aobjCheckFileData.idecNetPremium = aobjCheckFileData.idecMedicarePartDPremium - aobjCheckFileData.idecRHICApplied;
                }
            }
        }

        public busDBCacheData ibusDBCacheData { get; set; }
        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }
        public DataTable idtbPALifeOption { get; set; }
        public DataTable idtbGHDVHistory { get; set; }
        public DataTable idtbLifeHistory { get; set; }

        public void LoadDBCacheData()
        {
            if (ibusDBCacheData == null)
                ibusDBCacheData = new busDBCacheData();
            ibusDBCacheData.idtbCachedRateRef = busGlobalFunctions.LoadHealthRateRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedRateStructureRef = busGlobalFunctions.LoadHealthRateStructureCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedCoverageRef = busGlobalFunctions.LoadHealthCoverageRefCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHealthRate = busGlobalFunctions.LoadHealthRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLifeRate = busGlobalFunctions.LoadLifeRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedHMORate = busGlobalFunctions.LoadHMORateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedLtcRate = busGlobalFunctions.LoadLTCRateCacheData(iobjPassInfo);
            ibusDBCacheData.idtbCachedVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
        }

        public void LoadActiveProviders(DateTime adtBatchDate)
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoIbsHeader.LoadAllActiveProviders", new object[1] { adtBatchDate });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }

        public void LoadLifeOptionData(DateTime adtBatchDate)
        {
            idtbPALifeOption =
                busNeoSpinBase.Select("cdoIbsHeader.LoadLifeOption", new object[1] { adtBatchDate });
        }

        public void LoadGHDVHistory(DateTime adtBatchDate)
        {
            idtbGHDVHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadGHDVHistory", new object[1] { adtBatchDate });
        }

        public void LoadLifeHistory(DateTime adtBatchDate)
        {
            idtbLifeHistory =
                busNeoSpinBase.Select("cdoIbsHeader.LoadLifeHistory", new object[1] { adtBatchDate });
        }

        public string GetGroupHealthCoverageCodeDescription(int AintCoverageRefID)
        {
            if (AintCoverageRefID > 0)
            {
                DataTable ldtbCoverageCode = busNeoSpinBase.Select("cdoIbsHeader.GetCoverageCodeDescription",
                                                                                            new object[1] { AintCoverageRefID });
                if (ldtbCoverageCode.Rows.Count > 0)
                {
                    string lstrCoverageCodeDescription = ldtbCoverageCode.Rows[0]["CLIENT_DESCRIPTION"].ToString();
                    return lstrCoverageCodeDescription;
                }
            }
            return string.Empty;
        }

        private decimal LoadHealthMedicarePremium(DataRow adrRow)
        {
            int lintCurrIndex = 0;
            bool lblnInTransaction = false;
            bool lblnSuccess = false;
            bool lblnErrorFound = false;
            //uat pir 1344
            decimal ldecEmprSharePremium = 0.00M, ldecEmprShareFee = 0.00M, ldecEmprShareRHICAmt = 0.00M, ldecEmprShareOtherRHICAmt = 0.00M, ldecEmprShareJSRHICAmt = 0.00M;
            decimal ldecEmpShareBuydown = 0.00M;
            decimal ldecEmpShareMedicarePartD = 0.00M;
            busBase lbusBase = new busBase();
            lintCurrIndex++;
            lblnErrorFound = false;

            var lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lbusPersonAccount.icdoPersonAccount.LoadData(adrRow);

            lbusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lbusPersonAccount.ibusPerson.icdoPerson.LoadData(adrRow);

            lbusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
            lbusPersonAccount.ibusPlan.icdoPlan.LoadData(adrRow);

            lbusPersonAccount.ibusPaymentElection = new busPersonAccountPaymentElection
                                                        {
                                                            icdoPersonAccountPaymentElection =
                                                                new cdoPersonAccountPaymentElection()
                                                        };
            lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(adrRow);

            string lstrCoverageCode = string.Empty;
            decimal ldecGroupHealthFeeAmt = 0.00M;
            decimal ldecBuydownAmt = 0.00M;
            decimal ldecRHICAmt = 0.00M;
            /* UAT PIR 476, Including other and JS RHIC Amount */
            decimal ldecOthrRHICAmount = 0.00M;
            decimal ldecJSRHICAmount = 0.00M;
            /* UAT PIR 476 ends here */
            decimal ldecPremiumAmt = 0.00M;
            decimal ldecTotalPremiumAmt = 0.00M;
            decimal ldecProviderPremiumAmt = 0.00M;
            decimal ldecMemberPremiumAmt = 0.00M;
            decimal ldecMedicarePartD = 0.00M;
            //uat pir 1429 :- add ghdv_history_id in ibs_Detail
            int lintGHDVHistoryID = 0;
            string lstrGroupNumber = string.Empty;

            var lobjGhdv = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
            lobjGhdv.icdoPersonAccountGhdv.LoadData(adrRow);
            lobjGhdv.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
            lobjGhdv.ibusPerson = lbusPersonAccount.ibusPerson;
            lobjGhdv.ibusPlan = lbusPersonAccount.ibusPlan;
            lobjGhdv.ibusPaymentElection = lbusPersonAccount.ibusPaymentElection;

            lobjGhdv.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
            lobjGhdv.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
            lobjGhdv.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
            lobjGhdv.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
            lobjGhdv.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
            lobjGhdv.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
            lobjGhdv.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;

            //Loading the History Object                
            if ((idtbGHDVHistory != null) && (idtbGHDVHistory.Rows.Count > 0))
            {
                DataRow[] larrRow = idtbGHDVHistory.FilterTable(busConstant.DataType.Numeric,
                                                                "person_account_ghdv_id",
                                                                lobjGhdv.icdoPersonAccountGhdv.person_account_ghdv_id);

                lobjGhdv.iclbPersonAccountGHDVHistory =
                    lbusBase.GetCollection<busPersonAccountGhdvHistory>(larrRow, "icdoPersonAccountGhdvHistory");
            }

            //Look the Provider Org ID first , If null, get the default provider
            //Mail From Satya Dated On 1/29/2009

            //Get the GHDV History Object By Billing Month Year
            busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjGhdv.LoadHistoryByDate(idtPaymentDate);
            if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == 0)
            {
                lblnErrorFound = true;
            }

            if (!lblnErrorFound)
            {

                lobjGhdv = lobjPAGhdvHistory.LoadGHDVObject(lobjGhdv);

                //uat pir 1429 :- to post ghdv_history_id
                lintGHDVHistoryID = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                lstrGroupNumber = lobjGhdv.GetGroupNumber();

                if (lobjGhdv.ibusPerson == null)
                    lobjGhdv.LoadPerson();

                if (lobjGhdv.ibusPlan == null)
                    lobjGhdv.LoadPlan();
                //Initialize the Org Object to Avoid the NULL error
                lobjGhdv.InitializeObjects();
                lobjGhdv.idtPlanEffectiveDate = idtPaymentDate;

                lobjGhdv.LoadActiveProviderOrgPlan(idtPaymentDate);

                if (lobjGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    lobjGhdv.LoadRateStructureForUserStructureCode();
                }
                else
                {
                    lobjGhdv.LoadHealthParticipationDate();
                    //To Get the Rate Structure Code (Derived Field)
                    lobjGhdv.LoadRateStructure(idtPaymentDate);
                }

                //Get the Coverage Ref ID
                lobjGhdv.LoadCoverageRefID();

                //Get the Premium Amount
                lobjGhdv.GetMonthlyPremiumAmountByRefID(idtPaymentDate);

                if (lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID == 0)
                {
                    lblnErrorFound = true;
                }
                if (!lblnErrorFound)
                {
                    lstrCoverageCode =
                        GetGroupHealthCoverageCodeDescription(lobjGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID);
                    //uat pir 1344
                    //--Start--//
                    ldecEmprSharePremium = ldecEmprShareFee = ldecEmprShareRHICAmt = ldecEmprShareOtherRHICAmt = ldecEmprShareJSRHICAmt = 0.0m;
                    ldecEmpShareBuydown = ldecEmpShareMedicarePartD = 0.0m;
                    if (!string.IsNullOrEmpty(lobjGhdv.icdoPersonAccountGhdv.cobra_type_value) &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                        lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)
                    {
                        ldecEmprSharePremium = Math.Round(lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount *
                                                    lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareFee = Math.Round(lobjGhdv.icdoPersonAccountGhdv.FeeAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmpShareBuydown = Math.Round(lobjGhdv.icdoPersonAccountGhdv.BuydownAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmpShareMedicarePartD = Math.Round(lobjGhdv.icdoPersonAccountGhdv.MedicarePartDAmount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);//PIR 14271
                        ldecEmprShareRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareOtherRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                        ldecEmprShareJSRHICAmt = Math.Round(lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount *
                            lobjGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                    }
                    ldecPremiumAmt = lobjGhdv.icdoPersonAccountGhdv.PremiumExcludingFeeAmount - ldecEmprSharePremium;
                    ldecGroupHealthFeeAmt = lobjGhdv.icdoPersonAccountGhdv.FeeAmount - ldecEmprShareFee;
                    ldecBuydownAmt = lobjGhdv.icdoPersonAccountGhdv.BuydownAmount - ldecEmpShareBuydown;
                    ldecMedicarePartD = lobjGhdv.icdoPersonAccountGhdv.MedicarePartDAmount - ldecEmpShareMedicarePartD;//PIR 14271

                    ldecRHICAmt = lobjGhdv.icdoPersonAccountGhdv.total_rhic_amount - ldecEmprShareRHICAmt;
                    /* UAT PIR 476, Including other and JS RHIC Amount */
                    ldecOthrRHICAmount = lobjGhdv.icdoPersonAccountGhdv.other_rhic_amount - ldecEmprShareOtherRHICAmt;
                    ldecJSRHICAmount = lobjGhdv.icdoPersonAccountGhdv.js_rhic_amount - ldecEmprShareJSRHICAmt;
                    /* UAT PIR 476 ends here */
                    //--End--//
                    ldecMemberPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecRHICAmt - ldecBuydownAmt + ldecMedicarePartD;//PIR 14271
                    ldecTotalPremiumAmt = ldecPremiumAmt + ldecGroupHealthFeeAmt - ldecBuydownAmt + ldecMedicarePartD;//PIR 14271
                    ldecProviderPremiumAmt = ldecPremiumAmt;
                }
            }

            return ldecTotalPremiumAmt;
        }
    }
}
