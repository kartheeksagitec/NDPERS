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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPayment1099r:
    /// Inherited from busPayment1099rGen, the class is used to customize the business object busPayment1099rGen.
    /// </summary>
    [Serializable]
    public class busPayment1099r : busPayment1099rGen
    {        
        public busPerson ibusPerson { get; set; }
        public string istrPlanName { get; set; }  //Added for PIR 8890
        public string istrCorrected1099R { get; set; } //PIR 11131

        //FW Upgrade :: Code Conversion for "View Report" method
        public string istrReportTemplateName { get; set; }
        public void LoadPerson()
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(icdoPayment1099r.person_id);
        }
       
        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
                ibusPayeeAccount = new busPayeeAccount();
            ibusPayeeAccount.FindPayeeAccount(icdoPayment1099r.payee_account_id);
        }
        /// <summary>
        /// Method to create a temp table and store data for annual batch
        /// </summary>
        /// <param name="aintTaxYear">Tax Year</param>
        public void CreateTemp1099rTableWithData(int aintTaxYear)
        {
            DBFunction.DBNonQuery("cdoPayment1099r.CreateTableFor1099r", new object[1] { aintTaxYear },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        /// <summary>
        /// Method to drop the temp table for 1099r
        /// </summary>
        public void DropTemp1099rTable()
        {
            DBFunction.DBNonQuery("cdoPayment1099r.DropTableFor1099r", new object[0] { },
                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        /// <summary>
        /// Method to drop the temp table for 1099r
        /// </summary>
        public void DropTrialTemp1099rTable()
        {
            DBFunction.DBNonQuery("cdoPayment1099r.DropTableFor1099r", new object[0] { },
                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            DBFunction.DBNonQuery("cdoPayment1099r.DropTempTableForPayment1099r", new object[0] { },
                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        /// <summary>
        /// Method to create a temp table and store data for annual batch
        /// </summary>
        /// <param name="aintTaxYear">Tax Year</param>
        public void CreateTempCorrected1099rTableWithData(int aintTaxYear)
        {
            DBFunction.DBNonQuery("cdoPayment1099r.CreateTableForCorrected1099r", new object[1] { aintTaxYear },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        /// <summary>
        /// Method to drop the temp table for 1099r
        /// </summary>
        public void DropTempCorrected1099rTable()
        {
            DBFunction.DBNonQuery("cdoPayment1099r.DropTableForCorrected1099r", new object[0] { },
                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }


        public ArrayList ViewReport_Click(int aint1009rID)
        {
            ArrayList larrResult = new ArrayList();
            DataSet lds = new DataSet();
            lds = GetDataSetToCreateReport(aint1009rID);          
            larrResult.Add("Successfully Added");
            larrResult.Add(lds);
            return larrResult;
        }

        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            return ibusPerson;
        }

        public DataSet GetDataSetToCreateReport(int aint1009rID)
        {
            this.FindPayment1099r(aint1009rID);
            
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.ibusPersonCurrentAddress == null)
                ibusPerson.LoadPersonCurrentAddress();

            DataSet lds = new DataSet("MLC");
            DataTable ldt1099r = new DataTable();
            DataColumn ldc1 = new DataColumn("RUN_YEAR", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("GROSS_DIST_AMOUNT", Type.GetType("System.Decimal"));
            DataColumn ldc3 = new DataColumn("TAXABLE_AMOUNT", Type.GetType("System.Decimal"));
            DataColumn ldc4 = new DataColumn("TAXABLE_AMT_NOT_DETERMINED_FLAG", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("TOTAL_DISTRIBUTION_FLAG", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("CAPITAL_GAIN", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("FEDERAL_TAX", Type.GetType("System.Decimal"));
            DataColumn ldc8 = new DataColumn("EMPLOYEE_CONTRIBUTION_AMT", Type.GetType("System.Decimal"));
            DataColumn ldc9 = new DataColumn("NET_AMOUNT", Type.GetType("System.Decimal"));
            DataColumn ldc10 = new DataColumn("DISTRIBUTION_CODE", Type.GetType("System.String"));
            DataColumn ldc11 = new DataColumn("COMMENTS", Type.GetType("System.String"));
            DataColumn ldc12 = new DataColumn("OTHER_AMOUNTS", Type.GetType("System.Decimal"));
            DataColumn ldc13 = new DataColumn("OTHER_PERCENTAGE", Type.GetType("System.Decimal"));
            DataColumn ldc14 = new DataColumn("TOTAL_DIST_PERCENTAGE", Type.GetType("System.Decimal"));
            DataColumn ldc15 = new DataColumn("TOTAL_EMPLOYEE_CONTRIB_AMT", Type.GetType("System.Decimal"));
            DataColumn ldc16 = new DataColumn("STATE_TAX", Type.GetType("System.Decimal"));
            DataColumn ldc17 = new DataColumn("STATE_DISTRIBUTION", Type.GetType("System.String"));
            DataColumn ldc18 = new DataColumn("LOCAL_TAX", Type.GetType("System.Decimal"));
            DataColumn ldc19 = new DataColumn("LOCALITY", Type.GetType("System.String"));
            DataColumn ldc20 = new DataColumn("LOCAL_DISTRIBUTION", Type.GetType("System.String"));
            DataColumn ldc21 = new DataColumn("PAYEE_ACCOUNT_ID", Type.GetType("System.Decimal"));
            DataColumn ldc22 = new DataColumn("PAYEE_NAME", Type.GetType("System.String"));
            DataColumn ldc23 = new DataColumn("SSN_TIN", Type.GetType("System.String"));
            DataColumn ldc24 = new DataColumn("ADDR_LINE_1", Type.GetType("System.String"));
            DataColumn ldc25 = new DataColumn("ADDR_LINE_2", Type.GetType("System.String"));
            DataColumn ldc26 = new DataColumn("ADDR_CITY", Type.GetType("System.String"));
            DataColumn ldc27 = new DataColumn("ADDR_STATE_VALUE", Type.GetType("System.String"));
            DataColumn ldc28 = new DataColumn("ADDR_ZIP_CODE", Type.GetType("System.String"));
            DataColumn ldc29 = new DataColumn("CORRECTED_FLAG", Type.GetType("System.String"));
            DataColumn ldc30 = new DataColumn("ADDR_LINE_3", Type.GetType("System.String"));
            //PIR 8800
            DataColumn ldc31 = new DataColumn("ID_SUFFIX", Type.GetType("System.String"));
            ldt1099r.Columns.Add(ldc1);
            ldt1099r.Columns.Add(ldc2);
            ldt1099r.Columns.Add(ldc3);
            ldt1099r.Columns.Add(ldc4);
            ldt1099r.Columns.Add(ldc5);
            ldt1099r.Columns.Add(ldc6);
            ldt1099r.Columns.Add(ldc7);
            ldt1099r.Columns.Add(ldc8);
            ldt1099r.Columns.Add(ldc9);
            ldt1099r.Columns.Add(ldc10);
            ldt1099r.Columns.Add(ldc11);
            ldt1099r.Columns.Add(ldc12);
            ldt1099r.Columns.Add(ldc13);
            ldt1099r.Columns.Add(ldc14);
            ldt1099r.Columns.Add(ldc15);
            ldt1099r.Columns.Add(ldc16);
            ldt1099r.Columns.Add(ldc17);
            ldt1099r.Columns.Add(ldc18);
            ldt1099r.Columns.Add(ldc19);
            ldt1099r.Columns.Add(ldc20);
            ldt1099r.Columns.Add(ldc21);
            ldt1099r.Columns.Add(ldc22);
            ldt1099r.Columns.Add(ldc23);
            ldt1099r.Columns.Add(ldc24);
            ldt1099r.Columns.Add(ldc25);
            ldt1099r.Columns.Add(ldc26);
            ldt1099r.Columns.Add(ldc27);
            ldt1099r.Columns.Add(ldc28);
            ldt1099r.Columns.Add(ldc29);
            ldt1099r.Columns.Add(ldc30);
            ldt1099r.Columns.Add(ldc31);
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            DataRow dr = ldt1099r.NewRow();
            dr["RUN_YEAR"] = icdoPayment1099r.tax_year;
            dr["GROSS_DIST_AMOUNT"] = icdoPayment1099r.gross_benefit_amount;
            dr["TAXABLE_AMOUNT"] = icdoPayment1099r.taxable_amount;
            dr["TAXABLE_AMT_NOT_DETERMINED_FLAG"] = "";
            dr["TOTAL_DISTRIBUTION_FLAG"] = icdoPayment1099r.total_distribution_flag;
            dr["CAPITAL_GAIN"] = icdoPayment1099r.capital_gain;
            dr["FEDERAL_TAX"] = icdoPayment1099r.fed_tax_amount;
            dr["EMPLOYEE_CONTRIBUTION_AMT"] = icdoPayment1099r.non_taxable_amount;
            dr["NET_AMOUNT"] = icdoPayment1099r.net_amount;
            dr["DISTRIBUTION_CODE"] = icdoPayment1099r.distribution_code;
            dr["COMMENTS"] = "";
            dr["OTHER_AMOUNTS"] = 0.00m;
            dr["OTHER_PERCENTAGE"] = 0.00m;
            dr["TOTAL_DIST_PERCENTAGE"] = icdoPayment1099r.dist_percentage;
            if (icdoPayment1099r.tax_year == ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date.Year)
                dr["TOTAL_EMPLOYEE_CONTRIB_AMT"] = ibusPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance;
            dr["STATE_TAX"] = icdoPayment1099r.state_tax_amount;
            dr["STATE_DISTRIBUTION"] = "";
            dr["LOCAL_TAX"] = 0.0m;
            dr["LOCALITY"] = "";
            dr["LOCAL_DISTRIBUTION"] = "";
            dr["PAYEE_ACCOUNT_ID"] = icdoPayment1099r.payee_account_id;
            dr["PAYEE_NAME"] = icdoPayment1099r.name.ToUpper();
            dr["SSN_TIN"] = icdoPayment1099r.ssn;
            dr["ADDR_LINE_1"] = string.IsNullOrEmpty(icdoPayment1099r.addr_line_1) ? string.Empty : icdoPayment1099r.addr_line_1.ToUpper(); //PIR 10692
            dr["ADDR_LINE_2"] = string.IsNullOrEmpty(icdoPayment1099r.addr_line_2) ? string.Empty : icdoPayment1099r.addr_line_2.ToUpper();
            dr["ADDR_CITY"] = string.IsNullOrEmpty(icdoPayment1099r.addr_city) ? string.Empty : icdoPayment1099r.addr_city.ToUpper();
            dr["ADDR_STATE_VALUE"] = string.IsNullOrEmpty(icdoPayment1099r.addr_state_value) ? string.Empty : icdoPayment1099r.addr_state_value.ToUpper();
            dr["ADDR_ZIP_CODE"] = string.IsNullOrEmpty(icdoPayment1099r.addr_zip_code) ? string.Empty : icdoPayment1099r.addr_zip_code;
            string lstrForeignProvince = string.IsNullOrEmpty(icdoPayment1099r.foreign_province) ? string.Empty : icdoPayment1099r.foreign_province.ToUpper();
            string lstrForeignPostalCode = string.IsNullOrEmpty(icdoPayment1099r.foreign_postal_code) ? string.Empty : icdoPayment1099r.foreign_postal_code;
            lstrForeignProvince += " " + lstrForeignPostalCode;
            dr["ADDR_LINE_3"] = lstrForeignProvince;
            dr["CORRECTED_FLAG"] = icdoPayment1099r.corrected_flag;
            dr["ID_SUFFIX"] = string.IsNullOrEmpty(icdoPayment1099r.id_suffix) ? string.Empty : icdoPayment1099r.id_suffix;
            //PIR 8800
            //DataTable ldtbIDSuffix = Select("cdoPayment1099r.1099rGetIDSuffix", new object[3] { icdoPayment1099r.payee_account_id,icdoPayment1099r.tax_year,icdoPayment1099r.payment_1099r_id});
            //if (ldtbIDSuffix.Rows.Count > 0)
            //{
            //    foreach(DataRow ldr in ldtbIDSuffix.Rows)
            //        dr["ID_SUFFIX"] = ldr["ID_SUFFIX"];
            //}
            ldt1099r.Rows.Add(dr);
            ldt1099r.TableName = busConstant.ReportTableName;
            //PIR-16715 created report file with respective year
            ldt1099r.ExtendedProperties.Add("sgrReportName", "rptForm1099R_"+dr["run_year"] +".rpt");
            lds.Tables.Add(ldt1099r);
            return lds;
        }

        public DataSet GetDataSetToCreateReportMVVM(int aint1009rID)
        {
            this.FindPayment1099r(aint1009rID);

            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.ibusPersonCurrentAddress == null)
                ibusPerson.LoadPersonCurrentAddress();

            DataSet lds = new DataSet("MLC");
            DataTable ldt1099r = new DataTable();
            DataColumn ldc1 = new DataColumn("RUN_YEAR", Type.GetType("System.Int32"));
            DataColumn ldc2 = new DataColumn("GROSS_DIST_AMOUNT", Type.GetType("System.Decimal"));
            DataColumn ldc3 = new DataColumn("TAXABLE_AMOUNT", Type.GetType("System.Decimal"));
            DataColumn ldc4 = new DataColumn("TAXABLE_AMT_NOT_DETERMINED_FLAG", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("TOTAL_DISTRIBUTION_FLAG", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("CAPITAL_GAIN", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("FEDERAL_TAX", Type.GetType("System.Decimal"));
            DataColumn ldc8 = new DataColumn("EMPLOYEE_CONTRIBUTION_AMT", Type.GetType("System.Decimal"));
            DataColumn ldc9 = new DataColumn("NET_AMOUNT", Type.GetType("System.Decimal"));
            DataColumn ldc10 = new DataColumn("DISTRIBUTION_CODE", Type.GetType("System.String"));
            DataColumn ldc11 = new DataColumn("COMMENTS", Type.GetType("System.String"));
            DataColumn ldc12 = new DataColumn("OTHER_AMOUNTS", Type.GetType("System.Decimal"));
            DataColumn ldc13 = new DataColumn("OTHER_PERCENTAGE", Type.GetType("System.Decimal"));
            DataColumn ldc14 = new DataColumn("TOTAL_DIST_PERCENTAGE", Type.GetType("System.Decimal"));
            DataColumn ldc15 = new DataColumn("TOTAL_EMPLOYEE_CONTRIB_AMT", Type.GetType("System.Decimal"));
            DataColumn ldc16 = new DataColumn("STATE_TAX", Type.GetType("System.Decimal"));
            DataColumn ldc17 = new DataColumn("STATE_DISTRIBUTION", Type.GetType("System.String"));
            DataColumn ldc18 = new DataColumn("LOCAL_TAX", Type.GetType("System.Decimal"));
            DataColumn ldc19 = new DataColumn("LOCALITY", Type.GetType("System.String"));
            DataColumn ldc20 = new DataColumn("LOCAL_DISTRIBUTION", Type.GetType("System.String"));
            DataColumn ldc21 = new DataColumn("PAYEE_ACCOUNT_ID", Type.GetType("System.Decimal"));
            DataColumn ldc22 = new DataColumn("PAYEE_NAME", Type.GetType("System.String"));
            DataColumn ldc23 = new DataColumn("SSN_TIN", Type.GetType("System.String"));
            DataColumn ldc24 = new DataColumn("ADDR_LINE_1", Type.GetType("System.String"));
            DataColumn ldc25 = new DataColumn("ADDR_LINE_2", Type.GetType("System.String"));
            DataColumn ldc26 = new DataColumn("ADDR_CITY", Type.GetType("System.String"));
            DataColumn ldc27 = new DataColumn("ADDR_STATE_VALUE", Type.GetType("System.String"));
            DataColumn ldc28 = new DataColumn("ADDR_ZIP_CODE", Type.GetType("System.String"));
            DataColumn ldc29 = new DataColumn("CORRECTED_FLAG", Type.GetType("System.String"));
            DataColumn ldc30 = new DataColumn("ADDR_LINE_3", Type.GetType("System.String"));
            //PIR 8800
            DataColumn ldc31 = new DataColumn("ID_SUFFIX", Type.GetType("System.String"));
            ldt1099r.Columns.Add(ldc1);
            ldt1099r.Columns.Add(ldc2);
            ldt1099r.Columns.Add(ldc3);
            ldt1099r.Columns.Add(ldc4);
            ldt1099r.Columns.Add(ldc5);
            ldt1099r.Columns.Add(ldc6);
            ldt1099r.Columns.Add(ldc7);
            ldt1099r.Columns.Add(ldc8);
            ldt1099r.Columns.Add(ldc9);
            ldt1099r.Columns.Add(ldc10);
            ldt1099r.Columns.Add(ldc11);
            ldt1099r.Columns.Add(ldc12);
            ldt1099r.Columns.Add(ldc13);
            ldt1099r.Columns.Add(ldc14);
            ldt1099r.Columns.Add(ldc15);
            ldt1099r.Columns.Add(ldc16);
            ldt1099r.Columns.Add(ldc17);
            ldt1099r.Columns.Add(ldc18);
            ldt1099r.Columns.Add(ldc19);
            ldt1099r.Columns.Add(ldc20);
            ldt1099r.Columns.Add(ldc21);
            ldt1099r.Columns.Add(ldc22);
            ldt1099r.Columns.Add(ldc23);
            ldt1099r.Columns.Add(ldc24);
            ldt1099r.Columns.Add(ldc25);
            ldt1099r.Columns.Add(ldc26);
            ldt1099r.Columns.Add(ldc27);
            ldt1099r.Columns.Add(ldc28);
            ldt1099r.Columns.Add(ldc29);
            ldt1099r.Columns.Add(ldc30);
            ldt1099r.Columns.Add(ldc31);
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            DataRow dr = ldt1099r.NewRow();
            dr["RUN_YEAR"] = icdoPayment1099r.tax_year;
            dr["GROSS_DIST_AMOUNT"] = icdoPayment1099r.gross_benefit_amount;
            dr["TAXABLE_AMOUNT"] = icdoPayment1099r.taxable_amount;
            dr["TAXABLE_AMT_NOT_DETERMINED_FLAG"] = "";
            dr["TOTAL_DISTRIBUTION_FLAG"] = icdoPayment1099r.total_distribution_flag;
            dr["CAPITAL_GAIN"] = icdoPayment1099r.capital_gain;
            dr["FEDERAL_TAX"] = icdoPayment1099r.fed_tax_amount;
            dr["EMPLOYEE_CONTRIBUTION_AMT"] = icdoPayment1099r.non_taxable_amount;
            dr["NET_AMOUNT"] = icdoPayment1099r.net_amount;
            dr["DISTRIBUTION_CODE"] = icdoPayment1099r.distribution_code;
            dr["COMMENTS"] = "";
            dr["OTHER_AMOUNTS"] = 0.00m;
            dr["OTHER_PERCENTAGE"] = 0.00m;
            dr["TOTAL_DIST_PERCENTAGE"] = icdoPayment1099r.dist_percentage;
            if (icdoPayment1099r.tax_year == ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date.Year)
                dr["TOTAL_EMPLOYEE_CONTRIB_AMT"] = ibusPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance;
            dr["STATE_TAX"] = icdoPayment1099r.state_tax_amount;
            dr["STATE_DISTRIBUTION"] = "";
            dr["LOCAL_TAX"] = 0.0m;
            dr["LOCALITY"] = "";
            dr["LOCAL_DISTRIBUTION"] = "";
            dr["PAYEE_ACCOUNT_ID"] = icdoPayment1099r.payee_account_id;
            dr["PAYEE_NAME"] = icdoPayment1099r.name.ToUpper();
            dr["SSN_TIN"] = icdoPayment1099r.ssn;
            dr["ADDR_LINE_1"] = string.IsNullOrEmpty(icdoPayment1099r.addr_line_1) ? string.Empty : icdoPayment1099r.addr_line_1.ToUpper(); //PIR 10692
            dr["ADDR_LINE_2"] = string.IsNullOrEmpty(icdoPayment1099r.addr_line_2) ? string.Empty : icdoPayment1099r.addr_line_2.ToUpper();
            dr["ADDR_CITY"] = string.IsNullOrEmpty(icdoPayment1099r.addr_city) ? string.Empty : icdoPayment1099r.addr_city.ToUpper();
            dr["ADDR_STATE_VALUE"] = string.IsNullOrEmpty(icdoPayment1099r.addr_state_value) ? string.Empty : icdoPayment1099r.addr_state_value.ToUpper();
            dr["ADDR_ZIP_CODE"] = string.IsNullOrEmpty(icdoPayment1099r.addr_zip_code) ? string.Empty : icdoPayment1099r.addr_zip_code;
            string lstrForeignProvince = string.IsNullOrEmpty(icdoPayment1099r.foreign_province) ? string.Empty : icdoPayment1099r.foreign_province.ToUpper();
            string lstrForeignPostalCode = string.IsNullOrEmpty(icdoPayment1099r.foreign_postal_code) ? string.Empty : icdoPayment1099r.foreign_postal_code;
            lstrForeignProvince += " " + lstrForeignPostalCode;
            dr["ADDR_LINE_3"] = lstrForeignProvince;
            dr["CORRECTED_FLAG"] = icdoPayment1099r.corrected_flag;
            dr["ID_SUFFIX"] = string.IsNullOrEmpty(icdoPayment1099r.id_suffix) ? string.Empty : icdoPayment1099r.id_suffix;
            //PIR 8800
            //DataTable ldtbIDSuffix = Select("cdoPayment1099r.1099rGetIDSuffix", new object[3] { icdoPayment1099r.payee_account_id,icdoPayment1099r.tax_year,icdoPayment1099r.payment_1099r_id});
            //if (ldtbIDSuffix.Rows.Count > 0)
            //{
            //    foreach(DataRow ldr in ldtbIDSuffix.Rows)
            //        dr["ID_SUFFIX"] = ldr["ID_SUFFIX"];
            //}
            ldt1099r.Rows.Add(dr);
            ldt1099r.TableName = busConstant.ReportTableName;
            //PIR-16715 created report file with respective year
            //ldt1099r.ExtendedProperties.Add("sgrReportName", "rptForm1099R_" + dr["run_year"] + ".rpt");
            lds.Tables.Add(ldt1099r);
            return lds;
        }

        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            base.AddToResponse(aobjResponseData);
            Sagitec.Common.utlConstants.ilstReadonlyMethodNames.Add("btnGenerateReport_Click");
        }
    }
}
