using System.Drawing;
using System.Windows.Forms;

namespace Capturador._01_Pantallas._00_Principal;

public class CustomColorTable : ProfessionalColorTable
{
	public override Color MenuItemSelected => Color.DarkBlue;

	public override Color MenuItemSelectedGradientBegin => Color.DarkBlue;

	public override Color MenuItemSelectedGradientEnd => Color.DarkBlue;

	public override Color MenuItemBorder => Color.Blue;

	public override Color MenuItemPressedGradientBegin => Color.Navy;

	public override Color MenuItemPressedGradientEnd => Color.Navy;
}
