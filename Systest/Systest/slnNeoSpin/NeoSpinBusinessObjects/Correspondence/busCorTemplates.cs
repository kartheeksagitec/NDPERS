#region Using directives

using System;
using System.Data;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busCorTemplates : busExtendBase
    {
		public busCorTemplates()
		{

		} 

		private cdoCorTemplates _icdoCorTemplates;
		public cdoCorTemplates icdoCorTemplates
		{
			get
			{
				return _icdoCorTemplates;
			}

			set
			{
				_icdoCorTemplates = value;
			}
		}

		public bool FindCorTemplates(int ainttemplateid)
		{

			bool lblnResult = false;
			if (_icdoCorTemplates == null)
			{
				_icdoCorTemplates = new cdoCorTemplates();
			}
			if (_icdoCorTemplates.SelectRow(new object[1] { ainttemplateid }))
			{
				lblnResult = true;
			}
			return lblnResult;
			
		}

        public bool FindCorTemplatesByTemplateName(string astrTemplateName)
        {
            if (_icdoCorTemplates == null)
            {
                _icdoCorTemplates = new cdoCorTemplates();
            }
            DataTable ldtbCorTemplates = Select<cdoCorTemplates>(new string[1] { "template_name" },
                  new object[1] { astrTemplateName }, null, null);
            if (ldtbCorTemplates.Rows.Count > 0)
            {
                _icdoCorTemplates.LoadData(ldtbCorTemplates.Rows[0]);                
                return true;
            }
            return false;
        }

 
        /* To check if online or batch is selected (atleast one must be selected)*/

        public bool CheckIfOnLineOrBatch()
        {
            if ((icdoCorTemplates.online_flag == "N") && (icdoCorTemplates.batch_flag == "N"))
            {
                return true;
            }
            return false;
        }

}

	
}
