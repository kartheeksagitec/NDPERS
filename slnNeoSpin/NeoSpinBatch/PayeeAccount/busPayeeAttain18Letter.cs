#region Using directives
using System.Data;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using Sagitec.DBUtility;
using System.Collections;

#endregion

namespace NeoSpinBatch
{
    public class busPayeeAttain18Letter : busNeoSpinBatch
    {
        public void CreatePayeeAttain18Letter()
        {
            istrProcessName = "Payee Attains 18 Batch Letter";
            // Load all Payee who are all Attaining age 18 in two months.
            idlgUpdateProcessLog("Create Correspondence for Payee Attains 18 Notice Letters", "INFO", istrProcessName);
            DataTable ldtbPayee = DBFunction.DBSelect("cdoPayeeAccount.PayeeAttains18BatchLetters", new object[1] {iobjSystemManagement.icdoSystemManagement.batch_date},
                                            iobjPassInfo.iconFramework,
                                            iobjPassInfo.itrnFramework);
            if (ldtbPayee.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbPayee.Rows)
                {
                    busPayeeAccount lobjPE = new busPayeeAccount();
                    lobjPE.icdoPayeeAccount = new cdoPayeeAccount();
                    lobjPE.ibusPayee = new busPerson();
                    lobjPE.ibusPayee.icdoPerson = new cdoPerson();
                    lobjPE.icdoPayeeAccount.LoadData(dr);
                    lobjPE.ibusPayee.icdoPerson.LoadData(dr);
                    lobjPE.LoadApplication();
                    lobjPE.ibusApplication.LoadPersonAccount();
                    lobjPE.LoadMember();

                    //ArrayList larrList = new ArrayList();
                    //larrList.Add(lobjPE);

                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("FormTable", "Batch");

                    CreateCorrespondence("PAY-4101", lobjPE, lshtTemp);
                }
                idlgUpdateProcessLog("Correspondence created successfully", "INFO", istrProcessName);
            }
            else
                idlgUpdateProcessLog("No Correpondence Generated.", "INFO", istrProcessName);            
        }
    }
}
