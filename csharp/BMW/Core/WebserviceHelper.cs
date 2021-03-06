﻿using BMW.Models;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Linq;

namespace BMW.Core
{
    public static class WebserviceHelper
    {

        private static readonly string APIServer = "https://b2vapi.bmwgroup.com"; // altgernativ https://b2vapi.bmwgroup.us
        //private static readonly string APIServer = "https://b2vapi.bmwgroup.cn:8592";

        private static IWebProxy GetProxy()
        {
            IWebProxy defaultWebProxy = WebRequest.DefaultWebProxy;
            defaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
            return defaultWebProxy;
        }

        public static string GetAPIToken(string user, string pw)
        {
            using (var myWebClient = new WebClient() { Proxy = GetProxy() })
            {
                string key = "key"; //you need to replace it with actual values
                string secret = "secret";//you need to replace it with actual values
                string basic = ($"{key}:{secret}").ToBase64String();
                // hint: basic =  "blF2NkNxdHhKdVhXUDc0eGYzQ0p3VUVQOjF6REh4NnVuNGNEanliTEVOTjNreWZ1bVgya0VZaWdXUGNRcGR2RFJwSUJrN3JPSg==";

                var WS_Data = new NameValueCollection
                {
                    { "grant_type", "password" },
                    { "username",user },
                    { "password", pw },
                    { "scope" , "remote_services vehicle_data"  }
                };
                myWebClient.Headers.Add("Authorization", "Basic " + basic);
                myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                string response="";

                try
                {
                    response = myWebClient.UploadString($"{APIServer}/gcdm/oauth/token", WS_Data.ToQueryString(isEscaped: false));
                }
                catch (System.Exception ex)
                {
                    Trace.Write("TokenRenew");
                    Trace.WriteLine(ex.Message);

                }

                return response;

            }
        }

        public static string GetVehiclesJson(string token)
        {
            using (var myWebClient = new WebClient() { Proxy = GetProxy() })
            {
                myWebClient.Headers.Add("Authorization", "Bearer " + token);

                string response = "";

                try
                {
                    response = myWebClient.DownloadString($"{APIServer}/webapi/v1/user/vehicles/");

                }
                catch (System.Exception ex)
                {
                    Trace.Write("GetVehicles");
                    Trace.WriteLine(ex.Message);
                }
                return response;
            }
        }

        public static string GetStatusJson(string token, string vin)
        {
            using (var myWebClient = new WebClient() { Proxy = GetProxy() })
            {
                myWebClient.Headers.Add("Authorization", "Bearer " + token);
                return myWebClient.DownloadString($"{ APIServer}/webapi/v1/user/vehicles/{vin}/status");
            }
        }

        public static string GetLastTripJson(string token, string vin)
        {
            using (var myWebClient = new WebClient() { Proxy = GetProxy() })
            {
                myWebClient.Headers.Add("Authorization", "Bearer " + token);
                return myWebClient.DownloadString($"{APIServer}/webapi/v1/user/vehicles/{vin}/statistics/lastTrip");
            }
        }

        public static string GetAllTripsJson(string token, string vin)
        {
            using (var myWebClient = new WebClient() { Proxy = GetProxy() })
            {
                myWebClient.Headers.Add("Authorization", "Bearer " + token);

                string answer;

                try
                {
                   answer =  myWebClient.DownloadString($"{APIServer}/webapi/v1/user/vehicles/{vin}/statistics/allTrips");
                }
                catch (WebException)
                {
                    answer = "error";

                }

                return answer;
            }
        }
        public static (string,string) GetSOCJson(string token, string vin)
        {
            using (var myWebClient = new WebClient() { Proxy = GetProxy() })
            {
                myWebClient.Headers.Add("Authorization", "Bearer " + token);
                string _answer, ans2;
                try
                {
                    _answer = myWebClient.DownloadString($"https://www.bmw-connecteddrive.de/api/vehicle/navigation/v1/{vin}");
                    ans2 = myWebClient.DownloadString($"https://www.bmw-connecteddrive.de/api/vehicle/dynamic/v1/{vin}?offset=-120");
                    //Trace.Write("SocData:-120");
                    //Trace.WriteLine(ans2);
                    ////var ans4 = myWebClient.DownloadString($"https://www.bmw-connecteddrive.de/api/vehicle/dynamic/v1/{vin}?offset=-60");
                    //Trace.Write("SocData:-60");
                    //Trace.WriteLine(ans2);
                    //var ans3 = myWebClient.DownloadString($"https://www.bmw-connecteddrive.de/api/vehicle/dynamic/v1/{vin}?offset=-15");
                    //Trace.Write("SocData:-15");
                    //Trace.WriteLine(ans3);

                }
                catch (WebException)
                {
                    _answer = "error";
                    ans2 = "error";
                }
                return (_answer, ans2);
            }
        }

        public static string GettestJson(string token, string vin)
        {
            using (var myWebClient = new WebClient() { Proxy = GetProxy() })
            {
                myWebClient.Headers.Add("Authorization", "Bearer " + token);
                return myWebClient.DownloadString($"{APIServer}/webapi/v1/user/vehicles/{vin}/statistics/allTrips");
            }
        }

        public static (string,int) GetPortalToken(string user, string pw)
        {
            using (var myWebClient = new WebClientNoRedir() { Proxy = GetProxy()  })
            {
                var WS_Data = new NameValueCollection
                {
                    { "client_id", "id" }, //you need to replace it with actual values
                    { "username",user },
                    { "password", pw },
                    { "redirect_uri" , "https%3A%2F%2Fwww.bmw-connecteddrive.com%2Fapp%2Fdefault%2Fstatic%2Fexternal-dispatch.html"  },
                    { "state","state"}, //you need to replace it with actual values
                    { "response_type", "token" },
                    { "scope", "authenticate_user fupo" }
                };
               
                myWebClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                string response = "",
                         expires = "";

                string bodyRaw = "&client_id=id&redirect_uri=https%3A%2F%2Fwww.bmw-connecteddrive.com%2Fapp%2Fdefault%2Fstatic%2Fexternal-dispatch.html&response_type=token&scope=authenticate_user%20fupo&state=state&locale=DE-de&username=email&password=pass";

                
                try
                {
                   // response = myWebClient.UploadString("https://customer.bmwgroup.com/gcdm/oauth/authenticate", WS_Data.ToQueryString(isEscaped: true));
                    response = myWebClient.UploadString("https://customer.bmwgroup.com/gcdm/oauth/authenticate", bodyRaw);
                    var headerCollection = myWebClient.ResponseHeaders;
                    var tokenInside = headerCollection["Location"];

                     response = System.Web.HttpUtility.ParseQueryString(tokenInside)["access_token"];
                     expires = System.Web.HttpUtility.ParseQueryString(tokenInside)["expires_in"];

                }
                catch (System.Exception ex)
                {
                    Trace.Write("TokenRenew");
                    Trace.WriteLine(ex.Message);

                }

                return (response, int.Parse(expires));

            }
        }
    }
    public class WebClientNoRedir : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.AllowAutoRedirect = false;
            return request;
        }
    }
}
