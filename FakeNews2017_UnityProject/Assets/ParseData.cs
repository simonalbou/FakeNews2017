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

			if (card [0].Length == 0 || card[1].Length == 0)
				continue;
			
			Card cardObject = ScriptableObject.CreateInstance<Card> ();
			cardObject.reference = card [0];
			cardObject.family = (CardFamily) System.Enum.Parse (typeof(CardFamily), card[1]); 
			cardObject.author = card [2];
			cardObject.title = card [3];
			cardObject.fake = !(card [4] == "TRUE");

			AssetDatabase.CreateAsset (cardObject, "Assets/Cards/Card_" + cardObject.reference + ".asset");
			AssetDatabase.SaveAssets ();
		}
	}
}
