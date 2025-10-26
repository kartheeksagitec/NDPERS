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
	public partial class busPerson : busPersonBase
	{
		public busPerson()
		{

		}

		private cdoPerson _icdoPerson;
		public cdoPerson icdoPerson
		{
			get
			{
				return _icdoPerson;
			}

			set
			{
				_icdoPerson = value;
			}
		}
        public bool FindPerson(int Aintpersonid)
		{
			bool lblnResult = false;
			if (_icdoPerson == null)
			{
				_icdoPerson = new cdoPerson();
			}
			if (_icdoPerson.SelectRow(new object[1] { Aintpersonid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
