using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using OASMVC.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;
using OAS;
using System.Diagnostics;

namespace OASMVC.Repository
{
    public class OASRepository : IOASRepository
    {
        public OASData.Data oasData = new OASData.Data();
        private OASConfig.Config oasConfig = new OASConfig.Config();
        private Test oasTest = new Test();
        internal static string NetworkNode; // Set by the user
        internal static string NetworkPath; // Network path to include for remoting
        internal static bool NetworkNodeChanged;


        // A Queue or Hashtable is recommended to receieve your values so they can be processed on a seperate thread.
        // You can also process data directly from the ValuesChangedAll Event, but keep in mind other values will be buffered until you release the Event.
        private Queue m_DataValuesQueue = new Queue(); // This Queue demonstrates a technique for processing a lot of data.
        private Hashtable m_DataValuesHashtable = new Hashtable(); // This hashtable will contain the values for each Tag to access whenever you like.


        private IHubContext<OasTagHub> _hubContext;

        public OASRepository(IHubContext<OasTagHub> hubContext)
        {
            _hubContext = hubContext;
            oasData.ValuesChangedAll += OasData_ValuesChangedAll;
            
        }

        public int GetOASVersion(string networkNode)
        {
            OASVersion version = new OASVersion();
            version.Version = oasConfig.GetVersion(networkNode);

            GetOASGroups(networkNode);

            _hubContext.Clients.All.SendAsync("displayVersion", version.Version, networkNode);

            return version.Version;
        }

        public List<TagList> GetTagList(string networkNode, string groupName)
        {
            var tags = oasConfig.GetTagNames(groupName, networkNode);
            //Int32[] Errors = null;
            //var values = oasData.SyncReadTags(tags, ref Errors, 10000);

            List<TagList> tagList = new List<TagList>();

            foreach(var i in tags)
            {
                tagList.Add(new TagList { TagName = i });
            }

            _hubContext.Clients.All.SendAsync("updateTagList", tagList);

            return tagList;
        }

        public List<TagList> GetTagsAndValues(string networkNode, string groupName)
        {
            var tags = oasConfig.GetTagNames(groupName, networkNode);

            oasData.AddTags(tags);


            return null;
        }

        public void GetOASGroups(string networkNode)
        {
            List<string> groups = new List<string>();
            groups.Add("Root");

            var returnedGroups = oasConfig.GetGroupNames("", networkNode);

            foreach(var g in returnedGroups)
            {
                groups.Add(g);
            }

            _hubContext.Clients.All.SendAsync("populateGroups", groups);

        }

        private void OasData_ValuesChangedAll(string[] Tags, object[] Values, bool[] Qualities, DateTime[] TimeStamps)
        {
            // High speed version with a lot of data values changing
            lock (m_DataValuesQueue.SyncRoot)
            {
                m_DataValuesQueue.Enqueue(new ClassTagValues(Tags, Values, Qualities, TimeStamps));
                foreach(var v in m_DataValuesQueue)
                {
                    Debug.WriteLine($"Count Enqueued: {m_DataValuesQueue.Count}");
                }

            }

            // You can use the values directly here within the data event, but the example shown above with a Queue will work best with thousands of tags updating evey second.
            //' Simple version of just obtaining the tag value you are interested in.
            //Dim TagIndex As Int32
            //Dim NumberOfTagValues As Int32 = Tags.GetLength(0)
            //For TagIndex = 0 To NumberOfTagValues - 1
            //    Select Case Tags(TagIndex)
            //        Case "Ramp.Value"
            //            If Qualities(TagIndex) Then
            //                ' The value of Ramp.Value is contained in Values(TagIndex)
            //            Else
            //                ' The value of Ramp.Value is bad
            //            End If
            //    End Select
            //Next
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

        public static void SetNetworkNode(string NodeName)
        {
            NetworkNode = NodeName;
            if (string.Compare(NetworkNode, "localhost", true) == 0 || string.IsNullOrEmpty(NetworkNode))
            {
                NetworkPath = "";
            }
            else
            {
                NetworkPath = "\\\\" + NetworkNode + "\\";
                NetworkNodeChanged = true;
            }
        }
    }
}
