#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoWssMessageDetail:
	/// Inherited from doWssMessageDetail, the class is used to customize the database object doWssMessageDetail.
	/// </summary>
    [Serializable]
	public class cdoWssMessageDetail : doWssMessageDetail
	{
		public cdoWssMessageDetail() : base()
		{
		}

        public string istrCorrespondenceLink
        {
            get
            {
                if (!string.IsNullOrEmpty(correspondence_link))
                {
                    if(!string.IsNullOrEmpty(clear_message_flag) && clear_message_flag == busConstant.Flag_Yes)
                        return "Please contact NDPERS for correspondence";
                    else if (correspondence_link.LastIndexOf("\\") > 1)
                        return correspondence_link.Substring(correspondence_link.LastIndexOf("\\") + 1, correspondence_link.LastIndexOf(".") - correspondence_link.LastIndexOf("\\") - 1);
                    else
                        return correspondence_link;
                }
                else if (tracking_id > 0)
                {
                    return "Please contact NDPERS for correspondence";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        // PIR 10027
        public string istrReportLink
        {
            get
            {
                if (!string.IsNullOrEmpty(correspondence_link))
                {
                    if (correspondence_link.LastIndexOf("\\") > 1)
                        return correspondence_link.Substring(correspondence_link.LastIndexOf("\\") + 1, correspondence_link.IndexOf(".rpt") - correspondence_link.LastIndexOf("\\") - 1);
                    else
                        return correspondence_link;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public int iintTrackingId
        {
            get
            {
                if (!string.IsNullOrEmpty(correspondence_link))
                {
                    if (correspondence_link.Length > 10)
                        return Convert.ToInt32(correspondence_link.Substring(correspondence_link.LastIndexOf("-") + 1, 10));
                    else
                        return 0;
                }
                else
                    return 0;
            }
        }

        public string istrReportName
        {
            get;
            set;
        }
        public string istrReportNameText { get; set; }
        public string istrReportNameHidden { get; set; }
        public string istrPDF {
            get
            {
                return "PDF";
            }
        }
        public string istrCSV {
            get
            {
                return "CSV";
            }
        }
    }
} 
