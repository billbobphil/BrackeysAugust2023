using UnityEngine;

namespace Hook
{
    public interface IHookable
    {
        void OnHooked(GameObject hook);
    }
}