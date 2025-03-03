/**
* (C) Copyright IBM Corp. 2020, 2022.
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

namespace IBM.Watson.Assistant.v2.Model
{
    /// <summary>
    /// MessageContextStateless.
    /// </summary>
    public class MessageContextStateless
    {
        /// <summary>
        /// Session context data that is shared by all skills used by the assistant.
        /// </summary>
        [JsonProperty("global", NullValueHandling = NullValueHandling.Ignore)]
        public MessageContextGlobalStateless Global { get; set; }
        /// <summary>
        /// Information specific to particular skills used by the assistant.
        /// </summary>
        [JsonProperty("skills", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, MessageContextSkill> Skills { get; set; }
        /// <summary>
        /// An object containing context data that is specific to particular integrations. For more information, see the
        /// [documentation](https://cloud.ibm.com/docs/assistant?topic=assistant-dialog-integrations).
        /// </summary>
        [JsonProperty("integrations", NullValueHandling = NullValueHandling.Ignore)]
        public object Integrations { get; set; }
    }

}
