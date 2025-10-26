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
	/// Class NeoSpin.BusinessObjects.busWssPersonAccountFlexCompConversion:
	/// Inherited from busWssPersonAccountFlexCompConversionGen, the class is used to customize the business object busWssPersonAccountFlexCompConversionGen.
	/// </summary>
	[Serializable]
	public class busWssPersonAccountFlexCompConversion : busWssPersonAccountFlexCompConversionGen
	{
        //PIR 10044
        public busOrganization ibusProvider { get; set; }
        public bool iblnHaveVisionDentalOrLifePlan;
       
        public busPersonAccount ibusPersonAccount
        { get; set; }

        public bool IsValidProvider()
        
        {
            if (icdoWssPersonAccountFlexCompConversion.org_id != 0)
            {
                if (ibusProvider.IsNull()) LoadProvider();
                if (ibusProvider.iclbOrgPlan.IsNull()) ibusProvider.LoadOrgPlan();

                //UAT PIR 472 - This validation is only for Health / Dental / Vision / Life Providers
                bool lblnIsProviderToValidate = ibusProvider.iclbOrgPlan.Any(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupHealth ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdMedicarePartD ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdDental ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdVision);
                //PIR 7987  Providers that have Flex and other PERS plans (Vision, Dental, Life) should not appear in the list 
                iblnHaveVisionDentalOrLifePlan = ibusProvider.iclbOrgPlan.Any(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdDental ||
                                                                             i.icdoOrgPlan.plan_id == busConstant.PlanIdVision);
                if (lblnIsProviderToValidate)
                {

                    foreach (busOrgPlan lobjOrgPlan in ibusProvider.iclbOrgPlan)
                    {
                        if (lobjOrgPlan.icdoOrgPlan.plan_id != busConstant.PlanIdFlex)
                        {
                            if (busGlobalFunctions.CheckDateOverlapping(
                                        icdoWssPersonAccountFlexCompConversion.effective_start_date,
                                        icdoWssPersonAccountFlexCompConversion.effective_end_date,
                                        lobjOrgPlan.icdoOrgPlan.participation_start_date,
                                        lobjOrgPlan.icdoOrgPlan.participation_end_date))
                            {
                                if (lobjOrgPlan.icdoOrgPlan.plan_id == busConstant.PlanIdGroupLife)
                                {
                                    busPersonAccountLife lobjLife = new busPersonAccountLife
                                    {
                                        icdoPersonAccount = new cdoPersonAccount(),
                                        icdoPersonAccountLife = new cdoPersonAccountLife(),
                                        iclbLifeOption = new Collection<busPersonAccountLifeOption>()
                                    };
                                    if (ibusPersonAccount.ibusPerson == null)
                                        ibusPersonAccount.LoadPerson();
                                    busPersonAccount lobjPersonAccount = ibusPersonAccount.ibusPerson.LoadActivePersonAccountByPlan(lobjOrgPlan.icdoOrgPlan.plan_id);
                                    lobjLife.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                                    DataTable ldtbResult = Select<cdoPersonAccountLifeOption>(new string[1] { "PERSON_ACCOUNT_ID" },
                                                            new object[1] { lobjPersonAccount.icdoPersonAccount.person_account_id }, null, null);
                                    lobjLife.iclbLifeOption = GetCollection<busPersonAccountLifeOption>(ldtbResult, "icdoPersonAccountLifeOption");
                                    if (lobjLife.IsSupplementalEntered())
                                    {
                                        return true;
                                    }
                                }
                                else
                                {
                                    if (ibusPersonAccount.ibusPerson == null)
                                        ibusPersonAccount.LoadPerson();
                                    if (ibusPersonAccount.ibusPerson.IsMemberEnrolledInPlan(lobjOrgPlan.icdoOrgPlan.plan_id))
                                        return true;
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        public void LoadProvider()
        {
            if (ibusProvider == null)
            {
                ibusProvider = new busOrganization();
            }
            ibusProvider.FindOrganization(icdoWssPersonAccountFlexCompConversion.org_id);
        }

        public void LoadPersonAccount()
        {
            if (ibusPersonAccount == null)
            {
                ibusPersonAccount = new busPersonAccount();
            }
            ibusPersonAccount.FindPersonAccount(icdoWssPersonAccountFlexCompConversion.person_account_id);
        }
	}
}
