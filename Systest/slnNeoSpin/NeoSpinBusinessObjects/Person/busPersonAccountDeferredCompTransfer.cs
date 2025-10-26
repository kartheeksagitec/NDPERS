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

#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPersonAccountDeferredCompTransfer:
    /// Inherited from busPersonAccountDeferredCompTransferGen, the class is used to customize the business object busPersonAccountDeferredCompTransferGen.
    /// </summary>
    [Serializable]
    public class busPersonAccountDeferredCompTransfer : busPersonAccountDeferredCompTransferGen
    {
        private bool iblnIsRecordTransferred = false;

        # region Load Methods
        //this is loading the from Person account and the Person Account Def Comp
        public void LoadFromPersonAccountDefComp()
        {
            if (ibusFromPADefComp == null)
            {
                ibusFromPADefComp = new busPersonAccountDeferredComp();
            }
            ibusFromPADefComp.FindPersonAccountDeferredComp(icdoPersonAccountDeferredCompTransfer.from_person_account_id);
        }

        //this is loading the To Person account and the Person Account Def Comp
        public void LoadToPersonAccountDefComp()
        {
            if (ibusToPADefComp == null)
            {
                ibusToPADefComp = new busPersonAccountDeferredComp();
            }
            ibusToPADefComp.FindPersonAccountDeferredComp(icdoPersonAccountDeferredCompTransfer.to_person_account_id);
        }

        //LoadContributionDetails -- this query will loads the def comp with providers details and corresponding contributions.
        public void LoadDeferredCompContribution()
        {
            iclbPersonAccountDefCompTransferSourceContribution = new Collection<busPersonAccountDeferredCompContribution>();
            DataTable ldtbList = Select("cdoPersonAccountDeferredCompContribution.LoadContributionDetails", new object[1] { icdoPersonAccountDeferredCompTransfer.from_person_account_id });

            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountDeferredCompContribution lobjPersonAccountdefCompContribution = new busPersonAccountDeferredCompContribution { icdoPersonAccountDeferredCompContribution = new cdoPersonAccountDeferredCompContribution() };
                lobjPersonAccountdefCompContribution.icdoPersonAccountDeferredCompContribution.LoadData(dr);

                if (!IsContributionExcluded(lobjPersonAccountdefCompContribution))
                {
                    //Load Employer Name of the def Comp Provider
                    if ((dr["Employer_Name"]).IsNotNull())
                        lobjPersonAccountdefCompContribution.istrEmployerName = dr["Employer_Name"].ToString();

                    //Load Def Comp Provider Name
                    if ((dr["Employer_Name"]).IsNotNull())
                        lobjPersonAccountdefCompContribution.istrProviderName = dr["Provider_Name"].ToString();

                    //Add to the collection
                    iclbPersonAccountDefCompTransferSourceContribution.Add(lobjPersonAccountdefCompContribution);
                }

            }
            iclbPersonAccountDefCompTransferSourceContribution = busGlobalFunctions.Sort<busPersonAccountDeferredCompContribution>(
                                                "icdoPersonAccountDeferredCompContribution.PayPeriodYearMonth desc",
                                                 iclbPersonAccountDefCompTransferSourceContribution);
        }

        //Load all transferred contributions in destination grid.
        public void LoadDestinationContributionDetails()
        {
            if (ibusFromPADefComp.IsNull())
                LoadFromPersonAccountDefComp();
            if (ibusFromPADefComp.iclbDefCompContributionAll.IsNull())
                ibusFromPADefComp.LoadDefCompContributionAll();
            if (iclbPersonAccountDefCompTransferDestinationContribution.IsNull())
            {
                iclbPersonAccountDefCompTransferDestinationContribution = new Collection<busPersonAccountDeferredCompTransferContribution>();

                DataTable ldtbList = Select<cdoPersonAccountDeferredCompTransferContribution>(
                   new string[1] { "PERSON_ACCOUNT_DEFERRED_COMP_TRANSFER_ID" }, new object[1] { icdoPersonAccountDeferredCompTransfer.person_account_deferred_comp_transfer_id }
                   , null, null);

                foreach (DataRow dr in ldtbList.Rows)
                {
                    busPersonAccountDeferredCompTransferContribution lobjTransferContribution = new busPersonAccountDeferredCompTransferContribution { icdoPersonAccountDeferredCompTransferContribution = new cdoPersonAccountDeferredCompTransferContribution() };
                    lobjTransferContribution.icdoPersonAccountDeferredCompTransferContribution.LoadData(dr);
                    //assigning the contribution details
                    lobjTransferContribution.ibusPersonAccountDefCompContribution = ibusFromPADefComp.iclbDefCompContributionAll.Where(lobjContribution => lobjContribution.icdoPersonAccountDeferredCompContribution.deferred_comp_contribution_id
                                                                                                == lobjTransferContribution.icdoPersonAccountDeferredCompTransferContribution.deferred_comp_contribution_id).FirstOrDefault();
                    iclbPersonAccountDefCompTransferDestinationContribution.Add(lobjTransferContribution);
                }
            }
        }

        //Load Amount For Def Comp
        //Sum of all contribution amount for display purpose
        public void LoadTransferedAmount()
        {
            if (iclbPersonAccountDefCompTransferDestinationContribution == null)
                LoadDestinationContributionDetails();

            icdoPersonAccountDeferredCompTransfer.idecPayPeriodContributionAmount = iclbPersonAccountDefCompTransferDestinationContribution
                                                                                    .Sum(lobjContribution => lobjContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_contribution_amount);
        }

        # endregion

        # region Validations
        //this is to check if the To Person ID entered is also enrolled in the Same Plan As from Person Account
        public bool IsTransferMemberEnrolledInFromMemberPlan()
        {
            if (icdoPersonAccountDeferredCompTransfer.iintTransferToPersonId > 0)
            {
                if (icdoPersonAccountDeferredCompTransfer.to_person_account_id == 0)
                    return false;
            }
            return true;
        }

        public busPerson ibusTransferToPerson { get; set; }
        public void LoadTransferToPerson()
        {
            if (ibusTransferToPerson == null)
                ibusTransferToPerson = new busPerson();

            ibusTransferToPerson.FindPerson(icdoPersonAccountDeferredCompTransfer.iintTransferToPersonId);
        }

        //if the To Transfer Person is entered then get the person account for the plan of from Person account id.
        public void GetToPersonAccountID()
        {
            if (icdoPersonAccountDeferredCompTransfer.iintTransferToPersonId != 0)
            {
                if (ibusTransferToPerson == null)
                    LoadTransferToPerson();

                busPersonAccount lobjPersonAccount = ibusTransferToPerson.LoadActivePersonAccountByPlan(ibusFromPADefComp.icdoPersonAccount.plan_id);

                icdoPersonAccountDeferredCompTransfer.to_person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
            }
        }

        //check if the contribution records already transferred 
        private bool IsRecordAlreadyTransferred(int aintContributionID)
        {
            DataTable ldtbList = Select("cdoPersonAccountDeferredCompTransfer.LoadTransferredRecords",
                new object[2] { aintContributionID, icdoPersonAccountDeferredCompTransfer.from_person_account_id });
            if (ldtbList.Rows.Count > 0)
            {
                return true;
            }

            return false;
        }
        private bool IsContributionExcluded(busPersonAccountDeferredCompContribution aobjDefCompContribution)
        {
            // 1. Check if the Contribution is already transferred
            // 2. Check if the Contribution is a Contra entry of the contribution
            if (IsRecordAlreadyTransferred(aobjDefCompContribution.icdoPersonAccountDeferredCompContribution.deferred_comp_contribution_id))
            {
                return true;
            }
            else if ((aobjDefCompContribution.icdoPersonAccountDeferredCompContribution.subsystem_value == busConstant.SubSystemValueTransfer) &&
                     (aobjDefCompContribution.icdoPersonAccountDeferredCompContribution.subsystem_ref_id > 0))
            {
                if (IsContraEntry(aobjDefCompContribution))
                {
                    return true;
                }
            }
            return false;
        }


        private bool IsContraEntry(busPersonAccountDeferredCompContribution aobjDefCompContribution)
        {
            // Check if the contribution has a subsystem Ref Id and subsystem type as Transfer
            // Load the Transfer record and check if the FromPersonAccountId and the Contribution PersonAccount Id is the same. if so then this
            // Record is a contra entry record.           

            DataTable ldtblist = Select<cdoPersonAccountDeferredCompTransfer>(
                                                    new string[2] { "PERSON_ACCOUNT_DEFERRED_COMP_TRANSFER_ID", "from_person_account_id" },
                                                    new object[2] { aobjDefCompContribution.icdoPersonAccountDeferredCompContribution.subsystem_ref_id, 
                                                        aobjDefCompContribution.icdoPersonAccountDeferredCompContribution.person_account_id }, null, null);

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

            if (ibusFromPADefComp.IsNull())
                LoadFromPersonAccountDefComp();

            if (ibusToPADefComp.IsNull())
                LoadToPersonAccountDefComp();

            // Update Status
            icdoPersonAccountDeferredCompTransfer.posted_by = iobjPassInfo.istrUserID;
            icdoPersonAccountDeferredCompTransfer.posted_date = DateTime.Now;
            icdoPersonAccountDeferredCompTransfer.status_value = busConstant.TransferStatusPosted;

            // Contributions
            if (iclbPersonAccountDefCompTransferDestinationContribution == null)
                LoadDestinationContributionDetails();
            foreach (busPersonAccountDeferredCompTransferContribution lbusDefCompTransferContribution in iclbPersonAccountDefCompTransferDestinationContribution)
            {
                //UCS-041 check if the effective date is overlapping with the person account start date and end date
                //if not raise error and exit loop
                if (!(busGlobalFunctions.CheckDateOverlapping(lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.effective_date,
                    ibusToPADefComp.icdoPersonAccount.start_date, ibusToPADefComp.icdoPersonAccount.end_date)))
                {
                    lobjError = AddError(7654, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }
                else
                {
                    ibusFromPADefComp.PostDefCompContribution(busConstant.SubSystemValueTransfer,
                        icdoPersonAccountDeferredCompTransfer.person_account_deferred_comp_transfer_id,
                        icdoPersonAccountDeferredCompTransfer.posted_date,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.effective_date,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_start_date,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_start_date,
                        0, //?? Can we link it to old employment, if we create new employment record as part of this procedure then we need to link that here   
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.paid_date,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.transaction_type_value,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.provider_org_id,
                        -1M * lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_contribution_amount);
                    ibusToPADefComp.PostDefCompContribution(busConstant.SubSystemValueTransfer,
                        icdoPersonAccountDeferredCompTransfer.person_account_deferred_comp_transfer_id,
                        icdoPersonAccountDeferredCompTransfer.posted_date,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.effective_date,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_start_date,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_start_date,
                        0, //?? Can we link it to old employment, if we create new employment record as part of this procedure then we need to link that here
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.paid_date,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.transaction_type_value,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.provider_org_id,
                        lbusDefCompTransferContribution.ibusPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_contribution_amount);
                }
            }
            if (iclbPersonAccountDefCompTransferDestinationContribution.Count > 0)
            {
                icdoPersonAccountDeferredCompTransfer.Update();
                icdoPersonAccountDeferredCompTransfer.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(353, icdoPersonAccountDeferredCompTransfer.status_value);
                EvaluateInitialLoadRules();
            }
            alReturn.Add(this);
            return alReturn;
        }
        # endregion

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            GetToPersonAccountID();
            LoadToPersonAccountDefComp();
            ibusToPADefComp.LoadPerson();
        }
        public override int PersistChanges()
        {
            if (icdoPersonAccountDeferredCompTransfer.ienuObjectState.Equals(ObjectState.Insert))
                icdoPersonAccountDeferredCompTransfer.Insert();
            else if (icdoPersonAccountDeferredCompTransfer.ienuObjectState.Equals(ObjectState.Update))
                icdoPersonAccountDeferredCompTransfer.Update();

            foreach (busPersonAccountDeferredCompContribution lobjDefCompContribution in iclbPersonAccountDefCompTransferSourceContribution)
            {
                if (lobjDefCompContribution.icdoPersonAccountDeferredCompContribution.istrIsRecordSelected == busConstant.Flag_Yes)
                {
                    busPersonAccountDeferredCompTransferContribution lobjTransferDestination = new busPersonAccountDeferredCompTransferContribution { icdoPersonAccountDeferredCompTransferContribution = new cdoPersonAccountDeferredCompTransferContribution() };
                    lobjTransferDestination.icdoPersonAccountDeferredCompTransferContribution.deferred_comp_contribution_id = lobjDefCompContribution.icdoPersonAccountDeferredCompContribution.deferred_comp_contribution_id;
                    lobjTransferDestination.icdoPersonAccountDeferredCompTransferContribution.person_account_deferred_comp_transfer_id = icdoPersonAccountDeferredCompTransfer.person_account_deferred_comp_transfer_id;
                    lobjTransferDestination.icdoPersonAccountDeferredCompTransferContribution.Insert();
                    lobjTransferDestination.ibusPersonAccountDefCompContribution = lobjDefCompContribution;
                    iclbPersonAccountDefCompTransferDestinationContribution.Add(lobjTransferDestination);
                }
            }
            return 1;
        }
        public override void AfterPersistChanges()
        {
            LoadDestinationContributionDetails();
            LoadDeferredCompContribution();
        }

        public override int Delete()
        {
            if (ibusSoftErrors.IsNull())
                LoadErrors();
            ibusSoftErrors.DeleteErrors();

            foreach (busPersonAccountDeferredCompTransferContribution lobjContribution in iclbPersonAccountDefCompTransferDestinationContribution)
            {
                lobjContribution.Delete();
            }
            return base.Delete();
        }
    }
}
