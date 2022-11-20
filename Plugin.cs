using System;
using System.Timers;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Plugins;
using Rocket.Core.Logging;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using SDG;
using UnityEngine;
using UnityEngine.Events;
using UP = Rocket.Unturned.Player.UnturnedPlayer;
using Rocket.API.Serialisation;
using Rocket.Unturned.Chat;
using SDG.Provider;
using Logger = Rocket.Core.Logging.Logger;

namespace NLREnforcer
{
    public class EnforcerPlugin : RocketPlugin<Configuration>
    {
        public static EnforcerPlugin Instance;
        public string Creator = "XXFOGS";
        public string PluginName = "NLREnforcer";
        public string Version = "1.0.0";

        public static IDictionary<CSteamID, PlayerDeath> DeathDictionary = new Dictionary<CSteamID, PlayerDeath>();
        public static IDictionary<CSteamID, Vector3> LastPosDictionary = new Dictionary<CSteamID, Vector3>();

        protected override void Load()
        {
            Instance = this;

            Logger.Log($"{PluginName} by {Creator} has been loaded! Version: {Version}");
            UnturnedPlayerEvents.OnPlayerUpdatePosition += onPositionUpdate;
            UnturnedPlayerEvents.OnPlayerDeath += onPlayerDeath;
            UnturnedPlayerEvents.OnPlayerRevive += onPlayerRespawn;
            
        }

        protected override void Unload()
        {
            Logger.Log($"{PluginName} has been unloaded");
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= onPositionUpdate;
            UnturnedPlayerEvents.OnPlayerDeath -= onPlayerDeath;
            UnturnedPlayerEvents.OnPlayerRevive -= onPlayerRespawn;
        }

        void FixedUpdate()
        {
            foreach (var death in DeathDictionary.ToList())
            {
                if ((DateTimeOffset.Now.ToUnixTimeSeconds() - death.Value.DeathTime) > Configuration.Instance.ClearDeathAfterSeconds)
                {
                    DeathDictionary.Remove(death.Key);
                }
            }
        }

        private void onPositionUpdate(UnturnedPlayer player, Vector3 newPos)
        {
            if (!DeathDictionary.ContainsKey(player.CSteamID)) return;

            if (Vector3.Distance(DeathDictionary[player.CSteamID].DeathPosition, newPos) < Configuration.Instance.RestrictedRadiusFromDeathPosition || (player.IsInVehicle && (Vector3.Distance(DeathDictionary[player.CSteamID].DeathPosition, newPos) < (Configuration.Instance.RestrictedRadiusFromDeathPosition + 5)))) 
            {
                if (player.IsInVehicle) VehicleManager.forceRemovePlayer(player.CSteamID);
                player.Teleport(LastPosDictionary[player.CSteamID], (player.Rotation > 180) ? player.Rotation - 180 : player.Rotation + 180);
                if (Configuration.Instance.MessageWhenAttemptToEnterRestrictedZone) player.sendMessage(Configuration.Instance.MessageViolation);
            }

            LastPosDictionary[player.CSteamID] = newPos;
        }

        private void onPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb lim, CSteamID murderer)
        {
            if (player.HasPermission("nlrenforcer.bypass")) return;

            if (DeathDictionary.ContainsKey(player.CSteamID)) DeathDictionary.Remove(player.CSteamID);

            DeathDictionary.Add(player.CSteamID, new PlayerDeath(player.CSteamID, player.Position, DateTimeOffset.Now.ToUnixTimeSeconds()));
        }

        private void onPlayerRespawn(UnturnedPlayer player, Vector3 position, byte angle)
        {
            if (!DeathDictionary.ContainsKey(player.CSteamID)) return;

            if (Vector3.Distance(DeathDictionary[player.CSteamID].DeathPosition, position) < Configuration.Instance.RestrictedRadiusFromDeathPosition) DeathDictionary.Remove(player.CSteamID);
        }

        }

        public class PlayerDeath
        {
            public CSteamID SteamID { get; set; }
            public Vector3 DeathPosition { get; set; }
            public long DeathTime { get; set; }

            public PlayerDeath(CSteamID steamid, Vector3 deathpos, long deathtime)
            {
                SteamID = steamid;
                DeathPosition = deathpos;
                DeathTime = deathtime;
            }
        }
    }
}
