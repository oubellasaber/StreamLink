using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class VideoQuality
    {
        public int QualityId { get; private set; }
        public string QualityName { get; set; }

        private Enums.ObjectState _qualityState;

        public VideoQuality(string qualityName)
        {
            QualityId = -1;
            QualityName = qualityName;
            _qualityState = Enums.ObjectState.Creating;
        }

        private VideoQuality(int qualityId, string qualityName)
        {
            QualityId = qualityId;
            QualityName = qualityName;
            _qualityState = Enums.ObjectState.Editing;
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

        public static DataTable GetAll()
        {
            return StreamLinkDataAccessLayer.VideoQualityDA.GetAll();
        }

        private bool _Add()
        {
            QualityId = StreamLinkDataAccessLayer.VideoQualityDA.Add(QualityName);
            return (QualityId != -1);
        }

        private bool _Update()
        {
            return StreamLinkDataAccessLayer.VideoQualityDA.Update(QualityId, QualityName);

        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_qualityState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _qualityState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        public static bool Delete(int qualityId)
        {
            return StreamLinkDataAccessLayer.VideoQualityDA.Delete(qualityId);
        }
    }
}