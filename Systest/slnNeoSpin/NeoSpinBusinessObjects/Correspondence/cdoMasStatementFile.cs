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
	/// Class NeoSpin.CustomDataObjects.cdoMasStatementFile:
	/// Inherited from doMasStatementFile, the class is used to customize the database object doMasStatementFile.
	/// </summary>
    [Serializable]
	public class cdoMasStatementFile : doMasStatementFile
	{
		public cdoMasStatementFile() : base()
		{
		}

        public string report_name
        {
            get
            {
                if (statement_type_value == "ONLN")
                    return "rptOnlineMemberStatement.rpt";
                else if (statement_type_value == "SUMM")
                    return "rptSummaryMemberStatement.rpt";
                else
                    return string.Empty;
            }
        }
    } 
} 
