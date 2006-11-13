using System;
using System.Threading;
using WikiFunctions;

namespace WikiFunctions
{

    public class BackgroundRequest
    {
        public object Result;
        public bool Done
        {
            get
            {
                return (BgThread != null && BgThread.ThreadState == ThreadState.Stopped);
            }
        }
        public Exception Error;

        Thread BgThread;

        public BackgroundRequest()
        {
        }

        protected string Url;

        public void GetHTML(string url)
        {
            Url = url;

            BgThread = new Thread(new ThreadStart(GetHTMLFunc));
            BgThread.IsBackground = true;
            BgThread.Start();
        }

        private void GetHTMLFunc()
        {
            try
            {
                Result = Tools.GetHTML(Url);
            }
            catch (Exception e)
            {
                Error = e;
            }
        }

        public void Execute(Delegate d)
        {
            BgThread = new Thread(new ParameterizedThreadStart(ExecuteFunc));
            BgThread.IsBackground = true;
            BgThread.Start(d);
        }

        private void ExecuteFunc(object d)
        {
            try
            {
                Result = (d as Delegate).DynamicInvoke();
            }
            catch (Exception e)
            {
                Error = e;
            }
        }
    }
}