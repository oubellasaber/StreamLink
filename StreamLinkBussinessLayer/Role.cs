using StreamLinkDataAccessLayer;
using System;
using System.Data;
using static StreamLinkBussinessLayer.Enums;

namespace StreamLinkBussinessLayer
{
    public class Role
    {

        public int RoleId { get; private set; }
        public string Name { get; set; }
        public Permissions Permissions { get; private set; }

        private Enums.ObjectState _roleState;

        public Role(string roleName, int permissions)
        {
            RoleId = -1;
            Name = roleName;
            Permissions = (Permissions)permissions;
            _roleState = Enums.ObjectState.Creating;
        }

        private Role(int roleId, string roleName, int permissions)
        {
            RoleId = roleId;
            Name = roleName;
            Permissions = (Permissions)permissions;
            _roleState = Enums.ObjectState.Editing;
        }

        private static bool HasPermission(Enums.Permissions permission)
        {
            Role role = RoleContext.CurrentRole;

            if (role == null)
            {
                throw new InvalidOperationException();
            }

            return role.Permissions.HasFlag(permission);
        }

        public static bool IsRoleAuthorized(Enums.Permissions permission)
        {
            try
            {
                if (!Role.HasPermission(permission))
                {
                    Console.WriteLine($"Your role does not have the '{permission.ToString()}' permission required to perform this action.");
                    return false;
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: No role is currently set in the RoleContext. {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred while checking permissions: {ex.Message}");
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            return $"RoleId: {RoleId}, Name: {Name}, Permissions: {Permissions}";
        }

        public static Role Get(int roleId)
        {
            string roleName = string.Empty;
            int permissions = 0;

            if (StreamLinkDataAccessLayer.RoleDA.Get(roleId, ref roleName, ref permissions))
            {
                return new Role(roleId, roleName, permissions);
            }

            return null;
        }

        public static DataTable GetAll()
        {
            return StreamLinkDataAccessLayer.RoleDA.GetAll();
        }

        private bool _Add()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.AddRole))
            {
                RoleId = StreamLinkDataAccessLayer.RoleDA.Add(Name, (int)Permissions);
                return (RoleId != -1);
            }

            return false;
        }

        private bool _Update()
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.UpdateRole))
            {
                return StreamLinkDataAccessLayer.RoleDA.Update(RoleId, Name, (int)Permissions);
            }

            return false;
        }

        public bool Save()
        {
            bool isSaved = false;

            switch (_roleState)
            {
                case Enums.ObjectState.Creating:
                    if (isSaved = this._Add())
                    {
                        _roleState = Enums.ObjectState.Editing;
                    }
                    break;

                case Enums.ObjectState.Editing:
                    isSaved = this._Update();
                    break;
            }

            return isSaved;
        }
        
        public static bool Delete(int roleId)
        {
            if (Role.IsRoleAuthorized(Enums.Permissions.DeleteRole))
            {
                return StreamLinkDataAccessLayer.RoleDA.Delete(roleId);
            }

            return false;
        }
    }
}