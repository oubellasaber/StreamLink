using OpenQA.Selenium.DevTools;
using StreamLinkDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: you have to solve the problem when a certain list of subtitles
// is shared between two links (we should not load the same list twice,
// we should instead reference the existing one)

namespace StreamLinkBussinessLayer
{
    public class Link
    {
        private struct EpIdNetworkId
        {
            public int epId;
            public int networkId;
        }

        // To not Load the same subs more than twice
        private static Dictionary<EpIdNetworkId, List<Subtitles>> SubtitlesSets = new Dictionary<EpIdNetworkId, List<Subtitles>>();
        public int EpLinkId { get; private set; }
        public int EpId { get; private set; }
        public Network Network { get; set; }
        public Source Source { get; set; }
        public Host Host { get; set; }
        public VideoQuality VideoQuality { get; set; }
        public string Url { get; set; }
        public List<Subtitles> Subtitles { get; set; }

        private Enums.ObjectState _linkState;

        public Link(int epId, int networkId, int sourceId, int hostId, int qualityId, string url)
        {
            EpLinkId = -1;
            EpId = epId;
            Network = Network.Get(networkId);
            Source = Source.Get(sourceId);
            Host = Host.Get(hostId);
            VideoQuality = VideoQuality.Get(qualityId);
            Url = url;
            _linkState = Enums.ObjectState.Creating;
        }

        private Link(int epLinkId, int epId, int networkId, int sourceId, int hostId, int qualityId, string url)
        {
            EpLinkId = epLinkId;
            EpId = epId;
            Network = Network.Get(networkId);
            Source = Source.Get(sourceId);
            Host = Host.Get(hostId);
            VideoQuality = VideoQuality.Get(qualityId);
            Url = url;

            // try find, found get it from dictionnay, if not load it from DB
            EpIdNetworkId key = new EpIdNetworkId { epId = epId, networkId = networkId };

            if (SubtitlesSets.ContainsKey(key))
            {
                Subtitles = SubtitlesSets[key];

                //if(object.ReferenceEquals(Subtitles, SubtitlesSets[key]))
                //{
                //    Console.WriteLine("true");
                //}
            }
            else
            {
                Subtitles = StreamLinkBussinessLayer.Subtitles.GetAll(EpLinkId);
                SubtitlesSets.Add(key, Subtitles);
            }

            _linkState = Enums.ObjectState.Editing;
        }

        public override string ToString()
        {
            return $"EpLinkId = {EpLinkId}, NetworkName = {Network.NetworkName}, EpId = {EpId}, Host = {Host.HostName}, Quality = {VideoQuality.QualityName}, URL = {Url}";
        }

        // Add a get one link one link when you figure it out

        // To Do: when updating the link how should we deal with the Host entity
        public static List<Link> GetAll(int epId)
        {
            DataTable dt = StreamLinkDataAccessLayer.LinkDA.GetAll(epId);

            List<Link> eps = dt.AsEnumerable()
                              .Select(row => new Link(
                                  row.Field<int>("EpLinkId"),
                                  row.Field<int>("EpId"),
                                  row.Field<int>("NetworkId"),
                                  row.Field<int>("SourceId"),
                                  row.Field<int>("HostId"),
                                  row.Field<int>("QualityId"),
                                  row.Field<string>("Url")

                              ))
                              .ToList();
            return eps;
        }

        private bool _Add()
        {
            if(Role.IsRoleAuthorized(Enums.Permissions.AddLink))
            {
                EpLinkId = StreamLinkDataAccessLayer.LinkDA.Add(EpId, Network.NetworkId, Source.SourceId, Host.HostId, VideoQuality.QualityId, Url);
                return (EpLinkId != -1);
            }

            return false;
        }

        private bool _Update()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.UpdateLink))
            {
                return StreamLinkDataAccessLayer.LinkDA.Update(EpLinkId, EpId, Network.NetworkId, Source.SourceId, Host.HostId, VideoQuality.QualityId, Url);
            }

            return false;
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_linkState)
            { 
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _linkState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        public static bool Delete(int epLinkId)
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteLink))
            {
                return StreamLinkDataAccessLayer.LinkDA.Delete(epLinkId);
            }

            return false;
        }
    }
}