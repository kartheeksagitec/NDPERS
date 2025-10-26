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
	/// Class NeoSpin.DataObjects.doDocument:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDocument : doBase
    {
         
         public doDocument() : base()
         {
         }
         public int document_id { get; set; }
         public string document_code { get; set; }
         public string document_name { get; set; }
         public string ignore_process_flag { get; set; }
        public string wss { get; set; }
        public string filenet_document_type { get; set; }
    }
    [Serializable]
    public enum enmDocument
    {
         document_id ,
         document_code ,
         document_name ,
         ignore_process_flag ,
        wss,
        filenet_document_type,
    }
}

