using System;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using System.Data;
using NeoSpin.CustomDataObjects;
using System.Linq;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBNDClearedCheckFileInbound : busFileBase
    {
        public busBNDClearedCheckFileInbound()
        {

        }
        bool iblnIsValid = false;
        //property to store the data read from file
        public busPaymentHistoryDistribution ibusPaymentHistoryDistribution { get; set; }

        public override void InitializeFile()
        {
            //Load the data which are in outstanding status
            busBase lobjBase = new busBase();
            DataTable ldtbOutStandingChecks = busBase.Select("cdoPaymentHistoryDistribution.fleBNDClearedCheckFileInbound", new object[0] { });
            iclbPaymentHistoryDitribution = lobjBase.GetCollection<busPaymentHistoryDistribution>(ldtbOutStandingChecks, "icdoPaymentHistoryDistribution");
            base.InitializeFile();
        }
        //property to load the data which are in outstanding status
        public Collection<busPaymentHistoryDistribution> iclbPaymentHistoryDitribution { get; set; }
        public override busBase NewDetail()
        {
            ibusPaymentHistoryDistribution = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
            return ibusPaymentHistoryDistribution;
        }
        //if statement date exists in file ,assign to transaction date
        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrObjectField;
            string lstrReturnValue = astrFieldValue;
            if (astrFieldName.IndexOf(".") > -1)
                lstrObjectField = astrFieldName.Substring(astrFieldName.LastIndexOf(".") + 1);
            else
                lstrObjectField = astrFieldName;

            if (lstrObjectField == "status_effective_date")
            {
                if (lstrReturnValue.Trim() != string.Empty)
                    lstrReturnValue = DateTime.ParseExact(lstrReturnValue, "MMddyy", null).ToString("MMddyyyy");
            }
            if (lstrObjectField == "check_number")
            {
                lstrReturnValue = lstrReturnValue.Trim();
            }
            if (lstrObjectField == "net_amount")
            {
                lstrReturnValue = lstrReturnValue.Trim();
            }
            if (lstrObjectField == "account_number")
            {
                lstrReturnValue = lstrReturnValue.Trim();
            }
            return lstrReturnValue;
        }
        //Process every record in the inbound file and Do the validation
        //if the check is valid and cleared from and update the status of check
        public override void ProcessDetail()
        {
            utlError lobjError = new utlError();
            ArrayList larrError = new ArrayList();
            //if the file has check number column blank ,then throw an error
            if (string.IsNullOrEmpty(ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number))
            {
                lobjError = new utlError { istrErrorID = "8101", istrErrorMessage = "Invalid Check Number" };
                larrError.Add(lobjError);
                ibusPaymentHistoryDistribution.iarrErrors = larrError;
                return;
            }//  
            else
            {
                if (iclbPaymentHistoryDitribution != null && iclbPaymentHistoryDitribution.Count > 0)
                {
                    string lstrDateValue = DateTime.ParseExact(ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.status_effective_date,
                        "MMddyyyy", null).ToString("MM/dd/yyyy");
                    DateTime ldtStatusEffecttive = Convert.ToDateTime(lstrDateValue);
                    //Get Distribution record for given check number
                    busPaymentHistoryDistribution lobjPaymentHistoryDistribution = iclbPaymentHistoryDitribution.Where(o =>
                        o.icdoPaymentHistoryDistribution.check_number.ToString().PadLeft(11,'0') == //PROD Pir - 4596
                        ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.check_number).FirstOrDefault();
                    //if there is no record for given check number , then throw an error
                    if (lobjPaymentHistoryDistribution == null)
                    {
                        lobjError = new utlError { istrErrorID = "8104", istrErrorMessage = "There is no record of this check number in the system" };
                        larrError.Add(lobjError);
                        ibusPaymentHistoryDistribution.iarrErrors = larrError;
                        return;
                    }//if record exists for given check number but not in outstanding status - means it is already cleared and cancelled
                    else if (lobjPaymentHistoryDistribution.IsNotNull() && !((lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value ==
                        busConstant.CHKOutstanding) || (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RCHKOutstanding)))
                    {
                        lobjError = new utlError { istrErrorID = "8102", istrErrorMessage = "Invalid record due to Duplicate Check Number" };
                        larrError.Add(lobjError);
                        ibusPaymentHistoryDistribution.iarrErrors = larrError;
                        return;
                    }//if record exists for given check number 
                    else if (lobjPaymentHistoryDistribution.IsNotNull() && ((lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount * 100) !=
                        ibusPaymentHistoryDistribution.icdoPaymentHistoryDistribution.net_amount))
                    {
                        lobjError = new utlError { istrErrorID = "8103", istrErrorMessage = "Invalid record due to Amount Discrepancy" };
                        larrError.Add(lobjError);
                        ibusPaymentHistoryDistribution.iarrErrors = larrError;
                        return;
                    }///if the validations succeed,update the check status to cleared and distribution Status history
                    if (ibusPaymentHistoryDistribution.iarrErrors.Count == 0)
                    {
                        if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                        {
                            lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKCleared;
                            lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();
                            lobjPaymentHistoryDistribution.CreateHistory(ldtStatusEffecttive);
                        }
                        if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                        {
                            lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKCleared;
                            lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();
                            lobjPaymentHistoryDistribution.CreateHistory(ldtStatusEffecttive);
                        }
                        if (lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader == null)
                            lobjPaymentHistoryDistribution.LoadPaymentHistoryHeader();
                        lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value = busConstant.PaymentStatusProcessed;
                        lobjPaymentHistoryDistribution.ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.Update();
                    }
                }
            }
        }
        public override bool ValidateFile()
        {
            iblnIsValid = base.ValidateFile();
            return iblnIsValid;
        }
        public override void FinalizeFile()
        {

            busNeoSpinBase lobjNeospinBase = new busNeoSpinBase();
            //The system must generate the ‘Payment History Distribution Status’ report defined is UCS-078 for all checks with 
            //a status other than ‘Cleared’ and ‘Outstanding’ with ‘Transaction Date’ between the first day of the previous month through the last day of the previous month inclusive..
            DataTable ldtbStatusReport = busBase.Select("cdoPaymentHistoryDistribution.rptPaymentHistoryDistributionStatusReportOtherThanOutstandingAndCleared",
                                                                    new object[1] { busPayeeAccountHelper.GetLastBenefitPaymentDate() });
            if (ldtbStatusReport.Rows.Count > 0)
            {
                lobjNeospinBase.CreateReport("rptPaymentHistoryDistributionStatus.rpt", ldtbStatusReport,string.Empty);
            }
            //The system must generate a report of all checks in the system in an ‘Outstanding’ status with a ‘Transaction Date’ 
            //less than or equal to the last day of the previous month.
           
            DataTable ldtbOustandingReport = busBase.Select("cdoPaymentHistoryDistribution.rptOutstandingChecks",
                                                                  new object[1] { busPayeeAccountHelper.GetLastBenefitPaymentDate() });
            if (ldtbOustandingReport.Rows.Count > 0)
            {
                lobjNeospinBase.CreateReport("rptOutstandingChecksReport.rpt", ldtbOustandingReport,string.Empty);
            }
           
            base.FinalizeFile();
        }
    }
}