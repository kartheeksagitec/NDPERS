#region Using directives

using System;
using System.Data;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using System.Collections.ObjectModel;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvCase : srvNeoSpin
    {
        public srvCase()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        protected override ArrayList ValidateNew(string astrFormName, Hashtable ahstParam)
        {
            ArrayList larrErrors = null;
           // iobjPassInfo.iconFramework.Open();
            try
            {
                if (astrFormName == "wfmCaseManagementLookup")
                {
                    busCaseLookup lbusCase = new busCaseLookup();
                    larrErrors = lbusCase.ValidateNew(ahstParam);
                }
            }
            finally
            {
                //iobjPassInfo.iconFramework.Close();
            }
            return larrErrors;
        }

        public busCase NewCase(int AintPersonId, string astrCaseType, string astrAppealType)
        {
            busCase lobjCase = new busCase();
            lobjCase.icdoCase = new cdoCase();
            lobjCase.icdoCase.person_id = AintPersonId;
            lobjCase.icdoCase.case_type_value = astrCaseType;
            lobjCase.icdoCase.case_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2254, astrCaseType);
            lobjCase.LoadPerson();

            if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypeBenefitAppeal)
            {
                lobjCase.icdoCase.appeal_type_value = astrAppealType;
                lobjCase.icdoCase.appeal_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2256, astrAppealType);
            }
            else if ((lobjCase.icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
                || (lobjCase.icdoCase.case_type_value == busConstant.CaseTypePre1991DisabilityRecertification))
            {
                lobjCase.SetPayeeAccountIDAndPlan();
                lobjCase.LoadPayeeAccount();
                lobjCase.icdoCase.recertification_date = lobjCase.ibusPayeeAccount.icdoPayeeAccount.recertification_date;
                lobjCase.icdoCase.next_recertification_date = lobjCase.icdoCase.recertification_date.AddMonths(18);
                lobjCase.icdoCase.income_verification_date = lobjCase.icdoCase.recertification_date;
                lobjCase.icdoCase.case_status_value = busConstant.CaseStatusValuePendingMember;
                lobjCase.icdoCase.case_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2256, lobjCase.icdoCase.case_status_value);
            }
            else if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypeFinancialHardship)
                lobjCase.LoadDefCompProviderDetails();

            if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
            {
                lobjCase.LoadIncomeVerfication();
                lobjCase.SetComparableEarnings();
                lobjCase.SetIsComparableEarningExceedFlag();
            }

            lobjCase.LoadStepDetailsNewMode();
            lobjCase.LoadNotess();

            lobjCase.EvaluateInitialLoadRules();
            return lobjCase;
        }
        public busCase FindCase(int Aintcaseid)
        {
            busCase lobjCase = new busCase();
            if (lobjCase.FindCase(Aintcaseid))
            {
                lobjCase.LoadCaseStepDetails();
                lobjCase.LoadPerson();
                lobjCase.LoadCaseFileDetails();
                lobjCase.LoadFilenetImagesFromDB(false);
                lobjCase.LoadNotess();
                lobjCase.AssignCollectionByCaseType(lobjCase.iclbCaseStepDetail);
                lobjCase.SetOverAllTime(lobjCase.iclbCaseStepDetail);
                lobjCase.LoadEndDatedStepDetails();

                if ((lobjCase.icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
                    || (lobjCase.icdoCase.case_type_value == busConstant.CaseTypePre1991DisabilityRecertification))
                {
                    lobjCase.SetPayeeAccountIDAndPlan();
                    lobjCase.LoadPayeeAccount();
                }
                else if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypeFinancialHardship)
                {
                    lobjCase.LoadCaseFinancialHardshipProviderDetails();
                }
                if (lobjCase.icdoCase.case_type_value == busConstant.CaseTypeDisabilityRecertification)
                {
                    lobjCase.LoadCaseDisabilityIncomeVerificationDetails();
                    if (lobjCase.icdoCase.comparable_earnings_amount == 0M)
                        lobjCase.SetComparableEarnings();
                    lobjCase.SetIsComparableEarningExceedFlag();
                }
                lobjCase.CheckMemberEnrollment();
                lobjCase.LoadRetDisAppForCorres(); //PIR 15831 - APP 7007 template to be associated with case management screen too
            }
            return lobjCase;
        }

        public busCaseLookup LoadCases(DataTable adtbSearchResult)
        {
            busCaseLookup lobjCaseLookup = new busCaseLookup();
            lobjCaseLookup.LoadCases(adtbSearchResult);
            return lobjCaseLookup;
        }
        public busCaseFileDetail FindCaseFileDetail(int Aintcasefiledetailid)
        {
            busCaseFileDetail lobjCaseFileDetail = new busCaseFileDetail();
            if (lobjCaseFileDetail.FindCaseFileDetail(Aintcasefiledetailid))
            {
            }

            return lobjCaseFileDetail;
        }
    }
}
