#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPersonAccountEmploymentDetail : doPersonAccountEmploymentDetail
    {
        public cdoPersonAccountEmploymentDetail() : base()
        {
        }

        public int person_id { get; set; }

        public int org_id { get; set; }

        public override int Update()
        {
            // To update the Person ID and ORG ID to the Audit log.
            BusinessObjects.busPersonAccountEmploymentDetail lobjEmpDtl = new BusinessObjects.busPersonAccountEmploymentDetail { 
                icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail() };
            lobjEmpDtl.icdoPersonAccountEmploymentDetail.person_account_id = person_account_id;
            lobjEmpDtl.icdoPersonAccountEmploymentDetail.person_employment_dtl_id = person_employment_dtl_id;
            lobjEmpDtl.LoadPersonEmploymentDetail();
            lobjEmpDtl.ibusEmploymentDetail.LoadPersonEmployment();

            person_id = lobjEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_id;
            org_id = lobjEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
            return base.Update();
        }
    }
}
