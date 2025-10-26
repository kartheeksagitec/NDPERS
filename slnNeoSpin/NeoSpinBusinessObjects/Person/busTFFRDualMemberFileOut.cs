using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.CustomDataObjects;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busTFFRDualMemberFileOut : busFileBaseOut
    {
        public busTFFRDualMemberFileOut()
        {
        }

        static string istrUserId = "PERSLink Batch";

        private Collection<busTFFRDualMemberFile> _iclbTFFRDualMember;

        public Collection<busTFFRDualMemberFile> iclbTFFRDualMember
        {
            get { return _iclbTFFRDualMember; }
            set { _iclbTFFRDualMember = value; }
        }

        public override void InitializeFile()
        {
            istrFileName = "TFFR_Dual_Member" + DateTime.Now.ToString(busConstant.DateTimeFormatYYMMDD) + busConstant.FileFormattxt;
        }

        public void LoadTFFRDualMember(DataTable ldtTFFRDualMember)
        {
            ldtTFFRDualMember = busBase.Select("entPerson.LoadTFFRDualMembers", new object[0] { });
            iclbTFFRDualMember = new Collection<busTFFRDualMemberFile>();

            foreach(DataRow dr in ldtTFFRDualMember.Rows)
            {
                busTFFRDualMemberFile lobjTFFRDualMember = new busTFFRDualMemberFile();

                if (!Convert.IsDBNull(dr["ssn"]))
                    lobjTFFRDualMember.ssn = dr["ssn"].ToString();

                if (!Convert.IsDBNull(dr["last_name"]))
                    lobjTFFRDualMember.last_name = dr["last_name"].ToString();

                if (!Convert.IsDBNull(dr["first_name"]))
                    lobjTFFRDualMember.first_name = dr["first_name"].ToString();

                if (!Convert.IsDBNull(dr["middle_name"]))
                    lobjTFFRDualMember.middle_name = dr["middle_name"].ToString();

                if (!Convert.IsDBNull(dr["date_of_birth"]))
                    lobjTFFRDualMember.date_of_birth = Convert.ToDateTime(dr["date_of_birth"]);

                if (!Convert.IsDBNull(dr["month"]))
                    lobjTFFRDualMember.month = Convert.ToInt32(dr["month"]);

                if (!Convert.IsDBNull(dr["year"]))
                    lobjTFFRDualMember.year = Convert.ToInt32(dr["year"]);

                if (!Convert.IsDBNull(dr["salary"]))
                    lobjTFFRDualMember.salary = Convert.ToDecimal(dr["salary"]);

                if (!Convert.IsDBNull(dr["vsc"]))
                    lobjTFFRDualMember.vsc = Convert.ToInt32(dr["vsc"]);

                if (!Convert.IsDBNull(dr["person_id"]))
                    lobjTFFRDualMember.person_id = Convert.ToInt32(dr["person_id"]);

                iclbTFFRDualMember.Add(lobjTFFRDualMember);
            }
        }

        public override void FinalizeFile()
        {
            DBFunction.StoreProcessLog(100, "Create TFFR Outbound File", "INFO", "Started finalize file", istrUserId, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            try
            {
                if (iclbTFFRDualMember.Count > 0)
                {
                    var lclbDistinctPerson = iclbTFFRDualMember.Select(i => i.person_id).Distinct();
                    foreach (int lintPersonID in lclbDistinctPerson)
                    {
                        busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lobjPerson.FindPerson(lintPersonID);
                        lobjPerson.icdoPerson.tffr_request = busConstant.Flag_No;
                        lobjPerson.icdoPerson.Update();
                    }
                }
            }
            catch (Exception e)
            {
                DBFunction.StoreProcessLog(100, "An error occured while updating record." + "Error Message : " + e, "ERR", "Error in finalize file", istrUserId, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            base.FinalizeFile();
        }

        public override void AfterWriteRecord()
        {
            base.AfterWriteRecord();
        }
    }

    
}
