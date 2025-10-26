using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Collections;
using Sagitec.CustomDataObjects;

namespace NeoSpinBatch
{
    class busEmployeeDeathBatch : busNeoSpinBatch
    {
        public busDBCacheData ibusDBCacheData { get; set; }
        public Collection<busOrgPlan> iclbProviderOrgPlan { get; set; }
        public DataTable idtbPALifeOptionHistory { get; set; }
        public DataTable idtbGHDVHistory { get; set; }
        public DataTable idtbLifeHistory { get; set; }
        DataTable idtbBenProvisionBenType;
        DataTable idtbBenOptionFactor;
        DataTable idtbBenefitProvisionExclusion;
        DataTable idtbAgeEstimate;
        DateTime idtLastIntertestPostDate;
        DataTable idtbDeathBenOptionCodeValue;
        DataTable idtbBenOptionCodeValue;
        public void GenerateCorrepondenceForEmployeeDeath()
        {
            istrProcessName = "Employee Death Batch ";

            idlgUpdateProcessLog("Load all deceased Person enrolled in any Retirement Or Insurance Plan", "INFO", istrProcessName);

            //Loading DB Cache (optimization)
            idlgUpdateProcessLog("Loading DB Cache Data", "INFO", istrProcessName);
            busDeathNotification lobjDeathNotification = new busDeathNotification { icdoDeathNotification = new cdoDeathNotification() };
            lobjDeathNotification.LoadAllCacheDataAndOtherTableData();
            DataTable ldtbGetEmployeeDeathRecords = busBase.Select("cdoBenefitApplication.EmployeeDeathBatch", new object[0] { });
            if (ldtbGetEmployeeDeathRecords.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbGetEmployeeDeathRecords.Rows)
                {
                    // int lintCount = 1;                    
                    lobjDeathNotification.icdoDeathNotification.LoadData(dr);

                    idlgUpdateProcessLog("Processing Employee Death Notification for the Person "
                        + lobjDeathNotification.icdoDeathNotification.person_id, "INFO", istrProcessName);

                    lobjDeathNotification.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lobjDeathNotification.LoadPerson();

                    if (lobjDeathNotification.ibusPerson.iclbPersonBeneficiary.IsNull())
                        lobjDeathNotification.ibusPerson.LoadBeneficiary();

                    //PIR 7940
                    Collection<busPersonBeneficiary> icolPersonBeneficiary = new Collection<busPersonBeneficiary>();
                    foreach (busPersonBeneficiary lobj in lobjDeathNotification.ibusPerson.iclbPersonBeneficiary)
                    {
                        if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull()) lobj.ibusPersonAccountBeneficiary.LoadPersonAccount();
                        if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsNull()) lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.LoadPlan();

                        if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDBRetirementPlan())
                            icolPersonBeneficiary.Add(lobj);
                    }
                    //PIR 7940 - Pick the retirement record instead of the default/first record
                    //var lenuDistinctBeneficiary = lobjDeathNotification.ibusPerson.iclbPersonBeneficiary.GroupBy(i => i.icdoPersonBeneficiary.beneficiary_person_id).Select(i => i.First());
                    var lenuDistinctBeneficiary = icolPersonBeneficiary.GroupBy(i => i.icdoPersonBeneficiary.beneficiary_person_id).Select(i => i.First());

                    foreach (busPersonBeneficiary lobjPersonBeneficiary in lenuDistinctBeneficiary)
                    {
                        lobjDeathNotification.istrRetirementAccountVisibility = "N";// PIR 7940
                        if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
                        {
                            if (lobjPersonBeneficiary.ibusPerson.IsNull())
                                lobjPersonBeneficiary.LoadPerson();

                            if (lobjPersonBeneficiary.ibusBeneficiaryPerson == null)
                                lobjPersonBeneficiary.LoadBeneficiaryPerson();

                            if (lobjPersonBeneficiary.ibusBeneficiaryPerson.icdoPerson.date_of_death == DateTime.MinValue)
                            {
                                lobjDeathNotification.LoadBeneficiaryAndCorPropertiesData(lobjDeathNotification, lobjPersonBeneficiary);
                            }
                        }
                    }
                    //update flag that will check no correspondence for the same person send again
                    lobjDeathNotification.icdoDeathNotification.employee_death_batch_letter_sent = busConstant.Flag_Yes;
                    lobjDeathNotification.icdoDeathNotification.Update();
                }
            }
            else
            {
                idlgUpdateProcessLog("No Records found", "INFO", istrProcessName);
            }
        }

        private busPersonAccountRetirement SetBeneficiaryRelatedProperties(busDeathNotification lobjDeathNotification, busPersonBeneficiary abusPersonBeneficiary,
            Collection<busPersonBeneficiary> aclbPersonBeneficiary, decimal ldecBeneficiaryMonthAndYear, ref bool lblnIsPersonDepedentInVision,
            ref bool lblnIsPersonDepedentInDental, ref bool lblnIsPersonDepedentInHealth, bool ablnMultipleBeneExists)
        {
            //check if the recipient is not beneficiary in any of DB/DC plan -- PIR systest 2644
            if (aclbPersonBeneficiary.Any(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsRetirementPlan()))
            {
                lobjDeathNotification.ibusPerson.istrIsMemberNotBeneficiaryInDBDCPlan = busConstant.Flag_No;

                //relationship is estate in Db / DC plan
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate)
                {
                    lobjDeathNotification.ibusPerson.istrIsEstateInDBDCPlan = busConstant.Flag_Yes;
                }
            }
            else
            {
                lobjDeathNotification.ibusPerson.istrIsMemberNotBeneficiaryInDBDCPlan = busConstant.Flag_Yes;
            }

            if (aclbPersonBeneficiary.Any(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDCRetirementPlan() ||
                                               i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsHBRetirementPlan()))
            {
                lobjDeathNotification.ibusPerson.istrIsMemberHadDCPlan = busConstant.Flag_Yes;
            }

            //relationship is trustee
            if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipTrustee)
            {
                lobjDeathNotification.ibusPerson.istrIsRelationshipTrustee = busConstant.Flag_Yes;
            }
            busPersonAccountRetirement lbusPersonAccountRetirement = null;
            if (lobjDeathNotification.ibusPerson.istrIsMemberNotBeneficiaryInDBDCPlan == busConstant.Flag_No)
            {
                //check is vested or not           
                lbusPersonAccountRetirement = CheckEmployeeVestedOrNot(lobjDeathNotification);

                //is realtionship is spouse
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                    lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse = busConstant.Flag_Yes;
                else lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse = busConstant.Flag_No;


                if (abusPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value == busConstant.BeneficiaryMemberTypePrimary)
                {//PIR 7940- Display only for primary beneficiary and not contingent beneficiary
                    //if multiple benes and vested
                    //or non vested
                    //or vested and non spouse
                    if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                        lobjDeathNotification.ibusPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene = busConstant.Flag_Yes;

                    if (ablnMultipleBeneExists && lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes)
                        lobjDeathNotification.ibusPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene = busConstant.Flag_Yes;

                    if ((!ablnMultipleBeneExists) && (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_No) &&
                        (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes))
                        lobjDeathNotification.ibusPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene = busConstant.Flag_Yes;

                    //  vested and only spouse is beneficiary
                    if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                        && lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)
                        lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse = busConstant.Flag_Yes;
                }
                //beneficiary enrolled in DC and relation is spouse
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipSpouse)
                {
                    if (aclbPersonBeneficiary.Any(i => i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDCRetirementPlan() ||
                                                       i.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsHBRetirementPlan()))
                    {
                        lobjDeathNotification.ibusPerson.istrIsDCBeneficiarySpouse = busConstant.Flag_Yes;
                    }
                }
            }

            //Beneficiary under 18 years of age
            if (abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0)
            {
                if (ldecBeneficiaryMonthAndYear < 18)
                    lobjDeathNotification.ibusPerson.istrIsBeneficiaryAgeLessThan18 = busConstant.Flag_Yes;
            }

            //******************************HEALTH********************************************************//

            //check if beneficiairy is health dependent
            if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                lobjDeathNotification.ibusPerson.LoadDependent();

            //person account will not be loaded in the loadDependent method
            foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
            {
                if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                    lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
            }

            var lenumDependent = lobjDeathNotification.ibusPerson.iclbPersonDependent.Where(lobjPD => lobjPD.icdoPersonDependent.dependent_perslink_id == abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                && lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth);
            if (lenumDependent.Count() > 0)
            {
                lblnIsPersonDepedentInHealth = true;
            }

            //deceased is not vested and enrolled in health and person is health dependent
            if (lobjDeathNotification.ibusPerson.IsPersonInGroupHealth())
            {
                // if (lblnIsPersonDepedentInHealth) //-- systest 2637
                {
                    if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                        || ((lblnIsPersonDepedentInHealth)
                        && lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse != busConstant.Flag_Yes)) //--systest 2644
                    {
                        lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInHealth = busConstant.Flag_Yes;
                    }
                }
            }

            //deceased is not in health and not vested
            if (!lobjDeathNotification.ibusPerson.IsPersonInGroupHealth()
                && lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInHealth = busConstant.Flag_Yes;

            //deceased has health insurance but no dependents
            if (lobjDeathNotification.ibusPerson.IsPersonInGroupHealth())
            {
                //load dependents for deceased
                if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                    lobjDeathNotification.ibusPerson.LoadDependent();

                //person account will not be loaded in the loadDependent method
                foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
                {
                    if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                        lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
                }

                var lenumHealthDependent = lobjDeathNotification.ibusPerson.iclbPersonDependent
                    .Where(lobjPD => lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth);
                if (lenumHealthDependent.Count() == 0)
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInHealthNoDependents = busConstant.Flag_Yes;
            }

            //vested married and person is health plan dependent
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried
                && lblnIsPersonDepedentInHealth
                && lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInHealth = busConstant.Flag_Yes;

            //deceased has no health insurance vested relationship is spouse
            if (!lobjDeathNotification.ibusPerson.IsPersonInGroupHealth())
            {
                if (lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsVestedNotInHealthRelationshipMarried = busConstant.Flag_Yes;
            }
            else
            {
                if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                    && !lblnIsPersonDepedentInHealth
                    && lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsVestedNotInHealthRelationshipMarried = busConstant.Flag_Yes;
            }

            if (lobjDeathNotification.ibusPerson.istrIsVestedNotInHealthRelationshipMarried == busConstant.Flag_Yes
                || lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInHealth == busConstant.Flag_Yes
                || lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInHealthNoDependents == busConstant.Flag_Yes
                || lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInHealth == busConstant.Flag_Yes
                || lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInHealth == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrisHealthVisible = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes
                || lblnIsPersonDepedentInHealth)
                lobjDeathNotification.ibusPerson.istrIsSpouseOrDependentInHealth = busConstant.Flag_Yes;

            //******************************DENTAL********************************************************//                            
            //check if beneficiairy is Dental dependent
            if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                lobjDeathNotification.ibusPerson.LoadDependent();

            //person account will not be loaded in the loadDependent method
            foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
            {
                if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                    lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
            }

            var lenumDentalDependent = lobjDeathNotification.ibusPerson.iclbPersonDependent.Where(lobjPD => lobjPD.icdoPersonDependent.dependent_perslink_id == abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                && lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental);
            if (lenumDentalDependent.Count() > 0)
            {

                lblnIsPersonDepedentInDental = true;
            }

            //deceased is not vested and enrolled in dental and person is dental dependent
            if (lobjDeathNotification.ibusPerson.IsPersonInDental())
            {
                if (lblnIsPersonDepedentInDental)
                    //if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                    //&& lblnIsPersonDepedentInDental)

                    if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                       || (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)) //--systest 2644                
                    {
                        lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInDental = busConstant.Flag_Yes;
                    }
            }

            //deceased is not in health and not vested
            if (!lobjDeathNotification.ibusPerson.IsPersonInDental()
                && lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInDental = busConstant.Flag_Yes;

            //deceased has health insurance but no dependents
            if (lobjDeathNotification.ibusPerson.IsPersonInDental())
            {
                //load dependents for deceased
                if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                    lobjDeathNotification.ibusPerson.LoadDependent();

                //person account will not be loaded in the loadDependent method
                foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
                {
                    if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                        lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
                }

                var lenumdentalDependents = lobjDeathNotification.ibusPerson.iclbPersonDependent
                    .Where(lobjPD => lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental);
                if (lenumdentalDependents.Count() == 0)
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInDentalNoDependents = busConstant.Flag_Yes;
            }

            //vested married and person is health plan dependent
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried
                && lblnIsPersonDepedentInDental)
                lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInDental = busConstant.Flag_Yes;

            //deceased has no health insurance vested relationship is spouse
            if (!lobjDeathNotification.ibusPerson.IsPersonInDental())
            {
                if (lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsVestedNotInDentalRelationshipMarried = busConstant.Flag_Yes;
            }

            if (lobjDeathNotification.ibusPerson.istrIsVestedNotInDentalRelationshipMarried == busConstant.Flag_Yes
              || lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInDental == busConstant.Flag_Yes
              || lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInDentalNoDependents == busConstant.Flag_Yes
              || lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInDental == busConstant.Flag_Yes
              || lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInDental == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrisDentalVisible = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes
              || lblnIsPersonDepedentInDental)
                lobjDeathNotification.ibusPerson.istrIsSpouseOrDependentInDental = busConstant.Flag_Yes;

            //******************************VISION********************************************************//

            //check if beneficiairy is vision dependent
            if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                lobjDeathNotification.ibusPerson.LoadDependent();

            //person account will not be loaded in the loadDependent method
            foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
            {
                if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                    lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
            }

            var lenumVisionDependent = lobjDeathNotification.ibusPerson.iclbPersonDependent.Where(lobjPD => lobjPD.icdoPersonDependent.dependent_perslink_id == abusPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id
                && lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision);
            if (lenumVisionDependent.Count() > 0)
            {

                lblnIsPersonDepedentInVision = true;
            }

            //deceased is not vested and enrolled in vision and person is vision dependent
            if (lobjDeathNotification.ibusPerson.IsPersonInVision())
            {
                if (lblnIsPersonDepedentInVision)
                    //if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                    //&& lblnIsPersonDepedentInVision)
                    if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                      || (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes)) //--systest 2644                
                    {
                        lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInVision = busConstant.Flag_Yes;
                    }
            }

            //deceased is not in vision and not vested
            if (!lobjDeathNotification.ibusPerson.IsPersonInVision()
                && lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInVision = busConstant.Flag_Yes;

            //deceased has vision insurance but no dependents
            if (lobjDeathNotification.ibusPerson.IsPersonInVision())
            {
                //load dependents for deceased
                if (lobjDeathNotification.ibusPerson.iclbPersonDependent.IsNull())
                    lobjDeathNotification.ibusPerson.LoadDependent();

                //person account will not be loaded in the loadDependent method
                foreach (busPersonDependent lobjPDependent in lobjDeathNotification.ibusPerson.iclbPersonDependent)
                {
                    if (lobjPDependent.ibusPeronAccountDependent.ibusPersonAccount.IsNull())
                        lobjPDependent.ibusPeronAccountDependent.LoadPersonAccount();
                }

                var lenumdentalDependents = lobjDeathNotification.ibusPerson.iclbPersonDependent
                    .Where(lobjPD => lobjPD.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision);
                if (lenumdentalDependents.Count() == 0)
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInVisionNoDependents = busConstant.Flag_Yes;
            }

            //vested married and person is health plan dependent
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried
                && lblnIsPersonDepedentInVision)
                lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInVision = busConstant.Flag_Yes;

            //deceased has no health insurance vested relationship is spouse
            if (!lobjDeathNotification.ibusPerson.IsPersonInVision())
            {
                if (lobjDeathNotification.ibusPerson.istrIsVestedRelationshipSpouse == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsVestedNotInVisionRelationshipMarried = busConstant.Flag_Yes;
            }

            if (lobjDeathNotification.ibusPerson.istrIsVestedNotInVisionRelationshipMarried == busConstant.Flag_Yes
          || lobjDeathNotification.ibusPerson.istrIsVestedMarriedBeneDependentInVision == busConstant.Flag_Yes
          || lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInVisionNoDependents == busConstant.Flag_Yes
          || lobjDeathNotification.ibusPerson.istrIsDeceasedNotEnrolledNotVestedInVision == busConstant.Flag_Yes
          || lobjDeathNotification.ibusPerson.istrIsBeneficiaryDependentInVision == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrisVisionVisible = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsRelationshipSpouse == busConstant.Flag_Yes
             || lblnIsPersonDepedentInVision)
                lobjDeathNotification.ibusPerson.istrIsSpouseOrDependentInVision = busConstant.Flag_Yes;

            //******************************LIFE********************************************************//
            bool lblnIsBeneInLife = false;
            //check if beneficiairy is Life                    
            if (aclbPersonBeneficiary.Any(lobjPD => lobjPD.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife))
            {
                lblnIsBeneInLife = true;
            }

            // is bene in life
            if (lblnIsBeneInLife)
                lobjDeathNotification.ibusPerson.istrIsBeneficiaryInLife = busConstant.Flag_Yes;
            else
                lobjDeathNotification.ibusPerson.istrIsBeneficiaryNotInLife = busConstant.Flag_Yes;

            //is life bene under 18
            if (lblnIsBeneInLife && ldecBeneficiaryMonthAndYear < 18)
                lobjDeathNotification.ibusPerson.istrIsLifeBeneAgeUnder18 = busConstant.Flag_Yes;

            //check if the bene is estate or trustee
            if (lblnIsBeneInLife)
            {
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipTrustee)
                    lobjDeathNotification.ibusPerson.istrIsLifeBeneTrustee = busConstant.Flag_Yes;
                //estate
                if (abusPersonBeneficiary.icdoPersonBeneficiary.relationship_value == busConstant.PersonBeneficiaryRelationshipEstate)
                    lobjDeathNotification.ibusPerson.istrIsLifeBeneEstate = busConstant.Flag_Yes;

                if (lobjDeathNotification.ibusPerson.istrIsLifeBeneTrustee == busConstant.Flag_Yes
                    || lobjDeathNotification.ibusPerson.istrIsLifeBeneEstate == busConstant.Flag_Yes)
                    lobjDeathNotification.ibusPerson.istrIsLifeBeneTrusteeOrEstate = busConstant.Flag_Yes;
            }

            //*************************************LTC***************************************
            //checkif the LTC account exists for the deceased
            if (lobjDeathNotification.ibusPerson.istrPersonInLTC == busConstant.Flag_Yes
                && abusPersonBeneficiary.ibusBeneficiaryPerson.istrPersonInLTC != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsLTCExtistsForDeceased = busConstant.Flag_Yes;

            //checkif the LTC account exists for the spouse
            if (abusPersonBeneficiary.ibusBeneficiaryPerson.istrPersonInLTC == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.istrPersonInLTC != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsLTCExtistsForSpouse = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsLTCExtistsForDeceased == busConstant.Flag_Yes
                && lobjDeathNotification.ibusPerson.istrIsLTCExtistsForSpouse == busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsLTCExtistsForBoth = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsLTCExtistsForDeceased != busConstant.Flag_Yes
               && lobjDeathNotification.ibusPerson.istrIsLTCExtistsForSpouse != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsLTCExtistsForNone = busConstant.Flag_Yes;

            //********************************************deferred comp account exists
            if (lobjDeathNotification.ibusPerson.IsPersonInDeferredComp())
                lobjDeathNotification.ibusPerson.istrIsdefCompPlanExits = busConstant.Flag_Yes;
            else
                lobjDeathNotification.ibusPerson.istrIsdefCompPlanNotExits = busConstant.Flag_Yes;

            //*********************************************Other 457
            if (lobjDeathNotification.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdOther457))
                lobjDeathNotification.ibusPerson.istrIsOther457PlanExits = busConstant.Flag_Yes;
            else
                lobjDeathNotification.ibusPerson.istrIsOther457PlanNotExits = busConstant.Flag_Yes;

            //*********************************************Is DC Plan Exists
            if (lobjDeathNotification.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDC))
                lobjDeathNotification.ibusPerson.istrIsDCPlanExits = busConstant.Flag_Yes;
            if (lobjDeathNotification.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDC2020) ||  //PIR 20232
                lobjDeathNotification.ibusPerson.IsMemberEnrolledInPlan(busConstant.PlanIdDC2025)) //PIR 25920
                lobjDeathNotification.ibusPerson.istrIsDCPlanExits = busConstant.Flag_Yes;

            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInVisionNoDependents != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsMemberInVisionDependentsEligible = busConstant.Flag_Yes;
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInDentalNoDependents != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsMemberInDentalDependentsEligible = busConstant.Flag_Yes;
            if (lobjDeathNotification.ibusPerson.istrIsDeceasedEnrolledInHealthNoDependents != busConstant.Flag_Yes)
                lobjDeathNotification.ibusPerson.istrIsMemberInHealthDependentsEligible = busConstant.Flag_Yes;

            //DC non vested or single death 
            //OR
            //dc vested and married death
            if (lobjDeathNotification.ibusPerson.istrIsDCPlanExits == busConstant.Flag_Yes)
            {
                if ((lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested == busConstant.Flag_Yes
                    || lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle)
                    ||
                    (lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested == busConstant.Flag_Yes
                    && lobjDeathNotification.ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried))
                    lobjDeathNotification.ibusPerson.istrIsDCNonVestedOrSingleDeathOrDCVestedAndMarriedDeath = busConstant.Flag_Yes;
            }

            if (lbusPersonAccountRetirement == null)
            {
                lbusPersonAccountRetirement = new busPersonAccountRetirement
                {
                    icdoPersonAccountRetirement = new cdoPersonAccountRetirement(),
                    icdoPersonAccount = new cdoPersonAccount()
                };
            }
            return lbusPersonAccountRetirement;
        }

        private busPersonAccountRetirement CheckEmployeeVestedOrNot(busDeathNotification lobjDeathNotification)
        {
            busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
            if (lobjDeathNotification.ibusPerson.iclbRetirementAccount == null)
                lobjDeathNotification.ibusPerson.LoadRetirementAccount();

            var lenumDBPersonAccount = lobjDeathNotification.ibusPerson.iclbRetirementAccount
                .Where(lobjRA => !lobjRA.IsWithDrawn());

            if (lenumDBPersonAccount.Count() > 0)
            {
                //Load the Account Balance                                    
                lbusPersonAccountRetirement.FindPersonAccountRetirement(lenumDBPersonAccount.FirstOrDefault().icdoPersonAccount.person_account_id);
                lbusPersonAccountRetirement.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonAccountRetirement.ibusPerson.icdoPerson = lobjDeathNotification.ibusPerson.icdoPerson;
                lbusPersonAccountRetirement.LoadLTDSummary();
                if (lbusPersonAccountRetirement.ibusPlan == null)
                    lbusPersonAccountRetirement.LoadPlan();
                lbusPersonAccountRetirement.LoadTotalVSC();
                lbusPersonAccountRetirement.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lbusPersonAccountRetirement.ibusPersonAccount.icdoPersonAccount = lbusPersonAccountRetirement.icdoPersonAccount;

                //calculate person age
                int lintMemberAgeInMonths = 0;
                int lintMemberAgeInYears = 0;
                decimal ldecMonthAndYear = 0m;
                int lintMonths = 0;
                busPersonBase.CalculateAge(lobjDeathNotification.ibusPerson.icdoPerson.date_of_birth,
                    lobjDeathNotification.icdoDeathNotification.date_of_death, ref lintMonths, ref ldecMonthAndYear, 2,
                    ref lintMemberAgeInYears, ref lintMemberAgeInMonths);

                if (busPersonBase.CheckIsPersonVested(lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_id
                    , lbusPersonAccountRetirement.ibusPlan.icdoPlan.plan_code,
                    lbusPersonAccountRetirement.ibusPlan.icdoPlan.benefit_provision_id,
                    busConstant.ApplicationBenefitTypeRetirement,
                    lbusPersonAccountRetirement.icdoPersonAccount.Total_VSC,
                    ldecMonthAndYear, lobjDeathNotification.icdoDeathNotification.date_of_death.GetFirstDayofNextMonth(), false, lobjDeathNotification.icdoDeathNotification.date_of_death, lbusPersonAccountRetirement, iobjPassInfo)) //PIR 14646 - Vesting logic changes
                {
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeVested = busConstant.Flag_Yes;
                }
                else
                {
                    lobjDeathNotification.ibusPerson.istrIsDeceasedEmployeeNonVested = busConstant.Flag_Yes;
                }
            }
            return lbusPersonAccountRetirement;
        }

        public void LoadAmountFields(busPerson aobjPerson)
        {
            if (aobjPerson.icolPersonAccount.IsNull())
                aobjPerson.LoadPersonAccount();

            bool lblnErrorFound = false;
            decimal ldecMemberPremiumAmt = 0.00M;
            int lintGHDVHistoryID = 0;
            string lstrGroupNumber = string.Empty;

            GetHealthPremiumAmountBasedOnConditions(aobjPerson, ref lblnErrorFound, ref lintGHDVHistoryID, ref lstrGroupNumber);

            int lintPADentalID = aobjPerson.icolPersonAccount.Where(lobj =>
                //lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                            lobj.icdoPersonAccount.plan_id == busConstant.PlanIdDental).Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault();
            if (lintPADentalID > 0)
            {
                decimal ldecSinglePremium = 0M;
                decimal ldecFamilyPremium = 0M;
                busPersonAccount lobjPA = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

                lobjPA.FindPersonAccount(lintPADentalID);
                lobjPA.LoadPersonAccountGHDV();

                // if (!String.IsNullOrEmpty(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value))
                {
                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageIndividual;
                    {
                        ldecSinglePremium = GetPremiumForDentalAndVision(aobjPerson, lobjPA);
                        aobjPerson.idecDentalSinglePolicyCOBRAPremium = ldecSinglePremium;
                    }
                    //}
                    //  if (!String.IsNullOrEmpty(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value))
                    //{
                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageFamily;
                    {
                        ldecFamilyPremium = GetPremiumForDentalAndVision(aobjPerson, lobjPA);
                        aobjPerson.idecDentalFamilyPolicyCOBRAPremium = ldecFamilyPremium;
                    }
                }
            }

            int lintPAVisionID = aobjPerson.icolPersonAccount.Where(lobj =>
                //lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                            lobj.icdoPersonAccount.plan_id == busConstant.PlanIdVision).Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault();
            if (lintPAVisionID > 0)
            {
                decimal ldecSinglePremium = 0M;
                decimal ldecFamilyPremium = 0M;
                busPersonAccount lobjPA = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

                lobjPA.FindPersonAccount(lintPAVisionID);
                lobjPA.LoadPersonAccountGHDV();

                // if (!String.IsNullOrEmpty(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value))
                {

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageIndividual;
                    ldecSinglePremium = GetPremiumForDentalAndVision(aobjPerson, lobjPA);
                    aobjPerson.idecVisionSinglePolicyCOBRAPremium = ldecSinglePremium;

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageFamily;
                    ldecFamilyPremium = GetPremiumForDentalAndVision(aobjPerson, lobjPA);
                    aobjPerson.idecVisionFamilyPolicyCOBRAPremium = ldecFamilyPremium;

                    ////if (lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual)
                    //{
                    //    aobjPerson.idecVisionSinglePolicyCOBRAPremium = ldecPremium;
                    //}
                    //// else if (lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily)
                    //{
                    //    aobjPerson.idecVisionFamilyPolicyCOBRAPremium = ldecPremium;
                    //}
                }
            }

            busPersonAccount lbusPersonAccount = aobjPerson.icolPersonAccount.Where(lobj =>
                //lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                           lobj.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife).FirstOrDefault();
            if ((lbusPersonAccount != null) && (lbusPersonAccount.icdoPersonAccount.person_account_id > 0))
            {
                busPersonAccountLife lobjPersonAccountLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
                lobjPersonAccountLife.FindPersonAccountLife(lbusPersonAccount.icdoPersonAccount.person_account_id);
                lobjPersonAccountLife.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;

                lobjPersonAccountLife.LoadLifeOptionData();

                //include only basic and supplemental level of coverage
                var lCoverage = lobjPersonAccountLife.iclbLifeOption.Where(lobjLife => lobjLife.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled
                    && (lobjLife.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic
                    || lobjLife.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental))
                    .Sum(lobjLife => lobjLife.icdoPersonAccountLifeOption.coverage_amount);

                aobjPerson.idecLifeInsurancePolicyValue = Convert.ToDecimal(lCoverage);
            }
        }

        private decimal GetPremiumForDentalAndVision(busPerson aobjPerson, busPersonAccount lobjPA)
        {
            busBase lbusBase = new busBase();
            bool lblnErrorFound = false;
            decimal ldecPremiumAmt = 0m;

            //lobjPA.ibusPersonAccountGHDV.LoadHistoryByDate(aobjPerson.icdoPerson.date_of_death);
            lobjPA.ibusPersonAccountGHDV.ibusPerson = aobjPerson;
            lobjPA.ibusPersonAccountGHDV.LoadPlan();

            lobjPA.ibusPersonAccountGHDV.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
            lobjPA.ibusPersonAccountGHDV.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
            lobjPA.ibusPersonAccountGHDV.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
            lobjPA.ibusPersonAccountGHDV.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
            lobjPA.ibusPersonAccountGHDV.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
            lobjPA.ibusPersonAccountGHDV.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
            lobjPA.ibusPersonAccountGHDV.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;

            //Get the GHDV History Object By Billing Month Year
            busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjPA.ibusPersonAccountGHDV.LoadHistoryByDate(aobjPerson.icdoPerson.date_of_death);
            if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == 0)
            {
                idlgUpdateProcessLog(
                    "Error : No History Record Found for Person Account = " +
                    lobjPA.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                lblnErrorFound = true;
            }

            if (!lblnErrorFound)
            {
                if (iclbProviderOrgPlan != null)
                {
                    busOrgPlan lbusProviderOrgPlan = iclbProviderOrgPlan.FirstOrDefault(i => i.icdoOrgPlan.plan_id == lobjPA.ibusPersonAccountGHDV.icdoPersonAccount.plan_id);
                    if (lbusProviderOrgPlan != null)
                    {
                        lobjPA.ibusPersonAccountGHDV.ibusProviderOrgPlan = lbusProviderOrgPlan;
                    }
                    else
                    {
                        lobjPA.ibusPersonAccountGHDV.LoadActiveProviderOrgPlan(aobjPerson.icdoPerson.date_of_death);
                    }
                }
                else
                {
                    lobjPA.ibusPersonAccountGHDV.LoadActiveProviderOrgPlan(aobjPerson.icdoPerson.date_of_death);
                }

                if (lobjPA.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                {
                    ldecPremiumAmt =
                        busRateHelper.GetDentalPremiumAmount(
                            lobjPA.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value,
                            lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                            aobjPerson.icdoPerson.date_of_death,
                            ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);

                }
                else if (lobjPA.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    ldecPremiumAmt =
                        busRateHelper.GetVisionPremiumAmount(
                            lobjPA.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                            lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value,
                            lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value,
                            aobjPerson.icdoPerson.date_of_death,
                            ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);
                }
            }
            return ldecPremiumAmt;
        }


        private void LoadInitialData()
        {
            idlgUpdateProcessLog("Loading All Active Providers", "INFO", iobjBatchSchedule.step_name);
            //Loading Complete Activte Provider Org Plan List (Optimization Purpose)
            LoadActiveProviders(DateTime.Now);
        }
        public void LoadDBCacheData()
        {
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

        public void LoadActiveProviders(DateTime adtEffectiveChangeDate)
        {
            DataTable ldtbActiveProviders = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadAllActiveProviders", new object[1] { adtEffectiveChangeDate });
            iclbProviderOrgPlan = new busBase().GetCollection<busOrgPlan>(ldtbActiveProviders, "icdoOrgPlan");
        }

        public void LoadLifeOptionData(DateTime adtEffectiveChangeDate)
        {
            idtbPALifeOptionHistory =
                busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadLifeOption", new object[1] { adtEffectiveChangeDate });
        }

        public void LoadLifeHistory(DateTime adtEffectiveChangeDate)
        {
            idtbLifeHistory =
                busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadLifeHistory", new object[1] { adtEffectiveChangeDate });
        }

        public busOrgPlan LoadProviderOrgPlanByProviderOrgId(int aintProviderOrgId, int aintPlanId, DateTime adtEffectiveChangeDate)
        {
            busOrgPlan lbusOrgPlanToReturn = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            foreach (var lbusOrgPlan in iclbProviderOrgPlan)
            {
                if ((lbusOrgPlan.icdoOrgPlan.org_id == aintProviderOrgId) && (lbusOrgPlan.icdoOrgPlan.plan_id == aintPlanId))
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveChangeDate,
                        lbusOrgPlan.icdoOrgPlan.participation_start_date,
                        lbusOrgPlan.icdoOrgPlan.participation_end_date))
                    {
                        lbusOrgPlanToReturn = lbusOrgPlan;
                        break;
                    }
                }
            }
            return lbusOrgPlanToReturn;
        }
        private void CreateCorrespondence(busDeathNotification aobjDeathNotification)
        {
            // Generate Correspondence
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjDeathNotification);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence("APP-7050", aobjDeathNotification, lhstDummyTable);
        }

        private void GetPremiumForHealthRetiree(busPerson aobjPerson)
        {
            busPersonAccountGhdv lobjPersonAccountGhdv = new busPersonAccountGhdv
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv()
            };
            lobjPersonAccountGhdv.icdoPersonAccount.plan_id = busConstant.PlanIdGroupHealth;
            lobjPersonAccountGhdv.icdoPersonAccount.person_id = aobjPerson.icdoPerson.person_id;

            lobjPersonAccountGhdv.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjPersonAccountGhdv.ibusPerson = aobjPerson;

            lobjPersonAccountGhdv.icdoPersonAccountGhdv.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
            lobjPersonAccountGhdv.icdoPersonAccount.start_date = DateTime.Now;
            lobjPersonAccountGhdv.icdoPersonAccount.history_change_date = DateTime.Now;
            lobjPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.employment_type_value = busConstant.PersonJobTypePermanent;

            //for coverage code single
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = "0021";
            aobjPerson.idecHealthSinglePolicyPremium = GetPremiumBasedOnCoverage(aobjPerson, lobjPersonAccountGhdv);

            //for coverage code family
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = "0022";
            aobjPerson.idecHealthFamilyPolicyPremium = GetPremiumBasedOnCoverage(aobjPerson, lobjPersonAccountGhdv);

            //for coverage code family 3+
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code = "0023";
            aobjPerson.idecHealthFamily3PolicyPremium = GetPremiumBasedOnCoverage(aobjPerson, lobjPersonAccountGhdv);
        }

        private decimal GetPremiumBasedOnCoverage(busPerson aobjPerson, busPersonAccountGhdv lobjPersonAccountGhdv)
        {
            bool lblnErrorFound = false;
            string lstrGroupNumber = string.Empty;

            lobjPersonAccountGhdv.LoadPlan();

            lobjPersonAccountGhdv.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
            lobjPersonAccountGhdv.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
            lobjPersonAccountGhdv.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
            lobjPersonAccountGhdv.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
            lobjPersonAccountGhdv.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
            lobjPersonAccountGhdv.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
            lobjPersonAccountGhdv.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;


            lstrGroupNumber = lobjPersonAccountGhdv.GetGroupNumber();

            //Initialize the Org Object to Avoid the NULL error
            lobjPersonAccountGhdv.InitializeObjects();
            lobjPersonAccountGhdv.idtPlanEffectiveDate = aobjPerson.icdoPerson.date_of_death;

            lobjPersonAccountGhdv.LoadActiveProviderOrgPlan(lobjPersonAccountGhdv.idtPlanEffectiveDate);

            if (lobjPersonAccountGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
            {
                lobjPersonAccountGhdv.LoadRateStructureForUserStructureCode();
            }
            else
            {
                lobjPersonAccountGhdv.LoadHealthParticipationDate();
                lobjPersonAccountGhdv.LoadHealthPlanOption();
                //To Get the Rate Structure Code (Derived Field)
                lobjPersonAccountGhdv.LoadRateStructure(aobjPerson.icdoPerson.date_of_death);
            }

            //Get the Coverage Ref ID
            lobjPersonAccountGhdv.LoadCoverageRefID();

            //Get the Premium Amount
            lobjPersonAccountGhdv.GetMonthlyPremiumAmountByRefID(aobjPerson.icdoPerson.date_of_death);

            if (lobjPersonAccountGhdv.icdoPersonAccountGhdv.Coverage_Ref_ID == 0)
            {
                idlgUpdateProcessLog(
                    "Error : Invalid Coverage Ref ID for Person Account = " +
                    lobjPersonAccountGhdv.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                lblnErrorFound = true;
            }
            if (!lblnErrorFound)
            {
                return lobjPersonAccountGhdv.icdoPersonAccountGhdv.MonthlyPremiumAmount;
            }
            return 0.00M;
        }

        private void GetPremiumForDentalVisionRetiree(busPerson aobjPerson, int aintPlanId)
        {
            busPersonAccountGhdv lobjPersonAccountGhdv = new busPersonAccountGhdv
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv()
            };
            lobjPersonAccountGhdv.icdoPersonAccount.plan_id = aintPlanId;
            lobjPersonAccountGhdv.icdoPersonAccount.person_id = aobjPerson.icdoPerson.person_id;

            lobjPersonAccountGhdv.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
            lobjPersonAccountGhdv.ibusPerson = aobjPerson;

            if (aintPlanId == busConstant.PlanIdDental)
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeRetiree;
            if (aintPlanId == busConstant.PlanIdVision)
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeRetiree;
            lobjPersonAccountGhdv.icdoPersonAccount.start_date = DateTime.Now;
            lobjPersonAccountGhdv.icdoPersonAccount.history_change_date = DateTime.Now;
            lobjPersonAccountGhdv.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjPersonAccountGhdv.icdoPersonAccountGhdv.employment_type_value = busConstant.PersonJobTypePermanent;

            //load the org plan provider         
            {
                lobjPersonAccountGhdv.LoadActiveProviderOrgPlan(lobjPersonAccountGhdv.icdoPersonAccount.current_plan_start_date_no_null);
            }

            if (aintPlanId == busConstant.PlanIdDental)
            {
                //for single dental LOC
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageIndividual;

                aobjPerson.idecDentalSinglePolicyPremium =
                    busRateHelper.GetDentalPremiumAmount(
                         lobjPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                         lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value,
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                        aobjPerson.icdoPerson.date_of_death,
                        ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);

                //for family dental LOC
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.DentalLevelofCoverageFamily;

                aobjPerson.idecDentalFamilyPolicyPremium =
                    busRateHelper.GetDentalPremiumAmount(
                         lobjPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                         lobjPersonAccountGhdv.icdoPersonAccountGhdv.dental_insurance_type_value,
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                        aobjPerson.icdoPerson.date_of_death,
                        ibusDBCacheData.idtbCachedDentalRate, iobjPassInfo);
            }
            else if (aintPlanId == busConstant.PlanIdVision)
            {
                //for single vision LOC
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageIndividual;
                aobjPerson.idecVisionSinglePolicyPremium =
                     busRateHelper.GetVisionPremiumAmount(
                         lobjPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                         lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value,
                         lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                         aobjPerson.icdoPerson.date_of_death,
                         ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);

                //for family vision LOC
                lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value = busConstant.VisionLevelofCoverageFamily;
                aobjPerson.idecVisionFamilyPolicyPremium =
                    busRateHelper.GetVisionPremiumAmount(
                        lobjPersonAccountGhdv.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id,
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.vision_insurance_type_value,
                        lobjPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_value,
                        aobjPerson.icdoPerson.date_of_death,
                        ibusDBCacheData.idtbCachedVisionRate, iobjPassInfo);
            }
        }

        //get the benefit options for deceased
        private void SetBenefitOptionAmount(busPerson aobjPerson, busPlan aobjPlan, busPersonAccount aobjPersonAccount)
        {
            //check for this plan whether 50 percent or 100 % JS is available

            Collection<cdoCodeValue> lclbbenefitOptions = busPersonBase.LoadBenefitOptionsBasedOnPlans(aobjPlan.icdoPlan.plan_id, busConstant.ApplicationBenefitTypePreRetirementDeath);

            var lGetRequired50BenefitOption = lclbbenefitOptions.Where(lCode => lCode.code_value == busConstant.BenefitOption50Percent);
            var lGetRequired100JSBenefitOption = lclbbenefitOptions.Where(lCode => lCode.code_value == busConstant.BenefitOption100PercentJS);
            if (lGetRequired50BenefitOption.Count() > 0
                && lGetRequired100JSBenefitOption.Count() > 0)
            {

                busPreRetirementDeathBenefitCalculation lobjPreRetirementDeath = new busPreRetirementDeathBenefitCalculation
                {
                    icdoBenefitCalculation = new cdoBenefitCalculation(),
                    ibusMember = new busPerson { icdoPerson = new cdoPerson() },
                    ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan { icdoPlan = new cdoPlan() } },
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                };
                lobjPreRetirementDeath.ibusMember = aobjPerson;
                lobjPreRetirementDeath.ibusPlan = aobjPlan;
                lobjPreRetirementDeath.ibusPersonAccount = aobjPersonAccount;
                lobjPreRetirementDeath.ibusPersonAccount.ibusPerson = aobjPerson;
                lobjPreRetirementDeath.ibusPersonAccount.ibusPlan = aobjPlan;
                lobjPreRetirementDeath.idtbBenOptionFactor = idtbBenOptionFactor;
                lobjPreRetirementDeath.idtbBenefitProvisionExclusion = idtbBenefitProvisionExclusion;
                lobjPreRetirementDeath.iblnUseDataTableForBenOptionFactor = true;
                lobjPreRetirementDeath.idtbBenOptionCodeValue = idtbBenOptionCodeValue;
                lobjPreRetirementDeath.idtbDeathBenOptionCodeValue = idtbDeathBenOptionCodeValue;
                lobjPreRetirementDeath.idtLastIntertestPostDate = idtLastIntertestPostDate;
                lobjPreRetirementDeath.LoadLastContributedDate();
                lobjPreRetirementDeath.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypePreRetirementDeath;
                lobjPreRetirementDeath.icdoBenefitCalculation.person_id = lobjPreRetirementDeath.ibusMember.icdoPerson.person_id;
                lobjPreRetirementDeath.icdoBenefitCalculation.plan_id = lobjPreRetirementDeath.ibusPlan.icdoPlan.plan_id;
                lobjPreRetirementDeath.icdoBenefitCalculation.created_date = DateTime.Now;
                lobjPreRetirementDeath.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal; // No Projections
                //lobjRetirementCalculation.iblnIncludeJobServiceSickLeave = true;
                lobjPreRetirementDeath.icdoBenefitCalculation.termination_date = aobjPerson.icdoPerson.date_of_death;
                lobjPreRetirementDeath.icdoBenefitCalculation.date_of_death = aobjPerson.icdoPerson.date_of_death;
                lobjPreRetirementDeath.icdoBenefitCalculation.retirement_date = aobjPerson.icdoPerson.date_of_death.AddMonths(1);
                lobjPreRetirementDeath.iblnIsFromEmployeeDeathBatch = true;
                lobjPreRetirementDeath.icdoBenefitCalculation.benefit_option_value = busConstant.BenefitOption50Percent;
                //lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                //lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                lobjPreRetirementDeath.SetNormalRetirementDate(ibusDBCacheData);
                lobjPreRetirementDeath.GetNormalEligibilityDetails(ibusDBCacheData);
                lobjPreRetirementDeath.LoadBenefitCalculationPayeeForNewMode();
                lobjPreRetirementDeath.LoadBenefitProvisionBenefitType(idtbBenProvisionBenType);
                lobjPreRetirementDeath.CalculatePreRetirementDeathBenefit();

                //get benefit option amount for 50 JS
                decimal ldec50PercentBenefitAmt = 0.00M;
                decimal ldec100JSPercentBenefitAmt = 0.00M;
                var lBenefit50PercentOPtionAmount = lobjPreRetirementDeath.iclbBenefitCalculationOptions.Where(lobjBO => lobjBO.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value
                    == busConstant.BenefitOption50Percent);
                var lBenefit100PercentOPtionAmount = lobjPreRetirementDeath.iclbBenefitCalculationOptions.Where(lobjBO => lobjBO.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value
                   == busConstant.BenefitOption100PercentJS);
                if (lBenefit50PercentOPtionAmount.Count() > 0)
                    ldec50PercentBenefitAmt = lBenefit50PercentOPtionAmount.FirstOrDefault().icdoBenefitCalculationOptions.benefit_option_amount;

                //lobjPreRetirementDeath.icdoBenefitCalculation.benefit_option_value = busConstant.BenefitOption100PercentJS;
                ////lobjRetirementCalculation.iblnConsoldatedVSCLoaded = false;
                ////lobjRetirementCalculation.iblnConsolidatedPSCLoaded = false;
                //lobjPreRetirementDeath.SetNormalRetirementDate(ibusDBCacheData);
                //lobjPreRetirementDeath.GetNormalEligibilityDetails(ibusDBCacheData);
                //lobjPreRetirementDeath.LoadBenefitCalculationPayeeForNewMode();
                //lobjPreRetirementDeath.LoadBenefitProvisionBenefitType(idtbBenProvisionBenType);
                //lobjPreRetirementDeath.CalculatePreRetirementDeathBenefit();

                //get benefit option amount for 100 JS
                //var lBenefit100PercentOPtionAmount = lobjPreRetirementDeath.iclbBenefitCalculationOptions.Where(lobjBO => lobjBO.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value
                //    == busConstant.BenefitOption100PercentJS);

                if (lBenefit100PercentOPtionAmount.Count() > 0)
                    ldec100JSPercentBenefitAmt = lBenefit100PercentOPtionAmount.FirstOrDefault().icdoBenefitCalculationOptions.benefit_option_amount;

                aobjPerson.idecHigherValueOf50PercentOr100PercentJSAmt = ldec100JSPercentBenefitAmt;
                if (ldec100JSPercentBenefitAmt < ldec50PercentBenefitAmt)
                    aobjPerson.idecHigherValueOf50PercentOr100PercentJSAmt = ldec50PercentBenefitAmt;
            }
        }

        private decimal GetSpousePremiumsForCOBRA(string lstrCoverageCode, busPerson aobjPerson, busPersonAccountGhdv aobjPersonAccountGHDV)
        {
            busPersonAccountGhdv lobjPAGHDV = new busPersonAccountGhdv
            {
                ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv()
            };
            lobjPAGHDV.icdoPersonAccountGhdv = aobjPersonAccountGHDV.icdoPersonAccountGhdv;
            lobjPAGHDV.icdoPersonAccount = aobjPersonAccountGHDV.icdoPersonAccount;
            lobjPAGHDV.ibusPlan = aobjPersonAccountGHDV.ibusPlan;
            lobjPAGHDV.icdoPersonAccountGhdv.cobra_type_value = busConstant.COBRAType36Month;
            lobjPAGHDV.icdoPersonAccountGhdv.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
            lobjPAGHDV.icdoPersonAccountGhdv.coverage_code = lstrCoverageCode;
            lobjPAGHDV.idtPlanEffectiveDate = aobjPerson.icdoPerson.date_of_death;
            lobjPAGHDV.icdoPersonAccount.cobra_expiration_date = aobjPerson.icdoPerson.date_of_death.AddMonths(36);

            lobjPAGHDV.LoadCoverageRefID();
            lobjPAGHDV.GetMonthlyPremiumAmountByRefID();
            return lobjPAGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
        }

        private void GetHealthPremiumAmountBasedOnConditions(busPerson aobjPerson, ref bool lblnErrorFound, ref int lintGHDVHistoryID, ref string lstrGroupNumber)
        {
            int lintPAHealthID = aobjPerson.icolPersonAccount.Where(lobj =>
                //lobj.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                                         lobj.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth).Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault();

            //Loading the RHIC Upfront.. since Retiree Block also uses this bookmark
            //Load the Accured RHIC Benefit Amount
            aobjPerson.idecHealthRHIC = 0.00M;
            aobjPerson.LoadPensionSummary(); //Can be done Better later
            if (aobjPerson.iclbPensionAccounts != null && aobjPerson.iclbPensionAccounts.Count > 0)
                aobjPerson.idecHealthRHIC = aobjPerson.iclbPensionAccounts.First().idecAccruedRHICBenefit;

            if (lintPAHealthID > 0)
            {
                busPersonAccount lobjPA = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

                lobjPA.FindPersonAccount(lintPAHealthID);
                lobjPA.LoadPersonAccountGHDV();

                lobjPA.ibusPersonAccountGHDV.ibusPerson = aobjPerson;
                lobjPA.ibusPersonAccountGHDV.LoadPlan();

                lobjPA.ibusPersonAccountGHDV.idtbCachedCoverageRef = ibusDBCacheData.idtbCachedCoverageRef;
                lobjPA.ibusPersonAccountGHDV.idtbCachedDentalRate = ibusDBCacheData.idtbCachedDentalRate;
                lobjPA.ibusPersonAccountGHDV.idtbCachedHealthRate = ibusDBCacheData.idtbCachedHealthRate;
                lobjPA.ibusPersonAccountGHDV.idtbCachedHmoRate = ibusDBCacheData.idtbCachedHMORate;
                lobjPA.ibusPersonAccountGHDV.idtbCachedRateRef = ibusDBCacheData.idtbCachedRateRef;
                lobjPA.ibusPersonAccountGHDV.idtbCachedRateStructureRef = ibusDBCacheData.idtbCachedRateStructureRef;
                lobjPA.ibusPersonAccountGHDV.idtbCachedVisionRate = ibusDBCacheData.idtbCachedVisionRate;


                busPersonAccountGhdvHistory lobjPAGhdvHistory = lobjPA.ibusPersonAccountGHDV.LoadHistoryByDate(aobjPerson.icdoPerson.date_of_death);
                if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id == 0)
                {
                    idlgUpdateProcessLog(
                        "Error : No History Record Found for Person Account = " +
                        lobjPA.icdoPersonAccount.person_account_id, "INFO", istrProcessName);
                    lblnErrorFound = true;
                }
                else
                {
                    lobjPA.ibusPersonAccountGHDV = lobjPAGhdvHistory.LoadGHDVObject(lobjPA.ibusPersonAccountGHDV);

                    lintGHDVHistoryID = lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id;
                    lstrGroupNumber = lobjPA.ibusPersonAccountGHDV.GetGroupNumber();

                    //Initialize the Org Object to Avoid the NULL error
                    lobjPA.ibusPersonAccountGHDV.InitializeObjects();
                    lobjPA.ibusPersonAccountGHDV.idtPlanEffectiveDate = aobjPerson.icdoPerson.date_of_death;
                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = busConstant.COBRAType36Month;

                    lobjPA.ibusPersonAccountGHDV.LoadActiveProviderOrgPlan(lobjPA.ibusPersonAccountGHDV.idtPlanEffectiveDate);

                    if (lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                    {
                        lobjPA.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                    }
                    else
                    {
                        lobjPA.ibusPersonAccountGHDV.LoadHealthParticipationDate();
                        lobjPA.ibusPersonAccountGHDV.LoadHealthPlanOption();
                        //To Get the Rate Structure Code (Derived Field)
                        lobjPA.ibusPersonAccountGHDV.LoadRateStructure(aobjPerson.icdoPerson.date_of_death);
                    }

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0004";
                    aobjPerson.idecHealthSinglePolicyCOBRAPremium =
                        GetSpousePremiumsForCOBRA(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code, aobjPerson, lobjPA.ibusPersonAccountGHDV);

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0005";
                    aobjPerson.idecHealthFamilyPolicyCOBRAPremium =
                        GetSpousePremiumsForCOBRA(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code, aobjPerson, lobjPA.ibusPersonAccountGHDV);

                    lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = "0026";
                    aobjPerson.idecHealthFamilyOf3PolicyCOBRAPremium =
                        GetSpousePremiumsForCOBRA(lobjPA.ibusPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code, aobjPerson, lobjPA.ibusPersonAccountGHDV);
                }

                aobjPerson.idecHealthSinglePolicyCOBRANetAmt = aobjPerson.idecHealthSinglePolicyCOBRAPremium - aobjPerson.idecHealthRHIC;
                aobjPerson.idecHealthFamilyPolicyCOBRANetAmt = aobjPerson.idecHealthFamilyPolicyCOBRAPremium - aobjPerson.idecHealthRHIC;
                aobjPerson.idecHealthFamilyOf3PolicyCOBRANetAmt = aobjPerson.idecHealthFamilyOf3PolicyCOBRAPremium - aobjPerson.idecHealthRHIC;
            }
        }

        private void RefreshCorrespondencePropertyValues(busPerson aobjPerson)
        {
            aobjPerson.istrRelationshipToBeneficiary = busConstant.Flag_No;
            aobjPerson.istrIsMemberNotBeneficiaryInDBDCPlan = busConstant.Flag_No;
            aobjPerson.istrIsEstateInDBDCPlan = busConstant.Flag_No;
            aobjPerson.istrIsRelationshipTrustee = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEmployeeVested = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEmployeeNonVested = busConstant.Flag_No;
            aobjPerson.istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene = busConstant.Flag_No;
            aobjPerson.istrIsRelationshipSpouse = busConstant.Flag_No;
            aobjPerson.istrIsVestedRelationshipSpouse = busConstant.Flag_No;
            aobjPerson.istrIsBeneficiaryAgeLessThan18 = busConstant.Flag_No;
            aobjPerson.istrIsDCBeneficiarySpouse = busConstant.Flag_No;
            aobjPerson.istrIsMemberHadDCPlan = busConstant.Flag_No;
            //health
            aobjPerson.istrIsBeneficiaryDependentInHealth = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedNotEnrolledNotVestedInHealth = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEnrolledInHealthNoDependents = busConstant.Flag_No;
            aobjPerson.istrIsVestedMarriedBeneDependentInHealth = busConstant.Flag_No;
            aobjPerson.istrIsVestedNotInHealthRelationshipMarried = busConstant.Flag_No;

            //dental
            aobjPerson.istrIsBeneficiaryDependentInDental = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedNotEnrolledNotVestedInDental = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEnrolledInDentalNoDependents = busConstant.Flag_No;
            aobjPerson.istrIsVestedMarriedBeneDependentInDental = busConstant.Flag_No;
            aobjPerson.istrIsVestedNotInDentalRelationshipMarried = busConstant.Flag_No;

            //Vision
            aobjPerson.istrIsBeneficiaryDependentInVision = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedNotEnrolledNotVestedInVision = busConstant.Flag_No;
            aobjPerson.istrIsDeceasedEnrolledInVisionNoDependents = busConstant.Flag_No;
            aobjPerson.istrIsVestedMarriedBeneDependentInVision = busConstant.Flag_No;
            aobjPerson.istrIsVestedNotInVisionRelationshipMarried = busConstant.Flag_No;

            //Life
            aobjPerson.istrIsBeneficiaryInLife = busConstant.Flag_No;
            aobjPerson.istrIsBeneficiaryNotInLife = busConstant.Flag_No;
            aobjPerson.istrIsLifeBeneAgeUnder18 = busConstant.Flag_No;
            aobjPerson.istrIsLifeBeneTrusteeOrEstate = busConstant.Flag_No;
            aobjPerson.istrIsLifeBeneTrustee = busConstant.Flag_No;
            aobjPerson.istrIsLifeBeneEstate = busConstant.Flag_No;

            //LTC
            aobjPerson.istrIsLTCExtistsForBoth = busConstant.Flag_No;
            aobjPerson.istrIsLTCExtistsForNone = busConstant.Flag_No;
            aobjPerson.istrIsLTCExtistsForDeceased = busConstant.Flag_No;
            aobjPerson.istrIsLTCExtistsForSpouse = busConstant.Flag_No;

            //Def comp  
            aobjPerson.istrIsdefCompPlanExits = busConstant.Flag_No;
            aobjPerson.istrIsdefCompPlanNotExits = busConstant.Flag_No;

            //Other 457 
            aobjPerson.istrIsOther457PlanExits = busConstant.Flag_No;
            aobjPerson.istrIsOther457PlanNotExits = busConstant.Flag_No;

            //DC
            aobjPerson.istrIsDCPlanExits = busConstant.Flag_No;

            aobjPerson.istrIsMemberInHealthDependentsEligible = busConstant.Flag_No;
            aobjPerson.istrIsMemberInDentalDependentsEligible = busConstant.Flag_No;
            aobjPerson.istrIsMemberInVisionDependentsEligible = busConstant.Flag_No;
            aobjPerson.istrIsDCNonVestedOrSingleDeathOrDCVestedAndMarriedDeath = busConstant.Flag_No;

            //Amounts
            aobjPerson.idecDeceasedMemberBalanceAmount = 0.00M;
            aobjPerson.idecTaxableMemberBalanceAmount = 0.00M;
            aobjPerson.idecNonTaxableMemberBalanceAmount = 0.00M;
            aobjPerson.idec20PercentOfTaxableAmount = 0.00M;
            aobjPerson.idecHigherValueOf50PercentOr100PercentJSAmt = 0.00M;

            aobjPerson.idecHealthSinglePolicyCOBRAPremium = 0.00M;
            aobjPerson.idecHealthFamilyPolicyCOBRAPremium = 0.00M;
            aobjPerson.idecHealthFamilyOf3PolicyCOBRAPremium = 0.00M;

            aobjPerson.idecHealthSinglePolicyCOBRANetAmt = 0.00M;

            aobjPerson.idecHealthFamilyPolicyCOBRANetAmt = 0.00M;
            aobjPerson.idecHealthFamilyOf3PolicyCOBRANetAmt = 0.00M;

            aobjPerson.idecHealthRHIC = 0.00M;
            aobjPerson.idecHealthSinglePolicyNetAmount = 0.00M;
            aobjPerson.idecHealthFamilyPolicyNetAmount = 0.00M;
            aobjPerson.idecHealthFamilyOf3PolicyNetAmount = 0.00M;

            aobjPerson.idecHealthSinglePolicyPremium = 0.00M;
            aobjPerson.idecHealthFamilyPolicyPremium = 0.00M;
            aobjPerson.idecHealthFamily3PolicyPremium = 0.00M;

            aobjPerson.idecDentalSinglePolicyCOBRAPremium = 0.00M;
            aobjPerson.idecDentalFamilyPolicyCOBRAPremium = 0.00M;

            aobjPerson.idecDentalSinglePolicyPremium = 0.00M;
            aobjPerson.idecDentalFamilyPolicyPremium = 0.00M;

            aobjPerson.idecVisionSinglePolicyCOBRAPremium = 0.00M;
            aobjPerson.idecVisionFamilyPolicyCOBRAPremium = 0.00M;

            aobjPerson.idecVisionSinglePolicyPremium = 0.00M;
            aobjPerson.idecVisionFamilyPolicyPremium = 0.00M;
            //public decimal idecSinglePolicyPremium { get; set; }
            //public decimal idecFamilyPolicyPremium { get; set; }
            aobjPerson.idecLifeInsurancePolicyValue = 0.00M;
            aobjPerson.idecFamilyOfThreePolicyPremium = 0.00M;

            aobjPerson.istrisHealthVisible = busConstant.Flag_No;
            aobjPerson.istrisDentalVisible = busConstant.Flag_No;
            aobjPerson.istrisVisionVisible = busConstant.Flag_No;

            aobjPerson.istrIsSpouseOrDependentInVision = busConstant.Flag_No;
            aobjPerson.istrIsSpouseOrDependentInDental = busConstant.Flag_No;
            aobjPerson.istrIsSpouseOrDependentInHealth = busConstant.Flag_No;

        }
    }
}
