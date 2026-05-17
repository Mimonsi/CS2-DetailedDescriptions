using System.Collections.Generic;
using System.Linq;
using Colossal.Core;
using Colossal.Localization;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class ZoneLotSizeSystem : AssetDescriptionDisplaySystem
    {
        private PrefabSystem _prefabSystem;
        private EntityQuery _spawnableBuildings;
        private static readonly Dictionary<string, List<(int, int)>> ZoneLots = new();
        protected override void OnCreate()
        {
            base.OnCreate();
            _prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

            _spawnableBuildings = GetEntityQuery(new EntityQueryDesc()
            {
                All = new [] { ComponentType.ReadWrite<SpawnableBuildingData>() }
            });

            MainThreadDispatcher.RegisterUpdater(AddTextToAllDescriptions);
            Mod.log.Info("ZoneLotSizeSystem initialized");
        }
        
        protected override void AddTextToAllDescriptions()
        {
            if (!Setting.Instance.ShowZoneLotSizes) return;
            
            Mod.log.Info("ZoneLotSizeSystem AddTextToAllDescriptions");

            ZoneLots.Clear();
            var allSpawnableBuildings = _spawnableBuildings.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in allSpawnableBuildings)
            {
                if (_prefabSystem.TryGetPrefab(entity, out PrefabBase prefabBase) && prefabBase is not null)
                {
                    BuildingPrefab buildingPrefab = (BuildingPrefab)prefabBase;
                    if (buildingPrefab.TryGet(out SpawnableBuilding sbd))
                    {
                        if (sbd.m_ZoneType == null)
                        {
                            Mod.log.Info("Zone type is null for entity: " + entity);
                            continue;
                        }
                        string zoneName = sbd.m_ZoneType.GetPrefabID().GetName() ?? "";
                        if (!ZoneLots.ContainsKey(zoneName))
                        {
                            ZoneLots[zoneName] = new List<(int, int)>();
                        }
                        ZoneLots[zoneName].Add((buildingPrefab.m_LotWidth, buildingPrefab.m_LotDepth));
                    }
                }
                else
                {
                    Mod.log.Info("Failed to get building prefab for entity: " + entity);
                }
            }

            Mod.log.Trace("Zone Count: " + ZoneLots.Count);
            foreach (var item in ZoneLots)
            {
                string zoneName = item.Key.Replace("ZonePrefab:","");
                if (zoneName == "") continue;
                
                var sortedLots = item.Value
                    .OrderBy(lot => lot.Item1)
                    .ThenBy(lot => lot.Item2)
                    .Select(lot => $"{lot.Item1}x{lot.Item2}")
                    .Distinct()
                    .ToList();

                string lotSize = sortedLots.Count > 1
                    ? string.Join(", ", sortedLots.Take(sortedLots.Count - 1)) + " " + LocalizationProvider.GetLocalizedAnd(LocalizationManager.activeLocaleId) + " " + sortedLots.Last()
                    : sortedLots.FirstOrDefault() ?? "";
                
                string localizedText = LocalizationProvider.GetLocalizedText(LocalizationManager.activeLocaleId).Replace("%data%", lotSize);
                Mod.log.Trace($"Zone {zoneName} Text: {localizedText}");
                
                AddTextToDescription(zoneName, localizedText);
            }
        } 

        protected override void OnUpdate()
        {
        }
    }
}
