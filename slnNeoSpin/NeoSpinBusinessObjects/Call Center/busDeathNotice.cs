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
    public partial class busDeathNotice : busExtendBase
    {
        public bool FindDeathNoticeByContactTicket(int Aintcontactticketid)
        {
            if (_icdoDeathNotice == null)
            {
                _icdoDeathNotice = new cdoDeathNotice();
            }
            busPerson lbusperson = new busPerson();                   
            DataTable ldtb = Select<cdoDeathNotice>(new string[1] { "contact_ticket_id" },
                  new object[1] { Aintcontactticketid }, null, null);
            if (ldtb.Rows.Count > 0)
            {
                _icdoDeathNotice.LoadData(ldtb.Rows[0]);
                if (ldtb.Rows[0]["PERSLINK_ID"] != DBNull.Value)
                {
                    lbusperson.FindPerson(Convert.ToInt32(ldtb.Rows[0]["PERSLINK_ID"]));
                    //_icdoDeathNotice.deceased_name = Convert.ToInt32(ldtb.Rows[0]["PERSLINK_ID"]) > 0 ? lbusperson.icdoPerson.FullName + ',' + lbusperson.icdoPerson.person_id : ldtb.Rows[0]["DECEASED_NAME"].ToString();
                    _icdoDeathNotice.deceased_name =  ldtb.Rows[0]["DECEASED_NAME"].ToString();
                    if (ldtb.Rows[0]["DECEASED_NAME"].ToString() == string.Empty)
                    {
                        _icdoDeathNotice.deceased_name = Convert.ToInt32(ldtb.Rows[0]["PERSLINK_ID"]) > 0 ? lbusperson.icdoPerson.FullName + ',' + lbusperson.icdoPerson.person_id : ldtb.Rows[0]["DECEASED_NAME"].ToString();
                    }
                }
                return true;
            }
            return false;
        }

        public busContactTicket ibusContactTicket { get; set; }
    }
}
