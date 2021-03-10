using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Management;
using Newtonsoft.Json;
using System.IO;

namespace SocketServer
{
    class Program
    {
        // Mensaje del emisor
        public static string data = null;

        public static void RecibirMensaje()
        {
            // Buffer de espera para recibir el mensaje 
            byte[] bytes = new Byte[1024];
            // Dns.GetHostName retorna el nombre del host que esta emitiendo el mensaje
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("192.168.56.101");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Creamo el socket con la comunicacion TCP 
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Unimos el socket con el endpoint local y recibimos algun mensaje entrante.  
            try
            {
                ManagementObjectSearcher myVideoObject = new ManagementObjectSearcher("select * from Win32_VideoController");
                ManagementObjectSearcher myProcessorObject = new ManagementObjectSearcher("select * from Win32_Processor");
                ManagementObjectSearcher myOperativeSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Empezamos a escuchar 
                while (true)
                {
                    Console.WriteLine("Waiting for connection...");
                    // El programa se supende mientras espera el mensaje del emisor
                    //Aqui es donde debemos iniciar el programa del emisor para que envie el mensaje
                    Socket handler = listener.Accept();
                    Console.WriteLine("Client connected via: {0}", handler.RemoteEndPoint.ToString());
                    data = null;

                    // Cuando el mensaje se recibe, se lo procesa para decodificarlo
                    while (true)
                    {
                        data = null;
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        // Mostramos el mensaje el la consola
                        Console.WriteLine("Client request: {0}", data);

                        string resp = "";
                        ManagementObject video = null;
                        ManagementObject processor = null;
                        ManagementObject os = null;

                        foreach (ManagementObject obj in myVideoObject.Get())
                        {
                            video = obj;
                        }
                        foreach (ManagementObject obj in myProcessorObject.Get())
                        {
                            processor = obj;
                        }
                        foreach (ManagementObject obj in myOperativeSystemObject.Get())
                        {
                            os = obj;
                        }
                        // Le respondemos al emisor 
                        if (data == "status")
                        {
                            ServerStatus server = new ServerStatus()
                            {
                                GPUName = video["Name"].ToString(),
                                GPUStatus = video["Status"].ToString(),
                                GPUDriverVersion = video["DriverVersion"].ToString(),
                                ProcessorName = processor["Name"].ToString(),
                                ProcessorCurrentClockSpeed = processor["CurrentClockSpeed"].ToString(),
                                OS = os["Caption"].ToString(),
                                OSVersion = os["Version"].ToString()
                            };
                            string jsonData = JsonConvert.SerializeObject(server);
                            resp = jsonData;
                        }
                        else if(data == "hardware")
                        {
                            ServerHardware server = new ServerHardware()
                            {
                                GPUName = video["Name"].ToString(),
                                GPUDeviceID = video["DeviceID"].ToString(),
                                GPUAdapterRAM = video["AdapterRAM"].ToString(),
                                GPUProcessor = video["VideoProcessor"].ToString(),
                                GPUArchitecture = video["VideoArchitecture"].ToString(),
                                ProcessorName = processor["Name"].ToString(),
                                ProcessorManufacturer =processor["Manufacturer"].ToString(),
                                ProcessorCurrentClockSpeed = processor["CurrentClockSpeed"].ToString(),
                                ProcessorNumberOfCores = processor["NumberOfCores"].ToString(),
                                ProcessorNumberOfEnabledCores = processor["NumberOfEnabledCore"].ToString(),
                                NumberOfLogicalProcessors = processor["NumberOfLogicalProcessors"].ToString(),
                                ProcessorArchitecture = processor["Architecture"].ToString()
                            };
                            string jsonData = JsonConvert.SerializeObject(server);
                            resp = jsonData;
                        }

                        byte[] msg = Encoding.ASCII.GetBytes(resp);
                        handler.Send(msg);

                        if (data.IndexOf("<EOF>") > -1)
                        {
                            byte[] ms = Encoding.ASCII.GetBytes("<EOF>");
                            handler.Send(ms);
                            break;
                        }
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    break;


                }
                Console.WriteLine("Client disconnected");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.Read();

        }

        public static int Main(String[] args)
        {
            RecibirMensaje();
            return 0;
        }
    }
}
