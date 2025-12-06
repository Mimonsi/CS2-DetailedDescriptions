using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;

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

        #region General Settings

        /*[SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ApplyChanges
        {
            set
            {
                //Mod.ReloadActiveLocale();
                Mod.ApplySettingsChanges();
            }
        }*/
    
        [SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ReloadActiveLocale
        {
            set
            {
                Mod.ReloadActiveLocale();
            }
        }
        
        /*[SettingsUISection(kMainSection, kSettingsGroup)]
        public bool ExportAllLocalization
        {
            set
            {
                Mod.ExportAllLocalization();
            }
        }*/


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
        [SettingsUIDropdown(typeof(Setting), nameof(GetPublicTransportCapacityFormatItems))]
        [SettingsUIHideByCondition(typeof(Setting), nameof(ShowPublicTransportCapacity), true)]
        public string PublicTransportCapacityFormat { get; set; } = " ({capacity} {amount})";

        public DropdownItem<string>[] GetPublicTransportCapacityFormatItems()
        {
            return new DropdownItem<string>[]
            {
                new()
                {
                    value = " ({capacity} {amount})",
                    displayName = "Bus (Capacity 50)"
                },
                new()
                {
                    value = " ({amount} {capacity})",
                    displayName = "Bus (50 Capacity)"
                },
                new()
                {
                    value = " [{amount} {passengers}]",
                    displayName = "Bus [50 Passengers]"
                },
            };
        }
        
        [SettingsUISection(kMainSection, kDescriptionsGroup)]
        [SettingsUIHideByCondition(typeof(Setting), nameof(ShowPublicTransportCapacity), true)]
        public bool AvoidPublicTransportCapacityDuplication { get; set; } = true;
    
        #endregion

    
        public override void SetDefaults()
        {
        
        }
    }
}