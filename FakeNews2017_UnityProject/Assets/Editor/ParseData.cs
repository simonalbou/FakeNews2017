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
			cardObject.isFakeNews = !(card [4] == "TRUE");

			string[] firstlastday = card [6].Split (':');
			if (firstlastday.Length == 2)
			{
				cardObject.firstDay = int.Parse( firstlastday [0]) - 1;
				cardObject.lastDay = int.Parse (firstlastday [1]) - 1;
			}

			AssetDatabase.CreateAsset (cardObject, "Assets/Cards/Card_" + cardObject.reference + ".asset");
			AssetDatabase.SaveAssets ();
		}
	}

	[MenuItem("Damien/Create No Random")]
	public static void ParseNoRandom ()
	{
		string raw = AssetDatabase.LoadAssetAtPath<TextAsset> ("Assets/norandomdata.txt").text;

		string[] lines = raw.Split ('\n');

		for (int i = 3; i < lines.Length; i++)
		{
			string[] card = lines [i].Split ('\t');

			// CARD ASM
			if (card [4].Length > 0 && card [5].Length == 0 && card [6].Length == 0)
			{
				CreateNonRandomCard (i - 3, card [4], "ASN");
			}
			else if (card [4].Length > 0)
			{
				CreateNonRandomCard (i - 3, card [4], "ASN_FAKE_0");
			}

			if (card [5].Length > 0)
			{
				CreateNonRandomCard (i - 3, card [5], "ASN_FAKE_1");
			}

			if (card [6].Length > 0)
			{
				CreateNonRandomCard (i - 3, card [6], "ASN_FAKE_2");
			}

			// CARD Z
			Read (card, 7, 8, 9, 10, "Z", i-3);
			// CARD LONTARIN
			Read (card, 11, 12, 13, 14, "LONTARIN", i-3);
			// CARD BEAUREGARD
			Read( card, 15, 16, 17, 18, "BEAUREGARD", i-3);
			// CARD RUFALLOW
			Read( card, 19, 20, 21, 22, "RUFALLOW", i-3);
		}

		AssetDatabase.SaveAssets ();
	}

	public static void Read(string[] card, int col1, int col2, int col3, int col4, string baseRef, int day)
	{
		if (card [col1].Length > 0 && card[col2].Length == 0 && card[9].Length == 0 && card[col4].Length == 0)
		{
			CreateNonRandomCard (day, card [col1], baseRef + "_ALL");
		}
		else if (card [col1].Length > 0 && card [col2].Length == 0)
		{
			CreateNonRandomCard (day, card [col1], baseRef + "_SI -");
		}
		else if (card [col1].Length > 0 && card [col2].Length > 0)
		{
			CreateNonRandomCard (day, card [col1], baseRef + "_SI - --");
			CreateNonRandomCard (day, card [col2], baseRef + "_SI - -+");
		}

		if (card [col3].Length > 0 && card [col4].Length == 0)
		{
			CreateNonRandomCard (day, card [col3], baseRef + "_SI +");
		}
		else if (card[col3].Length > 0 && card[col4].Length > 0)
		{
			CreateNonRandomCard (day, card [col3], baseRef + "_SI + --");
			CreateNonRandomCard (day, card [col4], baseRef + "_SI + -+");
		}		
	}

	public static void CreateNonRandomCard (int day, string text, string reference)
	{
		Card card = ScriptableObject.CreateInstance<Card> ();
		card.title = text;
		card.firstDay = day;
		card.lastDay = day;
		card.family = CardFamily.META;
		card.author = "ASN";

		AssetDatabase.CreateAsset (card, "Assets/NonRandom/Card_" + reference + "_" + day + ".asset");
	}
}
