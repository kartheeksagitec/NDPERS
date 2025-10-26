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
    public class busESIMedicarePartDEnrollmentFileOut : busFileBaseOut
    {
        private Collection<busESIMedicarePartDEnrollmentFile> _iclbESIMedicarePartDEnrollmentFile;
        public Collection<busESIMedicarePartDEnrollmentFile> iclbESIMedicarePartDEnrollmentFile
        {
            get { return _iclbESIMedicarePartDEnrollmentFile; }
            set { _iclbESIMedicarePartDEnrollmentFile = value; }
        }

        //For Header
        public string istrContactPerson
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, busConstant.ESIPartDEnrollment, iobjPassInfo);
            }
        }

        public string istrPhoneNumber
        {
            get
            {
                return busGlobalFunctions.GetData2ByCodeValue(52, busConstant.ESIPartDEnrollment, iobjPassInfo);
            }
        }

        private DateTime _ldtTodaysDate;
        public DateTime ldtTodaysDate
        {
            get
            {
                _ldtTodaysDate = DateTime.Now;
                return _ldtTodaysDate;
            }
        }

        private string _ldtCurrentTime;
        public string ldtCurrentTime
        {
            get
            {
                _ldtCurrentTime = DateTime.Now.ToString("hhmmssff");
                return _ldtCurrentTime;
            }
        }

        public int iintTotalCount { get; set; }

        public string istrTotalCount { get; set; }

        public override void InitializeFile()
        {
            istrFileName = "ESI_700095_PART_D_" + ldtTodaysDate.ToString(busConstant.DateTimeFormatYYYYMMDD) + busConstant.FileFormattxt;
        }

        public void LoadESIMedicarePartDEnrollment(DataTable adtbESIMedicarePartDEnrollment)
        {
            adtbESIMedicarePartDEnrollment = new DataTable();
            _iclbESIMedicarePartDEnrollmentFile = new Collection<busESIMedicarePartDEnrollmentFile>();
            adtbESIMedicarePartDEnrollment = busBase.Select("cdoPerson.CreateESIMedicarePartDEnrollmentFile", new object[] { });
            
            foreach (DataRow dr in adtbESIMedicarePartDEnrollment.Rows)
            {
                busESIMedicarePartDEnrollmentFile lobjESIMedicareEnrollment = new busESIMedicarePartDEnrollmentFile();
                
                if (!Convert.IsDBNull(dr["record_type_flag"]))
                    lobjESIMedicareEnrollment.record_type = dr["record_type_flag"].ToString();

                if (!Convert.IsDBNull(dr["person_id"]))
                    lobjESIMedicareEnrollment.person_id = dr["person_id"].ToString();

                if (!Convert.IsDBNull(dr["first_name"]))
                    lobjESIMedicareEnrollment.first_name = dr["first_name"].ToString();

                if (!Convert.IsDBNull(dr["mi"]))
                    lobjESIMedicareEnrollment.mi = dr["mi"].ToString();

                if (!Convert.IsDBNull(dr["last_name"]))
                    lobjESIMedicareEnrollment.last_name = dr["last_name"].ToString();

                if (!Convert.IsDBNull(dr["dob"]))
                    lobjESIMedicareEnrollment.date_of_birth = dr["dob"].ToString();

                if (!Convert.IsDBNull(dr["gender"]))
                    lobjESIMedicareEnrollment.gender = dr["gender"].ToString();

                if (!Convert.IsDBNull(dr["ssn"]))
                    lobjESIMedicareEnrollment.ssn = dr["ssn"].ToString();

                if (!Convert.IsDBNull(dr["medicare_claim_no"]))
                    lobjESIMedicareEnrollment.medicare_claim_no = dr["medicare_claim_no"].ToString();

                if (!Convert.IsDBNull(dr["addr_line_1"]))
                    lobjESIMedicareEnrollment.address_line_1 = dr["addr_line_1"].ToString();

                if (!Convert.IsDBNull(dr["addr_line_2"]))
                    lobjESIMedicareEnrollment.address_line_2 = dr["addr_line_2"].ToString();

                if (!Convert.IsDBNull(dr["addr_city"]))
                    lobjESIMedicareEnrollment.city = dr["addr_city"].ToString();

                if (!Convert.IsDBNull(dr["addr_state_value"]))
                    lobjESIMedicareEnrollment.addr_state = dr["addr_state_value"].ToString();

                if (!Convert.IsDBNull(dr["addr_zip_code"]))
                    lobjESIMedicareEnrollment.zip_code = dr["addr_zip_code"].ToString();

                if (!Convert.IsDBNull(dr["addr_zip_code"]))
                    lobjESIMedicareEnrollment.zip_4_code = dr["addr_zip_4_code"].ToString();

                if (!Convert.IsDBNull(dr["home_phone_number"]))
                    lobjESIMedicareEnrollment.home_phone_number = dr["home_phone_number"].ToString();

                if (!Convert.IsDBNull(dr["disenrollment_reason"]))
                    lobjESIMedicareEnrollment.disenrollment_reason = dr["disenrollment_reason"].ToString();

                if (!Convert.IsDBNull(dr["disenrollment_date"]))
                    lobjESIMedicareEnrollment.disenrollment_date = dr["disenrollment_date"].ToString();

                if (!Convert.IsDBNull(dr["current_dt"]))
                    lobjESIMedicareEnrollment.current_date = dr["current_dt"].ToString();

                if (!Convert.IsDBNull(dr["initial_enroll_date"]))
                    lobjESIMedicareEnrollment.initial_enroll_date = dr["initial_enroll_date"].ToString();

                _iclbESIMedicarePartDEnrollmentFile.Add(lobjESIMedicareEnrollment);
            }

            istrTotalCount = ((_iclbESIMedicarePartDEnrollmentFile.Count() + 2).ToString().PadLeft(9,'0')); //+2 : 1 for header and another for footer
        }

        //Updating ENROLLMENT_FILE_SENT_FLAG flag to 'Y'
        public override void FinalizeFile()
        {
            DBFunction.DBNonQuery("cdoPerson.UpdateEnrollmentFileSentFlag", new object[] { }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            base.FinalizeFile();
        }
    }
}
