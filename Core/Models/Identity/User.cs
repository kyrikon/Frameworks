using Core.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
//using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Users : IUsers
    {

        #region Constructor


        #endregion

    }

    public class User : NotifyPropertyChanged , IUser
    {
        public event EventHandler IdentServerChanged;

        #region Constructor
        public User()
        {
            UserAuthorizations = new Dictionary<string, AccessType>();
        }
        #endregion

        #region Properties
        public Guid? ID { get; set; }

        public string UserAuthName
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }

        public string UserScreenName
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }

        public string OrganizationName
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }

        public IDictionary<string, AccessType> UserAuthorizations { get; set; }

        public  DateTime? LastLoginDate { get; set; }
        public DateTime? CurrentLoginDate { get; set; }

        public bool IsAuthenticated { get; set; }
        public bool IsLockedOut { get; set; }
        public string UserToken
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                if(value != GetPropertyValue<string>())
                { 
                    SetPropertyValue<string>(value);
                    DecodeToken();
                }
            }
        }
        public string IdentServer
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
                
                OnIdentServerChanged(new EventArgs());
            }
        }
        public APIDetails API
        {
            get
            {
                return GetPropertyValue<APIDetails>();
            }
            set
            {
                SetPropertyValue<APIDetails>(value);
            }
        }
      public NotificationSettings Notifications
        {
            get
            {
                return GetPropertyValue<NotificationSettings>();
            }
            set
            {
                SetPropertyValue<NotificationSettings>(value);
            }
        }
        public string Role
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue<string>(value);
            }
        }

        #endregion

        #region Methods
        private void DecodeToken()
        {
            if (!string.IsNullOrEmpty(UserToken))
            {

                JwtSecurityTokenHandler JstkHand = new JwtSecurityTokenHandler();
                JwtSecurityToken JTok = JstkHand.ReadJwtToken(UserToken);
                if (JTok != null)
                {
                    UserScreenName = JTok.Payload["name"].ToString();
                    ID = new Guid(JTok.Payload["sub"].ToString());
                    OrganizationName = JTok.Payload["Organisation"].ToString();
                    Role = JTok.Payload["role"].ToString();
                }
                
            }
        }
        #endregion

        #region event handlers
        protected virtual void OnIdentServerChanged(EventArgs e)
        {
            if (IdentServerChanged != null)
                IdentServerChanged(this, e);
        }
        #endregion
    }
}
