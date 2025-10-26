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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonEmployment:
	/// Inherited from doWssPersonEmployment, the class is used to customize the database object doWssPersonEmployment.
	/// </summary>
    [Serializable]
	public class cdoWssPersonEmployment : doWssPersonEmployment
	{
		public cdoWssPersonEmployment() : base()
		{
		}

        public string istrOrgCode { get; set; }
    } 
} 
