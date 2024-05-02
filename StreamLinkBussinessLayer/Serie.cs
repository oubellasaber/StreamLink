using StreamLinkDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace StreamLinkBussinessLayer
{
    public class Serie
    {
        public enum enEpsLoadType { None, All, InRange }
        private enum enSerieState { Creating, Editing }
        public int SerieId { get; private set; }
        public string Title { get; set; }
        public int TotalEps { get; set; }
        public int UploadedEps { get; set; }
        public List<Ep> Eps { get; private set; }

        private enSerieState SerieState;
        public enEpsLoadType LoadType { get; private set; }

        public Serie(string title, int totalEps, int uploadedEps)
        {
            SerieId = -1;
            Title = title;
            TotalEps = totalEps;
            UploadedEps = uploadedEps;
            SerieState = enSerieState.Creating;
        }
        private Serie(int serieId, string title, int totalEps, int uploadedEps, enEpsLoadType loadType, int from, int to)
        {
            SerieId = serieId;
            Title = title;
            TotalEps = totalEps;
            UploadedEps = uploadedEps;
            SerieState = enSerieState.Editing;
            LoadType = loadType;
            Eps = LoadSerieEps(loadType, from, to);
        }

        public override string ToString()
        {
            return $"SerieId: {SerieId}, Title: {Title}, Total Episodes: {TotalEps}, Uploaded Episodes: {UploadedEps}, State: {SerieState}";
        }

        private List<Ep> LoadSerieEps(enEpsLoadType loadType, int from, int to)
        {
            List<Ep> epsList = null;
            switch(loadType)
            {
                case enEpsLoadType.None:
                    break;

                case enEpsLoadType.All:
                    epsList = Ep.GetInRange(SerieId, 1, UploadedEps);
                    break;

                case enEpsLoadType.InRange:
                    if(IsOverlaping(UploadedEps, ref from, ref to))
                    {
                        epsList = Ep.GetInRange(SerieId, from, to);
                    }
                    break;
            }

            return epsList;
        }

        private bool IsOverlaping(int rangeEnd1, ref int rangeStart2, ref int rangeEnd2)
        {
            rangeStart2 = Math.Max(1, rangeStart2);
            rangeEnd1 = Math.Min(rangeEnd1, rangeEnd2);

            return rangeStart2 <= rangeEnd2;
        }

        // need to be configured to handle getting only one episode
        public static Serie Get(int serieId, enEpsLoadType loadType = enEpsLoadType.None, int from = 1, int to = 20)
        {
            string title = string.Empty;
            int totalEps = 0;
            int uploadedEps = 0;

            if (StreamLinkDataAccessLayer.SerieDA.Get(serieId, ref title, ref totalEps, ref uploadedEps))
            {
                return (new Serie(serieId, title, totalEps, uploadedEps, loadType, from, to));
            }

            return null;
        }

        // TODO: Give (Create,  Update, Delete) permessions only to admins and mods
        private bool _Add()
        {
            SerieId = StreamLinkDataAccessLayer.SerieDA.Add(SerieId, Title, TotalEps, UploadedEps);

            return (SerieId != -1);
        }
        private bool _Update()
        {
            return StreamLinkDataAccessLayer.SerieDA.Update(SerieId, Title, TotalEps, UploadedEps);
        }
        public bool Save()
        {
            bool isSaved = false;

            switch(SerieState)
            {
                case enSerieState.Creating:
                    if (isSaved = this._Add())
                    {
                        SerieState = enSerieState.Editing;
                    }
                    break;

                case enSerieState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }
        public static bool Delete(int serieId)
        {
            return StreamLinkDataAccessLayer.SerieDA.Delete(serieId);
        }
    }
}
