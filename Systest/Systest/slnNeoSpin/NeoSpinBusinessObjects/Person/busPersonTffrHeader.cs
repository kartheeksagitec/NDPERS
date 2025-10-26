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
using Sagitec.CustomDataObjects;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
    public class busPersonTffrHeader : busNeoSpinBase
    {
        private doPersonTffrHeader _icdoPersonTffrHeader;
        public doPersonTffrHeader icdoPersonTffrHeader
        {
            get
            {
                return _icdoPersonTffrHeader;
            }
            set
            {
                _icdoPersonTffrHeader = value;
            }
        }
        public busPersonTffrHeader LoadPersonTffrHeaderFromUploadId(int aintUploadId)
        {
            DataTable ldtbPersonTffrHeader = Select<doPersonTffrHeader>(new string[1] { "upload_id" },
            new object[1] { aintUploadId }, null, null);
            if (ldtbPersonTffrHeader.Rows.Count > 0)
                this.icdoPersonTffrHeader.LoadData(ldtbPersonTffrHeader.Rows[0]);

            return this;
        }
        public ArrayList btnSaveNotes_Click()
        {
            ArrayList larrList = new ArrayList();

            this.icdoPersonTffrHeader.Update();

            larrList.Add(this);
            return larrList;
        }
        private Collection<busPersonTffrDetail> _iclbPersonTffrDetail;
        public Collection<busPersonTffrDetail> iclbPersonTffrDetail
        {
            get
            {
                return _iclbPersonTffrDetail;
            }
            set
            {
                _iclbPersonTffrDetail = value;
            }
        }
        public int iintFileRecordType { get; set; }
        public string istrPersonSSNFromFile { get; set; }
        public decimal idecServiceCreditForYear { get; set; } //total months
        public int iintYearOfService { get; set; }
        public cdoFileHdr icdoFileHdr { get; set; }
        public cdoFileDtl icdoFileDtl { get; set; }

        public bool iblnIsFromFile { get; set; }

        public void LoadPersonTffrDetailsFromUploadId(int aintUploadId)
        {
            if (iclbPersonTffrDetail.IsNull())
            {
                iclbPersonTffrDetail = new Collection<busPersonTffrDetail>();
            }
            DataTable ldtbPersonTffrDetail = busNeoSpinBase.Select("entPersonTffrDetail.GetPersonTffrDetailsByUploadId",
                                              new object[1] { aintUploadId });
            if (ldtbPersonTffrDetail.IsNotNull() && ldtbPersonTffrDetail.Rows.Count > 0) {
                foreach (DataRow ldr in ldtbPersonTffrDetail.Rows)
                {
                    busPersonTffrDetail lobjPersonTffrDetail = new busPersonTffrDetail { icdoPersonTffrDetail = new doPersonTffrDetail() };
                    lobjPersonTffrDetail.icdoPersonTffrDetail.LoadData(ldr);
                    lobjPersonTffrDetail.istrTffrWage = Convert.ToString(ldr["istrTffrWage"]);
                    iclbPersonTffrDetail.Add(lobjPersonTffrDetail);
                }
            }
        }
        /// <summary>
        /// Create empty Person TFFR Detail record and Add it to the Collection, used in File Processing
        /// </summary>
        /// <returns></returns>
        public busPersonTffrDetail CreateNewPersonTffrDetail(int aintFileDtlLineNo)
        {
            if (_icdoPersonTffrHeader == null)
            {
                _icdoPersonTffrHeader = new doPersonTffrHeader();
            }
            busPersonTffrDetail lobjPersonTffrDtl = new busPersonTffrDetail { icdoPersonTffrDetail = new doPersonTffrDetail() };
            if (_iclbPersonTffrDetail == null)
                _iclbPersonTffrDetail = new Collection<busPersonTffrDetail>();

            lobjPersonTffrDtl.icdoPersonTffrDetail.line_no = aintFileDtlLineNo;

            _iclbPersonTffrDetail.Add(lobjPersonTffrDtl);
            return lobjPersonTffrDtl;
        }
    }
}
