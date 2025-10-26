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
	public class busPersonAccountFlexCompGen : busPersonAccount
	{
		public busPersonAccountFlexCompGen()
		{

		}       

        private Collection<busPersonAccountFlexCompHistory> _iclbFlexCompHistory;

        public Collection<busPersonAccountFlexCompHistory> iclbFlexCompHistory
        {
            get { return _iclbFlexCompHistory; }
            set { _iclbFlexCompHistory = value; }
        }

        private Collection<busPersonAccountFlexcompConversion> _iclbFlexcompConversion;

        public Collection<busPersonAccountFlexcompConversion> iclbFlexcompConversion
        {
            get { return _iclbFlexcompConversion; }
            set { _iclbFlexcompConversion = value; }
        }
		private cdoPersonAccountFlexComp _icdoPersonAccountFlexComp;
		public cdoPersonAccountFlexComp icdoPersonAccountFlexComp
		{
			get
			{
				return _icdoPersonAccountFlexComp;
			}
			set
			{
				_icdoPersonAccountFlexComp = value;
			}
		}
        public bool FindPersonAccountFlexComp(int Aintpersonaccountid)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoPersonAccountFlexComp>(
                new string[1] { "person_account_id" },
                new object[1] { Aintpersonaccountid }, null, null);
            if (_icdoPersonAccountFlexComp == null)
            {
                _icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp();
            }
            if (ldtbList.Rows.Count == 1)
            {
                _icdoPersonAccountFlexComp.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            else if (ldtbList.Rows.Count > 1)
            {
                throw new Exception("FindPersonAccountFlexComp method : Multiple records returned for given Person Account ID : " +
                    Aintpersonaccountid);
            }

            //Loading the Person Account too
            lblnResult = FindPersonAccount(Aintpersonaccountid);

            return lblnResult;
        }		
        public void LoadFlexCompConversion()
        {
            DataTable ldtbList = Select<cdoPersonAccountFlexcompConversion>(
              new string[1] { "person_account_id" },
              new object[1] { icdoPersonAccount.person_account_id }, null, null);
            _iclbFlexcompConversion = GetCollection<busPersonAccountFlexcompConversion>(ldtbList, "icdoPersonAccountFlexcompConversion");
            foreach (busPersonAccountFlexcompConversion lobjConversion in _iclbFlexcompConversion)
            {
                lobjConversion.LoadProvider();
            }
        }
       
        public void LoadFlexCompHistory()
        {
            DataTable ldtbList = Select<cdoPersonAccountFlexCompHistory>(
              new string[1] { "person_account_id" },
              new object[1] { icdoPersonAccount.person_account_id }, null, "PERSON_ACCOUNT_FLEX_COMP_HISTORY_ID DESC");
            _iclbFlexCompHistory = GetCollection<busPersonAccountFlexCompHistory>(ldtbList, "icdoPersonAccountFlexCompHistory");
            foreach (busPersonAccountFlexCompHistory lobjFlexHistory in _iclbFlexCompHistory)
            {
                lobjFlexHistory.LoadPersonAccount();
                lobjFlexHistory.LoadPlan();
            }
        }
	}
}
