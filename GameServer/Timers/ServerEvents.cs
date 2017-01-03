using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Timers
{
    public class ServerEvents
    {
        public EventHandler StatusChanged;

        private Timer timer;
        private string previousStatus;

        public ServerEvents()
        {
            timer = new Timer(new TimerCallback(CheckForEvents), new AutoResetEvent(false), 0, 1000);
        }

        private void CheckForEvents(object state)
        {
            CheckStatus();
        }

        private void CheckStatus()
        {
            if (StatusChanged == null)
                return;

            string currentStatus = DedicatedGameServer.GetStatus();
            if (currentStatus != previousStatus)
                StatusChanged(this, null);

            previousStatus = currentStatus;
        }
    }
}
