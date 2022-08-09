/**
* (C) Copyright IBM Corp. 2022.
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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace IBM.Watson.Discovery.v2.Model
{
    /// <summary>
    /// List of words to filter out of text that is submitted in queries.
    /// </summary>
    public class StopWordList
    {
        /// <summary>
        /// List of stop words.
        /// </summary>
        [JsonProperty("stopwords", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Stopwords { get; set; }
    }

}
