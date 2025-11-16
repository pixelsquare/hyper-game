using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Kumu.Kulitan.UI.OptimizedScrollView;

namespace Kumu.Kulitan.Social
{
    public class CommentController : MonoBehaviour
    {
        [SerializeField] private CommentsScrollView commentsScrollView;

        [SerializeField] private CommentView commentPrefab;
        [SerializeField] private TextMeshProUGUI txtCommentCount;
        [SerializeField] private RectTransform container;

        private List<CommentData> comments = new List<CommentData>();

        private string curPostId = "";

        public void Initialize(CommentData[] commentData, string postId)
        {
            comments.Clear();
            comments.AddRange(commentData);
            // HACK: remove tester
            if (NewsFeedHandler.Instance.IsPerformanceTestingOn)
            {
                for (var i = 0; i < 500; i++)
                {
                    var tempComment = new CommentData(new UserData("temp user"), "comment no. " + i);
                    comments.Add(tempComment);
                }
            }
            commentsScrollView.InitializeData(comments);
            UpdateCommentCount();
            curPostId = postId;
        }

        public void ShowCommentsPanel()
        {
            gameObject.SetActive(true);
        }

        public void HideCommentsPanel()
        {
            gameObject.SetActive(false);
        }

        public void AddComment(CommentData newComment)
        {
            comments.Add(newComment);
            AddCommentToDisplay(newComment);
            NewsFeedHandler.Instance.AddCommentToPost(curPostId, newComment);
        }

        private void AddCommentToDisplay(CommentData commentData)
        {
            commentsScrollView.AddNewItem(commentData);
            UpdateCommentCount();
        }

        private void UpdateCommentCount()
        {
            txtCommentCount.text = comments.Count.ToString() + " comments";
        }
    }
}
