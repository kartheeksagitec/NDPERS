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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busPaymentLifeTimeReductionRef:
	/// Inherited from busPaymentLifeTimeReductionRefGen, the class is used to customize the business object busPaymentLifeTimeReductionRefGen.
	/// </summary>
	[Serializable]
	public class busPaymentLifeTimeReductionRef : busPaymentLifeTimeReductionRefGen
	{
        /// <summary>
        /// Method to get number of payments for Life time reduction
        /// </summary>
        /// <param name="astrBenefitOption">Benefit Option</param>
        /// <param name="astrGender">Gender</param>
        /// <param name="adecMemberAge">Member Age</param>
        /// <param name="adecJoinAnntAge">Join Annuitant Age</param>
        /// <returns>Number of Payments</returns>
        public int GetNumberofPaymentsForRecovery(string astrBenefitOption, string astrGender, decimal adecMemberAge, decimal adecJoinAnntAge, string astrBenActTypeValue)
        {
            int lintNumberofPayments = 0;
            //PIR 17909 - If the payee account is DETH or PSTD the factor for the Life Time Reduction Table should consider it a Single Life 
            //option regardless of what option is in the Payee Account
            if ((astrBenActTypeValue == busConstant.ApplicationBenefitTypePostRetirementDeath) || (astrBenActTypeValue == busConstant.ApplicationBenefitTypePreRetirementDeath))
                astrBenefitOption = busConstant.BenefitOptionSingleLife;
            //For JS options 100% Joint and Survivor and 50% Joint and survivor, the value are to be floored
            //Compared exactly like the other options.
            //PIR: 1936
            if ((astrBenefitOption == busConstant.BenefitOption100PercentJS) ||
                (astrBenefitOption == busConstant.BenefitOption50PercentJS))
            {
                adecMemberAge = Math.Floor(adecMemberAge);
                adecJoinAnntAge = Math.Floor(adecJoinAnntAge);
            }
            else
            {
                astrBenefitOption = busConstant.BenefitOptionSingleLife; //PIR 25184 - Benefit Repayment Actuarial Option Factors should have only 3 options - Single, 50JS &100JS
            }
            DataTable ldtLifeTimeReductionRef = SelectWithOperator<cdoPaymentLifeTimeReductionRef>
                (new string[4]{enmPaymentLifeTimeReductionRef.benefit_option_value.ToString(),enmPaymentLifeTimeReductionRef.member_gender_value.ToString(),
                                enmPaymentLifeTimeReductionRef.member_age.ToString(), enmPaymentLifeTimeReductionRef.effective_date.ToString()}, new string[4] { "=", "=", "=", "<=" },
                new object[4] { astrBenefitOption, astrGender, adecMemberAge, DateTime.Now }, enmPaymentLifeTimeReductionRef.effective_date.ToString() + " DESC");
            if (ldtLifeTimeReductionRef.Rows.Count > 0)
            {
                if (ldtLifeTimeReductionRef.AsEnumerable().Where(o => o.Field<decimal>("joint_and_survivor_age") > 0).AsDataTable().Rows.Count > 0)
                    ldtLifeTimeReductionRef = ldtLifeTimeReductionRef.AsEnumerable().Where(o => o.Field<decimal>("joint_and_survivor_age") == adecJoinAnntAge).AsDataTable();

                if (ldtLifeTimeReductionRef.Rows.Count > 0)
                    lintNumberofPayments = Convert.ToInt32(ldtLifeTimeReductionRef.Rows[0][enmPaymentLifeTimeReductionRef.number_of_payments.ToString()]);
            }
            return lintNumberofPayments;
        }
    }
}
