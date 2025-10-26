using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;

namespace NeoSpin.BusinessObjects
{
    public class busDefferedCompPositiveOutFile : busFileBaseOut
    {
        private Collection<busProviderReportDataDeffComp> _iclbDeffComProvider;
        public Collection<busProviderReportDataDeffComp> iclbDeffComProvider
        {
            get { return _iclbDeffComProvider; }
            set { _iclbDeffComProvider = value; }
        }

        public DateTime idtNextRunDate { get; set; }
        private decimal _total_contribution;
        public decimal total_contribution
        {
            get
            {
                return _total_contribution;

            }
            set
            {
                _total_contribution = value;
            }
        }
        private int _record_count;
        public int record_count
        {
            get { return _record_count; }
            set { _record_count = value; }
        }
        public string istrRecordCount
        {
            get;
            set;
        }
        public string istrtotalcontribution
        {
            get;
            set;
        }
        private Hashtable lhstPositiveValues = new Hashtable();
        private Hashtable lhstNegativeValues = new Hashtable();

        private void LoadSigns()
        {
            lhstPositiveValues.Add("0", "{");
            lhstPositiveValues.Add("1", "A");
            lhstPositiveValues.Add("2", "B");
            lhstPositiveValues.Add("3", "C");
            lhstPositiveValues.Add("4", "D");
            lhstPositiveValues.Add("5", "E");
            lhstPositiveValues.Add("6", "F");
            lhstPositiveValues.Add("7", "G");
            lhstPositiveValues.Add("8", "H");
            lhstPositiveValues.Add("9", "I");

            lhstNegativeValues.Add("0", "}");
            lhstNegativeValues.Add("1", "J");
            lhstNegativeValues.Add("2", "K");
            lhstNegativeValues.Add("3", "L");
            lhstNegativeValues.Add("4", "M");
            lhstNegativeValues.Add("5", "N");
            lhstNegativeValues.Add("6", "O");
            lhstNegativeValues.Add("7", "P");
            lhstNegativeValues.Add("8", "Q");
            lhstNegativeValues.Add("9", "R");
        }
        public override void InitializeFile()
        {
            idtNextRunDate = (DateTime)iarrParameters[1];
            if (iclbDeffComProvider != null && iclbDeffComProvider.Count > 0 && iclbDeffComProvider[0].icdoProviderReportDataDeffComp.total_contribution > 0)
                istrFileName = "100455-01_FIN." + idtNextRunDate.ToString(busConstant.DateFormatD8) + "_Positive" + busConstant.FileFormattxt;
            else if (iclbDeffComProvider != null && iclbDeffComProvider.Count > 0 && iclbDeffComProvider[0].icdoProviderReportDataDeffComp.total_contribution < 0)
                istrFileName = "100455-01_FIN." + idtNextRunDate.ToString(busConstant.DateFormatD8) + "_Negative" + busConstant.FileFormattxt;
            else
                istrFileName = "100455-01_FIN." + idtNextRunDate.ToString(busConstant.DateFormatD8) + busConstant.FileFormattxt;
        }
        public void LoadTIAACREFContributions(DataTable ldtProviderReportDataDC)
        {
            decimal ldclTemp = 0.0M;
            string lstrKey, lstrValue;
            /// Loads the HashTable Values
            //LoadSigns();
            iclbDeffComProvider = (Collection<busProviderReportDataDeffComp>)iarrParameters[0];
            foreach (busProviderReportDataDeffComp lbusProviderReportDataDeffComp in iclbDeffComProvider)
            {
                lbusProviderReportDataDeffComp.idtNextRunDate = (DateTime)iarrParameters[1];
                total_contribution += lbusProviderReportDataDeffComp.icdoProviderReportDataDeffComp.total_contribution;
            }
            if (total_contribution > 0)
            {
                ldclTemp = total_contribution * (100);
                istrtotalcontribution = ldclTemp.ToString("#");
                istrtotalcontribution = istrtotalcontribution.PadLeft(9, '0');
            }
            else
            {
                ldclTemp = total_contribution * (-100);
                istrtotalcontribution = ldclTemp.ToString("#");
                istrtotalcontribution = istrtotalcontribution.PadLeft(9, '0');
                //lstrKey = istrtotalcontribution.Substring(istrtotalcontribution.Length - 1, 1);
                //istrtotalcontribution = istrtotalcontribution.Substring(0, istrtotalcontribution.Length - 1);
                //lstrValue = Convert.ToString(lhstNegativeValues[lstrKey]);
                //istrtotalcontribution = istrtotalcontribution+lstrValue;
            }
            record_count = iclbDeffComProvider.Count();
            istrRecordCount = record_count.ToString("#");
            istrRecordCount = istrRecordCount.PadLeft(9, '0');
        }

        public string istrFillerForFile
        {
            get
            {
                return " ";
            }
        }
    }
}
