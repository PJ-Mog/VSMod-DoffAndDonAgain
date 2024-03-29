using System.Collections.Generic;
using DoffAndDonAgain.Common;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace DoffAndDonAgain.Server {
  public class TransferHandler {
    protected ICoreAPI Api;
    protected bool CanDoffArmorToGround => WorldConfig.DoffArmorToGround.AsBool(Api);
    protected bool CanDoffClothingToGround => WorldConfig.DoffClothingToGround.AsBool(Api);
    protected bool IsDoffToGroundEnabled => CanDoffArmorToGround || CanDoffClothingToGround;

    protected bool CanDoffArmorToEntities => WorldConfig.DoffArmorToEntities.AsBool(Api);
    protected bool CanDoffClothingToEntities => WorldConfig.DoffClothingToEntities.AsBool(Api);
    protected bool IsDoffToEntitiesEnabled => CanDoffArmorToEntities || CanDoffClothingToEntities;
    protected bool CanDropUnplaceableArmor => WorldConfig.DropUnplaceableArmor.AsBool(Api);
    protected bool CanDropUnplaceableClothing => WorldConfig.DropUnplaceableClothing.AsBool(Api);

    protected bool CanDonArmorFromEntities => WorldConfig.DonArmorFromEntities.AsBool(Api);
    protected bool CanDonClothingFromEntities => WorldConfig.DonClothingFromEntities.AsBool(Api);
    protected bool CanDonMiscFromEntities => WorldConfig.DonMiscFromEntities.AsBool(Api);
    protected bool IsDonFromEntitiesEnabled => CanDonArmorFromEntities || CanDonClothingFromEntities || CanDonMiscFromEntities;

    protected bool CanSwapArmorWithEntities => WorldConfig.SwapArmorWithEntities.AsBool(Api);
    protected bool CanSwapClothingWithEntities => WorldConfig.SwapClothingWithEntities.AsBool(Api);
    protected bool IsSwapWithEntitiesEnabled => CanSwapArmorWithEntities || CanSwapClothingWithEntities;

    protected int HandsNeeded => WorldConfig.HandsNeeded.AsInt(Api);
    protected int SaturationRequired => System.Math.Abs(WorldConfig.SaturationCost.AsInt(Api));

    public TransferHandler(DoffAndDonSystem doffAndDonSystem) {
      if (doffAndDonSystem.Side != EnumAppSide.Server) {
        throw new System.Exception($"Tried to create an instance of {nameof(TransferHandler)} Client-side or without a valid {nameof(ICoreAPI)} reference.");
      }
      Api = doffAndDonSystem.Api;

      doffAndDonSystem.OnServerReceivedDoffRequest += OnDoffRequest;
      doffAndDonSystem.OnServerReceivedDonRequest += OnDonRequest;
      doffAndDonSystem.OnServerReceivedSwapRequest += OnSwapRequest;

      doffAndDonSystem.OnAfterServerHandledRequest += OnAfterServerHandledRequest;
    }

    protected void OnDoffRequest(DoffAndDonEventArgs eventArgs) {
      ErrorManager.SetCannotTransferError(eventArgs); // Setting default error code
      switch (eventArgs.TargetType) {
        case EnumTargetType.Nothing:
          TryDoffToGround(eventArgs);
          break;
        case EnumTargetType.EntityAgent:
          TryDoffToEntity(eventArgs);
          break;
      }
    }

    protected void OnDonRequest(DoffAndDonEventArgs eventArgs) {
      ErrorManager.SetCannotTransferError(eventArgs); // Setting default error code
      TryDonFromEntity(eventArgs);
    }

    protected void OnSwapRequest(DoffAndDonEventArgs eventArgs) {
      ErrorManager.SetCannotTransferError(eventArgs); // Setting default error code
      TryToSwapWithEntity(eventArgs);
    }

    protected void TryDoffToGround(DoffAndDonEventArgs eventArgs) {
      if (!IsDoffToGroundEnabled) {
        eventArgs.Successful = false;
        ErrorManager.SetDisabledError(eventArgs);
        return;
      }

      if (CanDoffArmorToGround) {
        DoffArmorToGround(eventArgs);
      }
      if (CanDoffClothingToGround) {
        DoffClothingToGround(eventArgs);
      }
    }

    protected void TryDoffToEntity(DoffAndDonEventArgs eventArgs) {
      if (!IsDoffToEntitiesEnabled) {
        eventArgs.Successful = false;
        ErrorManager.SetDisabledError(eventArgs);
        return;
      }

      if (eventArgs.TargetEntityAgentId == null) {
        eventArgs.Successful = false;
        ErrorManager.SetMustTargetEntityError(eventArgs);
        return;
      }

      EntityAgent targetEntity = eventArgs.ForPlayer?.Entity.World.GetEntityById((long)eventArgs.TargetEntityAgentId) as EntityAgent;
      if (targetEntity == null) {
        eventArgs.Successful = false;
        ErrorManager.SetTargetLostError(eventArgs);
        return;
      }

      if (CanDoffArmorToEntities) {
        DoffArmorToEntity(eventArgs, targetEntity);
      }
      if (CanDoffClothingToEntities) {
        DoffClothingToEntity(eventArgs, targetEntity);
      }
    }

    protected void TryDonFromEntity(DoffAndDonEventArgs eventArgs) {
      if (!IsDonFromEntitiesEnabled) {
        eventArgs.Successful = false;
        ErrorManager.SetDisabledError(eventArgs);
        return;
      }

      if (eventArgs.TargetEntityAgentId == null) {
        eventArgs.Successful = false;
        ErrorManager.SetMustTargetEntityError(eventArgs);
        return;
      }

      EntityAgent targetEntity = eventArgs.ForPlayer?.Entity.World.GetEntityById((long)eventArgs.TargetEntityAgentId) as EntityAgent;
      if (targetEntity == null) {
        eventArgs.Successful = false;
        ErrorManager.SetTargetLostError(eventArgs);
        return;
      }

      if (CanDonArmorFromEntities) {
        DonArmorFromEntity(eventArgs, targetEntity);
      }
      if (CanDonClothingFromEntities) {
        DonClothingFromEntity(eventArgs, targetEntity);
      }
      if (CanDonMiscFromEntities) {
        DonMiscFromEntity(eventArgs, targetEntity);
      }
    }

    protected void TryToSwapWithEntity(DoffAndDonEventArgs eventArgs) {
      if (!IsSwapWithEntitiesEnabled) {
        eventArgs.Successful = false;
        ErrorManager.SetDisabledError(eventArgs);
        return;
      }

      if (eventArgs.TargetEntityAgentId == null) {
        eventArgs.Successful = false;
        ErrorManager.SetMustTargetEntityError(eventArgs);
        return;
      }

      EntityAgent targetEntity = eventArgs.ForPlayer?.Entity.World.GetEntityById((long)eventArgs.TargetEntityAgentId) as EntityAgent;
      if (targetEntity == null) {
        eventArgs.Successful = false;
        ErrorManager.SetTargetLostError(eventArgs);
        return;
      }

      if (CanSwapArmorWithEntities) {
        SwapArmorWithEntity(eventArgs, targetEntity);
      }
      if (CanSwapClothingWithEntities) {
        SwapClothingWithEntity(eventArgs, targetEntity);
      }
    }

    protected void DoffArmorToGround(DoffAndDonEventArgs eventArgs) {
      foreach (var playerArmorSlot in eventArgs.ForPlayer.GetArmorSlots(eventArgs.ClientArmorSlotIds)) {
        DoffToGround(eventArgs, playerArmorSlot);
      }
    }

    protected void DoffClothingToGround(DoffAndDonEventArgs eventArgs) {
      foreach (var playerClothingSlot in eventArgs.ForPlayer.GetClothingSlots(eventArgs.ClientArmorSlotIds)) {
        DoffToGround(eventArgs, playerClothingSlot);
      }
    }

    protected void DoffToGround(DoffAndDonEventArgs eventArgs, ItemSlot slot) {
      bool itemDropped = eventArgs.ForPlayer.InventoryManager.DropItem(slot, true);
      eventArgs.Successful |= itemDropped;

      if (itemDropped && slot.Itemstack?.Collectible is ItemWearable wearable) {
        eventArgs.DroppedArmor.Add(wearable);
      }
    }

    protected void DoffArmorToEntity(DoffAndDonEventArgs eventArgs, EntityAgent targetEntity) {
      bool dropUnplaceable = CanDropUnplaceableArmor && eventArgs.DropUnplaceableArmor;
      Transfer(eventArgs, eventArgs.ForPlayer.GetArmorSlots(eventArgs.ClientArmorSlotIds), targetEntity.GetArmorSlots(), dropUnplaceableToGround: dropUnplaceable);
    }

    protected void DoffClothingToEntity(DoffAndDonEventArgs eventArgs, EntityAgent targetEntity) {
      bool dropUnplaceable = CanDropUnplaceableClothing && eventArgs.DropUnplaceableClothing;
      Transfer(eventArgs, eventArgs.ForPlayer.GetClothingSlots(eventArgs.ClientClothingSlotIds), targetEntity.GetClothingSlots(), dropUnplaceableToGround: dropUnplaceable);
    }

    protected void DonArmorFromEntity(DoffAndDonEventArgs eventArgs, EntityAgent targetEntity) {
      Transfer(eventArgs, targetEntity.GetArmorSlots(), eventArgs.ForPlayer.GetArmorSlots(eventArgs.ClientArmorSlotIds));
    }

    protected void DonClothingFromEntity(DoffAndDonEventArgs eventArgs, EntityAgent targetEntity) {
      Transfer(eventArgs, targetEntity.GetClothingSlots(), eventArgs.ForPlayer.GetClothingSlots(eventArgs.ClientClothingSlotIds));
    }

    protected void Transfer(DoffAndDonEventArgs eventArgs, List<ItemSlot> fromSlots, List<ItemSlot> toSlots, bool dropUnplaceableToGround = false) {
      foreach (var sourceSlot in fromSlots) {
        if (sourceSlot.Empty) {
          continue;
        }

        bool itemMoved = false;
        foreach (var sinkSlot in toSlots) {
          itemMoved = sourceSlot.TryPutInto(eventArgs.ForPlayer.Entity.World, sinkSlot) > 0;
          eventArgs.Successful |= itemMoved;

          if (itemMoved) {
            if (sinkSlot.Itemstack.Collectible is ItemWearable wearable) {
              eventArgs.MovedArmor.Add(wearable);
            }
            break;
          }
        }

        if (dropUnplaceableToGround && !itemMoved) {
          DoffToGround(eventArgs, sourceSlot);
        }
      }
    }

    protected void DonMiscFromEntity(DoffAndDonEventArgs eventArgs, EntityAgent targetEntity) {
      foreach (var sourceSlot in targetEntity.GetMiscDonFromSlots()) {
        ItemSlot sinkSlot;
        switch (eventArgs.ClientDonMiscBehavior) {
          case EnumDonMiscBehavior.ActiveSlotOnly:
            sinkSlot = eventArgs.ForPlayer.InventoryManager.ActiveHotbarSlot;
            break;
          case EnumDonMiscBehavior.Hotbar:
            sinkSlot = eventArgs.ForPlayer.InventoryManager.GetHotbarInventory().GetBestSuitedSlot(sourceSlot).slot;
            break;
          case EnumDonMiscBehavior.Anywhere:
          default:
            // skipSlots is an empty list instead of null due to a crash when in creative mode
            sinkSlot = eventArgs.ForPlayer.InventoryManager.GetBestSuitedSlot(sourceSlot, onlyPlayerInventory: true, skipSlots: new List<ItemSlot>());
            break;
        }

        if (sinkSlot != null) {
          eventArgs.Successful |= sourceSlot.TryPutInto(eventArgs.ForPlayer.Entity.World, sinkSlot) > 0;
        }
      }
    }

    protected void SwapArmorWithEntity(DoffAndDonEventArgs eventArgs, EntityAgent targetEntity) {
      Swap(eventArgs, eventArgs.ForPlayer.GetArmorSlots(eventArgs.ClientArmorSlotIds), targetEntity.GetArmorSlots());
    }

    protected void SwapClothingWithEntity(DoffAndDonEventArgs eventArgs, EntityAgent targetEntity) {
      Swap(eventArgs, eventArgs.ForPlayer.GetClothingSlots(eventArgs.ClientClothingSlotIds), targetEntity.GetClothingSlots());
    }

    protected void Swap(DoffAndDonEventArgs eventArgs, List<ItemSlot> slots, List<ItemSlot> otherSlots) {
      foreach (var slot in slots) {
        bool swapped = false;
        foreach (var otherSlot in otherSlots) {
          if (slot.Empty && otherSlot.Empty) {
            // TryFlipWith would return true, even though nothing is exchanged.
            continue;
          }
          swapped = slot.TryFlipWith(otherSlot);
          eventArgs.Successful |= swapped;
          if (swapped) {
            if (slot.Itemstack?.Collectible is ItemWearable wearable) {
              eventArgs.MovedArmor.Add(wearable);
            }
            if (otherSlot.Itemstack?.Collectible is ItemWearable otherWearable) {
              eventArgs.MovedArmor.Add(otherWearable);
            }
            break;
          }
        }
      }
    }

    protected bool VerifyEnoughHandsFree(DoffAndDonEventArgs eventArgs)
      => HandsChecker.VerifyEnoughHandsFree(eventArgs, eventArgs.ForPlayer?.Entity, HandsNeeded);

    protected void OnAfterServerHandledRequest(DoffAndDonEventArgs eventArgs) {
      if (eventArgs.Successful) {
        eventArgs.ForPlayer.Entity.GetBehavior<EntityBehaviorHunger>()?.ConsumeSaturation(System.Math.Max(SaturationRequired, eventArgs.SaturationCost));
      }
    }
  }
}
