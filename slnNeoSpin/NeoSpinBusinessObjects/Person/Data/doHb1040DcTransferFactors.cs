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
	/// Class NeoSpin.DataObjects.doHb1040DcTransferFactors:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doHb1040DcTransferFactors : doBase
    {
         public doHb1040DcTransferFactors() : base()
         {
         }
         public decimal age { get; set; }
         public decimal retirement_age { get; set; }
         public decimal factor { get; set; }
    }
    [Serializable]
    public enum enmHb1040DcTransferFactors
    {
         age ,
         retirement_age ,
         factor ,
    }
}
