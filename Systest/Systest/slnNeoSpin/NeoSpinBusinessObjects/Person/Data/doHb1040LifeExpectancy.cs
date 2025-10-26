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
	/// Class NeoSpin.DataObjects.doHb1040LifeExpectancy:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doHb1040LifeExpectancy : doBase
    {
         public doHb1040LifeExpectancy() : base()
         {
         }
         public decimal age { get; set; }
         public decimal male { get; set; }
         public decimal female { get; set; }
    }
    [Serializable]
    public enum enmHb1040LifeExpectancy
    {
         age ,
         male ,
         female ,
    }
}
