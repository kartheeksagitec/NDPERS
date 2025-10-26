#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busCustomCorrespondence:
    /// </summary>
	[Serializable]
	public partial class busCustomCorrespondence : busNeoSpinBase
	{
        /// <summary>
        /// Constructor for NeoSpin.busCustomCorrespondence
        /// </summary>
        public busDeathNotification ibusDeathNotification { get; set; }
        public string istrTemplateName { get; set; }
        
        public ArrayList GenerateDeathCorrespondence()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            if ((istrTemplateName == busConstant.NonEmployeeDeathLetterCodeValue && (!iclbDistinctPersonAccountDependent.Any(i => i.iblnIsRecordSelectToGenerateCor) &&
                    !iclbDistinctPersonAccountBeneficiary.Any(i => i.iblnIsRecordSelectToGenerateCor))) ||
                    (istrTemplateName == busConstant.EmployeeDeathLetterCodeValue && !iclbDistinctPersonBeneficiary.Any(i => i.iblnIsRecordSelectToGenerateCor)))
            {
                    lobjError = AddError(10484, busGlobalFunctions.GetMessageTextByMessageID(10484, iobjPassInfo));
                    alReturn.Add(lobjError);
                    return alReturn;
            }

            if (alReturn.Count == 0)
            {
                if (istrTemplateName == busConstant.NonEmployeeDeathLetterCodeValue)
                {
                    Collection<busPerson> lobjDummyPerson = new Collection<busPerson>();

                    foreach (busPersonAccountDependent lobjPersonAccountDependent in iclbDistinctPersonAccountDependent)
                    {
                        if(lobjPersonAccountDependent.iblnIsRecordSelectToGenerateCor)
                            ibusDeathNotification.SetPADependentCorrespondenceProperties(lobjDummyPerson, lobjPersonAccountDependent);
                    }
                    foreach (busPersonAccountBeneficiary lobjPersonAccountBeneficiary in iclbDistinctPersonAccountBeneficiary)
                    {
                        if (lobjPersonAccountBeneficiary.iblnIsRecordSelectToGenerateCor)
                            ibusDeathNotification.SetPABeneficiaryCorrespondenceProperties(lobjDummyPerson, lobjPersonAccountBeneficiary);
                    }
                }
                else if (istrTemplateName == busConstant.EmployeeDeathLetterCodeValue)
                {
                    ibusDeathNotification.LoadAllCacheDataAndOtherTableData();
                    ibusDeathNotification.ibusPerson = new busPerson { icdoPerson = new CustomDataObjects.cdoPerson() };
                    ibusDeathNotification.LoadPerson();
                    foreach (busPersonBeneficiary lobjPersonBeneficiary in iclbDistinctPersonBeneficiary)
                    {
                        if (lobjPersonBeneficiary.iblnIsRecordSelectToGenerateCor)
                        {
                            ibusDeathNotification.LoadBeneficiaryAndCorPropertiesData(ibusDeathNotification, lobjPersonBeneficiary);
                        }
                    }
                    
                }
                else if(istrTemplateName == busConstant.PayeeDeathLetterCodeValue)
                {
                    Collection<busPersonBeneficiary> lclbPersonBeneficiary = new Collection<busPersonBeneficiary>();
                    Collection<busDeathNotification> lclbDeathNotification = new Collection<busDeathNotification>();
                    DataTable ldtbPayeeDeathACOD = busBase.Select("cdoDeathNotification.PayeeDeathLetterACODFromScreen", new object[1] { ibusDeathNotification.icdoDeathNotification.person_id});
                    if (ldtbPayeeDeathACOD.Rows.Count > 0)
                    {
                        ibusDeathNotification.CreateLetter(ldtbPayeeDeathACOD, busConstant.PostRetirementAccountOwnerDeath, 
                            ref lclbPersonBeneficiary, ref lclbDeathNotification);
                    }

                    DataTable ldtbPayeeDeathFBED = busBase.Select("cdoDeathNotification.PayeeDeathLetterFBEDFromScreen", new object[1] { ibusDeathNotification.icdoDeathNotification.person_id });
                    if (ldtbPayeeDeathFBED.Rows.Count > 0)
                    {
                        ibusDeathNotification.CreateLetter(ldtbPayeeDeathFBED, string.Empty,
                            ref lclbPersonBeneficiary, ref lclbDeathNotification);
                    }

                    DataTable ldtbPayeeDeathALPD = busBase.Select("cdoDeathNotification.PayeeDeathLetterALPDFromScreen", new object[1] { ibusDeathNotification.icdoDeathNotification.person_id });
                    if (ldtbPayeeDeathALPD.Rows.Count > 0)
                    {
                        ibusDeathNotification.CreateLetter(ldtbPayeeDeathALPD, string.Empty,
                            ref lclbPersonBeneficiary, ref lclbDeathNotification);
                    }

                    DataTable ldtbPayeeDeathInsurance = busBase.Select("cdoDeathNotification.PayeeDeathLetterInsuranceFromScreen", new object[1] { ibusDeathNotification.icdoDeathNotification.person_id });
                    if (ldtbPayeeDeathInsurance.Rows.Count > 0)
                    {
                        ibusDeathNotification.CreateLetter(ldtbPayeeDeathInsurance, "INSU",
                            ref lclbPersonBeneficiary, ref lclbDeathNotification);
                    }
                }
                alReturn.Add(this);
                return alReturn;
            }
            else
                return alReturn;
        }
        public Collection<cdoCodeValue> LoadDeathLetterTemplateName()
        {
            Collection<cdoCodeValue> lclcCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(7034, null, null, null);
            Collection<cdoCodeValue> lclcAllCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            foreach (cdoCodeValue lcdoCodeValue in lclcAllCodeValue)
            {
                if ((ibusDeathNotification.icdoDeathNotification.non_employee_death_batch_flag == busConstant.Flag_Yes && lcdoCodeValue.code_value == busConstant.NonEmployeeDeathLetterCodeValue) ||
                    (ibusDeathNotification.icdoDeathNotification.employee_death_batch_letter_sent == busConstant.Flag_Yes && lcdoCodeValue.code_value == busConstant.EmployeeDeathLetterCodeValue) ||
                    (ibusDeathNotification.icdoDeathNotification.is_payee_death_letter_sent_flag == busConstant.Flag_Yes && lcdoCodeValue.code_value == busConstant.PayeeDeathLetterCodeValue))
                    lclcCodeValue.Add(lcdoCodeValue);                
            }
            return lclcCodeValue;
        }
        public Collection<busPersonBeneficiary> iclbDistinctPersonBeneficiary { get; set; }
        public Collection<busPersonAccountDependent> iclbDistinctPersonAccountDependent { get; set; }
        public Collection<busPersonAccountBeneficiary> iclbDistinctPersonAccountBeneficiary { get; set; }
        public void LoadDistinctPersonBeneficiary()
        {
            if (ibusDeathNotification.iclbCorPersonBeneficiary.IsNull())
                ibusDeathNotification.LoadCorPersonBeneficiary();
            if (ibusDeathNotification.ibusPerson.IsNull())
                ibusDeathNotification.LoadPerson();
            iclbDistinctPersonBeneficiary = new Collection<busPersonBeneficiary>();
            Collection<busPersonBeneficiary> lcolPersonBeneficiary = new Collection<busPersonBeneficiary>();
            foreach (busPersonBeneficiary lobj in ibusDeathNotification.iclbCorPersonBeneficiary)
            {
                if (lobj.ibusPerson.IsNull())
                    lobj.LoadPerson();
                if (lobj.ibusBeneficiaryPerson == null)
                    lobj.LoadBeneficiaryPerson();
                if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull())
                    lobj.ibusPersonAccountBeneficiary.LoadPersonAccount();
                if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsNull())
                    lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.LoadPlan();

                if (lobj.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsDBRetirementPlan() && lobj.icdoPersonBeneficiary.beneficiary_person_id > 0 
                    && lobj.ibusBeneficiaryPerson.icdoPerson.date_of_death == DateTime.MinValue 
                    && (busGlobalFunctions.CheckDateOverlapping(ibusDeathNotification.ibusPerson.icdoPerson.date_of_death,
                                      lobj.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                      lobj.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date)))
                    lcolPersonBeneficiary.Add(lobj);
            }
            var lenuDistinctBeneficiary = lcolPersonBeneficiary.GroupBy(i => i.icdoPersonBeneficiary.beneficiary_person_id).Select(i => i.First());
            foreach (busPersonBeneficiary lbusPersonBeneficiary in lenuDistinctBeneficiary)
            {
                iclbDistinctPersonBeneficiary.Add(lbusPersonBeneficiary);
            }
        }
        public void LoadCorPersonAccountDependent()
        {
            if (ibusDeathNotification.ibusPerson.IsNull())
                ibusDeathNotification.LoadPerson();
            DataTable ldtbPersonAccountDependent = Select("entPerson.LoadCorPersonAccountDependent", new object[1] { ibusDeathNotification.icdoDeathNotification.death_notification_id });
            iclbDistinctPersonAccountDependent = new Collection<busPersonAccountDependent>();
            Collection<busPersonAccountDependent> lcolPersonAccountDependent = new Collection<busPersonAccountDependent>();
            foreach (DataRow dr in ldtbPersonAccountDependent.Rows)
            {
                busPersonAccountDependent lobjPersonAccountDependent = new busPersonAccountDependent();
                lobjPersonAccountDependent.icdoPersonAccountDependent = new CustomDataObjects.cdoPersonAccountDependent();
                lobjPersonAccountDependent.FindPersonAccountDependent(Convert.ToInt32(dr[enmCorBeneDepTo.person_account_dependent_id.ToString()]));
                lobjPersonAccountDependent.FindPersonDependent(lobjPersonAccountDependent.icdoPersonAccountDependent.person_dependent_id);
                lobjPersonAccountDependent.LoadPerson();
                if(lobjPersonAccountDependent.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue &&
                  (busGlobalFunctions.CheckDateOverlapping(ibusDeathNotification.ibusPerson.icdoPerson.date_of_death,
                                          lobjPersonAccountDependent.icdoPersonAccountDependent.start_date,
                                          lobjPersonAccountDependent.icdoPersonAccountDependent.end_date)))
                    lcolPersonAccountDependent.Add(lobjPersonAccountDependent);
            }
            //iclbDistinctPersonAccountDependent = lcolPersonAccountDependent;
            var lenuDistinctPersonAccountDependent = lcolPersonAccountDependent.GroupBy(i => i.icdoPersonAccountDependent.person_dependent_id).Select(i => i.First());
            foreach (busPersonAccountDependent lbusPersonAccountDependent in lenuDistinctPersonAccountDependent)
            {
                iclbDistinctPersonAccountDependent.Add(lbusPersonAccountDependent);
            }
        }
        public void LoadCorPersonAccountBeneficiary()
        {
            if (ibusDeathNotification.ibusPerson.IsNull())
                ibusDeathNotification.LoadPerson();
            DataTable ldtbPersonAccountBeneficiary = Select("entPerson.LoadCorPersonAccountBeneficiary", new object[1] { ibusDeathNotification.icdoDeathNotification.death_notification_id });
            iclbDistinctPersonAccountBeneficiary = new Collection<busPersonAccountBeneficiary>();
            Collection<busPersonAccountBeneficiary> lcolPersonAccountBeneficiary = new Collection<busPersonAccountBeneficiary>();
            foreach (DataRow dr in ldtbPersonAccountBeneficiary.Rows)
            {
                busPersonAccountBeneficiary lobjPersonAccountBeneficiary = new busPersonAccountBeneficiary();
                lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary = new CustomDataObjects.cdoPersonAccountBeneficiary();
                lobjPersonAccountBeneficiary.FindPersonAccountBeneficiary(Convert.ToInt32(dr[enmCorBeneDepTo.person_account_beneficiary_id.ToString()]));
                lobjPersonAccountBeneficiary.FindPersonBeneficiary(lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_id);
                lobjPersonAccountBeneficiary.LoadPerson();
                if(lobjPersonAccountBeneficiary.ibusPerson.icdoPerson.date_of_death == DateTime.MinValue &&
                  (busGlobalFunctions.CheckDateOverlapping(ibusDeathNotification.ibusPerson.icdoPerson.date_of_death,
                                          lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                          lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date)))
                    lcolPersonAccountBeneficiary.Add(lobjPersonAccountBeneficiary);
            }
            //iclbDistinctPersonAccountDependent = lcolPersonAccountDependent;
            var lenuDistinctPersonAccountBeneficiary = lcolPersonAccountBeneficiary.GroupBy(i => i.icdoPersonAccountBeneficiary.beneficiary_id).Select(i => i.First());
            foreach (busPersonAccountBeneficiary lbusPersonAccountBeneficiary in lenuDistinctPersonAccountBeneficiary)
            {
                iclbDistinctPersonAccountBeneficiary.Add(lbusPersonAccountBeneficiary);
            }
        }
    }
}
