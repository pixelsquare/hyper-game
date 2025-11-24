using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

namespace Santelmo.Rinsurv.Backend
{
    /// <summary>
    /// Firestore specific implementation of the ICloudSyncService
    /// </summary>
    public class FirestoreCloudSyncService : ICloudSyncService
    {
        private FirebaseFirestore db;
        private readonly IAuthService authService;

        public static ICloudSyncService Create(IAuthService authService)
        {
            return new FirestoreCloudSyncService(authService);
        }

        private FirestoreCloudSyncService(IAuthService authService)
        {
            this.authService = authService;
            Initialize();
        }

        private void Initialize()
        {
#if UNITY_ANDROID
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    db = FirebaseFirestore.DefaultInstance;
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
#endif
        }

        public async UniTask<bool> WriteAsync(string key, object value, CancellationToken cancellationToken = default)
        {
            if (authService.UserId is not { } userId)
            {
                return false;
            }

            await UniTask.WaitUntil(() => db is not null, cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            var docRef = db.Collection(userId).Document(key);
            await docRef.SetAsync(value, SetOptions.Overwrite);
            return true;
        }

        public async UniTask<bool> WriteAsync(Dictionary<string, object> data, CancellationToken cancellationToken = default)
        {
            if (authService.UserId is not { } userId)
            {
                return false;
            }

            await UniTask.WaitUntil(() => db is not null, cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            var tasks = new List<UniTask>();
            foreach (var dataPair in data)
            {
                var docRef = db.Collection(userId).Document(dataPair.Key);
                tasks.Add(docRef.SetAsync(dataPair.Value, SetOptions.Overwrite).ContinueWithOnMainThread(_ =>
                {
                    Debug.Log($"Added data to the {dataPair.Key} document in the {userId} collection.");
                }).AsUniTask());
            }

            await UniTask.WhenAll(tasks);
            return false;
        }

        public async UniTask<T> ReadAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            if (authService.UserId is not { } userId)
            {
                return default;
            }

            await UniTask.WaitUntil(() => db is not null, cancellationToken: cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            var usersRef = db.Collection(userId).Document(key);
            try
            {
                var snap = await usersRef.GetSnapshotAsync();
                return snap.Exists ? snap.ConvertTo<T>() : default;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
        }
    }
}
