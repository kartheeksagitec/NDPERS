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
	/// Class NeoSpin.CustomDataObjects.cdoProcess:
	/// Inherited from doProcess, the class is used to customize the database object doProcess.
	/// </summary>
    [Serializable]
	public class cdoProcess : doProcess
	{
		public cdoProcess() : base()
		{
		}

        public string istrWorkflowFullPath { get; set; }
    } 
} 
