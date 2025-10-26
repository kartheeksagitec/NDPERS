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
	/// Class NeoSpin.CustomDataObjects.cdoFullAuditLog:
	/// Inherited from doFullAuditLog, the class is used to customize the database object doFullAuditLog.
	/// </summary>
    [Serializable]
	public class cdoFullAuditLog : doFullAuditLog
	{
		public cdoFullAuditLog() : base()
		{
		}

        public string istrChangeTypeDesc
        {
            get
            {
                switch (change_type)
                {
                    case "I": return "Insert";
                    case "U": return "Update";
                    case "D": return "Delete";
                    case "V": return "View";
                    default: return "Unknown";
                }
            }
        }

    } 
} 
