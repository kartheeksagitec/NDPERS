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
	public class busPersonAccountWorkerCompensation : busPersonAccountWorkerCompensationGen
	{
        public override void BeforePersistChanges()
        {
            if (icdoPersonAccountWorkerCompensation.provider_org_code != string.Empty)
            {
                icdoPersonAccountWorkerCompensation.provider_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoPersonAccountWorkerCompensation.provider_org_code);
                LoadProviderName();
            }
            base.BeforePersistChanges();
        }

        public void LoadProviderName()
        {
            if (icdoPersonAccountWorkerCompensation.provider_org_id != 0)
                icdoPersonAccountWorkerCompensation.provider_org_name = busGlobalFunctions.GetOrgNameByOrgID(icdoPersonAccountWorkerCompensation.provider_org_id);
        }

        public void LoadProviderOrgCode()
        {
            if (icdoPersonAccountWorkerCompensation.provider_org_id != 0)
                icdoPersonAccountWorkerCompensation.provider_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(icdoPersonAccountWorkerCompensation.provider_org_id);
        }

        //This method called when we click Hand Icon. That will reload the object and display the details
        public ArrayList ReloadHandButtonData()
        {
            ArrayList larrList = new ArrayList();
            icdoPersonAccountWorkerCompensation.provider_org_id = busGlobalFunctions.GetOrgIdFromOrgCode(icdoPersonAccountWorkerCompensation.provider_org_code);
            LoadProviderName();
            larrList.Add(this);
            return larrList;
        }
	}
}
