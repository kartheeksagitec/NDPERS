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
	/// Class NeoSpin.CustomDataObjects.cdoProcessInstanceParameters:
	/// Inherited from doProcessInstanceParameters, the class is used to customize the database object doProcessInstanceParameters.
	/// </summary>
    [Serializable]
	public class cdoProcessInstanceParameters : doProcessInstanceParameters
	{
		public cdoProcessInstanceParameters() : base()
		{
		}
    } 
} 
