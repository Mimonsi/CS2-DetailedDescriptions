using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;

namespace DetailedDescriptions
{
    
    public enum LengthUnitSetting
    {
        Default,
        Meters,
        Feet
    }
    
    public enum SpeedUnitSetting
    {
        Default,
        Kph,
        Mph
    }
    
    [FileLocation("ModsSettings/DetailedDescriptions/DetailedDescriptions")]
    [SettingsUIGroupOrder(kSettingsGroup, kDescriptionsGroup)]
    [SettingsUIShowGroupName(kSettingsGroup, kDescriptionsGroup)]
    public class Setting : ModSetting
    {
        public static Setting Instance;
        public const string kMainSection = "Settings";
        public const string kSettingsGroup = "GeneralSettings";
        public const string kDescriptionsGroup = "DescriptionTypes";
        public Setting(IMod mod) : base(mod)
        {
        }
        // TODO: Add settings to use Metric, US Customary, or default units

        #region General Settings

        [SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ApplyChanges
        {
            set
            {
                Mod.ApplySettingsChanges();
                //Mod.ReloadActiveLocale();
            }
        }
    
        [SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ReloadActiveLocale
        {
            set
            {
                Mod.ReloadActiveLocale();
            }
        }

        #endregion
    
        #region Description Types
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowBuildingLotSizes { get; set; } = true;
        
        [SettingsUIHideByCondition(typeof(Setting), nameof(ShowBuildingLotSizes), true)]
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public LengthUnitSetting BuildingLotSizeUnit { get; set; } = LengthUnitSetting.Default;
        
        
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowZoneLotSizes { get; set; } = true;
        
        
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowBuildingWorkplaces { get; set; } = true;
        
        
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowRoadSpeedLimit { get; set; } = true;
        
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(ShowRoadSpeedLimit), true)]
        public SpeedUnitSetting RoadSpeedLimitUnit { get; set; } = SpeedUnitSetting.Default;
        
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool ShowPublicTransportCapacity { get; set; } = true;
        
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        public bool AvoidPublicTransportCapacityDuplication { get; set; } = true;
    
        #endregion

    
        public override void SetDefaults()
        {
        
        }
    }
}