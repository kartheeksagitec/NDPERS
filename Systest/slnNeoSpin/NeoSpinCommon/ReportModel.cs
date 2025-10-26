using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoSpin.Common
{
    [Serializable]
    public class ReportModel
    {
        public ReportModel() { }

        public string ReportName { get; set; }
        public int? ORG_ID { get; set; }
        public string astrBenefitType { get; set; }
        public string PlanID { get; set; }
        public int? PayeeAccountId { get; set; }
        public string astrOrgCodeID { get; set; }
        public DateTime adtStartDate { get; set; }
        public DateTime adtEndDate { get; set; }
        public string astrHealthInsuranceTypeValue { get; set; }
        public string astrCoverageCode { get; set; }
        public string astrOrgCode { get; set; }
        public string astrStructureCode { get; set; }
        public DateTime adteStartDate { get; set; }
        public DateTime adteEndDate { get; set; }
        public DateTime adteHistoryDate { get; set; }
        public int? aintPERSLinkID { get; set; }
        public int? aintMailingBatchID { get; set; }
        public int? AssignToUserId { get; set; }
        public int? FromMonth { get; set; }
        public int? FromYear { get; set; }
        public int? ToMonth { get; set; }
        public int? ToYear { get; set; }
        public DateTime adtEffectiveDate { get; set; }
        public int? aintTicketNumber { get; set; }
        public string SeminarType { get; set; }
        public DateTime SeminarDateFrom { get; set; }
        public DateTime SeminarDateTo { get; set; }
        public int? aintsortby { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BenefitType { get; set; }
        public DateTime adtPayPeriodDate { get; set; }
        public DateTime PAYMENTDATE { get; set; }
        public DateTime Week_Start_Date { get; set; }
        public DateTime Week_End_Date { get; set; }
        public DateTime currentdate { get; set; }
        public DateTime Begin_Date { get; set; }
        public DateTime To_Date { get; set; }
        public string PaymentClass { get; set; }
        public string PayorValue { get; set; }
        public string CATEGORY_VALUE { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string ddlCounselor { get; set; }
        public string ddlAppointmentType { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string typeId { get; set; }
        public string qualifiedName { get; set; }
        public string userID { get; set; }
        public string status { get; set; }
        public string personID { get; set; }
        public string orgID { get; set; }
        public string ddlFacilitator { get; set; }
        public string ddlSeminarType { get; set; }
        public string Header_Type_Value { get; set; }
        public DateTime DateofDeathFrom { get; set; }
        public DateTime DateofDeathTo { get; set; }
        public DateTime EFFECTIVE_DATE { get; set; }
        public string POST_RETIREMENT_INCREASE_TYPE_VALUE { get; set; }
        public DateTime FROM_DATE { get; set; }
        public int APPROVAL_YEAR { get; set; }
        public string SCHEDULETYPE { get; set; }
        public DateTime PROCESS_INSTANCE_CREATED_DATE1 { get; set; }
        public DateTime PROCESS_INSTANCE_CREATED_DATE2 { get; set; }
        public DateTime EXTRACT_DATE_From { get; set; }
        public DateTime EXTRACT_DATE_To { get; set; }
        public string Fund_ID { get; set; }
        public string Account_Code { get; set; }

        public DateTime POSTED_DATE_FROM { get; set; }
        public DateTime POSTED_DATE_TO { get; set; }

    }
}
