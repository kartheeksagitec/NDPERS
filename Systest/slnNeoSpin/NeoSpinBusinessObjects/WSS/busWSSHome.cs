using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Collections.ObjectModel;
using System.Collections;
using NeoSpin.CustomDataObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
using Sagitec.CustomDataObjects;
using System.IO;
using System.Data;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busWSSHome : busExtendBase
    {
        public Collection<busContactTicket> iclbContactTicket { get; set; }
        public Collection<busSeminarSchedule> iclbSeminars { get; set; }

        /// <summary>
        /// Method to publish the correspondence and send mail to member or contact
        /// </summary>
        /// <param name="astrFileName"></param>
        /// <returns></returns>
        public ArrayList PublishToWSSAndEmail(string astrFileName)
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError;
            try
            {
                int lintTrackingID = Convert.ToInt32(astrFileName.Substring(astrFileName.LastIndexOf("-") + 1, 10));
                string lstrCorrsName = astrFileName.Substring(astrFileName.LastIndexOf("\\") + 1, astrFileName.LastIndexOf(".") - astrFileName.LastIndexOf("\\") - 1);//17968 Fixed UNC path issue
                string lstrDestinationPath = string.Empty;
                //Validation
                cdoCorTracking lcdoCorTracking = new cdoCorTracking();
                if (lcdoCorTracking.SelectRow(new object[1] { lintTrackingID }))
                {
                    busCorTemplates lobjCorTemplates = new busCorTemplates();
                    if (lobjCorTemplates.FindCorTemplates(lcdoCorTracking.template_id))
                    {
                        if (File.Exists(astrFileName))
                        {
                            lstrDestinationPath = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("CorrGenWSS");
                            lstrDestinationPath += astrFileName.Substring(astrFileName.LastIndexOf("\\") + 1, astrFileName.Length - astrFileName.LastIndexOf("\\") - 1);
                            File.Copy(astrFileName, lstrDestinationPath);
                        }
                        if (lcdoCorTracking.person_id > 0)
                        {
                            //PIR 8889 - messagetext should contain template_description not template_name
                            busWSSHelper.PublishMSSMessage(0, 0, string.Format(busConstant.WSSMessageForCorrs, lobjCorTemplates.icdoCorTemplates.template_desc), busConstant.WSSMessagePriorityHigh,
                            aintPersonID: lcdoCorTracking.person_id, astrCorrespondenceLink: lstrDestinationPath, aintTrackingID: lcdoCorTracking.tracking_id,
                            astrTemplateName: lobjCorTemplates.icdoCorTemplates.template_name);

                            busPerson lobjPerson = new busPerson();
                            lobjPerson.FindPerson(lcdoCorTracking.person_id);
                            if (!string.IsNullOrEmpty(lobjPerson.icdoPerson.email_address))
                            {
                                HelperFunction.SendMail(NeoSpin.Common.ApplicationSettings.Instance.WSSMailFrom.ToString(),
                                                                    lobjPerson.icdoPerson.email_address,
                                                                    NeoSpin.Common.ApplicationSettings.Instance.WSSMailSubject.ToString(),
                                                                    NeoSpin.Common.ApplicationSettings.Instance.WSSMailBody.ToString());
                                //HelperFunction.SendMail(AppSettingsHelper.Instance.Configuration["WSSMailFrom"].ToString(),
                                //                        lobjPerson.icdoPerson.email_address,
                                //                        AppSettingsHelper.Instance.Configuration["WSSMailSubject"].ToString(),
                                //                        AppSettingsHelper.Instance.Configuration["WSSMailBody"].ToString());
                            }
                        }
                        else if (lcdoCorTracking.org_id > 0)
                        {
                            busOrganization lobjOrganization = new busOrganization();
                            lobjOrganization.FindOrganization(lcdoCorTracking.org_id);
                            if (lobjOrganization.icdoOrganization.org_type_value == busConstant.OrganizationTypeEmployer)
                            {
                                //PIR 8889 - messagetext should contain template_description not template_name
                                string lstrPrioityValue = string.Empty;
                                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(1, iobjPassInfo, ref lstrPrioityValue), lobjCorTemplates.icdoCorTemplates.template_desc), lstrPrioityValue,
                                aintOrgID: lcdoCorTracking.org_id, aintContactID: lcdoCorTracking.contact_id, astrCorrespondenceLink: lstrDestinationPath,
                                aintTrackingID: lcdoCorTracking.tracking_id, astrTemplateName: lobjCorTemplates.icdoCorTemplates.template_name);
                                if (lcdoCorTracking.contact_id > 0)
                                {
                                    busContact lobjContact = new busContact();
                                    lobjContact.FindContact(lcdoCorTracking.contact_id);
                                    if (!string.IsNullOrEmpty(lobjContact.icdoContact.email_address))
                                    {
                                        HelperFunction.SendMail(NeoSpin.Common.ApplicationSettings.Instance.WSSMailFrom.ToString(),
                                            lobjContact.icdoContact.email_address,
                                            NeoSpin.Common.ApplicationSettings.Instance.WSSMailSubject.ToString(),
                                            NeoSpin.Common.ApplicationSettings.Instance.WSSMailBody.ToString());

                                        //HelperFunction.SendMail(AppSettingsHelper.Instance.Configuration["WSSMailFrom"].ToString(),
                                        //                        lobjContact.icdoContact.email_address,
                                        //                        AppSettingsHelper.Instance.Configuration["WSSMailSubject"].ToString(),
                                        //                        AppSettingsHelper.Instance.Configuration["WSSMailBody"].ToString());
                                    }
                                }
                            }
                            else
                            {
                                lobjError = new utlError();
                                lobjError.istrErrorMessage = "Only employer can publish correspondence to Web Self Service.";
                                larrList.Add(lobjError);
                                return larrList;
                            }
                        }
                    }
                    else
                    {
                        lobjError = new utlError();
                        lobjError.istrErrorMessage = "Invalid Template";
                        larrList.Add(lobjError);
                        return larrList;
                    }
                }
                else
                {
                    lobjError = new utlError();
                    lobjError.istrErrorMessage = "Invalid Tracking ID";
                    larrList.Add(lobjError);
                    return larrList;
                }
            }
            catch (Exception _exc)
            {
                ExceptionManager.Publish(_exc);
                lobjError = new utlError();
                lobjError.istrErrorMessage = _exc.Message;
                larrList.Add(lobjError);
            }
            return larrList;
        }

        public Collection<cdoCodeValue> iclcCodeValue { get; set; }

        public void LoadFormsForESS()
        {
            if (iclcCodeValue == null)
                iclcCodeValue = new Collection<cdoCodeValue>();

            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(1028);

            DataTable ldtbESSForms = ldtbList.AsEnumerable()
                                            .Where(o => o.Field<string>("data2") == busConstant.ESSPortal || o.Field<string>("data2") == busConstant.BothPortal)
                                            .AsDataTable();

            iclcCodeValue = cdoCodeValue.GetCollection<cdoCodeValue>(ldtbESSForms);
        }
    }
}
