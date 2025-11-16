using UnityEngine;
using TMPro;

namespace Kumu.Kulitan.Social
{
    public class CommentView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtUsername;
        [SerializeField] private TextMeshProUGUI txtComment;
        [SerializeField] private TextMeshProUGUI txtLikesCount;

        public void InitializeComment(string username, string comment, int likesCount)
        {
            txtUsername.text = username;
            txtComment.text = comment;
            txtLikesCount.text = likesCount.ToString();
        }
    }
}
