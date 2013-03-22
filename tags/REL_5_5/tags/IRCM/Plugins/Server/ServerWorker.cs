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

using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoWikiBrowser.Plugins.Server
{
    partial class ServerControl
    {
        /// <summary>
        /// A wrapper around System.Net.Sockets.Socket; handles one client connection per object and maintains state
        /// </summary>
        private class ServerWorker
        {
            /* Custom object as I'm likely to want to store some stuff other than the socket at some point,
             * and if not we at least get slightly better encapsulation. */

            private readonly Socket m_WorkerSocket;
            private ServerResponseCode LastServerResponse = ServerResponseCode.None;

            private readonly static Regex parse = new Regex("^(?<command>[^\\s]{1,})\\b\\s*(?<data>.*)",
                RegexOptions.Compiled | RegexOptions.ExplicitCapture);

            // Constructor, receives an IAsyncResult corresponding to an incoming connection attempt:
            internal ServerWorker(Socket workerSocket)
            { m_WorkerSocket = workerSocket; }

            /// <summary>
            /// Format data for sending to client and assign an event handler for the Completed event
            /// </summary>
            /// <param name="ResponseCode"></param>
            /// <param name="Data"></param>
            internal void SendData(ServerResponseCode ResponseCode)
            { SendData(ResponseCode, ""); }

            /// <summary>
            /// Format data for sending to client and assign an event handler for the Completed event
            /// </summary>
            /// <param name="ResponseCode"></param>
            /// <param name="Data"></param>
            internal void SendData(ServerResponseCode ResponseCode, string Data)
            {
                // Get a new SocketAsyncEventArgs object:
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();

                // Create response and buffer it:
                byte[] buffer = ServerResponse(ResponseCode, Data);
                e.SetBuffer(buffer, 0, buffer.Length);

                // Send a reference to this object for use in next callback:
                e.UserToken = this;

                // Store our last response:
                LastServerResponse = ResponseCode;

                // Assign delegate to handle the Completed event
                switch (ResponseCode)
                {
                    case ServerResponseCode.None:
                        throw new ArgumentException();

                    case ServerResponseCode.DISCON:
                    case ServerResponseCode.SERVICE_NOT_AVAILABLE:
                        e.Completed += this.rejectConnectionDelegate;
                        break;

                    default:
                        e.Completed += this.DataSentDelegate;
                        break;
                }

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
                if (Data != "") Data = " " + Data; // this will be ok for null too won't it?
                return Encoding.UTF8.GetBytes(
                    (int)ResponseCode + " " + ResponseCode.ToString() + Data + "\r\n");
            }

            internal bool Connected
            { get { return m_WorkerSocket.Connected; } }

            internal void Close()
            { m_WorkerSocket.Close(); }

            // Delegates:
            // TODO: Ensure all code inside delegates here and in outer class which will run on a different thread are thread-safe,
            // i.e. they safely access shared variables and don't directly interfere with the UI
            // see e.g. http://www.codeguru.com/csharp/csharp/cs_network/sockets/article.php/c8781/#Client6
            /// <summary>
            /// Delegate which closes a connection once a "Try again later" message has been sent
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void rejectConnectionDelegate(object sender, SocketAsyncEventArgs e)
            {
                // I don't know if there's a better way to do this or not (this is my first
                // attempt at System.Net.Sockets and it's complicated!) but let's extract the
                // socket object from the UserToken property and check that we're still connected,
                // and if we are... we kick out the JAMs
                //ServerWorker s = e.UserToken as ServerWorker; // probably don't need this now we're using delegates inside instance variables
                if (Connected) Close();
            }

            /// <summary>
            ///  Delegate which listens for a client response or command after we sent a request or a challenge
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void DataSentDelegate(object sender, SocketAsyncEventArgs e)
            {
                // Based on a sub from http://www.ajaxpro.info/download/SocketClient.cs.txt
                if (e.SocketError == SocketError.Success)
                {
                    if (e.LastOperation == SocketAsyncOperation.Send)
                    {
                        // Prepare receiving.
                        //Socket s = e.UserToken as Socket;

                        byte[] response = new byte[255];
                        e.SetBuffer(response, 0, response.Length);

                        switch (LastServerResponse)
                        {
                            case ServerResponseCode.OK:
                            case ServerResponseCode.REJECTED:
                            case ServerResponseCode.READY:
                                e.Completed += new EventHandler<SocketAsyncEventArgs>(this.OnReceiveDelegate);
                                break;

                            case ServerResponseCode.BYE:
                            case ServerResponseCode.BUSY:
                            case ServerResponseCode.HELLO:
                            case ServerResponseCode.REQUIRE:
                            case ServerResponseCode.UNAUTHORISED:
                            case ServerResponseCode.PROTOCOL:
                                e.Completed += new EventHandler<SocketAsyncEventArgs>
                                    (this.WaitingForClientResponseDelegate);
                                break;
                        }

                        m_WorkerSocket.ReceiveAsync(e);
                    }
                }
                else
                {
                    throw new SocketException((int)e.SocketError);
                }

            }

            /// <summary>
            ///  Delegate which handles client data when we're waiting for a specific response or an at-any-time command
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void WaitingForClientResponseDelegate(object sender, SocketAsyncEventArgs e)
            { // Called
                try
                {
                    GroupCollection captureGroups = ParseReceivedData(e);
                    switch (captureGroups["command"].Value)
                    {
                        case "HELLO":
                            SendData(ServerResponseCode.OK, "Get the fuck out of Dodge!");
                            break;

                            // TODO: Other commands / rejecting commands we can't accept right now
                    }
                }
                catch (AWBServerCommandNotRecognisedException)
                // Couldn't parse any command out of the data we received
                { SendData(ServerResponseCode.REJECTED, "Command not understood"); }
                catch { throw; }
            }

            /// <summary>
            ///  Delegate which handles client data when we're waiting for something to happen (us to send an event, or client to issue a command)
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void OnReceiveDelegate(object sender, SocketAsyncEventArgs e)
            {
            }

            // Helpers:
            private GroupCollection ParseReceivedData(SocketAsyncEventArgs e)
            {
                Match m = parse.Match(Encoding.UTF8.GetString(e.Buffer, 0, e.BytesTransferred));

                if (m.Success)
                    return m.Groups;
                else
                    // TODO: We're getting here when the client has quit and not actually sent anything
                    throw new AWBServerCommandNotRecognisedException();
            }


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
             * If we're sending something to the client, make sure we're not still waiting for a response and if we
             * are queue up the transmission. LastServerResponse may be enough for this or we may need a Blocking bool
             * or something... I'll see when I get that far.
             * 
             * Show details of current login and the last 5 or 10 connection attempts and other guff on the tab page
             * 
             * Don't forget to write to the trace listener where applicable.
            */
        }

        [Serializable]
        private sealed class AWBServerCommandNotRecognisedException : ApplicationException
        {
            //public AWBServerCommandNotRecognisedException(string message)
            //: base(message) { }
        }
    }
}
