#region [Using directives]
using Sagitec.Common;
using Sagitec.DataObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
#endregion [Using directives]

namespace NeoBase.Common.DataObjects
{
    /// <summary>
    /// Class to write common functions which will be used from cdo class.
    /// </summary>
    [Serializable]
    public class doNeoBase : doBase
    {
        public doNeoBase()
            : base()
        {
        }

        #region "Properties"
        public bool iblnHideLookupColumn { get; set; }
        private string _createdByUserName;
        private string _modifiedByFullName;
        private string _modifiedByUserId;
        private string _createdDate;

        //Property to set restrcited record caption as 'Restricted Record' for all lookup
        public string istrRestrictedRecord { get; set; }

        //Get created by user name
        public string CreatedByUserName
        {
            get
            {
                if (string.IsNullOrEmpty(this._createdByUserName))
                {
                    DataTable dtResult = this.iobjPassInfo.isrvDBCache.GetCacheData("sgs_user", " USER_ID='" + this.created_by + "'");
                    _createdByUserName = this.created_by;
                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        if (dtResult.Rows[0]["FIRST_NAME"] != null)
                        {
                            _createdByUserName = dtResult.Rows[0]["LAST_NAME"].ToString() + ", " + dtResult.Rows[0]["FIRST_NAME"].ToString();
                        }
                        else
                        {
                            _createdByUserName = dtResult.Rows[0]["USER_ID"].ToString();
                        }
                    }
                }
                return _createdByUserName;
            }
            set { _createdByUserName = value; }
        }

        //Get modified by user name
        public string ModifiedByUserName
        {
            get
            {
                if (string.IsNullOrEmpty(this._modifiedByFullName) || (!(_modifiedByUserId.Equals(this.modified_by))))
                {
                    _modifiedByUserId = this.modified_by;
                    DataTable dtResult = this.iobjPassInfo.isrvDBCache.GetCacheData("sgs_user", " USER_ID='" + this.modified_by + "'");
                    if (dtResult != null && dtResult.Rows.Count > 0)
                    {
                        if (dtResult.Rows[0]["FIRST_NAME"] != null)
                        {
                            _modifiedByFullName = dtResult.Rows[0]["LAST_NAME"].ToString() + ", " + dtResult.Rows[0]["FIRST_NAME"].ToString();
                        }
                        else
                        {
                            _modifiedByFullName = dtResult.Rows[0]["USER_ID"].ToString();
                        }
                    }
                    else
                    {
                        _modifiedByFullName = this.modified_by;
                    }
                }
                return _modifiedByFullName;
            }
            set { _modifiedByFullName = value; }
        }

        /// <summary>
        /// Holds the Created date with specified format
        /// </summary>
        public string istrCreatedDate
        {
            get
            {
                if (string.IsNullOrEmpty(this._createdDate))
                {
                    _createdDate = this.created_date.ToString("MM/dd/yyyy hh:mm:ss tt");
                }
                return _createdDate;
            }
        }

        #endregion

        /// <summary>
        /// Return all code values for passed code id & with optional parameters.
        /// </summary>
        /// <param name="aintCodeId"></param>
        /// <returns></returns>
        public List<utlCodeValue> GetCodeValuesByCodeId(int aintCodeId, string astrData1 = null, string astrData2 = null, string astrData3 = null)
        {
            List<utlCodeValue> lstutlCodeValues = iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(aintCodeId, astrData1, astrData2, astrData3);
            return lstutlCodeValues;
        }

        #region [Overridden Methods]
        public override int Insert()
        {
            int lintResult; //Assigning default value.

            //if (this is doRoles)
            //{
            //    doRoles lcdoRoles = this as doRoles;
            //    if (string.IsNullOrEmpty(lcdoRoles.status_value))
            //        lcdoRoles.status_value = "A"; // While adding a new user default status would be active
            //    lintResult = base.Insert();
            //    DBFunction.DBNonQuery("entSecurity.InitializeSecurity", new object[0], iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //    return lintResult;
            //}
            //else if (this.GetType() == typeof(doUser))
            //{
            //    doUser ldoUser = this as doUser;
            //    lintResult = base.Insert();
            //    return lintResult;
            //}


            //if (this is doResources)
            //{
            //    lintResult = base.Insert();
            //    DBFunction.DBNonQuery("entSecurity.InitializeSecurity", new object[0],
            //        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //    return lintResult;
            //}

            //else // Default base Insert.
            //{
                lintResult = base.Insert();
            //}

            return lintResult;

        }

        /// <summary>
        /// Update Password
        /// </summary>
        /// <returns></returns>

        public override int Update()
        {
            int lintResult;
            //if (this is doUser)
            //{
            //    doUser ldoUser = this as doUser;
            //    lintResult = base.Update();
            //    return lintResult;
            //}
            //else
            {
                lintResult = base.Update();
            }

            return lintResult;
        }

        /// <summary>
        /// After deleting person communication records of communication preference Postal Mail need to update person address flag.
        /// </summary>
        /// <returns></returns>
        public override int Delete()
        {
            var liintRowAffected = base.Delete();
            //if (this is doPersonCommPref)
            //{
            //    var lcdoPersonCommunicationPreference = this as doPersonCommPref;
            //    busPersonAddress lbusPersonAddress = new busPersonAddress { icdoPersonAddress = new doPersonAddress() };

            //    if (lcdoPersonCommunicationPreference.communication_preference_value.Equals(busNeoBaseConstants .Communication.DELIVERY_METHOD_POSTAL_MAIL)
            //        && lbusPersonAddress.icdoPersonAddress.FindByPrimaryKey(lcdoPersonCommunicationPreference.person_communication_ref_id))
            //    {
            //        if (Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonAddress.GetCountAssociatedCommunicationPreferenceForPerson",
            //            new object[1] { lcdoPersonCommunicationPreference.person_communication_ref_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) == 0)
            //        {
            //            lbusPersonAddress.icdoPersonAddress.used_in_communication_flag = busNeoBaseConstants .FLAG_NO;
            //            lbusPersonAddress.icdoPersonAddress.Update();
            //        }
            //    }
            //}
            return liintRowAffected;
        }

        /// <summary>
        /// Validate on Delete
        /// </summary>
        /// <returns>Error List</returns>
        public override ArrayList ValidateDelete()
        {
            ArrayList larrErrors = null;
            //if (this is doCode)
            //{
            //    int lintCount = (int)DBFunction.DBExecuteScalar("select count(*) from sgs_code_value", new Collection<IDbDataParameter>(),
            //        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            //    if (lintCount > 0)
            //    {
            //        //Cannot delete code, please delete code values associated with it 
            //        AddError(larrErrors, 50);
            //    }
            //}
            return larrErrors;
        }

        #endregion [Overridden Methods]
    }
}
