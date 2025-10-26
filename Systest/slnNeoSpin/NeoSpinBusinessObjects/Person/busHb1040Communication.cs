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
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busHb1040Communication:
    /// </summary>
	[Serializable]
	public partial class busHb1040Communication : busExtendBase
    {
		/// <summary>
        	/// Constructor for NeoSpin.busHb1040Communication
        	/// </summary>
		public busHb1040Communication()
		{


		}

        public doHb1040Communication icdoHb1040Communication { get; set; }

        public bool FindHB1040Communication(int Aintpersonaccountid)
        {
            bool lblnResult = false;
            if (icdoHb1040Communication == null)
            {
                icdoHb1040Communication = new doHb1040Communication();
            }
            DataTable ldtbList = Select<doHb1040Communication>(
                                    new string[1] { "person_account_id" },
                                    new object[1] { Aintpersonaccountid }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoHb1040Communication.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        public decimal idecCurrentSalary { get; set; }
        
        public decimal idecProjectedYearlyFASatRetirement { get; set; }
        public decimal idecMonthlyDBBenefitAmount { get; set; }
        public decimal idecBenefitASPercentileOfFAS  { get; set; }
        public decimal idecLifeExpDBBenefit { get; set; }
        public decimal idecPSCAtRetirement { get; set; }
        public decimal idecProjectedDBAccountBalance { get; set; }
        public decimal idecProjectedDCAccountBalance { get; set; }

        public decimal idec10YearDBLifeTimeBenefit { get; set; }
        public decimal idec15YearDBLifeTimeBenefit { get; set; }
        public decimal idec20YearDBLifeTimeBenefit { get; set; }
        public decimal idec25YearDBLifeTimeBenefit { get; set; }
        public decimal idec30YearDBLifeTimeBenefit { get; set; }
        public decimal idec35YearDBLifeTimeBenefit { get; set; }
        public decimal idec40YearDBLifeTimeBenefit { get; set; }
        public decimal idecLifeExpDBLifeTimeBenefit { get; set; }


        public decimal idec10YearDCMonthlyBenefit { get; set; }        
        public decimal idec15YearDCMonthlyBenefit { get; set; }
        public decimal idec20YearDCMonthlyBenefit { get; set; }
        public decimal idec25YearDCMonthlyBenefit { get; set; }
        public decimal idec30YearDCMonthlyBenefit { get; set; }
        public decimal idec35YearDCMonthlyBenefit { get; set; }
        public decimal idec40YearDCMonthlyBenefit { get; set; }
        public decimal idecLifeExpDCMonthlyBenefit { get; set; }


        public decimal idec10YearDCLifeTimeBenefit { get; set; }
        public decimal idec15YearDCLifeTimeBenefit { get; set; }
        public decimal idec20YearDCLifeTimeBenefit { get; set; }
        public decimal idec25YearDCLifeTimeBenefit { get; set; }
        public decimal idec30YearDCLifeTimeBenefit { get; set; }
        public decimal idec35YearDCLifeTimeBenefit { get; set; }
        public decimal idec40YearDCLifeTimeBenefit { get; set; }
        public decimal idecLifeExpDCLifeTimeBenefit { get; set; }

        public string istrCurrentPlanTierName { get; set; }
        public string istrFuturePlanTierName { get; set; }
        public decimal idecPSC { get; set; }

        public string idecEEContributionRateDB { get; set; }
        public string idecEEContributionRateDC { get; set; }
        public string idecERContributionRateDB { get; set; }
        public string idecERContributionRateDC { get; set; }

        public string AgeAtNRDYearMonth
        {
            get
            {
                decimal ldecAgeAtNRD = icdoHb1040Communication.age_at_nrd;
                decimal ldecAgetAtNRDMonth = ldecAgeAtNRD - Math.Truncate(ldecAgeAtNRD);
                return String.Format("{0} Years {1} Months", Math.Floor(ldecAgeAtNRD).ToString(),
                                     Math.Floor(ldecAgetAtNRDMonth * 12).ToString());
            }
        }
        public string PSCAtRetirementYearMonths
        {
            get
            {
                if (idecPSCAtRetirement < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(idecPSCAtRetirement / 12).ToString(),
                                     Math.Round((idecPSCAtRetirement % 12), 0, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(idecPSCAtRetirement / 12).ToString(),
                                     Math.Round((idecPSCAtRetirement % 12), 0, MidpointRounding.AwayFromZero).ToString());
            }
        }
        public string PSCFormattedAsYearMonths
        {
            get
            {
                if (idecPSC < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(idecPSC / 12).ToString(),
                                     Math.Round((idecPSC % 12), 0, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(idecPSC / 12).ToString(),
                                     Math.Round((idecPSC % 12), 0, MidpointRounding.AwayFromZero).ToString());
            }
        }
    }
}
