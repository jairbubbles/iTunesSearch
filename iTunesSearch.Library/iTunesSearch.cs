﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using iTunesSearch.Library.Models;

namespace iTunesSearch.Library
{
    /// <summary>
    /// A wrapper for the iTunes search API.  More information here: 
    /// https://www.apple.com/itunes/affiliates/resources/documentation/itunes-store-web-service-search-api.html
    /// </summary>
    public class iTunesSearch
    {
        /// <summary>
        /// The base API url for iTunes search
        /// </summary>
        private string _baseSearchUrl = "https://itunes.apple.com/search?{0}";

        /// <summary>
        /// Get a list of episodes for a given TV show
        /// </summary>
        /// <param name="showName"></param>
        /// <param name="resultLimit"></param>
        /// <returns></returns>
        public async Task<TVEpisodeListResult> GetEpisodesForShow(string showName, int resultLimit = 100)
        {
            var nvc = HttpUtility.ParseQueryString(string.Empty);

            //  Set attributes for a TV season.  
            nvc.Add("term", showName);
            nvc.Add("media", "tvShow");
            nvc.Add("entity", "tvEpisode");
            nvc.Add("attribute", "showTerm");
            nvc.Add("limit", resultLimit.ToString());

            //  Construct the url:
            string apiUrl = string.Format(_baseSearchUrl, nvc.ToString());

            //  Get the list of episodes
            var result = await MakeAPICall<TVEpisodeListResult>(apiUrl);

            return result;
        }

        /// <summary>
        /// Makes an API call and deserializes return value to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="apiCall"></param>
        /// <returns></returns>
        async private static Task<T> MakeAPICall<T>(string apiCall)
        {
            HttpClient client = new HttpClient();

            //  Make an async call to get the response
            var objString = await client.GetStringAsync(apiCall);

            //  Deserialize and return
            return (T)DeserializeObject<T>(objString);
        }

        /// <summary>
        /// Deserializes the JSON string to the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objString"></param>
        /// <returns></returns>
        private static T DeserializeObject<T>(string objString)
        {
            using(var stream = new MemoryStream(Encoding.Unicode.GetBytes(objString)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }
    }
}
