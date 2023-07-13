using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpaceEngineersVR.Wrappers
{
    public class MySpritesManager
    {
        public object Instance
        {
            get;
        }

        static MySpritesManager()
        {
            Type t = AccessTools.TypeByName("VRage.Render11.Sprites.MySpritesManager");
            acquireDrawMessages = AccessTools.Method(t, "AcquireDrawMessages");
            closeDebugDrawMessages = AccessTools.Method(t, "CloseDebugDrawMessages");
        }

        public MySpritesManager(object instance)
        {
            Instance = instance;
        }

        private static readonly MethodInfo acquireDrawMessages;

        public object AcquireDrawMessages(string textureName)
        {
            return acquireDrawMessages.Invoke(Instance, new object[] { textureName });
        }

        private static readonly MethodInfo closeDebugDrawMessages;

        public object CloseDebugDrawMessages()
        {
            return closeDebugDrawMessages.Invoke(Instance, null);
        }
    }
}
