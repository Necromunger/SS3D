﻿using SS3D.Data.Generated;
using SS3D.Interactions;
using SS3D.Interactions.Extensions;
using SS3D.Interactions.Interfaces;
using SS3D.Systems.Inventory.Containers;
using SS3D.Systems.Inventory.Items;
using UnityEngine;

namespace SS3D.Systems.Inventory.Interactions
{
    public sealed class StoreInteraction : Interaction
    {
        private readonly AttachedContainer _attachedContainer;

        public StoreInteraction(AttachedContainer attachedContainer)
        {
            _attachedContainer = attachedContainer;
        }

        public override string GetName(InteractionEvent interactionEvent)
        {
            return "Store in " + _attachedContainer.ContainerName;
        }

        public override Sprite GetIcon(InteractionEvent interactionEvent)
        {
            return Icon != null ? Icon : InteractionIcons.Discard;
        }

        public override bool CanInteract(InteractionEvent interactionEvent)
        {
            if (!InteractionExtensions.RangeCheck(interactionEvent))
            {
                return false;
            }

            IInteractionSource source = interactionEvent.Source;

            if(source is IGameObjectProvider sourceGameObjectProvider)
            {
                var hands = sourceGameObjectProvider.GameObject.GetComponentInParent<Hands>();
                if (hands != null && _attachedContainer != null)
                {
                    // Check if source is an item
                    var item = interactionEvent.Source.GetComponent<Item>();
                    if (item != null)
                    {
                        return !hands.SelectedHand.IsEmpty() && CanStore(item, _attachedContainer);
                    }
                }
            }

            return false;
        }

        private bool CanStore(Item item, AttachedContainer target)
        {
            return target.CanContainItem(item);
        }

        public override bool Start(InteractionEvent interactionEvent, InteractionReference reference)
        {
            IInteractionSource source = interactionEvent.Source;
            if (source is IGameObjectProvider sourceGameObjectProvider)
            {
                var hands = sourceGameObjectProvider.GameObject.GetComponentInParent<Hands>();
                var item = hands.SelectedHand.ItemInHand;

                hands.SelectedHand.Container.Dump();
                _attachedContainer.AddItem(item);
            }

            return false;
        }
    }
}