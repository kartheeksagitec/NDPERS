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
	/// Class NeoSpin.CustomDataObjects.cdoWssAcknowledgement:
	/// Inherited from doWssAcknowledgement, the class is used to customize the database object doWssAcknowledgement.
	/// </summary>
    [Serializable]
	public class cdoWssAcknowledgement : doWssAcknowledgement
	{
		public cdoWssAcknowledgement() : base()
		{
		}

        public string is_acknowledgement_selected { get; set; }
    } 
} 
