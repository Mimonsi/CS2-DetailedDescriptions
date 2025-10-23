using System.Collections.Generic;
using System.Linq;
using Colossal.Entities;
using Colossal.Localization;
using DetailedDescriptions.Helpers;
using Game;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;

namespace DetailedDescriptions.Systems
{
    public partial class VehicleCapacitySystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _transportVehicles;
        protected override void OnCreate()
        {
            base.OnCreate();

            _transportVehicles = GetEntityQuery(new EntityQueryDesc()
            {
                All = new [] { ComponentType.ReadWrite<PublicTransportVehicleData>() }
            });

            GameManager.instance.RegisterUpdater(AddTextToAllDescriptions);
            Mod.log.Info("VehicleCapacitySystem initialized");
        }
        
        protected override void AddTextToAllDescriptions()
        {
            if (!Setting.Instance.ShowPublicTransportCapacity) return;
            
            var allVehiclePrefabs = _transportVehicles.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in allVehiclePrefabs)
            {
                if (EntityManager.TryGetComponent(entity, out PublicTransportVehicleData data))
                {
                    string prefabName = PrefabSystem.GetPrefabName(entity);

                    int capacity = data.m_PassengerCapacity;
                    if (Setting.Instance.AvoidPublicTransportCapacityDuplication && prefabName.Contains(capacity.ToString()))
                    {
                        // Don't add capacity if it's already in the name in some way
                        continue;
                    }
                    AddTextToName(prefabName, $" (cap. {capacity})");
                }
            }
        } 

        protected override void OnUpdate()
        {
        }
    }
}
