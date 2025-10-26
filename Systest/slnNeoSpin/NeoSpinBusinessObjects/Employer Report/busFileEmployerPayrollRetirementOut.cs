#region Using directives

using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFileEmployerPayrollRetirementOut : busFileBaseOut
    {
        private busEmployerPayrollHeader _ibusEmployerPayrollHeader;
        public busEmployerPayrollHeader ibusEmployerPayrollHeader
        {
            get { return _ibusEmployerPayrollHeader; }
            set { _ibusEmployerPayrollHeader = value; }
        }
        public void LoadEmployerPayroll(DataTable ldtbEmployerPayroll)
        {
            if (iarrParameters[0] != null)
            {
            }
        }

    }
}
