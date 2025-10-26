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
	/// Class NeoSpin.CustomDataObjects.cdoPostRetirementIncreaseBatchRequest:
	/// Inherited from doPostRetirementIncreaseBatchRequest, the class is used to customize the database object doPostRetirementIncreaseBatchRequest.
	/// </summary>
    [Serializable]
	public class cdoPostRetirementIncreaseBatchRequest : doPostRetirementIncreaseBatchRequest
	{
		public cdoPostRetirementIncreaseBatchRequest() : base()
		{
		}       

        //this field is used in correspondence
        public string istrCurrentYear
        {
            get 
            {
                return DateTime.Now.Year.ToString();
            }
        }
    } 
} 
