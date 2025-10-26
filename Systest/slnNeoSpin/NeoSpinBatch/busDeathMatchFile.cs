#region Using directives
using System;
using System.Data;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.BusinessObjects;
#endregion

namespace NeoSpinBatch
{
    public class busDeathMatchFile : busNeoSpinBatch
    {
        public busDeathMatchFile()
        { }

        public Collection<busPerson> iclbPerson { get; set; }

        /// <summary>
        /// Function to generate file for SHA
        /// </summary>
        public void GenerateSHAFile()
        {
            try
            {
                istrProcessName = "Death Match File for State Health Administration";
                GenerateDeathMatchFile();
                if (iclbPerson.Count > 0)
                {
                    busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                    lobjProcessFiles.iarrParameters = new object[1];
                    lobjProcessFiles.iarrParameters[0] = iclbPerson;
                    lobjProcessFiles.CreateOutboundFile(63);
                    idlgUpdateProcessLog("Death Match File for State Health Administration generated successfully", "INFO", istrProcessName);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Function to generate file for SSA
        /// </summary>
        public void GenerateSSAFile()
        {
            istrProcessName = "Death Match File for Social Security Administration";
            GenerateDeathMatchFile();
            if (iclbPerson.Count > 0)
            {
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iarrParameters = new object[1];
                lobjProcessFiles.iarrParameters[0] = iclbPerson;
                lobjProcessFiles.CreateOutboundFile(64);
                idlgUpdateProcessLog("Death Match File for Social Security Administration generated successfully", "INFO", istrProcessName);
            }
        }

        /// <summary>
        /// Function which calls the query to generate file for SHA and SSA
        /// </summary>
        public void GenerateDeathMatchFile()
        {
            DataTable ldtPerson = busBase.Select("cdoDeathNotification.DeathMatchFile", new object[0] { });
            iclbPerson = new busBase().GetCollection<busPerson>(ldtPerson, "icdoPerson");
        }
    }
}
