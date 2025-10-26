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
	public partial class busGLTransactionLookup
	{
        private Collection<busGLTransaction> _iclbGLTransaction;
        public Collection<busGLTransaction> iclbGLTransaction
        {
            get { return _iclbGLTransaction; }
            set { _iclbGLTransaction = value; }
        }

        public void LoadGLTransaction(DataTable adtbSearchResult)
        {
            _iclbGLTransaction = GetCollection<busGLTransaction>(adtbSearchResult, "icdoGlTransaction");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busGLTransaction)
            {
                busGLTransaction lobjGLTransaction = (busGLTransaction)aobjBus;
                busPlan lobjPlan = new busPlan();
                lobjPlan.FindPlan(lobjGLTransaction.icdoGlTransaction.plan_id);
                lobjGLTransaction.lstrPlanName = lobjPlan.icdoPlan.plan_name;
                lobjGLTransaction.lstrOrgCodeID=busGlobalFunctions.GetOrgCodeFromOrgId(lobjGLTransaction.icdoGlTransaction.org_id);
                lobjGLTransaction.LoadAccountNumber();
                lobjGLTransaction.LoadSourceIDDerived();
            }
        }
	}
}
