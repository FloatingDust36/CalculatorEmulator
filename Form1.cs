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
        bool disableOperators = false;
        bool disableMemory = true;
        int indexMemory = -1;
        //this is an array that will store the expressions that the user has confirmed witht the equals button
        List<string> storedExpressions = new List<string>();


        private void buttonNumbers_Click(object sender, EventArgs e) //this is the event handler for the number buttons
        {
            indexMemory = -1;
            disableOperators = false;
            string currentInput = (sender as Button).Text;

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
                else if (displayedExpression.Length >= 2 &&
                         (displayedExpression[displayedExpression.Length - 2] == '+' || displayedExpression[displayedExpression.Length - 2] == '–' ||
                         displayedExpression[displayedExpression.Length - 2] == '×' || displayedExpression[displayedExpression.Length - 2] == '÷' ||
                         displayedExpression[displayedExpression.Length - 2] == '-' || displayedExpression[displayedExpression.Length - 2] == '(' ||
                         displayedExpression[displayedExpression.Length - 2] == ')') &&
                         lastChar == '0' && currentInput != "." &&
                         char.IsDigit(currentInput[0]))
                {
                    // If the last characters are "X+0" or the like and the current input is a non-decimal digit, remove the 0
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

        private void btnNegative_Click(object sender, EventArgs e)
        {
            indexMemory = -1;
            string currentInput = (sender as Button).Text;

            if (displayedExpression.Length == 0)
            {
                currentInput = "-";
            }
            else
            {
                char lastChar = displayedExpression[displayedExpression.Length - 1];

                if (lastChar != '×' && lastChar != '÷' && lastChar != '+' && lastChar != '–' && lastChar != '(' && lastChar != ')')
                {
                    return; //the user can't input a negative symbol if the last character is a number or a decimal
                }
                else
                {
                    currentInput = "-";
                }
            }

            displayedExpression += currentInput;
            tbxMainBox.Text = displayedExpression;
            disableOperators = true;

            if (startCalculation == true)
            {
                ComputeExpression();
            }
        }

        private void btnDecimal_Click(object sender, EventArgs e)
        {
            indexMemory = -1;
            disableOperators = false;
            btnDecimal.Enabled = false;

            string currentInput = (sender as Button).Text;
            displayedExpression += currentInput;
            tbxMainBox.Text = displayedExpression;

            if (startCalculation == true)
            {
                ComputeExpression();
            }
        }

        private void btnParenthesis_Click(object sender, EventArgs e)
        {
            indexMemory = -1;
            disableOperators = false;
            // this computes how many open and close parenthesis are in the expression
            int openParenthesis = displayedExpression.Count(f => f == '(');
            int closeParenthesis = displayedExpression.Count(f => f == ')');

            //if the number of open parenthesis is equal to the number of close parenthesis, the user can input an open parenthesis
            //if the number of open parenthesis is greater than the number of close parenthesis, the user can input a close parenthesis
            if (openParenthesis == closeParenthesis)
            {
                if (sender == btnOpenParenthesis)
                {
                    displayedExpression += "(";
                    tbxMainBox.Text = displayedExpression;

                    if (startCalculation == true)
                    {
                        ComputeExpression();
                    }
                }
                else
                {
                    return; //the user can't input a close parenthesis if there are no open parenthesis
                }
            }
            else if (openParenthesis > closeParenthesis)
            {
                if (sender == btnOpenParenthesis)
                {
                    return; //the user can't input an open parenthesis if there is an open parenthesis that is not closed
                }
                else
                {
                    displayedExpression += ")";
                    tbxMainBox.Text = displayedExpression;

                    if (startCalculation == true)
                    {
                        ComputeExpression();
                    }
                }
            }
        }

        private void buttonOperators_Click(object sender, EventArgs e)
        {
            indexMemory = -1;
            if (tbxMainBox.Text == string.Empty)
            {
                return; //the user can't input operators first
            }

            if (disableOperators == true)
            {
                return;
            }

            if (displayedExpression.Length > 0)
            {
                char lastChar = displayedExpression[displayedExpression.Length - 1];
                if (displayedExpression.Length == 1)
                {
                    if (lastChar == '.')
                    {
                        return;
                    }
                }
                else if (displayedExpression.Length > 2)
                {
                    char secondLastChar = displayedExpression[displayedExpression.Length - 2];

                    //prevents the operator from being inputted if the last character is a decimal and the second to the last char is not a number
                    if (lastChar == '.' && !char.IsDigit(secondLastChar))
                    {
                        return;
                    }
                }
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
            indexMemory = -1;
            disableOperators = false;
            if (displayedExpression.Length == 0)
            {
                return;
            }

            string formattedExpression = displayedExpression.Replace("×", "*").Replace("÷", "/").Replace("–", "-");

            try
            {
                var result = new DataTable().Compute(formattedExpression, null);
                if (result.ToString() == "∞" || result.ToString() == "NaN")
                {
                    tbxMainBox.Text = "Math Error";
                }
                else
                {
                    double numericResult = Convert.ToDouble(result);
                    tbxMainBox.Text = Math.Round(numericResult, 9).ToString();
                    // I used the Compute() function from the System.Data namespace to calculate the result instead of the hardcoded version
                    // The purpose of the try-catch construct is to catch invalid expressions, wherein in the catch block, it will print "Syntax Error"

                    //this will add the expression to the list of expressions
                    storedExpressions.Add(displayedExpression);
                    disableMemory = false;
                }
            }
            catch
            {
                tbxMainBox.Text = "Syntax Error";
            }

            displayedExpression = string.Empty;
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

            if (displayedExpression.Length == 0)
            {
                disableMemory = false;
            }
            ComputeExpression();
        }

        private void btnAllClear_Click(object sender, EventArgs e)
        {
            indexMemory = -1;
            disableMemory = false;
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

                //if the result is an infinity or a NaN, the program will display a Math error message
                if (result.ToString() == "∞" || result.ToString() == "NaN")
                {
                    tbxSecondaryBox.Text = string.Empty;
                }
                else
                {
                    double numericResult = Convert.ToDouble(result);
                    tbxSecondaryBox.Text = Math.Round(numericResult, 9).ToString();
                    // I used the Compute() function from the System.Data namespace to calculate the result instead of the hardcoded version
                    // The purpose of the try-catch construct is to catch invalid expressions, wherein in the catch block, it will print "Syntax Error"
                }
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

        private void buttonUpDown_Click(object sender, EventArgs e)
        {
            if (storedExpressions.Count == 0)
            {
                return;
            }

            if (sender == btnUp)
            {
                if (indexMemory == -1)
                {
                    indexMemory = storedExpressions.Count;
                }
                if(indexMemory == 0)
                {
                    return;
                }
                indexMemory--;
                
            }
            else if (sender == btnDown)
            {
                if(indexMemory == -1)
                {
                    return;
                }
                if(indexMemory+1 == storedExpressions.Count)
                {
                    return;
                }
                indexMemory++;
            }

            try
            {
                displayedExpression = storedExpressions[indexMemory];
                tbxMainBox.Text = displayedExpression;
                ComputeExpression();
            }
            catch
            {
                return;
            }
            
        }

    }
}
