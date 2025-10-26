#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitApplicationGen : busPersonBase
    {
        public busBenefitApplicationGen()
        {

        }

        private cdoBenefitApplication _icdoBenefitApplication;
        public cdoBenefitApplication icdoBenefitApplication
        {
            get
            {
                return _icdoBenefitApplication;
            }
            set
            {
                _icdoBenefitApplication = value;
            }
        }

        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get
            {
                return _ibusPersonAccount;
            }
            set
            {
                _ibusPersonAccount = value;
            }
        }

        private busPerson _ibusJointAnniutantPerson;
        public busPerson ibusJointAnniutantPerson
        {
            get
            {
                return _ibusJointAnniutantPerson;
            }
            set
            {
                _ibusJointAnniutantPerson = value;
            }
        }

        private DateTime _idtRetirementDateBasedOnEligibility;
        public DateTime idtRetirementDateBasedOnEligibility
        {
            get { return _idtRetirementDateBasedOnEligibility; }
            set { _idtRetirementDateBasedOnEligibility = value; }
        }


        //this property is used to set bool value for purpose of validation
        private bool _iblnRetirementWithProcessedStatusExistsForDisability;
        public bool iblnRetirementWithProcessedStatusExistsForDisability
        {
            get { return _iblnRetirementWithProcessedStatusExistsForDisability; }
            set { _iblnRetirementWithProcessedStatusExistsForDisability = value; }
        }

        //this property is used to set bool value for purpose of validation
        private bool _iblnDisabilityWithRvwOrVldStatusExistsForRetirement;
        public bool iblnDisabilityWithRvwOrVldStatusExistsForRetirement
        {
            get { return _iblnDisabilityWithRvwOrVldStatusExistsForRetirement; }
            set { _iblnDisabilityWithRvwOrVldStatusExistsForRetirement = value; }
        }

        //this Property returns the member s age based on the retirement date
        private decimal _idecMemberAgeBasedOnRetirementDate;
        public decimal idecMemberAgeBasedOnRetirementDate
        {
            get
            {
                return _idecMemberAgeBasedOnRetirementDate;
            }
            set
            {
                _idecMemberAgeBasedOnRetirementDate = value;
            }
        }

        //age at NRD
        public decimal idecMemberAgeAtNRD { get; set; }

        //this Property returns the member s age based on the given date
        private decimal _idecMemberAgeBasedOnGivenDate;
        public decimal idecMemberAgeBasedOnGivenDate
        {
            get
            {
                return _idecMemberAgeBasedOnGivenDate;
            }
            set
            {
                _idecMemberAgeBasedOnGivenDate = value;
            }
        }

        //this property is used to display the member's age in years and months
        public string _istrMemberAgeBasedOnRetirementDateFormatted;
        public string istrMemberAgeBasedOnRetirementDateFormatted
        {
            get
            {
                return _istrMemberAgeBasedOnRetirementDateFormatted;
            }
        }

        private Collection<busBenefitApplicationPersonAccount> _iclbBenefitApplicationPersonAccounts;

        public Collection<busBenefitApplicationPersonAccount> iclbBenefitApplicationPersonAccounts
        {
            get { return _iclbBenefitApplicationPersonAccounts; }
            set { _iclbBenefitApplicationPersonAccounts = value; }
        }

        private busPersonEmploymentDetail _ibusPersonEmploymentDtl;
        public busPersonEmploymentDetail ibusPersonEmploymentDtl
        {
            get { return _ibusPersonEmploymentDtl; }
            set { _ibusPersonEmploymentDtl = value; }
        }

        private cdoCodeValue _icdoVestingEligibilityData;
        public cdoCodeValue icdoVestingEligibilityData
        {
            get { return _icdoVestingEligibilityData; }
            set { _icdoVestingEligibilityData = value; }
        }

        private Collection<busCodeValue> _iclbNormalEligibilityData;
        public Collection<busCodeValue> iclbNormalEligibilityData
        {
            get { return _iclbNormalEligibilityData; }
            set { _iclbNormalEligibilityData = value; }
        }

        private Collection<busCodeValue> _iclbEarlyEligibilityData;
        public Collection<busCodeValue> iclbEarlyEligibilityData
        {
            get { return _iclbEarlyEligibilityData; }
            set { _iclbEarlyEligibilityData = value; }
        }

        private Collection<cdoBenAppOtherDisBenefit> _iclcBenAppOtherDisBenefit;
        public Collection<cdoBenAppOtherDisBenefit> iclcBenAppOtherDisBenefit
        {
            get { return _iclcBenAppOtherDisBenefit; }
            set { _iclcBenAppOtherDisBenefit = value; }
        }

        private string _istrJointAnnuitantRelationship;
        public string istrJointAnnuitantRelationship
        {
            get { return _istrJointAnnuitantRelationship; }
            set { _istrJointAnnuitantRelationship = value; }
        }

        //this int variable is used to check if the retirement date is equal or not equal to normal or early eligible date.
        private int _iintIsRetirementDateNotEqualToNormalOREalryEligibleDate;
        public int iintIsRetirementDateNotEqualToNormalOREalryEligibleDate
        {
            get
            {
                return _iintIsRetirementDateNotEqualToNormalOREalryEligibleDate;
            }
            set
            {
                _iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = value;
            }
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }

        private busPerson _ibusRecipient;
        public busPerson ibusRecipient
        {
            get { return _ibusRecipient; }
            set { _ibusRecipient = value; }
        }

        private Collection<busPayeeAccount> _iclbPayeeAccount;
        public Collection<busPayeeAccount> iclbPayeeAccount
        {
            get { return _iclbPayeeAccount; }
            set { _iclbPayeeAccount = value; }
        }

        private Collection<busPayeeAccount> _iclbPayeeAccountByPersonID;
        public Collection<busPayeeAccount> iclbPayeeAccountByPersonID
        {
            get { return _iclbPayeeAccountByPersonID; }
            set { _iclbPayeeAccountByPersonID = value; }
        }

        private decimal _idecChildAgeAsOnDOD;
        public decimal idecChildAgeAsOnDOD
        {
            get { return _idecChildAgeAsOnDOD; }
            set { _idecChildAgeAsOnDOD = value; }
        }
        private string _istrChildMaritalStatus;
        public string istrChildMaritalStatus
        {
            get { return _istrChildMaritalStatus; }
            set { _istrChildMaritalStatus = value; }
        }
        private bool _isChildDeceased;
        public bool isChildDeceased
        {
            get { return _isChildDeceased; }
            set { _isChildDeceased = value; }
        }

        private busOrganization _ibusApplicantOrganization;
        public busOrganization ibusApplicantOrganization
        {
            get { return _ibusApplicantOrganization; }
            set { _ibusApplicantOrganization = value; }
        }

        /* //Maik Mail - Strange issue, if for some reason, benefit application id is 0, 
         * this method was loading all QDRO payee accounts, so added condition 
         * icdoBenefitApplication.benefit_application_id > 0 */
        public void LoadPayeeAccount()
        {
            DataTable ldtbResult = new DataTable();
            if (icdoBenefitApplication.benefit_application_id > 0)
            {
                ldtbResult = Select<cdoPayeeAccount>(
                                          new string[1] { "application_id" },
                                          new object[1] { icdoBenefitApplication.benefit_application_id }, null, null);
            }
            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtbResult, "icdoPayeeAccount");
        }
        public bool CheckActivePayeeAccountExists()
        {
            if (iclbPayeeAccount.IsNull())
                LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                lobjPayeeAccount.LoadActivePayeeStatus();
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus != null)
                    if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value != busConstant.PayeeAccountStatusRetirmentCancelled)
                    {
                        return true;
                    }
            }
            return false;
        }
        public void LoadBenefitApplicationPersonAccount()
        {
            DataTable ldtbResult = Select<cdoBenefitApplicationPersonAccount>(
                                      new string[1] { "benefit_application_id" },
                                      new object[1] { icdoBenefitApplication.benefit_application_id }, null, null);
            iclbBenefitApplicationPersonAccounts = GetCollection<busBenefitApplicationPersonAccount>(ldtbResult, "icdoBenefitApplicationPersonAccount");
        }
        private Collection<busBenefitApplicationDbTffrTransfer> _iclbBenefitApplicationDbTffrTransfer;

        public Collection<busBenefitApplicationDbTffrTransfer> iclbBenefitApplicationDbTffrTransfer
        {
            get { return _iclbBenefitApplicationDbTffrTransfer; }
            set { _iclbBenefitApplicationDbTffrTransfer = value; }
        }
        public void LoadBenefitApplicationDbTffrTransfer()
        {
            DataTable ldtbResult = Select<cdoBenefitApplicationDbTffrTransfer>(
                                     new string[1] { "benefit_application_id" },
                                     new object[1] { icdoBenefitApplication.benefit_application_id }, null, null);
            iclbBenefitApplicationDbTffrTransfer = GetCollection<busBenefitApplicationDbTffrTransfer>(ldtbResult, "icdoBenefitApplicationDbTffrTransfer");
        }
        public bool FindBenefitApplication(int Aintbenefitapplicationid)
        {
            bool lblnResult = false;
            if (_icdoBenefitApplication == null)
            {
                _icdoBenefitApplication = new cdoBenefitApplication();
            }
            if (_icdoBenefitApplication.SelectRow(new object[1] { Aintbenefitapplicationid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        //Load Member for Pre retirement death
        public void LoadRecipient()
        {
            if (_ibusRecipient == null)
            {
                _ibusRecipient = new busPerson();
            }
            _ibusRecipient.FindPerson(icdoBenefitApplication.recipient_person_id);
        }

        //Load Member 
        public void LoadPerson()
        {
            if (ibusPerson == null)
            {
                ibusPerson = new busPerson();
            }
            ibusPerson.FindPerson(icdoBenefitApplication.member_person_id);
        }

        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }

            if (ibusPerson.IsNull())
                LoadPerson();

            _ibusPersonAccount = ibusPerson.LoadActivePersonAccountByPlan(icdoBenefitApplication.plan_id);
        }
        //prop to load retirement person account information
        public busPersonAccountRetirement ibusPersonAccountRetirement { get; set; }
        public void LoadRetirementPersonAccount()
        {
            if (ibusPersonAccountRetirement == null)
            {
                ibusPersonAccountRetirement = new busPersonAccountRetirement();
            }

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
        }
        public void LoadPlan()
        {
            if (ibusPlan == null)
            {
                ibusPlan = new busPlan();
            }
            ibusPlan.FindPlan(icdoBenefitApplication.plan_id);
        }

        public void LoadJointAnniutantPerson()
        {
            //the check for PERSlink id is done so that the object get refreshed when user empty the Joint annuitant PERSLink ID
            if ((_ibusJointAnniutantPerson == null) || (icdoBenefitApplication.joint_annuitant_perslink_id == 0))
            {
                _ibusJointAnniutantPerson = new busPerson();
            }
            _ibusJointAnniutantPerson.FindPerson(icdoBenefitApplication.joint_annuitant_perslink_id);
        }

        public void LoadApplicantOrganization()
        {
            if (ibusApplicantOrganization == null)
            {
                ibusApplicantOrganization = new busOrganization();
            }
            ibusApplicantOrganization.FindOrganization(icdoBenefitApplication.payee_org_id);
        }
        public Collection<busBenefitApplicationPersonAccount> iclbRTWPersonAccounts { get; set; }

        //this original Payee account will be used for benefit account type value as Post retirement Payee death
        public busPayeeAccount ibusOriginatingPayeeAccount { get; set; }

        public void LoadOriginatingPayeeAccount()
        {
            if (ibusOriginatingPayeeAccount.IsNull())
                ibusOriginatingPayeeAccount = new busPayeeAccount();
            ibusOriginatingPayeeAccount.FindPayeeAccount(icdoBenefitApplication.originating_payee_account_id);
        }
    }
}