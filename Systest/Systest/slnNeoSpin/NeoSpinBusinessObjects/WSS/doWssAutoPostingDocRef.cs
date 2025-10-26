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
	/// Class NeoSpin.DataObjects.doWssAutoPostingDocRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssAutoPostingDocRef : doBase
    {
         
         public doWssAutoPostingDocRef() : base()
         {
         }
         public int auto_posting_cross_ref_id { get; set; }
         public string document_code { get; set; }
    }
    [Serializable]
    public enum enmWssAutoPostingDocRef
    {
         auto_posting_cross_ref_id ,
         document_code ,
    }
}

