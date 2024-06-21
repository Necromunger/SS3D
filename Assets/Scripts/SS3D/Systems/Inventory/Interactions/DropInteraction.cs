using System;
using SS3D.Data.Generated;
using SS3D.Interactions;
using SS3D.Interactions.Extensions;
using SS3D.Systems.Inventory.Containers;
using UnityEngine;

namespace SS3D.Systems.Inventory.Interactions
{
    // a drop interaction is when we remove an item from the hand
    [Serializable]
    public class DropInteraction : Interaction
    {
        /// <summary>
        /// The maximum angle of surface the item will allow being dropped on
        /// </summary>
        private float _maxSurfaceAngle = 10;

        public override string GetName(InteractionEvent interactionEvent)
        {
            return "Drop";
        }

        public override Sprite GetIcon(InteractionEvent interactionEvent)
        {
            return Icon != null ? Icon : InteractionIcons.Discard;
        }

        public override bool CanInteract(InteractionEvent interactionEvent)
        {
            // if the interaction source's parent is not a hand we return false
            if (interactionEvent.Source.GetRootSource() is not Hand)
            {
                return false;
            }

            // and we do a range check just in case
            return InteractionExtensions.RangeCheck(interactionEvent);
        }

        public override bool Start(InteractionEvent interactionEvent, InteractionReference reference)
        {
            // we consider if the surface we are placing the item on is flat
            float angle = Vector3.Angle(interactionEvent.Normal, Vector3.up);  
            if (angle > _maxSurfaceAngle)
            {
                return false;
            }

            // we check if the source of the interaction is a hand
            if (interactionEvent.Source.GetRootSource() is Hand hand)
            {
                // we rotate the item based on the facing direction of the hand
                Quaternion rotation = Quaternion.Euler(0, hand.ItemInHand.transform.eulerAngles.y, 0);

                // we place the item in the hand in the point we clicked
                hand?.PlaceHeldItemOutOfHand(interactionEvent.Point, rotation);
            }

            return false;
        }
    }
}