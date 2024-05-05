using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Subtitles
    {
        private enum enSubtitlesState { Creating, Editing }
        public int SubsId { get; private set; }
        public SubsLang SubsLanguage { get; set; }
        public string Url { get; set; }

        private enSubtitlesState SubtitlesState;

        // add CRUD operations later if needed 

        private Subtitles(int subsId, int subsLangId, string url)
        {
            SubsId = subsId;
            SubsLanguage = SubsLang.Get(subsLangId);
            Url = url;
            SubtitlesState = enSubtitlesState.Editing;
        }

        public override string ToString()
        {
            return $"SubsId: {SubsId}, Language: {SubsLanguage.Language}, Url: {Url}";
        }

        private static List<Subtitles> ConvertToSubtitlesList(List<object> anonymousSubtitles)
        {
            List<Subtitles> subtitlesList = new List<Subtitles>();

            foreach (var anonymousSubtitles1 in anonymousSubtitles)
            {
                subtitlesList.Add(new Subtitles(
                    (int)anonymousSubtitles1.GetType().GetProperty("SubsId").GetValue(anonymousSubtitles1),
                    (int)anonymousSubtitles1.GetType().GetProperty("SubsLangId").GetValue(anonymousSubtitles1),
                    (string)anonymousSubtitles1.GetType().GetProperty("Url").GetValue(anonymousSubtitles1)
                    ));
            }

            return subtitlesList;
        }

        public static List<Subtitles> GetAll(int epLinkId)
        {
            return ConvertToSubtitlesList(StreamLinkDataAccessLayer.SubtitlesDA.GetAll(epLinkId));
        }



    }
}
