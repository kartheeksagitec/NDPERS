using System;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.Common;
using System.Data;
using NeoSpin.CustomDataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busExecuteDeathMatchInboundFile : busFileBase
    {
        public busExecuteDeathMatchInboundFile()
        { }

        //property to store the data read from file
        public busPerson iobjPerson { get; set; }

        //property to store all person details who come in death match criteria
        public DataTable idtPerson { get; set; }

        //property to check whether file is from SSA or SHA
        public bool iblnSSA { get; set; }

        //property to contain SSN of person
        public string istrSSNs { get; set; }        

        public override void InitializeFile()
        {            
            base.InitializeFile();
            //query to get all the employees who meets death match criteria
            idtPerson = busBase.Select("cdoDeathNotification.DeathMatchFile", new object[0] { });            
            iblnSSA = false;
        }

        public override busBase NewDetail()
        {            
            iobjPerson = new busPerson { icdoPerson = new cdoPerson() };
            return iobjPerson;
        }

        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            //To identify whether the file is from SSA or SHA
            if (astrFieldName.Substring(astrFieldName.LastIndexOf(".") + 1) == busConstant.FieldNameDeathIndicator)
                iblnSSA = true;

            return base.BeforeFieldAssigned(astrFieldName, astrFieldValue);
        }

        public override void ProcessDetail()
        {
            utlError lobjError = new utlError();
            ArrayList larrError = new ArrayList();
            bool lblnIsError = false;
            if (!iblnSSA)
                InitiateWorkFlow();
            else
            {
                switch (iobjPerson.icdoPerson.iintVerificationCode)
                {
                    case 1:
                        lobjError = new utlError { istrErrorID = "6201", istrErrorMessage = "SSN not on file" };                        
                        larrError.Add(lobjError);
                        lblnIsError = true;
                        break;
                    case 2:
                        lobjError = new utlError { istrErrorID = "6202", istrErrorMessage = "Name and DOB match; gender code does not match" };
                        larrError.Add(lobjError);
                        lblnIsError = true;
                        break;
                    case 3:
                        lobjError = new utlError { istrErrorID = "6203", istrErrorMessage = "Name and Gender Code match; DOB does not match" };
                        larrError.Add(lobjError);
                        lblnIsError = true;
                        break;
                    case 4:
                        lobjError = new utlError { istrErrorID = "6204", istrErrorMessage = "Name Matches; DOB and Gender Code do not match" };
                        larrError.Add(lobjError);
                        lblnIsError = true;
                        break;
                    case 5:
                        lobjError = new utlError { istrErrorID = "6205", istrErrorMessage = "Name does not match; DOB and gender not checked" };
                        larrError.Add(lobjError);
                        lblnIsError = true;
                        break;
                    case 6:
                        lobjError = new utlError { istrErrorID = "6206", istrErrorMessage = "SSN did not verify, other reason" };
                        larrError.Add(lobjError);
                        lblnIsError = true;
                        break;
                    default:
                        if ((iobjPerson.icdoPerson.iintVerificationCode == 0) &&
                           (iobjPerson.icdoPerson.istrDeathIndicator.ToUpper().Equals("Y")))
                        {
                            InitiateWorkFlow();
                        }
                        break;
                }
                if (lblnIsError)
                {
                    iobjPerson.iarrErrors = larrError;
                    lblnIsError = false;
                }
            }
        }       

        private void InitiateWorkFlow()
        {
            var lenuList = from ldrPerson in idtPerson.AsEnumerable()
                           where ldrPerson.Field<string>("ssn") == iobjPerson.icdoPerson.ssn
                           select ldrPerson;

            if (lenuList.AsDataTable().Rows.Count > 0)
                InitiateWorkFlow(Convert.ToInt32(lenuList.AsDataTable().Rows[0]["person_id"]));                 
        }

        //Function to initialize workflow for the selected person id
        private void InitiateWorkFlow(int aintPersonID)
        {
            busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_Death_Match, aintPersonID,0,0, iobjPassInfo);
        }     
    }
}
