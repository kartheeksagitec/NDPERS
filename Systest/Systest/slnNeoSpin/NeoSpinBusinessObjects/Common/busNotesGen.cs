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
	public class busNotesGen : busExtendBase
    {
		public busNotesGen()
		{

		} 

		private cdoNotes _icdoNotes;
		public cdoNotes icdoNotes
		{
			get
			{
				return _icdoNotes;
			}

			set
			{
				_icdoNotes = value;
			}
		}

		public bool FindNotes(int Aintnoteid)
		{
			bool lblnResult = false;
			if (_icdoNotes == null)
			{
				_icdoNotes = new cdoNotes();
			}
			if (_icdoNotes.SelectRow(new object[1] { Aintnoteid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
	}
}
