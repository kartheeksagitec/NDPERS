#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using NeoSpin.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoGlTransaction : doGlTransaction
	{
		public cdoGlTransaction() : base()
		{
		}

        private int _Extract_File_ID;
        public int Extract_File_ID
        {
            get { return _Extract_File_ID; }
            set { _Extract_File_ID = value; }
        }

        //this derived variable will be passed as navigation parameter to open respective maintenance screens from GL look up        
        public int source_id_derived { get; set; }
    } 
} 
