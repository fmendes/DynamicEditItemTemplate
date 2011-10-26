using System;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI.WebControls;

namespace SCS.Web.UI.WebControls
{
    public class TabStyle : Style
    {
        [Browsable(false)]
        private new BorderStyle BorderStyle
        {
            get { return base.BorderStyle; }
            set { base.BorderStyle = value; }
        }
        [Browsable(false)]
        private new Color BorderColor
        {
            get { return base.BorderColor; }
            set { base.BorderColor = value; }
        }

    }
}
