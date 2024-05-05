using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Network
    {
        private enum enNetworkState { Creating, Editing }
        public int NetworkId { get; private set; }
        public string NetworkName { get; set; }
        public string WebsiteLink { get; set; }

        private enNetworkState NetworkState;

        // add CRUD operations later if needed

        private Network(int networkId, string networkName, string websiteLink)
        {
            NetworkId = networkId;
            NetworkName = networkName;
            WebsiteLink = websiteLink;
            NetworkState = enNetworkState.Editing;
        }

        public override string ToString()
        {
            return $"NetworkId: {NetworkId}, NetworkName: {NetworkName}, WebsiteLink: {WebsiteLink}";
        }

        public static Network Get(int networkId)
        {
            string networkName = string.Empty;
            string websiteLink = string.Empty;

            if (StreamLinkDataAccessLayer.NetworkDA.Get(networkId, ref networkName, ref websiteLink))
            {
                return (new Network(networkId, networkName, websiteLink));
            }

            return null;
        }
    }
}
