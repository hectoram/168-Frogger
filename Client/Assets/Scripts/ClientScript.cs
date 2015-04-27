using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class ClientScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartClient();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// The port number for the remote device.
	public int port = 11000;
	public string ipAddress = "127.0.0.1";
	
	// ManualResetEvent instances signal completion.
	private static ManualResetEvent connectDone = 
		new ManualResetEvent(false);
    private static ManualResetEvent sendDone = 
		new ManualResetEvent(false);
	private static ManualResetEvent receiveDone = 
		new ManualResetEvent(false);
	
	// The response from the remote device.
	private static String response = String.Empty;
	
	public void StartClient() {

         // setup receiving thread
        Thread receiveThread = new Thread(delegate()
        {
		// Connect to a remote device.
		try {
			// Establish the remote endpoint for the socket.
			// The name of the 
			// remote device is "host.contoso.com".
			//IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			//IPAddress ipAddress = ipHostInfo.AddressList[0];
			//IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

			//var ip = IPAddress.Parse(ipAddress);
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            Debug.Log("Creating TCP/IP socket...");
			// Create a TCP/IP socket.
			Socket client = new Socket(AddressFamily.InterNetwork,
			                           SocketType.Stream, ProtocolType.Tcp);

            Debug.Log("Connecting to the remote endpoint...");
			// Connect to the remote endpoint.
			client.BeginConnect( remoteEP, 
			                    new AsyncCallback(ConnectCallback), client);
			connectDone.WaitOne();

            Debug.Log("Sending test data...");
			// Send test data to the remote device.
			Send(client,"This is a test<EOF>");
            Console.WriteLine("Message Sent!");
			sendDone.WaitOne();

            Debug.Log("Recieving response...");
			// Receive the response from the remote device.
			Receive(client);
            Console.WriteLine("Message Recieved!");
			receiveDone.WaitOne();
			
			// Write the response to the console.
			Console.WriteLine("Response received : {0}", response);

            Debug.Log("Response received : " + response);

            Debug.Log("Releasing the socket...");
			// Release the socket.
			try {
				client.Shutdown(SocketShutdown.Both);
			}
			catch (SocketException e) {
				Console.WriteLine ("Socket closed remotely");
			}
			client.Close();
			
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
        });
        receiveThread.Start();
	}
	
	private static void ConnectCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete the connection.
			client.EndConnect(ar);

            Debug.Log("Socket connected to {0}" + client.RemoteEndPoint.ToString());
			Console.WriteLine("Socket connected to {0}",
			                  client.RemoteEndPoint.ToString());
			
			// Signal that the connection has been made.
			connectDone.Set();
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	private static void Receive(Socket client) {
		try {
			// Create the state object.
			StateObject state = new StateObject();
			state.workSocket = client;
			
			// Begin receiving the data from the remote device.
			client.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,
			                    new AsyncCallback(ReceiveCallback), state);
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	private static void ReceiveCallback( IAsyncResult ar ) {
		try {
			// Retrieve the state object and the client socket 
			// from the asynchronous state object.
			StateObject state = (StateObject) ar.AsyncState;
			Socket client = state.workSocket;
			
			// Read data from the remote device.
			int bytesRead = client.EndReceive(ar);
			
			if (bytesRead > 0) {
				// There might be more data, so store the data received so far.
				state.sb.Append(Encoding.ASCII.GetString(state.buffer,0,bytesRead));
				
				// Get the rest of the data.
				client.BeginReceive(state.buffer,0,StateObject.BufferSize,0,
				                    new AsyncCallback(ReceiveCallback), state);
			} else {
				// All the data has arrived; put it in response.
				if (state.sb.Length > 1) {
					response = state.sb.ToString();
				}
				// Signal that all bytes have been received.
				receiveDone.Set();
			}
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	private static void Send(Socket client, String data) {
		// Convert the string data to byte data using ASCII encoding.
		byte[] byteData = Encoding.ASCII.GetBytes(data);
		
		// Begin sending the data to the remote device.
		client.BeginSend(byteData, 0, byteData.Length, 0,
		                 new AsyncCallback(SendCallback), client);
	}
	
	private static void SendCallback(IAsyncResult ar) {
		try {
			// Retrieve the socket from the state object.
			Socket client = (Socket) ar.AsyncState;
			
			// Complete sending the data to the remote device.
			int bytesSent = client.EndSend(ar);
			Console.WriteLine("Sent {0} bytes to server.", bytesSent);
			
			// Signal that all bytes have been sent.
			sendDone.Set();
		} catch (Exception e) {
			Console.WriteLine(e.ToString());
		}
	}
	
	/*public static int Main(String[] args) {
		StartClient();
		return 0;
	}*/
}

// State object for receiving data from remote device.
public class StateObject {
	// Client socket.
	public Socket workSocket = null;
	// Size of receive buffer.
	public const int BufferSize = 256;
	// Receive buffer.
	public byte[] buffer = new byte[BufferSize];
	// Received data string.
	public StringBuilder sb = new StringBuilder();
}
