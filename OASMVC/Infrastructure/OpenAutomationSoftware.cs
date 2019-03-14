using Microsoft.AspNetCore.SignalR;
using OASMVC.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static OASMVC.Models.OASTagModels;

namespace OASMVC.Infrastructure
{
    public class OpenAutomationSoftware : IOpenAutomationSoftware
    {
        private OASData.Data oasData = new OASData.Data();
        private OASConfig.Config oasConfig = new OASConfig.Config();

        internal static string NetworkNode; // Set by the user
        internal static string NetworkPath; // Network path to include for remoting
        internal static bool NetworkNodeChanged;
        internal static bool m_InProcessValues = false;
        internal static bool m_closing = false;

        // A Queue or Hashtable is recommended to receieve your values so they can be processed on a seperate thread.
        // You can also process data directly from the ValuesChangedAll Event, but keep in mind other values will be buffered until you release the Event.
        private Queue m_DataValuesQueue = new Queue(); // This Queue demonstrates a technique for processing a lot of data.
        private Hashtable m_DataValuesHashtable = new Hashtable(); // This hashtable will contain the values for each Tag to access whenever you like.

        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);

        private IHubContext<OasTagHub> _hubContext;
        private IOpenAutomationSoftware _oasComponents;

        public OpenAutomationSoftware(IHubContext<OasTagHub> oasHub)
        {
            _hubContext = oasHub;
            oasData.ValuesChangedAll += OasData_ValuesChangedAll;
            _timer = new Timer(ProcessQueue, null, _updateInterval, _updateInterval);

        }

        public void GetOASVersion(string networkNode)
        {
            SetNetworkNodeName(networkNode);
            OASVersion version = new OASVersion();
            version.Version = oasConfig.GetVersion(NetworkNode);
            SendGroups();
            _hubContext.Clients.All.SendAsync("displayVersion", version.Version, networkNode);

        }

        internal void SendGroups()
        {
            List<string> groups = new List<string>();
            groups.Add("Root");

            var returnedGroups = oasConfig.GetGroupNames("", NetworkNode);

            foreach (var g in returnedGroups)
            {
                groups.Add(g);
            }

            _hubContext.Clients.All.SendAsync("populateGroups", groups);
        }

        public List<TagList> GetTagList(string nodeName, string groupName)
        {
            if (groupName == "Root")
                groupName = "";

            List<TagList> tagList = new List<TagList>();

            var tags = oasConfig.GetTagNames(groupName, NetworkNode);

            foreach(var i in tags)
            {
                tagList.Add(new TagList { TagName = i });
            }

            AddTags(groupName, NetworkNode);

            _hubContext.Clients.All.SendAsync("loadTags", tagList);

            return tagList;
            
            
        }

        public void AddTags(string groupName, string networkNode)
        {

            var tags = oasConfig.GetTagNames(groupName, NetworkNode);
            string[] values = tags
                .Select(x => NetworkPath + x + ".Value")
                .ToArray();


            oasData.AddTags(values);
        }

        internal static void SetNetworkNodeName(string nodeName)
        {
            NetworkNode = nodeName;
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

        private void ProcessQueue(object state)
        {
            if (m_InProcessValues)
            {
                return;
            }
            if (m_closing)
            {
                return;
            }
            m_InProcessValues = true;

            ClassTagValues[] arrTagValues = new ClassTagValues[0];
            Int32 numberOfTagValues = 0;
            lock (m_DataValuesQueue.SyncRoot)
            {
                if (m_DataValuesQueue.Count < 1)
                {
                    Debug.WriteLine("Nothing in queue");
                    m_InProcessValues = false;
                    return; // There is nothing to do
                }
                numberOfTagValues = m_DataValuesQueue.Count;
                arrTagValues = new ClassTagValues[numberOfTagValues];
                m_DataValuesQueue.CopyTo(arrTagValues, 0);
                m_DataValuesQueue.Clear();
                Debug.WriteLine("Queue cleared");
            }

            Int32 allTagValuesIndex = 0;
            ClassTagValues allTagValues = null;

            string[] tagNames = null;
            object[] tagValues = null;
            bool[] tagQualities = null;
            DateTime[] tagTimeStamps = null;
            Int32 tagIndex = 0;
            Int32 numberOfTags = 0;

            System.Text.StringBuilder UpdateString = new System.Text.StringBuilder();
            double ValueDouble = 0;
            int ValueInteger = 0;
            bool ValueBoolean = false;
            string ValueString = null;
            ValueDouble = 0;
            ValueInteger = 0;
            ValueBoolean = false;
            ValueString = "";

            for (allTagValuesIndex = 0; allTagValuesIndex < numberOfTagValues; allTagValuesIndex++)
            {
                // Obtain arrays of tags and values from the tag values class previously stored in the OpcSystemsData_ValuesChangedAll event
                allTagValues = arrTagValues[allTagValuesIndex];
                tagNames = allTagValues.TagNames;
                tagValues = allTagValues.Values;
                tagQualities = allTagValues.Qualities;
                tagTimeStamps = allTagValues.TimeStamps;
                numberOfTags = tagNames.GetLength(0);
                TagList tagList = new TagList();
                string value = null;
                for (tagIndex = 0; tagIndex < numberOfTags; tagIndex++)
                {
                    if (tagQualities[tagIndex])
                    {
                        if (object.ReferenceEquals(tagValues[tagIndex].GetType(), typeof(double)))
                        {
                            value = Convert.ToDouble(tagValues[tagIndex]).ToString();
                        }
                        else if (object.ReferenceEquals(tagValues[tagIndex].GetType(), typeof(int)))
                        {
                            value = Convert.ToInt32(tagValues[tagIndex]).ToString();
                        }
                        else if (object.ReferenceEquals(tagValues[tagIndex].GetType(), typeof(bool)))
                        {
                            value = Convert.ToBoolean(tagValues[tagIndex]).ToString();
                        }
                        else if (object.ReferenceEquals(tagValues[tagIndex].GetType(), typeof(string)))
                        {
                            value = tagValues[tagIndex].ToString();
                        }

                        _hubContext.Clients.All.SendAsync("updateTagValue", new TagList { TagName = tagNames[tagIndex].Substring(0,tagNames[tagIndex].IndexOf(".")), TimeStamp = tagTimeStamps[tagIndex], Value = value });
                    }
                }

                lock (m_DataValuesHashtable.SyncRoot)
                {
                    for (tagIndex = 0; tagIndex < numberOfTags; tagIndex++)
                    {
                        try
                        {
                            UpdateString.Append(tagTimeStamps[tagIndex].ToString("HH:mm:ss.fff") + " ");
                        }
                        catch (Exception ex)
                        {
                            UpdateString.Append("Unknown Time ");
                        }
                        UpdateString.Append(tagNames[tagIndex] + " = ");

                        if (tagQualities[tagIndex]) // The Tag quality is good
                        {
                            // Store value to a hashtable so you can access it from your own routines
                            if (m_DataValuesHashtable.Contains(tagNames[tagIndex]))
                            {
                                m_DataValuesHashtable[tagNames[tagIndex]] = tagValues[tagIndex];
                            }
                            else
                            {
                                m_DataValuesHashtable.Add(tagNames[tagIndex], tagValues[tagIndex]);
                            }
                            try
                            {
                                if (object.ReferenceEquals(tagValues[tagIndex].GetType(), typeof(double)))
                                {
                                    ValueDouble = Convert.ToDouble(tagValues[tagIndex]);
                                    UpdateString.Append(ValueDouble.ToString());
                                }
                                else if (object.ReferenceEquals(tagValues[tagIndex].GetType(), typeof(int)))
                                {
                                    ValueInteger = Convert.ToInt32(tagValues[tagIndex]);
                                    UpdateString.Append(ValueInteger.ToString());
                                }
                                else if (object.ReferenceEquals(tagValues[tagIndex].GetType(), typeof(bool)))
                                {
                                    ValueBoolean = Convert.ToBoolean(tagValues[tagIndex]);
                                    UpdateString.Append(ValueBoolean.ToString());
                                }
                                else if (object.ReferenceEquals(tagValues[tagIndex].GetType(), typeof(string)))
                                {
                                    ValueString = tagValues[tagIndex].ToString();
                                    UpdateString.Append(ValueString);
                                }
                            }
                            catch (Exception ex)
                            {
                                UpdateString.Append("Error");
                            }
                        }
                        else // The Tag quality is bad
                        {
                            // Remove the value from the hashtable so you know the value is bad from your own routines
                            if (m_DataValuesHashtable.Contains(tagNames[tagIndex]))
                            {
                                m_DataValuesHashtable.Remove(tagNames[tagIndex]);
                            }
                            UpdateString.Append("Bad Quality");

                        }
                        UpdateString.Append("\r" + "\n");
                        try
                        {
                            //foreach(var i in tagListModel)
                            //{
                            //    Debug.WriteLine("TagListModel tagName: " + i.TagName);
                            //}

                            //_hubContext.Clients.All.SendAsync("updateTagValues", tagListModel);

                            Debug.WriteLine(UpdateString.ToString());
                            UpdateString.Remove(0, UpdateString.Length);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            m_InProcessValues = false;
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

        private void OasData_ValuesChangedAll(string[] Tags, object[] Values, bool[] Qualities, DateTime[] TimeStamps)
        {
            // High speed version with a lot of data values changing
            lock (m_DataValuesQueue.SyncRoot)
            {
                m_DataValuesQueue.Enqueue(new ClassTagValues(Tags, Values, Qualities, TimeStamps));
            }
            Debug.WriteLine($"Queue count: {m_DataValuesQueue.Count}");
        }
    }
}
