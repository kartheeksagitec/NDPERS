#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;
using System.Data;
using Sagitec.BusinessObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoCorTemplates : doCorTemplates
	{
		public cdoCorTemplates() : base()
		{
		}

		public string formatted_associated_forms
		{
			get
			{
				if (associated_forms == null)
					return null;
				else
					return associated_forms.Replace(";", Environment.NewLine);
			}

			set
			{
                if (value == null)
                    associated_forms = null;
                else
                //associated_forms = value.Replace(" ","").Replace(" \n", ";").Replace("\n", ";").Replace(Environment.NewLine, ";") + ";";
                {
                    //dropdown of template name on related page is not getting filled if there is space in form names
                    string lstrresult = string.Empty;
                    string[] separators = { " ",";", "\n"};
                    string[] arr = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var str in arr)
                    {
                        lstrresult += str + ";";
                    }
                    associated_forms = lstrresult;
                }
			}
		}

		public void LoadByTemplateName(string astrTemplateName)
		{
			DataTable ldtbList = busBase.Select<cdoCorTemplates>( new string[1] { "template_name" },
				new object[1] { astrTemplateName }, null, null);
			//If no records found create PersonAccount
			if (ldtbList.Rows.Count == 1)
			{
				LoadData(ldtbList.Rows[0]);
			}
			else if (ldtbList.Rows.Count > 1)
			{
				throw new Exception("LoadByTemplateName method : Multiple templates returned for given template name : " +
					astrTemplateName);
			}
		}

        public string istrDcfnacode;
    } 
} 
