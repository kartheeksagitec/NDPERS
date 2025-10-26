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
	/// Class NeoSpin.CustomDataObjects.cdoActuaryFileRhicDetail:
	/// Inherited from doActuaryFileRhicDetail, the class is used to customize the database object doActuaryFileRhicDetail.
	/// </summary>
    [Serializable]
	public class cdoActuaryFileRhicDetail : doActuaryFileRhicDetail
	{
		public cdoActuaryFileRhicDetail() : base()
		{
		}
        public string ssn { get; set; }
        public string Gender { get; set; }
        public string last_name { get; set; }
        public string first_name { get; set; }      
        public DateTime date_of_birth { get; set; }
        public int ben_acc_owner_perslinkid { get; set; }
        public string org_code { get; set; }
    } 
} 
