#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
    public class cdoReviewRemittanceDistribution 
    {
        public cdoReviewRemittanceDistribution() 
        {
        }

        private int _PayrollHeaderID;
        public int PayrollHeaderID
        {
            get { return _PayrollHeaderID; }
            set { _PayrollHeaderID = value; }
        }

        private string _lstrPayrollHeaderID;
        public string lstrPayrollHeaderID
        {
            get 
            {
                _lstrPayrollHeaderID = string.Empty;
                if (_PayrollHeaderID != 0)
                    _lstrPayrollHeaderID = Convert.ToString(_PayrollHeaderID);
                return _lstrPayrollHeaderID; 
            }
        }

        private int _SeminarID;
        public int SeminarID
        {
            get { return _SeminarID; }
            set { _SeminarID = value; }
        }

        private string _lstrSeminarID;
        public string lstrSeminarID
        {
            get 
            {
                _lstrSeminarID = string.Empty;
                if (_SeminarID != 0)
                    _lstrSeminarID = Convert.ToString(_SeminarID);
                return _lstrSeminarID; 
            }
        }

        private int _AgreementID;
        public int AgreementID
        {
            get { return _AgreementID; }
            set { _AgreementID = value; }
        }

        private string _lstrAgreementID;
        public string lstrAgreementID
        {
            get 
            {
                _lstrAgreementID = string.Empty;
                if (_AgreementID != 0)
                    _lstrAgreementID = Convert.ToString(_AgreementID);
                return _lstrAgreementID; 
            }
        }
        
        private int _PymtReceivableID;
        public int PymtReceivableID
        {
            get { return _PymtReceivableID; }
            set { _PymtReceivableID = value; }
        }

        private string _lstrPymtReceivalbleID;
        public string lstrPymtReceivableID
        {
            get 
            {
                _lstrPymtReceivalbleID = string.Empty;
                if (_PymtReceivableID != 0)
                    _lstrPymtReceivalbleID = Convert.ToString(_PymtReceivableID);
                return _lstrPymtReceivalbleID; 
            }
        }

        private decimal _ReportedAmount;
        public decimal ReportedAmount
        {
            get { return _ReportedAmount; }
            set { _ReportedAmount = value; }
        }

        private decimal _AppliedAmount;
        public decimal AppliedAmount
        {
            get { return _AppliedAmount; }
            set { _AppliedAmount = value; }
        }
   
    }
}
