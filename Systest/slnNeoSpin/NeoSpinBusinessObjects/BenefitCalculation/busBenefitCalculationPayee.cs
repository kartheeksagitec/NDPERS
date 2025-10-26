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
    [Serializable]
    public class busBenefitCalculationPayee : busBenefitCalculationPayeeGen
    {
        private Collection<busBenefitCalculationOptions> _iclbBenefitCalculationOptions;
        public Collection<busBenefitCalculationOptions> iclbBenefitCalculationOptions
        {
            get { return _iclbBenefitCalculationOptions; }
            set { _iclbBenefitCalculationOptions = value; }
        }

        // To Delete the Benefit Calculation Payee
        public void LoadBenefitCalculationOptions()
        {
            if (_iclbBenefitCalculationOptions == null)
                _iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
            DataTable ldtbResult = Select<cdoBenefitCalculationOptions>(new string[1] { "BENEFIT_CALCULATION_PAYEE_ID" },
                                                                    new object[1] { icdoBenefitCalculationPayee.benefit_calculation_payee_id }, null, null);
            _iclbBenefitCalculationOptions = GetCollection<busBenefitCalculationOptions>(ldtbResult, "icdoBenefitCalculationOptions");
        }

        public void LoadBenefitApplication()
        {
            if (ibusBenefitApplication == null)
            {
                ibusBenefitApplication = new busBenefitApplication();
                ibusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
            }
            if (icdoBenefitCalculationPayee.benefit_application_id > 0)
            {
                ibusBenefitApplication.icdoBenefitApplication.SelectRow(new object[1] { icdoBenefitCalculationPayee.benefit_application_id });
            }
        }

        public bool IsMember
        {
            get
            {
                if (ibusBenefitCalculation == null)
                    LoadBenefitCalculation();
                if (icdoBenefitCalculationPayee.payee_person_id == ibusBenefitCalculation.icdoBenefitCalculation.person_id)
                    return true;
                else
                    return false;
            }
        }

        public bool IsSpouseValid()
        {
            bool lblnIsActiveSpouseExists = true;
            if ((icdoBenefitCalculationPayee.payee_person_id != 0) &&
                (icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse))
            {
                if (ibusBenefitCalculation.ibusMember == null)
                    ibusBenefitCalculation.LoadMember();
                if (ibusBenefitCalculation.ibusMember.icolPersonContact == null)
                    ibusBenefitCalculation.ibusMember.LoadContacts();
                foreach (busPersonContact lobjPersonContact in ibusBenefitCalculation.ibusMember.icolPersonContact)
                {
                    if ((lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                            && (lobjPersonContact.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive)
                            && (icdoBenefitCalculationPayee.payee_person_id == lobjPersonContact.icdoPersonContact.contact_person_id))
                    {
                        lblnIsActiveSpouseExists = true;
                        break;
                    }
                    else
                    {
                        lblnIsActiveSpouseExists = false;
                    }
                }
                if (ibusBenefitCalculation.ibusMember.icolPersonContact.Count == 0)
                    lblnIsActiveSpouseExists = false;
            }
            return lblnIsActiveSpouseExists;
        }

        public bool IsSpouseAlreadyExists()
        {
            if (icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse)
            {
                // The Record is not Editable, So Update mode check is not done.
                DataTable ldtbResult = Select<cdoBenefitCalculationPayee>(new string[2] { "BENEFIT_CALCULATION_ID", "FAMILY_RELATIONSHIP_VALUE" },
                                        new object[2] { icdoBenefitCalculationPayee.benefit_calculation_id, busConstant.FamilyRelationshipSpouse }, null, null);
                if (ldtbResult.Rows.Count > 0)
                    return true;
            }
            return false;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (ibusBenefitCalculation == null)
                LoadBenefitCalculation();
            icdoBenefitCalculationPayee.benefit_calculation_id = ibusBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id;
            if (icdoBenefitCalculationPayee.payee_person_id != 0)
            {
                if (icdoBenefitCalculationPayee.payee_person_id != ibusBenefitCalculation.icdoBenefitCalculation.person_id)
                {
                    if (ibusPayee == null)
                        LoadPayee();
                    icdoBenefitCalculationPayee.payee_first_name = ibusPayee.icdoPerson.first_name;
                    icdoBenefitCalculationPayee.payee_middle_name = ibusPayee.icdoPerson.middle_name;
                    icdoBenefitCalculationPayee.payee_last_name = ibusPayee.icdoPerson.last_name;
                    icdoBenefitCalculationPayee.payee_date_of_birth = ibusPayee.icdoPerson.date_of_birth;
                }
            }
            base.BeforeValidate(aenmPageMode);
        }

        public void LoadPayeeApplicationDetails()
        {
            if (ibusBenefitCalculation == null)
                LoadBenefitCalculation();
            DataTable ldtbResult = new DataTable();
            ldtbResult = LoadApplicationDetailsForRecipient(icdoBenefitCalculationPayee.payee_org_id, icdoBenefitCalculationPayee.payee_person_id,
                        ibusBenefitCalculation.icdoBenefitCalculation.person_id, ibusBenefitCalculation.icdoBenefitCalculation.plan_id,
                        ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value);
            if (ldtbResult.Rows.Count > 0)
            {
                ibusBenefitApplication = new busBenefitApplication();
                ibusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication();
                ibusBenefitApplication.icdoBenefitApplication.LoadData(ldtbResult.Rows[0]);
                icdoBenefitCalculationPayee.benefit_application_id = ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;
                icdoBenefitCalculationPayee.payee_benefit_option = ibusBenefitApplication.icdoBenefitApplication.benefit_option_value;
            }
            else
                icdoBenefitCalculationPayee.payee_benefit_option = busConstant.Refund;
        }

        public DataTable LoadApplicationDetailsForRecipient(int aintPayeeOrgid, int aintPayeePersonID, int aintMemberPersonid, int aintPlanId, string astrBenefitAccountType)
        {
            DataTable ldtbResult = new DataTable();
            if (aintPayeeOrgid != 0)
            {
                ldtbResult = SelectWithOperator<cdoBenefitApplication>(
                    new string[5] { "MEMBER_PERSON_ID", "PLAN_ID", "PAYEE_ORG_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "ACTION_STATUS_VALUE" },
                    new string[5] { "=", "=", "=", "=", "<>" },
                    new object[5] { aintMemberPersonid ,
                                    aintPlanId ,
                                    aintPayeeOrgid,
                                    astrBenefitAccountType,
                                    busConstant.CalculationStatusCancel }, null);
            }
            else if (aintPayeePersonID != 0)
            {
                ldtbResult = SelectWithOperator<cdoBenefitApplication>(
                    new string[5] { "MEMBER_PERSON_ID", "PLAN_ID", "RECIPIENT_PERSON_ID", "BENEFIT_ACCOUNT_TYPE_VALUE", "ACTION_STATUS_VALUE" },
                    new string[5] { "=", "=", "=", "=", "<>" },
                    new object[5] { aintMemberPersonid ,
                                    aintPlanId,
                                    aintPayeePersonID,
                                    astrBenefitAccountType ,
                                    busConstant.CalculationStatusCancel }, null);
            }

            return ldtbResult;
        }

        public override void BeforePersistChanges()
        {
            if ((!string.IsNullOrEmpty(istrPayeeOrgCode)) && (icdoBenefitCalculationPayee.payee_org_id == 0))
                icdoBenefitCalculationPayee.payee_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrPayeeOrgCode);
            base.BeforePersistChanges();
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            if (icdoBenefitCalculationPayee.payee_person_id != 0)
                LoadPayee();
            if (!string.IsNullOrEmpty(istrPayeeOrgCode))
            {
                icdoBenefitCalculationPayee.payee_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrPayeeOrgCode);
                LoadPayeeOrg();
            }
            larrList.Add(this);
            return larrList;
        }

        public void LoadPayeeNames()
        {
            if (icdoBenefitCalculationPayee.payee_person_id != 0)
                LoadPayee();
            if (icdoBenefitCalculationPayee.payee_org_id != 0)
                LoadPayeeOrg();
        }
    }
}
