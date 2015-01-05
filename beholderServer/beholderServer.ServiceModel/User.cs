using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;

namespace beholderServer.ServiceModel
{
    
    public class User : IReturn<UserResponse>
    {
        
        public int ID { get; set; }
        public string FBID { get; set; }
        public string accessToken { get; set; }
        public string appID { get; set; }

        public void createUser(MySqlConnection DB, dynamic FB_Me)
        {
            if (appID != "" && accessToken != "" && FBID != "")
            {
                //Get User info from facebook.
                string now = (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
                string birthday = Convert.ToString(FB_Me.birthday);
                string bday = DateTime.ParseExact(birthday, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                string query = string.Format(DB_Queries.dbs_createUser, FBID, accessToken, Convert.ToString(FB_Me.name), now, now, bday);
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
        public void postUpdates(MySqlConnection DB, Dictionary<string,string> updateVars)
        {
            string vars = "";
            foreach(var x in updateVars)
            {
                vars += x.Key + "='" + x.Value + "'";
                if(x.Key != updateVars.Last().Key)
                {
                    vars += ",";
                }
                else
                {
                    break;
                }
            }
            string query = string.Format(DB_Queries.dbs_updateLastLogin, vars, FBID);
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
            Dictionary<string,string> updateVars = new Dictionary<string,string>();

            //get sql user
            string query = string.Format(DB_Queries.dbs_getUserByFBID, FBID);
            MySqlCommand cmd = new MySqlCommand(query, DB);
            MySqlDataReader rdr = cmd.ExecuteReader();
            bool userExisits = rdr.HasRows;
            

            //get Facebook user
            var client = new Facebook.FacebookClient(accessToken);
            dynamic result = client.Get("Me");
            if(!rdr.HasRows)
            {
                rdr.Close();
                rdr.Dispose();
                //user dosn't exist create there account.
                createUser(DB,result);
                response.status = "Logged In";
                return response;
            }
            rdr.Read();
            //validate accessToken Hasn't changed
            if(!this.accessToken.Equals(rdr["accessTocken"])) {
                updateVars.Add("accessToken", accessToken);
            }
            updateVars.Add("lastLogin", (DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss"));

            rdr.Close();
            postUpdates(DB, updateVars);

            return response;
        }
    }
    [Route("/user/login/")]
    public class UserLogin : User
    { }
    [Route("/user/logout/")]
    public class UserLogout : User
    { }
    [Route("/user/info/{IDtag}")]
    public class GetUserInfo : User
    {
        public string IDtag;

    }
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