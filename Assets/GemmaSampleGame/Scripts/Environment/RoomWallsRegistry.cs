using UnityEngine;
using System.Collections.Generic;

namespace GoogleDeepMind.GemmaSampleGame
{
    public interface IRoomWallsRegistry
    {
        void RegisterWalls(RoomWalls walls);
        void UnregisterWalls(RoomWalls walls);
        IReadOnlyCollection<RoomWalls> GetAllWalls();
    }

    public class RoomWallsRegistry : IRoomWallsRegistry
    {
        private HashSet<RoomWalls> registeredWalls = new HashSet<RoomWalls>();

        public void RegisterWalls(RoomWalls walls)
        {
            registeredWalls.Add(walls);
            Debug.Log($"Registered RoomWalls: {walls.gameObject.name}");
        }

        public void UnregisterWalls(RoomWalls walls)
        {
            registeredWalls.Remove(walls);
            Debug.Log($"Unregistered RoomWalls: {walls.gameObject.name}");
        }

        public IReadOnlyCollection<RoomWalls> GetAllWalls()
        {
            return registeredWalls;
        }
    }
}