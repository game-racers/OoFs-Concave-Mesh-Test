using UnityEngine;

namespace gameracers.Inventory
{
    public class Equipment : MonoBehaviour
    {
        [SerializeField] bool freezeX = false;

        public void AimFace(Vector3 facing)
        {
            if (facing == Vector3.zero)
                return;

            if (freezeX)
            {
                transform.LookAt(transform.position + new Vector3(facing.x, 0, facing.z));
            }
            else
            {
                transform.LookAt(transform.position + facing);
            }
        }
    }
}