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
	/// Class NeoSpin.CustomDataObjects.cdoProcessInstanceImageData:
	/// Inherited from doProcessInstanceImageData, the class is used to customize the database object doProcessInstanceImageData.
	/// </summary>
    [Serializable]
	public class cdoProcessInstanceImageData : doProcessInstanceImageData
	{
		public cdoProcessInstanceImageData() : base()
		{
		}
    } 
} 
