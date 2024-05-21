using StreamLinkDataAccessLayer;
using System;
using System.Data;

namespace StreamLinkBussinessLayer
{
    public class Source
    {
        public int SourceId { get; private set; }
        public string SourceName { get; set; }
        public string SourceDescription { get; set; }

        private Enums.ObjectState _sourceState;

        public Source(string sourceName, string sourceDescription)
        {
            SourceId = -1;
            SourceName = sourceName;
            SourceDescription = sourceDescription;
            _sourceState = Enums.ObjectState.Creating;
        }

        private Source(int sourceId, string sourceName, string sourceDescription)
        {
            SourceId = sourceId;
            SourceName = sourceName;
            SourceDescription = sourceDescription;
            _sourceState = Enums.ObjectState.Editing;
        }

        public override string ToString()
        {
            return $"SourceId: {SourceId}, SourceName: {SourceName}, SourceDescription: {SourceDescription}";
        }

        public static Source Get(int sourceId)
        {
            string sourceName = string.Empty;
            string sourceDescription = string.Empty;

            if (StreamLinkDataAccessLayer.SourceDA.Get(sourceId, ref sourceName, ref sourceDescription))
            {
                return new Source(sourceId, sourceName, sourceDescription);
            }

            return null;
        }

        public static DataTable GetAll()
        {
            return StreamLinkDataAccessLayer.SourceDA.GetAll();
        }

        private bool _Add()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.AddSource))
            {
                SourceId = StreamLinkDataAccessLayer.SourceDA.Add(SourceName, SourceDescription);
                return (SourceId != -1);
            }

            return false;
        }

        private bool _Update()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.UpdateSource))
            {
                return StreamLinkDataAccessLayer.SourceDA.Update(SourceId, SourceName, SourceDescription);
            }

            return false;
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_sourceState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _sourceState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        public static bool Delete(int sourceId)
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteSource))
            {
                return StreamLinkDataAccessLayer.SourceDA.Delete(sourceId);
            }

            return false;
        }
    }
}