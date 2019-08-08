﻿using Newtonsoft.Json;

namespace DiscordHaxx
{
    class TokenRequest : BotRequest
    {
        public TokenRequest(BotOpcode op) : base(op)
        { }



        [JsonProperty("id")]
#pragma warning disable CS0649
        private string _id;
#pragma warning restore CS0649

        public ulong Id
        {
            get { return ulong.Parse(_id); }
        }


        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
