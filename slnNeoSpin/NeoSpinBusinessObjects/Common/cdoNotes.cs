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
	public class cdoNotes : doNotes
	{
		public cdoNotes() : base()
		{
		}

        public string person_name { get; set; }

        public string org_name { get; set; }

    } 
} 
