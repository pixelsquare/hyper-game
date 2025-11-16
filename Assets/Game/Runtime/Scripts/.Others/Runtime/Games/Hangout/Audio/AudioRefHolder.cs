using System.Collections.Generic;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Hangout
{
    public class AudioRefHolder : SingletonMonoBehaviour<AudioRefHolder>
    {
        public SerializableDictionary<string, AudioClip> clipDictionary;
        public SerializableDictionary<string, int> usageDictionary;

        public async void AddAudioReference(AvatarAudioHandler requester, AudioClipConfig clipConfig)
        {
            if (clipDictionary.ContainsKey(clipConfig.audioReference.AssetGUID))
            {
                requester.OnClipReady(clipDictionary[clipConfig.audioReference.AssetGUID], clipConfig);
                usageDictionary[clipConfig.audioReference.AssetGUID] = usageDictionary[clipConfig.audioReference.AssetGUID] + 1;
                return;
            }

            var audioClip = await AvatarAddressablesUtility.LoadAddressable<AudioClip>(clipConfig.audioReference);

            if (!clipDictionary.ContainsKey(clipConfig.audioReference.AssetGUID))
            {
                clipDictionary.Add(clipConfig.audioReference.AssetGUID, audioClip);
            }

            if (!usageDictionary.ContainsKey(clipConfig.audioReference.AssetGUID))
            {
                usageDictionary.Add(clipConfig.audioReference.AssetGUID, 1);
            }

            requester.OnClipReady(audioClip, clipConfig);
        }

        public async void RemoveAudioReference(AssetReference clipRef)
        {
            if (usageDictionary != null && usageDictionary.ContainsKey(clipRef.AssetGUID))
            {
                usageDictionary[clipRef.AssetGUID] = usageDictionary[clipRef.AssetGUID] - 1;

                if (usageDictionary[clipRef.AssetGUID] <= 0)
                {
                    await AvatarAddressablesUtility.Release<AudioClip>(clipRef);

                    clipDictionary.Remove(clipRef.AssetGUID);
                    usageDictionary.Remove(clipRef.AssetGUID);
                }
            }
        }

        public void RemoveAudioReferenceList(HashSet<AssetReference> refList)
        {
            foreach (var aRef in refList)
            {
                RemoveAudioReference(aRef);
            }
        }

        public AudioClip GetAudioClip(AssetReference clipRef)
        {
            if (clipDictionary.ContainsKey(clipRef.AssetGUID))
            {
                return clipDictionary[clipRef.AssetGUID];
            }

            return null;
        }
    }
}
