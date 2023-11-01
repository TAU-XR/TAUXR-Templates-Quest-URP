using System;
using System.Collections;
using System.Collections.Generic;

public static class ListExtensions
{
	//Shuffles using the Fisher-Yates algorithm
	public static void Shuffle<T>(this IList<T> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			T temp = list[i];
			int randomIndex = UnityEngine.Random.Range(i, list.Count);
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}
	}
}
