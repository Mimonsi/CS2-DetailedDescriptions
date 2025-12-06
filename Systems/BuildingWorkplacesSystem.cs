using System.Collections.Generic;
using Colossal.Core;
using Colossal.Entities;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class BuildingWorkplacesSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _workplacesQuery;
        protected override void OnCreate()
        {
            base.OnCreate();
            _workplacesQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new [] { ComponentType.ReadWrite<WorkplaceData>() }
            });

            MainThreadDispatcher.RegisterUpdater(AddTextToAllDescriptions);
            Mod.log.Info("BuildingWorkplacesSystem initialized");
        }

        protected override void OnUpdate()
        {
            //Entity economyParamsEntity = SystemAPI.GetSingletonEntity<EconomyParameterData>();
        }

        protected override void AddTextToAllDescriptions()
        {
            if (!Setting.Instance.ShowBuildingWorkplaces) return;
            
            var buildings = _workplacesQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in buildings)
            {
                if (EntityManager.TryGetComponent(entity, out WorkplaceData workplaceData))
                {
                    string prefabName = PrefabSystem.GetPrefabName(entity);
                    var workplaceText = $"Workplaces: {workplaceData.m_MaxWorkers}";
                    if (workplaceData.m_MaxWorkers > 0)
                        AddTextToDescription(prefabName, workplaceText);
                }
            }
        } 
    }
}
