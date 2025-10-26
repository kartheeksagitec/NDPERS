using System;
using System.Collections.Generic;
using System.Data;
using NeoSpin.Common;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Reflection;
using Sagitec.CustomDataObjects;
using System.Net;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.ObjectModel;
using System.Net.Mail;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NeoSpin.BusinessObjects.Common;
using System.Linq;
using System.Text;
using Sagitec.ExceptionPub;
using Sagitec.Bpm;

namespace NeoSpin.BusinessObjects
{
    public static class busGlobalFunctions
    {
        /// <summary>
        /// Function to Validate the Given string as Proper Email ID.
        /// </summary>		
        /// <param name="astrEmail">Email Id</param>
        /// <returns>bool</returns>
        public static bool IsValidEmail(string astrEmail)
        {
            //PIR-18492 prod issue on validating email
            string lstremailValidation = "^[_A-Za-z0-9-\\+]+(\\.[_A-Za-z0-9-]+)*@[A-Za-z0-9-]+(\\.[A-Za-z0-9]+)*(\\.[A-Za-z]{2,})$";
            Regex regex = new Regex(lstremailValidation); // PIR 9588 
            if (regex.IsMatch(astrEmail))
                return true;
            return false;
        }

        public static bool IsEmailValid(string astrEmail)
        {
            string lstrFromEmail = Convert.ToString(NeoSpin.Common.ApplicationSettings.Instance.WSSRetrieveMailFrom);
            if (IsNotNullOrEmpty(lstrFromEmail) && IsValidEmail(astrEmail))
            {
                try
                {
                    MailMessage lmsgMessage = new MailMessage(lstrFromEmail, astrEmail);
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
                return false;
            return true;
        }

        /// <summary>
        /// Function to return LastDateMonth for the Given Date.
        /// </summary>		
        /// <param name="adtDate">Date</param>
        /// <returns>DateTime</returns>
        public static DateTime GetLastDayOfMonth(DateTime adtDate)
        {
            DateTime adtLastDateOfMonth = new DateTime(adtDate.Year, adtDate.Month, 1);
            return adtLastDateOfMonth.AddMonths(1).AddDays(-1);
        }
        /// <summary>
        /// Function to return LastDateMonth for the Given Date.
        /// </summary>		
        /// <param name="adtDate">Date</param>
        /// <returns>DateTime</returns>       
        /// <summary>
        /// Function to Validate the Given Begin, End Date Overlapping with other Begin and End Date
        /// </summary>		
        /// <param name="adtGivenDate">Given Date</param>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>bool</returns>
        public static bool CheckDateOverlapping(DateTime adtGivenStartDate, DateTime adtGivenEndDate, DateTime adtStartDate, DateTime adtEndDate)
        {
            bool lblnStartDateResult = CheckDateOverlapping(adtGivenStartDate, false, adtStartDate, adtEndDate);
            bool lblnEndDateResult = CheckDateOverlapping(adtGivenEndDate, true, adtStartDate, adtEndDate);
            if ((lblnStartDateResult) || (lblnEndDateResult))
                return true;
            return false;
        }

        /// <summary>
        /// Function to Validate the Given Date is Overlapping with other dates
        /// </summary>		
        /// <param name="adtGivenDate">Given Date</param>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>bool</returns>
        public static bool CheckDateOverlapping(DateTime adtGivenDate, DateTime adtStartDate, DateTime adtEndDate)
        {
            return CheckDateOverlapping(adtGivenDate, true, adtStartDate, adtEndDate);
        }

        public static bool CheckDateOverlapping(DateTime adtGivenDate, DateTime? adtStartDate, DateTime? adtEndDate)
        {
            DateTime ldtStartDate = DateTime.MinValue;
            if (adtStartDate.HasValue)
                ldtStartDate = adtStartDate.Value;

            DateTime ldtEndDate = DateTime.MaxValue;
            if (adtEndDate.HasValue)
                ldtEndDate = adtEndDate.Value;

            return CheckDateOverlapping(adtGivenDate, true, ldtStartDate, ldtEndDate);
        }

        public static bool CheckDateOverlapping(DateTime adtGivenDate, bool IsGivenDateSetToMaxValue, DateTime adtStartDate, DateTime adtEndDate)
        {

            //Given Date is NULL, based on the FLAG (IsGivenDateSetToMaxValue), it will update the 
            //Given Date into MIN VALUE or MAX VALUE
            if (((adtGivenDate == null) || (adtGivenDate == DateTime.MinValue))
                && IsGivenDateSetToMaxValue)
            {
                adtGivenDate = DateTime.MaxValue;
            }
            else if (((adtGivenDate == null) || (adtGivenDate == DateTime.MaxValue))
                && (!IsGivenDateSetToMaxValue))
            {
                adtGivenDate = DateTime.MinValue;
            }

            //if End Date is NULL, making it MaxValue 
            if (adtEndDate == DateTime.MinValue)
            {
                adtEndDate = DateTime.MaxValue;
            }

            if ((adtGivenDate.Date >= adtStartDate.Date) && (adtGivenDate.Date <= adtEndDate.Date))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// If only Zip Code entered, it calls the City State Lookup and get the City and State
        /// and then Calls the Address Validate Request Service. Otherwise, 
        /// it always calls the Address Validate Request Service.
        /// </summary>
        /// <param name="_acdoWebServiceAddress"></param>
        /// <returns></returns>
        public static cdoWebServiceAddress ValidateWebServiceAddress(cdoWebServiceAddress _acdoWebServiceAddress)
        {
            cdoWebServiceAddress _lcdoWebServiceAddressResult = new cdoWebServiceAddress();
            bool lblnSuccess = true;
            string lstrUSPSWebPath = NeoSpin.Common.ApplicationSettings.Instance.USPS_WebService_PATH;
            string lstrdecryptedUserID = HelperFunction.SagitecDecrypt(null, NeoSpin.Common.ApplicationSettings.Instance.USPS_WebService_USERID);
            string lstrWebAddressPath = lstrUSPSWebPath + "?API=Verify&XML=<AddressValidateRequest USERID='" + lstrdecryptedUserID + "'><Address ID='0'><Address1>";
            string lstrWebAddressPathAddress2 = "</Address1><Address2>";
            string lstrWebAddressPathCity = "</Address2><City>";
            string lstrWebAddressPathState = "</City><State>";
            string lstrWebAddressPathZip = "</State><Zip5>";
            string lstrWebAddressPathPart5 = "</Zip5><Zip4></Zip4></Address></AddressValidateRequest>";

            //City State Lookup
            if ((String.IsNullOrEmpty(_acdoWebServiceAddress.addr_city) &&
                (String.IsNullOrEmpty(_acdoWebServiceAddress.addr_state_value)) &&
                (!String.IsNullOrEmpty(_acdoWebServiceAddress.addr_zip_code)))
                )
            {
                string lstrCityStateWebAddressPath = lstrUSPSWebPath + "?API=CityStateLookup&XML=<CityStateLookupRequest USERID='" + lstrdecryptedUserID + "'><ZipCode ID='0'><Zip5>";
                string lstrCityStateWebAddressPathPart5 = "</Zip5></ZipCode></CityStateLookupRequest>";
                XmlTextReader lxtrCityStateReader = null;
                WebResponse lresCityStateResponse = null;
                try
                {
                    //PIR 19347 - For some reason, if the USPS service is down, the system should not throw any exception, changes made in this function so that, 
                    //the method invalidates the address instead of throwing exception
                    WebRequest lreqCityStateRequest = WebRequest.Create(lstrCityStateWebAddressPath + _acdoWebServiceAddress.addr_zip_code + lstrCityStateWebAddressPathPart5);
                    lresCityStateResponse = lreqCityStateRequest.GetResponse();
                    Stream lstmCityStateStream = lresCityStateResponse.GetResponseStream();
                    lxtrCityStateReader = new XmlTextReader(lstmCityStateStream);

                    //Validate Address
                    while (lxtrCityStateReader.Read())
                    {
                        if (!(lxtrCityStateReader.IsEmptyElement))
                        {
                            if (!((lxtrCityStateReader.Name.Equals("Error")) ||
                                  (lxtrCityStateReader.Name.Equals("Number")) ||
                                  (lxtrCityStateReader.Name.Equals("Source")) ||
                                  (lxtrCityStateReader.Name.Equals("Description")) ||
                                  (lxtrCityStateReader.Name.Equals("HelpContext")) ||
                                  (lxtrCityStateReader.Name.Equals("HelpFile"))))
                            {
                                if (lxtrCityStateReader.Name.Equals("City"))
                                {
                                    _acdoWebServiceAddress.addr_city = lxtrCityStateReader.ReadString();
                                }
                                if (lxtrCityStateReader.LocalName.Equals("State"))
                                {
                                    _acdoWebServiceAddress.addr_state_value = lxtrCityStateReader.ReadString();
                                }
                            }
                        }
                    }
                }
                catch (WebException exp)
                {
                    _lcdoWebServiceAddressResult.address_validate_flag = "N";
                    _lcdoWebServiceAddressResult.address_validate_error = exp.Message;
                    lblnSuccess = false;
                }
                if (lxtrCityStateReader.IsNotNull())
                    lxtrCityStateReader.Close();
                if (lresCityStateResponse.IsNotNull())
                    lresCityStateResponse.Close();
            }

            //Validate
            if (lblnSuccess)
            {
                XmlTextReader lxtrValidateReader = null;
                WebResponse lresValidateResponse = null;
                try
                {
                    //if (_acdoWebServiceAddress.addr_line_1.IsNullOrEmpty())
                    //{
                    //    _acdoWebServiceAddress.addr_line_1 = _acdoWebServiceAddress.addr_line_2;
                    //    _acdoWebServiceAddress.addr_line_2 = string.Empty;
                    //}
                    WebRequest lreqValidateRequest = WebRequest.Create(lstrWebAddressPath + _acdoWebServiceAddress.addr_line_1 + lstrWebAddressPathAddress2 +
                                                                      _acdoWebServiceAddress.addr_line_2 + lstrWebAddressPathCity +
                                                                      _acdoWebServiceAddress.addr_city + lstrWebAddressPathState +
                                                                      _acdoWebServiceAddress.addr_state_value + lstrWebAddressPathZip +
                                                                      _acdoWebServiceAddress.addr_zip_code + lstrWebAddressPathPart5);
                    lresValidateResponse = lreqValidateRequest.GetResponse();
                    Stream lstmValidateStream = lresValidateResponse.GetResponseStream();
                    lxtrValidateReader = new XmlTextReader(lstmValidateStream);
                    while (lxtrValidateReader.Read())
                    {
                        {
                            if (lxtrValidateReader.NodeType == XmlNodeType.Element)
                            {
                                if (!(lxtrValidateReader.IsEmptyElement))
                                {
                                    if (
                                        !((lxtrValidateReader.Name.Equals("Error")) || (lxtrValidateReader.Name.Equals("Number")) ||
                                          (lxtrValidateReader.Name.Equals("Source")) || (lxtrValidateReader.Name.Equals("Description")) ||
                                          (lxtrValidateReader.Name.Equals("HelpContext")) || (lxtrValidateReader.Name.Equals("HelpFile"))))
                                    {
                                        if (lxtrValidateReader.LocalName.Equals("Address1"))
                                        {
                                            _lcdoWebServiceAddressResult.addr_line_1 = lxtrValidateReader.ReadString();
                                        }
                                        if (lxtrValidateReader.LocalName.Equals("Address2"))
                                        {
                                            _lcdoWebServiceAddressResult.addr_line_2 = lxtrValidateReader.ReadString();
                                        }
                                        if (lxtrValidateReader.Name.Equals("City"))
                                        {
                                            _lcdoWebServiceAddressResult.addr_city = lxtrValidateReader.ReadString();
                                        }
                                        if (lxtrValidateReader.LocalName.Equals("State"))
                                        {
                                            _lcdoWebServiceAddressResult.addr_state_value = lxtrValidateReader.ReadString();
                                        }
                                        if (lxtrValidateReader.LocalName.Equals("Zip5"))
                                        {
                                            _lcdoWebServiceAddressResult.addr_zip_code = lxtrValidateReader.ReadString();
                                        }
                                        if (lxtrValidateReader.LocalName.Equals("Zip4"))
                                        {
                                            _lcdoWebServiceAddressResult.addr_zip_4_code = lxtrValidateReader.ReadString();
                                        }
                                        _lcdoWebServiceAddressResult.address_validate_flag = "Y";
                                        _lcdoWebServiceAddressResult.address_validate_error = "Address Validated Successfully";
                                    }
                                    else if (lxtrValidateReader.Name.Equals("Description"))
                                    {
                                        _lcdoWebServiceAddressResult.address_validate_flag = "N";
                                        _lcdoWebServiceAddressResult.address_validate_error = lxtrValidateReader.ReadString();
                                    }
                                }
                            }
                        }
                    }
                    if(string.IsNullOrEmpty(_lcdoWebServiceAddressResult.addr_line_1) && !string.IsNullOrEmpty(_lcdoWebServiceAddressResult.addr_line_2))
                    {
                        _lcdoWebServiceAddressResult.addr_line_1 = _lcdoWebServiceAddressResult.addr_line_2;
                        _lcdoWebServiceAddressResult.addr_line_2 = string.Empty;
                    }
                }
                catch (WebException exp)
                {
                    _lcdoWebServiceAddressResult.address_validate_flag = "N";
                    _lcdoWebServiceAddressResult.address_validate_error = exp.Message;
                }
                if (lxtrValidateReader.IsNotNull())
                    lxtrValidateReader.Close();
                if (lresValidateResponse.IsNotNull())
                    lresValidateResponse.Close();
            }
            return _lcdoWebServiceAddressResult;
        }

        /// <summary>
        /// Function to Create the Meeting Request & Sending to the Mentioned Email.
        /// </summary>
        /// <param name="start">Start Date</param>
        /// <param name="end">End Date</param>
        /// <param name="Subject">Subject of the Meeting Request</param>
        /// <param name="summary">Summary of the Meeting Request</param>
        /// <param name="location">Location of the Meeting Request</param>
        /// <param name="organizerName">Organizer Name</param>
        /// <param name="organizerEmail">Organizer Email</param>
        /// <param name="attendeeEmail">Attendee Email Address</param>
        /// <param name="meetingRequestUID">Meeting Request Unique ID</param>
        /// <returns>None</returns>
        public static void SendMeetingRequest(DateTime start,
                                        DateTime end,
                                        string subject,
                                        string summary,
                                        string location,
                                        string organizerName,
                                        string organizerEmail,
                                        string attendeeEmail,
                                        string meetingRequestUID)
        {
            if (string.IsNullOrEmpty(attendeeEmail))
            {
                return;
            }
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

            //  Set up the different mime types contained in the message    
            System.Net.Mime.ContentType textType = new System.Net.Mime.ContentType("text/plain");
            System.Net.Mime.ContentType HTMLType = new System.Net.Mime.ContentType("text/html");
            System.Net.Mime.ContentType calendarType = new System.Net.Mime.ContentType("text/calendar");

            //  Add parameters to the calendar header    
            calendarType.Parameters.Add("method", "REQUEST");
            calendarType.Parameters.Add("name", "PERSLinkAppointment.ics");
            //  Create message body parts    
            //  create the Body in text format    
            string bodyText = "Type:Single Meeting\r\nOrganizer: {0}\r\nStart Time:{1}\r\nEnd Time:{2}\r\nTime Zone:{3}\r\nLocation: {4}\r\n\r\n*~*~*~*~*~*~*~*~*~*\r\n\r\n{5}";
            bodyText = string.Format(bodyText, organizerName, start.ToLongDateString() + " " + start.ToLongTimeString(), end.ToLongDateString() + " " + end.ToLongTimeString(), System.TimeZone.CurrentTimeZone.StandardName, location, summary);

            System.Net.Mail.AlternateView textView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyText, textType);
            msg.AlternateViews.Add(textView);

            //create the Body in HTML format   
            string bodyHTML = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 3.2//EN\">\r\n<HTML>\r\n<HEAD>\r\n<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=utf-8\">\r\n<META NAME=\"Generator\" CONTENT=\"MS Exchange Server version 6.5.7652.24\">\r\n<TITLE>{0}</TITLE>\r\n</HEAD>\r\n<BODY>\r\n<!-- Converted from text/plain format -->\r\n<P><FONT SIZE=2>Type:Single Meeting<BR>\r\nOrganizer:{1}<BR>\r\nStart Time:{2}<BR>\r\nEnd Time:{3}<BR>\r\nTime Zone:{4}<BR>\r\nLocation:{5}<BR>\r\n<BR>\r\n*~*~*~*~*~*~*~*~*~*<BR>\r\n<BR>\r\n{6}<BR>\r\n</FONT>\r\n</P>\r\n\r\n</BODY>\r\n</HTML>";
            bodyHTML = string.Format(bodyHTML, summary, organizerName, start.ToLongDateString() + " " + start.ToLongTimeString(), end.ToLongDateString() + " " + end.ToLongTimeString(), System.TimeZone.CurrentTimeZone.StandardName, location, summary);
            System.Net.Mail.AlternateView HTMLView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyHTML, HTMLType);
            msg.AlternateViews.Add(HTMLView);
            //create the Body in VCALENDAR format   
            string calDateFormat = "yyyyMMddTHHmmssZ";
            string AttendeeData = "";

            //attendeeList (For Multiple Attendees)
            //for (int i = 0; i < attendeeList.Count; i++)
            //{
            //    AttendeeData = AttendeeData + "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE;CN=\"" + attendeeList[i].DisplayName.ToString() + "\":MAILTO:" + attendeeList[i].Address.ToString();
            //}

            AttendeeData = AttendeeData + "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE;CN=\"" + organizerName + "\":MAILTO:" + attendeeEmail;

            string bodyCalendar = "BEGIN:VCALENDAR\r\nMETHOD:REQUEST\r\nPRODID:Microsoft CDO for Microsoft Exchange\r\nVERSION:2.0\r\nBEGIN:VTIMEZONE\r\nTZID:(GMT-06.00) Central Time (US & Canada)\r\nX-MICROSOFT-CDO-TZID:11\r\nBEGIN:STANDARD\r\nDTSTART:16010101T020000\r\nTZOFFSETFROM:-0500\r\nTZOFFSETTO:-0600\r\nRRULE:FREQ=YEARLY;WKST=MO;INTERVAL=1;BYMONTH=11;BYDAY=1SU\r\nEND:STANDARD\r\nBEGIN:DAYLIGHT\r\nDTSTART:16010101T020000\r\nTZOFFSETFROM:-0600\r\nTZOFFSETTO:-0500\r\nRRULE:FREQ=YEARLY;WKST=MO;INTERVAL=1;BYMONTH=3;BYDAY=2SU\r\nEND:DAYLIGHT\r\nEND:VTIMEZONE\r\nBEGIN:VEVENT\r\nDTSTAMP:{8}\r\nDTSTART:{0}\r\nSUMMARY:{7}\r\nUID:{5}\r\n" + AttendeeData + "\r\nACTION;RSVP=TRUE;CN=\"{4}\":MAILTO:{4}\r\nORGANIZER;CN=\"{3}\":mailto:{4}\r\nLOCATION:{2}\r\nDTEND:{1}\r\nDESCRIPTION:{7}\\N\r\nSEQUENCE:1\r\nPRIORITY:5\r\nCLASS:\r\nCREATED:{8}\r\nLAST-MODIFIED:{8}\r\nSTATUS:CONFIRMED\r\nTRANSP:OPAQUE\r\nX-MICROSOFT-CDO-BUSYSTATUS:BUSY\r\nX-MICROSOFT-CDO-INSTTYPE:0\r\nX-MICROSOFT-CDO-INTENDEDSTATUS:BUSY\r\nX-MICROSOFT-CDO-ALLDAYEVENT:FALSE\r\nX-MICROSOFT-CDO-IMPORTANCE:1\r\nX-MICROSOFT-CDO-OWNERAPPTID:-1\r\nX-MICROSOFT-CDO-ATTENDEE-CRITICAL-CHANGE:{8}\r\nX-MICROSOFT-CDO-OWNER-CRITICAL-CHANGE:{8}\r\nBEGIN:VALARM\r\nACTION:DISPLAY\r\nDESCRIPTION:REMINDER\r\nTRIGGER;RELATED=START:-PT00H15M00S\r\nEND:VALARM\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n";
            bodyCalendar = string.Format(bodyCalendar, start.ToUniversalTime().ToString(calDateFormat), end.ToUniversalTime().ToString(calDateFormat), location, organizerName, organizerEmail, meetingRequestUID, summary, subject, DateTime.Now.ToUniversalTime().ToString(calDateFormat), attendeeEmail);
            System.Net.Mail.AlternateView calendarView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyCalendar, calendarType);
            calendarView.TransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;
            msg.AlternateViews.Add(calendarView);

            //Adress the message    
            msg.From = new System.Net.Mail.MailAddress(organizerEmail);
            msg.To.Add(attendeeEmail);
            msg.Subject = subject;

            //Sending the Meeting Request
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = NeoSpin.Common.ApplicationSettings.Instance.SmtpServer;
            smtp.Send(msg);
        }

        /// <summary>
        /// Function to Cancel the Meeting Request
        /// </summary>
        /// <param name="start">Start Date</param>
        /// <param name="end">End Date</param>
        /// <param name="Subject">Subject of the Meeting Request</param>
        /// <param name="summary">Summary of the Meeting Request</param>
        /// <param name="location">Location of the Meeting Request</param>
        /// <param name="organizerName">Organizer Name</param>
        /// <param name="organizerEmail">Organizer Email</param>
        /// <param name="attendeeEmail">Attendee Email Address</param>
        /// <param name="meetingRequestUID">Meeting Request Unique ID</param>
        /// <returns>None</returns>
        public static void CancelMeetingRequest(DateTime start,
                                        DateTime end,
                                        string subject,
                                        string summary,
                                        string location,
                                        string organizerName,
                                        string organizerEmail,
                                        string attendeeEmail,
                                        string meetingRequestUID)
        {
            if (string.IsNullOrEmpty(attendeeEmail))
            {
                return;
            }
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();

            //  Set up the different mime types contained in the message    
            System.Net.Mime.ContentType textType = new System.Net.Mime.ContentType("text/plain");
            System.Net.Mime.ContentType HTMLType = new System.Net.Mime.ContentType("text/html");
            System.Net.Mime.ContentType calendarType = new System.Net.Mime.ContentType("text/calendar");

            //  Add parameters to the calendar header    
            calendarType.Parameters.Add("method", "CANCEL");
            calendarType.Parameters.Add("name", "PERSLinkAppointment.ics");
            //  Create message body parts    
            //  create the Body in text format    
            string bodyText = "Type:Single Meeting\r\nOrganizer: {0}\r\nStart Time:{1}\r\nEnd Time:{2}\r\nTime Zone:{3}\r\nLocation: {4}\r\n\r\n*~*~*~*~*~*~*~*~*~*\r\n\r\n{5}";
            bodyText = string.Format(bodyText, organizerName, start.ToLongDateString() + " " + start.ToLongTimeString(), end.ToLongDateString() + " " + end.ToLongTimeString(), System.TimeZone.CurrentTimeZone.StandardName, location, summary);

            System.Net.Mail.AlternateView textView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyText, textType);
            msg.AlternateViews.Add(textView);

            //create the Body in HTML format   
            string bodyHTML = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 3.2//EN\">\r\n<HTML>\r\n<HEAD>\r\n<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=utf-8\">\r\n<META NAME=\"Generator\" CONTENT=\"MS Exchange Server version 6.5.7652.24\">\r\n<TITLE>{0}</TITLE>\r\n</HEAD>\r\n<BODY>\r\n<!-- Converted from text/plain format -->\r\n<P><FONT SIZE=2>Type:Single Meeting<BR>\r\nOrganizer:{1}<BR>\r\nStart Time:{2}<BR>\r\nEnd Time:{3}<BR>\r\nTime Zone:{4}<BR>\r\nLocation:{5}<BR>\r\n<BR>\r\n*~*~*~*~*~*~*~*~*~*<BR>\r\n<BR>\r\n{6}<BR>\r\n</FONT>\r\n</P>\r\n\r\n</BODY>\r\n</HTML>";
            bodyHTML = string.Format(bodyHTML, summary, organizerName, start.ToLongDateString() + " " + start.ToLongTimeString(), end.ToLongDateString() + " " + end.ToLongTimeString(), System.TimeZone.CurrentTimeZone.StandardName, location, summary);
            System.Net.Mail.AlternateView HTMLView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyHTML, HTMLType);
            msg.AlternateViews.Add(HTMLView);
            //create the Body in VCALENDAR format   
            string calDateFormat = "yyyyMMddTHHmmssZ";
            string AttendeeData = "";

            //attendeeList (For Multiple Attendees)
            //for (int i = 0; i < attendeeList.Count; i++)
            //{
            //    AttendeeData = AttendeeData + "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE;CN=\"" + attendeeList[i].DisplayName.ToString() + "\":MAILTO:" + attendeeList[i].Address.ToString();
            //}

            AttendeeData = AttendeeData + "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE;CN=\"" + organizerName + "\":MAILTO:" + attendeeEmail;

            string bodyCalendar = "BEGIN:VCALENDAR\r\nMETHOD:CANCEL\r\nPRODID:Microsoft CDO for Microsoft Exchange\r\nVERSION:2.0\r\nBEGIN:VTIMEZONE\r\nTZID:(GMT-06.00) Central Time (US & Canada)\r\nX-MICROSOFT-CDO-TZID:11\r\nBEGIN:STANDARD\r\nDTSTART:16010101T020000\r\nTZOFFSETFROM:-0500\r\nTZOFFSETTO:-0600\r\nRRULE:FREQ=YEARLY;WKST=MO;INTERVAL=1;BYMONTH=11;BYDAY=1SU\r\nEND:STANDARD\r\nBEGIN:DAYLIGHT\r\nDTSTART:16010101T020000\r\nTZOFFSETFROM:-0600\r\nTZOFFSETTO:-0500\r\nRRULE:FREQ=YEARLY;WKST=MO;INTERVAL=1;BYMONTH=3;BYDAY=2SU\r\nEND:DAYLIGHT\r\nEND:VTIMEZONE\r\nBEGIN:VEVENT\r\nDTSTAMP:{8}\r\nDTSTART:{0}\r\nSUMMARY:{7}\r\nUID:{5}\r\n" + AttendeeData + "\r\nACTION;RSVP=TRUE;CN=\"{4}\":MAILTO:{4}\r\nORGANIZER;CN=\"{3}\":mailto:{4}\r\nLOCATION:{2}\r\nDTEND:{1}\r\nDESCRIPTION:{7}\\N\r\nSEQUENCE:1\r\nPRIORITY:5\r\nCLASS:\r\nCREATED:{8}\r\nLAST-MODIFIED:{8}\r\nSTATUS:CANCELLED\r\nTRANSP:OPAQUE\r\nX-MICROSOFT-CDO-BUSYSTATUS:BUSY\r\nX-MICROSOFT-CDO-INSTTYPE:0\r\nX-MICROSOFT-CDO-INTENDEDSTATUS:BUSY\r\nX-MICROSOFT-CDO-ALLDAYEVENT:FALSE\r\nX-MICROSOFT-CDO-IMPORTANCE:1\r\nX-MICROSOFT-CDO-OWNERAPPTID:-1\r\nX-MICROSOFT-CDO-ATTENDEE-CRITICAL-CHANGE:{8}\r\nX-MICROSOFT-CDO-OWNER-CRITICAL-CHANGE:{8}\r\nBEGIN:VALARM\r\nACTION:DISPLAY\r\nDESCRIPTION:REMINDER\r\nTRIGGER;RELATED=START:-PT00H15M00S\r\nEND:VALARM\r\nEND:VEVENT\r\nEND:VCALENDAR\r\n";
            bodyCalendar = string.Format(bodyCalendar, start.ToUniversalTime().ToString(calDateFormat), end.ToUniversalTime().ToString(calDateFormat), location, organizerName, organizerEmail, meetingRequestUID, summary, subject, DateTime.Now.ToUniversalTime().ToString(calDateFormat), attendeeEmail);
            System.Net.Mail.AlternateView calendarView = System.Net.Mail.AlternateView.CreateAlternateViewFromString(bodyCalendar, calendarType);
            calendarView.TransferEncoding = System.Net.Mime.TransferEncoding.SevenBit;
            msg.AlternateViews.Add(calendarView);

            //Adress the message    
            msg.From = new System.Net.Mail.MailAddress(organizerEmail);
            msg.To.Add(attendeeEmail);
            msg.Subject = subject;

            //Sending the Meeting Request
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = NeoSpin.Common.ApplicationSettings.Instance.SmtpServer;
            smtp.Send(msg);
        }

        ///// <summary>
        ///// Function to Load all the Running Instances of the Given Document Type and Person
        ///// </summary>
        ///// <param name="aintPersonID"></param>
        ///// <param name="astrDocumentCode"></param>        
        ///// <returns></returns>
        //public static Collection<busActivityInstance> LoadRunningWorkflowByDocumentAndPerson(int aintPersonID, string astrDocumentCode)
        //{
        //    Collection<busActivityInstance> lclbActivityInstance = new Collection<busActivityInstance>();
        //    DataTable ldtpActivityInstance = busNeoSpinBase.Select("cdoActivityInstance.LoadRunningWorkflowByDocumentAndPerson",
        //        new object[2] { aintPersonID, astrDocumentCode });
        //    busBase lobjBase = new busBase();
        //    lclbActivityInstance = lobjBase.GetCollection<busActivityInstance>(ldtpActivityInstance, "icdoActivityInstance");
        //    return lclbActivityInstance;
        //}

        /// <summary>
        /// Function to Load all the Running Instances of the Given Document Type and Person
        /// </summary>
        /// <param name="aintPersonID"></param>
        /// <param name="astrDocumentCode"></param>        
        /// <returns></returns>
        public static Collection<busSolBpmActivityInstance> LoadRunningBPMByDocumentAndPerson(int aintPersonID, string astrDocumentCode)
        {
            Collection<busSolBpmActivityInstance> lclbActivityInstance = new Collection<busSolBpmActivityInstance>();
            DataTable ldtpActivityInstance = busNeoSpinBase.Select("cdoActivityInstance.LoadRunningWorkflowByDocumentAndPerson",
                new object[2] { aintPersonID, astrDocumentCode });
            busBase lobjBase = new busBase();
            lclbActivityInstance = lobjBase.GetCollection<busSolBpmActivityInstance>(ldtpActivityInstance, "icdoBpmActivityInstance");
            return lclbActivityInstance;
        }

        ///// <summary>
        ///// Function to Load all the Running Instances of the Given Document Type and Org Code
        ///// </summary>        
        ///// <param name="astrOrgCode"></param>
        ///// <param name="astrDocumentCode"></param>        
        ///// <returns></returns>
        //public static Collection<busActivityInstance> LoadRunningWorkflowByDocumentAndOrgCode(string astrOrgCode, string astrDocumentCode)
        //{
        //    Collection<busActivityInstance> lclbActivityInstance = new Collection<busActivityInstance>();
        //    DataTable ldtpActivityInstance = busNeoSpinBase.Select("cdoActivityInstance.LoadRunningWorkflowByDocumentAndOrgCode",
        //        new object[2] { astrOrgCode, astrDocumentCode });
        //    busBase lobjBase = new busBase();
        //    lclbActivityInstance = lobjBase.GetCollection<busActivityInstance>(ldtpActivityInstance, "icdoActivityInstance");
        //    return lclbActivityInstance;
        //}

        /// <summary>
        /// Function to Load all the Running Instances of the Given Document Type and Org Code
        /// </summary>        
        /// <param name="astrOrgCode"></param>
        /// <param name="astrDocumentCode"></param>        
        /// <returns></returns>
        public static Collection<busSolBpmActivityInstance> LoadRunningBPMByDocumentAndOrgCode(string astrOrgCode, string astrDocumentCode)
        {
            Collection<busSolBpmActivityInstance> lclbActivityInstance = new Collection<busSolBpmActivityInstance>();
            DataTable ldtpActivityInstance = busNeoSpinBase.Select("cdoActivityInstance.LoadRunningWorkflowByDocumentAndOrgCode",
                new object[2] { astrOrgCode, astrDocumentCode });
            busBase lobjBase = new busBase();
            lclbActivityInstance = lobjBase.GetCollection<busSolBpmActivityInstance>(ldtpActivityInstance, "icdoBpmActivityInstance");
            return lclbActivityInstance;
        }

        public static int GetOrgIdFromOrgCode(string lstrOrgCode)
        {
            int lintOrgId = 0;

            if (String.IsNullOrEmpty(lstrOrgCode)) return lintOrgId;

            DataTable ldtbOrganization = busBase.Select<cdoOrganization>(new string[1] { "org_code" },
                  new object[1] { lstrOrgCode }, null, null);
            if (ldtbOrganization.Rows.Count > 0)
            {
                lintOrgId = Convert.ToInt32(ldtbOrganization.Rows[0]["org_id"]);
            }
            return lintOrgId;
        }

        public static string GetOrgCodeFromOrgId(int lintOrgId)
        {
            string lstrOrgCode = null;
            if (lintOrgId > 0)
            {
                DataTable ldtbOrganization = busBase.Select<cdoOrganization>(new string[1] { "org_id" },
                                                                             new object[1] { lintOrgId }, null, null);
                if (ldtbOrganization.Rows.Count > 0)
                {
                    lstrOrgCode = ldtbOrganization.Rows[0]["org_code"].ToString();
                }
            }
            return lstrOrgCode;
        }

        public static string GetUserIdFromUserSerialId(int lintUserSerialId)
        {
            string lstrUserId = null;
            DataTable ldtbUser = busBase.Select<cdoUser>(new string[1] { "user_serial_id" },
                  new object[1] { lintUserSerialId }, null, null);
            if (ldtbUser.Rows.Count > 0)
            {
                lstrUserId = ldtbUser.Rows[0]["user_id"].ToString();
            }
            return lstrUserId;
        }

        public static int GetUserSerialIdFromUserId(string lstrUserId)
        {
            int lintUserSerialId = 0;
            DataTable ldtbUser = busBase.Select<cdoUser>(new string[1] { "user_id" },
                  new object[1] { lstrUserId }, null, null);
            if (ldtbUser.Rows.Count > 0)
            {
                lintUserSerialId = Convert.ToInt32(ldtbUser.Rows[0]["user_serial_id"].ToString());
            }
            return lintUserSerialId;
        }

        /// <summary>
        /// Get the User object
        /// </summary>
        /// <param name="lintUserSerialId"></param>
        /// <returns></returns>
        public static busUser GetUserObjectFromUserSerialId(int lintUserSerialId)
        {
            busUser lobjUser = new busUser { icdoUser = new cdoUser() };
            DataTable ldtbUser = busBase.Select<cdoUser>(new string[1] { "user_serial_id" },
                  new object[1] { lintUserSerialId }, null, null);
            if (ldtbUser.Rows.Count > 0)
            {
                lobjUser.icdoUser.LoadData(ldtbUser.Rows[0]);
            }
            return lobjUser;
        }

        public static int CalulateAge(DateTime adtDateOfBirth, DateTime adtAgeCalculationDate)
        {
            int years = adtAgeCalculationDate.Year - adtDateOfBirth.Year;
            // subtract another year if we're before the
            // birth day in the current year
            if (adtAgeCalculationDate.Month < adtDateOfBirth.Month ||
                 (adtAgeCalculationDate.Month == adtDateOfBirth.Month && adtAgeCalculationDate.Day < adtDateOfBirth.Day))
            {
                years--;
            }
            return years;
        }

        public static string GetPersonNameByPersonID(int AintPersonID)
        {
            string lstrPersonName = string.Empty;
            DataTable ldtblPerson = busBase.Select<cdoPerson>(new string[1] { "person_id" }, new object[1] { AintPersonID }, null, null);
            if (ldtblPerson.Rows.Count > 0)
            {
                lstrPersonName = Convert.ToString(ldtblPerson.Rows[0]["last_name"]) + ", " + Convert.ToString(ldtblPerson.Rows[0]["first_name"]);
            }
            return lstrPersonName;
        }

        public static int GetPersonIDBySSN(string astrSSN)
        {
            int lintPersonID = 0;
            DataTable ldtblPerson = busBase.Select<cdoPerson>(new string[1] { "ssn" }, new object[1] { astrSSN }, null, null);
            if (ldtblPerson.Rows.Count > 0)
            {
                lintPersonID = Convert.ToInt32(ldtblPerson.Rows[0]["person_id"]);
            }
            return lintPersonID;
        }

        public static string GetOrgNameByOrgID(int AintOrgID)
        {
            string lstrOrgName = string.Empty;
            DataTable ldtbOrg = busBase.Select<cdoOrganization>(new string[1] { "org_id" }, new object[1] { AintOrgID }, null, null);
            if (ldtbOrg.Rows.Count > 0)
            {
                lstrOrgName = Convert.ToString(ldtbOrg.Rows[0]["org_name"]);
            }
            return lstrOrgName;
        }

        public static cdoCodeValue GetCodeValueByDescription(int aintCodeId, string astrDescription)
        {
            cdoCodeValue lcdoCodeValue = new cdoCodeValue();
            DataTable ldtbCodeValue = busBase.Select<cdoCodeValue>(
               new string[2] { "code_id", "description" },
               new object[2] { aintCodeId, astrDescription }, null, null);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lcdoCodeValue.LoadData(ldtbCodeValue.Rows[0]);
            }
            return lcdoCodeValue;
        }

        //PIR 8127
        public static string GetDescriptionByCodeValue(int aintCodeId, string astrCodeValue, utlPassInfo aobjPassInfo)
        {
            string lstrResult = null;
            string lstrWhere = "code_id = '" + aintCodeId.ToString() + "' ";
            lstrWhere += "and code_value = '" + astrCodeValue + "' ";
            DataTable ldtbCodeValue = aobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lstrResult = ldtbCodeValue.Rows[0]["description"].ToString();
            }
            return lstrResult;
        }

        public static string GetData1ByCodeValue(int aintCodeId, string astrCodeValue, utlPassInfo aobjPassInfo)
        {
            string lstrResult = null;
            string lstrWhere = "code_id = '" + aintCodeId.ToString() + "' ";
            lstrWhere += "and code_value = '" + astrCodeValue + "' ";
            DataTable ldtbCodeValue = aobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lstrResult = ldtbCodeValue.Rows[0]["data1"].ToString();
            }
            return lstrResult;
        }

        public static string GetData2ByCodeValue(int aintCodeId, string astrCodeValue, utlPassInfo aobjPassInfo)
        {
            string lstrResult = null;
            string lstrWhere = "code_id = '" + aintCodeId.ToString() + "' ";
            lstrWhere += "and code_value = '" + astrCodeValue + "' ";
            DataTable ldtbCodeValue = aobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lstrResult = ldtbCodeValue.Rows[0]["data2"].ToString();
            }
            return lstrResult;
        }
        public static string GetData3ByCodeValue(int aintCodeId, string astrCodeValue, utlPassInfo aobjPassInfo)
        {
            string lstrResult = null;
            string lstrWhere = "code_id = '" + aintCodeId.ToString() + "' ";
            lstrWhere += "and code_value = '" + astrCodeValue + "' ";
            DataTable ldtbCodeValue = aobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lstrResult = ldtbCodeValue.Rows[0]["data3"].ToString();
            }
            return lstrResult;
        }

        public static string GetCommentsByCodeValue(int aintCodeId, string astrCodeValue, utlPassInfo aobjPassInfo)
        {
            string lstrResult = string.Empty;
            string lstrWhere = "code_id = '" + aintCodeId.ToString() + "' ";
            lstrWhere += "and code_value = '" + astrCodeValue + "' ";
            DataTable ldtbCodeValue = aobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lstrResult = ldtbCodeValue.Rows[0]["comments"].ToString();
            }
            return lstrResult;
        }

        public static string ToTitleCase(string lstrInputString)
        {
            if (lstrInputString.IsNotNullOrEmpty())
            {
                System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
                return textInfo.ToTitleCase(lstrInputString.ToLower());
            }
            return lstrInputString;
        }

        public static int DateDiffByMonth(DateTime adtStartDate, DateTime adtEndDate)
        {
            adtStartDate = new DateTime(adtStartDate.Year, adtStartDate.Month, 1);
            adtEndDate = new DateTime(adtEndDate.Year, adtEndDate.Month, 1);

            //Calculate Total Months Difference
            int lintTotalDueMonths = 0;
            //PIR 13996 Checked StartDate for Min value as it gives error while file upload.
            if (adtStartDate != DateTime.MinValue)
            {
                while (adtStartDate <= adtEndDate)
                {
                    lintTotalDueMonths++;
                    adtEndDate = adtEndDate.AddMonths(-1);
                }
            }

            return lintTotalDueMonths;
        }

        public static int DateDiffInDays(DateTime adtStartDate, DateTime adtEndDate)
        {
            TimeSpan ts = adtEndDate - adtStartDate;
            return ts.Days;
        }

        public static DateTime GetSysManagementBatchDate()
        {
            //Gettting the Batch Date
            busSystemManagement iobjSystemManagement = new busSystemManagement();
            iobjSystemManagement.FindSystemManagement();
            DateTime ldtBatchDate = DateTime.Now;
            if (iobjSystemManagement.icdoSystemManagement.batch_date != DateTime.MinValue)
            {
                ldtBatchDate = iobjSystemManagement.icdoSystemManagement.batch_date;
            }
            return ldtBatchDate;
        }

        public static string GetSysManagementEmailNotification()
        {
            //Gettting the Batch Date
            busSystemManagement iobjSystemManagement = new busSystemManagement();
            iobjSystemManagement.FindSystemManagement();
            return iobjSystemManagement.icdoSystemManagement.email_notification ?? string.Empty;
        }

        /// <summary>
        /// Function to round of a decimal value to integer.
        /// </summary>
        /// <param name="adecInput"></param>
        /// <returns></returns>
        public static int Round(decimal adecInput)
        {
            return ((int)Math.Round(adecInput, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Function to get Codevalue data for the given codeId and codeValue combination
        /// </summary>
        /// <param name="aintCodeId"></param>
        /// <param name="astrCodeValue"></param>
        /// <returns></returns>
        public static cdoCodeValue GetCodeValueDetails(int aintCodeId, string astrCodeValue)
        {
            cdoCodeValue lobjcdoCodeValue = null;
            DataTable ldtblList = busBase.Select<cdoCodeValue>(new string[2] { "code_id", "code_value" },
                                                              new object[2] { aintCodeId, astrCodeValue }, null, null);
            if (ldtblList.Rows.Count > 0)
            {
                lobjcdoCodeValue = new cdoCodeValue();
                lobjcdoCodeValue.LoadData(ldtblList.Rows[0]);
            }
            return lobjcdoCodeValue;
        }

        /// <summary>
        /// Function to get Codevalue data for the given codeId and codeValue combination
        /// </summary>
        /// <param name="aintCodeId"></param>
        /// <param name="astrData1"></param>
        /// <returns></returns>
        public static Collection<busCodeValue> LoadCodeValueByData1(int aintCodeId, string astrData1)
        {
            return LoadCodeValueByData1(aintCodeId, astrData1, null);
        }

        public static Collection<busCodeValue> LoadCodeValueByData1(int aintCodeId, string astrData1, string astrOrderByClause)
        {
            DataTable ldtblList = busBase.Select<cdoCodeValue>(new string[2] { "code_id", "data1" },
                                                              new object[2] { aintCodeId, astrData1 }, null, astrOrderByClause);
            busBase lobjBase = new busBase();
            Collection<busCodeValue> lclbCodeValue = lobjBase.GetCollection<busCodeValue>(ldtblList, "icdoCodeValue");
            return lclbCodeValue;
        }
		//PIR 25920 DC 2025 changes part 2
        public static Collection<busCodeValue> LoadCodeValueByData1And2(int aintCodeId, string astrData1, string astrData2, string astrOperatorData1, string astOperatorData2)
        {
            return LoadCodeValueByData1And2(aintCodeId, astrData1, astrData2, astrOperatorData1, astOperatorData2, null);
        }

        public static Collection<busCodeValue> LoadCodeValueByData1And2(int aintCodeId, string astrData1, string astrData2, string astrOperatorData1, string astOperatorData2, string astrOrderByClause)
        {
            DataTable ldtblList = busBase.SelectWithOperator<cdoCodeValue>(new string[3] { "code_id", "data1", "data2" },
                                                              new string[3] { "=", astrOperatorData1, astOperatorData2 },
                                                              new object[3] { aintCodeId, astrData1, astrData2 }, astrOrderByClause);
            busBase lobjBase = new busBase();
            Collection<busCodeValue> lclbCodeValue = lobjBase.GetCollection<busCodeValue>(ldtblList, "icdoCodeValue");
            return lclbCodeValue;
        }
        public static Collection<busCodeValue> LoadData1ByCodeID(int aintCodeId)
        {
            DataTable ldtblList = busBase.Select<cdoCodeValue>(new string[1] { "code_id"},
                                                              new object[1] { aintCodeId }, null, null);
            busBase lobjBase = new busBase();
            Collection<busCodeValue> lclbCodeValue = lobjBase.GetCollection<busCodeValue>(ldtblList, "icdoCodeValue");
            return lclbCodeValue;
        }

        /// <summary>
        /// Function to get the power of adecValue1 to adecValue2.
        /// </summary>
        /// <param name="adecValue1"></param>
        /// <param name="adecValue2"></param>
        /// <returns></returns>
        public static decimal Power(decimal adecValue1, decimal adecValue2)
        {
            return Convert.ToDecimal(Math.Pow(Convert.ToDouble(adecValue1), Convert.ToDouble(adecValue2)));
        }

        /// <summary>
        /// Function to round a given decimal value to 2 places.
        /// </summary>
        /// <param name="adecInput"></param>
        /// <returns></returns>
        public static decimal RoundToPenny(decimal adecInput)
        {
            return (Math.Round(adecInput, 2, MidpointRounding.AwayFromZero));
        }


        // PIR 9115
        public static void PostESSMessage(int aintOrgId, int aintPlanId, utlPassInfo aobjPassInfo)
        {
            //string istrMessageText = GetMessageTextByMessageID(10082, aobjPassInfo);
            string lstrPrioityValue = string.Empty;
            string istrMessageText = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(10, aobjPassInfo, ref lstrPrioityValue));


            DataTable ldtCreatedDate = busBase.Select("cdoWssMessageHeader.GetLastReportCreatedDate",
                new object[2] { aintOrgId, istrMessageText });

            DateTime createdDate = Convert.ToDateTime(GetData1ByCodeValue(52, busConstant.PERSLinkGoLiveDate, aobjPassInfo));
            if (ldtCreatedDate.Rows.Count > 0)
            {
                createdDate = Convert.ToDateTime(ldtCreatedDate.Rows[0]["REPORT_CREATED_DATE"]);
            }
            DataTable ldtMessagePosted = busBase.Select("cdoWssMessageHeader.CheckIfMessageAlreadyPosted",
                    new object[3] { aintOrgId, createdDate, istrMessageText });
            if (ldtMessagePosted.Rows.Count == 0)
            {
                busBenefitEnrollmentReport.PublishESSMessage(0, 0, istrMessageText, lstrPrioityValue, aintOrgID: aintOrgId);
            }
        }

        public static cdoPlanRetirementRate GetRetirementRateForPlanDateCombination(int aintPlanId, DateTime adtDateOfPurchase, string astrMemberType)
        {
            cdoPlanRetirementRate lobjcdoPlanRetirementRate = null;
            // Go and get the retirement rate from SGT_Plan_Retirement_Rate table for the plan selected by the user.            
            DataTable ldtbList = busBase.Select("cdoPlanRetirementRate.GetRateForPlanAndDate",
                                        new object[3]
                                            {
                                                aintPlanId,
                                                adtDateOfPurchase,
                                                astrMemberType
                                            });

            if (ldtbList.Rows.Count > 0)
            {
                lobjcdoPlanRetirementRate = new cdoPlanRetirementRate();
                lobjcdoPlanRetirementRate.LoadData(ldtbList.Rows[0]);
            }
            return lobjcdoPlanRetirementRate;
        }


        //get retirement type value
        public static string GetRetirementTypeValue(int aintPlanID)
        {
            string lstrRetirementTypeValue = String.Empty;
            DataTable ldtblList = busBase.Select<cdoPlan>(new string[1] { "plan_id" },
                                                                         new object[1] { aintPlanID }, null, null);
            if (ldtblList != null)
            {
                if (ldtblList.Rows.Count > 0)
                {
                    if (!String.IsNullOrEmpty(ldtblList.Rows[0]["retirement_type_value"].ToString()))
                    {
                        lstrRetirementTypeValue = ldtblList.Rows[0]["retirement_type_value"].ToString();
                    }
                }
            }
            return lstrRetirementTypeValue;
        }

        /// <summary>
        /// Method to Sort the Collection by Given Property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="astrSortExpression">Object Property</param>
        /// <param name="aclbCollection">Source Collection</param>
        /// <returns></returns>
        public static Collection<T> Sort<T>(string astrSortExpression, Collection<T> aclbCollection)
        {
            utlSortComparer lobjComparer = new utlSortComparer();
            lobjComparer.istrSortExpression = astrSortExpression;
            ArrayList larrBase = new ArrayList(aclbCollection);
            larrBase.Sort(lobjComparer);
            aclbCollection.Clear();
            foreach (object lobjTemp in larrBase)
            {
                aclbCollection.Add((T)lobjTemp);
            }
            return aclbCollection;
        }

        /// <summary>
        /// This Method will determine the given Map is Contact Ticket Related Workflow.
        /// Exclusion : Seminar Workflow
        /// </summary>
        /// <returns></returns>
        public static int GetWorkflowProcessIdByBpmProcessName(string astrProcessName)
        {
            WorkflowProcessInfo info = busWorkflowHelper.GetCaseMappingByBpmProcessName(astrProcessName);
            return info.process_id;
        }
        public static bool IsContactTicketRelatedWorkflow(int aintProcessID)
        {
            bool lblnResult = false;
            if ((aintProcessID == busConstant.Map_Transfer_Call_And_Contact_Ticket) ||
                (aintProcessID == busConstant.Map_Schedule_Appointment) ||
                (aintProcessID == busConstant.Map_Initialize_Process_Death_Notification_Workflow) ||
                (aintProcessID == busConstant.Map_Process_Online_Contact_Ticket) ||
                (aintProcessID == busConstant.Map_Process_Online_Benefit_Estimate_Request) ||
                (aintProcessID == busConstant.Map_Process_Online_Service_Purchase_Request) ||
                (aintProcessID == busConstant.Map_Process_Seminar_IDB) ||
                (aintProcessID == busConstant.Map_Schedule_Seminar) ||
                (aintProcessID == busConstant.Map_MSS_Schedule_Appointment) ||
                (aintProcessID == busConstant.Map_ESS_Schedule_Appointment) ||
                (aintProcessID == busConstant.Map_WSS_Death_Notification) ||
                (aintProcessID == busConstant.MapESSRetirementProblem) ||
                (aintProcessID == busConstant.MapESSInsuranceProblem) ||
                (aintProcessID == busConstant.MapESSDeferredCompProblem) ||
                (aintProcessID == busConstant.MapESSOtherProblem))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public static bool IsContactTicketRelatedWorkflow(string astrProcessName)
        {
            return IsContactTicketRelatedWorkflow(GetWorkflowProcessIdByBpmProcessName(astrProcessName));
        }

        /// <summary>
        /// This Method will returns the Maximum of Two Values
        /// </summary>
        public static T GetMax<T>(T aValue1, T aValue2) where T : IComparable
        {
            if (aValue1.CompareTo(aValue2) > 0)
                return aValue1;
            else
                return aValue2;
        }

        /// <summary>
        /// This Method will returns the Minimum of Two Values
        /// </summary>
        public static T GetMin<T>(T aValue1, T aValue2) where T : IComparable
        {
            if (aValue1.CompareTo(aValue2) > 0)
                return aValue2;
            else
                return aValue1;
        }

        /// <summary>
        /// Method to Get Distinct objects in a Collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="aclbCollection">Source Collection</param>
        /// <returns></returns>
        public static Collection<T> GetDistinctObjectsInCollection<T>(Collection<T> aclbCollection)
        {
            Collection<T> aclbFinalCollection = new Collection<T>();
            if (aclbCollection != null)
            {
                foreach (object lobjTemp in aclbCollection)
                {
                    if (!(aclbFinalCollection.Contains((T)lobjTemp)))
                        aclbFinalCollection.Add((T)lobjTemp);
                }
            }
            return aclbFinalCollection;
        }

        public static DataTable LoadHealthRateRefCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_group_health_medicare_part_d_rate_ref", null);
        }

        public static DataTable LoadHealthRateStructureCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_group_health_medicare_part_d_rate_structure_ref", null);
        }

        public static DataTable LoadHealthCoverageRefCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_group_health_medicare_part_d_coverage_ref", null);
        }

        public static DataTable LoadHealthRateCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_health_medicare_part_d_rate", null);
        }

        public static DataTable LoadMedicarePartDRateCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_medicare_part_d_rate_ref", null);
        }

        public static DataTable LoadHMORateCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_hmo_rate", null);
        }

        public static DataTable LoadVisionRateCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_vision_rate", null);
        }

        public static DataTable LoadDentalRateCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_dental_rate", null);
        }

        public static DataTable LoadLifeRateCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_life_rate", null);
        }

        public static DataTable LoadLTCRateCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_ltc_rate", null);
        }

        public static DataTable LoadEAPRateCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_eap_rate", null);
        }

        public static DataTable LoadBenefitProvisionEligibilityCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_benefit_provision_eligibility", null);
        }
        public static DataTable LoadCaseManagementCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_case_management", null);
        }
        public static DataTable LoadPaymentItemTypeCacheData(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", null);
        }
        public static DataTable LoadPostRetirementDeathBenefitOption(utlPassInfo aobjPassInfo)
        {
            return aobjPassInfo.isrvDBCache.GetCacheData("sgt_post_retirement_death_benefit_option_ref", null);
        }
        //PIR 1758
        public static DataTable LoadLowIncomeCreditRefCacheData(utlPassInfo aobjPassInfo)
        {
            return aobjPassInfo.isrvDBCache.GetCacheData("sgt_low_income_credit_ref", null);
        }
        //prod pir 933 fix
        public static DataTable LoadHealthCoverageRefCacheDataWithoutFlagCheck(utlPassInfo iobjPassInfo)
        {
            return iobjPassInfo.isrvDBCache.GetCacheData("sgt_org_plan_group_health_medicare_part_d_coverage_ref_without_flag_check", null);
        }
        public static Collection<T> ToCollection<T>(this List<T> items)
        {
            Collection<T> collection = new Collection<T>();

            for (int i = 0; i < items.Count; i++)
            {
                collection.Add(items[i]);
            }

            return collection;
        }

        public static DataTable AsDataTable(this EnumerableRowCollection<DataRow> rowCollection)
        {
            DataTable ldtbResult = new DataTable();
            if (rowCollection.AsDataView().Count > 0)
                ldtbResult = rowCollection.CopyToDataTable();
            return ldtbResult;
        }

        public static DataRow[] FilterTable<T>(this DataTable source, busConstant.DataType dataType, string filterFieldName, T filterFieldValue)
        {
            DataRow[] larrRows;
            if (dataType == busConstant.DataType.String)
            {
                larrRows = source.Select(filterFieldName + " = '" + filterFieldValue + "'");
            }
            else
            {
                larrRows = source.Select(filterFieldName + " = " + filterFieldValue);
            }
            return larrRows;
        }

        /// <summary>
        /// Method to convert amount to words
        /// </summary>
        /// <param name="astrAmount">Amount</param>
        /// <returns>Amount in Words</returns>
        public static string AmountToWords(string astrAmount)
        {
            long lintInputNum = 0;
            string lstrDollarsPart = string.Empty, lstrCents = string.Empty, lstrCentsPart = string.Empty;
            try
            {
                string[] lstrSplits = new string[2];
                lstrSplits = astrAmount.Split('.');
                lintInputNum = Convert.ToInt64(lstrSplits[0]);
                lstrCents = lstrSplits[1];
                if (lstrCents.Length == 1)
                {
                    lstrCents += "0";
                }
                lstrCents = (lstrCents.Length < 3) ? lstrCents : lstrCents.Substring(0, 2);
                lstrCentsPart = CentsPart(lstrCents);
            }
            catch
            {
                lintInputNum = Convert.ToInt32(astrAmount);
            }

            if (lintInputNum == 0)
            {
                if (string.IsNullOrEmpty(lstrCentsPart))
                    return lstrCentsPart;
                else
                    return lstrCentsPart + " cents";
            }
            else
            {
                lstrDollarsPart = DollarPart(lintInputNum.ToString());
                if (string.IsNullOrEmpty(lstrCentsPart))
                    return lstrDollarsPart;
                else
                    return lstrDollarsPart + " and " + lstrCentsPart + " cents";
            }
        }

        /// <summary>
        /// Method to convert Cents part to words
        /// </summary>
        /// <param name="astrCents">Cents</param>
        /// <returns>Cents in words</returns>
        public static string CentsPart(string astrCents)
        {
            string lstrCents = string.Empty;
            string[] lstrOnes = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] lstrTens = { "", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            int lintDig1, lintDig2, lintCents;

            lintCents = Convert.ToInt32(astrCents);
            if (lintCents < 20)
                lstrCents = lstrCents + lstrOnes[lintCents];
            else
            {
                lintDig1 = lintCents / 10;
                lintDig2 = lintCents % 10;
                lstrCents = lstrCents + lstrTens[lintDig1] + " " + lstrOnes[lintDig2];
            }
            return lstrCents;
        }

        /// <summary>
        /// Method to convert Dollar part to words
        /// </summary>
        /// <param name="astrInputNum">Dollars</param>
        /// <returns>Dollars in words</returns>
        public static string DollarPart(string astrInputNum)
        {
            string lstrLastThree = string.Empty, lstrDollars = string.Empty;
            string[] lstrOnes = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] lstrTens = { "", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            string[] lstrThous = { "", "thousand", "million", "billion", "trillion", "quadrillion", "quintillion" };
            int lintDig1, lintDig2, lintDig3, lintLevel = 0, lintLastTwo, lintThreeDigits;

            while (astrInputNum.Length > 0)
            {
                if (astrInputNum.Length > 0)
                {
                    //Get the three rightmost characters
                    lstrLastThree = (astrInputNum.Length < 3) ? astrInputNum : astrInputNum.Substring(astrInputNum.Length - 3, 3);

                    // Separate the three digits
                    lintThreeDigits = int.Parse(lstrLastThree);
                    lintLastTwo = lintThreeDigits % 100;
                    lintDig1 = lintThreeDigits / 100;
                    lintDig2 = lintLastTwo / 10;
                    lintDig3 = (lintThreeDigits % 10);

                    // append a "thousand" where appropriate
                    if (lintLevel > 0 && lintDig1 + lintDig2 + lintDig3 > 0)
                    {
                        lstrDollars = lstrThous[lintLevel] + " " + lstrDollars;
                        lstrDollars = lstrDollars.Trim();
                    }

                    // check that the last two digits is not a zero
                    if (lintLastTwo > 0)
                    {
                        if (lintLastTwo < 20)
                        {
                            // if less than 20, use "ones" only
                            lstrDollars = lstrOnes[lintLastTwo] + " " + lstrDollars;
                        }
                        else
                        {
                            // otherwise, use both "tens" and "ones" array
                            lstrDollars = lstrTens[lintDig2] + " " + lstrOnes[lintDig3] + " " + lstrDollars;
                        }
                        if (astrInputNum.Length < 3)
                        {
                            return lstrDollars + " dollars";
                        }
                    }

                    // if a hundreds part is there, translate it
                    if (lintDig1 > 0)
                    {
                        lstrDollars = lstrOnes[lintDig1] + " hundred " + lstrDollars;
                    }
                    astrInputNum = (astrInputNum.Length - 3) > 0 ? astrInputNum.Substring(0, astrInputNum.Length - 3) : "";
                    lintLevel++;
                }
            }
            return lstrDollars + " dollars";
        }

        #region Extension Methods
        /// <summary>
        /// Formats a string with a list of literal placeholders.
        /// </summary>
        /// <param name="astrText">The extension text</param>
        /// <param name="args">The argument list</param>
        /// <returns>The formatted string</returns>
        public static string Format(this string astrText, params object[] args)
        {
            return string.Format(astrText, args);
        }

        /// <summary>
        /// Determines whether a substring exists within a string.
        /// </summary>
        /// <param name="astrText">String to search.</param>
        /// <param name="astrSubString">Substring to match when searching.</param>
        /// <param name="ablnCaseSensitive">Determines whether or not to ignore case.</param>
        /// <returns>Indicator of substring presence within the string.</returns>
        public static bool Contains(this string astrText, string astrSubString, bool ablnCaseSensitive)
        {
            if (ablnCaseSensitive)
            {
                return astrText.Contains(astrSubString);
            }
            else
            {
                return astrText.ToLower().IndexOf(astrSubString.ToLower(), 0) >= 0;
            }
        }

        /// <summary>
        /// Detects if a string can be parsed to a valid date.
        /// </summary>
        /// <param name="astrText">Value to inspect.</param>
        /// <returns>Whether or not the string is formatted as a date.</returns>
        public static bool IsDate(this string astrText)
        {
            try
            {
                System.DateTime dtDateTime = System.DateTime.Parse(astrText);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified string is null or empty.
        /// </summary>
        /// <param name="astrText">The string value to check.</param>
        /// <returns>Boolean indicating whether the string is null or empty.</returns>
        public static bool IsEmpty(this string astrText)
        {
            return (astrText == null) || (astrText.Length == 0);
            // return astrText.Equals("\"\"") ? true : astrText.Equals("") ? true : false;
        }

        /// <summary>
        /// Determines whether the specified string is not null or empty.
        /// </summary>
        /// <param name="astrText">The string value to check.</param>
        /// <returns>Boolean indicating whether the string is not empty</returns>
        public static bool IsNotEmpty(this string astrText)
        {
            return (!astrText.IsEmpty());
        }

        /// <summary>
        /// Checks whether the string is null and returns a default value in case.
        /// </summary>
        /// <param name="astrDefaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfNull(this string astrText, string astrDefaultValue)
        {
            return (astrText.IsNull() ? astrText : astrDefaultValue);
        }

        /// <summary>
        /// Determines whether the string is empty and returns a default value in case.
        /// </summary>
        /// <param name="astrDefaultValue">The default value.</param>
        /// <returns>Either the string or the default value.</returns>
        public static string IfEmpty(this string astrText, string astrDefaultValue)
        {
            return (astrText.IsNotEmpty() ? astrText : astrDefaultValue);
        }

        /// <summary>
        /// Determines whether the specified object is null
        /// </summary>
        /// <returns>Boolean indicating whether the object is null</returns>
        public static bool IsNull(this object aobjObject)
        {
            return object.ReferenceEquals(aobjObject, null);
        }

        /// <summary>
        /// Determines whether the specified object is not null
        /// </summary>
        /// <returns>Boolean indicating whether the object is not null</returns>
        public static bool IsNotNull(this object aobjObject)
        {
            return !aobjObject.IsNull();
        }

        /// <summary>
        /// Determines whether the string is not null or empty.
        /// </summary>
        /// <returns>Boolean indicating whether the string is not null or not empty</returns>
        public static bool IsNotNullOrEmpty(this string astrText)
        {
            return !String.IsNullOrEmpty(astrText);
        }

        /// <summary>
        /// Creates a type from the given name
        /// </summary>
        /// <typeparam name="T">The type being created</typeparam>      
        /// <param name="args">Arguments to pass into the constructor</param>
        /// <returns>An instance of the type</returns>
        public static T CreateType<T>(this string astrTypeName, params object[] args)
        {
            Type type = Type.GetType(astrTypeName, true, true);
            return (T)Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// Determines if a string can be converted to an integer.
        /// </summary>
        /// <returns>True if the string is numeric.</returns>
        public static bool IsNumeric(this string astrText)
        {
            System.Text.RegularExpressions.Regex regularExpression = new System.Text.RegularExpressions.Regex("^-[0-9]+$|^[0-9]+$");
            return regularExpression.Match(astrText).Success;
        }

        /// <summary>
        /// Detects whether this instance is a valid email address.
        /// </summary>
        /// <returns>True if instance is valid email address</returns>
        public static bool IsValidEmailAddress(this string astrText)
        {
            return IsValidEmail(astrText);
        }

        /// <summary>
        /// Detects whether the supplied string is a valid IP address.
        /// </summary>
        /// <returns>True if the string is valid IP address.</returns>
        public static bool IsValidIPAddress(this string astrText)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(astrText, @"\b(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\b");
        }

        /// <summary>
        /// Checks if url is valid.
        /// </summary>
        /// <returns>True if the url is valid.</returns>
        public static bool IsValidUrl(this string astrURL)
        {
            string lstrRegex = "^(https?://)"
                + "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" // user@
                + @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184
                + "|" // allows either IP or domain
                + @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www.
                + @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]" // second level domain
                + @"(\.[a-z]{2,6})?)" // first level domain- .com or .museum is optional
                + "(:[0-9]{1,5})?" // port number- :80
                + "((/?)|" // a slash isn't required if there is no file name
                + "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";

            return new System.Text.RegularExpressions.Regex(lstrRegex).IsMatch(astrURL);
        }

        /// <summary>
        /// Retrieves the left x characters of a string.
        /// </summary>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        public static string Left(this string astrText, int count)
        {
            return astrText.Substring(0, count);
        }

        /// <summary>
        /// Retrieves the right x characters of a string.
        /// </summary>
        /// <param name="count">The number of characters to retrieve.</param>
        /// <returns>The resulting substring.</returns>
        public static string Right(this string astrText, int count)
        {
            return astrText.Substring(astrText.Length - count, count);
        }

        /// <summary>
        /// Capitalizes the first letter of a string
        /// </summary>      
        public static string Capitalize(this string astrText)
        {
            if (astrText.Length == 0)
            {
                return astrText;
            }
            if (astrText.Length == 1)
            {
                return astrText.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            }
            return astrText.Substring(0, 1).ToUpper(System.Globalization.CultureInfo.InvariantCulture) + astrText.Substring(1);
        }

        /// <summary>
        /// Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="astrRegexPattern">The regular expression pattern.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var isMatching = s.IsMatchingTo(@"^\d+$");
        /// </code>
        /// </example>
        public static bool IsMatchingTo(this string astrText, string astrRegexPattern)
        {
            return IsMatchingTo(astrText, astrRegexPattern, System.Text.RegularExpressions.RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to determine if the string matches to a given regex pattern.
        /// </summary>
        /// <param name="aRegexPattern">The regular expression pattern.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>
        /// 	<c>true</c> if the value is matching to the specified pattern; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var isMatching = s.IsMatchingTo(@"^\d+$");
        /// </code>
        /// </example>
        public static bool IsMatchingTo(this string astrText, string aRegexPattern, System.Text.RegularExpressions.RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(astrText, aRegexPattern, options);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="aRegexPattern">The regular expression pattern.</param>
        /// <param name="astrReplaceValue">The replacement value.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// </code>
        /// </example>
        public static string ReplaceWith(this string value, string aRegexPattern, string astrReplaceValue)
        {
            return ReplaceWith(value, aRegexPattern, astrReplaceValue, System.Text.RegularExpressions.RegexOptions.None);
        }

        /// <summary>
        /// Uses regular expressions to replace parts of a string.
        /// </summary>
        /// <param name="astrRegexPattern">The regular expression pattern.</param>
        /// <param name="astrReplaceValue">The replacement value.</param>
        /// <param name="options">The regular expression options.</param>
        /// <returns>The newly created string</returns>
        /// <example>
        /// <code>
        /// var s = "12345";
        /// var replaced = s.ReplaceWith(@"\d", m => string.Concat(" -", m.Value, "- "));
        /// </code>
        /// </example>
        public static string ReplaceWith(this string astrText, string astrRegexPattern, string astrReplaceValue, System.Text.RegularExpressions.RegexOptions options)
        {
            return System.Text.RegularExpressions.Regex.Replace(astrText, astrRegexPattern, astrReplaceValue, options);
        }

        /// <summary>
        /// A case insenstive replace function.
        /// </summary>
        /// <param name="astrText">The string to examine.</param>
        /// <param name="astrOldString">The new value to be inserted.</param>
        /// <param name="astrNewString">The value to replace.</param>
        /// <param name="ablnCaseSensitive">Determines whether or not to ignore case.</param>
        /// <returns>The resulting string.</returns>
        public static string Replace(this string astrText, string astrOldString, string astrNewString, bool ablnCaseSensitive)
        {
            if (ablnCaseSensitive)
            {
                return astrText.Replace(astrOldString, astrNewString);
            }
            else
            {
                System.Text.RegularExpressions.Regex aRegex = new System.Text.RegularExpressions.Regex(astrOldString, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);

                return aRegex.Replace(astrText, astrNewString);
            }
        }

        /// <summary>
        /// Reverses a string.
        /// </summary>
        /// <param name="astrText">The string to reverse.</param>
        /// <returns>The resulting string.</returns>
        public static string Reverse(this string astrText)
        {
            char[] arrChar = astrText.ToCharArray();
            Array.Reverse(arrChar);
            return new string(arrChar);
        }

        /// <summary>
        /// Splits a string into an array by delimiter.
        /// </summary>
        /// <param name="astrText">String to split.</param>
        /// <param name="delimiter">Delimiter string.</param>
        /// <returns>Array of strings.</returns>
        public static string[] Split(this string astrText, string delimiter)
        {
            return astrText.Split(delimiter.ToCharArray());
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters until the next whitespace on or after 
        /// the total character count has been reached for that line.  
        /// Uses the environment new line symbol for the break text.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aCharCount">The number of characters per line.</param>
        /// <returns>The resulting string.</returns>
        public static string WordWrap(this string astrText, int aCharCount)
        {
            return WordWrap(astrText, aCharCount, false, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cutOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the environment new line
        /// symbol for the break text.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aCharCount">The number of characters per line.</param>
        /// <param name="ablnCutOff">If true, will break in the middle of a word.</param>
        /// <returns>The resulting string.</returns>
        public static string WordWrap(this string astrText, int aCharCount, bool ablnCutOff)
        {
            return WordWrap(astrText, aCharCount, ablnCutOff, Environment.NewLine);
        }

        /// <summary>
        /// Wraps the passed string up the total number of characters (if cutOff is true)
        /// or until the next whitespace (if cutOff is false).  Uses the supplied breakText
        /// for line breaks.
        /// </summary>
        /// <param name="astrText">The string to wrap.</param>
        /// <param name="aCharCount">The number of characters per line.</param>
        /// <param name="ablnCutOff">If true, will break in the middle of a word.</param>
        /// <param name="astrBreakText">The line break text to use.</param>
        /// <returns>The resulting string</returns>
        public static string WordWrap(this string astrText, int aCharCount, bool ablnCutOff, string astrBreakText)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(astrText.Length + 100);
            int counter = 0;

            if (ablnCutOff)
            {
                while (counter < astrText.Length)
                {
                    if (astrText.Length > counter + aCharCount)
                    {
                        sb.Append(astrText.Substring(counter, aCharCount));
                        sb.Append(astrBreakText);
                    }
                    else
                    {
                        sb.Append(astrText.Substring(counter));
                    }

                    counter += aCharCount;
                }
            }
            else
            {
                string[] strings = astrText.Split(' ');

                for (int i = 0; i < strings.Length; i++)
                {
                    counter += strings[i].Length + 1; // the added one is to represent the inclusion of the space.

                    if (i != 0 && counter > aCharCount)
                    {
                        sb.Append(astrBreakText);
                        counter = 0;
                    }

                    sb.Append(strings[i] + ' ');
                }
            }

            return sb.ToString().TrimEnd(); // to get rid of the extra space at the end.
        }

        /// <summary>
        /// Converts String to Any Other Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="astrText">The input.</param>
        /// <returns>Return converted type</returns>
        public static T? ConvertTo<T>(this string astrText) where T : struct
        {
            T? ret = null;

            if (!string.IsNullOrEmpty(astrText))
            {
                ret = (T)Convert.ChangeType(astrText, typeof(T));
            }

            return ret;
        }

        /// <summary>
        /// Converts String to Any Other Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="astrText">The input.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static T? ConvertTo<T>(this string astrText, IFormatProvider provider) where T : struct
        {
            T? ret = null;

            if (!string.IsNullOrEmpty(astrText))
            {
                ret = (T)Convert.ChangeType(astrText, typeof(T), provider);

            }

            return ret;
        }

        /// <summary>
        /// Returns string converted from char.
        /// </summary>
        /// <param name="achrText"></param>
        /// <returns>Return string</returns>
        public static string ToString(this char? achrText)
        {
            return achrText.HasValue ? achrText.Value.ToString() : String.Empty;
        }

        /// <summary>
        /// Returns a Boolean value indicating whether a variable is of the indicated type.
        /// </summary>
        /// <param name="aobjObject">Object instance.</param>
        /// <param name="atypType">The Type to check the object against.</param>
        /// <returns>Result of the comparison.</returns>
        public static object IsType(this object aobjObject, Type atypType)
        {
            return aobjObject.GetType().Equals(atypType);
        }

        /// <summary>
        /// Creates an instance of the generic type specified using the default constructor.
        /// </summary>
        /// <typeparam name="T">The type to instantiate.</typeparam>
        /// <param name="atypType">The System.Type being instantiated.</param>
        /// <returns>An instance of the specified type.</returns>
        /// <example>
        /// typeof(MyObject).CreateInstance();
        /// </example>
        public static T CreateInstance<T>(this System.Type atypType) where T : new()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Determines whether an expression evaluates to the DBNull class.
        /// </summary>
        /// <param name="aobjObject">Object instance.</param>
        /// <returns>Returns true if the object is DBNull.</returns>
        public static object IsDBNull(this object aobjObject)
        {
            return aobjObject.IsType(typeof(DBNull));
        }

        /// <summary>
        /// Rounds the supplied decimal to the specified amount of decimal points.
        /// </summary>
        /// <param name="adecValue">The decimal to round.</param>
        /// <param name="aintDecimalPoints">The number of decimal points to round the output value to.</param>
        /// <returns>A rounded decimal.</returns>
        public static decimal RoundDecimalPoints(this decimal adecValue, int aintDecimalPoints)
        {
            return Math.Round(adecValue, aintDecimalPoints);
        }

        /// <summary>
        /// Rounds the supplied decimal value to two decimal points.
        /// </summary>
        /// <param name="adecValue">The decimal to round.</param>
        /// <returns>A decimal value rounded to two decimal points.</returns>
        public static decimal RoundToTwoDecimalPoints(this decimal adecValue)
        {
            return Math.Round(adecValue, 2);
        }

        /// <summary>
        /// Determine whether the collection/list is null or empty;
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Returns true if the list is null or empty.</returns>
        public static bool IsNullOrEmpty(this System.Collections.IEnumerable list)
        {
            return list == null ? true : list.GetEnumerator().MoveNext() == false;
        }

        /// <summary>
        /// Determine whether the collection/list is empty
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Returns true if the list is empty.</returns>
        public static bool IsEmpty(this System.Collections.IEnumerable list)
        {
            return list == null ? true : list.GetEnumerator().MoveNext() == false;
        }

        /// <summary>
        /// Checks a System.Type to see if it implements a given interface.
        /// </summary>
        /// <param name="source">The System.Type to check.</param>
        /// <param name="iface">The System.Type interface to check for.</param>
        /// <returns>True if the source implements the interface type, false otherwise.</returns>
        public static bool IsImplementationOf(this Type source, Type interfaceType)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return source.GetInterface(interfaceType.FullName) != null;
        }

        /// <summary>
        /// Set Empty string if input date is minimum value
        /// </summary>
        /// <param name="adtDateTime">Datetime</param>
        /// <returns>
        /// If input datetime is minimum value return empty string
        /// Else short date string representation of the input date
        /// </returns>
        public static string SetEmptyStringForMinDate(this DateTime adtDateTime)
        {
            if (adtDateTime == DateTime.MinValue)
                return string.Empty;
            else
                return adtDateTime.ToShortDateString();
        }

        /// <summary>
        /// To convert the input string to InitCap case
        /// </summary>
        /// <param name="astrInputString">Input String</param>
        /// <param name="ablnIncludeExceptionWords">Flag to include exception words</param>
        /// <returns></returns>
        public static string ToInitCapCase(this string astrInputString, bool ablnIncludeExceptionWords)
        {
            string[] lstrExceptionArray = new string[] { "of", "in" };
            string[] lstrInitCapCase = astrInputString.Trim().Split(" ");
            astrInputString = string.Empty;
            for (int intCnt = 0; intCnt < lstrInitCapCase.Length; intCnt++)
            {
                bool lblnFlag = false;

                if (ablnIncludeExceptionWords)
                {
                    for (int intCount = 0; intCount < lstrExceptionArray.Length; intCount++)
                    {
                        if (lstrInitCapCase[intCnt].Equals(lstrExceptionArray[intCount]))
                        {
                            lblnFlag = true;
                            break;
                        }
                    }
                }

                if (lblnFlag)
                    astrInputString += lstrInitCapCase[intCnt] + " ";
                else
                    astrInputString += lstrInitCapCase[intCnt].Capitalize() + " ";
            }

            return astrInputString.Trim();
        }

        /// <summary>
        /// Get the Last day of the month
        /// </summary>
        /// <param name="adtDateTime">Given date</param>
        /// <returns>Last day of given month</returns>
        public static DateTime GetLastDayofMonth(this DateTime adtDateTime)
        {
            if (adtDateTime != DateTime.MinValue)
            {
                return new DateTime(adtDateTime.Year, adtDateTime.Month, DateTime.DaysInMonth(adtDateTime.Year, adtDateTime.Month));
            }
            return adtDateTime;
        }

        /// <summary>
        /// Get the First day of the next month
        /// </summary>
        /// <param name="adtDateTime">Given date</param>
        /// <returns>First date of next month</returns>
        public static DateTime GetFirstDayofNextMonth(this DateTime adtDateTime)
        {
            if (adtDateTime != DateTime.MinValue)
            {
                DateTime ldtTempDate = adtDateTime.AddMonths(1);
                return new DateTime(ldtTempDate.Year, ldtTempDate.Month, 01);
            }
            return adtDateTime;
        }

        /// <summary>
        /// Get the First day of the next month
        /// </summary>
        /// <param name="adtDateTime">Given date</param>
        /// <returns>First date of next month</returns>
        public static DateTime GetFirstDayofCurrentMonth(this DateTime adtDateTime)
        {
            if (adtDateTime != DateTime.MinValue)
            {
                return new DateTime(adtDateTime.Year, adtDateTime.Month, 01);
            }
            return adtDateTime;
        }

        public static string ConvertMinValueAsEmpty(this DateTime adtDateTime, string astrDateFormat = null)
        {
            if (adtDateTime == DateTime.MinValue)
            {
                return string.Empty;
            }

            if (astrDateFormat != null)
                return adtDateTime.ToString(astrDateFormat);
            return astrDateFormat.ToString();
        }

        #endregion

        /// Returns true if the last character of the given string is *
        /// Returns false if the last character of the given string is not *
        public static bool IsLastCharacterAsterisk(string astrValue)
        {
            if (astrValue.Length > 0)
            {
                string lstrTemp = astrValue.Substring(astrValue.Length - 1, 1);
                if (lstrTemp == "*")
                    return true;
                return false;
            }
            return false;
        }

        /// Returns the string by removing the last character
        public static string RemoveLastCharacter(string astrValue)
        {
            string lstrTemp = string.Empty;
            if (astrValue.Length > 0)
                lstrTemp = astrValue.Substring(0, astrValue.Length - 1);
            return lstrTemp;
        }

        public static string FormatStringtoPreventSQLInjection(string astrInput)
        {
            string lstrOutput = astrInput.Trim().Replace("'", "''");
            return lstrOutput;
        }

        /// HIPAA file format of Zip code.
        public static string GetValidZipCode(string astrZipCode, string astrZip4Code)
        {
            string lstrZipCode = string.Empty;
            if (!string.IsNullOrEmpty(astrZipCode))
            {
                if (astrZipCode.Length == 5)
                    lstrZipCode = astrZipCode;
                else
                    lstrZipCode = "00000";
                if (!string.IsNullOrEmpty(astrZip4Code))
                {
                    if (astrZip4Code.Length == 4)
                        lstrZipCode += astrZip4Code;
                }
            }
            return lstrZipCode;
        }

        public static void SendMail(string astrFrom, string astrTo, string astrHeading, string astrMessage, bool ablnHighPriority, bool ablnHtmlFormat)
        {
            if ((astrTo != null) && (astrTo.Trim() != ""))
            {
                string appSettings = NeoSpin.Common.ApplicationSettings.Instance.SmtpServer;
                foreach (string lstrTo in astrTo.Split(';'))
                {
                    string lstrParsedTo = lstrTo.Replace(";", "");
                    if (lstrParsedTo.IsNotNullOrEmpty())
                    {
                        MailMessage message = new MailMessage(astrFrom, lstrParsedTo);
                        message.Subject = astrHeading;
                        message.Body = astrMessage;
                        message.IsBodyHtml = ablnHtmlFormat;
                        if (ablnHighPriority)
                            message.Priority = MailPriority.High;
                        new SmtpClient(appSettings).Send(message);
                    }
                }
            }
        }

        public static void SendMailRetryOnFail(string astrFrom, string astrTo, string astrHeading, string astrMessage, bool ablnHighPriority, bool ablnHtmlFormat)
        {
            if ((astrTo != null) && (astrTo.Trim() != ""))
            {
                //string appSettings = NeoSpin.Common.ApplicationSettings.Instance.SmtpServer;
                string appSettings = NeoSpin.Common.ApplicationSettings.Instance.SmtpServers;
                string[] lstrAppSettings = appSettings.Split(";");
                byte lbytSMTPIndex = 0;
                foreach (string lstrTo in astrTo.Split(';'))
                {
                    string lstrParsedTo = lstrTo.Replace(";", "");
                    if (lstrParsedTo.IsNotNullOrEmpty())
                    {
                        MailMessage message = new MailMessage(astrFrom, lstrParsedTo);
                        message.Subject = astrHeading;
                        message.Body = astrMessage;
                        message.IsBodyHtml = ablnHtmlFormat;
                        if (ablnHighPriority)
                            message.Priority = MailPriority.High;
                        RetrySending:
                        try
                        {
                            SmtpClient lsmtpClient = new SmtpClient(lstrAppSettings[lbytSMTPIndex]);
                            lsmtpClient.Timeout = NeoSpin.Common.ApplicationSettings.Instance.SmtpTimeOut;
                            lsmtpClient.Send(message);
                        }
                        catch (Exception ex)
                        {
                            ExceptionManager.Publish(ex);
                            lbytSMTPIndex++;
                            if (lbytSMTPIndex < lstrAppSettings.Length)
                                goto RetrySending;
                            else
                                throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// method to retrieve message text from message table
        /// </summary>
        /// <param name="aintMessageID">message id</param>
        /// <param name="aobjPassInfo">utlpassinfo</param>
        /// <returns>message text</returns>
        public static string GetMessageTextByMessageID(int aintMessageID, utlPassInfo aobjPassInfo)
        {
            string lstrMessage = string.Empty;
            DataTable ldtMessage = aobjPassInfo.isrvDBCache.GetCacheData("sgs_messages", "message_id=" + aintMessageID.ToString());
            if (ldtMessage.Rows.Count > 0)
            {
                lstrMessage = ldtMessage.Rows[0]["display_message"] != DBNull.Value ? ldtMessage.Rows[0]["display_message"].ToString() : string.Empty;
            }
            return lstrMessage;
        }

        /// <summary>
        /// method to retrieve DashBoard message text from DashBoard message table
        /// </summary>
        /// <param name="aintMessageID">DashBoard message id</param>
        /// <param name="aobjPassInfo">utlpassinfo</param>
        /// <returns>message text</returns>
        public static string GetDBMessageTextByDBMessageID(int aintMessageID, utlPassInfo aobjPassInfo, ref string astrPriorityValue)
        {
            string lstrMessage = string.Empty;
            DataTable ldtMessage = aobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_dashboard_messages", "wss_dashboard_messages_id=" + aintMessageID.ToString());
            if (ldtMessage.Rows.Count > 0)
            {
                lstrMessage = ldtMessage.Rows[0]["message_text"] != DBNull.Value ? ldtMessage.Rows[0]["message_text"].ToString() : string.Empty;
                astrPriorityValue = ldtMessage.Rows[0]["priority_value"] != DBNull.Value ? ldtMessage.Rows[0]["priority_value"].ToString() : string.Empty;
            }
            return lstrMessage;
        }
        
       
        /// <summary>
        /// pir 5465 
        /// employment start date should be between Jan 1 1980 to dec 31,2099
        /// </summary>
        /// <param name="adtDateTime"></param>
        /// <returns></returns>
        private static bool IsWithinMaxMinDates(DateTime adtDateTime)
        {
            DateTime minDate = Convert.ToDateTime("01/01/1980");
            DateTime maxDate = Convert.ToDateTime("12/31/2099");
            return (adtDateTime >= minDate && adtDateTime <= maxDate);
        }


        /// <summary>
        /// pir 5465: 
        /// employment start date should be between Jan 1 1980 to dec 31,2099
        /// it cannot be greater than 2 months from current date
        /// </summary>
        /// <param name="adtstartDate">start date</param>
        /// <returns>true if employment start date is valid</returns>
        public static bool CheckEmploymentStartDate(DateTime adtstartDate)
        {
            return (adtstartDate != DateTime.MinValue && IsWithinMaxMinDates(adtstartDate) && adtstartDate <= DateTime.Now.AddMonths(2));
        }

        /// <summary>
        /// pir 5465: 
        /// employment end date should be between Jan 1 1980 to dec 31,2099
        /// it cannot be greater than 6 months from current date
        /// </summary>
        /// <param name="adtEndDate"></param>
        /// <returns></returns>
        public static bool CheckEmploymentEndDate(DateTime adtEndDate)
        {
            return (adtEndDate != DateTime.MinValue && IsWithinMaxMinDates(adtEndDate) && adtEndDate <= DateTime.Now.AddMonths(6));
        }

        public static busOrganization GetProviderOrgByPlan(int aintPlanID)
        {
            return GetProviderOrgByPlan(aintPlanID, DateTime.Now);
        }

        public static busOrganization GetProviderOrgByPlan(int aintPlanID, DateTime adteGivenDate)
        {
            busOrganization lobjProviderOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            DataTable ldtbResult = busBase.Select("cdoPersonAccount.LoadActiveProviderByPlan", new object[2] { aintPlanID, adteGivenDate });
            if (ldtbResult.Rows.Count > 0)
                lobjProviderOrg.icdoOrganization.LoadData(ldtbResult.Rows[0]);
            return lobjProviderOrg;
        }
        //PIR-11030 Start
        public static string GetCodeValueDetailsfromData1(int aintCodeId, string astrData1, utlPassInfo aobjPassInfo)
        {
            string lstrResult = null;
            string lstrWhere = "code_id = '" + aintCodeId.ToString() + "' ";
            lstrWhere += "and data1 = '" + astrData1 + "' ";
            DataTable ldtbCodeValue = aobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);

            if (ldtbCodeValue.Rows.Count > 0)
            {
                lstrResult = ldtbCodeValue.Rows[0]["CODE_VALUE"].ToString();
            }
            return lstrResult;
        }
        //PIR-11030 End

        //PIR 12532
        public static DateTime GetLastDayOfEffectiveYear(DateTime adtDate)
        {
            DateTime adtLastDayOfYear = new DateTime(adtDate.Year, 12, 31);
            return adtLastDayOfYear;
        }

        //Backlog PIR 1869
        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, other);
                ms.Position = 0;
                return (T)formatter.Deserialize(ms);
            }
        }

        //PIR 15347 - Medicare Part D bookmarks
        public static decimal GetTotalMedicarePremiumAmountFromIBS(int aintPersonid, DateTime adtBillingMonth)
        {
            decimal TotalMedicarePremium = 0;
            DataTable ldtAmt = busBase.Select("cdoIbsDetail.GetTotalMedicarePremiumAmount", new object[2] { aintPersonid, adtBillingMonth });
            if (ldtAmt.Rows.Count > 0)
            {
                TotalMedicarePremium = Convert.ToDecimal(ldtAmt.Rows[0][0]);
            }
            return TotalMedicarePremium;
        }
        //PIR 16143 - btnPdfCorrespondence_Click not supported by 5.0 Framework
        public static string GetFileNameFromDownloadFile(this string astrFileName)
        {
            return astrFileName.ToLower().StartsWith("pdf") ? string.Concat(astrFileName.Substring(3), ".pdf") : astrFileName;
        }

        public static void CopyPropertyValues(object source, object destination)
        {
            var destProperties = destination.GetType().GetProperties();

            foreach (var sourceProperty in source.GetType().GetProperties())
            {
                foreach (var destProperty in destProperties)
                {
                    if (destProperty.Name == sourceProperty.Name &&
                destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType) &&
                        destProperty.SetMethod.IsNotNull() && destProperty.DeclaringType.Name != "doBase")
                    {
                        destProperty.SetValue(destination, sourceProperty.GetValue(
                            source, new object[] { }), new object[] { });

                        break;
                    }
                }
            }
        }
        /// <summary>
        /// PIR 17140 - Putting calculation in review if additional contributions come in;
        /// this method gets called from person account adjustment on post_click,
        /// interest posting batch, and employer report posting
        /// When called from interest posting batch sending all pending for approval calculations as parameter(adtbPendingApprovalCalculations)
        /// for performance.
        /// </summary>
        /// <param name="aintPersonAccountId"></param>
        /// <param name="adtbPendingApprovalCalculations"></param>
        public static bool PutCalculationInReviewIfExists(int aintPersonAccountId, DataTable adtbPendingApprovalCalculations = null)
        {
            bool lblnCalcReview = false;
            if (aintPersonAccountId > 0)
            {
                busPersonAccount lbusPersonAccount = new busPersonAccount();
                if (lbusPersonAccount.FindPersonAccount(aintPersonAccountId))
                {
                    DataTable ldtbPendingApprovalBenefitCalculations = null;
                    if (adtbPendingApprovalCalculations.IsNull())
                        ldtbPendingApprovalBenefitCalculations = busBase.Select("cdoBenefitCalculation.LoadPendingApprovalCalculations",
                                                                        new object[1] { aintPersonAccountId });
                    else
                        ldtbPendingApprovalBenefitCalculations = FilterTable(adtbPendingApprovalCalculations, busConstant.DataType.Numeric, "PERSON_ACCOUNT_ID", aintPersonAccountId, null);
                    if (ldtbPendingApprovalBenefitCalculations.Rows.Count > 0)
                    {
                        busBase lbusbase = new busBase();
                        Collection<busBenefitCalculation> lclcPendingApprovalBenefitCalculations =
                            lbusbase.GetCollection<busBenefitCalculation>(ldtbPendingApprovalBenefitCalculations, "icdoBenefitCalculation");
                        foreach (busBenefitCalculation lbusBenefitCalculation in lclcPendingApprovalBenefitCalculations)
                        {
                            return lbusBenefitCalculation.PutCalcInReview();
                        }
                    }
                }
            }
            return lblnCalcReview;
        }
        public static DataTable FilterTable<T>(DataTable source, busConstant.DataType dataType, string filterFieldName, T filterFieldValue, string astrDummy = null)
        {
            DataTable ldtbResuls = new DataTable();
            string lstrFilterString = string.Empty;
            if (dataType == busConstant.DataType.String)
                lstrFilterString = filterFieldName + "= '" + filterFieldValue + "'";
            else
                lstrFilterString = filterFieldName + "=" + filterFieldValue;
            source.DefaultView.RowFilter = lstrFilterString;
            ldtbResuls = source.DefaultView.ToTable();
            return ldtbResuls;
        }
        public static void UpdateBenActTaxAndNonTaxableAmts(int aintBenAcctId, decimal adecTaxableAmt, decimal adecNonTaxableAmt)
        {
            busBenefitAccount lbusBenefitAccount = new busBenefitAccount();
            if (lbusBenefitAccount.FindBenefitAccount(aintBenAcctId))
            {
                lbusBenefitAccount.icdoBenefitAccount.starting_taxable_amount += adecTaxableAmt;
                lbusBenefitAccount.icdoBenefitAccount.starting_nontaxable_amount += adecNonTaxableAmt;
                lbusBenefitAccount.icdoBenefitAccount.Update();
            }
        }
        // PIR-17512, for new interest rate based on date of purchase.   //POINT 1
        public static decimal GetCodeValueDetailsfromData2(int aintCodeId, DateTime adteEffectiveDate, utlPassInfo aobjPassInfo)
        {
            string lstrWhere = "code_id = '" + aintCodeId.ToString() + "' ";       
			DataTable ldtbCodeValue = aobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);
			return Convert.ToDecimal(ldtbCodeValue.AsEnumerable().OrderByDescending(i => Convert.ToDateTime(i.Field<string>("DATA2"))).Where(i => Convert.ToDateTime(i.Field<string>("DATA2")) <= adteEffectiveDate.Date).AsDataTable().Rows[0]["DATA1"]);       }

        /// <summary>
        /// PIR 18085 - rounded down if less than .26 and up if .26 or greater to the full year, i.e., 46.23 => 46; 46.26 => 47
        /// </summary>
        /// <param name="adecInput"></param>
        /// <param name="adecCustomRoundDecPart"></param>
        /// <returns></returns>
        public static int CustomRound(decimal adecInput, decimal adecCustomRoundDecPart = 0.26M)
        {
            return (adecInput - (int)adecInput) < adecCustomRoundDecPart ? Convert.ToInt32(Math.Floor(adecInput)) : Convert.ToInt32(Math.Ceiling(adecInput));
        }

        public static void MoveFileFromProcessedToErrorFolder(cdoFileHdr acdoFileHdr, utlPassInfo aobjPassInfo)
        {
            Collection<utlFilePaths> filePaths = aobjPassInfo.isrvDBCache.GetFilePaths();
            utlFilePaths lutlFilePath = filePaths.FirstOrDefault(fp => fp.iintFileId == acdoFileHdr.file_id);
            if (lutlFilePath.IsNotNull())
            {
                if (File.Exists(lutlFilePath.istrProcessDirectory + acdoFileHdr.processed_file_name))
                {
                    File.Move(lutlFilePath.istrProcessDirectory + acdoFileHdr.processed_file_name, lutlFilePath.istrErrorDirectory + acdoFileHdr.processed_file_name);
                }
            }
        }
		#region PIR-18492
        
        static Random irand = new Random();
        public static string GenerateAnOTP()
        {
            int llngrandnumber = irand.Next(0, 999999999);
            busOtp lbusOtp = new busOtp((ulong)llngrandnumber);
            return lbusOtp.GetCurrentOTP();
        }

        /// <summary>
        /// PIR 18492 - This function is used to get user IP address and location details.
        /// As discussed with vasu,due to security concern below free third party api code is commented.
        /// </summary>
        /// <param name="astrLoginCountry"></param>
        /// <param name="astrLoginState"></param>
        /// <param name="astrLoginCity"></param>
        //public static void GetIPLocation(string astrIPAddress,ref string astrLoginCountry, ref string astrLoginState, ref string astrLoginCity)
        //{
        //    try
        //    {
        //        string lstripResponse;
        //        lstripResponse = IPRequestHelper("http://ip-api.com/xml/" + astrIPAddress);
        //        using (TextReader sr = new StringReader(lstripResponse))
        //        {
        //            using (System.Data.DataSet ldsUserLocation = new System.Data.DataSet())
        //            {
        //                ldsUserLocation.ReadXml(sr);

        //                if (!string.IsNullOrEmpty(ldsUserLocation.Tables[0].Rows[0]["country"].ToString()))
        //                {
        //                    astrLoginCountry = ldsUserLocation.Tables[0].Rows[0]["country"].ToString();
        //                }
        //                if (!string.IsNullOrEmpty(ldsUserLocation.Tables[0].Rows[0]["regionName"].ToString()))
        //                {
        //                    astrLoginState = ldsUserLocation.Tables[0].Rows[0]["regionName"].ToString();
        //                }
        //                if (!string.IsNullOrEmpty(ldsUserLocation.Tables[0].Rows[0]["city"].ToString()))
        //                {
        //                    astrLoginCity = ldsUserLocation.Tables[0].Rows[0]["city"].ToString();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        System.Collections.Specialized.NameValueCollection addlInfo = new System.Collections.Specialized.NameValueCollection();
        //        addlInfo.Add("GetIPLocation", "failed with error : " + Ex.Message);
        //        ExceptionManager.Publish(Ex, addlInfo);
        //    }
        //}

        //public static string IPRequestHelper(string url)
        //{
        //    HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
        //    HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
        //    StreamReader responseStream = new StreamReader(objResponse.GetResponseStream());
        //    string responseRead = responseStream.ReadToEnd();
        //    responseStream.Close();
        //    responseStream.Dispose();
        //    return responseRead;
        //}

        #endregion PIR-18492
        /// <summary>
        /// PIR 18691 date difference between given dates
        /// </summary>
        /// <param name="adtGivenDate"></param>
        /// <param name="adtStartDate"></param>
        /// <param name="adtEndDate"></param>
        /// <returns></returns>
        public static bool IsWithinGivenDates(DateTime adtGivenDate, DateTime adtStartDate, DateTime adtEndDate)
        {
            return (adtGivenDate >= adtStartDate && adtGivenDate <= adtEndDate);
        }

        public static ArrayList GenerateReportDataSet(utlPassInfo aobjPassInfo, DataSet ldstApplication, ReportModel aclsReportModel, string astrReportPath)
        {
            string lstrReportName = aclsReportModel.ReportName;
            string istrReportPath = astrReportPath;

            ArrayList larrResult = new ArrayList();
            busNeoSpinBase lbusNeoSpinBase = new busNeoSpinBase();

            byte[] lbyteFile = lbusNeoSpinBase.CreateReportOffline(ldstApplication, istrReportPath + lstrReportName + ".rpt", aclsReportModel);
            string lstrFileName = lstrReportName;
            larrResult.Add(lstrFileName + ".pdf");
            larrResult.Add(lbyteFile);
            return larrResult;
        }
        /// <summary>
        /// PIR 23734 - returns the code value data by code id
        /// This method will allow NDPERS to end date existing code values from showing in the drop list.
        /// </summary>
        /// <param name="aintCodeId"></param>
        /// <returns></returns>
        public static Collection<cdoCodeValue> LoadCodeValuesDataByCodeId(int aintCodeId) =>  Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(busNeoSpinBase.Select("entCodeValue.GetCodeValuesDataByCodeId", new object[1] { aintCodeId }));

        public static Collection<busOrgContact> LoadPrimaryAutOrAAByOrgId(int aintOrgId)
        {
            busOrganization lbusOrganization = new busOrganization() { icdoOrganization = new cdoOrganization() };
            lbusOrganization.FindOrganization(aintOrgId);
            if (lbusOrganization.iclbOrgContact == null)
                lbusOrganization.LoadOrgContact();
            return lbusOrganization.iclbOrgContact.Where(o => (o.icdoContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent ||
                                                                                    o.icdoContactRole.contact_role_value == busConstant.OrgContactRoleAuthorizedAgent) &&
                                                                                    o.icdoOrgContact.status_value == busConstant.OrgContactStatusActive &&
                                                                                    o.ibusContact.icdoContact.status_value == busConstant.OrgContactStatusActive)
                                                                    .OrderBy(orgcnt => orgcnt.icdoOrgContact.contact_id)
                                                                    .ToList()
                                                                    .ToCollection();
        }
        public static T CheckAndGetValue<T>(this DataRow adtrRow, string astrColumnName)
        {
            var lRetVal = default(T);
            if (adtrRow.Table.Columns.Contains(astrColumnName) && !Convert.IsDBNull(adtrRow[astrColumnName]))
            {
                lRetVal = (T)Convert.ChangeType(adtrRow[astrColumnName], typeof(T));
            }
            return lRetVal;
        }
        public static bool IsNullOrEmpty(this DataTable adtTable)
        {
            if (adtTable != null && adtTable.Rows.Count > 0)
            {
                return false;
            }
            return true;
        }
        public static string GetLatestBenefitProvisionTypeByEffectiveDate(DateTime adtEffectiveDate, int aintBenefitProvisionID)
        {
            string lstrtBenefitProvisionType = string.Empty;
            DataTable ldtDataResult = busBase.Select("entBenefitProvisionMultiplier.GetLatestBenefitProvisionTypeByEffectiveDate",
                                            new object[2] { aintBenefitProvisionID, adtEffectiveDate});
            if (ldtDataResult.Rows.Count >= 1)
            {
                lstrtBenefitProvisionType = ldtDataResult.Rows[0]["BENEFIT_MULTIPLIER_TYPE_VALUE"].ToString();
            }

            return lstrtBenefitProvisionType;
        }
    }
}
