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
    public class busPersonAccountEmploymentDetail : busPersonAccountEmploymentDetailGen
    {
        private busPersonEmploymentDetail _ibusEmploymentDetail;
        public busPersonEmploymentDetail ibusEmploymentDetail
        {
            get { return _ibusEmploymentDetail; }
            set { _ibusEmploymentDetail = value; }
        }
        public void LoadPersonEmploymentDetail(bool ablnLoadMemberType = true, Collection<busPersonAccount> acblPersonAccount = null)
        {
            LoadPersonEmploymentDetail(DateTime.Now, ablnLoadMemberType, acblPersonAccount);
        }

        public void LoadPersonEmploymentDetail(DateTime adtEffectiveDate, bool ablnLoadMemberType, Collection<busPersonAccount> acblPersonAccount = null)
        {
            if (_ibusEmploymentDetail == null)
                _ibusEmploymentDetail = new busPersonEmploymentDetail();
            if (_ibusEmploymentDetail.icdoPersonEmploymentDetail == null)
                _ibusEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();

            DataTable ldtbList = Select<cdoPersonEmploymentDetail>(
                new string[1] { "person_employment_dtl_id" },
                new object[1] { icdoPersonAccountEmploymentDetail.person_employment_dtl_id }, null, null);
            if (ldtbList.Rows.Count > 0)
                _ibusEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtbList.Rows[0]);
            if (idtbPlanCacheData != null)
                _ibusEmploymentDetail.idtbPlanCacheData = idtbPlanCacheData;
            if (ablnLoadMemberType)
            {
                DateTime ldtEffectiveDate = ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date == DateTime.MinValue ?
                    adtEffectiveDate : (ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date > adtEffectiveDate ?
                    adtEffectiveDate : ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date);
                _ibusEmploymentDetail.LoadMemberType(ldtEffectiveDate, acblPersonAccount);
            }
        }

        public static Collection<busPersonAccountEmploymentDetail> SortDescending(Collection<busPersonAccountEmploymentDetail> aclbEmploymentDetail)
        {
            Collection<busPersonAccountEmploymentDetail> lclbEmploymentDetail = new Collection<busPersonAccountEmploymentDetail>();

            if (aclbEmploymentDetail != null)
            {
                int lintAt;
                busPersonAccountEmploymentDetail lbusAddedEmpl;
                foreach (busPersonAccountEmploymentDetail lbusEmploymentDetail in aclbEmploymentDetail)
                {
                    if (lbusEmploymentDetail.ibusEmploymentDetail == null)
                        lbusEmploymentDetail.LoadPersonEmploymentDetail();
                    // Loop thru the built list and insert at the right place
                    lintAt = 0;
                    for (int i = 0; i < lclbEmploymentDetail.Count; i++)
                    {
                        lbusAddedEmpl = lclbEmploymentDetail[i];
                        if (lbusEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null > lbusAddedEmpl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null)
                            break;
                        else if (lbusEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null == lbusAddedEmpl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null)
                        {
                            if (lbusEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date > lbusAddedEmpl.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date)
                                break;
                        }
                        lintAt += 1;
                    }
                    lclbEmploymentDetail.Insert(lintAt, lbusEmploymentDetail);
                }
            }
            return lclbEmploymentDetail;
        }
        public string istrPersonEmploymentDetail
        {
            get
            {
                return _ibusEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code + "-"
                    + _ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date.ToShortDateString() + " to " + _ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date.ToShortDateString();
            }
        }
        public int istrPersonEmploymentDetailID
        {
            get
            {
                return _ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            }
        }
    }
}
