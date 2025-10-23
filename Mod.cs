using System;
using Colossal.IO.AssetDatabase;
using DetailedDescriptions.Systems;
using Colossal.Logging;
using DetailedDescriptions.Helpers;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace DetailedDescriptions
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(DetailedDescriptions)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);
        
        private Setting m_Setting;
        public static event Action? OnSettingsChanged;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");
            
            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            foreach (var item in new LocaleHelper("DetailedDescriptions.Locale.json").GetAvailableLanguages())
            {
                GameManager.instance.localizationManager.AddSource(item.LocaleId, item);
            }
            
            AssetDatabase.global.LoadSettings(nameof(DetailedDescriptions), m_Setting, new Setting(this));
            Setting.Instance = m_Setting;
            
            updateSystem.UpdateAt<ZoneLotSizeSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<BuildingLotSizeSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<BuildingWorkplacesSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<SpeedLimitSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<VehicleCapacitySystem>(SystemUpdatePhase.MainLoop);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }

        public static void ApplySettingsChanges()
        {
            OnSettingsChanged?.Invoke();
        }

        public static void ReloadActiveLocale()
        {
            GameManager.instance.localizationManager.ReloadActiveLocale();
        }
    }
}