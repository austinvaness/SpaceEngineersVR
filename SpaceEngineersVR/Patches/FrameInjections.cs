using HarmonyLib;
using ParallelTasks;
using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using SpaceEngineersVR.GUI;
using SpaceEngineersVR.Plugin;
using SpaceEngineersVR.Wrappers;
using System;
using VRage.Profiler;
using VRageMath;
using VRageRender;

namespace SpaceEngineersVR.Patches
{
    [Util.InitialiseOnStart]
    public static class FrameInjections
    {
        public static bool DisablePresent = false;

        static FrameInjections()
        {
            Type t = AccessTools.TypeByName("VRageRender.MyRender11");

            Common.Plugin.Harmony.Patch(AccessTools.Method(t, "Present"), new HarmonyMethod(typeof(FrameInjections), nameof(Prefix_Present)));

            Common.Plugin.Harmony.Patch(AccessTools.Method(t, "DrawScene"), new HarmonyMethod(typeof(FrameInjections), nameof(Prefix_DrawScene)),
                new HarmonyMethod(typeof(FrameInjections), nameof(Postfix_DrawScene)));

            Common.Plugin.Harmony.Patch(AccessTools.Method(t, "RenderMainSprites", new Type[0]), new HarmonyMethod(typeof(FrameInjections), nameof(Prefix_RenderMainSprites)), 
                new HarmonyMethod(typeof(FrameInjections), nameof(Postfix_RenderMainSprites)));


            Logger.Info("Applied harmony game injections for renderer.");
        }

        private static bool Prefix_RenderMainSprites()
        {
            RawColor4 rawColor = new RawColor4(0, 0, 0, 0);
            MyRender11.RC.ClearRtv(MyRender11.Backbuffer, rawColor);

            if (!MyRender11.Settings.OffscreenSpritesRendering || !MyRender11.m_DrawScene)
            {
                MyViewport myViewport = new MyViewport(MyRender11.ViewportResolution.X, MyRender11.ViewportResolution.Y);
                Vector2 size = MyRender11.ViewportResolution;

                object defaultMessages = MyManagers.SpritesManager.AcquireDrawMessages("DefaultOffscreenTarget");
                object debugMessages = MyManagers.SpritesManager.CloseDebugDrawMessages();

                MyRender11.m_MainSpritesTask = Parallel.Start(delegate 
                {
                    MyRender11.RenderMainSpritesWorker(MyRender11.Backbuffer, MyRender11.ScaleMainViewport(myViewport), myViewport, size, defaultMessages, debugMessages, null);
                    
                }, Parallel.DefaultOptions.WithDebugInfo(MyProfiler.TaskType.GUI, "RenderMainSprites"), WorkPriority.VeryHigh);

                //TODO: Fix this call that is causing a exception
                //MyRender11.ConsumeMainSprites();

                VRGUIManager.Draw();

                /*MyRender11.m_MainSpritesTask = Parallel.Start(delegate {
                    MyRender11.RenderMainSpritesWorker(MyRender11.Backbuffer, MyRender11.ScaleMainViewport(myViewport), myViewport, size, defaultMessages, debugMessages, null);

                }, Parallel.DefaultOptions.WithDebugInfo(MyProfiler.TaskType.GUI, "RenderMainSprites"), WorkPriority.VeryHigh); */
            }
            
            return false;
        }

        private static void Postfix_RenderMainSprites()
        {
            MyRender11.ConsumeMainSprites();
        }

        private static bool Prefix_DrawScene()
        {
            //RawColor4 rawColor = new RawColor4(0, 0, 0, 0);
            //MyRender11.RC.ClearRtv(MyRender11.Backbuffer, rawColor);
            Player.Player.RenderUpdate();
            Player.Player.Headset.RenderUpdate();

            return true;
        }

        private static void Postfix_DrawScene()
        {
            //RawColor4 rawColor = new RawColor4(0, 0, 0, 0);
            //MyRender11.RC.ClearRtv(MyRender11.Backbuffer, rawColor);
            
        }

        private static bool Prefix_Present()
        {
            return !DisablePresent;
        }
    }
}