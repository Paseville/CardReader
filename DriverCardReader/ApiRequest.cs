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

        public const string apiURL = "http://localhost:2020/driver/";


        public static bool makePostRequest(APIRequestSchema schema)
        {
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(schema);
            Debug.WriteLine(jsonString);
            string apiURL = ApiRequest.apiURL + "authenticate";
            HttpWebResponse response= null;
            WebRequest request = WebRequest.Create(apiURL);
            request.Method = "POST";
            request.Timeout = 1000;
            request.ContentType = "application/json";
            request.ContentLength = jsonString.Length;
            try
            {
               Stream writeBodystream = request.GetRequestStream();
                byte[] jsonBytesTosend = Encoding.UTF8.GetBytes(jsonString);
                writeBodystream.Write(jsonBytesTosend , 0, jsonString.Length);
                if(response != null)
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                else
                {
                    throw new WebException("Api not reachable");
                    
                }
                

            } catch (Exception e) {
                throw new WebException("Api fehler");
            }

            return true;
        }

        public static APIResponseSchema makeGetRequest(string user, string password, string nfcTag)
        {
            string apiURL = ApiRequest.apiURL +  "lastStatus?mac=" + nfcTag +"&user=" + user +"&password="+password ;
           
            WebRequest request = WebRequest.Create(apiURL);
            request.Method = "GET";
            request.Timeout = 1000
;
          
           HttpWebResponse response = (HttpWebResponse)request.GetResponse();
         
          
            string t;
            
                StreamReader reader = new StreamReader(response.GetResponseStream());
                t = reader.ReadToEnd();


            return JsonConvert.DeserializeObject<APIResponseSchema>(t);
            





        }

        public static void makeUpdateRequest(string user, string password, string nfcTag, string timestamp, string status)
        {
            string apiURL = ApiRequest.apiURL + "updateStatus?mac=" + nfcTag + "&user=" + user + "&password=" + password + "&timestamp=" + timestamp + "&status=" + status;
            Debug.Write("updated Tag: " + nfcTag + " Timestamp: " + timestamp);
            HttpWebResponse response = null;
            WebRequest request = WebRequest.Create(apiURL);
            request.Method = "GET";
            request.Timeout = 1000;



            response = (HttpWebResponse)request.GetResponse();
            
            if(response == null)
            {
                throw new WebException("API nicht erreichbar");
            }

        }
    }
}
