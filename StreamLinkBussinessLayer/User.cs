using StreamLinkDataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace StreamLinkBussinessLayer
{
    public class User
    {
        public int UserId { get; private set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Role Role { get; private set; }
        private Enums.ObjectState _userState;


        protected User(string username, string password, int roleId)
        {
            _userState = Enums.ObjectState.Creating;
            Username = username;
            Password = password;
            Role = Role.Get(roleId);
        }

        private User(int userId, string username, string password, int roleId)
        {
            _userState = Enums.ObjectState.Editing;
            UserId = userId;
            Username = username;
            Password = password;
            Role = Role.Get(roleId);
            RoleContext.CurrentRole = Role;
        }

        public override string ToString()
        {
            return $"UserId: {UserId}, Username: {Username}, Password: {Password}, Role: {Role.Name}";
        }

        // Static method to get a user by its ID
        public static User Get(int userId)
        {
            string username = string.Empty;
            string password = string.Empty;
            int roleId= 0;

            if (StreamLinkDataAccessLayer.UserDA.Get(userId, ref username, ref password, ref roleId))
            {
                return new User(userId, username, password, roleId);
            }

            return null;
        }

        public static User IsCorrectCredentials(string username, string password)
        {
            int userId = 0;
            string hashedPassword = string.Empty;
            int roleId= 0;

            if (StreamLinkDataAccessLayer.UserDA.Get(ref userId, username, ref hashedPassword, ref roleId))
            {
                if(BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                {
                    return new User(userId, username, hashedPassword, roleId);
                }
                else
                {
                    return null;
                }
            }

            return null;
        }

        // Private method to add a new user
        private bool _Add()
        {
            UserId = StreamLinkDataAccessLayer.UserDA.Add(Username, BCrypt.Net.BCrypt.HashPassword(Password), Role.RoleId);
            return (UserId != -1);
        }

        // Private method to update an existing user
        private bool _Update()
        {
            return StreamLinkDataAccessLayer.UserDA.Update(UserId, Username, BCrypt.Net.BCrypt.HashPassword(Password), Role.RoleId);
        }

        // Public method to save the user (add or update based on state)
        public bool Save()
        {
            bool isSaved = false;

            switch (_userState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _userState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }

        // Delete method for leter
    }
}
