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
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPersonAccountMedicarePartDHistoryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPersonAccountMedicarePartDHistory and its children table. 
    /// </summary>
	[Serializable]
	public class busPersonAccountMedicarePartDHistoryGen : busPersonAccount
	{
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPersonAccountMedicarePartDHistoryGen
        /// </summary>
		public busPersonAccountMedicarePartDHistoryGen()
		{

		}
        /// <summary>
        /// Gets or sets the main-table object contained in busPersonAccountMedicarePartDHistoryGen.
        /// </summary>
		public cdoPersonAccountMedicarePartDHistory icdoPersonAccountMedicarePartDHistory { get; set; }

        private Collection<busPersonAccountMedicarePartDHistory> _iclbPersonAccountMedicarePartDHistory;
        public Collection<busPersonAccountMedicarePartDHistory> iclbPersonAccountMedicarePartDHistory
        {
            get { return _iclbPersonAccountMedicarePartDHistory; }
            set { _iclbPersonAccountMedicarePartDHistory = value; }
        }

        private Collection<busPersonAccountMedicarePartDHistory> _iclbHistory;
        public Collection<busPersonAccountMedicarePartDHistory> iclbHistory
        {
            get { return _iclbHistory; }
            set { _iclbHistory = value; }
        }

        private Collection<busPersonAccountMedicarePartDHistory> _iclbMedicarePreviousHistory;
        public Collection<busPersonAccountMedicarePartDHistory> iclbMedicarePreviousHistory
        {
            get { return _iclbMedicarePreviousHistory; }
            set { _iclbMedicarePreviousHistory = value; }
        }

        /// <summary>
        /// NeoSpin.busPersonAccountMedicarePartDHistoryGen.FindPersonAccountMedicarePartDHistory():
        /// Finds a particular record from cdoPersonAccountMedicarePartDHistory with its primary key. 
        /// </summary>
        /// <param name="aintPersonAccountMedicarePartDHistoryId">A primary key value of type int of cdoPersonAccountMedicarePartDHistory on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPersonAccountMedicarePartDHistory(int aintPersonAccountMedicarePartDHistoryId)
		{
			bool lblnResult = false;
			if (icdoPersonAccountMedicarePartDHistory == null)
			{
				icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
			}
			if (icdoPersonAccountMedicarePartDHistory.SelectRow(new object[1] { aintPersonAccountMedicarePartDHistoryId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		
        
        public void LoadPersonAccountMedicarePartDHistory(int aintPersonID)
        {
            //DataTable ldtbHistory = Select("cdoPersonAccountMedicarePartDHistory.LoadMedicarePartDHistory", new object[1] { aintPersonID});
            //_iclbPersonAccountMedicarePartDHistory = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbHistory, "icdoPersonAccountMedicarePartDHistory");

            //Change below code for neogrid, Neogrid requires MainCDO to be initialized(PIR 2369)
            DataTable ldtbHistory = Select("cdoPersonAccountMedicarePartDHistory.LoadMedicarePartDHistory", new object[1] { aintPersonID });
            _iclbPersonAccountMedicarePartDHistory = new Collection<busPersonAccountMedicarePartDHistory>();
            
            foreach (DataRow aTotalsRow in ldtbHistory.Rows)
            {
                busPersonAccountMedicarePartDHistory lbusPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                lbusPersonAccountMedicarePartDHistory.icdoPersonAccount = new cdoPersonAccount();

                if(icdoPersonAccount != null)
                lbusPersonAccountMedicarePartDHistory.icdoPersonAccount.person_account_id = icdoPersonAccount.person_account_id;
     
                lbusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.LoadData(aTotalsRow);
                _iclbPersonAccountMedicarePartDHistory.Add(lbusPersonAccountMedicarePartDHistory);
            }
        }

        //public void LoadPersonAccountMedicarePartDHistory(int aintPersonID)
        //{
        //    DataTable ldtbHistory = Select("cdoPersonAccountMedicarePartDHistory.LoadMedicarePartDHistory", new object[1] { aintPersonID });
        //    _iclbPersonAccountMedicarePartDHistory = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbHistory, "icdoPersonAccountMedicarePartDHistory");
        //}
        public void LoadHistory()
        {
            DataTable ldtbList = Select<cdoPersonAccountMedicarePartDHistory>(
                new string[1] { "person_account_id"},
                new object[1] { icdoPersonAccount.person_account_id}, null, "PERSON_ACCOUNT_MEDICARE_PART_D_HISTORY_ID DESC");
                _iclbHistory = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");
        }

        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccount(int AintPersonID)
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();
            ibusPerson.FindPerson(AintPersonID);

            ibusPerson.LoadPersonAccount();

        }

        public bool FindMedicareByPersonAccountID(int AintPersonAccountID)
        {
            bool lblnResult = false;
            
            DataTable ldtbList = Select<cdoPersonAccountMedicarePartDHistory>(
                new string[1] { "person_account_id" },
                new object[1] { AintPersonAccountID }, null, "person_account_medicare_part_d_history_id DESC");
            if (icdoPersonAccountMedicarePartDHistory == null)
                icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            if (ldtbList.Rows.Count > 0)
            {
                icdoPersonAccountMedicarePartDHistory.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        //PIR 15434
        public bool FindMedicareByPersonAccountIDAndEffectiveDate(int AintPersonAccountID, DateTime adtGivenDate)
        {
            bool lblnResult = false;

            DataTable ldtbList = Select<cdoPersonAccountMedicarePartDHistory>(
                new string[1] { "person_account_id" },
                new object[1] { AintPersonAccountID }, null, "person_account_medicare_part_d_history_id DESC");
            if (icdoPersonAccountMedicarePartDHistory == null)
                icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();

            _iclbPersonAccountMedicarePartDHistory = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");

            foreach (busPersonAccountMedicarePartDHistory lobjPAMedicareHistory in iclbPersonAccountMedicarePartDHistory)
            {
                //Ignore the Same Start Date and End Date Records
                if (lobjPAMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date != lobjPAMedicareHistory.icdoPersonAccountMedicarePartDHistory.end_date)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPAMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date,
                        lobjPAMedicareHistory.icdoPersonAccountMedicarePartDHistory.end_date))
                    {
                        this.icdoPersonAccountMedicarePartDHistory = lobjPAMedicareHistory.icdoPersonAccountMedicarePartDHistory;
                        lblnResult = true;
                        break;
                    }
                }
            }
            return lblnResult;
        }

        public bool FindByPersonID(int AintPersonID)
        {
            bool lblnResult = false;

            DataTable ldtbList = Select<cdoPersonAccountMedicarePartDHistory>(
                new string[1] { "person_id" },
                new object[1] { AintPersonID }, null, "person_account_medicare_part_d_history_id DESC");
            if (icdoPersonAccountMedicarePartDHistory == null)
                icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            if (ldtbList.Rows.Count > 0)
            {
                icdoPersonAccountMedicarePartDHistory.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindByMemberPersonID(int AintPersonID)
        {
            bool lblnResult = false;

            DataTable ldtbList = Select<cdoPersonAccountMedicarePartDHistory>(
                new string[1] { "member_person_id" },
                new object[1] { AintPersonID }, null, "person_account_medicare_part_d_history_id DESC");
            if (icdoPersonAccountMedicarePartDHistory == null)
                icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            if (ldtbList.Rows.Count > 0)
            {
                icdoPersonAccountMedicarePartDHistory.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
        //This function is used when history table has to be updated if there is change in other screens (Person maintenance, Person Address, etc)
        public bool FindMedicareByPersonID(int AintPersonID)
        {
            bool lblnResult = false;
            
            DataTable ldtbList = Select("cdoPersonAccountMedicarePartDHistory.LoadMedicareDetailsForFlagUpdate", new object[1] { AintPersonID });
            _iclbPersonAccountMedicarePartDHistory = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");

            if (icdoPersonAccountMedicarePartDHistory == null)
                icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            if (ldtbList.Rows.Count > 0)
            {
                icdoPersonAccountMedicarePartDHistory.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        //Loading members with plan participation status 'ENL2' and null end date.
        //Function is used for throwing soft error on GHDV maintenance
        public bool FindMedicareByMemberPersonID(int AintMemberPersonID)
        {
            bool lblnResult = false;

            DataTable ldtbList = Select("cdoPersonAccountMedicarePartDHistory.LoadEnrolledMembers", new object[1] { AintMemberPersonID });
            _iclbPersonAccountMedicarePartDHistory = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");

            if (icdoPersonAccountMedicarePartDHistory == null)
                icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            if (ldtbList.Rows.Count > 0)
            {
                icdoPersonAccountMedicarePartDHistory.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindMedicareByDependentID(int AintPersonID)
        {
            bool lblnResult = false;

            DataTable ldtbList = Select("cdoPersonAccountMedicarePartDHistory.LoadEnrolledDependent", new object[1] { AintPersonID });
            _iclbPersonAccountMedicarePartDHistory = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");

            if (icdoPersonAccountMedicarePartDHistory == null)
                icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            if (ldtbList.Rows.Count > 0)
            {
                icdoPersonAccountMedicarePartDHistory.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

	}
}
