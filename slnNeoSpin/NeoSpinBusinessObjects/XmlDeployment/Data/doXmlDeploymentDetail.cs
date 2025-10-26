#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
using System.Web.UI.WebControls;
#endregion
namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doXmlDeploymentDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doXmlDeploymentDetail : doBase
    {
         
         public doXmlDeploymentDetail() : base()
         {
         }
         public int xml_deployment_detail_id { get; set; }
         public int deployment_header_id { get; set; }
         public string xml_document_name { get; set; }
         public Image cached_data { get; set; }
         public string xml_document_hashcode { get; set; }
         public string detail_status { get; set; }
         public string deployment_message { get; set; }
    }
    [Serializable]
    public enum enmXmlDeploymentDetail
    {
         xml_deployment_detail_id ,
         deployment_header_id ,
         xml_document_name ,
         cached_data ,
         xml_document_hashcode ,
         detail_status ,
         deployment_message ,
    }
}
