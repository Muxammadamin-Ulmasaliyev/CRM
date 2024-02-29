


namespace InventoryManagementSystem.Services
{
    public static class StringHelper
    {
        public static string TrimAllWhiteSpaces(string str)
        {
            return new string(str.Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        public static string RemoveSumSignFromPrice(string text)
        {
            var array = text.Split(' ');
            return new string(array[0]);
        }

        public static string RemoveDollarSignFromPrice(string text)
        {
            return text.TrimStart('$');
        }

        public static string FormatAsCurrency(double amount)
        {
            string result = string.Empty;
            var chars = amount.ToString().ToList();
            int counter = 0;
            for (var i = chars.Count - 1; i >= 0; i--)
            {
                //var c = chars[i];
                counter++;

                if(counter == 2)
                {
                    chars.Insert(i, ' ');
                    counter = 0;
                }
            }

            return new string(chars.ToArray());
        }
    }
}

// 55 252
// 012345
// 012345