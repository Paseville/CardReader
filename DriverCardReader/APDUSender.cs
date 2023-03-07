using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using GemCard;


namespace DriverCardReader
{
    /// <summary>
    /// A Singleton Class used to send APDUs to the Smart Card. And some static utils to work with byte arrays
    /// </summary>
    public sealed class APDUSender
    {
        private static APDUSender instance = null;
        /// <summary>
        /// A Publicly accesable SmartCard 
        /// Connection to the driver Card
        /// </summary>
        public CardNative cardNative = new CardNative();

        private APDUSender(){
            //OpenSmartCardConnection();
         }

        public static APDUSender getInstance()
        {
            if(instance == null)
            {
                instance = new APDUSender();
            }

            return instance;
        }
        /// <summary>
        /// A Dictonary containing all available commands to interact with the driver card
        /// </summary>
        private Dictionary<string, byte[]> availableCommands = new Dictionary<string, byte[]>()
        {
            {"SELECT_FILE", new byte[] {
                (byte) 0x00,
                (byte) 0xA4,
                (byte) 0x02,
                (byte) 0x0C,
                (byte) 0x02
                }
            },

            {"SWITCH_TO_CONTROL_DEVICE", new byte[]{
                (byte) 0x00,
                (byte) 0xA4,
                (byte) 0x04,
                (byte) 0x0C,
                (byte) 0x06,
                (byte) 0xFF,
                (byte) 0x54,
                (byte) 0x41,
                (byte) 0x43,
                (byte) 0x48,
                (byte) 0x4F
                }
            },

            {"READ_BINARY", new byte[] {
                (byte) 0x00,
                (byte) 0xB0
                }
            },

            {"UPDATE_BINARY", new byte[] {
                (byte) 0x00,
                (byte) 0xD6
                }
            },

            {"UPDATE_DOWNLOAD_DATE", new byte[]{
                (byte) 0x00,
                (byte) 0xD6,
                (byte) 0x00,
                (byte) 0x00,
                (byte) 0x04
                }
            },

            {"GET_CHALLENGE", new byte[] {
                (byte) 0x00,
                (byte) 0x84,
                (byte) 0x00,
                (byte) 0x00,
                (byte) 0x08
                }
            },

            {"VERIFY", new byte[] {
                (byte) 0x00,
                (byte) 0x20,
                (byte) 0x00,
                (byte) 0x00,
                (byte) 0x08
                //MISSING: the CHV which is variable (page 111 of norm)
                }       
            },

            {"GET_RESPONSE", new byte[] {
                (byte) 0x00,
                (byte) 0xC0,
                (byte) 0x00,
                (byte) 0x00,
                //MISSING: length of expected response (page 112 of norm)
                } 
            },

            {"INTERNAL_AUTHENTICATE", new byte[] {
                (byte) 0x00,
                (byte) 0x88
                } 
            },

            {"EXTERNAL_AUTHENTICATE", new byte[] {
                (byte) 0x00,
                (byte) 0x82
                }
            },

            {"PERFORM_HASH_OF_FILE", new byte[] {
                (byte) 0x80,
                (byte) 0x2A,
                (byte) 0x90,
                (byte) 0x00
                }
            },

            {"COMPUTE_DIGITAL_SIGNATURE", new byte[] {
                (byte) 0x00,
                (byte) 0x2A,
                (byte) 0x9E,
                (byte) 0x9A,
                (byte) 0x80
                } 
            }

            
        };



        /// <summary>
        /// connects to a card in the reader reader
        /// </summary>
        /// 
        public void OpenSmartCardConnection(string reader)
        {

            
            cardNative.Connect(reader, SHARE.Shared, PROTOCOL.T0orT1);

  

        }

        /// <summary>
        /// Sends an ADPU build from the Inputs to the card and returns the response if the ResponseCode was 9000 succesful. All parameters despite command are optional an normally null
        /// </summary>
        /// <param name="command">Command to be send</param>
        /// <param name="P1">byte P1</param>
        /// <param name="P2">byte P2</param>
        /// <param name="additonalParameters">Some commands require additional Parameters for expample SELECT_FILE requires a FILEID</param>
        /// <returns></returns>
        public byte[] SendAPDU(String command, byte? P1 = null , byte? P2 = null, byte[] additonalParameters = null)
        {
            MemoryStream outputStream = new MemoryStream(1000);
            StreamWriter outputStreamWriter = new StreamWriter(outputStream);
            byte[] readyToSendAPDU;
            APDUResponse response;

            if (availableCommands.ContainsKey(command))
            {
                readyToSendAPDU = (byte[])availableCommands[command].Clone();
            }
            else
            {
                throw new IllegalCommandException("Command: " + command + "is not supported");
            }

            if(P1 != null)
            {
                readyToSendAPDU = AppendByteArray(readyToSendAPDU, new byte[] { (byte)P1 });
            }

            if (P2 != null)
            {
                readyToSendAPDU = AppendByteArray(readyToSendAPDU, new byte[] { (byte)P2 });
            }

            if (additonalParameters != null)
            {
                readyToSendAPDU = AppendByteArray(readyToSendAPDU, additonalParameters);
            }

           Debug.WriteLine("Sending Command: " + APDUSender.ByteArrayToString(readyToSendAPDU));


            //API gets buffer length over the single bytes set in command so use another constructor
            if (command.Equals("COMPUTE_DIGITAL_SIGNATURE"))
            {
                response = cardNative.Transmit(new APDUCommand(readyToSendAPDU, (byte) 0x80));
            } else
            {
                response = cardNative.Transmit(new APDUCommand(readyToSendAPDU));
            }
            




            // if the status is not 9000 something went wrong!
            if(response.Status != 36864)
            {
                throw new Exception("Card Reader returned an unsuccesfull code. Code: " + response.Status);
            }

            return response.Data;
           
        }

        /// <summary>
        /// Sends an ADPU build from the Inputs to the card and returns the response if the ResponseCode was 9000(succesful) .All parameters despite command are optional an normally null
        /// But uses ints as parameters for easier use with read binary etc
        /// </summary>
        /// <param name="command"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="additonalParameters"></param>
        /// <returns></returns>
        /// <exception cref="IllegalCommandException"></exception>
        /// <exception cref="Exception"></exception>
        public byte[] SendAPDU(String command, int? P1 = null, int? P2 = null, int? additonalParameters = null)
        {
            MemoryStream outputStream = new MemoryStream(1000);
            StreamWriter outputStreamWriter = new StreamWriter(outputStream);
            byte[] readyToSendAPDU;
            APDUResponse response;
            //check if all entrys are valid and if yes append them to the final to send apdu
            if (availableCommands.ContainsKey(command))
            {
                readyToSendAPDU = (byte[])availableCommands[command].Clone();
            }
            else
            {
                throw new IllegalCommandException("Command: " + command + "is not supported");
            }

            if (P1 != null)
            {
                readyToSendAPDU = AppendByteArray(readyToSendAPDU, new byte[] { IntToSingleByte((int)P1) });
            }

            if (P2 != null)
            {
                readyToSendAPDU = AppendByteArray(readyToSendAPDU, new byte[] { IntToSingleByte((int)P2) });
            }

            if (additonalParameters != null)
            {
                readyToSendAPDU = AppendByteArray(readyToSendAPDU, new byte[] {IntToSingleByte((int)additonalParameters)});
            }

            Debug.WriteLine("Sending Command: " + command + " " + APDUSender.ByteArrayToString(readyToSendAPDU));


            
            response = cardNative.Transmit(new APDUCommand(readyToSendAPDU, IntToSingleByte((int)additonalParameters)));




            // if the status is not 9000 something went wrong!
            if (response.Status != 36864)
            {
                throw new Exception("Card Reader returned an unsuccesfull code. Code: " + response.Status);
            }

            return response.Data;

        }

        /// <summary>
        /// turns a byte array into a string
        /// </summary>
        /// <param name="ba">the byte array which is to be turned into a string</param>
        /// <returns>the byte array as a string</returns>
        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }


        /// <summary>
        /// Appends a byte array to another
        /// </summary>
        /// <param name="src">the byte array which data is to be appended to</param>
        /// <param name="toAppend">the byte array that is to be appended</param>
        /// <returns>a byte array which contains the complete byte array</returns>
        public static byte[] AppendByteArray(byte[] src, byte[] toAppend)
        {
            if (src == null)
            {
                return toAppend;
            }
            byte[] toReturn = new byte[src.Length + toAppend.Length];


            
            for(int i = 0; i<src.Length; i++)
            {
                toReturn[i] = src[i];
            }

            for(int i = src.Length; i<toReturn.Length; i++)
            {

                toReturn[i] = toAppend[i - src.Length];
            }

            return toReturn;
           
        }
        /// <summary>
        /// Casts a int into a single byte
        /// </summary>
        /// <param name="toTransform">integer to be cast</param>
        /// <returns>the single byte</returns>
        /// <exception cref="ArgumentException">thrown if int was too big to fit into a single byte</exception>
        public static byte IntToSingleByte(int toTransform)
           
        {
            byte[] valueWithZeros;
            if (toTransform > 255)
            {
                throw new ArgumentException("int too big to be converted by this function");
            }
            else
            {
                valueWithZeros = BitConverter.GetBytes(toTransform);
                if (BitConverter.IsLittleEndian)
                {
                   Array.Reverse(valueWithZeros);
                }
            }
            return valueWithZeros[valueWithZeros.Length-1];
        }


        /// <summary>
        /// casts a int into two a two byte long byte array
        /// </summary>
        /// <param name="toTransform">the intetger to be transformed</param>
        /// <returns>byte array containing the integer as two bytes</returns>
        /// <exception cref="ArgumentException">Is thrown if the int is too big for two bytes</exception>
        public static byte[] IntToTwoBytes(int toTransform)
        {
            byte[] valueWithZeros;
            if (toTransform > 65535)
            {
                throw new ArgumentException("int is too big for this function");
            }
            else
            {
                valueWithZeros = BitConverter.GetBytes(toTransform);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(valueWithZeros);
                }

            }
            return new byte[]
            {
                valueWithZeros[valueWithZeros.Length-1],
                valueWithZeros[valueWithZeros.Length-2]
            };
        }
        /// <summary>
        /// Removes all special characters from a string only allows ascii also removes whitespaces!!!
        /// </summary>
        /// <param name="str">the string that needs changing</param>
        /// <returns>A string cleared of all whitespaces and non ascii</returns>
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9')
                    || (str[i] >= 'A' && str[i] <= 'z'
                        || (str[i] == '.' || str[i] == '_')))
                {
                    sb.Append(str[i]);
                }
            }

            return sb.ToString();
        }
        public static string StreamToString(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }



}
