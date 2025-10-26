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
using System.Collections.Generic;

#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPersonAccountInsuranceTransfer:
    /// Inherited from busPersonAccountInsuranceTransferGen, the class is used to customize the business object busPersonAccountInsuranceTransferGen.
    /// </summary>
    [Serializable]
    public class busPersonAccountInsuranceTransfer : busPersonAccountInsuranceTransferGen
    {

        # region Load Methods
        //this is loading the from Person account and the Person Account Def Comp
        public void LoadFromPersonAccount()
        {
            if (ibusFromPersonAccount == null)
            {
                ibusFromPersonAccount = new busPersonAccount();
            }
            ibusFromPersonAccount.FindPersonAccount(icdoPersonAccountInsuranceTransfer.from_person_account_id);
        }

        //this is loading the To Person account and the Person Account Def Comp
        public void LoadToPersonAccount()
        {
            if (ibusToPersonAccount == null)
            {
                ibusToPersonAccount = new busPersonAccount();
            }
            ibusToPersonAccount.FindPersonAccount(icdoPersonAccountInsuranceTransfer.to_person_account_id);
        }

        //Load Amount For insurance
        //Sum of all premium amount for display purpose
        public void LoadTransferedAmount()
        {
            if (iclbPersonAccountInsuranceTransferDestinationContribution == null)
                LoadInsuranceTransferContribution();

            icdoPersonAccountInsuranceTransfer.idecReceivedPremiumAmount = iclbPersonAccountInsuranceTransferDestinationContribution.Sum(lobjContribtuion => lobjContribtuion.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount);
        }

        //Load All Insurance Contribution Details
        //Load Def comp Contribution in the New Mode
        //Setting all fields
        public void LoadPersonAccountContributionDetail()
        {
            if (ibusFromPersonAccount.IsNull())
                LoadFromPersonAccount();
            if (ibusFromPersonAccount.iclbInsuranceContributionAll.IsNull())
                ibusFromPersonAccount.LoadInsuranceContributionAll();
            iclbPersonAccountInsuranceTransferSourceContribution = new Collection<busPersonAccountInsuranceContribution>();
            foreach (busPersonAccountInsuranceContribution lobjInsuranceContribution in ibusFromPersonAccount.iclbInsuranceContributionAll)
            {
                if (!IsContributionExcluded(lobjInsuranceContribution))
                    iclbPersonAccountInsuranceTransferSourceContribution.Add(lobjInsuranceContribution);
            }
        }

        //load contribution in update mode
        //This methods checks if the transfer contribution exists in the orgininal contribution table
        public void LoadInsuranceTransferContribution()
        {
            if (ibusFromPersonAccount.IsNull())
                LoadFromPersonAccount();
            if (ibusFromPersonAccount.iclbInsuranceContributionAll.IsNull())
                ibusFromPersonAccount.LoadInsuranceContributionAll();
            iclbPersonAccountInsuranceTransferDestinationContribution = new Collection<busPersonAccountInsuranceTransferContribution>();

            DataTable ldtbList = Select<cdoPersonAccountInsuranceTransferContribution>(
                new string[1] { "PERSON_ACCOUNT_INSURANCE_TRANSFER_ID" }, new object[1] { icdoPersonAccountInsuranceTransfer.person_account_insurance_transfer_id }
                , null, null);

            iclbPersonAccountInsuranceTransferDestinationContribution = GetCollection<busPersonAccountInsuranceTransferContribution>(ldtbList, "icdoPersonAccountInsuranceTransferContribution");

            foreach (busPersonAccountInsuranceTransferContribution lobjInsuranceTransferContribution in iclbPersonAccountInsuranceTransferDestinationContribution)
            {
                lobjInsuranceTransferContribution.ibusPersonAccountInsuranceContribution = ibusFromPersonAccount.iclbInsuranceContributionAll
                                                                                         .Where(lobjContribution => lobjContribution.icdoPersonAccountInsuranceContribution.health_insurance_contribution_id == lobjInsuranceTransferContribution.icdoPersonAccountInsuranceTransferContribution.health_insurance_contribution_id).FirstOrDefault();
            }
        }

        #endregion

        #region Validations
        //this is to check if the To Person ID entered is also enrolled in the Same Plan As from Person Account
        public bool IsTransferMemberEnrolledInFromMemberPlan()
        {
            if (icdoPersonAccountInsuranceTransfer.iintTransferToPersonID > 0)
            {
                if (icdoPersonAccountInsuranceTransfer.to_person_account_id == 0)
                    return false;
            }
            return true;
        }

        public busPerson ibusTransferToPerson { get; set; }
        public void LoadTransferToPerson()
        {
            if (ibusTransferToPerson == null)
                ibusTransferToPerson = new busPerson();

            ibusTransferToPerson.FindPerson(icdoPersonAccountInsuranceTransfer.iintTransferToPersonID);
        }

        //if the To Transfer Person is entered then get the person account for the plan of from Person account id.
        public void GetToPersonAccountID()
        {
            if (icdoPersonAccountInsuranceTransfer.iintTransferToPersonID != 0)
            {
                if (ibusTransferToPerson == null)
                    LoadTransferToPerson();

                busPersonAccount lobjPersonAccount = ibusTransferToPerson.LoadActivePersonAccountByPlan(ibusFromPersonAccount.icdoPersonAccount.plan_id);
                icdoPersonAccountInsuranceTransfer.to_person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
            }
        }

        //check if the contribution records already transferred 
        private bool IsRecordAlreadyTransferred(int aintContributionID)
        {
            DataTable ldtbList = Select("cdoPersonAccountInsuranceTransfer.GetTransferredRecords",
                 new object[2] { aintContributionID, icdoPersonAccountInsuranceTransfer.from_person_account_id });
            if (ldtbList.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        private bool IsContributionExcluded(busPersonAccountInsuranceContribution aobjInsuranceContribution)
        {
            // 1. Check if the Contribution is already transferred
            // 2. Check if the Contribution is a Contra entry of the contribution

            if (IsRecordAlreadyTransferred(aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.health_insurance_contribution_id))
            {
                return true;
            }
            else if ((aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueTransfer) &&
                     (aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_ref_id > 0))
            {
                if (IsContraEntry(aobjInsuranceContribution))
                {
                    return true;
                }
            }
            return false;
        }


        private bool IsContraEntry(busPersonAccountInsuranceContribution aobjInsuranceContribution)
        {
            // Check if the contribution has a subsystem Ref Id and subsystem type as Transfer
            // Load the Transfer record and check if the FromPersonAccountId and the Contribution PersonAccount Id is the same. if so then this
            // Record is a contra entry record.           

            DataTable ldtblist = Select<cdoPersonAccountInsuranceTransfer>(
                                                    new string[2] { "person_account_insurance_transfer_id", "from_person_account_id" },
                                                    new object[2] { aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_ref_id, 
                                                        aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_id }, null, null);
            if (ldtblist.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        # endregion

        # region Button Logic

        public ArrayList btnPost_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusFromPersonAccount.IsNull())
                LoadFromPersonAccount();
            if (ibusToPersonAccount.IsNull())
                LoadToPersonAccount();

            // Update Status
            icdoPersonAccountInsuranceTransfer.posted_by = iobjPassInfo.istrUserID;
            icdoPersonAccountInsuranceTransfer.posted_date = DateTime.Now;
            icdoPersonAccountInsuranceTransfer.status_value = busConstant.TransferStatusPosted;

            // Contributions
            if (iclbPersonAccountInsuranceTransferDestinationContribution == null)
                LoadInsuranceTransferContribution();
            foreach (busPersonAccountInsuranceTransferContribution lbusInsuranceTransferContribution in iclbPersonAccountInsuranceTransferDestinationContribution)
            {
                //UCS-041 check if the effective date is overlapping with the person account start date and end date
                //if not raise error and exit loop
                if (!(busGlobalFunctions.CheckDateOverlapping(lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                    ibusToPersonAccount.icdoPersonAccount.start_date, ibusToPersonAccount.icdoPersonAccount.end_date)))
                {
                    lobjError = AddError(7654, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }
                else
                {
                    /* UAT PIR 476, Including other and JS RHIC Amount */
                    ibusFromPersonAccount.PostInsuranceContribution(busConstant.SubSystemValueTransfer,
                        icdoPersonAccountInsuranceTransfer.person_account_insurance_transfer_id,
                        icdoPersonAccountInsuranceTransfer.posted_date,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                       0,  //?? Can we link it to old employment, if we create new employment record as part of this procedure then we need to link that here   
                       lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_type_value,
                       -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.due_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.rhic_benefit_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.othr_rhic_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.js_rhic_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.group_health_fee_amt,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.buydown_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.medicare_part_d_amt,//PIR 14271
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_basic_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_supp_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_spouse_supp_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_dep_supp_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_member_three_yrs_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_member_five_yrs_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_spouse_three_yrs_premium_amount,
                        -1M * lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_spouse_five_yrs_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id);

                    ibusToPersonAccount.PostInsuranceContribution(busConstant.SubSystemValueTransfer,
                        icdoPersonAccountInsuranceTransfer.person_account_insurance_transfer_id,
                        icdoPersonAccountInsuranceTransfer.posted_date,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                        0, //?? Can we link it to old employment, if we create new employment record as part of this procedure then we need to link that here
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_type_value,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.due_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.rhic_benefit_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.othr_rhic_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.js_rhic_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.group_health_fee_amt,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.buydown_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.medicare_part_d_amt,//PIR 14271
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_basic_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_supp_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_spouse_supp_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_dep_supp_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_member_three_yrs_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_member_five_yrs_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_spouse_three_yrs_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_spouse_five_yrs_premium_amount,
                        lbusInsuranceTransferContribution.ibusPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id);
                    /* UAT PIR 476 ends here */

                }
            }
            if (iclbPersonAccountInsuranceTransferDestinationContribution.Count > 0)
            {
                icdoPersonAccountInsuranceTransfer.Update();
                LoadInsuranceTransferContribution();
                icdoPersonAccountInsuranceTransfer.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(353, icdoPersonAccountInsuranceTransfer.status_value);
                EvaluateInitialLoadRules();
            }
            alReturn.Add(this);
            return alReturn;
        }

        # endregion

        //GetTransferredRecords
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            GetToPersonAccountID();
            LoadToPersonAccount();
            ibusToPersonAccount.LoadPerson();
        }
        public override int PersistChanges()
        {
            if (icdoPersonAccountInsuranceTransfer.ienuObjectState.Equals(ObjectState.Insert))
                icdoPersonAccountInsuranceTransfer.Insert();
            else if (icdoPersonAccountInsuranceTransfer.ienuObjectState.Equals(ObjectState.Update))
                icdoPersonAccountInsuranceTransfer.Update();

            foreach (busPersonAccountInsuranceContribution lobjInsuranceContribution in iclbPersonAccountInsuranceTransferSourceContribution)
            {
                if (lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.istrSelected == busConstant.Flag_Yes)
                {
                    busPersonAccountInsuranceTransferContribution lobjTransferDestination = new busPersonAccountInsuranceTransferContribution { icdoPersonAccountInsuranceTransferContribution = new cdoPersonAccountInsuranceTransferContribution() };
                    lobjTransferDestination.icdoPersonAccountInsuranceTransferContribution.health_insurance_contribution_id = lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.health_insurance_contribution_id;
                    lobjTransferDestination.icdoPersonAccountInsuranceTransferContribution.person_account_insurance_transfer_id = icdoPersonAccountInsuranceTransfer.person_account_insurance_transfer_id;
                    lobjTransferDestination.icdoPersonAccountInsuranceTransferContribution.Insert();
                    lobjTransferDestination.ibusPersonAccountInsuranceContribution = lobjInsuranceContribution;
                    iclbPersonAccountInsuranceTransferDestinationContribution.Add(lobjTransferDestination);
                }
            }
            return 1;
        }
        public override void AfterPersistChanges()
        {
            LoadPersonAccountContributionDetail();
            LoadInsuranceTransferContribution();
        }
        public override int Delete()
        {
            if (ibusSoftErrors.IsNull())
                LoadErrors();
            ibusSoftErrors.DeleteErrors();

            foreach (busPersonAccountInsuranceTransferContribution lobjContribution in iclbPersonAccountInsuranceTransferDestinationContribution)
            {
                lobjContribution.Delete();
            }
            return base.Delete();
        }

        public bool IsLoggedinUserNotSameasCreatedUser()
        {
            // UAT PIR ID 1410
            if (icdoPersonAccountInsuranceTransfer.modified_by.ToLower() != iobjPassInfo.istrUserID.ToLower())
                return true;
            return false;
        }
    }
}
