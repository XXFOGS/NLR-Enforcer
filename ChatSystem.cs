using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;

namespace NLREnforcer
{
    public static class ChatSystem
    {
        public static CSteamID GetCSteamID(this string id)
        {
            return new CSteamID(ulong.Parse(id));
        }

        public static void sendMessage(this IRocketPlayer caller, String message)
        {
            Regex filter = new Regex(@"<[^>]*>");
            String formatlessMessage = filter.Replace(message, string.Empty);

            if (caller.DisplayName == "Console")
            {
                UnturnedChat.Say(caller, formatlessMessage);
            }
            else
            {
                ChatManager.serverSendMessage(message, UnityEngine.Color.white, null, UnturnedPlayer.FromCSteamID(caller.Id.GetCSteamID()).SteamPlayer(), EChatMode.SAY, null, true);
            }
        }

        public static void sendMessage(this IRocketPlayer caller, String message, String icon)
        {
            Regex filter = new Regex(@"<[^>]*>");
            String formatlessMessage = filter.Replace(message, string.Empty);

            if (caller.DisplayName == "Console")
            {
                UnturnedChat.Say(caller, formatlessMessage);
            }
            else
            {
                ChatManager.serverSendMessage(message, UnityEngine.Color.white, null, UnturnedPlayer.FromCSteamID(caller.Id.GetCSteamID()).SteamPlayer(), EChatMode.SAY, icon, true);
            }
        }

        public static void sendGlobalMessage(String message)
        {
            ChatManager.serverSendMessage(message, UnityEngine.Color.white, null, null, EChatMode.GLOBAL, null, true);
        }

        public static void sendGlobalMessage(String message, String icon)
        {
            ChatManager.serverSendMessage(message, UnityEngine.Color.white, null, null, EChatMode.GLOBAL, icon, true);
        }
    }
}
