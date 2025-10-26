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
	public partial class busNewGroupPlanType : busExtendBase
    {
		public busNewGroupPlanType()
		{

		} 

		private cdoNewGroupPlanType _icdoNewGroupPlanType;
		public cdoNewGroupPlanType icdoNewGroupPlanType
		{
			get
			{
				return _icdoNewGroupPlanType;
			}

			set
			{
				_icdoNewGroupPlanType = value;
			}
		}

		private busNewGroup _ibusNewGroup;
		public busNewGroup ibusNewGroup
		{
			get
			{
				return _ibusNewGroup;
			}

			set
			{
				_ibusNewGroup = value;
			}
		}
        private string _istrSelectedPlan;

        public string istrSelectedPlan
        {
            get { return _istrSelectedPlan; }
            set { _istrSelectedPlan = value; }
        }
	
		public bool FindNewGroupPlanType(int Aintnewgroupplantypeid)
		{
			bool lblnResult = false;
			if (_icdoNewGroupPlanType == null)
			{
				_icdoNewGroupPlanType = new cdoNewGroupPlanType();
			}

			if (_icdoNewGroupPlanType.SelectRow(new object[1] { Aintnewgroupplantypeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
    }
}
