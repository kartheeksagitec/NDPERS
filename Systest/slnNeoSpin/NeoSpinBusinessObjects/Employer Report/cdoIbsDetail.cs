#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoIbsDetail : doIbsDetail
	{
		public cdoIbsDetail() : base()
		{
		}

        //ucs - 038 - addendum : correspondence
        public decimal idecChargedPremium { get; set; }
        //ucs - 038 - addendum : correspondence
        public decimal idecCorrectPremium { get; set; }

        //PROD PIR 2422
        public string billing_month_year_long_date_format
        {
            get
            {
                return billing_month_and_year.ToString(BusinessObjects.busConstant.DateFormatLongDate);
            }
        }

        public string header_report_type_value { get; set; }

        // PROD PIR ID 4717
        public override bool AuditColumn(string astrOperation, string astrColumnName)
        {
            if (header_report_type_value == BusinessObjects.busConstant.IBSHeaderReportTypeAdjustment)
                return true;
            else
                return false;
        }

        //prod pir 6076
        public string istrReportType { get; set; }
        public string istrProviderOrgCode { get; set; }

        public decimal idecLow_Income_Credit { get; set; }
        public int iintPremiumForPersonId { get; set; }//Org to bill
        public int payee_account_id_papit { get; set; }//PIR 8022

        public string istrPersonAccountParticipationStatus { get; set; } // PIR 26419

    } 
} 
