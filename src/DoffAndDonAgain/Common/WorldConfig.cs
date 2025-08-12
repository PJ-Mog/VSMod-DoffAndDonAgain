using Vintagestory.API.Common;

namespace DoffAndDonAgain.Common {
  public static class WorldConfig {
    // GENERAL
    public const string WorldConfigCategory = Constants.MOD_ID;
    public static readonly WorldConfigurationAttribute AllowArmorStands = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(AllowArmorStands),
      Default = true.ToString()
    };

    public static readonly WorldConfigurationAttribute SaturationCost = new() {
      DataType = EnumDataType.IntInput,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(SaturationCost),
      Default = 0.ToString()
    };

    public static readonly WorldConfigurationAttribute HandsNeeded = new() {
      DataType = EnumDataType.IntRange,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(HandsNeeded),
      Default = 2.ToString(),
      Min = 0,
      Max = 2,
      Step = 1
    };

    // MANNEQUINS
    public static readonly WorldConfigurationAttribute AllowMannequins = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(AllowMannequins),
      Default = true.ToString()
    };

    // DOFF
    public static readonly WorldConfigurationAttribute DoffArmorToEntities = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DoffArmorToEntities),
      Default = true.ToString()
    };

    public static readonly WorldConfigurationAttribute DoffArmorToGround = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DoffArmorToGround),
      Default = true.ToString()
    };

    public static readonly WorldConfigurationAttribute DropUnplaceableArmor = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DropUnplaceableArmor),
      Default = false.ToString()
    };

    public static readonly WorldConfigurationAttribute DoffClothingToEntities = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DoffClothingToEntities),
      Default = true.ToString()
    };

    public static readonly WorldConfigurationAttribute DoffClothingToGround = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DoffClothingToGround),
      Default = false.ToString()
    };

    public static readonly WorldConfigurationAttribute DropUnplaceableClothing = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DropUnplaceableClothing),
      Default = false.ToString()
    };

    // DON
    public static readonly WorldConfigurationAttribute DonArmorFromEntities = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DonArmorFromEntities),
      Default = true.ToString()
    };

    public static readonly WorldConfigurationAttribute DonClothingFromEntities = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DonClothingFromEntities),
      Default = true.ToString()
    };

    public static readonly WorldConfigurationAttribute DonMiscFromEntities = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(DonMiscFromEntities),
      Default = true.ToString()
    };

    public static readonly WorldConfigurationAttribute SwapArmorWithEntities = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(SwapArmorWithEntities),
      Default = true.ToString()
    };

    public static readonly WorldConfigurationAttribute SwapClothingWithEntities = new() {
      DataType = EnumDataType.Bool,
      Category = WorldConfigCategory,
      Code = Constants.MOD_ID + nameof(SwapClothingWithEntities),
      Default = true.ToString()
    };
  }

  public static class WorldConfigExtensions {
    public static bool AsBool(this WorldConfigurationAttribute attribute, ICoreAPI api) {
      switch (attribute.DataType) {
        case EnumDataType.Bool:
          return api.World.Config.GetBool(attribute.Code, (bool)attribute.TypedDefault);
        default:
          LogError(attribute, api, typeof(bool));
          return default;
      }
    }

    public static int AsInt(this WorldConfigurationAttribute attribute, ICoreAPI api) {
      switch (attribute.DataType) {
        case EnumDataType.IntInput:
        case EnumDataType.IntRange:
          return api.World.Config.GetInt(attribute.Code, (int)attribute.TypedDefault);
        default:
          LogError(attribute, api, typeof(int));
          return default;
      }
    }

    public static double AsDouble(this WorldConfigurationAttribute attribute, ICoreAPI api) {
      switch (attribute.DataType) {
        case EnumDataType.DoubleInput:
          return api.World.Config.GetDecimal(attribute.Code, (float)attribute.TypedDefault);
        default:
          LogError(attribute, api, typeof(double));
          return default;
      }
    }

    public static float AsFloat(this WorldConfigurationAttribute attribute, ICoreAPI api) {
      return (float)attribute.AsDouble(api);
    }

    public static string AsString(this WorldConfigurationAttribute attribute, ICoreAPI api) {
      switch (attribute.DataType) {
        case EnumDataType.String:
        case EnumDataType.DropDown:
          return api.World.Config.GetString(attribute.Code, (string)attribute.TypedDefault);
        default:
          LogError(attribute, api, typeof(string));
          return default;
      }
    }

    private static void LogError(WorldConfigurationAttribute attribute, ICoreAPI api, System.Type requestedType) {
      api?.Logger.Error("{0} - Cannot retrieve {1} as a {2}, it is defined as a {3}.", Constants.MOD_ID, attribute.Code, requestedType, attribute.DataType);
    }
  }
}
