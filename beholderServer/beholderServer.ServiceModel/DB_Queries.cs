using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beholderServer.ServiceModel
{
    public static class DB_Queries
    {
        public const string dbs_createUser = "Insert into users (fbID, accessToken, name, created, lastLogin, birthday) values ('{0}','{1}','{2}','{3}','{4}','{5}')";
        public const string dbs_getUserByID_OR_FBID = "Select * From users where userid = {0}";
        public const string dbs_getUserByFBID = "Select * From users where fbID = {0}";
        public const string dbs_updateLastLogin = "Update users SET {0} where fbID = {1}";
        public const string dbs_getoutfits = "SELECT * from outfits";

    }
}
