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
        WorldConfig.DoffArmorToEntities,
        WorldConfig.DropUnplaceableArmor,
        WorldConfig.DoffClothingToGround,
        WorldConfig.DoffClothingToEntities,
        WorldConfig.DropUnplaceableClothing,

        //WorldConfig.DonSpacer,
        WorldConfig.DonArmorFromEntities,
        WorldConfig.DonClothingFromEntities,
        WorldConfig.DonMiscFromEntities,

        //WorldConfig.SwapSpacer,
        WorldConfig.SwapArmorWithEntities,
        WorldConfig.SwapClothingWithEntities,

        //WorldConfig.MannequinSpacer,
        WorldConfig.AllowMannequinArmor,
        WorldConfig.AllowMannequinClothing,
        WorldConfig.AllowMannequinHands,
        WorldConfig.AllowMannequinBackpack
      ];
    }
  }
}
#endif
