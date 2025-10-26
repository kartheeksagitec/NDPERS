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
using System.Linq;
using System.Linq.Expressions;

#endregion
namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busActuaryFileHeader:
    /// Inherited from busActuaryFileHeaderGen, the class is used to customize the business object busActuaryFileHeaderGen.
    /// </summary>
    [Serializable]
    public class busActuaryFileHeader : busActuaryFileHeaderGen
    {
        public override void BeforePersistChanges()
        {
            //if file type selected as pension file ,Pension file type bydefault it will be Adhoc
            icdoActuaryFileHeader.status_value = busConstant.AdhocActuaryFileStatusPending;

            icdoActuaryFileHeader.pension_file_type_value = busConstant.PensionFileTypeAdhoc;
        }

        public bool IsActuaryFileHeaderExists()
        {
            bool lblnExists = false;
            if (icdoActuaryFileHeader.file_type_value != null)
            {
                DataTable ldtActuaryFileHeader = Select<cdoActuaryFileHeader>
                                                (new string[1] { "file_type_value" },
                                                new object[1] { icdoActuaryFileHeader.file_type_value }, null, null);
                Collection<busActuaryFileHeader> lclbActuarialFileHeader = GetCollection<busActuaryFileHeader>(ldtActuaryFileHeader, "icdoActuaryFileHeader");

                foreach (busActuaryFileHeader lobjHeader in lclbActuarialFileHeader)
                {
                    if (icdoActuaryFileHeader.pension_file_type_value == busConstant.PensionFileTypeAnnual &&
                        lobjHeader.icdoActuaryFileHeader.pension_file_type_value == icdoActuaryFileHeader.pension_file_type_value &&
                        lobjHeader.icdoActuaryFileHeader.effective_date.Year == icdoActuaryFileHeader.effective_date.Year &&
                         lobjHeader.icdoActuaryFileHeader.actuary_file_header_id != icdoActuaryFileHeader.actuary_file_header_id)
                    {
                        lblnExists = true;
                        break;
                    }
                    else if (lobjHeader.icdoActuaryFileHeader.pension_file_type_value == icdoActuaryFileHeader.pension_file_type_value &&
                        lobjHeader.icdoActuaryFileHeader.effective_date == icdoActuaryFileHeader.effective_date &&
                        lobjHeader.icdoActuaryFileHeader.plan_id == icdoActuaryFileHeader.plan_id &&
                        lobjHeader.icdoActuaryFileHeader.actuary_file_header_id != icdoActuaryFileHeader.actuary_file_header_id)
                    {
                        lblnExists = true;
                        break;
                    }
                }
            }
            return lblnExists;
        }

        public busPlan ibusPlan { get; set; }
        public void LoadPlan()
        {
            if (ibusPlan == null)
                ibusPlan = new busPlan();
            ibusPlan.FindPlan(icdoActuaryFileHeader.plan_id);
        }
    }
}