using System.Diagnostics;
using Colossal.Localization;
using Colossal.Serialization.Entities;
using Game;
using Game.Prefabs;
using Game.SceneFlow;

namespace DetailedDescriptions.Systems
{
    public abstract partial class AssetDescriptionDisplaySystem : GameSystemBase
    {
        protected PrefabSystem PrefabSystem;
        protected LocalizationManager LocalizationManager;

        private static bool _gameLoaded;
        private string _lastLocale;

        protected abstract bool IsEnabled { get; }
        protected abstract void AddTextToAllDescriptions();

        /// <summary>
        /// Override in subclasses that keep a "skip if unchanged" cache, so it can be
        /// reset when locale or settings change (which require a full re-run even if
        /// the entity count is identical).
        /// </summary>
        protected virtual void InvalidateCache() { }

        protected override void OnCreate()
        {
            base.OnCreate();
            PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            LocalizationManager = GameManager.instance.localizationManager;
            _lastLocale = LocalizationManager.activeDictionary.localeID;

            LocalizationManager.onActiveDictionaryChanged += OnDictionaryChanged;
            Mod.OnSettingsChanged += OnSettingsChanged;

            Mod.log.Info($"{GetType().Name} initialized");
        }

        protected override void OnDestroy()
        {
            LocalizationManager.onActiveDictionaryChanged -= OnDictionaryChanged;
            Mod.OnSettingsChanged -= OnSettingsChanged;
            base.OnDestroy();
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (mode != GameMode.Game) return;
            _gameLoaded = true;
            TriggerUpdate();
        }

        private void OnDictionaryChanged()
        {
            // Before the game finishes loading the dictionary changes constantly
            // as assets stream in. Processing each one is wasted work because
            // descriptions aren't visible in the main menu anyway.
            if (!_gameLoaded) return;

            // A genuine locale switch means previously-written texts are now in the
            // wrong language and must be regenerated. Bypass any subclass cache.
            var current = LocalizationManager.activeDictionary.localeID;
            if (_lastLocale != current)
            {
                _lastLocale = current;
                InvalidateCache();
            }

            TriggerUpdate();
        }

        private void OnSettingsChanged()
        {
            // Unit toggles etc. change the rendered text, so existing cached output
            // is stale even if the entity count is unchanged.
            InvalidateCache();
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
