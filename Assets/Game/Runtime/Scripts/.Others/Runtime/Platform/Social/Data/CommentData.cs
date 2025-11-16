using System;
using UnityEngine;

namespace Kumu.Kulitan.Social
{
    [Serializable]
    public class CommentData
    {
        public CommentData(UserData _userData, string _caption)
        {
            userData = _userData;
            comment = _caption;
            likes = new UserData[0];
        }

        [SerializeField] private UserData userData;
        [SerializeField] private string comment;
        [SerializeField] private UserData[] likes;

        public UserData User => userData;
        public string Comment => comment;
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
    }
}
