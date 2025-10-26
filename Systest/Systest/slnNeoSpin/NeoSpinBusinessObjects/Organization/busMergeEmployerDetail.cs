#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busMergeEmployerDetail : busMergeEmployerDetailGen
	{
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }
     
        private busMergeEmployerHeader _ibusMergeEmployerHeader;
        public busMergeEmployerHeader ibusMergeEmployerHeader
        {
            get { return _ibusMergeEmployerHeader; }
            set { _ibusMergeEmployerHeader = value; }
        }        

        public void LoadPerson()
        {
            if(_ibusPerson==null)
            {
                _ibusPerson=new busPerson();
            }
            _ibusPerson.FindPerson(icdoMergeEmployerDetail.person_id);
        }
        public void LoadMergeEmployerHeader()
        {
            if (_ibusMergeEmployerHeader == null)
            {
                _ibusMergeEmployerHeader = new busMergeEmployerHeader();
            }
            _ibusMergeEmployerHeader.FindMergeEmployerHeader(icdoMergeEmployerDetail.merge_employer_header_id);
        }
        public bool IsEffectDateAndEmploymentSame()
        {
            if(ibusPerson==null)
                LoadPerson();
            if(ibusPerson.icolPersonEmployment==null)
                ibusPerson.LoadPersonEmployment();
            foreach (busPersonEmployment lobjPersonEmployment in ibusPerson.icolPersonEmployment)
            {
                if (lobjPersonEmployment.icdoPersonEmployment.org_id == ibusMergeEmployerHeader.icdoMergeEmployerHeader.from_employer_id)
                {
                    if (lobjPersonEmployment.icdoPersonEmployment.start_date >= ibusMergeEmployerHeader.icdoMergeEmployerHeader.effective_date)
                    {
                        return true;
                    }
                }
            }
            return false;
        }  
	}
}
