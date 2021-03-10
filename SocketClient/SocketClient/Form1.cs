using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Management;
using Newtonsoft.Json;
using System.IO;

namespace SocketClient
{
    public partial class Form1 : Form
    {

        public static string data = null;
        byte[] bytes;
        Socket sendr;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartClient();
        }

        public void StartClient()
        {
            bytes = new byte[1024];
            try
            {
                IPHostEntry ipHostInfo = Dns.GetHostEntry("DESKTOP-9EGCNMG");
                IPAddress ipAddress = IPAddress.Parse("192.168.56.101");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Creamos un socket TCP
                sendr = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                sendr.Connect(remoteEP);

                msg("Socket connected to " +
                    sendr.RemoteEndPoint.ToString());
                label1.Text = "Client Socket Program - Server Connected ...";
            }
            catch (ArgumentNullException ane)
            {
                msg("ArgumentNullException : " + ane.ToString());
            }
            catch (SocketException se)
            {
                msg("SocketException : " + se.ToString());
            }
            catch (Exception e)
            {
                msg("Unexpected exception : " + e.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            // Mensaje codificado 
            string mensaje = txtRequest.Text;
            msg("Request: " + mensaje);
            if(mensaje != "status" && mensaje != "hardware")
            {
                msg("Request unknown '" + mensaje + "', try with 'status' or 'hardware'");
                return;
            }
            byte[] msag = Encoding.ASCII.GetBytes(mensaje);

            // Enviamos el mensaje mediante el socket 
            sendr.Send(msag);

            // Recibe una respuesta
            while (true)
            {
                data = null;
                int bytesRec = sendr.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if(mensaje == "status")
                {
                    ServerStatus obj = JsonConvert.DeserializeObject<ServerStatus>(data);
                    msg("GPU: ");
                    msg("\t" + obj.GPUName);
                    msg("\t Status - " + obj.GPUStatus);
                    msg("\t Driver version - " + obj.GPUDriverVersion);

                    msg("Processor:");
                    msg("\t" + obj.ProcessorName);
                    msg("\t Current clock speed - " + obj.ProcessorCurrentClockSpeed);

                    msg("Operanting System:");
                    msg("\t" + obj.OS);
                    msg("\t OS version - " + obj.OSVersion);
                }
                else if(mensaje == "hardware")
                {
                    ServerHardware obj = JsonConvert.DeserializeObject<ServerHardware>(data);

                    msg("GPU: ");
                    msg("\t" + obj.GPUName);
                    msg("\t DevideID - " + obj.GPUDeviceID);
                    msg("\t Adapter RAM - " + obj.GPUAdapterRAM);
                    msg("\t Video Architecture - " + obj.GPUArchitecture);
                    msg("\t Video Processor - " + obj.GPUProcessor);

                    msg("Processor:");
                    msg("\t" + obj.ProcessorName);
                    msg("\t Manufacturer - " + obj.ProcessorManufacturer);
                    msg("\t Current clock speed - " + obj.ProcessorCurrentClockSpeed);
                    msg("\t Number of cores - " + obj.ProcessorNumberOfCores);
                    msg("\t Number of enabled cores - " + obj.ProcessorNumberOfEnabledCores);
                    msg("\t Number of logical processor - " + obj.NumberOfLogicalProcessors);
                    msg("\t Architecture - " + obj.ProcessorArchitecture);
                }
                break;
            }
            txtRequest.Text = "";
            txtRequest.Focus();
        }

        public void msg(string mesg)
        {
            txtPrompt.Text = txtPrompt.Text + Environment.NewLine + " >> " + mesg;
        } 
    }
}
