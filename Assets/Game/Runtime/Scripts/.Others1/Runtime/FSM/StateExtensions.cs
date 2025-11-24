using System;

namespace Santelmo.Rinsurv
{
    public static class StateExtensions
    {
        public static T OfType<T>(this IState state)
        {
            if (state is not T returnState)
            {
                throw new InvalidCastException($"Unable to cast state to {nameof(T)}");
            }

            return returnState;
        }
    }
}
