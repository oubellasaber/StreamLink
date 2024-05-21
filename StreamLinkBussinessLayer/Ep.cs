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
        public enum LinksLoadType { None, All }
        public int EpId { get; private set; }
        public int EpNum { get; set; }
        public int SerieId { get; set; }
        public List<Link> Links { get; private set; }
        public LinksLoadType LoadType { get; private set; }
        private Enums.ObjectState _epState;

        public Ep(int epNum, int serieId)
        {
            EpId = -1;
            EpNum = epNum;
            SerieId = serieId;
            _epState = Enums.ObjectState.Creating;
        }
        
        private Ep(int epId, int epNum, int serieId, LinksLoadType loadType = LinksLoadType.All)
        {
            EpId = epId;
            EpNum = epNum;
            SerieId = serieId;
            _epState = Enums.ObjectState.Editing;
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
        private List<Link> LoadEpLinks(LinksLoadType loadType)
        {
            List<Link> linksList = null;
            switch (loadType)
            {
                case LinksLoadType.None:
                    break;

                case LinksLoadType.All:
                    // Add parameters when you get how eps works
                    linksList = Link.GetAll(EpId);
                    break;
            }

            return linksList;
        }

        public static List<Ep> GetInRange(int serieId, int from, int to)
        {
            DataTable dt = StreamLinkDataAccessLayer.EpDA.GetInRange(serieId, from, to);

            List<Ep> eps = dt.AsEnumerable()
                              .Select(row => new Ep(
                                  row.Field<int>("EpId"),
                                  row.Field<int>("EpNum"),
                                  row.Field<int>("SerieId")
                              ))
                              .ToList();
            return eps;
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

            switch (_epState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _epState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
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