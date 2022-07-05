using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using ParkingSimulation.SerializableEntities;

namespace ParkingSimulation.Displayers
{
    public class ParkingDisplayer
    {
        public delegate void   DataToCore  (Point spaceControlPoint);
        public delegate string DataFromCore(Point spaceControlPoint);

        private DataToCore   Sender;
        private DataFromCore RequestSpaceInfo;

        private PictureBox   displayerParking;

        private PictureBox[] parkingSpaces;
        private static Color defaultSpaceColor;
        private static Color specialSpaceColor = Color.Blue;

        private ToolTip spaceHint;
        private static Color hintBackColor = Color.DodgerBlue;
        private static Color hintForeColor = Color.Yellow;

        public  Bitmap DefaultBitmap { get; set; }

        private ParkingRenderer pRenderer;

        public ParkingDisplayer(ManagmentCore mCore, PictureBox parking, Size mainWindowSize, ParkingMap map)
        {
            displayerParking = parking;

            Sender           = mCore.HandleUserSpaceChoice;
            RequestSpaceInfo = mCore.GetSpaceInfo;

            defaultSpaceColor = map.GetParkingBackColor();

            displayerParking.Enabled   = true;
            displayerParking.Visible   = false;
            displayerParking.Size      = map.GetParkingSize();
            displayerParking.Location  = new Point(0, mainWindowSize.Height - displayerParking.Size.Height);

            CreateDefault(map);
            CreateHint();
            CreateSpaces (map);

            pRenderer = new ParkingRenderer(this, map);
        }

        public void Invalidate(Bitmap bmp)
        {
            displayerParking.Image = bmp;
            displayerParking.Invalidate();
        }

        public void Update(Bitmap bmp)
        {
            displayerParking.Image = bmp;
            displayerParking.Update();
        }

        public void FlushSpaces()
        {
            for (int i = 0; i < parkingSpaces.Length; i++)
            {
                parkingSpaces[i].Image     = null;
                parkingSpaces[i].BackColor = defaultSpaceColor;
            }
        }

        private void CreateDefault(ParkingMap map)
        {
            DefaultBitmap = new Bitmap(displayerParking.Width, displayerParking.Height);
            Graphics g = Graphics.FromImage(DefaultBitmap);

            g.FillRectangle(new SolidBrush(defaultSpaceColor), 0, 0, displayerParking.Width, displayerParking.Height);

            foreach (var marking in map.GetMarkings())
            {
                g.DrawLine(new Pen(marking.Color.ConvertToColor(), marking.Width),
                           marking.Begin,
                           marking.End);
            }
        }

        public void HighLightSpaces(Dictionary<Point, ParkingCell> spaces)
        {
            for (int i = 0; i < parkingSpaces.Length; i++)
            {
                try
                {
                    var cur = spaces[parkingSpaces[i].Location];

                    parkingSpaces[i].Enabled = true;
                    parkingSpaces[i].BackColor = specialSpaceColor;
                    parkingSpaces[i].Visible = true;
                }
                catch(Exception)
                {
                    continue;
                }
            }
        }

        public void DrawEmployedSpaces(Dictionary<Point, ParkingCell> spaces)
        {
            for (int i = 0; i < parkingSpaces.Length; i++)
            {
                try
                {
                    var cur = spaces[parkingSpaces[i].Location];
                    Bitmap bmp = new Bitmap(parkingSpaces[i].Width, parkingSpaces[i].Height);
                    pRenderer.DrawVehicle(new Point(0, 0), cur, ref bmp);
                    parkingSpaces[i].Image = bmp;
                }
                catch(Exception)
                {
                    continue;
                }
            }
        }

        private void ChangeSpacesCondition(bool visionbPredicate, bool enablingPredicate, Color backColor)
        {
            for (int i = 0; i < parkingSpaces.Length; i++)
            {
                parkingSpaces[i].Visible   = visionbPredicate;
                parkingSpaces[i].Enabled   = enablingPredicate;
                parkingSpaces[i].BackColor = backColor;
            }
        }

        public void DrawEntrance(ParkingCell space)
        {
            Bitmap bmp = new Bitmap(displayerParking.Image);
            pRenderer.DrawVehicle(space.CellSettings.ControlPoint, space, ref bmp);
            Update(bmp);
        }

        public void DisplayVehicleMove(Dictionary<Point, ParkingCell> employedSpaces,
                                       ParkingCell start,
                                       ParkingCell finish)
        {
            pRenderer.RenderVehicleMove(employedSpaces, start, finish);
        }

        public void Create()
        {
            Update(DefaultBitmap);

            ChangeSpacesCondition(true, true, defaultSpaceColor);

            displayerParking.Enabled = true;
            displayerParking.Visible = true;
        }

        public void Reset()
        {
            ChangeSpacesCondition(false, false, defaultSpaceColor);

            FlushSpaces();
            displayerParking.Enabled = false;
            displayerParking.Visible = false;
        }

        private void CreateSpaces(ParkingMap map)
        {
            parkingSpaces = new PictureBox[map.GetCellsCount()];

            int id = 0;
            foreach (var space in map.GetParkingSpaces())
            {
                parkingSpaces[id] = CreateSpace(id++ + 1, space.Value);
            }
        }

        private void CreateHint()
        {
            spaceHint = new ToolTip();

            spaceHint.BackColor    = hintBackColor;
            spaceHint.ForeColor    = hintForeColor;
            spaceHint.OwnerDraw    = true;
            spaceHint.InitialDelay = 0;
            spaceHint.ReshowDelay  = 0;
            spaceHint.ToolTipIcon  = System.Windows.Forms.ToolTipIcon.Info;
            spaceHint.ShowAlways   = true;
            spaceHint.Draw         += new DrawToolTipEventHandler(this.spaceHint_Draw);
        }

        private void spaceHint_Draw(object sender, DrawToolTipEventArgs e)
        {
            e.DrawBackground();

            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                using (Font f = new Font("Times New Roman", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))))
                {
                    e.Graphics.DrawString(e.ToolTipText, f, Brushes.Yellow, e.Bounds, sf);
                }
            }

            e.DrawBorder();
        }

        private PictureBox CreateSpace(int id, CellParams cell)
        {
            PictureBox res = new PictureBox();

            res.Enabled     = false;
            res.Visible     = false; 
            res.Location    = cell.ControlPoint;
            res.BackColor   = defaultSpaceColor;
            res.BorderStyle = BorderStyle.None;
            res.Name        = "Space " + Convert.ToString(id);
            res.TabIndex    = 2;
            res.TabStop     = false;
            res.Size        = cell.Size;
            res.Parent      = displayerParking;
            res.Click       += new EventHandler(parkingSpace_Click);
            res.MouseHover  += new EventHandler(parkingSpace_MouseHover);

            displayerParking.Controls.Add(res);

            res.BringToFront();

            return res;
        }

        public void parkingSpace_Click(object sender, EventArgs e)
        {
            PictureBox curPbox = (PictureBox)sender;

            if (curPbox.BackColor != specialSpaceColor)
                return;

            ChangeSpacesCondition(false, false, defaultSpaceColor);

            Sender(((PictureBox)sender).Location);

            ChangeSpacesCondition(true, true, defaultSpaceColor);
        }

        public void parkingSpace_MouseHover(object sender, EventArgs e)
        {
            PictureBox curPbox = (PictureBox)sender;

            spaceHint.SetToolTip(curPbox, RequestSpaceInfo(curPbox.Location));
        }
    }
}
