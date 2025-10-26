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

namespace NeoSpinBatch
{
    class busMailingLabelBatch : busNeoSpinBatch
    {
        /// <summary>
        /// Get the Mailing Label Criteria Whose Run Date is Null.
        /// Load the Mailing Addresses with the criteria.
        /// Create Out file for each Search Criteria
        /// </summary>
        public void GetMailingLabels()
        {
            string lstrProcessName = "Mailing Label Client - Generating CSV Outbound";
            idlgUpdateProcessLog("Mailing Label Client - Generating CSV Outbound Process Started", "INFO", lstrProcessName);
            DataTable ldtbMailingLabelClient = busNeoSpinBase.Select("cdoMailingLabel.GetRecordByRunDate", new object[] { });
            foreach (DataRow dr in ldtbMailingLabelClient.Rows)
            {
                int lintMailingLabelBatchID = Convert.ToInt32(dr["mailing_label_batch_id"]);
                idlgUpdateProcessLog("Generating CSV File for Batch ID " + lintMailingLabelBatchID.ToString(), "INFO", lstrProcessName);
                busProcessOutboundFile lobjProcessFiles = new busProcessOutboundFile();
                lobjProcessFiles.iarrParameters = new object[1];
                lobjProcessFiles.iarrParameters[0] = lintMailingLabelBatchID;
                busMailingLabel lobjMailingLabel = new busMailingLabel();
                lobjMailingLabel.FindMailingLabel(lintMailingLabelBatchID);
                if (lobjMailingLabel.lblnIsSearchForPerson)
                    lobjProcessFiles.CreateOutboundFile(busConstant.MailingLabelOutFileIDPerson);
                else
                    lobjProcessFiles.CreateOutboundFile(busConstant.MailingLabelOutFileIDOrg);

                // Update Run date and User ID
                lobjMailingLabel.icdoMailingLabel.run_date = DateTime.Now;
                lobjMailingLabel.icdoMailingLabel.Update();
            }
            idlgUpdateProcessLog("Mailing Label Client - Generating CSV Outbound Process Ended", "INFO", lstrProcessName);
        }
    }
}
