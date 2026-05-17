using Colossal.Entities;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class BuildingWorkplacesSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _workplacesQuery;

        protected override bool IsEnabled => Setting.Instance.ShowBuildingWorkplaces;

        protected override void OnCreate()
        {
            base.OnCreate();
            _workplacesQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] { ComponentType.ReadWrite<WorkplaceData>() }
            });
        }

        protected override void AddTextToAllDescriptions()
        {
            var buildings = _workplacesQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in buildings)
            {
                if (EntityManager.TryGetComponent(entity, out WorkplaceData workplaceData) && workplaceData.m_MaxWorkers > 0)
                {
                    string prefabName = PrefabSystem.GetPrefabName(entity);
                    AddTextToDescription(prefabName, $"Workplaces: {workplaceData.m_MaxWorkers}");
                }
            }
        }
    }
}
