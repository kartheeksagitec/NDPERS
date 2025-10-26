#region Using directives

using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	public partial class busAccountReferenceLookup 
	{

        private Collection<busAccountReference> _iclbAccountReference;
        public Collection<busAccountReference> iclbAccountReference
        {
            get { return _iclbAccountReference; }
            set { _iclbAccountReference = value; }
        }

        public void LoadAccountReference(DataTable adtbSearchResult)
        {
            _iclbAccountReference = GetCollection<busAccountReference>(adtbSearchResult, "icdoAccountReference");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busAccountReference)
            {
                busAccountReference lobjAccountReference = (busAccountReference)aobjBus;
                busPlan lobjPlan = new busPlan();
                lobjPlan.FindPlan(lobjAccountReference.icdoAccountReference.plan_id);
                lobjAccountReference.lstrPlanName = lobjPlan.icdoPlan.plan_name;
            }
        }
	}
}
