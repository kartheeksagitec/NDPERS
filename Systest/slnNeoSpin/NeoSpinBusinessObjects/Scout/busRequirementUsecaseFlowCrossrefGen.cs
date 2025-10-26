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
	public partial class busRequirementUsecaseFlowCrossref : busExtendBase
    {
		public busRequirementUsecaseFlowCrossref()
		{

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

        private busRequirement _ibusRequirement;
        public busRequirement ibusRequirement
		{
			get
			{
                return _ibusRequirement;
			}

			set
			{
                _ibusRequirement = value;
			}
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
        
		private Collection<busRequirement> _icolRequirement;
		public Collection<busRequirement> icolRequirement
		{
			get
			{
				return _icolRequirement;
			}

			set
			{
				_icolRequirement = value;
			}
		}

        public bool FindRequirementUsecaseFlowCrossref(int AintRequirementFlowCrossrefId)
		{
			bool lblnResult = false;
            if (_icdoRequirementUsecaseFlowCrossref == null)
            {
                _icdoRequirementUsecaseFlowCrossref = new cdoRequirementUsecaseFlowCrossref();
            }
            
            if(_icdoRequirementUsecaseFlowCrossref.SelectRow(new object[1] {AintRequirementFlowCrossrefId}))
            {           
                lblnResult = true;
			}
			return lblnResult;
		}

        public void LoadRequirement()
        {
            if (_ibusRequirement == null)
            {
                _ibusRequirement = new busRequirement();
            }
            _ibusRequirement.FindRequirement(_icdoRequirementUsecaseFlowCrossref.requirement_id);
        }

        public void LoadUsecaseFlow()
        {
            if (_ibusUsecaseFlow == null)
            {
                _ibusUsecaseFlow = new busUsecaseFlow();
            }
            _ibusUsecaseFlow.FindUsecaseFlow(_icdoRequirementUsecaseFlowCrossref.flow_id);           
        } 
	}
}
