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
using System.Linq;

#endregion

namespace NeoSpinBatch
{
    class busConfirmationOfAddressChange : busNeoSpinBatch
    {
        public void CreateAddressChangeCorrespondence()
        {
            istrProcessName = "Confirmation of Address Change";
            idlgUpdateProcessLog("Loading All Members Who Changed Their Address From Member Portal", "INFO", istrProcessName);
            DataTable ldbtMemberAddr = busNeoSpinBase.Select("cdoPersonAddress.LoadAddressChangedMembers", new object[0] { });
            foreach (DataRow ldrRow in ldbtMemberAddr.Rows)
            {
                busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new cdoPersonAddress() };
                lbusPersonAddress.icdoPersonAddress.LoadData(ldrRow);
                if (lbusPersonAddress.ibusPerson==null) lbusPersonAddress.LoadPerson();
                lbusPersonAddress.ibusPerson.iintPersonAddressID = lbusPersonAddress.icdoPersonAddress.person_address_id;
                try
                {
                    idlgUpdateProcessLog("Generating Confirmation Letter of Address Change for Person ID" + lbusPersonAddress.icdoPersonAddress.person_id.ToString(), "INFO", istrProcessName);
                    if (utlPassInfo.iobjPassInfo.iconFramework.ConnectionString.IsNullOrEmpty())
                        utlPassInfo.iobjPassInfo.iconFramework.ConnectionString = DBFunction.GetDBConnection().ConnectionString;
                    utlPassInfo.iobjPassInfo.BeginTransaction();
                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("sfwCallingForm", "Batch");
                    CreateCorrespondence("PER-0057", lbusPersonAddress, lshtTemp);
                    lbusPersonAddress.icdoPersonAddress.addr_change_letter_status_flag = busConstant.AddrChangeLetterSent;
                    lbusPersonAddress.icdoPersonAddress.Update();
                    utlPassInfo.iobjPassInfo.Commit();
                }
                catch (Exception _exc)
                {
                    ExceptionManager.Publish(_exc);
                    utlPassInfo.iobjPassInfo.Rollback();
                    idlgUpdateProcessLog("Confirmation Letter of Address Change Batch Failed for Person ID " + lbusPersonAddress.icdoPersonAddress.person_id.ToString() + " " +
                        " Message : " + _exc.Message, "ERR", iobjBatchSchedule.step_name);
                }
            }
            idlgUpdateProcessLog("Confirmation Letter of Address Change Batch Ended", "INFO", istrProcessName);
        }
    }
}
