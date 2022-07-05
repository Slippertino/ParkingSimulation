using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ParkingSimulation.Displayers
{
    public class UserConsoleDisplayer
    { 
        enum TextForeColors
        {
            STANDART     = 0xFFFFFF,
            FAIL         = 0xFF0000,
            SUCCESS      = 0x00FF00,
            COMMAND_NAME = 0x00FFFF,
            COMMAND_DESC = 0xFF8C00
        }

        private List<Tuple<int, int, Color>> textFormat = new List<Tuple<int, int, Color>>();

        public delegate void Sender(string instruction);
        private Sender SendInstruction;

        private RichTextBox displayerConsole;

        private const string consolePrefix = "$ ";
        private int lastPos;

        public bool GlobalBlock { get; set; }

        public UserConsoleDisplayer(ManagmentCore mCore, RichTextBox console, Size mainWindowSize, Size parkingSize) 
        {
            SendInstruction = mCore.HandleUserInstruction;

            displayerConsole = console;

            displayerConsole.Location = new Point(parkingSize.Width, 0);
            displayerConsole.Size     = new Size(mainWindowSize.Width - parkingSize.Width - 1, mainWindowSize.Height);

            displayerConsole.KeyDown   += new KeyEventHandler(Console_KeyDown);
            displayerConsole.KeyUp     += new KeyEventHandler(Console_KeyUp);

            Reset();

            InitConsoleNewLine();
        }

        private void Reset()
        {
            textFormat.Clear();
            lastPos = -1;
        }

        private void ApplyFormat()
        {
            foreach (var cur in textFormat)
            {
                displayerConsole.SelectionStart  = cur.Item1;
                displayerConsole.SelectionLength = cur.Item2;
                displayerConsole.SelectionColor  = cur.Item3;
            }
        }

        public void DisplayResponse(string message, ResponseStatus status)
        {
            displayerConsole.Text = displayerConsole.Text.Remove(lastPos + 1);

            switch (status)
            {
                case ResponseStatus.FAIL:
                    textFormat.Add(Tuple.Create(displayerConsole.Text.Length,
                                   message.Length,
                                   Color.FromArgb(Convert.ToInt32(TextForeColors.FAIL))));
                    break;
                case ResponseStatus.SUCCESS:
                    textFormat.Add(Tuple.Create(displayerConsole.Text.Length,
                                   message.Length,
                                   Color.FromArgb(Convert.ToInt32(TextForeColors.SUCCESS))));
                    break;
            }

            displayerConsole.Text += message + "\n";

            lastPos = displayerConsole.Text.LastIndexOf("\n");

            InitConsoleNewLine();
        }

        public void DisplayUserInstruction(Dictionary<string, string> instructionsDescription)
        {
            displayerConsole.Text = displayerConsole.Text.Remove(lastPos + 1);
            displayerConsole.Text += "Описание команд:\n\n";

            foreach (var curDescription in instructionsDescription)
            {
                textFormat.Add(Tuple.Create(displayerConsole.Text.Length,
                                            curDescription.Key.Length,
                                            Color.FromArgb(Convert.ToInt32(TextForeColors.COMMAND_NAME))));

                displayerConsole.Text += curDescription.Key + "\n";


                textFormat.Add(Tuple.Create(displayerConsole.Text.Length,
                                            curDescription.Value.Length,
                                            Color.FromArgb(Convert.ToInt32(TextForeColors.COMMAND_DESC))));

                displayerConsole.Text += curDescription.Value + "\n\n";
            }

            lastPos = displayerConsole.Text.LastIndexOf("\n");

            InitConsoleNewLine();
        }

        private string GetInstruction()
        {
            int posLineEnd   = lastPos;
            int posLineBegin = lastPos - 1;

            while (posLineBegin >= 0 && displayerConsole.Text[posLineBegin] != '\n')
            {
                posLineBegin--;
            }

            posLineBegin += consolePrefix.Length + 1;

            return displayerConsole.Text.Substring(posLineBegin, posLineEnd - posLineBegin);
        }

        private void Console_KeyDown(object sender, KeyEventArgs e)
        {
            int posCursor = displayerConsole.SelectionStart;
            int posNewLine = lastPos + 1;

            if (e.KeyCode == Keys.Back)
            {
                Block(posCursor <= posNewLine + consolePrefix.Length);
            }
            else
            {
                Block(posCursor < posNewLine + consolePrefix.Length);
            }
        }

        private void Console_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Block(true);

                    lastPos = displayerConsole.Text.LastIndexOf("\n");

                    InitConsoleNewLine();

                    Block(false);

                    SendInstruction(GetInstruction());
                }
            }
            catch (Exception) { }
        }

        private void InitConsoleNewLine()
        {
            displayerConsole.Visible = false;

            if (displayerConsole.Text.Length > lastPos + 1)
                displayerConsole.Text = displayerConsole.Text.Remove(lastPos + 1);

            displayerConsole.Text += consolePrefix;

            ApplyFormat();

            displayerConsole.Visible = true;

            displayerConsole.Select(0, 0);
            displayerConsole.ScrollToCaret();
            displayerConsole.Select(lastPos + consolePrefix.Length + 1, 0);
        }

        public void Clear()
        {
            Block(true);

            displayerConsole.Clear();
            Reset();
            InitConsoleNewLine();

            Block(false);
        }

        public void Block(bool predicate)
        {
            displayerConsole.ReadOnly = predicate | GlobalBlock;
        }
    }
}
