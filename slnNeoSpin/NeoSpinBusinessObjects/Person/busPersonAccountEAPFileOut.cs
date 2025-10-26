using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountEAPFileOut : busFileBaseOut
    {
        private string lstrOrgCode = string.Empty;

        public override void InitializeFile()
        {
            istrFileName = "NDPERS_EAP_Enrollment" + "_" + lstrOrgCode + busConstant.FileFormatcsv;
        }

        private Collection<busPersonAccountEAPFile> _iclbEAPMembers;
        public Collection<busPersonAccountEAPFile> iclbEAPMembers
        {
            get { return _iclbEAPMembers; }
            set { _iclbEAPMembers = value; }
        }

        public void LoadEAPMembers(DataTable ldtbEAPMembersByOrgID)
        {
            ldtbEAPMembersByOrgID = new DataTable();
            int lintOrgID = (int)iarrParameters[0];
            lstrOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lintOrgID);
            _iclbEAPMembers = new Collection<busPersonAccountEAPFile>();
            // Get Only EAP plan Enrolled Person Accounts with latest History.
            ldtbEAPMembersByOrgID = busBase.Select("cdoPersonAccount.NDPERS_EAP_Enrollment", new object[2] { lintOrgID, DateTime.Now.Date });
            foreach (DataRow dr in ldtbEAPMembersByOrgID.Rows)
            {
                // Load the EAP file object
                busPersonAccountEAPFile lobjEAPFile = new busPersonAccountEAPFile();
                if (!Convert.IsDBNull(dr["first_name"]))
                    lobjEAPFile.first_name = dr["first_name"].ToString();
                if (!Convert.IsDBNull(dr["last_name"]))
                    lobjEAPFile.last_name = dr["last_name"].ToString();
                if (!Convert.IsDBNull(dr["middle_name"]))
                    lobjEAPFile.middle_name = dr["middle_name"].ToString();
                if (!Convert.IsDBNull(dr["org_name"]))
                    lobjEAPFile.employer_name = dr["org_name"].ToString();
                if (!Convert.IsDBNull(dr["addr_line_1"]))
                    lobjEAPFile.address_line_1 = dr["addr_line_1"].ToString();
                if (!Convert.IsDBNull(dr["addr_line_2"]))
                    lobjEAPFile.address_line_2 = dr["addr_line_2"].ToString();
                if (!Convert.IsDBNull(dr["city"]))
                    lobjEAPFile.employer_city = dr["city"].ToString();
                if (!Convert.IsDBNull(dr["state_value"]))
                    lobjEAPFile.employer_state = dr["state_value"].ToString();
                if (!Convert.IsDBNull(dr["zip_code"]))
                    lobjEAPFile.employer_zip = dr["zip_code"].ToString();
                if (!Convert.IsDBNull(dr["start_date"]))
                    lobjEAPFile.employment_start_date = Convert.ToDateTime(dr["start_date"]);
                if (!Convert.IsDBNull(dr["end_date"]))
                    lobjEAPFile.employment_end_date = Convert.ToDateTime(dr["end_date"]);
                _iclbEAPMembers.Add(lobjEAPFile);
            }
        }
    }
}
