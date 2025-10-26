#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.DataObjects;
#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// partial Class NeoSpin.busCorBeneDep:
    /// </summary>
	[Serializable]
	public partial class busCorBeneDep : busNeoSpinBase
	{
		/// <summary>
        	/// Constructor for NeoSpin.busCorBeneDep
        	/// </summary>
		public busCorBeneDep()
		{
		}
        public string istrBeneficiaryName { get; set; }
    }
}
