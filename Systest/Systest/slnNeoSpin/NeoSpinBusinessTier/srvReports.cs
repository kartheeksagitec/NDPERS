#region Using directives

using System;
using System.Data;
using System.Collections;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessTier
{
    public class srvReports : srvNeoSpin
    {
        public srvReports()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public DataSet rptCallCenterMetricsReport(int FromMonth, int FromYear, int ToMonth, int ToYear)
        {
            return new busContactTicket().rptCallCenterMetrics(FromMonth, FromYear, ToMonth, ToYear);
        }

        public DataSet rptCallDistribution(int AssignToUserId, int FromMonth, int FromYear, int ToMonth, int ToYear)
        {
            return new busContactTicket().rptCallDistribution(AssignToUserId, FromMonth, FromYear, ToMonth, ToYear);
        }

        // PROD PIR ID 3253
        public DataSet rptNewDeferredComp(DateTime adtEffectiveDate)
        {
            return new busContactTicket().rptDeferredComp(adtEffectiveDate);
        }

        public DataSet rptOverdueEmployerReport(string PlanID, DateTime StartDate, DateTime EndDate)
        {
            return new busEmployerPayrollHeader().rptOverdueEmployerReport(PlanID, StartDate, EndDate);
        }

        public DataSet rptOutstandingEmployerReport(string astrBenefitType, DateTime adtEffectiveDate)
        {
            return new busEmployerPayrollHeader().rptOutstandingEmployerReport(astrBenefitType, adtEffectiveDate);
        }

        public DataSet rptPayrollSplitFromCentralPayroll(string BenefitType, DateTime StartDate, DateTime EndDate)
        {
            return new busEmployerPayrollHeader().rptPayrollSplitFromCentralPayroll(BenefitType, StartDate, EndDate);
        }

        public DataSet rptSeminarDemograhipics(string SeminarType, DateTime SeminarDateFrom, DateTime SeminarDateTo)
        {
            return new busContactTicket().rptSeminarDemogrhapics(SeminarType, SeminarDateFrom, SeminarDateTo);
        }

        public DataSet rptRetirementContributionConfirmation(string astrOrgCodeID, DateTime adtStartDate, DateTime adtEndDate)
        {
            return new busEmployerPayrollDetail().rptRetirementContributionConfirmation(astrOrgCodeID, adtStartDate, adtEndDate);
        }

        public DataSet rptPayeeReceivableSummary()
        {
            return new busPaymentBenefitOverpaymentHeader().LoadPayeeReceivableSummaryReport();
        }

        public DataSet rptSeminarAttendanceReport(int aintTicketNumber)
        {
            return new busContactTicket().rptSeminarAttendanceReport(aintTicketNumber);
        }

        public DataSet rptUnpaidSeminarReport()
        {
            return new busContactTicket().rptUnpaidSeminarReport();
        }

        public DataSet rptSeminarIDBReport(int aintTicketNumber, int aintsortby) // PIR 8885
        {
            return new busContactTicket().rptSeminarIDBReport(aintTicketNumber, aintsortby); // PIR 8885
        }
        public DataSet rptVestedEmployerContributionErrorReport(DateTime adtPayPeriodDate)
        {
            return new busPersonAccountRetirementContribution().rptVestedEmployerContributionErrorReport(adtPayPeriodDate);
        }
        public DataSet rptMailingLabelAvery(int aintMailingBatchID)
        {
            return new busMailingLabel().GenerateMailingLabelDataSetMVVM(aintMailingBatchID);
        }
        public DataSet rptForm1099R(int aint1009rID)
        {
            return new busPayment1099r().GetDataSetToCreateReportMVVM(aint1009rID);
        }

        public DataSet rptHealthPremiumReport(string astrHealthInsuranceTypeValue, string astrCoverageCode, string astrOrgCode, string astrStructureCode,
                                                    DateTime adteStartDate, DateTime adteEndDate, DateTime adteHistoryDate, int aintPERSLinkID)
        {
            busHealthPremiumReportBatchRequest lobjReport = new busHealthPremiumReportBatchRequest
            {
                icdoHealthPremiumReportBatchRequest = new cdoHealthPremiumReportBatchRequest
                {
                    health_insurance_type_value = astrHealthInsuranceTypeValue,
                    coverage_code = astrCoverageCode,
                    org_id = busGlobalFunctions.GetOrgIdFromOrgCode(astrOrgCode),
                    rate_structure_code = astrStructureCode,
                    plan_start_date = adteStartDate,
                    plan_end_date = adteEndDate,
                    history_date = adteHistoryDate,
                    perslink_id = aintPERSLinkID
                }
            };
            return lobjReport.CreateHealthPremiumReport();
        }

        public DataSet rptRetirementMissingPayrollReport()
        {
            return new busBenefitApplication().rptRetirementMissingPayrollReport();
        }

        public DataSet rptPensionPaymentHistory(int PayeeAccountId, DateTime StartDate, DateTime EndDate)
        {
            return new busPayeeAccountPaymentItemType().rptPensionPaymentHistory(PayeeAccountId, StartDate, EndDate);
        }

        //PIR - 5882
        public DataSet rptPaymentMethodDiscrepancies()
        {
            return new busPerson().LoadPaymentMethodDiscrepancies();
        }

        // PIR - 17242 Displaying First and last comment with respective detail in BIS Error Report
        public DataSet rptPayrollDetailsInReviewForBISErrors()
        {
            return new busPerson().LoadPayrollDetailsInReviewForBISErrors();
        }

        public DataSet GetReportDataSet(ReportModel aclsReportModel)
        {
            DataTable ldataTable = null;
            string lstrReportName = aclsReportModel.ReportName;
            switch (lstrReportName)
            {
                case "rptDuplicateEmail":
                    ldataTable = DBFunction.DBSelect("cdoPerson.rptDuplicateEmail",
                                               new object[0] { }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptFlexCompChangeReport":
                    ldataTable = DBFunction.DBSelect("entPersonAccountFlexComp.rptNonPeopleSoftEmployeesReport",
                                               new object[1] { DateTime.Today }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptFlexEnrollEmployees":
                    ldataTable = DBFunction.DBSelect("entWssPersonEmployment.LoadFlexEnrollEmployees",
                                               new object[1] { aclsReportModel.ORG_ID }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                //case "rptForeignLoginReport":
                //    ldataTable = DBFunction.DBSelect("cdoPerson.rptForeignLoginReport",
                //                               new object[0] { }, iobjPassInfo.iconFramework,
                //                              iobjPassInfo.itrnFramework);
                //    break;
                case "rptIBSTFFRPensionCheckPaymentReport":
                    ldataTable = DBFunction.DBSelect("entPersonAccountPaymentElection.IBSTFFRPensionCheckPaymentReport",
                                               new object[1] { aclsReportModel.PAYMENTDATE }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptLeaveOfAbsence":
                    ldataTable = DBFunction.DBSelect("entWssPersonEmployment.LeaveOfAbsence",
                                               new object[1] { aclsReportModel.ORG_ID }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptLifeEnrollEmployees":
                    ldataTable = DBFunction.DBSelect("entWssPersonEmployment.LoadLifeEnrollEmployees",
                                               new object[1] { aclsReportModel.ORG_ID }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptLifeLevelofCoverage":
                    ldataTable = DBFunction.DBSelect("entWssPersonEmployment.LoadLifeInsuranceLevelofCoverage",
                                               new object[1] { aclsReportModel.ORG_ID }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptMaritalStatusChange":
                    ldataTable = DBFunction.DBSelect("entPerson.rptMaritalStatusChange",
                                               new object[2] { aclsReportModel.Week_Start_Date, aclsReportModel.Week_End_Date }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptMedicareSplitErrorReport":
                    ldataTable = DBFunction.DBSelect("entPersonAccountGhdv.rptMedicareSplitErrorReport",
                                               new object[1] { aclsReportModel.currentdate }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptMemberPlanAccountDetails":
                    ldataTable = DBFunction.DBSelect("entPersonAccount.rptMemberPlanAccountDetails",
                                               new object[0] { }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptMissingContributions":
                    ldataTable = DBFunction.DBSelect("entWssPersonEmployment.LoadMissingRetirementContributions",
                                               new object[0] { }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptMissingContributionsDeferredComp":
                    ldataTable = DBFunction.DBSelect("entWssPersonEmployment.LoadMissingContributionsDeferredComp",
                                               new object[0] { }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptNonZeroPCDReport":
                    ldataTable = DBFunction.DBSelect("entPerson.rptNonZeroPCDReport",
                                               new object[0] { }, iobjPassInfo.iconFramework,
                                              iobjPassInfo.itrnFramework);
                    break;
                case "rptOverduePremiumAging":
                    ldataTable = DBFunction.DBSelect("entIbsHeader.rptOverDuePremiumAging",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptPensionCheckPaymentReport":
                    ldataTable = DBFunction.DBSelect("entPersonAccountPaymentElection.PensionCheckPaymentReport",
                                                   new object[1] { aclsReportModel.PAYMENTDATE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptPremiumAdjustmentReceivable":
                    ldataTable = DBFunction.DBSelect("entPaymentElectionAdjustment.rptPremiumAdjustmentReceivable",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptPurchasePaymentDetail":
                    ldataTable = DBFunction.DBSelect("entServicePurchaseHeader.rptPurchasePaymentDetail",
                                                   new object[4] { aclsReportModel.Begin_Date, aclsReportModel.To_Date, aclsReportModel.PaymentClass, aclsReportModel.PayorValue }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptRetireeHealthCredit":
                    ldataTable = DBFunction.DBSelect("entIbsHeader.rptRetireeHealthCredit",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptReturnToWork":
                    ldataTable = DBFunction.DBSelect("entPayeeAccount.ReturnToWorkReport",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptYearly415PurchaseLimit":
                    ldataTable = DBFunction.DBSelect("entServicePurchaseHeader.RptYearly415PurchaseLimit",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptYearlyDCTransferReport":
                    ldataTable = DBFunction.DBSelect("entBenefitApplication.rptYearlyDCTransferReport",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptRequirement":
                    ldataTable = DBFunction.DBSelect("entRequirement.rptRequirement",
                                                   new object[1] { aclsReportModel.CATEGORY_VALUE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptResourceBySecurityRole":
                    ldataTable = DBFunction.DBSelect("entRoles.rptResourceBySecurityRole",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptRoleByUser":
                    ldataTable = DBFunction.DBSelect("entUser.rptRoleByUser",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptAppointments":
                    ldataTable = DBFunction.DBSelect("entAppointmentSchedule.rptAppointments",
                                                   new object[4] { aclsReportModel.start_date, aclsReportModel.end_date, aclsReportModel.ddlAppointmentType, aclsReportModel.ddlCounselor }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptActivitiesInStatusExceeding3Days":
                    ldataTable = DBFunction.DBSelect("entBpmActivityInstance.rptActivitiesInStatusExceeding3Days",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptWorkFlowMetrics":
                    ldataTable = DBFunction.DBSelect("entSolBpmActivityInstance.rptWorkFlowMetrics",
                                                   new object[8] { aclsReportModel.startTime, aclsReportModel.endTime,
                                                                    aclsReportModel.typeId, aclsReportModel.qualifiedName,
                                                                    aclsReportModel.userID, aclsReportModel.personID,
                                                                    aclsReportModel.orgID,aclsReportModel.status }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptListOfScheduledSeminars":
                    ldataTable = DBFunction.DBSelect("entSeminarSchedule.rptListOfScheduledSeminars",
                                                   new object[4] { aclsReportModel.ddlFacilitator, aclsReportModel.ddlSeminarType, aclsReportModel.start_date, aclsReportModel.end_date }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptDeferredCompPayrollContribution":
                    ldataTable = DBFunction.DBSelect("entEmployerPayrollHeader.rptDeferredCompPayrollContributionReport",
                                                   new object[2] { aclsReportModel.StartDate, aclsReportModel.EndDate }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptEmployerReceivableAgingReport":
                    ldataTable = DBFunction.DBSelect("entEmployerPayrollHeader.rptEmployerReceivableAging",
                                                   new object[1] { aclsReportModel.Header_Type_Value }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptVestedEmployerContributionSummary":
                    ldataTable = DBFunction.DBSelect("entPersonAccountRetirementContribution.rptVestedEmployerContributionSummary",
                                                   new object[2] { aclsReportModel.start_date, aclsReportModel.end_date }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptUnPaidPreRetirementDeathBeneficiary":
                    ldataTable = DBFunction.DBSelect("entBenefitApplication.rptUnPaidPreRetirementDeathBeneficiary",
                                                   new object[2] { aclsReportModel.DateofDeathFrom, aclsReportModel.DateofDeathTo }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptAdhocIncreaseOrSupplementalCheckReport":
                    ldataTable = DBFunction.DBSelect("entPostRetirementIncreaseBatchRequest.rptAdhocIncreaseOrSupplementalCheckReport",
                                                   new object[2] { aclsReportModel.POST_RETIREMENT_INCREASE_TYPE_VALUE, aclsReportModel.EFFECTIVE_DATE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptChildSupport":
                    ldataTable = DBFunction.DBSelect("entProviderReportPayment.ChildSupportReport",
                                                   new object[1] { aclsReportModel.PAYMENTDATE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptDuesWithholding":
                    ldataTable = DBFunction.DBSelect("entPayeeAccount.FinalDuesWithholdingReport",
                                                   new object[1] { aclsReportModel.PAYMENTDATE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptDuplicateACHDetail":
                    ldataTable = DBFunction.DBSelect("cdoPayeeAccountAchDetail.rptDuplicateACHDetailReport",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptEscheatToStateReport":
                    ldataTable = DBFunction.DBSelect("entPayeeAccount.rptEscheatToStateReport",
                                                   new object[2] { aclsReportModel.FROM_DATE, aclsReportModel.To_Date }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptJobServiceCOLAReportfor3rdPartPayor":
                    ldataTable = DBFunction.DBSelect("entPostRetirementIncreaseBatchRequest.rptJobServiceCOLAReportfor3rdPartPayor",
                                                   new object[1] { aclsReportModel.EFFECTIVE_DATE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptOverPaymentReport":
                    ldataTable = DBFunction.DBSelect("entPaymentBenefitOverpaymentHeader.rptOverPaymentReport",
                                                   new object[1] { aclsReportModel.APPROVAL_YEAR }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptPaymentHistoryDistributionStatus":
                    ldataTable = DBFunction.DBSelect("entPaymentHistoryDistribution.rptPaymentHistoryDistributionStatusReport",
                                                   new object[3] { aclsReportModel.status, aclsReportModel.start_date, aclsReportModel.end_date }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptPaymentListingReport":
                    ldataTable = DBFunction.DBSelect("entPaymentHistoryHeader.rptPaymentListingReport",
                                                   new object[3] { aclsReportModel.status, aclsReportModel.StartDate, aclsReportModel.EndDate }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptSummaryreportofalladhocpaymentsforaMonth":
                    ldataTable = DBFunction.DBSelect("entPaymentHistoryHeader.SummaryreportofalladhocpaymentsforaMonth",
                                                   new object[3] { aclsReportModel.StartDate, aclsReportModel.EndDate, aclsReportModel.SCHEDULETYPE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case busConstant.ReportNameMissingRetirementEnrollment:
                    ldataTable = DBFunction.DBSelect("entWssPersonEmployment.LoadMissingRetirementEnrollment",
                                                   new object[1] { aclsReportModel.ORG_ID }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case busConstant.ReportNamePopupPayeeAccount:
                    ldataTable = DBFunction.DBSelect("entPayeeAccount.rptPopupPayeeAccount",
                                                   new object[2] { aclsReportModel.PROCESS_INSTANCE_CREATED_DATE1, aclsReportModel.PROCESS_INSTANCE_CREATED_DATE2 }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case busConstant.ReportNameHighRiskNewPayees:
                    ldataTable = DBFunction.DBSelect("entPayeeAccount.QueryHighRiskNewPayees",
                                                   new object[1] { aclsReportModel.PAYMENTDATE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case busConstant.ReportNameGLReportForAudit:
                    ldataTable = DBFunction.DBSelect("entGLTransaction.GetGLDetailsForMonthlyAudit",
                                                   new object[4] { aclsReportModel.EXTRACT_DATE_From, aclsReportModel.EXTRACT_DATE_To,
                                                       aclsReportModel.Fund_ID, aclsReportModel.Account_Code }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case busConstant.ReportNameContributionFileForAudit:
                    ldataTable = DBFunction.DBSelect("entEmployerPayrollDetail.GetContributionFileForAudit",
                                                   new object[3] { aclsReportModel.POSTED_DATE_FROM, aclsReportModel.POSTED_DATE_TO, aclsReportModel.PlanID }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case busConstant.ReportNameBenefitPaymentChangeDetails:
                    ldataTable = DBFunction.DBSelect("entPayeeAccount.rptBenefitPaymentChangeDetailsReport",
                                                   new object[1] { aclsReportModel.PAYMENTDATE }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptPersonReached401aLimit":
                    ldataTable = DBFunction.DBSelect("entPerson.LoadPersons401aLimitReached",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                case "rptDROAlternatePayeesPendingUpcomingPayment":
                    ldataTable = DBFunction.DBSelect("entBenefitDroApplication.LoadDROAlternatePayeesPendingUpcomingPayment",
                                                   new object[0] { }, iobjPassInfo.iconFramework,
                                                  iobjPassInfo.itrnFramework);
                    break;
                default:
                    ldataTable = new DataTable();
                    break;
            }
            ldataTable.TableName = "ReportTable01";
            DataSet ldataset = new DataSet();
            ldataset.Tables.Add(ldataTable.Copy());
            return ldataset;
        }

        public ArrayList GenerateReportDataSet(string astrReportPath, ReportModel aclsReportModel)
        {
            DataSet ldtDataSet = new DataSet();
            string lstrReportPath = astrReportPath;
            string lstrReportName = aclsReportModel.ReportName;
            switch (lstrReportName)
            {

                case "rptPaymentMethodDiscrepancies":
                    ldtDataSet = rptPaymentMethodDiscrepancies();
                    break;
                case "rptPayrollDetailsInReviewForBISErrors":
                    ldtDataSet = rptPayrollDetailsInReviewForBISErrors();
                    break;
                case "rptHealthPremiumReport":
                    ldtDataSet = rptHealthPremiumReport(aclsReportModel.astrHealthInsuranceTypeValue, aclsReportModel.astrCoverageCode, 
                                                        aclsReportModel.astrOrgCode, aclsReportModel.astrStructureCode, 
                                                        aclsReportModel.adteStartDate, aclsReportModel.adteEndDate, 
                                                        aclsReportModel.adteHistoryDate, Convert.ToInt32(aclsReportModel.aintPERSLinkID));
                    break;
                case "rptMailingLabelAvery":
                    ldtDataSet = rptMailingLabelAvery(Convert.ToInt32(aclsReportModel.aintMailingBatchID));
                    break;
                case "rptCallDistribution":
                    ldtDataSet = rptCallDistribution(Convert.ToInt32(aclsReportModel.AssignToUserId), Convert.ToInt32(aclsReportModel.FromMonth), Convert.ToInt32(aclsReportModel.FromYear), Convert.ToInt32(aclsReportModel.ToMonth), Convert.ToInt32(aclsReportModel.ToYear));
                    break;
                case "rptCallCenterMetricsReport":
                    ldtDataSet = rptCallCenterMetricsReport(Convert.ToInt32(aclsReportModel.FromMonth), Convert.ToInt32(aclsReportModel.FromYear), Convert.ToInt32(aclsReportModel.ToMonth), Convert.ToInt32(aclsReportModel.ToYear));
                    break;
                case "rptDeferredComp":
                    ldtDataSet = rptNewDeferredComp(aclsReportModel.adtEffectiveDate);
                    break;
                case "rptSeminarAttendance":
                    ldtDataSet = rptSeminarAttendanceReport(Convert.ToInt32(aclsReportModel.aintTicketNumber));
                    break;
                case "rptSeminarDemogrhapics":
                    ldtDataSet = rptSeminarDemograhipics(aclsReportModel.SeminarType, aclsReportModel.SeminarDateFrom, aclsReportModel.SeminarDateTo);
                    break;
                case "rptSeminarIDB":
                    ldtDataSet = rptSeminarIDBReport(Convert.ToInt32(aclsReportModel.aintTicketNumber), Convert.ToInt32(aclsReportModel.aintsortby));
                    break;
                case "rptUnpaidSeminar":
                    ldtDataSet = rptUnpaidSeminarReport();
                    break;
                case "rptOutstandingEmployerReport":
                    ldtDataSet = rptOutstandingEmployerReport(aclsReportModel.astrBenefitType, aclsReportModel.adtEffectiveDate);
                    break;
                case "rptOverdueEmployerReport":
                    ldtDataSet = rptOverdueEmployerReport(aclsReportModel.PlanID, aclsReportModel.StartDate, aclsReportModel.EndDate);
                    break;
                case "rptPayrollReportSplitFromCentralPayroll":
                    ldtDataSet = rptPayrollSplitFromCentralPayroll(aclsReportModel.BenefitType, aclsReportModel.StartDate, aclsReportModel.EndDate);
                    break;
                case "rptRetirementContributionConfirmation":
                    ldtDataSet = rptRetirementContributionConfirmation(aclsReportModel.astrOrgCodeID, aclsReportModel.adtStartDate, aclsReportModel.adtEndDate);
                    break;
                case "rptVestedEmployerContributionErrorReport":
                    ldtDataSet = rptVestedEmployerContributionErrorReport(aclsReportModel.adtPayPeriodDate);
                    break;
                case "rptPayeeReceivableSummary":
                    ldtDataSet = rptPayeeReceivableSummary();
                    break;
                case "rptPensionPaymentHistory":
                    ldtDataSet = rptPensionPaymentHistory(Convert.ToInt32(aclsReportModel.PayeeAccountId), aclsReportModel.StartDate, aclsReportModel.EndDate);
                    break;
                case "rptRetirementMissingPayrollReport":
                    ldtDataSet = rptRetirementMissingPayrollReport();
                    break;
                case "rptDuplicateEmail":
                case "rptFlexCompChangeReport":
                case "rptFlexEnrollEmployees":
                case "rptForeignLoginReport":
                case "rptIBSTFFRPensionCheckPaymentReport":
                case "rptLeaveOfAbsence":
                case "rptLifeEnrollEmployees":
                case "rptLifeLevelofCoverage":
                case "rptMaritalStatusChange":
                case "rptMedicareSplitErrorReport":
                case "rptMemberPlanAccountDetails":
                case "rptMissingContributions":
                case "rptMissingContributionsDeferredComp":
                case "rptNonZeroPCDReport":
                case "rptOverduePremiumAging":
                case "rptPensionCheckPaymentReport":
                case "rptPremiumAdjustmentReceivable":
                case "rptPurchasePaymentDetail":
                case "rptRetireeHealthCredit":
                case "rptReturnToWork":
                case "rptYearly415PurchaseLimit":
                case "rptYearlyDCTransferReport":
                case "rptRequirement":
                case "rptResourceBySecurityRole":
                case "rptRoleByUser":
                case "rptAppointments":
                case "rptActivitiesInStatusExceeding3Days":
                case "rptWorkFlowMetrics":
                case "rptListOfScheduledSeminars":
                case "rptEmployerReceivableAgingReport":
                case "rptVestedEmployerContributionSummary":
                case "rptUnPaidPreRetirementDeathBeneficiary":
                case "rptAdhocIncreaseOrSupplementalCheckReport":
                case "rptChildSupport":
                case "rptDuesWithholding":
                case "rptDuplicateACHDetail":
                case "rptEscheatToStateReport":
                case "rptJobServiceCOLAReportfor3rdPartPayor":
                case "rptOverPaymentReport":
                case "rptPaymentHistoryDistributionStatus":
                case "rptPaymentListingReport":
                case "rptSummaryreportofalladhocpaymentsforaMonth":
                case "rptDeferredCompPayrollContribution":
                case busConstant.ReportNameMissingRetirementEnrollment:
                case busConstant.ReportNamePopupPayeeAccount:
                case busConstant.ReportNameHighRiskNewPayees:
                case busConstant.ReportNameGLReportForAudit:
                case busConstant.ReportNameContributionFileForAudit:
                case busConstant.ReportNameBenefitPaymentChangeDetails:
                case "rptPersonReached401aLimit":
                case "rptDROAlternatePayeesPendingUpcomingPayment":
                    ldtDataSet = GetReportDataSet(aclsReportModel);
                    break;
            }
            return busGlobalFunctions.GenerateReportDataSet(iobjPassInfo, ldtDataSet, aclsReportModel, lstrReportPath);
        }
    }
}

