using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace hesong.plum.client.Utils
{
    class WaitCursorBlock : IDisposable
    {
        readonly static object lck = new object();
        readonly static Dictionary<Control, int> controlsCounter = new Dictionary<Control, int>();
        static int appCounter = 0;

        readonly Control control = null;
        readonly IReadOnlyCollection<Control> disableControls = null;

        public WaitCursorBlock()
        {
            Initial();
        }

        public WaitCursorBlock(IReadOnlyCollection<Control> disableControls)
        {
            this.disableControls = disableControls;
            Initial();
        }

        public WaitCursorBlock(Control control)
        {
            this.control = control;
            Initial();
        }

        public WaitCursorBlock(Control control, IReadOnlyCollection<Control> disableControls)
        {
            this.control = control;
            this.disableControls = disableControls;
            Initial();
        }

        void Initial()
        {
            int c;
            lock (lck)
            {
                if (control == null)
                {
                    c = ++appCounter;
                }
                else
                {
                    if (controlsCounter.ContainsKey(control))
                    {
                        c = ++controlsCounter[control];
                    }
                    else
                    {
                        c = controlsCounter[control] = 1;
                    }
                }
            }
            if (c == 1)
            {
                if (control == null)
                {
                    Application.UseWaitCursor = true;
                }
                else
                {
                    control.UseWaitCursor = true;
                }
            }
            if (disableControls != null)
            {
                foreach (var item in disableControls)
                {
                    item.Enabled = false;
                }
            }
        }

        ~WaitCursorBlock()
        {
            Dispose(false);
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Check to see if Dispose has already been called.
                if (!disposed)
                {
                    int c;
                    lock (lck)
                    {
                        if (control == null)
                        {
                            c = --appCounter;
                        }
                        else
                        {
                            c = --controlsCounter[control];
                            if (c < 1)
                            {
                                controlsCounter.Remove(control);
                            }
                        }
                    }
                    if (c == 0)
                    {
                        if (control == null)
                        {
                            Application.UseWaitCursor = false;
                        }
                        else
                        {
                            control.UseWaitCursor = false;
                        }
                    }
                    if (disableControls != null)
                    {
                        foreach (var item in disableControls)
                        {
                            item.Enabled = true;
                        }
                    }
                    // Note disposing has been done.
                    disposed = true;
                }
            }
        }
    }
}
