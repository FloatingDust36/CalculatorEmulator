using System.Data;
using System.Drawing.Drawing2D;

namespace CalculatorEmulator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string displayedExpression = string.Empty;
        bool startCalculation = false;

        private void buttonNumbers_Click(object sender, EventArgs e)
        {
            string currentInput = (sender as Button).Text;

            if (currentInput == "(-)")
            {
                if (displayedExpression.Length == 0)
                {
                    currentInput = "-"; //this is for the negative button, so that the user can input negative numbers
                }
                else
                {
                    char lastChar = displayedExpression[displayedExpression.Length - 1];

                    if (lastChar != '×' && lastChar != '÷' && lastChar != '+' && lastChar != '–')
                    {
                        return; //this is for the negative button, so that the user can't input the symbol after a number
                    }
                    else
                    {
                        currentInput = "-"; //this is for the negative button, so that the user can input negative numbers
                    }
                }
            }

            if (currentInput == ".")
            {
                btnDecimal.Enabled = false;
            }

            // this is the code where if 0 is pressed and followed with a non-zero digit, the non-zero digit will only be displayed
            if (displayedExpression.Length > 0)
            {
                char lastChar = displayedExpression[displayedExpression.Length - 1];

                // Check for invalid leading zero cases
                if (lastChar == '0' && currentInput != "." && displayedExpression.Length == 1)
                {
                    // If the expression is just "0" and a non-decimal digit is added, replace the 0
                    displayedExpression = string.Empty;
                }
                else if ((lastChar == '+' || lastChar == '–' || lastChar == '×' || lastChar == '÷') && currentInput == "0")
                {
                    // If the last character is an operator and the current input is 0, do nothing
                    // This ensures "5+0" remains valid but "5+098" becomes "5+98"
                }
                else if (displayedExpression.Length >= 2 &&
                         (displayedExpression[displayedExpression.Length - 2] == '+' || displayedExpression[displayedExpression.Length - 2] == '–' || displayedExpression[displayedExpression.Length - 2] == '×' || displayedExpression[displayedExpression.Length - 2] == '÷') &&
                         lastChar == '0' && currentInput != "." &&
                         char.IsDigit(currentInput[0]))
                {
                    // If the last two characters are "X+0" or the like and the current input is a non-decimal digit, remove the 0
                    displayedExpression = displayedExpression.Substring(0, displayedExpression.Length - 1);
                }
            }

            displayedExpression += currentInput;
            // gets the value of the text property of selected buttons like 1,2,+,=, etc.
            // this is so that I don't have to create individual events for these buttons, with the same functions
            /*
               In order to make this happen, I highlighted all the selected buttons and went to the Events tab 
               which is beside the Properties tab and, set the Click event to button_Click
            */

            tbxMainBox.Text = displayedExpression;

            if (startCalculation == true)
            {
                ComputeExpression();
            }
        }

        private void buttonOperators_Click(object sender, EventArgs e)
        {
            if (tbxMainBox.Text == string.Empty)
            {
                return; //this is for the multiplication and division buttons, so that the user can't input the symbols at the start of the expression
            }

            if (displayedExpression.Length > 0)
            {
                char lastChar = displayedExpression[displayedExpression.Length - 1];

                // this check if the last character is an operator and possibly replace it with the new one
                if (lastChar == '×' || lastChar == '÷' || lastChar == '+' || lastChar == '–')
                {
                    displayedExpression = displayedExpression.Substring(0, displayedExpression.Length - 1) + (sender as Button).Text;
                    tbxMainBox.Text = displayedExpression;
                    return;
                }
            }

            btnDecimal.Enabled = true;
            startCalculation = true; //once the user inputs an operator, the program will start calculating the expression if it is feasible and display the result in the secondary textbox
            displayedExpression += (sender as Button).Text;

            tbxMainBox.Text = displayedExpression;
            ComputeExpression();
        }

        private void btnEquals_Click(object sender, EventArgs e)
        {
            if(displayedExpression.Length == 0)
            {
                return;
            }

            string formattedExpression = displayedExpression.Replace("×", "*").Replace("÷", "/").Replace("–", "-");

            try
            {
                var result = new DataTable().Compute(formattedExpression, null);
                double numericResult = Convert.ToDouble(result);
                tbxMainBox.Text = Math.Round(numericResult, 9).ToString();
                // I used the Compute() function from the System.Data namespace to calculate the result instead of the hardcoded version
                // The purpose of the try-catch construct is to catch invalid expressions, wherein in the catch block, it will print "Syntax Error"
            }
            catch
            {
                tbxMainBox.Text = "Syntax Error";
            }

            tbxSecondaryBox.Text = string.Empty;
            btnDecimal.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (displayedExpression.Length > 0)
            {
                displayedExpression = displayedExpression.Remove(displayedExpression.Length - 1);

                int indexOperator = 0, indexDecimal = 0;
                // this gets the index of the last operator and decimal in the expression
                indexOperator = displayedExpression.LastIndexOfAny(new char[] { '+', '–', '×', '÷' });
                indexDecimal = displayedExpression.LastIndexOf('.');

                //this determines who is more recent, the operator or the decimal
                if (indexOperator >= indexDecimal)
                {
                    btnDecimal.Enabled = true;
                }
                else
                {
                    btnDecimal.Enabled = false;
                }

                tbxMainBox.Text = displayedExpression;
                tbxSecondaryBox.Text = string.Empty;
            }

            ComputeExpression();
        }

        private void btnAllClear_Click(object sender, EventArgs e)
        {
            btnDecimal.Enabled = true;
            displayedExpression = string.Empty;
            tbxMainBox.Text = string.Empty;
            tbxSecondaryBox.Text = string.Empty;
            tbxSecondaryBox.Text = string.Empty;
        }

        //this computes the expression whatever button the user presses
        private void ComputeExpression()
        {
            string formattedExpression = displayedExpression.Replace("×", "*").Replace("÷", "/").Replace("–", "-");

            try
            {
                var result = new DataTable().Compute(formattedExpression, null);
                double numericResult = Convert.ToDouble(result);
                tbxSecondaryBox.Text = Math.Round(numericResult, 9).ToString();
                // I used the Compute() function from the System.Data namespace to calculate the result instead of the hardcoded version
                // The purpose of the try-catch construct is to catch invalid expressions, wherein in the catch block, it will print "Syntax Error"
            }
            catch
            {
                tbxSecondaryBox.Text = string.Empty;
            }
        }

        private void btnRoundedButtons_Paint(object sender, PaintEventArgs e)
        {
            Button btn = sender as Button;
            GraphicsPath path = new GraphicsPath();

            int radius = 20; // Adjust the corner radius
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(btn.Width - radius, 0, radius, radius), 270, 90);
            path.AddArc(new Rectangle(btn.Width - radius, btn.Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, btn.Height - radius, radius, radius), 90, 90);
            path.CloseAllFigures();

            btn.Region = new Region(path);

            // Optionally, draw a border
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(Color.Black, 2))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void btnCircluarButtons_Paint(object sender, PaintEventArgs e)
        {
            Button btn = sender as Button;
            int diameter = Math.Min(btn.Width, btn.Height); // Ensures the button becomes circular even if Width and Height differ
            GraphicsPath path = new GraphicsPath();

            path.AddEllipse(0, 0, diameter, diameter); // Create a circular path
            btn.Region = new Region(path);

            // Optionally adjust button size to match a perfect circle
            btn.Width = diameter;
            btn.Height = diameter;
        }

        private void pnlRoundedPanels_Paint(object sender, PaintEventArgs e)
        {
            Panel pnl = sender as Panel;
            GraphicsPath path = new GraphicsPath();

            int radius = 25; // Adjust the corner radius
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(pnl.Width - radius, 0, radius, radius), 270, 90);
            path.AddArc(new Rectangle(pnl.Width - radius, pnl.Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, pnl.Height - radius, radius, radius), 90, 90);
            path.CloseAllFigures();

            pnl.Region = new Region(path);

            // Optionally, draw a border
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(Color.Black, 2))
            {
                e.Graphics.DrawPath(pen, path);
            }
        }
    }
}
