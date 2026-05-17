using System.Diagnostics;
using Colossal.Core;
using Colossal.Localization;
using Colossal.Serialization.Entities;
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

        private static bool _gameLoaded;

        protected abstract bool IsEnabled { get; }
        protected abstract void AddTextToAllDescriptions();

        protected override void OnCreate()
        {
            base.OnCreate();
            PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            LocalizationManager = GameManager.instance.localizationManager;

            LocalizationManager.onActiveDictionaryChanged += OnDictionaryChanged;
            Mod.OnSettingsChanged += TriggerUpdate;
            GameManager.instance.onGameLoadingComplete += OnGameLoadingComplete;

            Mod.log.Info($"{GetType().Name} initialized");
        }

        private void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            if (mode != GameMode.Game) return;
            //Mod.log.Info("Savegame loading complete");
            _gameLoaded = true;
            TriggerUpdate();
        }

        private void OnDictionaryChanged()
        {
            // Before the game finishes loading the dictionary changes constantly
            // as assets stream in. Processing each one is wasted work because
            // descriptions aren't visible in the main menu anyway.
            if (!_gameLoaded) return;
            TriggerUpdate();
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
