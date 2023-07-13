using HarmonyLib;
using System;
using System.Reflection;

namespace SpaceEngineersVR.Wrappers
{
    public static class MyManagers
    {
        static MyManagers()
        {
            Type t = AccessTools.TypeByName("VRage.Render11.Common.MyManagers");
            rwTexturesPool = t.GetField("RwTexturesPool", BindingFlags.Public | BindingFlags.Static);
            spritesManager = t.GetField("SpritesManager", BindingFlags.Public | BindingFlags.Static);
        }

        private static readonly FieldInfo rwTexturesPool;
        public static MyBorrowedRwTextureManager RwTexturesPool => new MyBorrowedRwTextureManager(rwTexturesPool.GetValue(null));

        private static readonly FieldInfo spritesManager;

        public static MySpritesManager SpritesManager => new MySpritesManager(spritesManager.GetValue(null));
    }
}