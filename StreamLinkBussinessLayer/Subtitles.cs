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

        // seperate entity for this one
        public int SubsLangId { get; set; }
        public string Url { get; set; }

        private enSubtitlesState SubtitlesState;

        // add CRUD operations later if needed 

        private Subtitles(int subsId, int subsLangId, string url)
        {
            SubsId = subsId;
            SubsLangId = subsLangId;
            Url = url;
            SubtitlesState = enSubtitlesState.Editing;
        }

        public override string ToString()
        {
            return $"SubsId: {SubsId}, SubsLangId: {SubsLangId}, Url: {Url}";
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
