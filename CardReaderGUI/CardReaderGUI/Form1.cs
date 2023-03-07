using DriverCardReader;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardReaderGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string apiURL = "https://www.apioverip.de?module=devices&action=list&nozlib=1&format=json&user=" + usernameBox.Text +"&pwd="+ CreateMD5Hash(passwordBox.Text);
            Debug.WriteLine("pwHash: " + CreateMD5Hash(passwordBox.Text));
            HttpWebRequest request = HttpWebRequest.CreateHttp(apiURL);
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            //response is only in json-format if the login failed the succesful response isnt json;
            //check if response is json and if yes see if tell user why it failed if success is false
            if (response.Headers.GetValues("Content-Type")[0].Equals("application/json; charset=utf-8")){
                JObject joResponse = JObject.Parse(APDUSender.StreamToString(response.GetResponseStream()));
                if (joResponse.ContainsKey("success")){
                    if (joResponse.GetValue("success").ToString().ToLower().Equals("false")) { 
                        statusLogin.Text = "Login failed. Reason: " + joResponse.GetValue("reason").ToString();
                    }
                }
                else
                {
                    statusLogin.Text = "Login failed. Http Status: " + response.StatusCode.ToString();
                }
                statusLogin.Visible = true;
            //Problem with API if code is not 200 
            }else if(response.StatusCode != HttpStatusCode.OK) {
                statusLogin.Text = "API request failed. Http Status Code: " + response.StatusCode.ToString();
            }
            //if response is received login user (create form two with username and pwd)
            else if(response.Headers.GetValues("Content-Type")[0].Equals("text/html"))
            {

                Form form2 = new Form2(usernameBox.Text, CreateMD5Hash(passwordBox.Text) );
                
                this.Visible = false;
                statusLogin.Text = "erfolg";
                statusLogin.Visible = true;
                form2.ShowDialog();
                
            //if sth weird goes wrong just show error
            } else
            {
                Debug.WriteLine(response.Headers.GetValues("Content-Type")[0]);
                statusLogin.Text = "error";
                statusLogin.Visible = true;
            }

        }

        public string CreateMD5Hash(string input)
        {
            // Step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            Debug.Write(sb.ToString());
            return sb.ToString();
        }

        private void usernameBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
