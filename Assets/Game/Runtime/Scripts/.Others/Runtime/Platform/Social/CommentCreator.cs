using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kumu.Kulitan.Social
{
    public class CommentCreator : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputFieldComment;
        [SerializeField] private Button btnPost;

        [SerializeField] private CommentController commentController;

        private void SubmitComment()
        {
            string inputComment = inputFieldComment.text;
            // TODO: CC add more info on UserData
            CommentData newCommentData = new CommentData(new UserData("username"), inputComment);
            // TODO: CC add comment only after a success response from the server
            commentController.AddComment(newCommentData);
            inputFieldComment.text = "";
        }

        private void OnEnable()
        {
            btnPost.onClick.AddListener(SubmitComment);
        }

        private void OnDisable()
        {
            btnPost.onClick.RemoveListener(SubmitComment);
        }
    }
}
