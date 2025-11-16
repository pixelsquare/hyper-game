using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public struct UserName
    {
        public string name;

        public string discriminator;

        public override string ToString()
        {
            return $"{name}#{discriminator}";
        }

        public static UserName FromString(string fullUserNameAsString)
        {
            var words = fullUserNameAsString.Split('#');
            
            if (words.Length != 2)
            {
                throw new FormatException("Format is incorrect.");
            }

            return new UserName
            {
                name = words[0],
                discriminator = words[1],
            };
        }

        public static implicit operator string(UserName username)
        {
            return username.ToString();
        }

        public static implicit operator UserName(string username)
        {
            return FromString(username);
        }
    }
}
