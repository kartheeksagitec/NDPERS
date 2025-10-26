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
    /// <summary>
    /// Class NeoSpin.busDocUpload:
    /// Inherited from busDocUploadGen, the class is used to customize the business object busDocUploadGen.
    /// </summary>
    [Serializable]
    public class busDocUpload : busDocUploadGen
    {
        public string istrUploadedTiffFileName
        {
            get
            {
                if (icdoDocUpload.document_id > 0 && icdoDocUpload.upload_id > 0)
                    return Convert.ToString(icdoDocUpload.document_id) + "-" + Convert.ToString(icdoDocUpload.upload_id).PadLeft(10, '0') + ".tif";
                return string.Empty;
            }
        }

        public string istrUploadedFileName
        {
            get
            {
                if (icdoDocUpload.document_id > 0 && icdoDocUpload.upload_id > 0)
                    return Convert.ToString(icdoDocUpload.document_id) + "-" + Convert.ToString(icdoDocUpload.upload_id).PadLeft(10, '0');
                return string.Empty;
            }
        }
        public string istrUserUploadedFileName { get; set; }
    }
}
