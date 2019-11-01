using System;
using System.Collections.Generic;
using System.Linq;

namespace StartupManager.Helpers.Tables {
    public class Table<T> {
        public readonly string RawTableString;
        public Header<T>[] Headers { get; set; }
        public T[] Models { get; set; }

        public Table(IEnumerable<T> models, Header<T>[] headers) {
            Models = models.ToArray();
            Headers = headers;
            RawTableString = models.ToStringTable(headers.Select(x => x.Name).ToArray(), headers.Select(x => x.DataSelector).ToArray());
        }

        public void OutputToConsoleColored() {
            var headerNames = Headers.Select(x => x.Name);
            var lines = RawTableString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++) {
                var line = lines[lineIndex];
                if (headerNames.All(x => line.Contains(x))) { //Header line skip
                    System.Console.WriteLine(line.Replace("|", string.Empty));
                    System.Console.WriteLine();
                    continue;
                }

                var model = Models[lineIndex - 1];
                var content = line.Split('|', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < content.Length; i++) {
                    Console.ForegroundColor = Headers[i].ColorSelector.Invoke(model);
                    System.Console.Write(content[i]);
                }
                Console.ResetColor();
                System.Console.WriteLine();
            }
        }
    }
}