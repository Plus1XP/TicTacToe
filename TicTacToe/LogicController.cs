using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TicTacToe
{
    public class LogicController : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Window mainWindow;
        private Hashtable gameBoard;

        private GameState gameState;
        private ICommand command;

        private string crossBoard;
        private string noughtBoard;

        private const char crosses = 'X';
        private const char noughts = 'O';

        private int crossesScore;
        private int noughtsScore;

        public LogicController()
        {
            this.mainWindow = (MainWindow)Application.Current.MainWindow;
            this.gameBoard = new Hashtable();
            //this.gameState = GameState.CrossesMove;
            this.CrossesScore = 0;
            this.NoughtsScore = 0;
            this.GameStart();
        }

        public int CrossesScore
        {
            get { return this.crossesScore; }
            set
            {
                this.crossesScore = value;
                this.NotifyPropertyChanged("ScoreBoard");
            }
        }
        public int NoughtsScore
        {
            get { return this.noughtsScore; }
            set
            {
                this.noughtsScore = value;
                this.NotifyPropertyChanged("ScoreBoard");
            }
        }

        public string CrossBoard
        {
            get { return this.crossBoard; }
            set
            {
                this.crossBoard = value;
                this.NotifyPropertyChanged("CrossBoard");
            }

        }

        public string NoughtBoard
        {
            get { return this.noughtBoard; }
            set
            {
                this.noughtBoard = value;
                this.NotifyPropertyChanged("NoughtBoard");
            }

        }

        public string ScoreBoard
        {
            get { return $"Score [{crosses}: {this.crossesScore} | {noughts}: {this.noughtsScore}]"; }
        }

        public ICommand Command
        {
            get
            {
                return this.command ?? (this.command = new RelayCommand<Button>(button => { this.ProcessMove(button); }));
            }
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void ProcessMove(Button button)
        {
            switch (this.gameState)
            {
                case GameState.CrossesMove:
                    this.SelectMove(button);
                    break;
                case GameState.NoughtsMove:
                    this.SelectMove(button);
                    break;
                case GameState.GameWon:
                    this.GameWon(button);
                    break;
                case GameState.GameOver:
                    this.GameOver();
                    break;
                default:
                    break;
            }
        }

        private void GameStart()
        {
            foreach (Button button in ControlsHelper.FindVisualChildren<Button>(this.mainWindow))
            {
                button.Content = null;
            }
            this.gameBoard.Clear();
            this.gameState = GameState.CrossesMove;
            this.CrossBoard = $"{crosses} moves...";
            this.NoughtBoard = null;
        }

        private void SelectMove(Button button)
        {
            if (button.Content == null)
            {
                if (this.gameState == GameState.CrossesMove)
                {
                    button.Foreground = Brushes.LightGreen;
                    button.Content = crosses;
                    this.gameState = GameState.NoughtsMove;
                    this.NoughtBoard = $"{noughts} moves...";
                    this.CrossBoard = null;
                }
                else if (this.gameState == GameState.NoughtsMove)
                {
                    button.Foreground = Brushes.LightSalmon;
                    button.Content = noughts;
                    this.gameState = GameState.CrossesMove;
                    this.CrossBoard = $"{crosses} moves...";
                    this.NoughtBoard = null;
                }
                this.UpdateHashTable();
                this.CheckForGameOver();
                this.CheckForWin();
                this.CheckGameState(button);
            }
            else
            {
                MessageBox.Show("Cant move here!");
            }
        }

        private void CheckGameState(Button button)
        {
            if (this.gameState == GameState.GameWon)
            {
                this.GameWon(button);
                this.GameStart();
            }
            else if (this.gameState == GameState.GameOver)
            {
                this.GameOver();
                this.GameStart();
            }
        }

        private void GameWon(Button button)
        {
            if (button.Content.Equals(crosses))
            {
                this.CrossesScore++;
                MessageBox.Show($"{crosses} has won the game!");
            }
            else if (button.Content.Equals(noughts))
            {
                this.NoughtsScore++;
                MessageBox.Show($"{noughts} has won the game!");
            }
        }

        private void GameOver()
        {
            MessageBox.Show("Game Over, no more moves");
        }

        private void UpdateHashTable()
        {
            foreach (Button button in ControlsHelper.FindVisualChildren<Button>(this.mainWindow))
            {
                if (button.Content != null && !this.gameBoard.ContainsKey(button.Name))
                {
                    this.gameBoard.Add(button.Name, button.Content);
                }
            }
        }

        private void CheckForGameOver()
        {
            if (this.gameBoard.Count == 9)
            {
                this.gameState = GameState.GameOver;
            }
        }

        private void CheckForWin()
        {
            if (this.CheckHashTableForMatch(this.gameBoard, "A1", "A2", "A3"))
            {
                this.gameState = GameState.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameBoard, "B1", "B2", "B3"))
            {
                this.gameState = GameState.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameBoard, "C1", "C2", "C3"))
            {
                this.gameState = GameState.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameBoard, "A1", "B1", "C1"))
            {
                this.gameState = GameState.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameBoard, "A2", "B2", "C2"))
            {
                this.gameState = GameState.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameBoard, "A3", "B3", "C3"))
            {
                this.gameState = GameState.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameBoard, "A1", "B2", "C3"))
            {
                this.gameState = GameState.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameBoard, "A3", "B2", "C1"))
            {
                this.gameState = GameState.GameWon;
            }
        }

        private bool CheckHashTableForMatch(Hashtable hash, string key1, string key2, string key3)
        {
            if (object.Equals(hash[key1], crosses) && object.Equals(hash[key2], crosses) && object.Equals(hash[key3], crosses))
            {
                return true;
            }
            else if (object.Equals(hash[key1], noughts) && object.Equals(hash[key2], noughts) && object.Equals(hash[key3], noughts))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
