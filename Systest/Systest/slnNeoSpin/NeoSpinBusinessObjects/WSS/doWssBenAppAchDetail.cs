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
	/// Class NeoSpin.DataObjects.doWssBenAppAchDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssBenAppAchDetail : doBase
    {
         public doWssBenAppAchDetail() : base()
         {
         }
         public int wss_ben_app_ach_detail_id { get; set; }
         public int wss_ben_app_id { get; set; }
         public string routing_no { get; set; }
         public string bank_name { get; set; }
         public int bank_account_type_id { get; set; }
         public string bank_account_type_description { get; set; }
         public string bank_account_type_value { get; set; }
         public string account_number { get; set; }
         public string primary_account_flag { get; set; }
         public decimal percentage_of_net_amount { get; set; }
         public decimal partial_amount { get; set; }
         public string ach_type { get; set; }
    }
    [Serializable]
    public enum enmWssBenAppAchDetail
    {
         wss_ben_app_ach_detail_id ,
         wss_ben_app_id ,
         routing_no ,
         bank_name ,
         bank_account_type_id ,
         bank_account_type_description ,
         bank_account_type_value ,
         account_number ,
         primary_account_flag ,
         percentage_of_net_amount ,
         partial_amount ,
         ach_type ,
    }
}

