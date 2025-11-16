using System;
using UnityEngine;

namespace Kumu.Kulitan.Social
{
    [Serializable]
    public class PostData
    {
        public PostData(UserData _userData, string _caption)
        {
            postId = Guid.NewGuid().ToString(); // just a temp random id generator
            userData = _userData;
            caption = _caption;
            likes = new UserData[0];
            comments = new CommentData[0];
        }

        [SerializeField] private string postId = "0";
        [SerializeField] private UserData userData;
        [SerializeField] private string caption = "this is a sample caption...";
        [SerializeField] private UserData[] likes;
        [SerializeField] private CommentData[] comments;

        public string PostId => postId;
        public UserData User => userData;
        public string Caption => caption;
        public UserData[] Likes
        {
            get
            {
                return likes;
            }
            set
            {
                likes = value;
            }
        }
        public CommentData[] Comments
        {
            get
            {
                return comments;
            }
            set
            {
                comments = value;
            }
        }
    }
}
