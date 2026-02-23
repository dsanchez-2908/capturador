using System;
using System.IO;
using PdfSharp.Fonts;

namespace Capturador._01_Pantallas._06_Lotes;

public class CustomFontResolver : IFontResolver
{
	private static readonly byte[] arialData = File.ReadAllBytes("arial.ttf");

	public string DefaultFontName => "Arial#";

	public byte[] GetFont(string faceName)
	{
		return arialData;
	}

	public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
	{
		if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
		{
			return new FontResolverInfo("Arial#");
		}
		return PlatformFontResolver.ResolveTypeface(familyName, isBold, isItalic);
	}
}
