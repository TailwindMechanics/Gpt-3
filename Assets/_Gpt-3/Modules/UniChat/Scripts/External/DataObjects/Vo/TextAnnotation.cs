using Newtonsoft.Json;
using System;


namespace Modules.UniChat.External.DataObjects.Vo
{
    [Serializable]
    public class TextAnnotation
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        public override string ToString()
            => $"{Description}";
    }
}