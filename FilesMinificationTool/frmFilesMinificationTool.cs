using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace FilesMinificationTool
{
    public partial class frmFilesMinificationTool : Form
    {
        public frmFilesMinificationTool()
        {
            InitializeComponent();
        }
        protected string logfilepath = System.Environment.CurrentDirectory + "\\FilesMinificationTool" + DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + "_log.log";

        string ToolTitle = "Files Minification Tool";

        private void logfile(string logtext, bool error, string logfilepath)
        {
            try
            {
                string er = "";
                StreamWriter swErrorLog = default(StreamWriter);
                if (!File.Exists(logfilepath))
                {
                    using (StreamWriter sw = File.CreateText(logfilepath))
                    {
                    }
                    swErrorLog = File.AppendText(logfilepath);                 
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
                swErrorLog.WriteLine((Convert.ToString(":") + er) + ":");
                swErrorLog.WriteLine("  : {0}", logtext);
                swErrorLog.Flush();
                swErrorLog.Close();
            }
            catch (Exception ex)
            {                
                MessageBox.Show(ex.Message, ToolTitle);
            }
        }
      
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                string projpath;

                if(rb4MinifyAll4DiffDesti.Checked ==true || rb4MinifyAll4SameDesti.Checked ==true || rb4MinifyAllCSS4DiffDesti.Checked ==true ||rb4MinifyAllCSS4SameDesti.Checked ==true || rb4MinifyAllJS4DiffDesti.Checked ==true || rb4MinifyAllJS4SameDesti.Checked ==true || rb4MinifySingleCSSFile4DiffDesti.Checked == true || rb4MinifySingleCSSFile4SameDesti.Checked ==true || rb4MinifySingleJSFile4DiffDesti.Checked ==true || rb4MinifySingleJSFile4SameDesti.Checked ==true)
                {                    
                    if (!string.IsNullOrEmpty(txt4FolderPath.Text.ToString()))
                    {
                        string finalpath = txt4FolderPath.Text.ToString();
                        string sourceFile = "";
                        string finalpath1 = txt4FolderPath.Text.ToString();
                        string sourceFile1 = "";
                        //MinifySingleJSFile_SameDesti.xml
                        //MinifySingleCSSFile_SameDesti.xml
                        //MinifySingleJSFile_DiffDesti.xml
                        //MinifySingleCSSFile_DiffDesti.xml
                        //MinifyAllJS_SameDesti.xml
                        //MinifyAllJS_DiffDesti.xml
                        //MinifyAllCSS_SameDesti.xml
                        //MinifyAllCSS_DiffDesti.xml
                        //MinifyAllFiles_SameDesti.xml
                        //MinifyAllFiles_DiffDesti.xml
                        //SmallSharpTools.Packer.dll

                        if (rb4MinifyAll4SameDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifyAllFiles_SameDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifyAllFiles_SameDesti.xml";
                        }
                        else if (rb4MinifyAll4DiffDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifyAllFiles_DiffDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifyAllFiles_DiffDesti.xml";
                        }
                        else if (rb4MinifyAllCSS4DiffDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifyAllCSS_DiffDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifyAllCSS_DiffDesti.xml";
                        }
                        else if (rb4MinifyAllCSS4SameDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifyAllCSS_SameDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifyAllCSS_SameDesti.xml";
                        }
                        else if (rb4MinifyAllJS4DiffDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifyAllJS_DiffDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifyAllJS_DiffDesti.xml";
                        }
                        else if (rb4MinifyAllJS4SameDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifyAllJS_SameDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifyAllJS_SameDesti.xml";
                        }
                        else if (rb4MinifySingleCSSFile4DiffDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifySingleCSSFile_DiffDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifySingleCSSFile_DiffDesti.xml";
                        }
                        else if (rb4MinifySingleCSSFile4SameDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifySingleCSSFile_SameDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifySingleCSSFile_SameDesti.xml";
                        }
                        else if (rb4MinifySingleJSFile4DiffDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifySingleJSFile_DiffDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifySingleJSFile_DiffDesti.xml";
                        }
                        else if (rb4MinifySingleJSFile4SameDesti.Checked == true)
                        {
                            finalpath = txt4FolderPath.Text.ToString() + "\\MinifySingleJSFile_SameDesti.xml";
                            sourceFile = Application.StartupPath + "\\MinifySingleJSFile_SameDesti.xml";
                        }
                        finalpath1 = txt4FolderPath.Text.ToString() + "\\SmallSharpTools.Packer.dll";
                        sourceFile1 = Application.StartupPath + "\\SmallSharpTools.Packer.dll";

                        if (!File.Exists(sourceFile.ToString()))
                        {
                            MessageBox.Show("Source File Not Exit,Please Check Once.",ToolTitle);
                        }
                        else
                        {
                            System.IO.File.Copy(sourceFile, finalpath, true);
                        }
                        if (!File.Exists(sourceFile1.ToString()))
                        {
                            MessageBox.Show("Source File Not Exit,Please Check Once.", ToolTitle);
                        }
                        else
                        {
                            System.IO.File.Copy(sourceFile1, finalpath1, true);
                        }
                        projpath = finalpath;

                        StartProcess(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe", '"' + projpath + '"' + " /flp:logfile=buildlog.txt;errorsonly", projpath, false, logfilepath);

                        FileInfo buildlogFinfo = new FileInfo(Path.Combine(Application.StartupPath, "buildlog.txt"));

                        if (buildlogFinfo.Exists)
                        {
                            if (buildlogFinfo.Length == 0)
                            {
                                MessageBox.Show("Minified Files Successfully.", ToolTitle);
                            }
                            else
                            {
                                MessageBox.Show("Error Occured During Minified Files.", ToolTitle);
                                logfile(File.ReadAllText(Path.Combine(Application.StartupPath, "buildlog.txt")), true, logfilepath);
                            }
                        }
                        else
                        {
                            logfile("Compilation End time : " + DateTime.Now.ToString(), false, logfilepath);
                            MessageBox.Show("Ending Minification Of  Filess....", ToolTitle);
                        }

                        System.IO.File.Delete(finalpath);
                        System.IO.File.Delete(finalpath1);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(txt4FolderPath.Text.ToString()))
                        {
                            MessageBox.Show("Please Enter Source File Path.", ToolTitle);
                        }
                    }
                }
                else
                {                    
                    MessageBox.Show("Please Select Atleast One Item From Minification Files Details.",ToolTitle);
                }                
            }
            catch (Exception ex)
            {
                logfile(ex.ToString(), true, logfilepath);
                //MessageBox.Show(ex.Message, ToolTitle);
            }
        }
    
        public Process StartProcess(string cmd, string args, string filename, bool createnowindow, string logfilepath)
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
                logfile(filename + " Not Minified And Error is : " + ex.ToString(), true, logfilepath);
            }
            return p;
        }

        private void frmFilesMinificationTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do You Wish To Close The Application?", ToolTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                logfile(ex.ToString(), true, logfilepath);
                //MessageBox.Show(ex.Message, ToolTitle.ToString());
            }
        }

        private void frmFilesMinificationTool_Load(object sender, EventArgs e)
        {            
        }
    }
}
