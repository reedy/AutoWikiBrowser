/*
Autowikibrowser
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.ComponentModel;

namespace AutoWikiBrowser
{
    internal sealed partial class NudgeTimer : System.Windows.Forms.Timer
    {
        // Events
        public new event TickEventHandler Tick;
        public delegate void TickEventHandler(object sender, NudgeTimerEventArgs eventArgs);

        // Methods
        public NudgeTimer(IContainer container)
            : base(container)
        {
            base.Tick += NudgeTimerTick;
        }

        public void StartMe()
        {
            Start();
        }

        public void Reset()
        {
            Interval = 120000;
        }

        private void NudgeTimerTick(object sender, EventArgs eventArgs)
        {
            NudgeTimerEventArgs myEventArgs = new NudgeTimerEventArgs();
            Tick(this, myEventArgs);
            if (!myEventArgs.Cancel)
            {
                switch (Interval)
                {
                    case 120000:
                        Interval = 240000;
                        break;

                    case 240000:
                        Interval = 360000;
                        break;

                    case 360000:
                        Interval = 600000;
                        break;
                }
            }
        }

        // Properties
        public override bool Enabled
        {
            get { return base.Enabled; }
            set
            {
                base.Enabled = value;
                Interval = 120000;
            }
        }

        // Nested Types
        internal sealed class NudgeTimerEventArgs : EventArgs
        {
            public bool Cancel { get; set; }
        }
    }
}
