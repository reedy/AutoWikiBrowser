/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com
 With some Net.Sockets help and snippets from http://www.gnu.org/projects/dotgnu/pnetlib-doc/System/Net/Sockets/Socket.html

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AutoWikiBrowser.Plugins.Server
{
    internal enum ServerResponseCode
    {
        HELLO = 200,
        REQUIRE = 201,
        BYE = 210,
        OK = 250,
        PROTOCOL = 260,
        READY = 270,
        BUSY = 500,
        REJECTED = 501,
        UNAUTHORISED = 502,
        DISCON = 510,
        SERVICENOTAVAILABLE = 520
    }

    partial class ServerControl
    {
        internal static class Server
        {
            private class ServerWorker
            {
                private readonly Socket m_WorkerSocket;
                private bool LoggedIn;

                ServerWorker(Socket WorkerSocket)
                { m_WorkerSocket = WorkerSocket; }
            }

            private const int MAX_CLIENTS = 3;
            private const int MAX_BACKLOG = 3; // the number of incoming connections that can be queued for acceptance

            private static AsyncCallback WorkerCallBack;
            private static Socket mMainSocket;
            private static int m_clientCount = 0;
            private static List<ServerWorker> mWorkerSockets = new List<ServerWorker>();

            // Management
            /// <summary>
            ///  Start listening for connections
            /// </summary>
            /// <param name="port">The TCP port number to listen on</param>
            internal static void Init(int port)
            {
                IAsyncResult asyncAccept = null;

                try
                {
                    string hostname = Dns.GetHostName();

                    // Create a listening socket:
                    mMainSocket =
                        new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Bind to our preferred port on all interfaces:
                    mMainSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                    // Start listening:
                    mMainSocket.Listen(Math.Min((int)SocketOptionName.MaxConnections, MAX_BACKLOG));
                    asyncAccept = mMainSocket.BeginAccept(new AsyncCallback(Server.acceptCallbackDelegate), mMainSocket);
                }
                catch (SocketException ex)
                {
                    // TODO: Port not free or similar binding error
                }
                catch
                {
                    throw;
                }
            }

            /// <summary>
            /// Stop the server
            /// </summary>
            internal static void Stop()
            {
            }

            // Delegates
            /// <summary>
            /// Delegate which is called when a new connection is received
            /// </summary>
            /// <param name="asyncAccept"></param>
            private static void acceptCallbackDelegate(IAsyncResult asyncAccept)
            {
                if (mWorkerSockets.Count >= MAX_CLIENTS)
                    RejectConnection(asyncAccept);
                else
                    mMainSocket.EndAccept(asyncAccept);
            }

            private static void rejectConnectionDelegate(object sender, SocketAsyncEventArgs e)
            {
                // I don't know if there's a better way to do this or not (this is my first
                // attempt at System.Net.Sockets and it's complicated!) but let's extract the
                // socket object from the UserToken property and check that we're still connected,
                // and if we are... we kick out the JAMs
                Socket s = e.UserToken as Socket;
                if (s.Connected) s.Close();
            }

            // Server functionality:
            private static byte[] ServerResponse(ServerResponseCode ResponseCode, string Data)
            {
                if (Data != "") Data = " " + Data;
                return Encoding.UTF8.GetBytes(
                    (int)ResponseCode + " " + ResponseCode.ToString() + Data + "\r\n");
            }

            /// <summary>
            /// Reject a connection because all worker sockets are busy
            /// </summary>
            /// <param name="asyncAccept"></param>
            private static void RejectConnection(IAsyncResult asyncAccept)
            {
                // Accept the connection on a new socket
                Socket socket = mMainSocket.EndAccept(asyncAccept);

                // Build a new SocketAsyncEventArgs object:
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();

                // Create a "try again later" response and buffer it:
                byte[] buffer = ServerResponse(ServerResponseCode.SERVICENOTAVAILABLE, "Try again later");
                e.SetBuffer(buffer, 0, buffer.Length);

                // Send a reference to this socket for use in next callback:
                e.UserToken = socket;

                // Handle the Completed event:
                e.Completed += new EventHandler<SocketAsyncEventArgs>(rejectConnectionDelegate);

                // Blast that data down the wire yo:
                socket.SendAsync(e);
            }

            // Helper routines

            /*
             * When receiving new connection, check if somebody is logged in or not, and if they are
             * issue a BUSY. Reject all other commands except FORCE LOGIN; if that is received close the
             * earlier connection *after* the new one is authenticated.
             * 
             * When interracting with the command track the state of what command we receive next, and
             * accept only that command or commands which can be issued at any time (NB: If login is required
             * VERSION can only be processed *after* authentication).
             * 
             * CAN PROBABLY DO SOME OF THIS BY ADDING AND REMOVING HANDLERS DEPENDING ON STATE (vs logic).
             * 
             * Store details of current login and the last 5 or 10 connection attempts, and have a pop up
             * form the user can view.
             * 
             * Don't forget to write to the trace listener where applicable.
            */
        }
    }
}