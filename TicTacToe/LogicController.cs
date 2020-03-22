using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TicTacToe
{
    public enum StateMachine
    {
        CrossesMove,
        NoughtsMove,
        GameWon,
        GameOver
    }
    public class LogicController : INotifyPropertyChanged
    {
        private Window mainWindow;
        private Hashtable gameGrid;
        public event PropertyChangedEventHandler PropertyChanged;

        private StateMachine gameState;

        private ICommand command;

        private const string crosses = "X";
        private const string noughts = "O";

        private int crossesScore;
        private int noughtsScore;

        public LogicController()
        {
            this.mainWindow = (MainWindow)Application.Current.MainWindow;
            this.gameGrid = new Hashtable();
            this.gameState = StateMachine.CrossesMove;
            this.CrossesScore = 0;
            this.NoughtsScore = 0;       
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
        public string ScoreBoard
        {
            get { return $"Score [{crosses}: {this.crossesScore} | {noughts}: {this.noughtsScore}]"; }
        }

        public ICommand Command
        {
            get
            {
                return this.command ?? (this.command = new RelayCommand<Button>( button => { this.ProcessMove(button);}));
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
                case StateMachine.CrossesMove:
                    this.SelectMove(button);
                    break;
                case StateMachine.NoughtsMove:
                    this.SelectMove(button);
                    break;
                case StateMachine.GameWon:
                    this.GameWon(button);
                    break;
                case StateMachine.GameOver:
                    this.GameOver();
                    break;
                default:
                    break;
            }
        }

        private void GameStart()
        {
            foreach (Button tb in FindVisualChildren<Button>(this.mainWindow))
            {
                tb.Content = null;
            }
            this.gameGrid.Clear();
            this.gameState = StateMachine.CrossesMove;
        }

        private void SelectMove(Button button)
        {
            if (button.Content == null)
            {
                if (this.gameState == StateMachine.CrossesMove)
                {
                    button.Content = crosses;
                    this.gameState = StateMachine.NoughtsMove;
                }
                else if (this.gameState == StateMachine.NoughtsMove)
                {
                    button.Content = noughts;
                    this.gameState = StateMachine.CrossesMove;
                }
                this.UpdateHashTable();
                this.CheckForGameOver();
                this.CheckForWin();
                this.ValidateMove(button);
            }
            else
            {
                MessageBox.Show("Cant move here!");
            }
        }

        private void ValidateMove(Button button)
        {
            if (this.gameState == StateMachine.GameWon)
            {
                this.GameWon(button);
                this.GameStart();
            }
            else if (this.gameState == StateMachine.GameOver)
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
            // Debug Scoreboard
            Debug.Print($"{crosses}: {this.CrossesScore} | {noughts}: {this.NoughtsScore}");
        }

        private void GameOver()
        {
            MessageBox.Show("Game Over, no more moves");
        }

        private void UpdateHashTable()
        {
            foreach (Button btn in FindVisualChildren<Button>(this.mainWindow))
            {
                if (btn.Content != null && !this.gameGrid.ContainsKey(btn.Name))
                {
                    this.gameGrid.Add(btn.Name, btn.Content);
                }
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

        private void CheckForGameOver()
        {
            if (this.gameGrid.Count == 9)
            {
                this.gameState = StateMachine.GameOver;
            }
        }

        private void CheckForWin()
        {
            if (this.CheckHashTableForMatch(this.gameGrid, "A1", "A2", "A3"))
            {
                this.gameState = StateMachine.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameGrid, "B1", "B2", "B3"))
            {
                this.gameState = StateMachine.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameGrid, "C1", "C2", "C3"))
            {
                this.gameState = StateMachine.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameGrid, "A1", "B1", "C1"))
            {
                this.gameState = StateMachine.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameGrid, "A2", "B2", "C2"))
            {
                this.gameState = StateMachine.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameGrid, "A3", "B3", "C3"))
            {
                this.gameState = StateMachine.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameGrid, "A1", "B2", "C3"))
            {
                this.gameState = StateMachine.GameWon;
            }
            else if (this.CheckHashTableForMatch(this.gameGrid, "A3", "B2", "C1"))
            {
                this.gameState = StateMachine.GameWon;
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
