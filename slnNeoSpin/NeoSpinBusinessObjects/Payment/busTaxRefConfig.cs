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
    /// partial Class NeoSpin.busTaxRefConfig:
    /// </summary>
	[Serializable]
	public partial class busTaxRefConfig : busNeoSpinBase
	{
		/// <summary>
        	/// Constructor for NeoSpin.busTaxRefConfig
        	/// </summary>
		public busTaxRefConfig()
		{
		}
        public bool FindTaxRefConfig(string astrTaxIdentifier, string astrTaxRef, DateTime adteEffectiveDate)
        {
            bool lblnResult = false;
            if (icdoTaxRefConfig == null)
            {
                icdoTaxRefConfig = new doTaxRefConfig();
            }
            DataTable ldtbList = SelectWithOperator<doTaxRefConfig>(
                new string[3] { enmTaxRefConfig.tax_identifier_value.ToString(), enmTaxRefConfig.tax_ref.ToString(), enmTaxRefConfig.effective_date.ToString() },
                new string[3] { "=", "=", "<=" },
                new object[3] { astrTaxIdentifier, astrTaxRef, adteEffectiveDate }, enmTaxRefConfig.effective_date.ToString() + " desc");
            if(ldtbList?.Rows.Count > 0)
            {
                icdoTaxRefConfig.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
