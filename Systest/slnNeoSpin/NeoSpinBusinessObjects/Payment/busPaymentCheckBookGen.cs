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
	public class busPaymentCheckBookGen : busExtendBase
    {
		public busPaymentCheckBookGen()
		{

		}

		public cdoPaymentCheckBook icdoPaymentCheckBook { get; set; }
        public virtual bool FindPaymentCheckBook(int Aintcheckbookid)
		{
			bool lblnResult = false;
			if (icdoPaymentCheckBook == null)
			{
				icdoPaymentCheckBook = new cdoPaymentCheckBook();
			}
            if (icdoPaymentCheckBook.SelectRow(new object[1] { Aintcheckbookid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
