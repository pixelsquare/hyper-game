using System.Collections.Generic;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public abstract class ListView<T1, T2> : MonoBehaviour
        where T1 : ListViewElement<T2>
        where T2 : struct
    {
        [SerializeField] protected T1 prefab;
        [SerializeField] protected Transform container;

        protected List<T1> uiElements = new();

        public bool HasElements => uiElements.Count > 0;

        public virtual void Display(IEnumerable<T2> data)
        {
            foreach (var datum in data)
            {
                var element = Instantiate(prefab, container);
                OnCreate(element, datum);
                uiElements.Add(element);
            }
        }

        public void Clear()
        {
            foreach (var element in uiElements)
            {
                Destroy(element.gameObject);
            }
            
            uiElements.Clear();
        }

        protected virtual void OnCreate(T1 element, T2 datum)
        {
            element.Refresh(datum); 
            element.gameObject.SetActive(true);
        }
    }

    public abstract class ListViewElement<T> : MonoBehaviour
        where T : struct
    {
        public abstract void Refresh(T data);
    }
}
