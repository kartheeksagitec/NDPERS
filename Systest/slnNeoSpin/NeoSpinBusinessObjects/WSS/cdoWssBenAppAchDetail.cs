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
	/// Class NeoSpin.CustomDataObjects.cdoWssBenAppAchDetail:
	/// Inherited from doWssBenAppAchDetail, the class is used to customize the database object doWssBenAppAchDetail.
	/// </summary>
    [Serializable]
	public class cdoWssBenAppAchDetail : doWssBenAppAchDetail
	{
		public cdoWssBenAppAchDetail() : base()
		{
		}
        public string istrBankName { get; set; }
    } 
} 
