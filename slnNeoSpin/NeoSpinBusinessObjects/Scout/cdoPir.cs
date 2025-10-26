#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoPir : doPir
    {
        public cdoPir()
            : base()
        {
        }

        private string _additional_notes;
        public string additional_notes
        {
            get
            {
                return _additional_notes;
            }

            set
            {
                _additional_notes = value;
            }
        }


        public string istrPriorityDate{ get; set; }

        public override int Update()
        {
            //Rules for inserting new history record
            //1. If long_description is not blank or status changes or assigned to changes,
            //   insert new record into history.
            bool iblnInsertNewRecord = false;
            if (additional_notes != "")
            {
                iblnInsertNewRecord = true;
            }

            if (status_value != Convert.ToString(ihstOldValues["status_value"]))
            {
                iblnInsertNewRecord = true;
            }
            InsertToPirHistory();

            int lintResult = base.Update();

            return lintResult;
        }

        public override int Insert()
        {
            int lintResult = base.Insert();
            InsertToPirHistory();
            return lintResult;
        }

        public void InsertToPirHistory()
        {
            //Create a cdoPirHistory and keep it ready 
            //Insert into history
            cdoPirHistory lobjPirHistory = new cdoPirHistory();
            lobjPirHistory.pir_id = pir_id;
            lobjPirHistory.long_description = additional_notes;
            additional_notes = "";
            lobjPirHistory.assigned_to_id = assigned_to_id;
            lobjPirHistory.status_value = status_value;           
            lobjPirHistory.created_by = created_by;
            lobjPirHistory.modified_by = iobjPassInfo.istrUserID;
            lobjPirHistory.created_date = created_date;
            lobjPirHistory.modified_date = DateTime.Now;
            lobjPirHistory.Insert();
        }

    }
}
