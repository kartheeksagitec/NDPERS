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
	[Serializable]
	public class busProcessInstanceChecklist : busProcessInstanceChecklistGen
	{
        private string _istrDocumentCode;

        public string istrDocumentCode
        {
            get { return _istrDocumentCode; }
            set { _istrDocumentCode = value; }
        }
        private busDocument _ibusDocument;
        public busDocument ibusDocument
        {
            get
            {
                return _ibusDocument;
            }
            set
            {
                _ibusDocument = value;
            }
        }

        public void LoadDocument()
        {
            if (ibusDocument == null)
            { ibusDocument = new busDocument(); }
            ibusDocument.FindDocumentByDocumentCode(istrDocumentCode);
        }
	
	}
}
