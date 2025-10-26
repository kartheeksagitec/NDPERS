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
	public class busDocumentProcessCrossrefGen : busExtendBase
    {
		public busDocumentProcessCrossrefGen()
		{

		}

		private cdoDocumentProcessCrossref _icdoDocumentProcessCrossref;
		public cdoDocumentProcessCrossref icdoDocumentProcessCrossref
		{
			get
			{
				return _icdoDocumentProcessCrossref;
			}
			set
			{
				_icdoDocumentProcessCrossref = value;
			}
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

		public bool FindDocumentProcessCrossref(int Aintdocumentprocesscrossrefid)
		{
			bool lblnResult = false;
			if (_icdoDocumentProcessCrossref == null)
			{
				_icdoDocumentProcessCrossref = new cdoDocumentProcessCrossref();
			}
			if (_icdoDocumentProcessCrossref.SelectRow(new object[1] { Aintdocumentprocesscrossrefid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public void LoadDocument()
		{
			if (_ibusDocument == null)
			{
				_ibusDocument = new busDocument();
			}			
			_ibusDocument.FindDocument(_icdoDocumentProcessCrossref.document_id);
		}

        /// <summary>
        /// Gets or sets the non-collection object of type busProcess.
        /// </summary>
        public busProcess ibusProcess { get; set; }


        /// <summary>
        /// NeoSpin.busActivityGen.LoadProcess():
        /// Loads non-collection object ibusProcess of type busProcess.
        /// </summary>
        public virtual void LoadProcess()
        {
            if (ibusProcess == null)
            {
                ibusProcess = new busProcess();
            }
            ibusProcess.FindProcess(icdoDocumentProcessCrossref.process_id);
        }
	}
}
