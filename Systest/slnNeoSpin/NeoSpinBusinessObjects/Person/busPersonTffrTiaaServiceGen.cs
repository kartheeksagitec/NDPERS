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
	public class busPersonTffrTiaaServiceGen : busExtendBase
    {
		public busPersonTffrTiaaServiceGen()
		{

		}

		private cdoPersonTffrTiaaService _icdoPersonTffrTiaaService;
		public cdoPersonTffrTiaaService icdoPersonTffrTiaaService
		{
			get
			{
				return _icdoPersonTffrTiaaService;
			}
			set
			{
				_icdoPersonTffrTiaaService = value;
			}
		}

		private busPerson _ibusPerson;
		public busPerson ibusPerson
		{
			get
			{
				return _ibusPerson;
			}
			set
			{
				_ibusPerson = value;
			}
		}

		public bool FindPersonTffrTiaaService(int Ainttffrtiaaserviceid)
		{
			bool lblnResult = false;
			if (_icdoPersonTffrTiaaService == null)
			{
				_icdoPersonTffrTiaaService = new cdoPersonTffrTiaaService();
			}
			if (_icdoPersonTffrTiaaService.SelectRow(new object[1] { Ainttffrtiaaserviceid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadPerson()
		{
			if (_ibusPerson == null)
			{
				_ibusPerson = new busPerson();
			}			
			_ibusPerson.FindPerson(_icdoPersonTffrTiaaService.person_id);
		}

	}
}
