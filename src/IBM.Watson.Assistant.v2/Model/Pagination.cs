/**
* (C) Copyright IBM Corp. 2017, 2022.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using Newtonsoft.Json;

namespace IBM.Watson.Assistant.v2.Model
{
    /// <summary>
    /// The pagination data for the returned objects.
    /// </summary>
    public class Pagination
    {
        /// <summary>
        /// The URL that will return the same page of results.
        /// </summary>
        [JsonProperty("refresh_url", NullValueHandling = NullValueHandling.Ignore)]
        public string RefreshUrl { get; set; }
        /// <summary>
        /// The URL that will return the next page of results.
        /// </summary>
        [JsonProperty("next_url", NullValueHandling = NullValueHandling.Ignore)]
        public string NextUrl { get; set; }
        /// <summary>
        /// The total number of objects that satisfy the request. This total includes all results, not just those
        /// included in the current page.
        /// </summary>
        [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
        public long? Total { get; set; }
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        [JsonProperty("matched", NullValueHandling = NullValueHandling.Ignore)]
        public long? Matched { get; set; }
        /// <summary>
        /// A token identifying the current page of results.
        /// </summary>
        [JsonProperty("refresh_cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string RefreshCursor { get; set; }
        /// <summary>
        /// A token identifying the next page of results.
        /// </summary>
        [JsonProperty("next_cursor", NullValueHandling = NullValueHandling.Ignore)]
        public string NextCursor { get; set; }
    }

}
