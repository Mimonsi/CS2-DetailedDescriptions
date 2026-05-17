using Colossal.Entities;
using DetailedDescriptions.Helpers;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class SpeedLimitSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _roadsQuery;

        protected override bool IsEnabled => Setting.Instance.ShowRoadSpeedLimit;

        protected override void OnCreate()
        {
            base.OnCreate();
            _roadsQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<RoadData>() }
            });
        }

        protected override void AddTextToAllDescriptions()
        {
            var roads = _roadsQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in roads)
            {
                if (!EntityManager.TryGetComponent(entity, out RoadData roadData))
                    continue;

                // RoadData.m_SpeedLimit is stored at 2x the displayed value
                float speedLimit = roadData.m_SpeedLimit / 2f;
                if (speedLimit == 0)
                    continue;

                string prefabName = PrefabSystem.GetPrefabName(entity);
                AddTextToDescription(prefabName, $"Speed Limit: {UnitHelper.FormatSpeedLimit(speedLimit)}");
            }
        }
    }
}
