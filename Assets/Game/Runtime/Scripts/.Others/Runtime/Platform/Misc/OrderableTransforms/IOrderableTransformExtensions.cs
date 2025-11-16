using System;
using Kumu.Kulitan.UI;

namespace Kumu.Extensions
{
    public static class IOrderableTransformExtensions
    {
        public static void OrderTransform(this IOrderableTransform orderable)
        {
            var parent = orderable.GetTransformParent();

            if (!parent)
            {
                throw new Exception("No parent!");
            }

            var orderables = parent.GetComponentsInChildren<IOrderableTransform>(true);
            var count = orderables.Length;
            if (count < 2)
            {
                return;
            }
            
            OrderTransforms(orderables);
        }

        /// <summary>
        /// Arranges sibling indexes of <see cref="IOrderableTransform"/> by their <see cref="IOrderableTransform.Order()"/> property. Non-IOrderableTransforms sibling indexes will not be affected. 
        /// </summary>
        /// <param name="orderables">Array to sort.</param>
        private static void OrderTransforms(IOrderableTransform[] orderables)
        {
            var count = orderables.Length;
            for (var i = 0; i < count; i++)
            {
                for (var j = 0; j + 1 < count; j++)
                {
                    if (orderables[j].Order <= orderables[j + 1].Order)
                    {
                        continue;
                    }

                    // swap sibling index
                    var tempSiblingIndex = orderables[j].GetTransform().GetSiblingIndex();
                    orderables[j].GetTransform().SetSiblingIndex(orderables[j + 1].GetTransform().GetSiblingIndex());
                    orderables[j + 1].GetTransform().SetSiblingIndex(tempSiblingIndex);

                    // swap array index
                    (orderables[j], orderables[j + 1]) = (orderables[j + 1], orderables[j]);
                }
            }
        }
    }
}
