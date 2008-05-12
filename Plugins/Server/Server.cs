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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
        DISCON = 510
    }

    internal static class Server
    {
        // Management
        // This sub is from http://www.gnu.org/projects/dotgnu/pnetlib-doc/System/Net/Sockets/Socket.html with modifications
        internal static void Init(int port)
        {
            IAsyncResult asyncAccept = null;

            try
            {
                string hostname = Dns.GetHostName();

                // Create a listening socket:
                Socket listenSocket =
                    new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Bind to our preferred port on all interfaces:
                listenSocket.Bind(new IPEndPoint(IPAddress.Any, port));

                // Start listening:
                listenSocket.Listen(Math.Min((int)SocketOptionName.MaxConnections, 10));
                asyncAccept = listenSocket.BeginAccept(new AsyncCallback(Server.acceptCallback), listenSocket);
            }
            catch (SocketException ex)
            {
                // TODO
            }
            catch
            {
                throw;
            }

            // could call listenSocket.EndAccept(asyncAccept) here
            // instead of in the callback method, but since 
            // EndAccept blocks, the behavior would be similar to 
            // calling the synchronous Accept method

            //if (writeDot(asyncAccept) == true)
            //{
            //    // allow time for callbacks to
            //    // finish before the program ends 
            //    Thread.Sleep(3000);
            //}
        }

        internal static void Stop()
        {
        }

        // Delegates
        internal static void acceptCallback(IAsyncResult asyncAccept)
        {
        }

        // Server functionality:
        private static string SendResponse(ServerResponseCode ResponseCode, string Data)
        {
            return ResponseCode + " " + ResponseCode.ToString() + Data;
        }

        // Helper routines

        // From http://www.gnu.org/projects/dotgnu/pnetlib-doc/System/Net/Sockets/Socket.html:
        // times out after 20 seconds but operation continues
        //private static bool writeDot(IAsyncResult ar)
        //{
        //    int i = 0;
        //    while (ar.IsCompleted == false)
        //    {
        //        if (i++ > 40)
        //        {
        //            Console.WriteLine("Timed out.");
        //            return false;
        //        }
        //        Console.Write(".");
        //        Thread.Sleep(500);
        //    }
        //    return true;
        //}

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
