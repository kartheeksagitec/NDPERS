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
    /// partial Class NeoSpin.busHb1040LifeExpectancy:
    /// </summary>
	[Serializable]
	public partial class busHb1040LifeExpectancy : busExtendBase
    {
		/// <summary>
        	/// Constructor for NeoSpin.busHb1040LifeExpectancy
        	/// </summary>
		public busHb1040LifeExpectancy()
		{
		}
        public doHb1040LifeExpectancy icdoHb1040LifeExpectancy { get; set; }

        public bool FindLifeExpectancy(decimal adecAge)
        {
            bool lblnResult = false;
            if (icdoHb1040LifeExpectancy == null)
            {
                icdoHb1040LifeExpectancy = new doHb1040LifeExpectancy();
            }
            DataTable ldtbList = Select<doHb1040LifeExpectancy>(
                                    new string[1] { enmHb1040LifeExpectancy.age.ToString() },
                                    new object[1] { adecAge }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                icdoHb1040LifeExpectancy.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
