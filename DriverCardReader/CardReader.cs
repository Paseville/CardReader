using GemCard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.System;

namespace DriverCardReader
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);
    public delegate void NfcStatusChangedEventHandler(object sender, StatusChangedEventArgs e);


    /// <summary>
    /// Used mostly for ini method which starts the cardeventloop to listen for cardremoved and card inserted events
    /// </summary>
    public class CardReader
    {
        APIRequestSchema schema;
        private bool isInitalisedDriver = false;
        private bool isInitalisedNFC = false;
        private string[] readers;
        private APDUSender apduSender;
        SmartCard driverCard = null;
        private string filePath;
        private System.Windows.Forms.Label driverStatusLabel;
        private System.Windows.Forms.Label nfcStatusLabel;
        private string user;
        private string password;
        /// <summary>
        /// event handler when a the status from a card is changed (being read, done reading, etc)
        /// </summary>
        public event StatusChangedEventHandler EventHandler; 
        public event NfcStatusChangedEventHandler NfcStatusChangedHandler;
        /// <summary>
        /// Initalises the reader and defines the eventhandlers for SmartCard Inserted events
        /// </summary>
        /// 

        protected virtual void OnNfcStatusChange(string statusText)
        {
            NfcStatusChangedEventHandler handler = NfcStatusChangedHandler;
            handler.Invoke(this, new StatusChangedEventArgs(statusText));
        }
        protected virtual void OnStatusChanged(string statusText)
        {
            
            StatusChangedEventHandler handler = EventHandler;
            if(handler != null)
            {
                Debug.WriteLine("Called on statusChanged");
                //Invoke delegate 
                handler.Invoke(this, new StatusChangedEventArgs(statusText)); 
            }
        }


        public void InitDriver(string filePath, System.Windows.Forms.Label statusLabel, string reader)
        {
            
            this.filePath = filePath;
          
                this.driverStatusLabel = statusLabel;
                if (isInitalisedDriver == false)
                {
                    apduSender = APDUSender.getInstance();
                    apduSender.cardNativeDriver.OnCardInserted += CardNative_OnDriverCardInserted;
                    apduSender.cardNativeDriver.OnCardRemoved += CardNative_OnDriverCardRemoved;
                    isInitalisedDriver = true;
                    
                }
                apduSender.cardNativeDriver.StartCardEvents(reader);
            

            

            
            
        }



        public void InitNFC(System.Windows.Forms.Label statusLabel, string reader, string user, string password)
        {
            
                this.nfcStatusLabel = statusLabel;
                if (isInitalisedNFC == false)
                {
                    this.schema = new APIRequestSchema(user, password);
                    this.user = user;
                    this.password = password;
                     apduSender = APDUSender.getInstance();
                    apduSender.cardNativeNFC.OnCardInserted += CardNative_OnNFCCardInserted;
                    apduSender.cardNativeNFC.OnCardRemoved += CardNative_OnNFCCardRemoved;
                    isInitalisedNFC = true;
                }
                apduSender.cardNativeNFC.StartCardEvents(reader);

            
        }
        private void CardNative_OnNFCCardRemoved(string reader)
        {
            Debug.WriteLine("Nfc Chip wurde entfernt");
        }
        private void CardNative_OnNFCCardInserted(string reader)
        {
            APIResponseSchema response;
            byte[] uidBigEndian;
            try
            {
                CardNative card = APDUSender.getInstance().cardNativeNFC;
                card.Connect(reader, SHARE.Shared, PROTOCOL.T0orT1);
                uidBigEndian = card.getUID().Take(7).ToArray();

            }
            catch (Exception e)
            {
                
                    OnNfcStatusChange("NFC-Tag zu schnell entfernt!");
                
                return;
            }


            byte[] uidLittleEndian = new byte[7];

            if (uidBigEndian != null)
            {
                for (int i = 0; i < uidBigEndian.Length; i++)
                {
                    uidLittleEndian[6 - i] = uidBigEndian[i];
                }
                string nfcTag = BitConverter.ToString(uidLittleEndian).Replace("-", "").ToUpper();
                schema.mac = nfcTag;
                Debug.WriteLine(nfcTag);


                try
                {
                    response = ApiRequest.makeGetRequest(user, password, nfcTag);
                    if (response != null)
                    {
                        checkResponse(response);
                    }
                    else
                    {
                       
                            OnNfcStatusChange("Error Getting response");

                       
                        return;
                    }
                }


                catch (WebException e)
                {
                    if (e.Status != WebExceptionStatus.UnknownError)
                    {
                        HttpStatusCode wRespStatusCode;
                        try { 
                            wRespStatusCode = (((HttpWebResponse)e.Response).StatusCode); 
                        }
                        catch(Exception ex)
                        {
                            OnNfcStatusChange("API nicht erreichbar");
                            return;
                        }
                        
                        if (wRespStatusCode == HttpStatusCode.Unauthorized)
                        {
                           
                                OnNfcStatusChange("Tag: " + nfcTag + " nicht bei diesem Account registriert");

                            return;
                        }
                        else if (wRespStatusCode == HttpStatusCode.NotFound)
                        {
                            schema.status = "4";
                            schema.time = DateTime.Now;

                            handlePostRequest(schema);
                            return;
                        }
                    }
                    else
                    {
                       
                         OnNfcStatusChange("API nicht erreichbar");

                    }
                }
                catch (Exception e)
                {
                  
                        OnNfcStatusChange("Programmfehler");

                    return;
                }





            }
            else
            {
                Debug.WriteLine("Tag was removed to fast keep it there for at least 1 second");
            }
        }
        /// <summary>
        /// Look at API response and Update last status (from contactid (One Worker can have many nfctags registered) because of that use ) depending on passed time.
        /// </summary>
        /// <param name="response"></param>
        private void checkResponse(APIResponseSchema response)
        {



            //if the last status does not have a valid status set meaning it is not known which status it should have
            //send current status as workbeginn (the no status set value is 8)

            if (response.status == 99)
            {
                DateTime dLastTime = DateTime.Parse(response.timestamp);
                DateTime currentTime = DateTime.Now;

                TimeSpan timeDifference = dLastTime - currentTime;
                TimeSpan maximumDifference = TimeSpan.FromHours(8);
                //send current Timestamp as workbegin
                schema.time = currentTime;
                schema.status = "4";
                handlePostRequest(schema);

                //update last timestamp either as work_end or break
                //depending on time difference
                if (timeDifference > maximumDifference)
                {
                    schema.time = dLastTime;
                    schema.status = "6";
                    Debug.WriteLine("last Status updated to Work_END");
                    handleUpdateRequest(user, password, response.nfcTag, response.timestamp, "6");


                }
                else
                {
                    schema.time = dLastTime;
                    schema.status = "5";
                    Debug.WriteLine("Last Status updated to Break");
                    handleUpdateRequest(user, password, response.nfcTag, response.timestamp, "5");

                }
            }
            else
            {
                schema.time = DateTime.Now;
                schema.status = "99";
                handlePostRequest(schema);
                OnNfcStatusChange("NFC-Tag gelesen");
                Debug.WriteLine("send TimeStamp with status 8 undefined");
            }
        }
        private void handleUpdateRequest(string user, string password, string nfcTag, string timestamp, string status)
        {
            try
            {
                //response nfctag has spaces remove them
                nfcTag = nfcTag.Replace(" ", "");
                ApiRequest.makeUpdateRequest(user, password, nfcTag, timestamp, status);
                OnNfcStatusChange("NFC-Tag gelesen");
            }
            catch (Exception e)
            {
                OnNfcStatusChange("Fehler beim Erreichen der API");

            }
        }
        private void handlePostRequest(APIRequestSchema schema)
        {
            try
            {
                ApiRequest.makePostRequest(schema);
            }
            catch (Exception e)
            {
                OnNfcStatusChange("Fehler beim Erreichen der API");
            }
        }
        /// <summary>
        /// Handler for the Card Inserted Event 
        /// </summary>
        /// <param name="reader"></param>
        ///
        private void CardNative_OnDriverCardInserted(string reader)
        {
            StatusChangedEventHandler handler = EventHandler;

            

            try
            {
                //apduSender.OpenSmartCardConnection(readers[0]) moved into try catch block
                apduSender.OpenSmartCardConnection(reader);
                OnStatusChanged("Reading Card...");
                driverCard = new SmartCard(filePath);
                OnStatusChanged("Done Reading Card");

             } catch(SmartCardException e)
            {
                OnStatusChanged("Error. Reinsert Card");
                driverCard = null;
                Debug.WriteLine("Dont remove the card while its being read");
            } catch(Exception e)
            {
                OnStatusChanged("Error Reading Card");
            }
       



        }
        /// <summary>
        /// event handler for the cardRemoved event
        /// </summary>
        /// <param name="reader"></param>
        private void CardNative_OnDriverCardRemoved(string reader)
        {

            if(driverCard != null && driverCard.hasBeenRead == false)
            {
                Debug.WriteLine("Dont remove the Card while its being read");
            }
            try
            {
                apduSender.cardNativeDriver.Disconnect(DISCONNECT.Eject);
            }
            catch(Exception e)
            {
                OnStatusChanged("Error. Reinsert Card");
            }
            
            driverCard = null;
        }

    }
}
