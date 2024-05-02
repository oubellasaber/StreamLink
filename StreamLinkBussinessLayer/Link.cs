using StreamLinkDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Link
    {
        private enum LinkState { Creating, Editing }
        public int EpLinkId { get; private set; }
        public int EpId { get; set; }
        public Host LinkHost { get; set; }
        public VideoQuality LinkVideoQuality { get; set; }
        public string Url { get; set; }

        //private static LinkSubsMapper LinkSubtitlesMapper { get; set; } <-- performance enhancer, look at it later

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

        private Link(int epLinkId, int epId, int hostId, int qualityId, string url)
        {
            EpLinkId = epLinkId;
            EpId = epId;
            LinkHost = Host.Get(hostId);
            LinkVideoQuality = VideoQuality.Get(qualityId);
            Url = url;
            LinkSubtitles = Subtitles.GetAll(EpLinkId);
            linkState = LinkState.Editing;
        }

        public override string ToString()
        {
            return $"EpLinkId = {EpLinkId}, EpId = {EpId}, HostId = {LinkHost.HostId}, QualityId = {LinkVideoQuality.QualityId}, Url = {Url}";
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