using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StreamLinkBussinessLayer
{
    public class SubsLang
    {
        public int SubsLangId { get; private set; }
        public string Language { get; set; }

        private Enums.ObjectState _subsLangState;

        public SubsLang(string language)
        {
            SubsLangId = -1;
            Language = language;
            _subsLangState = Enums.ObjectState.Creating;
        }

        private SubsLang(int subsLangId, string language)
        {
            SubsLangId = subsLangId;
            Language = language;
            _subsLangState = Enums.ObjectState.Editing;
        }

        public override string ToString()
        {
            return $"SubsLangId: {SubsLangId}, Language: {Language}";
        }

        public static SubsLang Get(int subsLangId)
        {
            string language = string.Empty;
            
            if (StreamLinkDataAccessLayer.SubsLangDA.Get(subsLangId, ref language))
            {
                return new SubsLang(subsLangId, language);
            }

            return null;
        }

        public static List<SubsLang> GetAll()
        {
            DataTable dt = StreamLinkDataAccessLayer.SubsLangDA.GetAll();

            List<SubsLang> subsLangs = dt.AsEnumerable()
                              .Select(row => new SubsLang(
                                  row.Field<int>("SubsLangId"),
                                  row.Field<string>("Language")

                              ))
                              .ToList();
            return subsLangs;
        }

        private bool _Add()
        {
            SubsLangId = StreamLinkDataAccessLayer.SubsLangDA.Add(Language);
            return (SubsLangId != -1);
        }

        private bool _Update()
        {
            return StreamLinkDataAccessLayer.SubsLangDA.Update(SubsLangId, Language);
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_subsLangState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _subsLangState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        public static bool Delete(int subsLangId)
        {
            return StreamLinkDataAccessLayer.SubsLangDA.Delete(subsLangId);
        }
    }
}