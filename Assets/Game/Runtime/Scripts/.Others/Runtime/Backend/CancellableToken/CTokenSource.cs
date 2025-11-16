using System;
using System.Collections.Generic;
using System.Threading;

namespace Kumu.Kulitan.Backend
{
    public class CTokenSource : IDisposable
    {
        private CancellationTokenSource cTokenSource;

        public CancellationToken Token => cTokenSource.Token;

        private static readonly List<CTokenSource> activeTokens = new();

        public static CTokenSource Create()
        {
            var cTokenSource = new CTokenSource();
            activeTokens.Add(cTokenSource);
            return cTokenSource;
        }

        public static void DisposeAll(bool cancelAll = false)
        {
            while (activeTokens.Count > 0)
            {
                var token = activeTokens[0];

                if (cancelAll)
                {
                    token.Cancel();
                }

                token.Dispose();
            }
        }

        private CTokenSource()
        {
            cTokenSource = new CancellationTokenSource();
        }

        public void Cancel()
        {
            cTokenSource.Cancel();
        }

        public void Dispose()
        {
            if (cTokenSource == null)
            {
                return;
            }
            
            cTokenSource.Dispose();
            cTokenSource = null;
            activeTokens.Remove(this);
        }
    }
}
