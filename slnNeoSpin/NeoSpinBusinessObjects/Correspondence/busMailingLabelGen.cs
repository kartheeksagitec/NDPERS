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
	public partial class busMailingLabel : busExtendBase
    {
		public busMailingLabel()
		{

		} 

		private cdoMailingLabel _icdoMailingLabel;
		public cdoMailingLabel icdoMailingLabel
		{
			get
			{
				return _icdoMailingLabel;
			}

			set
			{
				_icdoMailingLabel = value;
			}
		}

		public bool FindMailingLabel(int Aintmailinglabelclientid)
		{
			bool lblnResult = false;
			if (_icdoMailingLabel == null)
			{
				_icdoMailingLabel = new cdoMailingLabel();
			}
			if (_icdoMailingLabel.SelectRow(new object[1] { Aintmailinglabelclientid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
