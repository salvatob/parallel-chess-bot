namespace TestProject1;

public static class MatrixExtensions {
    public static TItem[] Flatten<TItem>(this TItem[,] matrix) {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        var result = new TItem[rows * cols];

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                result[i * rows + j] = matrix[i, j];
            }
        }

        return result;
    }
}