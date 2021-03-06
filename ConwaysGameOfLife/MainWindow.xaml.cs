﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ConwaysGameOfLife
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private int _boardSize;

		private GameCell[,] _gameBoard;

		private readonly DispatcherTimer _timer = new DispatcherTimer();

		public MainWindow()
		{
			InitializeComponent();
			_timer.Tick += TimerTick;
			_timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
		}

		private void TimerTick(object sender, EventArgs e)
		{
			CalculateNextGeneration();

			for (var i = 0; i < _boardSize; i++)
				for (var j = 0; j < _boardSize; j++)
				{
					var cell = _gameBoard[i, j];
					cell.NextGeneration();
					var gridCell = (Rectangle)UniformGrid.Children[cell.Row * _boardSize + cell.Column];
					gridCell.Fill = new SolidColorBrush(cell.State);
				}
		}

		private void CalculateNextGeneration()
		{
			for (var i = 0; i < _boardSize; i++)
			{
				for (var j = 0; j < _boardSize; j++)
				{
					var gameCell = _gameBoard[i, j];
					var aliveNeighbors = 0;

					var neighbors = new[]
					{
						new Point(i + 1, j),
						new Point(i + 1, j + 1),
						new Point(i, j + 1),
						new Point(i - 1, j + 1),
						new Point(i - 1, j),
						new Point(i - 1, j - 1),
						new Point(i, j - 1),
						new Point(i + 1, j - 1),
					};

					foreach (var neighbor in neighbors)
					{
						if (neighbor.X < 0 || neighbor.X >= _boardSize || neighbor.Y < 0 || neighbor.Y >= _boardSize) continue;
						var neighborCell = _gameBoard[(int)neighbor.X, (int)neighbor.Y];
						if (neighborCell.State == GameCell.Alive) aliveNeighbors++;
					}

					gameCell.DetermineCellSurvival(aliveNeighbors);
				}
			}

		}

		private void InitializeGame(int cellNumber)
		{
			UniformGrid.Children.Clear();
			UniformGrid.Columns = cellNumber;
			UniformGrid.Rows = cellNumber;

			_gameBoard = new GameCell[cellNumber, cellNumber];
			var brush = new SolidColorBrush(GameCell.Dead);

			for (var i = 0; i < cellNumber; i++)
				for (var j = 0; j < cellNumber; j++)
				{
					var rect = new Rectangle()
					{
						Fill = brush,
						Margin = new Thickness(1),
						Tag = new GameCell(i, j, GameCell.Dead)
					};

					_gameBoard[i, j] = (GameCell)rect.Tag;
					rect.MouseDown += RectMouseDown;
					UniformGrid.Children.Add(rect);
				}

		}

		private static void RectMouseDown(object sender, MouseButtonEventArgs e)
		{
			var cell = sender as Rectangle;
			var gameCell = (GameCell)cell.Tag;
			gameCell.ChangeState();
			cell.Fill = new SolidColorBrush(gameCell.State);
		}

		private void GameSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			_boardSize = (int)ValueSlider.Value;
			InitializeGame(_boardSize);
		}

		private void IsGameRunningChecked(object sender, RoutedEventArgs e)
		{
			_timer.Start();
		}

		private void IsGameRunningUnchecked(object sender, RoutedEventArgs e)
		{
			_timer.Stop();
		}
	}
}
