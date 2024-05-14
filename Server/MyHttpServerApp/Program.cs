using System;
using System.IO;
using System.Net;
using System.Text;
using System.IO.Ports;

namespace SimpleHttpServer
{
    class Program
    {
        static SerialPort serialPort;

        static void Main(string[] args)
        {
            // Configure serial port
            serialPort = new SerialPort("COM5", 9600);
            serialPort.Open();

            // Start HTTP server
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:8000/");
            listener.Start();
            Console.WriteLine("Server started, listening on port 8000...");

            // Handle requests
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                // Add CORS headers to allow cross-origin requests
                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
                response.AddHeader("Access-Control-Allow-Headers", "Content-Type");

                if (request.HttpMethod == "GET")
                {
                    // Example GET request to send values 1, 100, 100 to Arduino
                    SendDataToArduino(1, 100, 100);

                    // Send response
                    string responseString = "Data sent to Arduino: 1, 100, 100";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentType = "text/plain";
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                }
                else if (request.HttpMethod == "POST")
                {
                    string requestBody;
                    using (StreamReader reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        requestBody = reader.ReadToEnd();
                    }

                    // Process the received data
                    string[] values = requestBody.Split(',');
                    if (values.Length == 3)
                    {
                        try
                        {
                            int value1 = int.Parse(values[0]);
                            int value2 = int.Parse(values[1]);
                            int value3 = int.Parse(values[2]);
                            Console.WriteLine(value1.ToString() + "," + value2.ToString() + "," + value3.ToString());

                            // Send data to Arduino
                            SendDataToArduino(value1, value2, value3);

                            // Send response
                            string responseString = "Data sent to Arduino";
                            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                            response.ContentType = "text/plain";
                            response.ContentLength64 = buffer.Length;
                            response.OutputStream.Write(buffer, 0, buffer.Length);
                        }
                        catch (FormatException)
                        {
                            SendErrorResponse(response, "Invalid data format");
                        }
                    }
                    else
                    {
                        SendErrorResponse(response, "Invalid number of values");
                    }
                }
                else
                {
                    SendErrorResponse(response, "Unsupported method");
                }

                response.Close();
            }
        }

        static void SendDataToArduino(int value1, int value2, int value3)
        {
            // Send data to Arduino
            string dataToSend = $"{value1},{value2},{value3}\n";
            serialPort.Write(dataToSend);
        }

        static void SendErrorResponse(HttpListenerResponse response, string message)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            response.ContentType = "text/plain";
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}