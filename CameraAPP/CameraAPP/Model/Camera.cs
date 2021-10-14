using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace CameraAPP.Model
{
    public class Camera
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Streams { get; set; }
        public ImageSource IsSoundOn { get; set; }
        public ImageSource IsRecordingOn { get; set; }
    }
}
