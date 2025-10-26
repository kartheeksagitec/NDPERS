using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using Sagitec.Interface;
using Sagitec.MetaDataCache;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace NeoSpin.BusinessObjects
{
    public class utlSolObjectStore : IObjectSessionStore
    {
        private utlObjectStore _iobjutlObjectStore;
        public utlSolObjectStore()
        {
            _iobjutlObjectStore = new utlObjectStore();
        }
        public bool CallClearSessionStore()
        {
            return _iobjutlObjectStore.CallClearSessionStore();
        }

        public int CheckObjectInSQL(IDbConnection aconCore, string astrKey, out int aintUpdateSeq)
        {
            return _iobjutlObjectStore.CheckObjectInSQL(aconCore, astrKey,out aintUpdateSeq);
        }

        public void ClearSessionKey(string astrKey)
        {
            _iobjutlObjectStore.ClearSessionKey(astrKey);
        }

        public void ClearSessionStore(string astrKey)
        {
            _iobjutlObjectStore.ClearSessionStore(astrKey);
        }

        public object GetGridHashUsingSenderKey(long aintPrimaryKey)
        {
            return _iobjutlObjectStore.GetGridHashUsingSenderKey(aintPrimaryKey);
        }

        public object GetObjectFromDB(string astrFormName, int aintPrimaryKey)
        {
            return _iobjutlObjectStore.GetObjectFromDB(astrFormName, aintPrimaryKey);
        }

        public object GetObjectFromDB(string astrKey = null, bool ablnRaiseException = true, object aobjPassInfo = null)
        {
            return _iobjutlObjectStore.GetObjectFromDB(astrKey, ablnRaiseException, aobjPassInfo);
        }

        public object GetObjectUsingSenderKey()
        {
            return _iobjutlObjectStore.GetObjectUsingSenderKey();
        }

        public object GetSessionFromDB(string astrKey = null)
        {
            return _iobjutlObjectStore.GetSessionFromDB(astrKey);
        }

        public Dictionary<string, object> GetSessionStoreForCheckSecurityBySenderKey(out int aintSessionStoreUpdateSeq)
        {
            return _iobjutlObjectStore.GetSessionStoreForCheckSecurityBySenderKey(out aintSessionStoreUpdateSeq);
        }

        public int StoreObjectInDB(object aobjToStore, string astrKey = null, bool ablnWait = false)
        {
            bool lblnFormOpen = false;
            int lintUpdateSeq = 0;
            if (!utlPassInfo.iobjPassInfo.idictParams.ContainsKey("srvMethodName"))
            {
                lblnFormOpen = "MVVMGetInitialData".Equals(Convert.ToString(utlPassInfo.iobjPassInfo.idictParams["srvMethodName"]));
            }
            int lintSessionStoreKey = -1;
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            NameValueCollection lobjAdditionalInfo = new NameValueCollection();

            //Commented for Export To Text button is not visisble
            //if (lobjPassInfo.istrFormName == "wfmEmployerPayrollHeaderMaintenance" || lobjPassInfo.istrFormName == "wfmEmployerPayrollHeaderMaintenance" || lobjPassInfo.istrFormName == "wfmESSEmployerPayrollHeaderMaintenance")
            //{
            //    ablnWait = true;
            //}
            if (lobjPassInfo.idictParams.ContainsKey(utlConstants.istrKnowtionFormId) && SystemSettings.Instance.FormsObjectToBeCached.Contains(lobjPassInfo.istrFormName))
            {
                CustomCache.Instance.AddToCache($"{utlPassInfo.iobjPassInfo.istrCultureLanguage}_{lobjPassInfo.istrFormName}", aobjToStore, new TimeSpan(1, 0, 0, 0), MyCachePriority.SlidingExpiration);
                return lintSessionStoreKey;
            }

            long lintPrimaryKey = 0;
            if (lobjPassInfo.idictParams.ContainsKey(utlConstants.istrPrimaryKey))
            {
                lintPrimaryKey = Convert.ToInt64(lobjPassInfo.idictParams[utlConstants.istrPrimaryKey]);
            }

            if (lintPrimaryKey == 0 && aobjToStore is BaseInfo)
            {
                lintPrimaryKey = ((BaseInfo)aobjToStore).iintPrimaryKey;
            }

            string lstrSessionID = lobjPassInfo.idictParams[utlConstants.istrSessionID].ToString();
            string lstrWindowName = lobjPassInfo.idictParams[utlConstants.istrWindowName].ToString();
            string lstrFormName = lobjPassInfo.istrFormName;
            string lstrSessionUserKey = null;

            if (!string.IsNullOrEmpty(astrKey))
            {
                lstrSessionUserKey = astrKey;
            }
            else
            {
                lstrSessionUserKey = lstrSessionID.Substring(0, 4) + "_" + lstrWindowName + "_" + lstrFormName + "_" + lintPrimaryKey.ToString();
            }

            byte[] larrbyteRulesResult = null;
            int lintActivityInstanceId = 0;
            byte[] larrGridHash = null;
            lobjAdditionalInfo["ObjectType"] = aobjToStore.GetType().Name;
            BaseInfo lobjBaseInfo = aobjToStore as BaseInfo;
            if (lobjBaseInfo != null)
            {
                if (lobjBaseInfo.idictGridHash != null)
                    larrGridHash = HelperFunction.SerializeObject(lobjBaseInfo.idictGridHash);
                if (!lobjPassInfo.idictParams.ContainsKey(utlConstants.istrDefaultKey) || Convert.ToBoolean(lobjPassInfo.idictParams[utlConstants.istrDefaultKey]) == false)
                    lintPrimaryKey = lobjBaseInfo.iintPrimaryKey;
           
                Dictionary<string, object> ldictInfo = new Dictionary<string, object>();
                if (lobjBaseInfo.ilstNonEditableControls != null)
                {
                    ldictInfo["NonEditableControls"] = HelperFunction.SerializeObject(lobjBaseInfo.ilstNonEditableControls);
                }

                ldictInfo["Captcha"] = lobjBaseInfo.CaptchaTextToVerify;

                if (ldictInfo.Keys.Count > 0)
                    larrbyteRulesResult = HelperFunction.SerializeObject(ldictInfo);
       
                if (lobjBaseInfo is busBase)
                {
                    if (lobjBaseInfo.ibusBaseActivityInstance != null && lobjBaseInfo.ibusBaseActivityInstance.iblnIsBpmActivityInstance)
                    {
                        lintActivityInstanceId = (lobjBaseInfo.iobjMain).iintActivityInstanceId;
                    }
                    if (lobjBaseInfo.iobjMain.idctGridDataTables != null)
                    {
                        lobjBaseInfo.iobjMain.idctGridDataTables = null;
                    }
                }
            }

            IDbDataParameter lobjDbKey;
            int lintCounter = 0;
            lobjAdditionalInfo["SessionUserKey"] = lstrSessionUserKey;
            GetSessionStoreId(out lintSessionStoreKey, lobjPassInfo, lintPrimaryKey, lstrSessionID, lstrWindowName, lstrFormName, lstrSessionUserKey, larrbyteRulesResult, lintActivityInstanceId, larrGridHash, out lobjDbKey, ref lintCounter, lintUpdateSeq, lblnFormOpen);
            if (lintSessionStoreKey != -1)
            {
                lobjDbKey.Value = lintSessionStoreKey;
                lobjDbKey.Direction = ParameterDirection.Input;
                utlThreadStatic lutlThreadStatic = utlThreadStatic.Instance;
                Dictionary<string, object> ldictParams = utlPassInfo.iobjPassInfo.idictParams;
                lintUpdateSeq++;
                Task ltskSessionStore = Task.Run(() =>
                {
                    IDbConnection lconConnection = null;
                    Dictionary<string, object> ldictCallParams = ldictParams.Clone();
                    ExceptionManager.idictParams = ldictCallParams;
                    NameValueCollection lobjAdditionalInformation = lobjAdditionalInfo;

                    try
                    {
                        utlThreadStatic.Instance.Copy(lutlThreadStatic);
                        LocalServiceHelper<IMetaDataCache>.Initialize(srvMetaDataCache.isrvMetaDataCache);
                        utlThreadStatic.Instance.istrSubTransactionID = "SeperateThread";
                        Collection<IDbDataParameter> ldbParams = new Collection<IDbDataParameter>();
                        byte[] larrObject = HelperFunction.SerializeObject(aobjToStore);

                        ldbParams.Add(DBFunction.GetDBParameter("ACTIVITY_INSTANCE_ID", DbType.Int32, lintActivityInstanceId != 0 ? lintActivityInstanceId as object : DBNull.Value));
                        ldbParams.Add(DBFunction.GetDBParameter("RULE_RESULT", DbType.Binary, larrbyteRulesResult as object ?? DBNull.Value, -1));
                        ldbParams.Add(DBFunction.GetDBParameter("GRID_HASH", DbType.Binary, larrGridHash as object ?? DBNull.Value, -1));
                        ldbParams.Add(DBFunction.GetDBParameter("DATE_CREATED", DbType.DateTime, lobjPassInfo.ApplicationDateTime));
                        ldbParams.Add(DBFunction.GetDBParameter("BUS_OBJECT", DbType.Binary, larrObject, -1));
                        ldbParams.Add(DBFunction.GetDBParameter("SESSION_STORE_SERIAL_ID", DbType.Int32, lintSessionStoreKey));
                        ldbParams.Add(DBFunction.GetDBParameter("UPDATE_SEQ", DbType.Int32, lintUpdateSeq));
                        lconConnection = DBFunction.GetDBConnection("SessionData");
                        DBFunction.DBNonQuery("entFramework.UpdateSessionStore", ldbParams, lconConnection, null);
                    }
                    catch (Exception e)
                    {
                        ExceptionManager.Publish(e, lobjAdditionalInformation);
                    }
                    finally
                    {
                        HelperFunction.DisposeConnection(lconConnection);
                    }
                });
                if (ablnWait)
                {
                    ltskSessionStore.Wait();
                }
            }

            if (lobjBaseInfo != null)
            {
                lobjBaseInfo.iintSessionStoreKey = lintSessionStoreKey;
                lobjBaseInfo.iintSessionStoreUpdateSeq = lintUpdateSeq;
            }
            return lintSessionStoreKey;
        }

        private void GetSessionStoreId(out int lintSessionStoreKey, utlPassInfo lobjPassInfo, long lintPrimaryKey, string lstrSessionID, string lstrWindowName, string lstrFormName, string lstrSessionUserKey, byte[] larrbyteRulesResult, int lintActivityInstanceId, byte[] larrGridHash, out IDbDataParameter lobjDbKey, ref int aintCounter,int lintUpdateSeq,bool lblnFormOpen)
        {
            lintSessionStoreKey = -1;
            string lstrInsertQry = "entFramework.InsertSessionStore";//query in xml file has two parts for oracle and sql
            IDbConnection lconCore = DBFunction.GetDBConnection("SessionData");
            lobjDbKey = null;
            Collection<IDbDataParameter> lcolParameters = new Collection<IDbDataParameter>();
            try
            {
                if (DBFunction.IsOracleConnection())
                {
                    if (!SystemSettings.Instance.SupportedOracleIdentity)
                        lstrInsertQry = "entFramework.InsertSessionStoreWithoutIdentity";
                }

               // lobjAdditionalInfo["SessionUserKey"] = lstrSessionUserKey;
                lintSessionStoreKey = CheckObjectInSQL(lconCore, lstrSessionUserKey, out lintUpdateSeq);
                if (lblnFormOpen)
                {
                    lintUpdateSeq = 0;
                }
                lcolParameters.Add(DBFunction.GetDBParameter("GRID_HASH", DbType.Binary, larrGridHash as object ?? DBNull.Value, -1));
                lcolParameters.Add(DBFunction.GetDBParameter("DATE_CREATED", DbType.DateTime, lobjPassInfo.ApplicationDateTime));
                lcolParameters.Add(DBFunction.GetDBParameter("ACTIVITY_INSTANCE_ID", DbType.Int32, lintActivityInstanceId != 0 ? lintActivityInstanceId as object : DBNull.Value));
                lcolParameters.Add(DBFunction.GetDBParameter("RULE_RESULT", DbType.Binary, larrbyteRulesResult as object ?? DBNull.Value, -1));
                lobjDbKey = DBFunction.GetDBParameter("");
                lobjDbKey.ParameterName = "@SESSION_STORE_SERIAL_ID";
                lobjDbKey.DbType = DbType.Int32;

                if (lintSessionStoreKey == -1) // Insert
                {
                    lcolParameters.Add(lobjDbKey);
                    if (DBFunction.IsOracleConnection() && !SystemSettings.Instance.SupportedOracleIdentity)
                    {
                        lobjDbKey.Value = DBFunction.GetSequence("seq_session_store_serial_id");
                    }
                    else
                    {
                        lobjDbKey.Direction = ParameterDirection.Output;
                    }
                    lcolParameters.Add(DBFunction.GetDBParameter("SESSION_USER_KEY", DbType.String, lstrSessionUserKey, 200));
                    lcolParameters.Add(DBFunction.GetDBParameter("SESSION_ID", DbType.String, lstrSessionID, 50));
                    lcolParameters.Add(DBFunction.GetDBParameter("WINDOW_NAME", DbType.String, lstrWindowName, 50));
                    lcolParameters.Add(DBFunction.GetDBParameter("FORM_ID", DbType.String, lstrFormName, 100));
                    lcolParameters.Add(DBFunction.GetDBParameter("PRIMARY_KEY", DbType.Int32, lintPrimaryKey));
                    lcolParameters.Add(DBFunction.GetDBParameter("UPDATE_SEQ", DbType.Int32, lintUpdateSeq));
                    DBFunction.DBNonQuery(lstrInsertQry, lcolParameters, lconCore, null);
                    lintSessionStoreKey = Convert.ToInt32(lobjDbKey.Value);
                }
                else if (lblnFormOpen)
                {
                    lcolParameters.Add(DBFunction.GetDBParameter("BUS_OBJECT", DbType.Binary, DBNull.Value, -1));
                    lcolParameters.Add(DBFunction.GetDBParameter("SESSION_STORE_SERIAL_ID", DbType.Int32, lintSessionStoreKey));
                    lcolParameters.Add(DBFunction.GetDBParameter("UPDATE_SEQ", DbType.Int32, lintUpdateSeq));
                    DBFunction.DBNonQuery("entFramework.UpdateSessionStore", lcolParameters, lconCore, null);
                }
            }
            catch (Exception ex)
            {
                if (ex is SqlException || (ex.InnerException.IsNotNull() && ex.InnerException is SqlException))
                {
                    SqlException lsqlException = ex is SqlException ? (SqlException)ex : null;
                    SqlException lsqlinnerException = ex.InnerException is SqlException ? (SqlException)ex.InnerException : null;
                    if ((lsqlException.IsNotNull() && (lsqlException.Number == 2601 || lsqlException.Number == 2627)) ||
                        (lsqlinnerException.IsNotNull() && (lsqlinnerException.Number == 2601 || lsqlinnerException.Number == 2627)))
                    {

                        aintCounter++;
                        if (aintCounter < 3)
                            GetSessionStoreId(out lintSessionStoreKey, lobjPassInfo, lintPrimaryKey, lstrSessionID, lstrWindowName, lstrFormName, lstrSessionUserKey, larrbyteRulesResult, lintActivityInstanceId, larrGridHash, out lobjDbKey, ref aintCounter,lintUpdateSeq,lblnFormOpen);
                        else
                            throw;
                    }
                    else
                        throw;
                }
                else
                    throw;
            }
            finally
            {
                HelperFunction.DisposeConnection(lconCore);
            }
        }

        public void StoreSessionInDB(object aobjToStore, string astrKey = null)
        {
            _iobjutlObjectStore.StoreSessionInDB(aobjToStore, astrKey);
        }

        public void UpdateCaptchaText(string astrCaptchaText)
        {
            _iobjutlObjectStore.StoreObjectInDB(astrCaptchaText);
        }
    }
}
