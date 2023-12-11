using HarmonyLib;
using UnityEngine;

namespace TheOtherRoles.Patches
{
    [HarmonyPatch]
    public static class CredentialsPatch
    {

        public static string baseCredentials = $@"<size=130%><color=#ff351f>TheOtherRoles GM H</color></size> v{TheOtherRolesPlugin.Version}";


        public static string contributorsCredentials = "<size=80%>GitHub Contributors: Alex2911, amsyarasyiq, gendelo3</size>";

        [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
        private static class VersionShowerPatch
        {
            static void Postfix(VersionShower __instance)
            {
                var amongUsLogo = GameObject.Find("bannerLogo_AmongUs");
                if (amongUsLogo == null) return;

                var credentials = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(__instance.text);
                credentials.transform.position = new Vector3(0, 0.15f, 0);
                credentials.SetText(ModTranslation.getString("creditsMain"));
                credentials.alignment = TMPro.TextAlignmentOptions.Center;
                credentials.fontSize *= 0.75f;

                var version = UnityEngine.Object.Instantiate<TMPro.TextMeshPro>(credentials);
                version.transform.position = new Vector3(0, -0.25f, 0);
                version.SetText(string.Format(ModTranslation.getString("creditsVersion"), TheOtherRolesPlugin.Version.ToString()));

                credentials.transform.SetParent(amongUsLogo.transform);
                version.transform.SetParent(amongUsLogo.transform);
            }
        }

        [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
        private static class PingTrackerPatch
        {
            static void Postfix(PingTracker __instance)
            {
                __instance.text.alignment = TMPro.TextAlignmentOptions.TopRight;
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                {
                    __instance.text.text = $"{baseCredentials}\n{__instance.text.text}";
                    if (CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead || (!(CachedPlayer.LocalPlayer.PlayerControl == null) && CachedPlayer.LocalPlayer.PlayerControl.isLovers()))
                    {
                        // __instance.transform.localPosition = new Vector3(3.45f, __instance.transform.localPosition.y, __instance.transform.localPosition.z);
                        __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(1.2f, 0.8f, 0f);
                    }
                    else
                    {
                        // __instance.transform.localPosition = new Vector3(4.2f, __instance.transform.localPosition.y, __instance.transform.localPosition.z);
                        __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(2.0f, 0.1f, 0f);
                    }
                }
                else
                {
                    __instance.text.text = $"{baseCredentials}\n{ModTranslation.getString("creditsFull")}\n{__instance.text.text}";
                    // __instance.transform.localPosition = new Vector3(3.5f, __instance.transform.localPosition.y, __instance.transform.localPosition.z);
                    __instance.gameObject.GetComponent<AspectPosition>().DistanceFromEdge = new Vector3(2.7f, 0.0f, 0f);
                }
            }
        }

        [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
        private static class LogoPatch
        {
            static void Prefix(MainMenuManager __instance)
            {
                var name = AmongUs.Data.DataManager.Player.Customization.name;
                // 様子を見て外部サーバーでリストを管理する仕組みに変更する
                if (name == "なかのっち" || name == "ズズ")
                {
                    UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.Abort);
                }
            }

            static void Postfix(MainMenuManager __instance)
            {
                FastDestroyableSingleton<ModManager>.Instance.ShowModStamp();

                var amongUsLogo = GameObject.Find("bannerLogo_AmongUs");
                if (amongUsLogo != null)
                {
                    amongUsLogo.transform.localScale *= 0.6f;
                    amongUsLogo.transform.position += Vector3.up * 0.25f;
                }

                var torLogo = new GameObject("bannerLogo_TOR");
                torLogo.transform.position = Vector3.up;
                var renderer = torLogo.AddComponent<SpriteRenderer>();
                renderer.sprite = Helpers.loadSpriteFromResources("TheOtherRoles.Resources.Banner.png", 300f);

            }
        }
    }
}
