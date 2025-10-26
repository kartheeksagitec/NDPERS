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
	public class busBenefitFasIndexingGen : busExtendBase
    {
		public busBenefitFasIndexingGen()
		{

		}

		private cdoBenefitFasIndexing _icdoBenefitFasIndexing;
		public cdoBenefitFasIndexing icdoBenefitFasIndexing
		{
			get
			{
				return _icdoBenefitFasIndexing;
			}
			set
			{
				_icdoBenefitFasIndexing = value;
			}
		}

		public bool FindBenefitFasIndexing(int Aintbenefithpfasid)
		{
			bool lblnResult = false;
			if (_icdoBenefitFasIndexing == null)
			{
				_icdoBenefitFasIndexing = new cdoBenefitFasIndexing();
			}
			if (_icdoBenefitFasIndexing.SelectRow(new object[1] { Aintbenefithpfasid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
