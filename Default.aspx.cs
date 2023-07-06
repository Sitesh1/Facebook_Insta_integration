using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facebook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Dynamic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Diagnostics;
using RestSharp;
using System.Web.Services.Description;

namespace FacebookInstaIntegrate
{
    public partial class Default : System.Web.UI.Page
    {
        string app_id = "";
        string app_secret = "";
        string PageId = "";
        string scope = "";
        string accessToken = "EAAU89d7UC9UBAJwd6OKftAsG05HeX6vJJIlkK2g2xKUNclbJprzf7PxzLWRnH1NO4n7moFWk1AGWvjgkAoXUHpnclct0mnAm2ia7oAIEV3ra50QMzyZA7ErAsstwvZBYWMt8bDFYo2VR01VVVcImI9kdYRAjZA7jjG1YvjzAkWDRtjuDIfy";
        public class AccessUser
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string pageaccess_token { get; set; }
            public string id { get; set; }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
           // CheckAuthorization();
        }
        private void CheckAuthorization()
        {
            if (Request["Code"] != null)
            {
                div_btn.Visible = true;
                // Dictionary<string, string> tokens = new Dictionary<string, string>();

                string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&scope={2}&code={3}&client_secret={4}", app_id, Request.Url.AbsoluteUri, scope
                , Request["code"].ToString(), app_secret);


                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                string access_token = "";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)

                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string vals = reader.ReadToEnd();
                    var rs = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessUser>(vals);
                    access_token = rs.access_token;
                }

                accessToken = access_token;
                Session["atoken"] = access_token;
                List<FacebookPage> pageIds = GetPageIds(accessToken);

                //Bind the page IDs to the dropdown list
               
                ddlPageIds.DataSource = pageIds;
                ddlPageIds.DataTextField = "Name";
                ddlPageIds.DataValueField = "Id";
                ddlPageIds.DataBind();
                ddlPageIds.Items.Insert(0, new ListItem("Please select", ""));

                //lblAccessToken.Text = accessToken;
                //string PageAccessToken = pageAccesskey("102847089502067");
                //Session["PageAccessToken"] = PageAccessToken;
            }
        }
        public class FacebookPage
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
        public class FacebookPost
        {
            public string Id { get; set; }
            public string Message { get; set; }
        }
        private List<FacebookPage> GetPageIds(string accessToken)
        {
            using (HttpClient client = new HttpClient())
            {
                // Construct the request URL
                string requestUrl = $"https://graph.facebook.com/v12.0/me/accounts?access_token={accessToken}";

                // Send the request to get the user's page IDs
                HttpResponseMessage response = client.GetAsync(requestUrl).Result;
                string responseContent = response.Content.ReadAsStringAsync().Result;

                // Parse the response JSON
                dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);

                // Extract the page IDs from the response
                List<FacebookPage> pages = new List<FacebookPage>();
                foreach (dynamic page in jsonResponse.data)
                {

                    FacebookPage facebookPage = new FacebookPage
                    {
                        Id = page.id.ToString(),
                        Name = page.name.ToString()
                    };
                    pages.Add(facebookPage);

                    //pageIds.Add(page.id.ToString());
                }

                // Return the page IDs
                return pages;
            }
        }

        public List<FacebookPost> GetPostIds(string pageId, string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(pageId))
            {
                using (HttpClient client = new HttpClient())
                {
                    string fields = "posts";
                    dynamic result = new FacebookClient(accessToken).Get(pageId + "?fields=" + fields);
                    var posts = result["posts"]["data"];
                    List<FacebookPost> postIds = new List<FacebookPost>();

                    foreach (var post in posts)
                    {
                        FacebookPost facebookPost = new FacebookPost
                        {
                            Id = post["id"].ToString(),
                            Message = post["message"].ToString()
                        };
                        postIds.Add(facebookPost);
                    }


                    return postIds;
                }
            }
            else
            {
                return null;
            }
           
           
        }

        protected void fblogin_Click(object sender, EventArgs e)
        {
            try
            {
                app_id = "1474401586645973";
                app_secret = "7cc1c7225a3a75fb14d2fe194626e260";
                //PageId = "116342828011972";
                scope = "pages_show_list, pages_read_engagement,pages_read_user_content, pages_manage_posts";
               
                if (Request["Code"] == null)
                {
                    Response.Redirect(string.Format("https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2} ", app_id, Request.Url.AbsoluteUri, scope));
                  
                }
               
              CheckAuthorization();
            }
            catch (Exception ex)
            {
                //lbl1.Text = ex.Message;
            }
        }
        public string pageAccesskey(string pageId)
        {
            string accessToken = Session["atoken"] as string;
            //private const string AccessTokenUrl = "https://graph.facebook.com/v12.0/"pageId"?fields=access_token&access_token={app_access_token}";
            string AccessTokenUrl = string.Format("https://graph.facebook.com/" + pageId + "?fields=access_token&access_token=" + accessToken + "");
            //string appAccessToken = GetAppAccessToken();
            string accessTokenUrl = AccessTokenUrl.Replace("{page_id}", pageId).Replace("{app_access_token}", accessToken);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(accessTokenUrl).Result;
                string responseContent = response.Content.ReadAsStringAsync().Result;

                dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);
                string pageAccessToken = jsonResponse.access_token;

                return pageAccessToken;
            }
            //HttpWebRequest pagerequest = WebRequest.Create(pageurl) as HttpWebRequest;
            //string pageaccess_token = "";
            //using (HttpWebResponse response = pagerequest.GetResponse() as HttpWebResponse)
            //{
            //    StreamReader reader = new StreamReader(response.GetResponseStream());
            //    string Pagevals = reader.ReadToEnd();
            //    var pagers = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessUser>(Pagevals);
            //    pageaccess_token = pagers.access_token;
            //}
            //return pageaccess_token;
        }
        private string GetAppAccessToken()
        {
            string appAccessTokenUrl = $"https://graph.facebook.com/oauth/access_token?client_id={app_id}&client_secret={app_secret}&grant_type=client_credentials";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(appAccessTokenUrl).Result;
                string responseContent = response.Content.ReadAsStringAsync().Result;

                string appAccessToken = responseContent.Replace("access_token=", "");

                return appAccessToken;
            }
        }

        Dictionary<int, string> searchResults = new Dictionary<int, string>();
        protected void countCmnt_Click(object sender, EventArgs e)
        {
            try
            {
                string accessToken1 = Session["Atoken1"] as string;
                string folderPath = @"D:\FacebookIntegration\";
                Directory.CreateDirectory(folderPath);
                string commentname = "";
                string commentMessage = "";
                var client = new FacebookClient(accessToken1);
                string pageId = Session["Page_id"] as string;
                //string pageId = "102847089502067";
                string PostId = "";  //102847089502067_123996710705953
                dynamic posts = client.Get($"{pageId}/posts");


                int count = 0;

                foreach (dynamic post in posts.data)
                {
                    string postId = post.id;
                    if (!string.IsNullOrEmpty(PostId))
                    {
                        if (postId == PostId)
                        {
                            dynamic comments = client.Get($"{postId}/comments");

                            while (true)
                            {
                                foreach (dynamic comment in comments.data)
                                {
                                    count++;
                                    commentname = comment.from.name;
                                    commentMessage = comment.message + " name:-" + commentname;
                                    searchResults.Add(count, commentMessage);
                                }

                                if (comments.paging == null || comments.paging.next == null)
                                {
                                    break;
                                }

                                comments = client.Get(comments.paging.next);
                            }
                        }
                    }
                    else
                    {
                        dynamic comments = client.Get($"{postId}/comments");

                        while (true)
                        {
                            foreach (dynamic comment in comments.data)
                            {
                                count++;
                                commentname = comment.from.name;
                                commentMessage = comment.message + " name:-" + commentname;
                                searchResults.Add(count, commentMessage);
                            }

                            if (comments.paging == null || comments.paging.next == null)
                            {
                                break;
                            }

                            comments = client.Get(comments.paging.next);
                        }
                    }
                }
                HttpContext.Current.Session["SearchResults"] = searchResults;
              

                lblcntcmment.Text = "Total Comments: " + searchResults.Count;
            }
            catch (Exception ex)
            {
                lblcntcmment.Text = ex.Message;
            }
        }
        protected void countLike_Click(object sender, EventArgs e)
        {

            if (Div_like.Visible)
            {
                Div_like.Visible = false;
            }
            else
            {
                Div_like.Visible = true;
            }
            string accessToken1 = Session["Atoken1"] as string;
            string pid = Session["Page_id"] as string;
           
            List<FacebookPost> postIds = GetPostIds(pid,accessToken1);

            //Bind the page IDs to the dropdown list

            Drpselectpost.DataSource = postIds;
            Drpselectpost.DataTextField = "Message";
            Drpselectpost.DataValueField = "Id";
            Drpselectpost.DataBind();
            Drpselectpost.Items.Insert(0, new ListItem("Please select", ""));

        }
        protected void countShare_Click(object sender, EventArgs e)
        {
            try
            {
                string folderPath = @"D:\FacebookIntegration\";
                Directory.CreateDirectory(folderPath);
                string pid = Session["Page_id"] as string;
                //string pageId = "102847089502067";
                string accessToken1 = Session["accessToken1"] as string;

                var fb = new FacebookClient(accessToken);

                var parameters = new Dictionary<string, object>

                {
                     { "fields", "sharedposts" }
                };

                dynamic result = fb.Get($"{pid}/posts", parameters);

                // List<string> searchResults = new List<string>();
                Dictionary<Int32, string> shareList = new Dictionary<Int32, string>();

                if (result != null)
                {
                    if (result.data != null)
                    {
                        int count = 0;
                        foreach (var post in result.data)
                        {
                            if (post.sharedposts != null)
                            {

                                foreach (var sharedPost in post.sharedposts.data)
                                {
                                    count++;
                                    var sharedPostId = sharedPost.id;
                                    var sharedPostMessage = sharedPost.message;
                                    var sharerName = FacebookSharerName(accessToken);
                                    shareList.Add(count, "Id:-" + sharedPostId + " Message:-" + sharedPostMessage + " Shared By:-" + sharerName);
                                    // searchResults.Add("Message:-"+sharedPostMessage);
                                    //Console.WriteLine(searchResults);
                                    //Console.WriteLine($"Shared Post Message: {sharedPostMessage}");
                                }
                            }
                            HttpContext.Current.Session["shareList"] = shareList;
                            using (StreamWriter writer = new StreamWriter(folderPath + "" + "shared_list.txt"))
                            {
                                foreach (var result1 in shareList)
                                {
                                    lblcntshare.Text = result1.ToString();
                                }
                            }
                        }

                        lblcntshare.Text = "Your total count of sharepost is=" + shareList.Count();
                      //  Console.WriteLine("Your total count of sharepost is=" + searchResults.Count());


                        //Console.WriteLine("Please enter random shared number");
                        //var chk = Console.ReadLine();
                        //if (searchResults.ContainsKey(Convert.ToInt32(chk)))
                        //{
                        //    foreach (var result2 in searchResults)
                        //    {
                        //        if (result2.Key == Convert.ToInt32(chk))
                        //        {
                        //            Console.WriteLine("Match found! Value: " + result2.Value);
                        //        }
                        //    }
                        //    Console.ReadLine();
                        //}
                        //else
                        //{
                        //    Console.WriteLine("Key not found in the dictionary.");
                        //}

                        //Console.ReadLine();

                    }
                }
                else
                {
                    lblcntshare.Text ="Unable to retrieve shared posts.";
                   // Console.ReadLine();
                }


            }
            catch (Exception ex)
            {
                lblcntshare.Text = ex.Message;
                //Console.ReadLine();
            }
        }
        public static string FacebookSharerName(string accesstoken)
        {
            string resp = "";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string pageId = "102847089502067";


                    string url = $"https://graph.facebook.com/v12.0/{pageId}?fields=feed%7Bsharedposts%7Bfrom%2Cname%7D%7D&access_token={accesstoken}";

                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;

                        JObject json = JObject.Parse(jsonResponse);

                        // Check if feed and sharedposts exist
                        if (json["feed"] != null && json["feed"]["data"] != null)
                        {
                            // Extract data array
                            JArray data = (JArray)json["feed"]["data"];

                            // Iterate through each entry in the data array
                            foreach (JToken entry in data)
                            {
                                // Check if sharedposts field exists
                                if (entry["sharedposts"] != null)
                                {

                                    // Extract sharedposts array
                                    JArray sharedPosts = (JArray)entry["sharedposts"]["data"];

                                    // Iterate through each shared post
                                    foreach (JToken post in sharedPosts)
                                    {

                                        // Check if from and name fields exist
                                        if (post["from"] != null && post["from"]["name"] != null)
                                        {
                                            string sharerName = (string)post["from"]["name"];
                                            //string message = (string)post["message"];
                                            //string message = (post["message"] != null) ? (string)post["message"] : "No Message Available";
                                            //string postId = (string)post["id"];

                                            //Console.WriteLine($"Sharer Name: {sharerName}");
                                            resp = sharerName;
                                            // Console.WriteLine($"Message: {message}");
                                            //Console.WriteLine($"Post ID: {postId}");
                                        }
                                    }

                                }
                            }
                            return resp;
                        }
                        else
                        {
                            //Console.WriteLine("Feed or sharedposts field not found in the response.");
                            //Console.ReadLine();
                            return resp;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode}");
                        resp = Console.ReadLine();
                        return resp;
                    }
                }

            }
            catch (FacebookApiException ex)
            {
                Console.WriteLine(ex.Message);
                resp = Console.ReadLine();
                return resp;
            }
        }
        protected void picRandomshare_Click(object sender, EventArgs e)
        {
            Dictionary<int, string> shareLists = HttpContext.Current.Session["shareList"] as Dictionary<int, string>;
            string txt = txtrandomshare.Text;
            int key = 0;
            if (string.IsNullOrEmpty(txt))
            {
                lblshareshow.Text = "Please enter random no of share";
            }
            else
            {
                key = int.Parse(txt.Trim());
            }

            if (key == 0)
            {
                lblshareshow.Text = "Please enter random no of share";
            }
            else
            {
                if (shareLists != null)
                {
                    var randomshares = shareLists.Where(x => x.Key == key).ToList();
                    foreach (var res in randomshares)
                    {
                        lblshareshow.Text = res.ToString();
                    }

                }
                else
                {
                    lblshareshow.Text = "No Share Available";
                }
            }

        }
        protected void picRandomCmnt_Click(object sender, EventArgs e)
        {
            Dictionary<int, string> searchResults = HttpContext.Current.Session["SearchResults"] as Dictionary<int, string>;


            //string commentname = "";
            //string commentMessage = "";
            //var client = new FacebookClient("EAAU89d7UC9UBAJwd6OKftAsG05HeX6vJJIlkK2g2xKUNclbJprzf7PxzLWRnH1NO4n7moFWk1AGWvjgkAoXUHpnclct0mnAm2ia7oAIEV3ra50QMzyZA7ErAsstwvZBYWMt8bDFYo2VR01VVVcImI9kdYRAjZA7jjG1YvjzAkWDRtjuDIfy");
            //string pageId = "102847089502067";
            //string PostId = "";  //102847089502067_123996710705953
            //dynamic posts = client.Get($"{pageId}/posts");


            //int count = 0;

            //foreach (dynamic post in posts.data)
            //{
            //    string postId = post.id;
            //    if (!string.IsNullOrEmpty(PostId))
            //    {
            //        if (postId == PostId)
            //        {
            //            dynamic comments = client.Get($"{postId}/comments");

            //            while (true)
            //            {
            //                foreach (dynamic comment in comments.data)
            //                {
            //                    count++;
            //                    commentname = comment.from.name;
            //                    commentMessage = comment.message + " name:-" + commentname;
            //                    searchResults.Add(count, commentMessage);
            //                }

            //                if (comments.paging == null || comments.paging.next == null)
            //                {
            //                    break;
            //                }

            //                comments = client.Get(comments.paging.next);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        dynamic comments = client.Get($"{postId}/comments");

            //        while (true)
            //        {
            //            foreach (dynamic comment in comments.data)
            //            {
            //                count++;
            //                commentname = comment.from.name;
            //                commentMessage = comment.message + " name:-" + commentname;
            //                searchResults.Add(count, commentMessage);
            //            }

            //            if (comments.paging == null || comments.paging.next == null)
            //            {
            //                break;
            //            }

            //            comments = client.Get(comments.paging.next);
            //        }
            //    }
            //}
            
            string txt = txtrandomcmmnt.Text;
            int key = 0;
            if (string.IsNullOrEmpty(txt))
            {
                lblcmtshow.Text = "Please enter random no of comments";
            }
            else
            {
                key = int.Parse(txt.Trim());
            }
            
            if (key==0)
            {
                lblcmtshow.Text = "Please enter random no of comments";
            }
            else
            {
                if (searchResults != null)
                {
                    var randomComments = searchResults.Where(x => x.Key == key).ToList();
                    foreach (var res in randomComments)
                    {
                        lblcmtshow.Text = res.ToString();
                    }

                }
                else
                {
                    lblcmtshow.Text = "No Comments Available";
                }
            }
           
           
            
        }
        protected void pickshare_Click(object sender, EventArgs e)
        {
            if (div_fbpickshare.Visible)
            {
                div_fbpickshare.Visible = false;
            }
            else
            {
                div_fbpickshare.Visible = true;
            }
          
        }
        protected void pickrandmcmnt_Click(object sender, EventArgs e)
        {
            if (div_fbpickcmt.Visible)
            {
                div_fbpickcmt.Visible = false;
            }
            else
            {
                div_fbpickcmt.Visible = true;
            }
        }
        protected void btnfacebk_Click(object sender, EventArgs e)
        {
            if (div_facebook.Visible)
            {
                div_facebook.Visible = false;
            }
            else
            {
                div_facebook.Visible = true;
            }
           
        }
        protected void exportCmt_Click(object sender, EventArgs e)
        {
            string folderPath = @"D:\FacebookIntegration\";
            Directory.CreateDirectory(folderPath);
            Dictionary<int, string> searchResults = HttpContext.Current.Session["SearchResults"] as Dictionary<int, string>;
           
            if (searchResults != null)
            {
                using (StreamWriter writer = new StreamWriter(folderPath + "comment_list1.txt"))
                {
                    foreach (var result in searchResults)
                    {
                        //txtComments1.InnerText += result.Value + Environment.NewLine;
                        writer.WriteLine(result);
                    }

                }
                Process.Start(folderPath);
                lblexportCmt.Text = "Comments Exported Successfully";
            }
            else
            {
                lblexportCmt.Text = "No comments available";
            }
        }
        protected void ddlPageIds_SelectedIndexChanged1(object sender, EventArgs e)
        {
            string selectedPageId = ddlPageIds.SelectedValue;
            Session["Page_id"] = selectedPageId;
            // Display the selected page ID in a readable format
            string pageAT = pageAccesskey(selectedPageId);
            Session["Atoken1"] = pageAT; //page access token that can be used any where
           // lblAccessToken.Text = pageAT;
        }
        protected void btncountlike_Click(object sender, EventArgs e)
        {
            try
            {
                string accessToken1 = Session["Atoken1"] as string;
                // string pid = Session["Page_id"] as string;
                string postId = Session["Post_id"] as string;
                //string postId = "102847089502067_128385860267038";
                //string accessToken = "EAAU89d7UC9UBAJwd6OKftAsG05HeX6vJJIlkK2g2xKUNclbJprzf7PxzLWRnH1NO4n7moFWk1AGWvjgkAoXUHpnclct0mnAm2ia7oAIEV3ra50QMzyZA7ErAsstwvZBYWMt8bDFYo2VR01VVVcImI9kdYRAjZA7jjG1YvjzAkWDRtjuDIfy";
                Dictionary<Int32, string> likelist = new Dictionary<Int32, string>();
                string apiUrl = $"https://graph.facebook.com/v14.0/{postId}?fields=likes&access_token={accessToken1}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                    bool hasNextPage = true;
                    int count = 0;
                    string nextPageUrl = apiUrl;
                    // Console.WriteLine("Likers:");
                    while (hasNextPage)
                    {
                        count++;
                        if (response.IsSuccessStatusCode)
                        {
                            string jsonResponse = response.Content.ReadAsStringAsync().Result;
                            dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);
                            var likes = data.likes.data;
                            foreach (var like in likes)
                            {
                                string likerid = like.id;
                                string likerName = like.name;
                                //Console.WriteLine("Id: " + likerid);
                                //Console.WriteLine("Name: " + likerName);
                                likelist.Add(count," id: "+likerid+" Name: " +likerName);
                            }
                            if (data.likes.paging != null && data.likes.paging.next != null)
                            {
                                nextPageUrl = data.likes.paging.next;
                            }
                            else
                            {
                                hasNextPage = false;
                            }

                        }
                        else
                        {
                            // Console.WriteLine($"Failed to retrieve likes. Status code: {response.StatusCode}");
                            // Console.ReadLine();
                            return;
                        }
                       
                    }
                    lblcntlike.Text = "Total likes: " + count;


                    HttpContext.Current.Session["LikesList"] = likelist;

                    //HttpResponseMessage response = client.GetAsync(apiUrl).Result;

                    //if (response.IsSuccessStatusCode)
                    //{
                    //    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    //    dynamic data = JObject.Parse(jsonResponse);
                    //    int likeCount = data.summary.total_count;
                    //    lblcntlike.Text = "Total Likes: " + likeCount;
                    //    //Console.WriteLine($"Likes: {likeCount}");
                    //}
                    //else
                    //{
                    //    lblcntlike.Text="Failed to retrieve likes";
                    //}
                }

                //Console.ReadLine();
                // string url = $"https://graph.facebook.com/v12.0/{id}?fields=posts{{likes{{count}}}}&access_token={accessToken}";
            }
            catch (Exception ex)
            {
                lblcntlike.Text = ex.Message;
            }



        }
        protected void Drpselectpost_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPostId = Drpselectpost.SelectedValue;
            Session["Post_id"] = selectedPostId;
        }
        protected void btnexportlike_Click(object sender, EventArgs e)
        {
            string folderPath = @"D:\FacebookIntegration\";
            Directory.CreateDirectory(folderPath);
            Dictionary<Int32, string> LikeList = HttpContext.Current.Session["LikesList"] as Dictionary<Int32, string>;

            if (LikeList != null)
            {
                using (StreamWriter writer = new StreamWriter(folderPath + "likelist.txt"))
                {
                    foreach (var result in LikeList)
                    {
                        //txtComments1.InnerText += result.Value + Environment.NewLine;
                        writer.WriteLine(result);
                    }

                }
                Process.Start(folderPath);
                lblexportlike.Text = "Likes Exported Successfully";
            }
            else
            {
                lblexportlike.Text = "No likes available";
            }
        }
        protected void PickLike_Click(object sender, EventArgs e)
        {
            if (div_picklike.Visible)
            {
                div_picklike.Visible = false;
            }
            else
            {
                div_picklike.Visible = true;
            }
        }
        protected void btnpicklike_Click(object sender, EventArgs e)
        {
            Dictionary<Int32, string> LikeList = HttpContext.Current.Session["LikesList"] as Dictionary<Int32, string>;
            string txt = txtpicklike.Text;
            int key = 0;
            if (string.IsNullOrEmpty(txt))
            {
                lblPickLike.Text = "Please enter random no of share";
            }
            else
            {
                key = int.Parse(txt.Trim());
            }

            if (key == 0)
            {
                lblPickLike.Text = "Please enter random no of share";
            }
            else
            {
                if (LikeList != null)
                {
                    var randomlikes = LikeList.Where(x => x.Key == key).ToList();
                    foreach (var res in randomlikes)
                    {
                        lblPickLike.Text = res.ToString();
                    }

                }
                else
                {
                    lblPickLike.Text = "No Share Available";
                }
            }

        }

        //protected void getrandomComment_Click(object sender, EventArgs e)
        //{
        //    if (int.TryParse(getCommentTxt.Text, out int randomNoOfComments))
        //    {
        //        var randomComments = searchResults.Where(x => x.Key == randomNoOfComments).ToList();

        //        getCommentTxt.Text = string.Join(Environment.NewLine, randomComments);
        //    }
        //    else
        //    {
        //        getCommentTxt.Text = "Invalid input. Please enter a valid number.";
        //    }
        //}
    }

}