using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Enums
    {
        public enum ObjectState { Creating, Editing };

        [Flags]
        public enum Permissions
        {
            None = 0,

            // Host permissions
            AddHost = 1 << 0,              // 1
            UpdateHost = 1 << 1,           // 2
            DeleteHost = 1 << 2,           // 4
                                           
            // Link permissions            
            AddLink = 1 << 3,              // 8
            UpdateLink = 1 << 4,           // 16
            DeleteLink = 1 << 5,           // 32
                                           
            // Network permissions         
            AddNetwork = 1 << 6,            // 64
            UpdateNetwork = 1 << 7,         // 128
            DeleteNetwork = 1 << 8,         // 256
                                           
            // Role permissions            
            AddRole = 1 << 9,               // 512
            UpdateRole = 1 << 10,           // 1024
            DeleteRole = 1 << 11,           // 2048
                                           
            // Source permissions          
            AddSource = 1 << 12,            // 4096
            UpdateSource = 1 << 13,         // 8192
            DeleteSource = 1 << 14,         // 16384
                                           
            // Subtitles permissions       
            AddSubtitles = 1 << 15,         // 32768
            UpdateSubtitles = 1 << 16,      // 65536
            DeleteSubtitles = 1 << 17       // 131072
        }
    }
}
