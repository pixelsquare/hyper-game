using Kumu.Extensions;
using UnityEngine;

namespace Kumu.Kulitan.Tracking.Components
{
    public class AnalyticsDataTracker : MonoBehaviour
    {
        private static VisitorCountTracker visitorCountTracker;
        public static VisitorCountTracker VisitorCountTracker => visitorCountTracker;
        
        public void Initialize()
        {
            visitorCountTracker = new VisitorCountTracker();
        }
    }
}
