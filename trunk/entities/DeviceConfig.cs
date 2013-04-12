using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTML5Compiler
{
    public class DeviceConfig
    {
        public enum Devices
        {
            html5, bb, iOS, WindowsPhone, Android, html, mobile, responsive
        }

        [DataMemberAttribute]
        public Devices Device { get; set; }
        [DataMemberAttribute]
        public string Origin { get; set; }
        [DataMemberAttribute]
        public string Destiny { get; set; }
        [DataMemberAttribute]
        public string ModulesFolder { get; set; }
        [DataMemberAttribute]
        public string DefaultContainerFileName { get; set; }
        [DataMemberAttribute]
        public string DefaultBodyFileName { get; set; }
        [DataMemberAttribute]
        public string DefaultHeaderFileName { get; set; }
        [DataMemberAttribute]
        public string DefaultFooterFileName { get; set; }
        [DataMemberAttribute]
        public string ResourcesFolders { get; set; }
        [DataMemberAttribute]
        public bool MinifyJs { get; set; }
        [DataMemberAttribute]
        public bool MinifyCss { get; set; }
        [DataMemberAttribute]
        public bool CopyResources { get; set; }

        public DeviceConfig() { }
        
    }
}
