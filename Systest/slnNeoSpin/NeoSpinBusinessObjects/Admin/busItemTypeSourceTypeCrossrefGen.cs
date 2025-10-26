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
	public partial class busItemTypeSourceTypeCrossref : busExtendBase
    {
		public busItemTypeSourceTypeCrossref()
		{

		} 

		private cdoItemTypeSourceTypeCrossref _icdoItemTypeSourceTypeCrossref;
		public cdoItemTypeSourceTypeCrossref icdoItemTypeSourceTypeCrossref
		{
			get
			{
				return _icdoItemTypeSourceTypeCrossref;
			}

			set
			{
				_icdoItemTypeSourceTypeCrossref = value;
			}
		}

		public bool FindItemTypeSourceTypeCrossref(int Aintitemtypesourcetypecrossrefid)
		{
			bool lblnResult = false;
			if (_icdoItemTypeSourceTypeCrossref == null)
			{
				_icdoItemTypeSourceTypeCrossref = new cdoItemTypeSourceTypeCrossref();
			}
			if (_icdoItemTypeSourceTypeCrossref.SelectRow(new object[1] { Aintitemtypesourcetypecrossrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
