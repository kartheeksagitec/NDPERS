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
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin
{
    /// <summary>
    /// Class NeoSpin.busWssBenAppRolloverDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssBenAppRolloverDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busWssBenAppRolloverDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busWssBenAppRolloverDetailGen
        /// </summary>
		public busWssBenAppRolloverDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busWssBenAppRolloverDetailGen.
        /// </summary>
		public cdoWssBenAppRolloverDetail icdoWssBenAppRolloverDetail { get; set; }




        /// <summary>
        /// NeoSpin.busWssBenAppRolloverDetailGen.FindWssBenAppRolloverDetail():
        /// Finds a particular record from cdoWssBenAppRolloverDetail with its primary key. 
        /// </summary>
        /// <param name="aintWssBenAppRolloverDetailId">A primary key value of type int of cdoWssBenAppRolloverDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindWssBenAppRolloverDetail(int aintWssBenAppRolloverDetailId)
		{
			bool lblnResult = false;
			if (icdoWssBenAppRolloverDetail == null)
			{
				icdoWssBenAppRolloverDetail = new cdoWssBenAppRolloverDetail();
			}
			if (icdoWssBenAppRolloverDetail.SelectRow(new object[1] { aintWssBenAppRolloverDetailId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        public virtual bool FindWssBenAppRolloverDetailByWssBenAppId(int aintWssBenAppId)
        {
            bool lblnResult = false;
            if (icdoWssBenAppRolloverDetail == null)
            {
                icdoWssBenAppRolloverDetail = new cdoWssBenAppRolloverDetail();
            }
            DataTable ldtbBenAppRolloverDtls = Select<cdoWssBenAppRolloverDetail>(new string[1] { enmWssBenAppRolloverDetail.wss_ben_app_id.ToString() },
                                                        new object[1]
                                            { aintWssBenAppId }, null, "WSS_BEN_APP_ID DESC");
            if (ldtbBenAppRolloverDtls.Rows.Count > 0)
            {
                icdoWssBenAppRolloverDetail.LoadData(ldtbBenAppRolloverDtls.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
