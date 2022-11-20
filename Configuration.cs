using Rocket.API;
using Rocket.Core.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Rocket.Unturned.Items;
using SDG.Unturned;

namespace NLREnforcer
{
    public class Configuration : IRocketPluginConfiguration
    {
        public int RestrictedRadiusFromDeathPosition;
        public int ClearDeathAfterSeconds;
        public bool MessageWhenAttemptToEnterRestrictedZone;
        public string MessageViolation;

        public void LoadDefaults()
        {
            RestrictedRadiusFromDeathPosition = 50;
            ClearDeathAfterSeconds = 900;
            MessageWhenAttemptToEnterRestrictedZone = true;
            MessageViolation = "[<color=red> NLREnforcer </color>] You recently died here, you cannot come closer.";
        }
    }
}
