#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busResourcesScreen : busExtendBase
    {
		public busResourcesScreen()
		{
		}
        private string _istrResourceFileName;
        public string istrResourceFileName
        {
            get
            {
                return _istrResourceFileName;
            }
            set
            {
                _istrResourceFileName = value;
            }
        }
        private string _istrResourceElement;
        public string istrResourceElement
        {
            get
            {
                return _istrResourceElement;
            }
            set
            {
                _istrResourceElement = value;
            }
        }
        private string _istrResourceID;
        public string istrResourceID
        {
            get
            {
                return _istrResourceID;
            }
            set
            {
                _istrResourceID = value;
            }
        }
    }
}
