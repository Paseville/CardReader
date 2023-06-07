using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace DriverCardReader
{
    internal class ApiRequest
    {


        public static string makePostRequest(APIRequestSchema schema)
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(schema);
            Debug.WriteLine(jsonString);
            string apiURL = "http://localhost:2020/driver/authenticate";
            HttpWebResponse response= null;
            WebRequest request = WebRequest.Create(apiURL);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = jsonString.Length;
            try
            {
               Stream writeBodystream = request.GetRequestStream();
                byte[] jsonBytesTosend = Encoding.UTF8.GetBytes(jsonString);
                writeBodystream.Write(jsonBytesTosend , 0, jsonString.Length);
                response= (HttpWebResponse)request.GetResponse();

            } catch (Exception e) { 
               Debug.WriteLine("Something went wrong with the request." + e.Message);            
            }

            return "0";
        }

        public static APIResponseSchema makeGetRequest(string user, string password, string nfcTag)
        {
            string apiURL = "http://localhost:2020/driver/lastStatus?mac=" + nfcTag +"&user=" + user +"&password="+password ;
           
            WebRequest request = WebRequest.Create(apiURL);
            request.Method = "GET";

          
           HttpWebResponse response = (HttpWebResponse)request.GetResponse();
         
          
            string t;
            
                StreamReader reader = new StreamReader(response.GetResponseStream());
                t = reader.ReadToEnd();


            return JsonConvert.DeserializeObject<APIResponseSchema>(t);
            





        }

        public static void makeUpdateRequest(string user, string password, string nfcTag, string timestamp, string status)
        {
            string apiURL = "http://localhost:2020/driver/updateStatus?mac=" + nfcTag + "&user=" + user + "&password=" + password + "&timestamp=" + timestamp + "&status=" + status;
            Debug.Write("updated Tag: " + nfcTag + " Timestamp: " + timestamp);
            HttpWebResponse response = null;
            WebRequest request = WebRequest.Create(apiURL);
            request.Method = "GET";


            response = (HttpWebResponse)request.GetResponse();
            

        }
    }
}
