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
	public class cdoOrgContactRole : doOrgContactRole
	{
		public cdoOrgContactRole() : base()
		{
		}

        private int _plan_id;
        public int plan_id
        {
            get
            {
                return _plan_id;
            }

            set
            {
                _plan_id = value;
            }
        }

        private string _plan_name;
        public string plan_name
        {
            get
            {
                return _plan_name;
            }

            set
            {
                _plan_name = value;
            }
        }
    } 
} 
