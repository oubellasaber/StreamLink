using StreamLinkDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StreamLinkBussinessLayer.Enums;
using System.Xml.Linq;

namespace StreamLinkBussinessLayer
{
    public class Subtitles
    {
        public int SubsId { get; private set; }
        public SubsLang SubsLanguage { get; set; }
        public string Url { get; set; }
        private Enums.ObjectState _subsState;

        private Subtitles(int subsId, int subsLangId, string url)
        {
            SubsId = subsId;
            SubsLanguage = SubsLang.Get(subsLangId);
            Url = url;
            _subsState = Enums.ObjectState.Editing;
        }

        public override string ToString()
        {
            return $"SubsId: {SubsId}, Language: {SubsLanguage.Language}, Url: {Url}";
        }

        public static List<Subtitles> GetAll(int epLinkId)
        {
            DataTable dt = StreamLinkDataAccessLayer.SubtitlesDA.GetAll(epLinkId);

            List<Subtitles> subs = dt.AsEnumerable()
                              .Select(row => new Subtitles(
                                  row.Field<int>("SubsId"),
                                  row.Field<int>("SubsLangId"),
                                  row.Field<string>("Url")
                              ))
                              .ToList();
            return subs;
        }
        private bool _Add()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.AddSubtitles))
            {
                SubsId = StreamLinkDataAccessLayer.SubtitlesDA.Add(SubsLanguage.SubsLangId, Url);
                return (SubsId != -1);
            }

            return false;
        }

        private bool _Update()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.UpdateSubtitles))
            {
                return StreamLinkDataAccessLayer.SubtitlesDA.Update(SubsId, SubsLanguage.SubsLangId, Url);
            }

            return false;
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_subsState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _subsState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        public static bool Delete(int subsId)
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteRole))
            {
                return StreamLinkDataAccessLayer.SubtitlesDA.Delete(subsId);
            }

            return false;
        }
    }
}