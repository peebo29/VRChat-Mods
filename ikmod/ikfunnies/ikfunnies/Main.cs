using HarmonyLib;
using IKMod.API;
using IKMod.IKHandler;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using VRC.UI.Core;
using IKMod.Config;
using RootMotion.FinalIK;
using Photon.Realtime;

namespace IKMod
{
    public class Main : MelonMod
    {
        public new static HarmonyLib.Harmony Harmony { get; private set; }
        public static byte[] tabImage;

        public override void OnApplicationStart()
        {
            ClassInjector.RegisterTypeInIl2Cpp<IKComponent>();
            Harmony = HarmonyInstance;
            HarmonyInstance.Patch(typeof(NetworkManager).GetMethod("OnJoinedRoom"), new HarmonyMethod(typeof(Main).GetMethod(nameof(OnRoomJoin), BindingFlags.NonPublic | BindingFlags.Static)));
            UIAwait(delegate
            {

                using (WebClient wc = new WebClient())
                    tabImage = wc.DownloadData("https://avatars.githubusercontent.com/u/90781829?v=4");
                var tex2D = new Texture2D(256, 256, TextureFormat.ARGB32, false);
                if (!Il2CppImageConversionManager.LoadImage(tex2D, tabImage)) MelonLogger.Error("Couldn't load image");
                var sprite = Sprite.CreateSprite(tex2D, new Rect(0f, 0f, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect, default, false);
                sprite.hideFlags += 32;

                var menu = new QMNestedButton("Menu_Dashboard", "", 1000, 1000, "", "IK Funnies", true);

                new Tab(menu, "Open IK Funnies Menu", sprite);

                new QMToggleButton(menu, 1, 0, "T-Pose", delegate
                {
                    RuntimeConfig.tPose = true;
                }, delegate
                {
                    RuntimeConfig.tPose = false;
                    GetLocalPlayer().GetComponentInChildren<VRIK>().animator.enabled = true;
                }, "Disable's your animations");

                new QMToggleButton(menu, 2, 0, "Twist", delegate
                {
                    RuntimeConfig.twist = true;
                }, delegate
                {
                    RuntimeConfig.twist = false;
                    GetLocalPlayer().GetComponentInChildren<VRIK>().animator.enabled = true;
                    GetLocalPlayer().GetComponentInChildren<VRIK>().fixTransforms = true;
                }, "Disables fix transforms");

                new QMToggleButton(menu, 3, 0, "No Neck", delegate
                {
                    RuntimeConfig.noNeck = true;
                }, delegate
                {
                    RuntimeConfig.noNeck = false;
                    GetLocalPlayer().GetComponentInChildren<VRIK>().solver.hasNeck = true;
                }, "Disables the animator neck");

                new QMToggleButton(menu, 4, 0, "No Chest", delegate
                {
                    RuntimeConfig.noChest = true;
                }, delegate
                {
                    RuntimeConfig.noChest = false;
                    GetLocalPlayer().GetComponentInChildren<VRIK>().solver.hasChest = true;
                }, "Disables the animator neck");

                new QMToggleButton(menu, 1, 1, "Left Arm", delegate
                {
                    RuntimeConfig.leftArm = true;
                }, delegate
                {
                    RuntimeConfig.leftArm = false;
                    GetLocalPlayer().GetComponentInChildren<VRIK>().solver.leftArm.positionWeight = 0f;
                }, "Raise your left arm");

                new QMToggleButton(menu, 2, 1, "Right Arm", delegate
                {
                    RuntimeConfig.rightArm = true;
                }, delegate
                {
                    RuntimeConfig.rightArm = false;
                    GetLocalPlayer().GetComponentInChildren<VRIK>().solver.rightArm.positionWeight = 0f;
                }, "Raise your right arm");
            });
        }

        public static void UIAwait(Action action)
        {
            MelonCoroutines.Start(WaitForUI(action));
        }

        public static IEnumerator WaitForUI(Action action)
        {
            while (VRCUiManager.prop_VRCUiManager_0 == null || UIManager.field_Private_Static_UIManager_0 == null ||
                GameObject.Find("UserInterface").GetComponentInChildren<VRC.UI.Elements.QuickMenu>(true) == null ||
               APIUtils.GetMenuStateControllerInstance() == null || APIUtils.GetMenuPageTemplate() == null)
            {
                yield return null;
            }

            action();
        }

        private static void OnRoomJoin()
        {
            MelonCoroutines.Start(AddComponent());
        }

        public static IEnumerator AddComponent()
        {
            while (VRCPlayer.field_Internal_Static_VRCPlayer_0 == null) yield return null;

            VRCPlayer.field_Internal_Static_VRCPlayer_0.gameObject.AddComponent<IKComponent>();
        }

        public static VRCPlayer GetLocalPlayer()
        {
            return VRCPlayer.field_Internal_Static_VRCPlayer_0;
        }
    }
}
