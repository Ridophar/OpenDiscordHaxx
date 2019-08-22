﻿using Newtonsoft.Json;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using Discord;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DiscordHaxx
{
    public class CheckerEndpoint : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            Task.Run(() =>
            {
                if (Server.Bots.Count == 0)
                {
                    Send(new CheckerErrorRequest("notokens"));

                    return;
                }

                Send(new CheckerStartedRequest());

                int valid = 0;
                int invalid = 0;
                int total = Server.Bots.Count;
                foreach (var client in new List<DiscordClient>(Server.Bots))
                {
                    BotCheckedRequest req = new BotCheckedRequest()
                                                 { Bot = BotInfo.FromClient(client) };
                    req.Progress.Total = total;

                    try
                    {
                        client.JoinGuild("a");
                    }
                    catch (DiscordHttpException e)
                    {
                        req.Valid = e.Code == DiscordError.UnknownInvite;
                    }
                    catch (JsonReaderException)
                    {
                        Send(new CheckerErrorRequest("ratelimit"));

                        break;
                    }


                    if (req.Valid)
                        valid++;
                    else
                    {
                        Server.Bots.Remove(client);
                        invalid++;
                    }

                    req.Progress.Valid = valid;
                    req.Progress.Invalid = invalid;


                    Send(req);
                }

                if (total > Server.Bots.Count)
                {
                    StringBuilder tokensToAdd = new StringBuilder();
                    foreach (var bot in Server.Bots)
                        tokensToAdd.AppendLine(bot.Token);

                    File.WriteAllText("Tokens-checked.txt", tokensToAdd.ToString());
                }

                Send(new CheckerRequest(CheckerOpcode.Done));
            });
        }
    }
}