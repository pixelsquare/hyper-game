using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class TutorialButtonInvoker : MonoBehaviour
    {
        public void Show(SlidePagesCollection tutorialSlidePagesCollection)
        {
            var evt = new Tutorial.ShowTutorialEvent(tutorialSlidePagesCollection.Id);
            GlobalNotifier.Instance.Trigger(evt);
        }
    }
}
