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
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busWssMemberRecordRequestGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssMemberRecordRequest and its children table. 
    /// </summary>
    [Serializable]
    public class busWssMemberRecordRequestGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busWssMemberRecordRequestGen
        /// </summary>
        public busWssMemberRecordRequestGen()
        {

        }

        /// <summary>
        /// Gets or sets the main-table object contained in busWssMemberRecordRequestGen.
        /// </summary>
        public cdoWssMemberRecordRequest icdoWssMemberRecordRequest { get; set; }
        public cdoWssPersonAddress icdoWssPersonAddress { get; set; }
        public cdoWssPersonContact icdoWssPersonContact { get; set; }
        public cdoWssPersonEmployment icdoWssPersonEmployment { get; set; }
        public cdoWssPersonEmploymentDetail icdoWssPersonEmploymentDetail { get; set; }

        /// <summary>
        /// NeoSpin.busWssMemberRecordRequestGen.FindWssMemberRecordRequest():
        /// Finds a particular record from cdoWssMemberRecordRequest with its primary key. 
        /// </summary>
        /// <param name="aintmemberrecordrequestid">A primary key value of type int of cdoWssMemberRecordRequest on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindWssMemberRecordRequest(int aintmemberrecordrequestid)
        {
            bool lblnResult = false;
            if (icdoWssMemberRecordRequest == null)
            {
                icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest();
            }
            if (icdoWssMemberRecordRequest.SelectRow(new object[1] { aintmemberrecordrequestid }))
            {
                lblnResult = true;
                DataTable ldtrWSSAdderess = Select<cdoWssPersonAddress>(new string[1] { "member_record_request_id" },
                    new object[1] { icdoWssMemberRecordRequest.member_record_request_id }, null, null);
                if (icdoWssPersonAddress == null) icdoWssPersonAddress = new cdoWssPersonAddress();
                if (ldtrWSSAdderess.Rows.Count > 0)
                    icdoWssPersonAddress.LoadData(ldtrWSSAdderess.Rows[0]);
                DataTable ldtrWSSPersonContact = Select<cdoWssPersonContact>(new string[1] { "member_record_request_id" },
                   new object[1] { icdoWssMemberRecordRequest.member_record_request_id }, null, null);
                if (icdoWssPersonContact == null) icdoWssPersonContact = new cdoWssPersonContact();
                if (ldtrWSSPersonContact.Rows.Count > 0)
                {
                    icdoWssPersonContact.LoadData(ldtrWSSPersonContact.Rows[0]);
                    SetSpouseProperties();
                }
                if (icdoWssPersonEmployment == null) icdoWssPersonEmployment = new cdoWssPersonEmployment();
                DataTable ldtrWSSPersonEmployment = Select<cdoWssPersonEmployment>(new string[1] { "member_record_request_id" },
                   new object[1] { icdoWssMemberRecordRequest.member_record_request_id }, null, null);
                if (icdoWssPersonEmployment == null) icdoWssPersonEmployment = new cdoWssPersonEmployment();
                if (ldtrWSSPersonEmployment.Rows.Count > 0)
                    icdoWssPersonEmployment.LoadData(ldtrWSSPersonEmployment.Rows[0]);
                DataTable ldtrWSSPersonEmploymentDetail = Select<cdoWssPersonEmploymentDetail>(new string[1] { "member_record_request_id" },
                   new object[1] { icdoWssMemberRecordRequest.member_record_request_id }, null, null);
                if (icdoWssPersonEmploymentDetail == null)
                    icdoWssPersonEmploymentDetail = new cdoWssPersonEmploymentDetail();
                if (ldtrWSSPersonEmploymentDetail.Rows.Count > 0)
                    icdoWssPersonEmploymentDetail.LoadData(ldtrWSSPersonEmploymentDetail.Rows[0]);
                icdoWssMemberRecordRequest.ReenterSSN = icdoWssMemberRecordRequest.ssn;
                icdoWssPersonEmploymentDetail.istrMemberWorkLessThan12MonthsValue = !string.IsNullOrEmpty(icdoWssPersonEmploymentDetail.seasonal_value) ? "YES" : "NO";
            }
            return lblnResult;
        }

        /// <summary>
        /// Used to set Spouse contact properties
        /// </summary>
        public void SetSpouseProperties()
        {
            string[] spouseName=null;
            if(!string.IsNullOrEmpty(icdoWssPersonContact.contact_name))
                spouseName = icdoWssPersonContact.contact_name.Split(' ');
            if (spouseName.IsNotNull())
            {
                icdoWssPersonContact.contact_first_name = spouseName[0];
                if (spouseName.Count() == 2) // if name does not contain middle name
                {
                    icdoWssPersonContact.contact_last_name = spouseName[1];
                }
                else if (spouseName.Count() > 2) // if name contains middle name
                {
                    icdoWssPersonContact.contact_middle_name = spouseName[1];
                    icdoWssPersonContact.contact_last_name = spouseName[2];
                }
            }
            icdoWssPersonContact.ReenterContactSSN = icdoWssPersonContact.contact_ssn;  
        }
    }
}