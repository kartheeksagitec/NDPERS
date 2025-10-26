using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpinBatch
{
    public class busPayeeDeathLetterBatch : busNeoSpinBatch
    {
        //public Collection<busDeathNotification> iclbDeathNotification { get; set; }

        //public Collection<busPersonBeneficiary> iclbPersonBeneficiary { get; set; }

        public void CreatePayeeDeathLetter()
        {
            istrProcessName = "Payee Death Notification Letter";

            Collection<busDeathNotification> lclbDeathNotification = new Collection<busDeathNotification>();
            Collection<busPersonBeneficiary>  lclbPersonBeneficiary = new Collection<busPersonBeneficiary>();
            busDeathNotification lbusDeathNotification = new busDeathNotification() { icdoDeathNotification = new cdoDeathNotification() };

            DataTable ldtbPayeeDeathACOD = busBase.Select("cdoDeathNotification.PayeeDeathLetterACOD", new object[] { });
            if (ldtbPayeeDeathACOD.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Generating Payee Death Letter For Account Owner Beneficiaries", "INFO", istrProcessName);
                lbusDeathNotification.CreateLetter(ldtbPayeeDeathACOD, busConstant.PostRetirementAccountOwnerDeath, ref lclbPersonBeneficiary, ref lclbDeathNotification);
            }

            DataTable ldtbPayeeDeathFBED = busBase.Select("cdoDeathNotification.PayeeDeathLetterFBED", new object[] { });
            if (ldtbPayeeDeathFBED.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Generating Payee Death Letter For First Beneficiary Death Beneficiaries", "INFO", istrProcessName);
                lbusDeathNotification.CreateLetter(ldtbPayeeDeathFBED, string.Empty, ref lclbPersonBeneficiary, ref lclbDeathNotification);
            }

            DataTable ldtbPayeeDeathALPD = busBase.Select("cdoDeathNotification.PayeeDeathLetterALPD", new object[] { });
            if (ldtbPayeeDeathALPD.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Generating Payee Death Letter For Alternate Payee Death Beneficiaries", "INFO", istrProcessName);
                lbusDeathNotification.CreateLetter(ldtbPayeeDeathALPD, string.Empty, ref lclbPersonBeneficiary, ref lclbDeathNotification);
            }        

            DataTable ldtbPayeeDeathInsurance = busBase.Select("cdoDeathNotification.PayeeDeathLetterInsurance", new object[] { });
            if (ldtbPayeeDeathInsurance.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Generating Payee Death Letter For Insurance Beneficiaries", "INFO", istrProcessName);
                lbusDeathNotification.CreateLetter(ldtbPayeeDeathInsurance, "INSU", ref lclbPersonBeneficiary, ref lclbDeathNotification);
            }
            // Commented Since beneficiaries cannot be explicitly set for Deferred Comp or 457 Pln
            // idlgUpdateProcessLog("Generating Payee Death Letter For DeferredComp Beneficiaries", "INFO", istrProcessName);
            // DataTable ldtbPayeeDeathDeffComp = busBase.Select("cdoDeathNotification.PayeeDeathLetterDeffComp", new object[] { });
            // CreateLetter(ldtbPayeeDeathDeffComp);
           
            // Update the Payee Death Sent flag
            if (lclbDeathNotification.Count > 0)
            {
                foreach (busDeathNotification lobjDeathNotification in lclbDeathNotification)
                {
                    lobjDeathNotification.icdoDeathNotification.is_payee_death_letter_sent_flag = busConstant.Flag_Yes;
                    lobjDeathNotification.icdoDeathNotification.Update();
                }
            }
            else
            {
                idlgUpdateProcessLog("No Records found", "INFO", istrProcessName);
            }
        }

        //public void CreateLetter(DataTable adtbResults, string astrGroupType)
        //{
        //    int lintLastDeceasedPersonID = 0;            
        //    foreach (DataRow ldtr in adtbResults.Rows)
        //    {
        //        busDeathNotification lobjDeathNotification = new busDeathNotification
        //        {
        //            icdoDeathNotification = new cdoDeathNotification(),
        //            ibusPerson = new busPerson { icdoPerson = new cdoPerson(), iclbActiveBeneForGivenPlan = new Collection<busPersonBeneficiary>() },
        //            ibusDeceasedPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() },
        //            ibusPersonToBeneficiary = new busPersonBeneficiary
        //            {
        //                ibusPersonAccountBeneficiary = new busPersonAccountBeneficiary
        //                {
        //                    icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary()
        //                },
        //                icdoPersonBeneficiary = new cdoPersonBeneficiary(),
        //                ibusBeneficiaryPerson = new busPerson { icdoPerson = new cdoPerson() },
        //                ibusBeneficiaryOrganization = new busOrganization { icdoOrganization = new cdoOrganization() }
        //            }
        //        };

        //        lobjDeathNotification.icdoDeathNotification.LoadData(ldtr);
        //        lobjDeathNotification.ibusPerson.icdoPerson.LoadData(ldtr);
        //        lobjDeathNotification.ibusDeceasedPayeeAccount.icdoPayeeAccount.LoadData(ldtr);
        //        lobjDeathNotification.ibusPersonToBeneficiary.icdoPersonBeneficiary.LoadData(ldtr);
        //        lobjDeathNotification.ibusPersonToBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(ldtr);
        //        lobjDeathNotification.ibusPersonToBeneficiary.ibusBeneficiaryOrganization.icdoOrganization.LoadData(ldtr);

        //        if (((lobjDeathNotification.ibusPersonToBeneficiary.icdoPersonBeneficiary.beneficiary_person_id > 0) &&
        //             (!iclbPersonBeneficiary.Where(lobj =>
        //                    lobj.icdoPersonBeneficiary.beneficiary_person_id == lobjDeathNotification.ibusPersonToBeneficiary.icdoPersonBeneficiary.beneficiary_person_id &&
        //                    lobj.icdoPersonBeneficiary.person_id == lobjDeathNotification.icdoDeathNotification.person_id).Any())) ||
        //            ((lobjDeathNotification.ibusPersonToBeneficiary.icdoPersonBeneficiary.benificiary_org_id > 0) &&
        //             (!iclbPersonBeneficiary.Where(lobj =>
        //                    lobj.icdoPersonBeneficiary.benificiary_org_id == lobjDeathNotification.ibusPersonToBeneficiary.icdoPersonBeneficiary.benificiary_org_id &&
        //                    lobj.icdoPersonBeneficiary.person_id == lobjDeathNotification.icdoDeathNotification.person_id).Any()))) // SYS PIR ID 2619
        //        {
        //            lobjDeathNotification.istrFromBatch = busConstant.Flag_Yes;
        //            lobjDeathNotification.IsPayeeDeathLetter = true;
        //            lobjDeathNotification.ibusPersonToBeneficiary.LoadBeneficiaryInfo();
        //            lobjDeathNotification.ibusPersonToBeneficiary.LoadBeneficiaryAddress(true);
        //            lobjDeathNotification.ibusPersonToBeneficiary.LoadBeneficiaryPerson();
        //            lobjDeathNotification.ibusDeceasedPayeeAccount.LoadPayeePerson();
        //            lobjDeathNotification.ibusDeceasedPayeeAccount.LoadBenfitAccount();
        //            lobjDeathNotification.LoadMonthlyBenefitPaymentAmount();
        //            lobjDeathNotification.ibusDeceasedPayeeAccount.LoadPaymentDetails();
        //            // UAT PIR: 2132 While fetching the Monthly Benefit Amount, Fetch the amount as of 1st of the month following date of death, 
        //            // here retiremetn date or Go Live date which ever is later.
        //            lobjDeathNotification.ibusDeceasedPayeeAccount.LoadGrossBenefitAmount(
        //                    busGlobalFunctions.GetMax(lobjDeathNotification.icdoDeathNotification.date_of_death, busPayeeAccountHelper.GetPERSLinkGoLiveDate()));
        //            lobjDeathNotification.LoadMonthlyGrossBenefitAmount();
        //            lobjDeathNotification.LoadPayeeDeathLetterInsuranceProperties(iobjSystemManagement.icdoSystemManagement.batch_date);
        //            lobjDeathNotification.LoadTaxableandNonTaxablePaymentAmount();
        //            lobjDeathNotification.ibusDeceasedPayeeAccount.LoadLatestPaymentHistory();

        //            if (astrGroupType == "INSU")
        //                lobjDeathNotification.IsInsuranceOnly = busConstant.Flag_Yes;
        //            else
        //                lobjDeathNotification.IsInsuranceOnly = busConstant.Flag_No;

        //            bool lblnGenerateLetter = true;
        //            if ((astrGroupType == busConstant.PostRetirementAccountOwnerDeath) &&
        //                (lobjDeathNotification.ibusDeceasedPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS))
        //            {
        //                if ((lobjDeathNotification.ibusPersonToBeneficiary.icdoPersonBeneficiary.relationship_value != busConstant.DependentRelationshipSpouse) ||
        //                    (lobjDeathNotification.ibusDeceasedPayeeAccount.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.spouse_factor == 0M))
        //                    lblnGenerateLetter = false;
        //            }

        //            if (lblnGenerateLetter)
        //            {
        //                //ArrayList larrList = new ArrayList();
        //                //larrList.Add(lobjDeathNotification);
        //                Hashtable lshtTemp = new Hashtable();
        //                lshtTemp.Add("FormTable", "Batch");

        //                idlgUpdateProcessLog("Payee Death Letter for Deceased Member PERSLink ID: " + lobjDeathNotification.icdoDeathNotification.person_id.ToString()
        //                         + ", Beneficiary Name " + lobjDeathNotification.ibusPersonToBeneficiary.icdoPersonBeneficiary.beneficiary_name, "INFO", istrProcessName);
        //                CreateCorrespondence("APP-7252", lobjDeathNotification, lshtTemp);

        //                // Update letter sent flag
        //                // The query is not grouped by DeathNotification. Possibilities of occurence of same Death notification for different beneficiaries.
        //                if (lintLastDeceasedPersonID != lobjDeathNotification.icdoDeathNotification.person_id)
        //                    iclbDeathNotification.Add(lobjDeathNotification);
        //                lintLastDeceasedPersonID = lobjDeathNotification.icdoDeathNotification.person_id;

        //                iclbPersonBeneficiary.Add(lobjDeathNotification.ibusPersonToBeneficiary);
        //            }
        //        }
        //    }
        //}
    }
}
