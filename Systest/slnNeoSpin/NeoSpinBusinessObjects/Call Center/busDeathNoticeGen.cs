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
	public partial class busDeathNotice : busExtendBase
    {
		public busDeathNotice()
		{

		} 

		private cdoDeathNotice _icdoDeathNotice;
		public cdoDeathNotice icdoDeathNotice
		{
			get
			{
				return _icdoDeathNotice;
			}

			set
			{
				_icdoDeathNotice = value;
			}
		}

		public bool FindDeathNotice(int Aintdeathnoticeid)
		{
			bool lblnResult = false;
			if (_icdoDeathNotice == null)
			{
				_icdoDeathNotice = new cdoDeathNotice();
			}
			if (_icdoDeathNotice.SelectRow(new object[1] { Aintdeathnoticeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
