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
        // load all subs static right here, then when ever you create an insence of subs
        // get sus for it from thee list ( ref)

        private struct EpIdVideoNetwork
        {
            public int epId;
            public int networkId;
        }

        private static Dictionary<EpIdVideoNetwork, List<Subtitles>> SubtitlesSets = new Dictionary<EpIdVideoNetwork, List<Subtitles>>();
        private enum LinkState { Creating, Editing }
        public int EpLinkId { get; private set; }
        public int EpId { get; set; }
        public Host LinkHost { get; set; }
        public VideoQuality LinkVideoQuality { get; set; }
        public Network VideoNetwork { get; set; }
        public string Url { get; set; }
        public List<Subtitles> LinkSubtitles { get; set; }

        private LinkState linkState;

        public Link(int epId, int hostId, int qualityId, string url)
        {
            EpLinkId = -1;
            EpId = epId;
            LinkHost = Host.Get(hostId);
            LinkVideoQuality = VideoQuality.Get(qualityId);
            Url = url;
            linkState = LinkState.Creating;
        }

        private Link(int epLinkId, int networkId, int epId, int hostId, int qualityId, string url)
        {
            EpLinkId = epLinkId;
            EpId = epId;
            LinkHost = Host.Get(hostId);
            LinkVideoQuality = VideoQuality.Get(qualityId);
            VideoNetwork = Network.Get(networkId);
            Url = url;

            // try find, found get it from dictionnay, if not load it from DB
            EpIdVideoNetwork key = new EpIdVideoNetwork { epId = epId, networkId = networkId };

            if (SubtitlesSets.ContainsKey(key))
            {
                LinkSubtitles = SubtitlesSets[key];

                //if(object.ReferenceEquals(LinkSubtitles, SubtitlesSets[key]))
                //{
                //    Console.WriteLine("true");
                //}
            }
            else
            {
                LinkSubtitles = Subtitles.GetAll(EpLinkId);
                SubtitlesSets.Add(key, LinkSubtitles);
            }

            linkState = LinkState.Editing;
        }

        public override string ToString()
        {
            return $"EpLinkId = {EpLinkId}, NetworkName = {VideoNetwork.NetworkName}, EpId = {EpId}, Host = {LinkHost.HostName}, Quality = {LinkVideoQuality.QualityName}, URL = {Url}";
        }

        // Add a get one link one link when you figure it out

        // To Do: when updating the link how should we deal with the LinkHost entity

        private static List<Link> ConvertToLinksList(List<object> anonymousLinks)
        {
            List<Link> linksList = new List<Link>();

            foreach (var anonymousLink in anonymousLinks)
            {
                linksList.Add(new Link(
                    (int)anonymousLink.GetType().GetProperty("EpLinkId").GetValue(anonymousLink),
                    (int)anonymousLink.GetType().GetProperty("NetworkId").GetValue(anonymousLink),
                    (int)anonymousLink.GetType().GetProperty("EpId").GetValue(anonymousLink),
                    (int)anonymousLink.GetType().GetProperty("HostId").GetValue(anonymousLink),
                    (int)anonymousLink.GetType().GetProperty("QualityId").GetValue(anonymousLink),
                    (string)anonymousLink.GetType().GetProperty("Url").GetValue(anonymousLink)
                    ));
            }

            return linksList;
        }

        public static List<Link> GetAll(int epId)
        {
            return ConvertToLinksList(StreamLinkDataAccessLayer.LinkDA.GetAll(epId));
        }

        // Contributors can request a contributor badge
        // Admins grant the badge if the contributor has a positive impact on the community
        // Ensure that contributors are warned if they misuse this feature
        // Block contributors if they misuse this feature more than 3 times
        // Admins and mods should review links added by contributors before they are published.
        private bool _Add()
        {
            EpLinkId = StreamLinkDataAccessLayer.LinkDA.Add(EpId, LinkHost.HostId, LinkVideoQuality.QualityId, Url);

            return (EpLinkId != -1);
        }

        // Update link if Link is broken
        private bool _Update()
        {
            return StreamLinkDataAccessLayer.LinkDA.Update(EpLinkId, EpId, LinkHost.HostId, LinkVideoQuality.QualityId, Url);
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (linkState)
            {
                case LinkState.Creating:
                    if (isSaved = this._Add())
                    {
                        linkState = LinkState.Editing;
                    }
                    break;

                case LinkState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        // Delete if link is broken
        public static bool Delete(int epLinkId)
        {
            return StreamLinkDataAccessLayer.LinkDA.Delete(epLinkId);
        }
    }
}