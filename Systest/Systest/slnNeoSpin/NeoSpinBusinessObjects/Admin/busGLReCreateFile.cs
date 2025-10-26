using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{   
    [Serializable]
    public class busGLReCreateFile : busExtendBase
    {
        public busGLReCreateFile()
        {

        }

        private cdoGlTransaction _icdoGLTransaction;
        public cdoGlTransaction icdoGLTransaction
        {
            get { return _icdoGLTransaction; }
            set { _icdoGLTransaction = value; }
        }

        private Collection<busGLTransaction> _iclbGLTransaction;
        public Collection<busGLTransaction> iclbGLTransaction
        {
            get { return _iclbGLTransaction; }
            set { _iclbGLTransaction = value; }
        }

        public void LoadCollection()
        {
            if (_icdoGLTransaction.extract_date != DateTime.MinValue)
            {
                DataTable ldtGLTransaction = Select("entGLTransaction.GetExtractFileName", new object[1] { _icdoGLTransaction.extract_date });
                _iclbGLTransaction = GetCollection<busGLTransaction>(ldtGLTransaction, "icdoGlTransaction");
            }
        }

        public ArrayList ExtractFile_Click()
        {
            ArrayList larrList = new ArrayList();
            LoadCollection();
            larrList.Add(this);
            return larrList;
        }

        public void btnGLReCreateFile_Click(ArrayList aarrSelectedObjects)
        {
            busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
            if (aarrSelectedObjects != null)
            {
                if (aarrSelectedObjects.Count > 0)
                {
                    foreach (busGLTransaction lobjTransaction in aarrSelectedObjects)
                    {
                        lobjProcessFiles.iarrParameters = new object[2];
                        lobjProcessFiles.iarrParameters[0] = lobjTransaction.icdoGlTransaction.extract_file_name;
                        lobjProcessFiles.iarrParameters[1] = icdoGLTransaction.extract_date;
                        lobjProcessFiles.CreateOutboundFile(busConstant.GLReCreateFileID);
                    }
                }
            }
        }
    }
}
