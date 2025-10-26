#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.ExceptionPub;
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFilePaymentInfoInbound : busFileBase
    {
        public busFilePaymentInfoInbound()
        {

        }

        bool lblnErrorFound;

        private Collection<busRemittance> _iclbRemittance;
        public Collection<busRemittance> iclbRemittance
        {
            get { return _iclbRemittance; }
            set { _iclbRemittance = value; }
        }

        private busRemittance _ibusRemittance;
        public busRemittance ibusRemittance
        {
            get { return _ibusRemittance; }
            set { _ibusRemittance = value; }
        }

        public override void InitializeFile()
        {
            GetCentralPayrollOrgID();
            base.InitializeFile();
        }

        public int iintCentralPayrollOrgID;
        private bool iblnErrorInSubTotal;

        public void GetCentralPayrollOrgID()
        {
            string lstrOrgCodeID = busGlobalFunctions.GetData1ByCodeValue(
                busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstant_CentraPayrollOrg, iobjPassInfo);
            iintCentralPayrollOrgID = busGlobalFunctions.GetOrgIdFromOrgCode(lstrOrgCodeID);
        }

        public override busBase NewDetail()
        {
            _ibusRemittance = new busRemittance();
            _ibusRemittance.icdoRemittance = new cdoRemittance();
            return _ibusRemittance;
        }

        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrObjectField;
            string lstrReturnValue = astrFieldValue;
            if (astrFieldName.IndexOf(".") > -1)
                lstrObjectField = astrFieldName.Substring(astrFieldName.LastIndexOf(".") + 1);
            else
                lstrObjectField = astrFieldName;

            if (lstrObjectField == "istrOrgCodeID")
            {
                _ibusRemittance.icdoRemittance.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(lstrReturnValue);
            }
            if (lstrObjectField == "istrCheckNo")
            {
                DataTable ldtbDeposit = DBFunction.DBSelect("cdoRemittance.GetDepositIDforPaymentFile", new object[2] { lstrReturnValue, iintCentralPayrollOrgID },
                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbDeposit.Rows.Count > 0)
                    _ibusRemittance.icdoRemittance.deposit_id = Convert.ToInt32(ldtbDeposit.Rows[0]["deposit_id"]);
            }
            if (lstrObjectField == "remittance_type_value")
            {
                lstrReturnValue = GetBenefitType(lstrReturnValue);
            }
            if (lstrObjectField == "istrPlanCode")
            {
                _ibusRemittance.icdoRemittance.plan_id =
                    Convert.ToInt32(DBFunction.DBExecuteScalar("cdoRemittance.GetPlanIDByPlanCode",
                                                               new object[1] { lstrReturnValue },
                                                               iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            }
            return lstrReturnValue;
        }

        public decimal ldclTotRemittedAmount = 0.00M;

        public override void ProcessDetail()
        {
            lblnErrorFound = false;

            ArrayList larrErrors = new ArrayList();
            utlError lobjError = new utlError();

            if (_iclbRemittance == null)
                _iclbRemittance = new Collection<busRemittance>();

            if (_ibusRemittance.icdoRemittance.remittance_type_value == "TOT")
            {
                if (_ibusRemittance.icdoRemittance.remittance_amount != ldclTotRemittedAmount)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4149";
                    lobjError.istrErrorMessage = "Total Remittance Amount mismatch for Org Code: " + _ibusRemittance.istrOrgCodeID;
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                    iblnErrorInSubTotal = true;
                }
                ldclTotRemittedAmount = 0.0M;
            }
            else
            {
                ldclTotRemittedAmount += _ibusRemittance.icdoRemittance.remittance_amount;
                _iclbRemittance.Add(_ibusRemittance);

                if (_ibusRemittance.icdoRemittance.deposit_id == 0)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4143";
                    lobjError.istrErrorMessage = "Deposit does not exists.";
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                }
                if (_ibusRemittance.icdoRemittance.remittance_type_value == string.Empty || _ibusRemittance.icdoRemittance.remittance_type_value == null)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4150";
                    lobjError.istrErrorMessage = "Invalid Remittance Type.";
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                }
                if (_ibusRemittance.icdoRemittance.plan_id == 0)
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4151";
                    lobjError.istrErrorMessage = "Invalid Plan.";
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                }
                if (!IsOrgExists(_ibusRemittance.icdoRemittance.org_id))
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4152";
                    lobjError.istrErrorMessage = "Organization doest not exists.";
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                }
                //PROD PIR 4500 : We must allow negative amount also
                //if (_ibusRemittance.icdoRemittance.remittance_amount < 0)
                //{
                //    lobjError = new utlError();
                //    lobjError.istrErrorID = "4153";
                //    lobjError.istrErrorMessage = "Negative Remittance Amount.";
                //    larrErrors.Add(lobjError);
                //    lblnErrorFound = true;
                //}
                if (!IsRemittanceTypeBankAccountValid())
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4153";
                    lobjError.istrErrorMessage = "Remittance Type is not related to Deposit Tape Bank Account.";
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                }
                if (!IsOrgActive(_ibusRemittance.istrOrgCodeID))
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4144";
                    lobjError.istrErrorMessage = "Organization is not Active.";
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                }
                if (!IsOrgExistsinPlan(_ibusRemittance.icdoRemittance.org_id, _ibusRemittance.icdoRemittance.plan_id))
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4145";
                    lobjError.istrErrorMessage = "Plan does not exists for the given Organization.";
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                }
                if (!IsPlanValidForRemittanceType(_ibusRemittance.icdoRemittance.plan_id, _ibusRemittance.icdoRemittance.remittance_type_value))
                {
                    lobjError = new utlError();
                    lobjError.istrErrorID = "4142";
                    lobjError.istrErrorMessage = "Remitance Type, Plan combination doest not exists.";
                    larrErrors.Add(lobjError);
                    lblnErrorFound = true;
                }
            }
            if (lblnErrorFound)
            {
                _ibusRemittance.iarrErrors = larrErrors;
            }
        }

        public override bool ValidateFile()
        {
            //If any detail records with errors, let the header completes the process with warnings.
            foreach (busRemittance lobjRemittance in _iclbRemittance)
            {
                if (lobjRemittance.iarrErrors.Count > 0) return true;
            }

            ArrayList larrErrors = new ArrayList();
            if (IsTotalValid())
            {
                //If No Errors in Total Per ORG, Insert into Remittance
                if (!iblnErrorInSubTotal)
                {
                    foreach (busRemittance lobjRemittance in _iclbRemittance)
                    {
                        //PROD PIR 4500 : if the remittance amount is negative, change the remittance type to negative deposit
                        if (lobjRemittance.icdoRemittance.remittance_amount < 0)
                            lobjRemittance.icdoRemittance.remittance_type_value = busConstant.RemittanceTypeNegativeDeposit;
                        lobjRemittance.icdoRemittance.Insert();
                    }
                    //PIR 19516 - Negative Deposits Report
                    if (iclbRemittance.Any(rem => rem.icdoRemittance.remittance_amount < 0))
                    {
                        DataTable ldtNegataiveTable = CreateNegativeDepositReportTable();
                        ldtNegataiveTable.TableName = busConstant.ReportTableName;
                        foreach (busRemittance lbusRemittance in iclbRemittance.Where(rem => rem.icdoRemittance.remittance_amount < 0))
                        {
                            if (lbusRemittance.icdoRemittance.org_id > 0 && lbusRemittance.icdoRemittance.plan_id > 0)
                            {
                                lbusRemittance.LoadOrganization();
                                lbusRemittance.LoadPlan();
                                DataRow ldtrRow = ldtNegataiveTable.NewRow();
                                ldtrRow["ORG_CODE"] = lbusRemittance.ibusOrganization.icdoOrganization.org_code;
                                ldtrRow["ORG_NAME"] = lbusRemittance.ibusOrganization.icdoOrganization.org_name;
                                ldtrRow["PLAN_NAME"] = lbusRemittance.ibusPlan.icdoPlan.plan_name;
                                ldtrRow["AMOUNT"] = lbusRemittance.icdoRemittance.remittance_amount;
                                ldtNegataiveTable.Rows.Add(ldtrRow);
                            }
                        }
                        if (ldtNegataiveTable.Rows.Count > 0)
                        {
                            busNeoSpinBase lbusNeoSpinBase = new busNeoSpinBase();
                            lbusNeoSpinBase.CreateReport("rptCentralPayrollNegativeDeposits.rpt", ldtNegataiveTable, string.Empty);
                        }
                    }
                    /// Get Deposit Tape ID.
                    busDeposit lobjDeposit = new busDeposit();
                    lobjDeposit.ibusDepositTape = new busDepositTape();
                    lobjDeposit.ibusDepositTape.icdoDepositTape = new cdoDepositTape();
                    lobjDeposit.FindDeposit(_ibusRemittance.icdoRemittance.deposit_id);
                    lobjDeposit.ibusDepositTape.FindDepositTape(lobjDeposit.icdoDeposit.deposit_tape_id);
                    lobjDeposit.ibusDepositTape.LoadDepositsCountAndTotalAmount();
                    // Validate Deposit Tape.
                    larrErrors = lobjDeposit.ibusDepositTape.ValidateDepositTape();
                    if (larrErrors.Count > 0)
                    {
                        if (larrErrors[0] is utlError)
                        {
                            utlError lobjError = (utlError)larrErrors[0];
                            this.istrError = lobjError.istrErrorMessage;
                            return false;
                        }
                    }
                    lobjDeposit.icdoDeposit.Select();
                    lobjDeposit.LoadRemittances();
                    // Applying the Deposit Tape.
                    larrErrors = lobjDeposit.btnApply_Click();
                    if (larrErrors.Count > 0)
                    {
                        if (larrErrors[0] is utlError)
                        {
                            utlError lobjError = (utlError)larrErrors[0];
                            this.istrError = lobjError.istrErrorMessage;
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                this.istrError = "Sum of Total Remittance amount does not match with Deposit Amount.";
                return false;
            }
        }

        private DataTable CreateNegativeDepositReportTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("ORG_CODE", Type.GetType("System.String"));
            DataColumn dc2 = new DataColumn("ORG_NAME", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("PLAN_NAME", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("AMOUNT", Type.GetType("System.Decimal"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            return ldtbReportTable;
        }

        public bool IsTotalValid()
        {
            decimal ldclTotalAmount = 0.00M;
            foreach (busRemittance lobjRemittance in _iclbRemittance)
            {
                ldclTotalAmount += lobjRemittance.icdoRemittance.remittance_amount;
            }
            busDeposit lobjDeposit = new busDeposit();
            lobjDeposit.FindDeposit(_ibusRemittance.icdoRemittance.deposit_id);
            if (lobjDeposit.icdoDeposit.deposit_amount == ldclTotalAmount)
                return true;
            return false;
        }

        public bool IsRemittanceTypeBankAccountValid()
        {
            bool lblnFlag = false;
            if (_ibusRemittance.icdoRemittance.deposit_id > 0)
            {
                busDeposit lobjDeposit = new busDeposit();
                lobjDeposit.FindDeposit(_ibusRemittance.icdoRemittance.deposit_id);
                busDepositTape lobjDepositTape = new busDepositTape();
                lobjDepositTape.FindDepositTape(lobjDeposit.icdoDeposit.deposit_tape_id);
                if (_ibusRemittance.icdoRemittance.remittance_type_value != null)
                {
                    int lintCount =
                        Convert.ToInt32(DBFunction.DBExecuteScalar("cdoRemittance.ValidateByBankAccountRemittanceType",
                                                                   new object[2]
                                                                   {
                                                                       _ibusRemittance.icdoRemittance.remittance_type_value
                                                                       , lobjDepositTape.icdoDepositTape.bank_account_value
                                                                   },
                                                                   iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    if (lintCount > 0)
                        lblnFlag = true;
                }
            }
            return lblnFlag;

        }

        public bool IsPlanValidForRemittanceType(int aintPlanID, string astrRemittanceType)
        {
            bool lblnFlag = false;
            if (astrRemittanceType != null)
            {
                int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoRemittance.ValidateRemitanceTypePlanID", new object[2] { aintPlanID, astrRemittanceType },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if (lintCount > 0)
                    lblnFlag = true;
            }
            return lblnFlag;
        }

        public bool IsOrgExists(int aintOrgID)
        {
            bool lblnFlag = false;
            int lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoRemittance.ValidateOrgExists", new object[1] { aintOrgID },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            if (lintCount > 0)
                lblnFlag = true;
            return lblnFlag;
        }

        public bool IsOrgExistsinPlan(int aintOrgID, int aintPlanID)
        {
            bool lblnFlag = false;
            int lintCount = 0;
            if ((IsOrgExists(aintOrgID)) && aintPlanID != 0)
            {
                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoRemittance.ValidateOrgExistsinPlan", new object[2] { aintOrgID, aintPlanID },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            }
            if (lintCount > 0)
                lblnFlag = true;
            return lblnFlag;
        }

        public bool IsOrgActive(string astrOrgCodeID)
        {
            bool lblnFlag = false;
            int lintCount = 0;
            if (IsOrgExists(busGlobalFunctions.GetOrgIdFromOrgCode(astrOrgCodeID)))
            {
                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoRemittance.CheckOrgActiveByOrgCode", new object[1] { astrOrgCodeID },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            }
            if (lintCount > 0)
                lblnFlag = true;
            return lblnFlag;
        }

        public string GetBenefitType(string AstrBenefitType)
        {
            String lstrBenefitType = string.Empty;
            if (AstrBenefitType == "RET")
            {
                lstrBenefitType = "CNTR";
            }
            else if (AstrBenefitType == "PUR")
            {
                lstrBenefitType = "PURC";
            }
            else if (AstrBenefitType == "BPP")
            {
                lstrBenefitType = "BPBK";
            }
            else if (AstrBenefitType == "SEM")
            {
                lstrBenefitType = "SMNR";
            }
            else if (AstrBenefitType == "DEF")
            {
                lstrBenefitType = "DCMD";
            }
            else if (AstrBenefitType == "RHIC")
            {
                lstrBenefitType = "RHCO";
            }
            else if (AstrBenefitType == "RHIP")
            {
                lstrBenefitType = "RHPU";
            }
            else if (AstrBenefitType == "HEA")
            {
                lstrBenefitType = "HLDP";
            }
            else if (AstrBenefitType == "MED")
            {
                lstrBenefitType = "MPDD";
            }
            else if (AstrBenefitType == "HMO")
            {
                lstrBenefitType = "HMOD";
            }
            else if (AstrBenefitType == "LIF")
            {
                lstrBenefitType = "LIDP";
            }
            else if (AstrBenefitType == "EAP")
            {
                lstrBenefitType = "EAPD";
            }
            else if (AstrBenefitType == "DEN")
            {
                lstrBenefitType = "DNDP";
            }
            else if (AstrBenefitType == "VIS")
            {
                lstrBenefitType = "VISD";
            }
            else if (AstrBenefitType == "JSR")
            {
                lstrBenefitType = "JSRD";
            }
            else if (AstrBenefitType == "FLE")
            {
                lstrBenefitType = "FLXD";
            }
            else if (AstrBenefitType == "TOT")
            {
                lstrBenefitType = "TOT";
            }
            else if (AstrBenefitType == "LTC")
            {
                lstrBenefitType = "LTCD";
            }
            return lstrBenefitType;
        }
    }
}
