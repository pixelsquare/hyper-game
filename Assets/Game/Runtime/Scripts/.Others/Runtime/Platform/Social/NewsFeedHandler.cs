using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Kumu.Kulitan.Common;
using System.Collections;

namespace Kumu.Kulitan.Social
{
    public class NewsFeedHandler : SingletonMonoBehaviour<NewsFeedHandler>
    {
        [SerializeField] private bool isPerformanceTestingOn = false;   // HACK: remove when actual data is used. If true, hundreds of content will be generated to test performance.

        [SerializeField] private PostController postsController;
        private List<PostData> postsData = new List<PostData>();

        [SerializeField] private UnityEvent onDataRefresh;

        public bool IsPerformanceTestingOn => isPerformanceTestingOn;

        // HACK: temp method for changing data
        public void SaveTempData(PostData[] newPostData)
        {
            string dataToSave = JsonHelper.ToJson(newPostData, true);
            File.WriteAllText(Application.dataPath + "/StaticAssets/Test/generatedFeed.json", dataToSave);
        }

        public void AddNewPost(PostData newData)
        {
            postsData.Add(newData);
            SaveTempData(postsData.ToArray());
        }

        public void AddCommentToPost(string postId, CommentData comment)
        {
            PostData postData = postsData.Find(x => x.PostId == postId);
            List<CommentData> comments = new List<CommentData>();
            if (postData.Comments.Length > 0)
            {
                comments.AddRange(postData.Comments);
            }
            comments.Add(comment);
            postData.Comments = comments.ToArray();
            SaveTempData(postsData.ToArray());
            // refresh posts
            InitializePosts();
        }

        public void RefreshData()
        {
            StartCoroutine(TestDataRefresh());
        }

        // HACK: simulates data loading time
        private IEnumerator TestDataRefresh()
        {
            yield return new WaitForSeconds(2f);
            onDataRefresh?.Invoke();
        }

        /// <summary>
        /// Used to generate a json file with default data.
        /// </summary>
        private void GenerateTempData()
        {
            PostData[] postDatas = new PostData[1];

            postDatas[0] = new PostData(new UserData("user 2"), "temp caption");
            postDatas[0].Comments = new CommentData[3];
            postDatas[0].Likes = new UserData[3];

            string generatedJson = JsonHelper.ToJson(postDatas);

            File.WriteAllText(Application.dataPath + "/StaticAssets/Test/generatedFeed.json", generatedJson);
        }

        // HACK: temp method for loading data
        /// <summary>
        /// Loads a dummy json file from the game files.
        /// </summary>
        private List<PostData> LoadTempData()
        {
            string generatedJsonParse = File.ReadAllText(Application.dataPath + "/StaticAssets/Test/generatedFeed.json");
            PostData[] loadedData = JsonHelper.FromJson<PostData>(generatedJsonParse);
            Array.Reverse(loadedData);
            postsData.Clear();
            postsData.AddRange(loadedData);
            return postsData;
        }

        /// <summary>
        /// Initializes the data to display through the feed.
        /// </summary>
        private void InitializePosts()
        {
            postsController.InitializePosts(postsData);
        }

        private void Start()
        {
            LoadTempData();
            InitializePosts();
        }
    }
}
