using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DriverCardReader;
using static System.Net.WebRequestMethods;

namespace CardReaderGUI
{
    public partial class Form2 : System.Windows.Forms.Form
    {
        private string currentDirectoryPath;
        private string username;
        private string password;    
        public Form1 form;
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string username, string pwAsMD5)
        {
            this.username = username;
            this.password = pwAsMD5;
            InitializeComponent();
        }

        private void chosenDirectoryText_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                directoryPathText.Text = dialog.SelectedPath;
                currentDirectoryPath = dialog.SelectedPath;
                CardReader cardReader = new CardReader();
                cardReader.Init(dialog.SelectedPath, statusLabel);
                cardReader.EventHandler += cardReader_statusTextChanged;
                statusLabel.Text = "Insert a Card";
            }

        }

        /// <summary>
        /// Event handler for changing the status text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void cardReader_statusTextChanged(object sender, StatusChangedEventArgs e)
        {
            //changes to the UI can only be made no the UI thread .Invoke runs the delegate on the UI thread
            statusLabel.Invoke((MethodInvoker)delegate
            {
                statusLabel.Text = e.statusText;
            });
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// When upload button is clicked send a post requests to the api containing file content of chosen file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadButton_Click(object sender, EventArgs e)
        {
            //todo scan the directory for any ddd files and send a post request to api
            string[] dddFiles = Directory.GetFiles(currentDirectoryPath, "*.ddd");
            Debug.WriteLine("Files in directory: " + dddFiles);
            bool failed = false;
            foreach (string dddFile in dddFiles)
            {
                
                string driverName = Path.GetFileNameWithoutExtension(dddFile);
                byte[] filecontent = System.IO.File.ReadAllBytes(dddFile);
                string userName = this.username;
                string password = this.password;
                Stream writeBodyStream = null;

                if (failed)
                {
                    continue;
                }

                //prepare post request
                string apiURL = "https://tstapi.gpsoverip.de/lurz/remotefiles?user="+ userName+ "&password="+ password + "&driverCardID=" + driverName;
                
                WebRequest request = WebRequest.Create(apiURL);
                request.Method = "POST";
                request.ContentType = "application/octet-stream";
                request.ContentLength = filecontent.Length;
                try
                {
                    writeBodyStream = request.GetRequestStream();

                } catch(Exception ex)
                {
                    statusLabel.Text = "error has occured " + ex.Message;
                    return;
                }
                
                writeBodyStream.Write(filecontent, 0, filecontent.Length);
           
                HttpWebResponse response = null;
                    
               
                try { 
                       response = (HttpWebResponse)request.GetResponse(); 
                     }
                catch(Exception ex) {
                    statusLabel.Text = "an Error has Ocuured: " + ex.Message;
                    return;
                }
                                
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    statusLabel.Text = "File: " + Path.GetFileNameWithoutExtension(dddFile) + " uploaded succesfully";

                    //move file to archive/success
                }
                else
                {
                    statusLabel.Text = "Error uploading File " + Path.GetFileNameWithoutExtension(dddFile) + " please try again";
                    break;

                    //move file to archive/error
                };
                
                Debug.WriteLine(dddFile);
            }
        }
    }
}
