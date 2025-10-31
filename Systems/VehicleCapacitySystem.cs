using Colossal.Entities;
using DetailedDescriptions.Helpers;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DetailedDescriptions.Systems
{
    public partial class VehicleCapacitySystem : AssetDescriptionDisplaySystem
    {
        private EntityQuery _transportVehicles;

        protected override void OnCreate()
        {
            base.OnCreate();

            //_transportVehicles = GetEntityQuery(
            //    new EntityQueryDesc()
            //    {
            //        All = new[] { ComponentType.ReadWrite<PublicTransportVehicleData>() }
            //    }
            //);
            _transportVehicles = SystemAPI
                .QueryBuilder()
                .WithAll<PublicTransportVehicleData>()
                .WithNone<TrainCarriageData>()
                .Build();

            GameManager.instance.RegisterUpdater(AddTextToAllDescriptions);
            Mod.log.Info("VehicleCapacitySystem initialized");
        }

        protected override void AddTextToAllDescriptions()
        {
            if (!Setting.Instance.ShowPublicTransportCapacity)
                return;

            Mod.log.Info("VehicleCapacitySystem AddTextToAllDescriptions");
            var allVehiclePrefabs = _transportVehicles.ToEntityArray(Allocator.Temp);
            foreach (Entity entity in allVehiclePrefabs)
            {
                if (!EntityManager.TryGetComponent(entity, out PublicTransportVehicleData data))
                    continue;
                string prefabName = PrefabSystem.GetPrefabName(entity);
                if (
                    !GameManager.instance.localizationManager.activeDictionary.TryGetValue(
                        $"Assets.NAME[{prefabName}]",
                        out string localeName
                    )
                )
                    continue;

                int2 passenger = 0;

                if (
                    EntityManager.HasBuffer<VehicleCarriageElement>(entity)
                    && EntityManager.TryGetBuffer(
                        entity,
                        true,
                        out DynamicBuffer<VehicleCarriageElement> buffer
                    )
                )
                {
                    passenger.x += data.m_PassengerCapacity;
                    passenger.y += data.m_PassengerCapacity;
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        var carriage = buffer[i];
                        if (
                            EntityManager.TryGetComponent(
                                carriage.m_Prefab,
                                out PublicTransportVehicleData data2
                            )
                        )
                        {
                            passenger.x += carriage.m_Count.x * data2.m_PassengerCapacity;
                            passenger.y += carriage.m_Count.y * data2.m_PassengerCapacity;
                        }
                    }
                }
                else
                {
                    passenger.x += data.m_PassengerCapacity;
                    passenger.y += data.m_PassengerCapacity;
                }

                if (EntityManager.TryGetComponent(entity, out TrainEngineData data3))
                {
                    passenger.x *= data3.m_Count.x;
                    passenger.y *= data3.m_Count.y;
                }

                //Mod.log.Info($"{localeName}: {capacity.x},{capacity.y}");
                string amount = $"{passenger.x} - {passenger.y}";
                if (passenger.x == passenger.y)
                    amount = $"{passenger.x}";

                if (
                    (
                        Setting.Instance.AvoidPublicTransportCapacityDuplication
                        && localeName.Contains(amount)
                    )
                    || passenger.x <= 0
                )
                    // Don't add capacity if it's already in the name in some way
                    continue;
                
                string format = Setting.Instance.PublicTransportCapacityFormat;

                string text = format
                    .Replace("{capacity}", LocaleHelper.Translate("InfoPanels.CAPACITY")?.Replace(":", "") ?? "Capacity")
                    .Replace("{passengers}", LocaleHelper.Translate("SelectedInfoPanel.PASSENGERS_TITLE") ?? "Passengers")
                    .Replace("{amount}", amount);

                AddTextToName(prefabName, text);
            }
        }

        protected override void OnUpdate() { }
    }
}
