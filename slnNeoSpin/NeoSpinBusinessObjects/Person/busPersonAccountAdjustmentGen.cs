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
	public class busPersonAccountAdjustmentGen : busExtendBase
    {
        public busPersonAccountAdjustmentGen()
		{

		}
        private cdoPersonAccountAdjustment _icdoPersonAccountAdjustment;
        public cdoPersonAccountAdjustment icdoPersonAccountAdjustment
		{
			get
			{
                return _icdoPersonAccountAdjustment;
			}
			set
			{
                _icdoPersonAccountAdjustment = value;
			}
		}

        public bool FindPersonAccountAdjustment(int aintPersonAccountAdjustment)
		{
			bool lblnResult = false;
            if (_icdoPersonAccountAdjustment == null)
			{
                _icdoPersonAccountAdjustment = new cdoPersonAccountAdjustment();
			}
            if (_icdoPersonAccountAdjustment.SelectRow(new object[1] { aintPersonAccountAdjustment }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
