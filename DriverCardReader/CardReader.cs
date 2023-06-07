using GemCard;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DriverCardReader
{
    public delegate void StatusChangedEventHandler(object sender, StatusChangedEventArgs e);


    /// <summary>
    /// Used mostly for ini method which starts the cardeventloop to listen for cardremoved and card inserted events
    /// </summary>
    public class CardReader
    {
        private bool initalised = false;
        private string[] readers;
        private APDUSender apduSender;
        SmartCard driverCard = null;
        private string filePath;
        private System.Windows.Forms.Label statusLabel;

        /// <summary>
        /// event handler when a the status from a card is changed (being read, done reading, etc)
        /// </summary>
        public event StatusChangedEventHandler EventHandler; 
        /// <summary>
        /// Initalises the reader and defines the eventhandlers for SmartCard Inserted events
        /// </summary>
        /// 

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


        public void Init(string filePath, System.Windows.Forms.Label statusLabel)
        {
            this.statusLabel = statusLabel;
            this.filePath = filePath;

            if(initalised == false) {
                apduSender = APDUSender.getInstance();
                readers = apduSender.cardNative.ListReaders();
                foreach (string reader in readers)
                {
                    Debug.WriteLine(reader);
                }
                apduSender.cardNative.StartCardEvents(readers[0]);
                apduSender.cardNative.OnCardInserted += CardNative_OnCardInserted;
                apduSender.cardNative.OnCardRemoved += CardNative_OnCardRemoved;
                initalised = true;
            }
            
        }


        /// <summary>
        /// Handler for the Card Inserted Event 
        /// </summary>
        /// <param name="reader"></param>
        ///
        private void CardNative_OnCardInserted(string reader)
        {
            StatusChangedEventHandler handler = EventHandler;

            

            try
            {
                //apduSender.OpenSmartCardConnection(readers[0]) moved into try catch block
                apduSender.OpenSmartCardConnection(readers[0]);
                OnStatusChanged("Reading Card...");
                driverCard = new SmartCard(filePath);
                OnStatusChanged("Done Reading Card");

             } catch(SmartCardException e)
            {
                OnStatusChanged("Error. Reinsert Card");
                driverCard = null;
                Debug.WriteLine("Dont remove the card while its being read");
            }
       



        }
        /// <summary>
        /// event handler for the cardRemoved event
        /// </summary>
        /// <param name="reader"></param>
        private void CardNative_OnCardRemoved(string reader)
        {

            if(driverCard != null && driverCard.hasBeenRead == false)
            {
                Debug.WriteLine("Dont remove the Card while its being read");
            }
            try
            {
                apduSender.cardNative.Disconnect(DISCONNECT.Eject);
            }
            catch(Exception e)
            {
                OnStatusChanged("Error. Reinsert Card");
            }
            
            driverCard = null;
        }
    }
}
