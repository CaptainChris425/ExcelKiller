using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionTree;

namespace SpreadsheetEngine
{
    
}

namespace CptS321
{
    public abstract class Cell : INotifyPropertyChanged //Abstract cell class that can handle change events
    {
        //Cell contains its location and a string representation of its value
        public Cell(int row, int column)
        {
            Textvalue = "0";
            RowIndexvalue = row;
            ColumnIndexvalue = column;
        }
        private int RowIndexvalue = 0; //row location of cell in spreadsheet
        private int ColumnIndexvalue = 0; //column location of cell in spreadsheet
        private string Textvalue = String.Empty; //what will be displayed to the ui
        private string ValueValue = String.Empty; //evaluates if = is the first char
        public event PropertyChangedEventHandler PropertyChanged;
        
        public void NotifyPropertyChanged(string text)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(text));
        }
        
        public string Text
        {
            get { return Textvalue; }
            set {
                if (value != Textvalue)
                {
                    //Only change the text if it is different than what was before
                    
                    Textvalue = value;
                    NotifyPropertyChanged("Text"); //Notify that text has changed
                }
            }
        }
        public int RowIndex
        {
            get { return RowIndexvalue; }
        }
        public int ColumnIndex
        {
            get { return ColumnIndexvalue; }
        }
        public string Value
        {
            get { return ValueValue; }
            set
            {
                 ValueValue = value;
                 NotifyPropertyChanged("Value");
            }
        }


    }

    public class SpreadSheet
    {
        public class SpreadsheetCell : Cell
        {
            
            public SpreadsheetCell(int row, int column): base(row, column)
            {
            }
            private HashSet<SpreadsheetCell> referencesme = new HashSet<SpreadsheetCell>();
           
            public void Update()
            {
                string temp = Value;
                Value = string.Empty;
                Text = temp;
                
            }
            public void Sendupdate()
            {
                foreach(SpreadsheetCell cell in referencesme)
                {
                    cell.Update();
                }
            }
            public bool Referencesme(SpreadsheetCell cell)
            {
                return referencesme.Add(cell);
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return base.ToString();
            }

        }
        
        public SpreadsheetCell[,] cells;
        public SpreadSheet(int row, int column)
        {
            cells = new SpreadsheetCell[row,column];
            for (int i = 0; i<row; ++i)
            {
                for (int j = 0; j<column; ++j)
                {
                    
                    cells[i, j] = new SpreadsheetCell(i, j);
                    //subscribe so that the spreadsheet class knows when a cell value is changed
                    cells[i, j].PropertyChanged += Cells_PropertyChanged;
                }
            }

        }
        SpreadsheetCell getcellat(string column, string row)
        {
            column = column.ToLower();
            int col = column[0] - 'a';
            int roW = int.Parse(row)-1;
            return cells[ roW,col];
        }
        public void Cells_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            SpreadsheetCell spreadsheetCell = sender as SpreadsheetCell; //Sender as a spreadsheet cell

            switch (e.PropertyName) //switch on the property changed
            {
                case ("Text"): //if text has been changed
                    if (spreadsheetCell.Text == string.Empty)
                    {
                        spreadsheetCell.Value = string.Empty;
                        NotifyCellPropertyChanged(spreadsheetCell);
                        //spreadsheetCell.Sendupdate();
                    }

                    else if (spreadsheetCell.Text[0] == '=')
                    {
                        spreadsheetCell.Value = spreadsheetCell.Text;
                        NotifyCellPropertyChanged(spreadsheetCell);
                        spreadsheetCell.Sendupdate();
                    }
                    else if (spreadsheetCell.Text[0] != '=' && spreadsheetCell.Value == string.Empty)
                    {
                        spreadsheetCell.Value = spreadsheetCell.Text;
                        NotifyCellPropertyChanged(spreadsheetCell);
                        spreadsheetCell.Sendupdate();
                    }
                    else if (spreadsheetCell.Text[0] != '=' && spreadsheetCell.Value[0] != '=')
                    {
                        spreadsheetCell.Value = spreadsheetCell.Text;
                        NotifyCellPropertyChanged(spreadsheetCell);
                        spreadsheetCell.Sendupdate();
                    }

                    break;
                case ("Value"):
                    try
                    {
                        if (spreadsheetCell.Value != string.Empty)
                        {
                            if (spreadsheetCell.Value[0] == '=')
                            {
                                if (char.IsLetter(spreadsheetCell.Value[1]) && spreadsheetCell.Value.Length <= 4 && spreadsheetCell.Value.Length >= 2)
                                {
                                    string varcol = spreadsheetCell.Value[1].ToString();
                                    string varrow = spreadsheetCell.Value.Substring(2);
                                    SpreadsheetCell referencedcell = getcellat(varcol, varrow);
                                    if (spreadsheetCell.Text != referencedcell.Text)
                                    {
                                        spreadsheetCell.Text = referencedcell.Text;
                                    }
                                    referencedcell.Referencesme(spreadsheetCell);
                                }
                                else if (spreadsheetCell.Value.Length >= 2) //starts with = and is within the length of a valid function
                                {
                                    ExpTree expTree = new ExpTree(spreadsheetCell.Value.Substring(1));
                                    expTree.Eval();
                                    string varcol;
                                    string varrow;
                                    double vardata;
                                    Dictionary<string, double> vardic = new Dictionary<string, double>(expTree.variables);
                                    foreach (string key in vardic.Keys)
                                    {
                                        varcol = key.Substring(0, 1);
                                        varrow = key.Substring(1);
                                        SpreadsheetCell referencedcell = getcellat(varcol, varrow);
                                        if (referencedcell == spreadsheetCell)
                                        {
                                            //ToDo-- If self is in the equation
                                            vardata = 0;
                                        }
                                        else { Double.TryParse(referencedcell.Text, out vardata); }

                                        referencedcell.Referencesme(spreadsheetCell);
                                        expTree.SetVAr(key, vardata);

                                    }
                                    string eval = expTree.Eval().ToString();
                                    if (spreadsheetCell.Text != eval)
                                    {

                                        spreadsheetCell.Text = eval;
                                    }

                                }
                            }
                        }
                    }
                    catch
                    {
                        spreadsheetCell.Text = "#ERROR";
                    }
                    break;
              
            }
            
            
            
        }
        public static Cell CellFactory(string celltype, int row, int column)
        {
            switch (celltype) //Switch on celltype string to return the correct object type
            {
                case ("Spreadsheet"):
                    return new SpreadsheetCell(row, column);
                default:
                    return null;
            }
        }
        public SpreadsheetCell GetCell(int row, int column)
        {
            return cells[row, column];
        }
        public void SetCell(int row, int column, string text)
        {
            cells[row, column].Text = text;
        }
        
        public event PropertyChangedEventHandler CellPropertyChanged;
        public void NotifyCellPropertyChanged(Cell cell)
        {
            CellPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(cell.RowIndex.ToString()+","+ cell.ColumnIndex.ToString()));
        }
        public void Exporttoxml(string documentname)
        {
            XmlDocument xmldoc = new XmlDocument();
            XmlElement ssnode = xmldoc.CreateElement("spreadsheet");
            xmldoc.AppendChild(ssnode);

            int maxrow = cells.GetLength(0);
            int maxcol = cells.GetLength(1);
            for (int i = 0; i< maxrow; i++)
            {
                for (int j = 0; j<maxcol; j++)
                {
                    if (cells[i, j].Value != string.Empty)
                    {
                        XmlElement cellNode = xmldoc.CreateElement("cell");
                        XmlElement cellrow = xmldoc.CreateElement("row");
                        XmlText row = xmldoc.CreateTextNode(i.ToString());
                        cellrow.AppendChild(row);
                        XmlElement cellcol = xmldoc.CreateElement("col");
                        XmlText col = xmldoc.CreateTextNode(j.ToString());
                        cellcol.AppendChild(col);
                        XmlElement cellvalue = xmldoc.CreateElement("value");
                        XmlText val = xmldoc.CreateTextNode(cells[i, j].Value);
                        cellvalue.AppendChild(val);
                        cellNode.AppendChild(cellrow);
                        cellNode.AppendChild(cellcol);
                        cellNode.AppendChild(cellvalue);
                        ssnode.AppendChild(cellNode);
                    }
                }
            }

            xmldoc.Save(documentname);
        }
        public void Clearsheet()
        {
            int row = cells.GetLength(0);
            int column = cells.GetLength(1);
            cells = new SpreadsheetCell[row, column];
            for (int i = 0; i < row; ++i)
            {
                for (int j = 0; j < column; ++j)
                {

                    cells[i, j] = new SpreadsheetCell(i, j);
                    //subscribe so that the spreadsheet class knows when a cell value is changed
                    cells[i, j].PropertyChanged += Cells_PropertyChanged;
                }
            }
        }
        public void Importfromxml(string documentname)
        {
            Clearsheet();
            XmlDocument xmldoc= new XmlDocument();
            xmldoc.Load(documentname);
            XmlNodeList celllist = xmldoc.SelectNodes("/spreadsheet/cell");
            foreach (XmlNode cell in celllist)
            {
                int row = int.Parse(cell.SelectSingleNode("row").InnerText);
                int col = int.Parse(cell.SelectSingleNode("col").InnerText);
                string value = cell.SelectSingleNode("value").InnerText;
                cells[row, col].Text = value;
            }
        }
    }


}
