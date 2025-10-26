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
	/// Class NeoSpin.DataObjects.doComments:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doComments : doBase
    {
         
         public doComments() : base()
         {
         }
         public int comment_id { get; set; }
         public int employer_payroll_header_id { get; set; }
         public int employer_payroll_detail_id { get; set; }
         public string comments { get; set; }
    }
    [Serializable]
    public enum enmComments
    {
         comment_id ,
         employer_payroll_header_id ,
         employer_payroll_detail_id ,
         comments ,
    }
}

