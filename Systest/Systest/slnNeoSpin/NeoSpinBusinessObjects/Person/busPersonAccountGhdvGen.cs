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
    public class busPersonAccountGhdvGen : busPersonAccount
    {
        public busPersonAccountGhdvGen()
        {

        }

        private cdoPersonAccountGhdv _icdoPersonAccountGhdv;
        public cdoPersonAccountGhdv icdoPersonAccountGhdv
        {
            get
            {
                return _icdoPersonAccountGhdv;
            }
            set
            {
                _icdoPersonAccountGhdv = value;
            }
        }

        public bool FindPersonAccountGHDV(int AintPersonAccountGHDVID)
        {
            bool lblnResult = false;
            if (_icdoPersonAccountGhdv == null)
            {
                _icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
            }
            if (_icdoPersonAccountGhdv.SelectRow(new object[1] { AintPersonAccountGHDVID }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindGHDVByPersonAccountID(int AintPersonAccountID)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoPersonAccountGhdv>(
                new string[1] { "person_account_id" },
                new object[1] { AintPersonAccountID }, null, null);
            if (_icdoPersonAccountGhdv == null)
                _icdoPersonAccountGhdv = new cdoPersonAccountGhdv();
            if (ldtbList.Rows.Count == 1)
            {
                _icdoPersonAccountGhdv.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        private Collection<busPersonAccountGhdvHistory> _iclbPersonAccountGHDVHistory;
        public Collection<busPersonAccountGhdvHistory> iclbPersonAccountGHDVHistory
        {
            get { return _iclbPersonAccountGHDVHistory; }
            set { _iclbPersonAccountGHDVHistory = value; }
        }

        public void LoadPersonAccountGHDVHistory()
        {
            DataTable ldtbHistory = Select("cdoPersonAccountGhdv.LoadPersonAccountGHDVHistory", new object[1] { _icdoPersonAccountGhdv.person_account_ghdv_id });
            _iclbPersonAccountGHDVHistory = GetCollection<busPersonAccountGhdvHistory>(ldtbHistory, "icdoPersonAccountGhdvHistory");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busPersonAccountGhdvHistory)
            {
                busPersonAccountGhdvHistory lbusHistory = (busPersonAccountGhdvHistory)aobjBus;

                lbusHistory.ibusEPOProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusHistory.ibusEPOProvider.icdoOrganization.org_id = lbusHistory.icdoPersonAccountGhdvHistory.epo_org_id;
                if (!Convert.IsDBNull(adtrRow["EPO_ORG_NAME"]))
                {
                    lbusHistory.ibusEPOProvider.icdoOrganization.org_name = adtrRow["EPO_ORG_NAME"].ToString();
                }
                if (!Convert.IsDBNull(adtrRow["EPO_ORG_CODE"]))
                {
                    lbusHistory.ibusEPOProvider.icdoOrganization.org_name = adtrRow["EPO_ORG_CODE"].ToString();
                }
            }
        }
    }
}
