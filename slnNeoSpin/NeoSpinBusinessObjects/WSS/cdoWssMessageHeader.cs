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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoWssMessageHeader:
	/// Inherited from doWssMessageHeader, the class is used to customize the database object doWssMessageHeader.
	/// </summary>
    [Serializable]
	public class cdoWssMessageHeader : doWssMessageHeader
	{
		public cdoWssMessageHeader() : base()
		{            
		}

        public string display_message { get; set; }

        public string istrOrgCode { get; set; }

        public string istrWebLink { get; set; }

        public string istrCorrespondenceLink { get; set; }

        public string istrContactTicketId { get; set; }  //PIR-19351
        public string istrEndText { get; set; }
        public int iintRequestId { get; set; }
        public string istrRequestType { get; set; }
        public int iintPersonEmploymentId { get; set; }
        public int iintPersonEmploymentDetailId { get; set; }
    }
} 
