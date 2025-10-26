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
	/// Class NeoSpin.DataObjects.doMasStatementFile:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasStatementFile : doBase
    {
         
         public doMasStatementFile() : base()
         {
         }
         public int mas_statement_file_id { get; set; }
         public int mas_selection_id { get; set; }
         public int statement_type_id { get; set; }
         public string statement_type_description { get; set; }
         public string statement_type_value { get; set; }
         public string statement_name { get; set; }
    }
    [Serializable]
    public enum enmMasStatementFile
    {
         mas_statement_file_id ,
         mas_selection_id ,
         statement_type_id ,
         statement_type_description ,
         statement_type_value ,
         statement_name ,
    }
}

