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
	public partial class busRequirementUsecaseFlowCrossref : busExtendBase
    {
        private string _flow_key;
        public string flow_key
        {
            get { return _flow_key; }
            set { _flow_key = value; }
        }
        private string _increment_description;
        public string increment_description
        {
            get { return _increment_description; }
            set { _increment_description = value; }
        }
        private string _status_description;
        public string status_description
        {
            get { return _status_description; }
            set { _status_description = value; }
        }
        private string _flow_description;
        public string flow_description
        {
            get { return _flow_description; }
            set { _flow_description = value; }
        }
        public bool GetCountOfFlowForRequirementId()
        {           
            DataTable ldbtFlowList = Select<cdoRequirementUsecaseFlowCrossref>(
               new string[2] { "flow_id" , "requirement_id"},
               new object[2] { _icdoRequirementUsecaseFlowCrossref.flow_id, _icdoRequirementUsecaseFlowCrossref.requirement_id }, null, null);
            if (ldbtFlowList.Rows.Count == 1)
            {
                return true; 
            }
            return false;        
        }        
        public override void AfterPersistChanges()
        {
            LoadUsecaseFlow();
            base.AfterPersistChanges();
        }
        
	}
}
