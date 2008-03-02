/*
WikiFunctions
Copyright (C) 2007 Martin Richards

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
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace WikiFunctions.IRC
{
    public delegate void OtherMessagesDel(object sender, EventArgs e, string IrcCommand);
    public delegate void ConnectEnabledDel(object sender, EventArgs e);
    public delegate void DisconnectDel(object sender, EventArgs e);
    public delegate void EditDel(object sender, EventArgs e, string Article, string Minor, string DiffLink, string User, int plusminus, string comment);
    public delegate void NewArticleDel(object sender, EventArgs e, string Artice, string User, int PlusMinus, string Comment);
    public delegate void NewUserDel(object sender, EventArgs e, string User);
    public delegate void PageMoveDel(object sender, EventArgs e, string OldName, string NewName, string User, string Comment);
    public delegate void UploadDel(object sender, EventArgs e, string File, string User, string Comment);
    public delegate void DeleteDel(object sender, EventArgs e, string Admin, string Article, string Comment);
    public delegate void RestoreDel(object sender, EventArgs e, string Admin, string Article, string Comment);
    public delegate void BlockDel(object sender, EventArgs e, string Admin, string Article, string Comment, string time);
    public delegate void UnblockDel(object sender, EventArgs e, string Admin, string Article, string Comment);
    public delegate void ProtectDel(object sender, EventArgs e, string Admin, string Article, string Comment);
    public delegate void UnprotectDel(object sender, EventArgs e, string Admin, string Article, string Comment);

    /// <summary>
    /// Contains functions to monitor IRC channels.
    /// </summary>
    public class WikiIRC
    {
        /// <summary>
        /// Initialises the class, accepting the necessary variables.
        /// </summary>
        /// <param name="Ircserver">The IRC server e.g. "irc.wikimedia.org".</param>
        /// <param name="Ircport">The port to use e.g. 6667.</param>
        /// <param name="Ircnick">The IRC nickname to use.</param>
        /// <param name="Ircchannel">The IRC channel to use e.g. "#en.wikipedia".</param>
        public WikiIRC(string Ircserver, int Ircport, string Ircnick, string Ircchannel)
        {
            IrcNick = Ircnick;
            IrcUser = Ircnick;
            IrcRealName = Ircnick;
            IrcChannel = Ircchannel;
            IsInvisble = false;
            IrcServer = Ircserver;
            IrcPort = Ircport;

            SOPC = new SendOrPostCallback(Process);
        }

        /// <summary>
        /// Listen to this event to receive the raw IRC messages.
        /// </summary>
        public event OtherMessagesDel OtherMessages;
        public event ConnectEnabledDel ConnectEvent;
        /// <summary>
        /// IRC disconnected
        /// </summary>
        public event DisconnectDel DisconnectEvent;

        public event EditDel Edit;
        public event NewArticleDel NewArticle;
        public event PageMoveDel PageMove;
        public event NewUserDel NewUser;
        public event UploadDel Upload;
        public event DeleteDel Delete;
        public event RestoreDel Restore;
        public event BlockDel Block;
        public event UnblockDel Unblock;
        public event ProtectDel Protect;
        public event UnprotectDel Unprotect;

        readonly Regex editRegex = new Regex(":14\\[\\[07(.*?)14\\]\\]4 (M?)10 02(.*?) 5\\* 03(.*?) 5\\* \\((.*?)\\) 10(.*?)$", RegexOptions.Compiled);
        readonly Regex newArticleRegex = new Regex(":14\\[\\[07(.*?)14\\]\\]4 M?N10 02(.*?) 5\\* 03(.*?) 5\\* \\((.*?)\\) 10(.*?)$", RegexOptions.Compiled);
        readonly Regex movePageRegex = new Regex(":14\\[\\[07Special:Log/move14\\]\\]4 move(_redir)?10 02 5\\* 03(.*?) 5\\*  10moved \\[\\[02(.*?)10\\]\\] to \\[\\[(.*?)\\]\\](.*?)$", RegexOptions.Compiled);
        
        readonly Regex newUserRegex = new Regex(":14\\[\\[07Special:Log/newusers14\\]\\]4 create10 02 5\\* 03(.*?) 5\\*  10\\(New user account\\)$", RegexOptions.Compiled);
        readonly Regex uploadRegex = new Regex(":14\\[\\[07Special:Log/upload14\\]\\]4 upload10 02 5\\* 03(.*?) 5\\*  10uploaded \"\\[\\[02(.*?)10\\]\\]\"(.*?)$", RegexOptions.Compiled);

        readonly Regex deleteRegex = new Regex(":14\\[\\[07Special:Log/delete14\\]\\]4 delete10 02 5\\* 03(.*?) 5\\*  10deleted \"02(.*?)10\":(.*?)$", RegexOptions.Compiled);
        readonly Regex restoreRegex = new Regex(":14\\[\\[07Special:Log/delete14\\]\\]4 restore10 02 5\\* 03(.*?) 5\\*  10restored \"\\[\\[02(.*?)10\\]\\]\":(.*?)$", RegexOptions.Compiled);

        readonly Regex blockRegex = new Regex(":14\\[\\[07Special:Log/block14\\]\\]4 block10 02 5\\* 03(.*?) 5\\*  10blocked \"02(.*?)10\" with an expiry time of (.*?):(.*?)$", RegexOptions.Compiled);
        readonly Regex unblockRegex = new Regex(":14\\[\\[07Special:Log/block14\\]\\]4 unblock10 02 5\\* 03(.*?) 5\\*  10unblocked 02(.*?)10:(.*?)", RegexOptions.Compiled);

        readonly Regex protectRegex = new Regex(":14\\[\\[07Special:Log/protect14\\]\\]4 protect10 02 5\\* 03(.*?) 5\\*  10protected 02(.*?)10:(.*?\\[(edit=.*?):(move=.*?)\\])$", RegexOptions.Compiled);
        readonly Regex unprotectRegex = new Regex(":14\\[\\[07Special:Log/protect14\\]\\]4 unprotect10 02 5\\* 03(.*?) 5\\*  10unprotected 02(.*?)10:(.*?)$", RegexOptions.Compiled);

        SendOrPostCallback SOPC;
        private SynchronizationContext context;
        Thread IRCThread;

        private void Process(object s)
        {//process messages here, fire off events
            if (Pause)
                return;

            string msg = s.ToString();

            if (msg == "DISCONNECTED")//disconnected
            {
                this.DisconnectEvent(null, null);
                return;
            }
     
            if (editRegex.IsMatch(msg))//edit
            {
                Match m = editRegex.Match(msg);

                int plusminus = intFromMessage(m.Groups[5].Value);

                this.Edit(null, null, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value, m.Groups[4].Value, plusminus, m.Groups[6].Value);
            }
            else if (newArticleRegex.IsMatch(msg))//new article
            {
                Match m = newArticleRegex.Match(msg);
                int plusmin = intFromMessage(m.Groups[4].Value);

                this.NewArticle(null, null, m.Groups[1].Value, m.Groups[3].Value, plusmin, m.Groups[5].Value);
            }
            else if (newUserRegex.IsMatch(msg))//new user
            {
                Match m = newUserRegex.Match(msg);

                this.NewUser(null, null, m.Groups[1].Value);
            }
            else if (movePageRegex.IsMatch(msg))//move
            {
                Match m = movePageRegex.Match(msg);

                string newName = removeSyntax(m.Groups[4].Value);

                this.PageMove(null, null, m.Groups[3].Value, newName, m.Groups[2].Value, m.Groups[5].Value);
            }
            else if (uploadRegex.IsMatch(msg))//upload
            {
                Match m = uploadRegex.Match(msg);

                this.Upload(null, null, m.Groups[2].Value, m.Groups[1].Value, m.Groups[3].Value);
            }
            else if (deleteRegex.IsMatch(msg))//delete
            {
                Match m = deleteRegex.Match(msg);

                this.Delete(null, null, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }
            else if (restoreRegex.IsMatch(msg))//delete
            {
                Match m = restoreRegex.Match(msg);

                this.Restore(null, null, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }
            else if (blockRegex.IsMatch(msg))//block
            {
                Match m = blockRegex.Match(msg);

                this.Block(null, null, m.Groups[1].Value, m.Groups[2].Value, m.Groups[4].Value, m.Groups[3].Value);
            }
            else if (unblockRegex.IsMatch(msg))//block
            {
                Match m = unblockRegex.Match(msg);

                this.Unblock(null, null, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }            
            else if (protectRegex.IsMatch(msg))//protection
            {
                Match m = protectRegex.Match(msg);

                this.Protect(null, null, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }
            else if (unprotectRegex.IsMatch(msg))//unprotection
            {
                Match m = unprotectRegex.Match(msg);

                this.Unprotect(null, null, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
            }
            else
            {
                if (OtherMessages != null)
                    this.OtherMessages(null, null, msg);
            }
        }

        public string removeSyntax(string s)
        {
            s = s.Replace("02", "").Replace("10", "");
            return s;
        }

        private int intFromMessage(string s)
        {
            string z = s;
            int i = 0;

            s = s.Trim('');
            s = s.TrimStart('-', '+');
            i = Int32.Parse(s);

            if (z.StartsWith("-"))
                i = i * -1;

            return i;
        }

        public void Start()
        {
            context = SynchronizationContext.Current;

            ThreadStart thr_Process = new ThreadStart(Connect);
            IRCThread = new Thread(thr_Process);
            IRCThread.IsBackground = true;
            IRCThread.Name = "IRCListener";
            IRCThread.Start();
        }

        private void Reconnect()
        {
            System.Threading.Thread.Sleep(2000);
            Connect();
        }

        string ircServer;
        int ircPort;
        string ircNick;
        string ircUser;
        string ircRealName;
        string ircChannel;
        bool isInvisible;
        TcpClient ircConnection;
        NetworkStream ircStream;
        StreamWriter ircWriter;
        StreamReader ircReader;

        #region properies

        /// <summary>
        /// Detetmines whether to automatically reconnect.
        /// </summary>
        private bool boolautoreconnect = true;
        public bool AutoReconnect
        {
            get { return boolautoreconnect; }
            set { boolautoreconnect = value; }
        }

        /// <summary>
        /// Stops the event from firing.
        /// </summary>
        private bool boolpause;
        public bool Pause
        {
            get { return boolpause; }
            set { boolpause = value; }
        }

        /// <summary>
        /// Set to false to terminate the IRC connection
        /// </summary>
        private static bool boolrun = true;
        public static bool Run
        {
            get { return boolrun; }
            set { boolrun = value; }
        }

        public string IrcServer
        {
            get { return ircServer; }
            set { ircServer = value; }
        }

        public int IrcPort
        {
            get { return ircPort; }
            set { ircPort = value; }
        }

        public string IrcNick
        {
            get { return ircNick; }
            set { ircNick = value; }
        }

        public string IrcUser
        {
            get { return ircUser; }
            set { ircUser = value; }
        }

        public string IrcRealName
        {
            get { return ircRealName; }
            set { ircRealName = value; }
        }

        public string IrcChannel
        {
            get { return ircChannel; }
            set { ircChannel = value; }
        }

        public bool IsInvisble
        {
            get { return isInvisible; }
            set { isInvisible = value; }
        }

        public TcpClient IrcConnection
        {
            get { return ircConnection; }
            set { ircConnection = value; }
        }

        public NetworkStream IrcStream
        {
            get { return ircStream; }
            set { ircStream = value; }
        }

        public StreamWriter IrcWriter
        {
            get { return ircWriter; }
            set { ircWriter = value; }
        }

        public StreamReader IrcReader
        {
            get { return ircReader; }
            set { ircReader = value; }
        }
        #endregion

        #region connect

        /// <summary>
        /// Connect to the IRC network.
        /// </summary>
        private void Connect()
        {
            try
            {
                // Connect with the IRC server.
                IrcConnection = new TcpClient(IrcServer, IrcPort);
                IrcStream = IrcConnection.GetStream();
                IrcReader = new StreamReader(IrcStream, System.Text.Encoding.UTF8);
                IrcWriter = new StreamWriter(IrcStream);

                // Authenticate our user
                string isInvisible = IsInvisble ? "8" : "0";
                IrcWriter.WriteLine(String.Format("USER {0} {1} * :{2}", IrcUser, isInvisible, IrcRealName));
                IrcWriter.Flush();
                IrcWriter.WriteLine(String.Format("NICK {0}", IrcNick));
                IrcWriter.Flush();
                IrcWriter.WriteLine(String.Format("JOIN {0}", IrcChannel));
                IrcWriter.Flush();

                this.ConnectEvent(null, null);

                // Listen for commands
                while (Run)
                {
                    string ircCommand;
                    while (Run && (ircCommand = IrcReader.ReadLine()) != null)
                    {
                        string[] commandParts = new string[ircCommand.Split(' ').Length];
                        commandParts = ircCommand.Split(' ');
                        if (commandParts[0].Substring(0, 1) == ":")
                        {
                            commandParts[0] = commandParts[0].Remove(0, 1);
                        }

                        if (commandParts[0] == IrcServer)
                        {
                            // Server message
                            switch (commandParts[1])
                            {
                                //case "332": this.IrcTopic(commandParts); break;
                                //	case "333": this.IrcTopicOwner(commandParts); break;
                                //case "353": this.IrcNamesList(commandParts); break;
                                case "366": /*this.IrcEndNamesList(commandParts);*/ break;
                                case "372": /*this.IrcMOTD(commandParts);*/ break;
                                case "376": /*this.IrcEndMOTD(commandParts);*/ break;
                                //	default: this.IrcServerMessage(commandParts); break;
                            }
                        }
                        else if (commandParts[0] == "PING")
                        {
                            // Server PING, send PONG back
                            this.IrcPing(commandParts);
                        }
                        else
                        {
                            object ob = ircCommand;
                            context.Post(SOPC, ob);
                        }
                    }
                }
               // run = true; //for testing purposes - simulate unwanted disconect.
                IrcWriter.Close();
                IrcReader.Close();
                IrcConnection.Close();

            }
            catch //(Exception ex)
            {
                // System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                context.Post(SOPC, "DISCONNECTED");
                Thread.CurrentThread.Abort();
                Thread.CurrentThread.Join();
            }
        }

        #endregion

        #region Private Methods
        //private void IrcTopic(string[] IrcCommand) {
        //    string IrcChannel = IrcCommand[3];
        //    string IrcTopic = "";
        //    for (int intI = 4; intI < IrcCommand.Length; intI++) {
        //        IrcTopic += IrcCommand[intI] + " ";
        //    }
        //    if (eventTopicSet != null) { this.eventTopicSet(IrcChannel, IrcTopic.Remove(0, 1).Trim()); }
        //} /* IrcTopic */

        //private void IrcTopicOwner(string[] IrcCommand) {
        //    string IrcChannel = IrcCommand[3];
        //    string IrcUser = IrcCommand[4].Split('!')[0];
        //    string TopicDate = IrcCommand[5];
        //    if (eventTopicOwner != null) { this.eventTopicOwner(IrcChannel, IrcUser, TopicDate); }
        //} /* IrcTopicOwner */

        //private void IrcNamesList(string[] IrcCommand) {
        //  string UserNames = "";
        //    for (int intI = 5; intI < IrcCommand.Length; intI++) {
        //        UserNames += IrcCommand[intI] + " ";
        //    }
        //    if (eventNamesList != null) { this.eventNamesList(UserNames.Remove(0, 1).Trim()); }
        //} /* IrcNamesList */

        //private void IrcServerMessage(string[] IrcCommand) {
        //    string ServerMessage = "";
        //    for (int intI = 1; intI < IrcCommand.Length; intI++) {
        //        ServerMessage += IrcCommand[intI] + " ";
        //    }
        //    if (eventServerMessage != null) { this.eventServerMessage(ServerMessage.Trim()); }
        //} /* IrcServerMessage */

        private void IrcPing(string[] ircCommand)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            for (int intI = 1; intI < ircCommand.Length; intI++)
            {
                builder.Append(ircCommand[intI] + " ");
            }
            IrcWriter.WriteLine("PONG " + builder.ToString());
            IrcWriter.Flush();
        } /* IrcPing */

        //private void IrcJoin(string[] IrcCommand) {
        //    string IrcChannel = IrcCommand[2];
        //    string IrcUser = IrcCommand[0].Split('!')[0];
        //    if (eventJoin != null) { this.eventJoin(IrcChannel.Remove(0, 1), IrcUser); }
        //} /* IrcJoin */

        //private void IrcPart(string[] IrcCommand) {
        //    string IrcChannel = IrcCommand[2];
        //    string IrcUser = IrcCommand[0].Split('!')[0];
        //    if (eventPart != null) { this.eventPart(IrcChannel, IrcUser); }
        //} /* IrcPart */

        //private void IrcMode(string[] IrcCommand) {
        //    string IrcChannel = IrcCommand[2];
        //    string IrcUser = IrcCommand[0].Split('!')[0];
        //    string UserMode = "";
        //    for (int intI = 3; intI < IrcCommand.Length; intI++) {
        //        UserMode += IrcCommand[intI] + " ";
        //    }
        //    if (UserMode.Substring(0, 1) == ":") {
        //        UserMode = UserMode.Remove(0, 1);
        //    }
        //    if (eventMode != null) { this.eventMode(IrcChannel, IrcUser, UserMode.Trim()); }
        //} /* IrcMode */

        //private void IrcNickChange(string[] IrcCommand) {
        //    string UserOldNick = IrcCommand[0].Split('!')[0];
        //    string UserNewNick = IrcCommand[2].Remove(0, 1);
        //    if (eventNickChange != null) { this.eventNickChange(UserOldNick, UserNewNick); }
        //} /* IrcNickChange */

        //private void IrcKick(string[] IrcCommand) {
        //    string UserKicker = IrcCommand[0].Split('!')[0];
        //    string UserKicked = IrcCommand[3];
        //    string IrcChannel = IrcCommand[2];
        //    string KickMessage = "";
        //    for (int intI = 4; intI < IrcCommand.Length; intI++) {
        //        KickMessage += IrcCommand[intI] + " ";
        //    }
        //    if (eventKick != null) { this.eventKick(IrcChannel, UserKicker, UserKicked, KickMessage.Remove(0, 1).Trim()); }
        //} /* IrcKick */

        //private void IrcQuit(string[] IrcCommand)
        //{
        //    string UserQuit = IrcCommand[0].Split('!')[0];
        //    string QuitMessage = "";
        //    for (int intI = 2; intI < IrcCommand.Length; intI++)
        //    {
        //        QuitMessage += IrcCommand[intI] + " ";
        //    }
        //    if (eventQuit != null) { this.eventQuit(UserQuit, QuitMessage.Remove(0, 1).Trim()); }
        //}
        #endregion
    }
}