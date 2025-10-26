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
using Sagitec.DataObjects;
using Sagitec.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitRhicCombine : busBenefitRhicCombineGen
    {
        public Collection<busBenefitRhicCombineDetail> iclbRHICDonarDetails { get; set; }
        public Collection<busBenefitRhicEstimateCombineDetail> iclbEstimatedRHICDonorDetails { get; set; }
        public busPerson ibusSpouseDonor { get; set; }
        public busConstant.automatic_rhic_combine_trigger ienmAutomaticRhicCombineTrigger { get; set; }
        public bool iblnIsAutomaticCombine = false;
        public bool iblnIsFromScreenNewMode { get; set; } = false;

        //Receiver person can have more than one spouse. in such case, we must consider the spouse who is having highest RHIC.
        //Also, if the spouse of spouse is different person than and that spouse is already receiving from the other spouse, we should not include such spouse donors.
        public void LoadSpouseDonor(bool ablnIsEstimate = false, bool ablnIsAutomaticCombine = false)
        {
            ibusSpouseDonor = new busPerson { icdoPerson = new cdoPerson() };
            decimal ldecCurrentRhicAmount = 0.00M;
            if (ibusPerson == null)
                LoadPerson();

            if (ibusPerson.iclbAllSpouse == null)
                ibusPerson.LoadAllSpouse();

            foreach (busPerson lbusSpouse in ibusPerson.iclbAllSpouse)
            {
                //Loading Spouse of Spouse
                if (lbusSpouse.iclbAllSpouse == null)
                    lbusSpouse.LoadAllSpouse();

                //Raj : Donor Spouse is valid only if both side associated with spouse contact (Ignore the status of Spouse)
                if (lbusSpouse.iclbAllSpouse.Any(i => i.icdoPerson.person_id == icdoBenefitRhicCombine.person_id))
                {
                    var lenuOtherSpouses = lbusSpouse.iclbAllSpouse.Where(i => i.icdoPerson.person_id != icdoBenefitRhicCombine.person_id);
                    bool lblnIsSpouseReceivingRhicFromDifferentSpouse = false;
                    if (lbusSpouse.iclbBenefitRhicCombine == null)
                        lbusSpouse.LoadBenefitRhicCombine();

                    var lclbApprovedCombineRecords = lbusSpouse.iclbBenefitRhicCombine.Where(i => i.icdoBenefitRhicCombine.status_value == busConstant.RHICStatusValid
                                                                                            && i.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved);

                    if (lclbApprovedCombineRecords != null)
                    {
                        foreach (busBenefitRhicCombine lbusBenefitRhicCombine in lclbApprovedCombineRecords)
                        {
                            if (lbusBenefitRhicCombine.iclbBenefitRhicCombineDetail == null)
                                lbusBenefitRhicCombine.LoadBenefitRhicCombineDetails();

                            foreach (busBenefitRhicCombineDetail lbusBenRhicDetail in lbusBenefitRhicCombine.iclbBenefitRhicCombineDetail)
                            {
                                if (lbusBenRhicDetail.ibusPayeeAccount == null)
                                    lbusBenRhicDetail.LoadPayeeAccount();
                                //Receiving from Spouse of Spouse
                                if (lenuOtherSpouses.Any(i => i.icdoPerson.person_id == lbusBenRhicDetail.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id))
                                {
                                    lblnIsSpouseReceivingRhicFromDifferentSpouse = true;
                                    break;
                                }
                            }

                            if (!lblnIsSpouseReceivingRhicFromDifferentSpouse)
                            {
                                //Check the Originating Payee Account Id of Spouse Post Retirment and if matches also ignore it
                                Collection<busPayeeAccount> lclbPostRetPayeeAccount = lbusSpouse.LoadPostRetirementPayeeAccount();
                                foreach (busPayeeAccount lbusPayeeAccount in lclbPostRetPayeeAccount)
                                {
                                    if (lbusPayeeAccount.ibusApplication == null)
                                        lbusPayeeAccount.LoadApplication();

                                    if (lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount == null)
                                        lbusPayeeAccount.ibusApplication.LoadOriginatingPayeeAccount();

                                    if (lenuOtherSpouses.Any(i => i.icdoPerson.person_id == lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.icdoPayeeAccount.payee_perslink_id))
                                    {
                                        lblnIsSpouseReceivingRhicFromDifferentSpouse = true;
                                        break;
                                    }
                                }
                            }

                            if (lblnIsSpouseReceivingRhicFromDifferentSpouse) break;
                        }
                    }

                    if (lblnIsSpouseReceivingRhicFromDifferentSpouse) continue; //Skip this spouse

                    //Either Spouse of Spouse Person ID is same person id as receiver or spouse is not receiving rhic from his/her spouse
                    //So, get the Rhic and compare the Rhic with Spouse Rhic and if this goes maximum, take this spouse as donor

                    //Active Spouse
                    if (lbusSpouse.icdoPerson.date_of_death == DateTime.MinValue)
                    {
                        //For Active Spouse, Spouse Relationship should be active
                        if (ibusPerson.icolPersonContact == null)
                            ibusPerson.LoadContacts();
                        if (ibusPerson.icolPersonContact.Any(o => o.icdoPersonContact.contact_person_id == lbusSpouse.icdoPerson.person_id
                                                                 && o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                                                                 && o.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive))
                        {

                            Collection<busPayeeAccount> lclbPayeeAccount = ibusPerson.LoadActiveSpousePayeeAccount(lbusSpouse, icdoBenefitRhicCombine.start_date_no_null, ablnIsEstimate, ablnIsAutomaticCombine);
                            foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
                            {
                                if (lbusPayeeAccount.icdoPayeeAccount.rhic_amount > ldecCurrentRhicAmount)
                                {
                                    ldecCurrentRhicAmount = lbusPayeeAccount.icdoPayeeAccount.rhic_amount;
                                    ibusSpouseDonor = lbusSpouse;
                                }
                            }
                        }
                    }
                    else //Deceased Spouse
                    {
                        Collection<busPayeeAccount> lclbPayeeAccount = ibusPerson.LoadDeceasedSpousePayeeAccount(lbusSpouse);
                        foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
                        {
                            if (lbusPayeeAccount.ibusBenefitAccount == null)
                                lbusPayeeAccount.LoadBenfitAccount();
                            if (lbusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > ldecCurrentRhicAmount)
                            {
                                ldecCurrentRhicAmount = lbusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount;
                                ibusSpouseDonor = lbusSpouse;
                            }
                        }
                    }

                    //if the current spouse is spouse of member, we may also need to check post retirement death rhic. (because originating payee account id is from this spouse only)
                    if (ibusPerson.iclbAllSpouse.Any(i => i.icdoPerson.person_id == lbusSpouse.icdoPerson.person_id))
                    {
                        Collection<busPayeeAccount> lclbPayeeAccount = ibusPerson.LoadPostRetirementPayeeAccount();
                        foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
                        {
                            if (lbusPayeeAccount.ibusBenefitAccount == null)
                                lbusPayeeAccount.LoadBenfitAccount();
                            if (lbusPayeeAccount.icdoPayeeAccount.rhic_amount > ldecCurrentRhicAmount)
                            {
                                ldecCurrentRhicAmount = lbusPayeeAccount.icdoPayeeAccount.rhic_amount;
                                ibusSpouseDonor = lbusSpouse;
                            }
                        }
                    }                    
                    //PIR-10687 Start
                    //if current spouse is reciving benefits from x- spouse then we can add the spouse as donor with current RHIC benefits recieved by the spouse
                    //if(lbusSpouse.iclbPayeeAccount == null || lbusSpouse.iclbPayeeAccount.Count() == 0)
                        lbusSpouse.LoadPayeeAccount(true);
                    foreach(busPayeeAccount lbusPayeeAccount in lbusSpouse.iclbPayeeAccount)
                    {
                        lbusPayeeAccount.LoadApplication();
                        //Spouse is recieving from other/ x spouse
                        if(ibusPerson.icdoPerson.person_id != lbusPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id 
                            && lbusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == "RECV")
                        {
                             ldecCurrentRhicAmount = lbusPayeeAccount.icdoPayeeAccount.rhic_amount;
                             ibusSpouseDonor = lbusSpouse;
                        }
                    }
                    //PIR-10687 End
                  }
               }
            }
        

        /// <summary>
        /// Status = Valid
        /// Action Status = Approved
        /// Person ID is not current person id
        /// </summary>
        /// <returns></returns>
        public bool IsAlreadyCombined(busPayeeAccount abusPayeeAccount)
        {
            if (abusPayeeAccount.ibusLatestBenefitRhicCombine == null)
                abusPayeeAccount.LoadLatestBenefitRhicCombine();
            if (abusPayeeAccount.ibusLatestBenefitRhicCombine.IsNotNull() && abusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail == null)
                abusPayeeAccount.ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();
            if (abusPayeeAccount.ibusLatestBenefitRhicCombine != null)
            {
                if (abusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id != icdoBenefitRhicCombine.person_id)
                {
                    if (abusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.
                        Any(i=>i.icdoBenefitRhicCombineDetail.donar_payee_account_id == abusPayeeAccount.icdoPayeeAccount.payee_account_id && i.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes))
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Status = Valid
        /// Action Status = Approved
        /// Person ID is not current person id
        /// Apply To Health = true
        /// </summary>
        /// <returns></returns>
        public bool IsAlreadyCombinedWithApplyToHealth(busPayeeAccount abusPayeeAccount)
        {
            if (abusPayeeAccount.ibusLatestBenefitRhicCombine == null)
                abusPayeeAccount.LoadLatestBenefitRhicCombine();

            if (abusPayeeAccount.ibusLatestBenefitRhicCombine != null)
            {
                if ((abusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id != icdoBenefitRhicCombine.person_id) &&
                    (abusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.apply_to_value == busConstant.Flag_Yes))
                {
                    return true;
                }
            }
            return false;
        }

        public string IsAlreadyCombinedWithCombineFlag(busPayeeAccount abusPayeeAccount)
        {
            if (abusPayeeAccount.ibusLatestBenefitRhicCombine == null)
                abusPayeeAccount.LoadLatestBenefitRhicCombine();

            if (abusPayeeAccount.ibusLatestBenefitRhicCombine != null)
            {
                if (abusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.IsNull())
                    abusPayeeAccount.ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();

                // PROD PIR ID 7296 -- Confirmed by Maik dated Wed 6/29/2011
                // Now the automatic creation of RHIC in all scenarios will do the following,
                //      •	No change in loading the RHIC Donor details
                //      •	Only in the case of Post-retirement account, we will check for the Previous/Latest RHIC combine record. 
                //          If the Previous/Latest RHIC record is combined then only the newly created RHIC record will combine else it will not.

                if (abusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.IsNotNull() &&
                    abusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Where(lobj =>
                                    // PIR 9797 to set the flag properly no need to compare payee_account_id 
                                    //lobj.icdoBenefitRhicCombineDetail.donar_payee_account_id == abusPayeeAccount.icdoPayeeAccount.payee_account_id &&   
                                    lobj.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes).Any())
                    return busConstant.Flag_Yes;
            }
            return busConstant.Flag_No;
        }

        private void AddToRhicDonorCollection(Collection<busPayeeAccount> aclbPayeeAccount,
            busConstant.donor_payee_account_type aenmDonorPayeeAccountType, bool ablnTakeSpouseAmount, bool ablnIsAutomaticCombine = false)
        {
            foreach (busPayeeAccount lbusPayeeAccount in aclbPayeeAccount)
            {
                bool lblnIsCombined = false;
                if (aenmDonorPayeeAccountType == busConstant.donor_payee_account_type.active_spouse)
                {
                    if (ablnIsAutomaticCombine)
                    {
                        lblnIsCombined = IsAlreadyCombinedWithApplyToHealth(lbusPayeeAccount);
                    }
                    else
                    {
                        lblnIsCombined = IsAlreadyCombined(lbusPayeeAccount);
                    }
                }
                else
                {
                    lblnIsCombined = IsAlreadyCombined(lbusPayeeAccount);
                }

                if (!lblnIsCombined)
                {
                    if (!((ablnTakeSpouseAmount) && (lbusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value == busConstant.RHICOptionStandard) && 
                        (lbusPayeeAccount.icdoPayeeAccount.term_certain_end_date < icdoBenefitRhicCombine.start_date))) // PIR 9433
                    {
                        //PIR 25092
                        if ((!iblnIsFromScreenNewMode && lbusPayeeAccount.icdoPayeeAccount.payee_perslink_id == icdoBenefitRhicCombine.person_id) || iblnIsFromScreenNewMode)
                        {
                            busBenefitRhicCombineDetail lbusBenRhicCombineDetail = CreateRHICCombineDetail(lbusPayeeAccount, ablnTakeSpouseAmount);
                            lbusBenRhicCombineDetail.ienmDonorPayeeAccountType = aenmDonorPayeeAccountType;
                            iclbRHICDonarDetails.Add(lbusBenRhicCombineDetail);
                        }
                        if(!iblnIsFromScreenNewMode && aenmDonorPayeeAccountType == busConstant.donor_payee_account_type.active_spouse)
                        {
                            iblnInitiateWorkFlow = true;
                        }
                    }

                }
            }
        }

        public void LoadRHICDonorDetails(bool ablnIsAutomaticCombine = false, bool ablnReloadFromPayeeAccount = true)
        {
            iclbRHICDonarDetails = new Collection<busBenefitRhicCombineDetail>();

            if (ablnReloadFromPayeeAccount)
            {
                if (ibusPerson == null)
                    LoadPerson();

                //Always Reload Spouse Donor (Estimate Also uses the same object but different logic)
                LoadSpouseDonor(ablnIsAutomaticCombine: ablnIsAutomaticCombine);

                Collection<busPayeeAccount> lclbPayeeAccount = ibusPerson.LoadActiveDBRetirementDisablityPayeeAccount();
                AddToRhicDonorCollection(lclbPayeeAccount, busConstant.donor_payee_account_type.member_db, false);

                lclbPayeeAccount = ibusPerson.LoadActiveDCRetirementDisablityPayeeAccount();
                AddToRhicDonorCollection(lclbPayeeAccount, busConstant.donor_payee_account_type.member_dc, false);

                lclbPayeeAccount = ibusPerson.LoadActiveSpousePayeeAccount(ibusSpouseDonor, icdoBenefitRhicCombine.start_date_no_null, ablnIsAutomaticCombine: ablnIsAutomaticCombine);
                AddToRhicDonorCollection(lclbPayeeAccount, busConstant.donor_payee_account_type.active_spouse, false, ablnIsAutomaticCombine);

                lclbPayeeAccount = ibusPerson.LoadDeceasedSpousePayeeAccount(ibusSpouseDonor);
                AddToRhicDonorCollection(lclbPayeeAccount, busConstant.donor_payee_account_type.deceased_spouse, true);

                lclbPayeeAccount = ibusPerson.LoadPreRetirementPayeeAccount();
                AddToRhicDonorCollection(lclbPayeeAccount, busConstant.donor_payee_account_type.pre_retirement, false);

                lclbPayeeAccount = ibusPerson.LoadPostRetirementPayeeAccount();
                AddToRhicDonorCollection(lclbPayeeAccount, busConstant.donor_payee_account_type.post_retirement, false);
                 //PIR-10687 Start
                lclbPayeeAccount = ibusSpouseDonor.LoadReceivingSpousePayeeAccount(ibusSpouseDonor, icdoBenefitRhicCombine.start_date_no_null, ablnIsAutomaticCombine: ablnIsAutomaticCombine);
                AddToRhicDonorCollection(lclbPayeeAccount, busConstant.donor_payee_account_type.active_spouse, false);
                //PIR-10687 End
            }
            else
            {
                if (iclbBenefitRhicCombineDetail == null)
                    LoadBenefitRhicCombineDetails();
                foreach (busBenefitRhicCombineDetail lbusBenRhicComDetail in iclbBenefitRhicCombineDetail)
                {
                    if (lbusBenRhicComDetail.ibusPayeeAccount == null)
                        lbusBenRhicComDetail.LoadPayeeAccount();
                    if (lbusBenRhicComDetail.ibusPayeeAccount.ibusPayee == null)
                        lbusBenRhicComDetail.ibusPayeeAccount.LoadPayee();
                    lbusBenRhicComDetail.icdoBenefitRhicCombineDetail.ienuObjectState = ObjectState.Update;
                    iarrChangeLog.Add(lbusBenRhicComDetail.icdoBenefitRhicCombineDetail);
                    iclbRHICDonarDetails.Add(lbusBenRhicComDetail);
                }
            }
        }

        private busBenefitRhicCombineDetail CreateRHICCombineDetail(busPayeeAccount lbusPayeeAccount, bool lblnTakeSpouseRHICAmount)
        {
            if (iclbBenefitRhicCombineDetail == null)
                LoadBenefitRhicCombineDetails();
            if (lbusPayeeAccount.ibusBenefitAccount == null)
                lbusPayeeAccount.LoadBenfitAccount();

            busBenefitRhicCombineDetail lbusBenRhicCombineDetail = new busBenefitRhicCombineDetail { icdoBenefitRhicCombineDetail = new cdoBenefitRhicCombineDetail() };

            busBenefitRhicCombineDetail lbusBenRhicComDetil = iclbBenefitRhicCombineDetail.FirstOrDefault(i => i.icdoBenefitRhicCombineDetail.donar_payee_account_id == lbusPayeeAccount.icdoPayeeAccount.payee_account_id);
            if (lbusBenRhicComDetil != null)
            {
                lbusBenRhicCombineDetail = lbusBenRhicComDetil;
                lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.ienuObjectState = ObjectState.Update;
            }
            else
            {
                lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.ienuObjectState = ObjectState.Insert;
            }

            if (lblnTakeSpouseRHICAmount)
                lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.rhic_amount = lbusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount;
            else
                lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.rhic_amount = lbusPayeeAccount.icdoPayeeAccount.rhic_amount;

            lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.benefit_rhic_combine_id = icdoBenefitRhicCombine.benefit_rhic_combine_id;
            lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.donar_payee_account_id = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
            //Load this Object here to show person name in Grid
            lbusBenRhicCombineDetail.ibusPayeeAccount = lbusPayeeAccount;
            if (lbusBenRhicCombineDetail.ibusPayeeAccount.ibusPayee == null)
                lbusBenRhicCombineDetail.ibusPayeeAccount.LoadPayee();
            iarrChangeLog.Add(lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail);
            return lbusBenRhicCombineDetail;
        }

        private decimal GetRHICFactor(DataTable adtbBenProvisionData, busPersonAccount abusPersonAccount)
        {
            if (adtbBenProvisionData == null)
                adtbBenProvisionData = iobjPassInfo.isrvDBCache.GetCacheData("SGT_BENEFIT_PROVISION_BENEFIT_TYPE", null);

			//PIR 14646 - Benefit Tier Changes
            string lstrBenefitTierValue = string.Empty;

            busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
            lbusPersonAccountRetirement.FindPersonAccountRetirement(abusPersonAccount.icdoPersonAccount.person_account_id);

            if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain)
            {
                lstrBenefitTierValue = string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) ? busConstant.MainBenefit1997Tier :
                    lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
            }
            //PIR 26282
            else if (abusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf)
            {                
                lstrBenefitTierValue = string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) ? busConstant.BCIBenefit2011Tier :
                    lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
            }
            //PIR 26544
            else if(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
            {
                lstrBenefitTierValue = lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
            }
            decimal ldecRHICFactor = 0.00M;
            var lenumBenefitProvision = adtbBenProvisionData.AsEnumerable().Where(dr => dr.Field<int>("benefit_provision_id") == abusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id
                                                                                && (dr.Field<DateTime>("effective_date") <= (icdoBenefitRhicCombine.start_date == DateTime.MinValue ? DateTime.MaxValue : icdoBenefitRhicCombine.start_date))
                                                                                && dr.Field<string>("benefit_account_type_value") == busConstant.PlanBenefitTypeRetirement
                                                                                && dr.Field<string>("BENEFIT_TIER_VALUE") == (string.IsNullOrEmpty(lstrBenefitTierValue) ? null : lstrBenefitTierValue))
                                                                                .OrderByDescending(dr => dr.Field<DateTime>("effective_date"));
            if (lenumBenefitProvision.Count() > 0)
            {
                ldecRHICFactor = Math.Round(lenumBenefitProvision.First().Field<decimal>("RHIC_SERVICE_FACTOR"), 2, MidpointRounding.AwayFromZero);
            }
            return ldecRHICFactor;
        }

        private busBenefitRhicEstimateCombineDetail CreateEstimateRHICCombineDetail(busPersonAccount abusPersonAccount, DataTable adtbBenProvisionData)
        {
            decimal ldecRHICFactor = 0.00M;
            if (abusPersonAccount.ibusPlan == null)
                abusPersonAccount.LoadPlan();
            if (abusPersonAccount.ibusPerson == null)
                abusPersonAccount.LoadPerson();

            abusPersonAccount.LoadTotalPSC(icdoBenefitRhicCombine.start_date);

            ldecRHICFactor = GetRHICFactor(adtbBenProvisionData, abusPersonAccount);

            busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = new busBenefitRhicEstimateCombineDetail { icdoBenefitRhicEstimateCombineDetail = new cdoBenefitRhicEstimateCombineDetail() };
            lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.ienuObjectState = ObjectState.Insert;
            lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.rhic_amount = (abusPersonAccount.icdoPersonAccount.Total_PSC / 12) * ldecRHICFactor;

            lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.benefit_rhic_combine_id = icdoBenefitRhicCombine.benefit_rhic_combine_id;
            lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.donar_person_account_id = abusPersonAccount.icdoPersonAccount.person_account_id;
            lobjRHICCombineEstimateDetail.ibusPersonAccount = abusPersonAccount;

            iarrChangeLog.Add(lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail);
            lobjRHICCombineEstimateDetail.ibusPerson = abusPersonAccount.ibusPerson;
            lobjRHICCombineEstimateDetail.ibusPlan = abusPersonAccount.ibusPlan;
            return lobjRHICCombineEstimateDetail;
        }

        private busBenefitRhicEstimateCombineDetail CreateEstimateRHICCombineDetail(busPayeeAccount abusPayeeAccount, bool lblnTakeSpouseRHICAmount)
        {
            if (abusPayeeAccount.ibusPayee == null)
                abusPayeeAccount.LoadPayee();

            if (abusPayeeAccount.ibusPlan == null)
                abusPayeeAccount.LoadPlan();

            if (abusPayeeAccount.ibusBenefitAccount == null)
                abusPayeeAccount.LoadBenfitAccount();

            busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = new busBenefitRhicEstimateCombineDetail { icdoBenefitRhicEstimateCombineDetail = new cdoBenefitRhicEstimateCombineDetail() };
            lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.ienuObjectState = ObjectState.Insert;
            if (lblnTakeSpouseRHICAmount)
                lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.rhic_amount = abusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount;
            else
                lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.rhic_amount = abusPayeeAccount.icdoPayeeAccount.rhic_amount;

            lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.benefit_rhic_combine_id = icdoBenefitRhicCombine.benefit_rhic_combine_id;
            lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail.donar_payee_account_id = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
            lobjRHICCombineEstimateDetail.ibusPerson = abusPayeeAccount.ibusPayee;
            lobjRHICCombineEstimateDetail.ibusPlan = abusPayeeAccount.ibusPlan;

            iarrChangeLog.Add(lobjRHICCombineEstimateDetail.icdoBenefitRhicEstimateCombineDetail);
            return lobjRHICCombineEstimateDetail;
        }

        //Systest PIR 2574 - For estimate, use the payee account data itself if exists otherwise go with regular flow
        public void LoadEstimatedRHICDonorDetails(bool ablnReloadFromAccount = true)
        {
            iclbEstimatedRHICDonorDetails = new Collection<busBenefitRhicEstimateCombineDetail>();

            if (ablnReloadFromAccount)
            {
                if (ibusPerson == null)
                    LoadPerson();

                if (ibusPerson.icolPersonAccount == null)
                    ibusPerson.LoadPersonAccount(true);

                Collection<busPayeeAccount> lclbPayeeAccount = null;
                //load benefit type benefit provision
                DataTable ldtbBenefitProvisionBenefitType = iobjPassInfo.isrvDBCache.GetCacheData("SGT_BENEFIT_PROVISION_BENEFIT_TYPE", null);
                var lenumPersonAccount = ibusPerson.icolPersonAccount
                    .Where(lobjPA => lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn
                    && lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusTransferDC
                    && lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirmentCancelled
                    && ((lobjPA.ibusPlan.IsDBRetirementPlan()) || (lobjPA.ibusPlan.IsDCRetirementPlan()) || (lobjPA.ibusPlan.IsHBRetirementPlan())));

                lclbPayeeAccount = ibusPerson.LoadActiveDBRetirementDisablityPayeeAccount();
                if (lclbPayeeAccount.Count > 0)
                {
                    foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
                    {
                        busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = CreateEstimateRHICCombineDetail(lbusPayeeAccount, false);
                        iclbEstimatedRHICDonorDetails.Add(lobjRHICCombineEstimateDetail);
                    }
                }
                else
                {
                    //Filer the DB Person Account
                    var lenuDBList = lenumPersonAccount.Where(i => i.ibusPlan.IsDBRetirementPlan());
                    foreach (busPersonAccount lobjPersonAccount in lenuDBList)
                    {
                        busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = CreateEstimateRHICCombineDetail(lobjPersonAccount, ldtbBenefitProvisionBenefitType);
                        iclbEstimatedRHICDonorDetails.Add(lobjRHICCombineEstimateDetail);
                    }
                }

                lclbPayeeAccount = ibusPerson.LoadActiveDCRetirementDisablityPayeeAccount();
                if (lclbPayeeAccount.Count > 0)
                {
                    foreach (busPayeeAccount lbusPayeeAccount in lclbPayeeAccount)
                    {
                        busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = CreateEstimateRHICCombineDetail(lbusPayeeAccount, false);
                        iclbEstimatedRHICDonorDetails.Add(lobjRHICCombineEstimateDetail);
                    }
                }
                else
                {
                    //Filer the DC Person Account
                    var lenuDCList = lenumPersonAccount.Where(i => i.ibusPlan.IsDCRetirementPlan() || i.ibusPlan.IsHBRetirementPlan());
                    foreach (busPersonAccount lobjPersonAccount in lenuDCList)
                    {
                        busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = CreateEstimateRHICCombineDetail(lobjPersonAccount, ldtbBenefitProvisionBenefitType);
                        iclbEstimatedRHICDonorDetails.Add(lobjRHICCombineEstimateDetail);
                    }
                }

                //Always Reload this because main grid also uses the same
                LoadSpouseDonor(true);

                if (ibusSpouseDonor.icdoPerson.person_id > 0)
                {
                    //Active Spouse Retirement Accounts
                    if (ibusSpouseDonor.icdoPerson.date_of_death == DateTime.MinValue)
                    {
                        Collection<busPayeeAccount> lclbActiveSpousePayeeAccount = ibusSpouseDonor.LoadActiveSpousePayeeAccount(ibusSpouseDonor, icdoBenefitRhicCombine.start_date_no_null, true);
                        //PIR-10687 Start 
                        Collection<busPayeeAccount> lclbSPouseReceivingPreRetPayeeAccount = ibusSpouseDonor.LoadReceivingSpousePayeeAccount(ibusSpouseDonor, icdoBenefitRhicCombine.start_date_no_null, true);
                        var lclbActiveandSpouseReceivePayeeAccount = (lclbActiveSpousePayeeAccount != null && lclbSPouseReceivingPreRetPayeeAccount != null)?lclbActiveSpousePayeeAccount.Union<busPayeeAccount>(lclbSPouseReceivingPreRetPayeeAccount):null;
                        if (lclbActiveandSpouseReceivePayeeAccount != null && lclbActiveandSpouseReceivePayeeAccount.Count() > 0)
                        {
                            foreach (busPayeeAccount lbusPayeeAccount in lclbActiveandSpouseReceivePayeeAccount)
                            {
                                busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = CreateEstimateRHICCombineDetail(lbusPayeeAccount, false);
                                iclbEstimatedRHICDonorDetails.Add(lobjRHICCombineEstimateDetail);
                            }
                        } //PIR-10687 End 
                        else
                        {
                            if (ibusSpouseDonor.icolPersonAccount == null)
                                ibusSpouseDonor.LoadPersonAccount();

                            lenumPersonAccount = ibusSpouseDonor.icolPersonAccount
                                .Where(lobjPA => lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn
                                && lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusTransferDC
                                && lobjPA.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirmentCancelled
                                && ((lobjPA.ibusPlan.IsDBRetirementPlan()) || (lobjPA.ibusPlan.IsDCRetirementPlan()) || (lobjPA.ibusPlan.IsHBRetirementPlan())));

                            foreach (busPersonAccount lobjPersonAccount in lenumPersonAccount)
                            {
                                busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = CreateEstimateRHICCombineDetail(lobjPersonAccount, ldtbBenefitProvisionBenefitType);
                                iclbEstimatedRHICDonorDetails.Add(lobjRHICCombineEstimateDetail);
                            }
                        }
                    }
                    else
                    {
                        //Deceased Spoue Payee Accounts
                        lclbPayeeAccount = ibusPerson.LoadDeceasedSpousePayeeAccount(ibusSpouseDonor);
                        foreach (busPayeeAccount lobjPayeeAccount in lclbPayeeAccount)
                        {
                            busBenefitRhicEstimateCombineDetail lobjRHICCombineEstimateDetail = CreateEstimateRHICCombineDetail(lobjPayeeAccount, true);
                            iclbEstimatedRHICDonorDetails.Add(lobjRHICCombineEstimateDetail);
                        }
                    }
                }
            }
            else
            {
                if (iclbBenefitRhicEstimateCombineDetail == null)
                    LoadBenefitRhicEstimateCombineDetails();
                //Add the Items are which are Rhic table but not added to collection yet because of cancellation/completion of payee account 
                foreach (busBenefitRhicEstimateCombineDetail lbusBenRhicEstimateComDetail in iclbBenefitRhicEstimateCombineDetail)
                {
                    if (lbusBenRhicEstimateComDetail.icdoBenefitRhicEstimateCombineDetail.donar_payee_account_id > 0)
                    {
                        if (lbusBenRhicEstimateComDetail.ibusPayeeAccount == null)
                            lbusBenRhicEstimateComDetail.LoadPayeeAccount();
                        if (lbusBenRhicEstimateComDetail.ibusPayeeAccount.ibusPayee == null)
                            lbusBenRhicEstimateComDetail.ibusPayeeAccount.LoadPayee();
                        if (lbusBenRhicEstimateComDetail.ibusPayeeAccount.ibusPlan == null)
                            lbusBenRhicEstimateComDetail.ibusPayeeAccount.LoadPlan();
                        lbusBenRhicEstimateComDetail.ibusPerson = lbusBenRhicEstimateComDetail.ibusPayeeAccount.ibusPayee;
                        lbusBenRhicEstimateComDetail.ibusPlan = lbusBenRhicEstimateComDetail.ibusPayeeAccount.ibusPlan;
                    }
                    else if (lbusBenRhicEstimateComDetail.icdoBenefitRhicEstimateCombineDetail.donar_person_account_id > 0)
                    {
                        if (lbusBenRhicEstimateComDetail.ibusPersonAccount == null)
                            lbusBenRhicEstimateComDetail.LoadPersonAccount();
                        if (lbusBenRhicEstimateComDetail.ibusPersonAccount.ibusPlan == null)
                            lbusBenRhicEstimateComDetail.ibusPersonAccount.LoadPlan();
                        if (lbusBenRhicEstimateComDetail.ibusPersonAccount.ibusPerson == null)
                            lbusBenRhicEstimateComDetail.ibusPersonAccount.LoadPerson();
                        lbusBenRhicEstimateComDetail.ibusPerson = lbusBenRhicEstimateComDetail.ibusPersonAccount.ibusPerson;
                        lbusBenRhicEstimateComDetail.ibusPlan = lbusBenRhicEstimateComDetail.ibusPersonAccount.ibusPlan;
                    }
                    lbusBenRhicEstimateComDetail.icdoBenefitRhicEstimateCombineDetail.ienuObjectState = ObjectState.Update;
                    iarrChangeLog.Add(lbusBenRhicEstimateComDetail.icdoBenefitRhicEstimateCombineDetail);
                    iclbEstimatedRHICDonorDetails.Add(lbusBenRhicEstimateComDetail);
                }
            }
        }

        //This works for Health And medicare only
        private void CalculateReceiverPersonAccountPremium(busPersonAccountGhdv abusPersonAccountGhdv)
        {
            if (abusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
            {
                if (abusPersonAccountGhdv.ibusPerson == null)
                    abusPersonAccountGhdv.LoadPerson();

                if (abusPersonAccountGhdv.ibusPlan == null)
                    abusPersonAccountGhdv.LoadPlan();

                busPersonAccountGhdvHistory lobjPAGhdvHistory = abusPersonAccountGhdv.LoadHistoryByDate(icdoBenefitRhicCombine.start_date);
                if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                {
                    abusPersonAccountGhdv = lobjPAGhdvHistory.LoadGHDVObject(abusPersonAccountGhdv);
                    //Initialize the Org Object to Avoid the NULL error
                    abusPersonAccountGhdv.InitializeObjects();
                    abusPersonAccountGhdv.idtPlanEffectiveDate = icdoBenefitRhicCombine.start_date;

                    //prod pir 5555 : need to load org plan, employment details
                    //--start--//
                    //For Dependent COBRA, we need to load Member Employment
                    if (abusPersonAccountGhdv.icdoPersonAccount.from_person_account_id > 0)
                    {
                        //Load Member GHDV Object
                        abusPersonAccountGhdv.ibusMemberGHDVForDependent = new busPersonAccountGhdv();
                        abusPersonAccountGhdv.ibusMemberGHDVForDependent.FindGHDVByPersonAccountID(abusPersonAccountGhdv.icdoPersonAccount.from_person_account_id);
                        abusPersonAccountGhdv.ibusMemberGHDVForDependent.FindPersonAccount(abusPersonAccountGhdv.icdoPersonAccount.from_person_account_id);
                        abusPersonAccountGhdv.iblnIsDependentCobra = true;
                    }
                    //--end--//

                    abusPersonAccountGhdv.LoadActiveProviderOrgPlan(icdoBenefitRhicCombine.start_date);

                    //prod pir 5555 : need to load org plan, employment details
                    //--start--//
                    //we need org plan object for determining health participation date for IBS COBRA Members
                    if (abusPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                    {
                        //For Dependent COBRA, we need to load Member Employment
                        if (abusPersonAccountGhdv.icdoPersonAccount.from_person_account_id > 0)
                        {
                            abusPersonAccountGhdv.LoadEmploymentDetailByDate(abusPersonAccountGhdv.idtPlanEffectiveDate, abusPersonAccountGhdv.ibusMemberGHDVForDependent, true, true);
                        }
                        else
                        {
                            abusPersonAccountGhdv.LoadEmploymentDetailByDate(abusPersonAccountGhdv.idtPlanEffectiveDate, true);
                        }
                        if (abusPersonAccountGhdv.icdoPersonAccount.person_employment_dtl_id > 0)
                        {
                            abusPersonAccountGhdv.LoadPersonEmploymentDetail();
                            abusPersonAccountGhdv.ibusPersonEmploymentDetail.LoadPersonEmployment();
                            abusPersonAccountGhdv.LoadOrgPlan(abusPersonAccountGhdv.idtPlanEffectiveDate);
                        }
                    }

                    if (abusPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                    {
                        abusPersonAccountGhdv.LoadRateStructureForUserStructureCode();
                    }
                    else
                    {
                    abusPersonAccountGhdv.LoadHealthParticipationDate();
                    //To Get the Rate Structure Code (Derived Field)
                    abusPersonAccountGhdv.LoadRateStructure(icdoBenefitRhicCombine.start_date);
                    }
                    //--end--//

                    //Get the Coverage Ref ID
                    abusPersonAccountGhdv.LoadCoverageRefID();

                    //Get the Premium Amount
                    abusPersonAccountGhdv.GetMonthlyPremiumAmountByRefID(icdoBenefitRhicCombine.start_date_no_null);
                }
            }
        }

        public ArrayList btnApprove_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();

            if (IsEstimateRHICCombineFlagChecked())
            {
                lobjError = AddError(7110, "");
                larrList.Add(lobjError);
                return larrList;
            }

            if (!IsRHICCombineFlagChecked())
            {
                lobjError = AddError(7120, "");
                larrList.Add(lobjError);
                return larrList;
            }
            //Systest PIR 2588
            if (icdoBenefitRhicCombine.apply_to_value == busConstant.Flag_Yes)
            {
                if (ibusPerson == null)
                    LoadPerson();
                if (ibusPerson.ibusReceiverHealthPersonAccount == null)
                    ibusPerson.LoadReceiverHealthPersonAccount(icdoBenefitRhicCombine.start_date_no_null);

                if (ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id == 0)
                {
                    lobjError = AddError(7116, "");
                    larrList.Add(lobjError);
                    return larrList;
                }
            }

            ApproveRHICCombineAndAllocateToSplitTable();
            CreatePayrollAdjustment();
            CreatePAPITAdjustment();
            larrList.Add(this);
            EvaluateInitialLoadRules();
            return larrList;
        }

        public ArrayList btnDeny_Click()
        {
            ArrayList larrList = new ArrayList();

            icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusDeny;
            icdoBenefitRhicCombine.Update();
            larrList.Add(this);
            EvaluateInitialLoadRules();
            return larrList;
        }

        public ArrayList btnCancel_Click()
        {
            ArrayList larrList = new ArrayList();

            icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusCancelled;
            icdoBenefitRhicCombine.end_date = icdoBenefitRhicCombine.start_date; //Maik Mail dated June 17, 2015, ‘Cancel’ button is clicked we need to update the END_DATE = START_DATE 
            icdoBenefitRhicCombine.Update();
            larrList.Add(this);
            EvaluateInitialLoadRules();
            return larrList;
        }

        public ArrayList btnEnd_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();

            if (icdoBenefitRhicCombine.end_date == DateTime.MinValue)
            {
                lobjError = AddError(7106, "");
                larrList.Add(lobjError);
            }

            //BR-055-151 If the effective end date is greater than the terminated payee account benefit end date, throw an error
            foreach (busBenefitRhicCombineDetail lbusDetail in iclbRHICDonarDetails)
            {
                if (lbusDetail.ibusPayeeAccount == null)
                    lbusDetail.LoadPayeeAccount();

                if (lbusDetail.ibusPayeeAccount.icdoPayeeAccount.benefit_end_date != DateTime.MinValue)
                {
                    if (icdoBenefitRhicCombine.end_date > lbusDetail.ibusPayeeAccount.icdoPayeeAccount.benefit_end_date)
                    {
                        lobjError = AddError(7118, "");
                        larrList.Add(lobjError);
                        break;
                    }
                }
            }

            //A record can only be ended if Effective RHIC End Date indicates at least a 
            //full month from Effective RHIC Start Date and Applied RHIC Benefit Amount plus Applied Job Service RHIC Benefit Amount is greater than 0.
            //Otherwise cancel the Record with no end date

            //This code is commented now because now we need to allow the user to end the record even if it is not applying.
            //In active spouse, when rhic is not being used for self receiver (suspended health account), we may need to consider that for spouse receiver.
            //such cases, user will manually end this record before applying to spouse
            //||(icdoBenefitRhicCombine.total_js_rhic_amount + icdoBenefitRhicCombine.total_other_rhic_amount + icdoBenefitRhicCombine.total_reimbursement_amount <= 0)
            if (icdoBenefitRhicCombine.start_date.AddMonths(1).AddDays(-1) > icdoBenefitRhicCombine.end_date)
            {
                lobjError = AddError(7111, "");
                larrList.Add(lobjError);
            }

            if (larrList.Count > 0)
                return larrList;

            EndRHICCombine(icdoBenefitRhicCombine.end_date);
            CreatePayrollAdjustment(icdoBenefitRhicCombine.end_date.GetFirstDayofNextMonth());
            CreatePAPITAdjustment();

            EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }

        #region Validations
        private bool IsEstimateRHICCombineFlagChecked()
        {
            if (iclbEstimatedRHICDonorDetails == null)
                LoadEstimatedRHICDonorDetails();

            if (iclbEstimatedRHICDonorDetails.Where(i => i.icdoBenefitRhicEstimateCombineDetail.combine_flag == busConstant.Flag_Yes).Count() > 0)
                return true;
            return false;
        }
        public bool IsCombineFlagChecked()
        {
            if (iclbRHICDonarDetails == null)
                LoadRHICDonorDetails();

            if (iclbEstimatedRHICDonorDetails == null)
                LoadEstimatedRHICDonorDetails();

            foreach (busBenefitRhicCombineDetail lobjRHICDonorDetail in iclbRHICDonarDetails)
            {
                if (lobjRHICDonorDetail.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                {
                    return true;
                }
            }
            foreach (busBenefitRhicEstimateCombineDetail lobjRHICEstimatedDonorDetail in iclbEstimatedRHICDonorDetails)
            {
                if (lobjRHICEstimatedDonorDetail.icdoBenefitRhicEstimateCombineDetail.combine_flag == busConstant.Flag_Yes)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsRHICCombineFlagChecked()
        {
            if (iclbRHICDonarDetails == null)
                LoadRHICDonorDetails();

            if (iclbRHICDonarDetails.Where(i => i.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes).Count() > 0)
                return true;
            return false;
        }

        public bool IsOverlappingValidApprovedRecordForSameReceiver()
        {
            if (ibusPerson == null)
                LoadPerson();

            if (ibusPerson.iclbBenefitRhicCombine == null)
                ibusPerson.LoadBenefitRhicCombine();

            return ibusPerson.iclbBenefitRhicCombine.Any(i => i.icdoBenefitRhicCombine.status_value == busConstant.RHICStatusValid
                                                            && i.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved
                                                            && i.icdoBenefitRhicCombine.benefit_rhic_combine_id != icdoBenefitRhicCombine.benefit_rhic_combine_id
                                                            && busGlobalFunctions.CheckDateOverlapping(icdoBenefitRhicCombine.start_date, i.icdoBenefitRhicCombine.start_date,
                                                            i.icdoBenefitRhicCombine.end_date));
        }

        public bool IsOverlappingValidApprovedRecordForSameDonor()
        {
            foreach (busBenefitRhicCombineDetail lbusBenRhicCombineDetail in iclbRHICDonarDetails)
            {
                if (lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                {
                    if (lbusBenRhicCombineDetail.ibusPayeeAccount == null)
                        lbusBenRhicCombineDetail.LoadPayeeAccount();
                    if (IsAlreadyCombined(lbusBenRhicCombineDetail.ibusPayeeAccount))
                        return true;
                }
            }
            return false;
        }

        public bool IsSpouseDonorSelected()
        {
            if (iclbRHICDonarDetails == null)
                LoadRHICDonorDetails();
            foreach (busBenefitRhicCombineDetail lobjRHICDonorDetail in iclbRHICDonarDetails)
            {
                if (lobjRHICDonorDetail.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                {
                    if (lobjRHICDonorDetail.ibusPayeeAccount == null)
                        lobjRHICDonorDetail.LoadPayeeAccount();

                    if (lobjRHICDonorDetail.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != icdoBenefitRhicCombine.person_id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private busPerson GetSelectedSpouse()
        {
            if (iclbRHICDonarDetails == null)
                LoadRHICDonorDetails();
            foreach (busBenefitRhicCombineDetail lobjRHICDonorDetail in iclbRHICDonarDetails)
            {
                if (lobjRHICDonorDetail.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                {
                    if (lobjRHICDonorDetail.ibusPayeeAccount == null)
                        lobjRHICDonorDetail.LoadPayeeAccount();

                    if (lobjRHICDonorDetail.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != icdoBenefitRhicCombine.person_id)
                    {
                        if (lobjRHICDonorDetail.ibusPayeeAccount.ibusPayee == null)
                            lobjRHICDonorDetail.ibusPayeeAccount.LoadPayee();
                        return lobjRHICDonorDetail.ibusPayeeAccount.ibusPayee;
                    }
                }
            }
            return null;
        }

        public bool IsSpouseRelationAssociatedWithBothReceiverAndDonor()
        {
            bool lblnResult = false;
            if (ibusPerson == null)
                LoadPerson();

            if (ibusPerson.iclbAllSpouse == null)
                ibusPerson.LoadAllSpouse();

            busPerson lbusSelectedSpouse = GetSelectedSpouse();

            if (lbusSelectedSpouse != null)
            {
                if (ibusPerson.iclbAllSpouse.Any(i => i.icdoPerson.person_id == lbusSelectedSpouse.icdoPerson.person_id))
                {
                    lblnResult = true;
                }
            }
            return lblnResult;
        }
        #endregion

        public void InitiateRHICCombineWorkflow()
        {
            
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Maintain_Rhic, icdoBenefitRhicCombine.person_id, 0, 0, iobjPassInfo);
        }

        public void EndRHICCombine(DateTime adtEndDate)
        {
            //A record can only be ended if Effective RHIC End Date indicates at least a 
            //full month from Effective RHIC Start Date and Applied RHIC Benefit Amount plus Applied Job Service RHIC Benefit Amount is greater than 0.
            //Otherwise cancel the Record with no end date

            //&&(icdoBenefitRhicCombine.total_js_rhic_amount + icdoBenefitRhicCombine.total_other_rhic_amount + icdoBenefitRhicCombine.total_reimbursement_amount > 0)
            if (icdoBenefitRhicCombine.start_date.AddMonths(1).AddDays(-1) <= adtEndDate)
            {
                icdoBenefitRhicCombine.end_date = adtEndDate;
                icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusEnded;
            }
            else
            {
                icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusCancelled;
                icdoBenefitRhicCombine.end_date = icdoBenefitRhicCombine.start_date; //Maik Mail dated June 17, 2015, ‘Cancel’ button is clicked we need to update the END_DATE = START_DATE
            }
            icdoBenefitRhicCombine.status_value = busConstant.RHICStatusValid;
            icdoBenefitRhicCombine.Update();
        }


        public void CancelRHICCombine()
        {
            icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusCancelled;
            icdoBenefitRhicCombine.end_date = icdoBenefitRhicCombine.start_date; //Maik Mail dated June 17, 2015, ‘Cancel’ button is clicked we need to update the END_DATE = START_DATE
            icdoBenefitRhicCombine.status_value = busConstant.RHICStatusValid;
            icdoBenefitRhicCombine.Update();
        }

        //PIR 14346 - Commented
        //private DateTime CalculateRhicEndDate()
        //{
        //    DateTime ldtEndDate = DateTime.MinValue;
        //    switch (ienmAutomaticRhicCombineTrigger)
        //    {
        //        //case busConstant.automatic_rhic_combine_trigger.initial_payment_approval:
        //        case busConstant.automatic_rhic_combine_trigger.benefit_calculation_approval:
        //            //UAT PIR 2100 - Dual Case
        //            ldtEndDate = icdoBenefitRhicCombine.start_date.AddDays(-1);
        //            break;
        //        case busConstant.automatic_rhic_combine_trigger.benefit_adjustment_approval:
        //            //ldtEndDate = busIBSHelper.GetLastPostedIBSBatchDate().GetLastDayofMonth();
        //            ldtEndDate = icdoBenefitRhicCombine.start_date.AddDays(-1);
        //            break;
        //        case busConstant.automatic_rhic_combine_trigger.rhic_factor_change:
        //        case busConstant.automatic_rhic_combine_trigger.payment_completed_for_death_member:
        //        case busConstant.automatic_rhic_combine_trigger.spouse_rhic_for_termination_of_death_member:
        //        case busConstant.automatic_rhic_combine_trigger.enrollment_change:
        //        case busConstant.automatic_rhic_combine_trigger.online_end_click:
        //        case busConstant.automatic_rhic_combine_trigger.health_premium_change:
        //        case busConstant.automatic_rhic_combine_trigger.payment_suspended_or_completed:
        //            ldtEndDate = icdoBenefitRhicCombine.start_date.AddDays(-1);
        //            break;
        //    }
        //    return ldtEndDate;
        //}

        public bool iblnOverlapHistory = false;
        public bool iblnIsAlreadyCombined { get; set; } = false;
        //Commented this method as per PIR 14346 - have to undo the current RHIC logic
        //Before calling this method, make sure you assign the person id and start date
        //If this method returns true, RHIC combine record created else workflow initiated.
        //public bool CreateAutomaticRHICCombine()
        //{
        //    //Prod PIR 7728
        //    //Load person always             
        //    //if (ibusPerson == null)
        //        LoadPerson();
        //    if (ibusPerson.iclbBenefitRhicCombine == null)
        //        ibusPerson.LoadBenefitRhicCombine();

        //    //reload the Donor list based on the latest changes 
        //    LoadRHICDonorDetails(ablnIsAutomaticCombine: true);

        //    //BR-0550135a
        //    bool lblnInitiateWorkflow = false;
        //    if (iclbRHICDonarDetails.Count == 0)
        //    {
        //        //If No Donor Exists, Do Nothing.
        //        return false;
        //    }
        //    else
        //    {
        //        //PROD PIR : 4047 Ref Raj Mail dated on 12/29/2010
        //        //Current Person must be a Eligible Receiver (he should have payee account or he should receive from Deceased Person)
        //        //Commented for PIR 9024
        //        //if (iclbRHICDonarDetails.Count == 1)
        //        //{
        //        //    if (iclbRHICDonarDetails.First().ienmDonorPayeeAccountType == busConstant.donor_payee_account_type.active_spouse)
        //        //    {
        //        //        //Receiver is not eligible so don't create RHIC record / workflow
        //        //        return false;
        //        //    }
        //        //}

        //        //All donors must be same person or deceased spouse (no active spouse donor when we do automatic Rhic Combine)
        //        foreach (busBenefitRhicCombineDetail lbusBenrhicCombineDetail in iclbRHICDonarDetails)
        //        {
        //            if (lbusBenrhicCombineDetail.ienmDonorPayeeAccountType == busConstant.donor_payee_account_type.active_spouse)
        //            {
        //                lblnInitiateWorkflow = true;
        //                break;
        //            }
        //        }

        //        //None of the Eligible Donors are already associated with a Valid / Approved RHIC Maintenance Record
        //        if (!lblnInitiateWorkflow)
        //        {
        //            foreach (busBenefitRhicCombineDetail lbusBenrhicCombineDetail in iclbRHICDonarDetails)
        //            {
        //                if (lbusBenrhicCombineDetail.ibusPayeeAccount == null)
        //                    lbusBenrhicCombineDetail.LoadPayeeAccount();

        //                if (IsAlreadyCombined(lbusBenrhicCombineDetail.ibusPayeeAccount))
        //                {
        //                    lblnInitiateWorkflow = true;
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    if (lblnInitiateWorkflow)
        //    {
        //        InitiateRHICCombineWorkflow();
        //        return false;
        //    }

        //    //System must END current RHIC maintenance when a new donor established for Receiver Person / RHIC Amount Changes / Payee Account Ending or Completing / Health Enrollment Changes

        //    if (iblnOverlapHistory)
        //    {
        //        if (ibusPerson.iclbBenefitRhicCombine == null)
        //            ibusPerson.LoadBenefitRhicCombine();
        //        var lenuOverlapCombineRecords = ibusPerson.iclbBenefitRhicCombine.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoBenefitRhicCombine.start_date,
        //                                                                                i.icdoBenefitRhicCombine.start_date, i.icdoBenefitRhicCombine.end_date) ||
        //                                                                                i.icdoBenefitRhicCombine.start_date > icdoBenefitRhicCombine.start_date);
        //        foreach (busBenefitRhicCombine lbusRhicCombine in lenuOverlapCombineRecords)
        //        {
        //            //If there is no End Date / Start Date is future date
        //            if (lbusRhicCombine.icdoBenefitRhicCombine.end_date == DateTime.MinValue)
        //            {
        //                lbusRhicCombine.CancelRHICCombine();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //Load all Valid Approved Record which is overlapping with current start date
        //        if (ibusPerson.ibusLatestBenefitRhicCombine == null)
        //            ibusPerson.LoadLatestBenefitRhicCombine();
        //        if (ibusPerson.ibusLatestBenefitRhicCombine != null)
        //        {
        //            if (ienmAutomaticRhicCombineTrigger == busConstant.automatic_rhic_combine_trigger.payment_cancelled ||
        //                ienmAutomaticRhicCombineTrigger == busConstant.automatic_rhic_combine_trigger.benefit_adjustment_approval)  //PROD PIR ID 5867
        //                //ienmAutomaticRhicCombineTrigger == busConstant.automatic_rhic_combine_trigger.health_premium_change)
        //            {
        //                ibusPerson.ibusLatestBenefitRhicCombine.CancelRHICCombine();
        //            }
        //            else
        //            {
        //                DateTime ldtEndDate = CalculateRhicEndDate();
        //                ibusPerson.ibusLatestBenefitRhicCombine.EndRHICCombine(ldtEndDate);
        //            }
        //        }
        //    }

        //    if (ibusPerson.ibusReceiverHealthPersonAccount == null)
        //        ibusPerson.LoadReceiverHealthPersonAccount(icdoBenefitRhicCombine.start_date_no_null);

        //    if (ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
        //        icdoBenefitRhicCombine.apply_to_value = busConstant.Flag_Yes;
        //    icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusApproved;
        //    icdoBenefitRhicCombine.status_value = busConstant.RHICStatusValid;
        //    icdoBenefitRhicCombine.approved_by = iobjPassInfo.istrUserID;
        //    icdoBenefitRhicCombine.request_date = DateTime.Now;
        //    icdoBenefitRhicCombine.ienuObjectState = ObjectState.Insert;
        //    iarrChangeLog.Add(icdoBenefitRhicCombine);

        //    foreach (busBenefitRhicCombineDetail lbusBenRhicCompDetail in iclbRHICDonarDetails)
        //    {
        //        if (lbusBenRhicCompDetail.ibusPayeeAccount.IsNull()) lbusBenRhicCompDetail.LoadPayeeAccount();
        //        if (lbusBenRhicCompDetail.ibusPayeeAccount.IsNotNull() && // PROD PIR ID 7296
        //            lbusBenRhicCompDetail.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath) 
        //            lbusBenRhicCompDetail.icdoBenefitRhicCombineDetail.combine_flag = IsAlreadyCombinedWithCombineFlag(lbusBenRhicCompDetail.ibusPayeeAccount);
        //        else
        //            lbusBenRhicCompDetail.icdoBenefitRhicCombineDetail.combine_flag = busConstant.Flag_Yes;
        //    }

        //    iblnIsAutomaticCombine = true;
        //    BeforePersistChanges();
        //    PersistChanges();
        //    ApproveRHICCombineAndAllocateToSplitTable();
        //    iblnIsAutomaticCombine = false;
        //    return true;
        //}

        //NOTE :- to be set only if called from PERSLink BATCH
        //property to post the batch schedule ID into PAPIT if called from Update RHIC Rate Change batch
        public int iintBatchScheduleId { get; set; }

        public void CreatePAPITAdjustment()
        {
            if (ibusPerson.ibusReceiverHealthPersonAccount == null)
                ibusPerson.LoadReceiverHealthPersonAccount(icdoBenefitRhicCombine.start_date);

            if (ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
            {
                CreatePAPITAdjustmentByPlan(ibusPerson.ibusReceiverHealthPersonAccount);
            }

            if (ibusPerson.ibusReceiverMedicarePersonAccount == null)
                ibusPerson.LoadReceiverMedicarePersonAccount(icdoBenefitRhicCombine.start_date);

            if (ibusPerson.ibusReceiverMedicarePersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
            {
                CreatePAPITAdjustmentByPlan(ibusPerson.ibusReceiverMedicarePersonAccount);
            }
        }

        private void CreatePAPITAdjustmentByPlan(busPersonAccountGhdv abusPersonAccountGhdv)
        {
            DateTime ldtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            
            if (abusPersonAccountGhdv.ibusPaymentElection == null)
                abusPersonAccountGhdv.LoadPaymentElection();

            //PROD PIR 5538 : need to calculate premium based on latest date
            //Must reload Provider Org Plan based on effective date. if the enrollment change happens between 2010 and 2011 , chances for two different providers.
            abusPersonAccountGhdv.LoadActiveProviderOrgPlan(GetLatestDate(abusPersonAccountGhdv.icdoPersonAccount.current_plan_start_date,
                                abusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                ldtNextBenefitPaymentDate));
            abusPersonAccountGhdv.CalculatePremiumAmount(GetLatestDate(abusPersonAccountGhdv.icdoPersonAccount.current_plan_start_date,
                                abusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                ldtNextBenefitPaymentDate));

            //property to post the batch schedule ID into PAPIT if called from Update RHIC Rate Change batch
            abusPersonAccountGhdv.ibusPaymentElection.iintBatchScheduleId = iintBatchScheduleId;
            abusPersonAccountGhdv.ibusPaymentElection.ManagePayeeAccountPaymentItemType(
                    abusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                    abusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                    abusPersonAccountGhdv.icdoPersonAccountGhdv.MonthlyPremiumAmount - abusPersonAccountGhdv.icdoPersonAccountGhdv.total_rhic_amount,
                    abusPersonAccountGhdv.icdoPersonAccount.plan_id,
                    GetLatestDate(abusPersonAccountGhdv.icdoPersonAccount.current_plan_start_date,
                    abusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date, ldtNextBenefitPaymentDate),
                    abusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value,
                    abusPersonAccountGhdv.icdoPersonAccount.provider_org_id);

            //PROD PIR 5538 : need to calculate premium based on latest date
            abusPersonAccountGhdv.RecalculatePremiumBasedOnPlanEffectiveDate();
        }

        private DateTime GetLatestDate(DateTime ldtFirstDate, DateTime ldtSecondDate, DateTime ldtThirdDate)
        {
            return ldtFirstDate > ldtSecondDate ? (ldtFirstDate > ldtThirdDate ? ldtFirstDate : ldtThirdDate) :
                (ldtSecondDate > ldtThirdDate ? ldtSecondDate : ldtThirdDate);
        }
        public void CreatePayrollAdjustment()
        {
            CreatePayrollAdjustment(icdoBenefitRhicCombine.start_date);
        }
        //PROD ISSUE Fix : When end dated , we need to create adjustments only from Next Month of End Date
        public void CreatePayrollAdjustment(DateTime adtEffectiveDate)
        {
            if (adtEffectiveDate != DateTime.MinValue)
            {
                if (ibusPerson == null)
                    LoadPerson();

                if (ibusPerson.ibusReceiverHealthPersonAccount == null)
                    ibusPerson.LoadReceiverHealthPersonAccount(adtEffectiveDate);

                if (ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                {
                    ibusPerson.ibusReceiverHealthPersonAccount.ibusPerson = ibusPerson;
                    ibusPerson.ibusReceiverHealthPersonAccount.CreateAdjustmentPayrollForEnrollmentHistoryClose(adtEffectiveDate,
                        ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccount.history_change_date_no_null);

                    ibusPerson.ibusReceiverHealthPersonAccount.CreateAdjustmentPayrollForEnrollmentHistoryAdd(adtEffectiveDate);
                }

                if (ibusPerson.ibusReceiverMedicarePersonAccount == null)
                    ibusPerson.LoadReceiverMedicarePersonAccount(adtEffectiveDate);

                if (ibusPerson.ibusReceiverMedicarePersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                {
                    ibusPerson.ibusReceiverMedicarePersonAccount.ibusPerson = ibusPerson;
                    ibusPerson.ibusReceiverMedicarePersonAccount.CreateAdjustmentPayrollForEnrollmentHistoryClose(adtEffectiveDate,
                        ibusPerson.ibusReceiverMedicarePersonAccount.icdoPersonAccount.history_change_date_no_null);

                    ibusPerson.ibusReceiverMedicarePersonAccount.CreateAdjustmentPayrollForEnrollmentHistoryAdd(adtEffectiveDate);
                }
            }
        }

        private bool ApproveRHICCombineAndAllocateToSplitTable()
        {
            bool lblnAllocatedToHealth = false;

            //Email From Raj and Satya Dated on 10/24/2010 - If Apply Flag is not checked, System should not put entries into Split Table
            if (icdoBenefitRhicCombine.apply_to_value == busConstant.Flag_Yes)
            {
                if (ibusPerson == null)
                    LoadPerson();

                //Load the Receiver Health Premium Amount 
                if (ibusPerson.ibusReceiverHealthPersonAccount == null)
                    ibusPerson.LoadReceiverHealthPersonAccount(icdoBenefitRhicCombine.start_date_no_null);

                if (ibusPerson.ibusReceiverMedicarePersonAccount == null)
                    ibusPerson.LoadReceiverMedicarePersonAccount(icdoBenefitRhicCombine.start_date_no_null);

                //Health or Medicare must exists for Automatic Split TODO: UCS 79 : reimbursement will do later
                if (ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0 ||
                    ibusPerson.ibusReceiverMedicarePersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                {

                    if (iclbRHICDonarDetails == null)
                        LoadRHICDonorDetails();

                    //Load the Plan First to Sort the Collection By Job Service Plan,Because we must allocate JS plan RHIC first and then other plans
                    foreach (busBenefitRhicCombineDetail lbusBenRhicCombineDetail in iclbRHICDonarDetails)
                    {
                        if (lbusBenRhicCombineDetail.ibusPayeeAccount == null)
                            lbusBenRhicCombineDetail.LoadPayeeAccount();

                        if (lbusBenRhicCombineDetail.ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                            lbusBenRhicCombineDetail.ibusPayeeAccount.LoadActivePayeeStatus();

                        if (lbusBenRhicCombineDetail.ibusPayeeAccount.ibusPlan == null)
                            lbusBenRhicCombineDetail.ibusPayeeAccount.LoadPlan();
                    }
                    iclbRHICDonarDetails = busGlobalFunctions.Sort<busBenefitRhicCombineDetail>("ibusPayeeAccount.ibusPlan.icdoPlan.job_service_sort_order", iclbRHICDonarDetails);

                    //Load the Premium
                    if (ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        CalculateReceiverPersonAccountPremium(ibusPerson.ibusReceiverHealthPersonAccount);
                    decimal ldecTotalHealthPremium = ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                    if (ibusPerson.ibusReceiverMedicarePersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        CalculateReceiverPersonAccountPremium(ibusPerson.ibusReceiverMedicarePersonAccount);
                    decimal ldecTotalMedicarePremium = ibusPerson.ibusReceiverMedicarePersonAccount.icdoPersonAccountGhdv.MonthlyPremiumAmount;

                    decimal ldecAppliedHealthJSRHICAmount = 0.00M;
                    decimal ldecAppliedHealthOtherRHICAmount = 0.00M;

                    decimal ldecAppliedMedicareJSRHICAmount = 0.00M;
                    decimal ldecAppliedMedicareOtherRHICAmount = 0.00M;

                    foreach (busBenefitRhicCombineDetail lbusBenRhicCombineDetail in iclbRHICDonarDetails)
                    {
                        //UAT PIR 2143 : Apply to Health Only when Payee Account Status in RECEIVING Status
                        if (lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                        {
                            //First Apply to Health
                            //If Greater than zero, still rhic can be applied to health
                            decimal ldecRemainingHealthAmountToBeApplied = ldecTotalHealthPremium - (ldecAppliedHealthJSRHICAmount + ldecAppliedHealthOtherRHICAmount);
                            decimal ldecAvailableAmountToApply = lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.rhic_amount;
                            if (ldecRemainingHealthAmountToBeApplied > 0)
                            {
                                decimal ldecAmountToApply = Math.Min(ldecAvailableAmountToApply, ldecRemainingHealthAmountToBeApplied);
                                if (lbusBenRhicCombineDetail.ibusPayeeAccount.ibusPlan.IsJobServicePlan())
                                    ldecAppliedHealthJSRHICAmount += ldecAmountToApply;
                                else
                                    ldecAppliedHealthOtherRHICAmount += ldecAmountToApply;

                                ldecAvailableAmountToApply -= ldecAmountToApply;
                            }

                            //Apply to Medicare If RHIC Portion remained
                            if (ldecAvailableAmountToApply > 0)
                            {
                                decimal ldecRemainingMedicareAmountToBeApplied = ldecTotalMedicarePremium - (ldecAppliedMedicareJSRHICAmount + ldecAppliedMedicareOtherRHICAmount);
                                if (ldecRemainingMedicareAmountToBeApplied > 0)
                                {
                                    decimal ldecAmountToApply = Math.Min(ldecAvailableAmountToApply, ldecRemainingMedicareAmountToBeApplied);
                                    if (lbusBenRhicCombineDetail.ibusPayeeAccount.ibusPlan.IsJobServicePlan())
                                        ldecAppliedMedicareJSRHICAmount += ldecAmountToApply;
                                    else
                                        ldecAppliedMedicareOtherRHICAmount += ldecAmountToApply;

                                    ldecAvailableAmountToApply -= ldecAmountToApply;
                                }
                            }
                        }
                    }

                    if (ldecAppliedHealthJSRHICAmount > 0 || ldecAppliedHealthOtherRHICAmount > 0)
                    {
                        lblnAllocatedToHealth = true;
                        CreateRHICCombineHealthSplit(ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.person_account_id, ldecAppliedHealthJSRHICAmount, ldecAppliedHealthOtherRHICAmount);
                    }

                    if (ldecAppliedMedicareJSRHICAmount > 0 || ldecAppliedMedicareOtherRHICAmount > 0)
                    {
                        lblnAllocatedToHealth = true;
                        CreateRHICCombineHealthSplit(ibusPerson.ibusReceiverMedicarePersonAccount.icdoPersonAccountGhdv.person_account_id, ldecAppliedMedicareJSRHICAmount, ldecAppliedMedicareOtherRHICAmount);
                    }

                    //Reload Health Split After the Insert 
                    LoadBenefitRhicCombineHealthSplits();
                    if (ibusPerson.ibusReceiverHealthPersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        ibusPerson.ibusReceiverHealthPersonAccount.LoadBenefitRhicCombineHealthSplit();
                    if (ibusPerson.ibusReceiverMedicarePersonAccount.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        ibusPerson.ibusReceiverMedicarePersonAccount.LoadBenefitRhicCombineHealthSplit();
                    icdoBenefitRhicCombine.total_js_rhic_amount = iclbBenefitRhicCombineHealthSplit.Sum(i => i.icdoBenefitRhicCombineHealthSplit.js_rhic_amount);
                    icdoBenefitRhicCombine.total_other_rhic_amount = iclbBenefitRhicCombineHealthSplit.Sum(i => i.icdoBenefitRhicCombineHealthSplit.other_rhic_amount);
                }
            }
            icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusApproved;
            icdoBenefitRhicCombine.Update();
            return lblnAllocatedToHealth;
        }

        public void CreateRHICCombineHealthSplit(int aintPersonAccountID, decimal adecJSRhicAmount, decimal adecOtherRhicAmount, decimal adecReimbursementAmount = 0,
            int aintPayeeAccountRetroPaymentID = 0)
        {
            cdoBenefitRhicCombineHealthSplit lcdoBenefitRhicCombineHealthSplit = new cdoBenefitRhicCombineHealthSplit();
            lcdoBenefitRhicCombineHealthSplit.person_account_id = aintPersonAccountID;
            lcdoBenefitRhicCombineHealthSplit.benefit_rhic_combine_id = icdoBenefitRhicCombine.benefit_rhic_combine_id;
            lcdoBenefitRhicCombineHealthSplit.js_rhic_amount = adecJSRhicAmount;
            lcdoBenefitRhicCombineHealthSplit.other_rhic_amount = adecOtherRhicAmount;
            lcdoBenefitRhicCombineHealthSplit.reimbursement_amount = adecReimbursementAmount;
            lcdoBenefitRhicCombineHealthSplit.payee_account_retro_payment_id = aintPayeeAccountRetroPaymentID;
            lcdoBenefitRhicCombineHealthSplit.Insert();
        }

        public override void BeforePersistChanges()
        {
            //No Need to Reload this collection for Automatic Combine
            if (!iblnIsAutomaticCombine)
            {
                List<int> larrOldItems = new List<int>();
                foreach (busBenefitRhicCombineDetail lbusBenRhicCombineDetail in iclbRHICDonarDetails)
                {
                    if (lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                    {
                        larrOldItems.Add(lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.donar_payee_account_id);
                    }

                    var lbusBenRhicCombineDetailToRemove = iarrChangeLog.OfType<cdoBenefitRhicCombineDetail>()
                                                           .FirstOrDefault(i => i.donar_payee_account_id == lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.donar_payee_account_id);
                    if (lbusBenRhicCombineDetailToRemove != null)
                    {
                        iarrChangeLog.Remove(lbusBenRhicCombineDetailToRemove);
                    }
                }

                //Reload the RHIC Donor Details Collection
                LoadRHICDonorDetails();

                if (larrOldItems.Count > 0)
                {
                    foreach (busBenefitRhicCombineDetail lbusBenRhicCombineDetail in iclbRHICDonarDetails)
                    {
                        if (larrOldItems.Any(i => i == lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.donar_payee_account_id))
                        {
                            lbusBenRhicCombineDetail.icdoBenefitRhicCombineDetail.combine_flag = busConstant.Flag_Yes;
                        }
                    }
                }
            }

            if (iclbRHICDonarDetails != null)
                icdoBenefitRhicCombine.combined_rhic_amount = iclbRHICDonarDetails.Where(i => i.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                                                                                  .Sum(i => i.icdoBenefitRhicCombineDetail.rhic_amount);
            if (iclbEstimatedRHICDonorDetails != null)
                icdoBenefitRhicCombine.estimated_combined_rhic_amount = iclbEstimatedRHICDonorDetails.Where(i => i.icdoBenefitRhicEstimateCombineDetail.combine_flag == busConstant.Flag_Yes)
                                                                                .Sum(i => i.icdoBenefitRhicEstimateCombineDetail.rhic_amount);
            base.BeforePersistChanges();
        }

        public override busBase GetCorPerson()
        {
            return ibusPerson;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            //Reload Donor Grid
            LoadBenefitRhicCombineDetails();
            LoadBenefitRhicEstimateCombineDetails();
            LoadRHICDonorDetails(ablnReloadFromPayeeAccount: false);
            LoadEstimatedRHICDonorDetails(ablnReloadFromAccount: false);
        }

        //UAT PIR 1959 also Raj Email Dated on 8/17/2010
        //If RHIC is not applied. Then record can be cancelled without entering end date.
        public bool IsCancelButtonToShow()
        {
            if (icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusPendingApproval)
                return true;

            if ((icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved) && (icdoBenefitRhicCombine.apply_to_value != busConstant.Flag_Yes))
                return true;
            return false;
        }

        public bool IsApplyToHealthEnabled()
        {
            cdoCodeValue lobjCodeValue =
                    busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID,
                                                           busConstant.ApplyToHealthEnrollment);

            if (lobjCodeValue.data1.Equals("Y"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool iblnInitiateWorkFlow { get; set; } = false;
        public bool CreateAutomatedRHICCombine()
        {
            LoadPerson();
            if (ibusPerson.iclbBenefitRhicCombine == null)
                ibusPerson.LoadBenefitRhicCombine();

            //reload the Donor list based on the latest changes 
            LoadRHICDonorDetails(ablnIsAutomaticCombine: true);

            //BR-0550135a
            bool lblnInitiateWorkflow = false;
            if (iclbRHICDonarDetails.Count == 0)
            {
                //If No Donor Exists, Do Nothing.
                return false;
            }
            else
            {
                //PROD PIR : 4047 Ref Raj Mail dated on 12/29/2010
                //Current Person must be a Eligible Receiver (he should have payee account or he should receive from Deceased Person)
                //Commented for PIR 9024
                //if (iclbRHICDonarDetails.Count == 1)
                //{
                //    if (iclbRHICDonarDetails.First().ienmDonorPayeeAccountType == busConstant.donor_payee_account_type.active_spouse)
                //    {
                //        //Receiver is not eligible so don't create RHIC record / workflow
                //        return false;
                //    }
                //}

                //All donors must be same person or deceased spouse (no active spouse donor when we do automatic Rhic Combine)
                foreach (busBenefitRhicCombineDetail lbusBenrhicCombineDetail in iclbRHICDonarDetails)
                {
                    if (lbusBenrhicCombineDetail.ienmDonorPayeeAccountType == busConstant.donor_payee_account_type.active_spouse)
                    {
                        lblnInitiateWorkflow = true;
                        break;
                    }
                }

                //None of the Eligible Donors are already associated with a Valid / Approved RHIC Maintenance Record
                if (!lblnInitiateWorkflow)
                {
                    foreach (busBenefitRhicCombineDetail lbusBenrhicCombineDetail in iclbRHICDonarDetails)
                    {
                        if (lbusBenrhicCombineDetail.ibusPayeeAccount == null)
                            lbusBenrhicCombineDetail.LoadPayeeAccount();

                        if (IsAlreadyCombined(lbusBenrhicCombineDetail.ibusPayeeAccount))
                        {
                            lblnInitiateWorkflow = true;
                            iblnIsAlreadyCombined = true;
                            break;
                        }
                    }
                }
            }

            if (lblnInitiateWorkflow || iblnInitiateWorkFlow)
            {
                InitiateRHICCombineWorkflow();
                if (iblnIsAlreadyCombined)
                    return false;
            }
            if (ibusPerson.ibusLatestBenefitRhicCombine == null)
                ibusPerson.LoadLatestBenefitRhicCombine();
            if (ibusPerson.ibusLatestBenefitRhicCombine != null)
            {
                if (ienmAutomaticRhicCombineTrigger == busConstant.automatic_rhic_combine_trigger.payment_cancelled)
                {
                    ibusPerson.ibusLatestBenefitRhicCombine.CancelRHICCombine();
                }
                else
                {
                    ibusPerson.ibusLatestBenefitRhicCombine.EndRHICCombine(icdoBenefitRhicCombine.start_date.AddDays(-1));
                }
            }
            icdoBenefitRhicCombine.apply_to_value = busConstant.Flag_No;
            icdoBenefitRhicCombine.action_status_value = busConstant.RHICActionStatusApproved;
            icdoBenefitRhicCombine.status_value = busConstant.RHICStatusValid;
            icdoBenefitRhicCombine.approved_by = iobjPassInfo.istrUserID;
            icdoBenefitRhicCombine.request_date = DateTime.Now;
            icdoBenefitRhicCombine.ienuObjectState = ObjectState.Insert;
            iarrChangeLog.Add(icdoBenefitRhicCombine);

            foreach (busBenefitRhicCombineDetail lbusBenRhicCompDetail in iclbRHICDonarDetails)
            {
                //if (lbusBenRhicCompDetail.ibusPayeeAccount.IsNull()) lbusBenRhicCompDetail.LoadPayeeAccount();
                //if (lbusBenRhicCompDetail.ibusPayeeAccount.IsNotNull() && // PROD PIR ID 7296
                //    lbusBenRhicCompDetail.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                //    lbusBenRhicCompDetail.icdoBenefitRhicCombineDetail.combine_flag = IsAlreadyCombinedWithCombineFlag(lbusBenRhicCompDetail.ibusPayeeAccount);
                //else
                lbusBenRhicCompDetail.icdoBenefitRhicCombineDetail.combine_flag = busConstant.Flag_Yes;
            }
            //if (ienmAutomaticRhicCombineTrigger == busConstant.automatic_rhic_combine_trigger.death_notification_save)
            //{
            //    busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail = iclbRHICDonarDetails.Where(i => i.ienmDonorPayeeAccountType == busConstant.donor_payee_account_type.deceased_spouse).FirstOrDefault();
            //    if (lbusBenefitRhicCombineDetail.IsNotNull())
            //    {
            //        iclbRHICDonarDetails.Remove(lbusBenefitRhicCombineDetail);
            //        iarrChangeLog.Remove(lbusBenefitRhicCombineDetail.icdoBenefitRhicCombineDetail);
            //    }
            //}
            iblnIsAutomaticCombine = true;
            BeforePersistChanges();
            PersistChanges();
            //ApproveRHICCombineAndAllocateToSplitTable();
            iblnIsAutomaticCombine = false;
            return true;
        }
        //PIR 14346 - Initiating popup rhic amount workflow as per Maik mail
        public void InitiatePopUpRHICWorkflow()
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Popup_RHIC_Amount, icdoBenefitRhicCombine.person_id, 0, 0, iobjPassInfo);
        }
    }
}