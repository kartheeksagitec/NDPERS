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
	public class busCaseGen : busExtendBase
	{
		public busCaseGen()
		{

		}

		public cdoCase icdoCase { get; set; }

		public busPerson ibusPerson { get; set; }

        public busUser ibusUser { get; set; }

        public busPayeeAccount ibusPayeeAccount { get; set; } 

		public Collection<busCaseDisabilityIncomeVerificationDetail> iclbCaseDisabilityIncomeVerificationDetail{ get; set; }

		public Collection<busCaseFileDetail> iclbCaseFileDetail { get; set; } 

		public Collection<busCaseFinancialHardshipProviderDetail> iclbCaseFinancialHardshipProviderDetail { get; set; } 
		public Collection<busCaseStepDetail> iclbCaseStepDetailBenefitAppeal { get; set; }
        public Collection<busCaseStepDetail> iclbCaseStepDetailFinancialHardship { get; set; }
        public Collection<busCaseStepDetail> iclbCaseStepDetailDisability { get; set; }
        public Collection<busCaseStepDetail> iclbCaseStepDetailPre1991 { get; set; }
        public Collection<busCaseStepDetail> iclbCaseStepDetail { get; set; } 
		public Collection<busNotes> iclbNotes { get; set; } 



		public virtual bool FindCase(int Aintcaseid)
		{
			bool lblnResult = false;
			if (icdoCase == null)
			{
				icdoCase = new cdoCase();
			}
			if (icdoCase.SelectRow(new object[1] { Aintcaseid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public virtual void LoadPerson()
		{
			if (ibusPerson == null)
			{
				ibusPerson = new busPerson();
			}
			ibusPerson.FindPerson(icdoCase.person_id);
		}

		public virtual void LoadCaseDisabilityIncomeVerificationDetails()
		{
			DataTable ldtbList = Select<cdoCaseDisabilityIncomeVerificationDetail>(
				new string[1] { "case_id" },
				new object[1] { icdoCase.case_id }, null, null);
            iclbCaseDisabilityIncomeVerificationDetail = GetCollection<busCaseDisabilityIncomeVerificationDetail>(ldtbList, "icdoCaseDisabilityIncomeVerificationDetail");
            iclbCaseDisabilityIncomeVerificationDetail = busGlobalFunctions.Sort<busCaseDisabilityIncomeVerificationDetail>
                   ("icdoCaseDisabilityIncomeVerificationDetail.idtTempDate asc", iclbCaseDisabilityIncomeVerificationDetail);
		}

		public virtual void LoadCaseFileDetails()
		{
			DataTable ldtbList = Select<cdoCaseFileDetail>(
				new string[1] { "case_id" },
				new object[1] { icdoCase.case_id }, null, "filenet_received_date desc");
			iclbCaseFileDetail= GetCollection<busCaseFileDetail>(ldtbList, "icdoCaseFileDetail");           
            foreach(busCaseFileDetail lobjCaseFileDetail in iclbCaseFileDetail)
                lobjCaseFileDetail.icdoCaseFileDetail.istrObjectStore = NeoSpin.Common.ApplicationSettings.Instance.OBJECT_STORE; //PROD PIR 6308
		}

		public virtual void LoadCaseFinancialHardshipProviderDetails()
		{
			DataTable ldtbList = Select<cdoCaseFinancialHardshipProviderDetail>(
				new string[1] { "case_id" },
				new object[1] { icdoCase.case_id }, null, null);
			iclbCaseFinancialHardshipProviderDetail= GetCollection<busCaseFinancialHardshipProviderDetail>(ldtbList, "icdoCaseFinancialHardshipProviderDetail");
		}       

		public virtual Collection<busCaseStepDetail> LoadCaseStepDetails()
		{            
			DataTable ldtbList = Select<cdoCaseStepDetail>(
				new string[1] { "case_id" },
				new object[1] { icdoCase.case_id }, null, null);
            iclbCaseStepDetail = GetCollection<busCaseStepDetail>(ldtbList, "icdoCaseStepDetail");
            return iclbCaseStepDetail;
		}

		public virtual void LoadNotess()
		{
			DataTable ldtbList = Select<cdoNotes>(
				new string[1] { "person_id" },
				new object[1] { icdoCase.person_id }, null, null);
			iclbNotes= GetCollection<busNotes>(ldtbList, "icdoNotes");
		}

        public virtual void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
            {
                ibusPayeeAccount = new busPayeeAccount();
            }
            ibusPayeeAccount.FindPayeeAccount(icdoCase.payee_account_id);
        }

	}
}
