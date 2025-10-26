using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using Sagitec.Common;
using NeoSpin.CustomDataObjects;
using System.Net;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Sagitec.DBUtility;
using System.Globalization;
using System.IO;
using Sagitec.Common;
using NeoSpin.BusinessObjects;
using System.Web.UI.WebControls;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Interop.Excel;
using Sagitec.Interface;
using NeoSpin.Common;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busNeoSpinBase : busExtendBase
    {
        //public static utlServerDetail iutlServerDetail;
        public ReportDocument irptDocument;

        static busNeoSpinBase()
        {
            try
            {
                //iutlServerDetail = new utlServerDetail();
                //Server IP address is set-up once
                string lstrDns = Dns.GetHostName();
                //iutlServerDetail.istrIPAddress = lstrDns;

                System.IO.FileInfo lfi = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8));
                if (lfi != null)
                {
                    ServiceHelper.idteReleaseDate = lfi.LastWriteTime;
                }
            }
            catch
            {
                //Intentionally eating exception because this code is causing exception while completing the WF activity at Dns.GetHostName();
            }
        }
        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>
        {
            {"ai", "application/postscript"},
            {"aif", "audio/x-aiff"},
            {"aifc", "audio/x-aiff"},
            {"aiff", "audio/x-aiff"},
            {"asc", "text/plain"},
            {"atom", "application/atom+xml"},
            {"au", "audio/basic"},
            {"avi", "video/x-msvideo"},
            {"bcpio", "application/x-bcpio"},
            {"bin", "application/octet-stream"},
            {"bmp", "BMP File"},
            {"cdf", "application/x-netcdf"},
            {"cgm", "image/cgm"},
            {"class", "application/octet-stream"},
            {"cpio", "application/x-cpio"},
            {"cpt", "application/mac-compactpro"},
            {"csh", "application/x-csh"},
            {"css", "text/css"},
            {"dcr", "application/x-director"},
            {"dif", "video/x-dv"},
            {"dir", "application/x-director"},
            {"djv", "image/vnd.djvu"},
            {"djvu", "image/vnd.djvu"},
            {"dll", "application/octet-stream"},
            {"dmg", "application/octet-stream"},
            {"dms", "application/octet-stream"},
            {"doc", "Microsoft Word 97 - 2003 Document"},
            {"docx","Microsoft Word Document"},
            {"dotx", "Microsoft Word template"},
            {"docm","Microsoft Word macro-enabled document"},
            {"dotm","Microsoft Word macro-enabled template"},
            {"dtd", "application/xml-dtd"},
            {"dv", "video/x-dv"},
            {"dvi", "application/x-dvi"},
            {"dxr", "application/x-director"},
            {"eps", "application/postscript"},
            {"etx", "text/x-setext"},
            {"exe", "application/octet-stream"},
            {"ez", "application/andrew-inset"},
            {"gif", "Graphical Interchange Format file"},
            {"gram", "application/srgs"},
            {"grxml", "application/srgs+xml"},
            {"gtar", "application/x-gtar"},
            {"hdf", "application/x-hdf"},
            {"hqx", "application/mac-binhex40"},
            {"htm", "Chrome HTML Document"},
            {"html", "Chrome HTML Document"},
            {"ice", "x-conference/x-cooltalk"},
            {"ico", "image/x-icon"},
            {"ics", "text/calendar"},
            {"ief", "image/ief"},
            {"ifb", "text/calendar"},
            {"iges", "model/iges"},
            {"igs", "model/iges"},
            {"jnlp", "application/x-java-jnlp-file"},
            {"jp2", "image/jp2"},
            {"jpe", "image/jpeg"},
            {"jpeg", "JPEG File"},
            {"jpg", "JPEG File"},
            {"js", "application/x-javascript"},
            {"kar", "audio/midi"},
            {"latex", "application/x-latex"},
            {"lha", "application/octet-stream"},
            {"lzh", "application/octet-stream"},
            {"m3u", "audio/x-mpegurl"},
            {"m4a", "audio/mp4a-latm"},
            {"m4b", "audio/mp4a-latm"},
            {"m4p", "audio/mp4a-latm"},
            {"m4u", "video/vnd.mpegurl"},
            {"m4v", "video/x-m4v"},
            {"mac", "image/x-macpaint"},
            {"man", "application/x-troff-man"},
            {"mathml", "application/mathml+xml"},
            {"me", "application/x-troff-me"},
            {"mesh", "model/mesh"},
            {"mid", "audio/midi"},
            {"midi", "audio/midi"},
            {"mif", "application/vnd.mif"},
            {"mov", "video/quicktime"},
            {"movie", "video/x-sgi-movie"},
            {"mp2", "audio/mpeg"},
            {"mp3", "audio/mpeg"},
            {"mp4", "video/mp4"},
            {"mpe", "video/mpeg"},
            {"mpeg", "video/mpeg"},
            {"mpg", "video/mpeg"},
            {"mpga", "audio/mpeg"},
            {"ms", "application/x-troff-ms"},
            {"msg", "Outlook Item"},
            {"msh", "model/mesh"},
            {"mxu", "video/vnd.mpegurl"},
            {"nc", "application/x-netcdf"},
            {"oda", "application/oda"},
            {"ogg", "application/ogg"},
            {"pbm", "image/x-portable-bitmap"},
            {"pct", "image/pict"},
            {"pdb", "chemical/x-pdb"},
            {"pdf", "Microsoft Edge PDF Document "},
            {"pgm", "image/x-portable-graymap"},
            {"pgn", "application/x-chess-pgn"},
            {"pic", "image/pict"},
            {"pict", "image/pict"},
            {"png", "PNG File"},
            {"pnm", "image/x-portable-anymap"},
            {"pnt", "image/x-macpaint"},
            {"pntg", "image/x-macpaint"},
            {"ppm", "image/x-portable-pixmap"},
            {"ppt", "Microsoft PowerPoint 97-2003 Presentation"},
            {"pptx","Microsoft PowerPoint Presentation"},
            {"potx","application/vnd.openxmlformats-officedocument.presentationml.template"},
            {"ppsx","application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
            {"ppam","application/vnd.ms-powerpoint.addin.macroEnabled.12"},
            {"pptm","application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
            {"potm","application/vnd.ms-powerpoint.template.macroEnabled.12"},
            {"ppsm","application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
            {"ps", "application/postscript"},
            {"qt", "video/quicktime"},
            {"qti", "image/x-quicktime"},
            {"qtif", "image/x-quicktime"},
            {"ra", "audio/x-pn-realaudio"},
            {"ram", "audio/x-pn-realaudio"},
            {"ras", "image/x-cmu-raster"},
            {"rdf", "application/rdf+xml"},
            {"rgb", "image/x-rgb"},
            {"rm", "application/vnd.rn-realmedia"},
            {"roff", "application/x-troff"},
            {"rtf", "Rich Text Format"},
            {"rtx", "Rich Text Document"},
            {"sgm", "text/sgml"},
            {"sgml", "text/sgml"},
            {"sh", "application/x-sh"},
            {"shar", "application/x-shar"},
            {"silo", "model/mesh"},
            {"sit", "application/x-stuffit"},
            {"skd", "application/x-koan"},
            {"skm", "application/x-koan"},
            {"skp", "application/x-koan"},
            {"skt", "application/x-koan"},
            {"smi", "application/smil"},
            {"smil", "application/smil"},
            {"snd", "audio/basic"},
            {"so", "application/octet-stream"},
            {"spl", "application/x-futuresplash"},
            {"src", "application/x-wais-source"},
            {"sv4cpio", "application/x-sv4cpio"},
            {"sv4crc", "application/x-sv4crc"},
            {"svg", "image/svg+xml"},
            {"swf", "application/x-shockwave-flash"},
            {"sql", "Microsoft SQL Server Query File"},
            {"t", "application/x-troff"},
            {"tar", "application/x-tar"},
            {"tcl", "application/x-tcl"},
            {"tex", "application/x-tex"},
            {"texi", "application/x-texinfo"},
            {"texinfo", "application/x-texinfo"},
            {"tif", "image/tiff"},
            {"tiff", "image/tiff"},
            {"tr", "application/x-troff"},
            {"tsv", "text/tab-separated-values"},
            {"txt", "Text Document"},
            {"ustar", "application/x-ustar"},
            {"vcd", "application/x-cdlink"},
            {"vrml", "model/vrml"},
            {"vxml", "application/voicexml+xml"},
            {"wav", "audio/x-wav"},
            {"wbmp", "image/vnd.wap.wbmp"},
            {"wbmxl", "application/vnd.wap.wbxml"},
            {"wml", "text/vnd.wap.wml"},
            {"wmlc", "application/vnd.wap.wmlc"},
            {"wmls", "text/vnd.wap.wmlscript"},
            {"wmlsc", "application/vnd.wap.wmlscriptc"},
            {"wrl", "model/vrml"},
            {"xbm", "image/x-xbitmap"},
            {"xht", "application/xhtml+xml"},
            {"xhtml", "application/xhtml+xml"},
            {"xls", "Microsoft Excel 97-2003 Worksheet"},
            {"xml", "application/xml"},
            {"xpm", "image/x-xpixmap"},
            {"xsl", "application/xml"},
            {"xlsx","Microsoft Excel Worksheet"},
            {"xltx","application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
            {"xlsm","application/vnd.ms-excel.sheet.macroEnabled.12"},
            {"xltm","application/vnd.ms-excel.template.macroEnabled.12"},
            {"xlam","application/vnd.ms-excel.addin.macroEnabled.12"},
            {"xlsb","application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
            {"xslt", "application/xslt+xml"},
            {"xul", "application/vnd.mozilla.xul+xml"},
            {"xwd", "image/x-xwindowdump"},
            {"xyz", "chemical/x-xyz"},
            {"zip", "application/zip"}
        };
        public static string GetMIMEType(string fileName)
        {
            //get file extension
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (extension.Length > 0 &&
                MIMETypesDictionary.ContainsKey(extension.Remove(0, 1)))
            {
                return MIMETypesDictionary[extension.Remove(0, 1)];
            }
            return "unknown/unknown";
        }
        /// <summary>
        /// Author                  : Base Solution
        /// Modified By             : NA
        /// Applies To Use cases    : All 
        /// Usage                   : Derive Mime Type From File Name
        /// </summary>
        /// <param name="astrFileName">File Name</param>
        /// <returns>Mime Type From File Name</returns>
        public static string DeriveMimeTypeFromFileName(string astrFileName)
        {
            string lstrMimeType = "application/octet-stream";
            string[] larrDotSplit = astrFileName.Split(".");
            if (larrDotSplit != null && larrDotSplit.Length > 0)
            {
                string lstrFileExtension = larrDotSplit[larrDotSplit.Length - 1];
                switch (lstrFileExtension.ToLower())
                {
                    case "pdf":
                        lstrMimeType = "application/pdf";
                        break;
                    case "doc":
                    case "docx":
                        lstrMimeType = "application/msword";
                        break;
                    case "html":
                        lstrMimeType = "application/iexplore";
                        break;
                    case "xls":
                        lstrMimeType = "application/vnd.ms-excel";
                        break;
                    case "xlsx":
                        lstrMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    default:
                        lstrMimeType = "application/pdf";
                        break;
                }
            }
            return lstrMimeType;
        }
        public static utlCorresPondenceInfo SetCorrespondence(
            string astrTemplateName, object aarrResult, Hashtable ahtbQueryBkmarks)
        {
            bool lblnExists = false; // PIR 10400
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            //lobjPassInfo.istrUserID = astrUserID;
            //lobjPassInfo.iintUserSerialID = aintUserSerialID;
            //			iobjPassInfo.BeginTransaction();
            cdoCorTemplates lobjCorTemplate = new cdoCorTemplates();
            lobjCorTemplate.LoadByTemplateName(astrTemplateName);

            utlCorresPondenceInfo lobjCorresPondenceInfo =
                lobjPassInfo.isrvMetaDataCache.GetCorresPondenceInfo(astrTemplateName);

            lobjCorresPondenceInfo.istrContactRole = lobjCorTemplate.contact_role_value;

            lobjCorresPondenceInfo.istrTemplatePath = lobjPassInfo.isrvDBCache.GetPathInfo("CorrTmpl");
            lobjCorresPondenceInfo.istrGeneratePath = lobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");

            //Set the auto print flag and printer name from Cor template
            lobjCorresPondenceInfo.istrAutoPrintFlag = lobjCorTemplate.auto_print_flag;
            lobjCorresPondenceInfo.istrPrinterName = lobjCorTemplate.printer_name_description;

            cdoCorTracking lcdoCorTracking = new cdoCorTracking();
            lcdoCorTracking.template_id = lobjCorTemplate.template_id;
            //PIR 23338
            if (lobjCorresPondenceInfo.istrAutoPrintFlag == busConstant.Flag_Yes && lobjCorTemplate.batch_print_flag == busConstant.Flag_Yes)
            {
                lcdoCorTracking.cor_status_value = busConstant.CorrespondenceStatus_Ready_For_Batch_Printing;
                lobjCorresPondenceInfo.istrAutoPrintFlag = busConstant.Flag_No;
            }
            else
			{
                lcdoCorTracking.cor_status_value = "GENR";
			}
            lcdoCorTracking.generated_date = DateTime.Now;
            lcdoCorTracking.created_by = lobjPassInfo.istrUserID;
            lcdoCorTracking.modified_by = lobjPassInfo.istrUserID;
            lcdoCorTracking.comments = "";

            busOrganization lobjOrganization = null;
            busPerson lobjPerson = null;

            ArrayList lobjArrayList = new ArrayList();
            lobjArrayList.Add(aarrResult);

            GetNeoSpinValues(lobjCorresPondenceInfo, lobjArrayList, ahtbQueryBkmarks, lcdoCorTracking, lobjOrganization, lobjPerson);

            lcdoCorTracking.person_id = lobjCorresPondenceInfo.iintPersonID;
            lcdoCorTracking.org_contact_id = lobjCorresPondenceInfo.iintOrgContactID;

            //PIR 14770 - Created by & modified by issue
            if(lobjCorTemplate.template_id == 534 && string.IsNullOrEmpty(lobjPassInfo.istrUserID))
            {
                lcdoCorTracking.created_by = Convert.ToString(lobjCorresPondenceInfo.iintPersonID);
                lcdoCorTracking.modified_by = Convert.ToString(lobjCorresPondenceInfo.iintPersonID);
            }

            // PIR 10400
            if (lobjCorTemplate.template_id == 530 || lobjCorTemplate.template_id == 534)
            {
                System.Data.DataTable ldtbResult = busBase.Select("cdoCorTracking.GetCorrTrackPersonTemplate",
                                      new object[2] { lobjCorresPondenceInfo.iintPersonID, lobjCorTemplate.template_id });
                if (ldtbResult.Rows.Count == 0)
                {
                    lblnExists = false;
                }
                else
                {
                    // PIR 10400 If the letter is already in tracking with generated or imaged status we should not generate the same letter within 24 hours.
                    string trackingStatus = ldtbResult.Rows[0]["COR_STATUS_VALUE"].ToString();
                    DateTime generatedDateTime = Convert.ToDateTime(ldtbResult.Rows[0]["GENERATED_DATE"]);
                    if ((generatedDateTime.AddHours(24) > DateTime.Now) &&
                        ((trackingStatus == busConstant.CorrespondenceStatus_Imaged) || (trackingStatus == busConstant.CorrespondenceStatus_Generated)))
                    {
                        lblnExists = true;
                    }
                    else
                    {
                        lblnExists = false;
                    }
                }
            }
            else
            {
                lblnExists = false;
            }

            if (!lblnExists)  // PIR 10400
            {
                //Populate plan for the relevant tracking records
                string lstrValue = busBase.GetBookMarkValue("PlanId", lobjCorresPondenceInfo.icolBookmarkFieldInfo);
                lcdoCorTracking.plan_id = 0;
                if (string.IsNullOrEmpty(lstrValue))
                {
                    lcdoCorTracking.plan_id = 0;
                }
                else
                {
                    lcdoCorTracking.plan_id = Convert.ToInt32(lstrValue);
                }
                lcdoCorTracking.Insert();
                lobjCorresPondenceInfo.iintCorrespondenceTrackingId = lcdoCorTracking.tracking_id;

                //Generate the file name
                string strSlNo = lobjCorresPondenceInfo.iintCorrespondenceTrackingId.ToString().PadLeft(10, '0');
                lobjCorresPondenceInfo.istrGeneratedFileName = lobjCorresPondenceInfo.istrTemplateName + "-" + strSlNo + ".docx";
                LoadStandardConstantBookmarks(lobjCorresPondenceInfo);
                LoadLastBookmarks(lobjCorresPondenceInfo);
                //ucs - 032 : publishing message and sending mail , only from neospin batch
                busWSSHome lobjWSSHome = new busWSSHome();
                //if (astrUserID == busConstant.PERSLinkBatchUser && lcdoCorTracking.person_id > 0 &&
                //    lobjPerson != null && lobjPerson.icdoPerson != null && lobjPerson.icdoPerson.communication_preference_value == busConstant.PersonCommPrefMail)
                if (lobjPassInfo.istrUserID == busConstant.PERSLinkBatchUser && lcdoCorTracking.person_id > 0)
                {
                    busPerson lobjMember = new busPerson();
                    lobjMember.FindPerson(lcdoCorTracking.person_id);
                    if (lobjMember.icdoPerson.communication_preference_value == busConstant.PersonCommPrefMail)
                    {
                        string lstrCorrsGenPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenr");
                        lstrCorrsGenPath = lstrCorrsGenPath + "\\" + lobjCorresPondenceInfo.istrGeneratedFileName;
                        lobjWSSHome.PublishToWSSAndEmail(lstrCorrsGenPath);
                        lobjWSSHome = null;
                    }
                }
            }
            return lobjCorresPondenceInfo;
        }

        private static void GetNeoSpinValues(
            utlCorresPondenceInfo aobjCorrInfo, ArrayList aarrResult, Hashtable ahstQueryBookMks, cdoCorTracking acdoCorTracking, busOrganization abusOrganization, busPerson abusPerson)
        {
            busBase lobjCorr = null;
            ArrayList larrCorr = aarrResult;
            //Target Business Object Fix
            if ((aarrResult != null) && (aarrResult.Count > 0) && (aobjCorrInfo.ihstFormInfo != null) && (aobjCorrInfo.ihstFormInfo.Count > 0))
            {
                string lstrCurrentObjectName = aarrResult[0].GetType().Name;
                if (lstrCurrentObjectName != aobjCorrInfo.istrObjectID)
                {
                    string lstrPropertyName = (string)aobjCorrInfo.ihstFormInfo[ahstQueryBookMks["sfwCallingForm"]];
                    if (lstrPropertyName == null)
                        throw new Exception(lstrCurrentObjectName + " Calling Form is Not Set !");
                    lobjCorr = (busBase)HelperFunction.GetValue(lstrCurrentObjectName, lstrPropertyName, aarrResult[0], ReturnType.Object);
                    larrCorr = new ArrayList();
                    larrCorr.Add(lobjCorr);
                }
            }

            //This Block will load the objects that are needed only in correpondence.
            busBase lobjBus = null;

            if (larrCorr.Count > 0)
            {
                lobjBus = (busBase)HelperFunction.GetObjectFromResult(larrCorr[0], aobjCorrInfo.istrObjectID);
            }

            if (lobjBus == null)
            {
                throw new Exception("Unable to find correspondence object " + aobjCorrInfo.istrObjectID);
            }

            lobjBus.LoadCorresProperties(aobjCorrInfo.istrTemplateName);

            //Commented for 6.0 Upgrade
            //lobjBus = (busBase)busBase.GetStandardValues(aobjCorrInfo, larrCorr, ahstQueryBookMks);
            lobjBus = GetCorrespondenceObject(aobjCorrInfo, lobjBus, ahstQueryBookMks);
            lobjBus.LoadBookmarkValues(aobjCorrInfo, ahstQueryBookMks);

            abusPerson = (busPerson)lobjBus.GetCorPerson();
            if (abusPerson != null)
                LoadStandardPersonBookMarks(abusPerson, aobjCorrInfo);
            abusOrganization = (busOrganization)lobjBus.GetCorOrganization();
            if (aobjCorrInfo.icolBookmarkChldTemplateInfo.IsNotNull() && aobjCorrInfo.icolBookmarkChldTemplateInfo.Count > 0)
            {
                foreach (utlCorresPondenceInfo aobjChildBookmarkinfo in aobjCorrInfo.icolBookmarkChldTemplateInfo)
                {
                    foreach (utlBookmarkFieldInfo aobjChildFieldBookmarkinfo in aobjChildBookmarkinfo.icolBookmarkFieldInfo)
                    {
                        busExtendBase lbusExtendBase = new busExtendBase();
                        if (aobjChildFieldBookmarkinfo.istrName == "SFNLogo" && string.IsNullOrEmpty(aobjChildFieldBookmarkinfo.istrValue))
                        {
                            aobjChildFieldBookmarkinfo.istrValue = lbusExtendBase.istrImageSFNLogo;
                        }
                        if (aobjChildFieldBookmarkinfo.istrName == "SFNAddress" && string.IsNullOrEmpty(aobjChildFieldBookmarkinfo.istrValue))
                        {
                            aobjChildFieldBookmarkinfo.istrValue = lbusExtendBase.istrImageSFNAddress;
                        }
                        if (aobjChildFieldBookmarkinfo.istrName == "PlanId" && !string.IsNullOrEmpty(busBase.GetBookMarkValue("PlanId", aobjCorrInfo.icolBookmarkFieldInfo)))
                        {
                            aobjChildFieldBookmarkinfo.istrValue = busBase.GetBookMarkValue("PlanId", aobjCorrInfo.icolBookmarkFieldInfo);
                        }
                        if (aobjChildBookmarkinfo.istrTemplateName == "SFN-03803")
                        {
                            busPersonAccountDeferredComp lbusPersonAccountDeferredComp = new busPersonAccountDeferredComp();
                            if (aobjChildFieldBookmarkinfo.istrName == "50PlusLimitAmount")
                                aobjChildFieldBookmarkinfo.istrValue =Convert.ToString(lbusPersonAccountDeferredComp.idec50PlusLimitAmount);
                            if (aobjChildFieldBookmarkinfo.istrName == "AnnualLimitAmount")
                                aobjChildFieldBookmarkinfo.istrValue = Convert.ToString(lbusPersonAccountDeferredComp.idecAnnualLimitAmount);
                            if (aobjChildFieldBookmarkinfo.istrName == "Year")
                                aobjChildFieldBookmarkinfo.istrValue = DateTime.Now.ToString("yyyy"); ;
                        }
                    }
                    
                    if (aobjCorrInfo.istrTemplateName == "PER-0003")
                        RemovePersonBookMarks(aobjChildBookmarkinfo);
                    if (abusPerson != null)
                        LoadStandardPersonBookMarks(abusPerson, aobjChildBookmarkinfo);
                    if (abusOrganization != null)
                        LoadOrgContactPrimaryAddressBookMark(abusOrganization, aobjChildBookmarkinfo);
                     LoadStandardConstantBookmarks(aobjChildBookmarkinfo);
                }
            }
            
            if (abusOrganization != null)
            {
                LoadOrgContactPrimaryAddressBookMark(abusOrganization, aobjCorrInfo);

                //Assigning the Org ID for Storing in FileNet Indexing Data. 
                //We could get it from Org Contact ID, but, at time, we would generate the correpondence by role, and that might not have org contact
                //and the address will be picking it from org primary address
                if (abusOrganization.icdoOrganization != null)
                {
                    acdoCorTracking.org_id = abusOrganization.icdoOrganization.org_id;
                }
                if (abusOrganization.ibusContact != null && abusOrganization.ibusContact.icdoContact != null)
                {
                    acdoCorTracking.contact_id = abusOrganization.ibusContact.icdoContact.contact_id;
                }

            }
            return;
        }
        private static void RemovePersonBookMarks(utlCorresPondenceInfo aobjCorrInfo)
        {
            utlBookmarkFieldInfo lobjField;
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrPERSLinkID";
            lobjField.istrValue = String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFullNameLFM";
            lobjField.istrValue =  String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrLastFourDigitsOfSSN";
            lobjField.istrValue = String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrDateOfBirth";
            lobjField.istrValue = String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
        }
        private static void LoadStandardPersonBookMarks(busPerson aobjPerson, utlCorresPondenceInfo aobjCorrInfo)
        {
            aobjCorrInfo.iintPersonID = aobjPerson.icdoPerson.person_id;
            //PIR-20158 Changing selection for PAY-4151 to only include PERM addresses			
            if (aobjCorrInfo.istrTemplateName == "PAY-4151")
            {
                aobjPerson.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            }
            else if (aobjCorrInfo.istrTemplateName == "PER-0057") //PIR 20868
            {
                    aobjPerson.LoadPersonAddressFromAddressId();
            }
            else
            {
                aobjPerson.LoadPersonCurrentAddress();
            }
            string istrPlanId = busBase.GetBookMarkValue("PlanId", aobjCorrInfo.icolBookmarkFieldInfo);
            if (!String.IsNullOrEmpty(istrPlanId))
            {
                aobjPerson.LoadCurrentEmployer(Convert.ToInt32(istrPlanId));
            }

            if (aobjPerson.ibusPersonCurrentAddress == null)
            {
                aobjPerson.ibusPersonCurrentAddress = new busPersonAddress();
                aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress = new cdoPersonAddress();
            }

            utlBookmarkFieldInfo lobjField;

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdTitle";
            lobjField.istrValue = aobjPerson.icdoPerson.name_prefix_description;

            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrPERSLinkID";
            lobjField.istrValue = aobjPerson.icdoPerson.person_id.ToString();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFirstName";
            lobjField.istrValue = aobjPerson.icdoPerson.first_name ?? String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            // Member First name Lower case
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFirstNameLC";
            lobjField.istrValue = aobjPerson.icdoPerson.first_name ?? String.Empty;
            lobjField.istrValue = busGlobalFunctions.ToTitleCase(lobjField.istrValue);
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrMidInitial";
            lobjField.istrValue = aobjPerson.icdoPerson.middle_name ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrLastName";
            lobjField.istrValue = aobjPerson.icdoPerson.last_name ?? String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFullName";
            lobjField.istrValue = (aobjPerson.icdoPerson.FullName ?? String.Empty) + " " + (aobjPerson.icdoPerson.name_suffix_value ?? String.Empty);
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            // Member Full Name in Lower Case 
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFullNameLC";
            lobjField.istrValue = (aobjPerson.icdoPerson.FullName ?? String.Empty) + " " + (aobjPerson.icdoPerson.name_suffix_value ?? String.Empty);
            lobjField.istrValue = busGlobalFunctions.ToTitleCase(lobjField.istrValue);
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            //standard book mark which shows the full name in Last, First Middle format
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrFullNameLFM";
            lobjField.istrValue = (aobjPerson.icdoPerson.PersonName ?? String.Empty) + " " + (aobjPerson.icdoPerson.name_suffix_value ?? String.Empty);
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrSalutation";
            lobjField.istrValue = busGlobalFunctions.ToTitleCase(aobjPerson.icdoPerson.FullName);
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrCorStreet1";
            if (aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.care_of.IsNotNullOrEmpty())
                lobjField.istrValue = "C/O " + aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.care_of +
                                      Environment.NewLine + aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1 ?? String.Empty;
            else
                lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1 ?? String.Empty;
            
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrCorStreet2";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2 ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrCorCity";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_city ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrCorState";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrCountry";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrCountryDesc";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_description ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            //PIR 11355
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrForeignCountryDesc";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_country_description ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrForeignProvince";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_province ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrForeignPostalCode";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.foreign_postal_code ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrAdrCorZip";
            lobjField.istrValue = aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            if ((aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code != null) &&
                (aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code.Trim() != String.Empty))
            {
                lobjField.istrValue += "-" + aobjPerson.ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
            }
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrSSN";
            lobjField.istrValue = HelperFunction.FormatData(aobjPerson.icdoPerson.ssn, "{0:000-##-####}");
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrLastFourDigitsOfSSN";
            lobjField.istrValue = aobjPerson.icdoPerson.LastFourDigitsOfSSN ?? String.Empty;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrDateOfBirth";
            lobjField.istrValue = aobjPerson.icdoPerson.date_of_birth.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            // PROD PIR ID 5650
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdMbrPeopleSoftID";
            lobjField.istrValue = Convert.ToString(aobjPerson.icdoPerson.peoplesoft_id);
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            if ((!string.IsNullOrEmpty(istrPlanId)) &&
                (aobjPerson.ibusCurrentEmployment != null) &&
                (aobjPerson.ibusCurrentEmployment.ibusOrganization != null))
            {
                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdCurrentEmployerOrgCodeID";
                lobjField.istrValue = aobjPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.org_code;
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                lobjField = new utlBookmarkFieldInfo();
                lobjField.istrName = "stdCurrentEmployerOrgName";
                lobjField.istrValue = aobjPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.org_name;
                aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
            }

            //PIR 26471 new standard bookmark added for DC provider and phone number
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;
            System.Data.DataTable ldtbStandardbookMarklist = lobjPassInfo.isrvDBCache.GetCodeValues(52);

            if ((ldtbStandardbookMarklist != null) && (ldtbStandardbookMarklist.Rows.Count > 0))
            {
                foreach (DataRow dr in ldtbStandardbookMarklist.Rows)
                {
                    if ((dr["CODE_VALUE"]).ToString() == busConstant.CPOR)
                    {
                        string lstrEmpower = string.Empty;
                        lstrEmpower = Convert.ToString(dr["DATA1"]);
                        lobjField = new utlBookmarkFieldInfo();
                        lobjField.istrName = "stdCPOR_Name";
                        lobjField.istrValue = lstrEmpower;
                        aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                        string lstrPhoneNumber = string.Empty;
                        lstrPhoneNumber = Convert.ToString(dr["DATA2"]);
                        lobjField = new utlBookmarkFieldInfo();
                        lobjField.istrName = "stdCPOR_Phone";
                        lobjField.istrValue = HelperFunction.FormatData(lstrPhoneNumber, "{0:(###) ###-####}");
                        aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                        string lstrWebLink = string.Empty;
                        lstrWebLink = Convert.ToString(dr["DATA3"]);
                        lobjField = new utlBookmarkFieldInfo();
                        lobjField.istrName = "stdCPOR_Web";
                        lobjField.istrValue = lstrWebLink;
                        aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
                    }

                    if ((dr["CODE_VALUE"]).ToString() == busConstant.DCOR)
                    {
                        string lstrEmpower = string.Empty;
                        lstrEmpower = Convert.ToString(dr["DATA1"]);
                        lobjField = new utlBookmarkFieldInfo();
                        lobjField.istrName = "stdDCOR_Name";
                        lobjField.istrValue = lstrEmpower;
                        aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                        string lstrPhoneNumber = string.Empty;
                        lstrPhoneNumber = Convert.ToString(dr["DATA2"]);
                        lobjField = new utlBookmarkFieldInfo();
                        lobjField.istrName = "stdDCOR_Phone";
                        lobjField.istrValue = HelperFunction.FormatData(lstrPhoneNumber, "{0:(###) ###-####}");
                        aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

                        string lstrWebLink = string.Empty;
                        lstrWebLink = Convert.ToString(dr["DATA3"]);
                        lobjField = new utlBookmarkFieldInfo();
                        lobjField.istrName = "stdDCOR_Web";
                        lobjField.istrValue = lstrWebLink;
                        aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
                    }
                }
            }
            if (aobjCorrInfo.istrTemplateName == "APP-7403")
            {
                DateTime ldtDateOfDeath = new DateTime();
                foreach (var item in aobjCorrInfo.icolBookmarkQueryInfo)
                {
                    if (item.istrName == "MonthYearDOD" && !string.IsNullOrEmpty(item.istrValue))
                    {
                        int intMonthNoOfDeath = Convert.ToInt32(item.istrValue.Split("/")[0]);
                        int intYearOfDeath = Convert.ToInt32(item.istrValue.Split("/")[1]);
                        ldtDateOfDeath = new DateTime(intYearOfDeath, intMonthNoOfDeath, 1);
                        item.istrSelectedValues = ldtDateOfDeath.ToString("MMMM") + " " + ldtDateOfDeath.ToString("yyyy");
                    }
                }

                foreach (var item in aobjCorrInfo.icolBookmarkFieldInfo)
                {
                    if (item.istrName == "MonthYearDODNextMonth")
                    {
                        ldtDateOfDeath = ldtDateOfDeath.AddMonths(1);
                        item.istrValue = ldtDateOfDeath.ToString("MMMM") + " " + ldtDateOfDeath.ToString("yyyy");
                    }

                    if (item.istrName == "GeneratedDatePlusSixtyDays")
                    {
                        DateTime ldtDateToday = DateTime.Now;
                        ldtDateToday = ldtDateToday.AddDays(60);
                        item.istrValue = ldtDateToday.ToString("MMMM dd, yyyy");
                    }
                }
            }
            //PIR 24841
            if (aobjCorrInfo.istrTemplateName == "PER-0977")
            {
                foreach (var  item in aobjCorrInfo.icolBookmarkQueryInfo)
                {
                    if (item.istrName == "Month/Year" && !string.IsNullOrEmpty(item.istrValue))
                    {
                        DateTime lstrMonthYear = Convert.ToDateTime(item.istrValue);
                        item.istrSelectedValues = lstrMonthYear.ToString("MMMM yyyy");
                    }
                }
            }
            //PIR 25920 
            if (aobjCorrInfo.istrTemplateName == "ENR-5306")
            {
                foreach (var item in aobjCorrInfo.icolBookmarkFieldInfo)
                {
                    if (item.istrName == "DateofFirstNotice" && !string.IsNullOrEmpty(item.istrValue))
                    {
                        DateTime lstrMonthYear = Convert.ToDateTime(item.istrValue);
                        item.istrValue = lstrMonthYear.ToString("MMMM dd, yyyy");
                    }
                }
            }
            //PIR 25944 
            if (aobjCorrInfo.istrTemplateName == "SFN-51702" || aobjCorrInfo.istrTemplateName == "SFN-52254" || aobjCorrInfo.istrTemplateName == "SFN-53879")
            {
                foreach (var item in aobjCorrInfo.icolBookmarkChldTemplateInfo)
                {
                    if (item.istrChildTemplateName == "PER-0106")
                    {
                        foreach (var bookmark in item.icolBookmarkFieldInfo)
                        {
                            bookmark.istrName = "stdMbrPERSLinkID";
                            bookmark.istrValue = aobjPerson.icdoPerson.person_id.ToString();
                        }
                    }
                }
            }
            //PIR 26256 Get Pay Period Date Year
            if (aobjCorrInfo.istrTemplateName == "ENR-5063")
            {
                foreach (var item in aobjCorrInfo.icolBookmarkFieldInfo)
                {
                    if (item.istrName == "PlanYear" && !string.IsNullOrEmpty(item.istrValue))
                    {
                        int intPayPeriodMonth = Convert.ToInt32(item.istrValue.Split("/")[0]);
                        int intPayPeriodYear = Convert.ToInt32(item.istrValue.Split("/")[1]);
                        item.istrValue = intPayPeriodYear.ToString();
                    }
                    else if (item.istrName == "ReportMonthSpelledOut" && !string.IsNullOrEmpty(item.istrValue))
                    {
                        DateTime lstrMonthYear = Convert.ToDateTime(item.istrValue);
                        item.istrValue = lstrMonthYear.ToString("MMMM yyyy");
                    }
                }
            }
            if (aobjCorrInfo.istrTemplateName == "PER-0354")
            {
                foreach (utlBookmarkFieldInfo lultFieldinfo in aobjCorrInfo.icolBookmarkFieldInfo)
                {

                    if (lultFieldinfo.istrName.ToString() == "HealthCancel")
                    {
                        lultFieldinfo.istrValue = DateTime.Now.GetFirstDayofCurrentMonth().ToString("MMMM dd, yyyy");
                    }
                    if (lultFieldinfo.istrName.ToString() == "MedicarePartDCancel")
                    {
                        lultFieldinfo.istrValue = DateTime.Now.AddDays(21).GetFirstDayofNextMonth().ToString("MMMM dd, yyyy");
                    }
                }
                foreach (var item in aobjCorrInfo.icolBookmarkQueryInfo)
                {
                    if (item.istrName.ToString() == "DueDate")
                    {
                        if (item.istrValue.IsNotNullOrEmpty())
                        {
                            DateTime lstrMonthYear = Convert.ToDateTime(item.istrValue);
                            item.istrSelectedValues = lstrMonthYear.ToString("MMMM dd, yyyy");
                        }
                    }
                }
            }
        }
                

        public static void LoadOrgContactPrimaryAddressBookMark(busOrganization aobjOrganization, utlCorresPondenceInfo aobjCorrInfo)
        {
            //we must load the ibusOrgContact object from the generated correpondence form (eg: busContactTicket) , if the form has Org Contact Object.
            if (aobjOrganization.ibusOrgContact != null)
            {
                aobjCorrInfo.iintOrgContactID = aobjOrganization.ibusOrgContact.icdoOrgContact.org_contact_id;
                //Load the Org Contact Primary Address First
                if (aobjOrganization.ibusOrgContact.ibusOrgAndContactPrimaryAddress == null)
                    aobjOrganization.ibusOrgContact.LoadOrgAndContactPrimaryAddress();
                if (aobjOrganization.ibusOrgContact.ibusContact == null)
                    aobjOrganization.ibusOrgContact.LoadContact();
                aobjOrganization.ibusContact = aobjOrganization.ibusOrgContact.ibusContact;
                if (aobjOrganization.ibusOrgContact.ibusOrgAndContactPrimaryAddress.icdoOrgContactAddress.contact_org_address_id > 0)
                {
                    aobjOrganization.ibusOrgContactPrimaryAddress = aobjOrganization.ibusOrgContact.ibusOrgAndContactPrimaryAddress;
                }
                else //If not Org Contact Primary Address, go with Org Primiary Address / Contact Primary Address
                {
                    aobjOrganization.LoadOrgContactPrimaryAddressByContact(aobjOrganization.ibusOrgContact.icdoOrgContact.contact_id);
                }
            }
            else
            {
                string lstrPlanID = busBase.GetBookMarkValue("PlanId", aobjCorrInfo.icolBookmarkFieldInfo);
                if (String.IsNullOrEmpty(lstrPlanID))
                    lstrPlanID = "0";

                aobjCorrInfo.iintOrgContactID = 0;
                aobjOrganization.LoadOrgContactByRoleAndPlan(aobjCorrInfo.istrContactRole, Convert.ToInt32(lstrPlanID));
            }

            //If still Primary Address Object is Null, Assign the Empty Instance
            if (aobjOrganization.ibusOrgContactPrimaryAddress == null)
            {
                aobjCorrInfo.iintOrgContactID = 0;
                aobjOrganization.ibusOrgContactPrimaryAddress = new busOrgContactAddress();
                aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress = new cdoOrgContactAddress();
            }
            else if (aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress == null)
            {
                aobjCorrInfo.iintOrgContactID = 0;
                aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress = new cdoOrgContactAddress();
            }

            utlBookmarkFieldInfo lobjField;
            busContact lobjContact = new busContact();
            lobjContact.icdoContact = new cdoContact();
            busOrgContact lobjOrgContact = new busOrgContact();
            lobjOrgContact.icdoOrgContact = new cdoOrgContact();

            if (aobjOrganization.ibusContact != null)
                lobjContact = aobjOrganization.ibusContact;

            if (aobjOrganization.ibusOrgContact != null)
                lobjOrgContact = aobjOrganization.ibusOrgContact;

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOrgName";
            lobjField.istrValue = aobjOrganization.icdoOrganization.org_name ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            //PIR-413
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOrgCodeId";
            lobjField.istrValue = aobjOrganization.icdoOrganization.org_code ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCName";
            lobjField.istrValue = lobjContact.full_name.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCSalutation";
            lobjField.istrValue = busGlobalFunctions.ToTitleCase(lobjContact.full_name);
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCFirstName";
            lobjField.istrValue = lobjContact.icdoContact.first_name;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCLastName";
            lobjField.istrValue = lobjContact.icdoContact.last_name;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCPrefix";
            lobjField.istrValue = lobjContact.icdoContact.name_prefix_description;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCAdrCorStreet1";
            lobjField.istrValue = aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.addr_line_1 ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);


            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCAdrCorStreet2";
            lobjField.istrValue = aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.addr_line_2 ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCAdrCorCity";
            lobjField.istrValue = aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.city ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCAdrCorState";
            lobjField.istrValue = aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.state_value ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCAdrCorStateDesc";
            lobjField.istrValue = aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.state_description ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCAdrCorZip";
            lobjField.istrValue = aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.zip_code ?? String.Empty;
            lobjField.istrValue = lobjField.istrValue.ToUpper();
            if ((aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.zip_4_code != null) &&
                (aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.zip_4_code.Trim() != ""))
            {
                lobjField.istrValue += "-" + aobjOrganization.ibusOrgContactPrimaryAddress.icdoOrgContactAddress.zip_4_code ?? String.Empty;
                lobjField.istrValue = lobjField.istrValue.ToUpper();
            }
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCPhoneNo";
            lobjField.istrValue = HelperFunction.FormatData(lobjContact.icdoContact.phone_no, "{0:(###) ###-####}");
            if (lobjContact.icdoContact.phone_no != null)
                lobjField.istrValue += " (" + lobjContact.icdoContact.phone_no + ")";
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdOCFaxNo";
            lobjField.istrValue = HelperFunction.FormatData(lobjContact.icdoContact.fax_no, "{0:(###) ###-####}");
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);

            //PIR 26725
            if (aobjCorrInfo.istrTemplateName == "ORG-2054")
            {
                foreach (var item in aobjCorrInfo.icolBookmarkQueryInfo)
                {
                    if (item.istrName == "DateOfLastReportReceived" && item.istrValue.IsNotNullOrEmpty())
                    {
                        if (item.istrValue != busConstant.DateOfLastReportReceived)
                        {
                            DateTime lstrMonthYear = Convert.ToDateTime(item.istrValue);
                            item.istrSelectedValues = lstrMonthYear.ToString("MMMM dd, yyyy");
                        }
                    }
                }
            }
        }

        private static void LoadLastBookmarks(utlCorresPondenceInfo aobjCorrInfo)
        {
            utlBookmarkFieldInfo lobjField;
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdTrackingNo";
            lobjField.istrValue = aobjCorrInfo.istrGeneratedFileName;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
        }

        private string istrRBUrl;
        private string iabsRptDefPath;
        private string iabsRptGenPath;


        //Call this Method outside the Loop (From Caller) for Optimization

        public void InitializeReportBuilder(string astrReportGNPath)
        {
            iabsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            iabsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(astrReportGNPath);
        }

        public string CreateReport(string astrReportName, System.Data.DataTable adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportPath)
        {
            //DBFunction.StoreProcessLog(0, "Test", "INFO", "ReportName is " + astrReportName, iobjPassInfo.iintUserSerialID.ToString(), iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            InitializeReportBuilder(astrReportGNPath);

            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init

            irptDocument.Load(iabsRptDefPath + astrReportName);
            irptDocument.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            string lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            irptDocument.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName);
            irptDocument.Close();
            irptDocument.Dispose();
            return lstrReportFullName;
        }

        public string CreateReportWithGivenName(string astrReportName, System.Data.DataTable adstResult, string astrReportNameWithTimeStamp, string astrReportGNPath = busConstant.ReportPath)
        {
            string lstrResult = string.Empty;
            InitializeReportBuilder(astrReportGNPath);

            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptDocument.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptDocument.SetDataSource(adstResult);
            string lstrReportFullName = iabsRptGenPath + astrReportNameWithTimeStamp + ".pdf";
            irptDocument.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName);
            irptDocument.Close();
            irptDocument.Dispose();
            return lstrResult;
        }

        public string CreateReport(string astrReportName, DataSet adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportPath)
        {
            InitializeReportBuilder(astrReportGNPath);
            string lstrReportFullName = string.Empty;
            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptDocument.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptDocument.SetDataSource((DataSet)adstResult);

            lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            irptDocument.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName + ".pdf");
            irptDocument.Close();
            irptDocument.Dispose();
            return lstrReportFullName;
        }

        public byte[] CreateDynamicReport(string astrReportName, DataSet adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportPath, Hashtable hstParam = null)
        {
            InitializeReportBuilder(astrReportGNPath);

            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptDocument.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptDocument.SetDataSource(adstResult);

            if (hstParam != null)
            {
                if (astrReportName == "rptMissingContributions.rpt")
                {
                    irptDocument.SetParameterValue("txtReportRunDate", (string)hstParam["txtReportRunDate"]);
                }
                else if (astrReportName == "rptRetirementContributionsAuditConfirmation.rpt") //PIR 24994
                {
                    irptDocument.SetParameterValue("FromDate", (string)hstParam["START_DATE"]);
                    irptDocument.SetParameterValue("ToDate", (string)hstParam["END_DATE"]);
                    irptDocument.SetParameterValue("ReportRunDate", (string)hstParam["REPORT_RUN_DATE"]);
                }
                else
                {
                    if (astrReportName == "rptBenefitEnrollment.rpt")
                    {
                        irptDocument.SetParameterValue("FromDate", (DateTime)hstParam["FromDate"]);
                        irptDocument.SetParameterValue("ToDate", (DateTime)hstParam["ToDate"]);
                        irptDocument.SetParameterValue("OrgCodeAndName", (string)hstParam["OrgCodeAndName"]);
                        irptDocument.SetParameterValue("ReportName", busConstant.AnnualEnrollmentSummaryReportTitle); // PIR 23729
                    }
                    else
                    {
                        irptDocument.SetParameterValue("OrgName", (string)hstParam["OrgName"]);
                        irptDocument.SetParameterValue("OrgCode", (string)hstParam["OrgCode"]);
                    }
                }
            }
            string lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";

            //Fw Upgrade - used IO Stream instead of memoryStream because it is not support in crystal report newer version
            //MemoryStream lmstReport = (MemoryStream)irptDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            System.IO.Stream oStream = irptDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            byte[] byteArray = null;
            byteArray = new byte[oStream.Length];
            oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));

            irptDocument.Close();
            irptDocument.Dispose();
            //return lmstReport.ToArray();
            return byteArray;

        }
        //FW Upgrade :: wfmDefault.aspx.cs code conversion for "btn_OpenPDF" method to get only path of the file(instead of byte[]) to display in browser for htxMSS1099RMaintenance.Xml page
        public string CreateDynamicReportPath(string astrReportName, DataSet adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportPath, Hashtable hstParam = null)
        {
            InitializeReportBuilder(astrReportGNPath);

            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptDocument.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptDocument.SetDataSource(adstResult);

            if (hstParam != null)
            {
                if (astrReportName == "rptMissingContributions.rpt")
                {
                    irptDocument.SetParameterValue("txtReportRunDate", (string)hstParam["txtReportRunDate"]);
                }
                else
                {
                    irptDocument.SetParameterValue("OrgName", (string)hstParam["OrgName"]);
                    irptDocument.SetParameterValue("OrgCode", (string)hstParam["OrgCode"]);
                }
            }

            string lstrReportCurrentDateTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + lstrReportCurrentDateTime + ".pdf";

            //System.IO.Stream oStream = irptDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            irptDocument.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, lstrReportFullName);

            irptDocument.Close();
            irptDocument.Dispose();

            return AppServerReportPath(astrReportGNPath) + astrPrefix + astrReportName + "_" + lstrReportCurrentDateTime + ".pdf";
        }

        private string AppServerReportPath(string astrReportGNPath)
        {
            System.Data.DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = '"+ astrReportGNPath + "'");
            return ldtbPathData.Rows[0]["path_value"].ToString();
        }

        public byte[] CreateDynamicWordReport(string astrReportName, DataSet adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportPath)
        {
            InitializeReportBuilder(astrReportGNPath);
            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptDocument.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control

            irptDocument.SetDataSource(adstResult);

            string lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" +
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            //Fw Upgrade - used IO Stream instead of memoryStream because it is not support in crystal report newer version
            //MemoryStream lmstReport = (MemoryStream)irptDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);

            System.IO.Stream oStream = irptDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);
            byte[] byteArray = null;
            byteArray = new byte[oStream.Length];
            oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));


            irptDocument.Close();
            irptDocument.Dispose();
            return byteArray;
        }

        public byte[] CreateDynamicExcelReport(string astrReportName, DataSet adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportPath, Hashtable hstParam = null)
        {
            InitializeReportBuilder(astrReportGNPath);
            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptDocument.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptDocument.SetDataSource(adstResult);

            if (hstParam != null)
            {
                if (astrReportName == "rptMissingContributions.rpt")
                {
                    irptDocument.SetParameterValue("txtReportRunDate", (string)hstParam["txtReportRunDate"]);
                }
                else if (astrReportName == "rptBenefitEnrollment.rpt")
                {
                    irptDocument.SetParameterValue("FromDate", (DateTime)hstParam["FromDate"]);
                    irptDocument.SetParameterValue("ToDate", (DateTime)hstParam["ToDate"]);
                    irptDocument.SetParameterValue("OrgCodeAndName", (string)hstParam["OrgCodeAndName"]);
                    irptDocument.SetParameterValue("ReportName", busConstant.AnnualEnrollmentSummaryReportTitle); // PIR 23729
                }
                else
                {
                    irptDocument.SetParameterValue("OrgName", (string)hstParam["OrgName"]);
                    irptDocument.SetParameterValue("OrgCode", (string)hstParam["OrgCode"]);
                }
            }

            string lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";
            //Fw Upgrade - used IO Stream instead of memoryStream because it is not support in crystal report newer version
            //MemoryStream lmstReport = (MemoryStream)irptDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);

            System.IO.Stream oStream = irptDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            byte[] byteArray = null;
            byteArray = new byte[oStream.Length];
            oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));

            irptDocument.Close();
            irptDocument.Dispose();
            return byteArray;
        }

        // Initialize the report documnet. This event removes any databse logon information 
        // saved in the report. The call to Load the report in the above function fires this event.
        private void OnReportDocInit(object sender, System.EventArgs e)
        {
            irptDocument.SetDatabaseLogon("", "");
        }

        public static void LoadStandardConstantBookmarks(utlCorresPondenceInfo aobjCorrInfo)
        {
            utlBookmarkFieldInfo lobjField;
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;

            System.Data.DataTable ldtbNDPERSPhoneNumberlist = lobjPassInfo.isrvDBCache.GetCodeValues(52);

            if ((ldtbNDPERSPhoneNumberlist != null) && (ldtbNDPERSPhoneNumberlist.Rows.Count > 0))
            {
                foreach (DataRow dr in ldtbNDPERSPhoneNumberlist.Rows)
                {
                    if ((dr["CODE_VALUE"]).ToString() == "NDPH")
                    {
                        string lstrPhoneNumber = string.Empty;
                        lstrPhoneNumber = dr["DATA1"].ToString();
                        lobjField = new utlBookmarkFieldInfo();
                        lobjField.istrName = "stdNDPERSPhoneNumber";
                        lobjField.istrValue = HelperFunction.FormatData(lstrPhoneNumber, "{0:(###) ###-####}");
                        aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
                    }
                    if (dr["CODE_VALUE"].ToString() == "NDTP")
                    {
                        string lstrPhoneNumber = string.Empty;
                        lstrPhoneNumber = dr["DATA1"].ToString();
                        lobjField = new utlBookmarkFieldInfo();
                        lobjField.istrName = "stdNDPERSTollFreePhoneNumber";
                        lobjField.istrValue = HelperFunction.FormatData(lstrPhoneNumber, "{0:(###) ###-####}");
                        aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
                    }
                }
            }

            //PIR - 1863 1865
            //display the user full name 
            //first last name                        
            busUser lobjUser = busGlobalFunctions.GetUserObjectFromUserSerialId(lobjPassInfo.iintUserSerialID);
            lobjField = new utlBookmarkFieldInfo();
            lobjField.istrName = "stdUserFullName";
            lobjField.istrValue = lobjUser.icdoUser.first_name + " " + lobjUser.icdoUser.last_name;
            aobjCorrInfo.icolBookmarkFieldInfo.Add(lobjField);
        }

        public utlWhereClause GetWhereClause(object aobjValue1, object aobjValue2,
           string astrFieldName, string astrDataType, string astrOperator, string astrCondition, string astrQueryId)
        {
            utlWhereClause lobjWhereClause = new utlWhereClause();
            lobjWhereClause.iobjValue1 = aobjValue1;
            lobjWhereClause.iobjValue2 = aobjValue2;
            lobjWhereClause.istrQueryId = astrQueryId;
            lobjWhereClause.istrFieldName = astrFieldName;
            lobjWhereClause.istrDataType = astrDataType;
            lobjWhereClause.istrOperator = astrOperator;
            lobjWhereClause.istrCondition = astrCondition;
            return lobjWhereClause;
        }

        public string CreateExcelReport(string astrReportName, System.Data.DataTable adstResult, string astrPrefix, string astrExcelGNPath)
        {
            string lstrReportFullName = string.Empty;

            InitializeReportBuilder(astrExcelGNPath);

            if (astrReportName.Contains("1099R"))
            {
                // PIR 8984 - Excel report generate function
                lstrReportFullName = BuildExcelReport(iabsRptGenPath, astrReportName, adstResult, astrPrefix, astrExcelGNPath);
            }
            else
            {
                irptDocument = new ReportDocument();
                irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init

                irptDocument.Load(iabsRptDefPath + astrReportName);
                irptDocument.SetDataSource(adstResult);             // gets the data and bind to the report doc control
                lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                irptDocument.ExportToDisk(ExportFormatType.Excel, lstrReportFullName + ".xls");
                irptDocument.Close();
                irptDocument.Dispose();
            }

            return lstrReportFullName;
        }

        //PIR-10808 PDF Reports converted into Excel format.To pass Dataset as parameter.
        public string CreateExcelReport(string astrReportName, System.Data.DataSet adstResult, string astrPrefix, string astrExcelGNPath)
        {
            string lstrReportFullName = string.Empty;
            InitializeReportBuilder(astrExcelGNPath);
            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init
            irptDocument.Load(iabsRptDefPath + astrReportName);
            irptDocument.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            irptDocument.ExportToDisk(ExportFormatType.Excel, lstrReportFullName + ".xls");
            irptDocument.Close();
            irptDocument.Dispose();
            return lstrReportFullName;
        }

        protected string BuildExcelReport(string astrRptGenPath, string astrReportName, System.Data.DataTable adstResult, string astrPrefix, string astrExcelGNPath)
        {
            string lstrResult = string.Empty;
            string reportType = string.Empty;
            Hashtable hashTableRefund = new Hashtable();
            Hashtable hashTableAnnuitant = new Hashtable();

            if (astrReportName.Contains("Refund"))
            {
                reportType = "Refund";
            }
            else
            {
                reportType = "Annuitant";
            }


            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workBook = excel.Workbooks.Add();
            Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)workBook.ActiveSheet;

            try
            {
                if (reportType == "Refund")
                {
                    hashTableRefund["ACCT_NUM"] = "Account Number";
                    hashTableRefund["PERSON_ID"] = "Person ID";
                    hashTableRefund["PAYEE_NAME"] = "Payee Name";
                    hashTableRefund["PAYEE_STATUS"] = "Payee Status";
                    hashTableRefund["PLAN_NAME"] = "Plan";
                    hashTableRefund["BENEFIT_OPTION"] = "Benefit Option";
                    hashTableRefund["DIST_CD"] = "Dist CD";
                    hashTableRefund["TOTAL_PAID"] = "Total Paid";
                    hashTableRefund["CAPITAL_GAINS"] = "Capital Gains";
                    hashTableRefund["NON_TAX"] = "Non Taxable Amount";
                    hashTableRefund["TAXABLE"] = "Taxable Amount";
                    hashTableRefund["NON_TAX_ROLLOVER"] = "Non Taxable Rollover Amount"; // PIR 10009
                    hashTableRefund["FED_TAX"] = "Fed Tax";
                    hashTableRefund["STATE_TAX"] = "State Tax";
                    hashTableRefund["CHECK_NUM"] = "Check #";
                    hashTableRefund["MONTH_YEAR"] = "Month Year";
                    hashTableRefund["DIST_PER"] = "Distribution %";
                    hashTableRefund["CHECK_DATE"] = "Check Date";
                    hashTableRefund["CHECK_AMOUNT"] = "Check Amount";
                }
                else
                {
                    hashTableAnnuitant["ACCT_NUM"] = "Account Number";
                    hashTableAnnuitant["PERSON_ID"] = "Person ID";
                    hashTableAnnuitant["PAYEE_NAME"] = "Payee Name";
                    hashTableAnnuitant["PAYEE_STATUS"] = "Payee Status";
                    hashTableAnnuitant["PLAN_NAME"] = "Plan";
                    hashTableAnnuitant["BENEFIT_OPTION"] = "Benefit Option";
                    hashTableAnnuitant["DIST_CD"] = "Dist CD";
                    hashTableAnnuitant["DIST_PER"] = "Distribution %";
                    hashTableAnnuitant["CAPITAL_GAINS"] = "Capital Gains";
                    hashTableAnnuitant["PAID_YTD"] = "Paid YTD";
                    hashTableAnnuitant["TAXABLE_YTD"] = "Taxable YTD";
                    hashTableAnnuitant["NON_TAXABLE_YTD"] = "Non Taxable YTD";
                    hashTableAnnuitant["STATE_TAX_YTD"] = "State Tax YTD";
                    hashTableAnnuitant["FED_TAX_YTD"] = "Federal Tax YTD";
                }


                int i = 0;
                int colInd = 0, colIndCapital = 0, colIndNonTax = 0, colIndTax = 0, colIndFed = 0, colIndState = 0, colIndCheck = 0;
                int colIndPaidYTD = 0, colIndTaxableYTD = 0, colIndNonTaxYTD = 0, colIndStateTaxYTD = 0, colIndFedTaxYTD = 0;
                decimal totalPaid = 0.00M, capitalGainTotal = 0.00M, nonTaxTotal = 0.00M, taxableTotal = 0.00M, fedTaxTotal = 0.00M, stateTaxTotal = 0.00M, checkAmtTotal = 0.00M;
                decimal nonTaxRolloverTotal = 0.00M; // PIR 10009
                int colIndNonTaxRollover = 0; // PIR 10009
                decimal totalPaidYTD = 0.00M, totalTaxableYTD = 0.00M, totalNonTaxYTD = 0.00M, totalStateTaxYTD = 0.00M, totalFedTaxYTD = 0.00M;

                // Merge first row cells and set Header title
                sheet.Range[sheet.Cells[i + 1, 1], sheet.Cells[i + 1, adstResult.Columns.Count]].Merge();
                sheet.Range[sheet.Cells[i + 1, 1], sheet.Cells[i + 1, adstResult.Columns.Count]].Value = "Monthly " + reportType + " 1099R Report";

                // set font and alignment of header row
                Microsoft.Office.Interop.Excel.Range cellHeader = sheet.Cells[i + 1, 1];
                cellHeader.Font.Bold = true;
                cellHeader.Font.Size = 14;
                cellHeader.HorizontalAlignment = HorizontalAlign.Right;

                i++;
                sheet.Range[sheet.Cells[i + 1, 1], sheet.Cells[i + 1, adstResult.Columns.Count]].Merge(); // merge blank row cells

                i++;

                for (int j = 0; j < adstResult.Columns.Count; j++)
                {
                    if (reportType == "Refund")
                    {
                        sheet.Cells[i + 1, j + 1] = hashTableRefund[adstResult.Columns[j].ToString()];
                    }
                    else
                    {
                        sheet.Cells[i + 1, j + 1] = hashTableAnnuitant[adstResult.Columns[j].ToString()];
                    }
                    Microsoft.Office.Interop.Excel.Range cell = sheet.Cells[i + 1, j + 1];
                    cell.Font.Bold = true;
                }

                foreach (DataRow row in adstResult.Rows)
                {
                    for (int j = 0; j < row.ItemArray.Length; j++)
                    {
                        sheet.Cells[i + 2, j + 1] = row[j].ToString().Trim();
                        switch (adstResult.Columns[j].ToString())
                        {
                            case "TOTAL_PAID":
                                totalPaid = totalPaid + Convert.ToDecimal(row[j].ToString());
                                colInd = j + 1;
                                break;
                            case "CAPITAL_GAINS":
                                capitalGainTotal = capitalGainTotal + Convert.ToDecimal(row[j].ToString());
                                colIndCapital = j + 1;
                                break;
                            case "NON_TAX":
                                nonTaxTotal = nonTaxTotal + Convert.ToDecimal(row[j].ToString());
                                colIndNonTax = j + 1;
                                break;
                            case "TAXABLE":
                                taxableTotal = taxableTotal + Convert.ToDecimal(row[j].ToString());
                                colIndTax = j + 1;
                                break;
                            case "FED_TAX":
                                fedTaxTotal = fedTaxTotal + Convert.ToDecimal(row[j].ToString());
                                colIndFed = j + 1;
                                break;
                            case "STATE_TAX":
                                stateTaxTotal = stateTaxTotal + Convert.ToDecimal(row[j].ToString());
                                colIndState = j + 1;
                                break;
                            case "CHECK_AMOUNT":
                                checkAmtTotal = checkAmtTotal + Convert.ToDecimal(row[j].ToString());
                                colIndCheck = j + 1;
                                break;
                            case "PAID_YTD":
                                totalPaidYTD = totalPaidYTD + Convert.ToDecimal(row[j].ToString());
                                colIndPaidYTD = j + 1;
                                break;
                            case "TAXABLE_YTD":
                                totalTaxableYTD = totalTaxableYTD + Convert.ToDecimal(row[j].ToString());
                                colIndTaxableYTD = j + 1;
                                break;
                            case "NON_TAXABLE_YTD":
                                totalNonTaxYTD = totalNonTaxYTD + Convert.ToDecimal(row[j].ToString());
                                colIndNonTaxYTD = j + 1;
                                break;
                            case "STATE_TAX_YTD":
                                totalStateTaxYTD = totalStateTaxYTD + Convert.ToDecimal(row[j].ToString());
                                colIndStateTaxYTD = j + 1;
                                break;
                            case "FED_TAX_YTD":
                                totalFedTaxYTD = totalFedTaxYTD + Convert.ToDecimal(row[j].ToString());
                                colIndFedTaxYTD = j + 1;
                                break;
                            //PIR 10009
                            case "NON_TAX_ROLLOVER":
                                nonTaxRolloverTotal = nonTaxRolloverTotal + Convert.ToDecimal(row[j].ToString());
                                colIndNonTaxRollover = j + 1;
                                break;
                            default:
                                break;
                        }
                    }
                    i++;
                }

                sheet.Cells[i + 3, 1] = "Total:";
                if (reportType == "Refund")
                {
                    sheet.Cells[i + 3, colInd] = totalPaid.ToString().Trim();
                    sheet.Cells[i + 3, colIndCapital] = capitalGainTotal.ToString().Trim();
                    sheet.Cells[i + 3, colIndNonTax] = nonTaxTotal.ToString().Trim();
                    sheet.Cells[i + 3, colIndTax] = taxableTotal.ToString().Trim();
                    sheet.Cells[i + 3, colIndFed] = fedTaxTotal.ToString().Trim();
                    sheet.Cells[i + 3, colIndState] = stateTaxTotal.ToString().Trim();
                    sheet.Cells[i + 3, colIndCheck] = checkAmtTotal.ToString().Trim();
                    sheet.Cells[i + 3, colIndNonTaxRollover] = nonTaxRolloverTotal.ToString().Trim(); // PIR 10009
                }

                if (reportType == "Annuitant")
                {
                    sheet.Cells[i + 3, colIndPaidYTD] = totalPaidYTD.ToString().Trim();
                    sheet.Cells[i + 3, colIndTaxableYTD] = totalTaxableYTD.ToString().Trim();
                    sheet.Cells[i + 3, colIndNonTaxYTD] = totalNonTaxYTD.ToString().Trim();
                    sheet.Cells[i + 3, colIndStateTaxYTD] = totalStateTaxYTD.ToString().Trim();
                    sheet.Cells[i + 3, colIndFedTaxYTD] = totalFedTaxYTD.ToString().Trim();
                    sheet.Cells[i + 3, colIndCapital] = capitalGainTotal.ToString().Trim();
                }



                Microsoft.Office.Interop.Excel.Range cellTotal = sheet.Cells[i + 3, 1];
                cellTotal.Font.Bold = true;

                // Columns autofit
                sheet.Cells.Columns.AutoFit();

                // Currency formatting
                if (reportType == "Refund")
                {
                    sheet.Range[sheet.Cells[4, colInd], sheet.Cells[adstResult.Rows.Count + 5, colInd]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndCapital], sheet.Cells[adstResult.Rows.Count + 5, colIndCapital]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndNonTax], sheet.Cells[adstResult.Rows.Count + 5, colIndNonTax]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndTax], sheet.Cells[adstResult.Rows.Count + 5, colIndTax]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndFed], sheet.Cells[adstResult.Rows.Count + 5, colIndFed]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndState], sheet.Cells[adstResult.Rows.Count + 5, colIndState]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndCheck], sheet.Cells[adstResult.Rows.Count + 5, colIndCheck]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndNonTaxRollover], sheet.Cells[adstResult.Rows.Count + 5, colIndNonTaxRollover]].NumberFormat = "$#,##0.00"; // PIR 10009
                }
                if (reportType == "Annuitant")
                {
                    sheet.Range[sheet.Cells[4, colIndPaidYTD], sheet.Cells[adstResult.Rows.Count + 5, colIndPaidYTD]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndCapital], sheet.Cells[adstResult.Rows.Count + 5, colIndCapital]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndTaxableYTD], sheet.Cells[adstResult.Rows.Count + 5, colIndTaxableYTD]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndNonTaxYTD], sheet.Cells[adstResult.Rows.Count + 5, colIndNonTaxYTD]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndStateTaxYTD], sheet.Cells[adstResult.Rows.Count + 5, colIndStateTaxYTD]].NumberFormat = "$#,##0.00";
                    sheet.Range[sheet.Cells[4, colIndFedTaxYTD], sheet.Cells[adstResult.Rows.Count + 5, colIndFedTaxYTD]].NumberFormat = "$#,##0.00";
                }


                lstrResult = astrRptGenPath + astrPrefix + astrReportName + astrPrefix + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xls";

                workBook.SaveAs(lstrResult);
            }
            finally
            {
                workBook.Close();
                excel = null;
            }

            return lstrResult;
        }

        public byte[] CreateReportOffline(DataSet adstResult, string astrReportFullName, ReportModel aclsReportModel)
        {
            byte[] lbyteFile = null;
            irptDocument = new ReportDocument();
            
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init
            irptDocument.Load(astrReportFullName);
            irptDocument.SetDataSource(adstResult);             // gets the data and bind to the report doc control
            ExecuteReport(irptDocument, aclsReportModel);
            //irptDocument.ExportToDisk(ExportFormatType.PortableDocFormat, astrReportFullName);
            System.IO.Stream oStream = irptDocument.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            byte[] byteArray = null;
            byteArray = new byte[oStream.Length];
            oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length - 1));
            lbyteFile = byteArray;
            irptDocument.Close();
            irptDocument = null;
            return lbyteFile;
        }

        public void ExecuteReport(ReportDocument arptDocument, ReportModel aclsReportModel) //framework upgradation: Private to public
        {
            //Assigning the Parameter              
            string lstrReportName = aclsReportModel.ReportName;
            switch (lstrReportName)
            {
                case busConstant.ReportNamePaymentListingReport:
                    System.Data.DataTable ldtbCodevalue = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", "code_id=2515 and code_value='" + aclsReportModel.status + "'");
                    string lstrStatus = ldtbCodevalue.Rows.Count > 0 ? ldtbCodevalue.Rows[0]["Description"].ToString() : string.Empty;
                    arptDocument.SetParameterValue("txtPaymentStatus", lstrStatus);
                    arptDocument.SetParameterValue("txtStartDate", aclsReportModel.StartDate.ToShortDateString());
                    arptDocument.SetParameterValue("txtEndDate", (aclsReportModel.EndDate == DateTime.MinValue ? DateTime.Now : aclsReportModel.EndDate).ToShortDateString());
                    break;
                case busConstant.ReportNameListOfAppointments:
                    arptDocument.SetParameterValue("txtStartDate", aclsReportModel.start_date.ToShortDateString());
                    arptDocument.SetParameterValue("txtEndDate", aclsReportModel.end_date.ToShortDateString());
                    break;
                case busConstant.ReportNameMaritalStatusChangedRecords:
                    arptDocument.SetParameterValue("txtWeekStartDate", aclsReportModel.Week_Start_Date.ToShortDateString());
                    arptDocument.SetParameterValue("txtWeekEndDate", aclsReportModel.Week_End_Date.ToShortDateString());
                    break;
                case busConstant.ReportNamePensionCheckPaymentReport:
                    arptDocument.SetParameterValue("txtPAYMENTDATE", aclsReportModel.PAYMENTDATE.ToShortDateString());
                    break;
                case busConstant.ReportNamePensionPaymentHistory:
                    arptDocument.SetParameterValue("txtStartDate", aclsReportModel.StartDate.ToShortDateString());
                    arptDocument.SetParameterValue("txtEndDate", (aclsReportModel.EndDate == DateTime.MinValue ? DateTime.MaxValue : aclsReportModel.endTime).ToShortDateString());
                    break;
                case busConstant.ReportNameOverPaymentReport:
                    arptDocument.SetParameterValue("txtYear", aclsReportModel.APPROVAL_YEAR);
                    break;
                case busConstant.ReportNameSummaryreportofalladhocpaymentsforaMonth:
                    System.Data.DataTable ldtbSchCodevalue = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", "code_id=2501 and code_value='" + aclsReportModel.SCHEDULETYPE + "'");
                    arptDocument.SetParameterValue("txtStartDate", aclsReportModel.StartDate.ToShortDateString());
                    arptDocument.SetParameterValue("txtEndDate", aclsReportModel.EndDate.ToShortDateString());
                    arptDocument.SetParameterValue("txtScheduletype", (aclsReportModel.SCHEDULETYPE == "VNPM") ? "Vendor" : 
                                                    (ldtbSchCodevalue.IsNotNull() && ldtbSchCodevalue.Rows.Count > 0 && 
                                                    ldtbSchCodevalue.Rows[0]["Description"] != DBNull.Value) ? 
                                                    Convert.ToString(ldtbSchCodevalue.Rows[0]["Description"]) : string.Empty);
                    break;
                case busConstant.ReportNameMedicareSplitError:
                    arptDocument.SetParameterValue("txtcurrentdate", (aclsReportModel.currentdate == DateTime.MinValue ? DateTime.Now : aclsReportModel.currentdate).ToShortDateString());
                    break;
                case busConstant.ReportNameLeaveOfAbsence:
                case busConstant.ReportNameMissingRetirementEnrollment:
                    busOrganization lbusOrganization = new busOrganization();
                    string lstrOrgName = string.Empty;
                    string lstrOrgCode = string.Empty;
                    if (lbusOrganization.FindOrganization(Convert.ToInt32(aclsReportModel.ORG_ID)))
                    {
                        lstrOrgName = lbusOrganization.icdoOrganization.org_name;
                        lstrOrgCode = lbusOrganization.icdoOrganization.org_code;
                    }
                    arptDocument.SetParameterValue("OrgName", lstrOrgName);
                    arptDocument.SetParameterValue("OrgCode", lstrOrgCode);
                    break;
                default:
                    break;
            }
        }

        public string GetMessage(int aintMessageID)
        {
            string lstrMessage = string.Empty;
            lstrMessage = utlPassInfo.iobjPassInfo.isrvDBCache.GetMessageText(aintMessageID);

			if (!string.IsNullOrEmpty(lstrMessage))
            {
                lstrMessage = "" + lstrMessage + "";
            }
            return lstrMessage;
        }
   
        public string GetMessage(int aintMessageID, object[] aarrParam)
        {
            string lstrMessage = string.Empty;
            lstrMessage = utlPassInfo.iobjPassInfo.isrvDBCache.GetMessageText(aintMessageID);

            if (!string.IsNullOrEmpty(lstrMessage))
            {
                lstrMessage = String.Format(lstrMessage, aarrParam);
                lstrMessage = "" + lstrMessage + "";
            }

            return lstrMessage;
        }
     
        public string GetMessage(string astrMessage)
        {
            if (!string.IsNullOrEmpty(astrMessage))
            {
                astrMessage = "" + astrMessage + "";
            }


            return astrMessage;
        }

        public void SetMessage(int aintActualRecordCount, int aintMaxSearchCount = 100)
        {
            //Default value to be removed once Max Search Count is set correctly. Currently it is returning zero.

            if (aintActualRecordCount == 0)
            {
                this.iintMessageID = WorkflowConstants.MESSAGE_ID_2;
                this.istrMessage = GetMessage(WorkflowConstants.MESSAGE_ID_2);
            }
            else if (aintActualRecordCount > aintMaxSearchCount)
            {
                this.iintMessageID = WorkflowConstants.MESSAGE_ID_3;
                this.istrMessage = GetMessage(WorkflowConstants.MESSAGE_ID_3, new object[2] { aintActualRecordCount, aintMaxSearchCount });
            }
            else
            {
                this.iintMessageID = WorkflowConstants.MESSAGE_ID_1;
                this.istrMessage = GetMessage(WorkflowConstants.MESSAGE_ID_1, new object[1] { aintActualRecordCount });
            }
        }

    }
}
