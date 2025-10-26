#region Using directives

using System;
using Sagitec.Common;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busDocumentProcessCrossref : busDocumentProcessCrossrefGen
    {

        private string _istrDocumentCode;

        public string istrDocumentCode
        {
            get { return _istrDocumentCode; }
            set { _istrDocumentCode = value; }
        }

        private string _istrDocumentName;

        public string istrDocumentName
        {
            get { return _istrDocumentName; }
            set { _istrDocumentName = value; }
        }
        public void LoadDocumentByCode()
        {
            if (ibusDocument == null)
            { ibusDocument = new busDocument(); }
            ibusDocument.FindDocumentByDocumentCode(istrDocumentCode);
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (istrDocumentCode.IsNotNullOrEmpty())
            {
                LoadDocumentByCode();
            }
            else
            {
                ibusDocument = new busDocument { icdoDocument = new cdoDocument() };
            }
            icdoDocumentProcessCrossref.document_id = ibusDocument.icdoDocument.document_id;
            base.BeforeValidate(aenmPageMode);
        }
    }
}
