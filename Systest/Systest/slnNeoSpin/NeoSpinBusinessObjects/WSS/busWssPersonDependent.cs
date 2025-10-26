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

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busWssPersonDependent:
	/// Inherited from busWssPersonDependentGen, the class is used to customize the business object busWssPersonDependentGen.
	/// </summary>
	[Serializable]
	public class busWssPersonDependent : busWssPersonDependentGen
	{
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            iblnFromPortal = true;
            if (icdoPersonDependent.IsNull())
            {
                if (icdoWssPersonDependent.target_person_dependent_id > 0)
                {
                    FindPersonDependent(icdoWssPersonDependent.target_person_dependent_id);
                    LoadPersonAccountDependentUpdate();
                }
                else
                {
                    icdoPersonDependent = new cdoPersonDependent { person_id = icdoWssPersonDependent.iintPersonID, iblnNewResult = icdoWssPersonDependent.iblnResult,
                                                                   iintPlanId = icdoWssPersonDependent.iintPlanId,
                                                                   start_date = icdoWssPersonDependent.effective_start_date
                    };  //PIR 11841                  
                    LoadPersonAccountDependentNew();
                }
            }
            else if (icdoPersonDependent.person_dependent_id > 0)
                LoadPersonAccountDependentUpdate();

            if (ibusPerson.IsNull()) LoadPerson();
            if (!string.IsNullOrEmpty(icdoWssPersonDependent.ssn))
                icdoPersonDependent.dependent_perslink_id = busGlobalFunctions.GetPersonIDBySSN(icdoWssPersonDependent.ssn);
            icdoPersonDependent.marital_status_value = icdoWssPersonDependent.marital_status_value;
            icdoPersonDependent.gender_value = icdoWssPersonDependent.gender_value;
            icdoPersonDependent.relationship_value = icdoWssPersonDependent.relationship_value;
            icdoPersonDependent.mss_person_dependent_id = icdoWssPersonDependent.target_person_dependent_id;
            icdoPersonDependent.medicare_part_a_effective_date = icdoWssPersonDependent.medicare_part_a_effective_date;
            icdoPersonDependent.medicare_part_b_effective_date = icdoWssPersonDependent.medicare_part_b_effective_date;
            icdoPersonDependent.iintPlanId = icdoWssPersonDependent.iintPlanId; //PIR 11841
            base.BeforeValidate(aenmPageMode);
        }

        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            foreach (utlError lobjError in iarrErrors)
                lobjError.istrErrorID = string.Empty;
        }

        public busWssPersonAccountEnrollmentRequest ibusEnrollmentRequest { get; set; }

        public void LoadEnrollmentRequest()
        {
            if (ibusEnrollmentRequest.IsNull())
                ibusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest { icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest() };
            ibusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(icdoWssPersonDependent.wss_person_account_enrollment_request_id);
        }

        public bool IsValidRelationshipPerCoverage()
        {
            if (icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PersonAccountElectionValueEnrolled)
            {
                if (icdoWssPersonDependent.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualChild ||
                    icdoWssPersonDependent.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualChild)
                {
                    if (!IsChildDependent())
                        return false;
                }
                if (icdoWssPersonDependent.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse ||
                    icdoWssPersonDependent.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse)
                {
                    if (icdoPersonDependent.relationship_value != busConstant.DependentRelationshipSpouse)
                        return false;
                }
                if (icdoWssPersonDependent.level_of_coverage_value == busConstant.DentalLevelofCoverageFamily ||
                    icdoWssPersonDependent.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily)
                {
                    if (icdoPersonDependent.relationship_value != busConstant.DependentRelationshipSpouse && !IsChildDependent())
                        return false;
                }
            }
            return true;
        }

        //PIR 11905
        public bool IsDependentAgeGreaterThan26()
        {
            if (icdoWssPersonDependent.relationship_value != null )
            {
                if (icdoWssPersonDependent.relationship_value != busConstant.DependentRelationshipDisabledChild &&
                    icdoWssPersonDependent.relationship_value != busConstant.DependentRelationshipExSpouse &&
                    icdoWssPersonDependent.relationship_value != busConstant.DependentRelationshipSpouse)
                {
                    icdoWssPersonDependent.iintDependentAge = busGlobalFunctions.CalulateAge(icdoWssPersonDependent.date_of_birth,
                                                                                                   icdoWssPersonDependent.idtDateOfChange);

                    if (icdoWssPersonDependent.iintDependentAge >= 26)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        // PIR 10695 - for 'Finish later' status
        public bool IsChildDependentWSS()
        {
            if (icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild ||
                icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipChild ||
                icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild ||
                icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild ||
                icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipLegalGuardian ||
                icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipStepChild)
                return true;
            return false;
        }



        //Backlog PIR 13569
        public bool IsExSpouseSelectedForEnrollingInGHDV()
        {
            if (icdoWssPersonDependent.relationship_value != null && icdoWssPersonDependent.current_plan_enrollment_option_value != null)
            {
                if (icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipExSpouse 
                    && icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueEnrolled )
                {                 
                   return true;
                    
                }
            }
            return false;
        }
		
        // PIR - 17572
        public bool IsDuplicatePerson()
        {
            busPerson lbusDuplicatePerson = new busPerson();
            lbusDuplicatePerson = lbusDuplicatePerson.LoadPersonBySsn(icdoWssPersonDependent.ssn);
            return (lbusDuplicatePerson.IsNotNull()
			        && (lbusDuplicatePerson.icdoPerson.date_of_birth != icdoWssPersonDependent.date_of_birth
                    || lbusDuplicatePerson.icdoPerson.gender_value != icdoWssPersonDependent.gender_value));
        }
		
        public bool IsInvalidSSN()
        {
            busPerson lobjInvalidSSN = new busPerson();
            return (lobjInvalidSSN.LoadInvalidSSN(icdoWssPersonDependent.ssn));
        }
		//F/W Upgrade PIR 11927 - New Depedent Issue Fix - MSS
        public void LoadPersonDependentProperties()
        {
            if (icdoPersonDependent.IsNull())
            {
                if (icdoWssPersonDependent.target_person_dependent_id > 0)
                {
                    FindPersonDependent(icdoWssPersonDependent.target_person_dependent_id);
                }
                else
                {
                    icdoPersonDependent = new cdoPersonDependent
                    {
                        person_id = icdoWssPersonDependent.iintPersonID,
                        iblnNewResult = icdoWssPersonDependent.iblnResult,
                        iintPlanId = icdoWssPersonDependent.iintPlanId,
                        start_date = icdoWssPersonDependent.effective_start_date
                    };                
                }
            }
            if (ibusPerson.IsNull()) LoadPerson();
            if (!string.IsNullOrEmpty(icdoWssPersonDependent.ssn))
                icdoPersonDependent.dependent_perslink_id = busGlobalFunctions.GetPersonIDBySSN(icdoWssPersonDependent.ssn);
            icdoPersonDependent.marital_status_value = icdoWssPersonDependent.marital_status_value;
            icdoPersonDependent.gender_value = icdoWssPersonDependent.gender_value;
            icdoPersonDependent.relationship_value = icdoWssPersonDependent.relationship_value;
            icdoPersonDependent.mss_person_dependent_id = icdoWssPersonDependent.target_person_dependent_id;
            icdoPersonDependent.medicare_part_a_effective_date = icdoWssPersonDependent.medicare_part_a_effective_date;
            icdoPersonDependent.medicare_part_b_effective_date = icdoWssPersonDependent.medicare_part_b_effective_date;
            icdoPersonDependent.iintPlanId = icdoWssPersonDependent.iintPlanId;
        }
    }
}
