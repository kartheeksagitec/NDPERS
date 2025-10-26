#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Globalization;
using System.Collections;
using Sagitec.CorBuilder;
#endregion
namespace NeoSpinBatch
{
    class IBSOverDueBatchPDPDisenrollmentLetter : busNeoSpinBatch
    {
        public IBSOverDueBatchPDPDisenrollmentLetter()
        {
        }
        public void CreatePDPDisEnrollmentLetter()
        {
            istrProcessName = "Involuntary Disenrollment Letter";
            idlgUpdateProcessLog("Involuntary Disenrollment Letter process for the due members", "INFO", istrProcessName);
            DataTable ldtbIBSMbrs = busNeoSpinBase.Select("cdoIbsHeader.LoadMembersForPDPDisEnrollment", new object[0]);
            busBase lobjbase=new busBase();
            Collection<busPersonAccount> lclbPersonAccount = new Collection<busPersonAccount>();
            lclbPersonAccount = lobjbase.GetCollection<busPersonAccount>(ldtbIBSMbrs, "icdoPersonAccount");
            foreach (busPersonAccount lobjPersonAccount in lclbPersonAccount)
            {                
                lobjPersonAccount.LoadPerson();
                DateTime ldtPremiumDueDate = new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, iobjSystemManagement.icdoSystemManagement.batch_date.Month, 1);
                lobjPersonAccount.LoadInsurancePremium(ldtPremiumDueDate);
                decimal ldecDueAmount = lobjPersonAccount.idecPremiumDue - lobjPersonAccount.idecPaidAmount;                
                if (ldecDueAmount > 0.00M)
                {
                    try
                    {
                        lobjPersonAccount.PremiumDueDate = ldtPremiumDueDate;
                        //ArrayList larrlist = new ArrayList();
                        //larrlist.Add(lobjPersonAccount);
                        idlgUpdateProcessLog("Creating Involuntary Disenrollment Letter for PERSLinkID " + Convert.ToString(lobjPersonAccount.icdoPersonAccount.person_id)
                            , "INFO", istrProcessName);
                        Hashtable lhstDummyTable = new Hashtable();
                        lhstDummyTable.Add("sfwCallingForm", "Batch");
                        CreateCorrespondence("PAY-4303", lobjPersonAccount, lhstDummyTable);
                        idlgUpdateProcessLog("Involuntary Disenrollment Letter created", "INFO", istrProcessName);                      
                    }
                    catch (Exception _exc)
                    {
                        idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
                    }
                }
            }
        }
    }
}
