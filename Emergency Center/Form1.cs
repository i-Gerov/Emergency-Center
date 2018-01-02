using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace Emergency_Center
{
    public partial class Form1 : Form
    {
        private BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        private BackgroundWorker backgroundWorker2 = new BackgroundWorker();
        SpeechSynthesizer reader = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();

            // change form color
            this.BackColor = Color.LightGray;

            // thread-safe calls
            CheckForIllegalCrossThreadCalls = false;

            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);

            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork += new DoWorkEventHandler(backgroundWorker2_DoWork);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Designed by Mitko & Alex.", "About");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Panel[,] mapPanels;
        private string[,] mapLocations;
        private Button buttonOne = new Button();
        private Button buttonTwo = new Button();
        private Button buttonThree = new Button();
        private Color clr1 = Color.DarkGray;
        private Color clr2 = Color.Transparent;
        private NotifyIcon trayIcon = new NotifyIcon();
        Label label1 = new Label();
        Stopwatch stopWatch = new Stopwatch();

        private void Form1_Load(object sender, EventArgs e)
        {
            // open form in Full Screen
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            // add emergency center picture
            PictureBox emergencyBox = new PictureBox();
            emergencyBox.Location = new System.Drawing.Point(420, 40);
            emergencyBox.Name = "emergencyBox";
            emergencyBox.Size = new System.Drawing.Size(428, 80);
            emergencyBox.BackColor = clr2;
            Controls.Add(emergencyBox);
            emergencyBox.Load("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\emergency.png");

            // add color box
            PictureBox colorBox = new PictureBox();
            colorBox.Location = new System.Drawing.Point(160, 240);
            colorBox.Name = "colorBox";
            colorBox.Size = new System.Drawing.Size(1045, 495);
            colorBox.BackColor = clr1;
            colorBox.BringToFront();
            Controls.Add(colorBox);

            // add police department picture
            PictureBox policeBox = new PictureBox();
            policeBox.Location = new System.Drawing.Point(158, 175);
            policeBox.Name = "policeBox";
            policeBox.Size = new System.Drawing.Size(88, 65);
            policeBox.BackColor = clr2;
            Controls.Add(policeBox);
            policeBox.Load("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\p-station.png");

            // add fire department picture
            PictureBox fireBox = new PictureBox();
            fireBox.Location = new System.Drawing.Point(1095, 175);
            fireBox.Name = "fireBox";
            fireBox.Size = new System.Drawing.Size(111, 65);
            fireBox.BackColor = clr2;
            Controls.Add(fireBox);
            fireBox.Load("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\f-station.png");

            // map parameters
            const int tileSize = 55;
            const int gridRows = 19;
            const int gridCols = 9;
            const int alignTop = 0;
            const int alignRight = 0;

            Random rnd = new Random();

            // initialize the city map
            mapPanels = new Panel[gridRows, gridCols];
            mapLocations = new string[gridRows, gridCols];

            // double for loop to handle all rows and columns
            for (var n = 0; n < gridRows; n++)
            {
                for (var m = 0; m < gridCols; m++)
                {
                    // create new Panel control
                    var newPanel = new Panel
                    {
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(alignRight + tileSize * n, alignTop + tileSize * m),
                    };

                    // add to Form's Controls so that they show up
                    Controls.Add(newPanel);

                    // bring panel to front
                    newPanel.BringToFront();
                    newPanel.Parent = colorBox;
                    // add to our 2d array of panels for future use
                    mapPanels[n, m] = newPanel;

                    // color the backgrounds
                    if (n == 0 || n == gridRows - 1 || m == 0 || m == gridCols - 1)
                    {
                        newPanel.BackColor = clr1;
                        newPanel.BackgroundImageLayout = ImageLayout.Stretch;

                        if (m == 0 && n == 0)
                        {
                            newPanel.BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\top-left.png");
                            mapLocations[n, m] = "C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\top-left.png";
                        }
                        else if (m == 0 && n == gridRows - 1)
                        {
                            newPanel.BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\top-right.png");
                            mapLocations[n, m] = "C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\top-right.png";
                        }
                        else if (m == gridCols - 1 && n == 0)
                        {
                            newPanel.BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\down-left.png");
                            mapLocations[n, m] = "C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\down-left.png";
                        }
                        else if (m == gridCols - 1 && n == gridRows - 1)
                        {
                            newPanel.BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\down-right.png");
                            mapLocations[n, m] = "C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\down-right.png";
                        }
                        else if (m == 0 || m == gridCols - 1)
                        {
                            newPanel.BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\white line2.png");
                            mapLocations[n, m] = "C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\white line2.png";
                        }
                        else
                        {
                            newPanel.BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\white line.png");
                            mapLocations[n, m] = "C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\white line.png";
                        }
                    }
                    else
                    {
                        if (n % 2 == 1)
                        {
                            if (m % 2 == 1)
                            {
                                newPanel.BackColor = clr2;
                                newPanel.BackgroundImageLayout = ImageLayout.Stretch;
                                newPanel.BackgroundImage = Image.FromFile($"C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\house{rnd.Next(1, 17)}.png");
                            }
                            else
                            {
                                newPanel.BackColor = clr1;
                                newPanel.BackgroundImageLayout = ImageLayout.Stretch;
                                newPanel.BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\white line2.png");
                                mapLocations[n, m] = "C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\white line2.png";
                            }
                        }
                        else
                        {
                            newPanel.BackColor = clr1;
                            newPanel.BackgroundImageLayout = ImageLayout.Stretch;
                            newPanel.BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\white line.png");
                            mapLocations[n, m] = "C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\white line.png";
                        }
                    }
                }
            }

            // create random amount of objects

            // create start police patrolling button
            this.Controls.Add(buttonOne);
            buttonOne.Size = new Size(200, 80);
            buttonOne.Location = new Point(100, 60);
            buttonOne.TabStop = false;
            buttonOne.Text = "Start";
            buttonOne.Font = new Font("Arial", 24, FontStyle.Bold);
            buttonOne.Click += new EventHandler(this.btn_Click);

            // create start fire patrolling button
            this.Controls.Add(buttonTwo);
            buttonTwo.Size = new Size(200, 80);
            buttonTwo.Location = new Point(1050, 60);
            buttonTwo.TabStop = false;
            buttonTwo.Text = "Start";
            buttonTwo.Font = new Font("Arial", 24, FontStyle.Bold);
            buttonTwo.Click += new EventHandler(this.btn2_Click);

            // hide buttons
            buttonOne.Hide();
            buttonTwo.Hide();
            emergencyBox.Hide();

            // create action button
            this.Controls.Add(buttonThree);
            buttonThree.Size = new Size(140, 60);
            buttonThree.Location = new Point(558, 150);
            buttonThree.TabStop = false;
            buttonThree.Text = "Action";
            buttonThree.Font = new Font("Arial", 24, FontStyle.Bold);
            buttonThree.Click += new EventHandler(this.btn3_Click);

            // create timer label
            this.Controls.Add(label1);
            label1.Text = "00:00:00";
            label1.Visible = true;
            label1.Font = new Font("MS UI Gothic", 42, FontStyle.Regular);
            label1.Location = new Point(740, 152);
            label1.Size = new Size(220, 60);
        }

        private int cor1 = -1;
        private int cor2 = -1;
        private int cor3 = -1;
        private int cor4 = -1;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // Random patrooling action
            int row = 0;
            int col = 0;
            int prevRow = -1;
            int prevCol = -1;
            Random rnd = new Random();

            while (true)
            {
                if (prevRow >= 0 && prevCol >= 0)
                {
                    mapPanels[prevRow, prevCol].BackgroundImage = Image.FromFile(mapLocations[prevRow, prevCol]);
                }

                cor1 = prevRow;
                cor2 = prevCol;

                mapPanels[row, col].BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\police.png");

                prevRow = row;
                prevCol = col;

                if (backgroundWorker1.CancellationPending)
                {
                    cor1 = -1;
                    cor2 = -1;

                    mapPanels[row, col].BackgroundImage = Image.FromFile(mapLocations[row, col]);
                    e.Cancel = true;
                    return;
                }

                // check for crash
                if ((row == cor3 && col == cor4))
                {
                    cor1 = -1;
                    cor2 = -1;

                    mapPanels[row, col].BackgroundImage = Image.FromFile(mapLocations[row, col]);

                    if (buttonOne.Text == "Stop")
                    {
                        buttonOne.PerformClick();
                    }

                    if (buttonTwo.Text == "Stop")
                    {
                        buttonTwo.PerformClick();
                    }

                    reader.Speak("Vehicles have crashed");
                    e.Cancel = true;
                    MessageBox.Show("CRASH!");
                    return;
                }

                // generate random direction
                int dir = rnd.Next(0, 4);

                if (dir == 0)
                {
                    if (col + 1 < mapPanels.GetLength(1))
                    {
                        if (mapPanels[row, col + 1].BackColor == clr1)
                        {
                            col++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dir == 1)
                {
                    if (row + 1 < mapPanels.GetLength(0))
                    {
                        if (mapPanels[row + 1, col].BackColor == clr1)
                        {
                            row++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dir == 2)
                {
                    if (col - 1 >= 0)
                    {
                        if (mapPanels[row, col - 1].BackColor == clr1)
                        {
                            col--;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dir == 3)
                {
                    if (row - 1 >= 0)
                    {
                        if (mapPanels[row - 1, col].BackColor == clr1)
                        {
                            row--;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                //Thread.Sleep(300);
                Thread.Sleep(500);
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            // Random patrooling action
            int row = mapPanels.GetLength(0) - 1;
            int col = 0;
            int prevRow = -1;
            int prevCol = -1;
            Random rnd = new Random();

            while (true)
            {
                if (prevRow >= 0 && prevCol >= 0)
                {
                    mapPanels[prevRow, prevCol].BackgroundImage = Image.FromFile(mapLocations[prevRow, prevCol]);
                }

                cor3 = prevRow;
                cor4 = prevCol;

                mapPanels[row, col].BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\fire-engine.png");

                prevRow = row;
                prevCol = col;

                if (backgroundWorker2.CancellationPending)
                {
                    cor3 = -1;
                    cor4 = -1;

                    mapPanels[row, col].BackgroundImage = Image.FromFile(mapLocations[row, col]);
                    e.Cancel = true;
                    return;
                }

                // check for crash
                if ((row == cor1 && col == cor2))
                {
                    cor3 = -1;
                    cor4 = -1;

                    mapPanels[row, col].BackgroundImage = Image.FromFile(mapLocations[row, col]);

                    if (buttonOne.Text == "Stop")
                    {
                        buttonOne.PerformClick();
                    }

                    if (buttonTwo.Text == "Stop")
                    {
                        buttonTwo.PerformClick();
                    }

                    reader.Speak("Vehicles have crashed");
                    e.Cancel = true;
                    MessageBox.Show("CRASH!");
                }

                // generate random direction
                int dir = rnd.Next(0, 4);

                if (dir == 0)
                {
                    if (col + 1 < mapPanels.GetLength(1))
                    {
                        if (mapPanels[row, col + 1].BackColor == clr1)
                        {
                            col++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dir == 1)
                {
                    if (row + 1 < mapPanels.GetLength(0))
                    {
                        if (mapPanels[row + 1, col].BackColor == clr1)
                        {
                            row++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dir == 2)
                {
                    if (col - 1 >= 0)
                    {
                        if (mapPanels[row, col - 1].BackColor == clr1)
                        {
                            col--;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dir == 3)
                {
                    if (row - 1 >= 0)
                    {
                        if (mapPanels[row - 1, col].BackColor == clr1)
                        {
                            row--;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                //Thread.Sleep(300);
                Thread.Sleep(500);
            }
        }

        private static bool check = false;

        private void btn_Click(object sender, EventArgs e)
        {
            Button buttonOne = (Button)sender;

            if (check)
            {
                reader.Speak("Patrol returned");
                buttonOne.Text = "Start";
            }
            else
            {
                reader.Speak("Patrol sent");
                buttonOne.Text = "Stop";
            }

            if (buttonOne.Text == "Stop")
            {
                if (!backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            else
            {
                if (backgroundWorker1.IsBusy)
                {
                    backgroundWorker1.CancelAsync();
                }
            }

            check = !check;
        }

        private static bool check2 = false;

        private void btn2_Click(object sender, EventArgs e)
        {
            Button buttonTwo = (Button)sender;

            if (check2)
            {
                reader.Speak("Fire engine returned");
                buttonTwo.Text = "Start";
            }
            else
            {
                reader.Speak("Fire engine sent");
                buttonTwo.Text = "Stop";
            }

            if (buttonTwo.Text == "Stop")
            {
                if (!backgroundWorker2.IsBusy)
                {
                    backgroundWorker2.RunWorkerAsync();
                }
            }
            else
            {
                if (backgroundWorker2.IsBusy)
                {
                    backgroundWorker2.CancelAsync();
                }
            }

            check2 = !check2;
        }

        private List<string> cords = new List<string>();
        private Random rnd = new Random();

        private void btn3_Click(object sender, EventArgs e)
        {
            buttonThree.Enabled = false;
            stopWatch.Reset();
            label1.Text = "00:00:00";

            int row;
            int col;
            int obj;

            while (true)
            {
                row = rnd.Next(0, mapPanels.GetLength(0));
                col = rnd.Next(0, mapPanels.GetLength(1));

                if (mapPanels[row, col].BackColor == clr1 && !cords.Contains($"{row}-{col}"))
                {
                    cords.Add($"{row}-{col}");
                    break;
                }
            }

            obj = rnd.Next(0, 2);

            if (obj == 0)
            {
                mapPanels[row, col].BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\fire2.png");
                SendFireEngine(row, col);
            }
            else
            {
                mapPanels[row, col].BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\robber.png");
                SendPolice(row, col);
            }

            cords = new List<string>();
        }

        private void SendPolice(int tRow, int tCol)
        {
            trayIcon.Icon = System.Drawing.SystemIcons.Information;
            trayIcon.BalloonTipText = "New robbery signal received";
            trayIcon.Visible = true;
            trayIcon.BalloonTipTitle = "ROBBERY!";
            trayIcon.ShowBalloonTip(0);

            reader.Speak($"Robber on coordinates {rnd.Next(1, 10)} oh {rnd.Next(1, 10)}");
            Thread.Sleep(10);
            reader.Speak($"Police car sent");

            stopWatch.Start();

            int row = 0;
            int col = 0;

            int prevRow = -1;
            int prevCol = -1;
            int dir;

            while (true)
            {
                TimeSpan ts = stopWatch.Elapsed;
                label1.Text = String.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                label1.Update();

                if (prevRow >= 0 && prevCol >= 0)
                {
                    mapPanels[prevRow, prevCol].BackgroundImage = Image.FromFile(mapLocations[prevRow, prevCol]);
                }

                mapPanels[row, col].BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\police.png");

                Update();

                Thread.Sleep(100);

                prevRow = row;
                prevCol = col;

                if (tCol % 2 == 0)
                {
                    if (col < tCol)
                    {
                        dir = 0;
                    }
                    else
                    {
                        dir = 1;
                    }
                }
                else
                {
                    if (row < tRow)
                    {
                        dir = 1;
                    }
                    else
                    {
                        dir = 0;
                    }
                }

                if (col == tCol && row == tRow)
                {
                    reader.Speak($"Robber caught");
                    stopWatch.Stop();
                    mapPanels[row, col].BackgroundImage = Image.FromFile(mapLocations[row, col]);
                    reader.Speak($"Action completed in {ts.Seconds} seconds");
                    buttonThree.Enabled = true;
                    break;
                }

                if (dir == 0) // down
                {
                    if (col + 1 < mapPanels.GetLength(1))
                    {
                        if (mapPanels[row, col + 1].BackColor == clr1)
                        {
                            col++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dir == 1) // right
                {
                    if (row + 1 < mapPanels.GetLength(0))
                    {
                        if (mapPanels[row + 1, col].BackColor == clr1)
                        {
                            row++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        private void SendFireEngine(int tRow, int tCol)
        {
            trayIcon.Icon = System.Drawing.SystemIcons.Information;
            trayIcon.BalloonTipText = "New arson signal received";
            trayIcon.Visible = true;
            trayIcon.BalloonTipTitle = "FIRE!";
            trayIcon.ShowBalloonTip(0);

            reader.Speak($"Fire on coordinates {rnd.Next(1, 10)} oh {rnd.Next(1, 10)}");
            Thread.Sleep(10);
            reader.Speak($"Fire engine sent");

            stopWatch.Start();

            int row = mapPanels.GetLength(0) - 1;
            int col = 0;

            int prevRow = -1;
            int prevCol = -1;
            int dir;

            while (true)
            {
                TimeSpan ts = stopWatch.Elapsed;
                label1.Text = String.Format("{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                label1.Update();

                if (prevRow >= 0 && prevCol >= 0)
                {
                    mapPanels[prevRow, prevCol].BackgroundImage = Image.FromFile(mapLocations[prevRow, prevCol]);
                }

                mapPanels[row, col].BackgroundImage = Image.FromFile("C:\\Users\\i.Gerov\\source\\repos\\Emergency Center\\Emergency Center\\Images\\fire-engine.png");

                Update();

                Thread.Sleep(100);

                prevRow = row;
                prevCol = col;

                if (tCol % 2 == 0)
                {
                    if (col < tCol)
                    {
                        dir = 0;
                    }
                    else
                    {
                        dir = 1;
                    }
                }
                else
                {
                    if (row > tRow)
                    {
                        dir = 1;
                    }
                    else
                    {
                        dir = 0;
                    }
                }

                if (col == tCol && row == tRow)
                {
                    reader.Speak($"Fire extinguished");
                    stopWatch.Stop();
                    mapPanels[row, col].BackgroundImage = Image.FromFile(mapLocations[row, col]);
                    reader.Speak($"Action completed in {ts.Seconds} seconds");
                    buttonThree.Enabled = true;
                    break;
                }

                if (dir == 0) // down
                {
                    if (col + 1 < mapPanels.GetLength(1))
                    {
                        if (mapPanels[row, col + 1].BackColor == clr1)
                        {
                            col++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (dir == 1) // left
                {
                    if (row - 1 >= 0)
                    {
                        if (mapPanels[row - 1, col].BackColor == clr1)
                        {
                            row--;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }
}
