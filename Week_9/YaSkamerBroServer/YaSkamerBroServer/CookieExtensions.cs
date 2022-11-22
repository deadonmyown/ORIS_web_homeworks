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
        {
            var cookie = cookieCollection["SessionId"];
            if(cookie == null)
            {
                var args = cookie.Value.Split(new char[] { ' ', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 2 && bool.TryParse(args[1], out bool isAuth) && isAuth == true)
                    return true;
            }

            return false;
        }

        public static bool CheckAuthorizedAccountById(this CookieCollection cookieCollection, int accId)
        {
            var session = cookieCollection["SessionId"];
            var account = cookieCollection["Id"];
            if (session.Name != null && account.Name != null)
            {
                var args = session.Value.Split(new char[] { ' ', ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 2 && bool.TryParse(args[1], out bool isAuth) && int.TryParse(account.Value, out int authId) && authId == accId && isAuth == true)
                    return true;
            }

            return false;
        }
    }
}
