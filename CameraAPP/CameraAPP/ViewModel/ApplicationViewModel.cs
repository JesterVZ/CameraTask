using CameraAPP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace CameraAPP.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Camera> CameraList { get; set; }

        public ApplicationViewModel()
        {
            CameraList = new ObservableCollection<Camera>();
            Task.Factory.StartNew(GetXmlDataFromServer);
        }

        private async Task GetXmlDataFromServer()
        {

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://demo.macroscop.com/configex?login=root"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/xml");
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                HttpContent responseContent = response.Content;
                var xmlStr = await responseContent.ReadAsStringAsync();
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(xmlStr);
                XmlElement root = xmlData.DocumentElement;
                XmlNode xmlNode = root.SelectSingleNode("Channels");
                if (xmlNode.HasChildNodes)
                {
                    for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
                    {
                        XmlAttributeCollection thisAttr = xmlNode.ChildNodes[i].Attributes;


                        CameraList.Add(new Camera
                        {
                            Id = thisAttr[0].Value,
                            Name = thisAttr[1].Value,
                            IsSoundOn = Convert.ToBoolean(thisAttr[6].Value),

                        });
                    }
                }
                await GetStreamsDataFromServer();

            }

        }
        
        private async Task GetStreamsDataFromServer()
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://demo.macroscop.com/command?type=getchannelsstates&login=root"),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/xml");
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                HttpContent responseContent = response.Content;
                var xmlStr = await responseContent.ReadAsStringAsync();
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(xmlStr);
                XmlElement root = xmlData.DocumentElement;
                XmlNodeList xmlNodes = root.SelectNodes("ChannelState");
                for(int i = 0; i < xmlNodes.Count; i++)
                {
                    CameraList[i].Streams = new List<string>();
                    int index = IsCameraIdRight(xmlNodes[i].ChildNodes[0].InnerText);
                    if (index != -1)
                    {
                        CameraList[i].IsRecordingOn = Convert.ToBoolean(xmlNodes[i].ChildNodes[1].InnerText);
                        XmlNode streamsStates = xmlNodes[i].ChildNodes[2];
                        if (streamsStates.HasChildNodes)
                        {
                            XmlNodeList streams = streamsStates.SelectNodes("Stream");
                            for(int j = 0; j < streams.Count; j++)
                            {
                                if(streams[j].ChildNodes[1].InnerText == "Active")
                                {
                                    CameraList[i].Streams.Add(streams[j].ChildNodes[0].InnerText);
                                }
                            }
                        }
                    }
                }
            }
        }

        private int IsCameraIdRight(string id)
        {
            for(int i = 0; i < CameraList.Count; i++)
            {
                if(CameraList[i].Id == id)
                {
                    return i;
                }
            }
            return -1;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
