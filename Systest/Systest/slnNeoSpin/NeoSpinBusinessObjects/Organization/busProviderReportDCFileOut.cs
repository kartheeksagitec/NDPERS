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
using Sagitec.ExceptionPub;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busProviderReportDCFileOut : busFileBaseOut
    {
        public busProviderReportDCFileOut()
        {
        }

        private string lstrProviderOrgCodeID = string.Empty;
        private int lintProviderOrgID;
        private bool iblnIsDCTransfer = false;

        private Collection<busProviderReportDataDC> _iclbDCFileForFidelity;
        public Collection<busProviderReportDataDC> iclbDCFileForFidelity
        {
            get { return _iclbDCFileForFidelity; }
            set { _iclbDCFileForFidelity = value; }
        }

        public override void InitializeFile()
        {
            istrFileName = "DF.ER703020_" + DateTime.Now.ToString(busConstant.DateFormat) + "_" + lstrProviderOrgCodeID + busConstant.FileFormattxt;
            if(iblnIsDCTransfer)
                istrFileName = "DC_Transfer_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
        }

        private string istrFromEmail;

        public override void FinalizeFile()
        {
            if (!iblnIsDCTransfer)
            {
                Collection<busCodeValue> lclbCodeValue = busGlobalFunctions.LoadCodeValueByData1(5012, lstrProviderOrgCodeID);
                if (lclbCodeValue.Count > 0)
                {
                    try
                    {
                        // PROD PIR ID 512
                        busGlobalFunctions.SendMail(busGlobalFunctions.GetSysManagementEmailNotification(), lclbCodeValue[0].icdoCodeValue.data2,
                                                        istrFileName + " file is generated.", lclbCodeValue[0].icdoCodeValue.comments, true, true);
                    }
                    catch (Exception Ex)
                    {
                        ExceptionManager.Publish(Ex);
                    }
                }
            }
            base.FinalizeFile();
        }

        public DataTable GetDataTable()
        {
            DataTable ldtbProviderReportDataforDC = new DataTable();
            DataColumn SSN = new DataColumn("SSN");
            DataColumn Amount = new DataColumn("Amount", typeof(decimal));
            DataColumn Source = new DataColumn("Source");
            ldtbProviderReportDataforDC.Columns.Add(SSN);
            ldtbProviderReportDataforDC.Columns.Add(Amount);
            ldtbProviderReportDataforDC.Columns.Add(Source);
            return ldtbProviderReportDataforDC;
        }

        public void AddDCDataRow(DataTable adtDCTable, string astrSSN, string astrSource, decimal adecAmount)
        {
            DataRow ldr = adtDCTable.NewRow();
            ldr["SSN"] = astrSSN;
            ldr["Source"] = astrSource;
            ldr["Amount"] = adecAmount;
            adtDCTable.Rows.Add(ldr);
        }

        /// PIR ID 449
        public bool IsMutualFundYes(int aintPersonID,int aintPlanID)
        {
            busPerson lobjPerson = new busPerson();
            lobjPerson.icdoPerson = new cdoPerson();
            lobjPerson.FindPerson(aintPersonID);
            lobjPerson.LoadRetirementAccount();
            foreach (busPersonAccount lobjPersonAccount in lobjPerson.iclbRetirementAccount)
            {
                if (lobjPersonAccount.icdoPersonAccount.plan_id == aintPlanID)
                {
                    busPersonAccountRetirement lobjRetirement = new busPersonAccountRetirement();
                    lobjRetirement.icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
                    lobjRetirement.FindPersonAccountRetirement(lobjPersonAccount.icdoPersonAccount.person_account_id);
                    if (lobjRetirement.icdoPersonAccountRetirement.mutual_fund_window_flag == busConstant.Flag_Yes)
                    {
                        return true;
                    }
                }
            }
            return  false;
        }

        public void LoadProviderReportDataforDC(DataTable ldtbDCRecordsBySSN)
        {
            lstrProviderOrgCodeID = Convert.ToString(iarrParameters[0]);
            lintProviderOrgID = busGlobalFunctions.GetOrgIdFromOrgCode(lstrProviderOrgCodeID);
            DateTime ldtPaymentDate = Convert.ToDateTime(iarrParameters[1]);
            istrFromEmail = Convert.ToString(iarrParameters[2]);
            ldtbDCRecordsBySSN = busNeoSpinBase.Select("cdoProviderReportDataDc.LoadReportDataByProvider", new object[2] { lintProviderOrgID, ldtPaymentDate });
            DataTable ldtbProviderReportDataforDC = GetDataTable();
            foreach (DataRow drDC in ldtbDCRecordsBySSN.Rows)
            {
                int lintPersonID=0,lintPlanID=0;
                string lstrSSN = string.Empty, lstrEEPreTax = string.Empty, lstrEEPostTax = string.Empty, lstrER = string.Empty;
                if(drDC["ssn"]!=DBNull.Value)
                    lstrSSN = drDC["ssn"].ToString();
                if(drDC["person_id"]!=DBNull.Value)
                    lintPersonID = Convert.ToInt32(drDC["person_id"]);
                if(drDC["plan_id"]!=DBNull.Value)
                    lintPlanID = Convert.ToInt32(drDC["plan_id"]);
                if (IsMutualFundYes(lintPersonID,lintPlanID))
                {
                    lstrEEPreTax = "14"; lstrEEPostTax = "15"; lstrER = "07";
                }
                else
                {
                    lstrEEPreTax = "02"; lstrEEPostTax = "06"; lstrER = "01";
                }
                decimal SumOfPreTaxEmpPickupMmbrInt = Convert.ToDecimal(drDC["SumOfPreTaxEmpPickupMmbrInt"]);
                decimal SumOfPostTax = Convert.ToDecimal(drDC["SumOfPostTax"]);
                decimal SumofEREmpInt = Convert.ToDecimal(drDC["SumofEREmpInt"]);
                if (!((SumOfPreTaxEmpPickupMmbrInt == 0) &&
                    (SumOfPostTax == 0) &&
                    (SumofEREmpInt == 0)))
                {
                    AddDCDataRow(ldtbProviderReportDataforDC, lstrSSN, lstrEEPreTax, SumOfPreTaxEmpPickupMmbrInt);
                    AddDCDataRow(ldtbProviderReportDataforDC, lstrSSN, lstrEEPostTax, SumOfPostTax);
                    AddDCDataRow(ldtbProviderReportDataforDC, lstrSSN, lstrER, SumofEREmpInt);
                }
            }
            _iclbDCFileForFidelity = new Collection<busProviderReportDataDC>();
            foreach (DataRow dr in ldtbProviderReportDataforDC.Rows)
            {
                busProviderReportDataDC lobjProviderReportDataDC = new busProviderReportDataDC();
                lobjProviderReportDataDC.icdoProviderReportDataDc = new cdoProviderReportDataDc();
                lobjProviderReportDataDC.icdoProviderReportDataDc.LoadData(dr);
                lobjProviderReportDataDC.ldclTotalContributionAmount = Convert.ToDecimal(dr["Amount"]);
                lobjProviderReportDataDC.icdoProviderReportDataDc.Source = Convert.ToString(dr["Source"]);
                _iclbDCFileForFidelity.Add(lobjProviderReportDataDC);
            }
        }

        public override bool ValidateFile()
        {
            bool lblnFlag = true;
            if (_iclbDCFileForFidelity.Count <= 0)
            {
                this.istrError = "No Records Exists.";
                lblnFlag = false;
            }
            return lblnFlag;
        }

        /// <summary>
        /// Method to load the collection with DC transfer records
        /// </summary>
        /// <param name="adtDCTranfer">Data table</param>
        public void LoadDCTransferFile(DataTable adtDCTranfer)
        {
            _iclbDCFileForFidelity = new Collection<busProviderReportDataDC>();
            iblnIsDCTransfer = Convert.ToBoolean(iarrParameters[0]);
            DataTable ldtDCTransfer = (DataTable)iarrParameters[1];
            foreach (DataRow dr in ldtDCTransfer.Rows)
            {
                busProviderReportDataDC lobjProviderReportDataDC = new busProviderReportDataDC();
                lobjProviderReportDataDC.icdoProviderReportDataDc = new cdoProviderReportDataDc();
                if(dr["SSN"] != DBNull.Value)
                    lobjProviderReportDataDC.icdoProviderReportDataDc.ssn = Convert.ToString(dr["SSN"]);
                if (dr["AMOUNT"] != DBNull.Value)
                    lobjProviderReportDataDC.ldclTotalContributionAmount = Convert.ToDecimal(dr["AMOUNT"]);
                if (dr["SOURCE"] != DBNull.Value)
                    lobjProviderReportDataDC.icdoProviderReportDataDc.Source = Convert.ToString(dr["SOURCE"]);
                _iclbDCFileForFidelity.Add(lobjProviderReportDataDC);
            }
        }
    }
}
