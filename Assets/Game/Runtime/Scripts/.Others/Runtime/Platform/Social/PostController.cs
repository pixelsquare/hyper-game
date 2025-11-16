using System.Collections.Generic;
using UnityEngine;
using Kumu.Kulitan.UI.OptimizedScrollView;

namespace Kumu.Kulitan.Social
{
    public class PostController : MonoBehaviour
    {
        [SerializeField] private PostsScrollView postsScrollView;

        [SerializeField] private RectTransform postContainer;
        [SerializeField] private CommentController commentsController;

        private List<PostData> posts = new List<PostData>();

        public void InitializePosts(List<PostData> posts)
        {
            this.posts.Clear();
            this.posts.AddRange(posts);
            // HACK: remove tester
            if (NewsFeedHandler.Instance.IsPerformanceTestingOn)
            {
                for (var i = 0; i < 500; i++)
                {
                    var tempPost = new PostData(new UserData("temp user"), "caption no. " + i);
                    this.posts.Add(tempPost);
                }
            }
            postsScrollView.InitializeData(this.posts);
        }

        /// <summary>
        /// Adds a new post to the display.
        /// Updates the existing JSON file.
        /// </summary>
        /// <param name="newData">the new post data</param>
        public void AddPost(PostData newData)
        {
            posts.Insert(0, newData);
            AddPostDisplay(newData);
            NewsFeedHandler.Instance.AddNewPost(newData);
        }
              
        private void AddPostDisplay(PostData postData)
        {
            postsScrollView.AddNewItem(postData);
        }
    }
}
