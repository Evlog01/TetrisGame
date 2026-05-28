using System;
using System.Text;
using System.Threading;

namespace TetrisGame
{
    public class Game
    {
        private Board board;
        private Figure currentFigure;
        private Figure nextFigure;
        private int currentX;
        private int currentY;

        private bool isGameOver;
        private bool isPaused;

        private int score;
        private int linesCleared;

        private readonly int fallDelayMs = 500;
        private DateTime lastFallTime;

        private readonly Random random = new Random();

        private const int WindowWidth = 60;
        private const int WindowHeight = 25;

        public Game()
        {
            board = new Board();
        }

        public void Run()
        {
            Console.CursorVisible = false;
            Console.Title = "Тетрис";
            Console.SetWindowSize(WindowWidth, WindowHeight);
            Console.SetBufferSize(WindowWidth, WindowHeight);

            while (true)
            {
                Console.Clear();
                StartNewGame();
                GameLoop();
                if (!AskPlayAgain())
                    break;
            }

            Console.CursorVisible = true;
            Console.Clear();
            Console.WriteLine("Спасибо за игру! Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void StartNewGame()
        {
            board.Clear();
            isGameOver = false;
            isPaused = false;
            score = 0;
            linesCleared = 0;

            currentFigure = Figure.GetRandomFigure();
            nextFigure = Figure.GetRandomFigure();
            currentX = (Board.Width - currentFigure.Width) / 2;
            currentY = 0;

            if (!board.CanPlace(currentFigure, currentX, currentY))
            {
                isGameOver = true;
            }

            lastFallTime = DateTime.Now;
        }

        private void SpawnNewFigure()
        {
            currentFigure = nextFigure;
            nextFigure = Figure.GetRandomFigure();
            currentX = (Board.Width - currentFigure.Width) / 2;
            currentY = 0;

            if (!board.CanPlace(currentFigure, currentX, currentY))
            {
                isGameOver = true;
            }
        }

        private void GameLoop()
        {
            while (!isGameOver)
            {
                Draw();
                HandleInput();
                if (isPaused)
                {
                    Thread.Sleep(50);
                    continue;
                }

                if ((DateTime.Now - lastFallTime).TotalMilliseconds >= fallDelayMs)
                {
                    if (TryMove(0, 1))
                    {
                        lastFallTime = DateTime.Now;
                    }
                    else
                    {
                        LockFigure();
                        int cleared = board.ClearFullRows();
                        UpdateScore(cleared);
                        SpawnNewFigure();
                        if (isGameOver)
                            break;
                        lastFallTime = DateTime.Now;
                    }
                }

                Thread.Sleep(20);
            }

            Draw();
        }

        private void UpdateScore(int cleared)
        {
            linesCleared += cleared;
            switch (cleared)
            {
                case 1: score += 100; break;
                case 2: score += 300; break;
                case 3: score += 700; break;
                case 4: score += 1500; break;
            }
        }

        private void HandleInput()
        {
            if (!Console.KeyAvailable)
                return;

            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    TryMove(-1, 0);
                    break;
                case ConsoleKey.RightArrow:
                    TryMove(1, 0);
                    break;
                case ConsoleKey.DownArrow:
                    if (TryMove(0, 1))
                        lastFallTime = DateTime.Now;
                    break;
                case ConsoleKey.UpArrow:
                    TryRotate();
                    break;
                case ConsoleKey.Spacebar:
                    HardDrop();
                    break;
                case ConsoleKey.Escape:
                    isGameOver = true;
                    break;
                case ConsoleKey.P:
                    isPaused = !isPaused;
                    break;
            }

            while (Console.KeyAvailable)
                Console.ReadKey(true);
        }

        private bool TryMove(int dx, int dy)
        {
            int newX = currentX + dx;
            int newY = currentY + dy;
            if (board.CanPlace(currentFigure, newX, newY))
            {
                currentX = newX;
                currentY = newY;
                return true;
            }
            return false;
        }

        private void TryRotate()
        {
            Figure rotated = currentFigure.Rotate();
            if (board.CanPlace(rotated, currentX, currentY))
            {
                currentFigure = rotated;
            }
            else if (board.CanPlace(rotated, currentX - 1, currentY))
            {
                currentFigure = rotated;
                currentX--;
            }
            else if (board.CanPlace(rotated, currentX + 1, currentY))
            {
                currentFigure = rotated;
                currentX++;
            }
        }

        private void HardDrop()
        {
            while (TryMove(0, 1)) { }
            LockFigure();
            int cleared = board.ClearFullRows();
            UpdateScore(cleared);
            SpawnNewFigure();
            lastFallTime = DateTime.Now;
        }

        private void LockFigure()
        {
            board.PlaceFigure(currentFigure, currentX, currentY);
        }

        private void Draw()
        {
            var sb = new StringBuilder();

            sb.Append("┌" + new string('─', Board.Width * 2) + "┐");

            sb.Append("   СЧЁТ");
            sb.AppendLine();
            string topLine = sb.ToString();
            sb.Clear();
            sb.Append(topLine);

            for (int row = 0; row < Board.Height; row++)
            {
                sb.Append('│');
                for (int col = 0; col < Board.Width; col++)
                {
                    bool isFigureBlock = false;
                    if (!isGameOver && currentFigure != null)
                    {
                        int figureRow = row - currentY;
                        int figureCol = col - currentX;
                        if (figureRow >= 0 && figureRow < currentFigure.Height &&
                            figureCol >= 0 && figureCol < currentFigure.Width &&
                            currentFigure.Shape[figureRow, figureCol] == 1)
                        {
                            isFigureBlock = true;
                        }
                    }

                    sb.Append(isFigureBlock ? "[]" : (board[row, col] == 1 ? "[]" : "  "));
                }
                sb.Append('│');

                switch (row)
                {
                    case 1:
                        sb.Append($"   {score,6}");
                        break;
                    case 3:
                        sb.Append("   ЛИНИИ");
                        break;
                    case 4:
                        sb.Append($"   {linesCleared,6}");
                        break;
                    case 6:
                        sb.Append("   СЛЕДУЮЩАЯ");
                        break;
                    default:
                        if (row >= 7 && row < 7 + 4)
                        {
                            int figRow = row - 7;
                            sb.Append("   ");
                            for (int c = 0; c < 4; c++)
                            {
                                if (figRow < nextFigure.Height && c < nextFigure.Width && nextFigure.Shape[figRow, c] == 1)
                                    sb.Append("[]");
                                else
                                    sb.Append("  ");
                            }
                        }
                        break;
                }

                sb.AppendLine();
            }

            sb.Append("└" + new string('─', Board.Width * 2) + "┘");

            if (isGameOver)
                sb.AppendLine("   GAME OVER");
            else if (isPaused)
                sb.AppendLine("   PAUSE");

            sb.AppendLine();

            sb.AppendLine("← → - движение, ↑ - поворот, ↓ - ускорение, Пробел - сброс");
            sb.AppendLine("P - пауза, Esc - выход");

            Console.SetCursorPosition(0, 0);
            Console.Write(sb.ToString());
        }

        private bool AskPlayAgain()
        {
            Console.SetCursorPosition(0, WindowHeight - 1);
            Console.Write(new string(' ', WindowWidth));
            Console.SetCursorPosition(0, WindowHeight - 1);
            Console.Write("Хотите сыграть ещё раз? (Y/N): ");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                    return true;
                if (key.Key == ConsoleKey.N)
                    return false;
            }
        }
    }
}