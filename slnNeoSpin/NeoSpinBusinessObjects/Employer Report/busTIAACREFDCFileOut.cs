using System;
using System.Collections.Generic;
using System.Text;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CorBuilder;
using Sagitec.DataObjects;
using System.Data;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busTIAACREFDCFileOut : busFileBaseOut
    {
        //public busTIAACREFDCFileOut(){}
        public busTIAACREFDCFileOut()
        {
        }

        private Collection<busProviderReportDataDC> _iclbDCProvider;
        public Collection<busProviderReportDataDC> iclbDCProvider
        {
            get { return _iclbDCProvider; }
            set { _iclbDCProvider = value; }
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
                 _total_contribution=value;
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

              if (iclbDCProvider != null && iclbDCProvider.Count > 0 && iclbDCProvider[0].ldclTotalContributionAmount > 0)
                  istrFileName = "405545" + idtNextRunDate.ToString(busConstant.DateFormatD8) + "_Positive" + busConstant.FileFormattxt;
              else if (iclbDCProvider != null && iclbDCProvider.Count > 0 && iclbDCProvider[0].ldclTotalContributionAmount < 0)
                  istrFileName = "405545" + idtNextRunDate.ToString(busConstant.DateFormatD8) + "_Negative" + busConstant.FileFormattxt;
              else
                  istrFileName = "405545" + idtNextRunDate.ToString(busConstant.DateFormatD8) + busConstant.FileFormattxt;                     
          }
        public void LoadTIAACREFContributions(DataTable ldtProviderReportDataDC)
        {
            iclbDCProvider = (Collection<busProviderReportDataDC>)iarrParameters[0];

           decimal ldclTemp = 0.0M;
            string lstrKey, lstrValue;
            /// Loads the HashTable Values
            LoadSigns();
            foreach (busProviderReportDataDC lbusProviderReportDataDC in iclbDCProvider)
            {
                lbusProviderReportDataDC.idtNextRunDate = (DateTime)iarrParameters[1];
                total_contribution += lbusProviderReportDataDC.ldclTotalContributionAmount;
            }
            if (total_contribution > 0)
            {
                ldclTemp = total_contribution * (100);
                istrtotalcontribution = ldclTemp.ToString("#");
                istrtotalcontribution = istrtotalcontribution.PadLeft(13, '0');
            }
            else
            {
                ldclTemp = total_contribution * (-100);
                istrtotalcontribution = ldclTemp.ToString("#");
                istrtotalcontribution = istrtotalcontribution.PadLeft(13, '0');
                lstrKey = istrtotalcontribution.Substring(istrtotalcontribution.Length - 1, 1);
                istrtotalcontribution = istrtotalcontribution.Substring(0, istrtotalcontribution.Length - 1);
                lstrValue = Convert.ToString(lhstNegativeValues[lstrKey]);
                istrtotalcontribution = istrtotalcontribution + lstrValue;
            }
            record_count = iclbDCProvider.Count();
            istrRecordCount = record_count.ToString("#");
            istrRecordCount = istrRecordCount.PadLeft(9, '0');
        }
        public override void BeforeWriteRecord()
        {
            base.BeforeWriteRecord();
        }
        public override void BeforeDetail()
        {
            base.BeforeDetail();
        }
        public override void AfterWriteRecord()
        {
            base.AfterWriteRecord();
        }
    }
  }
