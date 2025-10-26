#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoPersonAccountWorkerCompensation : doPersonAccountWorkerCompensation
	{
		public cdoPersonAccountWorkerCompensation() : base()
		{
		}

        private string _provider_org_code;
        public string provider_org_code
        {
            get { return _provider_org_code; }
            set { _provider_org_code = value; }
        }

        //private string _provider_org_name;
        //public string provider_org_name
        //{
        //    get { return _provider_org_name; }
        //    set { _provider_org_name = value; }
        //}
    } 
} 
