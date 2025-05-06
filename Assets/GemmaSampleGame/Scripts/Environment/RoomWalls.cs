using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GoogleDeepMind.GemmaSampleGame
{
    public class RoomWalls : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] walls;

        private IRoomWallsRegistry registry;

        [Inject]
        public void Construct(IRoomWallsRegistry registry)
        {
            this.registry = registry;
            registry.RegisterWalls(this);
        }

        private void OnDestroy()
        {
            if (registry != null)
            {
                registry.UnregisterWalls(this);
            }
        }

        public void SetActive(bool active)
        {
            foreach (var wall in walls)
            {
                wall.SetActive(active);
            }
        }
    }
}
