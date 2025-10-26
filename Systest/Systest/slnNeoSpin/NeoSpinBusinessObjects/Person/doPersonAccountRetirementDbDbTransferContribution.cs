#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion

namespace NeoSpin.DataObjects
{
    [Serializable]
    public class doPersonAccountRetirementDbDbTransferContribution : doBase
    {
         
         public doPersonAccountRetirementDbDbTransferContribution() : base()
         {
         }
		private int _db_db_transfer_contribution_id;
		public int db_db_transfer_contribution_id
		{
			get
			{
				return _db_db_transfer_contribution_id;
			}

			set
			{
				_db_db_transfer_contribution_id = value;
			}
		}

		private int _db_db_transfer_id;
		public int db_db_transfer_id
		{
			get
			{
				return _db_db_transfer_id;
			}

			set
			{
				_db_db_transfer_id = value;
			}
		}

		private int _retirement_contribution_id;
		public int retirement_contribution_id
		{
			get
			{
				return _retirement_contribution_id;
			}

			set
			{
				_retirement_contribution_id = value;
			}
		}

		private string _transfer_flag;
		public string transfer_flag
		{
			get
			{
				return _transfer_flag;
			}

			set
			{
				_transfer_flag = value;
			}
		}

    }
}

