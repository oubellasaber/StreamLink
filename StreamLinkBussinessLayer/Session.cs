using OpenQA.Selenium;
using StreamLinkBussinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLinkBussinessLayer
{
    public class Session
    {
        public string SessionId { get; private set; }
        public int UserId { get; set; }

        private Enums.ObjectState _sessionState;


        public Session(int userId)
        {
            _sessionState = Enums.ObjectState.Creating;
            UserId = userId;
        }

        private Session(string sessionId, int userId)
        {
            _sessionState = Enums.ObjectState.Editing;
            SessionId = sessionId;
            UserId = userId;
        }

        public override string ToString()
        {
            return $"SessionId: {SessionId}, UserId: {UserId}";
        }

        public static Session Get(string sessionId)
        {
            int userId = 0;

            if (StreamLinkDataAccessLayer.SessionDA.Get(sessionId, ref userId))
            {
                return new Session(sessionId, userId);
            }

            return null;
        }

        private bool _Add()
        {
            SessionId = StreamLinkDataAccessLayer.SessionDA.Add(UserId);
            return (SessionId != String.Empty);
        }

        private bool _Update()
        {
            return false; // StreamLinkDataAccessLayer.SessionDA.Update(UserId, SessionId);
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_sessionState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _sessionState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        public static bool Delete(string sessionId)
        {
            return false; // StreamLinkDataAccessLayer.SessionDA.Delete(sessionId);
        }
    }
}
