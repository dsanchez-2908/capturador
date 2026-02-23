using System;
using System.Drawing;

namespace Capturador._02_Negocio;

public static class ImageExtensions
{
	public static Image Crop(this Image image, Rectangle selection)
	{
		if (selection.Width <= 0 || selection.Height <= 0)
		{
			return null;
		}
		if (!(image is Bitmap bmp))
		{
			throw new ArgumentException("La imagen no es un Bitmap.");
		}
		Bitmap cropBmp = new Bitmap(selection.Width, selection.Height);
		using (Graphics g = Graphics.FromImage(cropBmp))
		{
			g.DrawImage(bmp, new Rectangle(0, 0, cropBmp.Width, cropBmp.Height), selection, GraphicsUnit.Pixel);
		}
		return cropBmp;
	}
}
