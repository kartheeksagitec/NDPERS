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
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busTFFRDualServiceImportInboundFile : busFileBase
    {
        public busTFFRDualServiceImportInboundFile()
        { }
        private busPersonTffrDetail _ibusPersonTffrDetail;
        public busPersonTffrDetail ibusPersonTffrDetail
        {
            get { return _ibusPersonTffrDetail; }
            set { _ibusPersonTffrDetail = value; }
        }
        private busPersonTffrHeader _ibusPersonTffrHeader;
        public busPersonTffrHeader ibusPersonTffrHeader
        {
            get { return _ibusPersonTffrHeader; }
            set { _ibusPersonTffrHeader = value; }
        }

        private int GetPersonIdFromSSN(string astrSSN)
        {
            DataTable ldtbPerson = DBFunction.DBSelect("cdoPerson.GetPersonBySSN", new string[1] { astrSSN },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPerson.IsNotNull() && ldtbPerson.Rows.Count > 0)
            {
                return Convert.ToInt32(ldtbPerson.Rows[0]["person_id"]);
            }
            return 0;
        }

        private static int lintHeaderGroupValue = 0;
        private string lstrHeaderGroupValue = "0";
        private int lintUploadId = 1;
        public int lintLatestUploadId = 1;

        public bool iblnNoDetailRecordsForHeader { get; set; }
        public bool iblnIsHeaderForExistingPerson { get; set; }
        public Collection<busPersonTffrHeader> iclbPersonIdsFromFiles { get; set; }
        //Setting the Header Group Values if multiple SSN comes in the Same File
        public override void SetHeaderGroupValue()
        {
            if (icdoFileDtl != null)
            {
                if (icdoFileDtl.transaction_code_value == "1")
                {
                    lintHeaderGroupValue++;
                    if (lintHeaderGroupValue < 10)
                        lstrHeaderGroupValue = "0" + lintHeaderGroupValue.ToString();
                    else
                        lstrHeaderGroupValue = lintHeaderGroupValue.ToString();
                }
                icdoFileDtl.header_group_value = lstrHeaderGroupValue;
            }
        }
        public override busBase NewHeader()
        {
            _ibusPersonTffrHeader = new busPersonTffrHeader { icdoPersonTffrHeader = new doPersonTffrHeader() };
            return _ibusPersonTffrHeader;
        }
        public override busBase NewDetail()
        {
            if (icdoFileDtl.transaction_code_value == "1")
            {
                _ibusPersonTffrHeader.icdoFileDtl = icdoFileDtl;
                return _ibusPersonTffrHeader;
            }
            //sending line no from file dtl as we also need to save it
            return _ibusPersonTffrHeader.CreateNewPersonTffrDetail(icdoFileDtl.line_no);
        }
        public override void ProcessHeader()
        {
            //SGT_PERSON_TFFR_HEADER include Upload_ID (all records for the same SSN per file should have the same Upload_ID), Person_ID, Upload_Date, Notes
            //person_id - get from SSN
            if (!(String.IsNullOrEmpty(_ibusPersonTffrHeader.istrPersonSSNFromFile)))
            {
                _ibusPersonTffrHeader.icdoPersonTffrHeader.person_id = GetPersonIdFromSSN(_ibusPersonTffrHeader.istrPersonSSNFromFile);
            }
            // If the File has multiple header, increment the existing Upload_ID
            // But all records for the same SSN per file should have the same Upload_ID
            if (lintUploadId == lintLatestUploadId)
            {
                lintUploadId = Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonTffrHeader.GetMaxUploadId", new object[] { },
                                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) + 1;
                lintLatestUploadId = lintUploadId;
            }
            else
            {
                //reset the upload id to be incrementing from where it left off incase there are multiple header for 1 member.
                lintUploadId = lintLatestUploadId;
            }

            if (iclbPersonIdsFromFiles.IsNotNull())
            {
                if (iclbPersonIdsFromFiles.Where(i => i.icdoPersonTffrHeader.person_id == _ibusPersonTffrHeader.icdoPersonTffrHeader.person_id).Any())
                {
                    iblnIsHeaderForExistingPerson = true;
                    lintUploadId = Convert.ToInt32((iclbPersonIdsFromFiles.Where(i => i.icdoPersonTffrHeader.person_id == _ibusPersonTffrHeader.icdoPersonTffrHeader.person_id).FirstOrDefault()).icdoPersonTffrHeader.upload_id);
                }
                else
                {
                    iblnIsHeaderForExistingPerson = false;
                }
            }
            else
            {
                iclbPersonIdsFromFiles = new Collection<busPersonTffrHeader>();
            }

            _ibusPersonTffrHeader.icdoPersonTffrHeader.upload_id = lintUploadId;
            _ibusPersonTffrHeader.icdoPersonTffrHeader.upload_date = DateTime.Now;
            _ibusPersonTffrHeader.icdoPersonTffrHeader.notes = string.Empty;

            iclbPersonIdsFromFiles.Add(_ibusPersonTffrHeader);

            base.ProcessHeader();
        }

        public override void BeforePersisitHeader()
        {
            if (_ibusPersonTffrHeader.iclbPersonTffrDetail == null)
            {
                _ibusPersonTffrHeader.iclbPersonTffrDetail = new Collection<busPersonTffrDetail>();
            }
            _ibusPersonTffrHeader.iclbPersonTffrDetail = _ibusPersonTffrHeader.iclbPersonTffrDetail.OrderBy(i => i.icdoPersonTffrDetail.line_no).ToList().ToCollection();
            //Validation
            if (Convert.ToInt32(_ibusPersonTffrHeader.idecServiceCreditForYear) > 0 && (_ibusPersonTffrHeader.iclbPersonTffrDetail.Count == 0))
            {
                iblnNoDetailRecordsForHeader = true;
            }
            else
            {
                //Inserting the Header Record
                if (iblnIsHeaderForExistingPerson)
                {
                    _ibusPersonTffrHeader.icdoPersonTffrHeader.person_tffr_header_id = Convert.ToInt32((iclbPersonIdsFromFiles.Where(i => i.icdoPersonTffrHeader.upload_id == lintUploadId).FirstOrDefault()).icdoPersonTffrHeader.person_tffr_header_id);
                }
                else
                {
                    _ibusPersonTffrHeader.icdoPersonTffrHeader.Insert();
                }
                foreach (busPersonTffrDetail lobjPersonTffrDtl in _ibusPersonTffrHeader.iclbPersonTffrDetail)
                {
                    //Upload ID set for Detail recods
                    lobjPersonTffrDtl.icdoPersonTffrDetail.upload_id = _ibusPersonTffrHeader.icdoPersonTffrHeader.upload_id;
                }
            }
        }
        public override void AfterPersisitHeader()
        {
            //insert File Record Type = 1 in the TFFR Detail table as well
            if (!iblnNoDetailRecordsForHeader)
            {
                DBFunction.DBNonQuery("entPersonTffrDetail.InsertHeaderLineDataToDetailTable", new object[6] {
                                    _ibusPersonTffrHeader.icdoPersonTffrHeader.person_tffr_header_id, _ibusPersonTffrHeader.icdoPersonTffrHeader.upload_id,
                                    1, _ibusPersonTffrHeader.icdoFileDtl.line_no,
                                    _ibusPersonTffrHeader.iintYearOfService, _ibusPersonTffrHeader.idecServiceCreditForYear}, 
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            //reset flag
            iblnIsHeaderForExistingPerson = false;
            base.AfterPersisitHeader();
        }
        public override bool ValidateFile()
        {
            if (iblnNoDetailRecordsForHeader)
            {
                this.istrError = "No detail records with service included";
                return false;
            }
            return true;
        }
    }
}
