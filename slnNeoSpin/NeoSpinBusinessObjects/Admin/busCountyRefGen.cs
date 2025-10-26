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
	public class busCountyRefGen : busExtendBase
    {
		public busCountyRefGen()
		{

		} 

		private cdoCountyRef _icdoCountyRef;
		public cdoCountyRef icdoCountyRef
		{
			get
			{
				return _icdoCountyRef;
			}

			set
			{
				_icdoCountyRef = value;
			}
		}

		public bool FindCountyRef(int Aintcountyrefid)
		{
			bool lblnResult = false;
			if (_icdoCountyRef == null)
			{
				_icdoCountyRef = new cdoCountyRef();
			}
			if (_icdoCountyRef.SelectRow(new object[1] { Aintcountyrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
