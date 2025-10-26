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
	public partial class busSerPurServiceType : busExtendBase
    {
		public busSerPurServiceType()
		{

		} 

		private cdoSerPurServiceType _icdoSerPurServiceType;
		public cdoSerPurServiceType icdoSerPurServiceType
		{
			get
			{
				return _icdoSerPurServiceType;
			}

			set
			{
				_icdoSerPurServiceType = value;
			}
		}

		public bool FindSerPurServiceType(int Aintserpurservicetypeid)
		{
			bool lblnResult = false;
			if (_icdoSerPurServiceType == null)
			{
				_icdoSerPurServiceType = new cdoSerPurServiceType();
			}
			if (_icdoSerPurServiceType.SelectRow(new object[1] { Aintserpurservicetypeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
