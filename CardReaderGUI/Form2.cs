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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CardReaderGUI
{
    public partial class Form2 : System.Windows.Forms.Form
    {
        private CardReader cardReader = new CardReader();
        private string currentDirectoryPath;
        private string username;
        private string password;
        private string reader = "";
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
                if (reader.Equals(""))
                {
                    statusLabel.Text = "Bitte einen Kartenleser auswählen";
                }
                else
                {
                    statusLabel.Text = "Einstellungen Bestätigen";
                }
                
            }

        }

        /// <summary>
        /// Event handler for changing the status text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void cardReader_driverStatusTextChanged(object sender, StatusChangedEventArgs e)
        {
            //changes to the UI can only be made no the UI thread .Invoke runs the delegate on the UI thread
            statusLabel.Invoke((MethodInvoker)delegate
            {
                statusLabel.Text = e.statusText;
            });
        }

        public void cardReader_nfcStatusTextChanged(object sender, StatusChangedEventArgs e)
        {
            //changes to the UI can only be made no the UI thread .Invoke runs the delegate on the UI thread
            statusLabel.Invoke((MethodInvoker)delegate
            {
                nfcStatusLabel.Text = e.statusText;
            });
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            fillComboBoxes();
            APDUSender.getInstance().readerListChanged += updateComboBoxes;
            cardReader.EventHandler += cardReader_driverStatusTextChanged;
            cardReader.NfcStatusChangedHandler += cardReader_nfcStatusTextChanged;
        }
        /// <summary>
        /// When upload button is clicked send a post requests to the api containing file content of chosen file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uploadButton_Click(object sender, EventArgs e)
        {
            string[] dddFiles;
            //todo scan the directory for any ddd files and send a post request to api
            try
            {
                dddFiles = Directory.GetFiles(currentDirectoryPath, "*.ddd");
            } catch(Exception ex)
            {
                statusLabel.Text = "Keine Dateien für Upload vorhanden";
                return;
            }
            
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

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label1_Click_2(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            cardReader.InitNFC(statusLabel, comboBoxNFC.Text, username, password);
            nfcStatusLabel.Text = "Ready to Read tags";
        }

        private void nfcStatusLabel_Click(object sender, EventArgs e)
        {

        }

        private void okButtonFahrerkarten_Click(object sender, EventArgs e)
        {
            Debug.WriteLine(currentDirectoryPath);
            if(currentDirectoryPath != null) {
                cardReader.InitDriver(currentDirectoryPath, statusLabel, comboBoxFahrer.Text);
                statusLabel.Text = "Bereit Karte einzulesen!";
            }
            else
            {
                statusLabel.Text = "Bitte einen Ordner auswählen!";
            }
            
            
        }

        private void comboBoxFahrer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void fillComboBoxes()
        {
            this.comboBoxFahrer.DataSource = APDUSender.getInstance().cardNativeDriver.ListReaders();
            this.comboBoxNFC.DataSource = APDUSender.getInstance().cardNativeNFC.ListReaders();
            
        }

        private void comboBoxNFC_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void updateComboBoxes(object sender, EventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                comboBoxFahrer.DataSource = APDUSender.getInstance().Readers;
                comboBoxNFC.DataSource = APDUSender.getInstance().Readers;
            });
                
         
            
        }

   
    }
}
