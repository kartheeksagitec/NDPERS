#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Sagitec.DataObjects;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
  [Serializable]
  public class cdoSystemPaths : doSystemPaths
	{
      public cdoSystemPaths() : base() 
      { 

      } 

  } 
} 
