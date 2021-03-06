using System;
using Newtonsoft.Json;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace DoffAndDonAgain.Common {
  public class DoffAndDonAgainConfig {
    public string GameplaySectionTitle = "=== Gameplay Settings ===";

    public string DoffConfigOptions = "--- Doff: Remove armor ---";

    public string EnableDoffToGroundDescription = Constants.EnableDoffToGroundDescription;
    public bool EnableDoffToGround = Constants.DEFAULT_ENABLE_GROUND_DOFF;

    public string EnableDoffToArmorStandDescription = Constants.EnableDoffToArmorStandDescription;
    public bool EnableDoffToArmorStand = Constants.DEFAULT_ENABLE_STAND_DOFF;

    public string SaturationCostPerDoffDescription = Constants.SaturationCostPerDoffDescription;
    public float SaturationCostPerDoff = Constants.DEFAULT_DOFF_COST;

    public string HandsNeededToDoffDescription = Constants.HandsNeededToDoffDescription;
    public int HandsNeededToDoff = Constants.DEFAULT_HANDS_FREE;

    public string DropArmorWhenDoffingToStandDescription = Constants.DropArmorWhenDoffingToStandDescription;
    public bool DropArmorWhenDoffingToStand = Constants.DEFAULT_DROP_ON_STAND_DOFF;

    public string DonConfigOptions = "--- Don: Equip armor from an armor stand ---";

    public string EnableDonDescription = Constants.EnableDonDescription;
    public bool EnableDon = Constants.DEFAULT_ENABLE_DON;

    public string SaturationCostPerDonDescription = Constants.SaturationCostPerDonDescription;
    public float SaturationCostPerDon = Constants.DEFAULT_DON_COST;

    public string HandsNeededToDonDescription = Constants.HandsNeededToDonDescription;
    public int HandsNeededToDon = Constants.DEFAULT_HANDS_FREE;

    public string EnableToolDonningDescription = Constants.EnableToolDonningDescription;
    public bool EnableToolDonning = Constants.DEFAULT_ENABLE_TOOL_DONNING;

    public string DonToolOnlyToActiveHotbarDescription = Constants.DonToolOnlyToActiveHotbarDescription;
    public bool DonToolOnlyToActiveHotbar = Constants.DEFAULT_DON_TOOL_ACTIVE_HOTBAR_ONLY;

    public string DonToolOnlyToHotbarDescription = Constants.DonToolOnlyToHotbarDescription;
    public bool DonToolOnlyToHotbar = Constants.DEFAULT_DON_TOOL_HOTBAR_ONLY;

    public string SwapConfigOptions = "--- Swap: Exchange armor with an armor stand ---";

    public string EnableSwapDescription = Constants.EnableSwapDescription;
    public bool EnableSwap = Constants.DEFAULT_ENABLE_SWAP;

    public string SaturationCostPerSwapDescription = Constants.SaturationCostPerSwapDescription;
    public float SaturationCostPerSwap = Constants.DEFAULT_SWAP_COST;

    public string HandsNeededToSwapDescription = Constants.HandsNeededToSwapDescription;
    public int HandsNeededToSwap = Constants.DEFAULT_HANDS_FREE;

    public static DoffAndDonAgainConfig LoadOrCreateDefault(ICoreAPI api) {
      DoffAndDonAgainConfig config = TryLoadModConfig(api, Constants.FILENAME);

      if (config == null) {
        api.Logger.Notification("{0} configuration file not found. Generating with default settings.", Constants.FILENAME);
        config = new DoffAndDonAgainConfig();
      }

      // Saving here either places the newly generated config file in the ModConfig folder
      // or updates the existing configuration file with new/removed settings
      Save(api, config, Constants.FILENAME);

      Clamp(config);

      return config;
    }

    // Throws exception if the config file exists, but had parsing errors.
    // Returns null if no config file exists.
    private static DoffAndDonAgainConfig TryLoadModConfig(ICoreAPI api, string filename) {
      DoffAndDonAgainConfig config = null;
      try {
        config = api.LoadModConfig<DoffAndDonAgainConfig>(filename);
      }
      catch (JsonReaderException e) {
        api.Logger.Error("Unable to parse config JSON. Correct syntax errors and retry, or delete {0} and load the world again to generate a new configuration file with default settings.", filename);
        throw e;
      }
      catch (Exception e) {
        api.Logger.Error("I don't know what happened. Delete {0} in the config folder and try again.", filename);
        throw e;
      }
      return config;
    }

    public static void Clamp(DoffAndDonAgainConfig config) {
      if (config == null) { return; }
      config.SaturationCostPerDoff = Math.Max(config.SaturationCostPerDoff, Constants.MIN_SATURATION_COST);
      config.SaturationCostPerDon = Math.Max(config.SaturationCostPerDon, Constants.MIN_SATURATION_COST);
      config.SaturationCostPerSwap = Math.Max(config.SaturationCostPerSwap, Constants.MIN_SATURATION_COST);

      config.HandsNeededToDoff = GameMath.Clamp(config.HandsNeededToDoff, Constants.MIN_HANDS_FREE, Constants.MAX_HANDS_FREE);
      config.HandsNeededToDon = GameMath.Clamp(config.HandsNeededToDon, Constants.MIN_HANDS_FREE, Constants.MAX_HANDS_FREE);
      config.HandsNeededToSwap = GameMath.Clamp(config.HandsNeededToSwap, Constants.MIN_HANDS_FREE, Constants.MAX_HANDS_FREE);
    }

    public static void Save(ICoreAPI api, DoffAndDonAgainConfig config, string filename) {
      api.StoreModConfig(config, filename);
    }
  }
}
