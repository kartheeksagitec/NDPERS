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
	/// Class NeoSpin.CustomDataObjects.cdoActivity:
	/// Inherited from doActivity, the class is used to customize the database object doActivity.
	/// </summary>
    [Serializable]
	public class cdoActivity : doActivity
	{
		public cdoActivity() : base()
		{
		}

        public string new_mode_screen_name { get; set; }
        public string new_mode_focus_control_id { get; set; }

        public string update_mode_screen_name { get; set; }
        public string update_mode_focus_control_id { get; set; }

        public string istrReferenceID { get; set; }
        public bool iblnFirstActivityFlag { get; set; }
        public bool iblnLastActivityFlag { get; set; }
    } 
} 
