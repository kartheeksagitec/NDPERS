#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;

#endregion

namespace NeoSpin.BusinessObjects
{
	[Serializable]
	public class busMessages : busExtendBase
    {
		public busMessages()
		{
		}

		private cdoMessages _icdoMessages;
		public cdoMessages icdoMessages
		{
			get
			{
				return _icdoMessages;
			}

			set
			{
				_icdoMessages = value;
			}
		}

		public bool FindMessage(int aintMessageId)
		{
			bool lblnResult = false;
			if (_icdoMessages == null)
			{
				_icdoMessages = new cdoMessages();
			}
			if (_icdoMessages.SelectRow(new object[1] { aintMessageId }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
