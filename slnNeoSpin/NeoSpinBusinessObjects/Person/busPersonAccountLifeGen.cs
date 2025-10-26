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
	public class busPersonAccountLifeGen : busPersonAccount
	{
		public busPersonAccountLifeGen()
		{

		}

		private cdoPersonAccountLife _icdoPersonAccountLife;
		public cdoPersonAccountLife icdoPersonAccountLife
		{
			get
			{
				return _icdoPersonAccountLife;
			}
			set
			{
				_icdoPersonAccountLife = value;
			}
		}

		private Collection<busPersonAccountLifeHistory> _iclbPersonAccountLifeHistory;
		public Collection<busPersonAccountLifeHistory> iclbPersonAccountLifeHistory
		{
			get
			{
				return _iclbPersonAccountLifeHistory;
			}
			set
			{
				_iclbPersonAccountLifeHistory = value;
			}
		}

        public bool FindPersonAccountLife(int AintPersonAccountID)
		{
			bool lblnResult = false;
			if (_icdoPersonAccountLife == null)
			{
				_icdoPersonAccountLife = new cdoPersonAccountLife();
			}
            if (_icdoPersonAccountLife.SelectRow(new object[1] { AintPersonAccountID }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadHistory()
		{
            DataTable ldtbList = Select("cdoPersonAccountLife.LoadGroupLifeHistory",new object[1]{icdoPersonAccount.person_account_id});
            _iclbPersonAccountLifeHistory = GetCollection<busPersonAccountLifeHistory>(ldtbList, "icdoPersonAccountLifeHistory");
		}
	}
}
