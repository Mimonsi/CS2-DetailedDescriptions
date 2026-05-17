using Colossal.Entities;
using DetailedDescriptions.Helpers;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class BuildingLotSizeSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _buildingsQuery;
        private int _lastBuildingCount = -1;

        protected override bool IsEnabled => Setting.Instance.ShowBuildingLotSizes;

        protected override void InvalidateCache() => _lastBuildingCount = -1;

        protected override void OnCreate()
        {
            base.OnCreate();
            _buildingsQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<BuildingData>() }
            });
        }

        protected override void AddTextToAllDescriptions()
        {
            var buildings = _buildingsQuery.ToEntityArray(Allocator.Temp);
            if (buildings.Length == _lastBuildingCount)
                return;
            _lastBuildingCount = buildings.Length;

            foreach (Entity entity in buildings)
            {
                if (!EntityManager.TryGetComponent(entity, out BuildingData buildingData))
                    continue;

                int width = buildingData.m_LotSize.x;
                int depth = buildingData.m_LotSize.y;
                if (width == 0 || depth == 0)
                    continue;

                string prefabName = PrefabSystem.GetPrefabName(entity);
                var lotText = $"Lot Size: {width}x{depth} ({UnitHelper.FormatBuildingLotSize(width)} x {UnitHelper.FormatBuildingLotSize(depth)})";
                AddTextToDescription(prefabName, lotText);
            }
        }
    }
}
