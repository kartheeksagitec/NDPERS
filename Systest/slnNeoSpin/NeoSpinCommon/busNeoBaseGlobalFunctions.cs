#region [Using Directives]
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
#endregion [Using Directives]

namespace NeoBase.Common
{
    public static class busNeoBaseGlobalFunctions
    {

        public static DateTime GetSystemDate()
        {
            return utlPassInfo.iobjPassInfo.ApplicationDateTime;
        }

        public static Hashtable GetFNCredentials()
        {
            Hashtable lhstParameter = new Hashtable();

            String lstrceUsername = string.Empty;
            String lstrcePassword = string.Empty;

            IDbConnection myConnection = Sagitec.DBUtility.DBFunction.GetDBConnection("filenet");

            Hashtable lhstConnDetails = new Hashtable();
            IDataReader myReader = null;

            myConnection.Open();
            IDbCommand lcmdmyCommand = DBFunction.GetDBCommand("Select SETTING_NAME, SETTING_VALUE from  SGS_ECM_APPCONFIG where  SETTING_NAME = 'fn_username' OR SETTING_NAME = 'fn_pwd'", myConnection);
            myReader = lcmdmyCommand.ExecuteReader();


            while (myReader.Read())
            {
                lhstConnDetails.Add(myReader.GetValue(0), myReader.GetValue(1));
            }

            foreach (DictionaryEntry ldicentry in lhstConnDetails)
            {
                if (ldicentry.Key.ToString() == "fn_username")
                {
                    lhstParameter.Add("Username", ldicentry.Value.ToString());
                }
                if (ldicentry.Key.ToString() == "fn_pwd")
                {
                    lhstParameter.Add("Password", ldicentry.Value.ToString());
                }
            }
            myConnection.Close();
            return lhstParameter;
        }

    }
}


