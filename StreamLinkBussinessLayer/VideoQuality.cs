using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class VideoQuality
    {
        private enum enQualityState { Creating, Editing }
        public int QualityId { get; private set; }
        public string QualityName { get; set; }

        private enQualityState QualityState;

        // add CRUD operations later if needed

        private VideoQuality(int qualityId, string qualityName)
        {
            QualityId = qualityId;
            QualityName = qualityName;
            QualityState = enQualityState.Editing;
        }

        public override string ToString()
        {
            return $"QualityId: {QualityId}, QualityName: {QualityName}";
        }

        public static VideoQuality Get(int qualityId)
        {
            string qualityName = string.Empty;

            if (StreamLinkDataAccessLayer.VideoQualityDA.Get(qualityId, ref qualityName))
            {
                return new VideoQuality(qualityId, qualityName);
            }

            return null;
        }
    }
}