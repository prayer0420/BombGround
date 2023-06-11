using Inventory.Scripts.Inventory.Items.Enums;

namespace Inventory.Scripts.Inventory.Items.Utils
{
    public static class RotationUtils
    {
        public static float GetRotationByType(Rotation rotation)
        {
            if (rotation == Rotation.MinusNinety)
            {
                return -90f;
            }

            return 90f;
        }
    }
}
