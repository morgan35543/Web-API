using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DistSysACWClient
{
    #region Task 10 and beyond
    class Client
    {
        static HttpClient httpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            bool exit = false;

            httpClient.BaseAddress = new Uri("https://localhost:44307/");

            Console.WriteLine("Hello. What would you like to do?");

            string username = "";
            string apikey = "";
            bool apikeySet = false;

            while (exit == false)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                    return;
                                
                try
                {
                    Console.Clear();
                    Console.WriteLine("...please wait...");

                    string[] inputArr = input.Split(' ');
                    List<string> controllers = new List<string> { "talkback", "user", "protected" };


                    if (!controllers.Contains(inputArr[0].ToLower()) || inputArr.Length < 2)
                    {
                        Console.WriteLine("Input invalid.");
                        continue;
                    }

                    int requestType = 0;
                    // 1 = Get
                    // 2 = Post
                    // 3 = Delete

                    //[Controller]\[Action]
                    string controller = inputArr[0].ToLower();
                    string action = inputArr[1].ToLower();

                    string requestQuery = $"api/";
                    string requestBody = "";
                    string requestResponse = "";

                    if (apikey != "")
                    {
                        apikeySet = true;
                        if (!apikey.Contains("-"))
                        {
                            string[] keysplit = apikey.Split("");
                            string keyOut = "";
                            for (int x = 0; x < keysplit.Length; x++)
                            {
                                keyOut += keysplit;
                                if (x == 7)
                                    keyOut += "-";
                                if (x == 11)
                                    keyOut += "-";
                                if (x == 15)
                                    keyOut += "-";
                                if (x == 19)
                                    keyOut += "-";
                            }
                            apikey = keyOut;
                            User.apikey = keyOut;
                        }
                    }
                    else
                        apikeySet = false;

                    bool newUserPost = false;

                    if (controller == "talkback")
                    {
                        requestQuery += $"talkback/";
                        switch (action)
                        {
                            default:
                                Console.WriteLine("Input invalid.");
                                continue;

                            case "hello":
                                requestQuery += $"hello";
                                requestType = 1;

                                break;

                            case "sort":
                                if (inputArr.Length < 3)
                                {
                                    Console.WriteLine("Input invalid.");
                                    continue;
                                }
                                else
                                {
                                    requestQuery += $"sort?";
                                    inputArr[2] = inputArr[2].Replace("[", "");
                                    inputArr[2] = inputArr[2].Replace("]", "");
                                    string[] intArrayasStr = inputArr[2].Split(',');

                                    foreach (string integer in intArrayasStr)
                                    {
                                        requestQuery += $"integers={integer}&";
                                    }
                                }
                                requestType = 1;

                                break;
                        }

                    }
                    else if (controller == "user")
                    {
                        if (inputArr.Length < 3 && action != "delete")
                        {
                            Console.WriteLine("Input invalid.");
                            continue;
                        }
                        else if (inputArr.Length > 2)
                            username = inputArr[2];

                        requestQuery += "user/";
                        switch (action)
                        {
                            default:
                                Console.WriteLine("Input invalid.");
                                continue;

                            case "get":
                                requestQuery += $"new?username={username}";
                                requestType = 1;
                                break;

                            case "post":
                                requestQuery += $"new";
                                requestBody += $"\"{username}\"";
                                requestType = 2;
                                newUserPost = true;
                                break;

                            case "set":
                                if (inputArr.Length < 4)
                                {
                                    Console.WriteLine("Input invalid.");
                                    continue;
                                }

                                apikey = inputArr[3];
                                User.apikey = apikey;
                                User.username = username;
                                apikeySet = true;
                                requestResponse = "Stored";

                                break;

                            case "delete":
                                if (username == "" || apikey == "")
                                {
                                    requestResponse += "You need to do a User Post or User Set first";
                                    Console.WriteLine(requestResponse);
                                    continue;
                                }

                                requestQuery += $"removeuser?username={username}";
                                requestType = 3;

                                break;

                            case "role":
                                if (apikey == "")
                                {
                                    requestResponse = "You need to do a User Post or User Set first";
                                    Console.WriteLine(requestResponse);
                                    continue;
                                }

                                if (inputArr.Length < 4)
                                {
                                    Console.WriteLine("Input invalid.");
                                    continue;
                                }

                                string role = inputArr[3];

                                requestQuery += $"changerole";
                                requestBody += "{\"username\":\"" + username + "\",\"role\":\"" + role + "\"}";

                                requestType = 2;

                                break;
                        }
                    }
                    else if (controller == "protected")
                    {
                        requestQuery += $"protected/";
                        switch (action)
                        {
                            default:
                                Console.WriteLine("Input invalid.");
                                continue;

                            case "hello":
                                if (User.apikey == "")
                                {
                                    requestResponse = "You need to do a User Post or User Set first";
                                    Console.WriteLine(requestResponse);
                                    continue;
                                }

                                requestQuery += $"hello";
                                requestType = 1;

                                break;

                            case "sha1":
                                if (User.apikey == "")
                                {
                                    requestResponse += "You need to do a User Post or User Set first";
                                    Console.WriteLine(requestResponse);
                                    continue;
                                }

                                if (inputArr.Length < 3)
                                {
                                    Console.WriteLine("Input must contain a message.");
                                    continue;
                                }

                                string message = "";
                                for (int i = 2; i < inputArr.Length; i++)
                                    message += $"{inputArr[i]}";

                                requestQuery += $"sha1?message={message}";
                                requestType = 1;

                                break;

                            case "sha256":
                                if (User.apikey == "")
                                {
                                    requestResponse = "You need to do a User Post or User Set first";
                                    Console.WriteLine(requestResponse);
                                    continue;
                                }

                                if (inputArr.Length < 3)
                                {
                                    Console.WriteLine("Input must contain a message.");
                                    continue;
                                }

                                message = "";
                                for (int i = 2; i < inputArr.Length; i++)
                                    message += $"{inputArr[i]}";

                                requestQuery += $"sha256?message={message}";
                                requestType = 1;

                                break;
                        }
                    }


                    if (apikeySet == true)
                    {
                        httpClient.DefaultRequestHeaders.Remove("apikey");
                        httpClient.DefaultRequestHeaders.Add("apikey", apikey);
                    }


                    RequestObject requestObject = new RequestObject();
                    requestObject.requestQuery = requestQuery;
                    requestObject.username = username;
                    requestObject.requestBody = requestBody;
                    requestObject.requestResponse = requestResponse;

                    switch (requestType)
                    {
                        case 0:
                            Console.WriteLine(requestResponse);
                            break;

                        case 1: // Get string
                            string response = await GetRequest(requestObject);
                            Console.WriteLine(response);
                            break;

                        case 2: // Post string
                            response = await PostRequest(requestObject, newUserPost);
                            Console.WriteLine(response);

                            if (newUserPost)
                            {
                                username = User.username;
                                apikey = User.apikey;
                            }

                            break;

                        case 3: // Delete bool
                            string responseBool = await DeleteRequest(requestObject);
                            Console.WriteLine(responseBool);
                            if (responseBool.ToLower() == "true")
                            {
                                apikey = "";
                                apikeySet = false;
                                username = "";
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    Console.WriteLine("What would you like to do next?");
                }
            }

        }

        static async Task<string> GetRequest(RequestObject requestObject)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(requestObject.requestQuery);

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
                else
                    return response.StatusCode.ToString();
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        static async Task<string> PostRequest(RequestObject requestObject, bool newUserPost)
        {
            try
            {
                var content = new StringContent(requestObject.requestBody.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(requestObject.requestQuery, content);
                string responseString = await response.Content.ReadAsStringAsync();

                if (newUserPost == true && response.IsSuccessStatusCode)
                {
                    User.apikey = responseString;
                    User.username = requestObject.username;
                    return "Got API key";
                }
                else
                    return responseString;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        static async Task<string> DeleteRequest(RequestObject requestObject)
        {
            try
            {
                HttpResponseMessage response = await httpClient.DeleteAsync(requestObject.requestQuery);
                string responseBool = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return responseBool;
                else
                    return response.ReasonPhrase;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static class User
        {
            public static string username = "";
            public static string apikey = "";
        }

        public class RequestObject
        {
            public string requestQuery { get; set; }
            public string requestBody { get; set; }
            public string requestResponse { get; set; } 
            public string username { get; set; } // Obsolete
        }

        #endregion
    }
}


