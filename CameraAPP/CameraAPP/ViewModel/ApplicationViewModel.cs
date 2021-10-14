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

        private async Task<HttpResponseMessage> CheckHttpResponseMessage(string url)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("Accept", "application/xml");
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }

        private async Task GetXmlDataFromServer()
        {
            HttpResponseMessage http = await CheckHttpResponseMessage("http://demo.macroscop.com/configex?login=root");

            if (http.StatusCode == HttpStatusCode.OK)
            {
                HttpContent responseContent = http.Content;
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
                        if (Convert.ToBoolean(thisAttr[6].Value))
                        {
                            CameraList.Add(new Camera
                            {
                                Id = thisAttr[0].Value,
                                Name = thisAttr[1].Value,
                                IsSoundOn = "daw.png"

                            });
                        } else
                        {
                            CameraList.Add(new Camera
                            {
                                Id = thisAttr[0].Value,
                                Name = thisAttr[1].Value,
                                IsSoundOn = "disable.png"

                            });
                        }

                    }
                }
                await GetStreamsDataFromServer();

            }

        }
        
        private async Task GetStreamsDataFromServer()
        {
            HttpResponseMessage http = await CheckHttpResponseMessage("http://demo.macroscop.com/command?type=getchannelsstates&login=root");
            
            if (http.StatusCode == HttpStatusCode.OK)
            {
                HttpContent responseContent = http.Content;
                var xmlStr = await responseContent.ReadAsStringAsync();
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(xmlStr);
                XmlElement root = xmlData.DocumentElement;
                XmlNodeList xmlNodes = root.SelectNodes("ChannelState");
                for(int i = 0; i < xmlNodes.Count; i++)
                {
                    int index = IsCameraIdRight(xmlNodes[i].ChildNodes[0].InnerText);
                    if (index != -1)
                    {
                        if (Convert.ToBoolean(xmlNodes[i].ChildNodes[1].InnerText))
                        {
                            CameraList[i].IsRecordingOn = "daw.png";
                        } else
                        {
                            CameraList[i].IsRecordingOn = "disable.png";
                        }
                        XmlNode streamsStates = xmlNodes[i].ChildNodes[2];
                        if (streamsStates.HasChildNodes)
                        {
                            string streamsList = "";
                            XmlNodeList streams = streamsStates.SelectNodes("Stream");
                            for(int j = 0; j < streams.Count; j++)
                            {
                                if(streams[j].ChildNodes[1].InnerText == "Active")
                                {
                                    streamsList += streams[j].ChildNodes[0].InnerText + "\n";
                                }
                            }
                            CameraList[i].Streams = streamsList;
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
