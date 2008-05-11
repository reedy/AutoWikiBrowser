/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
        DISCON = 510
    }

    internal static class Server
    {
        private static string SendResponse(ServerResponseCode ResponseCode, string Data)
        {
            return ResponseCode + " " + ResponseCode.ToString() + Data;
        }

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
