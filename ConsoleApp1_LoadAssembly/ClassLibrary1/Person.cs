using System;

namespace ClassLibrary1
{
    public class Person
    {
        public int Id { get; set; } = 12;
        public string Name { get; set; } = "lili";

        public string GetName()
        {
            return Name;
        }
    }
}
