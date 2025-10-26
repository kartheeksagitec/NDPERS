using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using Sagitec.Bpm;
using Sagitec.DBUtility;

namespace NeoSpinBatch
{
    class busDeceasedStatusChange : busNeoSpinBatch
    {
        public void UpdateDeseasedMemberDetails()
        {
            istrProcessName = "Update Member Details";
            idlgUpdateProcessLog("Update Spouse Contact Status, member and spouse Marital Status and MS Change Date.", "INFO", istrProcessName);           
            DataTable ldtbCertifiedDeathNotification = busBase.Select("entDeathNotification.LoadCertifiedDeathNotification", new object[0] {});
            foreach (DataRow ldtRow in ldtbCertifiedDeathNotification.Rows)
            {
                busDeathNotification lbusDeathNotification = new busDeathNotification() { ibusPerson = new busPerson { icdoPerson = new cdoPerson() }};
                lbusDeathNotification.FindDeathNotification(Convert.ToInt32(ldtRow["DEATH_NOTIFICATION_ID"]));
                lbusDeathNotification.ibusPerson.icdoPerson.LoadData(ldtRow);                
                //if (lbusDeathNotification.ibusPerson.icdoPerson.person_id > 0)
                //{
                //    idlgUpdateProcessLog($"Updating deseased Member - {lbusDeathNotification.ibusPerson.icdoPerson.person_id} Marital Status and MS Change Date.", "INFO", istrProcessName);
                //    lbusDeathNotification.ibusPerson.icdoPerson.marital_status_value = busConstant.PersonMaritalStatusWidow;
                //    lbusDeathNotification.ibusPerson.icdoPerson.ms_change_date = lbusDeathNotification.icdoDeathNotification.date_of_death;
                //    lbusDeathNotification.ibusPerson.icdoPerson.Update();
                //}

                idlgUpdateProcessLog("Loading deseased members spouse contact", "INFO", istrProcessName);
                DataTable ldtbList = busBase.Select("entPersonContact.LoadMembersSpouseContact", new object[1] { lbusDeathNotification.icdoDeathNotification.person_id });
                foreach (DataRow ldtSpouseConatct in ldtbList.Rows)
                {
                    busPersonContact lbusPersonContact = new busPersonContact { icdoPersonContact = new cdoPersonContact() };
                    lbusPersonContact.icdoPersonContact.LoadData(ldtSpouseConatct);

                    if (lbusPersonContact.icdoPersonContact.IsNotNull() && lbusPersonContact.icdoPersonContact.status_value == busConstant.PersonContactStatusActive)
                    {
                        idlgUpdateProcessLog($"Updating deseased Spouse {lbusDeathNotification.ibusPerson.icdoPerson.person_id} Contact Status.", "INFO", istrProcessName);
                        lbusPersonContact.icdoPersonContact.status_value = busConstant.PersonContactStatusInActive;
                        lbusPersonContact.icdoPersonContact.Update();
                    }

                    busPerson lspousePerson = new busPerson { icdoPerson = new cdoPerson() };
                    idlgUpdateProcessLog($"Updating deseased Spouse {lbusDeathNotification.ibusPerson.icdoPerson.person_id} Marital Status and MS Change Date.", "INFO", istrProcessName);
                    if (lbusPersonContact.icdoPersonContact.contact_person_id > 0 && lbusPersonContact.icdoPersonContact.contact_person_id != lbusDeathNotification.icdoDeathNotification.person_id 
                        && lspousePerson.FindByPrimaryKey(lbusPersonContact.icdoPersonContact.contact_person_id) && lspousePerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    {
                        lspousePerson.icdoPerson.marital_status_value = busConstant.PersonMaritalStatusWidow;
                        lspousePerson.icdoPerson.ms_change_date = lbusDeathNotification.icdoDeathNotification.date_of_death;
                        lspousePerson.icdoPerson.Update();
                    }
                }
                if (lbusDeathNotification.ibusPerson.iclbContactTo.IsNull())
                    lbusDeathNotification.ibusPerson.LoadContactTo();
                foreach (busPerson lobjPerson in lbusDeathNotification.ibusPerson.iclbContactTo)
                {
                    if (lobjPerson.icdoPerson.person_id.IsNotNull() && lobjPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    {
                        idlgUpdateProcessLog($"Updating deseased Spouse {lbusDeathNotification.ibusPerson.icdoPerson.person_id} Contact Status.", "INFO", istrProcessName);
                        lobjPerson.LoadPersonContactTo();
                        if (lobjPerson.iclbPersonContactTo.IsNotNull())
                            lobjPerson.iclbPersonContactTo.ForEach(lobjFindSpouse =>
                            { if (lobjFindSpouse.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                                    lobjFindSpouse.icdoPersonContact.status_value = busConstant.PersonContactStatusInActive;
                                lobjFindSpouse.icdoPersonContact.Update();
                            });
                    }

                    idlgUpdateProcessLog($"Updating deseased Spouse {lbusDeathNotification.ibusPerson.icdoPerson.person_id} Marital Status and MS Change Date.", "INFO", istrProcessName);
                    if (lobjPerson.icdoPerson.person_id.IsNotNull() && lobjPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried &&
                        lobjPerson.icdoPerson.person_id != lbusDeathNotification.icdoDeathNotification.person_id)
                    {
                        lobjPerson.icdoPerson.marital_status_value = busConstant.PersonMaritalStatusWidow;
                        lobjPerson.icdoPerson.ms_change_date = lbusDeathNotification.icdoDeathNotification.date_of_death;
                        lobjPerson.icdoPerson.Update();
                    }
                }
            }
        }
    }
}
