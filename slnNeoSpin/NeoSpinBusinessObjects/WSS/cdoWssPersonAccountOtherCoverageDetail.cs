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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountOtherCoverageDetail:
	/// Inherited from doWssPersonAccountOtherCoverageDetail, the class is used to customize the database object doWssPersonAccountOtherCoverageDetail.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountOtherCoverageDetail : doWssPersonAccountOtherCoverageDetail
	{
		public cdoWssPersonAccountOtherCoverageDetail() : base()
		{
		}
        public int enroll_req_plan_id { get; set; } //PIR 18493
        public string istrMssPlanCode { get; set; }
    } 
} 
