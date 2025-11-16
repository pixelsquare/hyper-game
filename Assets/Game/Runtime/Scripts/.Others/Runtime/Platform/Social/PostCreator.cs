using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kumu.Kulitan.Social
{
    public class PostCreator : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputFieldCaption;
        [SerializeField] private Button btnPost;
        [SerializeField] private Button btnHashtag;

        [SerializeField] private PostController postController;

        public void SubmitPost()
        {
            string inputCaption = inputFieldCaption.text;
            // TODO: add more info on UserData 
            PostData newPostData = new PostData(new UserData("username"), inputCaption); // TODO: change UserData info as necessary
            postController.AddPost(newPostData);
        }

        public void AddHashtag()
        {
            inputFieldCaption.text += "#";
        }

        private void OnEnable()
        {
            btnPost.onClick.AddListener(SubmitPost);
            btnHashtag.onClick.AddListener(AddHashtag);
        }

        private void OnDisable()
        {
            btnPost.onClick.RemoveListener(SubmitPost);
            btnHashtag.onClick.RemoveListener(AddHashtag);
        }
    }
}
