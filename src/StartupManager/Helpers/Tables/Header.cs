using System;

namespace StartupManager.Helpers.Tables {
    public class Header<T> {
        public Header(string name, Func<T, object> dataSelector, Func<T, ConsoleColor> colorSelector) {
            Name = name;
            DataSelector = dataSelector;
            ColorSelector = colorSelector;
        }
        public string Name { get; set; }
        public Func<T, ConsoleColor> ColorSelector { get; }
        public Func<T, object> DataSelector { get; }
    }
}