using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StartupManager.Helpers.Tables {
    public static class TableParser {
        public static string ToStringTable<T>(this IEnumerable<T> values, string[] columnHeaders, params Func<T, object>[] valueSelectors) {
            return ToStringTable(values.ToArray(), columnHeaders, valueSelectors);
        }

        public static string ToStringTable<T>(this T[] values, string[] columnHeaders, params Func<T, object>[] valueSelectors) {
            if (valueSelectors == null) {
                throw new ArgumentNullException(nameof(valueSelectors));
            }
            if (columnHeaders.Length != valueSelectors.Length) {
                throw new ArgumentException($"{nameof(columnHeaders)} and {nameof(valueSelectors)} must have same length");
            }

            var arrValues = new string[values.Length + 1, valueSelectors.Length];

            // Fill headers
            for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++) {
                arrValues[0, colIndex] = columnHeaders[colIndex];
            }

            // Fill table rows
            for (int rowIndex = 1; rowIndex < arrValues.GetLength(0); rowIndex++) {
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++) {
                    if (valueSelectors != null) {
                        if (valueSelectors.Length >= colIndex) {
                            var value = valueSelectors[colIndex]
                                .Invoke(values[rowIndex - 1]).ToString() ?? string.Empty;
                            arrValues[rowIndex, colIndex] = value;
                        }
                    }
                }
            }

            return ToStringTable(arrValues);
        }

        public static string ToStringTable(this string[, ] arrValues) {
            int[] maxColumnsWidth = GetMaxColumnsWidth(arrValues);
            var headerSpliter = Environment.NewLine; //new string('-', maxColumnsWidth.Sum(i => i + 3) - 1);

            var sb = new StringBuilder();
            for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++) {
                for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++) {
                    // Print cell
                    string cell = arrValues[rowIndex, colIndex];
                    cell = cell.PadRight(maxColumnsWidth[colIndex]);
                    if (colIndex != 0)
                        sb.Append(" |-_-| ");
                    sb.Append(cell);
                }

                // Print end of line
                sb.Append("|_-_|");
                sb.AppendLine();

                // Print splitter
                if (rowIndex == 0) {
                    sb.AppendFormat("{0}", headerSpliter);
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        private static int[] GetMaxColumnsWidth(string[, ] arrValues) {
            var maxColumnsWidth = new int[arrValues.GetLength(1)];
            for (int colIndex = 0; colIndex < arrValues.GetLength(1); colIndex++) {
                for (int rowIndex = 0; rowIndex < arrValues.GetLength(0); rowIndex++) {
                    int newLength = arrValues[rowIndex, colIndex].Length;
                    int oldLength = maxColumnsWidth[colIndex];

                    if (newLength > oldLength) {
                        maxColumnsWidth[colIndex] = newLength;
                    }
                }
            }

            return maxColumnsWidth;
        }
    }
}