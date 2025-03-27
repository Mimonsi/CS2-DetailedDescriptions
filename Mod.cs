﻿using DetailedDescriptions.Systems;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;

namespace DetailedDescriptions
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(DetailedDescriptions)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");
            
            updateSystem.UpdateAt<ZoneLotSizeSystem>(SystemUpdatePhase.MainLoop);
            updateSystem.UpdateAt<BuildingLotSizeSystem>(SystemUpdatePhase.MainLoop);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
        }
    }
}