using UnityEngine;
using System.Collections;

public static class ExtensionMethods {

    public static string CapitalizeFirstLetter(this string s)
    {
        if (s.Length > 0)
        {
            string firstLetter = s.Substring(0, 1);
            firstLetter = firstLetter.ToUpper();

            s = firstLetter + s.Substring(1);
        }
        return s;
    }

}

