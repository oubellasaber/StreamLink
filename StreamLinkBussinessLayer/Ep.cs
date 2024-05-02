using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Ep
    {
        public enum enLinksLoadType { None, All }
        private enum enEpState { Creating, Editing }
        public int EpId { get; private set; }
        public int EpNum { get; set; }
        public int SerieId { get; set; }
        public List<Link> Links { get; private set; }

        private enEpState EpState;
        public enLinksLoadType LoadType { get; private set; }

        public Ep(int epNum, int serieId)
        {
            EpId = -1;
            EpNum = epNum;
            SerieId = serieId;
            EpState = enEpState.Creating;
        }

        private Ep(int epId, int epNum, int serieId, enLinksLoadType loadType = enLinksLoadType.All)
        {
            EpId = epId;
            EpNum = epNum;
            SerieId = serieId;
            EpState = enEpState.Editing;
            LoadType = loadType;
            Links = LoadEpLinks(loadType);
        }

        public static Ep Get(int epId)
        {
            int epNum = 0;
            int serieId = 0;

            if (StreamLinkDataAccessLayer.EpDA.Get(epId, ref epNum, ref serieId))
            {
                return (new Ep(epId, epNum, serieId));
            }

            return null;
        }

        public override string ToString()
        {
            return $"EpId: {EpId}, EpNum: {EpNum}, SerieId: {SerieId}";
        }


        private List<Link> LoadEpLinks(enLinksLoadType loadType)
        {
            List<Link> linksList = null;
            switch (loadType)
            {
                case enLinksLoadType.None:
                    break;

                case enLinksLoadType.All:
                    // Add parameters when you get how eps works
                    linksList = Link.GetAll(EpId);
                    break;
            }

            return linksList;
        }


        // full links, partial links
        private static List<Ep> ConvertToEpsList(List<object> anonymousEps)
        {
            List<Ep> epsList = new List<Ep>();

            foreach (var anonymousEp in anonymousEps)
            {
                epsList.Add(new Ep(
                    (int)anonymousEp.GetType().GetProperty("EpId").GetValue(anonymousEp),
                    (int)anonymousEp.GetType().GetProperty("EpNum").GetValue(anonymousEp),
                    (int)anonymousEp.GetType().GetProperty("SerieId").GetValue(anonymousEp)
                    ));
            }

            return epsList;
        }

        public static List<Ep> GetInRange(int serieId, int from, int to)
        {
            return ConvertToEpsList(StreamLinkDataAccessLayer.EpDA.GetInRange(serieId, from, to));
        }

        // TODO: Give (Create,  Update, Delete) permessions only to admins and mods
        private bool _Add()
        {
            EpId = StreamLinkDataAccessLayer.EpDA.Add(EpNum, SerieId);

            return (EpId != -1);
        }

        private bool _Update()
        {
            return StreamLinkDataAccessLayer.EpDA.Update(EpId, EpNum, SerieId);
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (EpState)
            {
                case enEpState.Creating:
                    if (isSaved = this._Add())
                    {
                        EpState = enEpState.Editing;
                    }
                    break;

                case enEpState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        public static bool Delete(int epId)
        {
            return StreamLinkDataAccessLayer.EpDA.Delete(epId);
        }
    }
}