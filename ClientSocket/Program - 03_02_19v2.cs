using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ClientSocket
{
    class Program
    {
        static int port = 8005;
        static string address = "127.0.0.1";
        //static string address = "192.168.1.3";

        static string getHistory = "0";
        static string wantExit = "2";
        static string whoIsOnline = "1";

        static bool sentName = false;

        static void Main(string[] args)
        {
            // READ SERVER CONFIGURATION FROM FILE CONFIG.TXT
            try
            {
                StreamReader rd = new StreamReader("config.txt");
                string strServer = rd.ReadLine().Trim();
                string strPort = rd.ReadLine().Trim();
                rd.Close();

                string[] strServers = strServer.Split(' ');
                string[] strPorts = strPort.Split(' ');
                port = int.Parse(strPorts[1]);
                address = strServers[1];

            }
            catch (Exception ex)
            {
                Console.WriteLine("\nCheck existance of original file config.txt in the directory with program.\n" +
                    "In this file you can change Server IP address and Server Port\n");
            }
           

            try
            {
                Console.WriteLine("Server configuration");
                Console.WriteLine("address : " + address);
                Console.WriteLine("port    : " + port);
                Console.WriteLine();

                Socket socket = null;

                Console.WriteLine("CHAT\n");
                Console.Write("Input your name: ");

                string name = Console.ReadLine().Trim();

                Console.WriteLine("\nType message and press Enter.\n");
                Console.WriteLine(getHistory + " - refresh messages");
                Console.WriteLine(whoIsOnline + " - online users");
                Console.WriteLine(wantExit + " - exit\n");

                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(address), port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);

                while (true)
                {
                    byte[] data = null;

                    if (sentName == false)
                    {
                        byte[] dataName = Encoding.Unicode.GetBytes(name);
                        sentName = true;
                        socket.Send(dataName);
                    }
                    else
                    {

                        Console.Write(name + " > ");
                        string message = Console.ReadLine();

                        if (message.Equals(wantExit))
                        {
                            data = Encoding.Unicode.GetBytes(message);
                            socket.Send(data);
                            Console.WriteLine("You was exited from the chat");
                            break;
                        }

                        data = Encoding.Unicode.GetBytes(message);
                        socket.Send(data);
                    }

                    //  GET ANSWER  
                    data = new byte[256];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;

                    do
                    {
                        bytes = socket.Receive(data, data.Length, 0);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (socket.Available > 0);

                    Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + builder.ToString() + "\n");

                }

                // CLOSE SOCKET
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("\n" +
                    "Run Server and try once more");
            }
            Console.Read();
        }
    }
}
