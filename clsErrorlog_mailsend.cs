using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Excelicare.Framework.Dal.AppSupport;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Excelicare.Framework.AppSupport
{
    public class clsExceptionHandler
    {

        private clsExceptionHandler objExp;
        private string _expMessage;
        public string ExpMessage
        {
            get
            {
                return _expMessage;
            }
            set
            {
                _expMessage = value;
            }
        }
        #region Variable Declarations
        private Exception objException;
        private HttpSessionState objHTTPSessionState;
        private HttpRequest objHTTPRequest;
        private string strExceptionAsString;
        #endregion

        #region Constructors and Destructor

        // -----------------------------------------------------------------------------------------------
        // <summary>
        // This is constructor which intializes class private variables at the time of object creation.
        // </summary>
        // <param name="objException"></param>
        // <param name="objHTTPSessionState"></param>
        // <param name="objHTTPRequest"></param>
        // <remarks>
        // </remarks>
        // <history>
        // [Nagodaya Bhaskar]    12/03/2004  Created
        // [Ram]	             23/06/2005  Added Comments
        // </history>
        // -----------------------------------------------------------------------------------------------
        public clsExceptionHandler(Exception objException, HttpSessionState objHTTPSessionState, HttpRequest objHTTPRequest)
        {
            this.objException = objException;
            this.objHTTPSessionState = objHTTPSessionState;
            this.objHTTPRequest = objHTTPRequest;
        }

        // -----------------------------------------------------------------------------------------------
        // <summary>
        // This is constructor which intializes class private variables at the time of object creation.
        // </summary>
        // <param name="objException"></param>
        // <remarks>
        // </remarks>
        // <history>
        // [Nagodaya Bhaskar]    25/03/2004  Created
        // [Ram]	             23/06/2005  Added Comments
        // </history>
        // -----------------------------------------------------------------------------------------------
        public clsExceptionHandler(Exception objException)
        {
            this.objException = objException;
            objHTTPRequest = null;
            objHTTPSessionState = null;
        }

        // -----------------------------------------------------------------------------------------------
        // <summary>
        // This is constructor which intializes client script class variables at the time of object creation.
        // </summary>
        // <param name="objHTTPRequest"></param>
        // <param name="objHTTPSessionState"></param>
        // <remarks>
        // </remarks>
        // <history>
        // [Nagodaya Bhaskar]    06/10/2004  Created
        // [Ram]	             23/06/2005  Added Comments
        // </history>
        // -----------------------------------------------------------------------------------------------
        public clsExceptionHandler(HttpRequest objHTTPRequest, HttpSessionState objHTTPSessionState)
        {
            objException = null;
            this.objHTTPRequest = objHTTPRequest;
            this.objHTTPSessionState = objHTTPSessionState;
        }

        #endregion

        #region Custom Properties
                
        public string GetException(string dt) // DateTime) As String
        {
            if (strExceptionAsString is null)
                strExceptionAsString = GetExceptionToLog(dt);
            if (HttpContext.Current is not null)
                strExceptionAsString = strExceptionAsString + "  Session Id: " + HttpContext.Current.Session.SessionID + Constants.vbCrLf;
            return strExceptionAsString;
        }

        #endregion

        #region Exception Handler Methods

        public void LogException()
        {
            var dtUtc = DateTime.UtcNow;
            string dt = Strings.Format(DateTime.Now, "dd MMMM yyyy HH:mm:ss");


            if (objException is System.Threading.ThreadAbortException)
                return;
            // To show the alert message when DB sever is down
            if (objException is System.Data.SqlClient.SqlException)
            {
                System.Data.SqlClient.SqlException objEx;
                objEx = (System.Data.SqlClient.SqlException)objException;
                if (objEx.Number == 17)
                {
                    HttpContext.Current.Response.Write("<script language='javascript'>alert('Unable to Establish connection with ExceliDB Server.');</script>");
                    HttpContext.Current.Response.End();
                }
                else if (objEx.Number == 11)
                {
                    HttpContext.Current.Response.Write("<script language='javascript'>alert('Unable to Establish connection with ExceliDB Server.');</script>");
                    HttpContext.Current.Response.End();
                }
            }

            try
            {
                if (ConfigurationSettings.AppSettings["AxException_WriteToFile"].ToUpper() == "TRUE")
                {
                    string strLogFile = ConfigurationSettings.AppSettings["AxException_FileNameToWrite"];
                    WriteExceptionToLogFile(strLogFile, dt);
                }
            }
            catch (Exception ex)
            {
                clsDalExceptionHandlerDat.WriteExceptionToDataBase(ex, dtUtc);
            }

            try
            {
                if (ConfigurationSettings.AppSettings["AxException_WriteToDataBase"].ToUpper() == "TRUE")
                {
                    if (objHTTPRequest is null && objHTTPSessionState is null)
                    {
                        clsDalExceptionHandlerDat.WriteExceptionToDataBase(objException, dtUtc);
                    }
                    else
                    {
                        clsDalExceptionHandlerDat.WriteExceptionToDataBase(objException, objHTTPSessionState, objHTTPRequest, dtUtc);
                    }
                }
                if (ConfigurationManager.AppSettings["AxException_WriteToDBExpMsg"]?.ToUpper() == "TRUE")
                {
                    if (objHTTPRequest == null && objHTTPSessionState == null)
                    {
                        clsDalExceptionHandlerDat objexDal = new clsDalExceptionHandlerDat();
                        string strMsg = objexDal.WriteExceptionToDBExpMsg(objException, dtUtc);
                        ExpMessage = strMsg;
                    }
                    else
                    {
                        clsDalExceptionHandlerDat.WriteExceptionToDataBase(objException, objHTTPSessionState, objHTTPRequest, dtUtc);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            try
            {

                if ((ConfigurationManager.AppSettings["LogExceptionToWindowsEventLog"] + "").ToUpper() == "TRUE")
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["WindowsEventApplicationName"]))
                    {
                        WriteToWindowsEventLog(ConfigurationManager.AppSettings["WindowsEventApplicationName"], GetException(dt), EventLogEntryType.Error, 0);
                    }
                    else
                    {
                        WriteToWindowsEventLog("ExcelicarePortal", GetException(dt), EventLogEntryType.Error, 0);
                    }
                }
            }

            catch (Exception ex)
            {
                clsDalExceptionHandlerDat.WriteExceptionToDataBase(ex, dtUtc);
            }

        }
        public bool WriteToWindowsEventLog(string appName, string strMessage, EventLogEntryType enumEventType, int intModuleID)
        {
            try
            {
                if (!EventLog.SourceExists(appName))
                {
                    EventLog.CreateEventSource(appName, "Application");
                }
                switch (enumEventType)
                {
                    case EventLogEntryType.Error:
                        {
                            EventLog.WriteEntry(appName, strMessage, EventLogEntryType.Error, intModuleID);
                            break;
                        }
                    case EventLogEntryType.Warning:
                        {
                            EventLog.WriteEntry(appName, strMessage, EventLogEntryType.Warning, intModuleID);
                            break;
                        }
                    case EventLogEntryType.Information:
                        {
                            EventLog.WriteEntry(appName, strMessage, EventLogEntryType.Information, intModuleID);
                            break;
                        }
                    case EventLogEntryType.FailureAudit:
                        {
                            EventLog.WriteEntry(appName, strMessage, EventLogEntryType.FailureAudit, intModuleID);
                            break;
                        }
                    case EventLogEntryType.SuccessAudit:
                        {
                            EventLog.WriteEntry(appName, strMessage, EventLogEntryType.SuccessAudit, intModuleID);
                            break;
                        }
                }
                return true;
            }
            catch (Exception Ex)
            {
                return false;
            }
            finally { }
        }


        public void LogException(bool CalledFromService)
        {
            // Log the exception as per the excelicare web.
            LogException();

            string strErrorDesc = string.Empty;

            if (CalledFromService == true)
            {
                // Throw the fault exception in case of exception occured in excelicare service
                if (objException is System.Data.SqlClient.SqlException)
                {
                    System.Data.SqlClient.SqlException objSQLExcepation;
                    objSQLExcepation = (System.Data.SqlClient.SqlException)objException;
                    strErrorDesc = GetSqlExceptionMessage(objSQLExcepation.Number, objSQLExcepation.ErrorCode);
                    throw new FaultException(strErrorDesc);
                }
                else
                {
                    // Throw New FaultException(objException.Message)
                    try
                    {
                        if (!string.IsNullOrEmpty(ExpMessage))
                        {
                            throw new FaultException(ExpMessage);
                        }
                        else if (ConfigurationManager.AppSettings["ShowCustomException"] is not null && !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ShowCustomException"]) && ConfigurationManager.AppSettings["ShowCustomException"].ToUpper() == "FALSE")
                        {
                            throw new FaultException(objException.Message);
                        }
                        else
                        {
                            throw new FaultException("Transaction failed.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public static string GetSqlExceptionMessage(int number, int ErrorCode)
        {
            string strErrorDesc;

            if (ErrorCode == -2146232060 & number == -1)
            {
                strErrorDesc = AxException.ExceptionDetails.DBNotFound;
            }
            else
            {
                switch (number)
                {
                    case -2:
                        {
                            // Connection Timeout
                            strErrorDesc = AxException.ExceptionDetails.ConnectionTimeout;
                            break;
                        }
                    case 4060:
                        {
                            // Invalid Database 
                            strErrorDesc = AxException.ExceptionDetails.DBNotFound;
                            break;
                        }
                    case 18456:
                    case 1326:
                        {
                            // Login Failed 
                            strErrorDesc = AxException.ExceptionDetails.LoginFailed;
                            break;
                        }
                    case 547:
                        {
                            // ForeignKey Violation 
                            strErrorDesc = AxException.ExceptionDetails.ForeignKeyViolation;
                            break;
                        }
                    case 2627:
                        {
                            // Unique Index/Constriant Violation 
                            strErrorDesc = AxException.ExceptionDetails.UniqueKeyViolation;
                            break;
                        }
                    case 2601:
                        {
                            // Unique Index/Constriant Violation 
                            strErrorDesc = AxException.ExceptionDetails.UniqueKeyViolation;
                            break;
                        }

                    default:
                        {
                            // throw a general DAL Exception 
                            strErrorDesc = AxException.ExceptionDetails.DBGeneralisedError;
                            break;
                        }
                }
            }

            return strErrorDesc;
        }

        public void LogException(ref System.Web.UI.Page objPage)
        {
            string strMessage;
            try
            {
                if (HttpContext.Current is not null)
                {
                    if (!string.IsNullOrEmpty(ConfigurationSettings.AppSettings["ShowServerErrors"]))
                    {
                        if (ConfigurationSettings.AppSettings["ShowServerErrors"].ToUpper() == "TRUE")
                        {
                            // strMessage = objException.StackTrace
                            strMessage = Constants.vbTab + "Message : " + objException.Message + Constants.vbCrLf + Constants.vbTab + "Exception Type : " + objException.GetType().ToString() + Constants.vbCrLf + Constants.vbTab + "Source : " + objException.Source + Constants.vbCrLf + "Stack Trace : " + objException.StackTrace;
                            strMessage = strMessage.Replace(@"\", @"\\");
                            strMessage = strMessage.Replace("\"", @"\""");
                            strMessage = strMessage.Replace("\r", @"\n");
                            strMessage = strMessage.Replace("\n", "");
                            strMessage = @"Exception Details : \n" + strMessage;
                            strMessage = "<script language='javascript'>alert(\"" + strMessage + "\");</script>";
                            // objPage.RegisterClientScriptBlock("ErrorDialog", strMessage)
                            objPage.RegisterStartupScript("ErrorDialog" + DateTime.Now.ToOADate().ToString(), strMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            LogException();
        }

        //Added by BRR
        private static Dictionary<string, int> exceptionCountMap = new Dictionary<string, int>();

        private void WriteExceptionToLogFile(string strFileName, string dt) // DateTime)
        {
            var objSWExceptionFile = default(StreamWriter);           
            FileInfo fsWebErrorLog;

            try
            {
                if (HttpContext.Current is not null)
                {
                    strFileName = HttpContext.Current.Request.PhysicalApplicationPath + strFileName;
                    // Added By Harish to create the ErrorLog File if it is not Present
                    fsWebErrorLog = new FileInfo(strFileName);
                    if (!fsWebErrorLog.Exists)
                    {
                        FileStream objFile;
                        objFile = fsWebErrorLog.Create();
                        objFile.Close();
                    }
                    else
                    {
                        CheckExceptionLogDuration(strFileName, fsWebErrorLog);
                    }                   
                    objSWExceptionFile = fsWebErrorLog.AppendText();
                    objSWExceptionFile.WriteLine(GetException(dt));                    
                    var objExceptionHandler = new clsDalExceptionHandlerDat();
                    objExceptionHandler.SaveBrowserRequestLogs(HttpContext.Current.Session.SessionID, 0, "", "", "Error", GetException(dt).ToString(), "", 200, "Server");
                    objExceptionHandler = null;
                }

                else // write something here for desktop applications
                {
                 
                    Assembly oAxAppSupport;
                    oAxAppSupport = Assembly.GetExecutingAssembly();
                    strFileName = Assembly.GetExecutingAssembly().CodeBase.Replace("/bin", "\u0016").Split('\u0016')[0].Replace("file:///", "") + "/" + strFileName;
                    oAxAppSupport = null;
                    fsWebErrorLog = new FileInfo(strFileName);
                  
                    if (!fsWebErrorLog.Exists)
                    {
                        FileStream objFile;
                        objFile = fsWebErrorLog.Create();
                        objFile.Close();
                    }
                    else
                    {
                        CheckExceptionLogDuration(strFileName, fsWebErrorLog);
                    }

                    objSWExceptionFile = fsWebErrorLog.AppendText();
                    objSWExceptionFile.WriteLine(GetException(dt));
                   
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                fsWebErrorLog = null;
                objSWExceptionFile.Flush();
                objSWExceptionFile.Close();
                objSWExceptionFile = null;
            }
        }
     
        private string GetExceptionToLog(string dt)  // DateTime) As String
        {
            if (objException is not null)  // Log server side exception
            {
                Exception objInnerException;
                var int32ExceptionLevel = default(int);
                string strIndent;
                var objSB = new StringBuilder();
                string strFormData;
                objSB.Append("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++" + Constants.vbCrLf);
                objSB.Append("Time of Server Script Exception : " + dt + Constants.vbCrLf); // .UtcNow.ToLongDateString & "  " & dt.UtcNow.ToLongTimeString & vbCrLf)

                if (!(objHTTPSessionState is null || objHTTPRequest is null))
                {
                    objSB.Append("------------------------" + Constants.vbCrLf);
                    objSB.Append("User Agent: " + objHTTPRequest.UserAgent + Constants.vbCrLf);
                    objSB.Append("User IP: " + objHTTPRequest.UserHostAddress + Constants.vbCrLf);
                    objSB.Append("User Host Name: " + objHTTPRequest.UserHostName + Constants.vbCrLf);
                    objSB.Append("User is Authenticated: " + HttpContext.Current.User.Identity.IsAuthenticated.ToString() + Constants.vbCrLf);
                    objSB.Append("SessionID:" + objHTTPSessionState.SessionID + Constants.vbCrLf);

                    strFormData = Constants.vbTab;
                    string.Join("-", objHTTPRequest.Form.AllKeys).Replace(Constants.vbCrLf, Constants.vbCrLf + Constants.vbTab);

                    if (strFormData.Length > 1) // 1 because if the form is empty it will just contain the tab prefixed to the line.
                    {
                        objSB.Append("Form Data:" + Constants.vbCrLf);
                        objSB.Append(strFormData.Substring(0, strFormData.Length - 1) + Constants.vbCrLf);
                    }
                    else
                    {
                        objSB.Append("Form Data: No Form Data Found" + Constants.vbCrLf);
                    }
                }
                objSB.Append("------------------------" + Constants.vbCrLf);
                objInnerException = objException;

                while (objInnerException is not null)
                {
                    int32ExceptionLevel += 1;

                    // Check if the exception is repeated
                    if (exceptionCountMap.ContainsKey(objInnerException.Message))
                    {
                       
                        exceptionCountMap[objInnerException.Message]++;                   
                       
                        objSB.Append(int32ExceptionLevel + ": Repeated Exception: " + objInnerException.Message + Constants.vbCrLf + "   " + exceptionCountMap[objInnerException.Message] + " Times.  ");
                        
                        if (exceptionCountMap[objInnerException.Message] > 5 && blnmailSent)
                        {
                            // mail sending with exception
                      
                            StringBuilder sb = new StringBuilder();
                            sb.Append("<html><body>");
                            sb.Append("<p>Hi Team,</p>");
                            sb.Append($"<p>The below exception has occured {exceptionCountMap[objInnerException.Message]} times in Excelicare system url : {HttpContext.Current.Request.Url}. Please check as priority.</p>");
                            sb.Append("<h3 style='color: red;'>" + objInnerException.InnerException.Message + "</h3>");                         
                            sb.Append("<p><strong>Exception Details:</strong> " + objInnerException.Message + "</p>");
                            sb.Append("<h5 style='color: darkred;'>Source Error:</h5>");
                            sb.Append("<pre style='background-color: #f4f4f4; border: 1px solid #ddd; padding: 10px; color: black;'>");
                            sb.Append(objInnerException.Source);
                            sb.Append("</pre>");
                            sb.Append("<p></p>");

                            sb.Append("<h5 style='color: darkred;'>Stack Trace:</h5>");
                            sb.Append("<pre style='background-color: #f4f4f4; border: 1px solid #ddd; padding: 10px; color: black;'>");
                            sb.Append(WebUtility.HtmlEncode(objInnerException.StackTrace));  // HTML encode stack trace
                            sb.Append("</pre>");

                            sb.Append("<p>Thanks,<br>Excelicare</p>");
                            sb.Append("</body></html>");

                            SendExceptionEmail(sb.ToString(), exceptionCountMap[objInnerException.Message]);

                            blnmailSent = false;
                            System.Threading.Timer timer = new System.Threading.Timer(ResetMailSentFlag, null, TimeSpan.FromMinutes(5), Timeout.InfiniteTimeSpan);
                        }
                        break;

                    }
                    else
                    {                        
                        // Log new exception                       
                        exceptionCountMap.Add(objInnerException.Message, 1);                      
                        objSB.Append(int32ExceptionLevel + ": Source:" + Strings.Replace(objInnerException.Source, Constants.vbCrLf, Constants.vbCrLf + int32ExceptionLevel + ": ") + Constants.vbCrLf);
                        objSB.Append(int32ExceptionLevel + ": Type :" + Strings.Replace(objInnerException.GetType().ToString(), Constants.vbCrLf, Constants.vbCrLf + int32ExceptionLevel + ": ") + Constants.vbCrLf);
                        objSB.Append(int32ExceptionLevel + ": Message:" + objInnerException.Message + Constants.vbCrLf);
                        objSB.Append(int32ExceptionLevel + ": Target Site:" + Strings.Replace(objInnerException.TargetSite.ToString(), Constants.vbCrLf, Constants.vbCrLf + int32ExceptionLevel + ": ") + Constants.vbCrLf);
                        objSB.Append(int32ExceptionLevel + ": Stack Trace:" + Strings.Replace(objInnerException.StackTrace, Constants.vbCrLf, Constants.vbCrLf + int32ExceptionLevel + ": ") + Constants.vbCrLf);
                    }
                    

                    // get the inner exception to log
                    objInnerException = objInnerException.InnerException;
                    if (objInnerException is not null)
                    {
                        objSB.Append("------------------------" + Constants.vbCrLf);
                    }
                }
                return objSB.ToString();
            }
            else // Log client script exception
            {
                var objSB = new StringBuilder();
                objSB.Append("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++" + Constants.vbCrLf);
                objSB.Append("Time of Client Script Exception : " + dt); // .UtcNow.ToLongDateString & "  " & dt.UtcNow.ToLongTimeString & vbCrLf)
                objSB.Append("------------------------" + Constants.vbCrLf);
                objSB.Append("User Agent: " + objHTTPRequest.UserAgent + Constants.vbCrLf);
                objSB.Append("User IP: " + objHTTPRequest.UserHostAddress + Constants.vbCrLf);
                objSB.Append("User Host Name: " + objHTTPRequest.UserHostName + Constants.vbCrLf);
                objSB.Append("User is Authenticated: " + HttpContext.Current.User.Identity.IsAuthenticated.ToString() + Constants.vbCrLf);
                objSB.Append("SessionID:" + objHTTPSessionState.SessionID + Constants.vbCrLf);
                objSB.Append("------------------------" + Constants.vbCrLf);
                objSB.Append("URL          :" + objHTTPRequest.QueryString["url"] + Constants.vbCrLf);
                objSB.Append("Error Message:" + objHTTPRequest.QueryString["em"] + Constants.vbCrLf);
                objSB.Append("Line Number  :" + objHTTPRequest.QueryString["l"] + Constants.vbCrLf);
                return objSB.ToString();
            }
        }
        private void CheckExceptionLogDuration(string strFileName, FileInfo fsWebErrorLog)
        {
            string strErrorLogkey = ConfigurationSettings.AppSettings["AxException_ErrorLogDuration"];
            var dt_errorlog = FileSystem.FileDateTime(strFileName);
            if (strErrorLogkey == "0")        // ' check for error log on DAILY basis
            {
                if (dt_errorlog.Date < DateTime.Now.Date)
                {
                    FileInfo fsErrLog;
                    fsErrLog = new FileInfo(strFileName);
                    CreateLogFile(strFileName, fsErrLog, dt_errorlog);
                    fsErrLog = null;
                }
            }
            else if (strErrorLogkey == "1")        // ' check for error log on WEEKLY basis
            {
                if (dt_errorlog.AddDays(GetDayofWeek(dt_errorlog)).Date < DateTime.Now.Date)
                {
                    FileInfo fsErrLog;
                    fsErrLog = new FileInfo(strFileName);
                    CreateLogFile(strFileName, fsErrLog, dt_errorlog.AddDays(GetDayofWeek(dt_errorlog)).Date);
                    fsErrLog = null;
                }
            }
            else if (strErrorLogkey == "2")        // ' check for error log on MONTHLY basis
            {
                if (dt_errorlog.Year < DateTime.Now.Year)
                {
                    FileInfo fsErrLog;
                    fsErrLog = new FileInfo(strFileName);
                    CreateLogFile(strFileName, fsErrLog, dt_errorlog);
                    fsErrLog = null;
                }
                else if (dt_errorlog.Year == DateTime.Now.Year)
                {
                    if (dt_errorlog.Month < DateTime.Now.Month)
                    {
                        if (dt_errorlog.Date < DateTime.Now.Date)
                        {
                            FileInfo fsErrLog;
                            fsErrLog = new FileInfo(strFileName);
                            CreateLogFile(strFileName, fsErrLog, dt_errorlog);
                            fsErrLog = null;
                        }
                    }
                }
            }
            else if (strErrorLogkey == "3")        // ' check for error log on YEARLY basis
            {
                if (dt_errorlog.Year < DateTime.Now.Year)
                {
                    if (dt_errorlog.Date < DateTime.Now.Date)
                    {
                        FileInfo fsErrLog;
                        fsErrLog = new FileInfo(strFileName);
                        CreateLogFile(strFileName, fsErrLog, dt_errorlog);
                        fsErrLog = null;
                    }
                }
            }
            fsWebErrorLog = new FileInfo(strFileName);
            if (!fsWebErrorLog.Exists)
            {
                FileStream objFile;
                objFile = fsWebErrorLog.Create();
                objFile.Close();
            }
        }
        
        // -----------------------------------------------------------------------------------------------
        // <summary>
        // This procedure takes backup of old error log file and create new log file
        // </summary>
        // <param name="strFileName">String</param>
        // <param name="fsWebErrorLog">FileInfo</param>
        // <param name="dt_ErrorLogDate">DateTime</param>
        // <remarks>
        // </remarks>
        // <history>  
        // </history>
        // -----------------------------------------------------------------------------------------------
        public void CreateLogFile(string strFileName, FileInfo fsWebErrorLog, DateTime dt_ErrorLogDate)
        {
            string strRenameFile = "";
            string strErrorLogName = ConfigurationSettings.AppSettings["AxException_FileNameToWrite"];
            string strErrorLogkey = ConfigurationSettings.AppSettings["AxException_ErrorLogDuration"];

            // rename the file
            string[] arrLogName = strErrorLogName.Split('.');
            DateTime dt_PastLogDate;
            if (strErrorLogkey == "0")
            {
                dt_PastLogDate = dt_ErrorLogDate;
                if (dt_PastLogDate.Day.ToString().Length == 1)
                {
                    strRenameFile = arrLogName[0].ToString() + "_D" + "0" + dt_PastLogDate.Day;
                }
                else
                {
                    strRenameFile = arrLogName[0].ToString() + "_D" + dt_PastLogDate.Day;
                }
                if (dt_PastLogDate.Month.ToString().Length == 1)
                {
                    strRenameFile = strRenameFile + "0" + dt_PastLogDate.Month;
                }
                else
                {
                    strRenameFile = strRenameFile + dt_PastLogDate.Month;
                }
                strRenameFile = strRenameFile + dt_PastLogDate.Year;
            }
            else if (strErrorLogkey == "1")
            {
                dt_PastLogDate = dt_ErrorLogDate;
                if (dt_PastLogDate.Day.ToString().Length == 1)
                {
                    strRenameFile = arrLogName[0].ToString() + "_W" + "0" + dt_PastLogDate.Day;
                }
                else
                {
                    strRenameFile = arrLogName[0].ToString() + "_W" + dt_PastLogDate.Day;
                }
                if (dt_PastLogDate.Month.ToString().Length == 1)
                {
                    strRenameFile = strRenameFile + "0" + dt_PastLogDate.Month;
                }
                else
                {
                    strRenameFile = strRenameFile + dt_PastLogDate.Month;
                }
                strRenameFile = strRenameFile + dt_PastLogDate.Year;
            }
            else if (strErrorLogkey == "2")
            {
                if (dt_ErrorLogDate.Month == Conversions.ToDouble("1") | dt_ErrorLogDate.Month == Conversions.ToDouble("3") | dt_ErrorLogDate.Month == Conversions.ToDouble("5") | dt_ErrorLogDate.Month == Conversions.ToDouble("7") | dt_ErrorLogDate.Month == Conversions.ToDouble("8") | dt_ErrorLogDate.Month == Conversions.ToDouble("10") | dt_ErrorLogDate.Month == Conversions.ToDouble("12"))
                {
                    if (dt_ErrorLogDate.Month.ToString().Length == 1)
                    {
                        strRenameFile = arrLogName[0].ToString() + "_M" + "31" + "0" + dt_ErrorLogDate.Month;
                    }
                    else
                    {
                        strRenameFile = arrLogName[0].ToString() + "_M" + "31" + dt_ErrorLogDate.Month;
                    }
                    strRenameFile = strRenameFile + dt_ErrorLogDate.Year;
                }
                else if (dt_ErrorLogDate.Month == Conversions.ToDouble("4") | dt_ErrorLogDate.Month == Conversions.ToDouble("6") | dt_ErrorLogDate.Month == Conversions.ToDouble("9") | dt_ErrorLogDate.Month == Conversions.ToDouble("11"))
                {
                    if (dt_ErrorLogDate.Month.ToString().Length == 1)
                    {
                        strRenameFile = arrLogName[0].ToString() + "_M" + "30" + "0" + dt_ErrorLogDate.Month;
                    }
                    else
                    {
                        strRenameFile = arrLogName[0].ToString() + "_M" + "30" + dt_ErrorLogDate.Month;
                    }
                    strRenameFile = strRenameFile + dt_ErrorLogDate.Year;
                }
                else if (DateTime.IsLeapYear(dt_ErrorLogDate.Year) == true)
                {
                    strRenameFile = arrLogName[0].ToString() + "_M" + "29" + "0" + dt_ErrorLogDate.Month + dt_ErrorLogDate.Year;
                }
                else
                {
                    strRenameFile = arrLogName[0].ToString() + "_M" + "28" + "0" + dt_ErrorLogDate.Month + dt_ErrorLogDate.Year;
                }
            }
            else if (strErrorLogkey == "3")
            {
                dt_PastLogDate = dt_ErrorLogDate;
                strRenameFile = arrLogName[0].ToString() + "_Y" + "3112" + dt_PastLogDate.Year;
            }

            strRenameFile = strRenameFile + "." + arrLogName[1];
            strRenameFile = strFileName.Replace(strErrorLogName, strRenameFile);
            fsWebErrorLog.MoveTo(strRenameFile);
        }

        private int GetDayofWeek(DateTime dt_errorlog)
        {
            if (dt_errorlog.DayOfWeek == DayOfWeek.Sunday)
            {
                return 6;
            }
            else if (dt_errorlog.DayOfWeek == DayOfWeek.Monday)
            {
                return 5;
            }
            else if (dt_errorlog.DayOfWeek == DayOfWeek.Tuesday)
            {
                return 4;
            }
            else if (dt_errorlog.DayOfWeek == DayOfWeek.Wednesday)
            {
                return 3;
            }
            else if (dt_errorlog.DayOfWeek == DayOfWeek.Thursday)
            {
                return 2;
            }
            else if (dt_errorlog.DayOfWeek == DayOfWeek.Friday)
            {
                return 1;
            }
            else if (dt_errorlog.DayOfWeek == DayOfWeek.Saturday)
            {
                return 0;
            }

            return default;
        }

        #endregion

        #region mailsend

    
        static bool blnmailSent = (ConfigurationSettings.AppSettings["senderrormail"] != null?Convert.ToBoolean(ConfigurationSettings.AppSettings["senderrormail"]): false);
        static void ResetMailSentFlag(object state)
        {
            blnmailSent = true;
        }

        // Email sending function
        private void SendExceptionEmail(string exceptionMessage, int count)
        {
            try
            {   
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("no-reply@excelicare.com"); // Sender email
                mail.To.Add("ramana.bhumpalli@excelicare.com"); // Recipient email
                mail.Subject = "Repeated Exception Alert: Exception Occurred More Than 5 Times";
                //mail.Body = $"The exception: \"{exceptionMessage}\" has occurred {count} times in the Excelicare system.";
                mail.Body = exceptionMessage;
                mail.IsBodyHtml = true;
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                // Set up SMTP client
                SmtpClient smtpClient = new SmtpClient("smtp.office365.com") // Your SMTP server
                {
                    Port = 587, // SMTP port (usually 587 for TLS)
                    Credentials = new System.Net.NetworkCredential("no-reply@excelicare.com", "practive$123"), // SMTP credentials
                    EnableSsl = true // Enable SSL for secure connection
                };                
                smtpClient.Send(mail);
            }
            catch (Exception ex){ }
        }
        #endregion

    } // clsExceptionHandler
}