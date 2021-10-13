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


        public async Task GetXmlDataFromServer()
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
                        bool isRecording = false;
                        for(int j = 0; j < thisAttr.Count; j++)
                        {
                            if(thisAttr[j].Name == "IsRecordingOn")
                            {
                                isRecording = Convert.ToBoolean(thisAttr[j].Value);
                            }
                        }
                        CameraList.Add(new Camera
                        {
                            Name = thisAttr[1].Value,
                            IsSoundOn = Convert.ToBoolean(thisAttr[6].Value),
                            IsRecordingOn = isRecording

                        });
                    }
                }
                return;

            }

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
