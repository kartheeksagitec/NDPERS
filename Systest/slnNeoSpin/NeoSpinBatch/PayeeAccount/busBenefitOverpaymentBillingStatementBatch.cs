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
using System.Collections;
using Sagitec.CorBuilder;
using Sagitec.ExceptionPub;
#endregion
namespace NeoSpinBatch
{
    public class busBenefitOverpaymentBillingStatementBatch : busNeoSpinBatch
    {
        public void GenerateBenefitOverpaymentBillingStatement()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog(istrProcessName + " Batch Started", "INFO", istrProcessName);
            try
            {
                idlgUpdateProcessLog("Generating Benefit Overpayment Billing Statement", "INFO", istrProcessName);
                //The system must create ‘Benefit Overpayment Billing’ letter 20th of every month if ‘Payment Option’ is ‘Personal Check’.
                DataTable ldtbRecovery = busBase.Select("cdoPaymentRecovery.BenefitOverPaymentBillingLetter",
                                                                        new object[0] { });
                busBase lobjBase = new busBase();
                Collection<busPaymentRecovery> lclbPaymentRecovery = lobjBase.GetCollection<busPaymentRecovery>(ldtbRecovery, "icdoPaymentRecovery");
                foreach (busPaymentRecovery lobjPaymentRecovery in lclbPaymentRecovery)
                {
                    lobjPaymentRecovery.LoadCorValues();
                    lobjPaymentRecovery.LoadPayeeAccount();
                    if (lobjPaymentRecovery.idecDueAmount > 0.0m)
                    {
                        CreateCorrespondence(lobjPaymentRecovery, "SFN-59179");
                    }
                }

                idlgUpdateProcessLog("Benefit Overpayment Billing Statement Generated Successfully", "INFO", istrProcessName);
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Generating PBenefit Overpayment Billing Statement Failed", "INFO", istrProcessName);
            }
        }
        private void CreateCorrespondence(busPaymentRecovery aobjPaymentRecovery, string astrCorName)
        {
            //ArrayList larrlist = new ArrayList();
            //larrlist.Add(aobjPaymentRecovery);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            CreateCorrespondence(astrCorName, aobjPaymentRecovery, lhstDummyTable);
        }
    }
}
