using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[Serializable]
public class Grammars
{
    public string[] nouns;
    public string[] adjectives;
    public string[] verbs;
    public string[] adverbs;
    public string[] fortunes;
}

public class TextGenerator : MonoBehaviour {

    [SerializeField]
    Text text;

    Grammars grammars;
    string[] tags;
    private string path = "grammars.json";

	// Use this for initialization
	void Start () {
        grammars = JsonUtility.FromJson<Grammars>(loadJSON());
        tags = new string[]{ "nouns", "adjectives", "verbs", "adverbs", "number" };
        ChangeText();
        InvokeRepeating("ChangeText", 1.0f, 2.0f);
    }

    void ChangeText()
    {
        text.text = GenerateRandom();
    }

    public string GenerateRandom()
    {
        //Select a random fortune
        string fortune = grammars.fortunes[(int)Math.Floor(UnityEngine.Random.value * grammars.fortunes.Length)];
        string[] sentence = fortune.Split(new char[]{ '#' });
        //Parse out tags
        int idx = 0;
        while (idx != sentence.Length)
        {
            if (Array.IndexOf(tags, sentence[idx]) != -1)
            {
                string[] list = { };
                switch (sentence[idx])
                {
                    case "nouns":
                        list = grammars.nouns;
                        break;
                    case "adjectives":
                        list = grammars.adjectives;
                        break;
                    case "verbs":
                        list = grammars.verbs;
                        break;
                    case "adverbs":
                        list = grammars.adverbs;
                        break;
                    case "number":
                        sentence[idx] = (Math.Floor((double)UnityEngine.Random.Range(0, 22))).ToString();
                        continue;
                }
                sentence[idx] = list[(int) Math.Floor(UnityEngine.Random.value * list.Length)];
                if ((idx + 1) < sentence.Length && sentence[idx + 1][0].Equals('.'))
                {
                    if (sentence[idx + 1].IndexOf("ing") != -1 && sentence[idx][sentence[idx].Length - 1].Equals('e'))
                    {
                        
                        sentence[idx] = sentence[idx].Substring(0, sentence[idx].Length - 1);
                    }
                    sentence[idx] = sentence[idx] + sentence[idx + 1].Substring(1);
                    sentence[idx + 1] = "";
                }
            }
            idx++;
        }
        return String.Join("", sentence);
    }

    private string loadJSON ()
    {
        string filePath = path.Replace(".json", "");

        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        return targetFile.text;
    }
}
