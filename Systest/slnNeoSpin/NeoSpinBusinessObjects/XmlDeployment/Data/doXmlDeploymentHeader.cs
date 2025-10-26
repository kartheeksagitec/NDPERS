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
	/// Class NeoSpin.DataObjects.doXmlDeploymentHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doXmlDeploymentHeader : doBase
    {
         
         public doXmlDeploymentHeader() : base()
         {
         }
         public int xml_deployment_header_id { get; set; }
         public string deployed_from_machine { get; set; }
         public string deployed_from_location { get; set; }
         public DateTime date_created { get; set; }
    }
    [Serializable]
    public enum enmXmlDeploymentHeader
    {
         xml_deployment_header_id ,
         deployed_from_machine ,
         deployed_from_location ,
         date_created ,
    }
}
