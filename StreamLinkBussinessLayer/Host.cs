using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Host
    {
        public int HostId { get; private set; }
        public string HostName { get; set; }
        public string WebsiteLink { get; set; }

        private Enums.ObjectState _hostState;

        // add CRUD operations later if needed

        public Host(string hostName, string websiteLink)
        {
            HostId = -1;
            HostName = hostName;
            WebsiteLink = websiteLink;
            _hostState = Enums.ObjectState.Creating;
        }
        private Host(int hostId, string hostName, string websiteLink)
        {
            HostId = hostId;
            HostName = hostName;
            WebsiteLink = websiteLink;
            _hostState = Enums.ObjectState.Editing;
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

        public static DataTable GetAll()
        {
            return StreamLinkDataAccessLayer.HostDA.GetAll();
        }

        private bool _Add()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.AddHost))
            {
                HostId = StreamLinkDataAccessLayer.HostDA.Add(HostName, WebsiteLink);
                return (HostId != -1);
            }

            return false;
        }

        // hosts have t go  into check in, later
        private bool _Update()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.UpdateHost))
            {
                return StreamLinkDataAccessLayer.HostDA.Update(HostId, HostName, WebsiteLink);

            }

            return false;
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_hostState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _hostState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        public static bool Delete(int hostId)
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteHost))
            {
                return StreamLinkDataAccessLayer.HostDA.Delete(hostId);
            }

            return false;
        }
    }
}
