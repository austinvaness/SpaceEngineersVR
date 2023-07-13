﻿using HarmonyLib;
using SharpDX.DXGI;
using SpaceEngineersVR.Util;
using System;
using System.Reflection;
using System.Threading.Tasks;
using VRageMath;
using VRageRender;
using VRageRender.Messages;

namespace SpaceEngineersVR.Wrappers
{
    public static class MyRender11
    {
        public static Vector2I Resolution
        {
            get => (Vector2I)resolution.GetValue(null);
            set => SetResolution(value);
        }

        static MyRender11()
        {
            Type t = AccessTools.TypeByName("VRageRender.MyRender11");
            Type iRtvBindable = AccessTools.TypeByName("VRage.Render11.Resources.IRtvBindable");

            m_debugOverrides = AccessTools.Field(t, "m_debugOverrides");
            m_rc = AccessTools.Field(t, "m_rc");
            settings = AccessTools.Field(t, "Settings");
            m_settings = AccessTools.Field(t, "m_settings");
            m_drawScene = AccessTools.Field(t, "m_drawScene");
            m_mainSpritesTask = AccessTools.Field(t, "m_mainSpritesTask");

            setupCameraMatricesDel = (Action<MyRenderMessageSetCameraViewMatrix>)Delegate.CreateDelegate(typeof(Action<MyRenderMessageSetCameraViewMatrix>), AccessTools.Method(t, "SetupCameraMatrices"));
            createScreenResourcesDel = (Action)Delegate.CreateDelegate(typeof(Action), AccessTools.Method(t, "CreateScreenResources"));
            fullDrawScene = (Action<bool>)Delegate.CreateDelegate(typeof(Action<bool>), AccessTools.Method(t, "FullDraw"));

            processMessageQueueDel = (Action)Delegate.CreateDelegate(typeof(Action), AccessTools.Method(t, "ProcessMessageQueue"));
            processUpdatesDel = (Action)Delegate.CreateDelegate(typeof(Action), AccessTools.Method(t, "ProcessUpdates"));
            updateGameSceneDel = (Action)Delegate.CreateDelegate(typeof(Action), AccessTools.Method(t, "UpdateGameScene"));

            backbuffer = AccessTools.Field(t, "<Backbuffer>k__BackingField");
            resolution = AccessTools.Field(t, "m_resolution");

            drawGameScene = AccessTools.Method(t, "DrawGameScene");
            resizeSwapChain = AccessTools.Method(t, "ResizeSwapchain");
            consumeMainSprites = AccessTools.Method(t, "ConsumeMainSprites");
            scaleMainViewport = AccessTools.Method(t, "ScaleMainViewport");
            renderMainSprites = AccessTools.Method(t, "RenderMainSprites", new Type[] { iRtvBindable, typeof(MyViewport), typeof(MyViewport), typeof(Vector2), typeof(MyViewport?) });
            renderMainSpritesWorker = AccessTools.Method(t, "RenderMainSpritesWorker");

            get_deviceInstance = AccessTools.Property(t, "DeviceInstance").GetGetMethod(true);
            viewportResolution = AccessTools.Property(t, "ViewportResolution");

            environment = AccessTools.Field(t, "Environment");
            environment_matrices = AccessTools.Field("VRageRender.MyEnvironment:Matrices");
        }

        private static readonly FieldInfo backbuffer;

        public static MyBackbuffer Backbuffer
        {
            get => new MyBackbuffer(backbuffer.GetValue(null));
            set => MyBackbuffer.SetBackbufferValues(value, backbuffer);
        }

        private static readonly FieldInfo m_debugOverrides;
        public static MyRenderDebugOverrides DebugOverrides => (MyRenderDebugOverrides)m_debugOverrides.GetValue(null);

        private static readonly FieldInfo settings;
        public static MyRenderSettings Settings
        {
            get => (MyRenderSettings)settings.GetValue(null);
            set => settings.SetValue(null, value);
        }

        private static readonly FieldInfo m_settings;
        public static MyRenderDeviceSettings m_Settings
        {
            get => (MyRenderDeviceSettings)m_settings.GetValue(null);
            set => settings.SetValue(null, value);
        }

        private static readonly FieldInfo m_drawScene;
        public static bool m_DrawScene => (bool)m_drawScene.GetValue(null);


        private static readonly MethodInfo resizeSwapChain;
        public static void ResizeSwapChain(int width, int height)
        {
            resizeSwapChain.Invoke(null, new object[] { width, height });
        }

        private static readonly Action<MyRenderMessageSetCameraViewMatrix> setupCameraMatricesDel;
        public static void SetupCameraMatrices(MyRenderMessageSetCameraViewMatrix message)
        {
            setupCameraMatricesDel.Invoke(message);
        }

        private static readonly Action processMessageQueueDel;
        public static void ProcessMessageQueue()
        {
            processMessageQueueDel.Invoke();
        }

        private static readonly Action processUpdatesDel;
        public static void ProcessUpdates()
        {
            processUpdatesDel.Invoke();
        }

        private static readonly Action createScreenResourcesDel;
        public static void CreateScreenResources()
        {
            createScreenResourcesDel.Invoke();
        }

        private static readonly Action updateGameSceneDel;
        public static void UpdateGameScene()
        {
            updateGameSceneDel.Invoke();
        }

        private static readonly FieldInfo resolution;
        private static void SetResolution(Vector2I vector)
        {
            resolution.SetValue(null, vector);
        }


        private static readonly Action<bool> fullDrawScene;
        public static void FullDrawScene(bool draw = false)
        {
            fullDrawScene.Invoke(draw);
        }

        private static readonly MethodInfo drawGameScene;
        public static void DrawGameScene(BorrowedRtvTexture renderTarget, out object debugAmbientOcclusion)
        {
            object[] args = new object[] { renderTarget.Instance, null };
            drawGameScene.Invoke(null, args);
            debugAmbientOcclusion = args[1];
        }

        private static readonly MethodInfo get_deviceInstance;
        public static Device1 DeviceInstance => (Device1)get_deviceInstance.Invoke(null, new object[0]);

        private static readonly FieldInfo m_rc;
        public static MyRenderContext RC => new MyRenderContext(m_rc.GetValue(null));

        private static readonly FieldInfo environment;
        private static readonly FieldInfo environment_matrices;
        public static EnvironmentMatrices Environment_Matrices
        {
            get
            {
                object obj = environment_matrices.GetValue(environment.GetValue(null));
                obj.TransmuteTo(new EnvironmentMatrices());
                return (EnvironmentMatrices)obj;
            }
        }

        private static readonly MethodInfo consumeMainSprites;

        public static void ConsumeMainSprites()
        {
            consumeMainSprites.Invoke(null, new object[0]);
        }

        private static readonly MethodInfo renderMainSprites;

        public static void RenderMainSprites()
        {
            MyViewport myViewport = new MyViewport(ViewportResolution.X, ViewportResolution.Y);
            Vector2 size = ViewportResolution;
            renderMainSprites.Invoke(null, new object[] { Backbuffer.Instance, ScaleMainViewport(myViewport), myViewport, size, null });
        }

        private static readonly MethodInfo renderMainSpritesWorker;

        public static void RenderMainSpritesWorker(object rtv, MyViewport viewportBound, MyViewport viewportFull, Vector2 size, object defaultMessages, object debugMessages, MyViewport? targetRegion = null)
        {
            renderMainSpritesWorker.Invoke(null, new object[] { rtv, viewportBound, viewportFull, size, defaultMessages, debugMessages, targetRegion });
        }

        private static readonly PropertyInfo viewportResolution;

        public static Vector2I ViewportResolution
        {
            get => (Vector2I)viewportResolution.GetValue(null);
            set => viewportResolution.SetValue(null, value);
        }

        private static readonly MethodInfo scaleMainViewport;

        public static MyViewport ScaleMainViewport(MyViewport viewport)
        {
            return (MyViewport)scaleMainViewport.Invoke(null, new object[] { viewport });
        }

        private static readonly FieldInfo m_mainSpritesTask;

        public static ParallelTasks.Task m_MainSpritesTask
        {
            get => (ParallelTasks.Task)m_mainSpritesTask.GetValue(null);
            set => m_mainSpritesTask.SetValue(null, value);
        }
    }

    public static class MyImmediateRC
    {
        public static MyRenderContext RC => MyRender11.RC;
    }

}