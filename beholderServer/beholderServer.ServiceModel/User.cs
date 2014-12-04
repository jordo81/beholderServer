using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using MySql.Data.MySqlClient;

namespace beholderServer.ServiceModel
{
    [Route("/user/info/{ID}")]
    public class User : IReturn<UserResponse>
    {
        const string dbs_createUser = "Insert into users (fbID, accessToken, name, created, lastLogin, birthday) values ('{0}','{1}','{2}','{3}','{4}','{5}')";
        const string dbs_getUserByID_OR_FBID = "Select * From users where userid = {0}";
        const string dbs_getUserByFBID = "Select * From users where fbID = {0}";
        const string dbs_updateLastLogin = "Update users SET lastLogin='{0}' where fbID = {1}";
        public int ID { get; set; }
        public string FBID { get; set; }
        public string accessToken { get; set; }
        public string appID { get; set; }

        public void createUser(MySqlConnection DB)
        {
            if (appID != "" && accessToken != "" && FBID != "")
            {
                //Get User info from facebook.
                string now = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                string bday = (new DateTime(1992, 05, 20)).ToString("yyyy-MM-dd HH:mm:ss");
                string query = string.Format(dbs_createUser, FBID, accessToken, "Michael Jordison", now, now, bday);
                MySqlCommand cmd = new MySqlCommand(query, DB);
                try
                {
                    MySqlDataReader rdr = cmd.ExecuteReader();
                    if(rdr.RecordsAffected != 1)
                    {
                        throw new ArgumentException("Invalid Create User String Rows Affected: " + rdr.RecordsAffected);
                    }
                    rdr.Close();
                    rdr.Dispose();
                }
                catch(Exception ex)
                {
                    throw new ArgumentException("Create User Failed:" + ex);
                }
                cmd.Dispose();
            }
            else
            {
                throw new ArgumentException("Create User: Missing Information");
            }

        }
        public void updatelastLogin(MySqlConnection DB)
        {
            string now = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
            string query = string.Format(dbs_updateLastLogin,now, FBID);
            MySqlCommand cmd = new MySqlCommand(query, DB);
            try {
                MySqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.RecordsAffected != 1) { throw new ArgumentException("Update Last Login Failed affected Count: " + rdr.RecordsAffected); }
                rdr.Close();
                rdr.Dispose();
                }
            catch(Exception ex) { throw new ArgumentException("UpdateLastLogin Failed for user " + FBID + "Exception: " + ex); }
            cmd.Dispose();
        }

        //Look at more effecent calls to update on login so session is closed as fast as possible.
        //such as geting facebook data and user data  storing / modifing query data and makeing up request.
        // {0} values {1}
        //keyvalue pair dictionary
        //prase into 0 and 1
        //post update. Update users set name="Michael Test", likeCount=1 where userid = 5
        //
        public UserLoginResponse loginUser(MySqlConnection DB)
        {
            UserLoginResponse response = new UserLoginResponse();
            response.FBID = FBID;

            //get sql user
            string query = string.Format(dbs_getUserByFBID, FBID);
            MySqlCommand cmd = new MySqlCommand(query, DB);
            MySqlDataReader rdr = cmd.ExecuteReader();

            //get Facebook user
            var client = new Facebook.FacebookClient(accessToken);
            dynamic result = client.Get("Me");
            //Validate provided token...

            if(!rdr.HasRows)
            {
                //user dosn't exist create there account.
                rdr.Close();
                createUser(DB);
                response.status = "Logged In";
                return response;
            }
           
            response.status = "Exists";
            //Validate Login
            //adasd

            response.status = "Logged In";

            rdr.Close();
            updatelastLogin(DB);

            return response;
        }
    }
    [Route("/user/login/")]
    public class UserLogin : User
    { }
    [Route("/user/logout/")]
    public class UserLogout : User
    { }
    public class UserLoginResponse
    {
        public string status { get; set; }
        public string FBID { get; set; }

        public UserLoginResponse()
        {
            this.status = "Error";
        }

    }
    // /User/info/{id} Response
    public class UserResponse
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string birthday { get; set; }
        public string created { get; set; }
        public string lastLogin { get; set; }
        public int likeCount { get; set; }
        public int credits { get; set; }
        public int activityTime { get; set; }
        public int sessionCount { get; set; }

        public UserResponse(User u, MySqlConnection DB)
        {
            //Set ID
            this.ID = u.ID;

            //Prepare Query
            string query = "Select * From users where userid = " + ID;
            MySqlCommand cmd = new MySqlCommand(query, DB);
            MySqlDataReader rdr = cmd.ExecuteReader();

            try
            {
                //Read Single Result
                rdr.Read();

                Name = rdr.GetString("Name");
                birthday = rdr.GetDateTime("Birthday").Date.ToShortDateString();
                created = rdr.GetDateTime("created").Date.ToShortDateString();
                lastLogin = rdr.GetDateTime("lastLogin").Date.ToShortDateString();
                likeCount = rdr.GetInt32("likeCount");
                credits = rdr.GetInt32("credits");
                activityTime = rdr.GetInt32("activityTime");
                sessionCount = rdr.GetInt32("sessionCount");
                
                rdr.Close();
                
            }
            catch
            {
                throw new ArgumentException("No User Found");
                
                
            }
            rdr.Dispose();
            cmd.Dispose();
            
        }
    }

}