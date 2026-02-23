using System.Drawing;
using System.Windows.Forms;

namespace Capturador._01_Pantallas._00_Principal;

public class CustomMenuRenderer : ToolStripProfessionalRenderer
{
	public CustomMenuRenderer()
		: base(new CustomColorTable())
	{
	}

	protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
	{
		if (e.Item.Selected)
		{
			e.Graphics.FillRectangle(Brushes.SteelBlue, e.Item.Bounds);
			e.Item.ForeColor = Color.White;
		}
		else
		{
			e.Graphics.FillRectangle(new SolidBrush(Color.LightGray), e.Item.Bounds);
			e.Item.ForeColor = Color.Black;
		}
	}

	protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
	{
		e.TextColor = e.Item.ForeColor;
		base.OnRenderItemText(e);
	}
}
