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
	public class cdoPersonAccountLife : doPersonAccountLife
	{
		public cdoPersonAccountLife() : base()
		{
		}

        // Age of Enrolling Person.
        private int _Life_Insurance_Age;
        public int Life_Insurance_Age
        {
            get { return _Life_Insurance_Age; }
            set { _Life_Insurance_Age = value; }
        }

        private string _Provider_Name;
        public string Provider_Name
        {
            get { return _Provider_Name; }
            set { _Provider_Name = value; }
        }

        public string IsEndDatedDueToLossOfSuppLife { get; set; }
    } 
} 
