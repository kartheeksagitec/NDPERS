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
using System.Globalization;
using NeoSpin.DataObjects;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using Sagitec.CustomDataObjects;
using System.Text.RegularExpressions;
#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busEmployerPayrollHeader : busExtendBase
    {
        public bool iblnSkipReloadAfterSave = false;
        //PIR 14493 - This property used in posting insurance batch to set the PayrollPaidDate when insurance header is reloaded from screen
        public bool iblnIsReloadFromScreen { get; set; }
        public string astrIsDetailPopulated { get; set; }
        public bool iblnFromFile { get; set; }
        public bool iblnContactNDPERSFlag { get; set; }
        public string PERSLinkBatchUser { get; set; }
        #region bus Properties
        public DateTime idtLastInterestPostingDate { get; set; }
        public int aintIndexEmployer_payroll_detail_id { get; set; }
        public string istrOnlyOther457Text { get; set; }
        public void LoadLastInterestBatchDate()
        {
            DateTime ldtResult = DateTime.Now;
            DataTable ldtpList = Select("cdoEmployerPayrollDetail.GetLastInterstPostingDate",
                                new object[0] { });
            if (ldtpList.Rows.Count > 0)
            {
                idtLastInterestPostingDate = Convert.ToDateTime(ldtpList.Rows[0]["EFFECTIVE_DATE"]);
            }
        }
        private busOrganization _ibusOrganization;
        public busOrganization ibusOrganization
        {
            get
            {
                return _ibusOrganization;
            }

            set
            {
                _ibusOrganization = value;
            }
        }
        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get
            {
                return _ibusPlan;
            }

            set
            {
                _ibusPlan = value;
            }
        }
        #endregion

        //this is used for Def Comp Cor
        private Collection<busEmployerPayrollDetail> _iclbCollectionForPostingDefCompCor;
        public Collection<busEmployerPayrollDetail> iclbCollectionForPostingDefCompCor
        {
            get { return _iclbCollectionForPostingDefCompCor; }
            set { _iclbCollectionForPostingDefCompCor = value; }
        }

        //this is used for Def Comp Cor
        private Collection<busEmployerPayrollDetail> _iclbCollectionForPostingInsuranceCor;
        public Collection<busEmployerPayrollDetail> iclbCollectionForPostingInsuranceCor
        {
            get { return _iclbCollectionForPostingInsuranceCor; }
            set { _iclbCollectionForPostingInsuranceCor = value; }
        }
        // Code Added for performance optimization
        private Collection<cdoPlan> _iclbPlan;
        public Collection<cdoPlan> iclbPlan
        {
            get { return _iclbPlan; }
            set { _iclbPlan = value; }
        }

        public void LoadPlanForOrganization() 
        {
            DataTable ldtbPlan = null;
            iclbPlan = new Collection<cdoPlan>();

            string lstrBenefitTypeValue = busEmployerReportHelper.GetBenefitTypeForEmployerHeaderType(icdoEmployerPayrollHeader.header_type_value);
            if (lstrBenefitTypeValue == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                ldtbPlan = busNeoSpinBase.Select("cdoPlan.GetOrgSpecificPlanWithdate",
                                                             new object[3] { icdoEmployerPayrollHeader.org_id, icdoEmployerPayrollHeader.pay_check_date, lstrBenefitTypeValue });
            }
            else
            {
                ldtbPlan = busNeoSpinBase.Select("cdoPlan.RetirementPlans",
                                                              new object[2] { icdoEmployerPayrollHeader.org_id, lstrBenefitTypeValue });
            }
            foreach (cdoPlan tcdoPlan in Sagitec.DataObjects.doBase.LoadData<cdoPlan>(ldtbPlan))
            {
                iclbPlan.Add(tcdoPlan);
            }
        }
        
        #region Collection Properties
        private Collection<busEmployerPayrollHeader> _iclbRetirementContributionByPlan;
        public Collection<busEmployerPayrollHeader> iclbRetirementContributionByPlan
        {
            get
            {
                return _iclbRetirementContributionByPlan;
            }
            set
            {
                _iclbRetirementContributionByPlan = value;
            }
        }
        private Collection<busEmployerPayrollHeader> _iclbDeferredCompContributionByPlan;
        public Collection<busEmployerPayrollHeader> iclbDeferredCompContributionByPlan
        {
            get
            {
                return _iclbDeferredCompContributionByPlan;
            }
            set
            {
                _iclbDeferredCompContributionByPlan = value;
            }
        }
        private Collection<busEmployerPayrollHeader> _iclbInsuranceContributionByPlan;
        public Collection<busEmployerPayrollHeader> iclbInsuranceContributionByPlan
        {
            get
            {
                return _iclbInsuranceContributionByPlan;
            }
            set
            {
                _iclbInsuranceContributionByPlan = value;
            }
        }

        private Collection<busEmployerPayrollHeader> _iclbPurchaseContributionByPlan;
        public Collection<busEmployerPayrollHeader> iclbPurchaseContributionByPlan
        {
            get
            {
                return _iclbPurchaseContributionByPlan;
            }
            set
            {
                _iclbPurchaseContributionByPlan = value;
            }
        }

        private Collection<busEmployerPayrollHeader> _iclbNegativeAdjustments;
        public Collection<busEmployerPayrollHeader> iclbNegativeAdjustments
        {
            get
            {
                return _iclbNegativeAdjustments;
            }
            set
            {
                _iclbNegativeAdjustments = value;
            }
        }

        private Collection<busEmployerRemittanceAllocation> _iclbEmployerRemittanceAllocation;
        public Collection<busEmployerRemittanceAllocation> iclbEmployerRemittanceAllocation
        {
            get
            {
                return _iclbEmployerRemittanceAllocation;
            }
            set
            {
                _iclbEmployerRemittanceAllocation = value;
            }
        }
        private Collection<busRemittance> _iclbAvailableRemittance;
        public Collection<busRemittance> iclbAvailableRemittance
        {
            get
            {
                return _iclbAvailableRemittance;
            }
            set
            {
                _iclbAvailableRemittance = value;
            }
        }

        private Collection<busEmployerPayrollDetail> _iclbEmployerPayrollDetail;
        public Collection<busEmployerPayrollDetail> iclbEmployerPayrollDetail
        {
            get
            {
                return _iclbEmployerPayrollDetail;
            }
            set
            {
                _iclbEmployerPayrollDetail = value;
            }
        }

        private Collection<busStatusSummary> _iclbStatusSummary;
        public Collection<busStatusSummary> iclbStatusSummary
        {
            get
            {
                return _iclbStatusSummary;
            }
            set
            {
                _iclbStatusSummary = value;
            }
        }
        private Collection<busErrorSummary> _iclbDetailError;
        public Collection<busErrorSummary> iclbDetailError
        {
            get
            {
                return _iclbDetailError;
            }
            set
            {
                _iclbDetailError = value;
            }
        }

        public Collection<busEmployerPayrollHeaderError> iclbErrorLOB { get; set; }

        private Collection<busEmployerPayrollDetailError> _iclbESSError;
        public Collection<busEmployerPayrollDetailError> iclbESSError
        {
            get
            {
                return _iclbESSError;
            }
            set
            {
                _iclbESSError = value;
            }
        }
        private Collection<busEmployerPayrollHeaderError> _iclbEmployerPayrollHeaderError;
        public Collection<busEmployerPayrollHeaderError> iclbEmployerPayrollHeaderError
        {
            get
            {
                return _iclbEmployerPayrollHeaderError;
            }
            set
            {
                _iclbEmployerPayrollHeaderError = value;
            }
        }

        public Collection<busComments> iclbPayrollHeaderCommentsHistory { get; set; } // ESS Backlog PIR - 13416
        public ArrayList iarrErrorList { get; set; } // pir 6939
        #endregion

        #region Correspondence Properties
        private decimal _idecTotalSpousePremium;
        public decimal idecTotalSpousePremium
        {
            get { return _idecTotalSpousePremium; }
            set { _idecTotalSpousePremium = value; }
        }
        private decimal _idecTotalSupplementPremium;
        public decimal idecTotalSupplementPremium
        {
            get { return _idecTotalSupplementPremium; }
            set { _idecTotalSupplementPremium = value; }
        }
        private decimal _idecTotalBasicPremium;
        public decimal idecTotalBasicPremium
        {
            get { return _idecTotalBasicPremium; }
            set { _idecTotalBasicPremium = value; }
        }
        private decimal _idecTotalDependentPremium;
        public decimal idecTotalDependentPremium
        {
            get { return _idecTotalDependentPremium; }
            set { _idecTotalDependentPremium = value; }
        }
        private decimal _idecTotalDentalInsPremium;
        public decimal idecTotalDentalInsPremium
        {
            get { return _idecTotalDentalInsPremium; }
            set { _idecTotalDentalInsPremium = value; }
        }
        private decimal _idecTotalLTCInsPremium;
        public decimal idecTotalLTCInsPremium
        {
            get { return _idecTotalLTCInsPremium; }
            set { _idecTotalLTCInsPremium = value; }
        }
        private decimal _idecTotalHealthInsPremium;
        public decimal idecTotalHealthInsPremium
        {
            get { return _idecTotalHealthInsPremium; }
            set { _idecTotalHealthInsPremium = value; }
        }
        private decimal _idecTotalVisionInsPremium;
        public decimal idecTotalVisionInsPremium
        {
            get { return _idecTotalVisionInsPremium; }
            set { _idecTotalVisionInsPremium = value; }
        }
        private decimal _idecTotalContributionFromDetails;

        public decimal idecTotalContributionFromDetails
        {
            get { return _idecTotalContributionFromDetails; }
            set { _idecTotalContributionFromDetails = value; }
        }

        #endregion


        private bool iblnInterestWaiverFlagChanged;

        public string istrEmptyPayPeriod
        {
            get
            {
                String lstrEmptyPayPeriod = String.Empty;
                if (String.IsNullOrEmpty(icdoEmployerPayrollHeader.pay_period))
                    return lstrEmptyPayPeriod;
                return icdoEmployerPayrollHeader.pay_period.ToString();
            }
        }

        // PIR 9723
        public DateTime idtPayPeriod
        {
            get
            {
                if (String.IsNullOrEmpty(icdoEmployerPayrollHeader.pay_period))
                    return DateTime.MinValue;
                else
                {
                    DateTime ldtPayPeriod = DateTime.Parse(icdoEmployerPayrollHeader.pay_period);
                    return ldtPayPeriod;
                }
            }
        }


        //this property is used in correspondence
        public DateTime idtDueDate
        {
            get
            {
                DateTime ldtDueDate = DateTime.MinValue;
                if ((_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    || ((_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)))
                {
                    ldtDueDate = icdoEmployerPayrollHeader.payroll_paid_date.AddMonths(1).AddDays(7);
                }
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    ldtDueDate = icdoEmployerPayrollHeader.pay_period_end_date.AddDays(5);
                }
                return ldtDueDate;
            }
        }

        public int iintEmloyerPayrollHeaderID { get; set; }

        private int _iintPlanID;
        public int iintPlanID
        {
            get { return _iintPlanID; }
            set { _iintPlanID = value; }
        }

        private int _iintOrgID;
        public int iintOrgID
        {
            get { return _iintOrgID; }
            set { _iintOrgID = value; }
        }
        private int _iintPersonID;
        public int iintPersonID
        {
            get { return _iintPersonID; }
            set { _iintPersonID = value; }
        }
        public string iblnIsForOrg { get; set; }

        private decimal _idecTotalRemittanceApplied;
        public decimal idecTotalRemittanceApplied
        {
            get { return _idecTotalRemittanceApplied; }
            set { _idecTotalRemittanceApplied = value; }
        }

        private decimal _idecTotalNegAdjContribution;
        public decimal idecTotalNegAdjContribution
        {
            get { return _idecTotalNegAdjContribution; }
            set { _idecTotalNegAdjContribution = value; }
        }

        #region Total Amount Properties
        private decimal _idecTotalContributionCalculatedForRet;
        public decimal idecTotalContributionCalculatedForRet
        {
            get { return _idecTotalContributionCalculatedForRet; }
            set { _idecTotalContributionCalculatedForRet = value; }
        }
        private decimal _idecTotalContributionCalculatedForDef;
        public decimal idecTotalContributionCalculatedForDef
        {
            get { return _idecTotalContributionCalculatedForDef; }
            set { _idecTotalContributionCalculatedForDef = value; }
        }
        private decimal _idecTotalPremiumReportedForIns;
        public decimal idecTotalPremiumReportedForIns
        {
            get { return _idecTotalPremiumReportedForIns; }
            set { _idecTotalPremiumReportedForIns = value; }
        }
        private decimal _idecTotalWagesCalculated;
        public decimal idecTotalWagesCalculated
        {
            get { return _idecTotalWagesCalculated; }
            set { _idecTotalWagesCalculated = value; }
        }
        private decimal _idecTotalInterestCalculated;
        public decimal idecTotalInterestCalculated
        {
            get { return _idecTotalInterestCalculated; }
            set { _idecTotalInterestCalculated = value; }
        }
		
		//PIR 15238
        private decimal _idecTotalMemberInterestCalculated;
        public decimal idecTotalMemberInterestCalculated
        {
            get { return _idecTotalMemberInterestCalculated; }
            set { _idecTotalMemberInterestCalculated = value; }
        }

        private decimal _idecTotalEmployerInterestCalculated;
        public decimal idecTotalEmployerInterestCalculated
        {
            get { return _idecTotalEmployerInterestCalculated; }
            set { _idecTotalEmployerInterestCalculated = value; }
        }

        private decimal _idecTotalEmployerRhicInterestCalculated;
        public decimal idecTotalEmployerRhicInterestCalculated
        {
            get { return _idecTotalEmployerRhicInterestCalculated; }
            set { _idecTotalEmployerRhicInterestCalculated = value; }
        }

        

        private decimal _idecTotalPurchaseAmount;
        public decimal idecTotalPurchaseAmount
        {
            get { return _idecTotalPurchaseAmount; }
            set { _idecTotalPurchaseAmount = value; }
        }

        //prod pir 4666
        public decimal idecTotalContributionCalculatedForOther457 { get; set; }
        #endregion

        #region Total Amount By Plan Properties
        private decimal _idecTotalRetrContributionByPlan;
        public decimal idecTotalRetrContributionByPlan
        {
            get { return _idecTotalRetrContributionByPlan; }
            set { _idecTotalRetrContributionByPlan = value; }
        }

        //Retirement
        private decimal _idecTotalReportedRetrContributionByPlan;
        public decimal idecTotalReportedRetrContributionByPlan
        {
            get { return _idecTotalReportedRetrContributionByPlan; }
            set { _idecTotalReportedRetrContributionByPlan = value; }
        }
        //PIR 26590 - Total contribution amount Exclude ADECAmount 
        private decimal _idecTotalReportedRetrContributionWithoutADECAmtByPlan;
        public decimal idecTotalReportedRetrContributionWithoutADECAmtByPlan
        {
            get { return _idecTotalReportedRetrContributionWithoutADECAmtByPlan; }
            set { _idecTotalReportedRetrContributionWithoutADECAmtByPlan = value; }
        }
        private decimal _idecTotalAppliedRetrContributionByPlan;
        public decimal idecTotalAppliedRetrContributionByPlan
        {
            get { return _idecTotalAppliedRetrContributionByPlan; }
            set { _idecTotalAppliedRetrContributionByPlan = value; }
        }

        private decimal _idecTotalAppliedRetrContributionByPlanWithAdec;
        public decimal idecTotalAppliedRetrContributionByPlanWithAdec
        {
            get { return _idecTotalAppliedRetrContributionByPlanWithAdec; }
            set { _idecTotalAppliedRetrContributionByPlanWithAdec = value; }
        }

        private decimal _idecTotalReportedRHICContributionByPlan;
        public decimal idecTotalReportedRHICContributionByPlan
        {
            get { return _idecTotalReportedRHICContributionByPlan; }
            set { _idecTotalReportedRHICContributionByPlan = value; }
        }
        private decimal _idecTotalAppliedRHICContributionByPlan;
        public decimal idecTotalAppliedRHICContributionByPlan
        {
            get { return _idecTotalAppliedRHICContributionByPlan; }
            set { _idecTotalAppliedRHICContributionByPlan = value; }
        }

        //Premium        
        private decimal _idecTotalAppliedPremiumByPlan;
        public decimal idecTotalAppliedPremiumByPlan
        {
            get { return _idecTotalAppliedPremiumByPlan; }
            set { _idecTotalAppliedPremiumByPlan = value; }
        }

        //purchase      
        private decimal _idecTotalAppliedPurchaseByPlan;
        public decimal idecTotalAppliedPurchaseByPlan
        {
            get { return _idecTotalAppliedPurchaseByPlan; }
            set { _idecTotalAppliedPurchaseByPlan = value; }
        }

        private decimal _idecTotalReportedPurchaseByPlan;
        public decimal idecTotalReportedPurchaseByPlan
        {
            get { return _idecTotalReportedPurchaseByPlan; }
            set { _idecTotalReportedPurchaseByPlan = value; }
        }

        private decimal _idecTotalReportedContributionByPlan;
        public decimal idecTotalReportedContributionByPlan
        {
            get { return _idecTotalReportedContributionByPlan; }
            set { _idecTotalReportedContributionByPlan = value; }
        }
        private decimal _idecTotalAppliedContributionByPlan;
        public decimal idecTotalAppliedContributionByPlan
        {
            get { return _idecTotalAppliedContributionByPlan; }
            set { _idecTotalAppliedContributionByPlan = value; }
        }

        private decimal _idecTotalRHICContributionbyPlan;
        public decimal idecTotalRHICContributionbyPlan
        {
            get { return _idecTotalRHICContributionbyPlan; }
            set { _idecTotalRHICContributionbyPlan = value; }
        }

        private decimal _idecTotalDeffContributionByPlan;
        public decimal idecTotalDeffContributionByPlan
        {
            get { return _idecTotalDeffContributionByPlan; }
            set { _idecTotalDeffContributionByPlan = value; }
        }

        private decimal _idecTotalPremiumAmountByPlan;
        public decimal idecTotalPremiumAmountByPlan
        {
            get { return _idecTotalPremiumAmountByPlan; }
            set { _idecTotalPremiumAmountByPlan = value; }
        }

        private decimal _idecTotalFeeAmountByPlan;
        public decimal idecTotalFeeAmountByPlan
        {
            get { return _idecTotalFeeAmountByPlan; }
            set { _idecTotalFeeAmountByPlan = value; }
        }

        private decimal _idecTotalBuydownAmountByPlan;
        public decimal idecTotalBuydownAmountByPlan
        {
            get { return _idecTotalBuydownAmountByPlan; }
            set { _idecTotalBuydownAmountByPlan = value; }
        }

        //PIR 14529
        private decimal _idecTotalMedicarePartDAmtByPlan;
        public decimal idecTotalMedicarePartDAmtByPlan
        {
            get { return _idecTotalMedicarePartDAmtByPlan; }
            set { _idecTotalMedicarePartDAmtByPlan = value; }
        }

        private decimal _idecTotalReportedByPlan;
        public decimal idecTotalReportedByPlan
        {
            get { return _idecTotalReportedByPlan; }
            set { _idecTotalReportedByPlan = value; }
        }
        private decimal _idecMemberTotalTaxableRetrContributionByPlan;
        public decimal idecMemberTotalTaxableRetrContributionByPlan
        {
            get { return _idecMemberTotalTaxableRetrContributionByPlan; }
            set { _idecMemberTotalTaxableRetrContributionByPlan = value; }
        }
        private decimal _idecMemberTotalNonTaxableRetrContributionByPlan;
        public decimal idecMemberTotalNonTaxableRetrContributionByPlan
        {
            get { return _idecMemberTotalNonTaxableRetrContributionByPlan; }
            set { _idecMemberTotalNonTaxableRetrContributionByPlan = value; }
        }
        public decimal idecJSRHICAmount { get; set; }
        public decimal idecOtherRHICAmount { get; set; }

        #endregion

        #region File Related Properties
        //this property is used for batch
        public bool iblnValidateDetail;

        private int _iintCountOfDetails;
        public int iintCountOfDetails
        {
            get { return _iintCountOfDetails; }
            set { _iintCountOfDetails = value; }
        }

        //this property is used for batch
        private string _istrOrgCodeId;
        public string istrOrgCodeId
        {
            get { return _istrOrgCodeId; }
            set { _istrOrgCodeId = value; }
        }

        //this property is used for batch
        private int _iintFileRecordType;
        public int iintFileRecordType
        {
            get { return _iintFileRecordType; }
            set { _iintFileRecordType = value; }
        }


        #endregion

        #region Retirment Contribution Reported Properties by Plan
        private decimal _idecTotalEEContributionReportedByPlan;
        public decimal idecTotalEEContributionReportedByPlan
        {
            get { return _idecTotalEEContributionReportedByPlan; }
            set { _idecTotalEEContributionReportedByPlan = value; }
        }

        private decimal _idecTotalERContributionReportedByPlan;
        public decimal idecTotalERContributionReportedByPlan
        {
            get { return _idecTotalERContributionReportedByPlan; }
            set { _idecTotalERContributionReportedByPlan = value; }
        }

        private decimal _idecTotalEEPreTaxReportedByPlan;
        public decimal idecTotalEEPreTaxReportedByPlan
        {
            get { return _idecTotalEEPreTaxReportedByPlan; }
            set { _idecTotalEEPreTaxReportedByPlan = value; }
        }

        private decimal _idecTotalRHICERContributionReportedByPlan;
        public decimal idecTotalRHICERContributionReportedByPlan
        {
            get { return _idecTotalRHICERContributionReportedByPlan; }
            set { _idecTotalRHICERContributionReportedByPlan = value; }
        }

        private decimal _idecTotalRHICEEContributionReportedByPlan;
        public decimal idecTotalRHICEEContributionReportedByPlan
        {
            get { return _idecTotalRHICEEContributionReportedByPlan; }
            set { _idecTotalRHICEEContributionReportedByPlan = value; }
        }

        private decimal _idecTotalEEEmployerPickupReportedByPlan;
        public decimal idecTotalEEEmployerPickupReportedByPlan
        {
            get { return _idecTotalEEEmployerPickupReportedByPlan; }
            set { _idecTotalEEEmployerPickupReportedByPlan = value; }
        }

        private decimal _idecTotalMemberInterestCalculatedByPlan;
        public decimal idecTotalMemberInterestCalculatedByPlan
        {
            get { return _idecTotalMemberInterestCalculatedByPlan; }
            set { _idecTotalMemberInterestCalculatedByPlan = value; }
        }
        private decimal _idecTotalEmployerInterestCalculatedByPlan;
        public decimal idecTotalEmployerInterestCalculatedByPlan
        {
            get { return _idecTotalEmployerInterestCalculatedByPlan; }
            set { _idecTotalEmployerInterestCalculatedByPlan = value; }
        }
       //PIR 25920 New Plan DC 2025
        //start - new ADEC columns
        private decimal _idecTotalADECCalculatedByPlan;
        public decimal idecTotalADECCalculatedByPlan
        {
            get { return _idecTotalADECCalculatedByPlan; }
            set { _idecTotalADECCalculatedByPlan = value; }
        }
        private decimal _idecTotalADECReportedByPlan;
        public decimal idecTotalADECReportedByPlan
        {
            get { return _idecTotalADECReportedByPlan; }
            set { _idecTotalADECReportedByPlan = value; }
        }
        private decimal _idecTotalADECAppliedByPlan;
        public decimal idecTotalADECAppliedByPlan
        {
            get { return _idecTotalADECAppliedByPlan; }
            set { _idecTotalADECAppliedByPlan = value; }
        }
        private decimal _idecTotalEEPreTaxAddlCalculatedByPlan;
        public decimal idecTotalEEPreTaxAddlCalculatedByPlan
        {
            get { return _idecTotalEEPreTaxAddlCalculatedByPlan; }
            set { _idecTotalEEPreTaxAddlCalculatedByPlan = value; }
        }
        private decimal _idecTotalEEPreTaxAddlReportedByPlan;
        public decimal idecTotalEEPreTaxAddlReportedByPlan
        {
            get { return _idecTotalEEPreTaxAddlReportedByPlan; }
            set { _idecTotalEEPreTaxAddlReportedByPlan = value; }
        }
        private decimal _idecTotalEEPostTaxAddlCalculatedByPlan;
        public decimal idecTotalEEPostTaxAddlCalculatedByPlan
        {
            get { return _idecTotalEEPostTaxAddlCalculatedByPlan; }
            set { _idecTotalEEPostTaxAddlCalculatedByPlan = value; }
        }
        private decimal _idecTotalEEPostTaxAddlReportedByPlan;
        public decimal idecTotalEEPostTaxAddlReportedByPlan
        {
            get { return _idecTotalEEPostTaxAddlReportedByPlan; }
            set { _idecTotalEEPostTaxAddlReportedByPlan = value; }
        }
        private decimal _idecTotalERPreTaxMatchCalculatedByPlan;
        public decimal idecTotalERPreTaxMatchCalculatedByPlan
        {
            get { return _idecTotalERPreTaxMatchCalculatedByPlan; }
            set { _idecTotalERPreTaxMatchCalculatedByPlan = value; }
        }
        private decimal _idecTotalERPreTaxMatchReportedByPlan;
        public decimal idecTotalERPreTaxMatchReportedByPlan
        {
            get { return _idecTotalERPreTaxMatchReportedByPlan; }
            set { _idecTotalERPreTaxMatchReportedByPlan = value; }
        }
        private decimal _idecTotalNegativeERPreTaxMatchReportedByPlan;
        public decimal idecTotalNegativeERPreTaxMatchReportedByPlan
        {
            get { return _idecTotalNegativeERPreTaxMatchReportedByPlan; }
            set { _idecTotalNegativeERPreTaxMatchReportedByPlan = value; }
        }
        public bool iblnIsPostDeferredCompContribution { get; set; }
        
        public bool iblnTotalEEContributionReportedByPlanHasValue { get; set; }
        public bool iblnTotalEEPreTaxReportedByPlanHasValue { get; set; }
        public bool iblnTotalEEEmployerPickupReportedByPlanHasValue { get; set; }
        public bool iblnTotalERContributionReportedByPlanHasValue { get; set; }
        public bool iblnTotalMemberInterestCalculatedByPlanHasValue { get; set; }
        public bool iblnTotalEmployerInterestCalculatedByPlanHasValue { get; set; }

        public bool iblnTotalRHICEEContributionReportedByPlanHasValue { get; set; }
        public bool iblnTotalRHICERContributionReportedByPlanHasValue { get; set; }
        public bool iblnTotalRHICEmployerInterestCalculatedByPlanHasValue { get; set; }

        public bool iblnTotalEEPreTaxAddlReportedByPlanHasValue { get; set; }
        public bool iblnTotalEEPostTaxAddlReportedByPlanHasValue { get; set; }
        public bool iblnTotalERPreTaxMatchReportedByPlanHasValue { get; set; }
        public bool iblnTotalADECReportedByPlanHasValue { get; set; }
        //end

        //PIR 14938
        public decimal idecTotalEEContributionCalculatedByPlan { get; set; }
        public decimal idecTotalERContributionCalculatedByPlan { get; set; }
        public decimal idecTotalRHICERContributionCalculatedByPlan { get; set; }
        public decimal idecTotalRHICEEContributionCalculatedByPlan { get; set; }
        public decimal idecTotalEEEmployerPickupCalculatedByPlan { get; set; }
        public decimal idecTotalEEPreTaxCalculatedByPlan { get; set; }
        public decimal idecTotalCalculatedRetrContributionByPlan { get; set; }
        public decimal idecTotalCalculatedRHICContributionByPlan { get; set; }

        private decimal _idecTotalCalculatedByPlan;
        public decimal idecTotalCalculatedByPlan
        {
            get { return _idecTotalCalculatedByPlan; }
            set { _idecTotalCalculatedByPlan = value; }
        }

        //PIR 15238 
        public decimal idecTotalNegAdjContributionCalculated { get; set; }
        public decimal idecTotalNegativeEEContributionCalculatedByPlan { get; set; }
        public decimal idecTotalNegativeERContributionCalculatedByPlan { get; set; }
        public decimal idecTotalNegativeEEPreTaxCalculatedByPlan { get; set; }
        public decimal idecTotalNegativeEEEmployerPickupCalculatedByPlan { get; set; }
        
        //PIR 15238 
        public decimal idecTotalNegativeCalculatedRHICContributionByPlan { get; set; }
        public decimal idecTotalNegativeRHICERContributionCalculatedByPlan { get; set; }
        public decimal idecTotalNegativeRHICEEContributionCalculatedByPlan { get; set; }

        public decimal idecTotalRHICEmployerInterestCalculatedByPlan { get; set; }
        //PIR 7705
        public decimal idecHSAAmount { get; set; }
        public decimal idecVendorAmount { get; set; }
        #endregion

        #region Deferred Comp Contribution Reported Properties by plan
        private decimal _idecTotalContributionAmount1ReportedByPlan;
        public decimal idecTotalContributionAmount1ReportedByPlan
        {
            get { return _idecTotalContributionAmount1ReportedByPlan; }
            set { _idecTotalContributionAmount1ReportedByPlan = value; }
        }
        private decimal _idecTotalContributionAmount2ReportedByPlan;
        public decimal idecTotalContributionAmount2ReportedByPlan
        {
            get { return _idecTotalContributionAmount2ReportedByPlan; }
            set { _idecTotalContributionAmount2ReportedByPlan = value; }
        }
        private decimal _idecTotalContributionAmount3ReportedByPlan;
        public decimal idecTotalContributionAmount3ReportedByPlan
        {
            get { return _idecTotalContributionAmount3ReportedByPlan; }
            set { _idecTotalContributionAmount3ReportedByPlan = value; }
        }
        private decimal _idecTotalContributionAmount4ReportedByPlan;
        public decimal idecTotalContributionAmount4ReportedByPlan
        {
            get { return _idecTotalContributionAmount4ReportedByPlan; }
            set { _idecTotalContributionAmount4ReportedByPlan = value; }
        }
        private decimal _idecTotalContributionAmount5ReportedByPlan;
        public decimal idecTotalContributionAmount5ReportedByPlan
        {
            get { return _idecTotalContributionAmount5ReportedByPlan; }
            set { _idecTotalContributionAmount5ReportedByPlan = value; }
        }
        private decimal _idecTotalContributionAmount6ReportedByPlan;
        public decimal idecTotalContributionAmount6ReportedByPlan
        {
            get { return _idecTotalContributionAmount6ReportedByPlan; }
            set { _idecTotalContributionAmount6ReportedByPlan = value; }
        }
        private decimal _idecTotalContributionAmount7ReportedByPlan;
        public decimal idecTotalContributionAmount7ReportedByPlan
        {
            get { return _idecTotalContributionAmount7ReportedByPlan; }
            set { _idecTotalContributionAmount7ReportedByPlan = value; }
        }
        #endregion

        public string PlanBenefitTypeForHeaderType
        {
            get
            {
                return busEmployerReportHelper.GetBenefitTypeForEmployerHeaderType(icdoEmployerPayrollHeader.header_type_value);
            }
        }

        public void LoadOrgCodeID()
        {
            if (_icdoEmployerPayrollHeader.org_id != 0)
            {
                if (ibusOrganization == null)
                    LoadOrganization();

                _istrOrgCodeId = ibusOrganization.icdoOrganization.org_code;
            }
        }

        public void LoadFileRecordType()
        {
            switch (_icdoEmployerPayrollHeader.report_type_value)
            {
                case busConstant.PayrollHeaderReportTypeRegular:
                    _iintFileRecordType = 1;
                    break;
                case busConstant.PayrollHeaderReportTypeAdjustment:
                    _iintFileRecordType = 2;
                    break;
            }
        }

        public void CalculateContributionWagesInterestFromDtl()
        {
            if (_iclbEmployerPayrollDetail == null)
            {
                LoadEmployerPayrollDetail();
            }
            //Resetting the Total Amount
            _idecTotalContributionCalculatedForRet = 0;
            _idecTotalWagesCalculated = 0;
            _idecTotalInterestCalculated = 0;
			//PIR 15238
            _idecTotalMemberInterestCalculated = 0;
            _idecTotalEmployerInterestCalculated = 0;
            _idecTotalEmployerRhicInterestCalculated = 0;
            _idecTotalContributionCalculatedForDef = 0;
            _idecTotalPremiumReportedForIns = 0;
            _idecTotalPurchaseAmount = 0;
            _idecTotalContributionFromDetails = 0;
			//PIR 25920 New Plan DC 2025
            _idecTotalADECCalculatedByPlan = 0;
            _idecTotalADECReportedByPlan = 0;
            
            idecTotalContributionCalculatedForOther457 = 0;
            foreach (busEmployerPayrollDetail lobjEmployerDeatil in _iclbEmployerPayrollDetail)
            {
                if (lobjEmployerDeatil.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusIgnored)
                {
                    if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        //Add the Contribution if detail entries are Regular / Bonus / +ADJ. Substract it for -ADJ
                        if ((lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular) ||
                            (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ||
                            (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment))
                        {
                            _idecTotalContributionCalculatedForRet +=
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_contribution_reported +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.er_contribution_reported +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pre_tax_reported +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.rhic_ee_contribution_reported +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.rhic_er_contribution_reported +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.adec_reported + 
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pretax_addl_reported +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_post_tax_addl_reported +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_reported;//PIR 25920 New Plan DC 2025

                            _idecTotalContributionFromDetails +=
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_contribution_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.er_contribution_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pre_tax_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_employer_pickup_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.rhic_ee_contribution_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.rhic_er_contribution_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.adec_calculated + 
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pretax_addl_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_post_tax_addl_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated;//PIR 25920 New Plan DC 2025

                            _idecTotalWagesCalculated += lobjEmployerDeatil.icdoEmployerPayrollDetail.eligible_wages;

                            _idecTotalInterestCalculated += lobjEmployerDeatil.icdoEmployerPayrollDetail.member_interest_calculated +
                                                            lobjEmployerDeatil.icdoEmployerPayrollDetail.employer_interest_calculated +
                                                            lobjEmployerDeatil.icdoEmployerPayrollDetail.employer_rhic_interest_calculated;
							
							//PIR 15238 
                            _idecTotalMemberInterestCalculated += lobjEmployerDeatil.icdoEmployerPayrollDetail.member_interest_calculated;
                            _idecTotalEmployerInterestCalculated += lobjEmployerDeatil.icdoEmployerPayrollDetail.employer_interest_calculated;
                            _idecTotalEmployerRhicInterestCalculated += lobjEmployerDeatil.icdoEmployerPayrollDetail.employer_rhic_interest_calculated;
                            //PIR 25920 New Plan DC 2025
                            _idecTotalADECCalculatedByPlan += lobjEmployerDeatil.icdoEmployerPayrollDetail.adec_calculated;
                            _idecTotalADECReportedByPlan += lobjEmployerDeatil.icdoEmployerPayrollDetail.adec_reported;
                            //_idecTotalEEPreTaxAddlCalculatedByPlan += lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pretax_addl_calculated;
                            //_idecTotalEEPostTaxAddlCalculatedByPlan += lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_post_tax_addl_calculated;
                        }
                        else if (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment ||
                                 lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)
                        {

                            _idecTotalContributionCalculatedForRet -=
                                    (lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_contribution_reported +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.er_contribution_reported +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pre_tax_reported +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.rhic_ee_contribution_reported +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.rhic_er_contribution_reported + 
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.adec_reported + 
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pretax_addl_reported +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_post_tax_addl_reported +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_reported);//PIR 25920 New Plan DC 2025

                            _idecTotalContributionFromDetails -=
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_contribution_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.er_contribution_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pre_tax_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_employer_pickup_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.rhic_ee_contribution_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.rhic_er_contribution_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.adec_calculated + 
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_pretax_addl_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.ee_post_tax_addl_calculated +
                                lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated;//PIR 25920 New Plan DC 2025

                            _idecTotalWagesCalculated -= lobjEmployerDeatil.icdoEmployerPayrollDetail.eligible_wages;

                            _idecTotalInterestCalculated -= (lobjEmployerDeatil.icdoEmployerPayrollDetail.member_interest_calculated +
                                                            lobjEmployerDeatil.icdoEmployerPayrollDetail.employer_interest_calculated +
                                                            lobjEmployerDeatil.icdoEmployerPayrollDetail.employer_rhic_interest_calculated);
								
							//PIR 15238 
                            _idecTotalMemberInterestCalculated -= lobjEmployerDeatil.icdoEmployerPayrollDetail.member_interest_calculated;
                            _idecTotalEmployerInterestCalculated -= lobjEmployerDeatil.icdoEmployerPayrollDetail.employer_interest_calculated;
                            _idecTotalEmployerRhicInterestCalculated -= lobjEmployerDeatil.icdoEmployerPayrollDetail.employer_rhic_interest_calculated;
                            //PIR 25920 New Plan DC 2025
                            _idecTotalADECCalculatedByPlan -= lobjEmployerDeatil.icdoEmployerPayrollDetail.adec_calculated;
                            _idecTotalADECReportedByPlan -= lobjEmployerDeatil.icdoEmployerPayrollDetail.adec_reported;
                        }
                        
                    }
                    else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    {
                        //Exclude Other 457 Amount
                        if (lobjEmployerDeatil.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457)
                        {
                            //Add the Contribution if detail entries are Regular / Bonus / +ADJ. Substract it for -ADJ
                            if ((lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular) ||
                                (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ||
                                (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment))
                            {
                                _idecTotalContributionCalculatedForDef +=
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount1 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount2 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount3 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount4 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount5 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount6 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount7 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated;
                                //PIR 25920 New Plan DC 2025
                                _idecTotalERPreTaxMatchCalculatedByPlan += lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated;
                            }
                            else if (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                            {
                                _idecTotalContributionCalculatedForDef -=
                                    (lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount1 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount2 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount3 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount4 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount5 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount6 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount7 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated);
                                //PIR 25920 New Plan DC 2025
                                _idecTotalERPreTaxMatchCalculatedByPlan -= lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated;
                            }
                            
                        }
                        //prod pir 4666
                        else
                        {
                            //Add the Contribution if detail entries are Regular / Bonus / +ADJ. Substract it for -ADJ
                            if ((lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular) ||
                                (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ||
                                (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment))
                            {
                                idecTotalContributionCalculatedForOther457 +=
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount1 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount2 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount3 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount4 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount5 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount6 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount7 +
                                    lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated;
                                //PIR 25920 New Plan DC 2025
                                _idecTotalERPreTaxMatchCalculatedByPlan += lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated;
                            }
                            else if (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                            {
                                idecTotalContributionCalculatedForOther457 -=
                                    (lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount1 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount2 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount3 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount4 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount5 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount6 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.contribution_amount7 +
                                     lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated);
                                //PIR 25920 New Plan DC 2025
                                _idecTotalERPreTaxMatchCalculatedByPlan -= lobjEmployerDeatil.icdoEmployerPayrollDetail.er_pretax_match_calculated;
                            }
                            
                        }
                        
                    }
                    else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                    {
                        if (lobjEmployerDeatil.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                        {
                            _idecTotalPremiumReportedForIns += (lobjEmployerDeatil.icdoEmployerPayrollDetail.premium_amount * -1M);
                        }
                        else
                        {
                            _idecTotalPremiumReportedForIns += lobjEmployerDeatil.icdoEmployerPayrollDetail.premium_amount;
                        }

                    }
                    else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                    {
                        _idecTotalPurchaseAmount += lobjEmployerDeatil.icdoEmployerPayrollDetail.purchase_amount_reported;
                    }
                }
            }

            //PIR 15238 
            _icdoEmployerPayrollHeader.TotalMemberInterestCalculated = _idecTotalMemberInterestCalculated;
            _icdoEmployerPayrollHeader.TotalEmployerInterestCalculated = _idecTotalEmployerInterestCalculated;
            _icdoEmployerPayrollHeader.TotalEmployerRhicInterestCalculated = _idecTotalEmployerRhicInterestCalculated;
            
            //if (_icdoEmployerPayrollHeader.total_adec_amount_reported > 0)
            //{
            //    decimal ldecTotalContributionPlusADEC = 0;
            //    ldecTotalContributionPlusADEC = _icdoEmployerPayrollHeader.total_contribution_reported + _icdoEmployerPayrollHeader.total_adec_amount_reported;
            //    _icdoEmployerPayrollHeader.total_contribution_reported = ldecTotalContributionPlusADEC;
            //}
        }

        public void LoadOrganization()
        {
            if (_ibusOrganization == null)
            {
                _ibusOrganization = new busOrganization();
            }
            _ibusOrganization.FindOrganization(_icdoEmployerPayrollHeader.org_id);
        }

        public void LoadPlan(int aintPlanID)
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(aintPlanID);
        }

        //used to delete header error
        public void LoadEmployerPayrollHeaderError()
        {
            DataTable ldtbList = Select<cdoEmployerPayrollHeaderError>(
                            new string[1] { "employer_payroll_header_id" },
                            new object[1] { _icdoEmployerPayrollHeader.employer_payroll_header_id }, null, null);
            _iclbEmployerPayrollHeaderError = GetCollection<busEmployerPayrollHeaderError>(ldtbList, "icdoEmployerPayrollHeaderError");
        }

        #region To Load Contribution By BenefitType And Plan
        public decimal idecOutstandingRetirementBalance { get; set; }
        public decimal idecOutstandingRetirementBalanceForACHPull { get; set; }
        // PIR 15639 - Addition of OutstandingRHICBalance field

        public decimal idecOutstandingRHICBalance { get; set; }
        //PIR 25920 DC 2025 changes
        public decimal idecOutstandingADECBalance { get; set; }
        public decimal idecOutstandingInsuranceBalance { get; set; }
        public decimal idecOutstandingDeferredCompBalance { get; set; }
        public decimal idecOutstandingAmount { get; set; }

        public void LoadRetirementContributionByPlan()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadRetirementContributionByPlan",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbRetirementContributionByPlan = new Collection<busEmployerPayrollHeader>();
            foreach (DataRow aTotalsRow in ldtbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader() { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
                sqlFunction.LoadQueryResult(lobjEmpHeader, aTotalsRow);
                _iclbRetirementContributionByPlan.Add(lobjEmpHeader);
            }

            //Reset the Total Reported By Plan
            _idecTotalReportedByPlan = 0;
            _idecTotalCalculatedByPlan = 0;

            foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbRetirementContributionByPlan)
            {
                lobjEmployerPayrollHeader.LoadPlan(lobjEmployerPayrollHeader.iintPlanID);
                lobjEmployerPayrollHeader.idecTotalReportedByPlan =
                    lobjEmployerPayrollHeader.idecTotalEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalRHICERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalRHICEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEEmployerPickupReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEmployerInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalRHICEmployerInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalADECReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxAddlReportedByPlan + 
                    lobjEmployerPayrollHeader.idecTotalEEPostTaxAddlReportedByPlan + 
                    lobjEmployerPayrollHeader.idecTotalERPreTaxMatchReportedByPlan;       //PIR 25920 New Plan DC 2025

                //PIR - 330
                lobjEmployerPayrollHeader.idecTotalReportedRetrContributionByPlan = lobjEmployerPayrollHeader.idecTotalEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEEmployerPickupReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEmployerInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalADECReportedByPlan + 
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxAddlReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPostTaxAddlReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERPreTaxMatchReportedByPlan;       //PIR 25920 New Plan DC 2025

                //Total Reported Retr Contribution Without ADECAmt
                lobjEmployerPayrollHeader.idecTotalReportedRetrContributionWithoutADECAmtByPlan = lobjEmployerPayrollHeader.idecTotalEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEEmployerPickupReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEmployerInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxAddlReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPostTaxAddlReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERPreTaxMatchReportedByPlan;       //PIR 25920 New Plan DC 2025

                //PIR 10779
                lobjEmployerPayrollHeader.idecTotalNegAdjContribution = 
				    lobjEmployerPayrollHeader.idecTotalNegativeEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEEPreTaxReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEEEmployerPickupReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEmployerInterestCalculatedByPlan;

                //PIR 15238 
                lobjEmployerPayrollHeader.idecTotalNegAdjContributionCalculated = lobjEmployerPayrollHeader.idecTotalNegativeEEContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeERContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeEEPreTaxCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeEEEmployerPickupCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeMemberInterestCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeEmployerInterestCalculatedByPlan;

                lobjEmployerPayrollHeader.idecTotalReportedRHICContributionByPlan = lobjEmployerPayrollHeader.idecTotalRHICERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalRHICEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalRHICEmployerInterestCalculatedByPlan;

                //PIR 14938
                lobjEmployerPayrollHeader.idecTotalCalculatedRetrContributionByPlan = lobjEmployerPayrollHeader.idecTotalEEContributionCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERContributionCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEEmployerPickupCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEmployerInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalADECCalculatedByPlan + 
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxAddlCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPostTaxAddlCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERPreTaxMatchCalculatedByPlan;       //PIR 25920 New Plan DC 2025

                //PIR 10779
                lobjEmployerPayrollHeader.idecTotalNegativeReportedRHICContributionByPlan = 
				    lobjEmployerPayrollHeader.idecTotalNegativeRHICERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeRHICEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeRHICEmployerInterestCalculatedByPlan;

                //PIR 15238 
                //PIR 14938
                lobjEmployerPayrollHeader.idecTotalCalculatedRHICContributionByPlan = lobjEmployerPayrollHeader.idecTotalRHICERContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalRHICEEContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalRHICEmployerInterestCalculatedByPlan;

                lobjEmployerPayrollHeader.idecTotalNegativeCalculatedRHICContributionByPlan = lobjEmployerPayrollHeader.idecTotalNegativeRHICERContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeRHICEEContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeRHICEmployerInterestCalculatedByPlan;

                //PIR 14938
                lobjEmployerPayrollHeader.idecTotalCalculatedByPlan = lobjEmployerPayrollHeader.idecTotalCalculatedRetrContributionByPlan +
                                                    lobjEmployerPayrollHeader.idecTotalCalculatedRHICContributionByPlan;

                //
                lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.ItemTypeContribution);

                lobjEmployerPayrollHeader.idecTotalAppliedRHICContributionByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.ItemTypeRHICContribution);
				
                lobjEmployerPayrollHeader.idecTotalADECAppliedByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.ItemTypeADECContribution);
                lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlanWithAdec = lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlan + lobjEmployerPayrollHeader.idecTotalADECAppliedByPlan;

                lobjEmployerPayrollHeader.idecOutstandingRetirementBalance =
				     lobjEmployerPayrollHeader.idecTotalReportedRetrContributionByPlan - 
                     lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlan -
                     lobjEmployerPayrollHeader.idecTotalNegAdjContribution -
                     lobjEmployerPayrollHeader.idecTotalADECAppliedByPlan;//PIR 10779 //PIR 25920

                lobjEmployerPayrollHeader.idecOutstandingRetirementBalanceForACHPull=
                    lobjEmployerPayrollHeader.idecTotalReportedRetrContributionWithoutADECAmtByPlan -
                     lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlan -
                     lobjEmployerPayrollHeader.idecTotalNegAdjContribution;//PIR 10779

                // PIR 15639 - Addition of OutstandingRHICBalance field
                lobjEmployerPayrollHeader.idecOutstandingRHICBalance =
				     lobjEmployerPayrollHeader.idecTotalReportedRHICContributionByPlan - 
                     lobjEmployerPayrollHeader.idecTotalAppliedRHICContributionByPlan -
                     lobjEmployerPayrollHeader.idecTotalNegativeReportedRHICContributionByPlan; //PIR 10779
                //PIR 25920 DC 2025 changes
                lobjEmployerPayrollHeader.idecOutstandingADECBalance = lobjEmployerPayrollHeader.idecTotalADECReportedByPlan -
                                                        lobjEmployerPayrollHeader.idecTotalADECAppliedByPlan;
                //Calculating Total Reported By All Plans
                _idecTotalReportedByPlan += lobjEmployerPayrollHeader.idecTotalReportedByPlan;
                //PIR 25920 DC 2025 Changes
                //icdoEmployerPayrollHeader.total_contribution_reported = lobjEmployerPayrollHeader._idecTotalReportedByPlan;
                _idecTotalCalculatedByPlan += lobjEmployerPayrollHeader.idecTotalCalculatedByPlan;

                //PIR 24399 Calculating all Contribution Amount Due and RHIC Amount Due for all plans.
                icdoEmployerPayrollHeader.idecOutstandingRetirementBalance += lobjEmployerPayrollHeader.idecOutstandingRetirementBalance;
                icdoEmployerPayrollHeader.idecOutstandingRHICBalance += lobjEmployerPayrollHeader.idecOutstandingRHICBalance;
                //PIR 25920 DC 2025 changes
                icdoEmployerPayrollHeader.idecOutstandingADECBalance += lobjEmployerPayrollHeader.idecOutstandingADECBalance;
            }
            //PIR 25920 DC 2025 Changes
            //_icdoEmployerPayrollHeader.total_contribution_reported = _icdoEmployerPayrollHeader.total_contribution_reported + _icdoEmployerPayrollHeader.total_adec_amount_reported;
            iblnTotalEEContributionReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalEEContributionReportedByPlan > 0);
            iblnTotalEEPreTaxReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalEEPreTaxReportedByPlan > 0);
            iblnTotalEEEmployerPickupReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalEEEmployerPickupReportedByPlan > 0);
            iblnTotalERContributionReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalERContributionReportedByPlan > 0);
            iblnTotalMemberInterestCalculatedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalMemberInterestCalculatedByPlan > 0);
            iblnTotalEmployerInterestCalculatedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalEmployerInterestCalculatedByPlan > 0);

            iblnTotalRHICEEContributionReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalRHICEEContributionReportedByPlan > 0);
            iblnTotalRHICERContributionReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalRHICERContributionReportedByPlan > 0);
            iblnTotalRHICEmployerInterestCalculatedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalRHICEmployerInterestCalculatedByPlan > 0);

            iblnTotalEEPreTaxAddlReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalEEPreTaxAddlReportedByPlan > 0);
            iblnTotalEEPostTaxAddlReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalEEPostTaxAddlReportedByPlan > 0);
            iblnTotalERPreTaxMatchReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalERPreTaxMatchReportedByPlan > 0);
            iblnTotalADECReportedByPlanHasValue = _iclbRetirementContributionByPlan.Any(objRetirementContributionByPlan => objRetirementContributionByPlan.idecTotalADECReportedByPlan > 0);

        }

        private decimal GetAppliedAmountByPlanAndRemittanceType(busEmployerPayrollHeader lobjEmployerPayrollHeader, string astrRemittanceType)
        {
            decimal ldecAppliedAmount = 0.00M;
            busEmployerRemittanceAllocation lobjRemittanceAllocation = new busEmployerRemittanceAllocation();
            lobjRemittanceAllocation.ibusEmployerPayrollHeader = this;
            lobjRemittanceAllocation.icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation();
            lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.employer_remittance_allocation_id = 0;
            ldecAppliedAmount = lobjRemittanceAllocation.GetTotalAppliedAmountByPlan(lobjEmployerPayrollHeader.iintPlanID, astrRemittanceType);
            return ldecAppliedAmount;
        }

        public void LoadDeferredCompContributionByPlan()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadDeferredCompContributionByPlan",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbDeferredCompContributionByPlan = new Collection<busEmployerPayrollHeader>();
            foreach (DataRow aTotalsRow in ldtbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader() { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id; ;
                sqlFunction.LoadQueryResult(lobjEmpHeader, aTotalsRow);
                _iclbDeferredCompContributionByPlan.Add(lobjEmpHeader);
            }

            //Reset the Total Reported By Plan
            _idecTotalReportedByPlan = 0;
            idecTotOutstandDefCompBal = 0;

            foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbDeferredCompContributionByPlan)
            {
                lobjEmployerPayrollHeader.LoadPlan(lobjEmployerPayrollHeader.iintPlanID);
                lobjEmployerPayrollHeader.idecTotalReportedByPlan =
                    lobjEmployerPayrollHeader.idecTotalContributionAmount1ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount2ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount3ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount4ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount5ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount6ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount7ReportedByPlan + 
                    lobjEmployerPayrollHeader.idecTotalERPreTaxMatchReportedByPlan;       //PIR 25920 New Plan DC 2025

                //Calculating Total Reported By All Plans
                //Excluding Other 457
                if (lobjEmployerPayrollHeader.iintPlanID != busConstant.PlanIdOther457)
                {
                    _idecTotalReportedByPlan += lobjEmployerPayrollHeader.idecTotalReportedByPlan;
                }

                lobjEmployerPayrollHeader.idecTotalAppliedContributionByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.ItemTypeDefeCompDeposit);
                lobjEmployerPayrollHeader.idecOutstandingDeferredCompBalance = lobjEmployerPayrollHeader.idecTotalReportedByPlan - lobjEmployerPayrollHeader.idecTotalAppliedContributionByPlan;

                if (lobjEmployerPayrollHeader.iintPlanID != busConstant.PlanIdOther457)
                {
                    idecTotOutstandDefCompBal  += lobjEmployerPayrollHeader.idecOutstandingDeferredCompBalance;
                }
            }
        }

        public void LoadInsurancePremiumByPlan()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadInsurancePremiumByPlan",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbInsuranceContributionByPlan = new Collection<busEmployerPayrollHeader>();
            foreach (DataRow aTotalsRow in ldtbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader=new cdoEmployerPayrollHeader()};
                lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
                sqlFunction.LoadQueryResult(lobjEmpHeader, aTotalsRow);
                _iclbInsuranceContributionByPlan.Add(lobjEmpHeader);
            }

            //Reset the Total Reported By Plan
            _idecTotalReportedByPlan = 0;

            foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbInsuranceContributionByPlan)
            {
                if (lobjEmployerPayrollHeader.ibusPlan == null)
                    lobjEmployerPayrollHeader.LoadPlan(lobjEmployerPayrollHeader.iintPlanID);
                lobjEmployerPayrollHeader.idecTotalReportedByPlan = lobjEmployerPayrollHeader.idecTotalPremiumAmountByPlan;
                lobjEmployerPayrollHeader.idecTotalFeeAmountByPlan = lobjEmployerPayrollHeader.idecTotalFeeAmountByPlan;
                lobjEmployerPayrollHeader.idecTotalBuydownAmountByPlan = lobjEmployerPayrollHeader.idecTotalBuydownAmountByPlan; // PIR 11239
                lobjEmployerPayrollHeader.idecTotalMedicarePartDAmtByPlan = lobjEmployerPayrollHeader.idecTotalMedicarePartDAmtByPlan;// PIR 14271
                lobjEmployerPayrollHeader.idecJSRHICAmount = lobjEmployerPayrollHeader.idecJSRHICAmount;
                lobjEmployerPayrollHeader.idecOtherRHICAmount = lobjEmployerPayrollHeader.idecOtherRHICAmount;

                //Calculating Total Reported By All Plans
                _idecTotalReportedByPlan += lobjEmployerPayrollHeader.idecTotalReportedByPlan;

                string lstrRemittanceType = busEmployerReportHelper.GetToItemTypeByPlan(lobjEmployerPayrollHeader.ibusPlan.icdoPlan.plan_code);
                lobjEmployerPayrollHeader.idecTotalAppliedPremiumByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, lstrRemittanceType);
                lobjEmployerPayrollHeader.idecOutstandingInsuranceBalance = lobjEmployerPayrollHeader.idecTotalReportedByPlan - lobjEmployerPayrollHeader.idecTotalAppliedPremiumByPlan;
            }
        }

        public void LoadPurchaseByPlan()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadPurchaseByPlan",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbPurchaseContributionByPlan = new Collection<busEmployerPayrollHeader>();
            foreach (DataRow aTotalsRow in ldtbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader() { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
                sqlFunction.LoadQueryResult(lobjEmpHeader, aTotalsRow);
                _iclbPurchaseContributionByPlan.Add(lobjEmpHeader);
            }

            //Reset the Total Reported By Plan
            _idecTotalReportedByPlan = 0;

            foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbPurchaseContributionByPlan)
            {
                lobjEmployerPayrollHeader.LoadPlan(lobjEmployerPayrollHeader.iintPlanID);
                lobjEmployerPayrollHeader.idecTotalReportedByPlan = lobjEmployerPayrollHeader.idecTotalReportedPurchaseByPlan;

                //Calculating Total Reported By All Plans
                _idecTotalReportedByPlan += lobjEmployerPayrollHeader.idecTotalReportedByPlan;

                lobjEmployerPayrollHeader.idecTotalAppliedPurchaseByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.FromItemTypePurchases);
                lobjEmployerPayrollHeader.idecOutstandingAmount = lobjEmployerPayrollHeader.idecTotalReportedByPlan - lobjEmployerPayrollHeader.idecTotalAppliedPurchaseByPlan;

                icdoEmployerPayrollHeader.idecOutstandingAmount += lobjEmployerPayrollHeader.idecTotalReportedByPlan - lobjEmployerPayrollHeader.idecTotalAppliedPurchaseByPlan;
            }
        }

        public decimal GetTotalReportedAmountByPlan(int aintPlanID, string astrRemittanceType)
        {
            decimal ldecTotalAmount = 0;
            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                foreach (busEmployerPayrollHeader lobjEmpPayrollHeader in iclbRetirementContributionByPlan)
                {
                    if (lobjEmpPayrollHeader.ibusPlan.icdoPlan.plan_id == aintPlanID)
                    {
                        if (astrRemittanceType == busConstant.ItemTypeContribution)
                        {
                            //Get the Total Amount for Remittance Type "CONTRIBUTION"
                            ldecTotalAmount = lobjEmpPayrollHeader.idecTotalEEContributionReportedByPlan +
                                              lobjEmpPayrollHeader.idecTotalERContributionReportedByPlan +
                                              lobjEmpPayrollHeader.idecTotalEEPreTaxReportedByPlan +
                                              lobjEmpPayrollHeader.idecTotalEEEmployerPickupReportedByPlan +
                                              lobjEmpPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                                              lobjEmpPayrollHeader.idecTotalEmployerInterestCalculatedByPlan +
                                              //lobjEmpPayrollHeader.idecTotalADECReportedByPlan + 
                                              lobjEmpPayrollHeader.idecTotalEEPreTaxAddlReportedByPlan + 
                                              lobjEmpPayrollHeader.idecTotalEEPostTaxAddlReportedByPlan + 
                                              lobjEmpPayrollHeader.idecTotalERPreTaxMatchReportedByPlan; //PIR 25920 DC 2025 
                        }
                        else if (astrRemittanceType == busConstant.ItemTypeRHICContribution)
                        {
                            ldecTotalAmount = lobjEmpPayrollHeader.idecTotalRHICEEContributionReportedByPlan +
                                              lobjEmpPayrollHeader.idecTotalRHICERContributionReportedByPlan +
                                              lobjEmpPayrollHeader.idecTotalRHICEmployerInterestCalculatedByPlan; //PIR-18755
                        }
                        else if (astrRemittanceType == busConstant.ItemTypeADECContribution)
                        {
                            ldecTotalAmount = lobjEmpPayrollHeader.idecTotalADECReportedByPlan; //PIR-25920 DC 2025 Changes
                        }
                        break;
                    }
                }
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                foreach (busEmployerPayrollHeader lobjEmpPayrollHeader in iclbDeferredCompContributionByPlan)
                {
                    if (lobjEmpPayrollHeader.ibusPlan.icdoPlan.plan_id == aintPlanID)
                    {
                        ldecTotalAmount = lobjEmpPayrollHeader.idecTotalReportedByPlan;
                        break;
                    }
                }
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                foreach (busEmployerPayrollHeader lobjEmpPayrollHeader in iclbInsuranceContributionByPlan)
                {
                    if (lobjEmpPayrollHeader.ibusPlan.icdoPlan.plan_id == aintPlanID)
                    {
                        ldecTotalAmount = lobjEmpPayrollHeader.idecTotalReportedByPlan;
                        break;
                    }
                }
            }
            return ldecTotalAmount;
        }

        #endregion

        //To Load Remittance Allocation Grid in other details
        public void LoadEmployerRemittanceAllocation()
        {
            DataTable ldtbList = Select<cdoEmployerRemittanceAllocation>(
                new string[1] { "employer_payroll_header_id" },
                new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id }, null, null);
            _iclbEmployerRemittanceAllocation = GetCollection<busEmployerRemittanceAllocation>(ldtbList, "icdoEmployerRemittanceAllocation");
            foreach (busEmployerRemittanceAllocation lobjEmployerRemittanceAllocation in iclbEmployerRemittanceAllocation)
            {
                lobjEmployerRemittanceAllocation.LoadRemittance();
                lobjEmployerRemittanceAllocation.ibusRemittance.LoadDeposit();
                lobjEmployerRemittanceAllocation.ibusRemittance.LoadDepositTape(); // PROD PIR ID 5265
                //prod pir 5358 : load plan name
                lobjEmployerRemittanceAllocation.ibusRemittance.LoadPlanName();
            }
        }

        public void LoadContributionByPlan()
        {
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                LoadRetirementContributionByPlan();
            }
            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                LoadDeferredCompContributionByPlan();
            }
            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                LoadInsurancePremiumByPlan();
            }
            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                LoadPurchaseByPlan();
            }
        }

        public void LoadStatusSummary()
        {
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.StatusSummary",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbStatusSummary = new Collection<busStatusSummary>();
            foreach (DataRow ldtrStatusSummary in ldtbList.Rows)
            {
                busStatusSummary lobjStatusSummary = new busStatusSummary();
                sqlFunction.LoadQueryResult(lobjStatusSummary, ldtrStatusSummary);
                _iclbStatusSummary.Add(lobjStatusSummary);
            }
        }
        public Collection<busStatusSummary> iclbStatusSummaryESS { get; set; }
        public void LoadStatusSummaryESS()
        {
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.ESSStatusSummary",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            iclbStatusSummaryESS = new Collection<busStatusSummary>();
            foreach (DataRow ldtrStatusSummary in ldtbList.Rows)
            {
                busStatusSummary lobjStatusSummary = new busStatusSummary();
                sqlFunction.LoadQueryResult(lobjStatusSummary, ldtrStatusSummary);
                iclbStatusSummaryESS.Add(lobjStatusSummary);
            }
        }
        public void LoadDetailError()
        {
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.ErrorSummaryByID", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbDetailError = new Collection<busErrorSummary>();
            foreach (DataRow drList in ldtbList.Rows)
            {
                busErrorSummary lobjErrors = new busErrorSummary();
                sqlFunction.LoadQueryResult(lobjErrors, drList);
                _iclbDetailError.Add(lobjErrors);
            }
        }

        public void LoadErrorLOB()
        {
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.ErrorSummaryByIDLOB", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            iclbErrorLOB = GetCollection<busEmployerPayrollHeaderError>(ldtbList, "icdoEmployerPayrollHeaderError");
        }

        public void LoadEmployerPayrollDetail()
        {
            DataTable ldtbList = Select<cdoEmployerPayrollDetail>(
                            new string[1] { "employer_payroll_header_id" },
                            new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id }, null, null);
            _iclbEmployerPayrollDetail = GetCollection<busEmployerPayrollDetail>(ldtbList, "icdoEmployerPayrollDetail");
        }
        public void LoadEmployerPayrollDetailWithPersonAccountAndOtherDetail()
        {
            _iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.LoadEmployerPayrollDetailWithPersonAccountAndOtherDetail", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            foreach (DataRow ldtrow in ldtbList.Rows)
            {
                busEmployerPayrollDetail lobjEmployerPayrollDetail = new busEmployerPayrollDetail { icdoEmployerPayrollDetail =new cdoEmployerPayrollDetail() };
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.LoadData(ldtrow);
                lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = this;
                lobjEmployerPayrollDetail.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.LoadData(ldtrow);
                lobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
                lobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.LoadData(ldtrow);
                _iclbEmployerPayrollDetail.Add(lobjEmployerPayrollDetail);
            }
           
        }
        //PIR 6906
        public int LoadEmployerPayrollDetailCount()
        {
            int lDetailCount=0;
            DataTable ldtbDetailCount = Select("cdoEmployerPayrollHeader.EmployerPayrollDetailCount", 
                                        new object[1] {icdoEmployerPayrollHeader.employer_payroll_header_id});
            lDetailCount = Convert.ToInt32(ldtbDetailCount.Rows[0]["DTLCOUNT"]);
            return lDetailCount;
        }

        public bool IsPayPeriodValid()
        {
            DataTable ldtbDetailCount = Select("cdoEmployerPayrollHeader.IsPayPeriodValid",
                                        new object[6] { Convert.ToDateTime(icdoEmployerPayrollHeader.pay_period).Month, Convert.ToDateTime(icdoEmployerPayrollHeader.pay_period).Year, icdoEmployerPayrollHeader.org_id, icdoEmployerPayrollHeader.header_type_value, icdoEmployerPayrollHeader.report_type_value,icdoEmployerPayrollHeader.employer_payroll_header_id });
            if(ldtbDetailCount.Rows.Count>0)
                return false;
            else
                return true;
        }
        public bool IsPayPeriodValidForUpload()
        {
            DataTable ldtbDetailCount = Select("cdoEmployerPayrollHeader.IsPayPeriodValid",
                                        new object[6] { Convert.ToDateTime(icdoEmployerPayrollHeader.istrPayrollPaidDateAsMonthYear).Month, Convert.ToDateTime(icdoEmployerPayrollHeader.istrPayrollPaidDateAsMonthYear).Year, icdoEmployerPayrollHeader.org_id, icdoEmployerPayrollHeader.header_type_value, icdoEmployerPayrollHeader.report_type_value, icdoEmployerPayrollHeader.employer_payroll_header_id });
            if (ldtbDetailCount.Rows.Count > 0)
                return false;
            else
                return true;
        }

        public void LoadEmployerPayrollDetailWithHeader()
        {
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.LoadEmployerPayrollDetailWithHeader",
                                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            foreach (DataRow ldtrRow in ldtbList.Rows)
            {
                busEmployerPayrollDetail lobjDetail = new busEmployerPayrollDetail
                {
                    icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail(),
                    ibusPlan = new busPlan { icdoPlan = new cdoPlan() },
                    ibusEmployerPayrollHeader = new busEmployerPayrollHeader
                    {
                        icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader(),
                        ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() }
                    }
                };
                lobjDetail.icdoEmployerPayrollDetail.LoadData(ldtrRow);
                lobjDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldtrRow);
                lobjDetail.ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.LoadData(ldtrRow);
                lobjDetail.ibusPlan.icdoPlan.LoadData(ldtrRow);
                _iclbEmployerPayrollDetail.Add(lobjDetail);
            }
        }

        //Optimization Load
        public void LoadEmployerPayrollDetailWithOtherObjects()
        {
            iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.LoadEmployerPayrollDetailWithOtherData", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                busEmployerPayrollDetail lbusEmpPayDetail = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                lbusEmpPayDetail.icdoEmployerPayrollDetail.LoadData(ldrRow);
                lbusEmpPayDetail.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusEmpPayDetail.ibusPerson.icdoPerson.LoadData(ldrRow);
                lbusEmpPayDetail.ibusPerson.icdoPerson.first_name = lbusEmpPayDetail.ibusPerson.icdoPerson.first_name_from_person; // first name and last name fields are loaded from person table - PIR 14166
                lbusEmpPayDetail.ibusPerson.icdoPerson.last_name = lbusEmpPayDetail.ibusPerson.icdoPerson.last_name_from_person; // first name and last name fields are loaded from person table - PIR 14166
                lbusEmpPayDetail.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusEmpPayDetail.ibusPlan.icdoPlan.LoadData(ldrRow);
                iclbEmployerPayrollDetail.Add(lbusEmpPayDetail);
            }
        }

        public DataTable idtbAllPersonAccounts { get; set; }
        public void LoadAllPersonAccounts()
        {
            idtbAllPersonAccounts = Select("cdoEmployerPayrollHeader.LoadAllPersonAccount", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
        }

        public DataTable idtbAllPAEmpDetailWithChildData { get; set; }
        public void LoadAllPAEmpDetailWithChildData()
        {
            idtbAllPAEmpDetailWithChildData = Select("cdoEmployerPayrollHeader.LoadAllPAEmpDetailChildData", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
        }

        public Collection<busPlanMemberTypeCrossref> iclbAllPlanMemberTypeCrossref { get; set; }
        public void LoadAllPlanMemberTypeCrossRef()
        {
            DataTable ldtbList = Select<cdoPlanMemberTypeCrossref>(new string[0] { }, new object[0] { }, null, null);
            iclbAllPlanMemberTypeCrossref = GetCollection<busPlanMemberTypeCrossref>(ldtbList, "icdoPlanMemberTypeCrossref");
        }

        public Collection<busOrgPlan> iclbAllOrgPlans { get; set; }
        public void LoadAllOrgPlans()
        {
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.LoadAllOrgPlans", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            iclbAllOrgPlans = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");
        }

        public DataTable idtbAllOrgPlanMemberType { get; set; }
        public void LoadAllOrgPlanMemberType()
        {
            idtbAllOrgPlanMemberType = Select("cdoEmployerPayrollHeader.LoadAllOrgPlanMemberType", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
        }

        /// <summary>
        /// Function to Load all the available remittance that can be applied to this payroll header
        /// 1) Loop through all the Applied Status Remittance and add only if the avialable amount
        /// is greater than zero
        /// </summary>
        public void LoadAvailableRemittanace()
        {
            string lstrBenefitType = busEmployerReportHelper.GetBenefitTypeForEmployerHeaderType(icdoEmployerPayrollHeader.header_type_value);
            DataTable ldtbAppliedRemittance = busNeoSpinBase.Select("cdoRemittance.GetAppliedRemittance", new object[2] { 
                        lstrBenefitType ?? string.Empty,
                        _icdoEmployerPayrollHeader.org_id });
            Collection<busRemittance> icolAppliedRemittance = new Collection<busRemittance>();
            icolAppliedRemittance = GetCollection<busRemittance>(ldtbAppliedRemittance, "icdoRemittance");
            _iclbAvailableRemittance = new Collection<busRemittance>();
            foreach (busRemittance lobjremittance in icolAppliedRemittance)
            {
                //PIR 413 - Skip the Seminar Remittance Type
                if (lobjremittance.icdoRemittance.remittance_type_value != busConstant.RemittanceTypeSeminar)
                {
                    lobjremittance.ldclBalanceAmount = busEmployerReportHelper.GetRemittanceAvailableAmount(lobjremittance.icdoRemittance.remittance_id);
                    if (lobjremittance.ldclBalanceAmount > 0)
                    {
                        //Load the Plan Name
                        lobjremittance.LoadPlanName();
                        lobjremittance.LoadDepositTape(); // PROD PIR ID 5265
                        _iclbAvailableRemittance.Add(lobjremittance);
                    }
                }
            }
        }

        //Calculate Applied Amount to this header to display in allocation panel
        public void LoadTotalAppliedRemittance()
        {
            _idecTotalRemittanceApplied = 0.00M;
            DataTable ldtbAppliedAmountList =
                Select<cdoEmployerRemittanceAllocation>(
                    new string[2] { "employer_payroll_header_id", "payroll_allocation_status_value" },
                    new object[2]
                        {
                            icdoEmployerPayrollHeader.employer_payroll_header_id,
                            busConstant.RemittanceAllocationStatusAllocated
                        }, null, null);
            if (ldtbAppliedAmountList.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbAppliedAmountList.Rows)
                {
                    if (!(String.IsNullOrEmpty(dr["allocated_amount"].ToString())))
                    {
                        _idecTotalRemittanceApplied = _idecTotalRemittanceApplied + Convert.ToDecimal(dr["allocated_amount"]);
                    }
                }
            }
        }

        /// <summary>
        /// Validate the Payroll Header & Detail and updated the status
        /// </summary>
        public void btnValidate_Click()
        {
            if (_iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
            //Set the Validate Detail Boolean Property to True
            iblnValidateDetail = true;
            SetInterestWaiverFlag(); //PIR 26688
            ValidateSoftErrors();
            UpdateValidateStatus();
            iblnValidateDetail = false;
        }

        /// <summary>
        /// Update the Payroll Header Status to Ignored
        /// </summary>
        /// <returns></returns>
        public ArrayList btnIgnore_Click()
        {
            //pir 6037
            ArrayList larrList = new ArrayList();
            
            int idecAllRemCount =  Convert.ToInt32(DBFunction.DBExecuteScalar("cdoEmployerPayrollHeader.GetCountofRemittanceAllocatedByHeaderID",
                                                    new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id },
                                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            if(idecAllRemCount > 0 )
            {
                utlError lutleror = new utlError();
                lutleror= AddError(9998, "");
                larrList.Add(lutleror);
                return larrList;
            }
            else
            {

                DeleteErrors();

                foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _iclbEmployerPayrollDetail)
                {
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusIgnored;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.Update();
                }
                _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusIgnored;
                //prod pir 6808 : need to change the header status to balanced if header is ignored
                _icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusBalanced;
                _icdoEmployerPayrollHeader.Update();
                return larrList;
            }
        }

        private void DeleteErrors()
        {
            //delete all errors from header
            DBFunction.DBNonQuery("cdoEmployerPayrollHeader.DeleteErrorsFromEmployerPayrollHeader", new object[1] { _icdoEmployerPayrollHeader.employer_payroll_header_id },
                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //delete all errors from detail
            DBFunction.DBNonQuery("cdoEmployerPayrollHeader.DeleteErrorsFromEmployerPayrollDetail", new object[1] { _icdoEmployerPayrollHeader.employer_payroll_header_id },
                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }


        public ArrayList btnAcceptReport_Click()
        {
            ArrayList larrresult = new ArrayList();
            iarrErrorList = new ArrayList();// pir 6939
            utlError lobjError = new utlError();
            iblnContactNDPERSFlag = false;// pir 6939
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                //PIR - 112               
                if (_icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular &&
                    _icdoEmployerPayrollHeader.last_reload_run_date.Date != DateTime.Today.Date)
                {
                    lobjError = AddError(4730, "");
                    iarrErrorList.Add(lobjError);
                    return iarrErrorList;
                }
                if (icdoEmployerPayrollHeader.total_premium_amount_reported != idecTotalPremiumReportedForIns)
                {
                    lobjError = AddError(4751, "");
                    iarrErrorList.Add(lobjError);
                    return iarrErrorList;
                }
            }

            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                //PIR 1854
                if (EvaluateRule("IsReportedTotalWagesMatchWithCalculated"))
                {
                    lobjError = AddError(4734, "");
                    iarrErrorList.Add(lobjError);
                    return iarrErrorList;
                }
            }

            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                //PROD PIR 4132
                if (icdoEmployerPayrollHeader.total_contribution_reported != icdoEmployerPayrollHeader.total_contribution_calculated)
                {
                    lobjError = AddError(4742, "");
                    iarrErrorList.Add(lobjError);
                    return iarrErrorList;
                }
            }

            if (_icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular)
            {
                if ((_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt && _icdoEmployerPayrollHeader.total_interest_calculated == 0) ||
                    (_icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusBalanced &&
                    (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp ||
                    _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)) ||
                    icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr ||
                    (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp &&
                    IsDetailContainOnlyOther457()))//prod pir 4064
                {
                    icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReadyToPost;
                }
                else
                {
                    icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusValid;
                    iblnContactNDPERSFlag = true;// pir 6939
                }
            }
            else if (icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment)
            {
                icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusValid;
                iblnContactNDPERSFlag = true;// pir 6939
            }
            //prod pir 5709
            //Update the Balancing Status If Any changes in the Reported Amount
            UpdateBalancingStatus();
            icdoEmployerPayrollHeader.Update();
            larrresult.Add(this);
            return larrresult;
        }
        public ArrayList btnReadyToPost_Click()
        {
            ArrayList larrresult = new ArrayList();
            utlError lobjError = new utlError();

            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                //PIR - 112               
                if (_icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular &&
                    _icdoEmployerPayrollHeader.last_reload_run_date.Date != DateTime.Today.Date)
                {
                    lobjError = AddError(4730, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                if (icdoEmployerPayrollHeader.total_premium_amount_reported != idecTotalPremiumReportedForIns)
                {
                    lobjError = AddError(4751, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
            }

            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                //PIR 1854
                if (EvaluateRule("IsReportedTotalWagesMatchWithCalculated"))
                {
                    lobjError = AddError(4734, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
            }

            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                //PROD PIR 4132
                if (icdoEmployerPayrollHeader.total_contribution_reported != icdoEmployerPayrollHeader.total_contribution_calculated)
                {
                    lobjError = AddError(4742, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
            }

            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                //As per mail from client dated Dec 28, 2010 SUB :- FW: Header 396 PROD PIR 5350
                //Below method looks into contract amount and as per discussion with Satya, we need to just look at total purchase amount and reported amount
                /*
                if (CompareTotalBalanceContractAmtAndReportedAmt() == false)
                {
                    lobjError = AddError(1323, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }*/

                if (icdoEmployerPayrollHeader.total_purchase_amount != icdoEmployerPayrollHeader.total_purchase_amount_reported)
                {
                    lobjError = AddError(1323, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
            }
            //PIR 26688
            SetInterestWaiverFlag();
            ValidateSoftErrors();
            //prod pir 5709
            //Update the Balancing Status If Any changes in the Reported Amount
            UpdateBalancingStatus();
            _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReadyToPost;
            _icdoEmployerPayrollHeader.Update();

            larrresult.Add(this);
            return larrresult;
        }
        public bool iblnIsDetailCountNone { get; set; }
        public ArrayList ExportToText_Click()
        {
            busProcessOutboundFile lobjProcessfiles = new busProcessOutboundFile();
            lobjProcessfiles.iarrParameters = new object[1];
            string astrFileName = string.Empty, istrGeneratedPath = string.Empty, istrImagedPath = string.Empty, lstrSourcePath = string.Empty, lstrDestinationPath=string.Empty;
            ArrayList larrresult = new ArrayList();
            utlError lobjError = new utlError();
            this.iblnHasErrors = false;

            // Code Added for performance optimization
            if ((iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0))
            {
                foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                {
                    if (lobjPayrollDetail.ibusEmployerPayrollHeader == null)
                    {
                        lobjPayrollDetail.ibusEmployerPayrollHeader = this;
                    }
                }
            }

            if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0 && astrIsDetailPopulated == busConstant.Flag_No_CAPS)
            {
                foreach (busEmployerPayrollDetail lbusEmployerPayrollDetail in iclbEmployerPayrollDetail)
                {
                    if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id_display;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn_display;
                    }
                    else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    {
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn_display_defComp;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name_display = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name_display_defComp;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name_display = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name_display_defComp;
                    }
                }
            }

            #region Validations
            #region Common Validation For Adjustment and regular Header & Detail
            //Total Field Validation
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                if (string.IsNullOrEmpty(icdoEmployerPayrollHeader.istrTotalWagesReported) || string.IsNullOrEmpty(icdoEmployerPayrollHeader.istrTotalContributionsReported))
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(10254, "");
                    lobjError.istrFocusControl = "Empty";
                    lobjError.istrFocusControls = "txtwfmESSActiveEmployerPayrollHeaderMaintenanceTotalWagesReported;txtwfmESSActiveEmployerPayrollHeaderMaintenanceTotalContributionReported";
                    
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                else
                {
                    if (icdoEmployerPayrollHeader.istrTotalWagesReported != "0.00")
                    {
                        decimal ldecTotalWagesReported;
                        if (decimal.TryParse(icdoEmployerPayrollHeader.istrTotalWagesReported, out ldecTotalWagesReported))
                        {
                            icdoEmployerPayrollHeader.total_wages_reported = ldecTotalWagesReported;
                        }
                        else
                        {
                            this.iblnHasErrors = true;
                            lobjError = AddError(0, "Enter a Valid total wages reported.");
                            larrresult.Add(lobjError);
                            return larrresult;
                        }
                    }
                    if (icdoEmployerPayrollHeader.istrTotalContributionsReported != "0.00")
                    {
                        decimal ldecTotalContributionsReported;
                        if (decimal.TryParse(icdoEmployerPayrollHeader.istrTotalContributionsReported, out ldecTotalContributionsReported))
                        {
                            icdoEmployerPayrollHeader.total_contribution_reported = ldecTotalContributionsReported;
                        }
                        else
                        {
                            this.iblnHasErrors = true;
                            lobjError = AddError(0, "Enter a Valid total contribution reported.");
                            larrresult.Add(lobjError);
                            return larrresult;
                        }
                    }
                }
            }
            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0)
                {
                    foreach (busEmployerPayrollDetail lbusEmployerPayrollDetail in iclbEmployerPayrollDetail)
                    {
                        if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id == 0)
                            lbusEmployerPayrollDetail.ibusPerson = GetPersonNameBySSN(lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn);
                        if (lbusEmployerPayrollDetail.ibusPerson.IsNull()) lbusEmployerPayrollDetail.LoadPerson();
                        if (lbusEmployerPayrollDetail.ibusPersonAccount.IsNull()) lbusEmployerPayrollDetail.LoadPersonAccount();
                        
                        busPersonAccount lbusPersonAccount = lbusEmployerPayrollDetail.ibusPersonAccount;
                        if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_start_date == DateTime.MinValue)
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_start_date = _icdoEmployerPayrollHeader.pay_period_start_date;
                        if ((lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.eligible_wages_defcomp == 0.00m || lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported == 0.00m)
                            && (lbusEmployerPayrollDetail.IsAnyOneContributionAvailableDefComp() && lbusEmployerPayrollDetail.IsAnyOneProviderAvailableDefComp()) 
                            && lbusEmployerPayrollDetail.ibusPersonAccount.IsEmployerMatchAvailableWithElection(lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_start_date))
                            //&& lbusEmployerPayrollDetail.IsEmployerMatchPlanRateAvailable)
                        {
                            if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.eligible_wages_defcomp == 0.00m)
                            {
                                this.iblnHasErrors = true;
                                lobjError = AddError(10513, "");
                                larrresult.Add(lobjError);
                                return larrresult;
                            }
                            if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported == 0.00m)
                            {
                                this.iblnHasErrors = true;
                                lobjError = AddError(10522, "");
                                larrresult.Add(lobjError);
                                return larrresult;
                            }
                        }
                    }
                }
            }

            if (iblnIsFromESS && _icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular && 
                           iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0)
            {
                if (iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.eligible_wages < 0).Any())
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(10257, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                //PIR 25920 DC 2025 For Def comp need positive eligible wages 
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    if (iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.eligible_wages_defcomp < 0).Any())
                    {
                        this.iblnHasErrors = true;
                        lobjError = AddError(10257, "");
                        larrresult.Add(lobjError);
                        return larrresult;
                    }
                }
            }
                //Commented as total field validations should only apply to retirement
                //else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                //{
                //    if (string.IsNullOrEmpty(icdoEmployerPayrollHeader.istrTotalContributionsReported))
                //    {
                //        this.iblnHasErrors = true;
                //        lobjError = AddError(10255, "");
                //        larrresult.Add(lobjError);
                //        return larrresult;
                //    }
                //    else
                //    {
                //        if (icdoEmployerPayrollHeader.istrTotalContributionsReported != "0.00")
                //        {
                //            decimal ldecTotalContributionsReported;
                //            if (decimal.TryParse(icdoEmployerPayrollHeader.istrTotalContributionsReported, out ldecTotalContributionsReported))
                //            {
                //                icdoEmployerPayrollHeader.total_contribution_reported = ldecTotalContributionsReported;
                //            }
                //            else
                //            {
                //                this.iblnHasErrors = true;
                //                lobjError = AddError(0, "Enter a Valid contribution reported.");
                //                larrresult.Add(lobjError);
                //                return larrresult;
                //            }
                //        }
                //    }
                //}
                //else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                //{
                //    if (icdoEmployerPayrollHeader.total_purchase_amount_reported == 0M)
                //    {
                //        this.iblnHasErrors = true;
                //        lobjError = AddError(10256, "");
                //        larrresult.Add(lobjError);
                //        return larrresult;
                //    }
                //}
                #endregion
                #region Adjustment Validation Saperate No and yes Cases
                if (_icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollHeaderReportTypeRegular)
            {
                if ((iclbEmployerPayrollDetail == null) || (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count==0))
                {
                        this.iblnHasErrors = true;
                        lobjError = AddError(10240, "");
                        larrresult.Add(lobjError);
                        return larrresult;  
                }
               
               
                String astrRequiredFieldRows = string.Empty;
                //PIR 13996
                string lstrReportingMonthRows = string.Empty;
                string lstrBeginMonthRows = string.Empty;
                string lstrPositiveRows = string.Empty;
                string lstrEndMonthRows = string.Empty;
                string lstrReportingMonthInvalidRows = string.Empty;
                string lstrEndMonthInvalidRows = string.Empty;
                //Retirement Bonus Retro Pay for all employees scenario
                if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0 &&
                    _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
                    _icdoEmployerPayrollHeader.report_type_description == "Bonus/Retro Pay" &&
                    astrIsDetailPopulated == busConstant.Flag_Yes_CAPS)
                {
                    //atleast one row with wages should be present
                    if (iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.eligible_wages != 0M).Count() == 0)
                    {
                        this.iblnHasErrors = true;
                        lobjError = AddError(10240, "");
                        larrresult.Add(lobjError);
                        return larrresult;
                    }
                    if (icdoEmployerPayrollHeader.total_wages_reported < 0M || icdoEmployerPayrollHeader.total_contribution_reported < 0M)
                    {
                        this.iblnHasErrors = true;
                        lobjError = AddError(10260, "");
                        larrresult.Add(lobjError);
                        return larrresult;
                    }
                    //PIR 13996
                    //show hard error if reporting month is less than Jan 1977 
                    int i = 0;
                    foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                    {
                        i++;
                        //Checking if eligible_wages is given then oly apply validation
                        if (lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages != 0M)
                        {
                            if (string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period) ||
                                 string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month))
                            {
                                if (string.IsNullOrEmpty(astrRequiredFieldRows))
                                {
                                    astrRequiredFieldRows = i.ToString();
                                }
                                else
                                {
                                    astrRequiredFieldRows = astrRequiredFieldRows + ", " + i.ToString();
                                }
                            }
                            if (lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages < 0M)
                            {
                                if (string.IsNullOrEmpty(lstrPositiveRows))
                                {
                                    lstrPositiveRows = i.ToString();
                                }
                                else
                                {
                                    lstrPositiveRows = lstrPositiveRows + ", " + i.ToString();
                                }
                            }
                            //If invalid pay period entered throw error for each detail
                            if (!IsReportingMonthValid(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period))
                            {
                                if (string.IsNullOrEmpty(lstrReportingMonthInvalidRows))
                                {
                                    lstrReportingMonthInvalidRows = i.ToString();
                                }
                                else
                                {
                                    lstrReportingMonthInvalidRows = lstrReportingMonthInvalidRows + ", " + i.ToString();
                                }
                            }
                            //PIR 13996
                            //show hard error if begin month or end month for bounus/retro pay is less than Jan 1977 
                            else if (lobjPayrollDetail.idtPayPeriod != DateTime.MinValue && lobjPayrollDetail.idtPayPeriod < new DateTime(1977, 01, 01))
                            {
                                if (string.IsNullOrEmpty(lstrBeginMonthRows))
                                {
                                    lstrBeginMonthRows = i.ToString();
                                }
                                else
                                {
                                    lstrBeginMonthRows = lstrBeginMonthRows + ", " + i.ToString();
                                }
                            }
                            //If invalid pay end month entered throw error for each detail
                            if (!IsReportingMonthValid(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month))
                            {
                                if (string.IsNullOrEmpty(lstrEndMonthInvalidRows))
                                {
                                    lstrEndMonthInvalidRows = i.ToString();
                                }
                                else
                                {
                                    lstrEndMonthInvalidRows = lstrEndMonthInvalidRows + ", " + i.ToString();
                                }
                            
                            }
                            else if ((lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month != string.Empty ? Convert.ToDateTime(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month) : DateTime.MinValue) < new DateTime(1977, 01, 01))
                            {
                                if (string.IsNullOrEmpty(lstrEndMonthRows))
                                {
                                    lstrEndMonthRows = i.ToString();
                                }
                                else
                                {
                                    lstrEndMonthRows = lstrEndMonthRows + ", " + i.ToString();
                                }
                            }
                        }
                    
                    }
                }
               
                //Retirement Bonus/Retro pay for only few employees scenario
                else if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0 &&
                    _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
                    _icdoEmployerPayrollHeader.report_type_description == "Bonus/Retro Pay" &&
                    astrIsDetailPopulated == busConstant.Flag_No_CAPS)
                {
                    if (icdoEmployerPayrollHeader.total_wages_reported < 0M || icdoEmployerPayrollHeader.total_contribution_reported < 0M)
                    {
                        this.iblnHasErrors = true;
                        lobjError = AddError(10259, "");
                        larrresult.Add(lobjError);
                        return larrresult;
                    }
                    int i = 0;
                    foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                    {
                        i++;
                        if (string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.ssn) ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.first_name_display) ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.last_name_display) ||
                            lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == 0 ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period) ||
                            lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages == 0M ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month))
                        {
                            if (string.IsNullOrEmpty(astrRequiredFieldRows))
                            {
                                astrRequiredFieldRows = i.ToString();
                            }
                            else
                            {
                                astrRequiredFieldRows = astrRequiredFieldRows + ", " + i.ToString();
                            }
                        }
                        if (lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages < 0M)
                        {
                            if (string.IsNullOrEmpty(lstrPositiveRows))
                            {
                                lstrPositiveRows = i.ToString();
                            }
                            else
                            {
                                lstrPositiveRows = lstrPositiveRows + ", " + i.ToString();
                            }
                        }
                        if (!IsReportingMonthValid(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period))
                        {
                            if (string.IsNullOrEmpty(lstrReportingMonthInvalidRows))
                            {
                                lstrReportingMonthInvalidRows = i.ToString();
                            }
                            else
                            {
                                lstrReportingMonthInvalidRows = lstrReportingMonthInvalidRows + ", " + i.ToString();
                            }
                        }
                        //PIR 13996
                        //show hard error if begin month or end month for bounus/retro pay is less than Jan 1977 
                        else if (lobjPayrollDetail.idtPayPeriod != DateTime.MinValue && lobjPayrollDetail.idtPayPeriod < new DateTime(1977, 01, 01))
                        {
                            if (string.IsNullOrEmpty(lstrBeginMonthRows))
                            {
                                lstrBeginMonthRows = i.ToString();
                            }
                            else
                            {
                                lstrBeginMonthRows = lstrBeginMonthRows + ", " + i.ToString();
                            }
                        }
                        if (!IsReportingMonthValid(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month))
                        {
                            if (string.IsNullOrEmpty(lstrEndMonthInvalidRows))
                            {
                                lstrEndMonthInvalidRows = i.ToString();
                            }
                            else
                            {
                                lstrEndMonthInvalidRows = lstrEndMonthInvalidRows + ", " + i.ToString();
                            }
                        
                        }
                        else if ((lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month != string.Empty ? Convert.ToDateTime(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month) : DateTime.MinValue) < new DateTime(1977, 01, 01))
                        {
                            if (string.IsNullOrEmpty(lstrEndMonthRows))
                            {
                                lstrEndMonthRows = i.ToString();
                            }
                            else
                            {
                                lstrEndMonthRows = lstrEndMonthRows + ", " + i.ToString();
                            }
                        }
                    }
                }
                //Retirement Adjustment for all employees scenario
                else if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0 &&
                     _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
                     _icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment &&
                     astrIsDetailPopulated == busConstant.Flag_Yes_CAPS)
                {
                   
                    //atleast one row with wages should be present
                    if (iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.eligible_wages != 0M).Count() == 0)
                    {
                        this.iblnHasErrors = true;
                        lobjError = AddError(10240, "");
                        larrresult.Add(lobjError);
                        return larrresult;
                    }
                    int i = 0;
                    foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                    {
                        i++;
                        if (lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages != 0M)
                        {
                            if (lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == 0 ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period))
                            {
                                if (string.IsNullOrEmpty(astrRequiredFieldRows))
                                {
                                    astrRequiredFieldRows = i.ToString();
                                }
                                else
                                {
                                    astrRequiredFieldRows = astrRequiredFieldRows + ", " + i.ToString();
                                }
                            }
                            if (!IsReportingMonthValid(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period))
                            {
                                if (string.IsNullOrEmpty(lstrReportingMonthInvalidRows))
                                {
                                    lstrReportingMonthInvalidRows = i.ToString();
                                }
                                else
                                {
                                    lstrReportingMonthInvalidRows = lstrReportingMonthInvalidRows + ", " + i.ToString();
                                }
                            }
                            //PIR 13996
                            //show hard error if reporting month is less than Jan 1977 
                            else if (lobjPayrollDetail.idtPayPeriod != DateTime.MinValue && lobjPayrollDetail.idtPayPeriod < new DateTime(1977, 01, 01))
                            {
                                if (string.IsNullOrEmpty(lstrReportingMonthRows))
                                {
                                    lstrReportingMonthRows = i.ToString();
                                }
                                else
                                {
                                    lstrReportingMonthRows = lstrReportingMonthRows + ", " + i.ToString();
                                }
                            }
                        }
                    }
                    
                }
                //Retirement Adjustment for only few employees scenario
                else if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0 &&
                    _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
                    _icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment &&
                    astrIsDetailPopulated == busConstant.Flag_No_CAPS)
                {
                    int i = 0;
                    foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                    {
                        i++;
                        if (string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.ssn) ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.first_name_display) ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.last_name_display) ||
                            lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == 0 ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period) ||
                            lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages == 0M)
                        {
                            if (string.IsNullOrEmpty(astrRequiredFieldRows))
                            {
                                astrRequiredFieldRows = i.ToString();
                            }
                            else
                            {
                                astrRequiredFieldRows = astrRequiredFieldRows + ", " + i.ToString();
                            }
                        }
                        //If invalid pay period entered on detail throw an error - Maik Mail Dated March 11, 2015
                        if (!IsReportingMonthValid(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period))
                        {
                            if (string.IsNullOrEmpty(lstrReportingMonthInvalidRows))
                            {
                                lstrReportingMonthInvalidRows = i.ToString();
                            }
                            else
                            {
                                lstrReportingMonthInvalidRows = lstrReportingMonthInvalidRows + ", " + i.ToString();
                            }
                        }
                        //PIR 13996
                        //show hard error if reporting month is less than Jan 1977 
                        else if (lobjPayrollDetail.idtPayPeriod != DateTime.MinValue && lobjPayrollDetail.idtPayPeriod < new DateTime(1977, 01, 01))
                         {
                             if(string.IsNullOrEmpty(lstrReportingMonthRows))
                             {
                                 lstrReportingMonthRows=i.ToString();
                             }
                             else
                             {
                                 lstrReportingMonthRows=lstrReportingMonthRows+", "+i.ToString();
                             }
                         }
                    }
                }
                //Deferred Comp adjustment for all employees scenario
                else if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0 &&
                    _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp &&
                    _icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment 
                    && astrIsDetailPopulated == busConstant.Flag_Yes_CAPS)
                {
                    //at least one row with wages should be present
                    if (_iclbEmployerPayrollDetail.Where(obj => obj.icdoEmployerPayrollDetail.contribution_amount1
                                                                                    + obj.icdoEmployerPayrollDetail.contribution_amount2
                                                                                    + obj.icdoEmployerPayrollDetail.contribution_amount3
                                                                                    + obj.icdoEmployerPayrollDetail.contribution_amount4
                                                                                    + obj.icdoEmployerPayrollDetail.contribution_amount5
                                                                                    + obj.icdoEmployerPayrollDetail.contribution_amount6
                                                                                    + obj.icdoEmployerPayrollDetail.contribution_amount7 != 0M).Count() == 0)
                    {
                        this.iblnHasErrors = true;
                        lobjError = AddError(10240, "");
                        larrresult.Add(lobjError);
                        return larrresult;
                    }
                    int i = 0;
                     foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                    {
                        i++;
                        if (lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 != 0M || lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 != 0M
                                || lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 != 0M || lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 != 0M
                                || lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 != 0M || lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 != 0M
                                || lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 != 0M)
                        {
                            if ((lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 == 0M && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 == 0M
                                && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 == 0M && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 == 0M
                                && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 == 0M && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 == 0M
                                && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 == 0M) ||
                                (string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id1)
                                && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id2)
                                && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id3)
                                && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id4)
                                && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id5)
                                && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id6)
                                && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id7)))
                            {
                                if (string.IsNullOrEmpty(astrRequiredFieldRows))
                                {
                                    astrRequiredFieldRows = i.ToString();
                                }
                                else
                                {
                                    astrRequiredFieldRows = astrRequiredFieldRows + ", " + i.ToString();
                                }
                            }
                        }
                        //PIR 25920 DC 2025 changes Allow employer to add only wages and employer match withount provider and contribution amount
                        if ((lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages_defcomp == 0M ||
                             lobjPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported == 0M))
                        {
                            //PIR 25920 DC 2025 changes Allow employer to add only wages and employer match withount provideer and contribution amount
                            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                            lbusPerson = lbusPerson.LoadPersonBySsn(lobjPayrollDetail.icdoEmployerPayrollDetail.ssn);
                            if (lbusPerson != null && lbusPerson.icdoPerson.person_id != 0)
                            {
                                lobjPayrollDetail.icdoEmployerPayrollDetail.person_id = lbusPerson.icdoPerson.person_id;
                                if (lobjPayrollDetail.ibusPerson.IsNull()) lobjPayrollDetail.ibusPerson = lbusPerson;
                                lobjPayrollDetail.LoadPerson(); lobjPayrollDetail.LoadPersonAccount();
                                if ((lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages_defcomp == 0M || lobjPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported == 0M)
                                    && (lobjPayrollDetail.IsAnyOneContributionAvailableDefComp() && lobjPayrollDetail.IsAnyOneProviderAvailableDefComp())
                                    && lobjPayrollDetail.ibusPersonAccount.IsEmployerMatchAvailableWithElection(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_start_date))
                                // && lobjPayrollDetail.IsEmployerMatchPlanRateAvailable)
                                {
                                    if (string.IsNullOrEmpty(astrRequiredFieldRows))
                                    {
                                        astrRequiredFieldRows = i.ToString();
                                    }
                                    else
                                    {
                                        if(!astrRequiredFieldRows.Contains(i.ToString()))
                                            astrRequiredFieldRows = astrRequiredFieldRows + ", " + i.ToString();
                                    }
                                }
                            }
                        }
                    }                                    
                }
                //Deferred Comp adjustment for few employees scenario
                else if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count > 0 &&
                    _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp &&
                    _icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment
                    && astrIsDetailPopulated == busConstant.Flag_No_CAPS)
                {
                    int i = 0;
                    foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                    {
                        i++;
                        //PIR 25920 DC 2025 changes Allow employer to add only wages and employer match withount provider and contribution amount
                        busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lbusPerson = lbusPerson.LoadPersonBySsn(lobjPayrollDetail.icdoEmployerPayrollDetail.ssn);
                        if (lbusPerson != null && lbusPerson.icdoPerson.person_id != 0)
                        {
                            lobjPayrollDetail.icdoEmployerPayrollDetail.person_id = lbusPerson.icdoPerson.person_id;
                            if(lobjPayrollDetail.ibusPerson.IsNull()) lobjPayrollDetail.ibusPerson = lbusPerson;
                            lobjPayrollDetail.LoadPerson();lobjPayrollDetail.LoadPersonAccount();
                        }
                        if (string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.ssn) ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.first_name_display) ||
                            string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.last_name_display) ||
                            (!lobjPayrollDetail.CheckMemberPayrollIsPostedInDefComp(_icdoEmployerPayrollHeader.org_id) && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 == 0M && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 == 0M
                            && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 == 0M && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 == 0M
                            && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 == 0M && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 == 0M
                            && lobjPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 == 0M) ||
                            lobjPayrollDetail.icdoEmployerPayrollDetail.plan_id == 0 ||
                            ((lobjPayrollDetail.icdoEmployerPayrollDetail.eligible_wages_defcomp == 0M || 
                             lobjPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported == 0M)
                             && lobjPayrollDetail.ibusPersonAccount.IsEmployerMatchAvailableWithElection(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_start_date)) ||
                             //&& lobjPayrollDetail.IsEmployerMatchPlanRateAvailable) || 
                            (!lobjPayrollDetail.CheckMemberPayrollIsPostedInDefComp(_icdoEmployerPayrollHeader.org_id) && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id1)
                            && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id2)
                            && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id3)
                            && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id4)
                            && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id5)
                            && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id6)
                            && string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.provider_org_code_id7)))
                        {
                            if (string.IsNullOrEmpty(astrRequiredFieldRows))
                            {
                                astrRequiredFieldRows = i.ToString();
                            }
                            else
                            {
                                astrRequiredFieldRows = astrRequiredFieldRows + ", " + i.ToString();
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(astrRequiredFieldRows))
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(0, String.Format("Please enter all information on line {0}", astrRequiredFieldRows));
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                if (!string.IsNullOrEmpty(lstrPositiveRows))
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10259, iobjPassInfo), lstrPositiveRows));
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                if (!string.IsNullOrEmpty(lstrReportingMonthInvalidRows))
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(0, String.Format("Invalid Reporting Month on line {0}", lstrReportingMonthInvalidRows));
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                if (!string.IsNullOrEmpty(lstrEndMonthInvalidRows))
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(0, String.Format("Invalid End Month for bonus on line {0}", lstrEndMonthInvalidRows));
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                //PIR 13996
                //Show Hard errors.
                if(!string.IsNullOrEmpty(lstrReportingMonthRows))
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10249,iobjPassInfo) , lstrReportingMonthRows));
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                if (!string.IsNullOrEmpty(lstrBeginMonthRows))
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10250, iobjPassInfo), lstrBeginMonthRows));
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                if (!string.IsNullOrEmpty(lstrEndMonthRows))
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10251, iobjPassInfo), lstrEndMonthRows));
                    larrresult.Add(lobjError);
                    return larrresult;
                }
               
                foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                {
                    if (icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollDetailRecordTypeRegular
                                 && icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        if (!string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month) && !string.IsNullOrEmpty(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period))
                        {
                            DateTime ldtEndMonthForBonus;
                            if (DateTime.TryParse(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_end_month, out ldtEndMonthForBonus))
                            {
                                lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus = ldtEndMonthForBonus;
                            }
                            DateTime ldtReportingMonth;
                            if (DateTime.TryParse(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period, out ldtReportingMonth))
                            {
                                lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_date = ldtReportingMonth;
                            }
                        }
                    }
                }
                if (iclbEmployerPayrollDetail != null &&
                   icdoEmployerPayrollHeader.report_type_description == "Bonus/Retro Pay" &&
                   iclbEmployerPayrollDetail.Where(obj => obj.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus < obj.icdoEmployerPayrollDetail.pay_period_date && obj.icdoEmployerPayrollDetail.eligible_wages != 0M).Count() > 0)
                                                   
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(10244, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
            }
            #endregion          
            #region Special case of defered comp where no detail present
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                
                if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count() == 0 && !iblnIsDetailCountNone)
                {
                    iblnIsDetailCountNone = true;
                    this.iblnHasErrors = true;
                    larrresult.Add(this);
                    this.EvaluateInitialLoadRules();
                    return larrresult;
                }
                if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count() == 0 && istrEmployerSelectionEnrollment == null)
                {
                    this.iblnHasErrors = true;
                    lobjError = AddError(0, "Please Select Yes or No.");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count() == 0 && istrEmployerSelectionEnrollment == "NO")
                {
                    if (ibusOrganization.IsNull())
                        LoadOrganization();
                    if (ibusOrganization.iclbOrgPlan.IsNull())
                        ibusOrganization.LoadOrgPlan();
                    var lenuOfferedPlans = ibusOrganization.iclbOrgPlan.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation || i.icdoOrgPlan.plan_id == busConstant.PlanIdOther457)
                                                .Where(i => i.icdoOrgPlan.participation_end_date == DateTime.MinValue || i.icdoOrgPlan.participation_end_date > DateTime.Now);

                    if (lenuOfferedPlans.Count() > 1)
                    {
                        this.iblnHasErrors = true;
                        lobjError = AddError(0, "Please Contact NDPERS.");
                        larrresult.Add(lobjError);
                        return larrresult;
                    }
                    else if (lenuOfferedPlans.Count() == 1 && lenuOfferedPlans.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdOther457).Count() == 1)
                    {
                        //this.iblnHasErrors = true;
                        //lobjError = AddError(0, "Please Go Back To Employee Lookup If You Want To Enroll Any For Other457");
                        //larrresult.Add(lobjError);
                        //Commented above code as we need to check the error and redirect to employeeLookup and not to show on screen
                        istrCreateReportNavigation = "EMPL";
                        return larrresult;
                    }
                    else if (lenuOfferedPlans.Count() == 1 && lenuOfferedPlans.Where(i => i.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation).Count() == 1)
                    {
                        this.iblnHasErrors = true;
                        lobjError = AddError(0, "Please Contact NDPERS.");
                        larrresult.Add(lobjError);
                        return larrresult;
                    }
                }
            }
            #endregion
            #endregion
            #region Pulling Information For File
            if (astrIsDetailPopulated != busConstant.Flag_Yes_CAPS)
            {
                //int i = 0;
                foreach (busEmployerPayrollDetail lbusEmployerPayrollDetail in iclbEmployerPayrollDetail)
                {
                    //i++;
                    busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusPerson = lbusPerson.LoadPersonBySsn(lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn);
                    busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
                    lbusPlan.FindPlan(lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id);
                    if (lbusPerson != null && lbusPerson.icdoPerson.person_id != 0)
                    {
                        DataTable ldtbActiveMembers;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id = lbusPerson.icdoPerson.person_id;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name_display;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name_display;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id = lbusPlan.icdoPlan.plan_id;
                        lbusEmployerPayrollDetail.ibusPlan = lbusPlan;
                        lbusEmployerPayrollDetail.istrPlanValue = lbusEmployerPayrollDetail.ibusPlan.icdoPlan.plan_code;
                        if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                        {
                            ldtbActiveMembers = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadRetirementMembersByOrgandPersonId",
                                                            new object[3] { _icdoEmployerPayrollHeader.payroll_paid_last_date, icdoEmployerPayrollHeader.org_id, lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id });
                            if (ldtbActiveMembers != null && ldtbActiveMembers.Rows.Count > 0)
                            {
                                lbusEmployerPayrollDetail.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                                lbusEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.LoadData(ldtbActiveMembers.Rows[0]);
                                lbusEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                                lbusEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtbActiveMembers.Rows[0]);

                            }
                        }
                        else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                        {
                            //Load all Active Members for this Employer
                            ldtbActiveMembers = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadActiveDeffCompMembersByOrgAndPerson",
                                                                      new object[4]
                                                              {                                                                  
                                                                  icdoEmployerPayrollHeader.pay_period_start_date,
                                                                  icdoEmployerPayrollHeader.pay_period_end_date,
                                                                  icdoEmployerPayrollHeader.org_id,
                                                                  lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id
                                                              });

                            if (ldtbActiveMembers != null && ldtbActiveMembers.Rows.Count > 0)
                            {
                                busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                                lbusPersonAccount.icdoPersonAccount.LoadData(ldtbActiveMembers.Rows[0]);

                            }
                        }
                    }
                    else 
                    {
                        //When entered person_id not in the system, invalid records should be there on the file 
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name_display;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name_display;
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id = lbusPlan.icdoPlan.plan_id;
                        lbusEmployerPayrollDetail.ibusPlan = lbusPlan;
                        lbusEmployerPayrollDetail.istrPlanValue = lbusEmployerPayrollDetail.ibusPlan.icdoPlan.plan_code;
                    }
                }
            }
            #endregion
            #region Genrating OutBound File
            if (_icdoEmployerPayrollHeader.employer_payroll_header_id != 0)
            {
                lobjProcessfiles.iarrParameters[0] = _icdoEmployerPayrollHeader.employer_payroll_header_id;
            }
            else
            {
                lobjProcessfiles.iarrParameters[0] = this;
            }
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                this.iblnHasErrors = false;
                lobjProcessfiles.CreateOutboundFile(busConstant.DeferredCompOutboundFileID);
            }
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
              int rest=  lobjProcessfiles.CreateOutboundFile(busConstant.RetirementOutboundFileID);
            }
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                lobjProcessfiles.CreateOutboundFile(busConstant.InsuranceOutboundFileID);
            }
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                lobjProcessfiles.CreateOutboundFile(busConstant.PurchaseOutboundFileID);
            }
            astrFileName = (string)lobjProcessfiles.iarrParameters[0];
            if (String.IsNullOrEmpty(istrGeneratedPath))
            {
                istrGeneratedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("EmpOut");
            }
            //Imaged Path
            if (String.IsNullOrEmpty(istrImagedPath))
            {
                istrImagedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("EmpInbox");
            }
            busCorTracking ibusCorTracking = new busCorTracking();
                lstrSourcePath = istrGeneratedPath + astrFileName;
                string[] lstrFilename = astrFileName.Split('.');
            string orgFile=lstrFilename[0] + "_" + DateTime.Now.ToLongDateString().Replace("/", "_") + "_" + DateTime.Now.ToLongTimeString().Replace(":", "_") + "." + lstrFilename[1];
            lstrDestinationPath = istrImagedPath + orgFile;
            
            if (File.Exists(lstrSourcePath))
            {
                File.Move(lstrSourcePath, lstrDestinationPath);

                lstrSourcePath = lstrDestinationPath;
                istrImagedPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("EmpStaging");
                lstrDestinationPath = istrImagedPath + orgFile;
                File.Move(lstrSourcePath, lstrDestinationPath);
                
            }

            int aintFileId = 0;
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                aintFileId = 11;
            }
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                aintFileId = 10;
            }
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                aintFileId = 12;
            }
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                aintFileId = 28;
            }
            cdoFileHdr lcdoFileHdr = new cdoFileHdr
            {
                mailbox_file_name = orgFile,
                processed_file_name = orgFile,
                processed_date = DateTime.Now,
                file_id = aintFileId,
                status_value = "UNPC",
                reference_id = _icdoEmployerPayrollHeader.org_id
            };
            lcdoFileHdr.Insert();
            #endregion
            istrCreateReportNavigation = "PRDL";
            larrresult.Add(this);
            return larrresult;
        }

        #region Validation Methods
        public bool VisibleIgnoreButton()
        {
            if (_iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
            if (_icdoEmployerPayrollHeader.status_value != busConstant.PayrollHeaderStatusIgnored)
            {
                foreach (busEmployerPayrollDetail lobjdetail in _iclbEmployerPayrollDetail)
                {
                    if (lobjdetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPosted)
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        public bool IsReportingSourceChanged()
        {
            if (_icdoEmployerPayrollHeader.ienuObjectState == ObjectState.Update)
            {
                if (_icdoEmployerPayrollHeader.reporting_source_value != _icdoEmployerPayrollHeader.ihstOldValues["reporting_source_value"].ToString())
                {
                    return true;
                }
            }
            return false;
        }

        public bool VisibleRuleForDefComp()
        {
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                if (_icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusUnbalanced)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// Function Check the Payroll Header Overlapping 
        /// 1) Regular Report should not have multiple header for the same period except Ignored Header
        /// </summary>
        /// <returns></returns>
        public bool CheckPayrollHeaderOverlapping()
        {
            bool lblnRecordMatch = false;
            if (_icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular)
            {
                DataTable ldtOtherPayrollHeaders = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadAllPayrollHeaderExceptIgnored",
                                                                         new object[4]
                                                                         {
                                                                             _icdoEmployerPayrollHeader.org_id,
                                                                             _icdoEmployerPayrollHeader.header_type_value,
                                                                             _icdoEmployerPayrollHeader.report_type_value,
                                                                             _icdoEmployerPayrollHeader.employer_payroll_header_id
                                                                         });
                Collection<busEmployerPayrollHeader> _iclbOtherEmployerPayrollHeaders = GetCollection<busEmployerPayrollHeader>(ldtOtherPayrollHeaders, "icdoEmployerPayrollHeader");
                foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbOtherEmployerPayrollHeaders)
                {
                    if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(_icdoEmployerPayrollHeader.pay_period_start_date,
                                                                    _icdoEmployerPayrollHeader.pay_period_end_date,
                                                                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_start_date,
                                                                    lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_end_date))
                        {
                            lblnRecordMatch = true;
                            break;
                        }
                    }
                    else
                    {
                        if (
                            (_icdoEmployerPayrollHeader.payroll_paid_date.Month == lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month)
                            &&
                            (_icdoEmployerPayrollHeader.payroll_paid_date.Year == lobjEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year)
                            )
                        {
                            lblnRecordMatch = true;
                            break;
                        }
                    }
                }
            }
            return lblnRecordMatch;
        }

        ///<summary>
        /// BR-034-26(The system must only allow adding a new �Regular� �Payroll Header� immediately after the �Regular� Report from the system)
        ///Check Continuity of date for header records 
        ///</summary>
        public bool CheckHeaderDateContinuity()
        {
            if ((_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) ||
                (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr))
            {
                if (_icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
                {
                    DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadPayrollHeaderExceptIgnoredForRetAndIns",
                                          new object[4]
                                              {
                                                  _icdoEmployerPayrollHeader.org_id,
                                                  _icdoEmployerPayrollHeader.header_type_value,
                                                  _icdoEmployerPayrollHeader.employer_payroll_header_id,
                                                  _icdoEmployerPayrollHeader.payroll_paid_date  // PIR 10018
                                              });
                    if (ldtbList.Rows.Count > 0)
                    {
                        foreach (DataRow adrRow in ldtbList.Rows)
                        {
                            if (!String.IsNullOrEmpty(adrRow["PAYROLL_PAID_DATE"].ToString()))
                            {
                                //We now allow multiple regular headers for the same period, we may need to skip the same pay period date records here
                                DateTime ldtPayPeriodDate = Convert.ToDateTime(adrRow["PAYROLL_PAID_DATE"]);
                                if (ldtPayPeriodDate.GetFirstDayofCurrentMonth() == icdoEmployerPayrollHeader.payroll_paid_date.GetFirstDayofCurrentMonth()) continue;

                                if ((ldtPayPeriodDate.AddMonths(1).Month == _icdoEmployerPayrollHeader.payroll_paid_date.Month)
                                    && (ldtPayPeriodDate.AddMonths(1).Year == _icdoEmployerPayrollHeader.payroll_paid_date.Year))
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                        //If only this month Entry found, system should allow them to post another same month entry.
                        return true;
                    }
                    else
                    {
                        // This is check when employer submit the payroll for the first time. 
                        //will be done later based on the plan participation of the employer
                        return true;
                    }
                }
            }
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadPayrollHeaderExceptIngoredForDefComp",
                                          new object[3]
                                              {
                                                  _icdoEmployerPayrollHeader.org_id,
                                                  _icdoEmployerPayrollHeader.employer_payroll_header_id,
                                                  _icdoEmployerPayrollHeader.pay_period_start_date  //PIR 13958
                                              });
                if (_icdoEmployerPayrollHeader.pay_period_start_date != DateTime.MinValue)
                {
                    if (ldtbList.Rows.Count > 0)
                    {
                        foreach (DataRow adrRow in ldtbList.Rows)
                        {
                            if (!String.IsNullOrEmpty(adrRow["PAY_PERIOD_END_DATE"].ToString()))
                            {
                                //We now allow multiple regular headers for the same period, we may need to skip the same pay period date records here
                                DateTime ldtPayPeriodEndDate = Convert.ToDateTime(adrRow["PAY_PERIOD_END_DATE"]);
                                if (ldtPayPeriodEndDate == icdoEmployerPayrollHeader.pay_period_end_date) continue;

                                if (Convert.ToDateTime(adrRow["PAY_PERIOD_END_DATE"]).AddDays(1) == _icdoEmployerPayrollHeader.pay_period_start_date)
                                {
                                    return true;
                                }
                                //pir 7892
                                else
                                {
                                    bool lblnSwitchAllowed = false;
                                    lblnSwitchAllowed = CheckIfThereIsChangeinReportingFrequency(ldtPayPeriodEndDate);
                                    return lblnSwitchAllowed;
                                }
                            }
                        }
                        //If only this month Entry found, system should allow them to post another same month entry.
                        return true;
                    }
                    else
                    {
                        // This is check when employer submit the payroll for the first time. 
                        //will be done later based on the plan participation of the employer
                        return true;
                    }
                }
            }
            return false;
        }


        //pir-7892
        //--start--//
        private bool CheckIfThereIsChangeinReportingFrequency(DateTime ldtPayPeriodEndDate)
        {
            DataTable ldtbPlan = busNeoSpinBase.Select("cdoEmployerPayrollHeader.CheckIfThereIsChangeinReportingFrequency", new object[1] { _icdoEmployerPayrollHeader.org_id });
            busBase lobjBase = new busBase();
            DateTime ldtPayPeriodDatewithCurrent = new DateTime();
            DateTime ldtPayPeriodDatewithPrev = new DateTime();
            Collection<busOrgPlan> lclbOrgplan = lobjBase.GetCollection<busOrgPlan>(ldtbPlan, "icdoOrgPlan");
            busOrgPlan lbusOrgPlanPrev = lclbOrgplan.Where(i => busGlobalFunctions.CheckDateOverlapping(ldtPayPeriodEndDate,
                                                                      i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date) &&
                                                                      i.icdoOrgPlan.participation_start_date != i.icdoOrgPlan.participation_end_date).FirstOrDefault();
            busOrgPlan lbusOrgPlanCurrent = lclbOrgplan.Where(i => busGlobalFunctions.CheckDateOverlapping(_icdoEmployerPayrollHeader.pay_period_start_date,
                                                                      i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date) && 
                                                                      i.icdoOrgPlan.participation_start_date!=i.icdoOrgPlan.participation_end_date).FirstOrDefault();
            if (lbusOrgPlanCurrent.IsNotNull() && lbusOrgPlanPrev.IsNotNull() && 
                lbusOrgPlanCurrent.icdoOrgPlan.report_frequency_value != lbusOrgPlanPrev.icdoOrgPlan.report_frequency_value)
            {
                ldtPayPeriodDatewithCurrent = SetReportingFrequency(lbusOrgPlanCurrent, ldtPayPeriodEndDate);
                ldtPayPeriodDatewithPrev = SetReportingFrequency(lbusOrgPlanPrev, ldtPayPeriodEndDate);
                if (_icdoEmployerPayrollHeader.pay_period_start_date < ldtPayPeriodDatewithCurrent && _icdoEmployerPayrollHeader.pay_period_start_date < ldtPayPeriodDatewithPrev)
                {
                    return true;
                }
            }
            return false;          
        }

        private DateTime SetReportingFrequency(busOrgPlan lbusOrgPlan,DateTime ldtPayPeriodEndDate)
        {
            DateTime ldtPayPeriodDate = new DateTime();
            if (lbusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly)
            {
                ldtPayPeriodDate = ldtPayPeriodEndDate.AddDays(15);
            }
            if (lbusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly)
            {
                ldtPayPeriodDate = ldtPayPeriodEndDate.AddDays(14);
            }
            if (lbusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyWeekly)
            {
                ldtPayPeriodDate = ldtPayPeriodEndDate.AddDays(7);
            }
            if (lbusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyMonthly)
            {
                ldtPayPeriodDate = ldtPayPeriodEndDate.AddDays(30);
            }
            return ldtPayPeriodDate;
        }
        //--end--//
        //to check frequency matches with start and end date
        public bool CheckDateForDefCompWithPlanParticipation()
        {
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                string lstrFrequency = GetDeferredCompFrequencyPeriodByOrg(_icdoEmployerPayrollHeader.org_id);
                if (!String.IsNullOrEmpty(lstrFrequency))
                {
                    //PIR 19701 Day_Of_Month from Org Plan Maintenance to be used for verify validation 4690
                    int lintDayofMonth = 1;
                    if(icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp && (lstrFrequency == busConstant.DeffCompFrequencyMonthly || lstrFrequency == busConstant.DeffCompFrequencySemiMonthly))
                        lintDayofMonth = GetDayOfMonthForMonthlyFrequency(icdoEmployerPayrollHeader.pay_period_start_date, icdoEmployerPayrollHeader.pay_period_end_date);
                    DateTime ldtEndDate = busEmployerReportHelper.GetEndDateByReportFrequency(_icdoEmployerPayrollHeader.pay_period_start_date, lstrFrequency, lintDayofMonth);
                    if (ldtEndDate == _icdoEmployerPayrollHeader.pay_period_end_date)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Getting the start day of month for Monthly reporting frequency deff comp paln //PIR 19701 Day_Of_Month from Org Plan Maintenance to be used for verify validation 4690
        /// </summary>
        /// <param name="adtPayPeriodStartDate"></param>
        /// <param name="adtPayPeriodEndDate"></param>
        /// <returns>integer day of month</returns>
        public int GetDayOfMonthForMonthlyFrequency(DateTime adtPayPeriodStartDate, DateTime adtPayPeriodEndDate)
        {
            if (ibusOrganization.IsNull())
                LoadOrganization();
            if (ibusOrganization.iclbOrgPlan.IsNull())
                ibusOrganization.LoadOrgPlan();
            //plan either in deff comp plan AND reporting frequency should be MONTHLY AND reporting dates falls between org plan dates. 
            busOrgPlan lbusOrgPlan = ibusOrganization.iclbOrgPlan.FirstOrDefault(i => (i.icdoOrgPlan.plan_id == busConstant.PlanIdDeferredCompensation 
                                    || i.icdoOrgPlan.plan_id == busConstant.PlanIdOther457) && (i.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyMonthly || i.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly)
                                    && busGlobalFunctions.CheckDateOverlapping(adtPayPeriodStartDate, adtPayPeriodEndDate, i.icdoOrgPlan.participation_start_date,i.icdoOrgPlan.participation_end_date));

            return lbusOrgPlan.IsNotNull() && lbusOrgPlan.icdoOrgPlan.day_of_month > 0 ? lbusOrgPlan.icdoOrgPlan.day_of_month : 1;
        }
        //To check if the org plan got frequency set or not
        public bool CheckIfFrequencySetForOrgPlan()
        {
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                DataTable ldtbList = Select<cdoOrgPlan>(
                                           new string[2] { "org_id", "REPORT_FREQUENCY_VALUE" },
                                           new object[2] { _icdoEmployerPayrollHeader.org_id, "null" }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    return false;
                }
            }
            return true;
        }

        // PIR - 13975 To check whether Life Insurance details exist or not.
        public bool CheckIfLifeInsuranceDetailsExist()  
        {
            if (_iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
                foreach (busEmployerPayrollDetail lobjdetail in _iclbEmployerPayrollDetail)
                {
                    if (lobjdetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupLife)
                    {
                        return true;
                    }
                }
            return false;
         }
            
        #endregion 

        public override bool ValidateSoftErrors()
        {
            //Loading the Detail
            if (_iclbEmployerPayrollDetail == null)
            {
                LoadEmployerPayrollDetail();
            }

            if ((iblnValidateDetail) && (_iclbEmployerPayrollDetail != null) && (_iclbEmployerPayrollDetail.Count > 0))
            {
                if (ibusOrganization == null)
                    LoadOrganization();

                foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _iclbEmployerPayrollDetail)
                {
                    if ((lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusIgnored)
                        && (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusPosted))
                    {
                        lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = this;

                        lobjEmployerPayrollDetail.LoadObjectsForValidation();

                        if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                        {
                            lobjEmployerPayrollDetail.LoadOrgIdForProviders();
                            lobjEmployerPayrollDetail.CheckProviderNotLinkedToEmployer();
                        }
                        lobjEmployerPayrollDetail.iblnValidateHeader = false;
                        if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                        {
                            lobjEmployerPayrollDetail.LoadOldSalaryAmount();
                        }

                        //Automatic Purchase Allocation for each detail
                        if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                        {
                            AllocateAutomaticForPurchase();
                        }

                        lobjEmployerPayrollDetail.UpdateCalculatedFields();
                        lobjEmployerPayrollDetail.ValidateSoftErrors();
                        //lobjEmployerPayrollDetail.ClearObjects(); //This line commented as part of PIR 13980 - Add Validate logic to Save button on ESS
                    }
                }
            }

            /**************************************************************************************************
            * The following methods we are calling here because whenever we change the payroll detail, 
            * it should recalculate and update the Total Values and the header status
            /**************************************************************************************************/
            //PROD PIR : 4858 This block earlier was in updatevalidatestatus. but, there was a soft error validation based on total amount reported vs calculated.
            //so, we must load that before calling validate soft errors.

            //Reset the Interest Amount in All Details Records If Interest Waiver Flag Checked
            if ((iblnInterestWaiverFlagChanged) && (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt))
            {
                RecalculateAllDetailsInterestBonusAmount();
                iblnInterestWaiverFlagChanged = false;
            }

            //Recalculate the Total Values
            CalculateContributionWagesInterestFromDtl();

            //Update the Total Fields in Payroll Header
            UpdateTotalValues();
            //PROD PIR 4858 Ends

            return base.ValidateSoftErrors();
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (!String.IsNullOrEmpty(_istrOrgCodeId))
            {
                ibusOrganization = new busOrganization();
                ibusOrganization.FindOrganizationByOrgCode(_istrOrgCodeId);
                _icdoEmployerPayrollHeader.org_id = ibusOrganization.icdoOrganization.org_id;
            }

            if ((String.IsNullOrEmpty(_icdoEmployerPayrollHeader.pay_period)))
            {
                _icdoEmployerPayrollHeader.pay_period = String.Empty;
            }
            else
            {
                _icdoEmployerPayrollHeader.payroll_paid_date = Convert.ToDateTime(icdoEmployerPayrollHeader.pay_period.ToString());
            }
            if (aenmPageMode == utlPageMode.New)
                iblnEmployerPayrollNewMode = true;
            base.BeforeValidate(aenmPageMode);
        }
        public bool DoesNotOrgCodeOnFileMatchUploadingOrg()
        {
            return (iblnFromFile && icdoFileHdr.IsNotNull() && icdoFileHdr.reference_id > 0 && ((icdoEmployerPayrollHeader.org_id > 0 &&
                   icdoEmployerPayrollHeader.org_id != icdoFileHdr.reference_id) || (iclbEmployerPayrollDetail.IsNotNull() && iclbEmployerPayrollDetail.Count > 0
                   && iclbEmployerPayrollDetail.Any(d => !string.IsNullOrEmpty(d.istrOrgCodeId) && d.istrOrgCodeId != busGlobalFunctions.GetOrgCodeFromOrgId(icdoFileHdr.reference_id)))));
        }

        public DateTime idtOldPayCheckDate { get; set; }

        public override void BeforePersistChanges()
        {
            //Update the Validated Date            
            _icdoEmployerPayrollHeader.validated_date = DateTime.Now;

            //Set the Flag If the Interest Waiver Flag Changed to Recalculate all the Detail Interest and Bonus
            if (_icdoEmployerPayrollHeader.ihstOldValues.Count > 0)
            {
                if ((_icdoEmployerPayrollHeader.ihstOldValues["interest_waiver_flag"] == null) &&
                    (_icdoEmployerPayrollHeader.interest_waiver_flag == busConstant.Flag_Yes))
                {
                    iblnInterestWaiverFlagChanged = true;
                }
                else if (Convert.ToString(_icdoEmployerPayrollHeader.ihstOldValues["interest_waiver_flag"]) != _icdoEmployerPayrollHeader.interest_waiver_flag)
                {
                    iblnInterestWaiverFlagChanged = true;
                }
                else if (_icdoEmployerPayrollHeader.interest_waiver_flag != busConstant.Flag_Yes) //PIR 26010 & PIR 25142
                {
                    iblnInterestWaiverFlagChanged = true;
                }
            }
            if (icdoEmployerPayrollHeader.ihstOldValues.Count > 0 && icdoEmployerPayrollHeader.ihstOldValues["pay_check_date"] != null)
            {
                idtOldPayCheckDate = Convert.ToDateTime(icdoEmployerPayrollHeader.ihstOldValues["pay_check_date"]);
            }
            else
            {
                idtOldPayCheckDate = DateTime.MinValue;
            }

            //Update the Balancing Status If Any changes in the Reported Amount
            UpdateBalancingStatus();

            base.BeforePersistChanges();
        }
        public override int PersistChanges()
        {
            int lintReturn = base.PersistChanges();
            if (_iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
            if (iblnIsFromESS && _iclbEmployerPayrollDetail.Count == 0 && icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollHeaderReportTypeAdjustment) // uat pir 1854
            {
                CreatePayrollDetailForESS();
                //Reload detail
                LoadEmployerPayrollDetail();
                UpdateValidateStatus();
                iblnValidateDetail = true;
            }
            //uat pir 2215
            //below block is added to suppress the warning message 4753
            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp &&
                icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular)
            {
                icdoEmployerPayrollHeader.suppress_warnings_flag = busConstant.Flag_Yes;
            }
            if (iclbEmployerPayrollDetail.Count > 0)
            {
                if (idtOldPayCheckDate != icdoEmployerPayrollHeader.pay_check_date)
                {
                    foreach (busEmployerPayrollDetail lobjDetail in iclbEmployerPayrollDetail)
                    {
                        lobjDetail.icdoEmployerPayrollDetail.pay_check_date = icdoEmployerPayrollHeader.pay_check_date;
                        lobjDetail.icdoEmployerPayrollDetail.Update();
                    }
                }
                if (iblnIsFromESS && _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) iblnValidateDetail = true; //PIR 13980 - Add Validate logic to Save button on ESS
                //else iblnValidateDetail = false;
            }
            // ESS Backlog PIR - 13416
            if (!String.IsNullOrEmpty(icdoEmployerPayrollHeader.comments))
            {
                busComments lbusComments = new busComments { icdoComments = new cdoComments() };
                lbusComments.icdoComments.comments = icdoEmployerPayrollHeader.comments;
                lbusComments.icdoComments.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
                lbusComments.icdoComments.created_by = iobjPassInfo.istrUserID;
                lbusComments.icdoComments.modified_by = iobjPassInfo.istrUserID;
                lbusComments.icdoComments.created_date = DateTime.Now;
                lbusComments.icdoComments.modified_date = DateTime.Now;
                lbusComments.icdoComments.Insert();
                LoadEmployerPayrollHeaderComments();
                icdoEmployerPayrollHeader.comments = String.Empty;
            }
            return lintReturn;
        }
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //Loading the Detail
            if (_iclbEmployerPayrollDetail == null)
            {
                LoadEmployerPayrollDetail();
            }

            if (!iblnSkipReloadAfterSave)
            {
                //Reloading Objects to Refresh                        
                LoadOrganization();
                istrOrgCodeId = ibusOrganization.icdoOrganization.org_code;
                LoadAvailableRemittanace();
                LoadEmployerRemittanceAllocation();
                LoadContributionByPlan();
                LoadTotalAppliedRemittance();
                if (_icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
                {
                    _icdoEmployerPayrollHeader.pay_period = _icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    _icdoEmployerPayrollHeader.pay_period = String.Empty;
                }
                // PROD PIR ID 6359 -- based on the fix of 4373
                //if (iblnIsFromESS)
                //{
                //    LoadStatusSummaryESS();
                //    LoadDetailErrorFromESS();
                //}
                //else
                //{
                    LoadStatusSummary();
                    LoadDetailError();
                //}
                LoadErrors();
                LoadEmployerPayrollHeaderError();
                LoadErrorLOB();
            }
            if (iblnFromFile && _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp
                && iclbEmployerPayrollDetail.Count() == 0 && _icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusValid)
            {
                _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusPosted;
                _icdoEmployerPayrollHeader.posted_date = DateTime.Now;
                _icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusBalanced;
                _icdoEmployerPayrollHeader.comments = "No contributions were reported for this pay period.";
                _icdoEmployerPayrollHeader.Update();
            }
            if (ibusSoftErrors.IsNotNull() && ibusSoftErrors.iclbError.IsNotNull() && ibusSoftErrors.iclbError.Count > 0 && iblnIsFromESS)
            {
                ibusSoftErrors.iclbError.ForEach(i => i.error_id = 1);
            }
            if (iblnIsFromESS)
            {
                LoadDetailErrorFromESS();
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) UpdateValidateStatus(); //PIR 13980 - Add Validate logic to Save button on ESS
            }
        }

        private void CreatePayrollDetailForESS(Boolean aboolFormpayrollReportGenration = false)
        {

            CreateDetailRecords(aboolFormpayrollReportGenration);
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                if (!aboolFormpayrollReportGenration)
                {
                    DataTable ldtbHeader = Select<cdoEmployerPayrollHeader>(new string[4] { "org_id", "report_type_value", "header_type_value", "status_value" },
                        new object[4] { icdoEmployerPayrollHeader.org_id, busConstant.PayrollHeaderReportTypeRegular, busConstant.PayrollHeaderBenefitTypePurchases, busConstant.PayrollHeaderStatusPosted },
                        null, "employer_payroll_header_id desc");
                    if (ldtbHeader.Rows.Count > 0)
                    {
                        int lintEmployerPayrollHeaderID = Convert.ToInt32(ldtbHeader.Rows[0]["employer_payroll_header_id"]);
                        busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader
                        {
                            icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader()
                        };
                        lbusEmployerPayrollHeader.FindEmployerPayrollHeader(lintEmployerPayrollHeaderID);
                        lbusEmployerPayrollHeader.LoadEmployerPayrollDetail();
                        //prod pir 6736 : need not take ignored records while creating new header
                        IEnumerable<busEmployerPayrollDetail> lenmEmpPayrollDetail = lbusEmployerPayrollHeader.iclbEmployerPayrollDetail
                                                                                        .Where(o => o.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusIgnored);
                        foreach (busEmployerPayrollDetail lobjEmployerPayrolDetail in lenmEmpPayrollDetail)
                        {
                            busEmployerPayrollDetail lbusEmployerPayrollDetail = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name = lobjEmployerPayrolDetail.icdoEmployerPayrollDetail.last_name;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name = lobjEmployerPayrolDetail.icdoEmployerPayrollDetail.first_name;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn = lobjEmployerPayrolDetail.icdoEmployerPayrollDetail.ssn;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusReview;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypePurchase;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.payment_class_value = lobjEmployerPayrolDetail.icdoEmployerPayrollDetail.payment_class_value;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_amount_reported = lobjEmployerPayrolDetail.icdoEmployerPayrollDetail.purchase_amount_reported;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period = lobjEmployerPayrolDetail.icdoEmployerPayrollDetail.pay_period;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.Insert();
                        }
                    }
                }
                else 
                {
                    DataTable ldtpList = Select("cdoServicePurchaseHeader.GetServicePurchaseDetailsForOrgID",
                        new object[1] { _icdoEmployerPayrollHeader.org_id });
                    if (ldtpList.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ldtpList.Rows)
                        {
                            busEmployerPayrollDetail lbusEmployerPayrollDetail = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
                            if (!Convert.IsDBNull(dr["LAST_NAME"])) lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name = dr["LAST_NAME"].ToString();  
                            if (!Convert.IsDBNull(dr["FIRST_NAME"])) lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name = dr["FIRST_NAME"].ToString();
                            if (!Convert.IsDBNull(dr["SSN"])) lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn = dr["SSN"].ToString();
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusReview;
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value = busConstant.PayrollDetailRecordTypePurchase;
                            if (!Convert.IsDBNull(dr["PAYROLL_DEDUCTION"]) && !Convert.IsDBNull(dr["PRE_TAX"]))
                            {
                                if(dr["PAYROLL_DEDUCTION"].ToString() == busConstant.Flag_Yes && dr["PRE_TAX"].ToString() == busConstant.Flag_Yes)
                                    lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.payment_class_value = busConstant.PayrollDetailPaymentClassInstallmentPreTax;
                                else if (dr["PAYROLL_DEDUCTION"].ToString() == busConstant.Flag_Yes && dr["PRE_TAX"].ToString() == busConstant.Flag_No)
                                    lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.payment_class_value = busConstant.PayrollDetailPaymentClassInstallmentPostTax;
                            }
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.payment_class_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(331, lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.payment_class_value);
                            if (!Convert.IsDBNull(dr["EXPECTED_INSTALLMENT_AMOUNT"])) lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_amount_reported = Convert.ToDecimal(dr["EXPECTED_INSTALLMENT_AMOUNT"]);
                            iclbEmployerPayrollDetail.Add(lbusEmployerPayrollDetail);
                        }
                    }
                }
            }
        }

        public override void UpdateValidateStatus()
        {
            base.UpdateValidateStatus();

            if (_iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();

            //Allocate the Automatic Remittance
            AllocateAutomaticRemittance();

            //Overriding the Header Status for Specfic Cases
            //PROD Pir 4064
            if (iblnFromFile)
                OverrideHeaderStatus();
        }

        /// <summary>
        /// This Method will be called if the Interest Waiver Flag is changed
        /// Resetting the Interest Amount in all Detail Records Except Posted and Ignored Records
        /// For Bonus Record Type, we will recalculate the Bonus and Update It.
        /// </summary>
        private void RecalculateAllDetailsInterestBonusAmount()
        {
            if (_iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();

            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _iclbEmployerPayrollDetail)
            {
                if ((lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusIgnored) &&
                    (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusPosted))
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = this;
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = _icdoEmployerPayrollHeader;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = _icdoEmployerPayrollHeader.employer_payroll_header_id;

                    if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)
                    {
                        lobjEmployerPayrollDetail.iclcEmployerPayrollBonusDetail = busEmployerReportHelper.CalculateBonus(lobjEmployerPayrollDetail);
                        lobjEmployerPayrollDetail.PersistBonusDetails();
                    }
                    //PIR 15616 - Interest caculdated for bonus same as normal adjustment
                    //Process Interest Calculation for all the entries except Bonus Record Type
                    //if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeBonus)
                    //{
                    lobjEmployerPayrollDetail.ProcessInterestCalculation();
                    //}
                    ////PIR 9538
                    //else
                    //{
                    //    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated = 0;
                    //    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated = 0;
                    //    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated = 0;
                    //    foreach (cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail in lobjEmployerPayrollDetail.iclcEmployerPayrollBonusDetail)
                    //    {
                    //        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated += lcdoEmployerPayrollBonusDetail.member_interest;
                    //        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated += lcdoEmployerPayrollBonusDetail.employer_interest;
                    //        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated += lcdoEmployerPayrollBonusDetail.employer_rhic_interest;
                    //    }
                    //}
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.Update();
                }
            }
        }

        private void OverrideHeaderStatus()
        {
            //Overriding the Status that are set by Framework for the specific Business rules            
            //If the Interest Amount is available, we should not change the status.
            if ((_icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular) &&
                (_icdoEmployerPayrollHeader.total_interest_calculated == 0) &&
                ((icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusReview) ||
                 (icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusValid)))
            {
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    if (!EvaluateRule("IsReportedTotalWagesMatchWithCalculated")) // PIR 14471 - Valid Reports are not posting
                    {
                        UpdateHeaderStatus();
                    }
                }
                else if ((_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    || (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases))
                {
                    if (((_icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusBalanced) ||
                        (_icdoEmployerPayrollHeader.ignore_balancing_status_flag == busConstant.Flag_Yes)) &&
                        (icdoEmployerPayrollHeader.total_contribution_reported == icdoEmployerPayrollHeader.total_contribution_calculated)) //PROD PIR 4132
                    {
                        UpdateHeaderStatus();
                    }
                    //prod pir 4031
                    else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    {
                        if (iclbEmployerPayrollDetail == null)
                            LoadEmployerPayrollDetail();
                        if (!iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457).Any())
                        {
                            UpdateHeaderStatus();
                        }
                    }
                }
            }
            _icdoEmployerPayrollHeader.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1209, _icdoEmployerPayrollHeader.status_value);
        }

        private void UpdateHeaderStatus()
        {
            bool lblnReviewPendingRecordsFound = false;
            bool lblnValidRecordFound = false;
            foreach (busEmployerPayrollDetail lobjEmployerReportDetail in _iclbEmployerPayrollDetail)
            {
                if (lobjEmployerReportDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusValid)
                {
                    _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReadyToPost;
                    _icdoEmployerPayrollHeader.Update();
                    lblnValidRecordFound = true;
                    break;
                }
                if ((lobjEmployerReportDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusReview) ||
                    (lobjEmployerReportDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPending))
                {
                    lblnReviewPendingRecordsFound = true;
                }
            }
            if ((!lblnReviewPendingRecordsFound && !lblnValidRecordFound) && (_iclbEmployerPayrollDetail.Count > 0))
            {
                _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReadyToPost;
                _icdoEmployerPayrollHeader.Update();
            }
        }

        public ArrayList btnCreatePayroll_Click()
        {
            ArrayList larrresult = new ArrayList();
            utlError lobjError = new utlError();

            if (_iclbEmployerPayrollDetail == null)
            {
                LoadEmployerPayrollDetail();
            }

            if (_iclbEmployerPayrollDetail.Count > 0)
            {
                lobjError = AddError(4585, "");
                larrresult.Add(lobjError);
                return larrresult;
            }

            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                lobjError = AddError(4589, "");
                larrresult.Add(lobjError);
                return larrresult;
            }
            if (_icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment && !iblnIsFromESS)
            {
                lobjError = AddError(4590, "");
                larrresult.Add(lobjError);
                return larrresult;
            }
            CreateDetailRecords();
            //Reload the Payroll Detail
            LoadEmployerPayrollDetail();
            iblnValidateDetail = true;
            this.ValidateSoftErrors();
            LoadDetailError();
            LoadStatusSummary();
            LoadErrors();
            LoadErrorLOB();
            iblnValidateDetail = false;
            larrresult.Add(this);
            return larrresult;
        }

        private void CreateDetailRecords(Boolean aboolFormpayrollReportGenration=false)
        {

            DataTable ldtbActiveMembers = null;
            //To Create Details for Retirement Type
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                //Load all Active Members for this Employer
                ldtbActiveMembers = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadActiveRetirementMembersByOrg",
                                                         new object[2] { _icdoEmployerPayrollHeader.payroll_paid_last_date, icdoEmployerPayrollHeader.org_id });


                CreateRetirementPayrollDetails(ldtbActiveMembers, aboolFormpayrollReportGenration);

            }
            // To create details for Benefit Type Deff comp    
            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                //Load all Active Members for this Employer
                ldtbActiveMembers = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadActiveDeffCompMembersByOrg",
                                                          new object[3]
                                                              {                                                                  
                                                                  icdoEmployerPayrollHeader.pay_period_start_date,
                                                                  icdoEmployerPayrollHeader.pay_period_end_date,
                                                                  icdoEmployerPayrollHeader.org_id
                                                              });

                DataTable ldtbProviderList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadActiveDeffCompProviderDetailsByOrg",
                                                          new object[3]
                                                              {                  
                                                                   icdoEmployerPayrollHeader.org_id,
                                                                  icdoEmployerPayrollHeader.pay_period_start_date,
                                                                  icdoEmployerPayrollHeader.pay_period_end_date
                                                                 
                                                              });
                CreateDeffCompPayrollDetails(ldtbActiveMembers, ldtbProviderList, aboolFormpayrollReportGenration);

            }
        }

        private void CreateDeffCompPayrollDetails(DataTable adtbMemberList, DataTable adtbProviderList, Boolean aboolFormpayrollReportGenration=false)
        {
            if (adtbMemberList != null)
            {
                //Create Details for the Current Header
                foreach (DataRow ldrRow in adtbMemberList.Rows)
                {
                    busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusPerson.icdoPerson.LoadData(ldrRow);

                    busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
                    lbusPlan.icdoPlan.LoadData(ldrRow);

                    busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccount.icdoPersonAccount.LoadData(ldrRow);
                    //Filter the Provider Data by Person Account
                    DataRow[] larrRows = adtbProviderList.FilterTable(busConstant.DataType.Numeric, "person_account_id",
                                                 lbusPersonAccount.icdoPersonAccount.person_account_id);
                    if (larrRows != null && larrRows.Length > 0)
                    {
                        cdoEmployerPayrollDetail lobjCdoEmpDtl = new cdoEmployerPayrollDetail();
                        lobjCdoEmpDtl.employer_payroll_header_id = _icdoEmployerPayrollHeader.employer_payroll_header_id;
                        lobjCdoEmpDtl.person_id = lbusPerson.icdoPerson.person_id;
                        lobjCdoEmpDtl.first_name = lbusPerson.icdoPerson.first_name;
                        lobjCdoEmpDtl.last_name = lbusPerson.icdoPerson.last_name;
                        lobjCdoEmpDtl.ssn = lbusPerson.icdoPerson.ssn;
                        lobjCdoEmpDtl.pay_period_start_date = _icdoEmployerPayrollHeader.pay_period_start_date;
                        lobjCdoEmpDtl.pay_period_end_date = _icdoEmployerPayrollHeader.pay_period_end_date;
                        lobjCdoEmpDtl.pay_check_date = icdoEmployerPayrollHeader.pay_check_date;
                        if (iblnIsFromESS && icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment)
                            lobjCdoEmpDtl.record_type_value = busConstant.PayrollDetailRecordTypePositiveAdjustment;
                        else
                            lobjCdoEmpDtl.record_type_value = busConstant.PayrollDetailRecordTypeRegular;
                        lobjCdoEmpDtl.plan_id = lbusPlan.icdoPlan.plan_id;
                        lobjCdoEmpDtl.status_value = busConstant.PayrollDetailStatusReview;

                        for (int i = 0; i < larrRows.Length; i++)
                        {
                            DataRow ldrAmtRow = larrRows[i];
                            switch (i)
                            {
                                case 0:
                                    lobjCdoEmpDtl.contribution_amount1 = Convert.ToDecimal(ldrAmtRow["per_pay_period_contribution_amt"]);
                                    lobjCdoEmpDtl.provider_id1 = Convert.ToInt32(ldrAmtRow["org_id"]);
                                    lobjCdoEmpDtl.provider_org_code_id1 = ldrAmtRow["org_code"].ToString();
                                    break;
                                case 1:
                                    lobjCdoEmpDtl.contribution_amount2 = Convert.ToDecimal(ldrAmtRow["per_pay_period_contribution_amt"]);
                                    lobjCdoEmpDtl.provider_id2 = Convert.ToInt32(ldrAmtRow["org_id"]);
                                    lobjCdoEmpDtl.provider_org_code_id2 = ldrAmtRow["org_code"].ToString();
                                    break;
                                case 2:
                                    lobjCdoEmpDtl.contribution_amount3 = Convert.ToDecimal(ldrAmtRow["per_pay_period_contribution_amt"]);
                                    lobjCdoEmpDtl.provider_id3 = Convert.ToInt32(ldrAmtRow["org_id"]);
                                    lobjCdoEmpDtl.provider_org_code_id3 = ldrAmtRow["org_code"].ToString();
                                    break;
                                case 3:
                                    lobjCdoEmpDtl.contribution_amount4 = Convert.ToDecimal(ldrAmtRow["per_pay_period_contribution_amt"]);
                                    lobjCdoEmpDtl.provider_id4 = Convert.ToInt32(ldrAmtRow["org_id"]);
                                    lobjCdoEmpDtl.provider_org_code_id4 = ldrAmtRow["org_code"].ToString();
                                    break;
                                case 4:
                                    lobjCdoEmpDtl.contribution_amount5 = Convert.ToDecimal(ldrAmtRow["per_pay_period_contribution_amt"]);
                                    lobjCdoEmpDtl.provider_id5 = Convert.ToInt32(ldrAmtRow["org_id"]);
                                    lobjCdoEmpDtl.provider_org_code_id5 = ldrAmtRow["org_code"].ToString();
                                    break;
                                case 5:
                                    lobjCdoEmpDtl.contribution_amount6 = Convert.ToDecimal(ldrAmtRow["per_pay_period_contribution_amt"]);
                                    lobjCdoEmpDtl.provider_id6 = Convert.ToInt32(ldrAmtRow["org_id"]);
                                    lobjCdoEmpDtl.provider_org_code_id6 = ldrAmtRow["org_code"].ToString();
                                    break;
                                case 6:
                                    lobjCdoEmpDtl.contribution_amount7 = Convert.ToDecimal(ldrAmtRow["per_pay_period_contribution_amt"]);
                                    lobjCdoEmpDtl.provider_id7 = Convert.ToInt32(ldrAmtRow["org_id"]);
                                    lobjCdoEmpDtl.provider_org_code_id7 = ldrAmtRow["org_code"].ToString();
                                    break;
                            }
                        }
                        if (aboolFormpayrollReportGenration)
                        {
                            busEmployerPayrollDetail lbusEmployerPayrollDetail = new busEmployerPayrollDetail();
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail = lobjCdoEmpDtl;
                            lbusEmployerPayrollDetail.istrPlanValue = lbusPlan.icdoPlan.plan_code;
                            iclbEmployerPayrollDetail.Add(lbusEmployerPayrollDetail);
                        }
                        else
                        {
                            lobjCdoEmpDtl.Insert();
                        }
                    }
                }
            }
        }

        private void CreateRetirementPayrollDetails(DataTable adtbList, Boolean aboolFormpayrollReportGenration=false)
        {
            if (adtbList != null)
            {
                //Create Details for the Current Header
                foreach (DataRow ldrRow in adtbList.Rows)
                {
                    busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    lbusPerson.icdoPerson.LoadData(ldrRow);

                    busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
                    lbusPlan.icdoPlan.LoadData(ldrRow);

                    cdoEmployerPayrollDetail lobjCdoEmpDtl = new cdoEmployerPayrollDetail();
                    lobjCdoEmpDtl.employer_payroll_header_id = _icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lobjCdoEmpDtl.person_id = lbusPerson.icdoPerson.person_id;
                    lobjCdoEmpDtl.first_name = lbusPerson.icdoPerson.first_name;
                    lobjCdoEmpDtl.last_name = lbusPerson.icdoPerson.last_name;
                    lobjCdoEmpDtl.ssn = lbusPerson.icdoPerson.ssn;
                    lobjCdoEmpDtl.pay_period_date = _icdoEmployerPayrollHeader.payroll_paid_date;
                    lobjCdoEmpDtl.pay_period = _icdoEmployerPayrollHeader.pay_period;
                   // lobjCdoEmpDtl. = _icdoEmployerPayrollHeader.pay_period;
                    if (iblnIsFromESS && icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment)
                        lobjCdoEmpDtl.record_type_value = busConstant.PayrollDetailRecordTypePositiveAdjustment;
                    else
                        lobjCdoEmpDtl.record_type_value = busConstant.PayrollDetailRecordTypeRegular;
                    lobjCdoEmpDtl.status_value = busConstant.PayrollDetailStatusReview;
                    lobjCdoEmpDtl.plan_id = lbusPlan.icdoPlan.plan_id;
                    if (aboolFormpayrollReportGenration)
                    {
                        busEmployerPayrollDetail lbusEmployerPayrollDetail = new busEmployerPayrollDetail();
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail = lobjCdoEmpDtl;
                        lbusEmployerPayrollDetail.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                        lbusEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.LoadData(ldrRow);
                        lbusEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                        lbusEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldrRow);
                        lbusEmployerPayrollDetail.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                        lbusEmployerPayrollDetail.ibusPlan.icdoPlan.LoadData(ldrRow);
                        lbusEmployerPayrollDetail.istrPlanValue = lbusEmployerPayrollDetail.ibusPlan.icdoPlan.plan_code;                       
                        iclbEmployerPayrollDetail.Add(lbusEmployerPayrollDetail);
                    }
                    else
                    {
                        lobjCdoEmpDtl.Insert();
                    }
                }
            }
        }

        /// <summary>
        /// Posting the Employer Report
        /// </summary>
        public void PostEmployerReport(int aintBatchScheduleId)
        {
            //Validate the Payroll Before Posting (Only for Retirement and Deff Comp)
            if ((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) ||
                (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp) ||
                (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases))
            {
                //Validate the Payroll                
                btnValidate_Click();

                //Reload the Detail Collection After the Validation
                LoadEmployerPayrollDetailWithOtherObjects();

                //Assign the Payroll Header Object into Detail
                foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in iclbEmployerPayrollDetail)
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = this;
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = icdoEmployerPayrollHeader;

                    if (lobjEmployerPayrollDetail.ibusPerson == null)
                        lobjEmployerPayrollDetail.LoadPerson();

                    if (lobjEmployerPayrollDetail.ibusPlan == null)
                        lobjEmployerPayrollDetail.LoadPlan();

                    if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    {
                        lobjEmployerPayrollDetail.LoadOrgIdForProviders();
                    }
                }

                //Update the Header Status to Ready to Post if any Valid Details records found
                UpdateHeaderStatus();

                //Reload the Header to Avoid Record Changed Since Last Display Error
                _icdoEmployerPayrollHeader.Select();

                if (icdoEmployerPayrollHeader.status_value != busConstant.PayrollHeaderStatusReadyToPost) return;
            }

            //Allocate Remittance if Amount Matches and balancing status is not balanced
            if (_icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusBalanced)
            {
                AllocateAutomaticRemittance();
            }

            //Reload the Header to Avoid Record Changed Since Last Display Error
            _icdoEmployerPayrollHeader.Select();

            //method to update balancing status while posting
            //UpdateBalancingStatusWhilePosting();     //PIR 24044 - A review header should never be balanced       

            //Process the Negative Adjustments if exists in detail records (Load Only the Valid Status Records)
            ProcessNegativeAdjustments();

            //Posting GL Records for Regular,Positivie Adjustment, Bonus Detail Entries (ITEM Level GL)
            //Negative Adjustment GL process is at ProcessNegativeAdjustments Method (Allocation Level)
            PostEmployerReportingGL();

            //Insert into Report Data
            PostDetailToProvider();

            //Get the System Managerment Batch Date
            if (idatRunDate == DateTime.MinValue)
                idatRunDate = busGlobalFunctions.GetSysManagementBatchDate();

            //prod pir 6153 : interest not getting added to refund calc
            decimal ldecInterestAmount = 0.00M;

            //Update the Payroll Details Records to Posted Status except Ignored Entries
            foreach (busEmployerPayrollDetail lobjEmpDtl in _iclbEmployerPayrollDetail)
            {
                if (lobjEmpDtl.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusValid)
                {
                    ldecInterestAmount = 0.00M;

                    //Delete the Error Detail if any
                    lobjEmpDtl.DeleteDetailErrors();
                    //Post into contribution table as per header type
                    PostContributionByHeaderType(lobjEmpDtl);
                    if ((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) ||
                        (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp))
                    {
                        if (lobjEmpDtl.ibusPersonAccount == null)
                            lobjEmpDtl.LoadPersonAccount();

                        //Loading the Employment Detail to Post it Vesting Contribution
                        if (lobjEmpDtl.ibusPersonAccount.ibusPersonEmploymentDetail == null)
                            lobjEmpDtl.LoadPersonEmploymentDetail();

                        //UCS 43 - Post PEP and Adjustment
                        busPostingVestedERContributionBatch lobjVestedERCont = new busPostingVestedERContributionBatch();
                        if (((lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == 1) ||
                            (lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMain2020) || //PIR 20232
                            (lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == 2) ||
                            (lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == 3) ||
                            (lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == 20) ||
                            (lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdStatePublicSafety) || //PIR 25729
                            (lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdBCILawEnf)) 
                           // && (lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeBonus)  //PIR-17777
                            ) // pir 7943 //PIR 15616 - Pep adjustment does not apply to bonus
                        {
                            //prod pir 6586
                            //lobjVestedERCont.idtBenefitAccountInfo = idtBenefitAccountInfo;
                            lobjVestedERCont.ibusDBCacheData = ibusDBCacheData;
							//PIR-18292
                            if (lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus || lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)
                            {
                                DataTable ldtbList = Select<cdoEmployerPayrollBonusDetail>(
                                                                                             new string[1] { "employer_payroll_detail_id" },
                                                                                             new object[1] { lobjEmpDtl.icdoEmployerPayrollDetail.employer_payroll_detail_id }, null,                              
                                                                                             null);

                                if (ldtbList.Rows.Count > 0)
                                {
                                    foreach (DataRow ldrRow in ldtbList.Rows)
                                    {
                                        if (!Convert.IsDBNull(ldrRow["BONUS_PERIOD"]))
                                        {
                                            lobjVestedERCont.CalculatePEPAdjustment(
                                                lobjEmpDtl.icdoEmployerPayrollDetail.plan_id,
                                                lobjEmpDtl.icdoEmployerPayrollDetail.person_id,
                                                Convert.ToDateTime(ldrRow["BONUS_PERIOD"]),
                                                lobjEmpDtl.icdoEmployerPayrollDetail.employer_payroll_detail_id,
                                                lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value,
                                                lobjEmpDtl.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id,
                                                idatRunDate, lobjEmpDtl.ibusPersonAccount.icdoPersonAccount.person_account_id, busConstant.SubSystemValueEmployerReporting);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                lobjVestedERCont.CalculatePEPAdjustment(
                                    lobjEmpDtl.icdoEmployerPayrollDetail.plan_id,
                                    lobjEmpDtl.icdoEmployerPayrollDetail.person_id,
                                    (lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus || lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus) ? icdoEmployerPayrollHeader.payroll_paid_date : lobjEmpDtl.icdoEmployerPayrollDetail.pay_period_date,
                                    lobjEmpDtl.icdoEmployerPayrollDetail.employer_payroll_detail_id,
                                    lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value,
                                    lobjEmpDtl.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id,
                                    idatRunDate, lobjEmpDtl.ibusPersonAccount.icdoPersonAccount.person_account_id, busConstant.SubSystemValueEmployerReporting);
                            }
                        }
                        else if ((lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == 8) ||
                                 (lobjEmpDtl.icdoEmployerPayrollDetail.plan_id == 19))
                        {
                            //prod pir 6586
                            //lobjVestedERCont.idtBenefitAccountInfo = idtBenefitAccountInfo;
                            lobjVestedERCont.ibusDBCacheData = ibusDBCacheData;

                            lobjVestedERCont.CalculatePEPAdjustment(lobjEmpDtl.icdoEmployerPayrollDetail.plan_id,
                                                                    lobjEmpDtl.icdoEmployerPayrollDetail.person_id,
                                                                    lobjEmpDtl.icdoEmployerPayrollDetail.pay_period_end_date,
                                                                    lobjEmpDtl.icdoEmployerPayrollDetail.employer_payroll_detail_id,
                                                                    lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value,
                                                                    lobjEmpDtl.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id,
                                                                    idatRunDate, lobjEmpDtl.ibusPersonAccount.icdoPersonAccount.person_account_id, busConstant.SubSystemValueEmployerReporting);
                        }

                        //UAT PIR 517
                        //--Start--//
                        //PIR 11946
                        //GetPersonAccountAndInitiateWorkflow(lobjEmpDtl.ibusPersonAccount.ibusPersonEmploymentDetail, lobjEmpDtl);
                        //--End--//

                        //UCS 43 Post Interest Adjustment
                        if (
                            //((lobjEmpDtl.icdoEmployerPayrollDetail.member_interest_calculated == 0) ||
                            // (lobjEmpDtl.icdoEmployerPayrollDetail.employer_interest_calculated == 0)) && //PIR 16349
                            //(lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value != "BONS") &&
                            //(icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypePurchases) &&
                            (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt))
                        {
                            //if (_ibusPlan == null)
                            //    LoadPlan(lobjEmpDtl.icdoEmployerPayrollDetail.plan_id);

                            if (lobjEmpDtl.ibusPlan.icdoPlan.allow_interest_posting == 1)
                            {
                                decimal ldecBalance = lobjEmpDtl.icdoEmployerPayrollDetail.ee_contribution_reported +
                                                      lobjEmpDtl.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                                      lobjEmpDtl.icdoEmployerPayrollDetail.ee_pre_tax_reported;

                                if ((lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment) || (lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus))
                                    ldecBalance = ldecBalance * -1;

                                busPostingInterestBatch lobjInterestBatch = new busPostingInterestBatch();
                                ldecInterestAmount = lobjInterestBatch.CalculateInterestAdjustment(
                                                    lobjEmpDtl.icdoEmployerPayrollDetail.plan_id,
                                                    lobjEmpDtl.icdoEmployerPayrollDetail.person_id,
                                                    ((lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) || (lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)) ? icdoEmployerPayrollHeader.payroll_paid_date
                                                    : lobjEmpDtl.icdoEmployerPayrollDetail.pay_period_date, 
                                                    ldecBalance,
                                                    lobjEmpDtl.icdoEmployerPayrollDetail.employer_payroll_detail_id, 
                                                    idatRunDate,
                                                    lobjEmpDtl.ibusPersonAccount.icdoPersonAccount.person_account_id, 
                                                    busConstant.SubSystemValueEmployerReporting);
                            }
                        }
                    }

                    //PIR 17140, 16601, 16896
                    if ((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) ||
                        (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp))
                    {
                        PutFinalCalcInReviewOrAdjustPayeeAccountOnAddlContriPosting(lobjEmpDtl);
                    }
                    //Update the Payroll Detail Status
                    lobjEmpDtl.icdoEmployerPayrollDetail.posted_date = idatRunDate;
                    lobjEmpDtl.icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusPosted;
                    lobjEmpDtl.icdoEmployerPayrollDetail.Update();

                    //PIR 24243
                    if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        DateTime ldtPayPeriodDate = (lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus || lobjEmpDtl.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus) ? icdoEmployerPayrollHeader.payroll_paid_date : lobjEmpDtl.icdoEmployerPayrollDetail.pay_period_date;
                        decimal ldecTotalSalaryForPayPeriodDate = 0;
                        DataTable ldtbEligibleWages = Select("entPersonAccountRetirementContribution.GetAcctIDAndSalaryForRetirementContByPayPeriod",
                                                         new object[2] { ldtPayPeriodDate, lobjEmpDtl.icdoEmployerPayrollDetail.person_id});
                        foreach (DataRow dr in ldtbEligibleWages.Rows)
                        {
                            ldecTotalSalaryForPayPeriodDate += Convert.ToDecimal(dr["salary_amount"].ToString());
                        }

                        //PIR 26602
                        decimal ldecMonthlyLimit = 0.0m;
                        List<utlCodeValue> lclb401MonthlyLimit = iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(7025);
                        utlCodeValue icdoCodeValue = lclb401MonthlyLimit.Where(i => Convert.ToDateTime(i.data1) <= lobjEmpDtl.icdoEmployerPayrollDetail.pay_period_date).OrderByDescending(i => Convert.ToDateTime(i.data1)).FirstOrDefault();
                        if (icdoCodeValue.IsNotNull())
                        {
                            ldecMonthlyLimit = Convert.ToDecimal(icdoCodeValue.data2) / 12;
                        }
                        if (ldecTotalSalaryForPayPeriodDate >= Math.Round(ldecMonthlyLimit, 2, MidpointRounding.AwayFromZero))
                        {
                            UpdatePerson401aLimitFlagToY(lobjEmpDtl.icdoEmployerPayrollDetail.person_id); //set Limit_401a = Y
                        }
                    }
                }
            }
            //Update the Header Status to Posted           
            if (iclbEmployerPayrollDetail.Any(i => i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusReview))
            {
                _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReview;
            }
            else
            {
                //PIR 18343 - Retirement headers were getting posted with soft errors, especially headers whose contribution calculated not matching with contribution reported within $1
                //variance
                if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    btnValidate_Click();
                }
                if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt && icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusReview)
                {
                    //PIR 18343 - Retirement headers were getting posted with soft errors, especially headers whose contribution calculated not matching with contribution reported within $1
                    //variance
                }
                else
                {
                    _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusPosted;
                    UpdateBalancingStatusWhilePosting(); //PIR 24044 - A review header should never be balanced
                    //PIR 24035
                    if (ibusOrganization.IsNull()) LoadOrganization();
                    Collection<busCodeValue> lclbLineOfDutySurvivorOrgCode = busGlobalFunctions.LoadData1ByCodeID(7022);
                    bool lblnIsLineOfDutySurvivor = lclbLineOfDutySurvivorOrgCode.Any(lbus => lbus.icdoCodeValue.data1 == this.ibusOrganization.icdoOrganization.org_code);
                    if(lblnIsLineOfDutySurvivor && icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusBalanced)
                    {
                        _icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusBalanced;
                    }
                    _icdoEmployerPayrollHeader.posted_date = idatRunDate;
                    //UCS - 32 : BR - 032 - 69
                    //Posting message board when employer payroll header is posted
                    PublishMessageBoard();
                }
            }
            _icdoEmployerPayrollHeader.Update();

            //UAT PIR 1047 - If any Positive and Negative Adjustment Exists and Balancing Status is not balance, allocate the Neg Adj Remittance to Postive Adjustment
            if (icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusBalanced)
            {
                AllocateRemittanceForPostiveAdjustments();
            }
            //PIR-6501 Allocate Remittance to Employer payroll headers.
            if (_icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusPosted && icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusBalanced)
            {
                AllocateRemittanceToEmployerPayroll(icdoEmployerPayrollHeader.header_type_value, aintBatchScheduleId);
            }
        }

        /// <summary>
        /// PIR 24243 - To set Limit_401a flag on Person to Y 
        /// </summary>
        /// <param name="aintPersonId"></param>
        private void UpdatePerson401aLimitFlagToY(int aintPersonId)
        {
            DBFunction.DBNonQuery("entPerson.UpdatePerson401aLimitFlagToY",
                                       new object[2] { iobjPassInfo.istrUserID, aintPersonId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        private void PutFinalCalcInReviewOrAdjustPayeeAccountOnAddlContriPosting(busEmployerPayrollDetail abusEmpPayrollDetail)
        {
            //try
            //{
                if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
                                abusEmpPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    bool lblnIsCalcPutIntoReview = false;
                    lblnIsCalcPutIntoReview = busGlobalFunctions
                        .PutCalculationInReviewIfExists(abusEmpPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id);
                    if (!lblnIsCalcPutIntoReview)
                    {
                        DataTable ldtbPostedRetContributions = Select("cdoPersonAccountRetirementContribution.LoadPostedContributionsByERP", new object[2] { abusEmpPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id, abusEmpPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id });
                        if (ldtbPostedRetContributions.Rows.Count > 0)
                        {
                            busPersonAccountAdjustment lbusPersonAccountAdjustment = new busPersonAccountAdjustment();
                            lbusPersonAccountAdjustment.ibusPersonAccount = abusEmpPayrollDetail.ibusPersonAccount;
                            lbusPersonAccountAdjustment.ibusPersonAccount.icdoPersonAccount = abusEmpPayrollDetail.ibusPersonAccount.icdoPersonAccount;
                            lbusPersonAccountAdjustment.ibusPersonAccount.ibusPerson = abusEmpPayrollDetail.ibusPerson;
                            lbusPersonAccountAdjustment.ibusPersonAccount.ibusPerson.icdoPerson = abusEmpPayrollDetail.ibusPerson.icdoPerson;
                            lbusPersonAccountAdjustment.AdjustPayeeAccountOrCreateAdjtCalc(abusEmpPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id, ldtbPostedRetContributions, true);
                        }
                    }
                }
                else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp &&
                    abusEmpPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    //On deferred comp detail posting, DUE to PEP logic, there is a chance that the member's retirement contributions may get adjusted
                    //so this logic of putting final benefit calculation if exists in review or adjusting payee accounts needs to be called 
                    //for deferred comp header posting as well. 
                    DataTable ldtbEligibleWages = Select("cdoPersonAccountRetirementContribution.GetAcctIDAndSalaryForRetirementCont",
                        new object[2] { abusEmpPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_date, abusEmpPayrollDetail.icdoEmployerPayrollDetail.person_id });
                    List<int> llstPersonAccountIds = null;
                    if (ldtbEligibleWages.Rows.Count > 0)
                        llstPersonAccountIds = ldtbEligibleWages.AsEnumerable().Select(dr => dr.Field<int>("person_account_id")).ToList();
                    if (llstPersonAccountIds.IsNotNull() && llstPersonAccountIds.Count() > 0)
                    {
                        foreach (int lintPersonAccountId in llstPersonAccountIds)
                        {
                            DataTable ldtbPostedRetContributions = Select("cdoPersonAccountRetirementContribution.LoadPostedContributionsByERP", new object[2] { lintPersonAccountId, abusEmpPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id });
                            if (ldtbPostedRetContributions.Rows.Count > 0)
                            {
                                bool lblnIsCalcuPutIntoReview = false;
                                lblnIsCalcuPutIntoReview = busGlobalFunctions.PutCalculationInReviewIfExists(lintPersonAccountId);
                                if (!lblnIsCalcuPutIntoReview)
                                {
                                    busPersonAccountAdjustment lbusPersonAccountAdjustment = new busPersonAccountAdjustment();
                                    busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                                    if (lbusPersonAccount.FindPersonAccount(lintPersonAccountId))
                                    {
                                        lbusPersonAccountAdjustment.ibusPersonAccount = lbusPersonAccount;
                                        lbusPersonAccountAdjustment.ibusPersonAccount.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                                        lbusPersonAccountAdjustment.ibusPersonAccount.ibusPerson = abusEmpPayrollDetail.ibusPerson;
                                        lbusPersonAccountAdjustment.ibusPersonAccount.ibusPerson.icdoPerson = abusEmpPayrollDetail.ibusPerson.icdoPerson;
                                        lbusPersonAccountAdjustment.AdjustPayeeAccountOrCreateAdjtCalc(lintPersonAccountId, ldtbPostedRetContributions, true);
                                    }
                                }
                            }
                        }
                    }
                }
            //}
            //catch(Exception ex)
            //{
            //    Sagitec.ExceptionPub.ExceptionManager.Publish(ex);
            //}
        }

        private void AllocateRemittanceForPostiveAdjustments()
        {
            //If Any Postive Adjustment Record Exists
            var lenuPayEmpDetail = iclbEmployerPayrollDetail.Where(i => i.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment &&
                                                                       i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPosted);
            if ((lenuPayEmpDetail != null) && (lenuPayEmpDetail.Count() > 0))
            {
                //Load the Total Contribution By Plan
                LoadContributionByPlan();
                LoadTotalAppliedRemittance();

                decimal ldecBalanceRemittanceToBeApplied = idecTotalReportedByPlan - idecTotalRemittanceApplied;
                if (ldecBalanceRemittanceToBeApplied > 0)
                {
                    //Load All Negative Adjustment Remittance
                    DataTable ldtbNegAdjustments = Select("cdoEmployerPayrollHeader.LoadRemittanceByPayrollHeader",
                                                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
                    Collection<busRemittance> lclbNARemittance = GetCollection<busRemittance>(ldtbNegAdjustments, "icdoRemittance");
                    foreach (busRemittance lbusRemittance in lclbNARemittance)
                    {
                        decimal ldecAvailableAmount = busEmployerReportHelper.GetRemittanceAvailableAmount(lbusRemittance.icdoRemittance.remittance_id);
                        decimal ldecReportedAmount = 0.00M;
                        if (ldecAvailableAmount > 0)
                        {
                            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                            {
                                ldecReportedAmount = 0.00M;
                                foreach (busEmployerPayrollDetail lbusDetail in lenuPayEmpDetail)
                                {
                                    if (lbusDetail.icdoEmployerPayrollDetail.plan_id == lbusRemittance.icdoRemittance.plan_id)
                                    {
                                        if (lbusRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeContribution)
                                        {
                                            ldecReportedAmount = lbusDetail.icdoEmployerPayrollDetail.ee_contribution_reported +
                                                                 lbusDetail.icdoEmployerPayrollDetail.er_contribution_reported +
                                                                 lbusDetail.icdoEmployerPayrollDetail.member_interest_calculated +
                                                                 lbusDetail.icdoEmployerPayrollDetail.employer_interest_calculated +
                                                                 lbusDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated +
                                                                 lbusDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                                                 lbusDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported +
                                                                 lbusDetail.icdoEmployerPayrollDetail.adec_reported + 
                                                                 lbusDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported +
                                                                 lbusDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported +
                                                                 lbusDetail.icdoEmployerPayrollDetail.er_pretax_match_reported;//PIR 25920 New Plan DC 2025
                                        }
                                        else if (lbusRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeRHICContribution)
                                        {
                                            ldecReportedAmount = lbusDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported +
                                                                 lbusDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported;
                                        }
                                    }
                                }
                            }
                            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                            {
                                ldecReportedAmount = 0.00M;
                                foreach (busEmployerPayrollDetail lbusDetail in lenuPayEmpDetail)
                                {
                                    if ((lbusDetail.icdoEmployerPayrollDetail.plan_id == lbusRemittance.icdoRemittance.plan_id) &&
                                        (lbusDetail.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457))
                                    {
                                        ldecReportedAmount = lbusDetail.icdoEmployerPayrollDetail.contribution_amount1 +
                                                             lbusDetail.icdoEmployerPayrollDetail.contribution_amount2 +
                                                             lbusDetail.icdoEmployerPayrollDetail.contribution_amount3 +
                                                             lbusDetail.icdoEmployerPayrollDetail.contribution_amount4 +
                                                             lbusDetail.icdoEmployerPayrollDetail.contribution_amount5 +
                                                             lbusDetail.icdoEmployerPayrollDetail.contribution_amount6 +
                                                             lbusDetail.icdoEmployerPayrollDetail.contribution_amount7 +
                                                             lbusDetail.icdoEmployerPayrollDetail.er_pretax_match_reported;//PIR 25920 New Plan DC 2025
                                    }
                                }
                            }
                            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                            {
                                ldecReportedAmount = 0.00M;
                                foreach (busEmployerPayrollDetail lbusDetail in lenuPayEmpDetail)
                                {
                                    if (lbusDetail.icdoEmployerPayrollDetail.plan_id == lbusRemittance.icdoRemittance.plan_id)
                                    {
                                        ldecReportedAmount = lbusDetail.icdoEmployerPayrollDetail.premium_amount;
                                    }
                                }
                            }

                            //Create and Allocate the Employer Remittance Allocation
                            decimal ldecAllocateAmount = Math.Min(ldecBalanceRemittanceToBeApplied, Math.Min(ldecAvailableAmount, ldecReportedAmount));
                            if (ldecAllocateAmount > 0)
                            {
                                CreateRemittanceAllocationAndAllocate(ldecAllocateAmount, lbusRemittance.icdoRemittance.remittance_id, 0.00M, string.Empty);
                                ldecBalanceRemittanceToBeApplied = ldecBalanceRemittanceToBeApplied - ldecAllocateAmount;
                            }
                        }
                    }
                }
            }
        }

        public void ReloadInsurance(bool ablnIsFromReloadInsuranceBatch = false)
        {
            //setting reload date          
            busReloadInsuranceBatch lobjReloadInsuranceBatch = new busReloadInsuranceBatch();

            //Delete the Existing Detail Records
            lobjReloadInsuranceBatch.DeletePayrollDetails(this);

            //Deleting the Allocated Remittance
            lobjReloadInsuranceBatch.DeleteRemittanceAllocation(this);

            busPostingInsuranceBatch lobjPostingInsuranceBatch = new busPostingInsuranceBatch();
            lobjPostingInsuranceBatch.ibusEmployerPayrollHeader = this;
            lobjPostingInsuranceBatch.ibusEmployerPayrollHeader.iblnIsReloadFromScreen = true; //PIR - 14493
            //Load the Org Object
            if (ibusOrganization == null)
                LoadOrganization();
            //prod pir 933
            lobjPostingInsuranceBatch.LoadPersonAccountDepenedents();
            //PIR 15610 - If we do not load orgplans here, the provider org id of the detail object is loaded as though the employee is retired even if he is actively employed
            lobjPostingInsuranceBatch.iclbOrgPlan = LoadInsOrgPlansByOrgForReload();
            //Reload the Detail and Process Validation
            lobjPostingInsuranceBatch.CreatePayrollDetailCollecton(ibusOrganization, false);
            lobjPostingInsuranceBatch.ValidatePayrollDetail();

            lobjPostingInsuranceBatch.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.last_reload_run_date = DateTime.Now;

            //Override the Header Status (Reload Batch should update the Header Status to Ready to Post 
            //if all the detail records are in VALID status
            lobjReloadInsuranceBatch.OverrideHeaderStatus(this, ablnIsFromReloadInsuranceBatch);

            //Update the Last Reload Run Date & Status (If Available)
            icdoEmployerPayrollHeader.Update();
        }

        private List<busOrgPlan> LoadInsOrgPlansByOrgForReload()
        {
            List<busOrgPlan> lclbOrgPlan = new List<busOrgPlan>();
            DataTable ldtbOrgPlansByOrgForReload = Select("cdoEmployerPayrollHeader.LoadInsOrgPlansByOrgForReload", new object[2] { _icdoEmployerPayrollHeader.payroll_paid_date, icdoEmployerPayrollHeader.org_id });
            foreach (DataRow ldrRow in ldtbOrgPlansByOrgForReload.Rows)
            {
                var lbusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                lbusOrgPlan.icdoOrgPlan.LoadData(ldrRow);

                lbusOrgPlan.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusOrgPlan.ibusOrganization.icdoOrganization.LoadData(ldrRow);

                lbusOrgPlan.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusOrgPlan.ibusPlan.icdoPlan.LoadData(ldrRow);

                lclbOrgPlan.Add(lbusOrgPlan);
            }
            return lclbOrgPlan;
        }

        /// <summary>
        /// Post into Contribution by Header type
        /// </summary>
        /// <param name="aobjEmployerPayrollDetail"></param>
        public void PostContributionByHeaderType(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {

            iintCredit = IsNegativeAdjustmentPayrollDetail(aobjEmployerPayrollDetail);
            //int lintIndex = 1;
            //if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
            //{
            //    lintIndex = -1;
            //}
            int lintPersonID = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id;
            int lintPlanID = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id;
            int lintOrgID = icdoEmployerPayrollHeader.org_id;

            //Get the Person Account Object
            if (aobjEmployerPayrollDetail.ibusPersonAccount == null)
            {
                aobjEmployerPayrollDetail.LoadPersonAccount();
            }

            if (aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                //Load the Employment Detail
                if (aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail == null)
                    aobjEmployerPayrollDetail.LoadPersonEmploymentDetail(iblnLoadMemberMemberType);

                if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    PostRetirementContribution(aobjEmployerPayrollDetail, iintCredit);
                }
                else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    //insert into Insurance contribution
                    PostInsuranceContribution(aobjEmployerPayrollDetail, iintCredit);
                }
                else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    PostDeferredCompContribution(aobjEmployerPayrollDetail, iintCredit);
                }
            }
            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                PostServicePurchaseContribution(aobjEmployerPayrollDetail, iintCredit);
            }
        }

        public bool iblnLoadMemberMemberType
        {
            get
            {
                if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                    return false;
                return true;
            }
        }

        private void PostInsuranceContribution(busEmployerPayrollDetail aobjEmployerPayrollDetail, int aintIndex)
        {
            busPersonAccountInsuranceContribution lobjPersonAccountInsuranceContribution = new busPersonAccountInsuranceContribution();
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution = new cdoPersonAccountInsuranceContribution();
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_id =
                aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.person_employment_dtl_id =
                aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;

            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.due_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount * aintIndex;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount * aintIndex;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.group_health_fee_amt = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.group_health_fee_amount * aintIndex;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.buydown_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.buydown_amount * aintIndex;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.medicare_part_d_amt = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.medicare_part_d_amt * aintIndex;//PIR 14271
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.rhic_benefit_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_benefit_amount * aintIndex;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_basic_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.basic_premium;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_supp_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.supplemental_premium;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_spouse_supp_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.spouse_premium;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_dep_supp_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.dependent_premium;

            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_member_three_yrs_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ltc_member_three_yrs_premium;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_member_five_yrs_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ltc_member_five_yrs_premium;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_spouse_three_yrs_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ltc_spouse_three_yrs_premium;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ltc_spouse_five_yrs_premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ltc_spouse_five_yrs_premium;

            /* UAT PIR 476, Including other and JS RHIC Amount */
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.othr_rhic_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.othr_rhic_amount * aintIndex;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.js_rhic_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.js_rhic_amount * aintIndex;
            /* UAT PIR 476 ends here */
            //PIR 7705
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.hsa_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.hsa_amount * aintIndex;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.vendor_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.vendor_amount * aintIndex;
            //End 7705
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_ref_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_date = busGlobalFunctions.GetSysManagementBatchDate();

            if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular)
            {
                lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_type_value = busConstant.TransactionTypeRegularPayroll;
            }
            else
            {
                lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_type_value = busConstant.TransactionTypePayrollAdjustment;
            }
            //uat pir 1429 : post ghdv_history_id and group number
            //prod pir 6076 & 6077 - Removal of person account ghdv history id
            //lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_ghdv_history_id =
                //aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_account_ghdv_history_id;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.group_number =
                aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.group_number;
            //prod pir 6076
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.coverage_code = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.coverage_code;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.rate_structure_code = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rate_structure_code;

            //prod pir 4260
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ad_and_d_basic_premium_rate = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ad_and_d_basic_premium_rate;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.ad_and_d_supplemental_premium_rate = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ad_and_d_supplemental_premium_rate;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_basic_coverage_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.life_basic_coverage_amount;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_supp_coverage_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.life_supp_coverage_amount;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_spouse_supp_coverage_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.life_spouse_supp_coverage_amount;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.life_dep_supp_coverage_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.life_dep_supp_coverage_amount;
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_id;
            
            lobjPersonAccountInsuranceContribution.icdoPersonAccountInsuranceContribution.Insert();
        }

        private void PostDeferredCompContribution(busEmployerPayrollDetail aobjEmployerPayrollDetail, int lintIndex)
        {
            busPersonAccountDeferredCompContribution lobjPersonAccountDefCompContribution = new busPersonAccountDeferredCompContribution();
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution = new cdoPersonAccountDeferredCompContribution();
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_end_date = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_date;
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_start_date = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_start_date;
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.person_account_id = aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id;
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.effective_date = icdoEmployerPayrollHeader.received_date;
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.person_employment_dtl_id = aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.subsystem_ref_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.transaction_date = busGlobalFunctions.GetSysManagementBatchDate();
            lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.paid_date = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_check_date;
            if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular)
            {
                lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.transaction_type_value = busConstant.TransactionTypeRegularPayroll;
            }
            else
            {
                lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.transaction_type_value = busConstant.TransactionTypePayrollAdjustment;
            }
            //PIR 25920 DC2025 Changes
            iblnIsPostDeferredCompContribution = false;
            //for each loop and insert for each provider
            if ((aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id1 != 0) && (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 != 0.00M))
            {
                //insert into def comp contribution for provider 1
                PostDeferredCompContributionDetail(lobjPersonAccountDefCompContribution, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id1, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 * lintIndex);
            }
            if ((aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id2 != 0) && (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 != 0.00M))
            {
                //insert into def comp contribution for provider 2
                PostDeferredCompContributionDetail(lobjPersonAccountDefCompContribution, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id2, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 * lintIndex);
            }
            if ((aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id3 != 0) && (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 != 0.00M))
            {
                //insert into def comp contribution for provider 3
                PostDeferredCompContributionDetail(lobjPersonAccountDefCompContribution, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id3, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 * lintIndex);
            }
            if ((aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id4 != 0) && (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 != 0.00M))
            {
                //insert into def comp contribution for provider 4
                PostDeferredCompContributionDetail(lobjPersonAccountDefCompContribution, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id4, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 * lintIndex);
            }
            if ((aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id5 != 0) && (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 != 0.00M))
            {
                //insert into def comp contribution for provider 5
                PostDeferredCompContributionDetail(lobjPersonAccountDefCompContribution, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id5, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 * lintIndex);
            }
            if ((aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id6 != 0) && (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 != 0.00M))
            {
                //insert into def comp contribution for provider 6
                PostDeferredCompContributionDetail(lobjPersonAccountDefCompContribution, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id6, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 * lintIndex);
            }
            if ((aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id7 != 0) && (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 != 0.00M))
            {
                //insert into def comp contribution for provider 7
                PostDeferredCompContributionDetail(lobjPersonAccountDefCompContribution, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_id7, aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7 * lintIndex);
            }
            //PIR 25920 New Plan DC 2025
            if (!iblnIsPostDeferredCompContribution)
            {
                aobjEmployerPayrollDetail.CheckMemberPayrollIsPostedInDefComp();
                //insert into def comp contribution for No provider 
                PostDeferredCompContributionDetail(lobjPersonAccountDefCompContribution, aobjEmployerPayrollDetail.iintPreviousPayrollProviderOrgID, 0);
            }
            DBFunction.DBNonQuery("cdoEmployerPayrollDetail.UpdateDefCompEmprMatchFromERPosting", new object[1] { aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id },
                   iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //lobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.Select();
        }

        // Date used in posting codes. Specially batches
        private DateTime _idatRunDate;
        public DateTime idatRunDate
        {
            get
            {
                return _idatRunDate;
            }
            set
            {
                _idatRunDate = value;
            }
        }
        // Post service purchase with the payroll detail
        private void PostServicePurchaseContribution(busEmployerPayrollDetail abusEmployerPayrollDetail, int aintPostDirection)
        {
            DateTime ldtePaymentDate = DateTime.MinValue; //service Purchase PIR-13599 repurposed under PIR-9224
            if (abusEmployerPayrollDetail.iclbEmployerPurchaseAllocation == null)
                abusEmployerPayrollDetail.LoadEmployerPurchaseAllocation();
            foreach (busEmployerPurchaseAllocation lbusPurchaseAllocation in abusEmployerPayrollDetail.iclbEmployerPurchaseAllocation)
            {
                if (lbusPurchaseAllocation.ibusServicePurchaseHeader == null)
                    lbusPurchaseAllocation.LoadPurchaseHeader();
                lbusPurchaseAllocation.ibusServicePurchaseHeader.idatRunDate = idatRunDate;
                if (lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount == null)
                    lbusPurchaseAllocation.ibusServicePurchaseHeader.LoadPersonAccount();
                if (lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                    throw new Exception("An open person account for retirement is not found for person / plan.");
                if (lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPlan == null)
                    lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.LoadPlan();
                DataTable ldtbRemiitanceApplied = busNeoSpinBase.Select("cdoDeposit.GetPaymentDateForEmployer", new object[1]{
                                 abusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id});
                if (ldtbRemiitanceApplied.Rows.Count > 0 && ldtbRemiitanceApplied.Rows[0]["PAYMENT_DATE"] != DBNull.Value) 
                    ldtePaymentDate = Convert.ToDateTime(ldtbRemiitanceApplied.Rows[0]["PAYMENT_DATE"]);
                decimal ldecPreTaxEE = 0, ldecPostTaxEE = 0, ldecPostTaxER = 0, ldecPostTaxEERHIC = 0, ldecPreTaxERRHIC = 0, ldecPreTaxER = 0, ldecEREEPickup = 0;
                decimal ldecPSCToPost = 0, ldecVSCToPost = 0;
                string lstrPayClass = abusEmployerPayrollDetail.icdoEmployerPayrollDetail.payment_class_value;
                //PIR 25705	Sara Stafford#248428 Purchase ID 66980 allocated as down payment should be Pretax, do not override the pay class here casuse the service credit always 0 here
                //if ((lbusPurchaseAllocation.ibusServicePurchaseHeader.icdoServicePurchaseHeader.paid_pension_service_credit == 0) &&
                //    (lbusPurchaseAllocation.ibusServicePurchaseHeader.icdoServicePurchaseHeader.paid_vesting_service_credit == 0))
                //    lstrPayClass = busConstant.Service_Purchase_Class_Employer_Down_Payment;
                lbusPurchaseAllocation.ibusServicePurchaseHeader.GetDistributedRemittance(lstrPayClass,
                    lbusPurchaseAllocation.icdoEmployerPurchaseAllocation.allocated_amount, ref ldecPreTaxEE, ref ldecPostTaxEE, ref ldecPostTaxER,
                    ref ldecPostTaxEERHIC, ref ldecPreTaxERRHIC, ref ldecPreTaxER, ref ldecEREEPickup);
                //Posting GL
                lbusPurchaseAllocation.ibusServicePurchaseHeader.PostGLAccount(ldecPreTaxEE, ldecPostTaxEE,
                                                                               ldecPostTaxER, ldecPostTaxEERHIC,
                                                                               ldecPreTaxERRHIC, ldecPreTaxER, 1, true, icdoEmployerPayrollHeader.org_id); // 938 - GL changes
                if (lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPlan.IsDCRetirementPlan() ||
                    lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPlan.IsHBRetirementPlan())
                {
                    //Post report to DC provider
                    if (lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPerson == null)
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.LoadPerson();
                    busProviderReportDataDC lbusProviderReport = new busProviderReportDataDC();
                    lbusProviderReport.PostContribution(busConstant.SubSystemValueServicePurchase,
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id,
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPerson.icdoPerson.ssn,
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPerson.icdoPerson.person_id,
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPerson.icdoPerson.last_name,
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.ibusPerson.icdoPerson.first_name,
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.plan_id,
                        idatRunDate,
                        ldecPostTaxER, ldecPostTaxEE, ldecPreTaxER, ldecPreTaxEE, ldecEREEPickup, 0, 0,
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.person_account_id,
                        lbusPurchaseAllocation.ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.provider_org_id);
                }

                //Insert the Payment Allocation Information into Purchase Payment Allocaton Table, So that, 
                //Amortization  Schedule will have these entries
                busServicePurchasePaymentAllocation lbusAllocatedRemittance = new busServicePurchasePaymentAllocation();
                lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation = new cdoServicePurchasePaymentAllocation();
                cdoServicePurchasePaymentAllocation lcdoSPPayAlloc = lbusAllocatedRemittance.icdoServicePurchasePaymentAllocation;

                lcdoSPPayAlloc.service_purchase_header_id = lbusPurchaseAllocation.ibusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id;
                lcdoSPPayAlloc.employer_payroll_detail_id = abusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
                lcdoSPPayAlloc.service_purchase_payment_class_value = lstrPayClass;
                // service Purchase PIR-13599 repurposed under PIR-9224
                lcdoSPPayAlloc.payment_date = (ldtePaymentDate == DateTime.MinValue) ? idatRunDate : ldtePaymentDate;
                //lcdoSPPayAlloc.payment_date = idatRunDate;
                lcdoSPPayAlloc.applied_amount = lbusPurchaseAllocation.icdoEmployerPurchaseAllocation.allocated_amount;
                lcdoSPPayAlloc.posted_flag = busConstant.Flag_Yes;
                lcdoSPPayAlloc.Insert();

                //Reload the Amortization Schdule to get the PayOff Amount
                lbusPurchaseAllocation.ibusServicePurchaseHeader.LoadAmortizationSchedule();

                lbusPurchaseAllocation.ibusServicePurchaseHeader.PostPersonAccountRetirement(
                    ldecPreTaxEE, ldecPostTaxEE, ldecPostTaxER, ldecPostTaxEERHIC, ldecPreTaxERRHIC,
                    ldecPreTaxER, ldecEREEPickup, aintPostDirection, ref ldecPSCToPost, ref ldecVSCToPost, idatRunDate,
                    lbusAllocatedRemittance);

                //Update the Payment Allocation Entry with Allocated Buckets
                lcdoSPPayAlloc.prorated_psc = ldecPSCToPost;
                lcdoSPPayAlloc.prorated_vsc = ldecVSCToPost;
                lcdoSPPayAlloc.pre_tax_ee_amount = ldecPreTaxEE;
                lcdoSPPayAlloc.pre_tax_er_amount = ldecPreTaxER;
                lcdoSPPayAlloc.post_tax_ee_amount = ldecPostTaxEE;
                lcdoSPPayAlloc.post_tax_er_amount = ldecPostTaxER;
                lcdoSPPayAlloc.ee_rhic_amount = ldecPostTaxEERHIC;
                lcdoSPPayAlloc.er_rhic_amount = ldecPreTaxERRHIC;
                lcdoSPPayAlloc.Update();
            }
        }

        private void PostRetirementContribution(busEmployerPayrollDetail aobjEmployerPayrollDetail, int lintIndex)
        {
            busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year = Convert.ToInt32(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Year);
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month = Convert.ToInt32(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Month);
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            //Changed the Effective Date from header payroll paid last date to Detail Payroll Paid Last Date. (As Per Satya Comments)
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_last_date;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_employment_dtl_id = aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id;
            lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = busGlobalFunctions.GetSysManagementBatchDate();

            //Common Fields Assignment for Regualar & +ADJ
            if ((aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular) ||
                (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment) ||
                (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment))
            {
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.eligible_wages * lintIndex;//PIR 24770
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_rhic_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.er_rhic_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported * lintIndex;
                //PIR 25920 New Plan DC 2025
				lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_pretax_addl_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_post_tax_addl_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.er_pretax_match_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.adec_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported  * lintIndex;
            }

            //Service Credit Calculation (Regular)
            if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular)
            {
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeRegularPayroll;

                if (IsServiceCreditAlreadyPosted(aobjEmployerPayrollDetail))
                {
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit = 0M;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit = 0M;
                }
                else
                {
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit =
                        lintIndex * Math.Round((decimal)1, 6, MidpointRounding.AwayFromZero);
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit =
                        lintIndex * Math.Round((decimal)1, 6, MidpointRounding.AwayFromZero);
                }
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
            }

            //For +ADJ, If any entry already exists in contribution table, dont post the VSC, PSC
            if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment)
            {
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypePayrollAdjustment;

                if (IsServiceCreditAlreadyPosted(aobjEmployerPayrollDetail))
                {
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit = 0M;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit = 0M;
                }
                else
                {
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit =
                        lintIndex * Math.Round((decimal)1, 6, MidpointRounding.AwayFromZero);
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit =
                        lintIndex * Math.Round((decimal)1, 6, MidpointRounding.AwayFromZero);
                }

                lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
            }

            //Negative Adjustment - If the Total Salary from Contribution Table matches with reported, post the VSC, PSC else post zero.
            if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
            {
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypePayrollAdjustment;

                DataTable ldtbList = Select<cdoPersonAccountRetirementContribution>(
                    new string[3] { "person_account_id", "pay_period_month", "pay_period_year" },
                    new object[3]
                        {
                            aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id,                            
                            aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Month,
                            aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Year
                        }, null, null);
                decimal ldecContributionWages = 0.00M;
                foreach (DataRow ldrRow in ldtbList.Rows)
                {
                    if (!Convert.IsDBNull(ldrRow["SALARY_AMOUNT"]))
                        ldecContributionWages += (decimal)ldrRow["SALARY_AMOUNT"];
                }

                if (ldecContributionWages == aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.eligible_wages)//PIR 25931 - Either both the amounts should be rounded or non should be rounded
                {
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit = Math.Round((decimal)1, 6, MidpointRounding.AwayFromZero) * lintIndex;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit = Math.Round((decimal)1, 6, MidpointRounding.AwayFromZero) * lintIndex;
                }
                else
                {
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit = 0M;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit = 0M;
                }
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
            }

            //Bonus
            if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus ||
                aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)
            {
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypePayrollAdjustment;

                DataTable ldtbList = Select<cdoEmployerPayrollBonusDetail>(
                    new string[1] { "employer_payroll_detail_id" },
                    new object[1] { aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id }, null,
                    null);

                foreach (DataRow ldrRow in ldtbList.Rows)
                {
                    DateTime ldtPeriod = (DateTime)ldrRow["bonus_period"];
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year = ldtPeriod.Year;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month = ldtPeriod.Month;

                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date =
                        new DateTime(ldtPeriod.Year, ldtPeriod.Month, 1).AddMonths(1).AddDays(-1);

                    if (!Convert.IsDBNull(ldrRow["eligible_wages"])) lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount = Math.Round(Convert.ToDecimal(ldrRow["eligible_wages"]), 2, MidpointRounding.AwayFromZero) * lintIndex;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_rhic_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.er_rhic_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount = 0;
					//PIR 25920 New Plan DC 2025
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_pretax_addl_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_post_tax_addl_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.er_pretax_match_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.adec_amount = 0;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit = 0M;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit = 0M;
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
                }
				//PIR 15616 - The contributions should go into the PCD in the pay period the employer is reporting the bonus
                if (aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.IsNull()) aobjEmployerPayrollDetail.LoadPayrollHeader();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year = aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month = aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date =
                        new DateTime(aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year, 
                                    aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month, 1)
                                    .AddMonths(1)
                                    .AddDays(-1);
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount = 0;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_rhic_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.er_rhic_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported * lintIndex;
                //PIR 25920 New Plan DC 2025
				lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_pretax_addl_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_post_tax_addl_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.er_pretax_match_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.adec_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported * lintIndex;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit = 0M;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit = 0M;
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
            }
            //PIR 16349 - ER interest and RHIC interest, neither has to post to the contribution table, Maik mail dated 07/30/2016, member interest also commented because we are inserting month-wise 
            //EE interest before this method call irrespective of interest waived or not, if we do not comment member interest part, interest would be posted twice(month-wise and lump sum)
            //Posting the Interest Amount
            //if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated != 0)
            //{
            //    lobjRetirementContribution = new busPersonAccountRetirementContribution();
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year =
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ? 
            //        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year : 
            //        Convert.ToInt32(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Year);
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month =
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ? 
            //        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month : 
            //        Convert.ToInt32(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Month);
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = 
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ?
            //        new DateTime(aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year,
            //                        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month, 1)
            //                        .AddMonths(1)
            //                        .AddDays(-1) : icdoEmployerPayrollHeader.payroll_paid_last_date;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_employment_dtl_id = aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.interest_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated * lintIndex;
            //    // UAT  PIR 518
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeInterest;

            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
            //}

            ////Posting the Employer Interest
            //if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated != 0)
            //{
            //    lobjRetirementContribution = new busPersonAccountRetirementContribution();
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year =
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ?
            //        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year :
            //        Convert.ToInt32(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Year);
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month =
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ?
            //        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month :
            //        Convert.ToInt32(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Month);
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date =
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ?
            //        new DateTime(aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year,
            //                        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month, 1)
            //                        .AddMonths(1)
            //                        .AddDays(-1) : icdoEmployerPayrollHeader.payroll_paid_last_date;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_employment_dtl_id = aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.employer_interest = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated * lintIndex;
            //    // UAT  PIR 518
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeEmployerInterest;

            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
            //}

            ////Posting the Employer Interest
            //if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated != 0)
            //{
            //    lobjRetirementContribution = new busPersonAccountRetirementContribution();
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year =
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ?
            //        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year :
            //        Convert.ToInt32(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Year);
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month =
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ?
            //        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month :
            //        Convert.ToInt32(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Month);
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date =
            //        (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ?
            //        new DateTime(aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Year,
            //                        aobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.Month, 1)
            //                        .AddMonths(1)
            //                        .AddDays(-1) : icdoEmployerPayrollHeader.payroll_paid_last_date;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_employment_dtl_id = aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id = aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = DateTime.Now;
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.employer_rhic_interest = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated * lintIndex;
            //    // UAT  PIR 518
            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value = busConstant.TransactionTypeEmployerInterest;

            //    lobjRetirementContribution.icdoPersonAccountRetirementContribution.Insert();
            //}

        }

        private bool IsServiceCreditAlreadyPosted(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            decimal ldecTotalPSC = 0;
            decimal ldecTotalVSC = 0;
            //Check if any PSC, VSC already posted for this month
            DataTable ldtbList = Select<cdoPersonAccountRetirementContribution>(
                new string[3] { "person_account_id", "pay_period_month", "pay_period_year" },
                new object[3]
                        {
                            aobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id,                            
                            aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Month,
                            aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.Year
                        }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                //If the Total PSC , VSC comes with zero, than post it. (REG and -NEG ADJ exists)                   
                foreach (DataRow ldrRow in ldtbList.Rows)
                {
                    if (!Convert.IsDBNull(ldrRow["VESTED_SERVICE_CREDIT"]))
                        ldecTotalVSC += (decimal)ldrRow["VESTED_SERVICE_CREDIT"];

                    if (!Convert.IsDBNull(ldrRow["PENSION_SERVICE_CREDIT"]))
                        ldecTotalPSC += (decimal)ldrRow["PENSION_SERVICE_CREDIT"];
                }
            }

            if ((ldecTotalVSC == 0) && (ldecTotalPSC == 0))
                return false;

            return true;
        }
        /// <summary>
        /// insert into Person Account Def Comp Contribution 
        /// </summary>       
        /// <param name="aobjPersonAccountDefCompContribution"></param>
        /// <param name="aintProviderId"></param>
        /// <param name="adecContributionAmt"></param>
        private void PostDeferredCompContributionDetail(busPersonAccountDeferredCompContribution aobjPersonAccountDefCompContribution, int aintProviderId, decimal adecContributionAmt)
        {
            aobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.provider_org_id = aintProviderId;
            aobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_contribution_amount = adecContributionAmt;
            aobjPersonAccountDefCompContribution.icdoPersonAccountDeferredCompContribution.Insert();
            iblnIsPostDeferredCompContribution = true;
        }

        /// <summary>
        /// Posting Regular / +ve Adjustment / Bonus detail entries GL
        /// Excluding Other 457 in Query
        /// </summary>
        private void PostEmployerReportingGL()
        {
            //PIR 13996 - As per this PIR Query updated,Amounts that post to the GL need to be the Calculated Amounts.
            DataTable ltdbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadItemLevelGLDetailAmountByPlan",
                                                       new object[1] { _icdoEmployerPayrollHeader.employer_payroll_header_id });
            Collection<busEmployerPayrollHeader> _iclbItemLevelGLAmount = new Collection<busEmployerPayrollHeader>();

            Collection<busCodeValue> lclbLineOfDutySurvivorOrgCode = busGlobalFunctions.LoadData1ByCodeID(7022);

            busOrganization lbusOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            lbusOrg.FindOrganization(_icdoEmployerPayrollHeader.org_id);

            bool lblnIsLineOfDutySurvivor = lclbLineOfDutySurvivorOrgCode.Any(lbus => lbus.icdoCodeValue.data1 == lbusOrg.icdoOrganization.org_code);

            //bool lblnResult1 = lclbLineOfDutySurvivorOrgCode.Any(lbus => busGlobalFunctions.GetOrgIdFromOrgCode(lbus.icdoCodeValue.data1) == _icdoEmployerPayrollHeader.org_id );
            
            foreach (DataRow dr in ltdbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader();
                sqlFunction.LoadQueryResult(lobjEmpHeader, dr);
                _iclbItemLevelGLAmount.Add(lobjEmpHeader);
            }
            foreach (busEmployerPayrollHeader lobjEmpHeader in _iclbItemLevelGLAmount)
            {
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    //Generate GL for EE Contrib 
                    if (lobjEmpHeader.idecTotalEEContributionReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEContribution, lobjEmpHeader.idecTotalEEContributionReportedByPlan);
                    }
                    //Generate GL for EE Pre Tax 
                    if (lobjEmpHeader.idecTotalEEPreTaxReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEPreTax, lobjEmpHeader.idecTotalEEPreTaxReportedByPlan);
                    }
                    //Generate GL for EE Emp Pickup 
                    if (lobjEmpHeader.idecTotalEEEmployerPickupReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEEmpPickup, lobjEmpHeader.idecTotalEEEmployerPickupReportedByPlan);
                    }
                    //Generate GL for ER Contrib 
                    if (lobjEmpHeader.idecTotalERContributionReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeERContribution, lobjEmpHeader.idecTotalERContributionReportedByPlan);
                    }
                    //Generate GL for Member Interest
                    if (lobjEmpHeader.idecTotalMemberInterestCalculatedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeMemberInterest, lobjEmpHeader.idecTotalMemberInterestCalculatedByPlan);
                    }
                    //Generate GL for Employer Interest
                    if (lobjEmpHeader.idecTotalEmployerInterestCalculatedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEmployerInterest, lobjEmpHeader.idecTotalEmployerInterestCalculatedByPlan);
                    }
                    //Generate GL for RHIC Employer Interest
                    if (lobjEmpHeader.idecTotalRHICEmployerInterestCalculatedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeRHICEmployerInterest, lobjEmpHeader.idecTotalRHICEmployerInterestCalculatedByPlan);
                    }
                    //Generate GL for EE RHIC
                    if (lobjEmpHeader.idecTotalRHICEEContributionReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeRHICEEContribution, lobjEmpHeader.idecTotalRHICEEContributionReportedByPlan);
                    }
                    //Generate GL for ER RHIC
                    if (lobjEmpHeader.idecTotalRHICERContributionReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeRHICERContribution, lobjEmpHeader.idecTotalRHICERContributionReportedByPlan);
                    }
                    //PIR 25920 New Plan DC 2025
                    //Generate GL for ER match Tax 
                    if (lobjEmpHeader.idecTotalEEPostTaxAddlCalculatedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEPostTaxAddlContribution, lobjEmpHeader.idecTotalEEPostTaxAddlCalculatedByPlan);
                    }
                    //Generate GL for ER match Tax 
                    if (lobjEmpHeader.idecTotalEEPreTaxAddlCalculatedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEPreTaxAddlCContribution, lobjEmpHeader.idecTotalEEPreTaxAddlCalculatedByPlan);
                    }
                    //Generate GL for ER match Tax 
                    if (lobjEmpHeader.idecTotalERPreTaxMatchReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeERPreTaxMatchContribution, lobjEmpHeader.idecTotalERPreTaxMatchReportedByPlan);
                    }
                    //Generate GL for ADEC contribution  //PIR 25920 New Plan DC 2025
                    if (lobjEmpHeader.idecTotalADECReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeADECContribution, lobjEmpHeader.idecTotalADECReportedByPlan);
                    }
                }
                else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    //Generate GL for Def Comp
                    if (lobjEmpHeader.idecTotalContributionAmount1ReportedByPlan +
                       lobjEmpHeader.idecTotalContributionAmount2ReportedByPlan +
                       lobjEmpHeader.idecTotalContributionAmount3ReportedByPlan +
                       lobjEmpHeader.idecTotalContributionAmount4ReportedByPlan +
                       lobjEmpHeader.idecTotalContributionAmount5ReportedByPlan +
                       lobjEmpHeader.idecTotalContributionAmount6ReportedByPlan +
                       lobjEmpHeader.idecTotalContributionAmount7ReportedByPlan > 0)
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeDefeCompAmount,
                                         lobjEmpHeader.idecTotalContributionAmount1ReportedByPlan +
                                         lobjEmpHeader.idecTotalContributionAmount2ReportedByPlan +
                                         lobjEmpHeader.idecTotalContributionAmount3ReportedByPlan +
                                         lobjEmpHeader.idecTotalContributionAmount4ReportedByPlan +
                                         lobjEmpHeader.idecTotalContributionAmount5ReportedByPlan +
                                         lobjEmpHeader.idecTotalContributionAmount6ReportedByPlan +
                                         lobjEmpHeader.idecTotalContributionAmount7ReportedByPlan);
                    }
                    //PIR 25920 New Plan DC 2025 Def Comp
                    if (lobjEmpHeader.idecTotalERPreTaxMatchReportedByPlan > 0)         //PIR 25920 New Plan DC 2025
                    {
                        GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeERPreTaxMatchContribution,
                                         lobjEmpHeader.idecTotalERPreTaxMatchReportedByPlan);         //PIR 25920 New Plan DC 2025
                    }
                }
                else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    if (lobjEmpHeader.idecTotalPremiumAmountByPlan > 0)
                    {
                        if (lobjEmpHeader.iintPlanID == busConstant.PlanIdGroupHealth)
                        {
                            //Generate GL for Premium Amount (Exclude Fee Amount)
                            GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeGroupHealthPremium, lobjEmpHeader.idecTotalPremiumAmountByPlan);

                            //Generate GL for Fee Amount                            
                            if (lobjEmpHeader.idecTotalFeeAmountByPlan > 0)
                                GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeHealthAdminFee, lobjEmpHeader.idecTotalFeeAmountByPlan);

                            //Generate GL for Buydown Amount  - PIR 11239                          
                            if (lobjEmpHeader.idecTotalBuydownAmountByPlan > 0)
                                GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeBuydownAmount, lobjEmpHeader.idecTotalBuydownAmountByPlan);

                            //Generate GL for Medicare Part D Amount - PIR 14529
                            if (lobjEmpHeader.idecTotalMedicarePartDAmtByPlan > 0)
                                GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeMedicarePartDAmount, lobjEmpHeader.idecTotalMedicarePartDAmtByPlan);

                            if (lobjEmpHeader.idecOtherRHICAmount > 0)
                                GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeRHICEmpReport, lobjEmpHeader.idecOtherRHICAmount);

                            if (lobjEmpHeader.idecJSRHICAmount > 0)
                                GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeJSRHICEmpReport, lobjEmpHeader.idecJSRHICAmount);
                            //PIR 7705
                            if (lobjEmpHeader.idecHSAAmount > 0)
                                GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeAllocationGHHSA, lobjEmpHeader.idecHSAAmount);

                            //PIR 24035
                           
                            if (lblnIsLineOfDutySurvivor) 
                            {
                                cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
                                lcdoAcccountReference.plan_id = lobjEmpHeader.iintPlanID;
                                lcdoAcccountReference.source_type_value = busConstant.SourceTypeEmployerReporting;
                                lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
                                lcdoAcccountReference.item_type_value = busConstant.HealthLifeUnderwritingGain; 

                                busGLHelper.GenerateGL(lcdoAcccountReference, 0, _icdoEmployerPayrollHeader.org_id,
                                                                   _icdoEmployerPayrollHeader.employer_payroll_header_id,
                                                                   lobjEmpHeader.idecTotalPremiumAmountByPlan,
                                                                   GetGLEffectiveDate(),
                                                                   busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);
                            }

                        }
                        else if (lobjEmpHeader.iintPlanID == busConstant.PlanIdHMO)
                        {
                            GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeHMOPremium, lobjEmpHeader.idecTotalPremiumAmountByPlan);
                        }
                        else if (lobjEmpHeader.iintPlanID == busConstant.PlanIdGroupLife)
                        {
                            GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeGroupLifePremium, lobjEmpHeader.idecTotalPremiumAmountByPlan);
                        }
                        else if (lobjEmpHeader.iintPlanID == busConstant.PlanIdEAP)
                        {
                            GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEAPPremium, lobjEmpHeader.idecTotalPremiumAmountByPlan);
                        }
                        else if (lobjEmpHeader.iintPlanID == busConstant.PlanIdLTC)
                        {
                            GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeLTCPremium, lobjEmpHeader.idecTotalPremiumAmountByPlan);
                        }
                        else if (lobjEmpHeader.iintPlanID == busConstant.PlanIdDental)
                        {
                            GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeGroupDentalPremium, lobjEmpHeader.idecTotalPremiumAmountByPlan);
                        }
                        else if (lobjEmpHeader.iintPlanID == busConstant.PlanIdVision)
                        {
                            GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeGroupVisionPremium, lobjEmpHeader.idecTotalPremiumAmountByPlan);
                        }
                        else if (lobjEmpHeader.iintPlanID == busConstant.PlanIdMedicarePartD)
                        {
                            GenerateGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeMedicarePremium, lobjEmpHeader.idecTotalPremiumAmountByPlan);
                        }
                    }
                }
            }

        }

        private void GenerateGLByType(int aintPlanID, string astrItemType, decimal adecTotalAmount)
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = aintPlanID;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeEmployerReporting;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
            lcdoAcccountReference.item_type_value = astrItemType;

            busGLHelper.GenerateGL(lcdoAcccountReference, 0, _icdoEmployerPayrollHeader.org_id,
                                               _icdoEmployerPayrollHeader.employer_payroll_header_id,
                                               adecTotalAmount,
                                               GetGLEffectiveDate(),
                                               busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);

            //Generate Fund Transfer GL only for RHIC
            if (astrItemType == busConstant.ItemTypeRHICEmpReport)
            {
                cdoAccountReference lcdoAcccountReferenceTransfer = new cdoAccountReference();
                lcdoAcccountReferenceTransfer.plan_id = aintPlanID;
                lcdoAcccountReferenceTransfer.source_type_value = busConstant.SourceTypeEmployerReporting;
                lcdoAcccountReferenceTransfer.transaction_type_value = busConstant.TransactionTypeTransfer;
                lcdoAcccountReferenceTransfer.status_transition_value = string.Empty;
                lcdoAcccountReferenceTransfer.item_type_value = astrItemType;
                busGLHelper.GenerateGL(lcdoAcccountReferenceTransfer, 0, _icdoEmployerPayrollHeader.org_id,
                                                  _icdoEmployerPayrollHeader.employer_payroll_header_id,
                                                   adecTotalAmount,
                                                   GetGLEffectiveDate(),
                                                   busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);
            }
        }

        /// <summary>
        /// Get the Effective Date for GL
        /// </summary>
        private DateTime GetGLEffectiveDate()
        {
            DateTime ldtEffectiveDate = DateTime.Now;
            if ((_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) ||
                (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr))
            {
                ldtEffectiveDate = new DateTime(_icdoEmployerPayrollHeader.payroll_paid_date.Year,
                                                _icdoEmployerPayrollHeader.payroll_paid_date.Month, 1).AddMonths(1).AddDays(-1);
            }
            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                ldtEffectiveDate = _icdoEmployerPayrollHeader.pay_period_end_date;
            }
            return ldtEffectiveDate;
        }

        private void GenerateNegativeAdjustmentGLByType(int aintPlanID, string astrItemType, decimal adecTotalAmount, bool adecIsForMember = false)
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = aintPlanID;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeEmployerReporting;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeAllocation;
            lcdoAcccountReference.from_item_type_value = astrItemType;

            busGLHelper.GenerateGL(lcdoAcccountReference, adecIsForMember ? iintPersonID : 0, adecIsForMember ? 0 :_icdoEmployerPayrollHeader.org_id,
                                               _icdoEmployerPayrollHeader.employer_payroll_header_id,
                                               adecTotalAmount,
                                               GetGLEffectiveDate(),
                                               busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);

            //Generate Fund Transfer GL only for RHIC
            if (astrItemType == busConstant.ItemTypeRHICEmpReport)
            {
                cdoAccountReference lcdoAcccountReferenceTransfer = new cdoAccountReference();
                lcdoAcccountReferenceTransfer.plan_id = aintPlanID;
                lcdoAcccountReferenceTransfer.source_type_value = busConstant.SourceTypeEmployerReporting;
                lcdoAcccountReferenceTransfer.transaction_type_value = busConstant.TransactionTypeTransfer;
                lcdoAcccountReferenceTransfer.status_transition_value = string.Empty;
                lcdoAcccountReferenceTransfer.item_type_value = busConstant.ItemTypeNegRHICEmpReport;
                busGLHelper.GenerateGL(lcdoAcccountReferenceTransfer, adecIsForMember ? iintPersonID : 0, adecIsForMember ? 0 : _icdoEmployerPayrollHeader.org_id,
                                                  _icdoEmployerPayrollHeader.employer_payroll_header_id,
                                                   adecTotalAmount,
                                                   GetGLEffectiveDate(),
                                                   busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);
            }
        }

        /// <summary>
        /// Create a Deposit and Remittance for the Negative Adjustments also Generate GL Accordingly
        /// *************Excludes Other457 Plan ************************
        /// </summary>
        /// Note :- If Any change in below method or Query please also change in busRemittance - btnCancelRefund_Click method code and 'LoadMembersNegativeAdjustmentsAmountByPlan' query
        public void ProcessNegativeAdjustments()
        {
            //Excluding the Other457
            //PIR 13996 - As per this PIR Query updated,Amounts that post to the GL need to be the Calculated Amounts.
            DataTable ltdbList = new DataTable();
            int lintDepositID = 0;
            if (icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                ltdbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadNegativeAdjustmentAmountByPlan",
                                                                                                 new object[1] { _icdoEmployerPayrollHeader.employer_payroll_header_id });
            }
            //PIR 26652 - All scenarios handle in below query - Member And Org Contributions - dont change below logic
            else
            {
                ltdbList = busNeoSpinBase.Select("entEmployerPayrollHeader.LoadAllNegativeAdjAmountByPlan",
                                                                                 new object[1] { _icdoEmployerPayrollHeader.employer_payroll_header_id });
            }
            _iclbNegativeAdjustments = new Collection<busEmployerPayrollHeader>();
            foreach (DataRow dr in ltdbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader();
                sqlFunction.LoadQueryResult(lobjEmpHeader, dr);
                _iclbNegativeAdjustments.Add(lobjEmpHeader);
            }
            foreach (busEmployerPayrollHeader lobjEmpHeader in _iclbNegativeAdjustments)
            {
                bool IsForMemberRefund = false;
                //Employee Contribution
                if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
                    lobjEmpHeader.iblnIsForOrg == "FALSE")
                {
                    iintPersonID = lobjEmpHeader.iintPersonID;
                    IsForMemberRefund = iintPersonID > 0 ? true : false;
                    if (IsForMemberRefund)
                    {
                        lobjEmpHeader.idecMemberTotalTaxableRetrContributionByPlan = lobjEmpHeader.idecTotalRHICEEContributionCalculatedByPlan +
                                                                                lobjEmpHeader.idecTotalEEPreTaxCalculatedByPlan +
                                                                                lobjEmpHeader.idecTotalEEEmployerPickupCalculatedByPlan +
                                                                                lobjEmpHeader.idecTotalMemberInterestCalculatedByPlan +
                                                                                lobjEmpHeader.idecTotalEEPostTaxAddlCalculatedByPlan +
                                                                                lobjEmpHeader.idecTotalEEPreTaxAddlCalculatedByPlan;

                        lobjEmpHeader.idecMemberTotalNonTaxableRetrContributionByPlan = lobjEmpHeader.idecTotalEEContributionCalculatedByPlan;
                    }
                }
                else
                {
                    lobjEmpHeader.idecTotalRetrContributionByPlan = lobjEmpHeader.idecTotalEEContributionCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalERContributionCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalEEPreTaxCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalEEEmployerPickupCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalMemberInterestCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalEmployerInterestCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan;         //PIR 25920 New Plan DC 2025

                    lobjEmpHeader.idecTotalRHICContributionbyPlan = lobjEmpHeader.idecTotalRHICERContributionCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalRHICEEContributionCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalRHICEmployerInterestCalculatedByPlan; //PIR 18755 

                    lobjEmpHeader.idecTotalDeffContributionByPlan =
                                                           lobjEmpHeader.idecTotalContributionAmount1ReportedByPlan +
                                                           lobjEmpHeader.idecTotalContributionAmount2ReportedByPlan +
                                                           lobjEmpHeader.idecTotalContributionAmount3ReportedByPlan +
                                                           lobjEmpHeader.idecTotalContributionAmount4ReportedByPlan +
                                                           lobjEmpHeader.idecTotalContributionAmount5ReportedByPlan +
                                                           lobjEmpHeader.idecTotalContributionAmount6ReportedByPlan +
                                                           lobjEmpHeader.idecTotalContributionAmount7ReportedByPlan;

                    //18755
                    lobjEmpHeader.idecTotalReportedRetrContributionByPlan = lobjEmpHeader.idecTotalEEContributionReportedByPlan +
                                                                    lobjEmpHeader.idecTotalERContributionReportedByPlan +
                                                                    lobjEmpHeader.idecTotalEEPreTaxReportedByPlan +
                                                                    lobjEmpHeader.idecTotalEEEmployerPickupReportedByPlan +
                                                                    lobjEmpHeader.idecTotalMemberInterestCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalEmployerInterestCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalEEPostTaxAddlCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalEEPreTaxAddlCalculatedByPlan +
                                                                    lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan;         //PIR 25920 New Plan DC 2025;

                    lobjEmpHeader.idecTotalReportedRHICContributionByPlan = lobjEmpHeader.idecTotalRHICERContributionReportedByPlan +
                                                                    lobjEmpHeader.idecTotalRHICEEContributionReportedByPlan +
                                                                    lobjEmpHeader.idecTotalRHICEmployerInterestCalculatedByPlan;
                }

                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    _idecTotalNegAdjContribution += lobjEmpHeader.idecTotalReportedRetrContributionByPlan + lobjEmpHeader.idecTotalReportedRHICContributionByPlan +
                                                        lobjEmpHeader.idecTotalADECCalculatedByPlan + //lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan +
                                                        lobjEmpHeader.idecMemberTotalTaxableRetrContributionByPlan + lobjEmpHeader.idecMemberTotalNonTaxableRetrContributionByPlan; 

                    //Generate GL for EE Contrib 
                    if (lobjEmpHeader.idecTotalEEContributionCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEContribution, lobjEmpHeader.idecTotalEEContributionCalculatedByPlan, IsForMemberRefund);
                    }
                    //Generate GL for EE Pre Tax 
                    if (lobjEmpHeader.idecTotalEEPreTaxCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEPreTax, lobjEmpHeader.idecTotalEEPreTaxCalculatedByPlan, IsForMemberRefund);
                    }
                    //Generate GL for EE Emp Pickup 
                    if (lobjEmpHeader.idecTotalEEEmployerPickupCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEEmpPickup, lobjEmpHeader.idecTotalEEEmployerPickupCalculatedByPlan, IsForMemberRefund);
                    }
                    //Generate GL for ER Contrib 
                    if (lobjEmpHeader.idecTotalERContributionCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeERContribution, lobjEmpHeader.idecTotalERContributionCalculatedByPlan);
                    }
                    //Generate GL for Member Interest
                    if (lobjEmpHeader.idecTotalMemberInterestCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeMemberInterest, lobjEmpHeader.idecTotalMemberInterestCalculatedByPlan, IsForMemberRefund);
                    }
                    //Generate GL for Employer Interest
                    if (lobjEmpHeader.idecTotalEmployerInterestCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEmployerInterest, lobjEmpHeader.idecTotalEmployerInterestCalculatedByPlan);
                    }
                    //Generate GL for RHIC Employer Interest
                    if (lobjEmpHeader.idecTotalRHICEmployerInterestCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeRHICEmployerInterest, lobjEmpHeader.idecTotalRHICEmployerInterestCalculatedByPlan);
                    }
                    //Generate GL for EE RHIC
                    if (lobjEmpHeader.idecTotalRHICEEContributionCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeRHICEEContribution, lobjEmpHeader.idecTotalRHICEEContributionCalculatedByPlan, IsForMemberRefund);
                    }
                    //Generate GL for ER RHIC
                    if (lobjEmpHeader.idecTotalRHICERContributionCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeRHICERContribution, lobjEmpHeader.idecTotalRHICERContributionCalculatedByPlan);
                    }
                    //PIR 25920 New Plan DC 2025
                    //Generate GL for ER match Tax 
                    if (lobjEmpHeader.idecTotalEEPostTaxAddlCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEPostTaxAddlContribution, lobjEmpHeader.idecTotalEEPostTaxAddlCalculatedByPlan, IsForMemberRefund);
                    }
                    //Generate GL for ER match Tax 
                    if (lobjEmpHeader.idecTotalEEPreTaxAddlCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeEEPreTaxAddlCContribution, lobjEmpHeader.idecTotalEEPreTaxAddlCalculatedByPlan, IsForMemberRefund);
                    }
                    //Generate GL for ER match Tax 
                    if (lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeERPreTaxMatchContribution, lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan);
                    }
                    //Generate GL for ADEC contribution  //PIR 25920 New Plan DC 2025
                    if (lobjEmpHeader.idecTotalADECCalculatedByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeADECContribution, lobjEmpHeader.idecTotalADECCalculatedByPlan);
                    }
                }
                else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    _idecTotalNegAdjContribution += lobjEmpHeader.idecTotalDeffContributionByPlan;
                    //Generate GL for Def Comp
                    if (lobjEmpHeader.idecTotalDeffContributionByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeDefeCompAmount, lobjEmpHeader.idecTotalDeffContributionByPlan);
                    }
                    //PIR 25920 New Plan DC 2025 Def Comp
                    if (lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan > 0)         //PIR 25920 New Plan DC 2025
                    {
                        _idecTotalNegAdjContribution += lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan;
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeERPreTaxMatchContribution,
                                         lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan);         //PIR 25920 New Plan DC 2025
                    }
                }
                else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    _idecTotalNegAdjContribution += lobjEmpHeader.idecTotalPremiumAmountByPlan + lobjEmpHeader.idecTotalFeeAmountByPlan +
                                                    lobjEmpHeader.idecJSRHICAmount + lobjEmpHeader.idecOtherRHICAmount;
                    //7705 - To Do - Should Vendor amount or hsa amount be included here?
                    //Generate GL for Insurance Premium
                    if (lobjEmpHeader.idecTotalPremiumAmountByPlan > 0)
                    {
                        string lstrItemType = busEmployerReportHelper.GetFromItemTypeByPlan(lobjEmpHeader.iintPlanID);
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, lstrItemType, lobjEmpHeader.idecTotalPremiumAmountByPlan);
                    }

                    //Generate GL for Fee Amount
                    if (lobjEmpHeader.idecTotalFeeAmountByPlan > 0)
                    {
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeHealthAdminFee, lobjEmpHeader.idecTotalFeeAmountByPlan);
                    }

                    if (lobjEmpHeader.idecJSRHICAmount > 0)
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeJSRHICEmpReport, lobjEmpHeader.idecJSRHICAmount);

                    if (lobjEmpHeader.idecOtherRHICAmount > 0)
                        GenerateNegativeAdjustmentGLByType(lobjEmpHeader.iintPlanID, busConstant.ItemTypeRHICEmpReport, lobjEmpHeader.idecOtherRHICAmount);

                    //pir 7705 - generate GL for HSA Amount -- TO DO
                }
            }
            if (_iclbNegativeAdjustments.Count > 0)
            {
                if (_idecTotalNegAdjContribution > 0)
                {
                    cdoDeposit lobjCdoDeposit = new cdoDeposit();
                    CreateDepositForNegativeAdjustments(lobjCdoDeposit, _idecTotalNegAdjContribution);
                    foreach (busEmployerPayrollHeader lobjEmpHeader in _iclbNegativeAdjustments)
                    {
                        if (lobjEmpHeader.idecTotalReportedRetrContributionByPlan > 0)
                        {
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecTotalReportedRetrContributionByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeContribution);
                        }
                        if (lobjEmpHeader.idecTotalReportedRHICContributionByPlan > 0)
                        {
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecTotalReportedRHICContributionByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeRHICContribution);
                        }
                        //PIR 25920 DC 2025 changes
                        if (lobjEmpHeader.idecTotalADECCalculatedByPlan > 0)
                        {
                            //cdoDeposit lobjCdoDepositForADEC = new cdoDeposit();
                            //CreateDepositForNegativeAdjustments(lobjCdoDepositForADEC, lobjEmpHeader.idecTotalADECReportedByPlan);
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecTotalADECCalculatedByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeADECContribution);
                        }
                        if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp && (lobjEmpHeader.idecTotalDeffContributionByPlan + lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan) > 0)
                        {
                            CreateRemittanceforNegativeAdjustments((lobjEmpHeader.idecTotalDeffContributionByPlan + lobjEmpHeader.idecTotalERPreTaxMatchCalculatedByPlan), lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeDefeCompDeposit);
                        }
                        if (lobjEmpHeader.idecTotalPremiumAmountByPlan > 0)
                        {
                            string lstrItemType = busEmployerReportHelper.GetToItemTypeByPlanID(lobjEmpHeader.iintPlanID);
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecTotalPremiumAmountByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, lstrItemType);
                        }

                        //PROD PIR 5416, 5284 : We should not create Remittance for Fee Amount since Fee is already included in the Premium Amount
                        /*if (lobjEmpHeader.idecTotalFeeAmountByPlan > 0)
                        {
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecTotalFeeAmountByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeGroupHealthDeposit);
                        }*/

                        if (lobjEmpHeader.idecJSRHICAmount > 0)
                        {
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecJSRHICAmount, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeGroupHealthDeposit);
                        }
                        if (lobjEmpHeader.idecOtherRHICAmount > 0)
                        {
                            CreateRemittanceforNegativeAdjustments(lobjEmpHeader.idecOtherRHICAmount, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeGroupHealthDeposit);
                        }
                        //Excess Contribution Taxable
                        if (lobjEmpHeader.idecMemberTotalTaxableRetrContributionByPlan > 0)
                        {
                            CreateRemittanceForMemberNegativeAdj(lobjEmpHeader.idecMemberTotalTaxableRetrContributionByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeMemberExcessContributionTaxable, lobjEmpHeader.iintPersonID);
                        }
                        //Excess Contribution Non Taxable
                        if (lobjEmpHeader.idecMemberTotalNonTaxableRetrContributionByPlan > 0)
                        {
                            CreateRemittanceForMemberNegativeAdj(lobjEmpHeader.idecMemberTotalNonTaxableRetrContributionByPlan, lobjEmpHeader.iintPlanID, lobjCdoDeposit.deposit_id, busConstant.ItemTypeMemberExcessContributionNonTaxable, lobjEmpHeader.iintPersonID);
                        }
                    }
                }
            }
        }

        public void CreateDepositForNegativeAdjustments(cdoDeposit lobjCdoDeposit, decimal adecDepositeAmount )
        {
            lobjCdoDeposit.org_id = _icdoEmployerPayrollHeader.org_id;
            lobjCdoDeposit.reference_no = "Payroll Header : " + _icdoEmployerPayrollHeader.employer_payroll_header_id;
            lobjCdoDeposit.deposit_amount = adecDepositeAmount;
            lobjCdoDeposit.status_value = busConstant.DepositDetailStatusApplied;
            lobjCdoDeposit.deposit_source_value = busConstant.DepositSourceNegativeAdjustment;
            lobjCdoDeposit.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
            lobjCdoDeposit.deposit_date = DateTime.Today;
            lobjCdoDeposit.Insert();
        }
        public void CreateRemittanceforNegativeAdjustments(decimal adecRemittanceAmount, int aintPlanId, int aintDepositID, string astrRemittanceType)
        {
            cdoRemittance lobjcdoRemittance = new cdoRemittance();
            lobjcdoRemittance.org_id = _icdoEmployerPayrollHeader.org_id;
            lobjcdoRemittance.plan_id = aintPlanId;
            lobjcdoRemittance.deposit_id = aintDepositID;
            lobjcdoRemittance.remittance_type_value = astrRemittanceType;
            lobjcdoRemittance.remittance_amount = adecRemittanceAmount;
            lobjcdoRemittance.applied_date = busGlobalFunctions.GetSysManagementBatchDate().Date;
            lobjcdoRemittance.Insert();
        }
        public void CreateRemittanceForMemberNegativeAdj(decimal adecRemittanceAmount, int aintPlanId, int aintDepositID, string astrRemittanceType, int aintPersonID)
        {
            busRemittance lobjRemittance = new busRemittance() { icdoRemittance = new cdoRemittance() };
            lobjRemittance.icdoRemittance.deposit_id = aintDepositID;
            lobjRemittance.icdoRemittance.person_id = aintPersonID;
            lobjRemittance.icdoRemittance.plan_id = aintPlanId;
            lobjRemittance.icdoRemittance.remittance_type_value = astrRemittanceType;
            lobjRemittance.icdoRemittance.remittance_amount = adecRemittanceAmount;
            lobjRemittance.icdoRemittance.applied_date = busGlobalFunctions.GetSysManagementBatchDate().Date;
            
            //Refund Information
            lobjRemittance.icdoRemittance.refund_to_id = 5014;
            lobjRemittance.icdoRemittance.refund_status_value = busConstant.DepositRefundStatusApproved;
            lobjRemittance.icdoRemittance.refund_appr_by = busConstant.PERSLinkBatchUser;
            lobjRemittance.icdoRemittance.refund_to_value = busConstant.RemittanceRefundSameMember;
            lobjRemittance.icdoRemittance.refund_to_person_id = aintPersonID;
            lobjRemittance.icdoRemittance.computed_refund_amount = adecRemittanceAmount;
            lobjRemittance.icdoRemittance.refund_notes = "Employer Header ID - " + icdoEmployerPayrollHeader.employer_payroll_header_id;
            lobjRemittance.icdoRemittance.Insert();
        }

        private void PostDetailToProvider()
        {
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();

            var lenuValidRecords = iclbEmployerPayrollDetail.Where(i => i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusValid);
            foreach (busEmployerPayrollDetail lobjEmployerDetail in lenuValidRecords)
            {
                if ((_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                                                    && (lobjEmployerDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDC ||
                                                        lobjEmployerDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDC2020 || //PIR 23782
                                                        lobjEmployerDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDC2025))   //PIR 25920 New Plan DC 2025
                {
                    //insert into report data
                    InsertIntoProviderReportDataDC(lobjEmployerDetail);
                }
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    //Org to bill
                    if (lobjEmployerDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
                        InsertIntoProviderReportDataMedicarePartD(lobjEmployerDetail);
                    else
                        InsertIntoProviderReportDataInsurance(lobjEmployerDetail);
                }
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    if (lobjEmployerDetail.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457)
                        ProcessAndInsertIntoProviderReportDataDefComp(lobjEmployerDetail);
                }
                // Service purchase is handled in posting method.
            }
        }

        //insert into Provider Data Insurance 
        public void InsertIntoProviderReportDataInsurance(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            iintCredit = IsNegativeAdjustmentPayrollDetail(aobjEmployerPayrollDetail);
            busProviderReportDataInsurance lobjProviderDataInsurance = new busProviderReportDataInsurance();
            lobjProviderDataInsurance.icdoProviderReportDataInsurance = new cdoProviderReportDataInsurance();
            //changed as per satya's comment on 04 May 2010
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.effective_date = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.first_name = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.last_name = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.person_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.plan_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id;

            //7705 - As per mail dated 1/20/2012 from Maik. This must contain only the provider premium amount
            //The employer payroll detail premium amount includes the hsa amount.
            //So we remove the HSA amount as it is not needed for the vendors.
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.premium_amount =
                (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount - aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.hsa_amount -
                 aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.group_health_fee_amount +
                 aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.buydown_amount +
                 aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_benefit_amount) * iintCredit -
                 aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.medicare_part_d_amt; //PIR 14271

            //get Provider org id from PersonAccount History
            if (aobjEmployerPayrollDetail.ibusPerson == null)
                aobjEmployerPayrollDetail.LoadPerson();
            if (aobjEmployerPayrollDetail.ibusPersonAccount == null)
                aobjEmployerPayrollDetail.LoadPersonAccount();

            lobjProviderDataInsurance.icdoProviderReportDataInsurance.provider_org_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_id;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.record_type_value = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.ssn = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.subsystem_ref_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.fee_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.group_health_fee_amount * iintCredit;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.buydown_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.buydown_amount * iintCredit; // PIR 11239
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.medicare_part_d_amt = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.medicare_part_d_amt * iintCredit; //PIR 14271

            //uat pir 1429 : post ghdv_history_id and group number
            //prod pir 6076 & 6077 - Removal of person account ghdv history id
            //lobjProviderDataInsurance.icdoProviderReportDataInsurance.person_account_ghdv_history_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_account_ghdv_history_id;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.group_number = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.group_number;
            //proe pir 6076
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.coverage_code = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.coverage_code;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.rate_structure_code = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rate_structure_code;
            lobjProviderDataInsurance.icdoProviderReportDataInsurance.Insert();
            //PIR 7705
            if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.vendor_amount > 0)
            {
                InsertVendorAmountIntoProviderReportDataInsurance(lobjProviderDataInsurance,aobjEmployerPayrollDetail);
            }
        }

        //Org to bill
        public void InsertIntoProviderReportDataMedicarePartD(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            busProviderReportDataMedicarePartD lobjProviderDataMedicare = new busProviderReportDataMedicarePartD();
            lobjProviderDataMedicare.icdoProviderReportDataMedicare = new cdoProviderReportDataMedicarePartD();
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.subsystem_ref_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.person_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id;
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.ssn = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn;
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.first_name = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.first_name;
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.last_name = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.last_name;

            lobjProviderDataMedicare.icdoProviderReportDataMedicare.provider_org_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_id;
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.plan_id = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id;
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.effective_date = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date;
            lobjProviderDataMedicare.icdoProviderReportDataMedicare.record_type_value = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value;

            //PIR 18047 - Negative amounts were shown as positive on Medicare Vendor Payment report. 
            if (lobjProviderDataMedicare.icdoProviderReportDataMedicare.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
            {
                //PIR 18047 - Premium amount from detail table should be displayed on Provider Report.
                lobjProviderDataMedicare.icdoProviderReportDataMedicare.premium_amount = Math.Abs(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount +
                                                                                                  aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lis_amount -
                                                                                                  aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lep_amount) * -1;
                lobjProviderDataMedicare.icdoProviderReportDataMedicare.lis_amount = Math.Abs(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lis_amount) * -1;
                lobjProviderDataMedicare.icdoProviderReportDataMedicare.lep_amount = Math.Abs(aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lep_amount) * -1;
            }
            else
            {
                //PIR 18047
                lobjProviderDataMedicare.icdoProviderReportDataMedicare.premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount +
                                                                                         aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lis_amount -
                                                                                         aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lep_amount;
                lobjProviderDataMedicare.icdoProviderReportDataMedicare.lis_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lis_amount;
                lobjProviderDataMedicare.icdoProviderReportDataMedicare.lep_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lep_amount;
            }

            lobjProviderDataMedicare.icdoProviderReportDataMedicare.Insert();
        }

        /// <summary>
        /// Checks if payroll detail record is negative adjustment
        /// </summary>
        /// <param name="aobjEmployerPayrollDetail"></param>
        /// <returns>
        /// -1 if negative adjustment
        /// 1 if otherwise
        /// </returns>
        private int IsNegativeAdjustmentPayrollDetail(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            if (aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment ||
                aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)
                return -1; 
            else
                return 1;
        }

        /// <summary>
        /// pir 7705 - Insert Vendor Amount as a separate record. This is to be sent in HSA Contribution file to HSA Provider
        /// </summary>
        /// <param name="lobjProviderDataInsurance">Copy of the PRDI record inserted previously</param>
        /// <param name="aobjEmployerPayrollDetail">Employer payroll detail containing the vendor amount</param>
        private void InsertVendorAmountIntoProviderReportDataInsurance(busProviderReportDataInsurance lobjProviderDataInsurance, busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            busProviderReportDataInsurance lobjHSA = lobjProviderDataInsurance;
            lobjHSA.icdoProviderReportDataInsurance = lobjProviderDataInsurance.icdoProviderReportDataInsurance;
            //Get HSA Provider
            lobjHSA.icdoProviderReportDataInsurance.provider_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.HSAProvider, iobjPassInfo));
            //Override effective date if hsa_effective_date is greater than effective date 
            DateTime ldtHSAEffectiveDate = GetHSAEffectiveDate(aobjEmployerPayrollDetail);
            if (lobjHSA.icdoProviderReportDataInsurance.effective_date.Date < ldtHSAEffectiveDate.Date)
            {    
                //lobjHSA.icdoProviderReportDataInsurance.effective_date = ldtHSAEffectiveDate;
                //As per mail from Maik dated 2/28/2012 ,
                //If current month is less than HSA Effective Date --> post contributions to HSA as HSA Effective Date � 1 month
                //If current month is equal to or greater than HSA Effective Date --> post contributions for current month i.e retain effective date of payroll
                lobjHSA.icdoProviderReportDataInsurance.effective_date = ldtHSAEffectiveDate.AddMonths(-1).Date;
                //Store HSA effective date -- Indicative of retro contributions
                lobjHSA.icdoProviderReportDataInsurance.hsa_effective_date = ldtHSAEffectiveDate.Date;
            }
            lobjHSA.icdoProviderReportDataInsurance.hsa_flag = "Y";
            lobjHSA.icdoProviderReportDataInsurance.fee_amount = 0;
            lobjHSA.icdoProviderReportDataInsurance.premium_amount = aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.vendor_amount * iintCredit;
            lobjHSA.icdoProviderReportDataInsurance.Insert();
        }


        /// <summary>
        /// PIR 7705: Method to get HSA Effective Date
        /// </summary>
        /// <param name="aobjEmployerPayrollDetail">Employer payroll detail containing the hsa amount</param>
        /// <returns>
        /// hsa effective date ,if hsa effective date is not null
        /// pay_period_date , if otherwise
        /// </returns>
        private DateTime GetHSAEffectiveDate(busEmployerPayrollDetail aobjEmployerPayrollDetail)
        {
            if (aobjEmployerPayrollDetail.ibusPersonAccount == null)
                aobjEmployerPayrollDetail.LoadPersonAccount();
            if(aobjEmployerPayrollDetail.ibusPersonAccount.IsNotNull())
                aobjEmployerPayrollDetail.ibusPersonAccount.LoadPersonAccountGHDV();
            if (aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                if (aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date != DateTime.MinValue)
                    return aobjEmployerPayrollDetail.ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date;
            return aobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date;
        }

        //Insert into ReportDataDC
        public void InsertIntoProviderReportDataDC(busEmployerPayrollDetail aobjEmployerDetail)
        {
            //int lintCredit = 1;
            busProviderReportDataDC lobjProviderReportData = new busProviderReportDataDC();
            lobjProviderReportData.icdoProviderReportDataDc = new cdoProviderReportDataDc();
            lobjProviderReportData.icdoProviderReportDataDc.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            lobjProviderReportData.icdoProviderReportDataDc.subsystem_ref_id = aobjEmployerDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            lobjProviderReportData.icdoProviderReportDataDc.ssn = aobjEmployerDetail.icdoEmployerPayrollDetail.ssn;
            lobjProviderReportData.icdoProviderReportDataDc.person_id = aobjEmployerDetail.icdoEmployerPayrollDetail.person_id;
            lobjProviderReportData.icdoProviderReportDataDc.last_name = aobjEmployerDetail.icdoEmployerPayrollDetail.last_name;
            lobjProviderReportData.icdoProviderReportDataDc.first_name = aobjEmployerDetail.icdoEmployerPayrollDetail.first_name;

            if (aobjEmployerDetail.ibusPersonAccount == null)
                aobjEmployerDetail.LoadPersonAccount();

            //Loading the Provider Org ID from the History. Because Suspended plan may not have provider org id at the person account level.
            if (aobjEmployerDetail.ibusPersonAccount.ibusPersonAccountRetirement == null)
                aobjEmployerDetail.ibusPersonAccount.LoadPersonAccountRetirement();
            if (aobjEmployerDetail.ibusPersonAccount.ibusPersonAccountRetirement.icolPersonAccountRetirementHistory == null)
                aobjEmployerDetail.ibusPersonAccount.ibusPersonAccountRetirement.LoadPersonAccountRetirementHistory(false);

            int lintProviderOrgID = 0;
            busPersonAccountRetirementHistory lbusHistory = aobjEmployerDetail.ibusPersonAccount.ibusPersonAccountRetirement.icolPersonAccountRetirementHistory
                                                            .Where(i => busGlobalFunctions.CheckDateOverlapping(aobjEmployerDetail.icdoEmployerPayrollDetail.pay_period_date,
                                                                      i.icdoPersonAccountRetirementHistory.start_date, i.icdoPersonAccountRetirementHistory.end_date)).FirstOrDefault();
            if (lbusHistory != null)
                lintProviderOrgID = lbusHistory.icdoPersonAccountRetirementHistory.provider_org_id;
            //prod pir 5834 : need to check for history as of pay period end date
            //--start--//
            if (lintProviderOrgID == 0) // PIR 13018 -- Person has suspended account as per pay period date, but enrolled as per pay period end date.
            {
                lbusHistory = aobjEmployerDetail.ibusPersonAccount.ibusPersonAccountRetirement.icolPersonAccountRetirementHistory
                                    .Where(i => busGlobalFunctions.CheckDateOverlapping(aobjEmployerDetail.icdoEmployerPayrollDetail.pay_period_date.GetLastDayofMonth(),
                                                i.icdoPersonAccountRetirementHistory.start_date, i.icdoPersonAccountRetirementHistory.end_date)).FirstOrDefault();
                if (lbusHistory != null)
                    lintProviderOrgID = lbusHistory.icdoPersonAccountRetirementHistory.provider_org_id;
            }
            //--end--//
            lobjProviderReportData.icdoProviderReportDataDc.provider_org_id = lintProviderOrgID;
            lobjProviderReportData.icdoProviderReportDataDc.plan_id = aobjEmployerDetail.icdoEmployerPayrollDetail.plan_id;
            iintCredit = IsNegativeAdjustmentPayrollDetail(aobjEmployerDetail);
            //if (aobjEmployerDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
            //{ lintCredit = -1; }
            lobjProviderReportData.icdoProviderReportDataDc.effective_date = aobjEmployerDetail.icdoEmployerPayrollDetail.pay_period_date;
            //lobjProviderReportData.icdoProviderReportDataDc.ee_contribution = aobjEmployerDetail.icdoEmployerPayrollDetail.ee_contribution_reported * iintCredit;
            lobjProviderReportData.icdoProviderReportDataDc.ee_contribution = (aobjEmployerDetail.icdoEmployerPayrollDetail.ee_contribution_calculated + 
                                                            aobjEmployerDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_calculated) * iintCredit;
            lobjProviderReportData.icdoProviderReportDataDc.ee_employer_pickup = aobjEmployerDetail.icdoEmployerPayrollDetail.ee_employer_pickup_calculated * iintCredit;
            //lobjProviderReportData.icdoProviderReportDataDc.ee_pre_tax = aobjEmployerDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported * iintCredit;
            lobjProviderReportData.icdoProviderReportDataDc.ee_pre_tax = (aobjEmployerDetail.icdoEmployerPayrollDetail.ee_pre_tax_calculated + 
                                                    aobjEmployerDetail.icdoEmployerPayrollDetail.ee_pretax_addl_calculated) * iintCredit;
            lobjProviderReportData.icdoProviderReportDataDc.er_contribution = (aobjEmployerDetail.icdoEmployerPayrollDetail.er_contribution_calculated + 
                                                    aobjEmployerDetail.icdoEmployerPayrollDetail.er_pretax_match_calculated) * iintCredit;

            if (aobjEmployerDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDC || 
                aobjEmployerDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDC2020 || 
                aobjEmployerDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDC2025)
            {
                lobjProviderReportData.icdoProviderReportDataDc.employer_interest = 0;
                lobjProviderReportData.icdoProviderReportDataDc.employer_rhic_interest = 0;
                lobjProviderReportData.icdoProviderReportDataDc.member_interest = 0;
            }
            else
            {
                lobjProviderReportData.icdoProviderReportDataDc.employer_interest = aobjEmployerDetail.icdoEmployerPayrollDetail.employer_interest_calculated * iintCredit;
                lobjProviderReportData.icdoProviderReportDataDc.employer_rhic_interest = aobjEmployerDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated * iintCredit;
                lobjProviderReportData.icdoProviderReportDataDc.member_interest = aobjEmployerDetail.icdoEmployerPayrollDetail.member_interest_calculated * iintCredit;
            }
            //PIR 25920 New Plan DC 2025
            //lobjProviderReportData.icdoProviderReportDataDc.ee_contribution += aobjEmployerDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_calculated * iintCredit;
            ////lobjProviderReportData.icdoProviderReportDataDc.ee_pre_tax += aobjEmployerDetail.icdoEmployerPayrollDetail.ee_pretax_addl_calculated * iintCredit;
            //lobjProviderReportData.icdoProviderReportDataDc.er_contribution += aobjEmployerDetail.icdoEmployerPayrollDetail.er_pretax_match_calculated * iintCredit;
            lobjProviderReportData.icdoProviderReportDataDc.Insert();

        }

        //Insert into ReportDataDefComp
        public void ProcessAndInsertIntoProviderReportDataDefComp(busEmployerPayrollDetail aobjEmployerDetail)
        {
            //int lintCredit = 1;
            busProviderReportDataDeffComp lobjProviderReportData = new busProviderReportDataDeffComp();
            lobjProviderReportData.icdoProviderReportDataDeffComp = new cdoProviderReportDataDeffComp();
            lobjProviderReportData.icdoProviderReportDataDeffComp.subsystem_value = busConstant.SubSystemValueEmployerReporting;
            lobjProviderReportData.icdoProviderReportDataDeffComp.subsystem_ref_id = aobjEmployerDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
            lobjProviderReportData.icdoProviderReportDataDeffComp.ssn = aobjEmployerDetail.icdoEmployerPayrollDetail.ssn;
            lobjProviderReportData.icdoProviderReportDataDeffComp.person_id = aobjEmployerDetail.icdoEmployerPayrollDetail.person_id;
            lobjProviderReportData.icdoProviderReportDataDeffComp.last_name = aobjEmployerDetail.icdoEmployerPayrollDetail.last_name;
            lobjProviderReportData.icdoProviderReportDataDeffComp.first_name = aobjEmployerDetail.icdoEmployerPayrollDetail.first_name;
            lobjProviderReportData.icdoProviderReportDataDeffComp.plan_id = aobjEmployerDetail.icdoEmployerPayrollDetail.plan_id;
            lobjProviderReportData.icdoProviderReportDataDeffComp.effective_start_date = aobjEmployerDetail.icdoEmployerPayrollDetail.pay_period_start_date;
            lobjProviderReportData.icdoProviderReportDataDeffComp.effective_end_date = aobjEmployerDetail.icdoEmployerPayrollDetail.pay_period_end_date;
            //if (aobjEmployerDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
            //{
            //    lintCredit = -1;
            //}
            iintCredit = IsNegativeAdjustmentPayrollDetail(aobjEmployerDetail);
            //PIR 25920 New Plan DC 2025
            //lobjProviderReportData.icdoProviderReportDataDeffComp.er_pretax_match = aobjEmployerDetail.icdoEmployerPayrollDetail.er_pretax_match_reported * iintCredit;
            iblnIsPostDeferredCompContribution = false;
            if ((aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id1 != 0) && (aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount1 != 0.00M))
            {
                // Insert into report data def comp provider 1
                InsertIntoProviderReportDataDefComp(lobjProviderReportData, aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id1, aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount1 * iintCredit);
            }
            if ((aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id2 != 0) && (aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount2 != 0.00M))
            {
                // Insert into report data def comp provider 2
                InsertIntoProviderReportDataDefComp(lobjProviderReportData, aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id2, aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount2 * iintCredit);
            }
            if ((aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id3 != 0) && (aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount3 != 0.00M))
            {
                // Insert into report data def comp provider 3
                InsertIntoProviderReportDataDefComp(lobjProviderReportData, aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id3, aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount3 * iintCredit);
            }
            if ((aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id4 != 0) && (aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount4 != 0.00M))
            {
                // Insert into report data def comp provider 4
                InsertIntoProviderReportDataDefComp(lobjProviderReportData, aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id4, aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount4 * iintCredit);
            }
            if ((aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id5 != 0) && (aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount5 != 0.00M))
            {
                // Insert into report data def comp provider 5
                InsertIntoProviderReportDataDefComp(lobjProviderReportData, aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id5, aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount5 * iintCredit);
            }
            if ((aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id6 != 0) && (aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount6 != 0.00M))
            {
                // Insert into report data def comp provider 6
                InsertIntoProviderReportDataDefComp(lobjProviderReportData, aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id6, aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount6 * iintCredit);
            }
            if ((aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id7 != 0) && (aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount7 != 0.00M))
            {
                // Insert into report data def comp provider 7
                InsertIntoProviderReportDataDefComp(lobjProviderReportData, aobjEmployerDetail.icdoEmployerPayrollDetail.provider_id7, aobjEmployerDetail.icdoEmployerPayrollDetail.contribution_amount7 * iintCredit);
            }
            //PIR 25920 New Plan DC 2025
            if (!iblnIsPostDeferredCompContribution)
            {
                aobjEmployerDetail.CheckMemberPayrollIsPostedInDefComp();
                //insert into def comp contribution for No provider 
                InsertIntoProviderReportDataDefComp(lobjProviderReportData, aobjEmployerDetail.iintPreviousPayrollProviderOrgID, 0);
            }
        }

        //insert into report data for def comp
        public void InsertIntoProviderReportDataDefComp(busProviderReportDataDeffComp aobjProviderReportDataDeffComp, int aintProviderId, Decimal adecContributionAmount)
        {
            busProviderReportDataDeffComp lobjProviderReportData = new busProviderReportDataDeffComp();
            lobjProviderReportData.icdoProviderReportDataDeffComp = new cdoProviderReportDataDeffComp();
            lobjProviderReportData.icdoProviderReportDataDeffComp = aobjProviderReportDataDeffComp.icdoProviderReportDataDeffComp;
            lobjProviderReportData.icdoProviderReportDataDeffComp.provider_org_id = aintProviderId;
            lobjProviderReportData.icdoProviderReportDataDeffComp.contribution_amount = adecContributionAmount;
            lobjProviderReportData.icdoProviderReportDataDeffComp.Insert();
            iblnIsPostDeferredCompContribution = true;
        }

        /// <summary>
        /// Create Empty Payroll Detail and Add it to the Collection, used in File Processing
        /// </summary>
        /// <returns></returns>
        public busEmployerPayrollDetail CreateNewEmployerPayrollDetail()
        {
            if (_icdoEmployerPayrollHeader == null)
            {
                _icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            }
            busEmployerPayrollDetail lobjEmployerPayrollDtl = new busEmployerPayrollDetail();
            lobjEmployerPayrollDtl.icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();
            if (_iclbEmployerPayrollDetail == null)
                _iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            _iclbEmployerPayrollDetail.Add(lobjEmployerPayrollDtl);
            return lobjEmployerPayrollDtl;
        }

        private void UpdateTotalValues()
        {
            if ((_iclbEmployerPayrollDetail != null) && (_iclbEmployerPayrollDetail.Count > 0))
            {
                _icdoEmployerPayrollHeader.total_detail_record_count = _iclbEmployerPayrollDetail.Count;
            }

            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
				//PIR 15238 
                _icdoEmployerPayrollHeader.total_contribution_calculated = _idecTotalContributionFromDetails;
                _icdoEmployerPayrollHeader.total_wages_calculated = _idecTotalWagesCalculated;
                //PIR 25920 New Plan DC 2025
                _icdoEmployerPayrollHeader.total_adec_amount_calculated = _idecTotalADECCalculatedByPlan;
                _icdoEmployerPayrollHeader.total_adec_amount_reported = _idecTotalADECReportedByPlan;
            }
            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                _icdoEmployerPayrollHeader.total_contribution_calculated = _idecTotalContributionCalculatedForDef;
            }
            else if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                _icdoEmployerPayrollHeader.total_purchase_amount = _idecTotalPurchaseAmount;
            }
            
            _icdoEmployerPayrollHeader.total_interest_calculated = _idecTotalInterestCalculated;
			
            _icdoEmployerPayrollHeader.Update();
        }

        /// <summary>
        /// Allocate the Remittance to the Payroll Header if the Remittance Type / Plan exactly matches with Payroll Detail
        /// </summary>
        public void AllocateAutomaticRemittance()
        {
            //Allocation for Retirement
            if (_icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusNoRemittance)
            {
                //prod pir 6062 : need to allocate remittance for RETR or INSR header only from the batch
                /*if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    if (_iclbRetirementContributionByPlan == null)
                    {
                        LoadRetirementContributionByPlan();
                    }

                    decimal ldecRetirementContribTotalAmount = 0;
                    decimal ldecRHICTotalAMount = 0.00M;
                    foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbRetirementContributionByPlan)
                    {
                        //Get the Total Amount for Remittance Type "CONTRIBUTION"
                        ldecRetirementContribTotalAmount = lobjEmployerPayrollHeader.idecTotalEEContributionReportedByPlan +
                                          lobjEmployerPayrollHeader.idecTotalERContributionReportedByPlan +
                                          lobjEmployerPayrollHeader.idecTotalEEPreTaxReportedByPlan +
                                          lobjEmployerPayrollHeader.idecTotalEEEmployerPickupReportedByPlan +
                                          lobjEmployerPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                                          lobjEmployerPayrollHeader.idecTotalEmployerInterestCalculatedByPlan +
                                          lobjEmployerPayrollHeader.idecTotalRHICEmployerInterestCalculatedByPlan;
                        if (ldecRetirementContribTotalAmount > 0)
                        {
                            AllocateRemittanceIfAmountMatch(lobjEmployerPayrollHeader, busConstant.ItemTypeContribution, ldecRetirementContribTotalAmount);
                        }

                        //Allocate RHIC 
                        ldecRHICTotalAMount = lobjEmployerPayrollHeader.idecTotalRHICEEContributionReportedByPlan +
                                              lobjEmployerPayrollHeader.idecTotalRHICERContributionReportedByPlan;
                        if (ldecRHICTotalAMount > 0)
                        {
                            AllocateRemittanceIfAmountMatch(lobjEmployerPayrollHeader, busConstant.ItemTypeRHICContribution, ldecRHICTotalAMount);
                        }
                    }
                }*/

                //Allocation for Deff Comp
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    if (_iclbDeferredCompContributionByPlan == null)
                    {
                        LoadDeferredCompContributionByPlan();
                    }
                    foreach (busEmployerPayrollHeader lobjEmployerHeader in _iclbDeferredCompContributionByPlan)
                    {
                        //Excluding Other 457
                        if (lobjEmployerHeader.iintPlanID != busConstant.PlanIdOther457)
                        {
                            if (lobjEmployerHeader.idecTotalReportedByPlan > 0)
                            {
                                AllocateRemittanceIfAmountMatch(lobjEmployerHeader, busConstant.ItemTypeDefeCompDeposit, lobjEmployerHeader.idecTotalReportedByPlan);
                            }
                        }
                    }
                }

                //prod pir 6062 : need to allocate remittance for RETR or INSR header only from the batch
                /*//Allocation for Insurance
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    if (_iclbInsuranceContributionByPlan == null)
                    {
                        LoadInsurancePremiumByPlan();
                    }
                    decimal ldecInsurancePremiumTotalAmount = 0;
                    foreach (busEmployerPayrollHeader lobjEmployerHeader in _iclbInsuranceContributionByPlan)
                    {
                        ldecInsurancePremiumTotalAmount = lobjEmployerHeader.idecTotalPremiumAmountByPlan;
                        if (ldecInsurancePremiumTotalAmount > 0)
                        {
                            if (lobjEmployerHeader.ibusPlan == null)
                                lobjEmployerHeader.LoadPlan(lobjEmployerHeader.iintPlanID);
                            string lstrRemittanceType = busEmployerReportHelper.GetToItemTypeByPlan(lobjEmployerHeader.ibusPlan.icdoPlan.plan_code);
                            AllocateRemittanceIfAmountMatch(lobjEmployerHeader, lstrRemittanceType, ldecInsurancePremiumTotalAmount);
                        }
                    }
                }*/

                //SysTes PIR 2346
                //Allocation for Purchase
                if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    if (iclbPurchaseContributionByPlan == null)
                    {
                        LoadPurchaseByPlan();
                    }
                    decimal ldecInsurancePremiumTotalAmount = 0;
                    foreach (busEmployerPayrollHeader lobjEmployerHeader in iclbPurchaseContributionByPlan)
                    {
                        if (lobjEmployerHeader.idecTotalReportedByPlan > 0)
                        {
                            AllocateRemittanceIfAmountMatch(lobjEmployerHeader, busConstant.RemittanceTypePurchase, lobjEmployerHeader.idecTotalReportedByPlan);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Allocate the Remittance if the Total Amount Exactly Matches
        /// </summary>
        /// <param name="aobjEmployerPayrollHeader"></param>
        /// <param name="astrRemittanceType"></param>        
        /// <param name="adecTotalAmount"></param>
        private void AllocateRemittanceIfAmountMatch(busEmployerPayrollHeader aobjEmployerPayrollHeader,
                                                     string astrRemittanceType,
                                                     decimal adecTotalAmount)
        {
            Collection<busRemittance> lclbRemittance =
                LoadAppliedRemittance(aobjEmployerPayrollHeader.iintPlanID,
                                      astrRemittanceType,
                                      _icdoEmployerPayrollHeader.org_id);
            //prod pir 4079
            decimal ldecAllowableVariance = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.ConstantAllowableVairance, iobjPassInfo));
            foreach (busRemittance lobjRemittance in lclbRemittance)
            {
                decimal ldecAvailableAmount =
                    busEmployerReportHelper.GetRemittanceAvailableAmount(
                        lobjRemittance.icdoRemittance.remittance_id);
                //prod pir 4079
                if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    decimal ldecDifferencAmount = ldecAvailableAmount - adecTotalAmount;
                    if (ldecAvailableAmount == adecTotalAmount || Math.Abs(ldecDifferencAmount) <= ldecAllowableVariance)
                    {
                        //Do the Allocation
                        CreateRemittanceAllocationAndAllocate(ldecAvailableAmount > adecTotalAmount ? adecTotalAmount : ldecAvailableAmount,
                                                              lobjRemittance.icdoRemittance.remittance_id,
                                                              ldecDifferencAmount,
                                                              astrRemittanceType);
                        CreateGLForDifferenceAmount(astrRemittanceType, ldecDifferencAmount, aobjEmployerPayrollHeader.iintPlanID);
                        break;
                    }
                }
                else
                {
                    if (ldecAvailableAmount == adecTotalAmount)
                    {
                        //Do the Allocation
                        CreateRemittanceAllocationAndAllocate(ldecAvailableAmount,
                                                                  lobjRemittance.icdoRemittance.remittance_id,
                                                                  0.00M,
                                                                  string.Empty);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Get all the Applied Remittance by Criteria
        /// </summary>
        /// <param name="aintPlanId"></param>
        /// <param name="astrRemittanceType"></param>
        /// <param name="aintOrgId"></param>
        /// <returns></returns>
        private Collection<busRemittance> LoadAppliedRemittance(int aintPlanId, string astrRemittanceType, int aintOrgId)
        {
            //GetAppliedRemittanceByPlanOrgRemittanceType
            Collection<busRemittance> lclbRemittance = new Collection<busRemittance>();
            DataTable ldtbAppliedAmountList = busNeoSpinBase.Select("cdoRemittance.GetAppliedRemittanceByPlanOrgRemittanceType",
                                                                     new object[3] { astrRemittanceType, aintPlanId, aintOrgId });

            lclbRemittance = GetCollection<busRemittance>(ldtbAppliedAmountList, "icdoRemittance");
            return lclbRemittance;
        }

        public void CreateRemittanceAllocationAndAllocate(decimal idecAmountAllocate, int aintRemittanceId, decimal adecDifferenceAmount, string astrRemittanceType)
        {
            busEmployerRemittanceAllocation lobjEmployerRemittance = new busEmployerRemittanceAllocation();
            lobjEmployerRemittance.icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation();
            lobjEmployerRemittance.icdoEmployerRemittanceAllocation.employer_payroll_header_id = _icdoEmployerPayrollHeader.employer_payroll_header_id;
            lobjEmployerRemittance.icdoEmployerRemittanceAllocation.remittance_id = aintRemittanceId;
            lobjEmployerRemittance.icdoEmployerRemittanceAllocation.allocated_amount = idecAmountAllocate;
            lobjEmployerRemittance.icdoEmployerRemittanceAllocation.payroll_allocation_status_value =
                busConstant.RemittanceAllocationStatusPending;
            //prod pir 4079
            //--start--//
            lobjEmployerRemittance.icdoEmployerRemittanceAllocation.difference_amount = Math.Abs(adecDifferenceAmount);
            if (adecDifferenceAmount < 0)
            {
                lobjEmployerRemittance.icdoEmployerRemittanceAllocation.difference_type_value =
                    astrRemittanceType == busConstant.ItemTypeContribution ? busConstant.GLItemTypeOverContribution : busConstant.GLItemTypeOverRHIC;
            }
            else if (adecDifferenceAmount > 0)
            {
                lobjEmployerRemittance.icdoEmployerRemittanceAllocation.difference_type_value =
                    astrRemittanceType == busConstant.ItemTypeContribution ? busConstant.GLItemTypeUnderContribution : busConstant.GLItemTypeUnderRHIC;
            }
            //--end--//

            //Load these object for Allocation Validation
            lobjEmployerRemittance.ibusEmployerPayrollHeader = this;
            if (lobjEmployerRemittance.ibusEmployerPayrollHeader.ibusOrganization == null)
                lobjEmployerRemittance.ibusEmployerPayrollHeader.LoadOrganization();
            lobjEmployerRemittance.ibusEmployerPayrollHeader.LoadContributionByPlan();
            lobjEmployerRemittance.ibusEmployerPayrollHeader.LoadAvailableRemittanace();
            lobjEmployerRemittance.ibusEmployerPayrollHeader.CalculateContributionWagesInterestFromDtl();

            lobjEmployerRemittance.LoadRemittance();
            lobjEmployerRemittance.LoadAvailableAmount();
            lobjEmployerRemittance.LoadDeposit();

            lobjEmployerRemittance.icdoEmployerRemittanceAllocation.ienuObjectState = ObjectState.Insert;
            lobjEmployerRemittance.btnAllocate_Click();
        }

        /// <summary>
        /// Function to Reload the Payroll Details (Only for Insurance and Header Status is not posted, ignored or Ready to post)
        /// </summary>
        public void ReLoad_Click()
        {
            ReloadInsurance();
        }

        #region Reports

        public DataTable GetEmployerReportTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn OrgCodeID = new DataColumn("OrgCodeID", Type.GetType("System.String"));
            DataColumn OrgName = new DataColumn("OrgName", Type.GetType("System.String"));
            DataColumn Plan = new DataColumn("Plan", Type.GetType("System.String"));
            DataColumn PayMonth = new DataColumn("PayMonth", Type.GetType("System.String"));
            DataColumn PayPeriodDate = new DataColumn("PayPeriodDate", Type.GetType("System.String"));
            ldtbReportTable.Columns.Add(OrgCodeID);
            ldtbReportTable.Columns.Add(OrgName);
            ldtbReportTable.Columns.Add(Plan);
            ldtbReportTable.Columns.Add(PayMonth);
            ldtbReportTable.Columns.Add(PayPeriodDate);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public DataTable CreatePayrollSplitTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("OrgCode", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("OrgName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("PayrollHeaderID", Type.GetType("System.Int32"));
            DataColumn ldc4 = new DataColumn("BenefitType", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("TotalRecordCount", Type.GetType("System.Int32"));
            DataColumn ldc6 = new DataColumn("TotalAmount", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("CentralPayrollRecordID", Type.GetType("System.Int32"));
            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public DataSet rptOverdueEmployerReport(string astrBenefitType, DateTime adtStartDate, DateTime adtEndDate)
        {
            DataSet ldsOverdueEmployerReport = new DataSet("Overdue Employer Report");
            DataTable ldtOverdueEmployerReport = GetEmployerReportTable();

            if ((adtStartDate == DateTime.MinValue || adtEndDate == DateTime.MinValue))
            {
                adtEndDate = DateTime.Now;
                adtStartDate = adtEndDate.AddYears(-1);
            }

            //Get the Collection of Active Employer For Each Plan Selected (If None Selected, we need select all three benefit type)
            Collection<busOrgPlan> lclbRetirmentOrgPlan = new Collection<busOrgPlan>();
            Collection<busOrgPlan> lclbInsuranceOrgPlan = new Collection<busOrgPlan>();
            Collection<busOrgPlan> lclbDeffCompOrgPlan = new Collection<busOrgPlan>();

            if (String.IsNullOrEmpty(astrBenefitType))
            {
                //Load Retirement Collection
                lclbRetirmentOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeRetirement, adtEndDate);

                //Load Insurance Collection
                lclbInsuranceOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeInsurance, adtEndDate);

                //Load Deff Comp Collection
                lclbDeffCompOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeDeferredComp, adtEndDate);
            }
            else
            {
                switch (astrBenefitType)
                {
                    case busConstant.PlanBenefitTypeRetirement:
                        lclbRetirmentOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeRetirement, adtEndDate);
                        break;
                    case busConstant.PlanBenefitTypeInsurance:
                        lclbInsuranceOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeInsurance, adtEndDate);
                        break;
                    case busConstant.PlanBenefitTypeDeferredComp:
                        lclbDeffCompOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeDeferredComp, adtEndDate);
                        break;

                }
            }

            if (lclbRetirmentOrgPlan.Count > 0)
                ProcessOverdueReportForRetIns(lclbRetirmentOrgPlan, busConstant.PayrollHeaderBenefitTypeRtmt, ldtOverdueEmployerReport, adtStartDate, adtEndDate);
            if (lclbInsuranceOrgPlan.Count > 0)
                ProcessOverdueReportForRetIns(lclbInsuranceOrgPlan, busConstant.PayrollHeaderBenefitTypeInsr, ldtOverdueEmployerReport, adtStartDate, adtEndDate);
            if (lclbDeffCompOrgPlan.Count > 0)
                ProcessOverdueReportForDeffComp(lclbDeffCompOrgPlan, ldtOverdueEmployerReport, adtStartDate, adtEndDate);

            ldsOverdueEmployerReport.Tables.Add(ldtOverdueEmployerReport);
            return ldsOverdueEmployerReport;
        }

        private void ProcessOverdueReportForRetIns(Collection<busOrgPlan> abusOrgPlan, string astrBenefitType, DataTable adtOverdueEmployerReport,
            DateTime adtStartDate, DateTime adtEndDate)
        {
            int lintRetInsGracePeriod = 15;

            //Get the Active Org Plan for Retirment
            foreach (busOrgPlan lbusOrgPlan in abusOrgPlan)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtStartDate, adtEndDate,
                    lbusOrgPlan.icdoOrgPlan.participation_start_date, lbusOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    DataTable ldtbLastReported = Select("cdoEmployerPayrollHeader.GetLastPaidPayrollForRETRandINSR", new object[2] {
                                                        lbusOrgPlan.icdoOrgPlan.org_id,astrBenefitType });

                    //Load the Header Object
                    busEmployerPayrollHeader lbusEmpPayrollHeader = new busEmployerPayrollHeader();
                    lbusEmpPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
                    if (ldtbLastReported != null)
                    {
                        if (ldtbLastReported.Rows.Count > 0)
                        {
                            lbusEmpPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldtbLastReported.Rows[0]);
                        }
                    }

                    DateTime ldtNextPaidDate = lbusEmpPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.AddMonths(1);
                    //Whichever is greater, Set the Start Date
                    DateTime ldtStartDateByOrgPlan = adtStartDate > ldtNextPaidDate ? adtStartDate : ldtNextPaidDate;

                    //Previous Month of End Date Report must be subbitted
                    //Else, two months before End Date report must be submitted
                    DateTime ldtEndDateByOrgPlan = DateTime.Now.Day > lintRetInsGracePeriod ? DateTime.Now.AddMonths(-1) : DateTime.Now.AddMonths(-2);

                    if (ldtEndDateByOrgPlan > adtEndDate)
                        ldtEndDateByOrgPlan = adtEndDate;

                    ldtEndDateByOrgPlan = new DateTime(ldtEndDateByOrgPlan.Year, ldtEndDateByOrgPlan.Month, 1);

                    //Skip the Record if the End Date is GT than ST. Which Means Payroll gets submitted for all the months.
                    if (ldtEndDateByOrgPlan < ldtStartDateByOrgPlan) continue; 

                    //Add it to the Report
                    AddReportDataRow(adtOverdueEmployerReport, lbusOrgPlan, astrBenefitType, ldtStartDateByOrgPlan, ldtEndDateByOrgPlan);
                }
            }
        }

        private void ProcessOverdueReportForDeffComp(Collection<busOrgPlan> aclbOrgPlan, DataTable adtOverdueEmployerReport,
            DateTime adtStartDate, DateTime adtEndDate)
        {
            int lintDeffCompGracePeriod = 5;
            //Get the Active Org Plan for Deferred Comp
            foreach (busOrgPlan lbusOrgPlan in aclbOrgPlan)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtStartDate, adtEndDate,
                    lbusOrgPlan.icdoOrgPlan.participation_start_date, lbusOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    DataTable ldtbLastReported = Select("cdoEmployerPayrollHeader.GetLastPaidPayrollForDefComp", new object[2] {
                                                        lbusOrgPlan.icdoOrgPlan.org_id,busConstant.PayrollHeaderBenefitTypeDefComp });

                    //Load the Header Object
                    busEmployerPayrollHeader lbusEmpPayrollHeader = new busEmployerPayrollHeader();
                    lbusEmpPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
                    if (ldtbLastReported != null)
                    {
                        if (ldtbLastReported.Rows.Count > 0)
                        {
                            lbusEmpPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldtbLastReported.Rows[0]);
                        }
                    }

                    DateTime ldtNextPaidDate = lbusEmpPayrollHeader.icdoEmployerPayrollHeader.pay_period_end_date.AddDays(1);
                    //Whichever is greater, Set the Start Date
                    DateTime ldtStartDateByOrgPlan = adtStartDate > ldtNextPaidDate ? adtStartDate : ldtNextPaidDate;

                    //If the End Date is not given by user, we must calculate based on the previous months
                    //Calculate the End Date. If the Current Day of End Date is greater than Grace Period, 
                    //Previous Month of End Date Report must be subbitted
                    //Else, two months before End Date report must be submitted
                    DateTime ldtEndDateByOrgPlan = DateTime.Now.AddDays(-lintDeffCompGracePeriod);

                    if (ldtEndDateByOrgPlan > adtEndDate)
                        ldtEndDateByOrgPlan = adtEndDate;

                    //Skip the Record if the End Date is GT than ST. Which Means Payroll gets submitted for all the months.
                    if (ldtEndDateByOrgPlan < ldtStartDateByOrgPlan) continue;

                    //Add it to the Report
                    AddReportDataRow(adtOverdueEmployerReport, lbusOrgPlan, busConstant.PayrollHeaderBenefitTypeDefComp, ldtStartDateByOrgPlan, ldtEndDateByOrgPlan);
                }
            }
        }

        public DataSet rptOutstandingEmployerReport(string astrBenefitType, DateTime adtEffectiveDate)
        {
            DataSet ldsOutstandingEmployerReport = new DataSet("Outstanding Employer Report");
            DataTable ldtOutstandingEmployerReport = GetEmployerReportTable();

            //Get the Collection of Active Employer For Each Plan Selected (If None Selected, we need select all three benefit type)
            Collection<busOrgPlan> lclbRetirmentOrgPlan = new Collection<busOrgPlan>();
            Collection<busOrgPlan> lclbInsuranceOrgPlan = new Collection<busOrgPlan>();
            Collection<busOrgPlan> lclbDeffCompOrgPlan = new Collection<busOrgPlan>();

            if (String.IsNullOrEmpty(astrBenefitType))
            {
                //Load Retirement Collection
                lclbRetirmentOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeRetirement, adtEffectiveDate);

                //Load Insurance Collection
                lclbInsuranceOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeInsurance, adtEffectiveDate);

                //Load Deff Comp Collection
                lclbDeffCompOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeDeferredComp, adtEffectiveDate);
            }
            else
            {
                switch (astrBenefitType)
                {
                    case busConstant.PlanBenefitTypeRetirement:
                        lclbRetirmentOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeRetirement, adtEffectiveDate);
                        break;
                    case busConstant.PlanBenefitTypeInsurance:
                        lclbInsuranceOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeInsurance, adtEffectiveDate);
                        break;
                    case busConstant.PlanBenefitTypeDeferredComp:
                        lclbDeffCompOrgPlan = busEmployerReportHelper.LoadActiveEmployerOrgPlanByBenefitType(busConstant.PlanBenefitTypeDeferredComp, adtEffectiveDate);
                        break;

                }
            }

            if (lclbRetirmentOrgPlan.Count > 0)
                ProcessOutstandingReportForRetIns(lclbRetirmentOrgPlan, busConstant.PayrollHeaderBenefitTypeRtmt, ldtOutstandingEmployerReport, adtEffectiveDate);
            if (lclbInsuranceOrgPlan.Count > 0)
                ProcessOutstandingReportForRetIns(lclbInsuranceOrgPlan, busConstant.PayrollHeaderBenefitTypeInsr, ldtOutstandingEmployerReport, adtEffectiveDate);
            if (lclbDeffCompOrgPlan.Count > 0)
                ProcessOutstandingReportForDeffComp(lclbDeffCompOrgPlan, ldtOutstandingEmployerReport, adtEffectiveDate);

            ldsOutstandingEmployerReport.Tables.Add(ldtOutstandingEmployerReport);
            return ldsOutstandingEmployerReport;
        }

        private void ProcessOutstandingReportForRetIns(Collection<busOrgPlan> abusOrgPlan, string astrBenefitType, DataTable adtOutstandingEmployerReport, DateTime adtEffectiveDate)
        {
            int lintRetInsGracePeriod = 15;

            //Get the Active Org Plan for Retirment
            foreach (busOrgPlan lbusOrgPlan in abusOrgPlan)
            {
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                    lbusOrgPlan.icdoOrgPlan.participation_start_date, lbusOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    DataTable ldtbLastReported = Select("cdoEmployerPayrollHeader.GetLastPaidPayrollForRETRandINSR", new object[2] {
                                                        lbusOrgPlan.icdoOrgPlan.org_id,astrBenefitType });

                    //Load the Header Object
                    busEmployerPayrollHeader lbusEmpPayrollHeader = new busEmployerPayrollHeader();
                    lbusEmpPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
                    if (ldtbLastReported != null)
                    {
                        if (ldtbLastReported.Rows.Count > 0)
                        {
                            lbusEmpPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldtbLastReported.Rows[0]);
                        }
                    }

                    DateTime ldtLastPaidDate = lbusEmpPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date;
                    //If no payroll entry submitted so far, set the start date as org plan start date
                    if (ldtLastPaidDate == DateTime.MinValue)
                    {
                        ldtLastPaidDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SystemConstantsLastEmployerPostingDate, iobjPassInfo));
                        ldtLastPaidDate = ldtLastPaidDate < lbusOrgPlan.icdoOrgPlan.participation_start_date ? lbusOrgPlan.icdoOrgPlan.participation_start_date : ldtLastPaidDate;
                    }

                    //Previous Month of End Date Report must be subbitted
                    //Else, two months before End Date report must be submitted
                    DateTime ldtOutstandingDate = adtEffectiveDate != DateTime.MinValue ? (adtEffectiveDate.Day > lintRetInsGracePeriod ? adtEffectiveDate.AddMonths(-1) : adtEffectiveDate.AddMonths(-2))
                        : (DateTime.Now.Day > lintRetInsGracePeriod ? DateTime.Now.AddMonths(-1) : DateTime.Now.AddMonths(-2));

                    ldtOutstandingDate = new DateTime(ldtOutstandingDate.Year, ldtOutstandingDate.Month, 1);

                    //Skip the Record if the Last Paid Date is GT than or Equal to Outstanding Date which Means Payroll gets submitted for all the months.
                    if (ldtLastPaidDate >= ldtOutstandingDate) continue;

                    //Add it to the Report
                    AddOutStandingReportDataRow(adtOutstandingEmployerReport, lbusOrgPlan, astrBenefitType, ldtOutstandingDate, DateTime.MinValue);
                }
            }
        }

        private void ProcessOutstandingReportForDeffComp(Collection<busOrgPlan> aclbOrgPlan, DataTable adtOutstandingEmployerReport, DateTime adtEffectiveDate)
        {
            int lintDeffCompGracePeriod = 5;
            //Get the Active Org Plan for Deferred Comp
            foreach (busOrgPlan lbusOrgPlan in aclbOrgPlan)
            {
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                    lbusOrgPlan.icdoOrgPlan.participation_start_date, lbusOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    DataTable ldtbLastReported = Select("cdoEmployerPayrollHeader.GetLastPaidPayrollForDefComp", new object[2] {
                                                        lbusOrgPlan.icdoOrgPlan.org_id,busConstant.PayrollHeaderBenefitTypeDefComp });
                    
                    //Load the Header Object
                    //Start PIR 8442
                    icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
                    if (ldtbLastReported != null)
                    {
                        if (ldtbLastReported.Rows.Count > 0)
                        {
                            icdoEmployerPayrollHeader.LoadData(ldtbLastReported.Rows[0]);
                        }
                    }

                    DateTime ldtLastPaidDate = icdoEmployerPayrollHeader.pay_period_end_date;
                    //End PIR 8442

                    //If no payroll entry submitted so far, set the start date as org plan start date
                    if (ldtLastPaidDate == DateTime.MinValue)
                    {
                        ldtLastPaidDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.SystemConstantsLastEmployerPostingDate, iobjPassInfo));
                        ldtLastPaidDate = ldtLastPaidDate < lbusOrgPlan.icdoOrgPlan.participation_start_date ? lbusOrgPlan.icdoOrgPlan.participation_start_date : ldtLastPaidDate;
                    }

                    string lstrFrequency = GetDeferredCompFrequencyPeriodByOrg(lbusOrgPlan.icdoOrgPlan.org_id);
                    //PIR 19701 Day_Of_Month for Org Plan Maintenance will be use for verify validation 4690
                        
                    DateTime ldtEndDateLastPaid = busEmployerReportHelper.GetEndDateByReportFrequency(ldtLastPaidDate, lstrFrequency);

                    //Previous Month of End Date Report must be subbitted
                    //Else, two months before End Date report must be submitted
                    DateTime ldtOutstandingDate = adtEffectiveDate.AddDays(-lintDeffCompGracePeriod);

                    //Skip the Record if the End Date is GT than ST. Which Means Payroll gets submitted for all the months.
                    if (ldtEndDateLastPaid >= ldtOutstandingDate) continue;

                    //Add it to the Report
                    AddOutStandingReportDataRow(adtOutstandingEmployerReport, lbusOrgPlan, busConstant.PayrollHeaderBenefitTypeDefComp, ldtOutstandingDate, ldtLastPaidDate);
                }
            }
        }

        public void AddReportDataRow(DataTable adtReportTable, busOrgPlan abusOrgPlan, string astrBenefitType, DateTime adtPaymentDateFrom, DateTime adtPaymentDateTo)
        {
            if (abusOrgPlan.ibusOrganization == null)
                abusOrgPlan.LoadOrganization();

            string lstrOrgCode = abusOrgPlan.ibusOrganization.icdoOrganization.org_code;
            string lstrOrgName = abusOrgPlan.ibusOrganization.icdoOrganization.org_name;
            string lstrPlan = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(250, astrBenefitType);
            string lstrPayMonth = string.Empty;
            string lstrPayPeriodDate = string.Empty;

            if ((astrBenefitType == busConstant.PayrollHeaderBenefitTypeRtmt) || (astrBenefitType == busConstant.PayrollHeaderBenefitTypeInsr))
            {
                lstrPayMonth = GetDateRange(astrBenefitType, adtPaymentDateFrom, adtPaymentDateTo);
                lstrPayPeriodDate = string.Empty;
            }
            if (astrBenefitType == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                lstrPayMonth = string.Empty;
                lstrPayPeriodDate = GetDateRange(astrBenefitType, adtPaymentDateFrom, adtPaymentDateTo);
            }

            if (!IsEntryExistsInDataTable(adtReportTable, lstrOrgCode, lstrPlan, lstrPayMonth, lstrPayPeriodDate))
            {
                DataRow drReport = adtReportTable.NewRow();
                drReport["OrgCodeID"] = lstrOrgCode;
                drReport["OrgName"] = lstrOrgName;
                drReport["Plan"] = lstrPlan;
                drReport["PayMonth"] = lstrPayMonth;
                drReport["PayPeriodDate"] = lstrPayPeriodDate;
                adtReportTable.Rows.Add(drReport);
            }
        }

        private bool IsEntryExistsInDataTable(DataTable adtReportTable, string astrOrgCode, string astrPlan, string astrPayMonth, string astrPayPeriodDate)
        {
            bool lblnFound = false;
            foreach (DataRow ldrRow in adtReportTable.Rows)
            {
                if ((ldrRow["OrgCodeID"].ToString() == astrOrgCode) &&
                    (ldrRow["Plan"].ToString() == astrPlan) &&
                    (ldrRow["PayMonth"].ToString() == astrPayMonth) &&
                    (ldrRow["PayPeriodDate"].ToString() == astrPayPeriodDate))
                {
                    lblnFound = true;
                    break;
                }
            }
            return lblnFound;
        }

        private void AddOutStandingReportDataRow(DataTable adtReportTable, busOrgPlan abusOrgPlan, string astrBenefitType,
            DateTime adtOutstandingDate, DateTime adtLastPaidDate)
        {
            if (abusOrgPlan.ibusOrganization == null)
                abusOrgPlan.LoadOrganization();

            string lstrOrgCode = abusOrgPlan.ibusOrganization.icdoOrganization.org_code;
            string lstrOrgName = abusOrgPlan.ibusOrganization.icdoOrganization.org_name;
            string lstrPlan = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(250, astrBenefitType);
            string lstrPayMonth = string.Empty;
            string lstrPayPeriodDate = string.Empty;

            if ((astrBenefitType == busConstant.PayrollHeaderBenefitTypeRtmt) || (astrBenefitType == busConstant.PayrollHeaderBenefitTypeInsr))
            {
                lstrPayMonth = adtOutstandingDate.ToString("MMMM yyyy");
                lstrPayPeriodDate = string.Empty;
            }
            if (astrBenefitType == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                lstrPayMonth = string.Empty;
                lstrPayPeriodDate = adtLastPaidDate.AddDays(1).ToString("MMMM yyyy") + " - " + adtOutstandingDate.ToString("MMMM yyyy");
            }

            if (!IsEntryExistsInDataTable(adtReportTable, lstrOrgCode, lstrPlan, lstrPayMonth, lstrPayPeriodDate))
            {
                DataRow drReport = adtReportTable.NewRow();
                drReport["OrgCodeID"] = lstrOrgCode;
                drReport["OrgName"] = lstrOrgName;
                drReport["Plan"] = lstrPlan;
                drReport["PayMonth"] = lstrPayMonth;
                drReport["PayPeriodDate"] = lstrPayPeriodDate;
                adtReportTable.Rows.Add(drReport);
            }
        }

        private void AddPayrollSplitDataRow(DataTable adtReportTable, busEmployerPayrollHeader abusHeader)
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("OrgCode", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("OrgName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("PayrollHeaderID", Type.GetType("System.Int32"));
            DataColumn ldc4 = new DataColumn("BenefitType", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("TotalRecordCount", Type.GetType("System.Int32"));
            DataColumn ldc6 = new DataColumn("TotalAmount", Type.GetType("System.Decimal"));
            DataColumn ldc7 = new DataColumn("CentralPayrollRecordID", Type.GetType("System.Int32"));

            if (abusHeader.ibusOrganization == null)
                abusHeader.LoadOrganization();

            //To Get the Total Contribution and Count
            abusHeader.CalculateContributionWagesInterestFromDtl();

            decimal ldecTotalAmount = 0.00M;
            if (abusHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                ldecTotalAmount = abusHeader.idecTotalContributionCalculatedForRet;
            if (abusHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                ldecTotalAmount = abusHeader.idecTotalPremiumReportedForIns;
            if (abusHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                ldecTotalAmount = abusHeader.idecTotalContributionCalculatedForDef;

            DataRow drReport = adtReportTable.NewRow();
            drReport["OrgCode"] = abusHeader.ibusOrganization.icdoOrganization.org_code;
            drReport["OrgName"] = abusHeader.ibusOrganization.icdoOrganization.org_name;
            drReport["PayrollHeaderID"] = abusHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;
            drReport["BenefitType"] = abusHeader.icdoEmployerPayrollHeader.header_type_description;
            drReport["TotalRecordCount"] = abusHeader.icdoEmployerPayrollHeader.total_detail_record_count;
            drReport["TotalAmount"] = ldecTotalAmount;
            drReport["CentralPayrollRecordID"] = abusHeader.icdoEmployerPayrollHeader.central_payroll_record_id;
            adtReportTable.Rows.Add(drReport);
        }

        public DataSet rptPayrollSplitFromCentralPayroll(string astrBenefitType, DateTime adtStartDate, DateTime adtEndDate)
        {
            DataSet ldsPayrollSplitReport = new DataSet("Payroll Split Report");
            DataTable ldtPayrollSplitReport = CreatePayrollSplitTable();

            //Get the Collection of Active Employer For Each Plan Selected (If None Selected, we need select all three benefit type)
            Collection<busEmployerPayrollHeader> lclbRetirmentPayrollHeader = new Collection<busEmployerPayrollHeader>();
            Collection<busEmployerPayrollHeader> lclbInsurancePayrollHeader = new Collection<busEmployerPayrollHeader>();
            Collection<busEmployerPayrollHeader> lclbDeffCompPayrollHeader = new Collection<busEmployerPayrollHeader>();

            if (String.IsNullOrEmpty(astrBenefitType))
            {
                //Load Retirement Collection
                lclbRetirmentPayrollHeader = LoadCentralPayrollHeaderByBenefitTypeAndDate(busConstant.PlanBenefitTypeRetirement, adtStartDate, adtEndDate);

                //Load Insurance Collection
                lclbInsurancePayrollHeader = LoadCentralPayrollHeaderByBenefitTypeAndDate(busConstant.PlanBenefitTypeInsurance, adtStartDate, adtEndDate);

                //Load Deff Comp Collection
                lclbDeffCompPayrollHeader = LoadCentralPayrollHeaderByBenefitTypeAndDate(busConstant.PlanBenefitTypeDeferredComp, adtStartDate, adtEndDate);
            }
            else
            {
                switch (astrBenefitType)
                {
                    case busConstant.PlanBenefitTypeRetirement:
                        lclbRetirmentPayrollHeader = LoadCentralPayrollHeaderByBenefitTypeAndDate(busConstant.PlanBenefitTypeRetirement, adtStartDate, adtEndDate);
                        break;
                    case busConstant.PlanBenefitTypeInsurance:
                        lclbInsurancePayrollHeader = LoadCentralPayrollHeaderByBenefitTypeAndDate(busConstant.PlanBenefitTypeInsurance, adtStartDate, adtEndDate);
                        break;
                    case busConstant.PlanBenefitTypeDeferredComp:
                        lclbDeffCompPayrollHeader = LoadCentralPayrollHeaderByBenefitTypeAndDate(busConstant.PlanBenefitTypeDeferredComp, adtStartDate, adtEndDate);
                        break;
                }
            }

            if (lclbRetirmentPayrollHeader.Count > 0)
            {
                foreach (busEmployerPayrollHeader lbusHeader in lclbRetirmentPayrollHeader)
                {
                    AddPayrollSplitDataRow(ldtPayrollSplitReport, lbusHeader);
                }
            }

            if (lclbInsurancePayrollHeader.Count > 0)
            {
                foreach (busEmployerPayrollHeader lbusHeader in lclbInsurancePayrollHeader)
                {
                    AddPayrollSplitDataRow(ldtPayrollSplitReport, lbusHeader);
                }
            }

            if (lclbDeffCompPayrollHeader.Count > 0)
            {
                foreach (busEmployerPayrollHeader lbusHeader in lclbDeffCompPayrollHeader)
                {
                    AddPayrollSplitDataRow(ldtPayrollSplitReport, lbusHeader);
                }
            }

            ldsPayrollSplitReport.Tables.Add(ldtPayrollSplitReport);
            return ldsPayrollSplitReport;
        }

        private Collection<busEmployerPayrollHeader> LoadCentralPayrollHeaderByBenefitTypeAndDate(string astrBenefitType, DateTime adtStartDate, DateTime adtEndDate)
        {
            Collection<busEmployerPayrollHeader> lclbEmployerPayrollHeader = new Collection<busEmployerPayrollHeader>();
            string lstrQueryName = "cdoEmployerPayrollHeader.LoadCentralPayrollHeaderForRetInsByDate";
            if (astrBenefitType == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                lstrQueryName = "cdoEmployerPayrollHeader.LoadCentralPayrollHeaderForDeff";
            }
            DataTable ldtbList = Select(lstrQueryName, new object[3] { astrBenefitType, adtStartDate, adtEndDate });
            lclbEmployerPayrollHeader = GetCollection<busEmployerPayrollHeader>(ldtbList, "icdoEmployerPayrollHeader");

            return lclbEmployerPayrollHeader;
        }

        private string GetDateRange(string astrBenefitType, DateTime adtFromDate, DateTime adtToDate)
        {
            string lstrDateRange = string.Empty;
            if (astrBenefitType == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                lstrDateRange = adtFromDate.ToString("MMMM dd yyyy") + " - " + adtToDate.ToString("MMMM dd yyyy");
            }
            else
            {
                if ((adtFromDate.Month == adtToDate.Month) && (adtFromDate.Year == adtToDate.Year))
                {
                    lstrDateRange = adtFromDate.ToString("MMMM yyyy");
                }
                else
                {
                    lstrDateRange = adtFromDate.ToString("MMMM yyyy") + " - " + adtToDate.ToString("MMMM yyyy");
                }
            }
            return lstrDateRange;
        }

        public string GetDeferredCompFrequencyPeriodByOrg(int aintOrgID)
        {
            //Set Default
            string lstrResult = null;
            //PROD PIR 5497            
            if (icdoEmployerPayrollHeader.pay_period_start_date != DateTime.MinValue && icdoEmployerPayrollHeader.pay_period_end_date != DateTime.MinValue)
            {
                DataTable ldtbFrequency = Select("cdoEmployerPayrollHeader.GetDeferredCompFrequencyByOrg",
                                                 new object[3] { aintOrgID, icdoEmployerPayrollHeader.pay_period_start_date, icdoEmployerPayrollHeader.pay_period_end_date });
                if (ldtbFrequency.Rows.Count > 0)
                {
                    lstrResult = ldtbFrequency.Rows[0]["REPORT_FREQUENCY_VALUE"].ToString();
                }
            }
            //this block to handle the call from outstanding report
            else
            {
                DataTable ldtbFrequency = Select("cdoEmployerPayrollHeader.GetDeferredCompFrequencyByOrg",
                                                                new object[3] { aintOrgID, DateTime.Now, DateTime.Now });
                if (ldtbFrequency.Rows.Count > 0)
                {
                    lstrResult = ldtbFrequency.Rows[0]["REPORT_FREQUENCY_VALUE"].ToString();
                }
            }
            return lstrResult;
        }
        #endregion

        ///<Summary>
        ///Check if interest waiver is checked when few detail records are posteed
        ///
        public bool CheckInterestWaiverIsModifiedAfterDetailPosted()
        {
            if (_icdoEmployerPayrollHeader.ihstOldValues["interest_waiver_flag"] != null)
            {
                if (_icdoEmployerPayrollHeader.ihstOldValues["interest_waiver_flag"].ToString() != _icdoEmployerPayrollHeader.interest_waiver_flag)
                {
                    foreach (busEmployerPayrollDetail lobjEmployerDetail in _iclbEmployerPayrollDetail)
                    {
                        if (lobjEmployerDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPosted)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Automatic Allocation for each payroll detail if amount matches 
        /// </summary>
        public void AllocateAutomaticForPurchase()
        {
            if ((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                && (icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusNoRemittance))
            {
                foreach (busEmployerPayrollDetail lobjEmployerPayrolldetail in iclbEmployerPayrollDetail)
                {
                    lobjEmployerPayrolldetail.AutomaticAllocation();
                }
            }
        }

        /// <summary>
        /// to compare total balance contract amount and total reported amount in header
        /// 1) load details
        /// 2) for each details sum total balance purchase  contract amount
        /// 3) sum total Purchase Contract Amount and compare with reported amount
        /// </summary>
        /// <returns></returns>
        public bool CompareTotalBalanceContractAmtAndReportedAmt()
        {
            if (iclbEmployerPayrollDetail == null)
            {
                LoadEmployerPayrollDetail();
            }


            decimal ldecTotalBalanceAmount = 0.00M;
            foreach (busEmployerPayrollDetail lobjEmployerPayrolldetail in _iclbEmployerPayrollDetail)
            {
                lobjEmployerPayrolldetail.ibusEmployerPurchaseAllocation = new busEmployerPurchaseAllocation();
                lobjEmployerPayrolldetail.ibusEmployerPurchaseAllocation.LoadPurchaseForPerson(lobjEmployerPayrolldetail);
                foreach (busServicePurchaseHeader lobjServicePurchase in lobjEmployerPayrolldetail.ibusEmployerPurchaseAllocation.iclbServicePurchase)
                {
                    int lintServicePurchaseHeaderId = lobjServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id;
                    decimal ldecTotalContractAmount = lobjServicePurchase.icdoServicePurchaseHeader.total_contract_amount;
                    ldecTotalBalanceAmount = +busEmployerReportHelper.GetTotalBalanceAmountForPurchase(lintServicePurchaseHeaderId, ldecTotalContractAmount);
                }
            }
            if (ldecTotalBalanceAmount != 0.00M)
            {
                if (icdoEmployerPayrollHeader.total_purchase_amount > ldecTotalBalanceAmount)
                {
                    return false;
                }
            }
            return true;
        }
        public bool LoadCurrentAdjustmentPayrollHeader(int aintOrgID, string astrHeaderType)
        {
            if (icdoEmployerPayrollHeader == null)
                icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            DataTable ldtbList = Select<cdoEmployerPayrollHeader>(
                new string[4] { "org_id", "header_type_value", "report_type_value", "status_value" },
                new object[4] { aintOrgID, astrHeaderType, busConstant.PayrollHeaderReportTypeAdjustment, busConstant.PayrollHeaderStatusReview }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoEmployerPayrollHeader.LoadData(ldtbList.Rows[0]);
                return true;
            }
            else
                return false;
        }
        public void CreateInsuranceAdjustmentPayrollHeader(int aintOrgID)
        {
            if (icdoEmployerPayrollHeader == null)
                icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();

            icdoEmployerPayrollHeader.org_id = aintOrgID;
            icdoEmployerPayrollHeader.header_type_value = busConstant.PayrollHeaderBenefitTypeInsr;
            icdoEmployerPayrollHeader.reporting_source_value = busConstant.PayrollHeaderReportingSourcePaperRpt; //??
            icdoEmployerPayrollHeader.report_type_value = busConstant.PayrollHeaderReportTypeAdjustment;
            icdoEmployerPayrollHeader.payroll_paid_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReview;
            icdoEmployerPayrollHeader.received_date = DateTime.Now;
            icdoEmployerPayrollHeader.submitted_date = DateTime.Now;
            icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Insert;
        }
        public void AddInsuranceAdjustmentPayrollDetail(int aintPersonID, int aintPlanID, string astrFirstName, string astrLastName, string astrSSN, string astrRecordType,
            DateTime adatEffective, decimal adecPremiumAmt, int aintProviderOrgID, decimal adecFeeAmt = 0M, decimal adecBuydownAmt = 0M, decimal adecMedicarePartDAmt = 0M, decimal adecRHICAmt = 0M, decimal adecOthrRHICAmt = 0M, decimal adecJSRHICAmt = 0M,
			decimal adecHSAAmt = 0M,decimal adecVendorAmt = 0M,//PIR 7705
            decimal adecBasicPremium = 0M, decimal adecSuppPremium = 0M, decimal adecSpouseSuppPremium = 0M, decimal adecDepSuppPremium = 0M,
            decimal adecLtcMember3YrsPremium = 0M, decimal adecLtcMember5YrsPremium = 0M, decimal adecLtcSpouse3YrsPremium = 0M, decimal adecLtcSpouse5YrsPremium = 0M,
            int aintGHDVHistoryID = 0, string astrGroupNumber = "", decimal adecADAndDBasiceRate = 0.0000M, decimal adecADAndDSuppRate = 0.0000M,
            decimal adecBasicCoverageAmount = 0.00M, decimal adecSuppCoverageAmount = 0.00M, decimal adecSpouSuppCoverageAmount = 0.00M,
            decimal adecDepSuppCoverageAmount = 0.00M, string astrCoverageCodeValue = "", string astrRateStructureCode = "", decimal adecLowIncomeCredit = 0M, decimal adecLateEnrollmentPenalty = 0M,
            int aintPersonAccountID = 0, Collection<busEmployerPayrollDetail> acolPosNegEmployerPayrollDtl = null)//Org to bill
        {
            busEmployerPayrollDetail lbusPayrollDtl = new busEmployerPayrollDetail();
            lbusPayrollDtl.icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();
            lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
            lbusPayrollDtl.icdoEmployerPayrollDetail.person_id = aintPersonID;
            lbusPayrollDtl.icdoEmployerPayrollDetail.plan_id = aintPlanID;
            lbusPayrollDtl.icdoEmployerPayrollDetail.first_name = astrFirstName;
            lbusPayrollDtl.icdoEmployerPayrollDetail.last_name = astrLastName;
            lbusPayrollDtl.icdoEmployerPayrollDetail.ssn = astrSSN;
            lbusPayrollDtl.icdoEmployerPayrollDetail.record_type_value = astrRecordType;
            lbusPayrollDtl.icdoEmployerPayrollDetail.pay_period_date = adatEffective; ;
            lbusPayrollDtl.icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusValid;
            lbusPayrollDtl.icdoEmployerPayrollDetail.premium_amount = adecPremiumAmt;
            lbusPayrollDtl.icdoEmployerPayrollDetail.premium_amount_from_enrollment = adecPremiumAmt;
            lbusPayrollDtl.icdoEmployerPayrollDetail.group_health_fee_amount = adecFeeAmt;
            lbusPayrollDtl.icdoEmployerPayrollDetail.buydown_amount = adecBuydownAmt; // PIR 11239
            lbusPayrollDtl.icdoEmployerPayrollDetail.medicare_part_d_amt = adecMedicarePartDAmt;//PIR 14271
            lbusPayrollDtl.icdoEmployerPayrollDetail.rhic_benefit_amount = adecRHICAmt;
            /* UAT PIR 476, Including other and JS RHIC Amount */
            lbusPayrollDtl.icdoEmployerPayrollDetail.othr_rhic_amount = adecOthrRHICAmt;
            lbusPayrollDtl.icdoEmployerPayrollDetail.js_rhic_amount = adecJSRHICAmt;
            lbusPayrollDtl.icdoEmployerPayrollDetail.basic_premium = adecBasicPremium;
            lbusPayrollDtl.icdoEmployerPayrollDetail.supplemental_premium = adecSuppPremium;
            lbusPayrollDtl.icdoEmployerPayrollDetail.spouse_premium = adecSpouseSuppPremium;
            lbusPayrollDtl.icdoEmployerPayrollDetail.dependent_premium = adecDepSuppPremium;
            lbusPayrollDtl.icdoEmployerPayrollDetail.ltc_member_three_yrs_premium = adecLtcMember3YrsPremium;
            lbusPayrollDtl.icdoEmployerPayrollDetail.ltc_member_five_yrs_premium = adecLtcMember5YrsPremium;
            lbusPayrollDtl.icdoEmployerPayrollDetail.ltc_spouse_three_yrs_premium = adecLtcSpouse3YrsPremium;
            lbusPayrollDtl.icdoEmployerPayrollDetail.ltc_spouse_five_yrs_premium = adecLtcSpouse5YrsPremium;
            /* UAT PIR 476 ends here */
            //uat pir 1429 : post ghdv_history_id and group number
            //prod pir 6076 & 6077 - Removal of person account ghdv history id
            //lbusPayrollDtl.icdoEmployerPayrollDetail.person_account_ghdv_history_id = aintGHDVHistoryID;
            lbusPayrollDtl.icdoEmployerPayrollDetail.group_number = astrGroupNumber;
            //PROD Pir 4260
            lbusPayrollDtl.icdoEmployerPayrollDetail.ad_and_d_basic_premium_rate = adecADAndDBasiceRate;
            lbusPayrollDtl.icdoEmployerPayrollDetail.ad_and_d_supplemental_premium_rate = adecADAndDSuppRate;
            lbusPayrollDtl.icdoEmployerPayrollDetail.life_basic_coverage_amount = adecBasicCoverageAmount;
            lbusPayrollDtl.icdoEmployerPayrollDetail.life_supp_coverage_amount = adecSuppCoverageAmount;
            lbusPayrollDtl.icdoEmployerPayrollDetail.life_spouse_supp_coverage_amount = adecSpouSuppCoverageAmount;
            lbusPayrollDtl.icdoEmployerPayrollDetail.life_dep_supp_coverage_amount = adecDepSuppCoverageAmount;

            //PIR 4444
            lbusPayrollDtl.icdoEmployerPayrollDetail.provider_org_id = aintProviderOrgID;

            //prod pir 6076
            lbusPayrollDtl.icdoEmployerPayrollDetail.coverage_code = astrCoverageCodeValue;
            lbusPayrollDtl.icdoEmployerPayrollDetail.rate_structure_code = astrRateStructureCode;
            //PIR 7705
            lbusPayrollDtl.icdoEmployerPayrollDetail.hsa_amount = adecHSAAmt;
            lbusPayrollDtl.icdoEmployerPayrollDetail.vendor_amount = adecVendorAmt;

            //Org to bill
            lbusPayrollDtl.icdoEmployerPayrollDetail.lis_amount = adecLowIncomeCredit;
            lbusPayrollDtl.icdoEmployerPayrollDetail.lep_amount = adecLateEnrollmentPenalty;
            lbusPayrollDtl.icdoEmployerPayrollDetail.person_account_id = aintPersonAccountID;

            lbusPayrollDtl.icdoEmployerPayrollDetail.ienuObjectState = ObjectState.Insert;
            if (_iclbEmployerPayrollDetail != null)
                _iclbEmployerPayrollDetail.Add(lbusPayrollDtl);

            if (acolPosNegEmployerPayrollDtl != null)
            {
                int MaxSrNo = 0;
                if (acolPosNegEmployerPayrollDtl.Count > 0)
                    MaxSrNo = acolPosNegEmployerPayrollDtl.Max(i => i.SrNo);
                lbusPayrollDtl.SrNo = MaxSrNo + 1;
                acolPosNegEmployerPayrollDtl.Add(lbusPayrollDtl);
            }
        }

        public override busBase GetCorOrganization()
        {
            if (ibusOrganization == null)
                LoadOrganization();
            return ibusOrganization;
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            if (!String.IsNullOrEmpty(istrOrgCodeId))
                icdoEmployerPayrollHeader.org_id = busGlobalFunctions.GetOrgIdFromOrgCode(istrOrgCodeId);
            LoadOrganization();
            larrList.Add(this);
            return larrList;
        }

        //Retirement Correspondence SFN
        public void LoadOtherDefCompObjectsForRetirement()
        {
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();

            foreach (busEmployerPayrollDetail lobjEmployerDetail in iclbEmployerPayrollDetail)
            {
                if (lobjEmployerDetail.ibusPerson == null)
                    lobjEmployerDetail.LoadPerson();

                if (lobjEmployerDetail.ibusPlan == null)
                    lobjEmployerDetail.LoadPlan();
            }
        }
        //SFN 52193 Corr Properties
        public decimal idecTotalProviderDeductions { get; set; }
        //used to create collection for correspondence after posting for header type deferred compensation
        public void LoadOtherDefCompObjectsForCor()
        {
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();

            _iclbCollectionForPostingDefCompCor = new Collection<busEmployerPayrollDetail>();
            idecTotalProviderDeductions = 0;
            foreach (busEmployerPayrollDetail lobjEmployerDetail in iclbEmployerPayrollDetail)
            {
                if (lobjEmployerDetail.ibusPerson == null)
                    lobjEmployerDetail.LoadPerson();

                if (lobjEmployerDetail.ibusPlan == null)
                    lobjEmployerDetail.LoadPlan();

                //Loading the Org ID Provider Object
                lobjEmployerDetail.LoadOrgIdForProviders();

                if (lobjEmployerDetail.icdoEmployerPayrollDetail.provider_id1 != 0)
                {
                    busEmployerPayrollDetail lobjNewDetail1 = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                    lobjNewDetail1.ibusPerson = lobjEmployerDetail.ibusPerson;
                    lobjNewDetail1.istrProviderName = lobjEmployerDetail.ibusProvider1.icdoOrganization.org_name;
                    lobjNewDetail1.idecMonthlyDeduction = lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment1;
                    idecTotalProviderDeductions += lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment1;
                    //PIR - 2231
                    lobjNewDetail1.icdoEmployerPayrollDetail.pay_check_date = lobjEmployerDetail.icdoEmployerPayrollDetail.pay_check_date;
                    _iclbCollectionForPostingDefCompCor.Add(lobjNewDetail1);
                }
                if (lobjEmployerDetail.icdoEmployerPayrollDetail.provider_id2 != 0)
                {
                    busEmployerPayrollDetail lobjNewDetail2 = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                    lobjNewDetail2.ibusPerson = lobjEmployerDetail.ibusPerson;
                    lobjNewDetail2.istrProviderName = lobjEmployerDetail.ibusProvider2.icdoOrganization.org_name;
                    lobjNewDetail2.idecMonthlyDeduction = lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment2;
                    idecTotalProviderDeductions += lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment2;
                    //PIR - 2231
                    lobjNewDetail2.icdoEmployerPayrollDetail.pay_check_date = lobjEmployerDetail.icdoEmployerPayrollDetail.pay_check_date;
                    _iclbCollectionForPostingDefCompCor.Add(lobjNewDetail2);
                }
                if (lobjEmployerDetail.icdoEmployerPayrollDetail.provider_id3 != 0)
                {
                    busEmployerPayrollDetail lobjNewDetail3 = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                    lobjNewDetail3.ibusPerson = lobjEmployerDetail.ibusPerson;
                    lobjNewDetail3.istrProviderName = lobjEmployerDetail.ibusProvider3.icdoOrganization.org_name;
                    lobjNewDetail3.idecMonthlyDeduction = lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment3;
                    idecTotalProviderDeductions += lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment3;
                    //PIR - 2231
                    lobjNewDetail3.icdoEmployerPayrollDetail.pay_check_date = lobjEmployerDetail.icdoEmployerPayrollDetail.pay_check_date;
                    _iclbCollectionForPostingDefCompCor.Add(lobjNewDetail3);
                }
                if (lobjEmployerDetail.icdoEmployerPayrollDetail.provider_id4 != 0)
                {
                    busEmployerPayrollDetail lobjNewDetail4 = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                    lobjNewDetail4.ibusPerson = lobjEmployerDetail.ibusPerson;
                    lobjNewDetail4.istrProviderName = lobjEmployerDetail.ibusProvider4.icdoOrganization.org_name;
                    lobjNewDetail4.idecMonthlyDeduction = lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment4;
                    idecTotalProviderDeductions += lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment4;
                    //PIR - 2231
                    lobjNewDetail4.icdoEmployerPayrollDetail.pay_check_date = lobjEmployerDetail.icdoEmployerPayrollDetail.pay_check_date;
                    _iclbCollectionForPostingDefCompCor.Add(lobjNewDetail4);
                }
                if (lobjEmployerDetail.icdoEmployerPayrollDetail.provider_id5 != 0)
                {
                    busEmployerPayrollDetail lobjNewDetail5 = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                    lobjNewDetail5.ibusPerson = lobjEmployerDetail.ibusPerson;
                    lobjNewDetail5.istrProviderName = lobjEmployerDetail.ibusProvider5.icdoOrganization.org_name;
                    lobjNewDetail5.idecMonthlyDeduction = lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment5;
                    idecTotalProviderDeductions += lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment5;
                    //PIR - 2231
                    lobjNewDetail5.icdoEmployerPayrollDetail.pay_check_date = lobjEmployerDetail.icdoEmployerPayrollDetail.pay_check_date;
                    _iclbCollectionForPostingDefCompCor.Add(lobjNewDetail5);
                }
                if (lobjEmployerDetail.icdoEmployerPayrollDetail.provider_id6 != 0)
                {
                    busEmployerPayrollDetail lobjNewDetail6 = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                    lobjNewDetail6.ibusPerson = lobjEmployerDetail.ibusPerson;
                    lobjNewDetail6.istrProviderName = lobjEmployerDetail.ibusProvider6.icdoOrganization.org_name;
                    lobjNewDetail6.idecMonthlyDeduction = lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment6;
                    idecTotalProviderDeductions += lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment6;
                    //PIR - 2231
                    lobjNewDetail6.icdoEmployerPayrollDetail.pay_check_date = lobjEmployerDetail.icdoEmployerPayrollDetail.pay_check_date;
                    _iclbCollectionForPostingDefCompCor.Add(lobjNewDetail6);
                }
                if (lobjEmployerDetail.icdoEmployerPayrollDetail.provider_id7 != 0)
                {
                    busEmployerPayrollDetail lobjNewDetail7 = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                    lobjNewDetail7.ibusPerson = lobjEmployerDetail.ibusPerson;
                    lobjNewDetail7.istrProviderName = lobjEmployerDetail.ibusProvider7.icdoOrganization.org_name;
                    lobjNewDetail7.idecMonthlyDeduction = lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment7;
                    idecTotalProviderDeductions += lobjEmployerDetail.icdoEmployerPayrollDetail.amount_from_enrollment7;
                    //PIR - 2231
                    lobjNewDetail7.icdoEmployerPayrollDetail.pay_check_date = lobjEmployerDetail.icdoEmployerPayrollDetail.pay_check_date;
                    _iclbCollectionForPostingDefCompCor.Add(lobjNewDetail7);
                }
            }
        }
        // System out of Memory Exception- code optmized
        public void LoadLifeSplitUpPremiumAmounts()
        {
            DataTable ldtLifeSplitUpPremiumAmounts = Select("cdoEmployerPayrollDetail.LoadLifeSplitUpPremiumAmounts",new object[1] {icdoEmployerPayrollHeader.employer_payroll_header_id});
            if (ldtLifeSplitUpPremiumAmounts.Rows.Count > 0)
            {
                _idecTotalSpousePremium = Convert.ToDecimal(ldtLifeSplitUpPremiumAmounts.Rows[0]["SpouseSum"]);
                _idecTotalSupplementPremium = Convert.ToDecimal(ldtLifeSplitUpPremiumAmounts.Rows[0]["SupplementalSum"]);
                _idecTotalDependentPremium = Convert.ToDecimal(ldtLifeSplitUpPremiumAmounts.Rows[0]["DependentSum"]);
                _idecTotalBasicPremium = Convert.ToDecimal(ldtLifeSplitUpPremiumAmounts.Rows[0]["BasicSum"]);
            }
        }
        //used to create collection for correspondence after posting for header type insurance
        public void LoadOtherInsuranceObjectsForCorr()
        {
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();

            _iclbCollectionForPostingInsuranceCor = new Collection<busEmployerPayrollDetail>();
            foreach (busEmployerPayrollDetail lobjEmployerDetail in iclbEmployerPayrollDetail)
            {
                if (lobjEmployerDetail.ibusPerson == null)
                    lobjEmployerDetail.LoadPerson();

                if (lobjEmployerDetail.ibusPlan == null)
                    lobjEmployerDetail.LoadPlan();

                busEmployerPayrollDetail lobjNewEmployerPayrollDetail = CheckPersonExistsInCollection(lobjEmployerDetail.icdoEmployerPayrollDetail.person_id);

                if (lobjNewEmployerPayrollDetail != null)
                {
                    SetPremiumAmountByPlan(lobjEmployerDetail, lobjNewEmployerPayrollDetail);
                }
                else
                {
                    SetPremiumAmountByPlan(lobjEmployerDetail, lobjEmployerDetail);
                    _iclbCollectionForPostingInsuranceCor.Add(lobjEmployerDetail);
                }
            }

            //Update the Total Amount Properties
            foreach (busEmployerPayrollDetail lbusEmployerPayrollDetail in _iclbCollectionForPostingInsuranceCor)
            {
                if (lbusEmployerPayrollDetail.idecHealthInsPremium != 0)
                    idecTotalHealthInsPremium += lbusEmployerPayrollDetail.idecHealthInsPremium;

                if (lbusEmployerPayrollDetail.idecDentalInsPremium != 0)
                    idecTotalDentalInsPremium += lbusEmployerPayrollDetail.idecDentalInsPremium;

                if (lbusEmployerPayrollDetail.idecLTCInsPremium != 0)
                    idecTotalLTCInsPremium += lbusEmployerPayrollDetail.idecLTCInsPremium;

                if (lbusEmployerPayrollDetail.idecVisionInsPremium != 0)
                    idecTotalVisionInsPremium += lbusEmployerPayrollDetail.idecVisionInsPremium;

                if (lbusEmployerPayrollDetail.idecBasicPremium != 0)
                {
                    idecTotalBasicPremium += lbusEmployerPayrollDetail.idecBasicPremium;
                    idecTotalSpousePremium += lbusEmployerPayrollDetail.idecSpousePremium;
                    idecTotalSupplementPremium += lbusEmployerPayrollDetail.idecSupplementPremium;
                    idecTotalDependentPremium += lbusEmployerPayrollDetail.idecDependentPremium;
                }
            }
        }

        private static void SetPremiumAmountByPlan(busEmployerPayrollDetail abusSourceEmpPayDetail, busEmployerPayrollDetail abusDestinationEmpPayDetail)
        {
            int lintPlanID = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.plan_id;
            if (lintPlanID == busConstant.PlanIdGroupHealth)
            {
                abusDestinationEmpPayDetail.idecHealthInsPremium = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.premium_amount_from_enrollment;
            }
            if (lintPlanID == busConstant.PlanIdDental)
            {
                abusDestinationEmpPayDetail.idecDentalInsPremium = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.premium_amount_from_enrollment;
            }
            if (lintPlanID == busConstant.PlanIdLTC)
            {
                abusDestinationEmpPayDetail.idecLTCInsPremium = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.premium_amount_from_enrollment;
            }
            if (lintPlanID == busConstant.PlanIdVision)
            {
                abusDestinationEmpPayDetail.idecVisionInsPremium = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.premium_amount_from_enrollment;
            }
            if (lintPlanID == busConstant.PlanIdGroupLife)
            {
                abusDestinationEmpPayDetail.idecBasicPremium = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.basic_premium;
                abusDestinationEmpPayDetail.idecInsAge = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.life_ins_age;
                abusDestinationEmpPayDetail.idecSpousePremium = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.spouse_premium;
                abusDestinationEmpPayDetail.idecSupplementPremium = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.supplemental_premium;
                abusDestinationEmpPayDetail.idecDependentPremium = abusSourceEmpPayDetail.icdoEmployerPayrollDetail.dependent_premium;
            }
        }

        //this method is used to check if the person already exists in the collection
        private busEmployerPayrollDetail CheckPersonExistsInCollection(int aintPersonId)
        {
            foreach (busEmployerPayrollDetail lobjEmployerDetail in _iclbCollectionForPostingInsuranceCor)
            {
                if (lobjEmployerDetail.icdoEmployerPayrollDetail.person_id == aintPersonId)
                {
                    return lobjEmployerDetail;
                }
            }
            return null;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            //Loading the Correpondence Related Objects. In new Framework, Pune Team will give the Virtual Method to Load this.
            //Loading Other Objects
            _iclbCollectionForPostingDefCompCor = new Collection<busEmployerPayrollDetail>();
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                LoadOtherDefCompObjectsForRetirement();
            }
            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                LoadOtherInsuranceObjectsForCorr();
            }
            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                LoadOtherDefCompObjectsForCor();
            }
        }

        public bool IsDetailEntryExists()
        {
            bool lblnResult = false;
            if ((iclbEmployerPayrollDetail != null) && (iclbEmployerPayrollDetail.Count > 0))
            {
                lblnResult = true;
            }
            if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count() == 0 && _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp && (iblnFromFile || iblnIsFromESS))
            {
                lblnResult = true;
            }

            return lblnResult;
        }

        public bool IsDefCompPostedDetailExists() 
        {
            bool lblnResult = false;

            if (iclbEmployerPayrollDetail != null && iclbEmployerPayrollDetail.Count() == 0 && _icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusPosted  && _icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp && (iblnFromFile || iblnIsFromESS))
            {
                lblnResult = true;
            }

            return lblnResult;
        }

        public bool IsDetailEntryExistsInReviewPendingStatus()
        {
            bool lblnResult = false;
            if ((iclbEmployerPayrollDetail != null) && (iclbEmployerPayrollDetail.Count > 0))
            {
                foreach (var lbusEmpPayrollDetail in iclbEmployerPayrollDetail)
                {
                    if ((lbusEmpPayrollDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusReview) ||
                       (lbusEmpPayrollDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPending))
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        public bool IsDetailEntryExistsInReviewStatus()
        {
            bool lblnResult = false;
            if ((iclbEmployerPayrollDetail != null) && (iclbEmployerPayrollDetail.Count > 0))
            {
                foreach (var lbusEmpPayrollDetail in iclbEmployerPayrollDetail)
                {
                    if (lbusEmpPayrollDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusReview)
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        public bool IsDetailEntryExistsInValidStatus()
        {
            bool lblnResult = false;
            if ((iclbEmployerPayrollDetail != null) && (iclbEmployerPayrollDetail.Count > 0))
            {
                foreach (var lbusEmpPayrollDetail in iclbEmployerPayrollDetail)
                {
                    if (lbusEmpPayrollDetail.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusValid)
                    {
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// Method to load Remittance allocation based on remittance id
        /// </summary>
        /// <param name="aintRemittanceID">remittance id</param>
        public void LoadEmployerRemittanceAllocationByRemittanceID(int aintRemittanceID)
        {
            DataTable ldtbList = Select<cdoEmployerRemittanceAllocation>(
                new string[1] { "remittance_id" },
                new object[1] { aintRemittanceID }, null, null);
            _iclbEmployerRemittanceAllocation = GetCollection<busEmployerRemittanceAllocation>(ldtbList, "icdoEmployerRemittanceAllocation");
        }

        #region UCS - 079

        /// <summary>
        /// Method to check conditions for Recalculate benefit workflow
        /// </summary>
        /// <param name="aobjEmpDtl">bus. Employer Payroll Detail object</param>
        private void CheckConditionsForWorkflow(busEmployerPayrollDetail aobjEmpDtl)
        {
            //Loading the person
            if (aobjEmpDtl.ibusPerson == null)
                aobjEmpDtl.LoadPerson();
            //Loading member payee account
            aobjEmpDtl.ibusPerson.LoadMemberPayeeAccount();
            if (aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount != null && aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
            {
                //aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.LoadPaymentHistoryHeader();
                //loading active payee account status
                aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                //laoding the data 2 for PAS
                DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203,
                    aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
                aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                    ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : string.Empty;
                //checking whether PAS is in Receiving or Approved
                if (aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved
                    || aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReceiving
                    || (aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview))
                {
                    InitiateWorkflow(aobjEmpDtl.icdoEmployerPayrollDetail.person_id, 0, aobjEmpDtl.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
                        busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
                }
            }
        }

        /// <summary>
        /// method to initiate workflow
        /// </summary>
        /// <param name="aintPersonID">Person ID</param>
        /// <param name="aintPayeeAccountID">Payee Account ID</param>
        public void InitiateWorkflow(int aintPersonID, int aintBeneficiaryID, int aintReferenceID, int aintTypeID)
        {
            if (!busWorkflowHelper.IsActiveInstanceAvailable(aintPersonID, aintTypeID))
            {
                //only for Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow, beneficiary id to be posted
                Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                ldctParams["additional_parameter1"] = aintBeneficiaryID > 0 ? aintBeneficiaryID.ToString() : "";
                busWorkflowHelper.InitiateBpmRequest(aintTypeID, aintPersonID, 0, aintReferenceID, iobjPassInfo, busConstant.WorkflowProcessSource_Batch, adictInstanceParameters: ldctParams);

            }
        }

        #endregion
        //propert to contain collection of person account employment detail
        public Collection<busPersonAccountEmploymentDetail> iclbPersonAccountEmploymentDetail { get; set; }
        /// <summary>
        /// method to load person account employment detail
        /// </summary>
        /// <param name="abusPersonEmploymentDetail">person employment detail bus. object</param>
        private void LoadPeronAccountEmployment(busPersonEmploymentDetail abusPersonEmploymentDetail)
        {
            DataTable ldtPersonAccountEmpDetail = Select<cdoPersonAccountEmploymentDetail>
                (new string[1] { enmPersonAccountEmploymentDetail.person_employment_dtl_id.ToString() },
                new object[1] { abusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id }, null, null);
            iclbPersonAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(ldtPersonAccountEmpDetail, "icdoPersonAccountEmploymentDetail");
            foreach (busPersonAccountEmploymentDetail lobjPersonAcctEmpDetail in iclbPersonAccountEmploymentDetail)
                lobjPersonAcctEmpDetail.LoadPersonAccount();
        }
        /// <summary}
        /// method to load person account and initiate workflow
        /// </summary>
        /// <param name="abusPersonEmploymentDetail">person employment detail bus. object</param>
        //public void GetPersonAccountAndInitiateWorkflow(busPersonEmploymentDetail abusPersonEmploymentDetail, busEmployerPayrollDetail abusEmployerPayrollDetail)
        //{
        //    LoadPeronAccountEmployment(abusPersonEmploymentDetail);
        //    busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail = iclbPersonAccountEmploymentDetail.Where
        //                                        (o => o.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled
        //                                            /*&& (o.icdoPersonAccountEmploymentDetail.plan_id == 1
        //                                            || o.icdoPersonAccountEmploymentDetail.plan_id == 2
        //                                            || o.icdoPersonAccountEmploymentDetail.plan_id == 3
        //                                            || o.icdoPersonAccountEmploymentDetail.plan_id == 20)*/ //uat pir 1976 : as per satya, need not check the plans
        //                                            && abusEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id == o.icdoPersonAccountEmploymentDetail.person_account_id  //PIR 11946
        //                                            && (o.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired
        //                                            || o.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn
        //                                             || o.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
        //                                            || o.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended
        //                                            || o.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusPreRetirementDeath
        //                                            || o.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC  //PIR 11946
        //                                            || o.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementTransferredTIAACREF  //PIR 11946
        //                                            || o.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferToTFFR)  //PIR 11946
        //                                        ).FirstOrDefault();
        //    if (lobjPersonAccountEmploymentDetail != null)
        //    {
        //        if (lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn)
        //        {
        //            InitiateWorkflow(lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.person_id, 0, 0, busConstant.Map_Process_Remider_Refund);
        //        }
        //        else if (lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusPreRetirementDeath)
        //        {
        //            CheckConditionForPreRetirementWorkflow(lobjPersonAccountEmploymentDetail, lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value);
        //        }
        //        //PIR 11946
        //        else if (lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC
        //            || lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementTransferredTIAACREF
        //            || lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferToTFFR)
        //        {
        //            CheckConditionsForTransferWorkflow(lobjPersonAccountEmploymentDetail, lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value);
        //        }
        //        else
        //        {
        //            //UCS - 079 : Initiating Recalculate Pension and RHIC Benefit workflow
        //            CheckConditionsForWorkflow(lobjPersonAccountEmploymentDetail, lobjPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value);
        //        }
        //    }
        //}

        //PIR 11946
        //public void CheckConditionsForTransferWorkflow(busPersonAccountEmploymentDetail aobjPersonAccountEmpDtl, string astrPlanParticipationStatusValue)
        //{
        //    if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson == null)
        //        aobjPersonAccountEmpDtl.ibusPersonAccount.LoadPerson();

        //    busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson { person_id = aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_id } };
        //    lbusPerson.LoadPersonAccount(false);
        //    busPersonAccount lbusPersonAccount = lbusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdDC
        //        && (i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
        //            || i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
        //        ).FirstOrDefault();

        //    //Person should have plan id = 7 with plan participation status = 'enroll' or 'suspended' then only initiate workflow
        //    if (lbusPersonAccount.IsNotNull() || astrPlanParticipationStatusValue == busConstant.PlanParticipationStatusRetirementTransferredTIAACREF
        //        || astrPlanParticipationStatusValue == busConstant.PlanParticipationStatusTransferToTFFR)
        //    {
        //        aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.LoadMemberPayeeAccountTransferFromDB(aobjPersonAccountEmpDtl.ibusPersonAccount);

        //        //checking whether PAS is in processed or Approved or review
        //        if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusApproved
        //            || aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusRefundProcessed
        //            || aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusReview)
        //        {
        //            if (!busWorkflowHelper.IsActiveInstanceAvailable(aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_id, busConstant.Map_Process_Remainder_Transfer_Refund))
        //            {
        //                InitiateWorkflow(aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.icdoPerson.person_id, 0,
        //                                        aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
        //                                        busConstant.Map_Process_Remainder_Transfer_Refund);
        //            }
        //        }
        //    }
        //}



        /// <summary>
        /// Method to check conditions and initiate pre retirement workflow if additional contribution come in
        /// </summary>
        /// <param name="aobjPersonAccountEmpDtl">person acct. empl. detail object</param>
        /// <param name="astrPlanParticipationStatusValue">plan participation value</param>
        //private void CheckConditionForPreRetirementWorkflow(busPersonAccountEmploymentDetail aobjPersonAccountEmpDtl, string astrPlanParticipationStatusValue)
        //{
        //    //Loading the person
        //    if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson == null)
        //        aobjPersonAccountEmpDtl.ibusPersonAccount.LoadPerson();
        //    if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.iclbBenefitApplication == null)
        //        aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.LoadBenefitApplication();

        //    busBenefitApplication lobjPreRetrApplication = aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.iclbBenefitApplication
        //                                                        .Where(o => o.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath &&
        //                                                            o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied &&
        //                                                            o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDeferred &&
        //                                                            o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled)
        //                                                        .FirstOrDefault();
        //    if (lobjPreRetrApplication != null)
        //    {
        //        if (lobjPreRetrApplication.iclbBenefitApplicationPersonAccounts == null)
        //            lobjPreRetrApplication.LoadBenefitApplicationPersonAccount();
        //        busBenefitApplicationPersonAccount lobjBAPA = lobjPreRetrApplication.iclbBenefitApplicationPersonAccounts
        //                                                        .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes)
        //                                                        .FirstOrDefault();
        //        if (lobjPreRetrApplication != null && lobjBAPA != null &&
        //            lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id == aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_account_id)
        //        {
        //            lobjPreRetrApplication.LoadPayeeAccount();
        //            foreach (busPayeeAccount lobjPayeeAccount in lobjPreRetrApplication.iclbPayeeAccount)
        //            {
        //                //lobjPayeeAccount.LoadPaymentHistoryHeader();
        //                lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
        //                //laoding the data 2 for PAS
        //                DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
        //                lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 = ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString()
        //                                                                                                                                    : string.Empty;

        //                if ((lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved &&
        //                aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
        //                || lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReceiving
        //                || (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview))
        //                {
        //                    if (!busWorkflowHelper.IsActiveInstanceAvailable(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id,
        //                        busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow))
        //                    {
        //                        InitiateWorkflow(lobjPreRetrApplication.icdoBenefitApplication.member_person_id, lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, lobjPayeeAccount.icdoPayeeAccount.application_id,
        //                            busConstant.Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow);
        //                        lobjPayeeAccount.CreateReviewPayeeAccountStatus();
        //                        lobjPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
        //                        lobjPayeeAccount.ValidateSoftErrors();
        //                        lobjPayeeAccount.UpdateValidateStatus();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Method to check conditions for Recalculate benefit workflow
        /// </summary>
        /// <param name="aobjEmpDtl">bus. Person account employment Detail object</param>
        //private void CheckConditionsForWorkflow(busPersonAccountEmploymentDetail aobjPersonAccountEmpDtl, string astrPlanParticipationStatusValue)
        //{
        //    //Loading the person
        //    if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson == null)
        //        aobjPersonAccountEmpDtl.ibusPersonAccount.LoadPerson();
        //    //Loading member payee account
        //    aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.LoadMemberPayeeAccount();
        //    if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount != null &&
        //        aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
        //    {
        //        if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusApplication == null)
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.LoadApplication();
        //        if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts == null)
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
        //        busBenefitApplicationPersonAccount lobjBAPA = aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts
        //            .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();
        //        if (lobjBAPA != null && lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id == aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_account_id)
        //        {
        //            //aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.LoadPaymentHistoryHeader();
        //            //loading active payee account status
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
        //            //laoding the data 2 for PAS
        //            DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203,
        //                aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
        //                ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : string.Empty;
        //            //checking whether PAS is in Receiving or Approved
        //            if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeRefund &&
        //                ((aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved &&
        //                    (aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
        //                    aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)) // PROD PIR 8312
        //                || aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReceiving
        //                || (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview)))
        //            {
        //                if (!busWorkflowHelper.IsActiveInstanceAvailable(aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_id, busConstant.Map_Recalculate_Pension_and_RHIC_Benefit))
        //                {
        //                    InitiateWorkflow(aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.icdoPerson.person_id, 0,
        //                                        aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id,
        //                                        busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
        //                    aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.CreateReviewPayeeAccountStatus();
        //                    aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
        //                    aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.ValidateSoftErrors();
        //                    aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberPayeeAccount.UpdateValidateStatus();
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        DataTable ldtbFinalDisOrRetCalculationsExcCanceled = busBase.Select("cdoPersonAccountAdjustment.GetRetOrDisFinalCalculations",
        //                                                                new object[1] { aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.icdoPerson.person_id });

        //        Collection<busBenefitCalculation> lclcFinalDisOrRetCalculationsExcCanceled = GetCollection<busBenefitCalculation>(ldtbFinalDisOrRetCalculationsExcCanceled, "icdoBenefitCalculation");
        //        if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.iclbRetirementAccount.IsNull())
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.LoadRetirementAccount();
        //        if (lclcFinalDisOrRetCalculationsExcCanceled.Count > 0 && !(aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson
        //                                                                            .iclbRetirementAccount
        //                                                                            .Where(i => i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
        //                                                                            .Any()))
        //        {
        //            InitiateWorkflow(aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_id,
        //                    0,
        //                    0,
        //                    busConstant.Map_Recalculate_Pension_and_RHIC_Benefit);
        //        }
        //        else
        //            CheckConditionForPreRetirementWorkflow(aobjPersonAccountEmpDtl, astrPlanParticipationStatusValue);
        //    }
        //    aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.LoadMemberRefundPayeeAccount();
        //    if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount != null &&
        //        aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
        //    {
        //        if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.ibusApplication == null)
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.LoadApplication();
        //        if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts == null)
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
        //        busBenefitApplicationPersonAccount lobjBAPA = aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts
        //            .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();
        //        if (lobjBAPA != null && lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id == aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_account_id)
        //        {
        //            //loading active payee account status
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
        //            //laoding the data 2 for PAS
        //            DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203,
        //                aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
        //                ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : string.Empty;
        //            //checking whether PAS is in Receiving or Approved
        //            if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund &&
        //                (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved ||
        //                aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.ibusMemberRefundPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview) &&
        //                (aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
        //                 aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)) // PROD PIR 8312
        //            {
        //                if (!busWorkflowHelper.IsActiveInstanceAvailable(aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_id, busConstant.Map_Process_Remider_Refund) &&
        //                    !busWorkflowHelper.IsActiveInstanceAvailable(aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_id, busConstant.Map_Initialize_Process_Refund_Application_And_Calculation))
        //                {
        //                    InitiateWorkflow(aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_id, 0, 0, busConstant.Map_Process_Remider_Refund);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        DataTable ldtbFinalDisOrRetCalculationsExcCanceled = busBase.Select("cdoPersonAccountAdjustment.GetRefundFinalCalculations",
        //                                                                new object[1] { aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.icdoPerson.person_id });

        //        Collection<busBenefitCalculation> lclcFinalDisOrRetCalculationsExcCanceled = GetCollection<busBenefitCalculation>(ldtbFinalDisOrRetCalculationsExcCanceled, "icdoBenefitCalculation");
        //        if (aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.iclbRetirementAccount.IsNull())
        //            aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson.LoadRetirementAccount();
        //        if (lclcFinalDisOrRetCalculationsExcCanceled.Count > 0 && !(aobjPersonAccountEmpDtl.ibusPersonAccount.ibusPerson
        //                                                                            .iclbRetirementAccount
        //                                                                            .Where(i => i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
        //                                                                            .Any()))
        //        {
        //            InitiateWorkflow(aobjPersonAccountEmpDtl.ibusPersonAccount.icdoPersonAccount.person_id,
        //                    0,
        //                    0,
        //                    busConstant.Map_Process_Remider_Refund);
        //        }
        //    }
        //}

        public bool iblnEmployerPayrollNewMode { get; set; }

        public bool IsInsuranceTypeCreatedManually()
        {
            bool lblnResult = false;

            if (iblnEmployerPayrollNewMode && icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr &&
                (icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeRegular || iblnIsFromESS)) //prod pir 6077
            {
                lblnResult = true;
            }

            return lblnResult;
        }

        /// <summary>
        /// method to check whether pay check date is same for header and detail records
        /// </summary>
        /// <returns></returns>
        public bool IsPayCheckDateDifferentForDetail()
        {
            bool lblnResult = false;
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
            foreach (busEmployerPayrollDetail lobjDetail in iclbEmployerPayrollDetail)
            {
                if (icdoEmployerPayrollHeader.pay_check_date != lobjDetail.icdoEmployerPayrollDetail.pay_check_date)
                {
                    lblnResult = true;
                    break;
                }
            }
            return lblnResult;
        }

        #region UCS-32
        public Collection<busOrgBank> iclbOrgbank { get; set; }

        public Collection<busWssDebitAchRequest> iclbWssDebitAchRequest { get; set; }
        public void LoadOrgBank()
        {
            DataTable ldtbOrgbank = Select<cdoOrgBank>(new string[1] { "org_id" }, new object[1] { icdoEmployerPayrollHeader.org_id }, null, null);
            iclbOrgbank = GetCollection<busOrgBank>(ldtbOrgbank, "icdoOrgBank");
        }

        public void LoadWssDebitAchRequest()
        {
            DataTable ldtbWssDebitAchRequest = Select<cdoWssDebitAchRequest>(new string[1] { "employer_payroll_header_id" },
                                                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id }, null, null);
            iclbWssDebitAchRequest = GetCollection<busWssDebitAchRequest>(ldtbWssDebitAchRequest, "icdoWssDebitAchRequest");
        }

        public bool IsbtnDeleteACHRequestButtonVisible()
        {
            if (iblnIsFromESS)
            {
                //prod pir 5764 : need to check debit ach request everytime
                //if (iclbWssDebitAchRequest == null)
                    LoadWssDebitAchRequest();
                if (iclbWssDebitAchRequest.Where(o => o.icdoWssDebitAchRequest.status_value == busConstant.WSS_Debit_ACH_Request_Status_Pending).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsbtnDebitACHRequestVisible()
        {
            if (iblnIsFromESS)
            {
                if (iclbOrgbank == null)
                    LoadOrgBank();
                //prod pir 5764 : need to check debit ach request everytime
                //if (iclbWssDebitAchRequest == null)
                    LoadWssDebitAchRequest();
                if ((iclbOrgbank.Count > 0 &&
                    iclbOrgbank.Where(o => o.icdoOrgBank.usage_value == busConstant.BankUsageACHWithdrawals &&
                                         o.icdoOrgBank.status_value == busConstant.OrgBankStatusActive).Count() > 0) &&
                    IsbtnRemittanceReportVisible())
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsbtnRemittanceReportVisible()
        {
            bool lblnbtnVisible = false;
            if (iblnIsFromESS)
            {
                //prod pir 5764 : need to check debit ach request everytime
                //if (iclbWssDebitAchRequest == null)
                    LoadWssDebitAchRequest();
                if (iclbWssDebitAchRequest.Any(request=>request.icdoWssDebitAchRequest.status_value == busConstant.StatusPending)) //PIR 14849
                {
                    return false;
                }
                else
                {
                    if (((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt || 
                        icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr) &&
                    icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusPosted && 
                    (icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusNoRemittance ||
                    icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusUnbalanced)) ||
                    ((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases || 
                    (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp && !IsDetailContainOnlyOther457())) && 
                    (icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusValid || 
                    icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusReadyToPost) &&
                    (icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusNoRemittance ||
                    icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusUnbalanced)))
                    {

                        lblnbtnVisible = true;
                    }
                }
            }
            return lblnbtnVisible;
        }

        public bool IsbtnRemittanceReportVisibleWithoutStatus()
        {
            bool lblnbtnVisible = false;
            if (iblnIsFromESS)
            {
                // PIR 9177 - Remittance report button visibility
                if (((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt || icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr) &&
                icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusPosted ) ||
                ((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases || icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp) &&
                icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusValid ))
                {
                    lblnbtnVisible = true;
                }
            }
            return lblnbtnVisible;
        }

        public void btnDebitACHRequest_Click()
        {
            if (IsbtnDebitACHRequestVisible())
            {
                busWssDebitAchRequest lobjWSSDebitAchRequest = new busWssDebitAchRequest { icdoWssDebitAchRequest = new cdoWssDebitAchRequest() };
                lobjWSSDebitAchRequest.icdoWssDebitAchRequest.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
                lobjWSSDebitAchRequest.icdoWssDebitAchRequest.org_bank_id = iclbOrgbank.Where(o => o.icdoOrgBank.usage_value == busConstant.BankUsageACHWithdrawals &&
                                                                            o.icdoOrgBank.status_value == busConstant.OrgBankStatusActive).Select(o => o.icdoOrgBank.org_bank_id).FirstOrDefault();
                lobjWSSDebitAchRequest.icdoWssDebitAchRequest.status_value = busConstant.WSS_Debit_ACH_Request_Status_Pending;
                lobjWSSDebitAchRequest.icdoWssDebitAchRequest.Insert();
                
                //if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                //    InitiateWorkflowForDebitACHRequest(busConstant.Map_ACH_Pull_For_DeferredCompensation);
                //else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                //    InitiateWorkflowForDebitACHRequest(busConstant.Map_ACH_Pull_For_Insurance);
                //else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt || icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                //    InitiateWorkflowForDebitACHRequest(busConstant.Map_ACH_Pull_For_Retirement);
            }
        }
        public ArrayList btnDeleteDebitACHRequest_Click()
        {
            ArrayList larrresult = new ArrayList();
            utlError lobjError = new utlError();
            //prod pir 5764 : need to check debit ach request everytime
            //if (iclbWssDebitAchRequest == null)
                LoadWssDebitAchRequest();
            foreach (busWssDebitAchRequest lbusWssDebitAchRequest in iclbWssDebitAchRequest.Where(o =>
                o.icdoWssDebitAchRequest.status_value == busConstant.WSS_Debit_ACH_Request_Status_Pending))
            {
                lbusWssDebitAchRequest.LoadDebitACHRequestDetail();
                if (lbusWssDebitAchRequest.iclbDebitAchRequestDetail.Count > 0)
                {
                    lobjError = AddError(8513, "");
                    larrresult.Add(lobjError);
                    return larrresult;
                }
                lbusWssDebitAchRequest.icdoWssDebitAchRequest.Delete();
            }
            larrresult.Add(this);
            return larrresult;
        }
        public DataTable idtRemittanceReport { get; set; }

        public byte[] btnGenerateRemittanceReport_Click()
        {
            string istrReportName = string.Empty;
            DataSet lstRemittanceReport = new DataSet();

            idtRemittanceReport = new DataTable();

            DataColumn ldc1 = new DataColumn("org_code", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("org_name", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("plan", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("benefit_type", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("type", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("amount", Type.GetType("System.Decimal"));
            //prod pir 4241,4804,6050
            DataColumn ldc7 = new DataColumn("employer_payroll_header_id", Type.GetType("System.Int32"));
            DataColumn ldc8 = new DataColumn("payroll_paid_month", Type.GetType("System.String"));
            DataColumn ldc9 = new DataColumn("report_type", Type.GetType("System.String"));
            DataColumn ldc10 = new DataColumn("addr_description", Type.GetType("System.String"));
            DataColumn ldc11 = new DataColumn("member_interest", Type.GetType("System.Decimal"));
            DataColumn ldc12 = new DataColumn("employer_interest", Type.GetType("System.Decimal"));
            DataColumn ldc13 = new DataColumn("rhic_interest", Type.GetType("System.Decimal"));
            DataColumn ldc14 = new DataColumn("addr_org_name", Type.GetType("System.String"));
            DataColumn ldc15 = new DataColumn("addr_line1", Type.GetType("System.String"));
            DataColumn ldc16 = new DataColumn("remittance_payment_applied", Type.GetType("System.Decimal"));
            DataColumn ldc17 = new DataColumn("count_of_details_for_report_id_in_posted", Type.GetType("System.Int32"));
            DataColumn ldc18 = new DataColumn("count_of_details_for_report_id_in_valid", Type.GetType("System.Int32"));
            DataColumn ldc19 = new DataColumn("credit_payment_applied", Type.GetType("System.Decimal"));

            idtRemittanceReport.Columns.Add(ldc1);
            idtRemittanceReport.Columns.Add(ldc2);
            idtRemittanceReport.Columns.Add(ldc3);
            idtRemittanceReport.Columns.Add(ldc4);
            idtRemittanceReport.Columns.Add(ldc5);
            idtRemittanceReport.Columns.Add(ldc6);
            //prod pir 4241,4804,6050
            idtRemittanceReport.Columns.Add(ldc7);
            idtRemittanceReport.Columns.Add(ldc8);
            idtRemittanceReport.Columns.Add(ldc9);
            idtRemittanceReport.Columns.Add(ldc10);
            idtRemittanceReport.Columns.Add(ldc11);
            idtRemittanceReport.Columns.Add(ldc12);
            idtRemittanceReport.Columns.Add(ldc13);
            idtRemittanceReport.Columns.Add(ldc14);
            idtRemittanceReport.Columns.Add(ldc15);
            idtRemittanceReport.Columns.Add(ldc16);
            idtRemittanceReport.Columns.Add(ldc17);
            idtRemittanceReport.Columns.Add(ldc18);
            idtRemittanceReport.Columns.Add(ldc19);

            busOrganization lobjOrganization = new busOrganization();
            string lstrOrgCode = busGlobalFunctions.GetData1ByCodeValue(52, busConstant.NDPERSCodeValue, iobjPassInfo);
            string lstrOrgName = string.Empty, lstrOrgAddLine1 = string.Empty, lstrOrgAddLine2 = string.Empty;
            if (!string.IsNullOrEmpty(lstrOrgCode))
            {
                lobjOrganization.FindOrganizationByOrgCode(lstrOrgCode);
                lobjOrganization.LoadOrgPrimaryAddress();
                lstrOrgName = lobjOrganization.icdoOrganization.org_name_caps;
                lstrOrgAddLine1 = lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1.ToUpper();
                lstrOrgAddLine2 = (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2) ?
                    lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2.ToUpper() : string.Empty) + " " +
                    (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.city) ?
                    lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.city.ToUpper() : string.Empty) + " " +
                    (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value) ?
                    lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value.ToUpper() : string.Empty) + " " +
                    (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code) ?
                    lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code.ToUpper() : string.Empty) + 
                    (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code) ?
                    " - " + lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code.ToUpper() : string.Empty);
            }
            idtRemittanceReport.TableName = busConstant.ReportTableName;
            if (iclbEmployerPayrollDetail.IsNull()) LoadEmployerPayrollDetail();
            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                istrReportName = "rptRetirementRemittanceReport.rpt";
                if (iclbRetirementContributionByPlan == null)
                    LoadRetirementContributionByPlan();
                foreach (busEmployerPayrollHeader lobjHeader in iclbRetirementContributionByPlan)
                {
                   //if (lobjHeader.icdoEmployerPayrollHeader == null)  //PIR-19341
                        lobjHeader.FindEmployerPayrollHeader(lobjHeader.iintEmloyerPayrollHeaderID);
                    if (lobjHeader.ibusOrganization == null)
                        lobjHeader.LoadOrganization();
                    //reducing the applied amount as per PROD pir 4111
                    //AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                    //   "RHIC Contribution", lobjHeader.idecTotalReportedRHICContributionByPlan - (lobjHeader.idecTotalRHICEmployerInterestCalculatedByPlan), lobjHeader.ibusOrganization.icdoOrganization.org_code,
                    //   lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                    //   icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, lobjHeader.idecTotalRHICEmployerInterestCalculatedByPlan);
                    //AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                    //    "Retirement Contribution", lobjHeader.idecTotalReportedRetrContributionByPlan - (lobjHeader.idecTotalMemberInterestCalculatedByPlan + lobjHeader.idecTotalEmployerInterestCalculatedByPlan),
                    //    lobjHeader.ibusOrganization.icdoOrganization.org_code, lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                    //    icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2,
                    //    lobjHeader.idecTotalMemberInterestCalculatedByPlan, lobjHeader.idecTotalEmployerInterestCalculatedByPlan, 0.00M);
                    //PIR 15238
					AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                       "RHIC Contribution", (lobjHeader.idecTotalReportedRHICContributionByPlan - lobjHeader.idecTotalRHICEmployerInterestCalculatedByPlan), lobjHeader.ibusOrganization.icdoOrganization.org_code,
                       lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                       icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, lobjHeader.idecTotalRHICEmployerInterestCalculatedByPlan, lobjHeader.idecTotalAppliedRHICContributionByPlan);
                    AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                        "Retirement Contribution", (lobjHeader.idecTotalReportedRetrContributionByPlan - lobjHeader.idecTotalMemberInterestCalculatedByPlan -
                    lobjHeader.idecTotalEmployerInterestCalculatedByPlan),
                        lobjHeader.ibusOrganization.icdoOrganization.org_code, lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                        icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2,
                        lobjHeader.idecTotalMemberInterestCalculatedByPlan, lobjHeader.idecTotalEmployerInterestCalculatedByPlan, 0.00M, lobjHeader.idecTotalAppliedRetrContributionByPlanWithAdec); //PIR 25920 DC 2025 changes
                }
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                istrReportName = "rptServicePurchaseRemittanceReport.rpt";
                if (iclbPurchaseContributionByPlan == null)
                    LoadPurchaseByPlan();
                foreach (busEmployerPayrollHeader lobjHeader in iclbPurchaseContributionByPlan)
                {
                   //if (lobjHeader.icdoEmployerPayrollHeader == null)  //PIR-19341
                        lobjHeader.FindEmployerPayrollHeader(lobjHeader.iintEmloyerPayrollHeaderID);
                    if (lobjHeader.ibusOrganization == null)
                        lobjHeader.LoadOrganization();
                    //reducing the applied amount as per PROD pir 4111
                    AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                      "Purchase Contribution", lobjHeader.idecTotalReportedPurchaseByPlan , lobjHeader.ibusOrganization.icdoOrganization.org_code,
                        lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, string.Empty,
                        icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, 0.00M, lobjHeader.idecTotalAppliedPurchaseByPlan);
                }
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                istrReportName = "rptInsuranceRemittanceReport.rpt";
                if (iclbInsuranceContributionByPlan == null)
                    LoadESSInsurancePremiumByPlan();
                foreach (busEmployerPayrollHeader lobjHeader in iclbInsuranceContributionByPlan)
                {
                    //if (lobjHeader.icdoEmployerPayrollHeader == null)
                        lobjHeader.FindEmployerPayrollHeader(lobjHeader.iintEmloyerPayrollHeaderID);
                    if (lobjHeader.ibusOrganization == null)
                        lobjHeader.LoadOrganization();
                    //reducing the applied amount as per PROD pir 4111
                    AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                      "Insurance Contribution", lobjHeader.idecTotalPremiumAmountByPlan , lobjHeader.ibusOrganization.icdoOrganization.org_code,
                        lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                        icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, 0.00M, lobjHeader.idecTotalAppliedPremiumByPlan);
                }
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                istrReportName = "rptDeferredCompRemittanceReport.rpt";
                if (iclbDeferredCompContributionByPlan == null)
                    LoadESSDeferredCompContributionByPlan();
                foreach (busEmployerPayrollHeader lobjHeader in iclbDeferredCompContributionByPlan)
                {
                    //PROD pir 4064
                    if (lobjHeader.iintPlanID != busConstant.PlanIdOther457)
                    {
                        //if (lobjHeader.icdoEmployerPayrollHeader == null)
                            lobjHeader.FindEmployerPayrollHeader(lobjHeader.iintEmloyerPayrollHeaderID);
                        if (lobjHeader.ibusOrganization == null)
                            lobjHeader.LoadOrganization();
                        //reducing the applied amount as per PROD pir 4111
                        AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                          "Deferred Compensation Contribution", lobjHeader.idecTotalReportedByPlan, lobjHeader.ibusOrganization.icdoOrganization.org_code,
                            lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.pay_period_start_date.ToString(busConstant.DateFormatMonthYear),
                        icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, 0.00M, lobjHeader.idecTotalAppliedContributionByPlan);
                    }
                }
            }
            busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
            lstRemittanceReport.Tables.Add(idtRemittanceReport);
            return lobjNeospinBase.CreateDynamicReport(istrReportName, lstRemittanceReport, string.Empty);
        }

        //FW Upgrade :: PIR 11480 - To open pdf in new tab in IE browser, get a file path only 
        public string btnGenerateRemittanceReport_ClickPath()
        {
            string istrReportName = string.Empty;
            DataSet lstRemittanceReport = new DataSet();

            idtRemittanceReport = new DataTable();

            DataColumn ldc1 = new DataColumn("org_code", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("org_name", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("plan", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("benefit_type", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("type", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("amount", Type.GetType("System.Decimal"));
            //prod pir 4241,4804,6050
            DataColumn ldc7 = new DataColumn("employer_payroll_header_id", Type.GetType("System.Int32"));
            DataColumn ldc8 = new DataColumn("payroll_paid_month", Type.GetType("System.String"));
            DataColumn ldc9 = new DataColumn("report_type", Type.GetType("System.String"));
            DataColumn ldc10 = new DataColumn("addr_description", Type.GetType("System.String"));
            DataColumn ldc11 = new DataColumn("member_interest", Type.GetType("System.Decimal"));
            DataColumn ldc12 = new DataColumn("employer_interest", Type.GetType("System.Decimal"));
            DataColumn ldc13 = new DataColumn("rhic_interest", Type.GetType("System.Decimal"));
            DataColumn ldc14 = new DataColumn("addr_org_name", Type.GetType("System.String"));
            DataColumn ldc15 = new DataColumn("addr_line1", Type.GetType("System.String"));
            DataColumn ldc16 = new DataColumn("remittance_payment_applied", Type.GetType("System.Decimal"));
            DataColumn ldc17 = new DataColumn("count_of_details_for_report_id_in_posted", Type.GetType("System.Int32"));
            DataColumn ldc18 = new DataColumn("count_of_details_for_report_id_in_valid", Type.GetType("System.Int32"));

            idtRemittanceReport.Columns.Add(ldc1);
            idtRemittanceReport.Columns.Add(ldc2);
            idtRemittanceReport.Columns.Add(ldc3);
            idtRemittanceReport.Columns.Add(ldc4);
            idtRemittanceReport.Columns.Add(ldc5);
            idtRemittanceReport.Columns.Add(ldc6);
            //prod pir 4241,4804,6050
            idtRemittanceReport.Columns.Add(ldc7);
            idtRemittanceReport.Columns.Add(ldc8);
            idtRemittanceReport.Columns.Add(ldc9);
            idtRemittanceReport.Columns.Add(ldc10);
            idtRemittanceReport.Columns.Add(ldc11);
            idtRemittanceReport.Columns.Add(ldc12);
            idtRemittanceReport.Columns.Add(ldc13);
            idtRemittanceReport.Columns.Add(ldc14);
            idtRemittanceReport.Columns.Add(ldc15);
            idtRemittanceReport.Columns.Add(ldc16);
            idtRemittanceReport.Columns.Add(ldc17);
            idtRemittanceReport.Columns.Add(ldc18);

            busOrganization lobjOrganization = new busOrganization();
            string lstrOrgCode = busGlobalFunctions.GetData1ByCodeValue(52, busConstant.NDPERSCodeValue, iobjPassInfo);
            string lstrOrgName = string.Empty, lstrOrgAddLine1 = string.Empty, lstrOrgAddLine2 = string.Empty;
            if (!string.IsNullOrEmpty(lstrOrgCode))
            {
                lobjOrganization.FindOrganizationByOrgCode(lstrOrgCode);
                lobjOrganization.LoadOrgPrimaryAddress();
                lstrOrgName = lobjOrganization.icdoOrganization.org_name_caps;
                lstrOrgAddLine1 = lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1.ToUpper();
                lstrOrgAddLine2 = (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2) ?
                    lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2.ToUpper() : string.Empty) + " " +
                    (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.city) ?
                    lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.city.ToUpper() : string.Empty) + " " +
                    (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value) ?
                    lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value.ToUpper() : string.Empty) + " " +
                    (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code) ?
                    lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code.ToUpper() : string.Empty) +
                    (!string.IsNullOrEmpty(lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code) ?
                    " - " + lobjOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code.ToUpper() : string.Empty);
                lstrOrgAddLine2 = lstrOrgAddLine2.Trim();
            }
            idtRemittanceReport.TableName = busConstant.ReportTableName;
            if (iclbEmployerPayrollDetail.IsNull()) LoadEmployerPayrollDetail();
            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                istrReportName = "rptRetirementRemittanceReport.rpt";
                if (iclbRetirementContributionByPlan == null)
                    LoadRetirementContributionByPlan();
                foreach (busEmployerPayrollHeader lobjHeader in iclbRetirementContributionByPlan)
                {
                    //if (lobjHeader.icdoEmployerPayrollHeader == null)  //PIR-19341
                    lobjHeader.FindEmployerPayrollHeader(lobjHeader.iintEmloyerPayrollHeaderID);
                    if (lobjHeader.ibusOrganization == null)
                        lobjHeader.LoadOrganization();
                    //reducing the applied amount as per PROD pir 4111
                    //AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                    //   "RHIC Contribution", lobjHeader.idecTotalReportedRHICContributionByPlan - (lobjHeader.idecTotalRHICEmployerInterestCalculatedByPlan), lobjHeader.ibusOrganization.icdoOrganization.org_code,
                    //   lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                    //   icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, lobjHeader.idecTotalRHICEmployerInterestCalculatedByPlan);
                    //AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                    //    "Retirement Contribution", lobjHeader.idecTotalReportedRetrContributionByPlan - (lobjHeader.idecTotalMemberInterestCalculatedByPlan + lobjHeader.idecTotalEmployerInterestCalculatedByPlan),
                    //    lobjHeader.ibusOrganization.icdoOrganization.org_code, lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                    //    icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2,
                    //    lobjHeader.idecTotalMemberInterestCalculatedByPlan, lobjHeader.idecTotalEmployerInterestCalculatedByPlan, 0.00M);
                    //PIR 15238
                    AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                       "RHIC Contribution", (lobjHeader.idecTotalReportedRHICContributionByPlan - lobjHeader.idecTotalRHICEmployerInterestCalculatedByPlan), lobjHeader.ibusOrganization.icdoOrganization.org_code,
                       lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                       icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, lobjHeader.idecTotalRHICEmployerInterestCalculatedByPlan, lobjHeader.idecTotalAppliedRHICContributionByPlan);
                    AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                        "Retirement Contribution", (lobjHeader.idecTotalReportedRetrContributionByPlan - lobjHeader.idecTotalMemberInterestCalculatedByPlan -
                    lobjHeader.idecTotalEmployerInterestCalculatedByPlan),
                        lobjHeader.ibusOrganization.icdoOrganization.org_code, lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                        icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2,
                        lobjHeader.idecTotalMemberInterestCalculatedByPlan, lobjHeader.idecTotalEmployerInterestCalculatedByPlan, 0.00M, lobjHeader.idecTotalAppliedRetrContributionByPlanWithAdec); //PIR 25920 DC 2025 changes
                }
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                istrReportName = "rptServicePurchaseRemittanceReport.rpt";
                if (iclbPurchaseContributionByPlan == null)
                    LoadPurchaseByPlan();
                foreach (busEmployerPayrollHeader lobjHeader in iclbPurchaseContributionByPlan)
                {
                    //if (lobjHeader.icdoEmployerPayrollHeader == null)  //PIR-19341
                    lobjHeader.FindEmployerPayrollHeader(lobjHeader.iintEmloyerPayrollHeaderID);
                    if (lobjHeader.ibusOrganization == null)
                        lobjHeader.LoadOrganization();
                    //reducing the applied amount as per PROD pir 4111
                    AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                      "Purchase Contribution", lobjHeader.idecTotalReportedPurchaseByPlan, lobjHeader.ibusOrganization.icdoOrganization.org_code,
                        lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, string.Empty,
                        icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, 0.00M, lobjHeader.idecTotalAppliedPurchaseByPlan);
                }
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                istrReportName = "rptInsuranceRemittanceReport.rpt";
                if (iclbInsuranceContributionByPlan == null)
                    LoadESSInsurancePremiumByPlan();
                foreach (busEmployerPayrollHeader lobjHeader in iclbInsuranceContributionByPlan)
                {
                    //if (lobjHeader.icdoEmployerPayrollHeader == null)
                        lobjHeader.FindEmployerPayrollHeader(lobjHeader.iintEmloyerPayrollHeaderID);
                    if (lobjHeader.ibusOrganization == null)
                        lobjHeader.LoadOrganization();
                    //reducing the applied amount as per PROD pir 4111
                    AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                      "Insurance Contribution", lobjHeader.idecTotalPremiumAmountByPlan, lobjHeader.ibusOrganization.icdoOrganization.org_code,
                        lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.payroll_paid_date.ToString(busConstant.DateFormatMonthYear),
                        icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, 0.00M, lobjHeader.idecTotalAppliedPremiumByPlan);
                }
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                istrReportName = "rptDeferredCompRemittanceReport.rpt";
                if (iclbDeferredCompContributionByPlan == null)
                    LoadESSDeferredCompContributionByPlan();
                foreach (busEmployerPayrollHeader lobjHeader in iclbDeferredCompContributionByPlan)
                {
                    //PROD pir 4064
                    if (lobjHeader.iintPlanID != busConstant.PlanIdOther457)
                    {
                        //if (lobjHeader.icdoEmployerPayrollHeader == null) //F/W upgrade UAT PIR 12681
                            lobjHeader.FindEmployerPayrollHeader(lobjHeader.iintEmloyerPayrollHeaderID);
                        if (lobjHeader.ibusOrganization == null)
                            lobjHeader.LoadOrganization();
                        //reducing the applied amount as per PROD pir 4111
                        AddRemittanceReportRow(icdoEmployerPayrollHeader.header_type_description, lobjHeader.ibusPlan.icdoPlan.plan_name,
                          "Deferred Compensation Contribution", lobjHeader.idecTotalReportedByPlan, lobjHeader.ibusOrganization.icdoOrganization.org_code,
                            lobjHeader.ibusOrganization.icdoOrganization.org_name, icdoEmployerPayrollHeader.employer_payroll_header_id, icdoEmployerPayrollHeader.pay_period_start_date.ToString(busConstant.DateFormatMonthYear),
                        icdoEmployerPayrollHeader.report_type_description, lstrOrgName, lstrOrgAddLine1, lstrOrgAddLine2, 0.00M, 0.00M, 0.00M, lobjHeader.idecTotalAppliedContributionByPlan);
                    }
                }
            }
            busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
            lstRemittanceReport.Tables.Add(idtRemittanceReport);
            return lobjNeospinBase.CreateDynamicReportPath(istrReportName, lstRemittanceReport, string.Empty);
        }


        private void AddRemittanceReportRow(string astrBenefitType, string astrPlan, string astrType, decimal adecAmount, string astrOrgCode, string astrOrgName,
            int aintEmployerPayrollHeaderID, string astrPayrollPaidMonthYear, string astrReportType, string astrOrgAddressName, string astrOrgAddressLine1, string astrOrgAddressLine2, 
            decimal adecMemberInterest, decimal adecEmployerInterest, decimal adecRHICInterest, decimal adeTotalAppliedContributionByPlan)
        {
            decimal adecRemittancePaymentApplied = 0;
            DataTable ldtbRemittancePaymentApplied = Select("cdoEmployerPayrollHeader.LoadRemittancePaymentApplied",
                                                              new object[2] { aintEmployerPayrollHeaderID, busConstant.RemittanceAllocationStatusAllocated });
            if (ldtbRemittancePaymentApplied.Rows.Count > 0)
            {
                 adecRemittancePaymentApplied = Convert.ToDecimal(ldtbRemittancePaymentApplied.Rows[0]["PAYMENT_APPLIED"]);
            }
            DataRow dr = idtRemittanceReport.NewRow();
            dr["plan"] = astrPlan;
            dr["benefit_type"] = astrBenefitType;
            dr["amount"] = adecAmount;
            dr["type"] = astrType;
            dr["org_code"] = astrOrgCode;
            dr["org_name"] = astrOrgName;
            dr["employer_payroll_header_id"] = aintEmployerPayrollHeaderID;
            dr["payroll_paid_month"] = astrPayrollPaidMonthYear;
            dr["report_type"] = astrReportType;
            dr["addr_org_name"] = astrOrgAddressName;
            dr["addr_line1"] = astrOrgAddressLine1;
            dr["addr_description"] = astrOrgAddressLine2;
            dr["member_interest"] = adecMemberInterest;
            dr["employer_interest"] = adecEmployerInterest;
            dr["rhic_interest"] = adecRHICInterest;
            dr["remittance_payment_applied"] = adecRemittancePaymentApplied;
            dr["count_of_details_for_report_id_in_posted"] = iclbEmployerPayrollDetail.Count(i => i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPosted);
            dr["count_of_details_for_report_id_in_valid"] = iclbEmployerPayrollDetail.Count(i => i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusValid);
            dr["credit_payment_applied"] = adeTotalAppliedContributionByPlan;
            idtRemittanceReport.Rows.Add(dr);
        }
        public Collection<busOrgContact> iclbOrgContact { get; set; }
        public void LoadOrgContact()
        {
            DataTable ldtbOrgContact = Select<cdoOrgContact>(new string[2] { "org_id", "contact_id" },
                new object[2] { icdoEmployerPayrollHeader.org_id, icdoEmployerPayrollHeader.contact_id }, null, null);
            iclbOrgContact = GetCollection<busOrgContact>(ldtbOrgContact, "icdoOrgContact");
        }

        /// <summary>
        /// method to initiate workflow
        /// </summary>
        /// <param name="aintPersonID">Type ID</param>

        public void InitiateWorkflowForDebitACHRequest(int aintTypeID)
        {
            //prod pir 6877 : need to initiate single workflow for ach pulls
            // DataTable ldtRunningInstance = new DataTable();
            //PIR 19345 - //Load Unprocessed Workflow Requests For the passed in process ID
            //ldtRunningInstance = busWorkflowHelper.LoadUnProcessedWorkflowRequestsByOrgAndProcess(aintTypeID, busConstant.DebitACHRequestWorkflowOrgCode);
            //if (ldtRunningInstance.IsNotNull() && ldtRunningInstance.Rows.Count == 0)
            //{
            int lintFirstActivityID = (aintTypeID == busConstant.Map_ACH_Pull_For_DeferredCompensation) ? busConstant.ACH_Pull_For_DeferredCompensation_FirstActivityID
                                        : (aintTypeID == busConstant.Map_ACH_Pull_For_Insurance) ? busConstant.ACH_Pull_For_Insurance_FirstActivityID 
                                        : busConstant.ACH_Pull_For_Retirement_FirstActivityID;

            if (busWorkflowHelper.IsWorkflowRequestNotProcessedForProcessID(aintTypeID) &&
            busWorkflowHelper.IsActiveInstanceNotAvailableForFirstActivityofProcess(lintFirstActivityID))
            {
                
                Dictionary<string, object> ldctParams = new Dictionary<string, object>();
                ldctParams["org_code"] = busConstant.DebitACHRequestWorkflowOrgCode;
                ldctParams["additional_parameter1"] = icdoEmployerPayrollHeader.employer_payroll_header_id;
                busWorkflowHelper.InitiateBpmRequest(aintTypeID, 0,0,0,iobjPassInfo, busConstant.WorkflowProcessSource_Batch, adictInstanceParameters: ldctParams);
            }
            //}
        }

        /// <summary>
        /// method to publish message
        /// </summary>
        private void PublishMessageBoard()
        {
            if (ibusOrganization == null)
                LoadOrganization();
            ibusOrganization.LoadOrgContactByRoleAndPlan(busConstant.OrgContactRolePrimaryAuthorizedAgent, 0);
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
            //If there is no Primary Auth Agent, we dont post it in Message Board.
            string lstrPrioityValue = string.Empty;
            if (ibusOrganization.ibusContact != null)
            {
                //PROD PIR 4803 : report id added to message
                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(13, iobjPassInfo, ref lstrPrioityValue),
                           icdoEmployerPayrollHeader.header_type_description, icdoEmployerPayrollHeader.employer_payroll_header_id.ToString()), lstrPrioityValue, aintOrgID: icdoEmployerPayrollHeader.org_id,
                           aintContactID: ibusOrganization.ibusContact.icdoContact.contact_id);
                if (icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusNoRemittance)
                {
                    if (icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        //prod pir 4106 : dont post below message if detail contain only other 457 plan
                        if (iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457).Any())
                        {
                            //PROD PIR 4803 : report id added to message

                            busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(11, iobjPassInfo, ref lstrPrioityValue),
                           icdoEmployerPayrollHeader.header_type_description, icdoEmployerPayrollHeader.employer_payroll_header_id.ToString()), lstrPrioityValue, aintOrgID: icdoEmployerPayrollHeader.org_id,
                                aintContactID: ibusOrganization.ibusContact.icdoContact.contact_id);
                        }
                    }
                    else
                    {
                        //PROD PIR 4803 : report id added to message
                        busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(12, iobjPassInfo, ref lstrPrioityValue),
                            icdoEmployerPayrollHeader.header_type_description, icdoEmployerPayrollHeader.employer_payroll_header_id.ToString()), busConstant.WSSMessagePriorityHigh, aintOrgID: icdoEmployerPayrollHeader.org_id,
                                aintContactID: ibusOrganization.ibusContact.icdoContact.contact_id);
                    }
                }
            }
        }

        public void LoadDetailErrorFromESS()
        {
            //DataTable ldtbList = Select("cdoEmployerPayrollHeader.ESSErrorSummaryById", new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            //_iclbDetailError = new Collection<busErrorSummary>();
            //foreach (DataRow drList in ldtbList.Rows)
            //{
            //    busErrorSummary lobjErrors = new busErrorSummary();
            //    sqlFunction.LoadQueryResult(lobjErrors, drList, iobjPassInfo.isrvDBCache);
            //    _iclbDetailError.Add(lobjErrors);
            //}
            iclbESSError = new Collection<busEmployerPayrollDetailError>();
            DataTable ldtbList = Select("cdoEmployerPayrollHeader.GetErrorOnESS",
                       new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            iclbESSError = GetCollection<busEmployerPayrollDetailError>(ldtbList, "icdoEmployerPayrollDetailError");
        }

        public byte[] GenerateCSVFromCollection()
        {

            //PIR - 13205  -- changing column titles, order as per the grid view excel

            ArrayList larrResult = new ArrayList();
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
            iclbEmployerPayrollDetail = iclbEmployerPayrollDetail.OrderByDescending(x => x.icdoEmployerPayrollDetail.employer_payroll_detail_id).ToList().ToCollection();
            StringBuilder sb = new StringBuilder();
            //Adding the Header

            sb.Append("Detail ID");
            sb.Append(",");
            sb.Append("Report ID");
            sb.Append(",");
            sb.Append("Last Name");
            sb.Append(",");
            sb.Append("First Name");
            sb.Append(",");
            sb.Append("PERSLink ID");
            sb.Append(",");
            sb.Append("SSN");
            sb.Append(",");
            sb.Append("Detail Status");
            sb.Append(",");
            sb.Append("Benefit");
            sb.Append(",");

            if (icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypePurchases)
            {
                sb.Append("Plan");
                sb.Append(",");
            }
            sb.Append("Record Type");
            sb.Append(",");



            if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {                            
                sb.Append("Reporting Month");
                sb.Append(",");
                sb.Append("End month for Bonus/Retro Pay"); //PIR 14758
                sb.Append(",");
                sb.Append("Eligible Wages");
                sb.Append(",");
                sb.Append("EE Post Tax");
                sb.Append(",");
                sb.Append("EE Pre Tax");
                sb.Append(",");
                sb.Append("EE Employer Pickup");
                sb.Append(",");
                sb.Append("ER Pre Tax");
                sb.Append(",");
                sb.Append("RHIC ER");
                sb.Append(",");
                sb.Append("RHIC EE");
                sb.Append(",");
                sb.Append("EE Interest");
                sb.Append(",");
                sb.Append("ER Interest");
                sb.Append(",");
                sb.Append("ER RHIC Interest");                
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                sb.Append("Reporting Month");
                sb.Append(",");
                sb.Append("Premium Amount");
               
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                sb.Append("Pay Period Start Date");
                sb.Append(",");
                sb.Append("Pay Period End Date");
                sb.Append(",");
                sb.Append("Pay Check Date");
                sb.Append(",");
              
                sb.Append("Contribution 1");
                sb.Append(",");
                sb.Append("Provider 1");
                sb.Append(",");

               
                sb.Append("Contribution 2");
                sb.Append(",");
                sb.Append("Provider 2");
                sb.Append(",");
                
                sb.Append("Contribution 3");
                sb.Append(",");

                sb.Append("Provider 3");
                sb.Append(",");

               
                sb.Append("Contribution 4");
                sb.Append(",");
                sb.Append("Provider 4");
                sb.Append(",");

              
                sb.Append("Contribution 5");
                sb.Append(",");
                sb.Append("Provider 5");
                sb.Append(",");
              
                sb.Append("Contribution 6");
                sb.Append(",");
                sb.Append("Provider 6");
                sb.Append(",");
              
                sb.Append("Contribution 7");
                sb.Append(",");
                sb.Append("Provider 7");
               
            }
            else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {             
                sb.Append("Submitted Date");
                sb.Append(",");
                sb.Append("Purchase Amount");
            }

            sb.Append(",");
            sb.Append("Comments");
            sb.Append("\r\n");

            foreach (busEmployerPayrollDetail lbusDetail in iclbEmployerPayrollDetail)
            {
                if (lbusDetail.ibusPlan == null)
                    lbusDetail.LoadPlan();


                sb.Append(lbusDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id );
                sb.Append(",");

                //icdoEmployerPayrollDetail.employer_payroll_header_id

                sb.Append(lbusDetail.icdoEmployerPayrollDetail.employer_payroll_header_id);
                sb.Append(",");

                sb.Append(lbusDetail.icdoEmployerPayrollDetail.last_name);
                sb.Append(",");
                sb.Append(lbusDetail.icdoEmployerPayrollDetail.first_name);

                sb.Append(",");
                sb.Append(lbusDetail.icdoEmployerPayrollDetail.person_id);
                sb.Append(",");
               
                sb.Append(lbusDetail.icdoEmployerPayrollDetail.ssn);
                sb.Append(",");

                //icdoEmployerPayrollDetail.status_description
                sb.Append(lbusDetail.icdoEmployerPayrollDetail.status_description);
                sb.Append(",");

                //ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_description
                sb.Append(icdoEmployerPayrollHeader.header_type_description);
                sb.Append(",");


                if (icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    sb.Append(lbusDetail.ibusPlan.icdoPlan.plan_name);
                    sb.Append(",");
                }

               
                sb.Append(lbusDetail.icdoEmployerPayrollDetail.record_type_description);
                sb.Append(",");

                if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    String strTempValue = String.Empty;
                    
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.pay_period_date.ToString("MM/yyyy"));
                    sb.Append(",");

                    if (lbusDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus == DateTime.MinValue) //PIR 14758
                        sb.Append(string.Empty);
                    else
                        sb.Append(lbusDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus.ToString("MM/yyyy"));                    
                    sb.Append(",");


                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.eligible_wages.ToString("$0.00") + ")";
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.eligible_wages.ToString("$0.00"); 
                    }
                    sb.Append(strTempValue);
                    sb.Append(",");


                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.ee_contribution_calculated.ToString("$0.00") + ")"; //PIR 14758
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.ee_contribution_calculated.ToString("$0.00"); //PIR 14758
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");



                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.ee_pre_tax_calculated.ToString("$0.00") + ")";
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.ee_pre_tax_calculated.ToString("$0.00");
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");





                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.ee_employer_pickup_calculated.ToString("$0.00") + ")";
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.ee_employer_pickup_calculated.ToString("$0.00");
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");


                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.er_contribution_calculated.ToString("$0.00") + ")"; //PIR 14758
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.er_contribution_calculated.ToString("$0.00"); //PIR 14758
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");



                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.rhic_er_contribution_calculated.ToString("$0.00") + ")";
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.rhic_er_contribution_calculated.ToString("$0.00");
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");




                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_calculated.ToString("$0.00") + ")";
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_calculated.ToString("$0.00");
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");



                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.member_interest_calculated.ToString("$0.00") + ")";
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.member_interest_calculated.ToString("$0.00");
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");




                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.employer_interest_calculated.ToString("$0.00") + ")";
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.employer_interest_calculated.ToString("$0.00");
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");


                    if (lbusDetail.icdoEmployerPayrollDetail.record_type_value.Equals("-ADJ"))
                    {
                        strTempValue = "(" + lbusDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated.ToString("$0.00") + ")";
                    }
                    else
                    {
                        strTempValue = lbusDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated.ToString("$0.00");
                    }


                    sb.Append(strTempValue);
                    sb.Append(",");                   
                   
                }
                else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.pay_period_date.ToString("MM/yyyy"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.premium_amount.ToString("$0.00"));
                    sb.Append(",");
                }
                else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                   
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.pay_period_start_date.ConvertMinValueAsEmpty("MM/dd/yyyy"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.pay_period_end_date.ConvertMinValueAsEmpty("MM/dd/yyyy"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.pay_check_date);
                    sb.Append(",");

                     sb.Append(lbusDetail.icdoEmployerPayrollDetail.contribution_amount1.ToString("$0.00"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.provider_org_code_id1);
                    sb.Append(",");
                  
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.contribution_amount2.ToString("$0.00"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.provider_org_code_id2);
                    sb.Append(",");


                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.contribution_amount3.ToString("$0.00"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.provider_org_code_id3);
                    sb.Append(",");


                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.contribution_amount4.ToString("$0.00"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.provider_org_code_id4);
                    sb.Append(",");

                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.contribution_amount5.ToString("$0.00"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.provider_org_code_id5);
                    sb.Append(",");

                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.contribution_amount6.ToString("$0.00"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.provider_org_code_id6);
                    sb.Append(",");
                   
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.contribution_amount7.ToString("$0.00"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.provider_org_code_id7);
                    sb.Append(",");
                }
                else if (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    sb.Append(icdoEmployerPayrollHeader.submitted_date.ToString("MM/yyyy"));
                    sb.Append(",");
                    sb.Append(lbusDetail.icdoEmployerPayrollDetail.purchase_amount_reported.ToString("$0.00"));
                    sb.Append(",");
                }
                
                sb.Append(lbusDetail.LoadCommentsOfPayrollDetailForExportToText());
                sb.Append("\r\n");
            }
            byte[] byteArray = ASCIIEncoding.ASCII.GetBytes(sb.ToString());
            return byteArray;
        }

        #endregion

        //uat pir - 2427
        /// <summary>
        /// method to allocate remittance to header if available from batch
        /// </summary>
        public void AllocateRemittanceFromBatch()
        {
            //Automatic Purchase Allocation for each detail
            AllocateAutomaticForPurchase();

            //Allocate the Automatic Remittance
            AllocateAutomaticRemittance();

            //Overriding the Header Status for Specfic Cases
            OverrideHeaderStatus();
        }
        /// <summary>
        /// Checks if Employer Payroll Detail contains only Other 457 Plan details
        /// </summary>
        /// <returns>
        /// True: If all details are of Plan ID 457
        /// False : Otherwise
        /// </returns>
        public bool IsDetailContainOnlyOther457()
        {
            bool lblnResult = false;
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
            if (!iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457).Any())
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        /// <summary>
        /// prod pir 4079 : method to create gl
        /// </summary>
        /// <param name="astrRemittanceType"></param>
        /// <param name="adecDifferencAmount"></param>
        /// <param name="aintPlanId"></param>
        public void CreateGLForDifferenceAmount(string astrRemittanceType, decimal adecDifferenceAmount, int aintPlanId, bool ablnCalledFromBrowser = false)
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = aintPlanId;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeEmployerReporting;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
            if (adecDifferenceAmount < 0)
            {
                lcdoAcccountReference.item_type_value =
                    astrRemittanceType == busConstant.ItemTypeContribution ? busConstant.GLItemTypeOverContribution : busConstant.GLItemTypeOverRHIC;
            }
            else if (adecDifferenceAmount > 0)
            {
                lcdoAcccountReference.item_type_value =
                    astrRemittanceType == busConstant.ItemTypeContribution ? busConstant.GLItemTypeUnderContribution : busConstant.GLItemTypeUnderRHIC;
            }
            busGLHelper.GenerateGL(lcdoAcccountReference, 0, icdoEmployerPayrollHeader.org_id,
                                              icdoEmployerPayrollHeader.employer_payroll_header_id,
                                              Math.Abs(adecDifferenceAmount),
                                              icdoEmployerPayrollHeader.payroll_paid_date, ablnCalledFromBrowser == true ? DateTime.Now :
                                              busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);
        }

        /// <summary>
        /// prod pir 5709 : method to update balancing status of the header
        /// </summary>
        private void UpdateBalancingStatus()
        {
            if (icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusNoRemittance)
            {
                LoadContributionByPlan();
                LoadTotalAppliedRemittance();

                if (idecTotalRemittanceApplied == idecTotalReportedByPlan)
                {
                    icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusBalanced;
                }
                else
                {
                    icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusUnbalanced;
                }
            }
        }

        //prod pir 5495 : updating balancing status value to Balanced if all details are Negative adj
        //PIR 18755 - filtering Ignore details and updating balancing status value to Balanced if all details are Negative adj and Negative Bonus.
        private void UpdateBalancingStatusForNegativeAdjustments()
        {
	    	IEnumerable<busEmployerPayrollDetail> lenumNotIgnoredPayrollDetail = iclbEmployerPayrollDetail.Where(d => d.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusIgnored);
            if (lenumNotIgnoredPayrollDetail.IsNotNull())
            {
                if (icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusBalanced &&
                    !lenumNotIgnoredPayrollDetail.Where(o => (o.icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeNegativeAdjustment &&
                                                        o.icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeNegativeBonus)).Any())
                {
                    icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusBalanced;
                }               
            }
        }

        /// <summary>
        /// method to update balancing status while posting
        /// </summary>
        private void UpdateBalancingStatusWhilePosting()
        {
            if (iclbEmployerPayrollDetail == null)
                LoadEmployerPayrollDetail();
            
            //prod pir 5495 : change balancing status to balanced for Negative headers
            UpdateBalancingStatusForNegativeAdjustments();

            //prod pir 6650
            if (_icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp && 
                icdoEmployerPayrollHeader.balancing_status_value != busConstant.PayrollHeaderBalancingStatusBalanced)
            {
                if (!iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457).Any())
                {
                    icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusBalanced;
                }
            }
        }
        public void LoadActiveMemberForHeader()
        {
            
            iclbEmployerPayrollDetail=new Collection<busEmployerPayrollDetail>();
            CreatePayrollDetailForESS(true);
            foreach (busEmployerPayrollDetail  lbusEmployerPayrollDetail in iclbEmployerPayrollDetail)
            {
                if (lbusEmployerPayrollDetail.ibusPerson==null )
                    lbusEmployerPayrollDetail.LoadPerson();
                if (lbusEmployerPayrollDetail.ibusPersonAccount==null )
                    lbusEmployerPayrollDetail.LoadPersonAccount();
                if (lbusEmployerPayrollDetail.ibusPersonAccount.ibusPersonEmploymentDetail == null)
                    lbusEmployerPayrollDetail.LoadPersonEmploymentDetail();
                if (lbusEmployerPayrollDetail.ibusPlan == null)
                    lbusEmployerPayrollDetail.LoadPlan();
                lbusEmployerPayrollDetail.ibusEmployerPayrollHeader = this;
            }


         }
        //prod pir 6586 : updating payee account infor for vested ER contr
        public DataTable idtBenefitAccountInfo { get; set; }
        public busDBCacheData ibusDBCacheData { get; set; }

        /// <summary>
        /// Stores 1 if regular or positive adjustment ,
        /// -1 if negative adjustment
        /// </summary>
        public int iintCredit { get; set; }


        public bool IsPayPeriodValidForDefComp()
        {
            DataTable ldtbDetailCount = Select("cdoEmployerPayrollHeader.IsPayPeriodValidForDefComp",
                                        new object[6] { icdoEmployerPayrollHeader.pay_period_start_date, icdoEmployerPayrollHeader.pay_period_end_date, icdoEmployerPayrollHeader.org_id, icdoEmployerPayrollHeader.header_type_value, icdoEmployerPayrollHeader.report_type_value, icdoEmployerPayrollHeader.employer_payroll_header_id });
            if (ldtbDetailCount.Rows.Count > 0)
                return false;
            else
                return true;

        }
        //PIR 13275
        public string istrEmployerSelectionEnrollment { get; set; }

        public string istrCreateReportNavigation { get; set; }
        public ArrayList EnrollPrevious_Click()
        {
            ArrayList larrList = new ArrayList();
            iblnIsDetailCountNone = false;
            this.EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }

        public busPerson GetPersonNameBySSN(string AintPersonSSN)
        {
            busPerson lbusperson = new busPerson();

            lbusperson = lbusperson.LoadPersonBySsn(AintPersonSSN);
            return lbusperson;
        }

        public decimal idecTotalNegativeEEContributionReportedByPlan { get; set; }
        public decimal idecTotalNegativeERContributionReportedByPlan { get; set; }
        public decimal idecTotalNegativeRHICERContributionReportedByPlan { get; set; }
        public decimal idecTotalNegativeRHICEEContributionReportedByPlan { get; set; }
        public decimal idecTotalNegativeMemberInterestCalculatedByPlan { get; set; }
        public decimal idecTotalNegativeEmployerInterestCalculatedByPlan { get; set; }
        public decimal idecTotalNegativeRHICEmployerInterestCalculatedByPlan { get; set; }
        public decimal idecTotalNegativeEEEmployerPickupReportedByPlan { get; set; }
        public decimal idecTotalNegativeEEPreTaxReportedByPlan { get; set; }
        public decimal idecTotalNegativeReportedRHICContributionByPlan { get; set; }

        public void LoadESSRetirementContributionByPlan()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadESSRetirementContributionByPlan",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbRetirementContributionByPlan = new Collection<busEmployerPayrollHeader>();
            foreach (DataRow aTotalsRow in ldtbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader();
                sqlFunction.LoadQueryResult(lobjEmpHeader, aTotalsRow);
                _iclbRetirementContributionByPlan.Add(lobjEmpHeader);
            }
        
            //Reset the Total Reported By Plan
            //_idecTotalReportedByPlan = 0;

            foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbRetirementContributionByPlan)
            {
                lobjEmployerPayrollHeader.LoadPlan(lobjEmployerPayrollHeader.iintPlanID);
                //lobjEmployerPayrollHeader.idecTotalReportedByPlan =
                //    lobjEmployerPayrollHeader.idecTotalEEContributionReportedByPlan +
                //    lobjEmployerPayrollHeader.idecTotalERContributionReportedByPlan +
                //    lobjEmployerPayrollHeader.idecTotalEEPreTaxReportedByPlan +
                //    lobjEmployerPayrollHeader.idecTotalRHICERContributionReportedByPlan +
                //    lobjEmployerPayrollHeader.idecTotalRHICEEContributionReportedByPlan +
                //    lobjEmployerPayrollHeader.idecTotalEEEmployerPickupReportedByPlan +
                //    lobjEmployerPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                //    lobjEmployerPayrollHeader.idecTotalEmployerInterestCalculatedByPlan +
                //    lobjEmployerPayrollHeader.idecTotalRHICEmployerInterestCalculatedByPlan;

                //PIR - 330
                lobjEmployerPayrollHeader.idecTotalReportedRetrContributionByPlan = lobjEmployerPayrollHeader.idecTotalEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEEmployerPickupReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEmployerInterestCalculatedByPlan;

                lobjEmployerPayrollHeader.idecTotalNegAdjContribution = lobjEmployerPayrollHeader.idecTotalNegativeEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEEPreTaxReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEEEmployerPickupReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEmployerInterestCalculatedByPlan;

                //PIR 15238 
                lobjEmployerPayrollHeader.idecTotalNegAdjContributionCalculated = lobjEmployerPayrollHeader.idecTotalNegativeEEContributionCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeERContributionCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEEPreTaxCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEEEmployerPickupCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeEmployerInterestCalculatedByPlan;

                //PIR 15238 
                lobjEmployerPayrollHeader.idecTotalCalculatedRetrContributionByPlan = lobjEmployerPayrollHeader.idecTotalEEContributionCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERContributionCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEPreTaxCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEEEmployerPickupCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalMemberInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalEmployerInterestCalculatedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERPreTaxMatchReportedByPlan;         //PIR 25920 New Plan DC 2025


                //PIR 13996 - Shifting RHIC Interests from  TotalReportedRetrContributionByPlan & TotalNegAdjContribution
                // to RHIC groups


                lobjEmployerPayrollHeader.idecTotalReportedRHICContributionByPlan = lobjEmployerPayrollHeader.idecTotalRHICERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalRHICEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalRHICEmployerInterestCalculatedByPlan;

                lobjEmployerPayrollHeader.idecTotalNegativeReportedRHICContributionByPlan = lobjEmployerPayrollHeader.idecTotalNegativeRHICERContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeRHICEEContributionReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeRHICEmployerInterestCalculatedByPlan;

                //PIR 15238 
                lobjEmployerPayrollHeader.idecTotalCalculatedRHICContributionByPlan = lobjEmployerPayrollHeader.idecTotalRHICERContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalRHICEEContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalRHICEmployerInterestCalculatedByPlan;

                //PIR 15238 
                lobjEmployerPayrollHeader.idecTotalNegativeCalculatedRHICContributionByPlan = lobjEmployerPayrollHeader.idecTotalNegativeRHICERContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeRHICEEContributionCalculatedByPlan +
                   lobjEmployerPayrollHeader.idecTotalNegativeRHICEmployerInterestCalculatedByPlan;

                //
                lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.ItemTypeContribution);

                lobjEmployerPayrollHeader.idecTotalAppliedRHICContributionByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.ItemTypeRHICContribution);

                lobjEmployerPayrollHeader.idecTotalADECAppliedByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.ItemTypeADECContribution);
                lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlanWithAdec = lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlan + lobjEmployerPayrollHeader.idecTotalADECAppliedByPlan;

                //PIR-18151
                lobjEmployerPayrollHeader.idecOutstandingRetirementBalance =
                                                                        lobjEmployerPayrollHeader.idecTotalReportedRetrContributionByPlan - lobjEmployerPayrollHeader.idecTotalAppliedRetrContributionByPlan
                                                                        - lobjEmployerPayrollHeader.idecTotalADECAppliedByPlan;//PIR 25920
                lobjEmployerPayrollHeader.idecTotalReportedByPlan = lobjEmployerPayrollHeader.idecTotalReportedRetrContributionByPlan +
                                                                    lobjEmployerPayrollHeader.idecTotalNegAdjContribution +
                                                                    lobjEmployerPayrollHeader.idecTotalReportedRHICContributionByPlan +
                                                                    lobjEmployerPayrollHeader.idecTotalNegativeReportedRHICContributionByPlan;


                //PIR 15238 
                lobjEmployerPayrollHeader.idecTotalCalculatedByPlan = lobjEmployerPayrollHeader.idecTotalCalculatedRetrContributionByPlan +
                                                                   lobjEmployerPayrollHeader.idecTotalNegAdjContributionCalculated +
                                                                   lobjEmployerPayrollHeader.idecTotalCalculatedRHICContributionByPlan +
                                                                   lobjEmployerPayrollHeader.idecTotalNegativeCalculatedRHICContributionByPlan;
                //PIR-18151
                lobjEmployerPayrollHeader.idecOutstandingRHICBalance = lobjEmployerPayrollHeader.idecTotalReportedRHICContributionByPlan - lobjEmployerPayrollHeader.idecTotalAppliedRHICContributionByPlan;
                //PIR 25920 DC 25025 changes
                lobjEmployerPayrollHeader.idecOutstandingADECBalance = lobjEmployerPayrollHeader.idecTotalADECReportedByPlan - lobjEmployerPayrollHeader.idecTotalADECAppliedByPlan;

                //Calculating Total Reported By All Plans
                _idecTotalReportedByPlan += lobjEmployerPayrollHeader.idecTotalReportedByPlan;
                _idecTotalCalculatedByPlan += lobjEmployerPayrollHeader.idecTotalCalculatedByPlan;
            }
        }
        public bool IsRoundingDifferenceWithinLimits()
        {
            if (_iclbEmployerPayrollDetail.IsNull())
                LoadEmployerPayrollDetail();
            _icdoEmployerPayrollHeader.total_contributions_from_details = 0;
            foreach (busEmployerPayrollDetail lbusEmployerPayrollDetail in iclbEmployerPayrollDetail)
            {
                if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusIgnored)
                {
                                            //Add the Contribution if detail entries are Regular / Bonus / +ADJ. Substract it for -ADJ
                        if ((lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular) ||
                            (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ||
                            (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment))
                        {
                            _icdoEmployerPayrollHeader.total_contributions_from_details += lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_calculated +
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_calculated +
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_calculated +
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_calculated +
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_calculated +
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_calculated +
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_calculated + 
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_calculated +
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_calculated +
                                                                              lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_calculated;       //PIR 25920 New Plan DC 2025
                        }
                        else if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment ||
                                lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)
                        {
                            _icdoEmployerPayrollHeader.total_contributions_from_details -= lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_calculated +
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_calculated +
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_calculated +
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_calculated +
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_calculated +
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_calculated +
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_calculated + 
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_calculated +
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_calculated +
                                                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_calculated;        //PIR 25920 New Plan DC 2025
                        }
                }
            }
			//PIR 25920 New Plan DC 2025, catching the difference for validation if header is retiremnet
            //if(icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
            //    Math.Abs(Math.Round(_icdoEmployerPayrollHeader.total_adec_amount_calculated, 2) - Math.Round(_icdoEmployerPayrollHeader.total_adec_amount_reported, 2)) > idecRoundingDifferenceValue)
            //    return false;
            return icdoEmployerPayrollHeader.idecRoundingDifference >= 0 && icdoEmployerPayrollHeader.idecRoundingDifference <= idecRoundingDifferenceValue ? true : false;
        }
        public decimal idecTotalNegativePremiumAmountByPlan { get; set; }
        //public decimal idecTotalNegativeFeeAmountByPlan { get; set; }
        //public decimal idecOtherNegativeRHICAmount { get; set; }
        //public decimal idecJSNegativeRHICAmount { get; set; }
        public decimal idecTotalNegativeReportedByPlan { get; set; }
        public decimal idecTotalNegPosReportedByPlan { get; set; }
        public decimal idecTotalReportedByAllPlansInsurance { get; set; }


        public void LoadESSInsurancePremiumByPlan()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadESSInsurancePremiumByPlan",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbInsuranceContributionByPlan = new Collection<busEmployerPayrollHeader>();
            foreach (DataRow aTotalsRow in ldtbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                sqlFunction.LoadQueryResult(lobjEmpHeader, aTotalsRow);
                _iclbInsuranceContributionByPlan.Add(lobjEmpHeader);
            }

            //Reset the Total Reported By Plan
            idecTotalReportedByAllPlansInsurance = 0;

            foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbInsuranceContributionByPlan)
            {
                if (lobjEmployerPayrollHeader.ibusPlan == null)
                    lobjEmployerPayrollHeader.LoadPlan(lobjEmployerPayrollHeader.iintPlanID);
                lobjEmployerPayrollHeader.idecTotalReportedByPlan = lobjEmployerPayrollHeader.idecTotalPremiumAmountByPlan;
                lobjEmployerPayrollHeader.idecTotalNegativeReportedByPlan = lobjEmployerPayrollHeader.idecTotalNegativePremiumAmountByPlan;
                lobjEmployerPayrollHeader.idecTotalNegPosReportedByPlan = lobjEmployerPayrollHeader.idecTotalReportedByPlan + lobjEmployerPayrollHeader.idecTotalNegativeReportedByPlan;
                //Calculating Total Reported By All Plans
                idecTotalReportedByAllPlansInsurance += lobjEmployerPayrollHeader.idecTotalNegPosReportedByPlan;

                //PIR-(5717) PIR-6501 Applied Amount field for insurance
                string lstrRemittanceType = busEmployerReportHelper.GetToItemTypeByPlan(lobjEmployerPayrollHeader.ibusPlan.icdoPlan.plan_code);
                lobjEmployerPayrollHeader.idecTotalAppliedPremiumByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, lstrRemittanceType);               

                icdoEmployerPayrollHeader.idecInsuranceContributionByPlan += lobjEmployerPayrollHeader.idecTotalReportedByPlan - lobjEmployerPayrollHeader.idecTotalAppliedPremiumByPlan;

            }
        }
        public decimal idecTotalNegativeContributionAmount1ReportedByPlan { get; set; }
        public decimal idecTotalNegativeContributionAmount2ReportedByPlan { get; set; }
        public decimal idecTotalNegativeContributionAmount3ReportedByPlan { get; set; }
        public decimal idecTotalNegativeContributionAmount4ReportedByPlan { get; set; }
        public decimal idecTotalNegativeContributionAmount5ReportedByPlan { get; set; }
        public decimal idecTotalNegativeContributionAmount6ReportedByPlan { get; set; }
        public decimal idecTotalNegativeContributionAmount7ReportedByPlan { get; set; }
        public decimal idecTotalNegAdjReportedByPlan { get; set; }
        public decimal idecTotalPosNegReportedByPlan { get; set; }

        public void LoadESSDeferredCompContributionByPlan()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadESSDeferredCompContributionByPlan",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id });
            _iclbDeferredCompContributionByPlan = new Collection<busEmployerPayrollHeader>();
            foreach (DataRow aTotalsRow in ldtbList.Rows)
            {
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader() { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() }; //F/W Upgrade PIR 11221 - F/W needs the main cdo to be initialized
                sqlFunction.LoadQueryResult(lobjEmpHeader, aTotalsRow);
                _iclbDeferredCompContributionByPlan.Add(lobjEmpHeader);
            }

            //Reset the Total Reported By Plan
            _idecTotalReportedByPlan = 0;

            foreach (busEmployerPayrollHeader lobjEmployerPayrollHeader in _iclbDeferredCompContributionByPlan)
            {
                lobjEmployerPayrollHeader.LoadPlan(lobjEmployerPayrollHeader.iintPlanID);
                lobjEmployerPayrollHeader.idecTotalReportedByPlan =
                    lobjEmployerPayrollHeader.idecTotalContributionAmount1ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount2ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount3ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount4ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount5ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount6ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalContributionAmount7ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalERPreTaxMatchReportedByPlan;      //PIR 25920 New Plan DC 2025
                lobjEmployerPayrollHeader.idecTotalNegAdjReportedByPlan =
                    lobjEmployerPayrollHeader.idecTotalNegativeContributionAmount1ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeContributionAmount2ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeContributionAmount3ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeContributionAmount4ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeContributionAmount5ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeContributionAmount6ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeContributionAmount7ReportedByPlan +
                    lobjEmployerPayrollHeader.idecTotalNegativeERPreTaxMatchReportedByPlan;      //PIR 25920 New Plan DC 2025
                lobjEmployerPayrollHeader.idecTotalPosNegReportedByPlan = lobjEmployerPayrollHeader.idecTotalReportedByPlan + lobjEmployerPayrollHeader.idecTotalNegAdjReportedByPlan;

                //Calculating Total Reported By All Plans
                //Excluding Other 457
                if (lobjEmployerPayrollHeader.iintPlanID != busConstant.PlanIdOther457)
                {
                    _idecTotalReportedByPlan += lobjEmployerPayrollHeader.idecTotalReportedByPlan;
                }

                lobjEmployerPayrollHeader.idecTotalAppliedContributionByPlan = GetAppliedAmountByPlanAndRemittanceType(lobjEmployerPayrollHeader, busConstant.ItemTypeDefeCompDeposit);
                lobjEmployerPayrollHeader.idecOutstandingDeferredCompBalance = lobjEmployerPayrollHeader.idecTotalReportedByPlan - lobjEmployerPayrollHeader.idecTotalAppliedContributionByPlan;
                if (lobjEmployerPayrollHeader.iintPlanID != busConstant.PlanIdOther457)
                {
                    //Calculating Total Reported By Plans
                    icdoEmployerPayrollHeader.idecOutstandingDeferredCompBalance = lobjEmployerPayrollHeader.idecTotalReportedByPlan - lobjEmployerPayrollHeader.idecTotalAppliedContributionByPlan;
                }
            }
        }     
        public decimal idecRoundingDifferenceValue
        {
            get
            {
                decimal idecRoundingDifferenceValue = 0.00M;
                return Decimal.TryParse(busGlobalFunctions.GetData1ByCodeValue(52, "RDVE", iobjPassInfo), out idecRoundingDifferenceValue) ? idecRoundingDifferenceValue : 0; 
            }
        }

        public decimal idecTotOutstandDefCompBal { get; set; }
        public cdoFileHdr icdoFileHdr { get; set; }

        /// <summary>
        /// This function will return true if Reporting month is less than Jan 1977
        /// </summary>
        /// <returns></returns>
        public bool iblnIsReportingMonthLessthanJan1977ForAdj()
        {
            if (iclbEmployerPayrollDetail.IsNotNull() && iclbEmployerPayrollDetail.Count > 0)
            {
                int i = 0;
                foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                {
                    i++;
                    if (lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeBonus)
                    {
                        if (lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_date < new DateTime(1977, 01, 01))
                        {
                            return true;
                        }
                    }
                }
            }    
            return false;
        }
        /// <summary>
        /// This function will return true if Begin month or End month for Retirement Bonus/Retro pay is less than Jan 1977
        /// </summary>
        /// <returns></returns>
        public bool iblnIsReportingMonthLessthanJan1977ForAdjBonusRetro()
        {
            if (iclbEmployerPayrollDetail.IsNotNull() && iclbEmployerPayrollDetail.Count > 0)
            {
                int i = 0;
                foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                {
                    i++;
                    if (lobjPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)
                    {
                        if (Convert.ToDateTime(lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus) < new DateTime(1977, 01, 01) || lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_date < new DateTime(1977, 01, 01))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //Maik Mail Dated March 11, 2015
        private bool IsReportingMonthValid(string strReportingMonth)
        {
            if (string.IsNullOrEmpty(strReportingMonth)) return true;
            string[] strdates = strReportingMonth.Split("/");
            int month = Convert.ToInt32(strdates[0]);
            int year = Convert.ToInt32(strdates[1]);

            if ((month >= 1 && month <= 12) && (year >= 1901 && year <= 2100))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///    //PIR 14154 - While uploading IF Header and  Detail dates  are not same throw hard error 
        /// </summary>
        /// <returns></returns>
        public bool CheckIfHeaderAndDetailDatesAreNotSame()
        {
            if (iclbEmployerPayrollDetail.IsNotNull() && iclbEmployerPayrollDetail.Count > 0)
            {
                foreach (busEmployerPayrollDetail lobjPayrollDetail in iclbEmployerPayrollDetail)
                {
                    if (lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_start_date != icdoEmployerPayrollHeader.pay_period_start_date ||
                        lobjPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_date != icdoEmployerPayrollHeader.pay_period_end_date)
                    {
                        return true;
                    }
                }
            }
            return false;

        }
        /// <summary>
        /// PIR - 15448 - Since employer_payroll_detail_id is specified as data key names for the grids on the create 
        /// payroll template and since the collection is not persisted, assigning unique key values
        /// </summary>
        public void AssignEmployerPayrollDetailID()
        {
            if (_iclbEmployerPayrollDetail.IsNotNull() && _iclbEmployerPayrollDetail.Count > 0)
            {
                int lintInitialEmpDetailID = 1;
                foreach (busEmployerPayrollDetail lbusEmployerPayrollDetail in _iclbEmployerPayrollDetail)
                {
                    lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id = lintInitialEmpDetailID;
                    lintInitialEmpDetailID++;
                }
            }
        }

        public void ValidateReviewDetails()
        {
            if ((_iclbEmployerPayrollDetail != null) && (_iclbEmployerPayrollDetail.Count > 0))
            {
                foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _iclbEmployerPayrollDetail.Where(i=>i.icdoEmployerPayrollDetail.status_value == busConstant.DepositDetailStatusReview))
                {
                    lobjEmployerPayrollDetail.iblnIsFromValidDetail = true;
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = this;
                    lobjEmployerPayrollDetail.LoadPersonBySSNAndLastName();
                    lobjEmployerPayrollDetail.LoadObjectsForValidation(true);
                    lobjEmployerPayrollDetail.iblnValidateHeader = false;
                    lobjEmployerPayrollDetail.LoadOldSalaryAmount();
                    lobjEmployerPayrollDetail.UpdateCalculatedFields();
                    lobjEmployerPayrollDetail.ValidateSoftErrors();
                }
            }
            CalculateContributionWagesInterestFromDtl();
            UpdateTotalValues();
            base.ValidateSoftErrors();
            base.UpdateValidateStatus();
        }


        // ESS Backlog PIR - 13416
        public void LoadEmployerPayrollHeaderComments()
        {
            DataTable ldtbList = Select<cdoComments>(
                               new string[2] { "employer_payroll_header_id", "employer_payroll_detail_id" },
                               new object[2] { _icdoEmployerPayrollHeader.employer_payroll_header_id, null }, null, null);
            iclbPayrollHeaderCommentsHistory = GetCollection<busComments>(ldtbList, "icdoComments");
            iclbPayrollHeaderCommentsHistory = iclbPayrollHeaderCommentsHistory.OrderBy(o => o.icdoComments.created_date).ToList<busComments>().ToCollection<busComments>();
            if ((this.iobjPassInfo.istrFormName == "wfmEmployerPayrollHeaderLookup" || this.iobjPassInfo.istrFormName == "wfmEmployerPayrollHeaderMaintenance") && iobjPassInfo.idictParams.ContainsKey("UserSettingForRenderNeoGrid") && Convert.ToBoolean(iobjPassInfo.idictParams["UserSettingForRenderNeoGrid"]) == false)
            {
                foreach (busComments item in iclbPayrollHeaderCommentsHistory)
                {
                    Regex regex = new Regex(@"\n");
                    string temp = item.icdoComments.comments;
                    item.icdoComments.comments = regex.Replace(temp, "</br>");
                }
            }
        }

        //ESS Backlog PIR - 13416
        public void LoadDetailComments()
        {
            foreach (busEmployerPayrollDetail lobjbusEmployerPayrollDetail in iclbEmployerPayrollDetail)
            {
                lobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.istrlookupComment = lobjbusEmployerPayrollDetail.LoadCommentsOfPayrollDetailForExportToText();
                lobjbusEmployerPayrollDetail.LoadPlan();
                lobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_name = lobjbusEmployerPayrollDetail.ibusPlan.icdoPlan.plan_name;
                lobjbusEmployerPayrollDetail.ibusEmployerPayrollHeader = this;
            }
        }
       //PIR - 8444
        public void btnSaveComments_Click(string astrComment)
        {
            if (!String.IsNullOrWhiteSpace(astrComment))
            {
                busComments lbusComments = new busComments { icdoComments = new cdoComments() };
                lbusComments.icdoComments.comments = astrComment;
                lbusComments.icdoComments.employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;                
                lbusComments.icdoComments.Insert();
                icdoEmployerPayrollHeader.comments = String.Empty;
            }
        }
		#region PIR-12562
        public void btnSuppress_Clicked()
        {
            icdoEmployerPayrollHeader.suppress_3rd_payroll_flag = busConstant.Flag_Yes;
            icdoEmployerPayrollHeader.Update();
            iblnValidateDetail = true;
            ValidateSoftErrors();
            UpdateValidateStatus();
        }

        public bool IsDetailEntryExistsWith1145Error()
        {
            return Select("cdoEmployerPayrollHeader.Error1145ExistsQuery", 
                new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id }).Rows.Count > 0;           
        }
		#endregion PIR-12562

        #region PIR-6501 
		//if Any change in remittance allocation to Employer Header batch, then we need to make changes in Employer Report posting batch (14).
        public void AllocateRemittanceToEmployerPayroll(string astrHeaderType,int aintBatchScheduleId)
        {
            if (idatRunDate == DateTime.MinValue)
                idatRunDate = busGlobalFunctions.GetSysManagementBatchDate();
            DataTable ldtRetrInsrHeader = new DataTable();
            if (astrHeaderType == busConstant.PayrollHeaderBenefitTypeRtmt || astrHeaderType == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                DeleteInvalidAllocations(astrHeaderType, aintBatchScheduleId);

                ldtRetrInsrHeader = Select("cdoEmployerPayrollHeader.LoadEmpHeaderForARBatchAllocation", new object[2] { icdoEmployerPayrollHeader.org_id, astrHeaderType });
                //getting the allowable remittance variance
                //idecAllowableVariance = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.ConstantAllowableVairance, iobjPassInfo));
                foreach (DataRow ldrHeader in ldtRetrInsrHeader.Rows)
                {
                    busEmployerPayrollHeader lobjHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                    lobjHeader.icdoEmployerPayrollHeader.LoadData(ldrHeader);
                    lobjHeader.LoadContributionByPlan();
                    if (lobjHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        foreach (busEmployerPayrollHeader lobjHeaderByPlan in lobjHeader._iclbRetirementContributionByPlan)
                        {
                            if ((lobjHeaderByPlan.idecTotalReportedRetrContributionWithoutADECAmtByPlan - lobjHeaderByPlan.idecTotalAppliedRetrContributionByPlan) > 0)
                            {
                                //query to load available remittance
                                DataTable ldtRemittance = Select("cdoRemittance.LoadAvailableRemittanceForEmpARBatch",
                                    new object[4] { idatRunDate, lobjHeader.icdoEmployerPayrollHeader.org_id, busConstant.ItemTypeContribution,
                                                    lobjHeaderByPlan.iintPlanID});
                                //method to allocate amount
                                AllocateAmount(lobjHeader, lobjHeaderByPlan, ldtRemittance,
                                    (lobjHeaderByPlan.idecTotalReportedRetrContributionWithoutADECAmtByPlan - lobjHeaderByPlan.idecTotalAppliedRetrContributionByPlan));
                            }
                            if ((lobjHeaderByPlan.idecTotalReportedRHICContributionByPlan - lobjHeaderByPlan.idecTotalAppliedRHICContributionByPlan) > 0)
                            {
                                //query to load available remittance
                                DataTable ldtRemittance = Select("cdoRemittance.LoadAvailableRemittanceForEmpARBatch",
                                    new object[4] { idatRunDate, lobjHeader.icdoEmployerPayrollHeader.org_id, busConstant.ItemTypeRHICContribution,
                                                    lobjHeaderByPlan.iintPlanID});
                                //method to allocate amount
                                AllocateAmount(lobjHeader, lobjHeaderByPlan, ldtRemittance,
                                    (lobjHeaderByPlan.idecTotalReportedRHICContributionByPlan - lobjHeaderByPlan.idecTotalAppliedRHICContributionByPlan));
                            }
                            //PIR 25920 DC 2025 changes
                            if ((lobjHeaderByPlan.idecTotalADECReportedByPlan - lobjHeaderByPlan.idecTotalADECAppliedByPlan) > 0)
                            {
                                //query to load available remittance
                                DataTable ldtRemittance = Select("cdoRemittance.LoadAvailableRemittanceForEmpARBatch",
                                    new object[4] { idatRunDate, lobjHeader.icdoEmployerPayrollHeader.org_id, busConstant.ItemTypeADECContribution,
                                                    lobjHeaderByPlan.iintPlanID});
                                //method to allocate amount
                                AllocateAmount(lobjHeader, lobjHeaderByPlan, ldtRemittance,
                                    (lobjHeaderByPlan.idecTotalADECReportedByPlan - lobjHeaderByPlan.idecTotalADECAppliedByPlan));
                            }
                        }
                    }
                    else if (lobjHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                    {
                        foreach (busEmployerPayrollHeader lobjHeaderByPlan in lobjHeader.iclbInsuranceContributionByPlan)
                        {
                            if ((lobjHeaderByPlan.idecTotalReportedByPlan - lobjHeaderByPlan.idecTotalAppliedPremiumByPlan) > 0)
                            {
                                string lstrRemittanceType = busEmployerReportHelper.GetToItemTypeByPlan(lobjHeaderByPlan.ibusPlan.icdoPlan.plan_code);
                                //query to load available remittance
                                DataTable ldtRemittance = Select("cdoRemittance.LoadAvailableRemittanceForEmpARBatch",
                                                                   new object[4] { idatRunDate, lobjHeader.icdoEmployerPayrollHeader.org_id, lstrRemittanceType,
                                                    lobjHeaderByPlan.iintPlanID});
                                //method to allocate amount
                                AllocateAmount(lobjHeader, lobjHeaderByPlan, ldtRemittance,
                                    (lobjHeaderByPlan.idecTotalReportedByPlan - lobjHeaderByPlan.idecTotalAppliedPremiumByPlan));
                            }
                        }
                    }
                    lobjHeader = null;
                }
            }
            if (astrHeaderType == busConstant.PayrollHeaderBenefitTypeDefComp || astrHeaderType == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                DataTable ldtEmployerReport = Select("cdoEmployerPayrollHeader.LoadEmployerPayrollHeaderForRemittanceAllocationByOrgId", new object[1] { icdoEmployerPayrollHeader.org_id });
                DataTable ldtEmployerDetailReport = Select("cdoEmployerPayrollDetail.LoadDetailForAllocateRemittanceByOrgId", new object[1] { icdoEmployerPayrollHeader.org_id });
                foreach (DataRow dr in ldtEmployerReport.Rows)
                {
                    busEmployerPayrollHeader lobjEmployerHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                    lobjEmployerHeader.icdoEmployerPayrollHeader.LoadData(dr);
                    DataTable ldtDetail = ldtEmployerDetailReport.AsEnumerable()
                                            .Where(o => o.Field<int>("employer_payroll_header_id") == lobjEmployerHeader.icdoEmployerPayrollHeader.employer_payroll_header_id)
                                            .AsDataTable();
                    lobjEmployerHeader.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
                    lobjEmployerHeader.iclbEmployerPayrollDetail = GetCollection<busEmployerPayrollDetail>(ldtDetail, "icdoEmployerPayrollDetail");
                    lobjEmployerHeader.AllocateRemittanceFromBatch();
                }
            }
            if (astrHeaderType == busConstant.PayrollHeaderBenefitTypeRtmt || astrHeaderType == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateRetrEmpHeaderBalnFromARBatchByOrgId", new object[2] { icdoEmployerPayrollHeader.org_id, aintBatchScheduleId },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateInsrEmpHeaderBalnFromARBatchByOrgId", new object[2] { icdoEmployerPayrollHeader.org_id, aintBatchScheduleId },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //prod pir 6739 & 6807
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateRetrEmpHeaderUnBalnFromARBatchByOrg", new object[2] { icdoEmployerPayrollHeader.org_id, aintBatchScheduleId },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateInsrEmpHeaderUnBalnFromARBatchByOrg", new object[2] { icdoEmployerPayrollHeader.org_id, aintBatchScheduleId },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                //prod pir 6780
                DBFunction.DBNonQuery("cdoEmployerPayrollHeader.UpdateRetrInsrEmpHeaderNORFromARBatchByOrg", new object[2] { icdoEmployerPayrollHeader.org_id, aintBatchScheduleId },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                //PIR 14938 - Create GL after header status goes to 'Balance'.
                foreach (DataRow ldrHeader in ldtRetrInsrHeader.Rows)
                {
                    busEmployerPayrollHeader lobjHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                    lobjHeader.icdoEmployerPayrollHeader.LoadData(ldrHeader);

                    if (lobjHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        lobjHeader.FindEmployerPayrollHeader(lobjHeader.icdoEmployerPayrollHeader.employer_payroll_header_id);

                        if (lobjHeader.icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusBalanced)
                        {
                            decimal ldecDifferenceAmount = 0;
                            lobjHeader.LoadContributionByPlan();

                            foreach (busEmployerPayrollHeader lobjHeaderByPlan in lobjHeader.iclbRetirementContributionByPlan)
                            {
                                ldecDifferenceAmount = lobjHeaderByPlan.idecTotalCalculatedRetrContributionByPlan - lobjHeaderByPlan.idecTotalReportedRetrContributionByPlan;
                                if (Math.Abs(ldecDifferenceAmount) > 0)
                                {
                                    lobjHeader.CreateGLForDifferenceAmount(busConstant.ItemTypeContribution, ldecDifferenceAmount, lobjHeaderByPlan.iintPlanID);
                                }

                                decimal ldecDifferenceAmountRHIC = 0;
                                ldecDifferenceAmountRHIC = lobjHeaderByPlan.idecTotalCalculatedRHICContributionByPlan - lobjHeaderByPlan.idecTotalReportedRHICContributionByPlan;
                                if (Math.Abs(ldecDifferenceAmountRHIC) > 0)
                                {
                                    lobjHeader.CreateGLForDifferenceAmount(busConstant.ItemTypeRHICContribution, ldecDifferenceAmountRHIC, lobjHeaderByPlan.iintPlanID);
                                }

                                // updating difference amount and difference_type_value
                                busEmployerRemittanceAllocation lbusEmployerRemittanceAllocation = new busEmployerRemittanceAllocation { icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation(), iclbEmployerRemittanceAllocation = new Collection<busEmployerRemittanceAllocation>() };
                                DataTable ldtbusEmployerRemittanceAllocation = Select("cdoEmployerRemittanceAllocation.GetCountOfRemittanceWithAlocSatus",
                                                                       new object[1] { lobjHeader.icdoEmployerPayrollHeader.employer_payroll_header_id });

                                lbusEmployerRemittanceAllocation.iclbEmployerRemittanceAllocation = GetCollection<busEmployerRemittanceAllocation>(ldtbusEmployerRemittanceAllocation, "icdoEmployerRemittanceAllocation");

                                foreach (busEmployerRemittanceAllocation lbusEmployerRemiAllocation in lbusEmployerRemittanceAllocation.iclbEmployerRemittanceAllocation)
                                {
                                    lbusEmployerRemiAllocation.LoadRemittance();
                                }

                                busEmployerRemittanceAllocation lbusEmployerRemittanceAllocationContr = lbusEmployerRemittanceAllocation.iclbEmployerRemittanceAllocation.Where(i => i.ibusRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeContribution && i.ibusRemittance.icdoRemittance.plan_id == lobjHeaderByPlan.iintPlanID).OrderByDescending(k => k.icdoEmployerRemittanceAllocation.employer_remittance_allocation_id).FirstOrDefault();
                                if (lbusEmployerRemittanceAllocationContr.IsNotNull())
                                {
                                    if (Math.Abs(ldecDifferenceAmount) > 0)
                                    {
                                        lbusEmployerRemittanceAllocationContr.icdoEmployerRemittanceAllocation.difference_amount = Math.Abs(ldecDifferenceAmount);
                                        lbusEmployerRemittanceAllocationContr.icdoEmployerRemittanceAllocation.difference_type_value = ldecDifferenceAmount > 0 ? busConstant.GLItemTypeUnderContribution : busConstant.GLItemTypeOverContribution;
                                        lbusEmployerRemittanceAllocationContr.icdoEmployerRemittanceAllocation.Update();
                                    }
                                }

                                busEmployerRemittanceAllocation lbusEmployerRemittanceAllocationRHIC = lbusEmployerRemittanceAllocation.iclbEmployerRemittanceAllocation.Where(i => i.ibusRemittance.icdoRemittance.remittance_type_value == busConstant.ItemTypeRHICContribution && i.ibusRemittance.icdoRemittance.plan_id == lobjHeaderByPlan.iintPlanID).OrderByDescending(k => k.icdoEmployerRemittanceAllocation.employer_remittance_allocation_id).FirstOrDefault();
                                if (lbusEmployerRemittanceAllocationRHIC.IsNotNull())
                                {
                                    if (Math.Abs(ldecDifferenceAmountRHIC) > 0)
                                    {
                                        lbusEmployerRemittanceAllocationRHIC.icdoEmployerRemittanceAllocation.difference_amount = Math.Abs(ldecDifferenceAmountRHIC);
                                        lbusEmployerRemittanceAllocationRHIC.icdoEmployerRemittanceAllocation.difference_type_value = ldecDifferenceAmountRHIC > 0 ? busConstant.GLItemTypeUnderRHIC : busConstant.GLItemTypeOverRHIC;
                                        lbusEmployerRemittanceAllocationRHIC.icdoEmployerRemittanceAllocation.Update();
                                    }
                                }
                            }
                        }
                    }
                }
            }                       
            //--end--//
        }

        private void DeleteInvalidAllocations(string astrHeaderType, int aintBatchScheduleId)
        {
            if (astrHeaderType == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                //query to delete invalid remittance allocation and updates header to unbalanced
                DataTable ldtInvalidRetrAllocations = Select("cdoEmployerRemittanceAllocation.GetInvalidAllocationsForRetirementByOrg", new object[1] { icdoEmployerPayrollHeader.org_id });
                //delet the invalid allocation
                DeleteInvalidAllocationAndUpdateEmpHeader(ldtInvalidRetrAllocations, aintBatchScheduleId);
            }
            else if (astrHeaderType == busConstant.PayrollHeaderBenefitTypeInsr)
            {

                DataTable ldtInvalidInsrAllocations = Select("cdoEmployerRemittanceAllocation.GetInvalidAllocationsorInsuranceByOrg", new object[1] { icdoEmployerPayrollHeader.org_id });
                //delet the invalid allocation
                DeleteInvalidAllocationAndUpdateEmpHeader(ldtInvalidInsrAllocations, aintBatchScheduleId,true);
            }
        }
        /// <summary>
        /// method to delete invalid remittance allocation and change header status to unbalanced
        /// </summary>
        /// <param name="adtInvalidRetrAllocations">allocation result set</param>
        private void DeleteInvalidAllocationAndUpdateEmpHeader(DataTable adtInvalidAllocations,int aintBatchScheduleId,bool ablnIsInsurance = false)
        {
            int lintEmployerPayrollHeaderID = 0;
            foreach (DataRow ldr in adtInvalidAllocations.Rows)
            {
                busEmployerRemittanceAllocation lobjRemittanceAllocation = new busEmployerRemittanceAllocation { icdoEmployerRemittanceAllocation = new cdoEmployerRemittanceAllocation() };
                lobjRemittanceAllocation.ibusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                busEmployerPayrollHeader lobjEmpHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                PERSLinkBatchUser = busConstant.PERSLinkBatchUser + ' ' + aintBatchScheduleId;
                lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.LoadData(ldr);
                lobjRemittanceAllocation.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldr);
                lobjEmpHeader.icdoEmployerPayrollHeader.LoadData(ldr);
                lobjEmpHeader.icdoEmployerPayrollHeader.created_by = ldr["HED_CREATED_BY"] == DBNull.Value ? PERSLinkBatchUser : Convert.ToString(ldr["HED_CREATED_BY"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.created_date = ldr["HED_CREATED_DATE"] == DBNull.Value ?
                    idatRunDate : Convert.ToDateTime(ldr["HED_CREATED_DATE"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.modified_by = ldr["HED_MODIFIED_BY"] == DBNull.Value ? PERSLinkBatchUser : Convert.ToString(ldr["HED_MODIFIED_BY"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.modified_date = ldr["HED_MODIFIED_DATE"] == DBNull.Value ?
                    idatRunDate : Convert.ToDateTime(ldr["HED_MODIFIED_DATE"]);
                lobjEmpHeader.icdoEmployerPayrollHeader.update_seq = ldr["HED_UPDATE_SEQ"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["HED_UPDATE_SEQ"]);
                lobjEmpHeader.iintPlanID = ldr["iintPlanID"] == DBNull.Value ? 0 : Convert.ToInt32(ldr["iintPlanID"]);

                if (lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_amount > 0 &&
                    !string.IsNullOrEmpty(lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_type_value) && ablnIsInsurance) //As discussed on call with Maik, no GL here for retirement headers
                {
                    lobjRemittanceAllocation.CreateGLForDifferenceAmount(lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.difference_amount, lobjEmpHeader.iintPlanID);
                }
                lobjRemittanceAllocation.icdoEmployerRemittanceAllocation.Delete();
                if (lintEmployerPayrollHeaderID != lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id)
                {
                    lobjEmpHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusUnbalanced;
                    lobjEmpHeader.icdoEmployerPayrollHeader.Update();
                }
                lintEmployerPayrollHeaderID = lobjEmpHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;

                lobjEmpHeader = null;
                lobjRemittanceAllocation = null;
            }
        }
        private void AllocateAmount(busEmployerPayrollHeader aobjHeader, busEmployerPayrollHeader aobjHeaderByPlan, DataTable adtRemittance, decimal adecTotalDueAmount)
        {
            decimal ldecAvailableAmount = 0.00M, ldecDifferenceAmount = 0.00M;
            string lstrDifferenceTypeValue = string.Empty;
            foreach (DataRow ldrRem in adtRemittance.Rows)
            {
                busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                lobjRemittance.icdoRemittance.LoadData(ldrRem);

                ldecDifferenceAmount = 0.00M;
                lstrDifferenceTypeValue = string.Empty;
                //getting available amount for remittance
                ldecAvailableAmount = busEmployerReportHelper.GetRemittanceAvailableAmount(lobjRemittance.icdoRemittance.remittance_id);
                if (ldecAvailableAmount > 0)
                {
                    decimal ldecAllocatedAmount = ldecAvailableAmount;
                    if (ldecAllocatedAmount > adecTotalDueAmount)
                        ldecAllocatedAmount = adecTotalDueAmount;

                                       //inserting remittance allocation
                    CreateRemittanceAllocation(aobjHeader.icdoEmployerPayrollHeader.employer_payroll_header_id, lobjRemittance.icdoRemittance.remittance_id,
                        ldecAllocatedAmount, Math.Abs(ldecDifferenceAmount), lstrDifferenceTypeValue);
                    
                    adecTotalDueAmount -= (ldecAllocatedAmount + (ldecDifferenceAmount < 0 ? Math.Abs(ldecDifferenceAmount) : 0.00M));
                    if (adecTotalDueAmount == 0.00M)
                        break;
                }

                lobjRemittance = null;
            }
        }
        private void CreateRemittanceAllocation(int aintEmployerPayrollHeaderID, int aintRemittanceID, decimal adecAllocatedAmount,
            decimal adecDifferenceAmount, string astrDifferenceTypeValue)
        {
            cdoEmployerRemittanceAllocation lcdoRemittanceAllocation = new cdoEmployerRemittanceAllocation();
            lcdoRemittanceAllocation.employer_payroll_header_id = aintEmployerPayrollHeaderID;
            lcdoRemittanceAllocation.remittance_id = aintRemittanceID;
            lcdoRemittanceAllocation.allocated_amount = adecAllocatedAmount;
            lcdoRemittanceAllocation.payroll_allocation_status_value = busConstant.Allocated;
            lcdoRemittanceAllocation.allocated_date = idatRunDate;
            lcdoRemittanceAllocation.difference_amount = adecDifferenceAmount;
            lcdoRemittanceAllocation.difference_type_value = astrDifferenceTypeValue;
            lcdoRemittanceAllocation.Insert();
        }

        #endregion PIR-6501

        #region PIR 23999
        public bool IsPenaltyHeaderExistsOnHeader()
        {
            DataTable ldtbEmployerPayrollHeader = Select<cdoEmployerPayrollHeader>(
                            new string[1] { "original_employer_payroll_header_id" },
                            new object[1] { _icdoEmployerPayrollHeader.employer_payroll_header_id }, null, null);
            if (ldtbEmployerPayrollHeader.IsNotNull() && ldtbEmployerPayrollHeader.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public ArrayList btnSavePenalty_Click()
        {
            ArrayList larrresult = new ArrayList();
            if (    (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr &&
                    icdoEmployerPayrollHeader.total_premium_amount_reported != icdoEmployerPayrollHeader.total_contribution_calculated)
                    || (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
                    icdoEmployerPayrollHeader.total_contribution_reported != icdoEmployerPayrollHeader.total_contribution_calculated)
               )
            {
                utlError lobjError = new utlError();
                lobjError = AddError(icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr ? 4743 : 4742, "");
                larrresult.Add(lobjError);
                return larrresult;
            }
            //Updating �Total Contribution Amount Calculated� and �Total Contribution Amount Reported� value on Save
            _icdoEmployerPayrollHeader.Update();
            btnSaveComments_Click(icdoEmployerPayrollHeader.comments);
            return larrresult;
        }
        public ArrayList btnApplyPenalty_Click()
        {
            ArrayList larrList = new ArrayList();
            //validate Apply penalty -	Multiple Penalty headers should not be allowed for a single Regular/Adjustment header             
            if (IsPenaltyHeaderExistsOnHeader())
            {
                utlError lutleror = new utlError();
                lutleror = AddError(10420, "");
                larrList.Add(lutleror);
                return larrList;
            }
            //Inserting New Header 
            int iintEmployerHeaderPayrollID = CreateNewPenaltyHeader();
            //Inserting message into SGT_COMMENTS table
            //btnSaveComments_Click("Payroll Header " + Convert.ToString(icdoEmployerPayrollHeader.employer_payroll_header_id));
            //inserting workflow Approve Employer Report Penalty PROCESS_ID = 374
            InsertWorkflow(icdoEmployerPayrollHeader.org_id,iintEmployerHeaderPayrollID, 374);
            return larrList;
        }
        public ArrayList btnApprovePenalty_Click()
        {
            ArrayList larrresult = new ArrayList();
            if ((icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr &&
                    icdoEmployerPayrollHeader.total_premium_amount_reported != icdoEmployerPayrollHeader.total_contribution_calculated)
                    || (icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt &&
                    icdoEmployerPayrollHeader.total_contribution_reported != icdoEmployerPayrollHeader.total_contribution_calculated)
               )
            {
                utlError lobjError = new utlError();
                lobjError = AddError(icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr?4743:4742, "");
                larrresult.Add(lobjError);
                return larrresult;
            }
            //Updating Status value to Posted on Approve
            _icdoEmployerPayrollHeader.validated_date = DateTime.Now;
            _icdoEmployerPayrollHeader.posted_date = DateTime.Now;
            _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusPosted;
            _icdoEmployerPayrollHeader.Update();
            //Inserting GL -- Credit � 463045 (Fund 483/480/470) Debit � 137101((Fund 483 / 480), 124001(Fund 470)
            GenerateGLForPenalty();
            return larrresult;
        }
        public void btnCancelPenalty_Click()
        {
            //Updating Status value to Ignored on Cancel
            string lstrTempstatus_value = _icdoEmployerPayrollHeader.status_value;
            _icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusIgnored;
            //_icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusBalanced;
            _icdoEmployerPayrollHeader.Update();
            //Inserting GL -- Credit � 137101 ((Fund 483/480), 124001 (Fund 470)   Debit � 463045(Fund 483 / 480 / 470)
            if(_icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypePenalty && lstrTempstatus_value == busConstant.PayrollHeaderStatusPosted)
                GenerateGLForPenalty(true);
        }
        private int CreateNewPenaltyHeader()
        {
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader() { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader = this.icdoEmployerPayrollHeader;

            //lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id = 0;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.original_employer_payroll_header_id = icdoEmployerPayrollHeader.employer_payroll_header_id;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value = busConstant.PayrollHeaderReportTypePenalty;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value = busConstant.PayrollHeaderStatusReview;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.received_date = DateTime.Now;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date = DateTime.Now;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;            
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_calculated = CalculateTotalContributionForPenalty(
                icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt ? icdoEmployerPayrollHeader.total_contribution_calculated:idecTotalPremiumReportedForIns);

            //Other columns set with null values
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_reported = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_wages_reported = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_wages_calculated = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_interest_reported = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_interest_calculated = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_detail_record_count = 0;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_purchase_amount = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_premium_amount_reported = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_purchase_amount_reported = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.comments = string.Empty;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.validated_date = DateTime.MinValue;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.posted_date = DateTime.MinValue;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.central_payroll_record_id = 0;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.ignore_balancing_status_flag = string.Empty;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.last_reload_run_date = DateTime.MinValue;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.created_by = iobjPassInfo.istrUserID;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.created_date = DateTime.Now;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.modified_by = iobjPassInfo.istrUserID;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.modified_date = DateTime.Now;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.update_seq = 0;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_check_date = DateTime.MinValue;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.suppress_salary_variance_validation_flag = string.Empty;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_wages_original = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_contribution_original = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_purchase_amount_original = 0.0M;
            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.suppress_3rd_payroll_flag = string.Empty;


            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.Insert();
            //Inserting message into SGT_COMMENTS table from same inserting object
            lbusEmployerPayrollHeader.btnSaveComments_Click("Payroll Header " + Convert.ToString(lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.original_employer_payroll_header_id));
            //returning newly inserted payroll header ID
            return lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;
        }
        private decimal CalculateTotalContributionForPenalty(decimal adeTotalContributionCalculated)
        {
            List<utlCodeValue> iclbCodeValue = iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(7023);
            utlCodeValue icdoCodeValue = iclbCodeValue.Where(codeValueRow => Convert.ToDateTime(codeValueRow.data1) <= DateTime.Now).OrderByDescending(codeValueRow=> codeValueRow.data1).FirstOrDefault();
            if (icdoCodeValue.IsNotNull())
                return Convert.ToInt64(icdoCodeValue.data2) + (adeTotalContributionCalculated * Convert.ToDecimal(icdoCodeValue.data3) / 100);
            else
                return 0;
        }

        private void InsertWorkflow(int aintOrgID, int aintReferenceID, int aintProcessID)
        {
            LoadOrgCodeID();
            //cdoWorkflowRequest lcdoWorkflowRequest = new cdoWorkflowRequest
            //{
            //    process_id = aintProcessID,
            //    reference_id = aintReferenceID,
            //    org_code = istrOrgCodeId,
            //    source_value = busConstant.WorkflowProcessSource_Online,
            //    status_value = busConstant.WorkflowProcessStatus_UnProcessed
            //};
            //lcdoWorkflowRequest.Insert();
            busWorkflowHelper.InitiateBpmRequest(aintProcessID, 0, aintOrgID, aintReferenceID, iobjPassInfo,busConstant.WorkflowProcessSource_Online);
            
        }
        private void GenerateGLForPenalty(bool ablnIsCanceled = false)
        {
            cdoAccountReference lcdoAcccountReference = new cdoAccountReference();
            lcdoAcccountReference.plan_id = icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt ? busConstant.PlanIdMain : busConstant.PlanIdGroupHealth;
            lcdoAcccountReference.source_type_value = busConstant.SourceTypeEmployerReporting;
            lcdoAcccountReference.transaction_type_value = busConstant.TransactionTypeItemLevel;
            lcdoAcccountReference.status_transition_value = ablnIsCanceled ? busConstant.GLStatusTransitionValueCanceled : busConstant.GLStatusTransitionApproved;
            //lcdoAcccountReference.item_type_value = icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt ? busConstant.ItemTypeContribution : busConstant.PlanCodeGroupHealth;
            lcdoAcccountReference.item_type_value = busConstant.ItemTypePenalty;
            busGLHelper.GenerateGL(lcdoAcccountReference, 0, _icdoEmployerPayrollHeader.org_id,
                                               _icdoEmployerPayrollHeader.employer_payroll_header_id,
                                               icdoEmployerPayrollHeader.total_contribution_calculated,
                                               GetGLEffectiveDate(),
                                               busGlobalFunctions.GetSysManagementBatchDate(), iobjPassInfo);
        }

        #endregion PRI 23999
        //PIR 25459
        public bool iblnCheckDebitACHRequestExists
        {
            get
            {
                LoadWssDebitAchRequest();
                if (iclbWssDebitAchRequest.Count > 0)
                    return true;
                return false;
            }
        }
        //PIR 25909 sum of Total Contributions Uploaded for Diff comp
        public void LoadTotalContributionDiffComp()
        {
            decimal idecContributionOriginal = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoEmployerPayrollHeader.LoadTotalContributionDeffComp",
                        new object[1] { icdoEmployerPayrollHeader.employer_payroll_header_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            icdoEmployerPayrollHeader.total_contribution_original = idecContributionOriginal;
        }
		//PIR 24918 Update payroll detail to ignore
        public void UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(int aintPersonId, int aintPlanId, DateTime adtmPayPeriod)
        {
            DBFunction.DBNonQuery("entEmployerPayrollHeader.UpdateEmployerPayrollDetailStatusToIgnore", new object[3] {
                                    aintPersonId, aintPlanId, adtmPayPeriod },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        private void SetInterestWaiverFlag()
        {
            //Set the Flag If the Interest Waiver Flag Changed to Recalculate all the Detail Interest and Bonus
            if (_icdoEmployerPayrollHeader.ihstOldValues.Count > 0)
            {
                if ((_icdoEmployerPayrollHeader.ihstOldValues["interest_waiver_flag"] == null) &&
                    (_icdoEmployerPayrollHeader.interest_waiver_flag == busConstant.Flag_Yes))
                {
                    iblnInterestWaiverFlagChanged = true;
                }
                else if (Convert.ToString(_icdoEmployerPayrollHeader.ihstOldValues["interest_waiver_flag"]) != _icdoEmployerPayrollHeader.interest_waiver_flag)
                {
                    iblnInterestWaiverFlagChanged = true;
                }
                else if (_icdoEmployerPayrollHeader.interest_waiver_flag != busConstant.Flag_Yes) //PIR 26010 & PIR 25142
                {
                    iblnInterestWaiverFlagChanged = true;
                }
            }
        }
    }
}
