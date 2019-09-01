using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Towers
{



    public class TowersForGenerate
    {
        int[,] board;
        int[,] numberOfVisibleColumns;
        int size;
        Random rand;
        struct permutation
        {
            public int i;
            public int j;
            public bool column;
        }
        struct number
        {
            public int dir;
            public int i;
        }

        List<number> deletedNumbers;
        List<permutation> permutations;

        public TowersForGenerate(int size)
        {
            this.size = size;
            numberOfVisibleColumns = new int[size, 4];
            board = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    numberOfVisibleColumns[i, dir] = 1;
                }
            }
            rand = new Random();
            GenerateBoard();
            UpdateNumbers();
            deletedNumbers = new List<number>();
            permutations = new List<permutation>();
        }

        public int GetField(int x, int y)
        {
            return board[x, y];
        }

        public int GetNumber(int i, int dir)
        {
            return numberOfVisibleColumns[i, dir];
        }

        private void GenerateBoard()
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    board[x, y] = (x + y) % size + 1;
                }

            }
        }

        public void Permutate(bool column)
        {
            int random1, random2;
            random1 = rand.Next(0, size);
            random2 = rand.Next(0, size);
            while (size > 1 && random2 == random1)
            {
                random2 = rand.Next(0, size);
            }
            if (column)
            {
                permutations.Add(new permutation { i = random1, j = random2, column = true });
            }
            else
            {
                permutations.Add(new permutation { i = random1, j = random2, column = false });
            }
            Change(random1, random2, column);
            UpdateNumbers();
        }

        public void DeleteRandomNumber()
        {
            int random1 = rand.Next(0, size);
            int random2 = rand.Next(0, 4);
            if (NumberToDeleteExist())
            {
                while(numberOfVisibleColumns[random1, random2] == 0)
                {
                     random1 = rand.Next(0, size);
                     random2 = rand.Next(0, 4);
                }
                numberOfVisibleColumns[random1, random2] = 0;
                deletedNumbers.Add(new number { i = random1, dir = random2 });
            }
            
        }

        public void RemoveNumber(int i,int dir)
        {
            if(numberOfVisibleColumns[i,dir] != 0)
            {
                numberOfVisibleColumns[i, dir] = 0;
                deletedNumbers.Add(new number { i = i, dir = dir });
            }
            
        }

        public void RestoreNumber(int i,int dir)
        {
            if(deletedNumbers.Contains(new number { i = i, dir = dir }))
            {
                deletedNumbers.Remove(new number { i = i, dir = dir });
                numberOfVisibleColumns[i, dir] = -1;
                UpdateNumbers();
            }
            
        }


        private bool NumberToDeleteExist()
        {
            for (int i = 0; i < size; i++)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    if (numberOfVisibleColumns[i, dir] != 0 )
                        return true;
                }
            }
            return false;
        }

        public void RestorDeletedNumber()
        {
            if (deletedNumbers.Any())
            {
                number n = deletedNumbers.Last();
                numberOfVisibleColumns[n.i, n.dir] = -1;
                deletedNumbers.RemoveAt(deletedNumbers.Count - 1);
                UpdateNumbers();
            }
        }

        public void ClearListOfOperations()
        {
            permutations.Clear();
        }

        public void BackLastPermutation()
        {
            if (permutations.Any())
            {
                permutation p = permutations.Last();
                Change(p.i, p.j, p.column);
                permutations.RemoveAt(permutations.Count - 1);
                UpdateNumbers();
            }
        }

        private void UpdateNumbers()
        {
            for (int i = 0; i < size; i++)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    if(numberOfVisibleColumns[i,dir]!=0)
                        UpdateNumberOfVisible(i, dir);
                }
            }
        }

        private void UpdateNumberOfVisible(int i, int dir)
        {
            int highestVisible = 0;
            int visibleCount = 0;
            for (int j = 0; j < size; j++)
            {
                if (dir == 0)
                {
                    if (board[j, i] > highestVisible)
                    {
                        highestVisible = board[j, i];
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
                        highestVisible = board[size - 1 - j, i];
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

            numberOfVisibleColumns[i, dir] = visibleCount;

        }

        private void Change(int random1, int random2, bool column)
        {
            int temp;
            for (int i = 0; i < size; i++)
            {
                if (column)
                {
                    temp = board[i, random1];
                    board[i, random1]= board[i, random2];
                    board[i, random2] = temp;
                }
                else
                {
                    temp = board[random1,i ];
                    board[random1, i] = board[random2, i];
                    board[random2, i] = temp;
                }
            }
        }
    }
}
