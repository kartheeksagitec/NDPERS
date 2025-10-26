using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busMLCFileOut:busFileBaseOut
    {
        public busMLCFileOut()
        {
        }

        private Collection<busMailingAddress> _iclbMailingAddress;
        public Collection<busMailingAddress> iclbMailingAddress
        {
            get { return _iclbMailingAddress; }
            set { _iclbMailingAddress = value; }
        }
        
        int lintMailingBatchID;

        String lstrFileName = string.Empty;

        public override void InitializeFile()
        {
            istrFileName = GetFileName(lintMailingBatchID) + busConstant.FileFormatcsv;
        }

        public string GetFileName(int aintMailingBatchID)
        {
            busMailingLabel lobjMailingLabel = new busMailingLabel();
            lobjMailingLabel.FindMailingLabel(aintMailingBatchID);
            return lobjMailingLabel.icdoMailingLabel.file_name;
        }
        
        public void LoadMailingLabelClient(DataTable ldtbMailingLabelClient)
        {
            lintMailingBatchID = Convert.ToInt32(iarrParameters[0]);
            busMailingLabel lobjMailingLabel = new busMailingLabel();
            _iclbMailingAddress = new Collection<busMailingAddress>();
            _iclbMailingAddress = lobjMailingLabel.GetSearchResult(lintMailingBatchID);
            foreach (busMailingAddress lobjMailingAddress in _iclbMailingAddress)
            {
                if (lobjMailingAddress.icdoMailingAddress.lstrFullName != null)
                    lobjMailingAddress.icdoMailingAddress.lstrFullName=lobjMailingAddress.icdoMailingAddress.lstrFullName.Trim().ToUpper();
                if (lobjMailingAddress.icdoMailingAddress.lstrContactName != null)
                    lobjMailingAddress.icdoMailingAddress.lstrContactName=lobjMailingAddress.icdoMailingAddress.lstrContactName.Trim().ToUpper();
                if(lobjMailingAddress.icdoMailingAddress.lstrAddressLine1!=null)
                    lobjMailingAddress.icdoMailingAddress.lstrAddressLine1=lobjMailingAddress.icdoMailingAddress.lstrAddressLine1.Trim().ToUpper();
                if(lobjMailingAddress.icdoMailingAddress.lstrAddressLine2!=null)
                    lobjMailingAddress.icdoMailingAddress.lstrAddressLine2=lobjMailingAddress.icdoMailingAddress.lstrAddressLine2.Trim().ToUpper();
                if (lobjMailingAddress.icdoMailingAddress.lstrCity != null)
                    lobjMailingAddress.icdoMailingAddress.lstrCity=lobjMailingAddress.icdoMailingAddress.lstrCity.Trim().ToUpper();
                if (lobjMailingAddress.icdoMailingAddress.lstrCountry != null)
                    lobjMailingAddress.icdoMailingAddress.lstrCountry=lobjMailingAddress.icdoMailingAddress.lstrCountry.Trim().ToUpper();
                if (lobjMailingAddress.icdoMailingAddress.lstrState != null)
                    lobjMailingAddress.icdoMailingAddress.lstrState=lobjMailingAddress.icdoMailingAddress.lstrState.Trim().ToUpper();

            }
        }
      

    }
}
