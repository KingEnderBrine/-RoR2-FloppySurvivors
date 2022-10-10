using BepInEx;
using RoR2;
using System.Reflection;
using UnityEngine;

[assembly: AssemblyVersion(FloppySurvivors.FloppySurvivorsPlugin.Version)]
namespace FloppySurvivors
{
    [BepInPlugin(GUID, Name, Version)]
    public class FloppySurvivorsPlugin : BaseUnityPlugin
    {
        public const string GUID = "com.KingEnderBrine.FloppySurvivors";
        public const string Name = "Floppy Survivors";
        public const string Version = "1.1.0";

        private void Awake()
        {
            RoR2Application.onLoad += ProcessSurvivors;
        }

        private static void ProcessSurvivors()
        {
            foreach (var survivor in SurvivorCatalog.allSurvivorDefs)
            {
                AddDynamicBone(survivor.bodyPrefab);
                AddDynamicBone(survivor.displayPrefab);
            }
        }

        private static void AddDynamicBone(GameObject prefab)
        {
            if (!prefab)
            {
                return;
            }

            var root = TryFindRoot(prefab);
            if (!root)
            {
                return;
            }

            var dynamicBone = root.gameObject.AddComponent<DynamicBone>();
            dynamicBone.m_Root = root;
            dynamicBone.m_UpdateRate = 60;
            dynamicBone.m_UpdateMode = DynamicBone.UpdateMode.Normal;
            dynamicBone.m_Damping = 0.344F;
            dynamicBone.m_DampingDistrib = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
            dynamicBone.m_Elasticity = 0.1F;
            dynamicBone.m_ElasticityDistrib = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0));
        }

        private static Transform TryFindRoot(GameObject prefab)
        {
            var locator = prefab.GetComponentInChildren<ChildLocator>();
            if (locator)
            {
                var root = locator.FindChild("Base");
                if (root) return root;

                root = locator.FindChild("Root");
                if (root) return root;

                root = locator.FindChild("ROOT");
                if (root) return root;
            }

            return prefab.GetComponentInChildren<SkinnedMeshRenderer>()?.rootBone;
        }
    }
}