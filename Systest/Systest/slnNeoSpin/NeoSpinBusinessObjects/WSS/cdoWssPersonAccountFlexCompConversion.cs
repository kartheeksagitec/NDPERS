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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountFlexCompConversion:
	/// Inherited from doWssPersonAccountFlexCompConversion, the class is used to customize the database object doWssPersonAccountFlexCompConversion.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountFlexCompConversion : doWssPersonAccountFlexCompConversion
	{
		public cdoWssPersonAccountFlexCompConversion() : base()
		{
		}

        public string istrOrgName { get; set; }
        public string istrIsSelected { get; set; }
        public int person_account_id { get; set; } //PIR 10044
    } 
} 
