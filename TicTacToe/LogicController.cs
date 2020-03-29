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

        private const char crosses = 'X';
        private const char noughts = 'O';

        private readonly string CrossMoves = $"{crosses} moves...";
        private readonly string NoughtsMoves = $"{noughts} moves...";
        private readonly string IllegalMove = "Cant move here";
        private readonly string CrossWins = $"{crosses} has won the game!";
        private readonly string NoughtsWins = $"{noughts} has won the game!";
        private readonly string Draw = "Draw, no more moves left";

        private string crossHeader;
        private string noughtHeader;

        private int crossesScore;
        private int noughtsScore;

        public LogicController()
        {
            this.mainWindow = (MainWindow)Application.Current.MainWindow;
            this.gameBoard = new Hashtable();
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
                this.NotifyPropertyChanged("CrossesScore");
            }
        }
        public int NoughtsScore
        {
            get { return this.noughtsScore; }
            set
            {
                this.noughtsScore = value;
                this.NotifyPropertyChanged("NoughtsScore");
            }
        }

        public string CrossHeader
        {
            get { return this.crossHeader; }
            set
            {
                this.crossHeader = value;
                this.NotifyPropertyChanged("CrossHeader");
            }

        }

        public string NoughtHeader
        {
            get { return this.noughtHeader; }
            set
            {
                this.noughtHeader = value;
                this.NotifyPropertyChanged("NoughtHeader");
            }

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

        private void SetCrossToMove()
        {
            this.gameState = GameState.CrossesMove;
            this.CrossHeader = this.CrossMoves;
            this.NoughtHeader = null;
        }

        private void SetNoughtToMove()
        {
            this.gameState = GameState.NoughtsMove;
            this.NoughtHeader = this.NoughtsMoves;
            this.CrossHeader = null;
        }

        private void GameStart()
        {
            foreach (Button button in ControlsHelper.FindVisualChildren<Button>(this.mainWindow))
            {
                button.Content = null;
            }
            this.gameBoard.Clear();
            this.SetCrossToMove();
        }

        private void SelectMove(Button button)
        {
            if (button.Content == null)
            {
                if (this.gameState == GameState.CrossesMove)
                {
                    button.Foreground = Brushes.LightGreen;
                    button.Content = crosses;
                    this.SetNoughtToMove();
                }
                else if (this.gameState == GameState.NoughtsMove)
                {
                    button.Foreground = Brushes.LightSalmon;
                    button.Content = noughts;
                    this.SetCrossToMove();
                }
                this.UpdateHashTable();
                this.CheckForGameOver();
                this.CheckForWin();
                this.CheckGameState(button);
            }
            else
            {
                MessageBox.Show(this.IllegalMove);
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
                MessageBox.Show(this.CrossWins);
            }
            else if (button.Content.Equals(noughts))
            {
                this.NoughtsScore++;
                MessageBox.Show(this.NoughtsWins);
            }
        }

        private void GameOver()
        {
            MessageBox.Show(this.Draw);
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
