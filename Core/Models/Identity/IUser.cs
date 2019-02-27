using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public interface IUsers
    {
        #region Constructor


        #endregion
    }

    public interface IUser
    {
        event EventHandler IdentServerChanged;

        #region Properties
        Guid? ID { get; set; }

        string UserAuthName { get; set; }

        string UserScreenName { get; set; }

        string UserToken { get; set; }
        IDictionary<string, AccessType> UserAuthorizations { get; set; }

        DateTime? LastLoginDate { get; set; }
        DateTime? CurrentLoginDate { get; set; }

        bool IsAuthenticated { get; set; }

        bool IsLockedOut { get; set; }
        string IdentServer { get; set; }
        #endregion

        #region Methods

      

        #endregion

    }
#region Enums

public enum AccessType
{
    None
  , Read
  , Admin

}
    #endregion
}
