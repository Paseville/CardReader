using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.UI.Xaml.Controls;

namespace DriverCardReader
{
    /// <summary>
    /// A class that represents a Single smartcard. Upon Init all the data is downloaded from the card and saved
    /// </summary>
    class SmartCard
    {
        /// <summary>
        /// Used to track if the card has already been read
        /// </summary>
        public bool hasBeenRead = false;
        /// <summary>
        /// Stores all the files in a byte array in the tlv format
        /// </summary>
        private byte[] fileContentsAsTLV = null;

        /// <summary>
        /// stores all the card file objects on the card
        /// </summary>
        private List<SmartCardFile> cardFiles = new List<SmartCardFile>();
        
        /// <summary>
        /// Constructor for creating a Smartcard and Save it to given directory with filename that is the drivers name
        /// </summary>
        /// <param name="directoryPath">directory to Save the file to</param>
        public SmartCard(string directoryPath)
        {
            cardFiles.Add(new SmartCardFile("EF_ICC", new byte[] { (byte)0x00, (byte)0x02 }, 25));
            cardFiles.Add(new SmartCardFile("EF_IC", new byte[] { (byte)0x00, (byte)0x05 }, 8));
            cardFiles.Add(new SmartCardFile("EF_APPLICATION_IDENTIFICATION", new byte[] { (byte)0x05, (byte)0x01 }, 10));
            cardFiles.Add(new SmartCardFile("EF_CARD_CERTIFICATE", new byte[] { (byte)0xC1, (byte)0x00 }, 194));
            cardFiles.Add(new SmartCardFile("EF_CA_CERTIFICATE", new byte[] { (byte)0xC1, (byte)0x08 }, 194));
            cardFiles.Add(new SmartCardFile("EF_IDENTIFICATION", new byte[] { (byte)0x05, (byte)0x20 }, 143));
            cardFiles.Add(new SmartCardFile("EF_CARD_DOWNLOAD", new byte[] { (byte)0x05, (byte)0x0E }, 4));
            cardFiles.Add(new SmartCardFile("EF_DRIVING_LICENCE_INFO", new byte[] { (byte)0x05, (byte)0x21 }, 53));
            cardFiles.Add(new SmartCardFile("EF_EVENTS_DATA", new byte[] { (byte)0x05, (byte)0x02 }, 1728));
            cardFiles.Add(new SmartCardFile("EF_FAULTS_DATA", new byte[] { (byte)0x05, (byte)0x03 }, 1152));
            cardFiles.Add(new SmartCardFile("EF_DRIVER_ACTIVITY_DATA", new byte[] { (byte)0x05, (byte)0x04 }, 13780));
            cardFiles.Add(new SmartCardFile("EF_VEHICLES_USED", new byte[] { (byte)0x05, (byte)0x05 }, 6202));
            cardFiles.Add(new SmartCardFile("EF_PLACES", new byte[] { (byte)0x05, (byte)0x06 }, 1121));
            cardFiles.Add(new SmartCardFile("EF_CURRENT_USAGE", new byte[] { (byte)0x05, (byte)0x07 }, 19));
            cardFiles.Add(new SmartCardFile("EF_CONTROL_ACTIVITY_DATA", new byte[] { (byte)0x05, (byte)0x08 }, 46));
            cardFiles.Add(new SmartCardFile("EF_SPECIFIC_CONDITIONS", new byte[] { (byte)0x05, (byte)0x22 }, 280));

            //cards read themselves on creation after this is done card counts as read
            hasBeenRead = true;
            //Get the drivers Name from the EF_IDENTIFICATION file
            byte[] cardIdentification = null;
            byte[] identContent = GetSmartCardFile("EF_IDENTIFICATION").content;
            string cardIdentifcationString = null;
            Debug.WriteLine("Content of EF_IDENTIFICATION: " + APDUSender.ByteArrayToString(identContent));
       

            for(int i = 0; i <=17; i++)
            {
                cardIdentification =APDUSender.AppendByteArray(cardIdentification, new[] { identContent[i] });
            }

            
            //get a byte array which contains all the filedata as byteArray
            foreach(SmartCardFile file in cardFiles)
            {
                
                fileContentsAsTLV = file.Save(fileContentsAsTLV);
            }
            cardIdentifcationString = System.Text.Encoding.Default.GetString(cardIdentification);
            File.WriteAllBytes(directoryPath + "/" + APDUSender.RemoveSpecialCharacters(cardIdentifcationString) + ".ddd", fileContentsAsTLV);
        }
        
        /// <summary>
        /// returns a smartcardfile with the specified name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public SmartCardFile GetSmartCardFile(string fileName)
        {
            Debug.WriteLine("Currently Reading: " + fileName);
            int indexOfFile = cardFiles.IndexOf(new SmartCardFile(fileName));
            return cardFiles[indexOfFile];
        }
        
    }
}
