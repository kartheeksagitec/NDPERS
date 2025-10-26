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
    public class cdoBenefitRefundCalculation : doBenefitRefundCalculation
    {
        public cdoBenefitRefundCalculation()
            : base()
        {
        }

        private decimal _total_refund_amount;

        public decimal total_refund_amount
        {
            get { return _total_refund_amount; }
            set { _total_refund_amount = value; }
        }

        private decimal _total_transfer_amount;
        public decimal total_transfer_amount
        {
            get { return _total_transfer_amount; }
            set { _total_transfer_amount = value; }
        }
        //This property value has to be set whereever force insert() or force update() method are being called
        private string _benefit_option_value_to_compare;
        public string benefit_option_value_to_compare
        {
            get { return _benefit_option_value_to_compare; }
            set { _benefit_option_value_to_compare = value; }
        }
        //This property added to post the person into Auditlog for sgt_refund_benefit_calculation
        private int _person_id;

        public int person_id
        {
            get { return _person_id; }
            set { _person_id = value; }
        }


        //This is to manually insert entries into audit log for manual update or insert of specific fields
        public override bool AuditColumn(string astrOperation, string astrColumnName)
        {
            if ((_benefit_option_value_to_compare == BusinessObjects.busConstant.BenefitOptionDBToDCTransferSpecialElection) ||
                (_benefit_option_value_to_compare == BusinessObjects.busConstant.BenefitOptionDBToDCTransfer) || (_benefit_option_value_to_compare == BusinessObjects.busConstant.BenefitOptionDBToTIAACREFTransfer))
            {
                if ((astrColumnName == "er_pre_tax_amount") || (astrColumnName == "er_interest_amount")
                    || (astrColumnName == "additional_er_amount_interest")
                    || (astrColumnName == "additional_er_amount") || (astrColumnName == "additional_ee_amount")
                    || (astrColumnName == "overridden_er_interest_amount") || (astrColumnName == "overridden_er_pre_tax_amount"))
                {
                    return true;
                }
            }
            else if ((_benefit_option_value_to_compare == BusinessObjects.busConstant.BenefitOptionDBToTFFRTransferForDPICTE) ||
                (_benefit_option_value_to_compare == BusinessObjects.busConstant.BenefitOptionDBToTFFRTransferForDualMembers))
            {
                return true;
            }
            return false;
        }

        #region Correspondence Properties PAY-4055

        public decimal idecTaxableMC { get; set; }
        public decimal idecNonTaxableMC { get; set; }
        public decimal idecTaxableMI { get; set; }
        public decimal idecNonTaxableMI { get; set; }
        public decimal idecTaxableCG { get; set; }
        public decimal idecNonTaxableCG { get; set; }
        public decimal idecTaxableEC { get; set; }
        public decimal idecNonTaxableEC { get; set; }
        public decimal idecTaxableEI { get; set; }
        public decimal idecNonTaxableEI { get; set; }
        public decimal idecCheckAmount { get; set; }
        public string istrOrgName { get; set; }
        public decimal idecTotalMC
        {
            get
            {
                return idecTaxableMC + idecNonTaxableMC;
            }
        }
        public decimal idecTotalMI
        {
            get
            {
                return idecTaxableMI + idecNonTaxableMI;
            }
        }
        public decimal idecTotalCG
        {
            get
            {
                return idecTaxableCG + idecNonTaxableCG;
            }
        }
        public decimal idecTotalEC
        {
            get
            {
                return idecTaxableEC + idecNonTaxableEC;
            }
        }
        public decimal idecTotalEI
        {
            get
            {
                return idecTaxableEI + idecNonTaxableEI;
            }
        }
        public decimal idecTotalTaxable
        {
            get
            {
                return idecTaxableCG + idecTaxableEC + idecTaxableEI + idecTaxableMC + idecTaxableMI;
            }
        }
        public decimal idecTotalNonTaxable
        {
            get
            {
                return idecNonTaxableMC + idecNonTaxableMI + idecNonTaxableCG + idecNonTaxableEC + idecNonTaxableEI;
            }
        }
        public decimal idecGrandTotal
        {
            get
            {
                return idecTotalTaxable + idecTotalNonTaxable;
            }
        }
        #endregion
    }
}