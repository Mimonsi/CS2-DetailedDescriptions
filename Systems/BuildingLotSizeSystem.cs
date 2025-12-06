using System.Collections.Generic;
using Colossal.Core;
using Colossal.Entities;
using DetailedDescriptions.Helpers;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class BuildingLotSizeSystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _buildingsQuery;
        protected override void OnCreate()
        {
            base.OnCreate();
            _buildingsQuery = GetEntityQuery(new EntityQueryDesc()
            {
                All = new [] { ComponentType.ReadWrite<BuildingData>() }
            });

            MainThreadDispatcher.RegisterUpdater(AddTextToAllDescriptions);
            Mod.log.Info("BuildingLotSizeSystem initialized");
        }

        protected override void AddTextToAllDescriptions()
        {
            if (!Setting.Instance.ShowBuildingLotSizes) return;
            var buildings = _buildingsQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in buildings)
            {
                if (EntityManager.TryGetComponent(entity, out BuildingData buildingData))
                {
                    string prefabName = PrefabSystem.GetPrefabName(entity);
                    
                    int width = buildingData.m_LotSize.x;
                    int depth = buildingData.m_LotSize.y;
                    var lotText = $"Lot Size: {width}x{depth}";
                    if (width == 0 || depth == 0)
                    {
                        //Mod.log.Warn($"Building {prefabName} has Lot Size {width}x{depth}, but one of the dimensions is 0. This should not be possible.");
                        continue;
                    }
                    lotText += $" ({UnitHelper.FormatBuildingLotSize(width)} x {UnitHelper.FormatBuildingLotSize(depth)})";
                    AddTextToDescription(prefabName, lotText);
                }
            }
        } 
    }
}
