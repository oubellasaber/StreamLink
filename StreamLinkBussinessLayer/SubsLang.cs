using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class SubsLang
    {
        private enum enSubsLangState { Creating, Editing }
        public int SubsLangId { get; private set; }
        public string Language { get; set; }

        private enSubsLangState SubsLangState;

        // add CRUD operations later if needed

        private SubsLang(int qualityId, string qualityName)
        {
            SubsLangId = qualityId;
            Language = qualityName;
            SubsLangState = enSubsLangState.Editing;
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
    }
}