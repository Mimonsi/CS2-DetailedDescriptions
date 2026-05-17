using System.Diagnostics;
using Colossal.Core;
using Colossal.Localization;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public abstract partial class AssetDescriptionDisplaySystem : GameSystemBase
    {
        protected PrefabSystem PrefabSystem;
        protected LocalizationManager LocalizationManager;

        protected abstract bool IsEnabled { get; }
        protected abstract void AddTextToAllDescriptions();

        protected override void OnCreate()
        {
            base.OnCreate();
            PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            LocalizationManager = GameManager.instance.localizationManager;

            LocalizationManager.onActiveDictionaryChanged += TriggerUpdate;
            Mod.OnSettingsChanged += TriggerUpdate;
            MainThreadDispatcher.RegisterUpdater(TriggerUpdate);

            Mod.log.Info($"{GetType().Name} initialized");
        }

        private void TriggerUpdate()
        {
            if (!IsEnabled) return;
            var sw = Stopwatch.StartNew();
            AddTextToAllDescriptions();
            Mod.log.Debug($"{GetType().Name} updated in {sw.Elapsed.TotalMilliseconds:0.##}ms");
        }

        protected void AddTextToName(string prefabName, string text, string separator = " ")
        {
            if (LocalizationManager.activeDictionary.TryGetValue($"Assets.NAME[{prefabName}]", out var entry))
            {
                if (string.IsNullOrEmpty(entry)) return;
                if (entry.Contains(text)) return;
                LocalizationManager.activeDictionary.Add($"Assets.NAME[{prefabName}]", $"{entry}{separator}{text}");
            }
        }

        protected void AddTextToDescription(string prefabName, string text, bool debugOutput = false)
        {
            if (LocalizationManager.activeDictionary.TryGetValue($"Assets.DESCRIPTION[{prefabName}]", out var entry))
            {
                if (string.IsNullOrEmpty(entry)) return;
                if (entry.Contains(text)) return;
                LocalizationManager.activeDictionary.Add($"Assets.DESCRIPTION[{prefabName}]", $"{entry}\r\n{text}");
            }
            else if (debugOutput)
            {
                Mod.log.Warn($"Could not get description for prefab: '{prefabName}'");
            }
        }

        protected override void OnUpdate() { }
    }
}
