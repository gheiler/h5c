using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTML5Compiler
{
    [DataContractAttribute]
    public class Project
    {
        [DataMemberAttribute]
        public string Name { get; set; }
        [DataMemberAttribute]
        public List<DeviceConfig> Configs { get; set; }        

        public Project() { }
    }
}
