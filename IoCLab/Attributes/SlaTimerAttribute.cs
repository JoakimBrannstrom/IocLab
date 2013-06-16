using System;

namespace IoCLab.Attributes
{
    /// <summary>
    ///  Service level agreemen attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SlaTimerAttribute : Attribute
    {
        // http://en.wikipedia.org/wiki/Service_level_agreement#Common_metrics

        public TimeSpan MaxTime { get; private set; }

        public SlaTimerAttribute() : this(0)
        {
        }

        public SlaTimerAttribute(double milliseconds)
        {
            MaxTime = TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}
