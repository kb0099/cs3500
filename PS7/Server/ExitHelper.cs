using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    class ExitHelper
    {
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(byte b);

        private Action OnExit;
        /// <summary>
        /// Helper to execute code after user clicks the close button.
        /// </summary>
        /// <param name="onExit">Action to execute at the program termination.</param>
        public ExitHelper(Action onExit)
        {
            OnExit += onExit;
            SetConsoleCtrlHandler((ctrl) =>
            {
                OnExit(); return true;
            }, true);
        }
    }
}
