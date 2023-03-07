using System;
using System.Diagnostics;

namespace DriverCardReader

{
    /// <summary>
    /// Class to represent a SmartCard file, on creation downloads file from card
    /// </summary>
    class SmartCardFile
    {
        /// <summary>
        /// 2 Byte long byte array that contains the identifier of a file
        /// </summary>
        public byte[] fileIdentifier;

        /// <summary>
        /// Instance of the APDUsender
        /// </summary>
        private APDUSender apduSender = APDUSender.getInstance();

        /// <summary>
        /// fileSize as integer
        /// </summary>
        public int fileSize;


        /// <summary>
        /// fileName as String
        /// </summary>
        public string fileName { get; set; }


        /// <summary>
        /// Contents of the file
        /// </summary>
        public byte[] content;


        /// <summary>
        /// Signature of the SmartCardFile
        /// </summary>
        public byte[] signature;

        /// <summary>
        /// Used to store the Identifier of the file for the saving in a ddd file it is file tag + 00
        /// </summary>
        public byte[] tagTLV;

        /// <summary>
        /// used to store the identifier of the file for saving in a ddd file it is file tag + 01
        /// </summary>
        public byte[] tagSignatureTLV;

        /// <summary>
        /// stores the file length in specified tlv format
        /// </summary>
        public byte[] lengthTLV = new byte[2];
        /// <summary>
        /// Dont Use this Constructor its only for comapring files in the List!!
        /// </summary>
        /// <param name="fileName">name of the file</param>
        public SmartCardFile(string fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// Creates a SmartCardFile also populates the signatureidentifier the identifier and the fileSize
        /// </summary>
        /// <param name="fileName">Name of the file on Smartcard</param>
        /// <param name="fileIdentifier">fileIdentfier of the file</param>
        /// <param name="fileSize">size of the smartcardfile used to optimally read it</param>
        public SmartCardFile(string fileName, byte[] fileIdentifier, int fileSize)
        {
            this.fileName = fileName;
            this.fileIdentifier = fileIdentifier;
            this.fileSize = fileSize;
            this.tagTLV = new byte[]
            {
                fileIdentifier[0],
                fileIdentifier[1],
                (byte) 0x00
            };

            this.tagSignatureTLV = new byte[]
            {
                fileIdentifier[0],
                fileIdentifier[1],
                (byte) 0x01
            };

            byte[] lengthTLVHelper = BitConverter.GetBytes(fileSize);
            //be sure to always get the right bytes
            if (BitConverter.IsLittleEndian)
            {
              Array.Reverse(lengthTLVHelper);
            }

            lengthTLV[0] = lengthTLVHelper[2];
            lengthTLV[1] = lengthTLVHelper[3];


            Debug.WriteLine("Currently Reading File: " + fileName);

            //Be sure to only download the files with signature which actually need one
            if (fileName.Equals("EF_ICC") || fileName.Equals("EF_IC") || fileName.Equals("EF_CARD_CERTIFICATE") || fileName.Equals("EF_CA_CERTIFICATE"))
            {
                DownloadFile(false);
            }
            else
            {
                DownloadFileWithSignature();
            }
        }


        /// <summary>
        /// Downloads the Contents of the File from the smartcard
        /// </summary>
        /// <param name="calledByDownloadHashed">ALWAYS falsed just used internally to skip selecting File if called by download with signature</param>
        public void DownloadFile(bool calledByDownloadHashed)
        {
            if (!calledByDownloadHashed)
            {
                Select();
            }

            for (int i = 0; i < fileSize; i = i + 255)
            {
                int z = i;
                if (z > fileSize)
                {
                    z = fileSize - 1;
                }
                //APDU has two byte fields for offset length from start so after first byte field is filled split the number in two bytes
                if (z <= 255)
                {
                    //check if the amount i want to read is not larger than is left of the field size
                    if (z + 255 < fileSize)
                    {
                        apduSender.SendAPDU("READ_BINARY", 0, z, 255);
                        content = APDUSender.AppendByteArray(content, apduSender.SendAPDU("READ_BINARY", 0, z, 255));
                        //if it was bigger read the rest of the file
                    }
                    else
                    {
                        content = APDUSender.AppendByteArray(content, apduSender.SendAPDU("READ_BINARY", 0, z, fileSize - z));
                    }
                    //upper limit is maximum that can be offset due to defined standard
                }
                else if (z > 255 && z < 32767)
                {
                    byte[] result = APDUSender.IntToTwoBytes(z);
                    if (z + 255 < fileSize)
                    {
                        content = APDUSender.AppendByteArray(content, apduSender.SendAPDU("READ_BINARY", result[1], result[0], 255));
                    }
                    else
                    {
                        content = APDUSender.AppendByteArray(content, apduSender.SendAPDU("READ_BINARY", result[1], result[0], fileSize-z));
                    }
                }
            }

            Debug.WriteLine(APDUSender.ByteArrayToString(content));
        }

        /// <summary>
        /// Downloads the File and its signature stored in the public variable signature and content
        /// </summary>
        public void DownloadFileWithSignature()
        {
            Select();
            apduSender.SendAPDU("PERFORM_HASH_OF_FILE", null, null, null);
            DownloadFile(true);
            signature = apduSender.SendAPDU("COMPUTE_DIGITAL_SIGNATURE", null);

        }





        /// <summary>
        /// Function which selects this file
        /// </summary>
        public void Select() {
            //for all files despite EF_ICC and EF_IC Control device mode must be select before being able to read the files
            if (!fileName.Equals("EF_ICC") && !fileName.Equals("EF_IC"))
            {
                apduSender.SendAPDU("SWITCH_TO_CONTROL_DEVICE", null);
            }
            apduSender.SendAPDU("SELECT_FILE", null, null, fileIdentifier);

        }

        /// <summary>
        /// appends data of this smartcard to the byte array given in the function
        /// </summary>
        /// <param name="toAppendTo">the byte array the data is to be appended to</param>
        public byte[] Save(byte[] toAppendTo)
        {
            byte[] tlvObject = null;
            byte[] tlvObjectSignature = null;

            tlvObject = APDUSender.AppendByteArray(tlvObject, tagTLV);
            tlvObject = APDUSender.AppendByteArray(tlvObject, lengthTLV);
            tlvObject = APDUSender.AppendByteArray(tlvObject, content);
            Debug.WriteLine("tlvObject length of file "+fileName + " :" + APDUSender.ByteArrayToString(lengthTLV));
            Debug.WriteLine("Tag: " + APDUSender.ByteArrayToString(tagTLV));

            toAppendTo = APDUSender.AppendByteArray(toAppendTo, tlvObject);

            if(this.signature != null)
            {
                tlvObjectSignature = APDUSender.AppendByteArray(tlvObjectSignature, tagSignatureTLV);
                tlvObjectSignature = APDUSender.AppendByteArray(tlvObjectSignature, new byte[] { (byte) 0x00, (byte) 0x80});
                tlvObjectSignature = APDUSender.AppendByteArray(tlvObjectSignature, this.signature);

                toAppendTo = APDUSender.AppendByteArray(toAppendTo, tlvObjectSignature);
            }

            return toAppendTo;
        }

        /// <summary>
        /// Overwrite Equals two smartCardFiles are equals if their name is the Same
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            SmartCardFile objAsSmartCardFile = obj as SmartCardFile;
            if (objAsSmartCardFile == null) return false;
            if (this.fileName.Equals(objAsSmartCardFile.fileName))
            { 
                return true; 
            } else
            {
                return false;
            }
        }

    }
  
}

