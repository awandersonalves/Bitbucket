using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;

namespace BitbucketConsumer
{
    public class ReadFile
    {
        public void Bitbucket()
        {
            try
            {
                string users;
                WebResponse webResponse;
                List<WebResponse> responseList = new List<WebResponse>();
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);


                string filePath = "C:/Users/Elli0t/source/repos/Bitbucket/Bitbucket/Users.txt";
                users = GetFile(filePath);
                users = users.Replace("\r\n", ",");
                string[] userRequest = users.Split(',');

                foreach (string userName in userRequest)
                {
                    if (config.AppSettings.Settings["LastRunTime"] != null)
                    {
                        if (DateTime.Now - DateTime.Parse(config.AppSettings.Settings["LastRunTime"].Value) >= TimeSpan.FromMilliseconds(60000))
                        {
                            webResponse = GetRequest(userName);
                            responseList.Add(webResponse);
                            SaveResponse(webResponse);
                            Thread.Sleep(5000);
                        }
                        else
                        {
                            throw new Exception("Time not exceeding 60 seconds");
                        }
                    }
                    else
                    {
                        webResponse = GetRequest(userName);
                        responseList.Add(webResponse);
                        SaveResponse(webResponse);
                        Thread.Sleep(5000);
                    }
                }                

                if (config.AppSettings.Settings["LastRunTime"] == null)
                {
                    config.AppSettings.Settings.Add("LastRunTime", DateTime.UtcNow.ToString());
                }
                else
                {
                    config.AppSettings.Settings["LastRunTime"].Value = DateTime.UtcNow.ToString();
                }
                config.Save();

                Console.WriteLine("To exit the application, press any key");
                Console.ReadKey(true);
                Thread.Sleep(5000);
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred in the request");
                Console.WriteLine(e.Message);
            }
        }

        public static string GetFile(string filePath)
        {
            string users = "";
            try
            {           
                // Open the text file using a stream reader.
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    // Read the stream as a string, and write the string to the console.
                    users = streamReader.ReadToEnd();
                }                
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return users;
        }

        public static WebResponse GetRequest(string user)
        {
            var webRequest = WebRequest.CreateHttp("https://api.bitbucket.org/2.0/users/" + user);
            webRequest.Method = "GET";

            return webRequest.GetResponse();
        }

        public static void SaveResponse(WebResponse webResponse)
        {
            string responseString;
            using (Stream stream = webResponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                responseString = reader.ReadToEnd();
            }
            string filePath = "C:/Users/Elli0t/source/repos/Bitbucket/Bitbucket/Response.txt";

            StreamWriter streamWriter = new StreamWriter(filePath);

            streamWriter.WriteLine(responseString);

        }
    }
}
