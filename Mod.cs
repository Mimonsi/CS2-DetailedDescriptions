using System;
using System.IO;
using Colossal.IO.AssetDatabase;
using DetailedDescriptions.Systems;
using Colossal.Logging;
using Colossal.PSI.Environment;
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
            //log.effectivenessLevel = Level.Debug; // TODO: Remove debug
            log.showsStackTraceAboveLevels = Level.Error;

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
            m_Setting.onSettingsApplied += ApplySettingsChanged;
            
            updateSystem.UpdateAt<ZoneLotSizeSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<BuildingLotSizeSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<BuildingWorkplacesSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<SpeedLimitSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<VehicleCapacitySystem>(SystemUpdatePhase.MainLoop);
        }

        private void ApplySettingsChanged(Game.Settings.Setting setting)
        {
            OnSettingsChanged?.Invoke();
            Mod.log.Debug("OnSettingsChanged called by Apply");
            //OnSettingsChanged?.Invoke();
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }

        public static void ReloadActiveLocale()
        {
            GameManager.instance.localizationManager.ReloadActiveLocale();
            OnSettingsChanged?.Invoke();
            Mod.log.Debug("OnSettingsChanged called by ReloadActiveLocale");
            
        }

        public static void ExportAllLocalization()
        {
            var outputFile = Path.Combine(EnvPath.kUserDataPath, "ModsData", "DetailedDescriptionsExport.txt");
            using StreamWriter writer = new StreamWriter(outputFile);
            foreach (var entry in GameManager.instance.localizationManager.activeDictionary.entries)
            {
                writer.Write($"{entry.Key}={entry.Value}\r\n");
            }
            writer.Close();
        }
    }
}