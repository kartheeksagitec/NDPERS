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
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busIbsCheckEntryHeader:
    /// Inherited from busIbsCheckEntryHeaderGen, the class is used to customize the business object busIbsCheckEntryHeaderGen.
    /// </summary>
    [Serializable]
    public class busIbsCheckEntryHeader : busIbsCheckEntryHeaderGen
    {
        //Create deposit tape ,deposit details and remittances for ibs check details and post deposit tape id into ibs check entry header
        public ArrayList btnSubmit_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();

            if (iclbIbsCheckEntryDetail == null)
                LoadIbsCheckEntryDetail();
            if (iclbIbsCheckEntryDetail.Count == 0)
            {
                lobjError = AddError(4183, "");
                larrList.Add(lobjError);
                return larrList;
            }
            if (IsDepositTotalRequired())
            {
                lobjError = AddError(4178, "");
                larrList.Add(lobjError);
                return larrList;
            }

            if (IsTotalAmountNotValid())
            {
                lobjError = AddError(4179, "");
                larrList.Add(lobjError);
                return larrList;
            }

            //Create Deposit Tape BR-033-42
            busDepositTape lbusDepositTape = new busDepositTape { icdoDepositTape = new cdoDepositTape() };
            lbusDepositTape.icdoDepositTape.deposit_method_value = icdoIbsCheckEntryHeader.deposit_method_value;
            lbusDepositTape.icdoDepositTape.deposit_date = icdoIbsCheckEntryHeader.deposit_date;
            lbusDepositTape.icdoDepositTape.status_value = busConstant.DepositTapeStatusValid;
            lbusDepositTape.icdoDepositTape.bank_account_value = busConstant.DepositTapeAccountInsurance;
            lbusDepositTape.icdoDepositTape.total_amount = icdoIbsCheckEntryHeader.deposit_total;
            lbusDepositTape.icdoDepositTape.Insert();

            if (iclbIbsCheckEntryDetail == null)
                LoadIbsCheckEntryDetail();
            foreach (busIbsCheckEntryDetail lbusIbsCheckEntryDetail in iclbIbsCheckEntryDetail)
            {
                //Create Deposits as per  BR-033-42
                int lintDepositID = lbusDepositTape.CreateDeposit(lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.amount_paid,
                    lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.payment_date, busConstant.DepositDetailStatusApplied,
                    busConstant.DepositSourceRegularDeposits, lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.reference_no,
                    aintPersonId: lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.person_id);
                if (lbusIbsCheckEntryDetail.ibusPerson.IsNull())
                    lbusIbsCheckEntryDetail.LoadPerson();
                if (lbusIbsCheckEntryDetail.ibusPerson.iclbInsuranceAccounts == null)
                    lbusIbsCheckEntryDetail.ibusPerson.LoadInsuranceAccounts();

                Collection<busPersonAccount> lclbInsuranceAcc = busGlobalFunctions.Sort<busPersonAccount>("icdoPersonAccount.plan_id",
                    lbusIbsCheckEntryDetail.ibusPerson.iclbInsuranceAccounts);
                //Create remittance as per BR-033-43
                if (lclbInsuranceAcc.Count > 0)
                {
                    int lintRemittanceID = lbusDepositTape.CreateRemittance(lintDepositID, lclbInsuranceAcc[0].icdoPersonAccount.plan_id,
                                                lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.amount_paid,
                                                busConstant.RemittanceTypeIBSDeposit, DateTime.Today,
                                                aintPersonId: lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.person_id);

                    //Generate GL                            
                    lbusDepositTape.CreateGL(lclbInsuranceAcc[0].icdoPersonAccount.plan_id, lintRemittanceID, lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.amount_paid,
                        busConstant.RemittanceTypeIBSDeposit, aintPersonId: lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.person_id);
                }
                lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.deposit_id = lintDepositID;
                lbusIbsCheckEntryDetail.icdoIbsCheckEntryDetail.Update();
            }
            icdoIbsCheckEntryHeader.status_value = busConstant.IBSCheckEntryHeaderStatusProcessed;
            icdoIbsCheckEntryHeader.deposit_tape_id = lbusDepositTape.icdoDepositTape.deposit_tape_id;
            icdoIbsCheckEntryHeader.Update();
            LoadIbsCheckEntryDetail();
            FindIbsCheckEntryHeader(icdoIbsCheckEntryHeader.ibs_check_entry_header_id);
            EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }

        //if deposit total and check entry detail total is not matching,throw an error. BR-33-47

        public bool IsTotalAmountNotValid()
        {
            if (iclbIbsCheckEntryDetail == null)
                LoadIbsCheckEntryDetail();
            decimal lcedsum = iclbIbsCheckEntryDetail.Sum(o => o.icdoIbsCheckEntryDetail.amount_paid);
            if (iclbIbsCheckEntryDetail.Count > 0 &&
                icdoIbsCheckEntryHeader.deposit_total != lcedsum)
                return true;
            else
                return false;
        }

        // deposit total is required when details are created.

        public bool IsDepositTotalRequired()
        {
            if (iclbIbsCheckEntryDetail == null)
                LoadIbsCheckEntryDetail();
            if (iclbIbsCheckEntryDetail.Count > 0 && icdoIbsCheckEntryHeader.deposit_total == 0.0m)
                return true;
            else
                return false;
        }

        public void LoadTotalFromDetails()
        {
            if (iclbIbsCheckEntryDetail == null)
                DisplayIbsCheckEntryDetail();
            //LoadIbsCheckEntryDetail();  //PIR 6617
            icdoIbsCheckEntryHeader.idecTotalAmountFromDetail = iclbIbsCheckEntryDetailFromQuery.Sum(o => o.icdoIbsCheckEntryDetail.amount_paid);  //PIR 6617
        }

        //PIR 6617
        public Collection<busIbsCheckEntryDetail> iclbIbsCheckEntryDetailFromQuery { get; set; }
        public void DisplayIbsCheckEntryDetail()
        {
            if (icdoIbsCheckEntryHeader == null)
                icdoIbsCheckEntryHeader = new cdoIbsCheckEntryHeader();
            DataTable ldtbCheckEnrtryDetail = Select("cdoIbsCheckEntryDetail.CheckDetails",
                new object[1] { icdoIbsCheckEntryHeader.ibs_check_entry_header_id});
            iclbIbsCheckEntryDetailFromQuery = GetCollection<busIbsCheckEntryDetail>(ldtbCheckEnrtryDetail, "icdoIbsCheckEntryDetail");  //PIR 6617

        }


    }
}