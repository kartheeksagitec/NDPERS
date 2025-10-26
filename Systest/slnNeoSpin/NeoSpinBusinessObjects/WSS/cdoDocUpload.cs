#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoDocUpload:
	/// Inherited from doDocUpload, the class is used to customize the database object doDocUpload.
	/// </summary>
    [Serializable]
	public class cdoDocUpload : doDocUpload
	{
		public cdoDocUpload() : base()
		{
		}
        public string istrUploadedDocumentCode { get; set; }
    } 
} 
            