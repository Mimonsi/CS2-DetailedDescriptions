using System.Collections.Generic;
using System.Linq;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class ZoneLotSizeSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _spawnableBuildings;
        private readonly Dictionary<string, List<(int, int)>> _zoneLots = new();

        protected override bool IsEnabled => Setting.Instance.ShowZoneLotSizes;

        protected override void OnCreate()
        {
            base.OnCreate();
            _spawnableBuildings = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<SpawnableBuildingData>() }
            });
        }

        protected override void AddTextToAllDescriptions()
        {
            _zoneLots.Clear();
            var allSpawnableBuildings = _spawnableBuildings.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in allSpawnableBuildings)
            {
                if (!PrefabSystem.TryGetPrefab(entity, out PrefabBase prefabBase) || prefabBase is null)
                {
                    Mod.log.Info("Failed to get building prefab for entity: " + entity);
                    continue;
                }

                var buildingPrefab = (BuildingPrefab)prefabBase;
                if (!buildingPrefab.TryGet(out SpawnableBuilding sbd))
                    continue;

                if (sbd.m_ZoneType == null)
                {
                    Mod.log.Info("Zone type is null for entity: " + entity);
                    continue;
                }

                string zoneName = sbd.m_ZoneType.GetPrefabID().GetName() ?? "";
                if (!_zoneLots.ContainsKey(zoneName))
                    _zoneLots[zoneName] = new List<(int, int)>();
                _zoneLots[zoneName].Add((buildingPrefab.m_LotWidth, buildingPrefab.m_LotDepth));
            }

            Mod.log.Trace("Zone Count: " + _zoneLots.Count);
            foreach (var item in _zoneLots)
            {
                string zoneName = item.Key.Replace("ZonePrefab:", "");
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
    }
}
