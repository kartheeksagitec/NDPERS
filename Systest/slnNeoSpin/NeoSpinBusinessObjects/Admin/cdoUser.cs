#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.DataObjects;
#endregion

namespace NeoSpin.CustomDataObjects
{
  [Serializable]
  public class cdoUser : doUser
	{
      public cdoUser() : base() 
      { 

      }

	 public string User_full_name
	 {
			get
			{
				return last_name + " " + first_name;
			}
	 }
      public int Loggedin_user_id
      {
          get
          {
              return iobjPassInfo.iintUserSerialID;
          }
      }

      // this property will be used for Appointment Confirmation Correspondence
      public string User_Name
      {
          get
          {
              return first_name+" " + last_name;
          }
      }

  } 
} 
