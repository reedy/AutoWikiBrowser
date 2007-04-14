using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AutoWikiBrowser
{
    internal partial class NudgeTimer : System.Windows.Forms.Timer
    {
        /* TODO: I'm quite certain the logic isn't right here. The timer needs to be started and reset on a
         * successful save, and it needs to increase the time until next fire if the page still doesn't get
         * saved (e.g. wiki or net connection is down). */
        // Events
        public event TickEventHandler Tick;
        public delegate void TickEventHandler(object sender, NudgeTimer.NudgeTimerEventArgs EventArgs);

        // Methods
        public NudgeTimer(IContainer container)
            : base(container)
        {
            base.Tick += new EventHandler(this.NudgeTimer_Tick);
        }

        public void StartMe()
        {
            //base.Interval = 120000;
            base.Start();
        }

        public void Reset()
        {
            base.Interval = 120000;
        }

        private void NudgeTimer_Tick(object sender, EventArgs EventArgs)
        {
            NudgeTimerEventArgs MyEventArgs = new NudgeTimerEventArgs();
            Tick(this, MyEventArgs);
            if (!MyEventArgs.Cancel)
            {
                switch (base.Interval)
                {
                    case 120000:
                        base.Interval = 240000;
                        break;

                    case 240000:
                        base.Interval = 360000;
                        break;

                    case 360000:
                        base.Interval = 600000;
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
                base.Interval = 120000;
            }
        }

        // Nested Types
        public sealed class NudgeTimerEventArgs : EventArgs
        {
            // Fields
            private bool mCancel;

            // Properties
            public bool Cancel
            {
                get { return this.mCancel; }
                set { this.mCancel = value; }
            }
        }
    }
}
