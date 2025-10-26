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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;

#endregion
namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busTFFRSSNOutboundFile : busFileBaseOut
    {
        public busTFFRSSNOutboundFile()
        { }

        private Collection<busTFFRSSNOutFile> _iclbTFFRSSNOutFile;

        public Collection<busTFFRSSNOutFile> iclbTFFRSSNOutFile
        {
            get { return _iclbTFFRSSNOutFile; }
            set { _iclbTFFRSSNOutFile = value; }
        }
        public Collection<busPerson> iclbPerson { get; set; }

        public override void InitializeFile()
        {
            istrFileName = "TFFR_SSN_Match_" + DateTime.Now.ToString(busConstant.DateFormat) + busConstant.FileFormattxt;
            
        }
        ///// <summary>
        ///// Method which takes the person details from batch.
        ///// </summary>
        public void LoadPersonDetails(DataTable ldtPerson)
        {
            if (iclbTFFRSSNOutFile == null)
                iclbTFFRSSNOutFile = new Collection<busTFFRSSNOutFile>();
            iclbTFFRSSNOutFile = (Collection<busTFFRSSNOutFile>)iarrParameters[0];
        }
    }
}