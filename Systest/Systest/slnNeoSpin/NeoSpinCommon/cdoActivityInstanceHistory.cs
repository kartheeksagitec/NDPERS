#region Using directives

using System;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoActivityInstanceHistory:
	/// Inherited from doActivityInstanceHistory, the class is used to customize the database object doActivityInstanceHistory.
	/// </summary>
    [Serializable]
	public class cdoActivityInstanceHistory : doActivityInstanceHistory
	{
		public cdoActivityInstanceHistory() : base()
		{            
		}

        public string end_time_null_as_empty
        {
            get
            {
                if (end_time == DateTime.MinValue)
                    return string.Empty;
                return end_time.ToString();
            }
        }
    } 
} 
