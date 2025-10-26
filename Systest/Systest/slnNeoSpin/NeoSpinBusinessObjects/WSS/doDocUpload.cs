#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doDocUpload:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDocUpload : doBase
    {
         
         public doDocUpload() : base()
         {
         }
         public int upload_id { get; set; }
         public int document_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int contact_id { get; set; }
         public int doc_status_id { get; set; }
         public string doc_status_description { get; set; }
         public string doc_status_value { get; set; }
         public DateTime uploaded_date { get; set; }
         public DateTime imaged_date { get; set; }
         public string converted_to_image_flag { get; set; }
    }
    [Serializable]
    public enum enmDocUpload
    {
         upload_id ,
         document_id ,
         person_id ,
         org_id ,
         contact_id ,
         doc_status_id ,
         doc_status_description ,
         doc_status_value ,
         uploaded_date ,
         imaged_date ,
         converted_to_image_flag ,
    }
}

