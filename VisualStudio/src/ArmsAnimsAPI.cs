namespace PastimeReading
{
    public static class CustomArmsAnimation
    {
        private static Animator currentAnimator;
        private static Animator animatorPreset;
        private static Animator vanillaAnimator;

        private static GameObject animatorHolder;
        private static GameObject objectToAppendTo;
        private static GameObject propPointRight;
        private static GameObject propPointLeft;

        private static GameObject toolRight;
        private static GameObject toolLeft;




        //private static string objectToAppendToName = "NEW_FPHand_Rig/GAME_DATA";

        public static void Register(AssetBundle assBun, string mainBundleObject) // setup new animator
        {
            animatorHolder = assBun.LoadAsset<GameObject>(mainBundleObject);
            if (!animatorHolder) return;
            animatorHolder = UnityEngine.Object.Instantiate(animatorHolder);
            UnityEngine.Object.DontDestroyOnLoad(animatorHolder);
            animatorHolder.active = false;

            objectToAppendTo = GameManager.GetTopLevelCharacterFpsPlayer()?.transform.Find("NEW_FPHand_Rig/GAME_DATA")?.gameObject;
            propPointRight = objectToAppendTo.transform.FindChild("Origin/HipJoint/Chest_Joint/Camera_Weapon_Offset/Shoulder_Joint/Shoulder_Joint_Offset/Right_Shoulder_Joint_Offset/RightClavJoint/RightShoulderJoint/RightElbowJoint/RightWristJoint/RightPalm/right_prop_point")?.gameObject;
            propPointLeft = objectToAppendTo.transform.FindChild("Origin/HipJoint/Chest_Joint/Camera_Weapon_Offset/Shoulder_Joint/Shoulder_Joint_Offset/Left_Shoulder_Joint_Offset/LeftClavJoint/LeftShoulderJoint/LeftElbowJoint/LeftWristJoint/LeftPalm/left_prop_point")?.gameObject;
            animatorPreset = animatorHolder.GetComponent<Animator>();
            vanillaAnimator = objectToAppendTo.transform.GetParent().GetComponent<Animator>();
        }

        public static void AppendTool(AssetBundle assBun, string objectToSearch, bool rightHand)
        {
            GameObject tool = assBun.LoadAsset<GameObject>(objectToSearch);
            if (!tool) return;
            tool = UnityEngine.Object.Instantiate(tool);

            tool.layer = LayerMask.NameToLayer("Weapon"); // 23
            tool.GetComponent<MeshRenderer>().material.shader = Shader.Find("Shader Forge/TLD_StandardSkinned");
            tool.transform.SetParent(rightHand ? propPointRight.transform : propPointLeft.transform);
            tool.transform.localPosition = Vector3.zero;
            tool.transform.localEulerAngles = Vector3.zero;
            tool.active = false;
            if (rightHand) toolRight = tool;
            else toolLeft = tool;
        }

        public static void Activate() // kill existing custom animator and enable this one
        {
            if (!objectToAppendTo) return;
            Animator temp = objectToAppendTo.GetComponent<Animator>();
            if (!currentAnimator || (temp && currentAnimator != temp))
            {
                if (temp) UnityEngine.Object.Destroy(temp);
                currentAnimator = objectToAppendTo.AddComponent<Animator>();
                currentAnimator.enabled = false;
                currentAnimator.runtimeAnimatorController = animatorPreset.runtimeAnimatorController;
                currentAnimator.avatar = animatorPreset.avatar;
            }

            vanillaAnimator.enabled = false;
            currentAnimator.enabled = true;

        }

        public static void Done()
        {
            vanillaAnimator.enabled = true;
            currentAnimator.enabled = false;
        }

    }
}
