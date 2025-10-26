#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busMasPayeeAccountGen:
    /// Inherited from busBase, used to create new business object for main table cdoMasPayeeAccount and its children table. 
    /// </summary>
	[Serializable]
	public class busMasPayeeAccountGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busMasPayeeAccountGen
        /// </summary>
		public busMasPayeeAccountGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busMasPayeeAccountGen.
        /// </summary>
		public cdoMasPayeeAccount icdoMasPayeeAccount { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busMasPayeeAccountPapit. 
        /// </summary>
		public Collection<busMasPayeeAccountPapit> iclbMasPayeeAccountPapit { get; set; }



        /// <summary>
        /// NeoSpin.busMasPayeeAccountGen.FindMasPayeeAccount():
        /// Finds a particular record from cdoMasPayeeAccount with its primary key. 
        /// </summary>
        /// <param name="aintmaspayeeaccountid">A primary key value of type int of cdoMasPayeeAccount on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindMasPayeeAccount(int aintmaspayeeaccountid)
		{
			bool lblnResult = false;
			if (icdoMasPayeeAccount == null)
			{
				icdoMasPayeeAccount = new cdoMasPayeeAccount();
			}
			if (icdoMasPayeeAccount.SelectRow(new object[1] { aintmaspayeeaccountid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busMasPayeeAccountGen.LoadMasPayeeAccountPapits():
        /// Loads Collection object iclbMasPayeeAccountPapit of type busMasPayeeAccountPapit.
        /// </summary>
		public virtual void LoadMasPayeeAccountPapits()
		{
			DataTable ldtbList = Select<cdoMasPayeeAccountPapit>(
				new string[1] { "MAS_PAYEE_ACCOUNT_ID" },
				new object[1] { icdoMasPayeeAccount.mas_payee_account_id }, null, null);
			iclbMasPayeeAccountPapit = GetCollection<busMasPayeeAccountPapit>(ldtbList, "icdoMasPayeeAccountPapit");
		}

	}
}
