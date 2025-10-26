#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Data;
using System.Data.Common;


#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busRequirement : busExtendBase
    {
        public busRequirement()
        {
        }

        private cdoRequirement _icdoRequirement;
        public cdoRequirement icdoRequirement
        {
            get
            {
                return _icdoRequirement;
            }

            set
            {
                _icdoRequirement = value;
            }
        }
        private cdoRequirementUsecaseFlowCrossref _icdoRequirementUsecaseFlowCrossref;
        public cdoRequirementUsecaseFlowCrossref icdoRequirementUsecaseFlowCrossref
        {
            get
            {
                return _icdoRequirementUsecaseFlowCrossref;
            }

            set
            {
                _icdoRequirementUsecaseFlowCrossref = value;
            }
        }
        private Collection<busRequirementUsecaseFlowCrossref> _iclbusRequirementUsecaseFlowCrossref;
        public Collection<busRequirementUsecaseFlowCrossref> iclbusRequirementUsecaseFlowCrossref
        {
            get
            {
                return _iclbusRequirementUsecaseFlowCrossref;
            }

            set
            {
                _iclbusRequirementUsecaseFlowCrossref = value;
            }
        }
        private cdoRequirement _icdoParentRequirement;
        public cdoRequirement icdoParentRequirement
        {
            get
            {
                return _icdoParentRequirement;
            }

            set
            {
                _icdoParentRequirement = value;
            }
        }

        private cdoRequirementHistory _icdoRequirementHistory;
        public cdoRequirementHistory icdoRequirementHistory
        {
            get { return _icdoRequirementHistory; }
            set { _icdoRequirementHistory = value; }
        }

        private busUsecaseFlow _ibusUsecaseFlow;
        public busUsecaseFlow ibusUsecaseFlow
        {
            get
            {
                return _ibusUsecaseFlow;
            }

            set
            {
                _ibusUsecaseFlow = value;
            }
        }
        private busRequirementHistory _ibusRequirementHistory;
        public busRequirementHistory ibusRequirementHistory
        {
            get
            {
                return _ibusRequirementHistory;
            }

            set
            {
                _ibusRequirementHistory = value;
            }
        }

        private Collection<busRequirementHistory> _iclbRequirementHistory;
        public Collection<busRequirementHistory> iclbRequirementHistory
        {
            get
            {
                return _iclbRequirementHistory;
            }

            set
            {
                _iclbRequirementHistory = value;
            }
        }
        private Collection<busUsecaseFlow> _iclbusUsecaseFlow;
        public Collection<busUsecaseFlow> iclbusUsecaseFlow
        {
            get
            {
                return _iclbusUsecaseFlow;
            }

            set
            {
                _iclbusUsecaseFlow = value;
            }
        }


        private Collection<busRequirement> _iclbChildRequirement;
        public Collection<busRequirement> iclbChildRequirement
        {
            get
            {
                return _iclbChildRequirement;
            }

            set
            {
                _iclbChildRequirement = value;
            }
        }

        private bool _IsParentRequirementExists;
        public bool IsParentRequirementExists
        {
            get { return _IsParentRequirementExists; }
            set { _IsParentRequirementExists = value; }
        }

        private bool _IsRequirementKeyExists;
        public bool IsRequirementKeyExists
        {
            get { return _IsRequirementKeyExists; }
            set { _IsRequirementKeyExists = value; }
        }

        public bool FindRequirement(int aintRequirementId)
        {
            bool lblnResult = false;
            if (_icdoRequirement == null)
            {
                _icdoRequirement = new cdoRequirement();
            }
            if (_icdoRequirement.SelectRow(new object[1] { aintRequirementId }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadRequirementKey()
        {
            _icdoRequirement.Parent_Requirement_Key = Convert.ToString(DBFunction.DBExecuteScalar("cdoRequirement.LoadRequirementKey", new object[1] { _icdoRequirement.parent_requirement_id } , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
        }

        public void LoadRequirementHistory()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoRequirementHistory.getRequirementHistoryDetails",
                               new object[1] { _icdoRequirement.requirement_id });
            _iclbRequirementHistory = GetCollection<busRequirementHistory>(ldtbList, "icdoRequirementHistory");
        }
        public void LoadParentRequirements()
        {
            DataTable ldbtList = Select<cdoRequirement>(new string[1] { "requirement_id" }
                , new object[1] { _icdoRequirement.parent_requirement_id },
                null, null);
            if (ldbtList.Rows.Count != 0)
            {
                if (_icdoParentRequirement == null)
                {
                    _icdoParentRequirement = new cdoRequirement();
                }
                _icdoParentRequirement.SelectRow(new object[1] { ldbtList.Rows[0]["requirement_id"] });
            }
        }

        public void LoadChildRequirements()
        {
            if (icdoRequirement.requirement_id == icdoRequirement.parent_requirement_id)
            {
                DataTable ldbtChildList = busNeoSpinBase.Select("cdoRequirement.GetChildRequirements", new object[] { icdoRequirement.parent_requirement_id });
                _iclbChildRequirement = GetCollection<busRequirement>(ldbtChildList, "icdoRequirement");
            }
        }

        public void LoadUsecaseFlow()
        {
            DataTable ldbtFlowList = busNeoSpinBase.Select("cdoRequirement.GetFlowDetails", new object[2] { _icdoRequirement.requirement_id,_icdoRequirement.parent_requirement_id });
           
            _iclbusRequirementUsecaseFlowCrossref = GetCollection<busRequirementUsecaseFlowCrossref>(ldbtFlowList, "icdoRequirementUsecaseFlowCrossref");
            int lintindex = 0;
            foreach (busRequirementUsecaseFlowCrossref lobjbusRequirementUsecaseFlowCrossref in _iclbusRequirementUsecaseFlowCrossref)
            {
                lobjbusRequirementUsecaseFlowCrossref.flow_key = ldbtFlowList.Rows[lintindex]["FLOW_KEY"].ToString();
                string increment_value = ldbtFlowList.Rows[lintindex]["INCREMENT_VALUE"].ToString();
                lobjbusRequirementUsecaseFlowCrossref.increment_description = (iobjPassInfo.isrvDBCache.GetCodeDescriptionString(36, increment_value)).ToString();
                string Status_value = ldbtFlowList.Rows[lintindex]["STATUS_VALUE"].ToString();
                lobjbusRequirementUsecaseFlowCrossref.status_description = (iobjPassInfo.isrvDBCache.GetCodeDescriptionString(39, Status_value)).ToString();
                lobjbusRequirementUsecaseFlowCrossref.flow_description = ldbtFlowList.Rows[lintindex]["FLOW_DESCRIPTION"].ToString();
                lintindex++;
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
             _IsParentRequirementExists = ValidateParentKeyExists();
             _IsRequirementKeyExists = ValidateRequirementKeyExists();
          
            base.BeforeValidate(aenmPageMode);
        }

        private bool ValidateRequirementKeyExists()
        {
            bool lblnResult = false;
            if (_icdoRequirement.ienuObjectState == ObjectState.Insert)
            {
                int lintcount = (int)DBFunction.DBExecuteScalar("cdoRequirement.IsRequirementKeyExists", new object[1] { _icdoRequirement.requirement_key }  , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (lintcount > 0)
                {
                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        private bool ValidateParentKeyExists()
        {
            bool lblnResult = true;
            if (_icdoRequirement.requirement_key != _icdoRequirement.Parent_Requirement_Key)
            {
                int Count = (int)DBFunction.DBExecuteScalar("cdoRequirement.ParentKeyExists", new object[1] { _icdoRequirement.Parent_Requirement_Key }  , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (Count <= 0)
                {
                    lblnResult = false;
                }
            }
            return lblnResult;            
        }

        public override void BeforePersistChanges()
        {
            if (_icdoRequirement.requirement_key != _icdoRequirement.Parent_Requirement_Key)
            {
                _icdoRequirement.parent_requirement_id = (int)DBFunction.DBExecuteScalar("cdoRequirement.GetRequirementIDbyKey", new object[1] { _icdoRequirement.Parent_Requirement_Key }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            else
            {
                //Changing the Parent Key from different one to the same Requirement Key. (only in update mode)
                if (_icdoRequirement.ienuObjectState == ObjectState.Update)
                {
                    _icdoRequirement.parent_requirement_id = _icdoRequirement.requirement_id;
                }
            }

            base.BeforePersistChanges();
        }

        public override int PersistChanges()
        {
            int lintvalue = base.PersistChanges();
            
            //First time, if the Parent Requirment ID is not Set, update with the Current Req Id on both Tables
            if (_icdoRequirement.parent_requirement_id == 0)
            {
                if (_icdoRequirement.requirement_key == _icdoRequirement.Parent_Requirement_Key)
                {
                    DBFunction.DBNonQuery("cdoRequirement.UpdateParentRequirmentId", new object[1] { this.icdoRequirement.requirement_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    DBFunction.DBNonQuery("cdoRequirementHistory.UpdateParentRequirmentId", new object[1] { this.icdoRequirement.requirement_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }
            return lintvalue;

        }

        public override void AfterPersistChanges()
        {
            FindRequirement(_icdoRequirement.requirement_id);
            LoadRequirementHistory();
            LoadParentRequirements();
            LoadChildRequirements();
            LoadUsecaseFlow();
            LoadRequirementKey();
        }

        public override int Delete()
        {
            DBFunction.DBNonQuery("cdoRequirement.DeleteFromCrossRef", new object[1] { this.icdoRequirement.requirement_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            base.Delete();
            return 1;
        }        
    }
}
