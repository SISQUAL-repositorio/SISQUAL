using System.Diagnostics;
Random rnd = new Random();
const int MATRIX_ROWS = 5;
const int MATRIX_COLUMNS = 5;

double[,] matrix = new double[MATRIX_ROWS, MATRIX_COLUMNS];
Process currentProcess = Process.GetCurrentProcess();
long usedMemory = currentProcess.PrivateMemorySize64;

for (int i = 0; i < MATRIX_ROWS; i++)
{
    for (int j = 0; j < MATRIX_COLUMNS; j++)
    {

        matrix[i, j] = rnd.Next(1, 100);
    }
}

for (int i = 0; i < MATRIX_ROWS; i++)
{
    for (int j = 0; j < MATRIX_COLUMNS; j++)
    {
        Console.Write(matrix[i, j] + "\t");
    }
    Console.WriteLine();
}

Console.WriteLine("Insira a posição que deseja encontrar");
Console.Write("X: ");
int posX = Convert.ToInt32(Console.ReadLine());
Console.Write("Y: ");
int posY = Convert.ToInt32(Console.ReadLine());

Console.WriteLine("Posição " + posX + "," + posY + " na matriz é "+ matrix[posX, posY]);

Console.WriteLine("Memória utilizada neste programa -> "+ usedMemory);

Console.ReadKey();