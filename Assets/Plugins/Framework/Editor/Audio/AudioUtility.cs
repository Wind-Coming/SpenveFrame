using System;
using System.Reflection;
using UnityEngine;

namespace UnityEditor
{
	public static class AudioUtility
	{
		public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
		{
			typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil").GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public, null, new Type[3]
			{
				typeof(AudioClip),
				typeof(int),
				typeof(bool)
			}, null).Invoke(null, new object[3]
			{
				clip,
				startSample,
				loop
			});
		}

		public static void StopClip(AudioClip clip)
		{
			typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil").GetMethod("StopClip", BindingFlags.Static | BindingFlags.Public, null, new Type[1]
			{
				typeof(AudioClip)
			}, null).Invoke(null, new object[1]
			{
				clip
			});
		}

		public static void PauseClip(AudioClip clip)
		{
			typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil").GetMethod("PauseClip", BindingFlags.Static | BindingFlags.Public, null, new Type[1]
			{
				typeof(AudioClip)
			}, null).Invoke(null, new object[1]
			{
				clip
			});
		}

		public static void ResumeClip(AudioClip clip)
		{
			typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil").GetMethod("ResumeClip", BindingFlags.Static | BindingFlags.Public, null, new Type[1]
			{
				typeof(AudioClip)
			}, null).Invoke(null, new object[1]
			{
				clip
			});
		}

		public static bool IsClipPlaying(AudioClip clip)
		{
			return (bool)typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil").GetMethod("IsClipPlaying", BindingFlags.Static | BindingFlags.Public, null, new Type[1]
			{
				typeof(AudioClip)
			}, null).Invoke(null, new object[1]
			{
				clip
			});
		}
	}
}
