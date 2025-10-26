using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.IO;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.DBUtility;
using System.Collections;

namespace NeoSpinBatch
{
    class busPersonAccountEAPEnrollmentBatch : busNeoSpinBatch
    {
        private busProcessOutboundFile iobjProcessFiles;

        public busPersonAccountEAPEnrollmentBatch()
        {

        }
        public void GenerateFiles(busProcessOutboundFile aobjProcessFiles)
        {
            iobjProcessFiles = aobjProcessFiles;
            GenerateEAPFileOut();
        }

        public void GenerateEAPFileOut()
        {                        
            // Load all Providers
            DataTable ldtbEAPProviders = busBase.Select("cdoPersonAccount.LoadEAPProvidersForEAPFileOut", new object[] { });
            foreach (DataRow dr in ldtbEAPProviders.Rows)
            {
                busProcessOutboundFile lbusProcessFiles = new busProcessOutboundFile();
                lbusProcessFiles.iarrParameters = new object[1];
                lbusProcessFiles.iobjSystemManagement = iobjProcessFiles.iobjSystemManagement;
                lbusProcessFiles.idlgUpdateProcessLog = iobjProcessFiles.idlgUpdateProcessLog;

                if (dr["org_id"] != DBNull.Value)
                {
                    lbusProcessFiles.iarrParameters[0] = Convert.ToInt32(dr["org_id"]);
                    lbusProcessFiles.CreateOutboundFile(47);
                }
            }
        }
    }
}