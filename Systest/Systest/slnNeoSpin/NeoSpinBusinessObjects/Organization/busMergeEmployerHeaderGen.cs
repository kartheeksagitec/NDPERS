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
	public class busMergeEmployerHeaderGen : busExtendBase
    {
		public busMergeEmployerHeaderGen()
		{

		}

		private cdoMergeEmployerHeader _icdoMergeEmployerHeader;
		public cdoMergeEmployerHeader icdoMergeEmployerHeader
		{
			get
			{
				return _icdoMergeEmployerHeader;
			}
			set
			{
				_icdoMergeEmployerHeader = value;
			}
		}

		public bool FindMergeEmployerHeader(int Aintmergeemployerheaderid)
		{
			bool lblnResult = false;
			if (_icdoMergeEmployerHeader == null)
			{
				_icdoMergeEmployerHeader = new cdoMergeEmployerHeader();
			}
			if (_icdoMergeEmployerHeader.SelectRow(new object[1] { Aintmergeemployerheaderid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        private busOrganization _ibusFromEmployer;
        public busOrganization ibusFromEmployer
        {
            get { return _ibusFromEmployer; }
            set { _ibusFromEmployer = value; }
        }
        private busOrganization _ibusToEmployer;
        public busOrganization ibusToEmployer
        {
            get { return _ibusToEmployer; }
            set { _ibusToEmployer = value; }
        }
        public void LoadFromEmployer()
        {
            if (_ibusFromEmployer == null)
            {
                _ibusFromEmployer = new busOrganization();
            }
            _ibusFromEmployer.FindOrganization(icdoMergeEmployerHeader.from_employer_id);
        }
        public void LoadToEmployer()
        {
            if (_ibusToEmployer == null)
            {
                _ibusToEmployer = new busOrganization();
            }
            _ibusToEmployer.FindOrganization(icdoMergeEmployerHeader.to_employer_id);
        }
	}
}
