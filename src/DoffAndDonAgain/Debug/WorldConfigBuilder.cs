#if DEBUG

using System;
using DoffAndDonAgain.Common;
using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace DoffAndDonAgain.Build {
  public class WorldConfigBuilder : ModSystem {
    public override bool ShouldLoad(EnumAppSide forSide) => forSide == EnumAppSide.Server;
    public override void Start(ICoreAPI api) {
      base.Start(api);
      var filenameAndPath = Environment.GetEnvironmentVariable("WORLDCONFIG");
      if (filenameAndPath != null) {
        api.StoreModConfig(new WorldConfigFile(), filenameAndPath);
      }
    }

    private class WorldConfigFile {
      [JsonProperty]
      private readonly PlayStyle[] PlayStyles = [];

      [JsonProperty]
      private readonly WorldConfigurationAttribute[] WorldConfigAttributes = [
        WorldConfig.AllowArmorStands,
        WorldConfig.SaturationCost,
        WorldConfig.HandsNeeded,

        //WorldConfig.DoffSpacer,
        WorldConfig.DoffArmorToGround,
        WorldConfig.DropUnplaceableArmor,
        WorldConfig.DoffClothingToGround,
        WorldConfig.DropUnplaceableClothing,

        //WorldConfig.MannequinSpacer,
        WorldConfig.AllowMannequins
      ];
    }
  }
}
#endif
