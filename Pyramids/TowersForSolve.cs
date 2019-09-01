using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Towers
{
    public class TowersForSolve
    {
        int[,] board;
        int[,,] isNumberPossible;
        int[,] numberOfVisibleColumns;
        int size;
        bool horizontally, zeroToSize;
        public CancellationToken token {  get;  set; }
        public TowersForSolve(int size, int[] rowTop, int[] rowBottom, int[] rowLeft, int[] rowRight)
        {
            this.size = size;
            numberOfVisibleColumns = new int[size, 4];
            for (int i = 0; i < size; i++)
            {
                //0,1,2,3 - directions (for whole class)
                numberOfVisibleColumns[i,0] = rowTop[i];
                numberOfVisibleColumns[i,1] = rowRight[i];
                numberOfVisibleColumns[i,2] = rowBottom[i];
                numberOfVisibleColumns[i,3] = rowLeft[i];
            }
            board = new int[size, size];
            isNumberPossible = new int[size, size, size + 1];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    board[x, y] = 0;
                    for (int number = 0; number <= size; number++)
                    {
                        isNumberPossible[x, y,number] = 1;
                    }
                }
            }       
        }

        public int[,] GetBoard()
        {
            return board;
        }

        public bool Solve()
        {
            PreliminarySolving();
            TryToSolveSmart();
            AproximateBestBacktracking();
            return SolveBacktracking(0);
        }

        private void PreliminarySolving()
        {
            for (int i = 0; i < size; i++)
            {
                for (int dir = 0; dir <= 3; dir++)
                {
                    if (numberOfVisibleColumns[i, dir] == 1)
                    {
                        FillOneVisible(i, dir);
                    }
                    else if (numberOfVisibleColumns[i, dir] == size)
                    {
                        FillAllVisible(i, dir);
                    }
                    if (numberOfVisibleColumns[i, dir] != 0)
                    {
                        MarkImpossibleNumbers(i, dir);
                    }
                }
            }

        }

        private void FillOneVisible(int i, int dir)
        {
            if (dir == 0)
            {
                PlaceNumber(0, i, size);
            }
            else if (dir == 1)
            {
                PlaceNumber(i, size - 1, size);
            }
            else if (dir == 2)
            {
                PlaceNumber(size - 1, i, size);
            }
            else if (dir == 3)
            {
                PlaceNumber(i, 0, size);
            }
        }

        private void FillAllVisible(int i, int dir)
        {
            for (int z = 0; z < size; z++)
            {
                if (dir == 0)
                {
                    PlaceNumber(z, i, z + 1);
                }
                else if (dir == 1)
                {
                    PlaceNumber(i, size - 1 - z, z + 1);
                }
                else if (dir == 2)
                {
                    PlaceNumber(size - 1 - z, i, z + 1);
                }
                else if (dir == 3)
                {
                    PlaceNumber(i, z, z + 1);
                }
            }
        }

        private void PlaceNumber(int x, int y, int number)
        {
            board[x, y] = number;
            for (int i = 0; i < size; i++)
            {
                isNumberPossible[i, y, number] = 0;
                isNumberPossible[x, i, number] = 0;
                isNumberPossible[x, y, i + 1] = 0;
            }
        }

        private void MarkImpossibleNumbers(int i, int dir)
        {
            int number = size;
            int temp = numberOfVisibleColumns[i, dir] - 1;
            while (temp > 0)
            {
                for (int j = 0; j < temp; j++)
                {
                    if (dir == 0)
                    {
                        isNumberPossible[j, i, number] = 0;
                    }
                    else if (dir == 1)
                    {
                        isNumberPossible[i, size - 1 - j, number] = 0;
                    }
                    else if (dir == 2)
                    {
                        isNumberPossible[size - 1 - j, i, number] = 0;
                    }
                    else if (dir == 3)
                    {
                        isNumberPossible[i, j, number] = 0;
                    }
                }
                temp--;
                number--;
            }
        }

        private void TryToSolveSmart()
        {
            while (FillOnlyPossibleInRow() || FillOnlyPossibleInPlace()|| TryToEliminateImpossible())
                if (token.IsCancellationRequested)
                    return;
        }

        private bool FillOnlyPossibleInRow()
        {
            bool filledAtLeastOne = false;
            for (int i = 0; i < size; i++)
            {
                if (FillOnlyPossibleInRow(i, true))
                {
                    filledAtLeastOne = true;
                }
                if (FillOnlyPossibleInRow(i, false))
                {
                    filledAtLeastOne = true;
                }
            }
            return filledAtLeastOne;
        }

        private bool FillOnlyPossibleInRow(int i, bool vertical)
        {
            bool filledAtLeastOne = false;
            int counterPossiblePlaces;
            int possiblePlace = 0;
            if (vertical)
            {
                for (int number = 1; number <= size; number++)
                {
                    counterPossiblePlaces = 0;
                    for (int x = 0; x < size; x++)
                    {
                        if (isNumberPossible[x, i, number] == 1)
                        {
                            counterPossiblePlaces++;
                            possiblePlace = x;
                        }
                    }
                    if (counterPossiblePlaces == 1)
                    {
                        PlaceNumber(possiblePlace, i, number);
                        filledAtLeastOne = true;
                    }
                }
            }
            else
            {
                for (int number = 1; number <= size; number++)
                {
                    counterPossiblePlaces = 0;

                    for (int y = 0; y < size; y++)
                    {
                        if (isNumberPossible[i, y, number] == 1)
                        {
                            counterPossiblePlaces++;
                            possiblePlace = y;
                        }
                    }
                    if (counterPossiblePlaces == 1)
                    {
                        PlaceNumber(i, possiblePlace, number);
                        filledAtLeastOne = true;
                    }
                }
            }
            return filledAtLeastOne;
        }

        private bool FillOnlyPossibleInPlace()
        {
            bool filledAtLeastOne = false;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    if (board[x, y] == 0)
                    {
                        int number = OnlyPossibleNumber(x, y);
                        if (number != 0)
                        {
                            PlaceNumber(x, y, number);
                            filledAtLeastOne = true;
                        }
                    }

                }
            }
            return filledAtLeastOne;
        }

        private int OnlyPossibleNumber(int x, int y)
        {
            int number = 0;
            bool existPossible = false;
            for (int z = 1; z <= size; z++)
            {
                if (isNumberPossible[x, y, z] == 1)
                {
                    if (!existPossible)
                    {
                        existPossible = true;
                        number = z;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            return number;
        }

        private bool TryToEliminateImpossible()
        {
            bool foundAtLaestOne = false;
            for (int i = 0; i < size; i++)
            {
                if (numberOfVisibleColumns[i, 0] > 0 || numberOfVisibleColumns[i, 2] > 0)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (board[j, i] == 0)
                        {
                            for (int number = 1; number <= size; number++)
                            {
                                if (isNumberPossible[j, i, number] == 1)
                                {
                                    board[j, i] = number;

                                    if (!IsRowSolvable(i, true, 0))
                                    {
                                        isNumberPossible[j, i, number] = 0;
                                        foundAtLaestOne = true;
                                    }
                                }
                            }
                            board[j, i] = 0;
                        }
                    }
                }
                if (numberOfVisibleColumns[i, 1] > 0 || numberOfVisibleColumns[i, 3] > 0)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (board[i, j] == 0)
                        {
                            for (int number = 1; number <= size; number++)
                            {
                                if (isNumberPossible[i, j, number] == 1)
                                {
                                    board[i, j] = number;
                                    if (!IsRowSolvable(i, false, 0))
                                    {
                                        isNumberPossible[i, j, number] = 0;
                                        foundAtLaestOne = true;
                                    }
                                }
                            }
                            board[i, j] = 0;
                        }
                    }
                }

            }
            return foundAtLaestOne;
        }

        private bool IsRowSolvable(int i, bool vertical, int z)
        {
            if (token.IsCancellationRequested)
                return true;

            if (z >= size)
            {
                if (vertical)
                    return IsRowCorrect(i, 0) && IsRowCorrect(i, 2);
                else
                    return IsRowCorrect(i, 1) && IsRowCorrect(i, 3);
            }
            else
            {
                if (vertical)
                {
                    if (board[z, i] != 0)
                        return IsRowSolvable(i, vertical, z + 1);
                    else
                    {
                        for (int number = 1; number <= size; number++)
                        {
                            if (CanPlaceBacktracking(number, z, i) && isNumberPossible[z, i, number] == 1)
                            {
                                board[z, i] = number;
                                if (IsRowSolvable(i, vertical, z + 1))
                                {
                                    board[z, i] = 0;
                                    return true;
                                }
                            }
                        }
                        board[z, i] = 0;
                        return false;
                    }
                }
                else
                {
                    if (board[i, z] != 0)
                        return IsRowSolvable(i, vertical, z + 1);
                    else
                    {
                        for (int number = 1; number <= size; number++)
                        {
                            if (CanPlaceBacktracking(number, i, z) && isNumberPossible[i, z, number] == 1)
                            {
                                board[i, z] = number;
                                if (IsRowSolvable(i, vertical, z + 1))
                                {
                                    board[i, z] = 0;
                                    return true;
                                }
                            }
                        }
                        board[i, z] = 0;
                        return false;
                    }
                }
            }
        }

        private void AproximateBestBacktracking()
        {
            // 0 - top to down,horizontally; 1-;right to left,vertically;2 - down to top,horizontally; 3-;left to right,vertically;
            double[] dirScores = new double[4];
            int i = 0;
            List<int> directions = new List<int>();
            for (int dir = 0; dir <= 3; dir++)
            {
                directions.Add(dir);
            }
            double max;
            while (directions.Count > 1 && i<size)
            {
                dirScores[0] = numberOfVisibleColumns[i, 1] + numberOfVisibleColumns[i, 3] + (Double)Math.Abs(numberOfVisibleColumns[i, 1] - numberOfVisibleColumns[i, 3]) / 2.0;
                dirScores[1] = numberOfVisibleColumns[size - 1 - i, 2] + numberOfVisibleColumns[size - 1 - i, 0] + (Double)Math.Abs(numberOfVisibleColumns[size - 1 - i, 2] - numberOfVisibleColumns[size - 1 - i, 0]) / 2.0;
                dirScores[2] = numberOfVisibleColumns[size - 1 - i, 1] + numberOfVisibleColumns[size - 1 - i, 3] + (Double)Math.Abs(numberOfVisibleColumns[size - 1 - i, 1] - numberOfVisibleColumns[size - 1 - i, 3]) / 2.0;
                dirScores[3] = numberOfVisibleColumns[i, 0] + numberOfVisibleColumns[i, 2] + (Double)Math.Abs(numberOfVisibleColumns[i, 0] - numberOfVisibleColumns[i, 2]) / 2.0;
                for (int dir = 0; dir <= 3; dir++)
                {
                    if (!directions.Contains(dir))
                    {
                        dirScores[dir] = -1;
                    }               
                }
                max = dirScores.Max()-0.1;
                for (int dir = 0; dir <= 3; dir++)
                {
                    if (dirScores[dir] < max)
                    {
                        directions.Remove(dir);
                    }
                }
                i++;
            }
            if (directions[0] == 0)
            {
                horizontally = true;
                zeroToSize = true;
            }
            else if (directions[0] == 1)
            {
                horizontally = false;
                zeroToSize = false;
            }
            else if (directions[0] == 2)
            {
                horizontally = true;
                zeroToSize = false;
            }
            else if (directions[0] == 3)
            {
                horizontally = false;
                zeroToSize = true;
            }
        }

        private bool SolveBacktracking(int z)
        {
            if (z >= size*size)
            {
                return IsBoardCorrect();             
            }
            else
            {
                int x, y;
                if (horizontally)
                {
                    x = z / size;
                    y = z % size;
                    if (!zeroToSize)
                    {
                        x = size - 1 - x;
                    }
                }
                else
                {
                    x = z % size;
                    y = z / size;
                    if (!zeroToSize)
                    {
                        y = size - 1 - y;
                    }
                }

                if (board[x, y] != 0)
                    return SolveBacktracking(z + 1);
                else
                {
                    for (int i = 1; i <= size; i++)
                    {
                        if (CanPlaceBacktracking(i, x, y))
                        {
                            board[x, y] = i;
                            if (horizontally)
                            {
                                if (y == size-1)
                                {
                                    if (IsRowCorrect(x, 1) && IsRowCorrect(x, 3))
                                    {
                                        if (SolveBacktracking(z + 1))
                                        {
                                            return true;
                                        }else if (token.IsCancellationRequested)
                                        {
                                            board[x, y] = 0;
                                            return false;

                                        }
                                    }
                                }
                                else
                                {
                                    if (SolveBacktracking(z + 1))
                                    {
                                        return true;
                                    }
                                    else if (token.IsCancellationRequested)
                                    {
                                        board[x, y] = 0;
                                        return false;

                                    }
                                }
                            }
                            else
                            {
                                if (x == size - 1)
                                {
                                    if (IsRowCorrect(y, 0) && IsRowCorrect(y, 2))
                                    {
                                        if (SolveBacktracking(z + 1))
                                        {
                                            return true;
                                        }
                                        else if (token.IsCancellationRequested)
                                        {
                                            board[x, y] = 0;
                                            return false;

                                        }
                                    }
                                }
                                else
                                {
                                    if (SolveBacktracking(z + 1))
                                    {
                                        return true;
                                    }
                                    else if (token.IsCancellationRequested)
                                    {
                                        board[x, y] = 0;
                                        return false;

                                    }
                                }
                            }

                                                       
                        }
                    }
                    board[x, y] = 0;
                }
                return false;
            }
            
        }

        private bool CanPlaceBacktracking(int value, int x, int y)
        {
            for (int i = 0; i < size; i++)
            {
                if (board[x, i] == value)
                    return false;
            }
            for (int i = 0; i < size; i++)
            {
                if (board[i, y] == value)
                    return false;
            }
            return true;
        }

        private bool IsBoardCorrect()
        {
            for(int i = 0; i < size; i++)
            {
                for (int dir = 0; dir <=3; dir++)
                {
                    if (!IsRowCorrect(i, dir))
                        return false;
                }
            }
            return true;
        }

        private bool IsRowCorrect(int i, int dir)
        {
            if (numberOfVisibleColumns[i, dir] == 0)
                return true;

            int highestVisible = 0;
            int visibleCount = 0;

            for (int j = 0; j < size; j++)
            {
                if (dir == 0)
                {
                    if (board[j,i] > highestVisible)
                    {
                        highestVisible = board[j,i];
                        visibleCount++;
                    }
                }
                else if (dir == 1)
                {
                    if (board[i, size - 1 - j] > highestVisible)
                    {
                        highestVisible = board[i, size - 1 - j];
                        visibleCount++;
                    }
                }
                else if (dir == 2)
                {
                    if (board[size - 1 - j, i] > highestVisible)
                    {
                        highestVisible = board[size - 1 - j,i];
                        visibleCount++;
                    }
                }
                else if (dir == 3)
                {
                    if (board[i, j] > highestVisible)
                    {
                        highestVisible = board[i, j];
                        visibleCount++;
                    }
                }
            }

            if (numberOfVisibleColumns[i, dir] == visibleCount)
                return true;
            else
                return false;
        }
        
    }
}
