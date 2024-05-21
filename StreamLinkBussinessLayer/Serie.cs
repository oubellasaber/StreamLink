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
        public enum EpsLoadType { None, All, InRange }
        public int SerieId { get; private set; }    
        public string Title { get; set; }
        public int TotalEps { get; set; }
        public int UploadedEps { get; set; }
        public List<Ep> Eps { get; private set; }
        public EpsLoadType LoadType { get; private set; }
        private Enums.ObjectState _serieState;

        public Serie(string title, int totalEps, int uploadedEps)
        {
            SerieId = -1;
            Title = title;
            TotalEps = totalEps;
            UploadedEps = uploadedEps;
            _serieState = Enums.ObjectState.Creating;
        }

        private Serie(int serieId, string title, int totalEps, int uploadedEps, EpsLoadType loadType, int from, int to)
        {
            SerieId = serieId;
            Title = title;
            TotalEps = totalEps;
            UploadedEps = uploadedEps;
            _serieState = Enums.ObjectState.Editing;
            LoadType = loadType;
            Eps = LoadSerieEps(loadType, from, to);
        }

        public override string ToString()
        {
            return $"SerieId: {SerieId}, Title: {Title}, Total Episodes: {TotalEps}, Uploaded Episodes: {UploadedEps}";
        }

        private List<Ep> LoadSerieEps(EpsLoadType loadType, int from, int to)
        {
            List<Ep> epsList = null;
            switch(loadType)
            {
                case EpsLoadType.None:
                    break;

                case EpsLoadType.All:
                    epsList = Ep.GetInRange(SerieId, 1, UploadedEps);
                    break;

                case EpsLoadType.InRange:
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
        public static Serie Get(int serieId, EpsLoadType loadType = EpsLoadType.None, int from = 1, int to = 20)
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
        
        private bool _Add()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteRole))
            {
                SerieId = StreamLinkDataAccessLayer.SerieDA.Add(SerieId, Title, TotalEps, UploadedEps);

                return (SerieId != -1);
            }

            return false;
        }
        
        private bool _Update()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteRole))
            {
                return StreamLinkDataAccessLayer.SerieDA.Update(SerieId, Title, TotalEps, UploadedEps);
            }

            return false;
        }
        
        public bool Save()
        {
            bool isSaved = false;

            switch(_serieState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _serieState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }
        
        public static bool Delete(int serieId)
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteRole))
            {
                return StreamLinkDataAccessLayer.SerieDA.Delete(serieId);
            }

            return false;
        }
    }
}
