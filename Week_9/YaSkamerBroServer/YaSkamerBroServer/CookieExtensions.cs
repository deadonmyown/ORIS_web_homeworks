using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace YaSkamerBroServer
{
    public static class CookieExtensions
    {
        public static bool CheckSessionId(this CookieCollection cookieCollection)
            => cookieCollection["SessionId"] != null ? true : false;

        public static int? CheckAuthorizedAccount(this CookieCollection cookieCollection)
        {
            
            
            var cookie = cookieCollection["SessionId"];
            if (cookie != null)
            {
                if(Guid.TryParse(cookie.Value, out Guid userGuid) && SessionManager.CheckSession(userGuid))
                {
                    return SessionManager.GetSession(userGuid).AccountId;
                }
            }
            return null;
        }
    }
}
