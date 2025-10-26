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
	public partial class busGLTransaction: busExtendBase
    {
		public busGLTransaction()
		{

		} 

		private cdoGlTransaction _icdoGlTransaction;
		public cdoGlTransaction icdoGlTransaction
		{
			get
			{
				return _icdoGlTransaction;
			}

			set
			{
				_icdoGlTransaction = value;
			}
		}

		public bool FindGLTransaction(int Aintgltransactionid)
		{
			bool lblnResult = false;
			if (_icdoGlTransaction == null)
			{
				_icdoGlTransaction = new cdoGlTransaction();
			}
			if (_icdoGlTransaction.SelectRow(new object[1] { Aintgltransactionid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
