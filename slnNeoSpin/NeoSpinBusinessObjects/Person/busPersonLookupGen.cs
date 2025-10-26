#region Using directives

using System;
using System.Data;
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
	public partial class busPersonLookup  : busMainBase
	{

		private Collection<busPerson> _icolPerson;
		public Collection<busPerson> icolPerson
		{
			get
			{
				return _icolPerson;
			}

			set
			{
				_icolPerson = value;
			}
		}

		public void LoadPerson(DataTable adtbSearchResult)
		{
			_icolPerson = GetCollection<busPerson>(adtbSearchResult, "icdoPerson");
		}

        public void LoadPersonFromESS(DataTable adtbSearchResult)
        {
            _icolPerson = GetCollection<busPerson>(adtbSearchResult, "icdoPerson");
            foreach (busPerson lobjPerson in icolPerson)
            {
                lobjPerson.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
                DataRow[] larrPerson = adtbSearchResult.FilterTable(busConstant.DataType.Numeric, "person_id", lobjPerson.icdoPerson.person_id);
                if (larrPerson.Length > 0)
                {
                    lobjPerson.ibusCurrentEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                    lobjPerson.ibusCurrentEmployment.icdoPersonEmployment.LoadData(larrPerson[0]);
                    lobjPerson.ibusCurrentEmployment.LoadLatestPersonEmploymentDetail();
                    lobjPerson.iintTermEmploymentChangeRequestID = lobjPerson.icdoPerson.iintTermEmpID; //F/W Upgrade PIR 11095 - Loading term request ID in lookup results grid
                }
            }
        }
	}
}
