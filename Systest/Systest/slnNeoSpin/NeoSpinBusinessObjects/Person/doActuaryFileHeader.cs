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
	/// Class NeoSpin.DataObjects.doActuaryFileHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doActuaryFileHeader : doBase
    {
         
         public doActuaryFileHeader() : base()
         {
         }
         public int actuary_file_header_id { get; set; }
         public int file_type_id { get; set; }
         public string file_type_description { get; set; }
         public string file_type_value { get; set; }
         public int pension_file_type_id { get; set; }
         public string pension_file_type_description { get; set; }
         public string pension_file_type_value { get; set; }
         public DateTime effective_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int plan_id { get; set; }
    }
    [Serializable]
    public enum enmActuaryFileHeader
    {
         actuary_file_header_id ,
         file_type_id ,
         file_type_description ,
         file_type_value ,
         pension_file_type_id ,
         pension_file_type_description ,
         pension_file_type_value ,
         effective_date ,
         status_id ,
         status_description ,
         status_value ,
         plan_id ,
    }
}

