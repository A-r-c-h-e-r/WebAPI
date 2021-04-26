using System.ComponentModel.DataAnnotations;

namespace WebAPI.Database.Validation
{
    public static class ValidationData
    {
        public static bool ValidationName(string name)
        {
            if (name != null && name.Length > 2)
            {
                char[] KeyCode = name.ToCharArray();
                for (int i = 1; i < KeyCode.Length; i++)
                    if ((KeyCode[i] >= 97 && KeyCode[i] <= 122) == false)
                        return false;
                if ((KeyCode[0] >= 97 && KeyCode[0] <= 122) || ((KeyCode[0] >= 65 && KeyCode[0] <= 90)))
                    return true;
            }
            return false;
        }

        public static bool ValidationEmail(string email)
        {
            return (email != null && email.Length > 10) ? (new EmailAddressAttribute().IsValid(email) ? true : false) : false;
        }

        public static bool ValidationPassword(string password)
        {
            if (password != null && password.Length > 5)
            {
                char[] KeyCode = password.ToCharArray();
                bool[] Check = new bool[2] { false, false };
                foreach (char c in KeyCode)
                    if (((c >= 48 && c <= 57) || (c >= 65 && c <= 90) || (c >= 97 && c <= 122)) == false)
                        return false;

                foreach (char c in KeyCode) if (c >= 48 && c <= 57) { Check[0] = true; break; }
                foreach (char c in KeyCode) if ((c >= 65 && c <= 90) || (c >= 97 && c <= 122)) { Check[1] = true; break; }

                if (Check[0] == true && Check[1] == true) return true;
            }
            return false;
        }

        public static bool ValidationLog(string log)
        {
            return (log != null && log.Length > 0); // При необходимости тут нужно добавить валидацию 
        }

        public static bool Valid(string data)
        {
            return (data != null && data.Length > 0); // При необходимости тут нужно добавить валидацию 
        }
    }
}