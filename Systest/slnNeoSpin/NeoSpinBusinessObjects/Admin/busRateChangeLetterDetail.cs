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
	/// Class NeoSpin.busRateChangeLetterDetail:
	/// Inherited from busRateChangeLetterDetailGen, the class is used to customize the business object busRateChangeLetterDetailGen.
	/// </summary>
	[Serializable]
	public class busRateChangeLetterDetail : busRateChangeLetterDetailGen
	{
        public busPerson ibusPerson { get; set; }

        //Correspondence Properties
        public Collection<busRateChangeLetterDetail> iclbInsuPlansExcLifePremDetails { get; set; }

        public Collection<busRateChangeLetterDetail> iclbInsuPlansLifePremDetails { get; set; }

        public busRateChangeLetterRequest ibusRateChangeLetterRequest { get; set; }

        public string istrInsurancePlanName { get; set; }

        public bool IsInsurancePlansExceptLifeEnrolled
        {
            get
            {
                if (iclbInsuPlansExcLifePremDetails.IsNotNull() && iclbInsuPlansExcLifePremDetails.Count > 0) return true;
                    return false;
            }
        }

        public bool IsInsurancePlanLifeEnrolled
        {
            get
            {
                if (iclbInsuPlansLifePremDetails.IsNotNull() && iclbInsuPlansLifePremDetails.Count > 0) return true;
                return false;
            }
        }
        public bool IsRHICApplied
        {
            get
            {
                return icdoRateChangeLetterDetail.rhic_amount > 0 ? true : false;
            }
        }
        public bool IsForeignCountry
        {
            get
            {
                if (ibusPerson.IsNotNull() && ibusPerson.ibusPersonCurrentAddress.IsNotNull() && ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.IsNotNull())
                {
                    if (ibusPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value != busConstant.US_Code_ID)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }
        public bool IsLetterTypeRHIC
        {
            get
            {
                if(ibusRateChangeLetterRequest.IsNotNull())
                {
                    if (ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.health == busConstant.Flag_Yes ||
                        ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.medd == busConstant.Flag_Yes ||
                        ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.dental == busConstant.Flag_Yes ||
                        ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.vision == busConstant.Flag_Yes ||
                        ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.life == busConstant.Flag_Yes)
                        return false;
                }
                return true;
            }
        }
        public bool IsInsuranceRateChanged
        {
            get
            {
                if (ibusRateChangeLetterRequest.IsNotNull())
                {
                    if (ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.health == busConstant.Flag_Yes ||
                        ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.medd == busConstant.Flag_Yes ||
                        ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.dental == busConstant.Flag_Yes ||
                        ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.vision == busConstant.Flag_Yes ||
                        ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.life == busConstant.Flag_Yes)
                        return true;
                }
                return false;            
            }
        }
        public bool IsMedicarePartDEnrolled
        {
            get
            {
                return (icdoRateChangeLetterDetail.new_medd_prem > 0 || icdoRateChangeLetterDetail.curr_medd_prem > 0) ? true : false;
            }
        }
        public override busBase GetCorPerson()
        {
            return ibusPerson;
        }
        public decimal idecNewPremium { get; set; }

        public decimal idecCurrPremium { get; set; }

        public decimal idecCoverageAmount { get; set; }
    }
}
