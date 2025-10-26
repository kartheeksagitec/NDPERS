// -----------------------------------------------------------------------
// <copyright file="busPersonAccountPeoplesoftInterface.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NeoSpinBatch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NeoSpin.CustomDataObjects;
    using NeoSpin.DataObjects;
    using NeoSpin.BusinessObjects;
    using System.Data;
    using Sagitec.BusinessObjects;
    using System.IO;
    using System.Collections;
    using Sagitec.Common;
    using System.Collections.ObjectModel;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class busPersonAccountPeoplesoftInterface : busNeoSpinBatch
    {


        //private string istrGeneratedPath;
        //private string istrImagedPath;
        //private string istrImagingPrinterName;
        //private string istrPurgedPath;
        public busPersonAccountPeoplesoftInterface()
        {
        }



        public void UpdateWSSRequestTables()
        {
            busNeoSpinBase lbusNeospinbase = new busNeoSpinBase();
            busPsPerson lbusPsPerson = new busPsPerson { icdoPsPerson = new cdoPsPerson() };
            Collection<busPsPerson> iclbPersonInformativeErrors = new Collection<busPsPerson>();
            lbusPsPerson.LoadUnprocessedPsPerson();
            lbusPsPerson.InsertPerson(ref iclbPersonInformativeErrors);

            busPsAddress lbusPsAddress = new busPsAddress { icdoPsAddress = new cdoPsAddress() };
            Collection<busPsAddress> iclbPersonAddressInformativeErrors = new Collection<busPsAddress>();
            lbusPsAddress.LoadUnprocessedPsAddress();
            lbusPsAddress.InsertPersonAddress(lbusPsPerson.iclbProcessedPsPerson, ref iclbPersonAddressInformativeErrors);
            busPsEmployment lbusPsEmployment = new busPsEmployment { icdoPsEmployment = new cdoPsEmployment() };
            Collection<busPsEmployment> iclbPersonEmploymentInformativeErrors = new Collection<busPsEmployment>();
            lbusPsEmployment.LoadUnprocessedPsEmployment();
            lbusPsEmployment.InsertPersonEmployment(lbusPsPerson.iclbProcessedPsPerson, lbusPsAddress.iclbProcessedPsPersonAddress, ref iclbPersonEmploymentInformativeErrors);
            DataTable ldtPsPersonTable = CreateErrorListOfPSperson();
            DataTable ldtPersonAddressTable = CreateErrorListOfPSAddress();
            DataTable ldtPersonEmploymentTable = CreateErrorListOfPSEmployment();
            //Update the Processed flags to N if they have some errors 
            foreach (busPsPerson lobjPsPerson in lbusPsPerson.iclbUnprocessedPsPerson)
            {
                if (iclbPersonInformativeErrors.Where(lobj=>lobj.icdoPsPerson.ps_person_id == lobjPsPerson.icdoPsPerson.ps_person_id).Count() > 0)
                {
                    lbusPsPerson.iclbProcessedPsPerson.Add(lobjPsPerson);
                    //lobjPsPerson.icdoPsPerson.processed_flag = busConstant.Flag_Yes;
                }
                else
                {
                    lobjPsPerson.icdoPsPerson.processed_flag = busConstant.Flag_No;
                }
                lobjPsPerson.icdoPsPerson.Update();

                foreach (KeyValuePair<int, ArrayList> aobj in lobjPsPerson.idictPsPersonError)
                {
                   
                    foreach (object obj in aobj.Value)
                    {
                        utlError lobj = (utlError)obj;
                        DataRow ldrRow = ldtPsPersonTable.NewRow();
                        ldrRow["Ps_person_id"] = aobj.Key;
                        ldrRow["Peoplesoft_id"] = lobjPsPerson.icdoPsPerson.peoplesoft_id;
                        ldrRow["First_Name"] = lobjPsPerson.icdoPsPerson.first_name;
                        ldrRow["Last_Name"] = lobjPsPerson.icdoPsPerson.last_name;
                        ldrRow["ssn"] = lobjPsPerson.icdoPsPerson.ssn.Substring(5);
                        ldrRow["Employer_Name"] = null;
                        ldrRow["Error_id"] = String.IsNullOrEmpty(lobj.istrErrorID) ? 0 : Convert.ToInt32(lobj.istrErrorID);
                        ldrRow["Error_description"] = lobj.istrErrorMessage;
                        ldtPsPersonTable.Rows.Add(ldrRow);
                    }
                }
            }
            
            
            foreach (busPsAddress lobjPsAddress in lbusPsAddress.iclbUnProcessedPsPersonAddress)
            {
                busPsPerson lobjPsPerson = lbusPsAddress.LoadPSPersonBySSN(lobjPsAddress.icdoPsAddress.ssn);
                if (iclbPersonAddressInformativeErrors.Where(lobj=>lobj.icdoPsAddress.ps_address_id==lobjPsAddress.icdoPsAddress.ps_address_id).Count() > 0)
                {
                    lbusPsAddress.iclbProcessedPsPersonAddress.Add(lobjPsAddress);
                    //lobjPsAddress.icdoPsAddress.processed_flag = busConstant.Flag_Yes;
                }
                else
                {
                    lobjPsAddress.icdoPsAddress.processed_flag = busConstant.Flag_No;
                }
                lobjPsAddress.icdoPsAddress.Update();
                //lobjPsAddress.icdoPsAddress.processed_flag = busConstant.Flag_No;
                //lobjPsAddress.icdoPsAddress.Update();
                foreach (KeyValuePair<int, ArrayList> aobj in lobjPsAddress.idictPsAddressError)
                {
                   
                    busPsPerson lobjPsPerson1 = lbusPsAddress.LoadPSPersonBySSN(lobjPsAddress.icdoPsAddress.ssn);                    
                    //var temp = a.Value;
                    foreach (object obj in aobj.Value)
                    {
                        utlError lobj = (utlError)obj;
                        DataRow ldrRow = ldtPersonAddressTable.NewRow();
                        ldrRow["Ps_address_id"] = aobj.Key;
                        if (lobjPsPerson1 != null )
                        {
                            ldrRow["First_Name"] = lobjPsPerson.icdoPsPerson.first_name;
                            ldrRow["Last_Name"] = lobjPsPerson.icdoPsPerson.last_name;
                            ldrRow["ssn"] = lobjPsPerson.icdoPsPerson.ssn.Substring(5);
                        }
                        ldrRow["Employer_Name"] = null;
                        ldrRow["Error_id"] = String.IsNullOrEmpty(lobj.istrErrorID) ? 0 : Convert.ToInt32(lobj.istrErrorID);
                        ldrRow["Error_description"] = lobj.istrErrorMessage;
                        ldtPersonAddressTable.Rows.Add(ldrRow);
                    }
                }
            }
            foreach (busPsEmployment lobjPsEmployment in lbusPsEmployment.iclbUnprocessedPsEmployment)
            {
                if (iclbPersonEmploymentInformativeErrors.Where(lobj=>lobj.icdoPsEmployment.ps_employment_id == lobjPsEmployment.icdoPsEmployment.ps_employment_id).Count() >0)
                {
                    lbusPsEmployment.iclbProcessedPsEmployment.Add(lobjPsEmployment);
                    //lobjPsEmployment.icdoPsEmployment.processed_flag = busConstant.Flag_Yes;
                }
                else
                {
                    lobjPsEmployment.icdoPsEmployment.processed_flag = busConstant.Flag_No;
                }
                lobjPsEmployment.icdoPsEmployment.Update();
                //lobjPsEmployment.icdoPsEmployment.processed_flag = busConstant.Flag_No;
                //lobjPsEmployment.icdoPsEmployment.Update();
                foreach (KeyValuePair<int, ArrayList> aobj in lobjPsEmployment.idictPsEmploymentError)
                {
                    
                    //busPsPerson lobjPsPerson = lbusPsAddress.LoadPSPersonBySSN(lobjPsEmployment.icdoPsEmployment.ssn);
                    foreach (object obj in aobj.Value)
                    {
                        utlError lobj = (utlError)obj;
                        DataRow ldrRow = ldtPersonEmploymentTable.NewRow();
                        ldrRow["Ps_employment_id"] = aobj.Key;
                        ldrRow["Peoplesoft_id"] = lobjPsEmployment.icdoPsEmployment.peoplesoft_id;
                        ldrRow["First_Name"] = lobjPsEmployment.icdoPsEmployment.first_name;
                        ldrRow["Last_Name"] = lobjPsEmployment.icdoPsEmployment.last_name;
                        ldrRow["ssn"] = lobjPsEmployment.icdoPsEmployment.ssn.Substring(5);
                        ldrRow["Employer_Name"] = null;
                        ldrRow["Error_id"] = String.IsNullOrEmpty(lobj.istrErrorID) ? 0:Convert.ToInt32(lobj.istrErrorID);
                        ldrRow["Error_description"] = lobj.istrErrorMessage;
                        ldrRow["Org_Code"] = lobjPsEmployment.icdoPsEmployment.org_code;
                        ldtPersonEmploymentTable.Rows.Add(ldrRow);
                    }
                }
            }

            DataSet ldsResult = new DataSet();
            ldsResult.Tables.Add(ldtPsPersonTable.Copy());
            ldsResult.Tables.Add(ldtPersonAddressTable.Copy());
            ldsResult.Tables.Add(ldtPersonEmploymentTable.Copy());
            //Update the records make the processed flag to Y which are processed in the batch
            foreach(busPsPerson lobjPsPerson in  lbusPsPerson.iclbProcessedPsPerson)
            {
                lobjPsPerson.icdoPsPerson.processed_flag = busConstant.Flag_Yes;
                lobjPsPerson.icdoPsPerson.Update();
            }
            foreach (busPsAddress lobjPsAddress in lbusPsAddress.iclbProcessedPsPersonAddress)
            {
                lobjPsAddress.icdoPsAddress.processed_flag = busConstant.Flag_Yes;
                lobjPsAddress.icdoPsAddress.Update();
            }
            foreach (busPsEmployment lobjPsEmployment in lbusPsEmployment.iclbProcessedPsEmployment)
            {
                lobjPsEmployment.icdoPsEmployment.processed_flag = busConstant.Flag_Yes;
                lobjPsEmployment.icdoPsEmployment.Update();
            }
            string lstr = lbusNeospinbase.CreateReport("rptPeoplesoftErrorDetails.rpt", ldsResult, "PS_");

            //PIR 23631 - Update Error flag to Y for records in Error report.
            if (ldtPsPersonTable?.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtPsPersonTable.Rows)
                {
                    busPsPerson lobjPSPerson = new busPsPerson { icdoPsPerson = new cdoPsPerson() };
                    lobjPSPerson.icdoPsPerson.LoadData(dr);
                    lobjPSPerson.icdoPsPerson.error = busConstant.Flag_Yes;
                    lobjPSPerson.icdoPsPerson.Update();
                }
            }
            if (ldtPersonAddressTable?.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtPersonAddressTable.Rows)
                {
                    busPsAddress lobjPSAddress = new busPsAddress { icdoPsAddress = new cdoPsAddress() };
                    lobjPSAddress.icdoPsAddress.LoadData(dr);
                    lobjPSAddress.icdoPsAddress.error = busConstant.Flag_Yes;
                    lobjPSAddress.icdoPsAddress.Update();
                }
            }
            if (ldtPersonEmploymentTable?.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtPersonEmploymentTable.Rows)
                {
                    busPsEmployment lobjPSEmp = new busPsEmployment { icdoPsEmployment = new cdoPsEmployment() };
                    lobjPSEmp.icdoPsEmployment.LoadData(dr);
                    lobjPSEmp.icdoPsEmployment.error = busConstant.Flag_Yes;
                    lobjPSEmp.icdoPsEmployment.Update();
                }
            }

        }

        private DataTable CreateErrorListOfPSEmployment()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("PS_employment_id", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("Employer_Name", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ssn", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("First_name", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("Last_Name", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("Error_id", Type.GetType("System.Int32"));
            DataColumn dc7 = new DataColumn("Error_description", Type.GetType("System.String"));
            DataColumn dc8 = new DataColumn("Peoplesoft_id", Type.GetType("System.String"));
            DataColumn dc9 = new DataColumn("Org_Code", Type.GetType("System.String"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            ldtbReportTable.Columns.Add(dc5);
            ldtbReportTable.Columns.Add(dc6);
            ldtbReportTable.Columns.Add(dc7);
            ldtbReportTable.Columns.Add(dc8);
            ldtbReportTable.Columns.Add(dc9);
            ldtbReportTable.TableName = busConstant.ReportTableName03;
            return ldtbReportTable;             
        }

        private DataTable CreateErrorListOfPSperson()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("PS_person_id", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("Employer_Name", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ssn", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("First_name", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("Last_Name", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("Error_id", Type.GetType("System.Int32"));
            DataColumn dc7 = new DataColumn("Error_description", Type.GetType("System.String"));
            DataColumn dc8 = new DataColumn("Peoplesoft_id", Type.GetType("System.String"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            ldtbReportTable.Columns.Add(dc5);
            ldtbReportTable.Columns.Add(dc6);
            ldtbReportTable.Columns.Add(dc7);
            ldtbReportTable.Columns.Add(dc8);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;            
        }

        private DataTable CreateErrorListOfPSAddress()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn dc1 = new DataColumn("PS_address_id", Type.GetType("System.Int32"));
            DataColumn dc2 = new DataColumn("Employer_Name", Type.GetType("System.String"));
            DataColumn dc3 = new DataColumn("ssn", Type.GetType("System.String"));
            DataColumn dc4 = new DataColumn("First_name", Type.GetType("System.String"));
            DataColumn dc5 = new DataColumn("Last_Name", Type.GetType("System.String"));
            DataColumn dc6 = new DataColumn("Error_id", Type.GetType("System.Int32"));
            DataColumn dc7 = new DataColumn("Error_description", Type.GetType("System.String"));
            ldtbReportTable.Columns.Add(dc1);
            ldtbReportTable.Columns.Add(dc2);
            ldtbReportTable.Columns.Add(dc3);
            ldtbReportTable.Columns.Add(dc4);
            ldtbReportTable.Columns.Add(dc5);
            ldtbReportTable.Columns.Add(dc6);
            ldtbReportTable.Columns.Add(dc7);
            ldtbReportTable.TableName = busConstant.ReportTableName02;
            return ldtbReportTable;
        }

        //public void ConvertToTIFF()
        //{
        //    string[] fileArray = Directory.GetFiles(@"C:\\Users\\ashish.kale\\Desktop\\DOC");

            
        //    //Generated Path
        //    if (String.IsNullOrEmpty(istrGeneratedPath))
        //    {
        //        istrGeneratedPath = "C:\\Users\\ashish.kale\\Desktop\\DOC";// utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
        //    }
        //    //Imaged Path
        //    if (String.IsNullOrEmpty(istrImagedPath))
        //    {
        //        istrImagedPath = "C:\\Users\\ashish.kale\\Desktop\\TIF\\";//utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrImag");
        //    }
        //    //imaging printer name
        //    if (String.IsNullOrEmpty(istrImagingPrinterName))
        //    {
        //        istrImagingPrinterName = utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(44, "TIFF");
        //    }
            
        //    foreach (string file in fileArray)
        //    {


        //        //busCorTracking ibusCorTracking = new busCorTracking();

        //        //ibusCorTracking.icdoCorTracking = new cdoCorTracking();
        //        //cdoCorTracking icdoCorTracking = ibusCorTracking.icdoCorTracking;
        //        //icdoCorTracking.LoadData(ldtrResult);
        //        //ibusCorTracking.ibusCorTemplates = new busCorTemplates();
        //        //busCorTemplates ibusCorTemplates = ibusCorTracking.ibusCorTemplates;
        //        //ibusCorTemplates.icdoCorTemplates = new cdoCorTemplates();
        //        //ibusCorTemplates.icdoCorTemplates.LoadData(ldtrResult);

        //        //string lstrFileName = istrGeneratedPath + ibusCorTracking.istrWordFileName;
        //        //string lstrImagedFileName = istrImagedPath + ibusCorTracking.istrTifFileName;
        //        //string a= "C:\\Users\\ashish.kale\\Desktop\\Test\"+;
        //        //    myfile.replace(extension,".Jpeg");

        //        string istrTifFileName=Path.ChangeExtension(file,".tif");
        //        string[] seperator = { "\\" };
        //        string[] k = istrTifFileName.Split(seperator, StringSplitOptions.None);
        //        istrTifFileName = k[k.Length - 1];
        //        //string istrTifFileName = file + ".tif"; 
        //        //Converting into TIFF Format
        //        //Open the doc and printer to the directory 
        //        iobjCorBuilder.m_corr.OpenDocReadonly(file);
        //        //iobjCorBuilder.m_corr.SetActivePrinter("FAX");
        //        iobjCorBuilder.m_corr.PrintDoc(istrImagingPrinterName, istrImagedPath + istrTifFileName);
        //        iobjCorBuilder.m_corr.CloseActiveDoc();

        //        //Idle for Few Seconds In order to Convert to TIFF..                
        //        //Wait Untill TIFF Image Gets Converted
        //        bool lblnConvertedToTIFF = false;
        //        while (!lblnConvertedToTIFF)
        //        {
        //            System.Threading.Thread.Sleep(5000);
        //            if (File.Exists(istrImagedPath + istrTifFileName))
        //            {
        //                FileInfo info = new FileInfo(istrImagedPath + istrTifFileName);
        //                if (info.Length > 0)
        //                    lblnConvertedToTIFF = true;
        //            }
        //        }

        //        //Update the Flag
        //        //icdoCorTracking.converted_to_image_flag = "Y";
        //        //icdoCorTracking.Update();
        //    }
        //}
    }
}
