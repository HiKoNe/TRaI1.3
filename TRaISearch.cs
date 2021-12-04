using System.Collections.Generic;

namespace TRaI
{
    public static class TRaISearch
    {
        public static List<int> Search(string text)
        {
            var list = new List<int>();
            for (int i = 0; i < TRaI.ItemsCount; i++)
            {
                if (TRaI.AllToolTips[TRaI.AllItems[i].type].Contains(text.ToLower()))
                    list.Add(i);
            }
            return list;
        }
    }
}
