using System.Data;

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
                if(displayedExpression.Length == 0)
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
            if (displayedExpression.Length > 0 && displayedExpression[displayedExpression.Length - 1] == '0' && currentInput != ".")
            {
                displayedExpression = displayedExpression.Substring(0, displayedExpression.Length - 1);
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

            string formattedExpression = displayedExpression.Replace("×", "*").Replace("÷", "/").Replace("–", "-");

            try
            {
                tbxMainBox.Text = new DataTable().Compute(formattedExpression, null).ToString();
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
                tbxSecondaryBox.Text = new DataTable().Compute(formattedExpression, null).ToString();
                // I used the Compute() function from the System.Data namespace to calculate the result instead of the hardcoded version
                // The purpose of the try-catch construct is to catch invalid expressions, wherein in the catch block, it will print "Syntax Error"
            }
            catch
            {
                tbxSecondaryBox.Text = string.Empty;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
