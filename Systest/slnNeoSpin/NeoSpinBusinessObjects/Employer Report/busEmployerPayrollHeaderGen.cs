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
	public partial class busEmployerPayrollHeader : busExtendBase
    {
		public busEmployerPayrollHeader()
		{

		}
        #region ESS Properties
        //Prop used inn Ess 
        public bool iblnIsFromESS { get; set; }
        public int iintContactid { get; set; }
        
        #endregion
        private cdoEmployerPayrollHeader _icdoEmployerPayrollHeader;
		public cdoEmployerPayrollHeader icdoEmployerPayrollHeader
		{
			get
			{
				return _icdoEmployerPayrollHeader;
			}

			set
			{
				_icdoEmployerPayrollHeader = value;
			}
		}

		public bool FindEmployerPayrollHeader(int Aintemployerpayrollheaderid)
		{
			bool lblnResult = false;
			if (_icdoEmployerPayrollHeader == null)
			{
				_icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
			}
			if (_icdoEmployerPayrollHeader.SelectRow(new object[1] { Aintemployerpayrollheaderid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}