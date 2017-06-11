using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParseData 
{
	[MenuItem("Damien/Create Cards")]
	public static void Parse()
	{
		string raw = AssetDatabase.LoadAssetAtPath<TextAsset> ("Assets/data.txt").text;

		string[] lines = raw.Split ('\n');

		for (int i = 1; i < lines.Length; i++)
		{
			string[] card = lines [i].Split ('\t');

			if (card [0].Length == 0)
				continue;
			
			Card cardObject = ScriptableObject.CreateInstance<Card> ();
			cardObject.family = (CardFamily) System.Enum.Parse (typeof(CardFamily), card[0]); 
			cardObject.author = card [1];
			cardObject.title = card [2];

			AssetDatabase.CreateAsset (cardObject, "Assets/Cards/Card_" + i + ".asset");
			AssetDatabase.SaveAssets ();
		}
	}
}
