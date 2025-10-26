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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonEmploymentDetail:
	/// Inherited from doWssPersonEmploymentDetail, the class is used to customize the database object doWssPersonEmploymentDetail.
	/// </summary>
    [Serializable]
	public class cdoWssPersonEmploymentDetail : doWssPersonEmploymentDetail
	{
		public cdoWssPersonEmploymentDetail() : base()
		{
		}


        public string istrMemberWorkLessThan12MonthsValue { get; set; }
      
        public string istrMemberWorkLessThan12MonthsDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(seasonal_value))
                    return "Yes";
                else
                    return "No";
            }
        }
    } 
} 
