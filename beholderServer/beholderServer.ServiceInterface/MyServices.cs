using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

using ServiceStack;
using beholderServer.ServiceModel;

    
                
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using ServiceStack.Configuration;
using ServiceStack.Web;


namespace beholderServer.ServiceInterface
{
    public class MyServices : Service {

        //Manage Mysql Connections At this level means a new sql connection each request.
        //I have left the DB Close in after each instance for cleaness.
        public MySqlConnection mysqlConnection = null;
        public MySqlConnection GetDbConnection()
        {
            if( mysqlConnection != null)
            {
                return mysqlConnection;
            }
            AppSettings appSettings = new AppSettings();
            try
            {
                mysqlConnection = new MySql.Data.MySqlClient.MySqlConnection();
                mysqlConnection.ConnectionString = appSettings.Get("ConnectionString");
                mysqlConnection.Open();
                return mysqlConnection;
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                throw HttpError.NotFound("404 Database Unreachable: " + ex.ToString());
            }
        }
        public void closeDBConnection()
        {
            if (mysqlConnection == null)
            {
                return;
            }
            else
            {
                mysqlConnection.Close();
                mysqlConnection.Dispose();
            }

        }

        //----Actions-------
        //user
        public object Any(User request)
        {
            if(request.ID <= 0)
            {
                throw new ArgumentNullException("ID");
                throw HttpError.NotFound("Invalid ID");
            }
            MySqlConnection DB = GetDbConnection();
            UserResponse x = new UserResponse(request, DB);
            closeDBConnection();
            return x;
        }

        public object Post(UserLogin request)
        {
            string body = base.Request.GetRawBody();
            Dictionary<string, object> jsonObject;
            if(body.Length > 0)
            {
                var serializer = new JavaScriptSerializer();
                jsonObject = serializer.DeserializeObject(body) as Dictionary<string, object>;
                request.accessToken = jsonObject["accessToken"].ToString();
                request.appID = jsonObject["appID"].ToString();
                request.FBID = jsonObject["FBID"].ToString();
                if (request.appID != "" && request.accessToken != "" && request.FBID != "")
                {
                    MySqlConnection DB = GetDbConnection();
                    UserLoginResponse response = request.loginUser(DB);
                    closeDBConnection();
                    return response;
                }
            }
            
            
            
            return null;
        }
        
        public object Any(UserLogout request)
        {
            return null;
        }

      
    }
}