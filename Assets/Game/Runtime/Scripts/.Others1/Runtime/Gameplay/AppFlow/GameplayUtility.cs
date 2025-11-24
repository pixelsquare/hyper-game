using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public static class GameplayUtility
    {
        public static async UniTask InitializeGameplaySystemsAsync(GameObject root)
        {
            var taskList = new List<UniTask>();
            
            var gameplaySystems = from system in root.GetComponentsInChildren<IGameplaySystem>()
                                  orderby system.GameplaySystemInitPriority
                                  select system;
            
            foreach (var gameplaySystem in gameplaySystems)
            {
                var uniTask = gameplaySystem.GameplaySystemInitAsync();
                taskList.Add(uniTask);
            }

            await UniTask.WhenAll(taskList);
        }
    }
}
