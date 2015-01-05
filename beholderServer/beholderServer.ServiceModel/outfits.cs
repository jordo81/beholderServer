using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;

namespace beholderServer.ServiceModel
{
    public class Outfit : IReturn<UserResponse>
    {
        public string outfitID;
        public string userID;
        public string blerb;
        public string image1URL;
        public string image2URL;
        public string image3URL;
        public string image4URL;
        public string image5URL;
        public int likeCount;
        public int totalVotes;
        public char[] privacy;

        public void newOutfit()
        { }

        public void getOutfit(string ID)
        { 
            
        }

        public void likeOutfit()
        { }
    }
    public class outfitForReview
    {
        public string outfitID { get; set; }
        public string userID { get; set; }
        public string blerb { get; set; }
        public string image1URL { get; set; }
        public string image2URL { get; set; }
        public string image3URL { get; set; }
        public string image4URL { get; set; }
        public string image5URL { get; set; }

        public string toString()
        {
            string tmp = "{";
            tmp += "\"outfitID\": \"" + outfitID + "\",";
            tmp += "\"userID\": \"" + userID + "\",";
            tmp += "\"blerb\": \"" + blerb + "\",";
            tmp += "\"image1URL\": \"" + image1URL + "\",";
            tmp += "\"image2URL\": \"" + image2URL + "\",";
            tmp += "\"image3URL\": \"" + image3URL + "\",";
            tmp += "\"image4URL\": \"" + image4URL + "\",";
            tmp += "\"image5URL\": \"" + image5URL + "\"";
            tmp += "}";
            return tmp;
        }

    }
    [Route("/getOutfits")]
    public class getOutfits
    {
        public Dictionary<int, outfitForReview> outfitForReview;
        
        public getOutfits()
        {
            outfitForReview = new Dictionary<int, outfitForReview>();
        }

        public void loadOutfits( MySqlConnection DB)
        {
            string query = string.Format(DB_Queries.dbs_getoutfits);
            MySqlCommand cmd = new MySqlCommand(query, DB);
            MySqlDataReader rdr = cmd.ExecuteReader();
            int count = 0;
            while(rdr.Read())
            {
                outfitForReview x = new outfitForReview();
                x.outfitID = rdr.GetString("outfitsID");
                x.blerb = rdr.GetString("blerb");
                x.image1URL = rdr.GetString("image1URL");
                x.userID = rdr.GetString("UserID");

                outfitForReview.Add(count, x);
                count++;
            }

        }
    }
}