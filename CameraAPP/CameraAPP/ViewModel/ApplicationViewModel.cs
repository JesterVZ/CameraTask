using CameraAPP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CameraAPP.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public List<Camera> CameraList { get; set; }

        public ApplicationViewModel()
        {
            CameraList = new List<Camera>();
            for (int i = 0; i < 5; i++)
            {
                CameraList.Add(new Camera
                {
                    Name = "camera-" + i,
                    IsSoundOn = true,
                    IsRecordingOn = false
                });
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
