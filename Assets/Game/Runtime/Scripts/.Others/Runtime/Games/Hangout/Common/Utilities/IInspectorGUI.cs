namespace Kumu.Kulitan.Hangout
{
    public interface IInspectorGUI
    {
#if UNITY_EDITOR
        void OnInspectorInit();

        void OnInspectorDraw();
#endif
    }
}
