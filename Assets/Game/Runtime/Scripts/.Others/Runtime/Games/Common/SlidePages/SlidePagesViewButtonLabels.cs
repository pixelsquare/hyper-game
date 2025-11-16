using System;

namespace Kumu.Kulitan.Common
{
    [Serializable]
    public class SlidePagesViewButtonLabels
    {
        public static readonly SlidePagesViewButtonLabels Default = new("Next", "Prev", "done", "Skip");
            
        public string nextLabel;
        public string prevLabel;
        public string doneLabel;
        public string skipLabel;

        public SlidePagesViewButtonLabels(string next, string prev, string done, string skip)
        {
            nextLabel = next;
            prevLabel = prev;
            doneLabel = done;
            skipLabel = skip;
        }

        public SlidePagesViewButtonLabels(string next, string prev) : this(next, prev, next, prev)
        {
        }
    }
}
