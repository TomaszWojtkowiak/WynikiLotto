using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wynikiLotto
{
    public partial class mainWindow : Form
    {
        string urlAddress = "http://app.lotto.pl/wyniki/?type=el";
        List<int[]> listOfNumericalBids = new List<int[]>();

        public mainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] liczby = inputBid.Text.Split(' ');

            int numOfBids = cBoxSystemPlay.Checked ? listSystemPlayNumbers.SelectedIndex + 6 : 5;

            int[] arr = Array.Empty<int>();
            Array.Resize<int>(ref arr, numOfBids);            

            if(liczby.Length == numOfBids)
            {
                for (int i = 0; i < numOfBids; i++)
                {
                    arr[i] = Int16.Parse(liczby[i]);
                }

                listOfNumericalBids.Add(arr);

                string bid = (bidList.Items.Count + 1).ToString().PadLeft(2, '0') + ": | ";

                for (int i = 0; i < liczby.Length; i++)
                {
                    bid += liczby[i].PadLeft(2, '0') + " | ";
                }

                bidList.Items.Add(bid);
                bidListCheckVisibility();

                inputBid.Text = "";
            }
            else
            {
                MessageBox.Show("Błąd: Niepoprawna ilość liczb", "BŁĄD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bidListCheckVisibility()
        {
            if (bidList.Items.Count == 0)
                bidList.Visible = false;
            else
                bidList.Visible = true;
        }

        private void bidListUpdateEnumaration()
        {
           for(int i = 1; i <= bidList.Items.Count; i++)
            {
                string _bid = bidList.Items[i-1].ToString();
                if(!_bid[0].Equals(i.ToString()))
                {
                    string _newBid = (i).ToString() + _bid.Substring(1);
                    bidList.Items.Insert(i, _newBid);
                }
            }
        }

        private void btnResults_Click(object sender, EventArgs e)
        {
            string lotto = "";
            string lottoPlus = "";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream,
                        Encoding.GetEncoding(response.CharacterSet));
                lotto = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
            }

            parseHTML(lotto, lottoPlus);
        }

        public void parseHTML(string _dataLotto, string _dataLottoPlus)
        {
            string[] resultsLotto = _dataLotto.Split('\n');
            //string[] resultLottoPlus = _dataLottoPlus.Split('\n');
            string[] lotto = { "", "", "", "", "", "" };
            //string[] lottoPlus = { "", "", "", "", "", "" };

            resultsDate.Text = $"Wyniki z dnia: {resultsLotto[0]}";

            Array.Copy(resultsLotto, 1, lotto, 0, 5);
            //Array.Copy(resultLottoPlus, 1, lottoPlus, 0, 6);

            Label[] lottoNumbers = { lotto1, lotto2, lotto3, lotto4, lotto5, lotto6 };
            //Label[] lottoPlusNumbers = { lottoPlus1, lottoPlus2, lottoPlus3, lottoPlus4, lottoPlus5, lottoPlus6 };

            int[] numbers = Array.Empty<int>();
            Array.Resize<int>(ref numbers, 5);
            //int[] numbersPlus = Array.Empty<int>();
            //Array.Resize<int>(ref numbersPlus, 6);

            for (int i = 0; i < 5; i++)
            {
                numbers[i] = Int16.Parse(lotto[i]);
                //numbersPlus[i] = Int16.Parse(lottoPlus[i]);
            }

            Array.Sort(numbers);
            //Array.Sort(numbersPlus);

            for (int i = 0; i < 5; i++)
            {
                lottoNumbers[i].Text = numbers[i].ToString();
                //lottoPlusNumbers[i].Text = numbersPlus[i].ToString();
            }

            if (numbers.Length > 0)
                btnCheck.Enabled = true;
        }

        private void mainWindow_Load(object sender, EventArgs e)
        {
            
        }

        private void btnDeleteBid_Click(object sender, EventArgs e)
        {
            bidList.Items.RemoveAt(bidList.SelectedIndex);
            bidListCheckVisibility();
            //bidListUpdateEnumaration();
        }

        private void checkBidSelection(object sender, EventArgs e)
        {

        }

        private void checkBidsAgainstResults(object sender, EventArgs e)
        {
            int[] results = { Int16.Parse(lotto1.Text), Int16.Parse(lotto2.Text), Int16.Parse(lotto3.Text), 
                Int16.Parse(lotto4.Text), Int16.Parse(lotto5.Text) };

            listBox1.Items.Clear();

            foreach(int[] arr in listOfNumericalBids)
            {
                int hitsInBid = 0;
                int bidIndex = listOfNumericalBids.IndexOf(arr);

                for(int i = 0; i < arr.Length; i++)
                {
                    for(int j = 0; j < 5; j++)
                    {
                        if (arr[i] == results[j])
                            hitsInBid++;
                    }
                }
                listBox1.Items.Add(bidList.Items[bidIndex].ToString() + '\t' + "Trafienia: " + hitsInBid);

            }
        }

        private void pickLottery(object sender, EventArgs e)
        {
            Button _btn = sender as Button;
            if(_btn.Tag.ToString() == "MiniLotto")
            {
                urlAddress = "https://app.lotto.pl/wyniki/?type=el";
                lottoName.Text = "Mini Lotto: ";
                Label[] lottoPlusLabels = { lottoPlus1, lottoPlus2, lottoPlus3, lottoPlus4, lottoPlus5, lottoPlus6};
                foreach(Label lotto in lottoPlusLabels)
                {
                    lotto.Visible = false;
                }
                lottoPlusName.Visible = false;
            }
            else
            {
                lottoName.Text = "Lotto: ";
                Label[] lottoPlusLabels = { lottoPlus1, lottoPlus2, lottoPlus3, lottoPlus4, lottoPlus5, lottoPlus6 };
                foreach (Label lotto in lottoPlusLabels)
                {
                    lotto.Visible = true;
                }
                lottoPlusName.Visible = true;
            }
        }

        private void fnSaveFile(object sender, EventArgs e)
        {
            saveDialog.DefaultExt = "txt";
            saveDialog.FileName = DateTime.Today.ToShortDateString() + "_bids";
            saveDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DialogResult result = saveDialog.ShowDialog();

            if(result == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveDialog.OpenFile());
                int numbersInBid = cBoxSystemPlay.Checked ? listSystemPlayNumbers.SelectedIndex + 6 : 5;
                sw.WriteLine(numbersInBid.ToString());
                foreach(int[] bid in listOfNumericalBids)
                {
                    string lineToWrite = "";

                    foreach(int num in bid)
                    {
                        lineToWrite += num.ToString() + " ";
                    }
                    sw.WriteLine(lineToWrite);
                }
                sw.Close();
            }
        }

        private void fnLoadFile(object sender, EventArgs e)
        {
            openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            DialogResult result = openDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                listOfNumericalBids.Clear();
                bidList.Items.Clear();
                listBox1.Items.Clear();

                StreamReader sr = new StreamReader(openDialog.OpenFile());

                int numbersInBid = Int16.Parse(sr.ReadLine());

                if(numbersInBid > 5)
                {
                    cBoxSystemPlay.Checked = true;
                    listSystemPlayNumbers.SelectedItem = listSystemPlayNumbers.Items[numbersInBid - 6];
                }

                while(!sr.EndOfStream)
                {
                    string bid = sr.ReadLine();
                    string[] bidArr = bid.Split(' ');

                    int[] numBid = Array.Empty<int>();
                    Array.Resize<int>(ref numBid, numbersInBid);

                    string bidToList = bidList.Items.Count.ToString().PadLeft(2, '0') + ": ";

                    for(int i=0; i<numbersInBid; i++)
                    {
                        numBid[i] = Int16.Parse(bidArr[i]);
                        bidToList += " | " +  bidArr[i].PadLeft(2, '0');
                    }

                    listOfNumericalBids.Add(numBid);
                    bidToList += " |";
                    bidList.Items.Add(bidToList);
                }

                sr.Close();
                bidListCheckVisibility();
            }
        }
    }
}

/*
 * Mini Lotto
 * ?type=el 
 * Lotto
 * ?type=dl
 * Lotto Plus
 * ?type=lp
*/