using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace OAS
{
    public class OASComponents : IOASComponents
    {
        private OASData.Data oasData = new OASData.Data();
        private OASConfig.Config oasConfig = new OASConfig.Config();
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(10000);
        internal static string NetworkNode; // Set by the user
        internal static string NetworkPath; // Network path to include for remoting
        internal static bool NetworkNodeChanged;
        internal static bool m_InProcessValues = false;
        internal static bool m_closing = false;
        private readonly IHubContext<OasHub> _hubContext;

        // A Queue or Hashtable is recommended to receieve your values so they can be processed on a seperate thread.
        // You can also process data directly from the ValuesChangedAll Event, but keep in mind other values will be buffered until you release the Event.
        private Queue m_DataValuesQueue = new Queue(); // This Queue demonstrates a technique for processing a lot of data.
        private Hashtable m_DataValuesHashtable = new Hashtable(); // This hashtable will contain the values for each Tag to access whenever you like.
        
        public OASComponents()
        {
            oasData.ValuesChangedAll += OasData_ValuesChangedAll;
        }

        public int GetOASVersion()
        {
            return 10;
        }

        public void AddTags()
        {
            oasData.AddTag(@"\\13-01-0110.ssc.savageservices.com\Pump.Value");

            oasData.ValuesChangedAll += OasData_ValuesChangedAll;
        }

        private void OasData_ValuesChangedAll(string[] Tags, object[] Values, bool[] Qualities, DateTime[] TimeStamps)
        {
            // High speed version with a lot of data values changing
            lock (m_DataValuesQueue.SyncRoot)
            {
                m_DataValuesQueue.Enqueue(new ClassTagValues(Tags, Values, Qualities, TimeStamps));
            }
            Debug.WriteLine($"Inside eventHandler: {m_DataValuesQueue.Count}");
        }

        internal class ClassTagValues
        {
            internal string[] TagNames;
            internal object[] Values;
            internal bool[] Qualities;
            internal DateTime[] TimeStamps;

            public ClassTagValues(string[] NewTagNames, object[] NewValues, bool[] NewQualities, DateTime[] NewTimeStamps)
            {
                TagNames = NewTagNames;
                Values = NewValues;
                Qualities = NewQualities;
                TimeStamps = NewTimeStamps;
            }
        }

    }
}
