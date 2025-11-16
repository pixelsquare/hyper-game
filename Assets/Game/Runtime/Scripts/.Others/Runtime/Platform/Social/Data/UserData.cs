using System;
using UnityEngine;

namespace Kumu.Kulitan.Social
{
    [Serializable]
    public class UserData
    {
        public UserData(string userName)
        {
            this.userName = userName;
        }

        [SerializeField] private string userName = "name";

        public string UserName => userName;
    }
}
