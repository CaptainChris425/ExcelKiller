using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionTree
{
    public class ExpTree
    {
        Dictionary<char, int> precedence = new Dictionary<char, int>() { { '*', 3 }, { '/', 3 }, { '+', 2 }, { '-', 2 } };
        NodeFactory NodeFactory = new NodeFactory();
        Node t1, t2, t3;
        Node root;
        string equation;
        public Dictionary<string, double> variables = new Dictionary<string, double>();
        public void SetVAr(string varName, double Varvalue)
        {
            /*Adds a variable to the dictionary
             */
            if (variables.ContainsKey(varName))
            {
                variables.Remove(varName);
            }
            variables.Add(varName, Varvalue);
        }
        public void ClearVars()
        {
            /*Clear variable dictionary
             */
            variables.Clear();
        }
        public ExpTree(string equ)
        {
            /* Constructor to set equation
             */
            equation = equ;
        }
        void MakeTree(string equation)
        {
            /* converts equation and sets root
             */
            string[] postfix = InfixtoPostfix(equation).Split(','); //convert to postfix
            foreach (string token in postfix)
            {
                if (Char.IsLetter(token[0]) && !variables.ContainsKey(token))
                {
                    variables.Add(token, 0);
                }
            }
            root = ConstructTree(postfix); //set root to a node representing a tree
        }
        Node ConstructTree(string[] expression)
        {
            /*
             * Creates a tree from an array of strings
             * returns root node
             */
            Stack<Node> stack = new Stack<Node>();
            foreach (string var in expression)
            {
                string bit;
                if (variables.ContainsKey(var)) //If character is a variable name 
                {
                    bit = variables[var].ToString(); //set current char to its value
                }
                else { bit = var; }
                if (Isoperand(bit)) //If char is a operand
                {
                    t1 = NodeFactory.GetNode(bit); //get a node from the factory
                    t2 = stack.Pop(); //pop two items off the stack
                    t3 = stack.Pop();
                    t1.Right = t2; //make the two items the children
                    t1.Left = t3;
                    stack.Push(t1); //push the parent onto the stack
                }
                else //If the char is not an operand
                {
                    t1 = NodeFactory.GetNode(bit); //get the correct node
                    stack.Push(t1); //push the new node
                }
            }
            t1 = stack.Peek(); //set t1 to the top item in the stack
            stack.Pop();
            return t1; //return t1 as the root of the tree

        }
        public double Eval()
        {
            MakeTree(equation); //Create a tree
            return root.Eval(); //Evaluate the equation using the roots native eval function
        }
        string InfixtoPostfix(string infix)
        {
            /*
              * Using a stack converts a basic equation
               * from infix notation to postfix notation
               */
            Stack<char> output = new Stack<char>();
            Stack<char> op = new Stack<char>();
            StringBuilder postfix = new StringBuilder();
            char token;
            for (int i = 0; i < infix.Length; i++) //Basically a foreach but allows for skipping
            {
                token = infix[i];
                if (Isoperand(token)) //If current token is an operator
                {
                    while (op.Count != 0 && op.Peek() != '(' && (precedence[token] < precedence[op.Peek()]))
                    //while there is an operator on the operator stack with higher presedence than the token and the operator stack is not a left bracket
                    {
                        output.Push(op.Pop()); //push the operator onto the optput stack
                        output.Push(',');
                    }
                    op.Push(token); //push the operator token onto the operator stack
                }
                else if (token == '(') //if the token is a left bracket
                {
                    op.Push(token); //push it onto the operator stack
                }
                else if (token == ')')
                {    //if the token is a right bracket
                    while (op.Peek() != '(') // while the top of the operator stack is not a left bracket
                    {
                        output.Push(op.Pop());  //push the operator stack onto the output
                        output.Push(',');
                    }
                    op.Pop(); //pop the mathing parenthesis
                }
                else //token is a number or a variable
                {
                    output.Push(token); //push the token onto the stack
                    while (i < infix.Length - 1 && !Isoperand(infix[i + 1]) && infix[i + 1] != ')')
                    {
                        i++;
                        token = infix[i];
                        output.Push(token);
                    }
                    if (i + 2 == infix.Length)
                    {
                        if (!Isoperand(infix[i + 1]) && infix[i + 1] != ')')
                        {
                            i++;
                            token = infix[i];
                            output.Push(token);
                        }
                    }
                    output.Push(',');
                }
            }
            while (op.Count != 0)
            {
                output.Push(op.Pop());
                output.Push(',');
            }
            output.Pop(); //delete last deliminator
            while (output.Count != 0)
            {
                postfix.Insert(0, (output.Pop()));
            }
            return postfix.ToString();
        }
        bool Isoperand(char value)
        {
            return (value == '+' || value == '-'
                || value == '*' || value == '/'
                || value == '^');
        }
        bool Isoperand(string value)
        {
            return (value == "+" || value == "-"
                || value == "*" || value == "/"
                || value == "^");
        }

    }
    public abstract class Node
    {
        /* 
         * Abstract Node Class
         * {Left,Right,Value}
         *
         */
        public abstract double Eval();
        string value;
        public Node right;
        public Node left;
        protected Node(string value)
        {
            this.Value = value;
            left = right = null;
        }
        public Node Left { get => left; set => left = value; }
        public Node Right { get => right; set => right = value; }
        public string Value { get => value; set => this.value = value; }
    }
    public class NodeFactory
    {
        /*
         * Factory for returning concrete nodes
         * who know how to evaluate themselves
         */
        public NodeFactory()
        {
        }
        public Node GetNode(string value)
        {
            if (Isoperand(value))
            {
                return new ExpressionNode(value);
            }
            else
            {
                return new ValueNode(value);
            }
        }
        public class ExpressionNode : Node
        {
            override public double Eval()
            {
                switch (Value)
                {
                    case ("+"):
                        return Left.Eval() + Right.Eval();
                    case ("-"):
                        return Left.Eval() - Right.Eval();
                    case ("*"):
                        return Left.Eval() * Right.Eval();
                    case ("/"):
                        return Left.Eval() / Right.Eval();
                    default:
                        return 0;
                }

            }
            public ExpressionNode(string value) : base(value)
            {
            }
        }
        public class ValueNode : Node
        {
            public ValueNode(string value) : base(value)
            {
            }

            public override double Eval()
            {
                return double.Parse(Value);
            }
        }
        private bool Isoperand(string value)
        {
            return (value == "+" || value == "-"
                || value == "*" || value == "/"
                || value == "^");
        }
    }
}
