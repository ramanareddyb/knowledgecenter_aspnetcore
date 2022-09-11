using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AxCommonFun;
using System.Xml;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace Compile_And_Deploy
{
    public partial class Form1 : Form
    {
        List<string> Exlcucompre = new List<string>();
        private bool checkOnlyModified = false;
        private bool checkAllFiles = false;
        private bool checkFromDateTime = false;
        private bool checkToDateTime = false;
        private bool checkIsWebRelease = false;
        private bool checkbackupfolder = false;
        private bool checkdeletefolder = false;
        private bool checkIsServiceRelease = false;
        private bool checkcompilation = false;
        private bool checkcreatepatch = false;
        private bool checkprotectdlls = false;
        private bool checkcombinecss = false;
        private bool checkProduct = false;
        private bool checkPSP = false;
        private bool CheckCSP = false;
        private bool checkversionincreament = false;
        private bool compilationstatus = false;
        private bool getlateststatus = false;
        private bool versionincrementstatus = false;
        private bool CreatePatchstatus = false;
        private bool protectdllsstatus = false;
        private bool BackupFolderstatus = false;
        private bool Deleteexstingfolderstatus = false;
        private bool DeployFolderstatus = false;
        private bool combinecssstatus = false;
        private string WrokFolder = "";
        string[] ALLModifiedFilesList = null;
        string[] ALLModifiedDllList = null;
        List<string> webmodules;
        protected string logfilepath = System.Environment.CurrentDirectory + "\\Compile_And_Deploy" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_log.log";
        string rootfilepath;
        public class IniFile
        {
            /// <summary>
            /// The maximum size of a section in an ini file.
            /// </summary>
            /// <remarks>
            /// This property defines the maximum size of the buffers 
            /// used to retreive data from an ini file.  This value is 
            /// the maximum allowed by the win32 functions 
            /// GetPrivateProfileSectionNames() or 
            /// GetPrivateProfileString().
            /// </remarks>
            public const int MaxSectionSize = 32767; // 32 KB

            //The path of the file we are operating on.
            private string m_path;

            #region P/Invoke declares

            /// <summary>
            /// A static class that provides the win32 P/Invoke signatures 
            /// used by this class.
            /// </summary>
            /// <remarks>
            /// Note:  In each of the declarations below, we explicitly set CharSet to 
            /// Auto.  By default in C#, CharSet is set to Ansi, which reduces 
            /// performance on windows 2000 and above due to needing to convert strings
            /// from Unicode (the native format for all .Net strings) to Ansi before 
            /// marshalling.  Using Auto lets the marshaller select the Unicode version of 
            /// these functions when available.
            /// </remarks>
            [System.Security.SuppressUnmanagedCodeSecurity]
            private static class NativeMethods
            {
                [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                public static extern int GetPrivateProfileSectionNames(IntPtr lpszReturnBuffer,
                                                                       uint nSize,
                                                                       string lpFileName);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                public static extern uint GetPrivateProfileString(string lpAppName,
                                                                  string lpKeyName,
                                                                  string lpDefault,
                                                                  StringBuilder lpReturnedString,
                                                                  int nSize,
                                                                  string lpFileName);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                public static extern uint GetPrivateProfileString(string lpAppName,
                                                                  string lpKeyName,
                                                                  string lpDefault,
                                                                  [In, Out] char[] lpReturnedString,
                                                                  int nSize,
                                                                  string lpFileName);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                public static extern int GetPrivateProfileString(string lpAppName,
                                                                 string lpKeyName,
                                                                 string lpDefault,
                                                                 IntPtr lpReturnedString,
                                                                 uint nSize,
                                                                 string lpFileName);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                public static extern int GetPrivateProfileInt(string lpAppName,
                                                              string lpKeyName,
                                                              int lpDefault,
                                                              string lpFileName);

                [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
                public static extern int GetPrivateProfileSection(string lpAppName,
                                                                  IntPtr lpReturnedString,
                                                                  uint nSize,
                                                                  string lpFileName);

                //We explicitly enable the SetLastError attribute here because
                // WritePrivateProfileString returns errors via SetLastError.
                // Failure to set this can result in errors being lost during 
                // the marshal back to managed code.
                [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
                public static extern bool WritePrivateProfileString(string lpAppName,
                                                                    string lpKeyName,
                                                                    string lpString,
                                                                    string lpFileName);


            }
            #endregion

            /// <summary>
            /// Initializes a new instance of the <see cref="IniFile"/> class.
            /// </summary>
            /// <param name="path">The ini file to read and write from.</param>
            public IniFile(string path)
            {
                //Convert to the full path.  Because of backward compatibility, 
                // the win32 functions tend to assume the path should be the 
                // root Windows directory if it is not specified.  By calling 
                // GetFullPath, we make sure we are always passing the full path
                // the win32 functions.
                m_path = System.IO.Path.GetFullPath(path);
            }

            /// <summary>
            /// Gets the full path of ini file this object instance is operating on.
            /// </summary>
            /// <value>A file path.</value>
            public string Path
            {
                get
                {
                    return m_path;
                }
            }

            #region Get Value Methods

            /// <summary>
            /// Gets the value of a setting in an ini file as a <see cref="T:System.String"/>.
            /// </summary>
            /// <param name="sectionName">The name of the section to read from.</param>
            /// <param name="keyName">The name of the key in section to read.</param>
            /// <param name="defaultValue">The default value to return if the key
            /// cannot be found.</param>
            /// <returns>The value of the key, if found.  Otherwise, returns 
            /// <paramref name="defaultValue"/></returns>
            /// <remarks>
            /// The retreived value must be less than 32KB in length.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> are 
            /// a null reference  (Nothing in VB)
            /// </exception>
            public string GetString(string sectionName,
                                    string keyName,
                                    string defaultValue)
            {
                if (sectionName == null)
                    throw new ArgumentNullException("sectionName");

                if (keyName == null)
                    throw new ArgumentNullException("keyName");

                StringBuilder retval = new StringBuilder(IniFile.MaxSectionSize);

                NativeMethods.GetPrivateProfileString(sectionName,
                                                      keyName,
                                                      defaultValue,
                                                      retval,
                                                      IniFile.MaxSectionSize,
                                                      m_path);

                return retval.ToString();
            }

            /// <summary>
            /// Gets the value of a setting in an ini file as a <see cref="T:System.Int16"/>.
            /// </summary>
            /// <param name="sectionName">The name of the section to read from.</param>
            /// <param name="keyName">The name of the key in section to read.</param>
            /// <param name="defaultValue">The default value to return if the key
            /// cannot be found.</param>
            /// <returns>The value of the key, if found.  Otherwise, returns 
            /// <paramref name="defaultValue"/>.</returns>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> are 
            /// a null reference  (Nothing in VB)
            /// </exception>
            public int GetInt16(string sectionName,
                                string keyName,
                                short defaultValue)
            {
                int retval = GetInt32(sectionName, keyName, defaultValue);

                return Convert.ToInt16(retval);
            }

            /// <summary>
            /// Gets the value of a setting in an ini file as a <see cref="T:System.Int32"/>.
            /// </summary>
            /// <param name="sectionName">The name of the section to read from.</param>
            /// <param name="keyName">The name of the key in section to read.</param>
            /// <param name="defaultValue">The default value to return if the key
            /// cannot be found.</param>
            /// <returns>The value of the key, if found.  Otherwise, returns 
            /// <paramref name="defaultValue"/></returns>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> are 
            /// a null reference  (Nothing in VB)
            /// </exception>
            public int GetInt32(string sectionName,
                                string keyName,
                                int defaultValue)
            {
                if (sectionName == null)
                    throw new ArgumentNullException("sectionName");

                if (keyName == null)
                    throw new ArgumentNullException("keyName");


                return NativeMethods.GetPrivateProfileInt(sectionName, keyName, defaultValue, m_path);
            }

            /// <summary>
            /// Gets the value of a setting in an ini file as a <see cref="T:System.Double"/>.
            /// </summary>
            /// <param name="sectionName">The name of the section to read from.</param>
            /// <param name="keyName">The name of the key in section to read.</param>
            /// <param name="defaultValue">The default value to return if the key
            /// cannot be found.</param>
            /// <returns>The value of the key, if found.  Otherwise, returns 
            /// <paramref name="defaultValue"/></returns>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> are 
            /// a null reference  (Nothing in VB)
            /// </exception>
            public double GetDouble(string sectionName,
                                    string keyName,
                                    double defaultValue)
            {
                string retval = GetString(sectionName, keyName, "");

                if (retval == null || retval.Length == 0)
                {
                    return defaultValue;
                }

                return Convert.ToDouble(retval, CultureInfo.InvariantCulture);
            }

            #endregion

            #region GetSectionValues Methods

            /// <summary>
            /// Gets all of the values in a section as a list.
            /// </summary>
            /// <param name="sectionName">
            /// Name of the section to retrieve values from.
            /// </param>
            /// <returns>
            /// A <see cref="List{T}"/> containing <see cref="KeyValuePair{T1, T2}"/> objects 
            /// that describe this section.  Use this verison if a section may contain
            /// multiple items with the same key value.  If you know that a section 
            /// cannot contain multiple values with the same key name or you don't 
            /// care about the duplicates, use the more convenient 
            /// <see cref="GetSectionValues"/> function.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> is a null reference  (Nothing in VB)
            /// </exception>
            public List<KeyValuePair<string, string>> GetSectionValuesAsList(string sectionName)
            {
                List<KeyValuePair<string, string>> retval;
                string[] keyValuePairs;
                string key, value;
                int equalSignPos;

                if (sectionName == null)
                    throw new ArgumentNullException("sectionName");

                //Allocate a buffer for the returned section names.
                IntPtr ptr = Marshal.AllocCoTaskMem(IniFile.MaxSectionSize);

                try
                {
                    //Get the section key/value pairs into the buffer.
                    int len = NativeMethods.GetPrivateProfileSection(sectionName,
                                                                     ptr,
                                                                     IniFile.MaxSectionSize,
                                                                     m_path);

                    keyValuePairs = ConvertNullSeperatedStringToStringArray(ptr, len);
                }
                finally
                {
                    //Free the buffer
                    Marshal.FreeCoTaskMem(ptr);
                }

                //Parse keyValue pairs and add them to the list.
                retval = new List<KeyValuePair<string, string>>(keyValuePairs.Length);

                for (int i = 0; i < keyValuePairs.Length; ++i)
                {
                    //Parse the "key=value" string into its constituent parts
                    equalSignPos = keyValuePairs[i].IndexOf('=');

                    key = keyValuePairs[i].Substring(0, equalSignPos);

                    value = keyValuePairs[i].Substring(equalSignPos + 1,
                                                       keyValuePairs[i].Length - equalSignPos - 1);

                    retval.Add(new KeyValuePair<string, string>(key, value));
                }

                return retval;
            }

            /// <summary>
            /// Gets all of the values in a section as a dictionary.
            /// </summary>
            /// <param name="sectionName">
            /// Name of the section to retrieve values from.
            /// </param>
            /// <returns>
            /// A <see cref="Dictionary{T, T}"/> containing the key/value 
            /// pairs found in this section.  
            /// </returns>
            /// <remarks>
            /// If a section contains more than one key with the same name, 
            /// this function only returns the first instance.  If you need to 
            /// get all key/value pairs within a section even when keys have the 
            /// same name, use <see cref="GetSectionValuesAsList"/>.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> is a null reference  (Nothing in VB)
            /// </exception>
            public Dictionary<string, string> GetSectionValues(string sectionName)
            {
                List<KeyValuePair<string, string>> keyValuePairs;
                Dictionary<string, string> retval;

                keyValuePairs = GetSectionValuesAsList(sectionName);

                //Convert list into a dictionary.
                retval = new Dictionary<string, string>(keyValuePairs.Count);

                foreach (KeyValuePair<string, string> keyValuePair in keyValuePairs)
                {
                    //Skip any key we have already seen.
                    if (!retval.ContainsKey(keyValuePair.Key))
                    {
                        retval.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }

                return retval;
            }

            #endregion

            #region Get Key/Section Names

            /// <summary>
            /// Gets the names of all keys under a specific section in the ini file.
            /// </summary>
            /// <param name="sectionName">
            /// The name of the section to read key names from.
            /// </param>
            /// <returns>An array of key names.</returns>
            /// <remarks>
            /// The total length of all key names in the section must be 
            /// less than 32KB in length.
            /// </remarks>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> is a null reference  (Nothing in VB)
            /// </exception>
            public string[] GetKeyNames(string sectionName)
            {
                int len;
                string[] retval;

                if (sectionName == null)
                    throw new ArgumentNullException("sectionName");

                //Allocate a buffer for the returned section names.
                IntPtr ptr = Marshal.AllocCoTaskMem(IniFile.MaxSectionSize);

                try
                {
                    //Get the section names into the buffer.
                    len = NativeMethods.GetPrivateProfileString(sectionName,
                                                                null,
                                                                null,
                                                                ptr,
                                                                IniFile.MaxSectionSize,
                                                                m_path);

                    retval = ConvertNullSeperatedStringToStringArray(ptr, len);
                }
                finally
                {
                    //Free the buffer
                    Marshal.FreeCoTaskMem(ptr);
                }

                return retval;
            }

            /// <summary>
            /// Gets the names of all sections in the ini file.
            /// </summary>
            /// <returns>An array of section names.</returns>
            /// <remarks>
            /// The total length of all section names in the section must be 
            /// less than 32KB in length.
            /// </remarks>
            public string[] GetSectionNames()
            {
                string[] retval;
                int len;

                //Allocate a buffer for the returned section names.
                IntPtr ptr = Marshal.AllocCoTaskMem(IniFile.MaxSectionSize);

                try
                {
                    //Get the section names into the buffer.
                    len = NativeMethods.GetPrivateProfileSectionNames(ptr,
                        IniFile.MaxSectionSize, m_path);

                    retval = ConvertNullSeperatedStringToStringArray(ptr, len);
                }
                finally
                {
                    //Free the buffer
                    Marshal.FreeCoTaskMem(ptr);
                }

                return retval;
            }

            /// <summary>
            /// Converts the null seperated pointer to a string into a string array.
            /// </summary>
            /// <param name="ptr">A pointer to string data.</param>
            /// <param name="valLength">
            /// Length of the data pointed to by <paramref name="ptr"/>.
            /// </param>
            /// <returns>
            /// An array of strings; one for each null found in the array of characters pointed
            /// at by <paramref name="ptr"/>.
            /// </returns>
            private static string[] ConvertNullSeperatedStringToStringArray(IntPtr ptr, int valLength)
            {
                string[] retval;

                if (valLength == 0)
                {
                    //Return an empty array.
                    retval = new string[0];
                }
                else
                {
                    //Convert the buffer into a string.  Decrease the length 
                    //by 1 so that we remove the second null off the end.
                    string buff = Marshal.PtrToStringAuto(ptr, valLength - 1);

                    //Parse the buffer into an array of strings by searching for nulls.
                    retval = buff.Split('\0');
                }

                return retval;
            }

            #endregion

            #region Write Methods

            /// <summary>
            /// Writes a <see cref="T:System.String"/> value to the ini file.
            /// </summary>
            /// <param name="sectionName">The name of the section to write to .</param>
            /// <param name="keyName">The name of the key to write to.</param>
            /// <param name="value">The string value to write</param>
            /// <exception cref="T:System.ComponentModel.Win32Exception">
            /// The write failed.
            /// </exception>
            private void WriteValueInternal(string sectionName, string keyName, string value)
            {
                if (!NativeMethods.WritePrivateProfileString(sectionName, keyName, value, m_path))
                {
                    throw new System.ComponentModel.Win32Exception();
                }
            }

            /// <summary>
            /// Writes a <see cref="T:System.String"/> value to the ini file.
            /// </summary>
            /// <param name="sectionName">The name of the section to write to .</param>
            /// <param name="keyName">The name of the key to write to.</param>
            /// <param name="value">The string value to write</param>
            /// <exception cref="T:System.ComponentModel.Win32Exception">
            /// The write failed.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> or 
            /// <paramref name="value"/>  are a null reference  (Nothing in VB)
            /// </exception>
            public void WriteValue(string sectionName, string keyName, string value)
            {
                if (sectionName == null)
                    throw new ArgumentNullException("sectionName");

                if (keyName == null)
                    throw new ArgumentNullException("keyName");

                if (value == null)
                    throw new ArgumentNullException("value");

                WriteValueInternal(sectionName, keyName, value);
            }

            /// <summary>
            /// Writes an <see cref="T:System.Int16"/> value to the ini file.
            /// </summary>
            /// <param name="sectionName">The name of the section to write to .</param>
            /// <param name="keyName">The name of the key to write to.</param>
            /// <param name="value">The value to write</param>
            /// <exception cref="T:System.ComponentModel.Win32Exception">
            /// The write failed.
            /// </exception>
            public void WriteValue(string sectionName, string keyName, short value)
            {
                WriteValue(sectionName, keyName, (int)value);
            }

            /// <summary>
            /// Writes an <see cref="T:System.Int32"/> value to the ini file.
            /// </summary>
            /// <param name="sectionName">The name of the section to write to .</param>
            /// <param name="keyName">The name of the key to write to.</param>
            /// <param name="value">The value to write</param>
            /// <exception cref="T:System.ComponentModel.Win32Exception">
            /// The write failed.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> are 
            /// a null reference  (Nothing in VB)
            /// </exception>
            public void WriteValue(string sectionName, string keyName, int value)
            {
                WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
            }

            /// <summary>
            /// Writes an <see cref="T:System.Single"/> value to the ini file.
            /// </summary>
            /// <param name="sectionName">The name of the section to write to .</param>
            /// <param name="keyName">The name of the key to write to.</param>
            /// <param name="value">The value to write</param>
            /// <exception cref="T:System.ComponentModel.Win32Exception">
            /// The write failed.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> are 
            /// a null reference  (Nothing in VB)
            /// </exception>
            public void WriteValue(string sectionName, string keyName, float value)
            {
                WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
            }

            /// <summary>
            /// Writes an <see cref="T:System.Double"/> value to the ini file.
            /// </summary>
            /// <param name="sectionName">The name of the section to write to .</param>
            /// <param name="keyName">The name of the key to write to.</param>
            /// <param name="value">The value to write</param>
            /// <exception cref="T:System.ComponentModel.Win32Exception">
            /// The write failed.
            /// </exception>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> are 
            /// a null reference  (Nothing in VB)
            /// </exception>
            public void WriteValue(string sectionName, string keyName, double value)
            {
                WriteValue(sectionName, keyName, value.ToString(CultureInfo.InvariantCulture));
            }

            #endregion

            #region Delete Methods

            /// <summary>
            /// Deletes the specified key from the specified section.
            /// </summary>
            /// <param name="sectionName">
            /// Name of the section to remove the key from.
            /// </param>
            /// <param name="keyName">
            /// Name of the key to remove.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> or <paramref name="keyName"/> are 
            /// a null reference  (Nothing in VB)
            /// </exception>
            public void DeleteKey(string sectionName, string keyName)
            {
                if (sectionName == null)
                    throw new ArgumentNullException("sectionName");

                if (keyName == null)
                    throw new ArgumentNullException("keyName");

                WriteValueInternal(sectionName, keyName, null);
            }

            /// <summary>
            /// Deletes a section from the ini file.
            /// </summary>
            /// <param name="sectionName">
            /// Name of the section to delete.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// <paramref name="sectionName"/> is a null reference (Nothing in VB)
            /// </exception>
            public void DeleteSection(string sectionName)
            {
                if (sectionName == null)
                    throw new ArgumentNullException("sectionName");

                WriteValueInternal(sectionName, null, null);
            }

            #endregion
        }
       
        public Form1()
        {
            InitializeComponent();
        }
        private bool Protectdlls(string binpath)
        {
            bool istrue = false;
            if (!string.IsNullOrEmpty(binpath))
            {
                logfile("Create Protectdlls begin : " + DateTime.Now.ToString(), false);
                try
                {
                    var Files = Directory.GetFiles(binpath);
                    string Command = '"' + @"C:\Program Files (x86)\Remotesoft\Protector\bin\protector.exe" + '"';
                    if (Files.Count() != 0)
                    {
                        List<string> lins = new List<string>();
                        foreach (string fil in Files)
                        {
                            if (fil.Contains(".dll"))
                            {
                                if (!fil.ToLower().Contains("Interop".ToLower()) && !fil.ToLower().Contains("Interop".ToLower()) && !fil.ToLower().Contains("AxWebUCPRLink".ToLower()))
                                {
                                    StartProcess(Command, " " + "-neutral" + " " + "-resource" + " " + "-string" + " " + "-clrversion v2.0.50727 " + '"' + fil + '"', fil, true);
                                }
                            }
                        }
                    }
                    var movefile = Directory.GetFiles(Path.Combine(binpath, "protected"));
                    foreach (string mf in movefile)
                    {
                        File.Copy(mf, mf.Replace("\\protected", ""), true);

                    }
                    Directory.Delete(Path.Combine(binpath, "protected"), true);
                    istrue = true;
                }
                catch (Exception ex)
                {
                    logfile(ex.ToString(), true);
                }
                logfile("Protectdlls End : " + DateTime.Now.ToString(), false);
            }
            return istrue;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if(Product.Checked)
            {
                checkProduct = true;
            }
            if(ProductServicePacks.Checked)
            {
                checkPSP = true;
            }
            if(ClientServicePacks.Checked)
            {
                CheckCSP = true;
            }
            if (DeleteFolder.Checked)
            {
                checkdeletefolder = true;
            }
            if(ProtectDllscheck.Checked)
            {
                checkprotectdlls = true;
            }
                if (OnlyModified.Checked)
                {
                    checkOnlyModified = true;
                }
                if (ALLFILES.Checked)
                {
                    checkAllFiles = true;
                }
                if (FrmCheckdate.Checked)
                {
                    checkFromDateTime = true;
                   // fromdateinstring = dateTimePicker1.Value.ToShortDateString() + " " + dateTimePicker3.Value.ToShortTimeString();
                }
                if (Tocheckdate.Checked)
                {
                    checkToDateTime = true;
                  //  Todateinstring = dateTimePicker2.Value.ToShortDateString() + " " + dateTimePicker4.Value.ToShortTimeString();
                }
            if(Compilation.Checked)
            {
                checkcompilation = true;
            }
            if(ComCSSFILES.Checked)
            {
                checkcombinecss = true;
            }
            if(VersionIncrement.Checked)
            {
                checkversionincreament = true;
            }
            if(Dstructure.Checked)
            {
                checkcreatepatch = true;
            }

            webmodules = new List<string>();
            foreach (string m in checkLBWEB.SelectedItems)
            {
                webmodules.Add(m);
            }

            tabControl1.SelectedIndex = 2;
            this.backgroundWorker1.RunWorkerAsync();
          

                         
        }
        private bool Increamentversion(string[] filepaths)
        {
            bool isittrue = false;
            try
            {
                foreach (string f in filepaths)
                {
                    FileInfo finfo = new FileInfo(f);
                    if(finfo.Extension.ToLower() == ".vb")
                    {
                        if(VBversionincrement(finfo.FullName,false,false,true,false))
                        {
                            isittrue = true;
                            string INIPATH1 = Application.StartupPath.ToString() + "\\WebVersionInfoINI.ini";
                            IniFile hhh1 = new IniFile(INIPATH1);
                            string key = hhh1.GetString("CurrentVersion", "VersionIncreamentedAssemblyinfofiles", "");
                            if(key.EndsWith(";"))
                            {
                                key = key + finfo.FullName.Replace(WrokFolder, "AXXRoot");
                            }
                            else
                            {
                                key = key +";"+ finfo.FullName.Replace(WrokFolder, "AXXRoot");
                            }
                            hhh1.WriteValue("CurrentVersion", "VersionIncreamentedAssemblyinfofiles", key);

                        }
                        else
                        {
                            isittrue = false;
                            break;
                        }
                    }
                    else if (finfo.Extension.ToLower() == ".cs")
                    {
                        if(CSversionincrement(finfo.FullName,false,false,true,false))
                        {
                            string INIPATH1 = Application.StartupPath.ToString() + "\\WebVersionInfoINI.ini";
                            IniFile hhh1 = new IniFile(INIPATH1);
                            string key = hhh1.GetString("CurrentVersion", "VersionIncreamentedAssemblyinfofiles", "");
                            if (key.EndsWith(";"))
                            {
                                key = key + finfo.FullName.Replace(WrokFolder, "AXXRoot");
                            }
                            else
                            {
                                key = key + ";" + finfo.FullName.Replace(WrokFolder, "AXXRoot");
                            }
                            hhh1.WriteValue("CurrentVersion", "VersionIncreamentedAssemblyinfofiles", key);
                            isittrue = true;
                        }
                        else
                        {
                           
                            isittrue = false;
                            break;
                        }
                    }

                }
                
            }
            catch(Exception ex)
            {
                isittrue = false;
            }
                
            return isittrue;
        }
        private string[] getallassemblyinfos(string[] filepaths)
        {

           string[] ainfos = null;
            List<string> ainfos1 = new List<string>();
            string INIPATH1 = Application.StartupPath.ToString() + "\\WebVersionInfoINI.ini";
            IniFile hhh1 = new IniFile(INIPATH1);
            string key = hhh1.GetString("CurrentVersion", "VersionIncreamentedAssemblyinfofiles", "");
             string[] icrementedfile = {"",""};
            if(string.IsNullOrEmpty(key))
            {
                string[] split = key.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                 icrementedfile = split.Select(x => x.Replace("AXXRoot", WrokFolder)).ToArray();
            }
          
            try
            {
                foreach(string f in filepaths)
                {
                    FileInfo finfo = new FileInfo(f);
                    if(finfo.Exists)
                    {
                        if (finfo.Extension.ToLower() == ".vb"  )
                        {
                            var afile = finfo.Directory.GetFiles("assemblyinfo.vb", SearchOption.AllDirectories);
                            foreach(FileInfo af in afile)
                            {
                                if (!ainfos1.Contains(af.FullName) && !icrementedfile.Contains(af.FullName))
                                {
                                    ainfos1.Add(af.FullName);
                                }
                            }
                        }
                        else if(finfo.Extension.ToLower() == ".cs")
                        {
                            var afile = finfo.Directory.GetFiles("assemblyinfo.cs", SearchOption.AllDirectories);
                            foreach (FileInfo af in afile)
                            {
                                if (!ainfos1.Contains(af.FullName) && !icrementedfile.Contains(af.FullName))
                                {
                                    ainfos1.Add(af.FullName);
                                }
                            }
                        }
                        
                    }
              
                }
                ainfos = ainfos1.ToArray();
            }
            catch(Exception ex)
            {

            }

            return ainfos;
        }
        private bool VBversionincrement(string filename, bool Major, bool Minor, bool Build, bool Revision)
        {
            bool istrue = false;
            string Majorversion = string.Empty;
            string Minorversion = string.Empty;
            string Buildversion = string.Empty;
            string Revisionversion = string.Empty;
            try
            {
                if (File.Exists(filename))
                {
                    // Open the file to read from.
                    string[] readText = File.ReadAllLines(filename, Encoding.Default);
                    string text = File.ReadAllText(filename, Encoding.Default);
                    // var versionRx = new Regex(@"\<Assembly: AssemblyVersion\((.*)\)\>");
                    var versionInfoLines = Array.FindAll(readText, (s => s.ToLower().StartsWith("<assembly: assemblyversion(") || s.ToLower().StartsWith("<assembly: assemblyfileversion(")));// File.ReadAllLines(filename).Select(s => s.StartsWith("<Assembly: AssemblyVersion("));
                    foreach (string v in versionInfoLines)
                    {
                        int index = v.IndexOf('(');
                        string versionm = v.Substring(index);
                        index = versionm.IndexOf(')');
                        var ver = new Version(versionm.Replace(versionm.Substring(index), "").Replace("\"", "").Replace("(", "").Replace(")", "").Replace("\\", ""));
                        string ver1 = (versionm.Replace(versionm.Substring(index), "").Replace("\"", "").Replace("(", "").Replace(")", "").Replace("\\", ""));
                        string increversion = string.Empty;
                        if (Major)
                        {
                            increversion = (ver.Major + 1).ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + ver.Revision.ToString();
                        }
                        if (Minor)
                        {
                            increversion = (ver.Major).ToString() + "." + increversion + "." + (ver.Minor + 1000).ToString() + "." + ver.Build.ToString() + "." + ver.Revision.ToString();
                        }
                        if (Build)
                        {
                            increversion = (ver.Major).ToString() + "." + ver.Minor.ToString() + "." + (ver.Build + 1).ToString() + "." + ver.Revision.ToString();
                        }
                        if (Revision)
                        {
                            increversion = (ver.Major).ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + (ver.Revision + 1).ToString();
                        }
                        string versionm1 = versionm.Replace(ver1, increversion);
                        text.Replace('"' + ver1 + '"', '"' + increversion + '"');
                        System.IO.File.WriteAllText(filename, System.IO.File.ReadAllText(filename).Replace(versionm, versionm1));
                        istrue = true;
                    }

                    // File.WriteAllText(filename, text, Encoding.Default);
                }
            }
            catch (Exception ex)
            {
                istrue = false;
            }
            return istrue;
        }
        private bool CSversionincrement(string filename, bool Major, bool Minor, bool Build, bool Revision)
        {
            bool istrue = false;
            string Majorversion = string.Empty;
            string Minorversion = string.Empty;
            string Buildversion = string.Empty;
            string Revisionversion = string.Empty;
            try
            {
                if (File.Exists(filename))
                {
                    // Open the file to read from.
                    string[] readText = File.ReadAllLines(filename, Encoding.Default);
                    string text = File.ReadAllText(filename, Encoding.Default);
                    // var versionRx = new Regex(@"\<Assembly: AssemblyVersion\((.*)\)\>");
                    var versionInfoLines = Array.FindAll(readText, (s => s.ToLower().StartsWith("[assembly: assemblyversion(") || s.ToLower().StartsWith("[assembly: assemblyfileversion(")));// File.ReadAllLines(filename).Select(s => s.StartsWith("<Assembly: AssemblyVersion("));
                    foreach (string v in versionInfoLines)
                    {
                        int index = v.IndexOf('(');
                        string versionm = v.Substring(index);
                        index = versionm.IndexOf(')');
                        var ver = new Version(versionm.Replace(versionm.Substring(index), "").Replace("\"", "").Replace("(", "").Replace(")", "").Replace("\\", ""));
                        string ver1 = (versionm.Replace(versionm.Substring(index), "").Replace("\"", "").Replace("(", "").Replace(")", "").Replace("\\", ""));
                        string increversion = string.Empty;
                        if (Major)
                        {
                            increversion = (ver.Major + 1).ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + ver.Revision.ToString();
                        }
                        if (Minor)
                        {
                            increversion = (ver.Major).ToString() + "." + increversion + "." + (ver.Minor + 1000).ToString() + "." + ver.Build.ToString() + "." + ver.Revision.ToString();
                        }
                        if (Build)
                        {
                            increversion = (ver.Major).ToString() + "." + ver.Minor.ToString() + "." + (ver.Build + 1).ToString() + "." + ver.Revision.ToString();
                        }
                        if (Revision)
                        {
                            increversion = (ver.Major).ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + (ver.Revision + 1).ToString();
                        }
                        string versionm1 = versionm.Replace(ver1, increversion);
                        text.Replace('"' + ver1 + '"', '"' + increversion + '"');
                        System.IO.File.WriteAllText(filename, System.IO.File.ReadAllText(filename).Replace(versionm, versionm1));
                        istrue = true;
                    }

                    // File.WriteAllText(filename, text, Encoding.Default);
                }
            }
            catch (Exception ex)
            {
                istrue = false;
            }
            return istrue;
        }
       
        private void OnlyModified_CheckedChanged(object sender, EventArgs e)
        {
            if (OnlyModified.Checked)
            {
                ALLFILES.Enabled = false;
                Tocheckdate.Enabled = false;
                FrmCheckdate.Enabled = false;
                ALLFILES.Checked = false;
                Tocheckdate.Checked = false;
                FrmCheckdate.Checked = false;
            }
            else
            {
                ALLFILES.Enabled = true;
                Tocheckdate.Enabled = true;
                FrmCheckdate.Enabled = true;
                //ALLFILES.Checked = true;
                //Tocheckdate.Checked = true;
                //FrmCheckdate.Checked = true;
            }

        }

        private void ALLFILES_CheckedChanged(object sender, EventArgs e)
        {
            if (ALLFILES.Checked)
            {
                OnlyModified.Enabled = false;
                Tocheckdate.Enabled = false;
                FrmCheckdate.Enabled = false;
                OnlyModified.Checked = false;
                Tocheckdate.Checked = false;
                FrmCheckdate.Checked = false;
            }
            else
            {
                OnlyModified.Enabled = true;
                Tocheckdate.Enabled = true;
                FrmCheckdate.Enabled = true;
                //OnlyModified.Checked = true;
                //Tocheckdate.Checked = true;
                //FrmCheckdate.Checked = true;
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {
           
        }

        private void FrmCheckdate_CheckedChanged(object sender, EventArgs e)
        {
            if (FrmCheckdate.Checked)
            {
                ALLFILES.Enabled = false;
                OnlyModified.Enabled = false;
                ALLFILES.Checked = false;
                OnlyModified.Checked = false;
               
            }
            else
            {
                if (!Tocheckdate.Checked)
                {
                    ALLFILES.Enabled = true;
                    OnlyModified.Enabled = true;
                }
                //ALLFILES.Enabled = true;
                //OnlyModified.Checked = true;
                
            }
        }

        private void Tocheckdate_CheckedChanged(object sender, EventArgs e)
        {
            if (Tocheckdate.Checked)
            {
                ALLFILES.Enabled = false;
                OnlyModified.Enabled = false;
                ALLFILES.Checked = false;
                OnlyModified.Checked = false;

            }
            else
            {
                if (!FrmCheckdate.Checked)
                {
                    ALLFILES.Enabled = true;
                    OnlyModified.Enabled = true;
                }
                //ALLFILES.Enabled = true;
                //OnlyModified.Checked = true;

            }
        }
        private void logfile(string logtext, bool error)
        {
            string er = "";
            StreamWriter swErrorLog;


            if (!File.Exists(logfilepath))
            {
                using (StreamWriter sw = File.CreateText(logfilepath))
                {

                }
                swErrorLog = File.AppendText(logfilepath);
                //swErrorLog.Write("/n", "Log Entry : ");

                swErrorLog.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                swErrorLog.Flush();
                swErrorLog.Close();
            }
            swErrorLog = File.AppendText(logfilepath);
            if (error)
            {
                er = "ERROR";
            }

            swErrorLog.WriteLine("  : ");
            swErrorLog.WriteLine(":" + er + ":");
            swErrorLog.WriteLine("  : {0}", logtext);

            swErrorLog.Flush();
            swErrorLog.Close();

        }

        private void DirectoryCopy(
        string sourceDirName, string destDirName, bool copySubDirs, bool javafilecompress, bool CSSFilescompress, string RdetinationDir)
        {

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            string INIPATH1 = Application.StartupPath.ToString() + "\\DipExclude.ini";
            IniFile hhh1 = new IniFile(INIPATH1);
            //string Extepath = Application.StartupPath.ToString() + "\\Exten.txt";
            //System.IO.StreamReader myFile = new System.IO.StreamReader(Extepath);
            string myString = hhh1.GetString("ExcludeFiles", "Exten", "NA");
            string Myfiles = hhh1.GetString("ExcludeFiles", "Files", "NA");
            string CopyFileTO = hhh1.GetString("COPYFILESTO", "Files", "NA");
            string[] CopyFilesTO = CopyFileTO.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] ExteFiles = Myfiles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] Exte = myString.Split(new char[] { ',' });
            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                bool istrue = false;
                foreach (string s in ExteFiles)
                {
                    if (destDirName.ToLower().Contains(s.ToLower()))
                    {
                        istrue = true;
                        break;
                    }
                }
                if (!istrue)
                {
                    Directory.CreateDirectory(destDirName);
                }
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                bool istrue = false;
                foreach (string s in ExteFiles)
                {
                    if (file.FullName.ToLower().Contains(s.ToLower()))
                    {
                        istrue = true;
                        break;
                    }
                }
                if (!istrue)
                {
                    // Create the path to the new copy of the file.
                    if (!Exte.Contains(file.Extension.ToLower()))
                    {
                        string temppath = Path.Combine(destDirName, file.Name);
                        if (File.Exists(temppath))
                        {
                            FileInfo jjjj = new FileInfo(temppath);
                            jjjj.IsReadOnly = false;
                        }
                        // Copy the file.

                        if (file.Extension.ToLower() == ".js")
                        {
                            bool isitexclude = false;
                            foreach (string s in Exlcucompre)
                            {
                                if (temppath.ToLower().Contains(s))
                                {
                                    isitexclude = true;
                                    break;
                                }

                            }
                            if (!isitexclude)
                            {
                                if (javafilecompress)
                                {
                                    //string sourcefolder = textBox1.Text;
                                    //string destinationfolder = textBox2.Text;
                                    string msbuildscriptpath = Path.Combine(Application.StartupPath, "Sourcefiles\\MsJAVABuild.xml");
                                    // logfilepath = System.Environment.CurrentDirectory + "\\Error" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_log.log";

                                    if (dir.Exists)
                                    {
                                        //var Jsfiles = Directory.GetFiles(sourcefolder, "*.js", SearchOption.AllDirectories);
                                        //foreach (string file1 in Jsfiles)
                                        //{
                                        XmlDocument xmlload = new XmlDocument();
                                        XmlNodeList nodelist;
                                        xmlload.Load(msbuildscriptpath);
                                        nodelist = xmlload.GetElementsByTagName("JavaScriptFiles");
                                        nodelist[0].Attributes["Include"].Value = file.FullName;
                                        nodelist = null;
                                        nodelist = xmlload.GetElementsByTagName("OutPutJavaFile");
                                        nodelist[0].Attributes["Include"].Value = temppath;
                                        nodelist = null;
                                        nodelist = xmlload.GetElementsByTagName("JavaScriptCompressorTask");
                                        if (Ojavafiles.Checked)
                                        {

                                            nodelist[0].Attributes["ObfuscateJavaScript"].Value = "True";
                                        }
                                        else
                                        {
                                            nodelist[0].Attributes["ObfuscateJavaScript"].Value = "False";
                                        }
                                        xmlload.Save(msbuildscriptpath);
                                        FileInfo finfo = new FileInfo(temppath);
                                        if (!finfo.Directory.Exists) Directory.CreateDirectory(finfo.Directory.FullName);

                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            finfo.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        StartProcess(@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe", '"' + msbuildscriptpath + '"', file.Name, true);
                                        logfile("Copyied File with minification From " + file.FullName + " To " + temppath + " ", false);
                                        
                                        //}0

                                        //MessageBox.Show("Done");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Folder " + dir.FullName + " does not exists");
                                    }
                                }
                                else
                                {
                                    bool iscopytrue = false;
                                    foreach (string s1 in CopyFilesTO)
                                    {
                                        iscopytrue = true;
                                        string[] copytofilessplit = s1.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (copytofilessplit.Count() == 2)
                                        {
                                            string RootPathto = copytofilessplit[1];
                                            RootPathto = RootPathto.Replace("root", rootfilepath);

                                            if (file.FullName.ToLower().Contains(copytofilessplit[0].ToLower()))
                                            {
                                                if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                                {
                                                    FileInfo Rfinfo = new FileInfo(RootPathto.Replace(destDirName, RdetinationDir));
                                                    if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                    file.CopyTo(Rfinfo.FullName);
                                                    logfile("Copyied File with minification From " + RootPathto + " To " + Rfinfo.FullName + " ", false);
                                                }
                                                file.CopyTo(RootPathto, true);
                                                logfile("Copyied File From " + file.FullName + " To " + rootfilepath + "\\" + file.Name + " ", false);
                                            }
                                            else
                                            {

                                                if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                                {
                                                    FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                                    if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                    file.CopyTo(Rfinfo.FullName);
                                                    logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                                }
                                                file.CopyTo(temppath, true);
                                                logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                            }
                                        }
                                        else
                                        {
                                            if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                            {
                                                FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                file.CopyTo(Rfinfo.FullName);
                                                logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                            }
                                            file.CopyTo(temppath, true);
                                            logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                        }
                                    }
                                    if (!iscopytrue)
                                    {
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        file.CopyTo(temppath, true);
                                        logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                    }

                                }
                            }
                            else
                            {
                                bool iscopytrue = false;
                                foreach (string s1 in CopyFilesTO)
                                {
                                    iscopytrue = true;
                                    string[] copytofilessplit = s1.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (copytofilessplit.Count() == 2)
                                    {
                                        string RootPathto = copytofilessplit[1];
                                        RootPathto = RootPathto.Replace("root", rootfilepath);
                                        if (file.FullName.ToLower().Contains(copytofilessplit[0].ToLower()))
                                        {
                                            if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                            {
                                                FileInfo Rfinfo = new FileInfo(RootPathto.Replace(destDirName, RdetinationDir));
                                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                file.CopyTo(Rfinfo.FullName);
                                                logfile("Copyied File with minification From " + RootPathto + " To " + Rfinfo.FullName + " ", false);
                                            }
                                            file.CopyTo(RootPathto, true);
                                            logfile("Copyied File From " + file.FullName + " To " + rootfilepath + "\\" + file.Name + " ", false);
                                        }
                                        else
                                        {
                                            if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                            {
                                                FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                file.CopyTo(Rfinfo.FullName);
                                                logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                            }
                                            file.CopyTo(temppath, true);
                                            logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                        }
                                    }
                                    else
                                    {
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        file.CopyTo(temppath, true);
                                        logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                    }
                                }
                                if (!iscopytrue)
                                {
                                    if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                    {
                                        FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                        if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                        file.CopyTo(Rfinfo.FullName);
                                        logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                    }
                                    file.CopyTo(temppath, true);
                                    logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                }
                            }
                        }


                        if (file.Extension.ToLower() == ".css")
                        {
                            bool isitexclude = false;
                            foreach (string s in Exlcucompre)
                            {
                                if (temppath.ToLower().Contains(s))
                                {
                                    isitexclude = true;
                                    break;
                                }

                            }
                            if (!isitexclude)
                            {
                                if (CSSFilescompress)
                                {
                                    string msbuildscriptpath = Path.Combine(Application.StartupPath, "Sourcefiles\\MsCSSBuild.xml");
                                    if (dir.Exists)
                                    {
                                        //var Jsfiles = Directory.GetFiles(sourcefolder, "*.js", SearchOption.AllDirectories);
                                        //foreach (string file1 in Jsfiles)
                                        //{
                                        XmlDocument xmlload = new XmlDocument();
                                        XmlNodeList nodelist;
                                        xmlload.Load(msbuildscriptpath);
                                        nodelist = xmlload.GetElementsByTagName("CSSScriptFiles");
                                        nodelist[0].Attributes["Include"].Value = file.FullName;
                                        nodelist = null;
                                        nodelist = xmlload.GetElementsByTagName("OutPutCSSFile");
                                        nodelist[0].Attributes["Include"].Value = temppath;
                                        xmlload.Save(msbuildscriptpath);
                                        FileInfo finfo = new FileInfo(temppath);
                                        if (!finfo.Directory.Exists) Directory.CreateDirectory(finfo.Directory.FullName);
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        StartProcess(@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe", '"' + msbuildscriptpath + '"', file.Name, true);
                                        logfile("Copyied File With Minification From " + file.FullName + " To " + temppath + " ", false);
                                        //}0

                                        //MessageBox.Show("Done");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Folder " + dir.FullName + " does not exists");
                                    }
                                }
                                else
                                {
                                    bool iscopytrue = false;
                                    foreach (string s1 in CopyFilesTO)
                                    {
                                        iscopytrue = true;
                                        string[] copytofilessplit = s1.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                        if (copytofilessplit.Count() == 2)
                                        {
                                            string RootPathto = copytofilessplit[1];
                                            RootPathto = RootPathto.Replace("root", rootfilepath);
                                            if (file.FullName.ToLower().Contains(copytofilessplit[0].ToLower()))
                                            {
                                                if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                                {
                                                    FileInfo Rfinfo = new FileInfo(RootPathto.Replace(destDirName, RdetinationDir));
                                                    if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                    file.CopyTo(Rfinfo.FullName);
                                                    logfile("Copyied File with minification From " + RootPathto + " To " + Rfinfo.FullName + " ", false);
                                                }
                                                file.CopyTo(RootPathto, true);
                                                logfile("Copyied File From " + file.FullName + " To " + rootfilepath + "\\" + file.Name + " ", false);
                                            }
                                            else
                                            {
                                                if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                                {
                                                    FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                                    if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                    file.CopyTo(Rfinfo.FullName);
                                                    logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                                }
                                                file.CopyTo(temppath, true);
                                                logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                            }
                                        }
                                        else
                                        {
                                            if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                            {
                                                FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                file.CopyTo(Rfinfo.FullName);
                                                logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                            }
                                            file.CopyTo(temppath, true);
                                            logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                        }
                                    }
                                    if (!iscopytrue)
                                    {
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        file.CopyTo(temppath, true);
                                        logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                    }
                                }
                            }
                            else
                            {
                                bool iscopytrue = false;
                                foreach (string s1 in CopyFilesTO)
                                {
                                    iscopytrue = true;
                                    string[] copytofilessplit = s1.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (copytofilessplit.Count() == 2)
                                    {
                                        string RootPathto = copytofilessplit[1];
                                        RootPathto = RootPathto.Replace("root", rootfilepath);
                                        if (file.FullName.ToLower().Contains(copytofilessplit[0].ToLower()))
                                        {
                                            if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                            {
                                                FileInfo Rfinfo = new FileInfo(RootPathto.Replace(destDirName, RdetinationDir));
                                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                file.CopyTo(Rfinfo.FullName);
                                                logfile("Copyied File with minification From " + RootPathto + " To " + Rfinfo.FullName + " ", false);
                                            }
                                            file.CopyTo(RootPathto, true);
                                            logfile("Copyied File From " + file.FullName + " To " + rootfilepath + "\\" + file.Name + " ", false);
                                        }
                                        else
                                        {
                                            if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                            {
                                                FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                                file.CopyTo(Rfinfo.FullName);
                                                logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                            }

                                            file.CopyTo(temppath, true);
                                            logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                        }
                                    }
                                    else
                                    {
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        file.CopyTo(temppath, true);
                                        logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                    }
                                }
                                if (!iscopytrue)
                                {
                                    if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                    {
                                        FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                        if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                        file.CopyTo(Rfinfo.FullName);
                                        logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                    }
                                    file.CopyTo(temppath, true);
                                    logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                }
                            }
                            // file.CopyTo(temppath, true);
                        }
                        else if (file.Extension.ToLower() != ".js")
                        {
                            bool iscopytrue = false;
                            foreach (string s1 in CopyFilesTO)
                            {
                                iscopytrue = true;
                                string[] copytofilessplit = s1.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                if (copytofilessplit.Count() == 2)
                                {
                                    string RootPathto = copytofilessplit[1];
                                    RootPathto = RootPathto = RootPathto.Replace("root", rootfilepath);
                                    if (file.FullName.ToLower().Contains(copytofilessplit[0].ToLower()))
                                    {
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(RootPathto.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + RootPathto + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        file.CopyTo(RootPathto, true);
                                        logfile("Copyied File From " + file.FullName + " To " + rootfilepath + "\\" + file.Name + " ", false);
                                    }
                                    else
                                    {
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        file.CopyTo(temppath, true);
                                        logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                    }
                                }
                                else
                                {
                                    if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                    {
                                        FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                        if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                        file.CopyTo(Rfinfo.FullName);
                                        logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                    }
                                    file.CopyTo(temppath, true);
                                    logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                }
                            }
                            if (!iscopytrue)
                            {
                                if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                {
                                    FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                    if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                    file.CopyTo(Rfinfo.FullName);
                                    logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                }
                                file.CopyTo(temppath, true);
                                logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                            }
                        }

                        if (!javafilecompress && !CSSFilescompress)
                        {
                            bool iscopytrue = false;
                            foreach (string s1 in CopyFilesTO)
                            {
                                iscopytrue = true;
                                string[] copytofilessplit = s1.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                if (copytofilessplit.Count() == 2)
                                {
                                    string RootPathto = copytofilessplit[1];
                                    RootPathto = RootPathto.Replace("root", rootfilepath);
                                    if (file.FullName.ToLower().Contains(copytofilessplit[0].ToLower()))
                                    {
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(RootPathto.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + RootPathto + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        file.CopyTo(RootPathto, true);
                                        logfile("Copyied File From " + file.FullName + " To " + rootfilepath + "\\" + file.Name + " ", false);
                                    }
                                    else
                                    {
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(RootPathto.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + RootPathto + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        file.CopyTo(temppath, true);
                                        logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                    }
                                }
                                else
                                {
                                    if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                    {
                                        FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                        if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                        file.CopyTo(Rfinfo.FullName);
                                        logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                    }
                                    file.CopyTo(temppath, true);
                                    logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                }
                            }
                            if (!iscopytrue)
                            {
                                if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                {
                                    FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                    if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                    file.CopyTo(Rfinfo.FullName);
                                    logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                }
                                file.CopyTo(temppath, true);
                                logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                            }
                        }



                    }
                }
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    if (subdir.Name.ToLower() != "obj" && subdir.Name.ToLower() != "bin")
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);

                        // Copy the subdirectories.
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs, javafilecompress, CSSFilescompress,RdetinationDir);
                    }
                }
            }
        }
        private void DirectoryCopy1(
         string sourceDirName, string destDirName, bool copySubDirs, bool javafilecompress, bool CSSFilescompress, string RdetinationDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();
            //string Extepath = Application.StartupPath.ToString() + "\\ExtenBin.txt";
            ////string Extepath = Application.StartupPath.ToString() + "\\Exten.txt";
            //System.IO.StreamReader myFile = new System.IO.StreamReader(Extepath);
            //string myString = myFile.ReadToEnd();
            string INIPATH1 = Application.StartupPath.ToString() + "\\DipExclude.ini";
            IniFile hhh1 = new IniFile(INIPATH1);
            //string Extepath = Application.StartupPath.ToString() + "\\Exten.txt";
            //System.IO.StreamReader myFile = new System.IO.StreamReader(Extepath);
            string myString = hhh1.GetString("ExcludeBin", "Exten", "NA");
            string Myfiles = hhh1.GetString("ExcludeBin", "Files", "NA");
            string[] Exte = myString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] ExteFiles = Myfiles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                bool istrue = false;
                foreach (string s in ExteFiles)
                {
                    if (destDirName.ToLower().Contains(s.ToLower()))
                    {
                        istrue = true;
                        break;
                    }
                }
                if (!istrue)
                {
                    Directory.CreateDirectory(destDirName);
                }
                // Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                //if (!Exte.Contains(file.Extension.ToLower()))
                //{
                bool istrue = false;
                foreach (string s in ExteFiles)
                {
                    if (file.FullName.ToLower().Contains(s.ToLower()))
                    {
                        istrue = true;
                    }
                }
                if (!istrue)
                {
                    if (!Exte.Contains(file.Extension.ToLower()))
                    {
                        string temppath = Path.Combine(destDirName, file.Name);
                        if (File.Exists(temppath))
                        {
                            FileInfo jjjj = new FileInfo(temppath);
                            jjjj.IsReadOnly = false;
                        }
                        // Copy the file.

                        if (file.Extension.ToLower() == ".js")
                        {
                            bool isitexclude = false;
                            foreach (string s in Exlcucompre)
                            {
                                if (temppath.ToLower().Contains(s))
                                {
                                    isitexclude = true;
                                    break;
                                }

                            }
                            if (!isitexclude)
                            {
                                if (javafilecompress)
                                {
                                    string msbuildscriptpath = Path.Combine(Application.StartupPath, "Sourcefiles\\MsJAVABuild.xml");
                                    if (dir.Exists)
                                    {
                                        //var Jsfiles = Directory.GetFiles(sourcefolder, "*.js", SearchOption.AllDirectories);
                                        //foreach (string file1 in Jsfiles)
                                        //{
                                        XmlDocument xmlload = new XmlDocument();
                                        XmlNodeList nodelist;
                                        xmlload.Load(msbuildscriptpath);
                                        nodelist = xmlload.GetElementsByTagName("JavaScriptFiles");
                                        nodelist[0].Attributes["Include"].Value = file.FullName;
                                        nodelist = null;
                                        nodelist = xmlload.GetElementsByTagName("OutPutJavaFile");
                                        nodelist[0].Attributes["Include"].Value = temppath;
                                        nodelist = null;
                                        nodelist = xmlload.GetElementsByTagName("JavaScriptCompressorTask");
                                        if (Ojavafiles.Checked)
                                        {

                                            nodelist[0].Attributes["ObfuscateJavaScript"].Value = "True";
                                        }
                                        else
                                        {
                                            nodelist[0].Attributes["ObfuscateJavaScript"].Value = "False";
                                        }
                                        xmlload.Save(msbuildscriptpath);
                                        FileInfo finfo = new FileInfo(temppath);
                                        if (!finfo.Directory.Exists) Directory.CreateDirectory(finfo.Directory.FullName);
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        StartProcess(@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe", '"' + msbuildscriptpath + '"', file.Name, true);
                                        logfile("Copyied File With Minification From " + file.FullName + " To " + temppath + " ", false);
                                        //}0

                                        //MessageBox.Show("Done");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Folder " + dir.FullName + " does not exists");
                                    }
                                }
                                else
                                {
                                    if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                    {
                                        FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                        if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                        file.CopyTo(Rfinfo.FullName);
                                        logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                    }
                                    file.CopyTo(temppath, true);
                                    logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                }
                            }
                            else
                            {
                                if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                {
                                    FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                    if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                    file.CopyTo(Rfinfo.FullName);
                                    logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                }
                                file.CopyTo(temppath, true);
                                logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                            }
                        }

                        if (file.Extension.ToLower() == ".css")
                        {
                            bool isitexclude = false;
                            foreach (string s in Exlcucompre)
                            {
                                if (temppath.ToLower().Contains(s))
                                {
                                    isitexclude = true;
                                    break;
                                }

                            }
                            if (!isitexclude)
                            {
                                if (CSSFilescompress)
                                {
                                    string msbuildscriptpath = Path.Combine(Application.StartupPath, "Sourcefiles\\MsCSSBuild.xml");
                                    if (dir.Exists)
                                    {
                                        //var Jsfiles = Directory.GetFiles(sourcefolder, "*.js", SearchOption.AllDirectories);
                                        //foreach (string file1 in Jsfiles)
                                        //{
                                        XmlDocument xmlload = new XmlDocument();
                                        XmlNodeList nodelist;
                                        xmlload.Load(msbuildscriptpath);
                                        nodelist = xmlload.GetElementsByTagName("CSSScriptFiles");
                                        nodelist[0].Attributes["Include"].Value = file.FullName;
                                        nodelist = null;
                                        nodelist = xmlload.GetElementsByTagName("OutPutCSSFile");
                                        nodelist[0].Attributes["Include"].Value = temppath;
                                        xmlload.Save(msbuildscriptpath);
                                        FileInfo finfo = new FileInfo(temppath);
                                        if (!finfo.Directory.Exists) Directory.CreateDirectory(finfo.Directory.FullName);
                                        if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                        {
                                            FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                            if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                            file.CopyTo(Rfinfo.FullName);
                                            logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                        }
                                        StartProcess(@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe", '"' + msbuildscriptpath + '"', file.Name, true);
                                        logfile("Copyied File With Minification From " + file.FullName + " To " + temppath + " ", false);
                                        //}0

                                        //MessageBox.Show("Done");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Folder " + dir.FullName + " does not exists");
                                    }
                                }
                                else
                                {
                                    if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                    {
                                        FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                        if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                        file.CopyTo(Rfinfo.FullName);
                                        logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                    }
                                    file.CopyTo(temppath, true);
                                    logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                                }
                            }
                            else
                            {
                                if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                                {
                                    FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                    if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                    file.CopyTo(Rfinfo.FullName);
                                    logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                                }
                                file.CopyTo(temppath, true);
                                logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                            }
                            // file.CopyTo(temppath, true);
                        }
                        else if (file.Extension.ToLower() != ".js")
                        {
                            if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                            {
                                FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                file.CopyTo(Rfinfo.FullName);
                                logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                            }
                            file.CopyTo(temppath, true);
                            logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                        }

                        if (!javafilecompress && !CSSFilescompress)
                        {
                            if (ALLModifiedFilesList.Contains(file.FullName) || ALLModifiedDllList.Contains(file.FullName))
                            {
                                FileInfo Rfinfo = new FileInfo(temppath.Replace(destDirName, RdetinationDir));
                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                file.CopyTo(Rfinfo.FullName);
                                logfile("Copyied File with minification From " + temppath + " To " + Rfinfo.FullName + " ", false);
                            }
                            file.CopyTo(temppath, true);
                            logfile("Copyied File From " + file.FullName + " To " + temppath + " ", false);
                        }
                    }
                }
                //}
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    if (subdir.Name.ToLower() != "_PublishedWebsites".ToLower())
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);

                        // Copy the subdirectories.
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs, javafilecompress, CSSFilescompress,RdetinationDir);
                    }
                }
            }
        }
        private bool CreatePatch(string sourcefolder, string sourcebin, string destinationfolder, string RdetinationDir)
        {

            bool istrue = false;
            if (!string.IsNullOrEmpty(sourcefolder))
            {
                logfile("Create Path With Deployment Structure begin : " + DateTime.Now.ToString(), false);
                if (Directory.Exists(destinationfolder))
                {
                    var allf = Directory.GetFiles(destinationfolder, "*", SearchOption.AllDirectories);
                    foreach (string fin in allf)
                    {
                        FileInfo fg = new FileInfo(fin);
                        fg.IsReadOnly = false;
                        fg.Delete();
                    }
                    var dd = Directory.GetDirectories(destinationfolder, "*", SearchOption.AllDirectories);
                    if (dd.Count() != 0)
                    {
                        Directory.Delete(destinationfolder, true);
                    }
                    else
                    {
                        Directory.Delete(destinationfolder);
                    }
                }

                try
                {

                    string BinFolder = string.Empty;
                    string DirPath = string.Empty;
                    string DestiDir = string.Empty;
                    string INIPATH = Application.StartupPath.ToString() + "\\Dip.ini";
                    IniFile hhh = new IniFile(INIPATH);
                    var DDirs = hhh.GetSectionNames();
                    rootfilepath = destinationfolder;
                    DestiDir = destinationfolder;
                    DirPath = sourcefolder;
                    BinFolder = sourcebin;
                    //if (Directory.Exists(DestiDir)) Directory.Delete(DestiDir, true);
                    bool javacom = false;
                    bool csscom = false;
                    if (JavaMinification.Checked)
                    {
                        javacom = true;
                    }
                    if (CSSMinification.Checked)
                    {
                        csscom = true;
                    }
                    List<string> ListDIRS = new List<string>();
                    foreach (string g in DDirs)
                    {
                        if (g != "AxCommon")
                        {
                            string ProName = hhh.GetString(g, "ProjFile", "NA");
                            var Dirs = Directory.GetFiles(DirPath, ProName, SearchOption.AllDirectories);
                            // string h = g.ToLower();
                            // DirectoryInfo jk = new DirectoryInfo(h);


                            if (Dirs.Count() != 0)
                            {

                                FileInfo jk = new FileInfo(Dirs[0]);
                                if (jk.Directory.Name.ToLower() != "obj" && jk.Directory.Name.ToLower() != "bin")
                                {
                                    if (webmodules.Contains(g))
                                    {
                                        DirectoryCopy(jk.Directory.ToString(), DestiDir + "\\" + g, true, javacom, csscom, DestiDir.Replace(DestiDir,RdetinationDir));
                                    }
                                }
                            }
                            // ListDIRS.Add(jk.Name);
                            // if (h.Contains(SrFolder))
                            //{
                            //    DirectoryCopy(h, DestiDir, true);
                            //}
                        }
                    }

                    DirectoryCopy1(BinFolder, DestiDir + "\\" + "Bin", true, javacom, csscom, (DestiDir+ "\\" + "Bin").Replace(DestiDir, RdetinationDir));


                    var rootfiles = Directory.GetFiles(DirPath, "*", SearchOption.TopDirectoryOnly);
                    foreach (string f in rootfiles)
                    {
                        FileInfo Fin = new FileInfo(f);
                        string INIPATH1 = Application.StartupPath.ToString() + "\\DipExclude.ini";
                        IniFile hhh1 = new IniFile(INIPATH1);
                        //string Extepath = Application.StartupPath.ToString() + "\\Exten.txt";
                        //System.IO.StreamReader myFile = new System.IO.StreamReader(Extepath);
                        string myString = hhh1.GetString("ExcludeFiles", "Exten", "NA");
                        //string Extepath = Application.StartupPath.ToString() + "\\Exten.txt";
                        //System.IO.StreamReader myFile = new System.IO.StreamReader(Extepath);
                        //string myString = myFile.ReadToEnd();
                        string[] Exte = myString.Split(new char[] { ',' });
                        if (!Exte.Contains(Fin.Extension.ToLower()))
                        {
                            FileInfo ff = new FileInfo(f.Replace(DirPath, DestiDir));
                            if (ff.Exists) ff.IsReadOnly = false;
                            File.Copy(f, f.Replace(DirPath, DestiDir), true);
                            if (ALLModifiedFilesList.Contains(f) || ALLModifiedDllList.Contains(f))
                            {
                                FileInfo Rfinfo = new FileInfo(f.Replace(DirPath, RdetinationDir));
                                if (!Rfinfo.Directory.Exists) Directory.CreateDirectory(Rfinfo.Directory.FullName);
                                File.Copy(f, f.Replace(DirPath, RdetinationDir), true);
                                //finfo.CopyTo(Rfinfo.FullName);
                                logfile("Copyied File with minification From " + f + " To " + f.Replace(DirPath, RdetinationDir) + " ", false);
                            }
                        }
                    }
                    foreach (var directory in Directory.GetDirectories(DestiDir, "*", SearchOption.AllDirectories))
                    {
                        //processDirectory(directory);
                        if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                        {
                            Directory.Delete(directory, false);
                        }
                    }

                    foreach (string g in DDirs)
                    {
                        if (g == "AxCommon")
                        {

                            foreach (string pp in hhh.GetKeyNames(g))
                            {
                                string ProName = hhh.GetString(g, pp, "NA");
                                var Dirs = Directory.GetDirectories(DirPath, ProName, SearchOption.AllDirectories);
                                if (webmodules.Contains(g))
                                {
                                    DirectoryCopy1(Dirs[0], DestiDir + "\\" + pp, true, javacom, csscom, DestiDir.Replace(DestiDir, RdetinationDir));
                                }
                            }
                        }
                    }
                    logfile("Create Path With Deployment Structure End : " + DateTime.Now.ToString(), false);
                    istrue = true;

                }
                catch (Exception ex)
                {
                    logfile(ex.ToString(), true);
                }
            }

            return istrue;
        }
        public Process StartProcess(string cmd, string args, string filename, bool createnowindow)
        {
            ProcessStartInfo psi = new ProcessStartInfo(cmd);
            Process p = new Process();
            try
            {
                psi.CreateNoWindow = createnowindow;
                psi.UseShellExecute = false;
                psi.Verb = "runas";
                p.StartInfo = psi;
                p.StartInfo.Arguments = string.Format(args);

                p.StartInfo.RedirectStandardError = true;
                p.Start();
                p.WaitForExit();
                p.Close();

            }
            catch (Exception ex)
            {
                logfile(filename + " Not Compressed And Error is : " + ex.ToString(), true);

            }
            return p;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string INIPATH = Application.StartupPath.ToString() + "\\Dip.ini";
            IniFile hhh = new IniFile(INIPATH);
            var DDirs = hhh.GetSectionNames();
            foreach (string g in DDirs)
            {
                checkLBWEB.Items.Add(g);
            }
            string INIPATH1 = Application.StartupPath.ToString() + "\\DipExclude.ini";
            IniFile hhh1 = new IniFile(INIPATH1);
            //  var DDirs1 = hhh1.GetSectionValues("ExcludeFile");
            string ProName = hhh1.GetString("ExcludeFile", "FilePath", "");
            var DDirs1 = ProName.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string g in DDirs1)
            {
                Exlcucompre.Add(g.ToLower());
            }
        }
        int i123 = 0;
        private string[] GetDLLSList(List<KeyValuePair<string,string>> allmodifiedfiles,string excelicarebin)
        {
            List<string> files = new List<string>();
            foreach (KeyValuePair<string, string> f in allmodifiedfiles)
            {
                FileInfo finfo = new FileInfo(f.Value);
               if(finfo.Extension == ".vb" || finfo.Extension == ".cs")
               {
                   var getvbprojfile = finfo.Directory.GetFiles("*.vbproj");
                   var getcsprojfile = finfo.Directory.GetFiles("*.csproj");
                   if (getvbprojfile == null && getcsprojfile == null)
                   {
                       getvbprojfile = finfo.Directory.Parent.GetFiles("*.vbproj");
                       getcsprojfile = finfo.Directory.Parent.GetFiles("*.csproj");
                   }
                   foreach (FileInfo fi in getvbprojfile)
                   {
                       if(fi.Exists)
                       {
                           XmlDocument xmlload = new XmlDocument();
                           XmlNodeList nodelist;
                           XmlNodeList nodelist1;
                           xmlload.Load((fi.FullName));
                           nodelist = xmlload.GetElementsByTagName("AssemblyName"); 
                           nodelist1 = xmlload.GetElementsByTagName("OutputType");
                          if(nodelist1[0].ChildNodes[0].Value.ToLower()=="WinExe".ToLower())
                          {
                              string dllanme = Path.Combine(excelicarebin,nodelist[0].ChildNodes[0].Value+".exe");


                              if(!files.Contains(dllanme))
                              {
                                  files.Add(dllanme);
                              }
                          }
                          else if (nodelist1[0].ChildNodes[0].Value.ToLower() == "Library".ToLower())
                          {
                              string dllanme = Path.Combine(excelicarebin, nodelist[0].ChildNodes[0].Value + ".dll");


                              if (!files.Contains(dllanme))
                              {
                                  files.Add(dllanme);
                              }
                          }
                           
                           // WinExe
                          // Library
                       }
                   }
                   foreach (FileInfo fi in getcsprojfile)
                   {
                       if (fi.Exists)
                       {
                           XmlDocument xmlload = new XmlDocument();
                           XmlNodeList nodelist;
                           XmlNodeList nodelist1;
                           xmlload.Load((fi.FullName));
                           nodelist = xmlload.GetElementsByTagName("AssemblyName");
                           nodelist1 = xmlload.GetElementsByTagName("OutputType");
                           if (nodelist1[0].ChildNodes[0].Value.ToLower() == "WinExe".ToLower())
                           {
                               string dllanme = Path.Combine(excelicarebin, nodelist[0].ChildNodes[0].Value + ".exe");


                               if (!files.Contains(dllanme))
                               {
                                   files.Add(dllanme);
                               }
                           }
                           else if (nodelist1[0].ChildNodes[0].Value.ToLower() == "Library".ToLower())
                           {
                               string dllanme = Path.Combine(excelicarebin, nodelist[0].ChildNodes[0].Value + ".dll");


                               if (!files.Contains(dllanme))
                               {
                                   files.Add(dllanme);
                               }
                           }
                       }
                   }
                  // files.Add(f.Value);
               }
               
            }
            return files.ToArray();

        }
        private string [] getfilesfromlist(List<KeyValuePair<string,string>> allmodifiedfiles)
        {
            List<string> files = new List<string>();
            foreach (KeyValuePair<string, string> f in allmodifiedfiles)
            {
                files.Add(f.Value);
            }
            return files.ToArray();
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            AxVSSAdmin Axvssclass = new AxVSSAdmin();
            CopyFolder Axcopy = new CopyFolder();
            AXRemote axremotly = new AXRemote();
            AxIISAdmin axiis = new AxIISAdmin();
            string INIPATH1 = Application.StartupPath.ToString() + "\\Deploy.ini";
            IniFile hhh1 = new IniFile(INIPATH1);
            string rootdir = string.Empty;
            string rootproject = string.Empty;
            string vssIni = string.Empty;
            string user = string.Empty;
            string pwd = string.Empty;
            string fromdateinstring = string.Empty;
            string Todateinstring = string.Empty;
            if (checkProduct)
            {
                fromdateinstring = dateTimePicker1.Value.ToShortDateString() + " " + dateTimePicker3.Value.ToShortTimeString();
                Todateinstring = dateTimePicker2.Value.ToShortDateString() + " " + dateTimePicker4.Value.ToShortTimeString();
                rootdir = hhh1.GetString("VssPath", "Rootdir", "");
                WrokFolder = rootdir;
                rootproject = hhh1.GetString("VssPath", "RootVSSProject", "");
                vssIni = hhh1.GetString("VssPath", "VssIniPath", ""); ;
                user = hhh1.GetString("VssPath", "Username", "");
                pwd = hhh1.GetString("VssPath", "Pwd", "");
                string projfile = hhh1.GetString("VssPath", "ProjFilePath", "");
                if ((checkOnlyModified || checkAllFiles || checkFromDateTime || checkToDateTime))
                {
                    if (checkFromDateTime || checkToDateTime)
                    {
                        this.backgroundWorker1.ReportProgress(-1, string.Format("startGetting Latest From VSS..." + i123));
                        if (Axvssclass.GetLatest(vssIni, user, pwd, rootproject, rootdir, 128, checkFromDateTime, checkToDateTime, fromdateinstring, Todateinstring))
                        {
                            ALLModifiedFilesList = getfilesfromlist(Axvssclass.allmodifiedfiles);
                            XmlDocument xmlload = new XmlDocument();
                            XmlNodeList nodelist;
                            xmlload.Load(projfile);
                            nodelist = xmlload.GetElementsByTagName("OutputDir");

                            ALLModifiedDllList = GetDLLSList(Axvssclass.allmodifiedfiles, nodelist[0].ChildNodes[0].Value);
                            this.backgroundWorker1.ReportProgress(-1, string.Format("true/Getting Latest From VSS Completed"));
                            getlateststatus = true;
                        }
                        else
                        {
                            this.backgroundWorker1.ReportProgress(-1, string.Format("false/Error on Getting Latest From VSS please check error log"));
                            getlateststatus = false;
                        }
                        i123++;
                    }
                    else if (OnlyModified.Checked || ALLFILES.Checked)
                    {
                        this.backgroundWorker1.ReportProgress(-1, string.Format("startGetting Latest From VSS..." + i123));
                        if (Axvssclass.GetLatest(vssIni, user, pwd, rootproject, rootdir, 128, checkOnlyModified, checkAllFiles))
                        {
                            ALLModifiedFilesList = getfilesfromlist(Axvssclass.allmodifiedfiles);
                            XmlDocument xmlload = new XmlDocument();
                            XmlNodeList nodelist;
                            xmlload.Load(projfile);
                            nodelist = xmlload.GetElementsByTagName("OutputDir");

                            ALLModifiedDllList = GetDLLSList(Axvssclass.allmodifiedfiles, nodelist[0].ChildNodes[0].Value);

                            this.backgroundWorker1.ReportProgress(-1, string.Format("true/Getting Latest From VSS Completed Successfully"));
                            getlateststatus = true;
                        }
                        else
                        {
                            this.backgroundWorker1.ReportProgress(-1, string.Format("false/Error on Getting Latest From VSS please check error log"));
                            getlateststatus = false;
                        }
                        i123++;
                    }

                }
                else
                {
                    getlateststatus = true;
                }
                if (getlateststatus)
                {


                    if (checkversionincreament)
                    {


                        string[] AssemblyFiles = getallassemblyinfos(getfilesfromlist(Axvssclass.allmodifiedfiles));
                        if (AssemblyFiles != null)
                        {
                            if (AssemblyFiles.Count() != 0)
                            {
                                if (Increamentversion(AssemblyFiles))
                                {
                                    versionincrementstatus = true;
                                }
                                else
                                {
                                    versionincrementstatus = false;
                                }
                            }
                            else
                            {
                                versionincrementstatus = true;
                            }
                        }
                        else
                        {
                            versionincrementstatus = true;
                        }

                    }
                    else
                    {
                        versionincrementstatus = true;
                    }
                    if (versionincrementstatus)
                    {

                        if (checkcompilation)
                        {
                            this.backgroundWorker1.ReportProgress(-1, string.Format("startCompiling Projects...." + i123));
                            logfile("Compilation Start time : " + DateTime.Now.ToString(), false);
                            string projpath = hhh1.GetString("ProjfilePath", "FilePath", "");
                            StartProcess(@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe", '"' + projpath + '"' + " /flp:logfile=buildlog.txt;errorsonly", projpath, false);
                            FileInfo buildlogFinfo = new FileInfo(Path.Combine(Application.StartupPath, "buildlog.txt"));
                            if (buildlogFinfo.Exists)
                            {

                                if (buildlogFinfo.Length == 0)
                                {
                                    this.backgroundWorker1.ReportProgress(-1, string.Format("true/Compiled Projects Successfully"));
                                    compilationstatus = true;
                                }
                                else
                                {
                                    compilationstatus = false;
                                    this.backgroundWorker1.ReportProgress(-1, string.Format("false/Error on Project Compilation"));
                                    logfile(File.ReadAllText(Path.Combine(Application.StartupPath, "buildlog.txt")), true);
                                }
                            }
                            logfile("Compilation End time : " + DateTime.Now.ToString(), false);
                            i123++;
                        }
                        else
                        {
                            compilationstatus = true;
                        }

                        if (compilationstatus)
                        {
                            if (checkcreatepatch)
                            {
                                this.backgroundWorker1.ReportProgress(-1, string.Format("startCreating Cumulative Path...." + i123));
                                string BinFolder = string.Empty;
                                string DirPath = string.Empty;
                                string DestiDir = string.Empty;
                                string RdetinationDir = string.Empty;
                                DestiDir = Path.Combine(hhh1.GetString("DeployeFolders", "Destinationdir", ""), "FullBuilds");
                                RdetinationDir = Path.Combine(hhh1.GetString("DeployeFolders", "Destinationdir", ""), "Replacement Patch");
                                DirPath = hhh1.GetString("DeployeFolders", "Sourcedir", "");
                                BinFolder = hhh1.GetString("DeployeFolders", "BinDir", "");
                                if (CreatePatch(DirPath, BinFolder, DestiDir, RdetinationDir))
                                {
                                    this.backgroundWorker1.ReportProgress(-1, string.Format("true/Created Cumulative Path Successfully"));
                                    CreatePatchstatus = true;

                                }
                                else
                                {
                                    this.backgroundWorker1.ReportProgress(-1, string.Format("false/Error on Creating Cumulative Path"));
                                    CreatePatchstatus = false;
                                }
                                i123++;
                            }
                            else
                            {
                                CreatePatchstatus = true;
                            }

                            if (CreatePatchstatus)
                            {
                                if (checkprotectdlls)
                                {
                                    string prodllpath = Path.Combine(hhh1.GetString("DeployeFolders", "Destinationdir", ""), "Bin");
                                    this.backgroundWorker1.ReportProgress(-1, string.Format("startProtecting dlls...." + i123));
                                    if (Protectdlls(prodllpath))
                                    {
                                        this.backgroundWorker1.ReportProgress(-1, string.Format("true/Protected dlls Successfully"));
                                        protectdllsstatus = true;
                                    }
                                    else
                                    {
                                        protectdllsstatus = false;
                                        this.backgroundWorker1.ReportProgress(-1, string.Format("false/Error on Protecting dlls"));
                                    }
                                    i123++;
                                }
                                else
                                {

                                    protectdllsstatus = true;
                                }
                                if (protectdllsstatus)
                                {
                                    if (checkcombinecss)
                                    {
                                        try
                                        {
                                            this.backgroundWorker1.ReportProgress(-1, string.Format("startCombining CSS Files...." + i123));
                                            string Folderq = hhh1.GetString("DeployeFolders", "Destinationdir", "");
                                            logfile("Combined  minifiction css files Begin : " + DateTime.Now.ToString(), false);
                                            string INIPATH12 = Application.StartupPath.ToString() + "\\DipExclude.ini";
                                            IniFile hhh12 = new IniFile(INIPATH12);
                                            var Split1 = (hhh12.GetString("CompressFolder", "Key", "")).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                            foreach (string h in Split1)
                                            {
                                                var split2 = h.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                                string foldername = Path.Combine(Folderq, split2[0]);
                                                var fileslist = split2[1].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                                string outputpath = Path.Combine(foldername, split2[2]);
                                                string outputpath1 = Path.Combine(foldername, "mainaxsys.css");
                                                string msbuildscriptpath = Path.Combine(Application.StartupPath, "Sourcefiles\\MsComCSSBuild.xml");
                                                XmlDocument xmlload = new XmlDocument();
                                                XmlNodeList nodelist;
                                                xmlload.Load(msbuildscriptpath);
                                                nodelist = xmlload.GetElementsByTagName("ItemGroup");
                                                nodelist[0].RemoveAll();
                                                XmlNode nn = xmlload.CreateNode(XmlNodeType.Element, "ItemGroup", null);
                                                nodelist = null;
                                                nodelist = xmlload.GetElementsByTagName("ItemGroup");
                                                foreach (string j in fileslist)
                                                {
                                                    XmlNode nn1 = xmlload.CreateNode(XmlNodeType.Element, "CSSScriptFiles", null);
                                                    XmlAttribute quantity = xmlload.CreateAttribute("Include");
                                                    quantity.Value = Path.Combine(foldername, j);

                                                    nn1.Attributes.Append(quantity);
                                                    nodelist[0].AppendChild(nn1);
                                                }
                                                XmlNode nn12 = xmlload.CreateNode(XmlNodeType.Element, "OutPutCSSFile", null);
                                                XmlAttribute quantity1 = xmlload.CreateAttribute("Include");
                                                quantity1.Value = outputpath1;

                                                nn12.Attributes.Append(quantity1);
                                                nodelist[0].AppendChild(nn12);

                                                xmlload.Save(msbuildscriptpath);
                                                string text = File.ReadAllText(msbuildscriptpath).Replace("xmlns=" + '"' + '"', "");

                                                File.WriteAllText(msbuildscriptpath, text);
                                                StartProcess(@"C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe", '"' + msbuildscriptpath + '"', Path.Combine(Folderq, split2[2]), true);
                                                foreach (string j in fileslist)
                                                {
                                                    File.Delete(Path.Combine(foldername, j));
                                                }
                                                if (File.Exists(outputpath1))
                                                {
                                                    if (File.Exists(outputpath)) File.Delete(outputpath);
                                                    File.Move(outputpath1, outputpath);
                                                }
                                            }
                                            combinecssstatus = true;
                                            this.backgroundWorker1.ReportProgress(-1, string.Format("true/Combined CSS Files Successfully"));
                                            logfile("Combined  minifiction css files End : " + DateTime.Now.ToString(), false);
                                        }
                                        catch (Exception ex)
                                        {
                                            this.backgroundWorker1.ReportProgress(-1, string.Format("false/Error on Combine CSS Files"));
                                            combinecssstatus = false;
                                            logfile(ex.ToString(), true);
                                        }
                                        i123++;
                                    }
                                    else
                                    {
                                        combinecssstatus = true;
                                    }
                                    if (combinecssstatus)
                                    {
                                        if (checkbackupfolder)
                                        {
                                            string Sourcedir = string.Empty;
                                            string SourceIsitNeworkDir = string.Empty;
                                            bool sistrue = false;
                                            if (SourceIsitNeworkDir == "0")
                                            {
                                                sistrue = true;
                                            }
                                            string CopYdir = string.Empty;
                                            string DestiIsitNeworkDir = string.Empty;
                                            bool distrue = false;
                                            if (DestiIsitNeworkDir == "0")
                                            {
                                                distrue = true;
                                            }
                                            string SourceDomain = string.Empty;
                                            string SourceUser = string.Empty;
                                            string Sourcepass = string.Empty;
                                            string DestiDomain = string.Empty;
                                            string DestiUser = string.Empty;
                                            string Destipass = string.Empty;

                                            Sourcedir = hhh1.GetString("BackupFolder", "SourceDir", "");
                                            SourceIsitNeworkDir = hhh1.GetString("BackupFolder", "DestiIsitneworkdirDesti", "");
                                            sistrue = false;
                                            if (SourceIsitNeworkDir == "0")
                                            {
                                                sistrue = true;
                                            }
                                            CopYdir = hhh1.GetString("BackupFolder", "CopyToDir", "");
                                            DestiIsitNeworkDir = hhh1.GetString("BackupFolder", "DestiIsitneworkdirDesti", "");
                                            distrue = false;
                                            if (DestiIsitNeworkDir == "0")
                                            {
                                                distrue = true;
                                            }
                                            SourceDomain = hhh1.GetString("BackupFolder", "SourceDomain", "");
                                            SourceUser = hhh1.GetString("BackupFolder", "SourceUsername", "");
                                            Sourcepass = hhh1.GetString("BackupFolder", "SourcePassword", "");
                                            DestiDomain = hhh1.GetString("BackupFolder", "DestiDomain", "");
                                            DestiUser = hhh1.GetString("BackupFolder", "DestiUsername", "");
                                            Destipass = hhh1.GetString("BackupFolder", "DestiPassword", "");

                                            if (BackupFolder(Sourcedir, sistrue, SourceDomain, SourceUser, Sourcepass, CopYdir, distrue, DestiDomain, DestiUser, Destipass) || !Backup.Checked)
                                            {
                                                BackupFolderstatus = true;
                                            }
                                            else
                                            {
                                                BackupFolderstatus = false;
                                            }
                                        }
                                        else
                                        {
                                            BackupFolderstatus = true;
                                        }
                                        if(BackupFolderstatus)
                                        {
                                            if(checkdeletefolder)
                                            {
                                                string dsourcefolder = hhh1.GetString("DeployemtFolder", "CopyToDir", "");
                                                string dsevername = hhh1.GetString("DeployemtFolder", "CopyToDir", "");
                                                string ddomain = hhh1.GetString("DeployemtFolder", "CopyToDir", "");
                                                string dusername = hhh1.GetString("DeployemtFolder", "CopyToDir", "");
                                                string dpass = hhh1.GetString("DeployemtFolder", "CopyToDir", "");
                                                string apppool = hhh1.GetString("DeployemtFolder", "CopyToDir", "");
                                                bool dsistrue = false;
                                                string dSourceIsitNeworkDir = hhh1.GetString("DeployemtFolder", "CopyToDir", "");
                                                if (dSourceIsitNeworkDir == "0")
                                                {
                                                    dsistrue = true;
                                                }
                                                if (axiis.CheckAPPoolExists(apppool, dsistrue, dsevername, ddomain + "\\" + dusername, dpass))
                                                {
                                                   
                                                    //if(axiis.StopAppool(apppool, dsistrue, dsevername, ddomain + "\\" + dusername, dpass))
                                                    //{
                                                    //    string sfile = Path.Combine(Application.StartupPath, "Tools\\VBCompiledComponentsUnReg.bat");
                                                    //    if(Axcopy.CopyFIle(sfile,false,"","","","",Path.Combine(dsourcefolder,"bin\\VBCompiledComponents\\VBCompiledComponentsUnReg.bat"),dsistrue,dsevername,ddomain,dusername,dpass))
                                                    //    {
                                                    //        if (axremotly.invocebatremotly(Path.Combine(dsourcefolder, "bin\\VBCompiledComponents\\VBCompiledComponentsUnReg.bat"), dsevername, ddomain + "\\" + dusername, dpass)) 
                                                    //        {
                                                    //            if(Axcopy.DeleteFoder(dsourcefolder,dsistrue,dsevername,ddomain,dusername,dpass))
                                                    //            {
                                                    //                Deleteexstingfolderstatus = true;
                                                    //            }
                                                    //        }
                                                    //    }
                                                    //}
                                                }
                                               
                                            }
                                            else
                                            {
                                                Deleteexstingfolderstatus = false;
                                            }
                                        }
                                        else
                                        {
                                            Deleteexstingfolderstatus = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(checkPSP)
            {

            }
            if(CheckCSP)
            {

            }
        }
        protected PictureBox pixc;
        protected Label labelInput;
        protected Label labelInput1;
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is String)
            {
                string str = (String)e.UserState;

                if (str.StartsWith("start"))
                {
                    var list = Enumerable.Range(0, str.Length / 1).Select(i => str.Substring(i * 1, 1)).ToList();
                    pixc = new PictureBox();
                    labelInput = new Label();
                    labelInput1 = new Label();
                    pixc.Location = new Point(10, label9.Bottom + (Convert.ToInt32(list[list.Count - 1]) * 52));
                    // pixc.Text = drs[1].ToString(); 300, labelInput.Top - 3
                    pixc.Name = "PIXC" + (String)e.UserState;
                    pixc.Width = 50;
                    pixc.Height = 50;
                    this.pixc.Image = Properties.Resources.Animation;
                    labelInput.Location = new Point(65, pixc.Top + 10);
                    labelInput.AutoSize = true;
                    labelInput.Name = "lbl" + (String)e.UserState;
                    labelInput.Font = new Font("Toma", label10.Font.Size);
                    labelInput1.Location = new Point(65, labelInput.Height + pixc.Top + 10);
                    labelInput1.AutoSize = true;
                    labelInput1.Name = "lbltime" + (String)e.UserState;
                    labelInput1.Font = new Font("Toma", label10.Font.Size);
                    tabControl1.TabPages[2].Controls.Add(pixc);
                    tabControl1.TabPages[2].Controls.Add(labelInput);
                    tabControl1.TabPages[2].Controls.Add(labelInput1);

                }
                else
                {
                    labelInput.Text = (String)e.UserState;

                    if (str.StartsWith("true"))
                    {
                        string[] split = str.Split(new char[] { '/' });
                        labelInput.Text = split[1];
                        pixc.Image = null;
                        pixc.Image = Properties.Resources._1407925701_notification_done;


                    }
                    else if (str.StartsWith("false"))
                    {
                        string[] split = str.Split(new char[] { '/' });
                        labelInput.Text = split[1];
                        pixc.Image = null;
                        pixc.Image = Properties.Resources._1407925682_notification_error;

                    }

                }
                //this.label57.Text = (String)e.UserState;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void ChkWebModule_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkWebModule.Checked)
            {


                for (int i = 0; i < checkLBWEB.Items.Count; i++)
                {
                    checkLBWEB.SetSelected(i, true);
                }
            }
            else
            {
                for (int i = 0; i < checkLBWEB.Items.Count; i++)
                {
                    checkLBWEB.SetSelected(i, false);
                }
            }
        }

        private void WebReleasecheck_CheckedChanged(object sender, EventArgs e)
        {

        }
        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset, string Sourcedir, string SourceUser, string SourceDomain, string Sourcepass, string CopYdir, string DestiUser, string DestiDomain, string Destipass, bool sorcenet, bool destinet)
        {
            try
            {

                if (sorcenet && destinet)
                {
                    using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                    {

                        if (unc.NetUseWithCredentials(Sourcedir, SourceUser, SourceDomain, Sourcepass))
                        {

                            using (UNCAccessWithCredentials unc1 = new UNCAccessWithCredentials())
                            {

                                if (unc1.NetUseWithCredentials(CopYdir, DestiUser, DestiDomain, Destipass))
                                {
                                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                                    foreach (string filename in files)
                                    {

                                        FileInfo fi = new FileInfo(filename);
                                        fi.IsReadOnly = false;
                                        string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                                        entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                                        ZipEntry newEntry = new ZipEntry(entryName);
                                        newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                                        // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                                        // A password on the ZipOutputStream is required if using AES.
                                        //   newEntry.AESKeySize = 256;

                                        // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                                        // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                                        // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                                        // but the zip will be in Zip64 format which not all utilities can understand.
                                        //   zipStream.UseZip64 = UseZip64.Off;
                                        newEntry.Size = fi.Length;

                                        zipStream.PutNextEntry(newEntry);

                                        // Zip the file in buffered chunks
                                        // the "using" will close the stream even if an exception occurs
                                        byte[] buffer = new byte[4096];
                                        using (FileStream streamReader = File.OpenRead(filename))
                                        {
                                            StreamUtils.Copy(streamReader, zipStream, buffer);
                                        }
                                        zipStream.CloseEntry();
                                    }
                                    //string[] folders = Directory.GetDirectories(path);
                                    //foreach (string folder in folders)
                                    //{
                                    //    CompressFolder(folder, zipStream, folderOffset, Sourcedir, SourceUser, SourceDomain, Sourcepass, CopYdir, DestiUser, DestiDomain, Destipass);
                                    //}
                                }
                            }
                        }
                    }
                }
                else if (!sorcenet && !destinet)
                {
                    string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                    foreach (string filename in files)
                    {

                        FileInfo fi = new FileInfo(filename);
                        fi.IsReadOnly = false;
                        string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                        entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                        ZipEntry newEntry = new ZipEntry(entryName);
                        newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity


                        newEntry.Size = fi.Length;

                        zipStream.PutNextEntry(newEntry);

                        byte[] buffer = new byte[4096];
                        using (FileStream streamReader = File.OpenRead(filename))
                        {
                            StreamUtils.Copy(streamReader, zipStream, buffer);
                        }
                        zipStream.CloseEntry();
                    }
                }
                else if (sorcenet && !destinet)
                {
                    using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                    {

                        if (unc.NetUseWithCredentials(Sourcedir, SourceUser, SourceDomain, Sourcepass))
                        {
                            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                            foreach (string filename in files)
                            {

                                FileInfo fi = new FileInfo(filename);
                                fi.IsReadOnly = false;
                                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                                ZipEntry newEntry = new ZipEntry(entryName);
                                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity


                                newEntry.Size = fi.Length;

                                zipStream.PutNextEntry(newEntry);

                                byte[] buffer = new byte[4096];
                                using (FileStream streamReader = File.OpenRead(filename))
                                {
                                    StreamUtils.Copy(streamReader, zipStream, buffer);
                                }
                                zipStream.CloseEntry();
                            }
                        }
                    }
                }
                else if (!sorcenet && destinet)
                {
                    using (UNCAccessWithCredentials unc1 = new UNCAccessWithCredentials())
                    {

                        if (unc1.NetUseWithCredentials(CopYdir, DestiUser, DestiDomain, Destipass))
                        {
                            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

                            foreach (string filename in files)
                            {

                                FileInfo fi = new FileInfo(filename);
                                fi.IsReadOnly = false;
                                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                                ZipEntry newEntry = new ZipEntry(entryName);
                                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity


                                newEntry.Size = fi.Length;

                                zipStream.PutNextEntry(newEntry);

                                byte[] buffer = new byte[4096];
                                using (FileStream streamReader = File.OpenRead(filename))
                                {
                                    StreamUtils.Copy(streamReader, zipStream, buffer);
                                }
                                zipStream.CloseEntry();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logfile(ex.ToString(), true);
            }
        }

        private bool BackupFolder(string sourceapth, bool isitnetworkpath, string sourcedomain, string sourceusername, string sourcepassword, string DestinationDir, bool isitneworkdeti, string destidomain, string destiusername, string destipass)
        {
            bool istrue = false;
            if (!string.IsNullOrEmpty(sourceapth))
            {
                logfile("Backup Server Folder begin : " + DateTime.Now.ToString(), false);
                string Sourcedir = sourceapth;
                bool SourceIsitNeworkDir = isitnetworkpath;
                string CopYdir = DestinationDir;
                bool DestiIsitNeworkDir = isitneworkdeti;
                string SourceDomain = sourcedomain;
                string SourceUser = sourceusername;
                string Sourcepass = sourcepassword;
                string DestiDomain = destidomain;
                string DestiUser = destiusername;
                string Destipass = destipass;

                try
                {
                    if (SourceIsitNeworkDir && DestiIsitNeworkDir)
                    {
                        using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                        {

                            if (unc.NetUseWithCredentials(Sourcedir, SourceUser, SourceDomain, Sourcepass))
                            {

                                using (UNCAccessWithCredentials unc1 = new UNCAccessWithCredentials())
                                {

                                    if (unc1.NetUseWithCredentials(CopYdir, DestiUser, DestiDomain, Destipass))
                                    {
                                        string copy1 = CopYdir;
                                        string filename = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".zip";
                                        CopYdir = Path.Combine(CopYdir, filename);
                                        FileStream fsOut = File.Create(CopYdir);
                                        ZipOutputStream zipStream = new ZipOutputStream(fsOut);
                                        string folderName = Sourcedir;
                                        zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                                        //zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

                                        // This setting will strip the leading part of the folder path in the entries, to
                                        // make the entries relative to the starting folder.
                                        // To include the full path for each entry up to the drive root, assign folderOffset = 0.
                                        int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                                        CompressFolder(folderName, zipStream, folderOffset, Sourcedir, SourceUser, SourceDomain, Sourcepass, copy1, DestiUser, DestiDomain, Destipass, SourceIsitNeworkDir, DestiIsitNeworkDir);

                                        zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                                        zipStream.Close();

                                        //logfile("Deleting source Folder strat : " + DateTime.Now.ToString(), false);
                                        //if (Directory.Exists(Sourcedir))
                                        //    Directory.Delete(Sourcedir, true);
                                        //if (!Directory.Exists(Sourcedir))
                                        //    Directory.CreateDirectory(Sourcedir);
                                        // logfile("Deleting source Folder End : " + DateTime.Now.ToString(), false);
                                    }
                                }
                            }
                        }
                    }
                    else if (!SourceIsitNeworkDir && !DestiIsitNeworkDir)
                    {
                        string copy1 = CopYdir;
                        string filename = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".zip";
                        CopYdir = Path.Combine(CopYdir, filename);
                        FileStream fsOut = File.Create(CopYdir);
                        ZipOutputStream zipStream = new ZipOutputStream(fsOut);
                        string folderName = Sourcedir;
                        zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                        //zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

                        // This setting will strip the leading part of the folder path in the entries, to
                        // make the entries relative to the starting folder.
                        // To include the full path for each entry up to the drive root, assign folderOffset = 0.
                        int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                        CompressFolder(folderName, zipStream, folderOffset, Sourcedir, SourceUser, SourceDomain, Sourcepass, copy1, DestiUser, DestiDomain, Destipass, SourceIsitNeworkDir, DestiIsitNeworkDir);

                        zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                        zipStream.Close();

                    }
                    else if (!SourceIsitNeworkDir && DestiIsitNeworkDir)
                    {
                        using (UNCAccessWithCredentials unc1 = new UNCAccessWithCredentials())
                        {

                            if (unc1.NetUseWithCredentials(CopYdir, DestiUser, DestiDomain, Destipass))
                            {
                                string copy1 = CopYdir;
                                string filename = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".zip";
                                CopYdir = Path.Combine(CopYdir, filename);
                                FileStream fsOut = File.Create(CopYdir);
                                ZipOutputStream zipStream = new ZipOutputStream(fsOut);
                                string folderName = Sourcedir;
                                zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                                //zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

                                // This setting will strip the leading part of the folder path in the entries, to
                                // make the entries relative to the starting folder.
                                // To include the full path for each entry up to the drive root, assign folderOffset = 0.
                                int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                                CompressFolder(folderName, zipStream, folderOffset, Sourcedir, SourceUser, SourceDomain, Sourcepass, copy1, DestiUser, DestiDomain, Destipass, SourceIsitNeworkDir, DestiIsitNeworkDir);

                                zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                                zipStream.Close();

                                //logfile("Deleting source Folder strat : " + DateTime.Now.ToString(), false);
                                //if (Directory.Exists(Sourcedir))
                                //    Directory.Delete(Sourcedir, true);
                                //if (!Directory.Exists(Sourcedir))
                                //    Directory.CreateDirectory(Sourcedir);
                                // logfile("Deleting source Folder End : " + DateTime.Now.ToString(), false);
                            }
                        }
                    }
                    else if (SourceIsitNeworkDir && !DestiIsitNeworkDir)
                    {
                        using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                        {

                            if (unc.NetUseWithCredentials(Sourcedir, SourceUser, SourceDomain, Sourcepass))
                            {


                                string copy1 = CopYdir;
                                string filename = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".zip";
                                CopYdir = Path.Combine(CopYdir, filename);
                                FileStream fsOut = File.Create(CopYdir);
                                ZipOutputStream zipStream = new ZipOutputStream(fsOut);
                                string folderName = Sourcedir;
                                zipStream.SetLevel(3); //0-9, 9 being the highest level of compression

                                //zipStream.Password = password;  // optional. Null is the same as not setting. Required if using AES.

                                // This setting will strip the leading part of the folder path in the entries, to
                                // make the entries relative to the starting folder.
                                // To include the full path for each entry up to the drive root, assign folderOffset = 0.
                                int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                                CompressFolder(folderName, zipStream, folderOffset, Sourcedir, SourceUser, SourceDomain, Sourcepass, copy1, DestiUser, DestiDomain, Destipass, SourceIsitNeworkDir, DestiIsitNeworkDir);

                                zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                                zipStream.Close();

                                //logfile("Deleting source Folder strat : " + DateTime.Now.ToString(), false);
                                //if (Directory.Exists(Sourcedir))
                                //    Directory.Delete(Sourcedir, true);
                                //if (!Directory.Exists(Sourcedir))
                                //    Directory.CreateDirectory(Sourcedir);
                                // logfile("Deleting source Folder End : " + DateTime.Now.ToString(), false);

                            }
                        }
                    }

                    logfile("Backup Server Folder End : " + DateTime.Now.ToString(), false);

                    istrue = true;
                }
                catch (Exception ex)
                {
                    logfile(ex.ToString(), true);
                }
            }
            return istrue;

        }
       
    }
}
