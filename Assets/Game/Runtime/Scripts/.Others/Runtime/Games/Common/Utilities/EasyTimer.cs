using System;
using System.Timers;

namespace Kumu.Kulitan.Common
{
    public class EasyTimer : IDisposable
    {
        private readonly Timer timer;
        
        public EasyTimer(float time, Action actionToBeExecuted)
        {
            timer = new Timer
            {
                Interval = time * 1000,
            };
            timer.Elapsed += (o, args) =>
            {
                actionToBeExecuted();
                Dispose();
            };
            
            timer.Start();
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}
