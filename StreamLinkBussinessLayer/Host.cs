using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Host
    {
        private enum enHostState { Creating, Editing }
        public int HostId { get; private set; }
        public string HostName { get; set; }
        public string WebsiteLink { get; set; }

        private enHostState HostState;

        // add CRUD operations later if needed

        private Host(int hostId, string hostName, string websiteLink)
        {
            HostId = hostId;
            HostName = hostName;
            WebsiteLink = websiteLink;
            HostState = enHostState.Editing;
        }

        public override string ToString()
        {
            return $"HostId: {HostId}, HostName: {HostName}, WebsiteLink: {WebsiteLink}";
        }

        public static Host Get(int hostId)
        {
            string hostName = string.Empty;
            string websiteLink = string.Empty;

            if(StreamLinkDataAccessLayer.HostDA.Get(hostId, ref hostName, ref websiteLink))
            {
                return (new Host(hostId, hostName, websiteLink));
            }

            return null;
        }
    }
}
