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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoUserActivityLogQueries:
	/// Inherited from doUserActivityLogQueries, the class is used to customize the database object doUserActivityLogQueries.
	/// </summary>
    [Serializable]
	public class cdoUserActivityLogQueries : doUserActivityLogQueries
	{
		public cdoUserActivityLogQueries() : base()
		{
		}

        public double iintTimeTakenInMS
        {
            get
            {
                return end_time.Subtract(start_time).TotalMilliseconds;
            }
        }

        public int param_count { get; set; }

    } 
} 
