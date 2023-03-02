using HarmonyLib;

namespace PastimeReading
{
    
    [HarmonyPatch(typeof(PlayerManager), "ShouldSuppressCrosshairs")]
    public class HideCrosshairWhileUsingBook
    {

        private static void Postfix(ref bool __result)
        {
            //if (ReadMain.lockInteraction) __result = true;
            __result = __result || (ReadMain.lockInteraction && Settings.options.disableInteraction);
        }
    }
    /*
    [HarmonyPatch(typeof(InputManager), "ExecuteInteractAction")]
    public class DisableInteractionsWhenUsingBook
    {

        private static bool Prefix()
        {
            if (ReadMain.lockInteraction && Settings.options.disableInteraction)
            {
                return false;
            }
            return true;
        }
    }
    /*
    [HarmonyPatch(typeof(PlayerManager), "UpdateHUDText")]
    public class DisableHoverTextWhenUsingBook
    {

        private static void Prefix()
        {
            if (ReadMain.lockInteraction && Settings.options.disableInteraction)
            {
                if (InterfaceManager.GetPanel<Panel_HUD>().m_HoverTextObject.activeSelf)
                {
                    InterfaceManager.GetPanel<Panel_HUD>().m_HoverTextObject.SetActive(false);
                }
                //return true;
            }
            //return true;
        }
    }
    */

    [HarmonyPatch(typeof(BaseAi), "Update")]
    public class ResetWildLifeAnimatorsWhileSlowdown
    {

        private static void Postfix(BaseAi __instance)
        {
            if (ReadMain.bookIsOpened)
            {
                if (__instance.m_Animator.updateMode != AnimatorUpdateMode.UnscaledTime)
                {
                    __instance.m_Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }
            else
            {
                if (__instance.m_Animator.updateMode != AnimatorUpdateMode.Normal)
                {
                    __instance.m_Animator.updateMode = AnimatorUpdateMode.Normal;
                }
            }
        }
    }

    



}
