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
	/// Class NeoSpin.CustomDataObjects.cdoWssPersonAccountWorkerCompensation:
	/// Inherited from doWssPersonAccountWorkerCompensation, the class is used to customize the database object doWssPersonAccountWorkerCompensation.
	/// </summary>
    [Serializable]
	public class cdoWssPersonAccountWorkerCompensation : doWssPersonAccountWorkerCompensation
	{
		public cdoWssPersonAccountWorkerCompensation() : base()
		{
		}

        public int enroll_req_plan_id { get; set; } //PIR 18493
        public string text_edit
        {
            get
            {
                return "Edit";
            }
        } 
    } 
} 
