using NUnit.Framework;
using UnityEngine;

namespace NullCore.Tests.Editor
{
	[TestFixture]
	public class StaticStringsTests
	{
		[Test]
		public void Builder_IsNotNull()
		{
			Assert.IsNotNull(StaticStrings.Builder);
		}

		[Test]
		public void PrefixScripts_IsCorrect()
		{
			Assert.AreEqual("NullCore/", StaticStrings.PrefixScripts);
		}

		[Test]
		public void PrefixTools_IsCorrect()
		{
			Assert.AreEqual("Tools/NullCore/", StaticStrings.PrefixTools);
		}

		[Test]
		public void DontDestroyOnLoad_IsCorrect()
		{
			Assert.AreEqual("DontDestroyOnLoad", StaticStrings.DontDestroyOnLoad);
		}

		[Test]
		public void ResourcesPath_IsCorrect()
		{
			Assert.AreEqual("Assets/Resources/", StaticStrings.ResourcesPath);
		}
	}
}
