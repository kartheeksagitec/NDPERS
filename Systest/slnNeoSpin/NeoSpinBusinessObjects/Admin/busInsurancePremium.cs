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
    /// Class NeoSpin.BusinessObjects.busDuesRateChangeRequest:
    /// Inherited from busDuesRateChangeRequestGen, the class is used to customize the business object busDuesRateChangeRequestGen.
    /// </summary>
    [Serializable]
    public class busInsurancePremium : busExtendBase
    {
        //Employer Letter
        #region Health
        public decimal idecWithWellnessPremium { get; set; }
        public decimal idecWithoutWellnessPremium { get; set; }
        public string istrClientHealthDescription { get; set; }
        public string istrCoverageCode { get; set; }
        public decimal idecTempHealthMedicarePremium { get; set; }
        public int iintRateRefID { get; set; }
        #endregion

        # region Dental
        public decimal idecCurrentDentalPremium { get; set; }
        public decimal idecNewDentalPremium { get; set; }
        public string istrClientDentalDescription { get; set; }
        public string istrDentalInsuranceTypeValue { get; set; }
        public string istrDentalLevelOfCoverage { get; set; }
        # endregion

        # region Vision
        public decimal idecCurrentVisionPremium { get; set; }
        public decimal idecNewVisionPremium { get; set; }
        public string istrClientVisionDescription { get; set; }
        public string istrVisionInsuranceTypeValue { get; set; }
        public string istrVisionLevelOfCoverage { get; set; }
        # endregion

        # region EAP
        public decimal idecNewEAPPremium { get; set; }
        public decimal idecCurrentEAPPremium { get; set; }
        public string istrClientEAPDescription { get; set; }
        public string istrEAPInsuranceValue { get; set; }
        # endregion

        //ORG TO Bill and TFFR Pension Check
        public decimal idecCurrentPremium { get; set; }
        public decimal idecNewPremium { get; set; }
        //PIR 14304 - for template PAY-4305
        public decimal idecTotalRHIC { get; set; }

        public decimal idecCurrentRHICAmount { get; set; }
        public decimal idecNewRHICAmount { get; set; }

        //PIR 11355
        public bool iblnForeignAddress
        {
            get
            {

                if (ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress != null)
                {
                    if (ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value != busConstant.US_Code_ID)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        public bool iblnRHICExists
        {
            get
            {
                //PIR 15119 - RHIC bookmark is not populating
                if (ibusPerson.IsNotNull() && ibusPerson.ibusLatestBenefitRhicCombine.IsNotNull() && ibusRateChangeLetterRequest.IsNotNull())
                {
                    if (ibusPerson.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.start_date <= ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date)
                        return true;
                }
                //if (idecCurrentRHICAmount != 0.00M || idecNewRHICAmount != 0.00M)
                //    return true;
                return false;
            }
        }

        //PIR 8823
        public bool iblnRHICEqualOrExceedsPremium
        {
            get
            {
                if ((Math.Abs(idecCurrentPremium) - Math.Abs(idecCurrentRHICAmount)) <= 0.00M)
                    return true;
                return false;
            }
        }

        public string istrLevelOfCoverageValue { get; set; }
        public string istrLevelOfCoverage
        {
            get
            {
                if (istrLevelOfCoverageValue.IsNotNullOrEmpty())
                    return iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, istrLevelOfCoverageValue);
                return string.Empty;
            }
        }
        public decimal idecCoverageAmount { get; set; }

        //IBS Letter
        public string istrIBSMemberFullName { get; set; }
        public string istrIsPERSPensionPayment { get; set; }
        public int iintPayeeAccountID { get; set; }

        public Collection<busInsurancePremium> iclbCoverageLevelLTCMemberPremium { get; set; }
        public Collection<busInsurancePremium> iclbCoverageLevelLTCSpousePremium { get; set; }
        public Collection<busInsurancePremium> iclbCoverageLevelLifePremium { get; set; }

        //Health (We have to create seperate property for Medicare since both Health and Medicare Printing on the same letter if member enrolled in both.
        public decimal idecNewMedicarePartDPremium { get; set; }
        public decimal idecCurrentMedicarePartDPremium { get; set; }

        public decimal idecCurrentMedicareRHICAmount { get; set; }
        public decimal idecNewMedicareRHICAmount { get; set; }
		
		//PIR 15347 - Medicare Part D bookmarks
        public decimal idecTotalMedicareDPremiumAmount { get; set; }
        
        public decimal idecCurrentNetPremium
        {
            get
            {
                return idecCurrentPremium - idecCurrentRHICAmount;
            }
        }

        public decimal idecNewNetPremium
        {
            get
            {
                //PIR-11301 should return 0 if difference is negative in PAY-4305
                if ((idecNewPremium - idecNewRHICAmount) > 0.0m)
                    return idecNewPremium - idecNewRHICAmount;
                else
                    return 0.0m;
            }
        }

        public decimal idecCurrentMedicareNetPremium
        {
            get
            {
				//PIR 15347 - Medicare Part D bookmarks
                return idecCurrentMedicarePartDPremium - idecCurrentMedicareRHICAmount;
            }
        }

        public decimal idecNewNetMedicarePremium
        {
            get
            {
                return idecNewMedicarePartDPremium - idecNewMedicareRHICAmount;
            }
        }

        public busRateChangeLetterRequest ibusRateChangeLetterRequest { get; set; }
         

        public busPerson ibusPerson { get; set; }

        public override busBase GetCorPerson()
        {
            return ibusPerson;
        }

        public decimal idecCurrentNetCheckAmount { get; set; }
        public decimal idecNewNetCheckAmount { get; set; }

        public decimal idecPremiumdifference
        {
            get
            {
                return idecNewPremium - idecCurrentPremium;
            }
        }

        // PROD PIR ID 5238
        public int iintMemberAge { get; set; }

        public void CalculateMemberAge()
        {
            if (ibusPerson.IsNotNull() && ibusRateChangeLetterRequest.IsNotNull() &&
                ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date != DateTime.MinValue)
            {
                int lintYears, lintMonths;
                DateTime ldteFromDate = ibusPerson.icdoPerson.date_of_birth.GetFirstDayofCurrentMonth();
                DateTime ldteToDate = new DateTime(ibusRateChangeLetterRequest.icdoRateChangeLetterRequest.effective_date.AddYears(-1).Year, 12, 31);
                HelperFunction.GetMonthSpan(ldteFromDate, ldteToDate, out lintYears, out lintMonths);
                iintMemberAge = lintYears;
            }
        }

        //prod pir 7008
        public string istrRateSturctureCode { get; set; }

        public void LoadPerson(int aintpersonid)
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(aintpersonid);
        }
    }
}
