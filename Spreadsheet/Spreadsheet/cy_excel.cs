using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using CptS321;

namespace Spreadsheet
{
    public partial class cy_excel : Form
    {
        SpreadSheet spreadSheetApp;
        public cy_excel()
        {
            Versionnumber versionnumber = Versionnumber.Instance;
            versionnumber.version = 8.0;
            InitializeComponent();

        }
        public void SpreadsheetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //update the UI when the cell triggers that it has had its value changed
            string[] indicies = e.PropertyName.Split(',');
            int row = int.Parse(indicies[0]);
            int column = int.Parse(indicies[1]);
            dataGridView1[column, row].Value = spreadSheetApp.cells[row, column].Text;
        }

        private void Spreadsheet_load(object sender, EventArgs e)
        {
            //Initialize spreadsheet with alphabet for columns with 50 rows
            Load_Datagrid();
            //Make a new underlying spreadsheet control
            spreadSheetApp = new SpreadSheet(50, 26);
            //Subscribe to event so UI knows about underlying cell changes
            spreadSheetApp.CellPropertyChanged += new PropertyChangedEventHandler(SpreadsheetPropertyChanged);
            

            
            

        }
        private void Load_Datagrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            string[] alphabet = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            foreach (string letter in alphabet)
            {
                //Add columns
                dataGridView1.Columns.Add(letter, letter);
            }
            for (int i = 1; i <= 50; i++)
            {
                //Add rows
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i - 1].HeaderCell.Value = i.ToString();
            }
            dataGridView1.Refresh();
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //Update underlying spreadsheet when a cell in the DataGrid is getting validated
            DataGridView mainGrid = sender as DataGridView;
            if (e.RowIndex < 0 | e.ColumnIndex < 0) { return; }
            if (mainGrid.CurrentCell.Value is null) { return; }
            string text = mainGrid.CurrentCell.Value.ToString();
            if (text.Length == 0) { return; }
            //Update underlying spreaqdsheet cell with value entered into UI. Underlying spreadsheet will do 
            //logic to determine what to do with the entered text.
            
            spreadSheetApp.GetCell(e.RowIndex, e.ColumnIndex).Text = mainGrid[e.ColumnIndex,e.RowIndex].Value.ToString();
        }

        public void UpdateCell(int row, int column)
        {
            
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            

        }

        private void DataGridView1_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView maingrid = sender as DataGridView;
            int row = maingrid.CurrentCell.RowIndex;
            int col = maingrid.CurrentCell.ColumnIndex;
            char coL = (char)(col + 'a');
            string roW = (row + 1).ToString();
            coL = char.ToUpper(coL);
            textBoxCellName.Text = coL.ToString() + roW;
            variableEditor.Text = spreadSheetApp.cells[row,col].Value;
        }

        private void variableEditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                dataGridView1.CurrentCell.Value = variableEditor.Text;
                variableEditor.Text = "";
            }

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream savestream;
            string filename = "";
            using (SaveFileDialog saveFileDialog1 = new SaveFileDialog()) //Open an instance of savefiledialog
            {
                saveFileDialog1.Filter = "xml (*.xml)|*.xml"; //Filter .xml files
                saveFileDialog1.FilterIndex = 1; //Default to xml file
                saveFileDialog1.RestoreDirectory = true;
                
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) //If the openfiledialog was able to run and recieve input
                {
                    if ((savestream = saveFileDialog1.OpenFile()) != null) //set savestream to the file selected if it is not null
                    {
                        filename = saveFileDialog1.FileName;
                        savestream.Close();
                    }
                    spreadSheetApp.Exporttoxml(filename);

                }
            }
            
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open File helper function
            
            Stream openstream;
            string filename = "";
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog()) //Open an instance of openfiledialog
            {
                openFileDialog1.Filter = "xml (*.xml)|*.xml"; //Filter only .doc and .txt files
                openFileDialog1.FilterIndex = 1; //Default to txt file
                if (openFileDialog1.ShowDialog() == DialogResult.OK)    //If the openfiledialog was able to run and recieve input
                {
                    if ((openstream = openFileDialog1.OpenFile()) != null)  //set openstream to the file selected if it is not null
                    {
                        using (TextReader sr = new StreamReader(openstream)) //open an instance of a textreader using the file selected
                        {
                            
                            filename = openFileDialog1.FileName;
                            openstream.Close();   //call load text with passed in textreader
                        }
                        Load_Datagrid();
                        spreadSheetApp.Importfromxml(filename);

                    }

                }
            }
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 a = new AboutBox1();
            a.Show();
        }
    }

}
