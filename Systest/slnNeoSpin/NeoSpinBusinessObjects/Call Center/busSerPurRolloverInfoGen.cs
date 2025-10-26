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
	public partial class busSerPurRolloverInfo : busExtendBase
    {
		public busSerPurRolloverInfo()
		{

		} 

		private cdoSerPurRolloverInfo _icdoSerPurRolloverInfo;
		public cdoSerPurRolloverInfo icdoSerPurRolloverInfo
		{
			get
			{
				return _icdoSerPurRolloverInfo;
			}

			set
			{
				_icdoSerPurRolloverInfo = value;
			}
		}

		public bool FindSerPurRolloverInfo(int Aintserpurrolloverid)
		{
			bool lblnResult = false;
			if (_icdoSerPurRolloverInfo == null)
			{
				_icdoSerPurRolloverInfo = new cdoSerPurRolloverInfo();
			}
			if (_icdoSerPurRolloverInfo.SelectRow(new object[1] { Aintserpurrolloverid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
