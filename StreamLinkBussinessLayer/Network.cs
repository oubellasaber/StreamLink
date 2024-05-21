using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Network
    {
        public int NetworkId { get; private set; }
        public string NetworkName { get; set; }
        public string WebsiteLink { get; set; }

        private Enums.ObjectState _networkState;

        private Network(int networkId, string networkName, string websiteLink)
        {
            NetworkId = networkId;
            NetworkName = networkName;
            WebsiteLink = websiteLink;
            _networkState = Enums.ObjectState.Editing;
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

        public static List<Network> GetAll(int networkId)
        {
            DataTable dt = StreamLinkDataAccessLayer.NetworkDA.GetAll(networkId);

            List<Network> networks = dt.AsEnumerable()
                              .Select(row => new Network(
                                  row.Field<int>("NetworkId"),
                                  row.Field<string>("NetworkName"),
                                  row.Field<string>("WebsiteLink")
                              ))
                              .ToList();
            return networks;
        }

        private bool _Add()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.AddNetwork))
            {
                NetworkId = StreamLinkDataAccessLayer.NetworkDA.Add(NetworkName, WebsiteLink);
                return (NetworkId != -1);
            }

            return false;
        }

        private bool _Update()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.UpdateNetwork))
            {
                return StreamLinkDataAccessLayer.NetworkDA.Update(NetworkId, NetworkName, WebsiteLink);
            }

            return false;
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_networkState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _networkState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }
        public static bool Delete(int networkId)
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteNetwork))
            {
                return StreamLinkDataAccessLayer.NetworkDA.Delete(networkId);
            }

            return false;
        }
    }
}
