// Copyright 2025 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
