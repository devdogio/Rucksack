using System.Text;

namespace Devdog.Rucksack
{
    public static class StringArray
    {

        public static string ToSimpleString(this int[] arr)
        {
            if (arr == null)
            {
                return "null";
            }
            
            var builder = new StringBuilder(arr.Length * 3 + 2);
            builder.Append("[");
            for (var i = 0; i < arr.Length; i++)
            {
                builder.Append(arr[i]);
                if (i < arr.Length - 1)
                {
                    builder.Append(",");
                }
            }
            builder.Append("]");
            return builder.ToString();
        }
        
    }
}