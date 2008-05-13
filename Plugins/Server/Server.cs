/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com

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
using System.Text.RegularExpressions;

namespace AutoWikiBrowser.Plugins.Server
{
    internal enum ServerResponseCode
    {
        None = 0,
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
        SERVICE_NOT_AVAILABLE = 520
    }

    partial class ServerControl
    {
        /// <summary>
        /// Receives TCP/IP connections from clients and processes them
        /// </summary>
        internal class Server : IDisposable
        {
            private const int MAX_CLIENTS = 3;
            private const int MAX_BACKLOG = 3; // the number of incoming connections that can be queued for acceptance

            private Socket mMainSocket; // Main server socket which accepts connections and passes them onto...:
            private List<ServerWorker> mWorkerSockets = new List<ServerWorker>(); // Worker sockets which talk to clients
            private ServerWorker mLoggedInClient; // A reference to the ServerWorker which is managing the one allowed active session (if any)

            // Management
            /// <summary>
            ///  Start listening for connections
            /// </summary>
            /// <param name="port">The TCP port number to listen on</param>
            internal void Init(int port)
            {
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
                    AcceptConnections();
                }
                //catch (SocketException ex)
                //{
                    // TODO: Port not free or similar binding error: Need to alert the user, stop server and pop up config
                //}
                catch { throw; }
            }

            private void AcceptConnections()
            { mMainSocket.BeginAccept(new AsyncCallback(this.AcceptConnectionDelegate), mMainSocket); }

            /// <summary>
            /// Stop the server
            /// </summary>
            internal static void Stop()
            {
                // TODO: Stop the server (maybe make disposable; certainly have to close all sockets)
            }

            /// <summary>
            ///  We've received a connection and have no user logged in, or we've received a FORCE LOGIN.
            ///  We get encryption set up and the user logged in (if specified by user settings).
            /// </summary>
            /// <param name="sw"></param>
            private void ProcessLogin(ServerWorker sw)
            {
                // send HELLO:
                //sw.SendData(ServerResponseCode.HELLO, "", 
                // Once logged in:
                mLoggedInClient = sw;

                // TODO: Reset to null when discon or log out
            }

            // Delegates
            /// <summary>
            /// Delegate which is called when a new connection is received
            /// </summary>
            /// <param name="asyncAccept"></param>
            private void AcceptConnectionDelegate(IAsyncResult asyncAccept)
            {
                // Accept the connection on a new socket
                Socket socket = mMainSocket.EndAccept(asyncAccept);

                // Assign the socket to a ServerWorker object:
                ServerWorker sw = new ServerWorker(socket);

                if (mWorkerSockets.Count >= MAX_CLIENTS)
                    // We cannot accept the connection as we have reached our limit:
                    sw.SendData(ServerResponseCode.SERVICE_NOT_AVAILABLE);
                else if (HaveLoggedInUser)
                    // We can accept the connection but client will have to FORCE LOGIN to proceed:
                    sw.SendData(ServerResponseCode.BUSY);
                else
                {
                    // We wish to accept the connection; let's get the user logged in (if need be) etc
                    sw.SendData(ServerResponseCode.HELLO);
                }

                // Release the main socket:
                AcceptConnections();
            }

            // Helper routines and properties
            private bool HaveLoggedInUser
            { get { return mLoggedInClient != null; } }

            #region IDisposable Members
            public void Dispose()
            {
                // TODO: IDisposable
                throw new NotImplementedException();
            }
            #endregion
        }
    }
}