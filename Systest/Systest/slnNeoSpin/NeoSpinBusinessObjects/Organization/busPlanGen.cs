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
    [Serializable]
    public partial class busPlan : busExtendBase
    {
        public busPlan()
        {

        }

        private cdoPlan _icdoPlan;
        public cdoPlan icdoPlan
        {
            get
            {
                return _icdoPlan;
            }

            set
            {
                _icdoPlan = value;
            }
        }

        public DataTable idtbPlanCacheData { get; set; }

        public void LoadPlanCacheData()
        {
            idtbPlanCacheData = iobjPassInfo.isrvDBCache.GetCacheData("sgt_plan", null);
        }

        public bool FindPlan(int Aintplanid)
        {
            bool lblnResult = false;
            if (_icdoPlan == null)
            {
                _icdoPlan = new cdoPlan();
            }

            if (idtbPlanCacheData == null)
                LoadPlanCacheData();

            DataRow[] larrRows = busGlobalFunctions.FilterTable(idtbPlanCacheData, busConstant.DataType.Numeric, "plan_id", Aintplanid);

            if (larrRows.Length > 0)
            {
                _icdoPlan.LoadData(larrRows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
    }
}
