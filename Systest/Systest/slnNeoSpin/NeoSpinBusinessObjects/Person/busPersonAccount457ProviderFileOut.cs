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
    public class busPersonAccount457ProviderFileOut : busFileBaseOut
    {
        public int lintOrgID = 0;
        private Collection<busPersonAccount457ProviderFile> _iclb457Members;
        public Collection<busPersonAccount457ProviderFile> iclb457Members
        {
            get { return _iclb457Members; }
            set { _iclb457Members = value; }
        }
        public override void InitializeFile()
        {
            istrFileName = "Def_Comp_Enrollment" + "_" + busGlobalFunctions.GetOrgCodeFromOrgId(lintOrgID) + ".txt";
        }
        public void Load457Members(DataTable ldtb457Members)
        {
            ldtb457Members = new DataTable();
            int lintOrgPlanID = (int)iarrParameters[0];
            lintOrgID = (int)iarrParameters[2];
            DateTime ldtBatchDate = Convert.ToDateTime(iarrParameters[1]);
            _iclb457Members = new Collection<busPersonAccount457ProviderFile>();
            ldtb457Members = busBase.Select("cdoPersonAccountDeferredCompProvider.fle457ProviderEnrollmentFileOut", new object[1] { lintOrgPlanID });
            foreach (DataRow dr in ldtb457Members.Rows)
            {
                busPersonAccount457ProviderFile lobjMembers = new busPersonAccount457ProviderFile();
                sqlFunction.LoadQueryResult(lobjMembers, dr);
                if (busGlobalFunctions.CheckDateOverlapping(ldtBatchDate, lobjMembers.deduction_begin_date, lobjMembers.deduction_end_date))
                {                   
                    _iclb457Members.Add(lobjMembers);
                }
            }
        }
    }
}
