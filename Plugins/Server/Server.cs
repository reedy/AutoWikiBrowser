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

// Note: This is a lightweight server. A heavyweight server would probably
// want to use object pooling.

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
        /// <summary>
        /// Receives TCP/IP connections from clients and processes them
        /// </summary>
        internal static class Server
        {
            private const int MAX_CLIENTS = 0; // HACK: // 3;
            private const int MAX_BACKLOG = 3; // the number of incoming connections that can be queued for acceptance

            private static Socket mMainSocket; // Main server socket which accepts connections and passes them onto...:
            private static List<ServerWorker> mWorkerSockets = new List<ServerWorker>(); // Worker sockets which talk to clients
            private static ServerWorker mLoggedInClient; // A reference to the ServerWorker which is managing the one allowed active session (if any)

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
                    // TODO: Port not free or similar binding error: Need to alert the user, stop server and pop up config
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
                // TODO: Stop the server
            }

            /// <summary>
            ///  We've received a connection and have no user logged in, or we've received a FORCE LOGIN.
            ///  We get encryption set up and the user logged in (if specified by user settings).
            /// </summary>
            /// <param name="sw"></param>
            private static void ProcessLogin(ServerWorker sw)
            {

                // Once logged in:
                mLoggedInClient = sw;

                // TODO: Reset to null when discon or log out
            }

            // Delegates
            /// <summary>
            /// Delegate which is called when a new connection is received
            /// </summary>
            /// <param name="asyncAccept"></param>
            private static void acceptCallbackDelegate(IAsyncResult asyncAccept)
            {
                // Accept the connection on a new socket
                Socket socket = mMainSocket.EndAccept(asyncAccept);

                // Assign the socket to a ServerWorker object:
                ServerWorker sw = new ServerWorker(socket);

                if (mWorkerSockets.Count >= MAX_CLIENTS)
                    // We cannot accept the connection as we have reached our limit:
                    sw.SendData(ServerResponseCode.SERVICENOTAVAILABLE, "Try again later",
                        new EventHandler<SocketAsyncEventArgs>(rejectConnectionDelegate));
                else if (HaveLoggedInUser)
                    // We can accept the connection but client will have to FORCE LOGIN to proceed:
                    sw.SendData(ServerResponseCode.BUSY, "",
                        new EventHandler<SocketAsyncEventArgs>(UserAlreadyLoggedInDelegate));
                else
                    // We wish to accept the connection; let's get the user logged in (if need be) etc
                    ProcessLogin(sw);
            }

            /// <summary>
            /// Delegate which closes a connection once a "Try again later" message has been sent
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private static void rejectConnectionDelegate(object sender, SocketAsyncEventArgs e)
            {
                // I don't know if there's a better way to do this or not (this is my first
                // attempt at System.Net.Sockets and it's complicated!) but let's extract the
                // socket object from the UserToken property and check that we're still connected,
                // and if we are... we kick out the JAMs
                ServerWorker s = e.UserToken as ServerWorker;
                if (s.Connected) s.Close();
            }

            /// <summary>
            ///  Delegate which receives a client response after we have sent BUSY
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private static void UserAlreadyLoggedInDelegate(object sender, SocketAsyncEventArgs e)
            {
            }

            // Helper routines and properties
            private static bool HaveLoggedInUser
            { get { return mLoggedInClient == null; } }

            /* TODO: NOTES:
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

            /// <summary>
            /// A wrapper around System.Net.Sockets.Socket; handles one client connection per object and maintains state
            /// </summary>
            private class ServerWorker
            {
                /* Custom object as I'm likely to want to store some stuff other than the socket at some point,
                 * and if not we at least get slightly better encapsaulation. */
                private readonly Socket m_WorkerSocket;

                // Constructor, receives an IAsyncResult corresponding to an incoming connection attempt:
                internal ServerWorker(Socket workerSocket)
                { m_WorkerSocket = workerSocket; }

                /// <summary>
                /// Format data for sending to client and assign an event handler for the Completed event
                /// </summary>
                /// <param name="ResponseCode"></param>
                /// <param name="Data"></param>
                /// <param name="Delegate"></param>
                internal void SendData(ServerResponseCode ResponseCode, string Data,
                    EventHandler<SocketAsyncEventArgs> Delegate)
                {
                    // Get a new SocketAsyncEventArgs object:
                    SocketAsyncEventArgs e = new SocketAsyncEventArgs();

                    // Create response and buffer it:
                    byte[] buffer = ServerResponse(ResponseCode, Data);
                    e.SetBuffer(buffer, 0, buffer.Length);

                    // Send a reference to this object for use in next callback:
                    e.UserToken = this;

                    // Assign delegate to handle the Completed event
                    e.Completed += Delegate;

                    // Blast that data down the wire yo:
                    m_WorkerSocket.SendAsync(e);
                }

                /// <summary>
                /// Server-response-code generator:
                /// </summary>
                /// <param name="ResponseCode"></param>
                /// <param name="Data"></param>
                /// <returns></returns>
                private static byte[] ServerResponse(ServerResponseCode ResponseCode, string Data)
                {
                    if (Data != "") Data = " " + Data;
                    return Encoding.UTF8.GetBytes(
                        (int)ResponseCode + " " + ResponseCode.ToString() + Data + "\r\n");
                }

                internal bool Connected
                { get { return m_WorkerSocket.Connected; } }

                internal void Close()
                { m_WorkerSocket.Close(); }
            }
        }
    }
}