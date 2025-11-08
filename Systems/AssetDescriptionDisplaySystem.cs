using Colossal.Localization;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI.Menu;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class AssetDescriptionDisplaySystem : GameSystemBase
    {
        protected PrefabSystem PrefabSystem;
        protected LocalizationManager LocalizationManager;
        protected override void OnCreate()
        {
            base.OnCreate();
            PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            LocalizationManager = GameManager.instance.localizationManager;
            LocalizationManager.onActiveDictionaryChanged += OnActiveDictionaryChanged;
            Mod.OnSettingsChanged += OnSettingsChanged;
        }

        protected virtual void AddTextToAllDescriptions()
        {

        }

        protected void AddTextToName(string prefabName, string text, string separator = " ")
        {
            if (LocalizationManager.activeDictionary.TryGetValue($"Assets.NAME[{prefabName}]", out var entry))
            {
                if (string.IsNullOrEmpty(entry)) return;
                string newDescription = $"{entry}{separator}{text}";
                if (entry.Contains(text)) return;
                LocalizationManager.activeDictionary.Add($"Assets.NAME[{prefabName}]", newDescription);
            }
        }

        protected void AddTextToDescription(string prefabName, string text)
        {
            if (LocalizationManager.activeDictionary.TryGetValue($"Assets.DESCRIPTION[{prefabName}]", out var entry))
            {
                if (string.IsNullOrEmpty(entry)) return;
                string newDescription = $"{entry}\r\n{text}";
                if (entry.Contains(text)) return;
                LocalizationManager.activeDictionary.Add($"Assets.DESCRIPTION[{prefabName}]", newDescription);
            }
        }
    
        private void OnSettingsChanged()
        {
            AddTextToAllDescriptions();
        }

        private string lastLocale = string.Empty;

        private void OnActiveDictionaryChanged()
        {
            string currentLocale = GameManager
                .instance
                .localizationManager
                .activeDictionary
                .localeID;
            //if (lastLocale == currentLocale)
            //    return;
            lastLocale = currentLocale;
#if DEBUG
            Mod.log.Debug("OnActiveDictionaryChanged");
#endif
            AddTextToAllDescriptions();
        }
        
        
        protected override void OnUpdate()
        {
        
        }
    }
}