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
        public bool IsSoundOn { get; set; }
        public bool IsRecordingOn { get; set; }
        public List<string> Streams { get; set; }
    }
}
