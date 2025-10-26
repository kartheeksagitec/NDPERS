using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.IO;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.DBUtility;
using System.Collections;

namespace NeoSpinBatch
{
    class busDeferredCompEnrollmentFileBatch : busNeoSpinBatch
    {
        public busDeferredCompEnrollmentFileBatch()
        {
        }
        public void GenerateDeferredCompFileOut()
        {
            busBase lobjBase=new busBase();
            busProcessOutboundFile lobjProcessFiles =new busProcessOutboundFile();
            lobjProcessFiles.iarrParameters = new object[3];
            DataTable ldtb457Providers = busBase.Select("cdoPersonAccountDeferredCompProvider.DeferredCompProviderPlans", new object[] { });
            Collection<busOrgPlan> lclbOrgPlan=new Collection<busOrgPlan>();
            lclbOrgPlan = lobjBase.GetCollection<busOrgPlan>(ldtb457Providers, "icdoOrgPlan");
            foreach(busOrgPlan lobjOrgPlan in lclbOrgPlan)
            {
                if (lobjOrgPlan.icdoOrgPlan.end_date_no_null > DateTime.Today)
                {
                    lobjProcessFiles.iarrParameters[0] = lobjOrgPlan.icdoOrgPlan.org_plan_id;
                    lobjProcessFiles.iarrParameters[1] = iobjSystemManagement.icdoSystemManagement.batch_date;
                    lobjProcessFiles.iarrParameters[2] = lobjOrgPlan.icdoOrgPlan.org_id;         
                    lobjProcessFiles.CreateOutboundFile(52);
                }
            }
        }
    }
}
